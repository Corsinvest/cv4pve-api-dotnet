/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node
{
    /// <summary>
    /// Node dns
    /// </summary>
    public class NodeDns
    {
        /// <summary>
        /// Search
        /// </summary>
        [JsonProperty("search")]
        public string Search { get; set; }

        /// <summary>
        /// DNS 1
        /// </summary>
        [JsonProperty("dns1")]
        public string Dns1 { get; set; }

        /// <summary>
        /// DNS 2
        /// </summary>
        [JsonProperty("dns2")]
        public string Dns2 { get; set; }

        /// <summary>
        /// DNS 3
        /// </summary>
        [JsonProperty("dns3")]
        public string Dns3 { get; set; }

        /// <summary>
        /// Is equal
        /// </summary>
        /// <param name="dns"></param>
        /// <returns></returns>
        public bool IsEqual(NodeDns dns)
            => Search == dns.Search
                && Dns1 == dns.Dns1
                && Dns2 == dns.Dns2
                && Dns3 == dns.Dns3;
    }
}