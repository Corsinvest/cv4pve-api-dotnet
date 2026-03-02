/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace Corsinvest.ProxmoxVE.Api.Shared.Utils;

/// <summary>
/// Helper for PVE web UI URLs
/// </summary>
public static class PveWebUrlHelper
{
    /// <summary>
    /// Get web URL path for a node
    /// </summary>
    public static string GetWebUrlNode(string node)
        => $"nodes/{node}";

    /// <summary>
    /// Get web URL path for a Qemu VM
    /// </summary>
    public static string GetWebUrlQemu(string node, long vmId)
        => $"nodes/{node}/qemu/{vmId}";

    /// <summary>
    /// Get web URL path for a LXC container
    /// </summary>
    public static string GetWebUrlLxc(string node, long vmId)
        => $"nodes/{node}/lxc/{vmId}";

    /// <summary>
    /// Get web URL path for a storage
    /// </summary>
    public static string GetWebUrlStorage(string node, string storage)
        => $"nodes/{node}/storage/{storage}";

    /// <summary>
    /// Get web URL path for a pool
    /// </summary>
    public static string GetWebUrlPool(string pool)
        => $"pool/{pool}";
}
