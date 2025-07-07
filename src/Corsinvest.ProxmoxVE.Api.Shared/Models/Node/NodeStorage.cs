﻿/*
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
    /// Type
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Used
    /// </summary>
    [JsonProperty("used")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public long Used { get; set; }

    /// <summary>
    /// Available
    /// </summary>
    [JsonProperty("avail")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public long Available { get; set; }

    /// <summary>
    /// Content
    /// </summary>
    [JsonProperty("content")]
    public string Content { get; set; }

    /// <summary>
    /// Active
    /// </summary>
    [JsonProperty("active")]
    public bool Active { get; set; }

    /// <summary>
    /// Usage Percentage
    /// </summary>
    [JsonProperty("used_fraction")]
    [Display(Name = "Usage %")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatPercentage)]
    public double UsagePercentage { get; set; }

    /// <summary>
    /// Shared
    /// </summary>
    [JsonProperty("shared")]
    public bool Shared { get; set; }

    /// <summary>
    /// Size
    /// </summary>
    [JsonProperty("total")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public long Size { get; set; }

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
}