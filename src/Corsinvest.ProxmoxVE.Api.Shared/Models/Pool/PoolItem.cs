/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;
using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Pool;

/// <summary>
/// Pool
/// </summary>
public class PoolItem : ModelBase
{
    /// <summary>
    /// Id
    /// </summary>
    [JsonProperty("poolid")]
    public string Id { get; set; }

    /// <summary>
    /// Comment
    /// </summary>
    [JsonProperty("comment")]
    public string Comment { get; set; }
    /// <summary>
    /// Members.
    /// </summary>
    [JsonProperty("members")]
    public IEnumerable<ClusterResource> Members { get; set; } = [];
}
