/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Access;

/// <summary>
/// Access User Token
/// </summary>
public class AccessUserToken : ModelBase
{
    /// <summary>
    /// Token id
    /// </summary>
    [JsonProperty("tokenid")]
    public string TokenId { get; set; }

    /// <summary>
    /// Comment
    /// </summary>
    [JsonProperty("comment")]
    public string Comment { get; set; }

    /// <summary>
    /// Expiration date (seconds since epoch)
    /// </summary>
    [JsonProperty("expire")]
    public long? Expire { get; set; }

    /// <summary>
    /// Restrict privileges to user's ACL
    /// </summary>
    [JsonProperty("privsep")]
    public bool? PrivSep { get; set; }
}
