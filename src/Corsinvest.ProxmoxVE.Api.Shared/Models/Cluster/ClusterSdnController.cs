/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Cluster SDN Controller
/// </summary>
public class ClusterSdnController : ModelBase
{
    /// <summary>
    /// Controller id
    /// </summary>
    [JsonProperty("controller")]
    public string Controller { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// ASN
    /// </summary>
    [JsonProperty("asn")]
    public int? Asn { get; set; }

    /// <summary>
    /// Peers
    /// </summary>
    [JsonProperty("peers")]
    public string Peers { get; set; }

    /// <summary>
    /// Node
    /// </summary>
    [JsonProperty("node")]
    public string Node { get; set; }

    /// <summary>
    /// State
    /// </summary>
    [JsonProperty("state")]
    public string State { get; set; }

    /// <summary>
    /// Digest
    /// </summary>
    [JsonProperty("digest")]
    public string Digest { get; set; }
}
