/*
 * SPDX-FileCopyrightText: 2022 Daniele Corsini <daniele.corsini@corsinvest.it>
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm
{
    /// <summary>
    /// Vm Qemu Status Current
    /// </summary>
    public class VmLxcStatusCurrent : VmBaseStatusCurrent
    {
        /// <summary>
        /// Type
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Swap
        /// </summary>
        [JsonProperty("swap")]
        [DisplayFormat(DataFormatString = FormatHelper.FormatBytes)]
        public int SwapUsage { get; set; }

        /// <summary>
        /// Swap
        /// </summary>
        [JsonProperty("maxswap")]
        [DisplayFormat(DataFormatString = FormatHelper.FormatBytes)]
        public int SwapSize { get; set; }

        /// <summary>
        /// Swap usage percentage
        /// </summary>
        /// <value></value>
        [Display(Name = "Disk usage %")]
        [DisplayFormat(DataFormatString = "{0:P1}")]
        public double SwapUsagePercentage => FormatHelper.CalculatePercentage(SwapUsage, SwapSize);

        [OnDeserialized]
        internal void OnSerializedMethod(StreamingContext context) => OnSerializedMethodBase();
    }
}