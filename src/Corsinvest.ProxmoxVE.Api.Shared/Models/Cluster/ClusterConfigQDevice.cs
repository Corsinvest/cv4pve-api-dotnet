/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Cluster config Qdevice
/// </summary>
public class ClusterConfigQDevice : ModelBase
{
    /// <summary>
    /// Algorithm
    /// </summary>
    /// <value></value>
    [JsonProperty("Algorithm")]
    public string Algorithm { get; set; }

    /// <summary>
    /// Last poll call
    /// </summary>
    /// <value></value>
    [JsonProperty("Last poll call")]
    public string LastPollCall { get; set; }

    /// <summary>
    /// Model
    /// </summary>
    /// <value></value>
    [JsonProperty("Model")]
    public string Model { get; set; }

    /// <summary>
    /// QNetd host
    /// </summary>
    /// <value></value>
    [JsonProperty("QNetd host")]
    public string QNetdHost { get; set; }

    /// <summary>
    /// State
    /// </summary>
    /// <value></value>
    [JsonProperty("State")]
    public string State { get; set; }

    /// <summary>
    /// Tie-breaker
    /// </summary>
    /// <value></value>
    [JsonProperty("Tie-breaker")]
    public string TieBreaker { get; set; }
}