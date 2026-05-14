/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

/// <summary>
/// Kind of volume entry parsed from a VM/CT config.
/// Orthogonal to <see cref="VmDisk.IsUnused"/> (which marks detached data disks).
/// </summary>
public enum VmDiskKind
{
    /// <summary>
    /// Real data disk: rootfs, scsi*, virtio*, sata*, ide*, mp*, efidisk*, tpmstate*, unusedN.
    /// </summary>
    Disk,

    /// <summary>
    /// CD-ROM / ISO drive (config contains <c>media=cdrom</c>).
    /// </summary>
    Cdrom,

    /// <summary>
    /// Cloud-init drive — a CD-ROM whose filename matches <c>vm-{vmid}-cloudinit</c>.
    /// Auto-created by Proxmox when a cloud-init drive is attached.
    /// </summary>
    CloudInit,
}
