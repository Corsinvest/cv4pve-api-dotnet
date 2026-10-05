/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Text.RegularExpressions;
using System.Xml.Linq;
using Corsinvest.ProxmoxVE.Api.Extension.Utils;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Node;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Storage;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Xunit;
using static Corsinvest.ProxmoxVE.Api.Extension.Utils.NetworkDiagramBuilder;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Utils;

/// <summary>
/// The diagram is read back as XML: every box is a group with a tooltip, a rectangle and its lines of text.
/// </summary>
public class NetworkDiagramBuilderTests
{
    private const string Blue = "#4A90D9";
    private const string Red = "#E74C3C";
    private const string Violet = "#7B68EE";
    private const string Green = "#27AE60";
    private const string Light = "#ECF0F1";
    private const string Orange = "#FF9100";
    private const string Gray = "#95A5A6";
    private const string Teal = "#00897B";

    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";
    private static readonly DiagramInfo Info = new("cv4pve-diag", "https://example.com/cv4pve-diag", "1.2.3");

    private sealed record Box(string Title, string[] Lines, string Fill, int X, int Y, int Width, string Tooltip);

    private sealed class Diagram(string svg)
    {
        public string Text { get; } = svg;
        public XDocument Xml { get; } = XDocument.Parse(svg);

        public List<Box> Boxes => [.. Xml.Root!.Elements(Svg + "g").Select(g =>
        {
            var rect = g.Element(Svg + "rect")!;
            var lines = g.Elements(Svg + "text").Select(a => a.Value).ToArray();
            return new Box(lines[0],
                           lines,
                           rect.Attribute("fill")!.Value,
                           (int)rect.Attribute("x")!,
                           (int)rect.Attribute("y")!,
                           (int)rect.Attribute("width")!,
                           g.Element(Svg + "title")!.Value);
        })];

        // the box whose title is the name, alone or followed by a comment
        public Box Box(string name) => Boxes.Single(a => a.Title == name || a.Title.StartsWith(name + " · "));
        public bool HasBox(string name) => Boxes.Any(a => a.Title == name || a.Title.StartsWith(name + " · "));
        public int Lines => Xml.Root!.Elements(Svg + "path").Count();
        public List<string> Texts => [.. Xml.Root!.Elements(Svg + "text").Select(a => a.Value)];

        public string InfoValue(string key)
            => Xml.Root!.Elements(Svg + "text").First(a => a.Value == key + ":").ElementsAfterSelf(Svg + "text").First().Value;
    }

    private static NodeNetworkRow Net(string node, string name, string type, Action<NodeNetwork>? set = null)
    {
        var network = new NodeNetwork { Interface = name, Type = type, Active = true };
        set?.Invoke(network);
        return new(node, network);
    }

    private static VmNetworkRow Nic(long vmId,
                                    string name,
                                    string bridge,
                                    string id = "net0",
                                    string node = "pve01",
                                    string type = "qemu",
                                    string status = "running",
                                    string? hostname = null,
                                    Action<VmNetwork>? set = null)
    {
        var network = new VmNetwork { Id = id, Bridge = bridge };
        set?.Invoke(network);
        return new(vmId, name, node, type, status, hostname, network);
    }

    private static Diagram Build(IEnumerable<NodeNetworkRow> hostNets,
                                 IEnumerable<VmNetworkRow>? vmNets = null,
                                 IEnumerable<StorageItem>? storages = null,
                                 IEnumerable<SdnVnetRow>? sdnVnets = null)
        => new(BuildSvg(hostNets, sdnVnets ?? [], vmNets ?? [], storages ?? [], Info));

    [Fact]
    public void Without_nodes_the_diagram_is_empty()
    {
        var svg = BuildSvg([], [], [Nic(100, "web01", "vmbr0")], [new StorageItem { Storage = "nfs1", Type = "nfs" }], Info);

        Assert.Equal("<svg xmlns='http://www.w3.org/2000/svg'/>", svg);
        Assert.Empty(XDocument.Parse(svg).Root!.Elements());
    }

