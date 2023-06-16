/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node
{
    /// <summary>
    /// Rrd Data Node
    /// </summary>
    public class NodeRrdData : ICpu, INetIO, IMemory
    {
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
        /// Time unix time
        /// </summary>
        /// <value></value>
        [JsonProperty("time")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatUnixTime + "}")]
        long Time { get; set; }

        /// <summary>
        /// Time
        /// </summary>
        public DateTime TimeDate => DateTimeOffset.FromUnixTimeSeconds(Time).DateTime;

        /// <summary>
        /// Load average
        /// </summary>
        /// <value></value>
        [JsonProperty("loadavg")]
        public double Loadavg { get; set; }

        /// <summary>
        /// Io wait
        /// </summary>
        /// <value></value>
        [JsonProperty("iowait")]
        public double IoWait { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        [JsonProperty("memtotal")]
        [Display(Name = "Max Memory")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatBytes + "}")]
        public long MemorySize { get; set; }

        /// <summary>
        /// Memory Used
        /// </summary>
        /// <value></value>
        [JsonProperty("memused")]
        [Display(Name = "Memory")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatBytes + "}")]
        public long MemoryUsage { get; set; }

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
        /// Swap size
        /// </summary>
        /// <value></value>
        [JsonProperty("swaptotal")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatBytes + "}")]
        public long SwapSize { get; set; }

        /// <summary>
        /// Swap used
        /// </summary>
        /// <value></value>
        [JsonProperty("swapused")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatBytes + "}")]
        public double SwapUsage { get; set; }

        /// <summary>
        /// Root size
        /// </summary>
        /// <value></value>
        [JsonProperty("roottotal")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatBytes + "}")]
        public double RootSize { get; set; }

        /// <summary>
        /// Root used
        /// </summary>
        /// <value></value>
        [JsonProperty("rootused")]
        [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatBytes + "}")]
        public double RootUsage { get; set; }

        [OnDeserialized]
        internal void OnSerializedMethod(StreamingContext context)
        {
            ((ICpu)this).ImproveData();
            ((IMemory)this).ImproveData();
        }
    }
}