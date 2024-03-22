/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace Corsinvest.ProxmoxVE.Api;

/// <summary>
/// Pve authentication exception
/// </summary>
public class PveAuthenticationException : PveResultException
{
    /// <summary>
    ///Constructor
    /// </summary>
    /// <param name="result"></param>
    public PveAuthenticationException(Result result) : base(result) { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="result"></param>
    /// <param name="message"></param>
    public PveAuthenticationException(Result result, string message) : base(result, message) { }
}