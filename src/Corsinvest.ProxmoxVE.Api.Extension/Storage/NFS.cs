namespace Corsinvest.ProxmoxVE.Api.Extension.Storage
{
    /// <summary>
    /// NFS Storage.
    /// </summary>
    public class NFS : StorageInfo
    {
        internal NFS(Client client, object apiData) : base(client, apiData, StorageTypeEnum.NFS) { }

        /// <summary>
        /// Path.
        /// </summary>
        public string Path => ApiData.path;

        /// <summary>
        /// Nodes
        /// </summary>
        public string Nodes => ApiData.nodes;

        /// <summary>
        /// Options
        /// </summary>
        public string Options => ApiData.options;

        /// <summary>
        /// Server
        /// </summary>
        public string Server => ApiData.server;

        /// <summary>
        /// Export
        /// </summary>
        public string Export => ApiData.export;

        /// <summary>
        /// Max files.
        /// </summary>
        public int Maxfiles => ApiData.maxfiles;
    }
}