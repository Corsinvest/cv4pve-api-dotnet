/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Status
/// </summary>
public class ClusterStatus : ModelBase
{
    /// <summary>
    /// Name
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Tape
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Nodes
    /// </summary>
    [JsonProperty("nodes")]
    public int Nodes { get; set; }

    /// <summary>
    /// Version
    /// </summary>
    [JsonProperty("version")]
    public int Version { get; set; }

    /// <summary>
    /// Quorate
    /// </summary>
    [JsonProperty("quorate")]
    public int Quorate { get; set; }

    /// <summary>
    /// Id
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Level
    /// </summary>
    [JsonProperty("level")]
    public string Level { get; set; }

    /// <summary>
    /// IpAddress
    /// </summary>
    [JsonProperty("ip")]
    public string IpAddress { get; set; }

    /// <summary>
    /// Local
    /// </summary>
    [JsonProperty("local")]
    public int? Local { get; set; }

    /// <summary>
    /// Node id
    /// </summary>
    [JsonProperty("nodeid")]
    public int? NodeId { get; set; }

    /// <summary>
    /// Is Online
    /// </summary>
    [JsonProperty("online")]
    public bool IsOnline { get; set; }
}