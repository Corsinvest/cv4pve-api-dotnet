namespace Corsinvest.ProxmoxVE.Api.Extension.Storage
{
    /// <summary>
    /// Unkown storage
    /// </summary>
    public class Unknown : StorageInfo
    {
        internal Unknown(Client client, object apiData) : base(client, apiData, StorageTypeEnum.Unknown) { }
    }
}