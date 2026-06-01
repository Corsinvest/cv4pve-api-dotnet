/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// QEMU CPU model exposed by the node (one entry per supported/custom CPU model).
/// </summary>
public class NodeCapabilitiesQemuCpu
{
    /// <summary>
    /// CPU model name (e.g. "host", "kvm64", "x86-64-v2-AES").
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// CPU vendor (e.g. "Intel", "AMD").
    /// </summary>
    [JsonProperty("vendor")]
    public string Vendor { get; set; }

    /// <summary>
    /// 1 if this is a custom CPU model defined via /nodes/{node}/capabilities/qemu/cpu, 0 for built-in.
    /// </summary>
    [JsonProperty("custom")]
    public int Custom { get; set; }
}
