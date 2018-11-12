namespace EnterpriseVE.ProxmoxVE.Api.Extension.Storage
{
    public class Unknown : StorageInfo
    {
        internal Unknown(Client client, object apiData) : base(client, apiData, StorageTypeEnum.Unknown) { }
    }
}