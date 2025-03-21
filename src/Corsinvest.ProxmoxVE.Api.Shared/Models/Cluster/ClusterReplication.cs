/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Cluster replication
/// </summary>
public class ClusterReplication : ModelBase
{
    /// <summary>
    /// Schedule
    /// </summary>
    [JsonProperty("schedule")]
    public string Schedule { get; set; }

    /// <summary>
    /// Id
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Source
    /// </summary>
    [JsonProperty("source")]
    public string Source { get; set; }

    /// <summary>
    /// Guest
    /// </summary>
    [JsonProperty("guest")]
    public string Guest { get; set; }

    /// <summary>
    /// Job Num
    /// </summary>
    [JsonProperty("jobnum")]
    public string JobNum { get; set; }

    /// <summary>
    /// Traget
    /// </summary>
    [JsonProperty("target")]
    public string Target { get; set; }

    /// <summary>
    /// Disable
    /// </summary>
    [JsonProperty("disable")]
    public bool Disable { get; set; }

    /// <summary>
    /// Rate
    /// </summary>
    [JsonProperty("rate")]
    public int Rate { get; set; }
}