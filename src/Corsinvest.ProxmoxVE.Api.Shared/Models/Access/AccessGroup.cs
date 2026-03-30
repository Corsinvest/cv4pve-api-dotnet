/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Access;

/// <summary>
/// Group
/// </summary>
public class AccessGroup : ModelBase
{
    /// <summary>
    /// Group Id
    /// </summary>
    [JsonProperty("groupid")]
    public string Id { get; set; }

    /// <summary>
    /// list of users which form this group
    /// </summary>
    [JsonProperty("users")]
    public string Users { get; set; }

    /// <summary>
    /// Comment
    /// </summary>
    [JsonProperty("comment")]
    public string Comment { get; set; }
}