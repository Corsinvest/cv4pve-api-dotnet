/*
 * This file is part of the cv4pve-api-dotnet https://github.com/Corsinvest/cv4pve-api-dotnet,
 * Copyright (C) 2016 Corsinvest Srl
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
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