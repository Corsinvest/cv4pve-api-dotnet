/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Access;

/// <summary>
/// Access TFA Entry
/// </summary>
public class AccessTfaEntry : ModelBase
{
    /// <summary>
    /// Id
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    [JsonProperty("description")]
    public string Description { get; set; }

    /// <summary>
    /// Creation time (seconds since epoch)
    /// </summary>
    [JsonProperty("created")]
    public long Created { get; set; }

    /// <summary>
    /// Enable
    /// </summary>
    [JsonProperty("enable")]
    public bool? Enable { get; set; }
}
