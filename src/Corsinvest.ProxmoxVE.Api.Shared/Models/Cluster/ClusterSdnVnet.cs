/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Cluster SDN VNet
/// </summary>
public class ClusterSdnVnet : ModelBase
{
    /// <summary>
    /// VNet id
    /// </summary>
    [JsonProperty("vnet")]
    public string Vnet { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Zone
    /// </summary>
    [JsonProperty("zone")]
    public string Zone { get; set; }

    /// <summary>
    /// Tag
    /// </summary>
    [JsonProperty("tag")]
    public int? Tag { get; set; }

    /// <summary>
    /// Alias
    /// </summary>
    [JsonProperty("alias")]
    public string Alias { get; set; }

    /// <summary>
    /// Digest
    /// </summary>
    [JsonProperty("digest")]
    public string Digest { get; set; }

    /// <summary>
    /// Isolate ports
    /// </summary>
    [JsonProperty("isolate-ports")]
    public bool? IsolatePorts { get; set; }

    /// <summary>
    /// Vlan aware
    /// </summary>
    [JsonProperty("vlanaware")]
    public bool? VlanAware { get; set; }

    /// <summary>
    /// State
    /// </summary>
    [JsonProperty("state")]
    public string State { get; set; }
}