    [Fact]
    public void Cards_bond_bridge_and_guest_are_drawn_from_left_to_right()
    {
        var diagram = Build(
            [
                Net("pve01", "eth0", "eth"),
                Net("pve01", "eth1", "eth"),
                Net("pve01", "bond0", "bond", a => { a.Slaves = "eth0 eth1"; a.BondMode = "802.3ad"; a.BondXmitHashPolicy = "layer2+3"; }),
                Net("pve01", "vmbr0", "bridge", a => { a.BridgePorts = "bond0"; a.Cidr = "192.168.1.10/24"; a.Gateway = "192.168.1.1"; a.BridgeVlanAware = true; a.BridgeVids = "2-4094"; }),
            ],
            [Nic(100, "web01", "vmbr0", set: a => a.Tag = 10)]);

        var eth0 = diagram.Box("eth0");
        var bond = diagram.Box("bond0");
        var bridge = diagram.Box("vmbr0");
        var vm = diagram.Box("VM 100");

        Assert.Equal(["eth0"], eth0.Lines);
        Assert.Equal(Blue, eth0.Fill);
        Assert.Equal(["bond0", "802.3ad", "Policy: layer2+3", "← eth0 eth1"], bond.Lines);
        Assert.Equal(Violet, bond.Fill);
        Assert.Equal(["vmbr0", "IP: 192.168.1.10/24", "GW: 192.168.1.1", "VLANs: 2-4094", "VLAN-aware", "Ports: bond0"], bridge.Lines);
        Assert.Equal(Green, bridge.Fill);
        Assert.Equal(["VM 100 · web01", "net0 → vmbr0 VLAN 10"], vm.Lines);
        Assert.Equal(Light, vm.Fill);

        Assert.True(eth0.X < bond.X && bond.X < bridge.X && bridge.X < vm.X);
        Assert.Equal(eth0.X, diagram.Box("eth1").X);
        Assert.True(eth0.Y < diagram.Box("eth1").Y);

        // eth0 and eth1 to the bond, the bond to the bridge, the bridge to the guest
        Assert.Equal(4, diagram.Lines);
        Assert.Contains("VLAN 10", diagram.Texts);
        Assert.Contains("Node: pve01", diagram.Texts);

        Assert.Contains("Type: bridge", bridge.Tooltip);
        Assert.Contains("VLAN-aware: Yes", bridge.Tooltip);
        Assert.Contains("Status: Active", bridge.Tooltip);
        Assert.Contains("Mode: 802.3ad", bond.Tooltip);
        Assert.Contains("VM: 100 (web01)", vm.Tooltip);
        Assert.Contains("Bridges: vmbr0", vm.Tooltip);
    }

    [Fact]
    public void Info_panel_counts_what_is_drawn()
    {
        var diagram = Build(
            [
                Net("pve01", "eth0", "eth"),
                Net("pve01", "eth1", "eth"),
                Net("pve01", "bond0", "bond", a => a.Slaves = "eth0 eth1"),
                Net("pve01", "vmbr0", "bridge", a => a.BridgePorts = "bond0"),
                Net("pve01", "vmbr1", "bridge"),
                Net("pve02", "eth0", "eth"),
                Net("pve02", "vmbr0", "OVSBridge", a => a.OvsPorts = "eth0"),
            ],
            [
                Nic(100, "fw", "vmbr0"),
                Nic(100, "fw", "vmbr1", "net1"),
                Nic(101, "web01", "vmbr1"),
                Nic(200, "ct01", "vmbr0", node: "pve02", type: "lxc"),
            ]);

        Assert.Equal("2", diagram.InfoValue("Nodes"));
        Assert.Equal("3", diagram.InfoValue("Bridges"));
        Assert.Equal("1", diagram.InfoValue("Bonds"));
        Assert.Equal("3", diagram.InfoValue("Physical NICs"));
        Assert.Equal("3", diagram.InfoValue("VMs / CTs"));
        Assert.Equal("1", diagram.InfoValue("Multi-homed"));
        Assert.Matches(@"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$", diagram.InfoValue("Generated"));

        var link = diagram.Xml.Root!.Elements(Svg + "a").First();
        Assert.Equal("https://example.com/cv4pve-diag", link.Attribute("href")!.Value);
        Assert.Equal("cv4pve-diag v1.2.3", link.Value.Trim());
        Assert.Contains("Legend", diagram.Texts);
        Assert.Contains("Physical NIC", diagram.Texts);
    }

