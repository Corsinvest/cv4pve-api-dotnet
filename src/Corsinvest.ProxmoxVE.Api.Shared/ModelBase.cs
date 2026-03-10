/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared;

/// <summary>
/// Model base result
/// </summary>
public class ModelBase
{
    /// <summary>
    /// Extension Data
    /// </summary>
    [JsonExtensionData]
    public IDictionary<string, object> ExtensionData { get; set; }
}