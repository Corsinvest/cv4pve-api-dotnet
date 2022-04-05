/*
 * SPDX-FileCopyrightText: 2022 Daniele Corsini <daniele.corsini@corsinvest.it>
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

namespace Corsinvest.ProxmoxVE.Api.Shared.Models.Vm
{
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
        /// <value></value>
        public string Storage { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        /// <value></value>
        public string FileName { get; set; }

        /// <summary>
        /// Size
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// Backup enabled.
        /// </summary>
        public bool Backup { get; set; }
    }
}