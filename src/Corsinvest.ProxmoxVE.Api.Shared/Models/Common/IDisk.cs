/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common;

/// <summary>
/// Disk
/// </summary>
public interface IDisk
{
    /// <summary>
    /// Disk usage
    /// </summary>
    [JsonProperty("disk")]
    [Display(Name = "Disk usage")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    ulong DiskUsage { get; set; }

    /// <summary>
    /// Disk size
    /// </summary>
    [JsonProperty("maxdisk")]
    [Display(Name = "Disk size")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    ulong DiskSize { get; set; }

    /// <summary>
    /// Disk usage percentage
    /// </summary>
    [Display(Name = "Disk usage %")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatPercentage)]
    double DiskUsagePercentage { get; set; }
}