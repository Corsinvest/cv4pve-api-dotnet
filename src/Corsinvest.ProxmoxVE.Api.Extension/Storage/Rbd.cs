using Corsinvest.ProxmoxVE.Api.Extension.Utils;

namespace Corsinvest.ProxmoxVE.Api.Extension.Storage
{
    /// <summary>
    /// Rbd storage
    /// </summary>
    public class Rbd : StorageInfo
    {
        internal Rbd(Client client, object apiData) : base(client, apiData, StorageTypeEnum.Rbd)
        {
            JsonHelper.GetValueOrCreate(apiData, "monhost", "");
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