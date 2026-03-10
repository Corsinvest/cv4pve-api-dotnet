/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node Disk LVM Thin
/// </summary>
public class NodeDiskLvmThin : ModelBase
{
    /// <summary>
    /// LV name
    /// </summary>
    [JsonProperty("lv")]
    public string Lv { get; set; }

    /// <summary>
    /// VG name
    /// </summary>
    [JsonProperty("vg")]
    public string Vg { get; set; }

    /// <summary>
    /// LV size in bytes
    /// </summary>
    [JsonProperty("lv_size")]
    public long LvSize { get; set; }

    /// <summary>
    /// Metadata size in bytes
    /// </summary>
    [JsonProperty("metadata_size")]
    public long MetadataSize { get; set; }

    /// <summary>
    /// Metadata used in bytes
    /// </summary>
    [JsonProperty("metadata_used")]
    public long MetadataUsed { get; set; }

    /// <summary>
    /// Used in bytes
    /// </summary>
    [JsonProperty("used")]
    public long Used { get; set; }
}
