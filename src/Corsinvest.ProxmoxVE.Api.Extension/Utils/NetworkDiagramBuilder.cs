/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

#nullable enable

using System.Text;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Node;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Storage;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils;

/// <summary>
/// Builds an SVG network topology diagram from Proxmox VE cluster data.
/// </summary>
public static partial class NetworkDiagramBuilder
{
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

    // Semantic color palette
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

    // Gray hierarchy for text
    private const string SvgColTextHeader = "#333";
    private const string SvgColTextSecondary = "#555";
    private const string SvgColTextMuted = "#888";

    // Stroke / border palette
    private const string SvgColBorder = "#CCC";
    private const string SvgColBorderStrong = "#666";
    private const string SvgColBorderDashed = "#AAA";
    private const string SvgColBorderLight = "#888";

    // Brand link color
    private const string SvgColLink = "#1565C0";

    /// <summary>
    /// Builds the SVG network topology diagram.
    /// </summary>
    public static string BuildSvg(IEnumerable<NodeNetworkRow> hostNets,
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
            <rect x="{x}" y="{y}" width="{w}" height="{h}" rx="6" fill="{SvgColWhite}" stroke="{SvgColBorder}" stroke-width="1"/>
            <text x="{x + 12}" y="{y + 20}" font-size="13" font-weight="bold" fill="{SvgColTextHeader}">Info</text>
            """);

        var ry = y + 40;
        foreach (var (k, v) in rows)
        {
            sb.AppendLine($"""
                <text x="{x + 12}" y="{ry}" font-size="10" font-weight="bold" fill="{SvgColTextSecondary}">{Escape(k)}:</text>
                <text x="{x + 130}" y="{ry}" font-size="10" fill="{SvgColTextHeader}">{Escape(v)}</text>
                """);
            ry += 18;
        }

        var footerY = y + h - 28;
        sb.AppendLine($"""
            <text x="{x + 12}" y="{footerY}" font-size="10" font-style="italic" fill="{SvgColTextMuted}">Generated by</text>
            <a href="{Escape(info.ApplicationUrl)}" target="_blank">
              <text x="{x + 95}" y="{footerY}" font-size="10" font-style="italic" fill="{SvgColLink}" text-decoration="underline">{Escape(info.ApplicationName)} v{Escape(info.ApplicationVersion)}</text>
            </a>
            """);

        var copyY = y + h - 10;
        sb.AppendLine($"""
            <text x="{x + 12}" y="{copyY}" font-size="10" font-style="italic" fill="{SvgColTextMuted}">© Corsinvest Srl —</text>
            <a href="https://www.corsinvest.it" target="_blank">
              <text x="{x + 130}" y="{copyY}" font-size="10" font-style="italic" fill="{SvgColLink}" text-decoration="underline">www.corsinvest.it</text>
            </a>
            """);

        return sb.ToString();
    }

    private static string RenderLegend(int x, int y, int w, int h)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"""
            <rect x="{x}" y="{y}" width="{w}" height="{h}" rx="6" fill="{SvgColWhite}" stroke="{SvgColBorder}" stroke-width="1"/>
            <text x="{x + 12}" y="{y + 20}" font-size="13" font-weight="bold" fill="{SvgColTextHeader}">Legend</text>
            """);

        var rows = new (string Color, string? Stroke, string Label)[]
        {
            (SvgColNic, null, "Physical NIC"),
            (SvgColNicGw, null, "NIC with gateway on host"),
            (SvgColBond, null, "Bond (link aggregation)"),
            (SvgColBridge, null, "Bridge (host)"),
            (SvgColFw, null, "Multi-homed VM/CT (gateway between external and internal bridges)"),
            (SvgColVm, SvgColBorderLight, "Normal VM / CT"),
            (SvgColStorage, null, "Network storage (NFS, CIFS, PBS, iSCSI, Ceph, RBD, ...)"),
            (SvgColDown, null, "Inactive / stopped (NIC, bond, bridge, VM, CT)"),
        };

        var ry = y + 34;
        foreach (var (color, stroke, label) in rows)
        {
            var strokeAttr = stroke != null ? $""" stroke="{stroke}" """.TrimEnd() : "";
            sb.AppendLine($"""
                <rect x="{x + 12}" y="{ry}" width="22" height="14" rx="2" fill="{color}"{strokeAttr}/>
                <text x="{x + 42}" y="{ry + 11}" font-size="10" fill="{SvgColTextHeader}">{Escape(label)}</text>
                """);
            ry += 20;
        }
        return sb.ToString();
    }
}
