/*
 * SPDX-FileCopyrightText: 2022 Daniele Corsini <daniele.corsini@corsinvest.it>
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common
{
    /// <summary>
    /// Vm Base
    /// </summary>
    public interface IVmBase
    {
        /// <summary>
        /// Vm name
        /// </summary>
        /// <value></value>
        [JsonProperty("name")]
        string Name { get; set; }

        /// <summary>
        /// Vm Id
        /// </summary>
        /// <value></value>
        [JsonProperty("vmid")]
        long VmId { get; set; }

        /// <summary>
        /// Status Is running
        /// </summary>
        /// <value></value>
        bool IsRunning { get; set; }

        /// <summary>
        /// Status Is stopped
        /// </summary>
        /// <value></value>
        bool IsStopped { get; set; }

        /// <summary>
        /// Status Is paused
        /// </summary>
        /// <value></value>
        bool IsPaused { get; set; }

        /// <summary>
        /// Is template
        /// </summary>
        /// <value></value>
        [JsonProperty("template")]
        bool IsTemplate { get; set; }
    }
}