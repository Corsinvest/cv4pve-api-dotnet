/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Cluster SDN Zone
/// </summary>
public class ClusterSdnZone : ModelBase
{
    /// <summary>
    /// Zone id
    /// </summary>
    [JsonProperty("zone")]
    public string Zone { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// MTU
    /// </summary>
    [JsonProperty("mtu")]
    public int? Mtu { get; set; }

    /// <summary>
    /// Nodes
    /// </summary>
    [JsonProperty("nodes")]
    public string Nodes { get; set; }

    /// <summary>
    /// Bridge
    /// </summary>
    [JsonProperty("bridge")]
    public string Bridge { get; set; }

    /// <summary>
    /// Controller
    /// </summary>
    [JsonProperty("controller")]
    public string Controller { get; set; }

    /// <summary>
    /// Tag
    /// </summary>
    [JsonProperty("tag")]
    public int? Tag { get; set; }

    /// <summary>
    /// State
    /// </summary>
    [JsonProperty("state")]
    public string State { get; set; }

    /// <summary>
    /// Digest
    /// </summary>
    [JsonProperty("digest")]
    public string Digest { get; set; }

    /// <summary>
    /// IPAM
    /// </summary>
    [JsonProperty("ipam")]
    public string Ipam { get; set; }

    /// <summary>
    /// DNS
    /// </summary>
    [JsonProperty("dns")]
    public string Dns { get; set; }
}
