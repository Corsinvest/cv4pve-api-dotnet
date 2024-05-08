/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Cluster Ha Status Current
/// </summary>
public class ClusterHaStatusCurrent : ModelBase
{
    /// <summary>
    /// Id
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Node
    /// </summary>
    [JsonProperty("node")]
    public string Node { get; set; }

    /// <summary>
    /// Quorate
    /// </summary>
    [JsonProperty("quorate")]
    public int Quorate { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Timestamp
    /// </summary>
    [JsonProperty("timestamp")]
    public long Timestamp { get; set; }

    /// <summary>
    /// CrmState
    /// </summary>
    [JsonProperty("crm_state")]
    public string CrmState { get; set; }

    /// <summary>
    /// Group
    /// </summary>
    [JsonProperty("group")]
    public string Group { get; set; }

    /// <summary>
    /// MaxRelocate
    /// </summary>
    [JsonProperty("max_relocate")]
    public int MaxRelocate { get; set; }

    /// <summary>
    /// MaxRestart
    /// </summary>
    [JsonProperty("max_restart")]
    public int MaxRestart { get; set; }

    /// <summary>
    /// RequestState
    /// </summary>
    [JsonProperty("request_state")]
    public string RequestState { get; set; }

    /// <summary>
    /// Sid
    /// </summary>
    [JsonProperty("sid")]
    public string Sid { get; set; }

    /// <summary>
    /// State
    /// </summary>
    [JsonProperty("state")]
    public string State { get; set; }
}