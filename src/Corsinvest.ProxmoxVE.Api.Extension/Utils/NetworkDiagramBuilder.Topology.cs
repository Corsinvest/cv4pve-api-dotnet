/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

#nullable enable

using Corsinvest.ProxmoxVE.Api.Shared.Models.Node;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Storage;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils;

public static partial class NetworkDiagramBuilder
{
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

        // SDN vnets aren't reported by /nodes/{node}/network, but VMs attach to them
        // by name. Inject them as synthetic bridges so VM NICs targeting an SDN vnet
        // still link to a box on this node. An empty Nodes list means cluster-wide.
        // Skip when a real bridge with the same name already exists.
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

        // A bridge is "external" if at least one of its ports is a physical NIC or a bond
        // (i.e. it can reach the LAN). Bridges without such ports are "internal" — typical
        // for VM-only networks used as private segments behind a gateway VM.
        var externalBridges = bridgeByName.Values
                                .Where(br => br.BridgePorts.SplitWords()
                                               .Concat(br.OvsBonds.SplitWords())
                                               .Any(p => bondByName.ContainsKey(p) || nicByName.ContainsKey(p)))
                                .Select(br => br.Interface).ToHashSet();

        var internalBridges = bridgeByName.Keys.Where(b => !externalBridges.Contains(b)).ToHashSet();

        // Fallback: when no bridge has physical uplinks (e.g. lab/SDN-only setups),
        // treat all bridges as external so the diagram still has anchor points.
        if (externalBridges.Count == 0) { externalBridges = [.. bridgeByName.Keys]; }

        // A VM is treated as a gateway (firewall/router) when it attaches to both an
        // external and an internal bridge — i.e. it bridges traffic between segments.
        // Rendered in orange (SvgColFw) to make multi-homed VMs visually obvious.
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

        // Add (or keep) a node; column = max depth seen, so a box reached through
        // multiple paths ends up at its furthest position (avoids back-edges in layout).
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
            var fill = !nic.Active
                        ? SvgColDown
                        : showIpGw && !string.IsNullOrEmpty(nic.Gateway)
                            ? SvgColNicGw
                            : SvgColNic;

            AddNode($"nic_{nic.Interface}",
                    BoxLabel(InterfaceTitle(nic.Interface, nic.Comments),
                    !nic.Active ? "DOWN" : null,
                    nic.Type != "eth" ? nic.Type : null,
                    showIpGw ? LabeledValue("IP", nic.Cidr ?? nic.Address) : null,
                    showIpGw ? LabeledValue("GW", nic.Gateway) : null,
                    MtuLabel(nic.Mtu)),
                    TooltipLines(("Type", nic.Type),
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
                var vmPrefix = first.Type?.ToLowerInvariant() switch
                {
                    "lxc" => "CT",
                    _ => "VM",
                };

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

                // Filter out interfaces that aren't meaningful in a topology view:
                // loopback, container runtime veths/bridges (docker/cni) and tunnels —
                // they only clutter the box without adding routing information.
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

                // Show hostname only when it adds information: not empty, not echoing
                // the VM name, and not a guest-agent placeholder ("Agent not running").
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

            // Render gateways first, then walk into their internal bridges. The
            // `targetCol <= myCol` guard avoids drawing a back-edge into a bridge
            // that already sits at or before the current column (would loop the
            // layout). Non-gateway VMs are rendered last so their boxes don't
            // shuffle the layout of the chain above.
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

            foreach (var nic in nicByName.Values.Where(n => !bondSlaves.Contains(n.Interface)
                                                            && (!string.IsNullOrEmpty(n.Cidr)
                                                                || !string.IsNullOrEmpty(n.Address))))
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

        // Best-effort match of a storage server IP to the bridge it would reach on
        // this node. Walks every bridge's CIDR and checks if the server IP falls in
        // the same subnet by comparing the high `prefix` bits byte-by-byte.
        // Returns null when the server is a hostname (not an IP) or no bridge matches.
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
                // IPv4 vs IPv6 mismatch: byte arrays differ in length, can't compare.
                if (netIp.GetAddressBytes().Length != ipBytes.Length) { continue; }

                var netBytes = netIp.GetAddressBytes();
                var ok = true;
                for (var i = 0; i < ipBytes.Length && ok; i++)
                {
                    // Within one byte: 8 full bits left → compare whole byte;
                    // partial (<8) → mask off the don't-care low bits; 0 → done.
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

                // Proxmox storages can be restricted to a subset of nodes (`nodes`
                // field, comma/semicolon/space separated). Empty means cluster-wide.
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

        // Walk external bridges first (they have physical uplinks → they sit at the
        // top of the chain, depth=2). Internal bridges reached via gateway VMs get
        // picked up recursively by WalkBridge. The second pass catches any bridge
        // not reached from an external (e.g. isolated internal networks with no
        // gateway VM in the dataset).
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

        // Assign row index per column: nodes are sorted by column then by id so the
        // order is stable across renders. `rowPerCol` tracks the next free row in
        // each column as we iterate.
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
}
