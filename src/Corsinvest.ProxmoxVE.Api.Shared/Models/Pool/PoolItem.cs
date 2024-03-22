/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Pool;

/// <summary>
/// Pool
/// </summary>
public class PoolItem
{
    /// <summary>
    /// Id
    /// </summary>
    /// <value></value>
    [JsonProperty("poolid")]
    public string Id { get; set; }

    /// <summary>
    /// Comment
    /// </summary>
    /// <value></value>
    [JsonProperty("comment")]
    public string Comment { get; set; }
}