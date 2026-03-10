/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node service
/// </summary>
public class NodeService : ModelBase
{
    /// <summary>
    /// Short identifier for the service (e.g., "pveproxy").
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Execution status of the service (systemd SubState).
    /// </summary>
    [JsonProperty("state")]
    public string State { get; set; }

    /// <summary>
    /// IS running
    /// </summary>
    /// <value></value>
    public bool IsRunning => State == "running";

    /// <summary>
    /// Systemd unit name (e.g., pveproxy).
    /// </summary>
    [JsonProperty("service")]
    public string Service { get; set; }

    /// <summary>
    /// Description of the service.
    /// </summary>
    [JsonProperty("desc")]
    public string Description { get; set; }
    /// <summary>
    /// Current state of the service process (systemd ActiveState).
    /// </summary>
    [JsonProperty("active-state")]
    public string ActiveState { get; set; }

    /// <summary>
    /// Whether the service is enabled (systemd UnitFileState).
    /// </summary>
    [JsonProperty("unit-state")]
    public string UnitState { get; set; }
}
