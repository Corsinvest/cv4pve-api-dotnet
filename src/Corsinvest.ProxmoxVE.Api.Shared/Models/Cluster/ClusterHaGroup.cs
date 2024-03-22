/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Cluster Ha group
/// </summary>
public class ClusterHaGroup
{
    /// <summary>
    /// Digest
    /// </summary>
    [JsonProperty("digest")]
    public string Digest { get; set; }

    /// <summary>
    /// Group
    /// </summary>
    [JsonProperty("group")]
    public string Group { get; set; }

    /// <summary>
    /// Nodes
    /// </summary>
    [JsonProperty("nodes")]
    public string Nodes { get; set; }

    /// <summary>
    /// Nofailback
    /// </summary>
    [JsonProperty("nofailback")]
    public bool Nofailback { get; set; }

    /// <summary>
    /// Restricted
    /// </summary>
    [JsonProperty("restricted")]
    public bool Restricted { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Comment
    /// </summary>
    [JsonProperty("comment")]
    public string Comment { get; set; }
}