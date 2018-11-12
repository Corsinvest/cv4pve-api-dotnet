namespace EnterpriseVE.ProxmoxVE.Api.Extension
{
    public class BaseInfo
    {
        internal Client Client { get; }
        internal dynamic ApiData { get; }

        internal BaseInfo(Client client, object apiData)
        {
            Client = client;
            ApiData = apiData;
        }
    }
}