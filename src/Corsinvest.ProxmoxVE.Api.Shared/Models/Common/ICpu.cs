/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common;

/// <summary>
/// Cpu
/// </summary>
public interface ICpu
{
    /// <summary>
    /// Cpu usage
    /// </summary>
    [Display(Name = "CPU Usage %")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatPercentage)]
    [JsonProperty("cpu")]
    double CpuUsagePercentage { get; set; }

    /// <summary>
    /// Cpu size
    /// </summary>
    [JsonProperty("maxcpu")]
    long CpuSize { get; set; }

    /// <summary>
    /// Cpu info
    /// </summary>
    [Display(Name = "Cpu")]
    string CpuInfo { get; set; }
}