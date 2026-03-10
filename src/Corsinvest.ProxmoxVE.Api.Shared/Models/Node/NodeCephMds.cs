/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node Ceph MDS
/// </summary>
public class NodeCephMds : ModelBase
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
    /// Standby replay
    /// </summary>
    [JsonProperty("standby_replay")]
    public bool? StandbyReplay { get; set; }
}
