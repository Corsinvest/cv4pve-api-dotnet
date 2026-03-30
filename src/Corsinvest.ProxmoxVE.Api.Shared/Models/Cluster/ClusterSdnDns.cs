/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Cluster SDN DNS
/// </summary>
public class ClusterSdnDns : ModelBase
{
    /// <summary>
    /// DNS id
    /// </summary>
    [JsonProperty("dns")]
    public string Dns { get; set; }

    /// <summary>
    /// Plugin type (powerdns)
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }
}
