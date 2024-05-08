/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Cluster Firewall Options
/// </summary>
public class ClusterFirewallOptions : ModelBase
{
    /// <summary>
    /// Digest
    /// </summary>
    [JsonProperty("digest")]
    public string Digest { get; set; }

    /// <summary>
    /// Enable
    /// </summary>
    [JsonProperty("enable")]
    public bool Enable { get; set; }

    /// <summary>
    /// LogRatelimit
    /// </summary>
    [JsonProperty("log_ratelimit")]
    public string LogRatelimit { get; set; }

    /// <summary>
    /// PolicyIn
    /// </summary>
    [JsonProperty("policy_in")]
    public string PolicyIn { get; set; }

    /// <summary>
    /// PolicyOut
    /// </summary>
    [JsonProperty("policy_out")]
    public string PolicyOut { get; set; }
}