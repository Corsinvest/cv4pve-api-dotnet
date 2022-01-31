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

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Corsinvest.ProxmoxVE.Api.Extension.Storage
{
    /// <summary>
    /// Client extension for sotrage
    /// </summary>
    public static class ClientExtension
    {
        /// <summary>
        /// Get all astorage client.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task<IReadOnlyList<StorageInfo>> GetStorages(this PveClient client)
        {
            var storages = new List<StorageInfo>();
            foreach (var apiData in (await client.Storage.GetRest()).Response.data)
            {
                switch (apiData.type)
                {
                    case "rbd": storages.Add(new Rbd(client, apiData)); break;
                    case "dir": storages.Add(new Dir(client, apiData)); break;
                    case "nfs": storages.Add(new NFS(client, apiData)); break;
                    case "zfs": storages.Add(new ZFS(client, apiData)); break;

                    // cephfs cifs drbd glusterfs iscsi
                    // iscsidirect lvm lvmthin zfspool

                    default: storages.Add(new Unknown(client, apiData)); break;
                }
            }
            return storages.AsReadOnly();
        }
    }
}