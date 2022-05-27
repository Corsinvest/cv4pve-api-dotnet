/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common
{
    /// <summary>
    /// Firewall rules
    /// </summary>
    public class FirewallRule
    {
        /// <summary>
        /// Protocol
        /// </summary>
        [JsonProperty("proto")]
        public string Protocol { get; set; }

        /// <summary>
        /// Destination
        /// </summary>
        [JsonProperty("dest")]
        public string Dest { get; set; }

        /// <summary>
        /// Destination Port 
        /// </summary>
        [JsonProperty("dport")]
        public string DestinationPort { get; set; }

        /// <summary>
        /// Positon
        /// </summary>
        [JsonProperty("pos")]
        public int Positon { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Action
        /// </summary>
        [JsonProperty("action")]
        public string Action { get; set; }

        /// <summary>
        /// Source
        /// </summary>
        [JsonProperty("source")]
        public string Source { get; set; }

        /// <summary>
        /// Source Port
        /// </summary>
        [JsonProperty("sport")]
        public string SourcePort { get; set; }

        /// <summary>
        /// Log
        /// </summary>
        [JsonProperty("log")]
        public string Log { get; set; }

        /// <summary>
        /// Enabled
        /// </summary>
        [JsonProperty("enable")]
        public bool Enable { get; set; }

        /// <summary>
        /// Comment
        /// </summary>
        [JsonProperty("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Marco
        /// </summary>
        [JsonProperty("marco")]
        public string Marco { get; set; }

        /// <summary>
        /// Digest
        /// </summary>
        [JsonProperty("digest")]
        public string Digest { get; set; }
    }
}