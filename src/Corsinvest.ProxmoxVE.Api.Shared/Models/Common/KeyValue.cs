/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common;

/// <summary>
/// Key value
/// </summary>
public class KeyValue
{
    /// <summary>
    /// Value
    /// </summary>
    [JsonProperty("value")]
    public object Value { get; set; }

    /// <summary>
    /// Key
    /// </summary>
    [JsonProperty("key")]
    public string Key { get; set; }

    /// <summary>
    /// Pending value
    /// </summary>
    [JsonProperty("pending")]
    public object Pending { get; set; }

    /// <summary>
    /// Indicates a pending delete
    /// </summary>
    [JsonProperty("delete")]
    public int? Delete { get; set; }
}