/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common
{

    /// <summary>
    /// Disk
    /// </summary>
    public interface IDisk
    {
        /// <summary>
        /// Disk usage
        /// </summary>
        /// <value></value>
        [JsonProperty("disk")]
        [Display(Name = "Disk usage")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatBytes + "}")]
        long DiskUsage { get; set; }

        /// <summary>
        /// Disk size
        /// </summary>
        /// <value></value>
        [JsonProperty("maxdisk")]
        [Display(Name = "Disk size")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatBytes + "}")]
        long DiskSize { get; set; }

        /// <summary>
        /// Disk usage percentage
        /// </summary>
        /// <value></value>
        [Display(Name = "Disk usage %")]
        [DisplayFormat(DataFormatString = "{0:P1}")]
        double DiskUsagePercentage { get; set; }
    }
}