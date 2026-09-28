/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common;

/// <summary>
/// Pool item
/// </summary>
public interface IPoolItem
{
    /// <summary>
    /// Pool
    /// </summary>
    [JsonProperty("pool")]
    string Pool { get; set; }
}
