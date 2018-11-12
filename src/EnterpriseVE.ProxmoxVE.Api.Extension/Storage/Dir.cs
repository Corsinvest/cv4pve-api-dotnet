namespace EnterpriseVE.ProxmoxVE.Api.Extension.Storage
{
    public class Dir : StorageInfo
    {
        internal Dir(Client client, object apiData) : base(client, apiData, StorageTypeEnum.Dir) { }
        public string Path => ApiData.path;
        public string Nodes => ApiData.nodes;
        public int Maxfiles => ApiData.maxfiles;
    }
}