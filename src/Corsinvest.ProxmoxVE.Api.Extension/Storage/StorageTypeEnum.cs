namespace Corsinvest.ProxmoxVE.Api.Extension.Storage
{
    /// <summary>
    /// Type storage
    /// </summary>
    public enum StorageTypeEnum
    {
        /// <summary>
        /// Rbd
        /// </summary>
        Rbd,

        /// <summary>
        /// ZFS
        /// </summary>
        ZFS,

        /// <summary>
        /// Directory
        /// </summary>
        Dir,

        /// <summary>
        /// Network file system
        /// </summary>
        NFS,

        /// <summary>
        /// Unknown
        /// </summary>
        Unknown
    }
}