    [Fact]
    public void Size_of_the_picture_holds_every_box()
    {
        var diagram = Build(
            [Net("pve01", "eth0", "eth"), Net("pve01", "vmbr0", "bridge", a => a.BridgePorts = "eth0")],
            [Nic(100, "web01", "vmbr0"), Nic(101, "web02", "vmbr0"), Nic(102, "web03", "vmbr0")]);

        var root = diagram.Xml.Root!;
        var width = (int)root.Attribute("width")!;
        var height = (int)root.Attribute("height")!;

        Assert.Equal($"0 0 {width} {height}", root.Attribute("viewBox")!.Value);
        Assert.All(diagram.Boxes, a => Assert.True(a.X >= 0 && a.X + a.Width <= width && a.Y >= 0 && a.Y <= height));
        // boxes of the same column do not overlap: one row each
        Assert.Equal(3, diagram.Boxes.Where(a => a.Title.StartsWith("VM ")).Select(a => a.Y).Distinct().Count());
    }

    [Fact]
    public void Guest_between_an_external_and_an_internal_bridge_is_a_gateway()
    {
        var diagram = Build(
            [
                Net("pve01", "eth0", "eth"),
                Net("pve01", "vmbr0", "bridge", a => a.BridgePorts = "eth0"),
                Net("pve01", "vmbr1", "bridge"),
            ],
            [
                Nic(100, "fw", "vmbr0"),
                Nic(100, "fw", "vmbr1", "net1"),
                Nic(101, "web01", "vmbr1"),
                Nic(102, "ct01", "vmbr1", type: "lxc", status: "stopped"),
            ]);

        var gateway = diagram.Box("VM 100");
        Assert.Equal(Orange, gateway.Fill);
        Assert.Equal(["VM 100 · fw", "net0 → vmbr0", "net1 → vmbr1"], gateway.Lines);
        Assert.Equal(Light, diagram.Box("VM 101").Fill);

        var container = diagram.Box("CT 102");
        Assert.Equal(Gray, container.Fill);
        Assert.Equal(["CT 102 · ct01", "[stopped]", "net0 → vmbr1"], container.Lines);
        Assert.Contains("CT: 102 (ct01)", container.Tooltip);

        // the internal bridge is after the gateway, its guests after it
        Assert.True(diagram.Box("eth0").X < diagram.Box("vmbr0").X);
        Assert.True(diagram.Box("vmbr0").X < gateway.X);
        Assert.True(gateway.X < diagram.Box("vmbr1").X);
        Assert.True(diagram.Box("vmbr1").X < diagram.Box("VM 101").X);
        Assert.Equal(diagram.Box("VM 101").X, container.X);

        // eth0 to vmbr0, vmbr0 to the gateway, the gateway to vmbr1, vmbr1 to its two guests
        Assert.Equal(5, diagram.Lines);
    }

    [Fact]
    public void Without_physical_ports_no_guest_is_a_gateway()
    {
        var diagram = Build(
            [Net("pve01", "vmbr0", "bridge"), Net("pve01", "vmbr1", "bridge")],
            [Nic(100, "router", "vmbr0"), Nic(100, "router", "vmbr1", "net1", set: a => a.Trunks = "10;20"), Nic(101, "web01", "vmbr0")]);

        Assert.Equal(Light, diagram.Box("VM 100").Fill);
        Assert.Equal(Light, diagram.Box("VM 101").Fill);
        Assert.Equal(diagram.Box("vmbr0").X, diagram.Box("vmbr1").X);
        // one line from each bridge to the guest of both, one to the other guest
        Assert.Equal(3, diagram.Lines);
        Assert.Contains("trunks 10;20", diagram.Texts);
        Assert.Contains("net1 → vmbr1 trunks 10;20", diagram.Box("VM 100").Lines);
    }

