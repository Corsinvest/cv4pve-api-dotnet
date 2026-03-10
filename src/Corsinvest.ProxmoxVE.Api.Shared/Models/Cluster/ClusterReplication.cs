/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Cluster replication
/// </summary>
public class ClusterReplication : ModelBase
{
    /// <summary>
    /// Storage replication schedule. The format is a subset of `systemd` calendar events.
    /// </summary>
    [JsonProperty("schedule")]
    public string Schedule { get; set; }

    /// <summary>
    /// Replication Job ID. The ID is composed of a Guest ID and a job number, separated by a hyphen, i.e. '&lt;GUEST&gt;-&lt;JOBNUM&gt;'.
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Section type.
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// For internal use, to detect if the guest was stolen.
    /// </summary>
    [JsonProperty("source")]
    public string Source { get; set; }

    /// <summary>
    /// Guest ID.
    /// </summary>
    [JsonProperty("guest")]
    public string Guest { get; set; }

    /// <summary>
    /// Unique, sequential ID assigned to each job.
    /// </summary>
    [JsonProperty("jobnum")]
    public string JobNum { get; set; }

    /// <summary>
    /// Target node.
    /// </summary>
    [JsonProperty("target")]
    public string Target { get; set; }

    /// <summary>
    /// Flag to disable/deactivate the entry.
    /// </summary>
    [JsonProperty("disable")]
    public bool Disable { get; set; }

    /// <summary>
    /// Rate limit in mbps (megabytes per second) as floating point number.
    /// </summary>
    [JsonProperty("rate")]
    public int Rate { get; set; }
    /// <summary>
    /// Description.
    /// </summary>
    [JsonProperty("comment")]
    public string Comment { get; set; }

    /// <summary>
    /// Mark the replication job for removal. The job will remove all local replication snapshots. When set to 'full', it also tries to remove replicated volumes on the target. The job then removes itself fro...
    /// </summary>
    [JsonProperty("remove_job")]
    public string RemoveJob { get; set; }
}
