/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

/// <summary>
/// Vm Base Status Current
/// </summary>
public class VmBaseStatusCurrent : ModelBase, IVmBase, INetIO, IDisk, IMemory, ICpu, IDiskIO
{
    /// <summary>
    /// The amount of traffic in bytes that was sent to the guest over the network since it was started.
    /// </summary>
    [JsonProperty("netin")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public long NetIn { get; set; }

    /// <summary>
    /// The amount of traffic in bytes that was sent from the guest over the network since it was started.
    /// </summary>
    [JsonProperty("netout")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public long NetOut { get; set; }

    /// <summary>
    /// Root disk image space-usage in bytes.
    /// </summary>
    [JsonProperty("disk")]
    [Display(Name = "Disk usage")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public ulong DiskUsage { get; set; }

    /// <summary>
    /// Root disk size in bytes.
    /// </summary>
    [JsonProperty("maxdisk")]
    [Display(Name = "Disk size")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public ulong DiskSize { get; set; }

    /// <summary>
    /// Disk usage percentage
    /// </summary>
    /// <value></value>
    [Display(Name = "Disk usage %")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatPercentage)]
    public double DiskUsagePercentage { get; set; }

    /// <summary>
    /// Currently used memory in bytes.
    /// </summary>
    [JsonProperty("mem")]
    [Display(Name = "Memory")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public ulong MemoryUsage { get; set; }

    /// <summary>
    /// Maximum memory in bytes.
    /// </summary>
    [JsonProperty("maxmem")]
    [Display(Name = "Max Memory")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public ulong MemorySize { get; set; }

    /// <summary>
    /// Memory info
    /// </summary>
    /// <value></value>
    [Display(Name = "Memory")]
    public string MemoryInfo { get; set; }

    /// <summary>
    /// Memory usage percentage
    /// </summary>
    /// <value></value>
    [Display(Name = "Memory Usage %")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatPercentage)]
    public double MemoryUsagePercentage { get; set; }

    /// <summary>
    /// Current CPU usage.
    /// </summary>
    [Display(Name = "CPU Usage %")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatPercentage)]
    [JsonProperty("cpu")]
    public double CpuUsagePercentage { get; set; }

    /// <summary>
    /// Maximum usable CPUs.
    /// </summary>
    [JsonProperty("cpus")]
    public long CpuSize { get; set; }

    /// <summary>
    /// Cpu info
    /// </summary>
    /// <value></value>
    [Display(Name = "Cpu")]
    public string CpuInfo { get; set; }

    /// <summary>
    /// VM (host)name.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// The (unique) ID of the VM.
    /// </summary>
    [JsonProperty("vmid")]
    public long VmId { get; set; }

    /// <summary>
    /// The amount of bytes the guest read from it's block devices since the guest was started. (Note: This info is not available for all storage types.)
    /// </summary>
    [JsonProperty("diskread")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public long DiskRead { get; set; }

    /// <summary>
    /// The amount of bytes the guest wrote from it's block devices since the guest was started. (Note: This info is not available for all storage types.)
    /// </summary>
    [JsonProperty("diskwrite")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public long DiskWrite { get; set; }

    /// <summary>
    /// PID of the QEMU process, if the VM is running.
    /// </summary>
    [JsonProperty("pid")]
    public long Pid { get; set; }

    /// <summary>
    /// QEMU process status.
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; }

    /// <summary>
    /// Status Is running
    /// </summary>
    /// <value></value>
    public bool IsRunning { get; set; }

    /// <summary>
    /// Status Is stopped
    /// </summary>
    /// <value></value>
    public bool IsStopped { get; set; }

    /// <summary>
    /// Status Is paused
    /// </summary>
    /// <value></value>
    public bool IsPaused { get; set; }

    /// <summary>
    /// Determines if the guest is a template.
    /// </summary>
    [JsonProperty("template")]
    public bool IsTemplate { get; set; }

    /// <summary>
    /// Uptime in seconds.
    /// </summary>
    [JsonProperty("uptime")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatUptimeUnixTime)]
    public long Uptime { get; set; }

    /// <summary>
    /// HA manager service status.
    /// </summary>
    [JsonProperty("ha")]
    public HaInt Ha { get; set; }

    /// <summary>
    /// Ha
    /// </summary>
    public class HaInt
    {
        /// <summary>
        /// Managed
        /// </summary>
        [JsonProperty("managed")]
        public int Managed { get; set; }
    }

    /// <summary>
    /// The current config lock, if any.
    /// </summary>
    [JsonProperty("lock")]
    public string Lock { get; set; }

    /// <summary>
    /// The current configured tags, if any
    /// </summary>
    [JsonProperty("tags")]
    public string Tags { get; set; }

    /// <summary>
    /// Current memory usage on the host.
    /// </summary>
    [JsonProperty("memhost")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public ulong MemoryHostUsage { get; set; }

    /// <summary>PSI CPU pressure (some) — requires PVE 9.0+.</summary>
    [JsonProperty("pressurecpusome")]
    public double PressureCpuSome { get; set; }

    /// <summary>PSI CPU pressure (full) — requires PVE 9.0+.</summary>
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

    /// <summary>
    /// Serialized Method
    /// </summary>
    protected void OnSerializedMethodBase()
    {
        ((IDisk)this).ImproveData();
        ((IMemory)this).ImproveData();
        ((ICpu)this).ImproveData();
        ((IVmBase)this).ImproveData(Status);
    }
}