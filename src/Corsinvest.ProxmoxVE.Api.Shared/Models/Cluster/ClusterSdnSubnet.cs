/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Cluster SDN Subnet
/// </summary>
public class ClusterSdnSubnet : ModelBase
{
    /// <summary>
    /// Subnet identifier (e.g. 10.0.0.0/24)
    /// </summary>
    [JsonProperty("subnet")]
    public string Subnet { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// VNet this subnet belongs to
    /// </summary>
    [JsonProperty("vnet")]
    public string Vnet { get; set; }

    /// <summary>
    /// Subnet gateway
    /// </summary>
    [JsonProperty("gateway")]
    public string Gateway { get; set; }

    /// <summary>
    /// DHCP DNS server IP
    /// </summary>
    [JsonProperty("dhcp-dns-server")]
    public string DhcpDnsServer { get; set; }

    /// <summary>
    /// DNS zone prefix
    /// </summary>
    [JsonProperty("dnszoneprefix")]
    public string DnsZonePrefix { get; set; }

    /// <summary>
    /// Enable masquerade (SNAT) for this subnet
    /// </summary>
    [JsonProperty("snat")]
    public bool? Snat { get; set; }

    /// <summary>
    /// Digest
    /// </summary>
    [JsonProperty("digest")]
    public string Digest { get; set; }
}
