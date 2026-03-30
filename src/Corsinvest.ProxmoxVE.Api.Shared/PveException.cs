/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
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
    public PveException(string message) : base(message) { }

    /// <summary>
    /// Constructor
    /// </summary>
    public PveException(string message, Exception innerException) : base(message, innerException) { }
}
