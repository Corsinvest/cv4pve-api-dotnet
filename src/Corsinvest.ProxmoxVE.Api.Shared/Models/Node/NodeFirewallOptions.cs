/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node
{
    /// <summary>
    /// Node Apt Update
    /// </summary>
    public class NodeFirewallOptions
    {
        /// <summary>
        /// Smurf Log Level
        /// </summary>
        [JsonProperty("smurf_log_level")]
        public string SmurfLogLevel { get; set; }

        /// <summary>
        /// Digest
        /// </summary>
        [JsonProperty("digest")]
        public string Digest { get; set; }

        /// <summary>
        /// Ndp
        /// </summary>
        [JsonProperty("ndp")]
        public bool Ndp { get; set; }

        /// <summary>
        /// Nf Conntrack Tcp Timeout Established
        /// </summary>
        [JsonProperty("nf_conntrack_tcp_timeout_established")]
        public int NfConntrackTcpTimeoutEstablished { get; set; }

        /// <summary>
        /// Nosmurfs
        /// </summary>
        [JsonProperty("nosmurfs")]
        public bool Nosmurfs { get; set; }

        /// <summary>
        /// Log Level Out 
        /// </summary>
        [JsonProperty("log_level_out")]
        public string LogLevelOut { get; set; }

        /// <summary>
        /// Tcp Filter Flags
        /// </summary>
        [JsonProperty("tcpflags")]
        public bool Tcpflags { get; set; }

        /// <summary>
        /// Log Level In
        /// </summary>
        [JsonProperty("log_level_in")]
        public string LogLevelIn { get; set; }

        /// <summary>
        /// Tcp Flags Log Level
        /// </summary>
        [JsonProperty("tcp_flags_log_level")]
        public string TcpFlagsLogLevel { get; set; }

        /// <summary>
        /// Enabled
        /// </summary>
        [JsonProperty("enable")]
        public bool Enable { get; set; }

        /// <summary>
        /// Nf Conntrack Max
        /// </summary>
        [JsonProperty("nf_conntrack_max")]
        public int NfConntrackMax { get; set; }
    }
}