    [Fact]
    public void Guest_box_shows_the_cards_that_matter()
    {
        var diagram = Build(
            [Net("pve01", "vmbr0", "bridge")],
            [
                Nic(100, "web01", "vmbr0", "net10", hostname: "web.example.com"),
                Nic(100, "web01", "vmbr0", "net2", hostname: "web.example.com", set: a => { a.Name = "eth0"; a.IpAddress = "10.0.0.5"; a.Gateway = "10.0.0.1"; }),
                Nic(100, "web01", "vmbr0", "", hostname: "web.example.com", set: a => a.Name = "docker0"),
                Nic(100, "web01", "", "", hostname: "web.example.com", set: a => { a.Name = "lo"; a.IpAddress = "127.0.0.1"; }),
                Nic(101, "db01", "vmbr0", hostname: "Agent not running"),
                Nic(102, "app01", "vmbr0", hostname: "APP01"),
            ]);

        var web = diagram.Box("VM 100");
        Assert.Equal(["VM 100 · web01", "web.example.com", "net2 (eth0) → vmbr0 IP:10.0.0.5 GW:10.0.0.1", "net10 → vmbr0"], web.Lines);
        Assert.Contains("Hostname: web.example.com", web.Tooltip);
        Assert.Contains("IPs: 10.0.0.5, 127.0.0.1", web.Tooltip);
        Assert.Contains("docker0 → vmbr0", web.Tooltip);
        Assert.Contains("lo →  IP:127.0.0.1", web.Tooltip);

        Assert.Equal(["VM 101 · db01", "net0 → vmbr0"], diagram.Box("VM 101").Lines);
        Assert.Equal(["VM 102 · app01", "net0 → vmbr0"], diagram.Box("VM 102").Lines);
    }

    [Fact]
    public void Guests_of_another_node_are_not_drawn()
    {
        var diagram = Build(
            [Net("pve01", "vmbr0", "bridge"), Net("pve02", "vmbr0", "bridge")],
            [Nic(100, "web01", "vmbr0"), Nic(200, "web02", "vmbr0", node: "pve02"), Nic(300, "lost", "vmbr0", node: "pve03")]);

        Assert.True(diagram.HasBox("VM 100"));
        Assert.True(diagram.HasBox("VM 200"));
        Assert.False(diagram.HasBox("VM 300"));
        Assert.Equal(2, diagram.Boxes.Count(a => a.Title == "vmbr0"));
        Assert.True(diagram.Box("VM 100").Y < diagram.Box("VM 200").Y);
    }

    [Fact]
    public void Nodes_are_drawn_in_natural_order()
    {
        var diagram = Build([Net("pve10", "vmbr0", "bridge"), Net("pve2", "vmbr0", "bridge"), Net("pve1", "vmbr0", "bridge")]);

        Assert.Equal(["Node: pve1", "Node: pve2", "Node: pve10"], diagram.Texts.Where(a => a.StartsWith("Node: ")));
    }

    [Fact]
    public void State_address_and_comment_of_a_card_are_shown()
    {
        var diagram = Build(
            [
                Net("pve01", "eth0", "eth", a => { a.Comments = " uplink \n"; a.Mtu = 9000; }),
                Net("pve01", "eth1", "eth", a => a.Active = false),
                Net("pve01", "ib0", "InfiniBand"),
                Net("pve01", "bond0", "bond", a => { a.Slaves = "eth0 eth1 eth9"; a.Active = false; a.BondMiimon = "100"; a.Mtu = 9000; }),
                Net("pve01", "vmbr0", "bridge", a => { a.BridgePorts = "bond0 ib0"; a.Active = false; a.Mtu = 1500; a.Cidr6 = "fd00::1/64"; a.Gateway6 = "fd00::ff"; }),
                // not in a bridge: drawn only when it has an address
                Net("pve01", "eth5", "eth", a => { a.Cidr = "10.9.9.9/24"; a.Gateway = "10.9.9.1"; }),
                Net("pve01", "eth6", "eth", a => a.Address = "10.8.8.8"),
                Net("pve01", "eth7", "eth"),
            ]);

        Assert.Equal(["eth0 · uplink", "MTU 9000"], diagram.Box("eth0").Lines);
        Assert.Equal(["eth1", "DOWN"], diagram.Box("eth1").Lines);
        Assert.Equal(Gray, diagram.Box("eth1").Fill);
        Assert.Contains("Status: Inactive", diagram.Box("eth1").Tooltip);
        Assert.Equal(["ib0", "InfiniBand"], diagram.Box("ib0").Lines);

        Assert.Equal(["bond0", "DOWN", "Miimon: 100", "← eth0 eth1 eth9", "MTU 9000"], diagram.Box("bond0").Lines);
        Assert.Equal(Gray, diagram.Box("bond0").Fill);
        Assert.Equal(["vmbr0", "DOWN", "IP6: fd00::1/64", "GW6: fd00::ff", "MTU 1500", "Ports: bond0 ib0"], diagram.Box("vmbr0").Lines);
        Assert.Equal(Gray, diagram.Box("vmbr0").Fill);

        Assert.Equal(["eth5", "IP: 10.9.9.9/24", "GW: 10.9.9.1"], diagram.Box("eth5").Lines);
        Assert.Equal(Red, diagram.Box("eth5").Fill);
        Assert.Equal(["eth6", "IP: 10.8.8.8"], diagram.Box("eth6").Lines);
        Assert.Equal(Blue, diagram.Box("eth6").Fill);
        Assert.False(diagram.HasBox("eth7"));
        Assert.False(diagram.HasBox("eth9"));

        // eth0 and eth1 to the bond, the bond and ib0 to the bridge
        Assert.Equal(4, diagram.Lines);
    }

