/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;

/// <summary>
/// Cluster Resource Type
/// </summary>
public enum ClusterResourceType
{
    /// <summary>
    /// All
    /// </summary>
    All,

    /// <summary>
    /// Node
    /// </summary>
    Node,

    /// <summary>
    /// Vm
    /// </summary>
    Vm,

    /// <summary>
    /// Storage
    /// </summary>
    Storage,

    /// <summary>
    /// Pool
    /// </summary>
    Pool,

    /// <summary>
    /// Sdn
    /// </summary>
    Sdn,

    /// <summary>
    /// Sdn
    /// </summary>
    Unknown,
}