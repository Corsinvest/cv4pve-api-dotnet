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
    /// Status entry ID (quorum, master, lrm:&lt;node&gt;, service:&lt;sid&gt;).
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Node associated to status entry.
    /// </summary>
    [JsonProperty("node")]
    public string Node { get; set; }

    /// <summary>
    /// For type 'quorum'. Whether the cluster is quorate or not.
    /// </summary>
    [JsonProperty("quorate")]
    public bool Quorate { get; set; }

    /// <summary>
    /// Status of the entry (value depends on type).
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; }

    /// <summary>
    /// Type of status entry.
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// For type 'lrm','master'. Timestamp of the status information.
    /// </summary>
    [JsonProperty("timestamp")]
    public long Timestamp { get; set; }

    /// <summary>
    /// For type 'service'. Service state as seen by the CRM.
    /// </summary>
    [JsonProperty("crm_state")]
    public string CrmState { get; set; }

    /// <summary>
    /// For type 'service'.
    /// </summary>
    [JsonProperty("max_relocate")]
    public int MaxRelocate { get; set; }

    /// <summary>
    /// For type 'service'.
    /// </summary>
    [JsonProperty("max_restart")]
    public int MaxRestart { get; set; }

    /// <summary>
    /// For type 'service'. Requested service state.
    /// </summary>
    [JsonProperty("request_state")]
    public string RequestState { get; set; }

    /// <summary>
    /// For type 'service'. Service ID.
    /// </summary>
    [JsonProperty("sid")]
    public string Sid { get; set; }

    /// <summary>
    /// For type 'service'. Verbose service state.
    /// </summary>
    [JsonProperty("state")]
    public string State { get; set; }

    /// <summary>
    /// The HA resource is automatically migrated to the node with the highest priority according to their node affinity rule, if a node with a higher priority than the current node comes online.
    /// </summary>
    [JsonProperty("failback")]
    public bool Failback { get; set; }
}