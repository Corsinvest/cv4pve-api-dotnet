namespace EnterpriseVE.ProxmoxVE.Api.Extension.Storage
{
    public class NFS : StorageInfo
    {
        internal NFS(Client client, object apiData) : base(client, apiData, StorageTypeEnum.NFS) { }
        public string Path => ApiData.path;
        public string Nodes => ApiData.nodes;
        public string Options => ApiData.options;
        public string Server => ApiData.server;
        public string Export => ApiData.export;
        public int Maxfiles => ApiData.maxfiles;
    }
}