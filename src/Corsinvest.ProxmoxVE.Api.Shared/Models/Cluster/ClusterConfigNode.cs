/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Cluster config node
/// </summary>
public class ClusterConfigNode : ModelBase
{
    /// <summary>
    /// Quorum Votes
    /// </summary>
    [JsonProperty("quorum_votes")]
    public string QuorumVotes { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Node id
    /// </summary>
    [JsonProperty("nodeid")]
    public string NodeId { get; set; }

    /// <summary>
    /// Node
    /// </summary>
    [JsonProperty("node")]
    public string Node { get; set; }
}