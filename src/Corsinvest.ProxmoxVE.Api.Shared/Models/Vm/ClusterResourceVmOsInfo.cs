/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

/// <summary>
/// VM/CT extra Info
/// </summary>
public class ClusterResourceVmOsInfo : IClusterResourceVmOsInfo
{
    /// <summary>
    /// Qemu Agent OsInfo
    /// </summary>
    public VmQemuAgentOsInfo VmQemuAgentOsInfo { get; set; }

    /// <summary>
    /// HostName
    /// </summary>
    public string HostName { get; set; }

    /// <summary>
    /// OsVersion
    /// </summary>
    public string OsVersion { get; set; }

    /// <summary>
    /// OsType
    /// </summary>
    public VmOsType? OsType { get; set; }
}
