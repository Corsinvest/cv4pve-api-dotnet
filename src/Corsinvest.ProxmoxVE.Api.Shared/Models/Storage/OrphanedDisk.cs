/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

#nullable enable

using Corsinvest.ProxmoxVE.Api.Shared.Utils;

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Storage;

/// <summary>
/// Information about an orphaned or dangling disk volume on Proxmox VE storage.
/// </summary>
public class OrphanedDisk : ModelBase
{
    /// <summary>
    /// Storage identifier (e.g. "local-lvm", "ceph").
    /// </summary>
    public string Storage { get; set; } = string.Empty;

    /// <summary>
    /// Full volume identifier (e.g. "local-lvm:vm-100-disk-0").
    /// </summary>
    public string Volume { get; set; } = string.Empty;

    /// <summary>
    /// Volume file or image name (e.g. "vm-100-disk-0").
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Associated VM or Container ID.
    /// </summary>
    public long VmId { get; set; }

    /// <summary>
    /// Storage content type ("images" or "rootdir").
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Format identifier ('raw', 'qcow2', 'subvol' ...).
    /// </summary>
    public string Format { get; set; } = string.Empty;

    /// <summary>
    /// Volume size in bytes.
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// Human-readable volume size info.
    /// </summary>
    public string SizeInfo => FormatHelper.FromBytes(Size);

    /// <summary>
    /// Node where the storage volume was scanned.
    /// </summary>
    public string Node { get; set; } = string.Empty;

    /// <summary>
    /// Parent volume identifier if this volume is a linked clone.
    /// </summary>
    public string? Parent { get; set; }
}
