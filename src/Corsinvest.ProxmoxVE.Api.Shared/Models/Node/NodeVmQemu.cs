/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Node;

/// <summary>
/// Qemu VM list item
/// </summary>
public class NodeVmQemu : NodeVmBase
{
    /// <summary>
    /// VM run state from the 'query-status' QMP monitor command.
    /// </summary>
    [JsonProperty("qmpstatus")]
    public string Qmpstatus { get; set; }

    /// <summary>
    /// The currently running machine type (if running).
    /// </summary>
    [JsonProperty("running-machine")]
    public string RunningMachine { get; set; }

    /// <summary>
    /// The QEMU version the VM is currently using (if running).
    /// </summary>
    [JsonProperty("running-qemu")]
    public string RunningQemu { get; set; }

    /// <summary>
    /// Guest has serial device configured.
    /// </summary>
    [JsonProperty("serial")]
    public bool Serial { get; set; }

    /// <summary>PSI CPU pressure (full) — requires PVE 9.0+. Only on QEMU.</summary>
    [JsonProperty("pressurecpufull")]
    public double PressureCpuFull { get; set; }
}