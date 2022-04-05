/*
 * SPDX-FileCopyrightText: 2022 Daniele Corsini <daniele.corsini@corsinvest.it>
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster
{
    /// <summary>
    /// Cluster Config Totem
    /// </summary>
    public class ClusterConfigTotem
    {
        /// <summary>
        /// Cluster name
        /// </summary>
        [JsonProperty("cluster_name")]
        public string ClusterName { get; set; }

        /// <summary>
        /// Cluster Version
        /// </summary>
        [JsonProperty("config_version")]
        public string ConfigVersion { get; set; }

        /// <summary>
        /// Interfaces
        /// </summary>
        [JsonProperty("interface")]
        public IDictionary<string, Interface> Interfaces { get; set; }

        /// <summary>
        /// Ip Version
        /// </summary>
        [JsonProperty("ip_version")]
        public string IpVersion { get; set; }

        /// <summary>
        /// Link Mode
        /// </summary>
        [JsonProperty("link_mode")]
        public string LinkMode { get; set; }

        /// <summary>
        /// Secauth 
        /// </summary>
        [JsonProperty("secauth")]
        public string Secauth { get; set; }

        /// <summary>
        /// Version 
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// Interface
        /// </summary>
        public class Interface
        {
            /// <summary>
            /// LinkNumber
            /// </summary>
            [JsonProperty("linknumber")]
            public string LinkNumber { get; set; }
        }
    }
}