    [Theory]
    [InlineData("10.0.0.1", "24", "10.0.0.1/24")]
    [InlineData("10.0.0.1", "255.255.240.0", "10.0.0.1/20")]
    [InlineData("10.0.0.1/16", null, "10.0.0.1/16")]
    [InlineData("10.0.0.1", null, "10.0.0.1")]
    [InlineData("10.0.0.1", "not-a-mask", "10.0.0.1")]
    public void Address_with_a_netmask_is_shown_as_cidr(string address, string? netmask, string expected)
    {
        var diagram = Build([Net("pve01", "vmbr0", "bridge", a => { a.Address = address; a.Netmask = netmask; })]);

        Assert.Equal(["vmbr0", "IP: " + expected], diagram.Box("vmbr0").Lines);
    }

    [Fact]
    public void Open_vSwitch_bridge_shows_its_ports_and_the_address_of_its_internal_port()
    {
        var diagram = Build(
            [
                Net("pve01", "eth0", "eth"),
                Net("pve01", "eth1", "eth"),
                Net("pve01", "eth2", "OVSPort", a => a.OvsBridge = "vmbr1"),
                Net("pve01", "bond1", "OVSBond", a => { a.Slaves = "eth0 eth1"; a.BondMode = "balance-slb"; }),
                Net("pve01", "vmbr1", "OVSBridge", a => { a.OvsPorts = "bond1 vlan50"; }),
                Net("pve01", "vlan50", "OVSIntPort", a => { a.OvsBridge = "vmbr1"; a.Cidr = "10.50.0.1/24"; }),
                Net("pve01", "vlan60", "OVSIntPort", a => a.OvsBridge = "vmbr1"),
            ],
            storages: [new StorageItem { Storage = "nfs1", Type = "nfs", Server = "10.50.0.9" }]);

        Assert.Equal(["vmbr1", "OVSBridge", "OVS Ports: bond1 vlan50", "IntPort: vlan50: 10.50.0.1/24, vlan60: -"], diagram.Box("vmbr1").Lines);
        Assert.Equal(["bond1", "OVSBond", "balance-slb", "← eth0 eth1"], diagram.Box("bond1").Lines);
        Assert.Equal(["eth2", "OVSPort"], diagram.Box("eth2").Lines);
        Assert.False(diagram.HasBox("vlan50"));

        // eth0 and eth1 to the bond, the bond and eth2 to the bridge, the bridge to the storage of its subnet
        Assert.Equal(5, diagram.Lines);
    }

    [Fact]
    public void Virtual_network_of_the_sdn_is_a_bridge_of_its_nodes()
    {
        var diagram = Build(
            [
                Net("pve01", "vmbr0", "bridge"),
                Net("pve02", "vmbr0", "bridge"),
                Net("pve02", "real", "bridge", a => a.Comments = "bridge of the node"),
            ],
            [Nic(100, "web01", "vnet1"), Nic(200, "web02", "all", node: "pve02")],
            sdnVnets:
            [
                new("vnet1", "zone1", "vlan", "vmbr0", 100, "Customer", ["pve01"]),
                new("all", "zone2", "simple", null, null, null, []),
                new("real", "zone2", "simple", null, null, null, []),
            ]);

        var vnets = diagram.Boxes.Where(a => a.Title.StartsWith("vnet1")).ToList();
        Assert.Equal("vnet1 · SDN vnet · zone zone1 (vlan) · VLAN 100 · via vmbr0 · Customer", Assert.Single(vnets).Title);
        Assert.Equal(Green, vnets[0].Fill);
        Assert.True(vnets[0].X < diagram.Box("VM 100").X);

        Assert.Equal(2, diagram.Boxes.Count(a => a.Title == "all · SDN vnet · zone zone2 (simple)"));
        Assert.True(diagram.HasBox("VM 200"));

        // on pve01 the name is free, on pve02 the bridge of the node stays
        Assert.Contains(diagram.Boxes, a => a.Title == "real · SDN vnet · zone zone2 (simple)");
        Assert.Contains(diagram.Boxes, a => a.Title == "real · bridge of the node");
        Assert.Equal(2, diagram.Boxes.Count(a => a.Title.StartsWith("real · ")));
    }

