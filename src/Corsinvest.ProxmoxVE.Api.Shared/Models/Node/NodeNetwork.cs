/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node network
/// </summary>
public class NodeNetwork : ModelBase
{
    /// <summary>
    /// The network configuration method for IPv6.
    /// </summary>
    [JsonProperty("method6")]
    public string Method6 { get; set; }

    /// <summary>
    /// The order of the interface.
    /// </summary>
    [JsonProperty("priority")]
    public int Priority { get; set; }

    /// <summary>
    /// Network interface type
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Bonding mode.
    /// </summary>
    [JsonProperty("bond_mode")]
    public string BondMode { get; set; }

    /// <summary>
    /// IPv4 CIDR.
    /// </summary>
    [JsonProperty("cidr")]
    public string Cidr { get; set; }

    /// <summary>
    /// Set to true if the interface is active.
    /// </summary>
    [JsonProperty("active")]
    public bool Active { get; set; }

    /// <summary>
    /// IP address.
    /// </summary>
    [JsonProperty("address")]
    public string Address { get; set; }

    /// <summary>
    /// Comments
    /// </summary>
    [JsonProperty("comments")]
    public string Comments { get; set; }

    /// <summary>
    /// The network families.
    /// </summary>
    [JsonProperty("families")]
    public IEnumerable<string> Families { get; set; } = [];

    /// <summary>
    /// Network interface name.
    /// </summary>
    [JsonProperty("iface")]
    public string Interface { get; set; }

    /// <summary>
    /// Bond Miimon
    /// </summary>
    /// <value></value>
    [JsonProperty("bond_miimon")]
    public string BondMiimon { get; set; }

    /// <summary>
    /// Specify the interfaces used by the bonding device.
    /// </summary>
    [JsonProperty("slaves")]
    public string Slaves { get; set; }

    /// <summary>
    /// Automatically start interface on boot.
    /// </summary>
    [JsonProperty("autostart")]
    public bool AutoStart { get; set; }

    /// <summary>
    /// Specify the primary interface for active-backup bond.
    /// </summary>
    [JsonProperty("bond-primary")]
    public string BondPrimary { get; set; }

    /// <summary>
    /// The network configuration method for IPv4.
    /// </summary>
    [JsonProperty("method")]
    public string Method { get; set; }

    /// <summary>
    /// Network mask.
    /// </summary>
    [JsonProperty("netmask")]
    public string Netmask { get; set; }

    /// <summary>
    /// Bridge Stp
    /// </summary>
    /// <value></value>
    [JsonProperty("bridge_stp")]
    public string BridgeStp { get; set; }

    /// <summary>
    /// Enable bridge vlan support.
    /// </summary>
    [JsonProperty("bridge_vlan_aware")]
    public bool? BridgeVlanAware { get; set; }

    /// <summary>
    /// Specify the allowed VLANs. For example: '2 4 100-200'. Only used if the bridge is VLAN aware.
    /// </summary>
    [JsonProperty("bridge_vids")]
    public string BridgeVids { get; set; }

    /// <summary>
    /// Bridge Fd
    /// </summary>
    /// <value></value>
    [JsonProperty("bridge_fd")]
    public string BridgeFd { get; set; }

    /// <summary>
    /// Specify the interfaces you want to add to your bridge.
    /// </summary>
    [JsonProperty("bridge_ports")]
    public string BridgePorts { get; set; }

    /// <summary>
    /// Set to true if the interface physically exists.
    /// </summary>
    [JsonProperty("exists")]
    public bool? Exists { get; set; }

    /// <summary>
    /// Default gateway address.
    /// </summary>
    [JsonProperty("gateway")]
    public string Gateway { get; set; }

    /// <summary>
    /// IP address.
    /// </summary>
    [JsonProperty("address6")]
    public string Address6 { get; set; }

    /// <summary>
    /// IPv6 CIDR.
    /// </summary>
    [JsonProperty("cidr6")]
    public string Cidr6 { get; set; }

    /// <summary>
    /// Default ipv6 gateway address.
    /// </summary>
    [JsonProperty("gateway6")]
    public string Gateway6 { get; set; }

    /// <summary>
    /// Network mask.
    /// </summary>
    [JsonProperty("netmask6")]
    public int? Netmask6 { get; set; }

