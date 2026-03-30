/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
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