    [Fact]
    public void Network_storages_are_drawn_and_linked_to_the_bridge_of_their_subnet()
    {
        var diagram = Build(
            [
                Net("pve01", "vmbr0", "bridge", a => a.Cidr = "10.0.0.1/24"),
                Net("pve01", "vmbr1", "bridge", a => a.Cidr = "172.16.0.1/20"),
                Net("pve01", "vmbr2", "bridge", a => a.Cidr = "fd00::1/64"),
            ],
            storages:
            [
                new StorageItem { Storage = "nfs1", Type = "nfs", Server = "10.0.0.50", Export = "/srv/nfs", Content = "backup,iso", Shared = true },
                new StorageItem { Storage = "pbs1", Type = "pbs", Server = "pbs.example.com", Datastore = "store1", Disable = true },
                new StorageItem { Storage = "ceph1", Type = "rbd", Monhost = "192.168.99.1,172.16.15.7;172.16.15.8", Pool = "vm" },
                new StorageItem { Storage = "ceph6", Type = "cephfs", Monhost = "fd00::10", Path = "/mnt/pve/ceph6" },
                new StorageItem { Storage = "far", Type = "CIFS", Server = "172.16.16.1" },
                new StorageItem { Storage = "other", Type = "nfs", Server = "10.0.0.51", Nodes = "pve02, pve03" },
                new StorageItem { Storage = "mine", Type = "nfs", Server = "10.0.0.52", Nodes = "pve03,PVE01" },
                new StorageItem { Storage = "local", Type = "dir", Path = "/var/lib/vz" },
                new StorageItem { Storage = "local-lvm", Type = "lvmthin" },
            ]);

        var nfs = diagram.Box("nfs1");
        Assert.Equal(["nfs1 · nfs", "Shared", "Server: 10.0.0.50", "Target: /srv/nfs", "Content: backup,iso"], nfs.Lines);
        Assert.Equal(Teal, nfs.Fill);
        Assert.Contains("Shared: Yes", nfs.Tooltip);
        Assert.Contains("Export: /srv/nfs", nfs.Tooltip);

        var pbs = diagram.Box("pbs1");
        Assert.Equal(["pbs1 · pbs", "[disabled]", "Server: pbs.example.com", "Target: store1"], pbs.Lines);
        Assert.Equal(Gray, pbs.Fill);
        Assert.Contains("Status: Disabled", pbs.Tooltip);

        Assert.Equal(["ceph1 · rbd", "Server: 192.168.99.1,172.16.15.7;172.16.15.8", "Target: vm"], diagram.Box("ceph1").Lines);
        Assert.Equal(["ceph6 · cephfs", "Server: fd00::10", "Target: /mnt/pve/ceph6"], diagram.Box("ceph6").Lines);
        Assert.True(diagram.HasBox("far"));
        Assert.True(diagram.HasBox("mine"));
        Assert.False(diagram.HasBox("other"));
        Assert.False(diagram.HasBox("local"));
        Assert.False(diagram.HasBox("local-lvm"));

        // storages are a strip under the bridges, in order of name
        var bridgeY = diagram.Box("vmbr2").Y;
        string[] names = ["ceph1", "ceph6", "far", "mine", "nfs1", "pbs1"];
        var strip = names.Select(diagram.Box).ToList();
        Assert.All(strip, a => Assert.True(a.Y > bridgeY));
        Assert.Single(strip.Select(a => a.Y).Distinct());
        Assert.Equal(strip.Select(a => a.X).Order(), strip.Select(a => a.X));
        Assert.Contains("Storages", diagram.Texts);

        // linked: nfs1 and mine to vmbr0, ceph1 to vmbr1, ceph6 to vmbr2. A host name and an address of no subnet have no line
        Assert.Equal(4, diagram.Lines);
    }

