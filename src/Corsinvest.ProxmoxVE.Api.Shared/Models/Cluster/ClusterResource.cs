/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Node;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Resources
/// </summary>
public class ClusterResource : ModelBase, IClusterResourceNode, IClusterResourceVm, IClusterResourceStorage
{
    /// <summary>
    /// Node Level
    /// </summary>
    public NodeLevel NodeLevel { get; set; }

    /// <summary>
    /// The pool name (for types 'pool', 'qemu' and 'lxc').
    /// </summary>
    [JsonProperty("pool")]
    public string Pool { get; set; }

    /// <summary>
    /// The amount of traffic in bytes that was sent to the guest over the network since it was started. (for types 'qemu' and 'lxc')
    /// </summary>
    [JsonProperty("netin")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public long NetIn { get; set; }

    /// <summary>
    /// The amount of traffic in bytes that was sent from the guest over the network since it was started. (for types 'qemu' and 'lxc')
    /// </summary>
    [JsonProperty("netout")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public long NetOut { get; set; }

    /// <summary>
    /// Name of the resource.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// The numerical vmid (for types 'qemu' and 'lxc').
    /// </summary>
    [JsonProperty("vmid")]
    public long VmId { get; set; }

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
    /// Determines if the guest is a template. (for types 'qemu' and 'lxc')
    /// </summary>
    [JsonProperty("template")]
    public bool IsTemplate { get; set; }

    /// <summary>
    /// The number of bytes the guest read from its block devices since the guest was started. This info is not available for all storage types. (for types 'qemu' and 'lxc')
    /// </summary>
    [JsonProperty("diskread")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public long DiskRead { get; set; }

    /// <summary>
    /// The number of bytes the guest wrote to its block devices since the guest was started. This info is not available for all storage types. (for types 'qemu' and 'lxc')
    /// </summary>
    [JsonProperty("diskwrite")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public long DiskWrite { get; set; }

    /// <summary>
    /// Vm type
    /// </summary>
    /// <value></value>
    public VmType VmType { get; set; }

    /// <summary>
    /// The guest's current config lock (for types 'qemu' and 'lxc')
    /// </summary>
    [JsonProperty("lock")]
    public string Lock { get; set; }

    /// <summary>
    /// Is Locked
    /// </summary>
    public bool IsLocked { get; set; }

    /// <summary>
    /// Host cpu usage
    /// </summary>
    /// <value></value>
    [Display(Name = "Host Cpu Usage")]
    public string HostCpuUsage { get; set; }

    /// <summary>
    /// Host memory usage
    /// </summary>
    /// <value></value>
    [Display(Name = "Host Memory Usage %")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatPercentage)]
    public double HostMemoryUsage { get; set; }

    /// <summary>
    /// Node Cpu Assigned
    /// </summary>
    public long NodeCpuAssigned { get; set; }

    /// <summary>
    /// Node Memory Assigned
    /// </summary>
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public ulong NodeMemoryAssigned { get; set; }

    /// <summary>
    /// The storage identifier (for type 'storage').
    /// </summary>
    [JsonProperty("storage")]
    public string Storage { get; set; }

    /// <summary>
    /// Shared storage
    /// </summary>
    [JsonProperty("shared")]
    public bool Shared { get; set; }

    /// <summary>
    /// More specific type, if available.
    /// </summary>
    [JsonProperty("plugintype")]
    public string PluginType { get; set; }

    /// <summary>
    /// Allowed storage content types (for type 'storage').
    /// </summary>
    [JsonProperty("content")]
    public string Content { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Resource id.
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Resource type.
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Resource Type
    /// </summary>
    /// <value></value>
    public ClusterResourceType ResourceType { get; set; }

    /// <summary>
    /// Resource type dependent status.
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; }

    /// <summary>
    /// Status Is Unknown
    /// </summary>
    /// <value></value>
    public bool IsUnknown { get; set; }

    /// <summary>
    /// Used disk space in bytes (for type 'storage'), used root image space for VMs (for types 'qemu' and 'lxc').
    /// </summary>
    [JsonProperty("disk")]
    [Display(Name = "Disk usage")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public ulong DiskUsage { get; set; }

    /// <summary>
    /// Storage size in bytes (for type 'storage'), root image size for VMs (for types 'qemu' and 'lxc').
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
    /// The cluster node name (for types 'node', 'storage', 'qemu', and 'lxc').
    /// </summary>
    [JsonProperty("node")]
    public string Node { get; set; }

    /// <summary>
    /// Used memory in bytes (for types 'node', 'qemu' and 'lxc').
    /// </summary>
    [JsonProperty("mem")]
    [Display(Name = "Memory")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public ulong MemoryUsage { get; set; }

    /// <summary>
    /// Number of available memory in bytes (for types 'node', 'qemu' and 'lxc').
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
    /// CPU utilization (for types 'node', 'qemu' and 'lxc').
    /// </summary>
    [Display(Name = "CPU Usage %")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatPercentage)]
    [JsonProperty("cpu")]
    public double CpuUsagePercentage { get; set; }

    /// <summary>
    /// Number of available CPUs (for types 'node', 'qemu' and 'lxc').
    /// </summary>
    [JsonProperty("maxcpu")]
    public long CpuSize { get; set; }

    /// <summary>
    /// Cpu info
    /// </summary>
    /// <value></value>
    [Display(Name = "Cpu")]
    public string CpuInfo { get; set; }

    /// <summary>
    /// Uptime of node or virtual guest in seconds (for types 'node', 'qemu' and 'lxc').
    /// </summary>
    [JsonProperty("uptime")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatUptimeUnixTime)]
    public long Uptime { get; set; }

    /// <summary>
    /// The cgroup mode the node operates under (for type 'node').
    /// </summary>
    [Display(Name = "Cgroup mode")]
    [JsonProperty("cgroup-mode")]
    public int CgroupMode { get; set; }

    /// <summary>
    /// Support level (for type 'node').
    /// </summary>
    [JsonProperty("level")]
    public string Level { get; set; }

    /// <summary>
    /// Is online
    /// </summary>
    /// <value></value>
    public bool IsOnline { get; set; }

    /// <summary>
    /// Is Available
    /// </summary>
    public bool IsAvailable { get; set; }

    /// <summary>
    /// The guest's tags (for types 'qemu' and 'lxc')
    /// </summary>
    [JsonProperty("tags")]
    public string Tags { get; set; }

    /// <summary>
    /// HA service status (for HA managed VMs).
    /// </summary>
    [JsonProperty("hastate")]
    public string HaState { get; set; }

    /// <summary>
    /// Used memory in bytes from the point of view of the host (for types 'qemu').
    /// </summary>
    [JsonProperty("memhost")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public ulong MemoryHostUsage { get; set; }

    /// <summary>
    /// The name of a Network entity (for type 'network').
    /// </summary>
    [JsonProperty("network")]
    public string Network { get; set; }

    /// <summary>
    /// The type of network resource (for type 'network').
    /// </summary>
    [JsonProperty("network-type")]
    public string NetworkType { get; set; }

    /// <summary>
    /// The protocol of a fabric (for type 'network', network-type 'fabric').
    /// </summary>
    [JsonProperty("protocol")]
    public string Protocol { get; set; }

    /// <summary>
    /// The name of an SDN entity (for type 'sdn')
    /// </summary>
    [JsonProperty("sdn")]
    public string Sdn { get; set; }

    /// <summary>
    /// The type of an SDN zone (for type 'sdn').
    /// </summary>
    [JsonProperty("zone-type")]
    public string ZoneType { get; set; }

    [OnDeserialized]
    internal void OnSerializedMethod(StreamingContext context) => this.ImproveData();
}