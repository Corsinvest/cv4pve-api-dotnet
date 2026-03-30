/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
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
    [JsonProperty("Algorithm")]
    public string Algorithm { get; set; }

    /// <summary>
    /// Last poll call
    /// </summary>
    [JsonProperty("Last poll call")]
    public string LastPollCall { get; set; }

    /// <summary>
    /// Model
    /// </summary>
    [JsonProperty("Model")]
    public string Model { get; set; }

    /// <summary>
    /// QNetd host
    /// </summary>
    [JsonProperty("QNetd host")]
    public string QNetdHost { get; set; }

    /// <summary>
    /// State
    /// </summary>
    [JsonProperty("State")]
    public string State { get; set; }

    /// <summary>
    /// Tie-breaker
    /// </summary>
    [JsonProperty("Tie-breaker")]
    public string TieBreaker { get; set; }
}