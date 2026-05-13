/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

#nullable enable

using System.Text;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Node;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Storage;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils;

/// <summary>
/// Builds an SVG network topology diagram from Proxmox VE cluster data.
/// </summary>
public static class NetworkDiagramBuilder
{

    private static string JoinAsString<T>(this IEnumerable<T> source, string separator)
        => string.Join(separator, source);

    private static string BuildSdnComment(SdnVnetRow v)
    {
        var parts = new List<string> { $"SDN vnet · zone {v.Zone} ({v.ZoneType})" };
        if (v.Tag is int tag) { parts.Add($"VLAN {tag}"); }
        if (!string.IsNullOrWhiteSpace(v.ZoneBridge)) { parts.Add($"via {v.ZoneBridge}"); }
        if (!string.IsNullOrWhiteSpace(v.Alias)) { parts.Add(v.Alias); }
        return string.Join(" · ", parts);
    }

    private static string[] SplitWords(this string source)
        => string.IsNullOrEmpty(source)
            ? []
            : source.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    /// <summary>Host network interface row (one per node/interface pair).</summary>
    public record NodeNetworkRow(string Node, NodeNetwork Network);

    /// <summary>VM or CT network row (one per VM NIC).</summary>
    public record VmNetworkRow(long VmId,
                               string Name,
                               string Node,
                               string Type,
                               string Status,
                               string? Hostname,
                               VmNetwork Network,
                               bool IsInternal = false);

    /// <summary>SDN virtual network row (one per SDN vnet, repeated for each node if attached to multiple nodes).</summary>
    public record SdnVnetRow(string Vnet,
                             string Zone,
                             string ZoneType,
                             string? ZoneBridge,
                             int? Tag,
                             string? Alias,
                             IReadOnlyList<string> Nodes);

    /// <summary>Metadata shown in the diagram info panel.</summary>
    public record DiagramInfo(string ApplicationName, string ApplicationUrl, string ApplicationVersion);

    // Minimum box width; actual width is computed per-node from the longest label line.
    private const int SvgBoxW = 90;
    private const int SvgBoxH = 48;
    private const int SvgColGap = 80;
    private const int SvgRowGap = 20;
    private const int SvgMarginX = 40;
    private const int SvgMarginY = 40;
    private const int SvgNodeGap = 60;
    private const int SvgColStride = SvgBoxW + SvgColGap;

    private const string SvgColNic = "#4A90D9";
    private const string SvgColNicGw = "#E74C3C";
    private const string SvgColBond = "#7B68EE";
    private const string SvgColBridge = "#27AE60";
    private const string SvgColVm = "#ECF0F1";
    private const string SvgColFw = "#FF9100";
    private const string SvgColText = "#1A1A1A";
    private const string SvgColWhite = "#FFFFFF";
    private const string SvgColLine = "#888888";
    private const string SvgColBg = "#F7F9FC";
    private const string SvgColDown = "#95A5A6";
    private const string SvgColStorage = "#00897B";

    private record SvgNode(string Id, string Label, string Tooltip, string Fill, string TextColor, int Col, int Row);
    private record SvgEdge(string FromId, string ToId, string Label = "");
    private record SvgStorage(string Id, string Label, string Tooltip, string? FromBridgeId, bool IsDisabled);

    /// <summary>
    /// Builds the SVG network topology diagram.
    /// </summary>
    public static string BuildSvg(
        IEnumerable<NodeNetworkRow> hostNets,
        IEnumerable<SdnVnetRow> sdnVnets,
        IEnumerable<VmNetworkRow> vmNets,
        IEnumerable<StorageItem> storages,
        DiagramInfo info)
    {
        var hostNetsList = hostNets.ToList();
        var sdnList = sdnVnets.ToList();
        var vmNetsList = vmNets.ToList();
        var storagesList = storages.ToList();

        var nodeNames = hostNetsList.Select(r => r.Node).Distinct().Order().ToList();
        if (nodeNames.Count == 0) { return "<svg xmlns='http://www.w3.org/2000/svg'/>"; }

        var sections = nodeNames.ConvertAll(n => BuildNodeSection(n, hostNetsList, vmNetsList, storagesList, sdnList));

        const int legendW = 360;
        const int infoW = 360;
        const int headerGap = 16;
        const int headerH = 190;
        const int headerW = legendW + headerGap + infoW;

        int totalW = Math.Max(sections.Max(s => s.Width), headerW) + (SvgMarginX * 2);
        int totalH = SvgMarginY + headerH + SvgNodeGap;
        foreach (var s in sections) { totalH += s.Height + SvgNodeGap; }
        totalH += SvgMarginY;

        var sb = new StringBuilder();
        sb.AppendLine($"""<svg xmlns="http://www.w3.org/2000/svg" width="{totalW}" height="{totalH}" viewBox="0 0 {totalW} {totalH}" preserveAspectRatio="xMinYMin meet" font-family="Segoe UI,Arial,sans-serif">""");

        sb.Append(RenderLegend(SvgMarginX, SvgMarginY, legendW, headerH));
        sb.Append(RenderInfo(SvgMarginX + legendW + headerGap, SvgMarginY, infoW, headerH, sections.Count,
                             hostNetsList, vmNetsList, info));

        var offsetY = SvgMarginY + headerH + SvgNodeGap;
        foreach (var section in sections)
        {
            sb.Append(section.Render(SvgMarginX, offsetY));
            offsetY += section.Height + SvgNodeGap;
        }

        sb.AppendLine("</svg>");
        return sb.ToString();
    }

