/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

/// <summary>
/// VM/CT extra Info
/// </summary>
public interface IClusterResourceVmOsInfo
{
    /// <summary>
    /// Qemu Agent OsInfo
    /// </summary>
    VmQemuAgentOsInfo VmQemuAgentOsInfo { get; set; }

    /// <summary>
    /// HostName
    /// </summary>
    string HostName { get; set; }

    /// <summary>
    /// OsVersion
    /// </summary>
    string OsVersion { get; set; }

    /// <summary>
    /// OsType
    /// </summary>
    VmOsType? OsType { get; set; }
}
