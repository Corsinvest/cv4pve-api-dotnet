/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

namespace Corsinvest.ProxmoxVE.Api;

/// <summary>
/// Pve exception result
/// </summary>
public class PveResultException : Exception
{
    /// <summary>
    /// Constructor
    /// </summary>
    public PveResultException(Result result) : base() => Result = result;

    /// <summary>
    /// Constructor
    /// </summary>
    public PveResultException(Result result, string message) : base(message) => Result = result;

    /// <summary>
    /// Result
    /// </summary>
    public Result Result { get; }
}