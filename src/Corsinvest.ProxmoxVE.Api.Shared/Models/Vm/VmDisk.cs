/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
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
    /// Size
    /// </summary>
    public string Size { get; set; }

    /// <summary>
    /// Backup enabled.
    /// </summary>
    public bool Backup { get; set; }
}