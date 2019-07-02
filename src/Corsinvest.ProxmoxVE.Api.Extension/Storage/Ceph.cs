using Corsinvest.ProxmoxVE.Api.Extension.Utils;

namespace Corsinvest.ProxmoxVE.Api.Extension.Storage
{
    public class Ceph : StorageInfo
    {
        internal Ceph(Client client, object apiData) : base(client, apiData, StorageTypeEnum.Ceph)
        {
            JsonHelper.GetValueOrCreate(apiData, "monhost", "");
        }

        public string Pool => ApiData.pool;
        public string MonitorHosts => ApiData.monhost;
        public string Username => ApiData.username;
        public bool Krbd => ApiData.krbd == 1;
    }
}