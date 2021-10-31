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