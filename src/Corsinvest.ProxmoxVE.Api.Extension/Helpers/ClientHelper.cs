using System.IO;
using System.Net.NetworkInformation;

namespace Corsinvest.ProxmoxVE.Api.Extension.Helpers
{
    /// <summary>
    /// Client helper
    /// </summary>
    public class ClientHelper
    {
        /// <summary>
        /// Host and port for HA
        /// Format 10.1.1.90:8006,10.1.1.91:8006,10.1.1.92:8006
        /// </summary>
        /// <param name="hostsAndPortHA"></param>
        /// <param name="out"></param>
        public static PveClient GetClientFromHA(string hostsAndPortHA, TextWriter @out)
        {
            foreach (var hostAndPort in hostsAndPortHA.Split(','))
            {
                var data = hostAndPort.Split(':');
                var host = data[0];
                var port = 8006;
                if (data.Length == 2) { int.TryParse(data[1], out port); }

                using (var ping = new Ping())
                {
                    if (ping.Send(host).Status != IPStatus.Success)
                    {
                        @out?.WriteLine($"Error: try login unknown host {host}");
                        continue;
                    }
                }

                return new PveClient(host, port);
            }

            return null;
        }
    }
}