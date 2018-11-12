using System.Collections.Generic;

namespace EnterpriseVE.ProxmoxVE.Api.Extension.Storage
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
        public static IReadOnlyList<StorageInfo> GetStorages(this Client client)
        {
            var storages = new List<StorageInfo>();
            foreach (var apiData in client.Storage.GetRest().Response.data)
            {
                switch (apiData.type)
                {
                    case "rbd": storages.Add(new Ceph(client, apiData)); break;
                    case "dir": storages.Add(new Dir(client, apiData)); break;
                    case "nfs": storages.Add(new NFS(client, apiData)); break;
                    case "zfs": storages.Add(new ZFS(client, apiData)); break;
                    default: storages.Add(new Unknown(client, apiData)); break;
                }
            }
            return storages.AsReadOnly();
        }
    }
}