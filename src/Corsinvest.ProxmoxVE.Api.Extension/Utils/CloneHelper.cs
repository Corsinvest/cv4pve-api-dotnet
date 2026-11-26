/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.ComponentModel;
using System.Globalization;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils;

/// <summary>
/// Unified VM and LXC container cloning and template conversion helpers.
/// </summary>
public static class CloneHelper
{
    /// <summary>
    /// Retrieves the next free VM/CT ID from the cluster, or validates a preferred VMID.
    /// </summary>
    /// <param name="client">The Proxmox VE client.</param>
    /// <param name="preferredVmId">Optional preferred VMID to check for availability.</param>
    /// <returns>The next available VMID.</returns>
    /// <exception cref="PveResultException">Thrown when the API call fails or returns an invalid VMID.</exception>
    public static async Task<long> GetNextVmIdAsync(PveClient client, int? preferredVmId = null)
    {
        ArgumentNullException.ThrowIfNull(client);

        if (preferredVmId.HasValue && preferredVmId.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(preferredVmId), "Preferred VM ID must be greater than zero.");
        }

        var result = await client.Cluster.Nextid.Nextid(preferredVmId);
        if (!result.IsSuccessStatusCode)
        {
            throw new PveResultException(result, $"Failed to retrieve next free VMID: {result.ReasonPhrase}");
        }

        var rawData = result.ToData();
        string rawText = rawData != null ? Convert.ToString(rawData, CultureInfo.InvariantCulture) : null;
        if (!long.TryParse(rawText, NumberStyles.Integer, CultureInfo.InvariantCulture, out long nextVmId) || nextVmId <= 0)
        {
            throw new PveResultException(result, "Proxmox VE returned an invalid next VMID.");
        }

