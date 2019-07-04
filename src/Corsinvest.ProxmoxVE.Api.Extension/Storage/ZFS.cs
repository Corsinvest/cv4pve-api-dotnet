namespace Corsinvest.ProxmoxVE.Api.Extension.Storage
{
    /// <summary>
    /// ZFS storage.
    /// </summary>
    public class ZFS : StorageInfo
    {
        internal ZFS(Client client, object apiData) : base(client, apiData, StorageTypeEnum.ZFS) { }

        /// <summary>
        /// Sparse
        /// </summary>
        public bool Sparse => ApiData.sparse == 1;

        /// <summary>
        /// Pool
        /// </summary>
        public string Pool => ApiData.pool;
    }
}