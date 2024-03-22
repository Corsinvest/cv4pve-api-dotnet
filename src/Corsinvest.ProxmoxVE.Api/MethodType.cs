/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace Corsinvest.ProxmoxVE.Api;

/// <summary>
/// Method type
/// </summary>
public enum MethodType
{
    /// <summary>
    /// Get
    /// </summary>
    Get,

    /// <summary>
    /// Set
    /// </summary>
    Set,

    /// <summary>
    /// Create
    /// </summary>
    Create,

    /// <summary>
    /// Delete
    /// </summary>
    Delete
}