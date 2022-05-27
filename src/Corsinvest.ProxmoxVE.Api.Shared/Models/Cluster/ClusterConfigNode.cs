/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster
{
    /// <summary>
    /// Cluster config node
    /// </summary>
    public class ClusterConfigNode
    {
        /// <summary>
        /// Quorum Votes
        /// </summary>
        /// <value></value>
        [JsonProperty("quorum_votes")]
        public string QuorumVotes { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        /// <value></value>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Node id
        /// </summary>
        /// <value></value>
        [JsonProperty("nodeid")]
        public string NodeId { get; set; }

        /// <summary>
        /// Node
        /// </summary>
        /// <value></value>
        [JsonProperty("node")]
        public string Node { get; set; }
    }
}