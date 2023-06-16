/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm
{
    /// <summary>
    /// Vm Qemu Status Current
    /// </summary>
    public class VmQemuStatusCurrent : VmBaseStatusCurrent
    {
        /// <summary>
        /// Spice
        /// </summary>
        [JsonProperty("spice")]
        public bool Spice { get; set; }

        /// <summary>
        /// Agent
        /// </summary>
        [JsonProperty("agent")]
        public bool Agent { get; set; }

        /// <summary>
        /// Running Qemu
        /// </summary>
        [JsonProperty("running-qemu")]
        public string RunningQemu { get; set; }

        /// <summary>
        /// Running Machine
        /// </summary>
        [JsonProperty("running-machine")]
        public string RunningMachine { get; set; }

        /// <summary>
        /// Qmp status
        /// </summary>
        [JsonProperty("qmpstatus")]
        public string Qmpstatus { get; set; }

        /// <summary>
        /// Nigs
        /// </summary>
        [JsonProperty("nics")]
        public IDictionary<string, NicsInt> Nics { get; set; }

        /// <summary>
        /// Block Stat
        /// </summary>
        [JsonProperty("blockstat")]
        public IDictionary<string, BlockstatInt> BlockStat { get; set; }

        /// <summary>
        /// Balloon
        /// </summary>
        [JsonProperty("balloon")]
        public long Balloon { get; set; }

        /// <summary>
        /// Balloon info
        /// </summary>
        [JsonProperty("ballooninfo")]
        public BalloonInfoInt Ballooninfo { get; set; }

        /// <summary>
        /// Proxmox Support
        /// </summary>
        [JsonProperty("proxmox-support")]
        public ProxmoxSupportInt ProxmoxSupport { get; set; }

        [OnDeserialized]
        internal void OnSerializedMethod(StreamingContext context) => OnSerializedMethodBase();

        /// <summary>
        /// Nics
        /// </summary>
        public class NicsInt : INetIO
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
        }

        /// <summary>
        /// Block stat
        /// </summary>
        public class BlockstatInt
        {
            /// <summary>
            /// Unmap Bytes
            /// </summary>
            [JsonProperty("unmap_bytes")]
            public long UnmapBytes { get; set; }

            /// <summary>
            /// Flush Operations
            /// </summary>
            [JsonProperty("flush_operations")]
            public long FlushOperations { get; set; }

            /// <summary>
            /// Account Invalid
            /// </summary>
            [JsonProperty("account_invalid")]
            public bool AccountInvalid { get; set; }

            /// <summary>
            /// Account Failed
            /// </summary>
            [JsonProperty("account_failed")]
            public bool AccountFailed { get; set; }

            /// <summary>
            /// Flush Total Time Ns
            /// </summary>
            [JsonProperty("flush_total_time_ns")]
            public long FlushTotalTimeNs { get; set; }

            /// <summary>
            /// Failed Wr Operations
            /// </summary>
            [JsonProperty("failed_wr_operations")]
            public long FailedWrOperations { get; set; }

            /// <summary>
            /// Wr Merged
            /// </summary>
            [JsonProperty("wr_merged")]
            public long WrMerged { get; set; }

            /// <summary>
            /// Rd Total Time Ns
            /// </summary>
            [JsonProperty("rd_total_time_ns")]
            public long RdTotalTimeNs { get; set; }

            /// <summary>
            /// Rd Operations
            /// </summary>
            [JsonProperty("rd_operations")]
            public long RdOperations { get; set; }

            /// <summary>
            /// Rd Bytes
            /// </summary>
            [JsonProperty("rd_bytes")]
            public long RdBytes { get; set; }

            /// <summary>
            /// Invalid Rd Operations
            /// </summary>
            [JsonProperty("invalid_rd_operations")]
            public long InvalidRdOperations { get; set; }

            /// <summary>
            /// Failed Flush Operations
            /// </summary>
            [JsonProperty("failed_flush_operations")]
            public long FailedFlushOperations { get; set; }

            /// <summary>
            /// Failed Rd Operations
            /// </summary>
            [JsonProperty("failed_rd_operations")]
            public long FailedRdOperations { get; set; }

            /// <summary>
            /// Unmap Total Time Ns
            /// </summary>
            [JsonProperty("unmap_total_time_ns")]
            public long UnmapTotalTimeNs { get; set; }

            /// <summary>
            /// Timed Stats
            /// </summary>
            [JsonProperty("timed_stats")]
            public IEnumerable<object> TimedStats { get; set; }

            /// <summary>
            /// Invalid Flush Operations
            /// </summary>
            [JsonProperty("invalid_flush_operations")]
            public long InvalidFlushOperations { get; set; }

            /// <summary>
            /// Unmap Operations
            /// </summary>
            [JsonProperty("unmap_operations")]
            public long UnmapOperations { get; set; }

            /// <summary>
            /// Invalid Unmap Operations
            /// </summary>
            [JsonProperty("invalid_unmap_operations")]
            public long InvalidUnmapOperations { get; set; }

            /// <summary>
            /// Invalid Wr Operations
            /// </summary>
            [JsonProperty("invalid_wr_operations")]
            public long InvalidWrOperations { get; set; }

            /// <summary>
            /// Wr Highest Offset
            /// </summary>
            [JsonProperty("wr_highest_offset")]
            public long WrHighestOffset { get; set; }

            /// <summary>
            /// Unmap Merged
            /// </summary>
            [JsonProperty("unmap_merged")]
            public long UnmapMerged { get; set; }

            /// <summary>
            /// Rd Merged
            /// </summary>
            [JsonProperty("rd_merged")]
            public long RdMerged { get; set; }

            /// <summary>
            /// Wr Operations
            /// </summary>
            [JsonProperty("wr_operations")]
            public long WrOperations { get; set; }

            /// <summary>
            /// Wr Total TimeNs
            /// </summary>
            [JsonProperty("wr_total_time_ns")]
            public long WrTotalTimeNs { get; set; }

            /// <summary>
            /// Wr Bytes
            /// </summary>
            [JsonProperty("wr_bytes")]
            public long WrBytes { get; set; }

            /// <summary>
            /// Failed Unmap Operations
            /// </summary>
            [JsonProperty("failed_unmap_operations")]
            public long FailedUnmapOperations { get; set; }
        }

        /// <summary>
        /// Balloon info
        /// </summary>
        public class BalloonInfoInt
        {
            /// <summary>
            /// Actual
            /// </summary>
            [JsonProperty("actual")]
            public long Actual { get; set; }

            /// <summary>
            /// MaxMem
            /// </summary>
            [JsonProperty("max_mem")]
            public long MaxMem { get; set; }

            /// <summary>
            /// LastUpdate
            /// </summary>
            [JsonProperty("last_update")]
            public long LastUpdate { get; set; }
        }

        /// <summary>
        /// Proxmox Support
        /// </summary>
        public class ProxmoxSupportInt
        {
            /// <summary>
            /// Pbs Dirty Bitmap Save Vm
            /// </summary>
            [JsonProperty("pbs-dirty-bitmap-savevm")]
            public bool PbsDirtyBitmapSaveVm { get; set; }

            /// <summary>
            /// Pbs Master Key
            /// </summary>
            [JsonProperty("pbs-masterkey")]
            public bool PbsMasterKey { get; set; }

            /// <summary>
            /// Query Bitmap Info
            /// </summary>
            [JsonProperty("query-bitmap-info")]
            public bool QueryBitmapInfo { get; set; }

            /// <summary>
            /// Pbs Dirty Bitmap Migration
            /// </summary>
            [JsonProperty("pbs-dirty-bitmap-migration")]
            public bool PbsDirtyBitmapMigration { get; set; }

            /// <summary>
            /// Pbs Dirty Bitmap
            /// </summary>
            [JsonProperty("pbs-dirty-bitmap")]
            public bool PbsDirtyBitmap { get; set; }

            /// <summary>
            /// Pbs Library Version
            /// </summary>
            [JsonProperty("pbs-library-version")]
            public string PbsLibraryVersion { get; set; }
        }

    }
}