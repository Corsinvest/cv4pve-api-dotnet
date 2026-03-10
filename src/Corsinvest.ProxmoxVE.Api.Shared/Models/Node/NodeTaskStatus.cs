/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node Task Status
/// </summary>
public class NodeTaskStatus : ModelBase
{
    /// <summary>
    /// UPID
    /// </summary>
    [JsonProperty("upid")]
    public string Upid { get; set; }

    /// <summary>
    /// Node
    /// </summary>
    [JsonProperty("node")]
    public string Node { get; set; }

    /// <summary>
    /// PID
    /// </summary>
    [JsonProperty("pid")]
    public int Pid { get; set; }

    /// <summary>
    /// PStart
    /// </summary>
    [JsonProperty("pstart")]
    public long PStart { get; set; }

    /// <summary>
    /// Start time
    /// </summary>
    [JsonProperty("starttime")]
    public long StartTime { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Id
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// User
    /// </summary>
    [JsonProperty("user")]
    public string User { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; }

    /// <summary>
    /// Exit status
    /// </summary>
    [JsonProperty("exitstatus")]
    public string ExitStatus { get; set; }
}
