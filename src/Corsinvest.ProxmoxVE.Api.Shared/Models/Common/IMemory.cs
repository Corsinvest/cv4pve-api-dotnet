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
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatBytes + "}")]
        long MemoryUsage { get; set; }

        /// <summary>
        ///Memory size
        /// </summary>
        /// <value></value>
        [JsonProperty("maxmem")]
        [Display(Name = "Max Memory")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatBytes + "}")]
        long MemorySize { get; set; }

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
        [DisplayFormat(DataFormatString = "{0:P1}")]
        double MemoryUsagePercentage { get; set; }
    }
}