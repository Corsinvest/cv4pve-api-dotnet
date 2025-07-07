/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Common;

/// <summary>
/// Cluster type
/// </summary>
public enum ClusterType
{
    /// <summary>
    /// Single node cluster
    /// </summary>
    SingleNode = 0,

    /// <summary>
    /// Cluster
    /// </summary>
    Cluster = 1,
}
