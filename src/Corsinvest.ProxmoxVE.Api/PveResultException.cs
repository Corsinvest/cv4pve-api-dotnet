/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using System;

namespace Corsinvest.ProxmoxVE.Api;

/// <summary>
/// Pve exception result
/// </summary>
public class PveResultException : Exception
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public PveResultException(Result result) : base() => Result = result;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="result"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public PveResultException(Result result, string message) : base(message) => Result = result;

    /// <summary>
    /// Result
    /// </summary>
    /// <value></value>
    public Result Result { get; }
}