/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
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
    /// <value></value>
    [JsonProperty("groupid")]
    public string Id { get; set; }

    /// <summary>
    /// Users
    /// </summary>
    /// <value></value>
    [JsonProperty("users")]
    public string Users { get; set; }

    /// <summary>
    /// Comment
    /// </summary>
    /// <value></value>
    [JsonProperty("comment")]
    public string Comment { get; set; }
}