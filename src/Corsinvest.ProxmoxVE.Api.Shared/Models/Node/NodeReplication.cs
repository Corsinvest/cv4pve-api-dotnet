/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node
{
    /// <summary>
    /// Node replication
    /// </summary>
    public class NodeReplication
    {
        /// <summary>
        /// Comment
        /// </summary>
        [JsonProperty("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Disable
        /// </summary>
        [JsonProperty("disable")]
        public bool Disable { get; set; }

        /// <summary>
        /// Source
        /// </summary>
        [JsonProperty("source")]
        public string Source { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Vm Type
        /// </summary>
        [JsonProperty("vmtype")]
        public string Vmtype { get; set; }

        /// <summary>
        /// Fail Count
        /// </summary>
        [JsonProperty("fail_count")]
        public int FailCount { get; set; }

        /// <summary>
        /// Last Sync
        /// </summary>
        [JsonProperty("last_sync")]
        public int LastSync { get; set; }

        /// <summary>
        /// Job Num
        /// </summary>
        [JsonProperty("jobnum")]
        public string JobNum { get; set; }

        /// <summary>
        /// Next Sync
        /// </summary>
        [JsonProperty("next_sync")]
        public int NextSync { get; set; }

        /// <summary>
        /// Guest
        /// </summary>
        [JsonProperty("guest")]
        public string Guest { get; set; }

        /// <summary>
        /// Schedule
        /// </summary>
        [JsonProperty("schedule")]
        public string Schedule { get; set; }

        /// <summary>
        /// Duration
        /// </summary>
        [JsonProperty("duration")]
        public double Duration { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Last Try
        /// </summary>
        [JsonProperty("last_try")]
        public int LastTry { get; set; }

        /// <summary>
        /// Target
        /// </summary>
        [JsonProperty("target")]
        public string Target { get; set; }

        /// <summary>
        /// Error
        /// </summary>
        [JsonProperty("error")]
        public string Error { get; set; }

        /// <summary>
        /// Extension Data
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, object> ExtensionData { get; set; }
    }
}