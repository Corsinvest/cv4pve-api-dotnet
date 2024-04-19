/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils;

/// <summary>
/// Client helper
/// </summary>
public static class ClientHelper
{
    /// <summary>
    /// Get Client and try login
    /// </summary>
    /// <param name="hostsAndPortHA"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="apiToken"></param>
    /// <param name="validateCertificate"></param>
    /// <param name="loggerFactory"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    /// <exception cref="PveException"></exception>
    public static async Task<PveClient> GetClientAndTryLoginAsync(string hostsAndPortHA,
                                                                  string username,
                                                                  string password,
                                                                  string apiToken,
                                                                  bool validateCertificate,
                                                                  ILoggerFactory loggerFactory,
                                                                  int timeout = 4000)
    {
        var client = GetClientFromHA(hostsAndPortHA, timeout);
        if (client != null)
        {
            client.ValidateCertificate = validateCertificate;
            client.LoggerFactory = loggerFactory;

            bool login;
            if (!string.IsNullOrEmpty(apiToken))
            {
                client.ApiToken = apiToken;
                login = (await client.Version.Version()).IsSuccessStatusCode;
            }
            else
            {
                login = await client.LoginAsync(username, password);
            }

            return login
                    ? client
                    : throw new PveException(client.LastResult.ReasonPhrase);
        }

        throw new PveException("ClientHelper.GetClient error!");
    }

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
        var errors = new List<string>();

        var found = true;
        string hostTest = "";
        int portTest = defaultPort;
        foreach (var hostAndPort in hostsAndPorts.Split(','))
        {
            var data = hostAndPort.Split(':');
            hostTest = data[0];
            portTest = defaultPort;
            if (data.Length == 2) { int.TryParse(data[1], out portTest); }

            if (!checkPort) { break; }

            found = false;
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
                    errors.Add($"Problem connection host {hostTest} with port {portTest}");
                }
            }
            else
            {
                // timed out
                errors.Add($"Timeout connection host {hostTest} with port {portTest}");
            }
        }

        if (!found && errors.Count != 0) { throw new PveException(string.Join(Environment.NewLine, errors)); }

        host = hostTest;
        port = portTest;

        return found;
    }
}