namespace Corsinvest.ProxmoxVE.Api.Extension
{
    public class BaseInfo
    {
        internal Client Client { get; }
        protected dynamic ApiData { get; }
        internal BaseInfo(Client client, object apiData) => (Client, ApiData) = (client, apiData);
    }
}