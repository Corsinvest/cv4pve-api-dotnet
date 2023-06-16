/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node
{
    /// <summary>
    /// Node disk Zfs
    /// </summary>
    public class NodeDiskZfs
    {
        /// <summary>
        /// Free
        /// </summary>
        /// <value></value>
        [JsonProperty("free")]
        public long Free { get; set; }

        /// <summary>
        /// Dedup
        /// </summary>
        /// <value></value>
        [JsonProperty("dedup")]
        public double Dedup { get; set; }

        /// <summary>
        /// Health
        /// </summary>
        /// <value></value>
        [JsonProperty("health")]
        public string Health { get; set; }

        /// <summary>
        /// Size
        /// </summary>
        /// <value></value>
        [JsonProperty("size")]
        public long Size { get; set; }

        /// <summary>
        /// Alloc
        /// </summary>
        /// <value></value>
        [JsonProperty("alloc")]
        public long Alloc { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        /// <value></value>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Frag
        /// </summary>
        /// <value></value>
        [JsonProperty("frag")]
        public long Frag { get; set; }
    }
}