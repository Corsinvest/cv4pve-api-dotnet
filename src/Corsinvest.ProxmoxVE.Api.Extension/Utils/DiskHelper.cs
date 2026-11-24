/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.ComponentModel;
using System.Text.RegularExpressions;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Storage;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils;

/// <summary>
/// Helper for managing VM and LXC container disk attachments, slots, and unused disks.
/// </summary>
public static partial class DiskHelper
{
    private static readonly IReadOnlyDictionary<string, int> QemuBusLimits = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        { "scsi", 31 },
        { "virtio", 16 },
        { "sata", 6 },
        { "ide", 4 },
    };

    private const int LxcMountPointLimit = 256;

    [GeneratedRegex(@"^mp(?<index>\d+)$", RegexOptions.IgnoreCase)]
    private static partial Regex LxcSlotRegex();

    [GeneratedRegex(@"^(?<bus>scsi|virtio|sata|ide)(?<index>\d+)$", RegexOptions.IgnoreCase)]
    private static partial Regex QemuSlotRegex();

    [GeneratedRegex(@"^(?:vm|subvol|base)-(?<vmid>\d+)-disk-", RegexOptions.IgnoreCase)]
    private static partial Regex VmDiskVolumeRegex();

    /// <summary>
    /// Finds the first free disk slot or mount point for a VM or LXC container, checking both current and pending configurations.
    /// </summary>
    /// <param name="client">The Proxmox VE client.</param>
    /// <param name="node">Node name.</param>
    /// <param name="vmType">VM type (Qemu or Lxc).</param>
    /// <param name="vmId">VM or Container ID.</param>
    /// <param name="bus">Bus type to find free slot for. Defaults to null, which selects "scsi" for QEMU and "mp" for LXC.
    /// For QEMU, valid buses are "scsi" (0..30), "virtio" (0..15), "sata" (0..5), and "ide" (0..3).
    /// For LXC, valid bus is "mp" (0..255).</param>
    /// <returns>First free slot identifier (e.g. "scsi0", "virtio1", "mp0").</returns>
    /// <exception cref="ArgumentException">Thrown when an invalid bus is requested.</exception>
    /// <exception cref="InvalidOperationException">Thrown when all available slots for the specified bus are occupied.</exception>
    /// <exception cref="InvalidEnumArgumentException">Thrown when vmType is not supported.</exception>
    public static async Task<string> GetFreeSlotAsync(
        PveClient client,
        string node,
        VmType vmType,
        long vmId,
        string bus = null)
    {
        bus ??= vmType switch
        {
            VmType.Qemu => "scsi",
            VmType.Lxc => "mp",
            _ => throw new InvalidEnumArgumentException(nameof(vmType), (int)vmType, typeof(VmType)),
        };

        if (string.IsNullOrWhiteSpace(bus))
        {
            throw new ArgumentException("Bus cannot be null or empty.", nameof(bus));
        }

        var normalizedBus = bus.Trim().ToLowerInvariant();
        int maxSlots;

        switch (vmType)
        {
            case VmType.Qemu:
                if (!QemuBusLimits.TryGetValue(normalizedBus, out maxSlots))
                {
                    throw new ArgumentException($"Invalid bus '{bus}' for QEMU VM. Supported buses are scsi, virtio, sata, ide.", nameof(bus));
                }
                break;

            case VmType.Lxc:
                if (normalizedBus != "mp")
                {
                    throw new ArgumentException($"Invalid bus '{bus}' for LXC container. Supported bus is mp.", nameof(bus));
                }
                maxSlots = LxcMountPointLimit;
                break;

            default:
                throw new InvalidEnumArgumentException(nameof(vmType), (int)vmType, typeof(VmType));
        }

        var config = await client.GetVmConfigAsync(node, vmType, vmId);
        var occupiedSlots = new HashSet<int>();

        if (config?.DisksAll != null)
        {
            foreach (var disk in config.DisksAll)
            {
                if (!string.IsNullOrEmpty(disk.Id) &&
                    disk.Id.StartsWith(normalizedBus, StringComparison.OrdinalIgnoreCase) &&
                    int.TryParse(disk.Id[normalizedBus.Length..], out var slotIdx))
                {
                    occupiedSlots.Add(slotIdx);
                }
            }
        }

        var pending = vmType switch
        {
            VmType.Qemu => await client.Nodes[node].Qemu[vmId].Pending.GetAsync(),
            VmType.Lxc => await client.Nodes[node].Lxc[vmId].Pending.GetAsync(),
            _ => throw new InvalidEnumArgumentException(nameof(vmType), (int)vmType, typeof(VmType)),
        };

        if (pending != null)
        {
            foreach (var item in pending)
            {
                if (!string.IsNullOrEmpty(item?.Key) &&
                    item.Key.StartsWith(normalizedBus, StringComparison.OrdinalIgnoreCase) &&
                    int.TryParse(item.Key[normalizedBus.Length..], out var pendingSlot))
                {
                    occupiedSlots.Add(pendingSlot);
                }
            }
        }

        for (var i = 0; i < maxSlots; i++)
        {
            if (!occupiedSlots.Contains(i))
            {
                return $"{normalizedBus}{i}";
            }
        }

        throw new InvalidOperationException($"No free {normalizedBus.ToUpperInvariant()} slots available on {vmType} {vmId} ({normalizedBus}0..{normalizedBus}{maxSlots - 1} are occupied).");
    }

    /// <summary>
    /// Attaches a disk volume to a VM or LXC container.
    /// </summary>
    /// <param name="client">The Proxmox VE client.</param>
    /// <param name="node">Node name.</param>
    /// <param name="vmType">VM type (Qemu or Lxc).</param>
    /// <param name="vmId">VM or Container ID.</param>
    /// <param name="volume">Volume identifier (e.g. "local-zfs:vm-100-disk-0" or "local-lvm:subvol-200-disk-1").</param>
    /// <param name="slot">Target slot name (e.g. "scsi0", "virtio1", "mp0"). If null, automatically assigned using <see cref="GetFreeSlotAsync"/>.</param>
    /// <param name="mountPath">Mount path inside the container (e.g. "/mnt/data"). Required for LXC containers.</param>
    /// <param name="options">Additional disk or mount point options (e.g. "backup=0,ssd=1" or "ro=1").</param>
    /// <param name="digest">Optional configuration digest to prevent concurrent modifications.</param>
    /// <returns>Result of the attach operation.</returns>
    /// <exception cref="ArgumentException">Thrown when required parameters are missing or an invalid slot format is passed.</exception>
    /// <exception cref="InvalidEnumArgumentException">Thrown when vmType is not supported.</exception>
    public static async Task<Result> AttachDiskAsync(
        PveClient client,
        string node,
        VmType vmType,
        long vmId,
        string volume,
        string slot = null,
        string mountPath = null,
        string options = null,
        string digest = null)
    {
        if (string.IsNullOrWhiteSpace(volume))
        {
            throw new ArgumentException("Volume cannot be null or empty.", nameof(volume));
        }

        volume = volume.Trim();
        mountPath = mountPath?.Trim();
        options = options?.Trim().TrimStart(',');
        if (string.IsNullOrWhiteSpace(options))
        {
            options = null;
        }

        slot = slot?.Trim();
        if (string.IsNullOrWhiteSpace(slot))
        {
            slot = await GetFreeSlotAsync(client, node, vmType, vmId);
        }

        switch (vmType)
        {
            case VmType.Lxc:
            {
                if (string.IsNullOrWhiteSpace(mountPath))
                {
                    throw new ArgumentException("Mount path is required when attaching a disk to an LXC container.", nameof(mountPath));
                }

                var match = LxcSlotRegex().Match(slot);
                if (!match.Success || !int.TryParse(match.Groups["index"].Value, out var mpIdx) || mpIdx < 0 || mpIdx >= LxcMountPointLimit)
                {
                    throw new ArgumentException($"Invalid mount point slot '{slot}'. Expected mpN (e.g. mp0..mp{LxcMountPointLimit - 1}).", nameof(slot));
                }

                var mpValue = $"{volume},mp={mountPath}";
                if (!string.IsNullOrWhiteSpace(options))
                {
                    mpValue += $",{options}";
                }

                var mpN = new Dictionary<int, string> { { mpIdx, mpValue } };
                return await client.Nodes[node].Lxc[vmId].Config.UpdateVm(mpN: mpN, digest: digest);
            }

            case VmType.Qemu:
            {
                var match = QemuSlotRegex().Match(slot);
                if (!match.Success)
                {
                    throw new ArgumentException($"Invalid bus slot '{slot}'. Expected scsiN, virtioN, sataN, or ideN.", nameof(slot));
                }

                var bus = match.Groups["bus"].Value.ToLowerInvariant();
                var idx = int.Parse(match.Groups["index"].Value);
                if (!QemuBusLimits.TryGetValue(bus, out var maxSlots))
                {
                    throw new ArgumentException($"Invalid bus '{bus}'.", nameof(slot));
                }

                if (idx < 0 || idx >= maxSlots)
                {
                    throw new ArgumentException($"Slot index {idx} exceeds maximum allowed slots for bus '{bus}' (0..{maxSlots - 1}).", nameof(slot));
                }

                var diskValue = string.IsNullOrWhiteSpace(options) ? volume : $"{volume},{options}";
                var diskDict = new Dictionary<int, string> { { idx, diskValue } };

                return bus switch
                {
                    "scsi" => await client.Nodes[node].Qemu[vmId].Config.UpdateVmAsync(scsiN: diskDict, digest: digest),
                    "virtio" => await client.Nodes[node].Qemu[vmId].Config.UpdateVmAsync(virtioN: diskDict, digest: digest),
                    "sata" => await client.Nodes[node].Qemu[vmId].Config.UpdateVmAsync(sataN: diskDict, digest: digest),
                    "ide" => await client.Nodes[node].Qemu[vmId].Config.UpdateVmAsync(ideN: diskDict, digest: digest),
                    _ => throw new ArgumentException($"Unsupported bus '{bus}'.", nameof(slot)),
                };
            }

            default:
                throw new InvalidEnumArgumentException(nameof(vmType), (int)vmType, typeof(VmType));
        }
    }

    /// <summary>
    /// Detaches a disk slot or mount point from a VM or LXC container.
    /// </summary>
    /// <param name="client">The Proxmox VE client.</param>
    /// <param name="node">Node name.</param>
    /// <param name="vmType">VM type (Qemu or Lxc).</param>
    /// <param name="vmId">VM or Container ID.</param>
    /// <param name="slot">Slot name to detach (e.g. "scsi0", "virtio1", "mp0").</param>
    /// <param name="digest">Optional configuration digest to prevent concurrent modifications.</param>
    /// <returns>Result of the detach operation.</returns>
    /// <exception cref="ArgumentException">Thrown when slot is null or empty.</exception>
    /// <exception cref="InvalidEnumArgumentException">Thrown when vmType is not supported.</exception>
    public static async Task<Result> DetachDiskAsync(
        PveClient client,
        string node,
        VmType vmType,
        long vmId,
        string slot,
        string digest = null)
    {
        if (string.IsNullOrWhiteSpace(slot))
        {
            throw new ArgumentException("Slot cannot be null or empty.", nameof(slot));
        }

        slot = slot.Trim();

        return vmType switch
        {
            VmType.Qemu => await client.Nodes[node].Qemu[vmId].Config.UpdateVmAsync(delete: slot, digest: digest),
            VmType.Lxc => await client.Nodes[node].Lxc[vmId].Config.UpdateVm(delete: slot, digest: digest),
            _ => throw new InvalidEnumArgumentException(nameof(vmType), (int)vmType, typeof(VmType)),
        };
    }

    /// <summary>
    /// Removes an unused disk volume entry from a VM or LXC container configuration.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>DESTRUCTIVE OPERATION:</b> On both Proxmox VE QEMU VMs and LXC containers, deleting an <c>unused[n]</c>
    /// configuration entry triggers physical removal and permanent deletion of the underlying disk volume from storage.
    /// </para>
    /// </remarks>
    /// <param name="client">The Proxmox VE client.</param>
    /// <param name="node">Node name.</param>
    /// <param name="vmType">VM type (Qemu or Lxc).</param>
    /// <param name="vmId">VM or Container ID.</param>
    /// <param name="volume">Full volume identifier to remove (e.g. "local-zfs:vm-100-disk-1" or "local-lvm:subvol-200-disk-0").</param>
    /// <param name="digest">Optional configuration digest to prevent concurrent modifications.</param>
    /// <returns>Result of the removal operation.</returns>
    /// <exception cref="ArgumentException">Thrown when volume is null, empty, or cannot be found among unused disks.</exception>
    /// <exception cref="InvalidEnumArgumentException">Thrown when vmType is not supported.</exception>
    public static async Task<Result> RemoveUnusedDiskAsync(
        PveClient client,
        string node,
        VmType vmType,
        long vmId,
        string volume,
        string digest = null)
    {
        if (string.IsNullOrWhiteSpace(volume))
        {
            throw new ArgumentException("Volume cannot be null or empty.", nameof(volume));
        }

        volume = volume.Trim();

        if (vmType != VmType.Qemu && vmType != VmType.Lxc)
        {
            throw new InvalidEnumArgumentException(nameof(vmType), (int)vmType, typeof(VmType));
        }

        var config = await client.GetVmConfigAsync(node, vmType, vmId);
        var matched = config?.Disks?.FirstOrDefault(d => d.IsUnused && $"{d.Storage}:{d.FileName}" == volume);

        if (matched == null)
        {
            throw new ArgumentException($"Unused disk '{volume}' not found on {vmType} {vmId}.", nameof(volume));
        }

        var slot = matched.Id;

        return vmType switch
        {
            VmType.Qemu => await client.Nodes[node].Qemu[vmId].Config.UpdateVmAsync(delete: slot, digest: digest),
            VmType.Lxc => await client.Nodes[node].Lxc[vmId].Config.UpdateVm(delete: slot, digest: digest),
            _ => throw new InvalidEnumArgumentException(nameof(vmType), (int)vmType, typeof(VmType)),
        };
    }

    /// <summary>
    /// Scans storages for orphaned or dangling VM and container disk volumes whose VM/CT ID
    /// no longer exists in the Proxmox VE cluster.
    /// </summary>
    /// <remarks>
    /// Every guest listed in the cluster resources counts as existing, whatever its status: a guest on an
    /// offline node is reported as unknown but still owns its disks.
    /// A base volume that still has linked clones is never reported.
    /// The account needs VM.Audit on /vms: guests it cannot audit are filtered out of the cluster resources
    /// without an error, so their disks would look orphaned.
    /// </remarks>
    /// <param name="client">The Proxmox VE client.</param>
    /// <param name="node">Optional node name to limit scanning. If null, scans every node.</param>
    /// <param name="storage">Optional storage identifier to limit scanning. If null, scans all available storages holding disk images or container volumes.</param>
    /// <returns>A read-only list of orphaned disk volumes.</returns>
    /// <exception cref="InvalidOperationException">The account lacks VM.Audit on /vms.</exception>
    /// <exception cref="PveResultException">A storage content cannot be read.</exception>
    public static async Task<IReadOnlyList<OrphanedDisk>> GetOrphanedDisksAsync(
        PveClient client,
        string node = null,
        string storage = null)
    {
        ArgumentNullException.ThrowIfNull(client);

        var permissions = await client.Access.Permissions.GetPermissionsAsync("/vms");
        if (!permissions.TryGetValue("/vms", out var privileges)
            || !privileges.Contains("VM.Audit", StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("VM.Audit on /vms is required: guests the account cannot audit are hidden from the cluster resources and their disks would look orphaned.");
        }

        node = string.IsNullOrWhiteSpace(node) ? null : node.Trim();
        storage = string.IsNullOrWhiteSpace(storage) ? null : storage.Trim();

        var resources = (await client.Cluster.Resources.GetAsync()).ToList();
        var activeVmIds = resources.Where(a => a.ResourceType == ClusterResourceType.Vm)
                                   .Select(a => a.VmId)
                                   .ToHashSet();

        // One entry per storage and node: a shared storage is read once, a local one on each node that has it.
        var targetStorages = resources.Where(a => a.ResourceType == ClusterResourceType.Storage
                                                  && a.IsAvailable
                                                  && (node == null || a.Node == node)
                                                  && (storage == null || a.Storage == storage)
                                                  && a.Content != null
                                                  && a.Content.Split(',').Any(c => c is "images" or "rootdir"))
                                      .GroupBy(a => a.Shared ? a.Storage : $"{a.Node}/{a.Storage}")
                                      .Select(a => a.First());

        var orphanedDisks = new List<OrphanedDisk>();
        foreach (var item in targetStorages)
        {
            var disks = (await client.Nodes[item.Node].Storage[item.Storage].Content.GetAsync())
                            .Where(a => a.Content is "images" or "rootdir" && !string.IsNullOrEmpty(a.Volume))
                            .ToList();

            foreach (var disk in disks)
            {
                var vmid = disk.VmId;
                if (vmid <= 0)
                {
                    // A linked clone carries its base in the volume name (base-100-disk-0/vm-101-disk-0): read the last part
                    var match = VmDiskVolumeRegex().Match(disk.FileName.Split('/')[^1]);
                    if (match.Success) { vmid = long.Parse(match.Groups["vmid"].Value); }
                }

                if (vmid <= 0 || activeVmIds.Contains(vmid)) { continue; }

                // A base volume still in use: the volume of each linked clone starts with the volume of its base
                if (disks.Any(a => a.Volume.StartsWith(disk.Volume + "/", StringComparison.Ordinal))) { continue; }

                orphanedDisks.Add(new OrphanedDisk
                {
                    Storage = item.Storage,
                    Volume = disk.Volume,
                    FileName = disk.FileName,
                    VmId = vmid,
                    Content = disk.Content,
                    Format = disk.Format ?? string.Empty,
                    Size = disk.Size,
                    Node = item.Node,
                    Parent = disk.Parent,
                });
            }
        }

        return orphanedDisks;
    }

    /// <summary>
    /// Safely removes an orphaned disk volume from physical storage via the Proxmox storage content API.
    /// </summary>
    /// <param name="client">The Proxmox VE client.</param>
    /// <param name="node">Node name where storage is accessible.</param>
    /// <param name="storage">Storage identifier (e.g. "local-lvm", "ceph").</param>
    /// <param name="volume">Full volume identifier (e.g. "local-lvm:vm-100-disk-0") or volume name.</param>
    /// <param name="delay">Optional delay in seconds to wait for background task.</param>
    /// <returns>Result of the storage content delete API call.</returns>
    public static async Task<Result> RemoveOrphanedDiskAsync(
        PveClient client,
        string node,
        string storage,
        string volume,
        int? delay = null)
    {
        ArgumentNullException.ThrowIfNull(client);
        if (string.IsNullOrWhiteSpace(node)) { throw new ArgumentException("Node cannot be null or empty.", nameof(node)); }
        if (string.IsNullOrWhiteSpace(storage)) { throw new ArgumentException("Storage cannot be null or empty.", nameof(storage)); }
        if (string.IsNullOrWhiteSpace(volume)) { throw new ArgumentException("Volume cannot be null or empty.", nameof(volume)); }

        return await client.Nodes[node.Trim()].Storage[storage.Trim()].Content[volume.Trim()].Delete(delay: delay);
    }

    /// <summary>
    /// Scans and purges all orphaned disks across the specified or all storages in the cluster.
    /// Supports dry-run mode (default: true) to preview which volumes will be affected without deleting them.
    /// </summary>
    /// <param name="client">The Proxmox VE client.</param>
    /// <param name="node">Optional node name to limit scanning.</param>
    /// <param name="storage">Optional storage name to limit scanning.</param>
    /// <param name="dryRun">If true (default), returns orphaned disks without deleting them.</param>
    /// <param name="timeout">Timeout in milliseconds to wait for each delete task. Default 60,000 (1 minute).</param>
    /// <returns>List of results for each orphaned disk volume found. Removed is true only when the delete task ended with exit status OK.</returns>
    public static async Task<IReadOnlyList<(OrphanedDisk Disk, Result Result, bool Removed)>> PurgeOrphanedDisksAsync(
        PveClient client,
        string node = null,
        string storage = null,
        bool dryRun = true,
        long timeout = 60000)
    {
        var orphanedDisks = await GetOrphanedDisksAsync(client, node, storage);
        var results = new List<(OrphanedDisk Disk, Result Result, bool Removed)>();

        foreach (var disk in orphanedDisks)
        {
            if (dryRun)
            {
                results.Add((disk, null, false));
            }
            else
            {
                var result = await RemoveOrphanedDiskAsync(client, disk.Node, disk.Storage, disk.Volume);
                var removed = false;
                if (PveClientBase.GetTaskFromResult(result) is { } task
                    && await client.WaitForTaskToFinishAsync(task, timeout: timeout))
                {
                    removed = await client.GetExitStatusTaskAsync(task) == "OK";
                }
                results.Add((disk, result, removed));
            }
        }

        return results;
    }
}
