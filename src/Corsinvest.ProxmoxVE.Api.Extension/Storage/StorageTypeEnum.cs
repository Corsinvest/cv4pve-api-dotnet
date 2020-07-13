/*
 * This file is part of the cv4pve-api-dotnet https://github.com/Corsinvest/cv4pve-api-dotnet,
 *
 * This source file is available under two different licenses:
 * - GNU General Public License version 3 (GPLv3)
 * - Corsinvest Enterprise License (CEL)
 * Full copyright and license information is available in
 * LICENSE.md which is distributed with this source code.
 *
 * Copyright (C) 2016 Corsinvest Srl	GPLv3 and CEL
 */

namespace Corsinvest.ProxmoxVE.Api.Extension.Storage
{
    /// <summary>
    /// Type storage
    /// </summary>
    public enum StorageTypeEnum
    {
        /// <summary>
        /// Rbd
        /// </summary>
        Rbd,

        /// <summary>
        /// ZFS
        /// </summary>
        ZFS,

        /// <summary>
        /// Directory
        /// </summary>
        Dir,

        /// <summary>
        /// Network file system
        /// </summary>
        NFS,

        /// <summary>
        /// Unknown
        /// </summary>
        Unknown
    }
}