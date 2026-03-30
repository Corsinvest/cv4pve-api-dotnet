/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common;

/// <summary>
/// Sdn item
/// </summary>
public interface ISdnItem
{
    /// <summary>
    /// Sdn
    /// </summary>
    [JsonProperty("sdn")]
    string Sdn { get; set; }
}
