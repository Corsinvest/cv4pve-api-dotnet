/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

/// <summary>
/// rrd data structure
/// </summary>
public class VmRrdData : ModelBase, IDisk, INetIO, IDiskIO, ICpu, IMemory, IPressureInfo
{
    /// <summary>
    /// Time unix time
    /// </summary>
    [JsonProperty("time")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatUnixTime)]
    public long Time { get; set; }

    /// <summary>
    /// Time
    /// </summary>
    public DateTime TimeDate => DateTimeOffset.FromUnixTimeSeconds(Time).DateTime;

    /// <summary>
    /// Disk usage
    /// </summary>
    [JsonProperty("disk")]
    [Display(Name = "Disk usage")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public ulong DiskUsage { get; set; }

    /// <summary>
    /// Disk size
    /// </summary>
    [JsonProperty("maxdisk")]
    [Display(Name = "Disk size")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public ulong DiskSize { get; set; }

    /// <summary>
    /// Disk usage percentage
    /// </summary>
    [Display(Name = "Disk usage %")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatPercentage)]
    public double DiskUsagePercentage { get; set; }

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
    /// Disk read
    /// </summary>
    [JsonProperty("diskread")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public long DiskRead { get; set; }

    /// <summary>
    /// Disk write
    /// </summary>
    [JsonProperty("diskwrite")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public long DiskWrite { get; set; }

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
    /// Memory usage
    /// </summary>
    [JsonProperty("mem")]
    [Display(Name = "Memory")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public ulong MemoryUsage { get; set; }

    /// <summary>
    ///Memory size
    /// </summary>
    [JsonProperty("maxmem")]
    [Display(Name = "Max Memory")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public ulong MemorySize { get; set; }

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

    /// <summary>PSI CPU pressure (some) — requires PVE 9.0+.</summary>
    [JsonProperty("pressurecpusome")]
    public double PressureCpuSome { get; set; }

    /// <summary>PSI CPU pressure (full) — requires PVE 9.0+. Only on QEMU.</summary>
    [JsonProperty("pressurecpufull")]
    public double PressureCpuFull { get; set; }

    /// <summary>PSI I/O pressure (some) — requires PVE 9.0+.</summary>
    [JsonProperty("pressureiosome")]
    public double PressureIoSome { get; set; }

    /// <summary>PSI I/O pressure (full) — requires PVE 9.0+.</summary>
    [JsonProperty("pressureiofull")]
    public double PressureIoFull { get; set; }

    /// <summary>PSI memory pressure (some) — requires PVE 9.0+.</summary>
    [JsonProperty("pressurememorysome")]
    public double PressureMemorySome { get; set; }

    /// <summary>PSI memory pressure (full) — requires PVE 9.0+.</summary>
    [JsonProperty("pressurememoryfull")]
    public double PressureMemoryFull { get; set; }

    [OnDeserialized]
    internal void OnSerializedMethod(StreamingContext context)
    {
        ((ICpu)this).EnrichData();
        ((IMemory)this).EnrichData();
        ((IDisk)this).EnrichData();
    }
}