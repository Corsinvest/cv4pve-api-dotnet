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
    /// NFS Storage.
    /// </summary>
    public class NFS : StorageInfo
    {
        internal NFS(PveClient client, object apiData) : base(client, apiData, StorageTypeEnum.NFS) { }

        /// <summary>
        /// Path.
        /// </summary>
        public string Path => ApiData.path;

        /// <summary>
        /// Nodes
        /// </summary>
        public string Nodes => ApiData.nodes;

        /// <summary>
        /// Options
        /// </summary>
        public string Options => ApiData.options;

        /// <summary>
        /// Server
        /// </summary>
        public string Server => ApiData.server;

        /// <summary>
        /// Export
        /// </summary>
        public string Export => ApiData.export;

        /// <summary>
        /// Max files.
        /// </summary>
        public int Maxfiles => ApiData.maxfiles;
    }
}