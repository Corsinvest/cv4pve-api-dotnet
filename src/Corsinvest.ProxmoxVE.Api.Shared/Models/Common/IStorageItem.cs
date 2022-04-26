/*
 * SPDX-FileCopyrightText: 2022 Daniele Corsini <daniele.corsini@corsinvest.it>
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common
{
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
}