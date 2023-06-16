/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm
{
    /// <summary>
    /// Vm Qemu Agent GetFsInfo
    /// </summary>
    public class VmQemuAgentGetFsInfo
    {
        /// <summary>
        /// Result
        /// </summary>
        [JsonProperty("result")]
        public IEnumerable<ResultInt> Result { get; set; }

        /// <summary>
        /// Result
        /// </summary>
        public class ResultInt
        {
            /// <summary>
            /// Error
            /// </summary>
            [JsonProperty("error")]
            public object Error { get; set; }

            /// <summary>
            /// Name
            /// </summary>
            [JsonProperty("name")]
            public string Name { get; set; }

            /// <summary>
            /// Type
            /// </summary>
            [JsonProperty("type")]
            public string Type { get; set; }

            /// <summary>
            /// Total Bytes
            /// </summary>
            [JsonProperty("total-bytes")]
            [DisplayFormat(DataFormatString = "{0:" + FormatHelper.FormatBytes + "}")]
            public long TotalBytes { get; set; }

            /// <summary>
            /// Mount point
            /// </summary>
            [JsonProperty("mountpoint")]
            public string MountPoint { get; set; }

            /// <summary>
            /// Used Bytes
            /// </summary>
            [JsonProperty("used-bytes")]
            public long UsedBytes { get; set; }

            /// <summary>
            /// Disks
            /// </summary>
            [JsonProperty("disk")]
            public IEnumerable<Disk> Disks { get; set; }
        }

        /// <summary>
        /// Pci Controller
        /// </summary>
        public class PciController
        {
            /// <summary>
            /// Bus
            /// </summary>
            [JsonProperty("bus")]
            public int Bus { get; set; }

            /// <summary>
            /// Slot
            /// </summary>
            [JsonProperty("slot")]
            public int Slot { get; set; }

            /// <summary>
            /// Domain
            /// </summary>
            [JsonProperty("domain")]
            public int Domain { get; set; }

            /// <summary>
            /// Function
            /// </summary>
            [JsonProperty("function")]
            public int Function { get; set; }
        }

        /// <summary>
        /// Disk
        /// </summary>
        public class Disk
        {
            /// <summary>
            /// Dev
            /// </summary>
            [JsonProperty("dev")]
            public string Dev { get; set; }

            /// <summary>
            /// Pci Controller
            /// </summary>
            [JsonProperty("pci-controller")]
            public PciController PciController { get; set; }

            /// <summary>
            /// Bus
            /// </summary>
            [JsonProperty("bus")]
            public int Bus { get; set; }

            /// <summary>
            /// Bus type
            /// </summary>
            [JsonProperty("bus-type")]
            public string BusType { get; set; }

            /// <summary>
            /// Target
            /// </summary>
            [JsonProperty("target")]
            public int Target { get; set; }

            /// <summary>
            /// Unit
            /// </summary>
            [JsonProperty("unit")]
            public int Unit { get; set; }
        }
    }
}