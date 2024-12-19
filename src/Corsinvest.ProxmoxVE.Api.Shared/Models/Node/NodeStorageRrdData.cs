/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Storage RrdData
/// </summary>
public class NodeStorageRrdData : ModelBase
{
    /// <summary>
    /// Used
    /// </summary>
    [JsonProperty("used")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public long Used { get; set; }

    /// <summary>
    /// Time
    /// </summary>
    [JsonProperty("time")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatUnixTime )]
    public int Time { get; set; }

    /// <summary>
    /// Size
    /// </summary>
    [JsonProperty("total")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public long Size { get; set; }
}