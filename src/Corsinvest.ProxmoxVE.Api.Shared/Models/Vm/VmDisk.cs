/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;

/// <summary>
/// Disk
/// </summary>
public class VmDisk
{
    /// <summary>
    /// Identifier
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Kind of volume entry. Defaults to <see cref="VmDiskKind.Disk"/>.
    /// CD-ROM and cloud-init drives are not exposed in <see cref="VmConfig.Disks"/>
    /// but appear in <see cref="VmConfig.DisksAll"/>.
    /// </summary>
    public VmDiskKind Kind { get; set; }

    /// <summary>
    /// Storage
    /// </summary>
    public string Storage { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Device
    /// </summary>
    public string Device { get; set; }

    /// <summary>
    /// Passthrough
    /// </summary>
    public bool Passthrough { get; set; }

    /// <summary>
    /// Mount Point
    /// </summary>
    public string MountPoint { get; set; }

    /// <summary>
    /// Mount Source Path
    /// </summary>
    public string MountSourcePath { get; set; }

    /// <summary>
    /// Size (PVE format, e.g. "32G", "500M")
    /// </summary>
    public string Size { get; set; }

    /// <summary>
    /// Size in bytes, parsed from <see cref="Size"/>. Returns 0 if not available or not parseable.
    /// </summary>
    public long SizeBytes => Utils.ByteHelper.ParsePveSize(Size);

    /// <summary>
    /// Disk cache mode (e.g. "none", "writeback", "unsafe", "directsync", "writethrough").
    /// </summary>
    public string Cache { get; set; }

    /// <summary>
    /// True if this disk is detached (unused) from the VM config but still present in storage.
    /// </summary>
    public bool IsUnused { get; set; }

    /// <summary>
    /// Backup enabled.
    /// </summary>
    public bool Backup { get; set; }

    /// <summary>
    /// Raw definition string from the VM config (e.g. "local-zfs:vm-100-disk-0,size=32G,cache=writeback").
    /// Useful for accessing keys not yet parsed into dedicated properties.
    /// </summary>
    public string RawDefinition { get; set; }

    /// <summary>
    /// Disk format (e.g. "qcow2", "raw", "vmdk"). Parsed from <see cref="RawDefinition"/>.
    /// </summary>
    public string Format { get; set; }

    /// <summary>
    /// Preallocation mode (e.g. "1" for preallocated). Parsed from <see cref="RawDefinition"/>.
    /// </summary>
    public string Prealloc { get; set; }
}