/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using System.ComponentModel;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils;

/// <summary>
/// Snapshot helper
/// </summary>
public static class SnapshotHelper
{
    /// <summary>
    /// Get Snapshot
    /// </summary>
    /// <param name="client"></param>
    /// <param name="node"></param>
    /// <param name="vmType"></param>
    /// <param name="vmId"></param>
    /// <returns></returns>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    public static async Task<IEnumerable<VmSnapshot>> GetSnapshotsAsync(PveClient client, string node, VmType vmType, long vmId)
        => (vmType switch
        {
            VmType.Qemu => await client.Nodes[node].Qemu[vmId].Snapshot.GetAsync(),
            VmType.Lxc => await client.Nodes[node].Lxc[vmId].Snapshot.GetAsync(),
            _ => throw new InvalidEnumArgumentException(),
        }).OrderBy(a => a.Date);

    /// <summary>
    /// Create snapshot
    /// </summary>
    /// <param name="client"></param>
    /// <param name="node"></param>
    /// <param name="vmType"></param>
    /// <param name="vmId"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="state"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    public static async Task<Result> CreateSnapshotAsync(PveClient client,
                                                         string node,
                                                         VmType vmType,
                                                         long vmId,
                                                         string name,
                                                         string description,
                                                         bool state,
                                                         long timeout)
    {
        var result = vmType switch
        {
            VmType.Qemu => await client.Nodes[node].Qemu[vmId].Snapshot.Snapshot(name, description, state),
            VmType.Lxc => await client.Nodes[node].Lxc[vmId].Snapshot.Snapshot(name, description),
            _ => throw new InvalidEnumArgumentException(),
        };
        await client.WaitForTaskToFinishAsync(result, timeout: timeout);

        return result;
    }

    /// <summary>
    /// Remove snapshot
    /// </summary>
    /// <param name="client"></param>
    /// <param name="node"></param>
    /// <param name="vmType"></param>
    /// <param name="vmId"></param>
    /// <param name="name"></param>
    /// <param name="timeout"></param>
    /// <param name="force"></param>
    /// <returns></returns>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    public static async Task<Result> RemoveSnapshotAsync(PveClient client,
                                                         string node,
                                                         VmType vmType,
                                                         long vmId,
                                                         string name,
                                                         long timeout,
                                                         bool? force = null)
    {
        var result = vmType switch
        {
            VmType.Qemu => await client.Nodes[node].Qemu[vmId].Snapshot[name].Delsnapshot(force),
            VmType.Lxc => await client.Nodes[node].Lxc[vmId].Snapshot[name].Delsnapshot(force),
            _ => throw new InvalidEnumArgumentException(),
        };
        await client.WaitForTaskToFinishAsync(result, timeout: timeout);

        return result;
    }

    /// <summary>
    /// Get config snapshot
    /// </summary>
    /// <param name="client"></param>
    /// <param name="node"></param>
    /// <param name="vmType"></param>
    /// <param name="vmId"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    public static async Task<Result> GetConfigSnapshotAsync(PveClient client,
                                                            string node,
                                                            VmType vmType,
                                                            long vmId,
                                                            string name)
        => vmType switch
        {
            VmType.Qemu => await client.Nodes[node].Qemu[vmId].Snapshot[name].Config.GetSnapshotConfig(),
            VmType.Lxc => await client.Nodes[node].Lxc[vmId].Snapshot[name].Config.GetSnapshotConfig(),
            _ => throw new InvalidEnumArgumentException(),
        };

    /// <summary>
    /// Update snapshot
    /// </summary>
    /// <param name="client"></param>
    /// <param name="node"></param>
    /// <param name="vmType"></param>
    /// <param name="vmId"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    public static async Task<Result> UpdateSnapshotAsync(PveClient client,
                                                         string node,
                                                         VmType vmType,
                                                         long vmId,
                                                         string name,
                                                         string description)
        => vmType switch
        {
            VmType.Qemu => await client.Nodes[node].Qemu[vmId].Snapshot[name].Config.UpdateSnapshotConfig(description),
            VmType.Lxc => await client.Nodes[node].Lxc[vmId].Snapshot[name].Config.UpdateSnapshotConfig(description),
            _ => throw new InvalidEnumArgumentException(),
        };

    /// <summary>
    /// Rollback snapshot
    /// </summary>
    /// <param name="client"></param>
    /// <param name="node"></param>
    /// <param name="vmType"></param>
    /// <param name="vmId"></param>
    /// <param name="name"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    public static async Task<Result> RollbackSnapshotAsync(PveClient client,
                                                           string node,
                                                           VmType vmType,
                                                           long vmId,
                                                           string name,
                                                           long timeout)
    {
        var result = vmType switch
        {
            VmType.Qemu => await client.Nodes[node].Qemu[vmId].Snapshot[name].Rollback.Rollback(),
            VmType.Lxc => await client.Nodes[node].Lxc[vmId].Snapshot[name].Rollback.Rollback(),
            _ => throw new InvalidEnumArgumentException(),
        };

        await client.WaitForTaskToFinishAsync(result, timeout: timeout);
        return result;
    }
}