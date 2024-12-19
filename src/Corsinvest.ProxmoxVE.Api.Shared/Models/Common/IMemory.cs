/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common;

/// <summary>
/// memory
/// </summary>
public interface IMemory
{
    /// <summary>
    /// Memory usage
    /// </summary>
    /// <value></value>
    [JsonProperty("mem")]
    [Display(Name = "Memory")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    ulong MemoryUsage { get; set; }

    /// <summary>
    ///Memory size
    /// </summary>
    /// <value></value>
    [JsonProperty("maxmem")]
    [Display(Name = "Max Memory")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    ulong MemorySize { get; set; }

    /// <summary>
    /// Memory info
    /// </summary>
    /// <value></value>
    [Display(Name = "Memory")]
    string MemoryInfo { get; set; }

    /// <summary>
    /// Memory usage percentage
    /// </summary>
    /// <value></value>
    [Display(Name = "Memory Usage %")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatPercentage)]
    double MemoryUsagePercentage { get; set; }
}