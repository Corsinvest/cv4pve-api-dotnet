/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Cluster Job Realm Sync
/// </summary>
public class ClusterJobRealmSync : ModelBase
{
    /// <summary>
    /// Id
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Realm
    /// </summary>
    [JsonProperty("realm")]
    public string Realm { get; set; }

    /// <summary>
    /// Schedule
    /// </summary>
    [JsonProperty("schedule")]
    public string Schedule { get; set; }

    /// <summary>
    /// Enabled
    /// </summary>
    [JsonProperty("enabled")]
    public bool Enabled { get; set; }

    /// <summary>
    /// Comment
    /// </summary>
    [JsonProperty("comment")]
    public string Comment { get; set; }

    /// <summary>
    /// Last run
    /// </summary>
    [JsonProperty("last-run")]
    public long? LastRun { get; set; }

    /// <summary>
    /// Next run
    /// </summary>
    [JsonProperty("next-run")]
    public long? NextRun { get; set; }

    /// <summary>
    /// Remove vanished
    /// </summary>
    [JsonProperty("remove-vanished")]
    public string RemoveVanished { get; set; }

    /// <summary>
    /// Scope
    /// </summary>
    [JsonProperty("scope")]
    public string Scope { get; set; }
}