    private static string RenderInfo(int x, int y, int w, int h, int nodeCount,
                                     List<NodeNetworkRow> hostNets, List<VmNetworkRow> vmNets,
                                     DiagramInfo info)
    {
        var bridgeCount = hostNets.Count(r => r.Network.Type is "bridge" or "OVSBridge");
        var bondCount = hostNets.Count(r => r.Network.Type is "bond" or "OVSBond");
        var nicCount = hostNets.Count(r => r.Network.Type is "eth" or "InfiniBand");
        var vmIds = vmNets.Select(r => r.VmId).Distinct().Count();

        var gwCount = vmNets.Where(r => !string.IsNullOrEmpty(r.Network.Bridge))
                            .GroupBy(r => r.VmId)
                            .Count(g => g.Select(r => r.Network.Bridge).Distinct().Count() >= 2);

        var rows = new (string Key, string Value)[]
        {
            ("Generated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
            ("Nodes", nodeCount.ToString()),
            ("Bridges", bridgeCount.ToString()),
            ("Bonds", bondCount.ToString()),
            ("Physical NICs", nicCount.ToString()),
            ("VMs / CTs", vmIds.ToString()),
            ("Multi-homed", gwCount.ToString()),
        };

        var sb = new StringBuilder();
        sb.AppendLine($"""
            <rect x="{x}" y="{y}" width="{w}" height="{h}" rx="6" fill="#FFFFFF" stroke="#CCC" stroke-width="1"/>
            <text x="{x + 12}" y="{y + 20}" font-size="13" font-weight="bold" fill="#333">Info</text>
            """);

        var ry = y + 40;
        foreach (var (k, v) in rows)
        {
            sb.AppendLine($"""
                <text x="{x + 12}" y="{ry}" font-size="10" font-weight="bold" fill="#555">{Escape(k)}:</text>
                <text x="{x + 130}" y="{ry}" font-size="10" fill="#333">{Escape(v)}</text>
                """);
            ry += 18;
        }

        var footerY = y + h - 28;
        sb.AppendLine($"""
            <text x="{x + 12}" y="{footerY}" font-size="10" font-style="italic" fill="#888">Generated by</text>
            <a href="{Escape(info.ApplicationUrl)}" target="_blank">
              <text x="{x + 95}" y="{footerY}" font-size="10" font-style="italic" fill="#1565C0" text-decoration="underline">{Escape(info.ApplicationName)} v{Escape(info.ApplicationVersion)}</text>
            </a>
            """);

        var copyY = y + h - 10;
        sb.AppendLine($"""
            <text x="{x + 12}" y="{copyY}" font-size="10" font-style="italic" fill="#888">© Corsinvest Srl —</text>
            <a href="https://www.corsinvest.it" target="_blank">
              <text x="{x + 130}" y="{copyY}" font-size="10" font-style="italic" fill="#1565C0" text-decoration="underline">www.corsinvest.it</text>
            </a>
            """);

        return sb.ToString();
    }

    private static string RenderLegend(int x, int y, int w, int h)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"""
            <rect x="{x}" y="{y}" width="{w}" height="{h}" rx="6" fill="#FFFFFF" stroke="#CCC" stroke-width="1"/>
            <text x="{x + 12}" y="{y + 20}" font-size="13" font-weight="bold" fill="#333">Legend</text>
            """);

        var rows = new (string Color, string? Stroke, string Label)[]
        {
            (SvgColNic, null, "Physical NIC"),
            (SvgColNicGw, null, "NIC with gateway on host"),
            (SvgColBond, null, "Bond (link aggregation)"),
            (SvgColBridge, null, "Bridge (host)"),
            (SvgColFw, null, "Multi-homed VM/CT (gateway between external and internal bridges)"),
            (SvgColVm, "#888", "Normal VM / CT"),
            (SvgColStorage, null, "Network storage (NFS, CIFS, PBS, iSCSI, Ceph, RBD, ...)"),
            (SvgColDown, null, "Inactive / stopped (NIC, bond, bridge, VM, CT)"),
        };

