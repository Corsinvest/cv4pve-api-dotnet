/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.ComponentModel;
using System.Text.RegularExpressions;
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
}
