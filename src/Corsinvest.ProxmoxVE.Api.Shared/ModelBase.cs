/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
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