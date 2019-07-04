namespace Corsinvest.ProxmoxVE.Api.Extension.Storage
{
    /// <summary>
    /// Directory storage
    /// </summary>
    public class Dir : StorageInfo
    {
        internal Dir(Client client, object apiData) : base(client, apiData, StorageTypeEnum.Dir) { }

        /// <summary>
        /// Path
        /// </summary>
        public string Path => ApiData.path;

        /// <summary>
        /// Nodes
        /// </summary>
        public string Nodes => ApiData.nodes;
        
        /// <summary>
        /// Max files.
        /// </summary>
        public int Maxfiles => ApiData.maxfiles;
    }
}