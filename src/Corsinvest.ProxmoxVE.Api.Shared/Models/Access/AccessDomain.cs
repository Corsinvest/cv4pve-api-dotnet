/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Access;

/// <summary>
/// Domain
/// </summary>
public class AccessDomain : ModelBase
{
    /// <summary>
    /// Realm
    /// </summary>
    /// <value></value>
    [JsonProperty("realm")]
    public string Realm { get; set; }

    /// <summary>
    /// A comment. The GUI use this text when you select a domain (Realm) on the login window.
    /// </summary>
    [JsonProperty("comment")]
    public string Comment { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    /// <value></value>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Two-factor authentication provider.
    /// </summary>
    [JsonProperty("tfa")]
    public string Tfa { get; set; }
}