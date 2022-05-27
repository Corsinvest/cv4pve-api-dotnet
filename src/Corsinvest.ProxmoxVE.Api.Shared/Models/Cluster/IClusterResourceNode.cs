/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Node;
using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster
{
    /// <summary>
    /// Resource data
    /// </summary>
    public interface IClusterResourceNode : IClusterResourceHost
    {
        /// <summary>
        /// Level
        /// </summary>
        /// <value></value>
        [JsonProperty("level")]
        string Level { get; set; }

        /// <summary>
        /// Is online
        /// </summary>
        /// <value></value>
        bool IsOnline { get; set; }

        /// <summary>
        /// Node Level
        /// </summary>
        public NodeLevel NodeLevel { get; set; }
    }
}