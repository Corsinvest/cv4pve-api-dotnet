/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Access;

/// <summary>
/// Acl
/// </summary>
public class AccessAcl: ModelBase
{
    /// <summary>
    /// Path
    /// </summary>
    /// <value></value>
    [JsonProperty("path")]
    public string Path { get; set; }

    /// <summary>
    /// Role id
    /// </summary>
    /// <value></value>
    [JsonProperty("roleid")]
    public string Roleid { get; set; }

    /// <summary>
    /// User group id
    /// </summary>
    /// <value></value>
    [JsonProperty("ugid")]
    public string UsersGroupid { get; set; }

    /// <summary>
    /// Propagate
    /// </summary>
    /// <value></value>
    [JsonProperty("propagate")]
    public int Propagate { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    /// <value></value>
    [JsonProperty("type")]
    public string Type { get; set; }
}