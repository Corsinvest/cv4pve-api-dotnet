using Corsinvest.ProxmoxVE.Api.Extension.Utils;

namespace Corsinvest.ProxmoxVE.Api.Extension.Storage
{
    /// <summary>
    /// Ceph storage
    /// </summary>
    public class Ceph : StorageInfo
    {
        internal Ceph(Client client, object apiData) : base(client, apiData, StorageTypeEnum.Ceph)
        {
            JsonHelper.GetValueOrCreate(apiData, "monhost", "");
        }

        /// <summary>
        /// Pool
        /// </summary>
        public string Pool => ApiData.pool;

        /// <summary>
        /// Monitorh hosts
        /// </summary>
        public string MonitorHosts => ApiData.monhost;

        /// <summary>
        /// Username
        /// </summary>
        public string Username => ApiData.username;

        /// <summary>
        /// Krbf
        /// </summary>
        public bool Krbd => ApiData.krbd == 1;
    }
}