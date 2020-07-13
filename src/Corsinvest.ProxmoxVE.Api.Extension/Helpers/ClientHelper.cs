using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Linq;

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
        /// <param name="pingTimeout"></param>
        public static PveClient GetClientFromHA(string hostsAndPortHA,
                                                TextWriter @out,
                                                int pingTimeout = 4000)
        {
            var data = GetHostsAndPorts(hostsAndPortHA, 8006, true, @out, pingTimeout);
            return data.Count() == 0 ?
                    null :
                    new PveClient(data[0].Host, data[0].Port);
        }

        /// <summary>
        /// GetHostAndPort
        /// Format 10.1.1.90:8006,10.1.1.91:8006,10.1.1.92:8006
        /// </summary>
        /// <param name="hostsAndPorts"></param>
        /// <param name="defaultPort"></param>
        /// <param name="checkPing"></param>
        /// <param name="out"></param>
        /// <param name="pingTimeout"></param>
        /// <returns></returns>
        public static (string Host, int Port)[] GetHostsAndPorts(string hostsAndPorts,
                                                                 int defaultPort,
                                                                 bool checkPing,
                                                                 TextWriter @out,
                                                                 int pingTimeout = 4000)
        {
            var ret = new List<(string Host, int port)>();
            foreach (var hostAndPort in hostsAndPorts.Split(','))
            {
                var data = hostAndPort.Split(':');
                var host = data[0];
                var port = defaultPort;
                if (data.Length == 2) { int.TryParse(data[1], out port); }

                var add = true;
                if (checkPing)
                {
                    using var ping = new Ping();
                    if (ping.Send(host, pingTimeout).Status != IPStatus.Success)
                    {
                        @out?.WriteLine($"Error: unknown host {host}");
                        add = false;
                    }
                }

                if (add) { ret.Add((host, port)); }
            }

            return ret.ToArray();
        }
    }
}