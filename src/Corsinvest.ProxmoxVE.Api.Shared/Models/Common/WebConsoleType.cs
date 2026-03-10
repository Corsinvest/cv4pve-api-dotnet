/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common;

/// <summary>
/// Web console type
/// </summary>
public enum WebConsoleType
{
    /// <summary>
    /// Spice console
    /// </summary>
    Spice,

    /// <summary>
    /// NoVNC console
    /// </summary>
    NoVnc,

    /// <summary>
    /// Xterm.js console
    /// </summary>
    XtermJs
}