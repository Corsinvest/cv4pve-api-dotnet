/*
 * SPDX-FileCopyrightText: 2022 Daniele Corsini <daniele.corsini@corsinvest.it>
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster
{
    /// <summary>
    /// Cluster backup
    /// </summary>
    public class ClusterBackup : IStorageItem
    {
        /// <summary>
        /// All
        /// </summary>
        [JsonProperty("all")]
        public bool All { get; set; }

        /// <summary>
        /// Mode
        /// </summary>
        [JsonProperty("mode")]
        public string Mode { get; set; }

        /// <summary>
        /// Start time
        /// </summary>
        [JsonProperty("starttime")]
        public string StartTime { get; set; }

        /// <summary>
        /// Storage
        /// </summary>
        [JsonProperty("storage")]
        public string Storage { get; set; }

        /// <summary>
        /// Enabled
        /// </summary>
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Mail to
        /// </summary>
        [JsonProperty("mailto")]
        public string Mailto { get; set; }

        /// <summary>
        /// Pool
        /// </summary>
        [JsonProperty("pool")]
        public string Pool { get; set; }

        /// <summary>
        /// VmId
        /// </summary>
        [JsonProperty("vmid")]
        public string VmId { get; set; }

        /// <summary>
        /// Day of week
        /// </summary>
        [JsonProperty("dow")]
        public string DayOfWeek { get; set; }

        /// <summary>
        /// Mail notification
        /// </summary>
        [JsonProperty("mailnotification")]
        public string MailNotification { get; set; }

        /// <summary>
        /// Quiet
        /// </summary>
        [JsonProperty("quiet")]
        public bool Quiet { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}