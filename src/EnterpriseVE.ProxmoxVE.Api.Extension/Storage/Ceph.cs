namespace EnterpriseVE.ProxmoxVE.Api.Extension.Storage
{
    public class Ceph : StorageInfo
    {
        internal Ceph(Client client, object apiData) : base(client, apiData, StorageTypeEnum.Ceph) { }
        public string Pool => ApiData.pool;
        public bool Krbd => ApiData.krbd == 1;
    }
}