    /// <summary>
    /// Comments
    /// </summary>
    [JsonProperty("comments6")]
    public string Comments6 { get; set; }

    /// <summary>
    /// MTU.
    /// </summary>
    [JsonProperty("mtu")]
    public int? Mtu { get; set; }

    /// <summary>
    /// The link type.
    /// </summary>
    [JsonProperty("link-type")]
    public string LinkType { get; set; }

    /// <summary>
    /// The uplink ID.
    /// </summary>
    [JsonProperty("uplink-id")]
    public string UplinkId { get; set; }

    /// <summary>
    /// vlan-id for a custom named vlan interface (ifupdown2 only).
    /// </summary>
    [JsonProperty("vlan-id")]
    public int? VlanId { get; set; }

    /// <summary>
    /// The VLAN protocol.
    /// </summary>
    [JsonProperty("vlan-protocol")]
    public string VlanProtocol { get; set; }

    /// <summary>
    /// Specify the raw interface for the vlan interface.
    /// </summary>
    [JsonProperty("vlan-raw-device")]
    public string VlanRawDevice { get; set; }

    /// <summary>
    /// Selects the transmit hash policy to use for slave selection in balance-xor and 802.3ad modes.
    /// </summary>
    [JsonProperty("bond_xmit_hash_policy")]
    public string BondXmitHashPolicy { get; set; }

    /// <summary>
    /// A list of additional interface options for IPv4.
    /// </summary>
    [JsonProperty("options")]
    public IEnumerable<string> Options { get; set; } = [];

    /// <summary>
    /// A list of additional interface options for IPv6.
    /// </summary>
    [JsonProperty("options6")]
    public IEnumerable<string> Options6 { get; set; } = [];

    /// <summary>
    /// The bridge port access VLAN.
    /// </summary>
    [JsonProperty("bridge-access")]
    public int? BridgeAccess { get; set; }

    /// <summary>
    /// Bridge port ARP/ND suppress flag.
    /// </summary>
    [JsonProperty("bridge-arp-nd-suppress")]
    public bool? BridgeArpNdSuppress { get; set; }

    /// <summary>
    /// Bridge port learning flag.
    /// </summary>
    [JsonProperty("bridge-learning")]
    public bool? BridgeLearning { get; set; }

    /// <summary>
    /// Bridge port multicast flood flag.
    /// </summary>
    [JsonProperty("bridge-multicast-flood")]
    public bool? BridgeMulticastFlood { get; set; }

    /// <summary>
    /// Bridge port unicast flood flag.
    /// </summary>
    [JsonProperty("bridge-unicast-flood")]
    public bool? BridgeUnicastFlood { get; set; }

    /// <summary>
    /// Specify the interfaces used by the bonding device.
    /// </summary>
    [JsonProperty("ovs_bonds")]
    public string OvsBonds { get; set; }

    /// <summary>
    /// The OVS bridge associated with a OVS port. This is required when you create an OVS port.
    /// </summary>
    [JsonProperty("ovs_bridge")]
    public string OvsBridge { get; set; }

    /// <summary>
    /// OVS interface options.
    /// </summary>
    [JsonProperty("ovs_options")]
    public string OvsOptions { get; set; }

    /// <summary>
    /// Specify the interfaces you want to add to your bridge.
    /// </summary>
    [JsonProperty("ovs_ports")]
    public string OvsPorts { get; set; }

    /// <summary>
    /// Specify a VLan tag (used by OVSPort, OVSIntPort, OVSBond)
    /// </summary>
    [JsonProperty("ovs_tag")]
    public int? OvsTag { get; set; }

    /// <summary>
    /// The VXLAN ID.
    /// </summary>
    [JsonProperty("vxlan-id")]
    public int? VxlanId { get; set; }

    /// <summary>
    /// The VXLAN local tunnel IP.
    /// </summary>
    [JsonProperty("vxlan-local-tunnelip")]
    public string VxlanLocalTunnelIp { get; set; }

    /// <summary>
    /// The physical device for the VXLAN tunnel.
    /// </summary>
    [JsonProperty("vxlan-physdev")]
    public string VxlanPhysDev { get; set; }

    /// <summary>
    /// The VXLAN SVC node IP.
    /// </summary>
    [JsonProperty("vxlan-svcnodeip")]
    public string VxlanSvcNodeIp { get; set; }
}