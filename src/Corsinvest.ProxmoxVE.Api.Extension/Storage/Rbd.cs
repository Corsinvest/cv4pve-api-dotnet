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
    /// Rbd storage
    /// </summary>
    public class Rbd : StorageInfo
    {
        internal Rbd(PveClient client, object apiData) : base(client, apiData, StorageTypeEnum.Rbd)
        {
            DynamicHelper.CheckKeyOrCreate(apiData, "monhost", "");
        }

        /// <summary>
        /// Pool
        /// </summary>
        public string Pool => ApiData.pool;

        /// <summary>
        /// Monitor hosts
        /// </summary>
        public string MonitorHosts => ApiData.monhost;

        /// <summary>
        /// Username
        /// </summary>
        public string Username => ApiData.username;

        /// <summary>
        /// Krbb
        /// </summary>
        public bool Krbd => ApiData.krbd == "1";
    }
}