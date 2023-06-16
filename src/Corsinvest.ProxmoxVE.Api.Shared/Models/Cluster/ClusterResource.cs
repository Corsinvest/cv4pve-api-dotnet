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

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster
{
    /// <summary>
    /// Resources
    /// </summary>
    public class ClusterResource : IClusterResourceNode, IClusterResourceVm, IClusterResourceStorage
    {
        /// <summary>
        /// Node Level
        /// </summary>
        public NodeLevel NodeLevel { get; set; }

        /// <summary>
        /// Pool
        /// </summary>
        /// <value></value>
        [JsonProperty("pool")]
        public string Pool { get; set; }

        /// <summary>
        /// Net in
        /// </summary>
        /// <value></value>
        [JsonProperty("netin")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatBytes + "}")]
        public long NetIn { get; set; }

        /// <summary>
        /// Net out
        /// </summary>
        /// <value></value>
        [JsonProperty("netout")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatBytes + "}")]
        public long NetOut { get; set; }

        /// <summary>
        /// Vm name
        /// </summary>
        /// <value></value>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Vm Id
        /// </summary>
        /// <value></value>
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
        /// Is template
        /// </summary>
        /// <value></value>
        [JsonProperty("template")]
        public bool IsTemplate { get; set; }

        /// <summary>
        /// Disk read
        /// </summary>
        /// <value></value>
        [JsonProperty("diskread")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatBytes + "}")]
        public long DiskRead { get; set; }

        /// <summary>
        /// Disk write
        /// </summary>
        /// <value></value>
        [JsonProperty("diskwrite")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatBytes + "}")]
        public long DiskWrite { get; set; }

        /// <summary>
        /// Vm type
        /// </summary>
        /// <value></value>
        public VmType VmType { get; set; }

        /// <summary>
        /// Vm is lock
        /// </summary>
        /// <value></value>
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
        [DisplayFormat(DataFormatString = "{0:P1}")]
        public double HostMemoryUsage { get; set; }

        /// <summary>
        /// Storage
        /// </summary>
        [JsonProperty("storage")]
        public string Storage { get; set; }


        /// <summary>
        /// Shared storage
        /// </summary>
        [JsonProperty("shared")]
        public bool Shared { get; set; }

        /// <summary>
        /// Plugin Type
        /// </summary>
        [JsonProperty("plugintype")]
        public string PluginType { get; set; }

        /// <summary>
        /// Content
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }

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
        /// Status
        /// </summary>
        /// <value></value>
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
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatBytes + "}")]
        public long DiskUsage { get; set; }

        /// <summary>
        /// Disk size
        /// </summary>
        /// <value></value>
        [JsonProperty("maxdisk")]
        [Display(Name = "Disk size")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatBytes + "}")]
        public long DiskSize { get; set; }

        /// <summary>
        /// Disk usage percentage
        /// </summary>
        /// <value></value>
        [Display(Name = "Disk usage %")]
        [DisplayFormat(DataFormatString = "{0:P1}")]
        public double DiskUsagePercentage { get; set; }

        /// <summary>
        /// Node
        /// </summary>
        [JsonProperty("node")]
        public string Node { get; set; }

        /// <summary>
        /// Memory usage
        /// </summary>
        /// <value></value>
        [JsonProperty("mem")]
        [Display(Name = "Memory")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatBytes + "}")]
        public long MemoryUsage { get; set; }

        /// <summary>
        ///Memory size
        /// </summary>
        /// <value></value>
        [JsonProperty("maxmem")]
        [Display(Name = "Max Memory")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatBytes + "}")]
        public long MemorySize { get; set; }

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
        [DisplayFormat(DataFormatString = "{0:P1}")]
        public double MemoryUsagePercentage { get; set; }

        /// <summary>
        /// Cpu usage
        /// </summary>
        /// <value></value>
        [Display(Name = "CPU Usage %")]
        [DisplayFormat(DataFormatString = "{0:P1}")]
        [JsonProperty("cpu")]
        public double CpuUsagePercentage { get; set; }

        /// <summary>
        /// Cpu size
        /// </summary>
        /// <value></value>
        [JsonProperty("maxcpu")]
        public long CpuSize { get; set; }

        /// <summary>
        /// Cpu info
        /// </summary>
        /// <value></value>
        [Display(Name = "Cpu")]
        public string CpuInfo { get; set; }

        /// <summary>
        /// Uptime
        /// </summary>
        /// <value></value>
        [JsonProperty("uptime")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatUptimeUnixTime + "}")]
        public long Uptime { get; set; }

        /// <summary>
        /// Level
        /// </summary>
        /// <value></value>
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
        /// Tags
        /// </summary>
        /// <value></value>
        [JsonProperty("tags")]
        public string Tags { get; set; }

        [OnDeserialized]
        internal void OnSerializedMethod(StreamingContext context) => this.ImproveData();
    }
}