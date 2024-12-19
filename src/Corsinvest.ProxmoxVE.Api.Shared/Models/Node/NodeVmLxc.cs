﻿/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Lxc Data
/// </summary>
public class NodeVmLxc : NodeVmBase
{
    /// <summary>
    /// Swap size
    /// </summary>
    [JsonProperty("maxswap")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public ulong SwapSize { get; set; }

    /// <summary>
    /// Swap
    /// </summary>
    [JsonProperty("swap")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public ulong Swap { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }
}