/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Cluster backup
/// </summary>
public class ClusterBackup : IStorageItem
{
    /// <summary>
    /// All
    /// </summary>
    [JsonProperty("all")]
    public bool All { get; set; }

    /// <summary>
    /// Mode
    /// </summary>
    [JsonProperty("mode")]
    public string Mode { get; set; }

    /// <summary>
    /// Start time
    /// </summary>
    [JsonProperty("starttime")]
    public string StartTime { get; set; }

    /// <summary>
    /// Storage
    /// </summary>
    [JsonProperty("storage")]
    public string Storage { get; set; }

    /// <summary>
    /// Enabled
    /// </summary>
    [JsonProperty("enabled")]
    public bool Enabled { get; set; }

    /// <summary>
    /// Mail to
    /// </summary>
    [JsonProperty("mailto")]
    public string Mailto { get; set; }

    /// <summary>
    /// Pool
    /// </summary>
    [JsonProperty("pool")]
    public string Pool { get; set; }

    /// <summary>
    /// VmId
    /// </summary>
    [JsonProperty("vmid")]
    public string VmId { get; set; }

    /// <summary>
    /// Day of week
    /// </summary>
    [JsonProperty("dow")]
    public string DayOfWeek { get; set; }

    /// <summary>
    /// Mail notification
    /// </summary>
    [JsonProperty("mailnotification")]
    public string MailNotification { get; set; }

    /// <summary>
    /// Quiet
    /// </summary>
    [JsonProperty("quiet")]
    public bool Quiet { get; set; }

    /// <summary>
    /// Id
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Compress
    /// </summary>
    [JsonProperty("compress")]
    public string Compress { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// VmId
    /// </summary>
    [JsonProperty("vmid")]
    public string Vmid { get; set; }

    /// <summary>
    /// Schedule
    /// </summary>
    [JsonProperty("schedule")]
    public string Schedule { get; set; }

    /// <summary>
    /// NotesTemplate
    /// </summary>
    [JsonProperty("notes-template")]
    public string NotesTemplate { get; set; }

    /// <summary>
    /// NextRun
    /// </summary>
    [JsonProperty("next-run")]
    public int NextRun { get; set; }
}