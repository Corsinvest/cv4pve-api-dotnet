﻿/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace Corsinvest.ProxmoxVE.Api.Shared;

/// <summary>
/// Pve exception
/// </summary>
public class PveException : Exception
{
    /// <summary>
    /// Constructor
    /// </summary>
    public PveException() { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="message"></param>
    public PveException(string message) : base(message) { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public PveException(string message, Exception innerException) : base(message, innerException) { }
}
