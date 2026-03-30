/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Cluster SDN IPAM
/// </summary>
public class ClusterSdnIpam : ModelBase
{
    /// <summary>
    /// IPAM id
    /// </summary>
    [JsonProperty("ipam")]
    public string Ipam { get; set; }

    /// <summary>
    /// Plugin type (netbox, phpipam, pve)
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }
}
