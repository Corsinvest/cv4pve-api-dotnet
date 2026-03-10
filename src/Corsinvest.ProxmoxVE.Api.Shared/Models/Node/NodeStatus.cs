/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node status
/// </summary>
public class NodeStatus : ModelBase
{
    /// <summary>
    /// The PVE version string.
    /// </summary>
    [JsonProperty("pveversion")]
    public string PveVersion { get; set; }

    /// <summary>
    /// Swap
    /// </summary>
    [JsonProperty("swap")]
    public NodeStatusSwap Swap { get; set; }

    /// <summary>
    /// Cpu Info
    /// </summary>
    [JsonProperty("cpuinfo")]
    public NodeStatusCpuInfo CpuInfo { get; set; }

    /// <summary>
    /// K version
    /// </summary>
    [JsonProperty("kversion")]
    public string Kversion { get; set; }

    /// <summary>
    /// Memory
    /// </summary>
    [JsonProperty("memory")]
    public NodeStatusMemory Memory { get; set; }

    /// <summary>
    /// Ksm
    /// </summary>
    [JsonProperty("ksm")]
    public NodeStatusKsm Ksm { get; set; }

    /// <summary>
    /// Wait
    /// </summary>
    [JsonProperty("wait")]
    public double Wait { get; set; }

    /// <summary>
    /// Uptime
    /// </summary>
    [JsonProperty("uptime")]
    [DisplayFormat(DataFormatString = FormatHelper.DataFormatUptimeUnixTime)]
    public int Uptime { get; set; }

    /// <summary>
    /// An array of load avg for 1, 5 and 15 minutes respectively.
    /// </summary>
    [JsonProperty("loadavg")]
    public IEnumerable<string> LoadAvg { get; set; } = [];

    /// <summary>
    /// Root fs
    /// </summary>
    [JsonProperty("rootfs")]
    public NodeStatusRootFs RootFs { get; set; }

    /// <summary>
    /// The current cpu usage.
    /// </summary>
    [JsonProperty("cpu")]
    public double Cpu { get; set; }

    /// <summary>
    /// Idle
    /// </summary>
    [JsonProperty("idle")]
    public int Idle { get; set; }

    /// <summary>
    /// Meta-information about the boot mode.
    /// </summary>
    [JsonProperty("boot-info")]
    public NodeStatusBootInfo BootInfo { get; set; }

    /// <summary>
    /// Meta-information about the currently booted kernel of this node.
    /// </summary>
    [JsonProperty("current-kernel")]
    public NodeStatusCurrentKernel CurrentKernel { get; set; }

    /// <summary>
    /// Boot info
    /// </summary>
    public class NodeStatusBootInfo
    {
        /// <summary>
        /// Mode — Boot mode: legacy BIOS or UEFI.
        /// </summary>
        [JsonProperty("mode")]
        public string Mode { get; set; }

        /// <summary>
        /// Secureboot — Whether Secure Boot is enabled.
        /// </summary>
        [JsonProperty("secureboot")]
        public bool Secureboot { get; set; }
    }

    /// <summary>
    /// Current kernel info
    /// </summary>
    public class NodeStatusCurrentKernel
    {
        /// <summary>
        /// Machine — Hardware (machine) type.
        /// </summary>
        [JsonProperty("machine")]
        public string Machine { get; set; }

        /// <summary>
        /// Release — OS kernel release (e.g. "6.8.12-8-pve").
        /// </summary>
        [JsonProperty("release")]
        public string Release { get; set; }

        /// <summary>
        /// Sysname — OS kernel name (e.g. "Linux").
        /// </summary>
        [JsonProperty("sysname")]
        public string Sysname { get; set; }

        /// <summary>
        /// Version — OS kernel version.
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }
    }

    /// <summary>
    /// Node status Swap
    /// </summary>
    public class NodeStatusSwap
    {
        /// <summary>
        /// Free
        /// </summary>
        [JsonProperty("free")]
        [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
        public long Free { get; set; }

        /// <summary>
        /// Used
        /// </summary>
        [JsonProperty("used")]
        [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
        public long Used { get; set; }

        /// <summary>
        /// Total
        /// </summary>
        [JsonProperty("total")]
        [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
        public long Total { get; set; }
    }

    /// <summary>
    /// Node status Cpu Info
    /// </summary>
    public class NodeStatusCpuInfo
    {
        /// <summary>
        /// Models
        /// </summary>
        [JsonProperty("model")]
        public string Model { get; set; }

        /// <summary>
        /// Cpus
        /// </summary>
        [JsonProperty("cpus")]
        public int Cpus { get; set; }

        /// <summary>
        /// Hvm
        /// </summary>
        [JsonProperty("hvm")]
        public string Hvm { get; set; }

        /// <summary>
        /// UserHz
        /// </summary>
        [JsonProperty("user_hz")]
        public int UserHz { get; set; }

        /// <summary>
        /// Flags
        /// </summary>
        [JsonProperty("flags")]
        public string Flags { get; set; }

        /// <summary>
        /// Cores
        /// </summary>
        [JsonProperty("cores")]
        public int Cores { get; set; }

        /// <summary>
        /// Sockets
        /// </summary>
        [JsonProperty("sockets")]
        public int Sockets { get; set; }

        /// <summary>
        /// Mhz
        /// </summary>
        [JsonProperty("mhz")]
        public string Mhz { get; set; }
    }

    /// <summary>
    /// Node status Memory
    /// </summary>
    public class NodeStatusMemory
    {
        /// <summary>
        /// Free
        /// </summary>
        [JsonProperty("free")]
        [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
        public long Free { get; set; }

        /// <summary>
        /// Used
        /// </summary>
        [JsonProperty("used")]
        [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
        public long Used { get; set; }

        /// <summary>
        /// Total
        /// </summary>
        [JsonProperty("total")]
        [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
        public long Total { get; set; }
    }

    /// <summary>
    /// Ksm
    /// </summary>
    public class NodeStatusKsm
    {
        /// <summary>
        /// Shared
        /// </summary>
        [JsonProperty("shared")]
        public long Shared { get; set; }
    }

    /// <summary>
    /// Root Fs
    /// </summary>
    public class NodeStatusRootFs
    {
        /// <summary>
        /// Available
        /// </summary>
        [JsonProperty("avail")]
        [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
        public long Available { get; set; }

        /// <summary>
        /// Total
        /// </summary>
        [JsonProperty("total")]
        [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
        public long Total { get; set; }

        /// <summary>
        /// Used
        /// </summary>
        [JsonProperty("used")]
        [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
        public long Used { get; set; }

        /// <summary>
        /// Free
        /// </summary>
        [JsonProperty("free")]
        [DisplayFormat(DataFormatString = FormatHelper.DataFormatBytes)]
        public long Free { get; set; }
    }
}