        return nextVmId;
    }

    /// <summary>
    /// Clones a QEMU VM or LXC container, automatically allocating a free VMID if omitted
    /// and awaiting the background task completion.
    /// </summary>
    /// <param name="client">The Proxmox VE client.</param>
    /// <param name="node">Source node name.</param>
    /// <param name="vmType">Guest type (Qemu or Lxc).</param>
    /// <param name="vmId">Source VM or container ID.</param>
    /// <param name="newVmId">Target VMID. If null, the next free cluster VMID is automatically allocated.</param>
    /// <param name="name">Name (QEMU) or hostname (LXC) for the cloned guest.</param>
    /// <param name="description">Optional description for the cloned guest.</param>
    /// <param name="pool">Optional resource pool to add the cloned guest to.</param>
    /// <param name="targetNode">Optional target node for the clone (must be on shared storage if different from source).</param>
    /// <param name="targetStorage">Optional target storage for full clones.</param>
    /// <param name="fullClone">If true, creates a full copy of all disks; if false, creates a linked clone; null uses Proxmox VE default.</param>
    /// <param name="snapName">Optional snapshot name to clone from.</param>
    /// <param name="format">Target disk format (QEMU only, e.g. "raw", "qcow2", "vmdk").</param>
    /// <param name="bwlimit">Optional I/O bandwidth limit in KiB/s.</param>
    /// <param name="timeout">Timeout in milliseconds to wait for the clone task. Default 300,000 (5 minutes). 0 to not wait.</param>
    /// <param name="wait">Polling interval in milliseconds when awaiting the task. Default 500.</param>
    /// <returns>A tuple containing the resolved target VMID and the API Result.</returns>
    public static async Task<(long NewVmId, Result Result)> CloneAsync(
        PveClient client,
        string node,
        VmType vmType,
        long vmId,
        long? newVmId = null,
        string name = null,
        string description = null,
        string pool = null,
        string targetNode = null,
        string targetStorage = null,
        bool? fullClone = null,
        string snapName = null,
        string format = null,
        int? bwlimit = null,
        long timeout = 300000,
        int wait = 500)
    {
        ArgumentNullException.ThrowIfNull(client);
        if (string.IsNullOrWhiteSpace(node)) { throw new ArgumentException("Node cannot be null or empty.", nameof(node)); }
        if (vmId <= 0) { throw new ArgumentOutOfRangeException(nameof(vmId), "VM ID must be greater than zero."); }
        if (vmType != VmType.Qemu && vmType != VmType.Lxc)
        {
            throw new InvalidEnumArgumentException(nameof(vmType), (int)vmType, typeof(VmType));
        }

        if (vmType == VmType.Lxc && !string.IsNullOrWhiteSpace(format))
        {
            throw new ArgumentException("Target disk format is only supported for QEMU VMs.", nameof(format));
        }

        if (newVmId.HasValue && newVmId.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(newVmId), "Target VM ID must be greater than zero.");
        }

        node = node.Trim();
        var resolvedVmId = newVmId ?? await GetNextVmIdAsync(client);
        if (resolvedVmId > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(newVmId), $"Target VM ID {resolvedVmId} exceeds Int32 range supported by Proxmox VE clone API.");
        }

        var result = vmType switch
        {
            VmType.Qemu => await client.Nodes[node].Qemu[vmId].Clone.CloneVm(
                newid: (int)resolvedVmId,
                bwlimit: bwlimit,
                description: description,
                format: format,
                full: fullClone,
                name: name,
                pool: pool,
                snapname: snapName,
                storage: targetStorage,
                target: targetNode),
            VmType.Lxc => await client.Nodes[node].Lxc[vmId].Clone.CloneVm(
                newid: (int)resolvedVmId,
                bwlimit: bwlimit,
                description: description,
                full: fullClone,
                hostname: name,
                pool: pool,
                snapname: snapName,
                storage: targetStorage,
                target: targetNode),
            _ => throw new InvalidEnumArgumentException(nameof(vmType), (int)vmType, typeof(VmType)),
        };

        if (result.IsSuccessStatusCode && timeout > 0)
        {
            await WaitAndVerifyTaskAsync(client, node, vmType, vmId, "Clone", result, wait, timeout);
        }

        return (resolvedVmId, result);
    }

    /// <summary>
    /// Converts a stopped QEMU VM or LXC container into a template.
    /// </summary>
    /// <param name="client">The Proxmox VE client.</param>
    /// <param name="node">Node name.</param>
    /// <param name="vmType">Guest type (Qemu or Lxc).</param>
    /// <param name="vmId">VM or container ID.</param>
    /// <param name="disk">Optional specific disk slot to convert (QEMU only).</param>
    /// <param name="timeout">Timeout in milliseconds to wait if a background task UPID is returned. Default 60,000. 0 to not wait.</param>
    /// <param name="wait">Polling interval in milliseconds when awaiting the task. Default 500.</param>
    /// <returns>The Result of the template conversion operation.</returns>
    public static async Task<Result> ConvertToTemplateAsync(
        PveClient client,
        string node,
        VmType vmType,
        long vmId,
        string disk = null,
        long timeout = 60000,
        int wait = 500)
    {
        ArgumentNullException.ThrowIfNull(client);
        if (string.IsNullOrWhiteSpace(node)) { throw new ArgumentException("Node cannot be null or empty.", nameof(node)); }
        if (vmId <= 0) { throw new ArgumentOutOfRangeException(nameof(vmId), "VM ID must be greater than zero."); }
        if (vmType != VmType.Qemu && vmType != VmType.Lxc)
        {
            throw new InvalidEnumArgumentException(nameof(vmType), (int)vmType, typeof(VmType));
        }

        if (vmType == VmType.Lxc && !string.IsNullOrWhiteSpace(disk))
        {
            throw new ArgumentException("Disk parameter is only supported for QEMU VMs.", nameof(disk));
        }

        node = node.Trim();

        var result = vmType switch
        {
            VmType.Qemu => await client.Nodes[node].Qemu[vmId].Template.Template(disk),
            VmType.Lxc => await client.Nodes[node].Lxc[vmId].Template.Template(),
            _ => throw new InvalidEnumArgumentException(nameof(vmType), (int)vmType, typeof(VmType)),
        };

        if (result.IsSuccessStatusCode && timeout > 0)
        {
            await WaitAndVerifyTaskAsync(client, node, vmType, vmId, "Convert to template", result, wait, timeout);
        }

        return result;
    }

    private static async Task WaitAndVerifyTaskAsync(
        PveClient client,
        string node,
        VmType vmType,
        long vmId,
        string operation,
        Result result,
        int wait,
        long timeout)
    {
        var upid = PveClientBase.GetTaskFromResult(result);
        if (string.IsNullOrEmpty(upid))
        {
            return;
        }

        var finished = await client.WaitForTaskToFinishAsync(result, wait, timeout);
        if (!finished)
        {
            throw new TimeoutException($"Timed out waiting {timeout}ms for {operation.ToLowerInvariant()} task of {vmType} {vmId} on node '{node}' to finish.");
        }

        var exitStatus = await client.GetExitStatusTaskAsync(upid);
        if (!string.Equals(exitStatus, "OK", StringComparison.OrdinalIgnoreCase) &&
            !(exitStatus?.StartsWith("WARNINGS", StringComparison.OrdinalIgnoreCase) ?? false))
        {
            throw new PveResultException(result, $"{operation} task for {vmType} {vmId} failed with exit status: {exitStatus}");
        }
    }
}
