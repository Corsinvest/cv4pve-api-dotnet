namespace Corsinvest.ProxmoxVE.Api.Extension
{
    /// <summary>
    /// Info base object.
    /// </summary>
    public class BaseInfo
    {
        internal Client Client { get; }

        /// <summary>
        /// Data Json API.
        /// </summary>
        /// <value></value>
        protected dynamic ApiData { get; }

        internal BaseInfo(Client client, object apiData) => (Client, ApiData) = (client, apiData);
    }
}