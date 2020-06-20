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
        public StorageInfo StorageInfo => _storageInfo ??= Client.GetStorages().FirstOrDefault(a => a.Id == Storage);

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