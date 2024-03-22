/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Access;

/// <summary>
/// Domain
/// </summary>
public class AccessDomain
{
    /// <summary>
    /// Realm
    /// </summary>
    /// <value></value>
    [JsonProperty("realm")]
    public string Realm { get; set; }

    /// <summary>
    /// Comment
    /// </summary>
    /// <value></value>
    [JsonProperty("comment")]
    public string Comment { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    /// <value></value>
    [JsonProperty("type")]
    public string Type { get; set; }
}