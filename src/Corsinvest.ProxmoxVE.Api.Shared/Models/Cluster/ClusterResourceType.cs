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
    /// Sdn
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Node
    /// </summary>
    Node = 1,

    /// <summary>
    /// Vm
    /// </summary>
    Vm = 2,

    /// <summary>
    /// Storage
    /// </summary>
    Storage = 4,

    /// <summary>
    /// Pool
    /// </summary>
    Pool = 8,

    /// <summary>
    /// Sdn
    /// </summary>
    Sdn = 16,

    /// <summary>
    /// All
    /// </summary>
    All = Node | Vm | Storage | Pool | Sdn,
}