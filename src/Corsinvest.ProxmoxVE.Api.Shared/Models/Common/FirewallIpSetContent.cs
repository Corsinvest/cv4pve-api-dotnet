/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common
{
    /// <summary>
    /// Firewall IpSet content
    /// </summary>
    public class FirewallIpSetContent
    {
        /// <summary>
        /// Digest
        /// </summary>
        [JsonProperty("digest")]
        public string Digest { get; set; }

        /// <summary>
        /// Cidr
        /// </summary>
        [JsonProperty("cidr")]
        public string Cidr { get; set; }

        /// <summary>
        /// Comment
        /// </summary>
        [JsonProperty("comment")]
        public string Comment { get; set; }
    }
}