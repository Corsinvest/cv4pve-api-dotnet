/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node
{
    /// <summary>
    /// Node service
    /// </summary>
    public class NodeService
    {
        /// <summary>
        /// Name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// State
        /// </summary>
        [JsonProperty("state")]
        public string State { get; set; }

        /// <summary>
        /// IS running
        /// </summary>
        /// <value></value>
        public bool IsRunning => State == "running";

        /// <summary>
        /// Service
        /// </summary>
        [JsonProperty("service")]
        public string Service { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [JsonProperty("desc")]
        public string Description { get; set; }
    }
}