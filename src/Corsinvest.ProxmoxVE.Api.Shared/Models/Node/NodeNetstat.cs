﻿/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node Netstat
/// </summary>
public class NodeNetstat : ModelBase
{
    /// <summary>
    /// Device
    /// </summary>
    [JsonProperty("dev")]
    public string Device { get; set; }

    /// <summary>
    /// In
    /// </summary>
    [JsonProperty("in")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public long In { get; set; }

    /// <summary>
    /// Out
    /// </summary>
    [JsonProperty("out")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public long Out { get; set; }

    /// <summary>
    /// VmId
    /// </summary>
    [JsonProperty("vmid")]
    public long Vmid { get; set; }
}