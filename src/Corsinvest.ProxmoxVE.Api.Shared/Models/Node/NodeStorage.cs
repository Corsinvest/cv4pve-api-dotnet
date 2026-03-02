/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node storage
/// </summary>
public class NodeStorage : ModelBase, IStorageItem
{
    /// <summary>
    /// Storage type.
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Used storage space in bytes.
    /// </summary>
    [JsonProperty("used")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public long Used { get; set; }

    /// <summary>
    /// Available storage space in bytes.
    /// </summary>
    [JsonProperty("avail")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public long Available { get; set; }

    /// <summary>
    /// Allowed storage content types.
    /// </summary>
    [JsonProperty("content")]
    public string Content { get; set; }

    /// <summary>
    /// Set when storage is accessible.
    /// </summary>
    [JsonProperty("active")]
    public bool Active { get; set; }

    /// <summary>
    /// Used fraction (used/total).
    /// </summary>
    [JsonProperty("used_fraction")]
    [Display(Name = "Usage %")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatPercentage)]
    public double UsagePercentage { get; set; }

    /// <summary>
    /// Shared flag from storage configuration.
    /// </summary>
    [JsonProperty("shared")]
    public bool Shared { get; set; }

    /// <summary>
    /// Total storage space in bytes.
    /// </summary>
    [JsonProperty("total")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public long Size { get; set; }

    /// <summary>
    /// The storage identifier.
    /// </summary>
    [JsonProperty("storage")]
    public string Storage { get; set; }

    /// <summary>
    /// Set when storage is enabled (not disabled).
    /// </summary>
    [JsonProperty("enabled")]
    public bool Enabled { get; set; }

    /// <summary>
    /// Lists the supported and default format. Use 'formats' instead. Only included if 'format' parameter is set.
    /// </summary>
    [JsonProperty("formats")]
    public object Formats { get; set; }

    /// <summary>
    /// Instead of creating new volumes, one must select one that is already existing. Only included if 'format' parameter is set.
    /// </summary>
    [JsonProperty("select_existing")]
    public bool SelectExisting { get; set; }
}