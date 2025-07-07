/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Rrd Data Node
/// </summary>
public class NodeRrdData : ModelBase, ICpu, INetIO, IMemory
{
    /// <summary>
    /// Net in
    /// </summary>
    [JsonProperty("netin")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public long NetIn { get; set; }

    /// <summary>
    /// Net out
    /// </summary>
    [JsonProperty("netout")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public long NetOut { get; set; }

    /// <summary>
    /// Cpu usage
    /// </summary>
    [Display(Name = "CPU Usage %")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatPercentage)]
    [JsonProperty("cpu")]
    public double CpuUsagePercentage { get; set; }

    /// <summary>
    /// Cpu size
    /// </summary>
    [JsonProperty("maxcpu")]
    public long CpuSize { get; set; }

    /// <summary>
    /// Cpu info
    /// </summary>
    [Display(Name = "Cpu")]
    public string CpuInfo { get; set; }

    /// <summary>
    /// Time unix time
    /// </summary>
    [JsonProperty("time")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatUnixTime )]
    long Time { get; set; }

    /// <summary>
    /// Time
    /// </summary>
    public DateTime TimeDate => DateTimeOffset.FromUnixTimeSeconds(Time).DateTime;

    /// <summary>
    /// Load average
    /// </summary>
    [JsonProperty("loadavg")]
    public double Loadavg { get; set; }

    /// <summary>
    /// Io wait
    /// </summary>
    [JsonProperty("iowait")]
    public double IoWait { get; set; }

    /// <summary>
    /// Memory Size
    /// </summary>
    [JsonProperty("memtotal")]
    [Display(Name = "Max Memory")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public ulong MemorySize { get; set; }

    /// <summary>
    /// Memory Used
    /// </summary>
    [JsonProperty("memused")]
    [Display(Name = "Memory")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public ulong MemoryUsage { get; set; }

    /// <summary>
    /// Memory info
    /// </summary>
    [Display(Name = "Memory")]
    public string MemoryInfo { get; set; }

    /// <summary>
    /// Memory usage percentage
    /// </summary>
    [Display(Name = "Memory Usage %")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatPercentage)]
    public double MemoryUsagePercentage { get; set; }

    /// <summary>
    /// Swap size
    /// </summary>
    [JsonProperty("swaptotal")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public ulong SwapSize { get; set; }

    /// <summary>
    /// Swap used
    /// </summary>
    [JsonProperty("swapused")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public double SwapUsage { get; set; }

    /// <summary>
    /// Root size
    /// </summary>
    [JsonProperty("roottotal")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public double RootSize { get; set; }

    /// <summary>
    /// Root used
    /// </summary>
    [JsonProperty("rootused")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public double RootUsage { get; set; }

    [OnDeserialized]
    internal void OnSerializedMethod(StreamingContext context)
    {
        ((ICpu)this).ImproveData();
        ((IMemory)this).ImproveData();
    }
}