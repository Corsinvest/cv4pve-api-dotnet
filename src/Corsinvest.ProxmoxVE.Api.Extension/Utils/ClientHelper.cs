/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared;
using System;
using System.Net.Sockets;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils
{
    /// <summary>
    /// Client helper
    /// </summary>
    public static class ClientHelper
    {
        /// <summary>
        /// Host and port for HA
        /// Format 10.1.1.90:8006,10.1.1.91:8006,10.1.1.92:8006
        /// </summary>
        /// <param name="hostsAndPortHA"></param>
        /// <param name="timeout"></param>
        public static PveClient GetClientFromHA(string hostsAndPortHA, int timeout = 4000)
            => TryHostAndPort(hostsAndPortHA, 8006, true, timeout, out var host, out var port)
                    ? new PveClient(host, port)
                    : null;

        /// <summary>
        /// Try host and port. Format 10.1.1.90:8006,10.1.1.91:8006,10.1.1.92:8006
        /// </summary>
        /// <param name="hostsAndPorts"></param>
        /// <param name="defaultPort"></param>
        /// <param name="checkPort"></param>
        /// <param name="timeout"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        /// <exception cref="PveException"></exception>
        public static bool TryHostAndPort(string hostsAndPorts,
                                          int defaultPort,
                                          bool checkPort,
                                          int timeout,
                                          out string host,
                                          out int port)
        {
            var found = true;
            string hostTest = "";
            int portTest = defaultPort;
            foreach (var hostAndPort in hostsAndPorts.Split(','))
            {
                var data = hostAndPort.Split(':');
                hostTest = data[0];
                portTest = defaultPort;
                if (data.Length == 2) { int.TryParse(data[1], out portTest); }

                if (checkPort)
                {
                    found = false;
                    try
                    {
                        //open connection tcp ip to test port exists
                        using var tcpClient = new TcpClient();
                        var task = tcpClient.ConnectAsync(hostTest, portTest);
                        if (task.Wait(timeout))
                        {
                            //if fails within timeout, task.Wait still returns true.
                            if (tcpClient.Connected)
                            {
                                found = true;
                                break;
                            }
                            else
                            {
                                // connection refused probably
                                throw new PveException($"Error: problem connection host {hostTest} with port {portTest}");
                            }
                        }
                        else
                        {
                            // timed out
                            throw new PveException($"Error: timeout problem connection host {hostTest} with port {portTest}");
                        }
                    }
                    catch (Exception ex) { throw new PveException($"Error: host {hostTest}", ex); }
                }
                else
                {
                    break;
                }
            }

            host = hostTest;
            port = portTest;

            return found;
        }
    }
}