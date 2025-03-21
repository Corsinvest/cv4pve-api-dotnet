/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;
using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Pool;

/// <summary>
/// Pool detail
/// </summary>
public class PoolDetail : ModelBase
{
    /// <summary>
    /// Comment
    /// </summary>
    /// <value></value>
    [JsonProperty("comment")]
    public string Comment { get; set; }

    /// <summary>
    /// Id
    /// </summary>
    /// <value></value>
    [JsonProperty("members")]
    public IEnumerable<ClusterResource> Members { get; set; }
}