    [Fact]
    public void Without_network_storages_there_is_no_strip()
    {
        var diagram = Build([Net("pve01", "vmbr0", "bridge")], storages: [new StorageItem { Storage = "local", Type = "dir" }]);

        Assert.DoesNotContain("Storages", diagram.Texts);
        Assert.Empty(diagram.Xml.Root!.Elements(Svg + "line"));
    }

    [Fact]
    public void Names_with_special_characters_give_a_valid_document()
    {
        var diagram = Build(
            [Net("pve<01>", "vmbr0", "bridge", a => a.Comments = "a & b \"quoted\"")],
            [Nic(100, "a<b>&\"c\"", "vmbr0", node: "pve<01>", hostname: "<host>")]);

        Assert.Contains("Node: pve<01>", diagram.Texts);
        Assert.Equal("vmbr0 · a & b \"quoted\"", diagram.Box("vmbr0").Title);
        Assert.Equal(["VM 100 · a<b>&\"c\"", "<host>", "net0 → vmbr0"], diagram.Box("VM 100").Lines);
        Assert.Contains("VM 100 · a&lt;b&gt;&amp;&quot;c&quot;", diagram.Text);
    }

    [Fact]
    public void Guests_are_ordered_so_the_lines_do_not_cross()
    {
        var diagram = Build(
            [
                Net("pve01", "eth0", "eth"),
                Net("pve01", "eth1", "eth"),
                Net("pve01", "vmbr0", "bridge", a => a.BridgePorts = "eth0"),
                Net("pve01", "vmbr1", "bridge", a => a.BridgePorts = "eth1"),
            ],
            [
                Nic(100, "both", "vmbr0"),
                Nic(100, "both", "vmbr1", "net1"),
                Nic(105, "first", "vmbr0"),
                Nic(102, "second", "vmbr1"),
            ]);

        // by id the guest of both bridges is first and the line of vmbr1 crosses the one of 105
        Assert.True(diagram.Box("vmbr0").Y < diagram.Box("vmbr1").Y);
        Assert.True(diagram.Box("VM 105").Y < diagram.Box("VM 100").Y);
        Assert.True(diagram.Box("VM 100").Y < diagram.Box("VM 102").Y);
        Assert.Equal(Light, diagram.Box("VM 100").Fill);
        Assert.Equal(6, diagram.Lines);
    }

    [Fact]
    public void Order_of_the_rows_given_does_not_change_the_diagram()
    {
        NodeNetworkRow[] hostNets =
        [
            Net("pve01", "eth0", "eth"),
            Net("pve01", "eth1", "eth"),
            Net("pve01", "bond0", "bond", a => a.Slaves = "eth0 eth1"),
            Net("pve01", "vmbr0", "bridge", a => { a.BridgePorts = "bond0"; a.Cidr = "10.0.0.1/24"; }),
            Net("pve01", "vmbr1", "bridge"),
            Net("pve02", "eth0", "eth"),
            Net("pve02", "vmbr0", "bridge", a => a.BridgePorts = "eth0"),
        ];
        VmNetworkRow[] vmNets =
        [
            Nic(100, "fw", "vmbr0"),
            Nic(100, "fw", "vmbr1", "net1"),
            Nic(101, "web01", "vmbr1"),
            Nic(103, "web03", "vmbr1"),
            Nic(102, "web02", "vmbr0"),
            Nic(200, "ct01", "vmbr0", node: "pve02", type: "lxc"),
        ];
        StorageItem[] storages =
        [
            new() { Storage = "nfs2", Type = "nfs", Server = "10.0.0.51" },
            new() { Storage = "nfs1", Type = "nfs", Server = "10.0.0.50" },
        ];

        static string WithoutTime(string svg) => Regex.Replace(svg, @"\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}", "TIME");

        var straight = BuildSvg(hostNets, [], vmNets, storages, Info);
        var reversed = BuildSvg(Enumerable.Reverse(hostNets), [], Enumerable.Reverse(vmNets), Enumerable.Reverse(storages), Info);

        Assert.Equal(WithoutTime(straight), WithoutTime(reversed));
    }
}
