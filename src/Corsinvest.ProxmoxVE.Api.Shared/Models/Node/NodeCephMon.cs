/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node Ceph MON
/// </summary>
public class NodeCephMon : ModelBase
{
    /// <summary>
    /// Name
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// State
    /// </summary>
    [JsonProperty("state")]
    public string State { get; set; }

    /// <summary>
    /// Address
    /// </summary>
    [JsonProperty("addr")]
    public string Addr { get; set; }

    /// <summary>
    /// Host
    /// </summary>
    [JsonProperty("host")]
    public string Host { get; set; }

    /// <summary>
    /// Rank
    /// </summary>
    [JsonProperty("rank")]
    public int? Rank { get; set; }

    /// <summary>
    /// Quorum
    /// </summary>
    [JsonProperty("quorum")]
    public bool? Quorum { get; set; }

    /// <summary>
    /// Ceph version
    /// </summary>
    [JsonProperty("ceph_version")]
    public string CephVersion { get; set; }

    /// <summary>
    /// Service
    /// </summary>
    [JsonProperty("service")]
    public int? Service { get; set; }
}
