/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node
/// </summary>
public class NodeItem : ModelBase, IClusterResourceNode
{
    /// <summary>
    /// Node Level
    /// </summary>
    public NodeLevel NodeLevel { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Id
    /// </summary>
    /// <value></value>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    /// <value></value>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Resource Type
    /// </summary>
    /// <value></value>
    public ClusterResourceType ResourceType { get; set; }

    /// <summary>
    /// Node status.
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; }

    /// <summary>
    /// Status Is Unknown
    /// </summary>
    /// <value></value>
    public bool IsUnknown { get; set; }

    /// <summary>
    /// Disk usage
    /// </summary>
    /// <value></value>
    [JsonProperty("disk")]
    [Display(Name = "Disk usage")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public ulong DiskUsage { get; set; }

    /// <summary>
    /// Disk size
    /// </summary>
    /// <value></value>
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
    /// The cluster node name.
    /// </summary>
    [JsonProperty("node")]
    public string Node { get; set; }

    /// <summary>
    /// Used memory in bytes.
    /// </summary>
    [JsonProperty("mem")]
    [Display(Name = "Memory")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
    public ulong MemoryUsage { get; set; }

    /// <summary>
    /// Number of available memory in bytes.
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
    /// CPU utilization.
    /// </summary>
    [Display(Name = "CPU Usage %")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatPercentage)]
    [JsonProperty("cpu")]
    public double CpuUsagePercentage { get; set; }

    /// <summary>
    /// Number of available CPUs.
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
    /// Node uptime in seconds.
    /// </summary>
    [JsonProperty("uptime")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatUptimeUnixTime)]
    public long Uptime { get; set; }

    /// <summary>
    /// The SSL fingerprint for the node certificate.
    /// </summary>
    [JsonProperty("ssl_fingerprint")]
    public string SslFingerprint { get; set; }

    /// <summary>
    /// Support level.
    /// </summary>
    [JsonProperty("level")]
    public string Level { get; set; }

    /// <summary>
    /// Is online
    /// </summary>
    /// <value></value>
    public bool IsOnline { get; set; }

    [OnDeserialized]
    internal void OnSerializedMethod(StreamingContext context) => ((IClusterResourceNode)this).ImproveData();
}