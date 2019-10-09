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
 
using System.Linq;
using Corsinvest.ProxmoxVE.Api.Extension.Storage;

namespace Corsinvest.ProxmoxVE.Api.Extension.VM
{
    /// <summary>
    /// Disk
    /// </summary>
    public abstract class Disk
    {
        /// <summary>
        /// Client API.
        /// </summary>
        protected PveClient Client { get; }

        internal Disk(PveClient client, string id, string definition)
        {
            Client = client;
            Id = id;
            Definition = definition;
            Backup = true;

            var data = definition.Split(':');
            Storage = data[0];

            data = data[1].Split(',');
            Name = data[0];
            Size = data.Where(a => a.StartsWith("size=")).Select(a => a.Substring(5)).FirstOrDefault();
        }

        /// <summary>
        /// dEFINITION
        /// </summary>
        /// <value></value>
        public string Definition { get; }

        /// <summary>
        /// Identifier
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Storage
        /// </summary>
        /// <value></value>
        public string Storage { get; }

        private StorageInfo _storageInfo;
        /// <summary>
        /// Storage info
        /// </summary>
        /// <returns></returns>
        public StorageInfo StorageInfo => _storageInfo ?? (_storageInfo = Client.GetStorages().FirstOrDefault(a => a.Id == Storage));

        /// <summary>
        /// Name
        /// </summary>
        /// <value></value>
        public string Name { get; }

        /// <summary>
        /// Size
        /// </summary>
        public string Size { get; }

        /// <summary>
        /// Backup enabled.
        /// </summary>
        public bool Backup { get; protected set; }
    }
}