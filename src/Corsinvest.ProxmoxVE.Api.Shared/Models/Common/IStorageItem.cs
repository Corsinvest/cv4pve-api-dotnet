/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common;

/// <summary>
/// Storage Item
/// </summary>
public interface IStorageItem
{
    /// <summary>
    /// Storage
    /// </summary>
    [JsonProperty("storage")]
    string Storage { get; set; }
}