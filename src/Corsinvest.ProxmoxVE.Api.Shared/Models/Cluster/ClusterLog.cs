/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Cluster log
/// </summary>
public class ClusterLog : ModelBase
{
    /// <summary>
    /// Message
    /// </summary>
    [JsonProperty("msg")]
    public string Message { get; set; }

    /// <summary>
    /// Uid
    /// </summary>
    [JsonProperty("uid")]
    public string Uid { get; set; }

    /// <summary>
    /// User
    /// </summary>
    [JsonProperty("user")]
    public string User { get; set; }

    /// <summary>
    /// Pid
    /// </summary>
    [JsonProperty("pid")]
    public int Pid { get; set; }

    /// <summary>
    /// Severity
    /// </summary>
    [JsonProperty("pri")]
    public int Severity { get; set; }

    /// <summary>
    /// Severity
    /// </summary>
    [Display(Name = "Severity")]
    public ClusterLogSeverity SeverityEnum => (ClusterLogSeverity)Severity;

    /// <summary>
    /// Id
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Service
    /// </summary>
    [JsonProperty("tag")]
    public string Service { get; set; }

    /// <summary>
    /// Node
    /// </summary>
    [JsonProperty("node")]
    public string Node { get; set; }

    /// <summary>
    /// Time unix format
    /// </summary>
    [JsonProperty("time")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatUnixTime)]
    public int Time { get; set; }

    /// <summary>
    /// Time
    /// </summary>
    public DateTime TimeDate => DateTimeOffset.FromUnixTimeSeconds(Time).DateTime;
}