        var ry = y + 34;
        foreach (var (color, stroke, label) in rows)
        {
            var strokeAttr = stroke != null ? $""" stroke="{stroke}" """.TrimEnd() : "";
            sb.AppendLine($"""
                <rect x="{x + 12}" y="{ry}" width="22" height="14" rx="2" fill="{color}"{strokeAttr}/>
                <text x="{x + 42}" y="{ry + 11}" font-size="10" fill="#333">{Escape(label)}</text>
                """);
            ry += 20;
        }
        return sb.ToString();
    }

    private record NodeSection(string NodeName, List<SvgNode> Nodes, List<SvgEdge> Edges, List<SvgStorage> Storages)
    {
        private const int SvgBoxLineH = 13;
        private const int SvgBoxPadY = 8;
        private const int SvgBoxPadX = 10;
        private const double SvgCharWBody = 5.3;
        private const double SvgCharWTitle = 6.4;
        private const int SvgStorageStripGapY = 40;
        private const int SvgStorageGapX = 20;

        private static int NodeHeight(SvgNode n)
            => Math.Max(SvgBoxH, (Math.Max(1, n.Label.Split('\n').Length) * SvgBoxLineH) + SvgBoxPadY);

        private static int NodeWidth(SvgNode n)
        {
            var lines = n.Label.Split('\n');
            double maxPx = 0;
            for (var i = 0; i < lines.Length; i++)
            {
                var w = lines[i].Length * (i == 0 ? SvgCharWTitle : SvgCharWBody);
                if (w > maxPx) { maxPx = w; }
            }
            return Math.Max(SvgBoxW, (int)Math.Ceiling(maxPx) + SvgBoxPadX);
        }

        private int RowHeight(int row)
            => Nodes.Where(n => n.Row == row)
                    .Select(NodeHeight)
                    .DefaultIfEmpty(SvgBoxH)
                    .Max();

        private int ColWidth(int col)
            => Nodes.Where(n => n.Col == col)
                    .Select(NodeWidth)
                    .DefaultIfEmpty(SvgBoxW)
                    .Max();

        private int RowYOffset(int row)
        {
            var y = 0;
            for (int r = 0; r < row; r++) { y += RowHeight(r) + SvgRowGap; }
            return y;
        }

        private int ColXOffset(int col)
        {
            var x = 0;
            for (int c = 0; c < col; c++) { x += ColWidth(c) + SvgColGap; }
            return x;
        }

        private static int StorageBoxH(SvgStorage s)
            => Math.Max(SvgBoxH, (Math.Max(1, s.Label.Split('\n').Length) * SvgBoxLineH) + SvgBoxPadY);

        private static int StorageBoxW(SvgStorage s)
        {
            var lines = s.Label.Split('\n');
            double maxPx = 0;
            for (var i = 0; i < lines.Length; i++)
            {
                var w = lines[i].Length * (i == 0 ? SvgCharWTitle : SvgCharWBody);
                if (w > maxPx) { maxPx = w; }
            }
            return Math.Max(SvgBoxW, (int)Math.Ceiling(maxPx) + SvgBoxPadX);
        }

        private int StorageStripHeight
            => Storages.Count == 0 ? 0 : Storages.Max(StorageBoxH) + SvgStorageStripGapY + 32;

        private int StorageStripWidth
            => Storages.Count == 0 ? 0 : Storages.Sum(StorageBoxW) + (Math.Max(0, Storages.Count - 1) * SvgStorageGapX);

        public int Width
            => Math.Max(Nodes.Count == 0
                            ? 200
                            : Enumerable.Range(0, Nodes.Max(n => n.Col) + 1).Sum(c => ColWidth(c) + SvgColGap) - SvgColGap,
                        StorageStripWidth);

        public int Height
        {
            get
            {
                var topologyH = 60;
                if (Nodes.Count > 0)
                {
                    var maxRow = Nodes.Max(n => n.Row);
                    var total = 0;
                    for (var i = 0; i <= maxRow; i++) { total += RowHeight(i) + SvgRowGap; }
                    topologyH = total + 40;
                }
                return topologyH + StorageStripHeight;
            }
        }

        private static void RenderBox(StringBuilder sb, int x, int y, int w, int h,
                                      string label, string tooltip, string fill, string textColor)
        {
            sb.AppendLine($"""
                <g>
                  <title>{Escape(tooltip)}</title>
                  <rect x="{x}" y="{y}" width="{w}" height="{h}" rx="4" fill="{fill}" stroke="#666" stroke-width="1"/>
                """);

            var lines = label.Split('\n');
            var blockH = lines.Length * SvgBoxLineH;
            var startY = y + ((h - blockH) / 2) + (SvgBoxLineH / 2);
            for (var i = 0; i < lines.Length; i++)
            {
                var ty = startY + (i * SvgBoxLineH);
                var bold = i == 0;
                var fontSize = bold ? 11 : 9;
                var fontWeight = bold ? "bold" : "normal";
                sb.AppendLine($"""  <text x="{x + (w / 2)}" y="{ty}" font-size="{fontSize}" font-weight="{fontWeight}" fill="{textColor}" text-anchor="middle" dominant-baseline="middle">{Escape(lines[i])}</text>""");
            }
            sb.AppendLine("</g>");
        }

        public string Render(int offsetX, int offsetY)
        {
            var pos = Nodes.ToDictionary(n => n.Id,
                                         n => (x: offsetX + ColXOffset(n.Col),
                                               y: offsetY + 30 + RowYOffset(n.Row),
                                               w: ColWidth(n.Col),
                                               h: NodeHeight(n)));

            var sb = new StringBuilder();

            sb.AppendLine($"""
                <rect x="{offsetX - 8}" y="{offsetY}" width="{Width + 16}" height="{Height}" rx="8" fill="{SvgColBg}" stroke="#CCC" stroke-width="1"/>
                <text x="{offsetX}" y="{offsetY + 18}" font-size="13" font-weight="bold" fill="#333">Node: {NodeName}</text>
                """);

            var edgesByTarget = Edges.Where(e => pos.ContainsKey(e.FromId) && pos.ContainsKey(e.ToId))
                                     .GroupBy(e => e.ToId)
                                     .ToDictionary(g => g.Key, g => g.OrderBy(e => pos[e.FromId].y).ToList());

            foreach (var edge in Edges)
            {
                if (!pos.TryGetValue(edge.FromId, out var from)) { continue; }
                if (!pos.TryGetValue(edge.ToId, out var to)) { continue; }

                var x1 = from.x + from.w;
                var y1 = from.y + (from.h / 2);
                var x2 = to.x;
                var y2 = to.y + (to.h / 2);

                const int finalRunX = 50;
                const int bendStrideX = 8;
                var siblings = edgesByTarget[edge.ToId];
                var idx = siblings.IndexOf(edge);
                var totalIn = siblings.Count;
                var spreadOffset = (idx - ((totalIn - 1) / 2)) * bendStrideX;
                var mx = Math.Max(x1 + 10, x2 - finalRunX + spreadOffset);
                sb.AppendLine($"""<path d="M {x1} {y1} L {mx} {y1} L {mx} {y2} L {x2} {y2}" fill="none" stroke="{SvgColLine}" stroke-width="1.5" stroke-linejoin="miter" marker-end="url(#arrow)"/>""");

                if (!string.IsNullOrEmpty(edge.Label))
                {
                    var lx = (mx + x2) / 2;
                    var ly = y2 - 7;
                    sb.AppendLine($"""<text x="{lx}" y="{ly}" font-size="9" fill="#555" text-anchor="middle">{Escape(edge.Label)}</text>""");
                }
            }

            foreach (var node in Nodes)
            {
                var (bx, by, bw, bh) = pos[node.Id];
                RenderBox(sb, bx, by, bw, bh, node.Label, node.Tooltip, node.Fill, node.TextColor);
            }

            if (Storages.Count > 0)
            {
                var topologyEndY = offsetY + Height - StorageStripHeight;
                var dividerY = topologyEndY + 8;
                sb.AppendLine($"""<line x1="{offsetX - 8}" y1="{dividerY}" x2="{offsetX + Width + 8}" y2="{dividerY}" stroke="#AAA" stroke-width="1" stroke-dasharray="5,3"/>""");

                var stripHeaderY = dividerY + 16;
                sb.AppendLine($"""<text x="{offsetX}" y="{stripHeaderY}" font-size="12" font-weight="bold" fill="#555">Storages</text>""");

                var sy = stripHeaderY + 8;
                var sx = offsetX;
                foreach (var s in Storages)
                {
                    var sw = StorageBoxW(s);
                    var sh = StorageBoxH(s);

                    if (s.FromBridgeId != null && pos.TryGetValue(s.FromBridgeId, out var br))
                    {
                        var bx = br.x + (br.w / 2);
                        var by = br.y + br.h;
                        var tx = sx + (sw / 2);
                        var ty = sy;
                        var routeY = dividerY - 6;
                        sb.AppendLine($"""<path d="M {bx} {by} L {bx} {routeY} L {tx} {routeY} L {tx} {ty}" fill="none" stroke="{SvgColLine}" stroke-width="1.5" stroke-linejoin="miter" marker-end="url(#arrow)"/>""");
                    }

                    var sFill = s.IsDisabled ? SvgColDown : SvgColStorage;
                    RenderBox(sb, sx, sy, sw, sh, s.Label, s.Tooltip, sFill, SvgColWhite);
                    sx += sw + SvgStorageGapX;
                }
            }

            var arrowDefs = $"""
                <defs>
                  <marker id="arrow" markerWidth="8" markerHeight="8" refX="8" refY="3" orient="auto">
                    <path d="M0,0 L0,6 L8,3 z" fill="{SvgColLine}"/>
                  </marker>
                </defs>

                """;
            return arrowDefs + sb;
        }
    }

    private static string Escape(string s)
        => s.Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;");

    private static string TooltipLines(params (string Key, string? Value)[] fields)
        => fields.Where(f => !string.IsNullOrWhiteSpace(f.Value))
                 .Select(f => $"{f.Key}: {f.Value}")
                 .JoinAsString("\n");

    private static string InterfaceTitle(string name, string? comment)
        => string.IsNullOrWhiteSpace(comment) ? name : $"{name} · {comment.Trim()}";

    private static string ActiveStatus(bool active) => active ? "Active" : "Inactive";
    private static string? MtuLabel(int? mtu) => mtu.HasValue ? $"MTU {mtu}" : null;
    private static string YesNo(bool value) => value ? "Yes" : "No";

    private static string BoxLabel(params string?[] lines)
        => lines.Where(l => !string.IsNullOrWhiteSpace(l))
                .SelectMany(l => l!.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                .Select(l => l.Trim())
                .Where(l => l.Length > 0)
                .JoinAsString("\n");

    private static string? LabeledValue(string key, string? value)
        => string.IsNullOrWhiteSpace(value) ? null : $"{key}: {value}";

    private static NodeSection BuildNodeSection(string nodeName,
                                                List<NodeNetworkRow> hostNets,
                                                List<VmNetworkRow> vmNets,
                                                List<StorageItem> storageConfigs,
                                                List<SdnVnetRow> sdnVnets)
    {
        var hostNetworks = hostNets.Where(r => r.Node == nodeName).Select(r => r.Network).ToList();

        var vmsInNode = vmNets.Where(v => v.Node == nodeName)
                              .GroupBy(v => v.VmId)
                              .ToDictionary(g => g.Key, g => g.ToList());

        var bridgeByName = hostNetworks.Where(n => n.Type is "bridge" or "OVSBridge").ToDictionary(n => n.Interface);
        var bondByName = hostNetworks.Where(n => n.Type is "bond" or "OVSBond").ToDictionary(n => n.Interface);
        var nicByName = hostNetworks.Where(n => n.Type is "eth" or "InfiniBand").ToDictionary(n => n.Interface);

        foreach (var item in sdnVnets.Where(v => v.Nodes.Count == 0 || v.Nodes.Contains(nodeName)))
        {
            if (bridgeByName.ContainsKey(item.Vnet)) { continue; }
            bridgeByName[item.Vnet] = new NodeNetwork
            {
                Interface = item.Vnet,
                Type = "bridge",
                Active = true,
                Comments = BuildSdnComment(item),
            };
        }

        var vmsByBridge = new Dictionary<string, HashSet<long>>();
        foreach (var (vmId, nics) in vmsInNode)
        {
            foreach (var nic in nics.Where(n => !string.IsNullOrEmpty(n.Network.Bridge)))
            {
                if (!vmsByBridge.TryGetValue(nic.Network.Bridge, out var set))
                {
                    set = [];
                    vmsByBridge[nic.Network.Bridge] = set;
                }
                set.Add(vmId);
            }
        }

        var externalBridges = bridgeByName.Values
                                .Where(br => br.BridgePorts.SplitWords()
                                               .Concat(br.OvsBonds.SplitWords())
                                               .Any(p => bondByName.ContainsKey(p) || nicByName.ContainsKey(p)))
                                .Select(br => br.Interface).ToHashSet();

        var internalBridges = bridgeByName.Keys.Where(b => !externalBridges.Contains(b)).ToHashSet();

        if (externalBridges.Count == 0) { externalBridges = [.. bridgeByName.Keys]; }

        var gatewayVmIds = vmsInNode.Where(kv =>
        {
            var bridges = kv.Value.Select(n => n.Network.Bridge)
                                  .Where(b => !string.IsNullOrEmpty(b))
                                  .ToHashSet();

            return bridges.Any(externalBridges.Contains) && bridges.Any(internalBridges.Contains);
        })
        .Select(kv => kv.Key)
        .ToHashSet();

        var nodeAttrs = new Dictionary<string, (string Label, string Tooltip, string Fill, string TextColor)>();
        var edges = new List<SvgEdge>();
        var colMap = new Dictionary<string, int>();
        var visited = new HashSet<string>();

        void AddNode(string id, string label, string tooltip, string fill, string textColor, int depth)
        {
            if (!nodeAttrs.ContainsKey(id)) { nodeAttrs[id] = (label, tooltip, fill, textColor); }
            if (!colMap.TryGetValue(id, out var cur) || depth > cur) { colMap[id] = depth; }
        }

        void AddEdge(string from, string to, string label = "")
        {
            if (!edges.Any(e => e.FromId == from && e.ToId == to))
            {
                edges.Add(new SvgEdge(from, to, label));
            }
        }

        void AddNicNode(NodeNetwork nic, int depth, bool showIpGw)
        {
            var fill = !nic.Active ? SvgColDown
                     : showIpGw && !string.IsNullOrEmpty(nic.Gateway) ? SvgColNicGw
                     : SvgColNic;

            AddNode($"nic_{nic.Interface}",
                BoxLabel(InterfaceTitle(nic.Interface, nic.Comments),
                    !nic.Active ? "DOWN" : null,
                    nic.Type != "eth" ? nic.Type : null,
                    showIpGw ? LabeledValue("IP", nic.Cidr ?? nic.Address) : null,
                    showIpGw ? LabeledValue("GW", nic.Gateway) : null,
                    MtuLabel(nic.Mtu)),
                TooltipLines(
                    ("Type", nic.Type),
                    ("IPv4", nic.Cidr ?? nic.Address),
                    ("GW", nic.Gateway),
                    ("MTU", nic.Mtu?.ToString()),
                    ("Status", ActiveStatus(nic.Active)),
                    ("Comment", nic.Comments)),
                fill, SvgColWhite, depth);
        }

        void WalkBridge(string brName, int depth)
        {
            if (!visited.Add($"br_{brName}")) { return; }

            var br = bridgeByName[brName];
            AddNode($"br_{brName}",
                    BoxLabel(InterfaceTitle(br.Interface, br.Comments),
                             !br.Active ? "DOWN" : null,
                             br.Type != "bridge" ? br.Type : null,
                             LabeledValue("IP", br.Cidr ?? br.Address),
                             LabeledValue("IP6", br.Cidr6 ?? br.Address6),
                             LabeledValue("GW", br.Gateway),
                             LabeledValue("GW6", br.Gateway6),
                             MtuLabel(br.Mtu),
                             LabeledValue("VLANs", br.BridgeVids),
                             br.BridgeVlanAware is true ? "VLAN-aware" : null,
                             LabeledValue("Ports", br.BridgePorts),
                             LabeledValue("OVS Bonds", br.OvsBonds)),
                    TooltipLines(("Type", br.Type),
                                 ("IPv4", br.Cidr ?? br.Address),
                                 ("GW", br.Gateway),
                                 ("IPv6", br.Cidr6 ?? br.Address6),
                                 ("GW6", br.Gateway6),
                                 ("MTU", br.Mtu?.ToString()),
                                 ("VLANs", br.BridgeVids),
                                 ("VLAN-aware", br.BridgeVlanAware is true ? "Yes" : null),
                                 ("Ports", br.BridgePorts),
                                 ("OVS Bonds", br.OvsBonds),
                                 ("Status", ActiveStatus(br.Active)),
                                 ("Comment", br.Comments)),
                    br.Active ? SvgColBridge : SvgColDown,
                    SvgColWhite,
                    depth);

            if (!vmsByBridge.TryGetValue(brName, out var connectedVms)) { return; }

            void RenderVm(long vmId)
            {
                var nodeId = $"vm_{vmId}";
                if (nodeAttrs.ContainsKey(nodeId)) { return; }

                var nics = vmsInNode[vmId];
                var first = nics[0];
                var vmPrefix = VmTypeLabel(first.Type);
                var bridgesUsed = nics.Select(n => n.Network.Bridge)
                                      .Where(b => !string.IsNullOrEmpty(b))
                                      .Distinct()
                                      .ToList();

                var isGateway = gatewayVmIds.Contains(vmId);

                var ips = nics.Select(n => n.Network.IpAddress)
                              .Where(ip => !string.IsNullOrEmpty(ip))
                              .Distinct()
                              .ToList();

                static string NicLabel(VmNetworkRow n)
                {
                    var id = n.Network.Id ?? "";
                    if (!string.IsNullOrEmpty(n.Network.Name) && !string.Equals(n.Network.Name, id, StringComparison.OrdinalIgnoreCase))
                    {
                        id = string.IsNullOrEmpty(id) ? n.Network.Name : $"{id} ({n.Network.Name})";
                    }
                    return $"{id} → {n.Network.Bridge}"
                         + (n.Network.Tag.HasValue ? $" VLAN {n.Network.Tag}" : "")
                         + (!string.IsNullOrEmpty(n.Network.IpAddress) ? $" IP:{n.Network.IpAddress}" : "")
                         + (!string.IsNullOrEmpty(n.Network.Gateway) ? $" GW:{n.Network.Gateway}" : "");
                }

                static bool IsRelevantNic(VmNetworkRow n)
                {
                    if (string.IsNullOrEmpty(n.Network.Bridge)) { return false; }
                    var name = (n.Network.Name ?? n.Network.Id ?? "").ToLowerInvariant();
                    if (name == "lo") { return false; }
                    return !name.StartsWith("veth")
                            && !name.StartsWith("docker")
                            && !name.StartsWith("br-")
                            && !name.StartsWith("tun")
                            && !name.StartsWith("cni");
                }

                var nicBoxLines = nics.Where(IsRelevantNic).Select(NicLabel).ToList();
                var nicTooltipLines = nics.ConvertAll(n => "  " + NicLabel(n));

                var hostname = first.Hostname;
                var hostnameOk = !string.IsNullOrWhiteSpace(hostname)
                                 && !hostname!.StartsWith("Agent ", StringComparison.OrdinalIgnoreCase)
                                 && !string.Equals(hostname, first.Name, StringComparison.OrdinalIgnoreCase);

                var isDown = !string.Equals(first.Status, "running", StringComparison.OrdinalIgnoreCase);

                var labelParts = new List<string?>
                {
                    $"{vmPrefix} {vmId} · {first.Name}",
                    isDown ? $"[{first.Status}]" : null,
                    hostnameOk ? hostname : null,
                };
                labelParts.AddRange(nicBoxLines);

                AddNode(nodeId,
                        BoxLabel([.. labelParts]),
                        TooltipLines((Key: vmPrefix, Value: $"{vmId} — {first.Name}"),
                                     ("Hostname", hostnameOk ? hostname : null),
                                     ("Status", first.Status),
                                     ("Bridges", bridgesUsed.JoinAsString(", ")),
                                     ("IPs", ips.JoinAsString(", ")))
                                     + "\n" + nicTooltipLines.JoinAsString("\n"),
                        isDown ? SvgColDown : (isGateway ? SvgColFw : SvgColVm),
                        isDown || isGateway ? SvgColWhite : SvgColText,
                        depth + 1);

                var nicOnThisBridge = nics.FirstOrDefault(n => n.Network.Bridge == brName);
                var vlan = nicOnThisBridge?.Network.Tag.HasValue is true
                            ? $"VLAN {nicOnThisBridge.Network.Tag}"
                            : "";
                AddEdge($"br_{brName}", nodeId, vlan);
            }

            var gatewaysHere = connectedVms.Where(v => gatewayVmIds.Contains(v)).Order().ToList();
            foreach (var vmId in gatewaysHere) { RenderVm(vmId); }

            var myCol = colMap[$"br_{brName}"];
            foreach (var vmId in gatewaysHere)
            {
                var nics = vmsInNode[vmId];
                var bridgesUsed = nics.Select(n => n.Network.Bridge).Where(b => !string.IsNullOrEmpty(b)).Distinct();
                foreach (var innerBr in bridgesUsed.Where(b => b != brName && bridgeByName.ContainsKey(b)))
                {
                    if (colMap.TryGetValue($"br_{innerBr}", out var targetCol) && targetCol <= myCol) { continue; }
                    AddEdge($"vm_{vmId}", $"br_{innerBr}");
                    WalkBridge(innerBr, depth + 2);
                }
            }

            foreach (var vmId in connectedVms.Where(v => !gatewayVmIds.Contains(v)).Order()) { RenderVm(vmId); }
        }

        void AddPhysical(string brName, int brDepth)
        {
            if (!bridgeByName.TryGetValue(brName, out var br)) { return; }

            var ports = br.BridgePorts.SplitWords()
                          .Concat(br.OvsBonds.SplitWords())
                          .Distinct()
                          .ToList();

            foreach (var port in ports)
            {
                if (bondByName.TryGetValue(port, out var bond))
                {
                    var bondId = $"bond_{port}";
                    AddNode(bondId,
                            BoxLabel(InterfaceTitle(bond.Interface, bond.Comments),
                                     !bond.Active ? "DOWN" : null,
                                     bond.Type != "bond" ? bond.Type : null,
                                     !string.IsNullOrEmpty(bond.BondMode) ? bond.BondMode : null,
                                     LabeledValue("Policy", bond.BondXmitHashPolicy),
                                     LabeledValue("Miimon", bond.BondMiimon),
                                     !string.IsNullOrEmpty(bond.Slaves) ? $"← {bond.Slaves}" : null,
                                     MtuLabel(bond.Mtu)),
                            TooltipLines(("Type", bond.Type),
                                         ("Mode", bond.BondMode),
                                         ("Policy", bond.BondXmitHashPolicy),
                                         ("Slaves", bond.Slaves),
                                         ("Miimon", bond.BondMiimon),
                                         ("MTU", bond.Mtu?.ToString()),
                                         ("Status", ActiveStatus(bond.Active)),
                                         ("Comment", bond.Comments)),
                            bond.Active ? SvgColBond : SvgColDown,
                            SvgColWhite,
                            brDepth - 1);

                    AddEdge(bondId, $"br_{brName}");

                    foreach (var slave in bond.Slaves.SplitWords())
                    {
                        if (!nicByName.TryGetValue(slave, out var nic)) { continue; }
                        AddNicNode(nic, brDepth - 2, showIpGw: false);
                        AddEdge($"nic_{slave}", bondId);
                    }
                }
                else if (nicByName.TryGetValue(port, out var nic))
                {
                    AddNicNode(nic, brDepth - 1, showIpGw: false);
                    AddEdge($"nic_{port}", $"br_{brName}");
                }
            }
        }

        void AddStandaloneNics()
        {
            var bondSlaves = bondByName.Values
                .SelectMany(b => b.Slaves.SplitWords())
                .ToHashSet();

            foreach (var nic in nicByName.Values.Where(n =>
                !bondSlaves.Contains(n.Interface) &&
                (!string.IsNullOrEmpty(n.Cidr) || !string.IsNullOrEmpty(n.Address))))
            {
                AddNicNode(nic, depth: 0, showIpGw: true);
            }
        }

        static bool IsNetworkStorageType(string? type) => (type ?? "").ToLowerInvariant() switch
        {
            "nfs" or "cifs" or "pbs" or "iscsi" or "iscsidirect"
            or "rbd" or "cephfs" or "glusterfs" or "zfs" or "esxi" => true,
            _ => false,
        };

        string? FindBridgeForServer(string? server)
        {
            if (string.IsNullOrWhiteSpace(server)) { return null; }
            if (!System.Net.IPAddress.TryParse(server.Trim(), out var ip)) { return null; }
            var ipBytes = ip.GetAddressBytes();

            foreach (var br in bridgeByName.Values)
            {
                var cidr = br.Cidr ?? br.Address;
                if (string.IsNullOrWhiteSpace(cidr)) { continue; }
                var slash = cidr.IndexOf('/');
                if (slash < 0) { continue; }
                if (!System.Net.IPAddress.TryParse(cidr[..slash], out var netIp)) { continue; }
                if (!int.TryParse(cidr[(slash + 1)..], out var prefix)) { continue; }
                if (netIp.GetAddressBytes().Length != ipBytes.Length) { continue; }

                var netBytes = netIp.GetAddressBytes();
                var ok = true;
                for (int i = 0; i < ipBytes.Length && ok; i++)
                {
                    int bitsLeft = prefix - (i * 8);
                    if (bitsLeft >= 8) { ok = ipBytes[i] == netBytes[i]; }
                    else if (bitsLeft > 0)
                    {
                        int mask = 0xFF << (8 - bitsLeft) & 0xFF;
                        ok = (ipBytes[i] & mask) == (netBytes[i] & mask);
                    }
                }
                if (ok) { return br.Interface; }
            }
            return null;
        }

        var storages = new List<SvgStorage>();

        void CollectStorages()
        {
            foreach (var st in storageConfigs)
            {
                if (!IsNetworkStorageType(st.Type)) { continue; }

                if (!string.IsNullOrWhiteSpace(st.Nodes))
                {
                    var allowed = st.Nodes.Split([',', ';', ' '], StringSplitOptions.RemoveEmptyEntries);
                    if (!allowed.Any(n => string.Equals(n.Trim(), nodeName, StringComparison.OrdinalIgnoreCase))) { continue; }
                }

                var serverHost = !string.IsNullOrEmpty(st.Server)
                                    ? st.Server
                                    : !string.IsNullOrEmpty(st.Monhost)
                                        ? st.Monhost
                                        : null;

                var bridgeFor = FindBridgeForServer(serverHost);
                var target = !string.IsNullOrEmpty(st.Export)
                                ? st.Export
                                : !string.IsNullOrEmpty(st.Datastore)
                                    ? st.Datastore
                                    : !string.IsNullOrEmpty(st.Pool)
                                        ? st.Pool
                                        : !string.IsNullOrEmpty(st.Path)
                                            ? st.Path
                                            : null;

                storages.Add(new($"storage_{st.Storage}",
                                  BoxLabel(InterfaceTitle(st.Storage, st.Type),
                                           st.Disable ? "[disabled]" : null,
                                           st.Shared ? "Shared" : null,
                                           LabeledValue("Server", serverHost),
                                           LabeledValue("Target", target),
                                           LabeledValue("Content", st.Content)),
                                  TooltipLines(("Storage", st.Storage),
                                               ("Type", st.Type),
                                               ("Status", st.Disable ? "Disabled" : "Enabled"),
                                               ("Shared", YesNo(st.Shared)),
                                               ("Server", st.Server),
                                               ("Monhost", st.Monhost),
                                               ("Export", st.Export),
                                               ("Datastore", st.Datastore),
                                               ("Pool", st.Pool),
                                               ("Path", st.Path),
                                               ("Mountpoint", st.Mountpoint),
                                               ("Content", st.Content),
                                               ("Nodes", st.Nodes)),
                                  bridgeFor != null ? $"br_{bridgeFor}" : null,
                                  st.Disable));
            }
        }

        foreach (var brName in externalBridges.Order())
        {
            WalkBridge(brName, 2);
            AddPhysical(brName, 2);
        }

        foreach (var brName in bridgeByName.Keys.Where(b => !visited.Contains($"br_{b}")).Order())
        {
            WalkBridge(brName, 2);
            AddPhysical(brName, 2);
        }

        AddStandaloneNics();
        CollectStorages();

        var rowPerCol = new Dictionary<int, int>();
        var rowMap = new Dictionary<string, int>();
        foreach (var id in nodeAttrs.Keys.OrderBy(id => colMap.GetValueOrDefault(id)).ThenBy(id => id))
        {
            var c = colMap.GetValueOrDefault(id);
            if (!rowPerCol.TryGetValue(c, out var r)) { r = 0; }
            rowMap[id] = r;
            rowPerCol[c] = r + 1;
        }

        return new NodeSection(nodeName,
                               [.. nodeAttrs.Select(a => new SvgNode(a.Key,
                                                      a.Value.Label,
                                                      a.Value.Tooltip,
                                                      a.Value.Fill,
                                                      a.Value.TextColor,
                                                      colMap.GetValueOrDefault(a.Key),
                                                      rowMap[a.Key]))],
                               edges,
                               storages);
    }

    private static string VmTypeLabel(string type)
        => type?.ToLowerInvariant() switch
        {
            "lxc" => "CT",
            _ => "VM",
        };
}
