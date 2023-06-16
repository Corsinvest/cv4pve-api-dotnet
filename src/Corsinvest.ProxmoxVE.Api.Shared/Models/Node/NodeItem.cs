/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node
{
    /// <summary>
    /// Node
    /// </summary>
    public class NodeItem : IClusterResourceNode
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
        /// Ssl Fingerprint
        /// </summary>
        /// <value></value>
        [JsonProperty("ssl_fingerprint")]
        public string SslFingerprint { get; set; }

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

        [OnDeserialized]
        internal void OnSerializedMethod(StreamingContext context) => ((IClusterResourceNode)this).ImproveData();
    }
}