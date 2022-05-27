/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm
{
    /// <summary>
    /// Vm firewall options
    /// </summary>
    public class VmFirewallOptions
    {
        /// <summary>
        /// Enable 
        /// </summary>
        [JsonProperty("enable")]
        public bool Enable { get; set; }

        /// <summary>
        /// Ipfilter
        /// </summary>
        [JsonProperty("ipfilter")]
        public bool Ipfilter { get; set; }

        /// <summary>
        /// PolicyOut
        /// </summary>
        [JsonProperty("policy_out")]
        public string PolicyOut { get; set; }

        /// <summary>
        /// Router Advertisement
        /// </summary>
        [JsonProperty("radv")]
        public bool RouterAdvertisement { get; set; }

        /// <summary>
        /// Log Level Out 
        /// </summary>
        [JsonProperty("log_level_out")]
        public string LogLevelOut { get; set; }

        /// <summary>
        /// Log Level In 
        /// </summary>
        [JsonProperty("log_level_in")]
        public string LogLevelIn { get; set; }

        /// <summary>
        /// Mac filter 
        /// </summary>
        [JsonProperty("macfilter")]
        public bool MacFilter { get; set; }

        /// <summary>
        /// Policy In 
        /// </summary>
        [JsonProperty("policy_in")]
        public string PolicyIn { get; set; }

        /// <summary>
        /// Digest
        /// </summary>
        [JsonProperty("digest")]
        public string Digest { get; set; }

        /// <summary>
        /// Dhcp 
        /// </summary>
        [JsonProperty("dhcp")]
        public bool Dhcp { get; set; }

        /// <summary>
        /// Ndp
        /// </summary>
        [JsonProperty("ndp")]
        public bool Ndp { get; set; }
    }
}