/*
 * SPDX-FileCopyrightText: 2022 Daniele Corsini <daniele.corsini@corsinvest.it>
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster
{
    /// <summary>
    /// Cluster Ha group
    /// </summary>
    public class ClusterHaResource
    {
        /// <summary>
        /// Digest
        /// </summary>
        [JsonProperty("digest")]
        public string Digest { get; set; }

        /// <summary>
        /// Group
        /// </summary>
        [JsonProperty("group")]
        public string Group { get; set; }

        /// <summary>
        /// Sid
        /// </summary>
        [JsonProperty("sid")]
        public string Sid { get; set; }

        /// <summary>
        /// State
        /// </summary>
        [JsonProperty("state")]
        public string State { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Comment
        /// </summary>
        [JsonProperty("comment")]
        public string Comment { get; set; }
    }
}