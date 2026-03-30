/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common;

/// <summary>
/// Vm Base
/// </summary>
public interface IVmBase
{
    /// <summary>
    /// Vm name
    /// </summary>
    [JsonProperty("name")]
    string Name { get; set; }

    /// <summary>
    /// Vm Id
    /// </summary>
    [JsonProperty("vmid")]
    long VmId { get; set; }

    /// <summary>
    /// Status Is running
    /// </summary>
    bool IsRunning { get; set; }

    /// <summary>
    /// Status Is stopped
    /// </summary>
    bool IsStopped { get; set; }

    /// <summary>
    /// Status Is paused
    /// </summary>
    bool IsPaused { get; set; }

    /// <summary>
    /// Is template
    /// </summary>
    [JsonProperty("template")]
    bool IsTemplate { get; set; }
}
