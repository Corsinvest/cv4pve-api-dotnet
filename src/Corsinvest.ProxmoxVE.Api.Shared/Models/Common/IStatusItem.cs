/*
 * SPDX-FileCopyrightText: 2022 Daniele Corsini <daniele.corsini@corsinvest.it>
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common
{

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
}