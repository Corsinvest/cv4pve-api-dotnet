/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
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
    /// <value></value>
    VmQemuAgentOsInfo VmQemuAgentOsInfo { get; set; }

    /// <summary>
    /// HostName
    /// </summary>
    /// <value></value>
    string HostName { get; set; }

    /// <summary>
    /// OsVersion
    /// </summary>
    /// <value></value>
    string OsVersion { get; set; }

    /// <summary>
    /// OsType
    /// </summary>
    /// <value></value>
    VmOsType? OsType { get; set; }
}