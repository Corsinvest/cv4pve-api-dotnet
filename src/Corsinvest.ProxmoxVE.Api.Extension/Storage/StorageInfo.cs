using Corsinvest.ProxmoxVE.Api.Extension.Utils;

namespace Corsinvest.ProxmoxVE.Api.Extension.Storage
{
    /// <summary>
    /// Storage info
    /// </summary>
    public abstract class StorageInfo : BaseInfo
    {
        internal StorageInfo(Client client, object apiData, StorageTypeEnum type) : base(client, apiData)
        {
            Type = type;
            JsonHelper.GetValueOrCreate(apiData, "disable", "0");
        }

        // public bool Active =>  Storage.active == 1;
        /// <summary>
        /// Disabled
        /// </summary>
        public bool Disabled => ApiData.disable == "1";

        /// <summary>
        /// Shared
        /// </summary>
        public bool Shared => ApiData.shared == "1";

        /// <summary>
        /// Identifier
        /// </summary>
        public string Id => ApiData.storage;

        /// <summary>
        /// Digest
        /// </summary>
        public string Digest => ApiData.digest;

        /// <summary>
        /// Type String
        /// </summary>
        public string TypeString => ApiData.type;

        /// <summary>
        /// Content
        /// </summary>
        public string Content => ApiData.content;

        /// <summary>
        /// Type storage
        /// </summary>
        /// <value></value>
        public StorageTypeEnum Type { get; }
    }
}