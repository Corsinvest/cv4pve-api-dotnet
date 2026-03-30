/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Access;

/// <summary>
/// Role
/// </summary>
public class AccessRole : ModelBase
{
    /// <summary>
    /// Privileges
    /// </summary>
    [JsonProperty("privs")]
    public string Privileges { get; set; }

    /// <summary>
    /// Role Id
    /// </summary>
    [JsonProperty("roleid")]
    public string Id { get; set; }

    /// <summary>
    /// Special
    /// </summary>
    [JsonProperty("special")]
    public int Special { get; set; }
}