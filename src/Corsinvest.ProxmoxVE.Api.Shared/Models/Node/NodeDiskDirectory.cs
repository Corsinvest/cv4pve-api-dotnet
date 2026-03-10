/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Node Disk Directory
/// </summary>
public class NodeDiskDirectory : ModelBase
{
    /// <summary>
    /// Device
    /// </summary>
    [JsonProperty("device")]
    public string Device { get; set; }

    /// <summary>
    /// Path
    /// </summary>
    [JsonProperty("path")]
    public string Path { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Mount options
    /// </summary>
    [JsonProperty("options")]
    public string Options { get; set; }

    /// <summary>
    /// Systemd unit file
    /// </summary>
    [JsonProperty("unitfile")]
    public string UnitFile { get; set; }
}
