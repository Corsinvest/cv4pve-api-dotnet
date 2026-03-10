/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common;

/// <summary>
/// Node item
/// </summary>
public interface INodeItem
{
    /// <summary>
    /// Node
    /// </summary>
    [JsonProperty("node")]
    public string Node { get; set; }
}