namespace EnterpriseVE.ProxmoxVE.Api.Extension.Storage
{
    public class ZFS : StorageInfo
    {
        internal ZFS(Client client, object apiData) : base(client, apiData, StorageTypeEnum.ZFS) { }
        public bool Sparse => ApiData.sparse == 1;
        public string Pool => ApiData.pool;
    }
}