using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System;

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
        /// <param name="timeout"></param>
        public static PveClient GetClientFromHA(string hostsAndPortHA, TextWriter @out, int timeout = 4000)
        {
            var data = GetHostsAndPorts(hostsAndPortHA, 8006, true, @out, timeout);
            return data.Count() == 0 ? null : new PveClient(data[0].Host, data[0].Port);
        }

        /// <summary>
        /// GetHostAndPort
        /// Format 10.1.1.90:8006,10.1.1.91:8006,10.1.1.92:8006
        /// </summary>
        /// <param name="hostsAndPorts"></param>
        /// <param name="defaultPort"></param>
        /// <param name="checkPort"></param>
        /// <param name="out"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static (string Host, int Port)[] GetHostsAndPorts(string hostsAndPorts,
                                                                 int defaultPort,
                                                                 bool checkPort,
                                                                 TextWriter @out,
                                                                 int timeout = 4000)
        {
            var ret = new List<(string Host, int port)>();
            foreach (var hostAndPort in hostsAndPorts.Split(','))
            {
                var data = hostAndPort.Split(':');
                var host = data[0];
                var port = defaultPort;
                if (data.Length == 2) { int.TryParse(data[1], out port); }

                var add = true;
                if (checkPort)
                {
                    add = false;
                    try
                    {
                        using var tcpClient = new TcpClient();
                        var task = tcpClient.ConnectAsync(host, port);
                        if (task.Wait(timeout))
                        {
                            //if fails within timeout, task.Wait still returns true.
                            if (tcpClient.Connected)
                            {
                                add = true;
                            }
                            else
                            {
                                // connection refused probably
                                @out?.WriteLine($"Error: problem connection host {host} with port {port}");
                            }
                        }
                        else
                        {
                            // timed out
                            @out?.WriteLine($"Error: timeput problem connection host {host} with port {port}");
                        }
                    }
                    catch (Exception ex)
                    {
                        @out?.WriteLine($"Error: host {host} {ex.Message}");
                    }

                    //using var ping = new Ping();
                    //if (ping.Send(host, timeout).Status != IPStatus.Success)
                    //{
                    //    @out?.WriteLine($"Error: unknown host {host}");
                    //    add = false;
                    //}
                }

                if (add) { ret.Add((host, port)); }
            }

            return ret.ToArray();
        }
    }
}