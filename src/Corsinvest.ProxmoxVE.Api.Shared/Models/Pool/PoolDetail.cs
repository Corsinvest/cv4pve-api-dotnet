/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Pool;

/// <summary>
/// Pool Detail
/// </summary>
public class PoolDetail : ModelBase
{
    /// <summary>
    /// Comment
    /// </summary>
    [JsonProperty("comment")]
    public string Comment { get; set; }

    /// <summary>
    /// Members
    /// </summary>
    [JsonProperty("members")]
    public IEnumerable<object> Members { get; set; } = [];
}
