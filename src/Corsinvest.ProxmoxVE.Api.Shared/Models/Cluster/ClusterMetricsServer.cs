/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Cluster Metrics Server
/// </summary>
public class ClusterMetricsServer : ModelBase
{
    /// <summary>
    /// Id
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Server address
    /// </summary>
    [JsonProperty("server")]
    public string Server { get; set; }

    /// <summary>
    /// Port
    /// </summary>
    [JsonProperty("port")]
    public int Port { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Disable
    /// </summary>
    [JsonProperty("disable")]
    public bool Disable { get; set; }
}
