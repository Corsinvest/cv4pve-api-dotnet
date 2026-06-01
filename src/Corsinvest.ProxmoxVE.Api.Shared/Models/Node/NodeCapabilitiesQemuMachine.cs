/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// QEMU machine type available on the node (one entry per supported machine).
/// </summary>
public class NodeCapabilitiesQemuMachine
{
    /// <summary>
    /// Full machine identifier (e.g. "pc-i440fx-8.0", "pc-q35-9.2", "pc-i440fx-latest").
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Machine family (e.g. "i440fx", "q35").
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// QEMU version of the machine type (e.g. "8.0").
    /// </summary>
    [JsonProperty("version")]
    public string Version { get; set; }

    /// <summary>
    /// Optional human-readable changelog / notes for this machine version.
    /// </summary>
    [JsonProperty("changes")]
    public string Changes { get; set; }
}
