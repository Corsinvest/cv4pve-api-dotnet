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
 
using System.Collections.Generic;

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
        public static IReadOnlyList<StorageInfo> GetStorages(this PveClient client)
        {
            var storages = new List<StorageInfo>();
            foreach (var apiData in client.Storage.GetRest().Response.data)
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