/*
 * SPDX-FileCopyrightText: 2022 Daniele Corsini <daniele.corsini@corsinvest.it>
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common
{
    /// <summary>
    /// Pool item
    /// </summary>
    public interface IPoolItem
    {
        /// <summary>
        /// Pool
        /// </summary>
        /// <value></value>
        [JsonProperty("pool")]
        string Pool { get; set; }
    }
}