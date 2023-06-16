/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster
{
    /// <summary>
    /// Cluster options
    /// </summary>
    public class ClusterOptions
    {
        /// <summary>
        /// Keyboard
        /// </summary>
        [JsonProperty("keyboard")]
        public string Keyboard { get; set; }

        /// <summary>
        /// Allowed Tags
        /// </summary>
        /// <value></value>
        [JsonProperty("allowed-tags")]
        public List<string> AllowedTags { get; set; }

        /// <summary>
        /// Migration
        /// </summary>
        [JsonProperty("migration")]
        public MigrationInt Migration { get; set; }

        /// <summary>
        /// Migration
        /// </summary>
        public class MigrationInt
        {
            /// <summary>
            /// Network
            /// </summary>
            [JsonProperty("network")]
            public string Network { get; set; }

            /// <summary>
            /// Type
            /// </summary>
            [JsonProperty("type")]
            public string Type { get; set; }
        }
    }
}