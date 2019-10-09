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
 
 using Corsinvest.ProxmoxVE.Api.Extension.Helpers;

namespace Corsinvest.ProxmoxVE.Api.Extension.Storage
{
    /// <summary>
    /// Storage info
    /// </summary>
    public abstract class StorageInfo : BaseInfo
    {
        internal StorageInfo(PveClient client, object apiData, StorageTypeEnum type) : base(client, apiData)
        {
            Type = type;
            DynamicHelper.CheckKeyOrCreate(apiData, "disable", "0");
        }

        // public bool Active =>  Storage.active == 1;
        /// <summary>
        /// Disabled
        /// </summary>
        public bool Disabled => ApiData.disable == "1";

        /// <summary>
        /// Shared
        /// </summary>
        public bool Shared => ApiData.shared == "1";

        /// <summary>
        /// Identifier
        /// </summary>
        public string Id => ApiData.storage;

        /// <summary>
        /// Digest
        /// </summary>
        public string Digest => ApiData.digest;

        /// <summary>
        /// Type String
        /// </summary>
        public string TypeString => ApiData.type;

        /// <summary>
        /// Content
        /// </summary>
        public string Content => ApiData.content;

        /// <summary>
        /// Type storage
        /// </summary>
        /// <value></value>
        public StorageTypeEnum Type { get; }
    }
}