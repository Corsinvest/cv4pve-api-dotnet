using EnterpriseVE.ProxmoxVE.Api;

namespace EnterpriseVE.ProxmoxVE.Api.Extension.Storage
{
    public abstract class StorageInfo : BaseInfo
    {
        internal StorageInfo(Client client, object apiData, StorageTypeEnum type)
            : base(client, apiData)
        {
            Type = type;
        }

        // public bool Active =>  Storage.active == 1;
        // public bool Enabled =>  Storage.enabled == 1;
        public bool Shared => ApiData.shared == 1;
        public string Id => ApiData.storage;
        public string Digest => ApiData.digest;
        public string TypeString => ApiData.type;
        public string Content => ApiData.content;
        public StorageTypeEnum Type { get; }
    }
}