/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Cluster HA Rule
/// </summary>
public class ClusterHaRule : ModelBase
{
    /// <summary>
    /// Rule
    /// </summary>
    [JsonProperty("rule")]
    public string Rule { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }
}
