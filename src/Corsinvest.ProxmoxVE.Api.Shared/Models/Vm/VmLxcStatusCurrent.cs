/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

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
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public ulong SwapUsage { get; set; }

    /// <summary>
    /// Swap
    /// </summary>
    [JsonProperty("maxswap")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public ulong SwapSize { get; set; }

    /// <summary>
    /// Swap usage percentage
    /// </summary>
    /// <value></value>
    [Display(Name = "Disk usage %")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatPercentage)]
    public double SwapUsagePercentage => FormatHelper.CalculatePercentage(SwapUsage, SwapSize);

    [OnDeserialized]
    internal void OnSerializedMethod(StreamingContext context) => OnSerializedMethodBase();
}