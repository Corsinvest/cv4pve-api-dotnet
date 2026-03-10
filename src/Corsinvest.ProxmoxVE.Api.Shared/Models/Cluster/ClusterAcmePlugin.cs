/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Cluster ACME Plugin
/// </summary>
public class ClusterAcmePlugin : ModelBase
{
    /// <summary>
    /// Plugin id
    /// </summary>
    [JsonProperty("plugin")]
    public string Plugin { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// API provider
    /// </summary>
    [JsonProperty("api")]
    public string Api { get; set; }

    /// <summary>
    /// Plugin data
    /// </summary>
    [JsonProperty("data")]
    public string Data { get; set; }

    /// <summary>
    /// Digest
    /// </summary>
    [JsonProperty("digest")]
    public string Digest { get; set; }

    /// <summary>
    /// Disable
    /// </summary>
    [JsonProperty("disable")]
    public bool? Disable { get; set; }

    /// <summary>
    /// Nodes
    /// </summary>
    [JsonProperty("nodes")]
    public string Nodes { get; set; }

    /// <summary>
    /// Validation delay
    /// </summary>
    [JsonProperty("validation-delay")]
    public int? ValidationDelay { get; set; }
}
