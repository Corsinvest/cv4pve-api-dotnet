/*
 * SPDX-FileCopyrightText: 2022 Daniele Corsini <daniele.corsini@corsinvest.it>
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common
{
    /// <summary>
    /// Cpu
    /// </summary>
    public interface ICpu
    {
        /// <summary>
        /// Cpu usage
        /// </summary>
        /// <value></value>
        [Display(Name = "CPU Usage %")]
        [DisplayFormat(DataFormatString = "{0:P1}")]
        [JsonProperty("cpu")]
        double CpuUsagePercentage { get; set; }

        /// <summary>
        /// Cpu size
        /// </summary>
        /// <value></value>
        [JsonProperty("maxcpu")]
        long CpuSize { get; set; }

        /// <summary>
        /// Cpu info
        /// </summary>
        /// <value></value>
        [Display(Name = "Cpu")]
        string CpuInfo { get; set; }
    }
}