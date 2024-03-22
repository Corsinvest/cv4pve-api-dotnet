/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Cluster Config Join
/// </summary>
public class ClusterConfigJoin
{
    /// <summary>
    /// ConfigDigest
    /// </summary>
    [JsonProperty("config_digest")]
    public string ConfigDigest { get; set; }

    /// <summary>
    /// Nodes
    /// </summary>
    [JsonProperty("nodelist")]
    public IEnumerable<Node> Nodes { get; set; }

    /// <summary>
    /// Preferred Node
    /// </summary>
    [JsonProperty("preferred_node")]
    public string PreferredNode { get; set; }

    /// <summary>
    /// Totem
    /// </summary>
    [JsonProperty("totem")]
    public ClusterConfigTotem Totem { get; set; }

    /// <summary>
    /// Node
    /// </summary>
    public class Node : ClusterConfigNode
    {
        /// <summary>
        /// Pve Address
        /// </summary>
        [JsonProperty("pve_addr")]
        public string PveAddress { get; set; }

        /// <summary>
        /// Pve Fingerprint
        /// </summary>
        [JsonProperty("pve_fp")]
        public string PveFingerprint { get; set; }
    }
}