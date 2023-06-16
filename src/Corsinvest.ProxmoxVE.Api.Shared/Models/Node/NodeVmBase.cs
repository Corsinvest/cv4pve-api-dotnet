/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node
{
    /// <summary>
    /// vm base
    /// </summary>
    public abstract class NodeVmBase : IVmBase, ICpu, INetIO, IMemory, IDiskIO, IDisk, IStatusItem, IUptimeItem
    {
        /// <summary>
        /// Uptime
        /// </summary>
        /// <value></value>
        [JsonProperty("uptime")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatUptimeUnixTime + "}")]
        public long Uptime { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

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
        /// Pid
        /// </summary>
        [JsonProperty("pid")]
        public string Pid { get; set; }

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

        [OnDeserialized]
        internal void OnSerializedMethod(StreamingContext context)
        {
            this.ImproveData(Status);
            ((ICpu)this).ImproveData();
            ((IMemory)this).ImproveData();
            ((IDisk)this).ImproveData();
        }
    }
}