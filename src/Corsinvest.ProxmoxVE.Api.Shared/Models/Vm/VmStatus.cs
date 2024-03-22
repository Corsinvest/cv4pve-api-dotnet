/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

/// <summary>
/// Vm status
/// </summary>
public enum VmStatus
{
    /// <summary>
    /// Start
    /// </summary>
    Start,

    /// <summary>
    /// Stop
    /// </summary>
    Stop,

    /// <summary>
    /// Shutdown
    /// </summary>
    Shutdown,

    /// <summary>
    /// Reboot
    /// </summary>
    Reboot,

    /// <summary>
    /// Suspend
    /// </summary>
    Suspend,

    /// <summary>
    /// Resume
    /// </summary>
    Resume,

    /// <summary>
    /// Reset
    /// </summary>
    Reset,
}
