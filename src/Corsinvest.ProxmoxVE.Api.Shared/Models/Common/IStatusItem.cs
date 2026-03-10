/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common;

/// <summary>
/// Interface status
/// </summary>
public interface IStatusItem
{
    /// <summary>
    /// Status
    /// </summary>
    [JsonProperty("status")]
    string Status { get; set; }
}