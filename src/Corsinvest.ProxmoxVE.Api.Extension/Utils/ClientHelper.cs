/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using System.Net.Sockets;
using Corsinvest.ProxmoxVE.Api.Shared;
using Microsoft.Extensions.Logging;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils
{
    /// <summary>
    /// Client helper with improved error handling and async operations
    /// </summary>
    public static class ClientHelper
    {
        private const int DEFAULT_PORT = 8006;
        private const int DEFAULT_TIMEOUT = 4000;

        /// <summary>
        /// Represents a host and port combination
        /// </summary>
        public class HostEndpoint(string host, int port)
        {
            /// <summary>
            /// Host
            /// </summary>
            public string Host { get; } = host;

            /// <summary>
            /// Port
            /// </summary>
            public int Port { get; } = port;
        }


        /// <summary>
        /// Get Client and try login with improved error handling (simplified version without factory)
        /// </summary>
        /// <param name="hostsAndPortHA">Comma-separated list of hosts (e.g., "10.1.1.90:8006,10.1.1.91:8006")</param>
        /// <param name="username">Username for authentication</param>
        /// <param name="password">Password for authentication</param>
        /// <param name="apiToken">API token for authentication (alternative to username/password)</param>
        /// <param name="validateCertificate">Whether to validate SSL certificates</param>
        /// <param name="loggerFactory">Logger factory for logging</param>
        /// <param name="timeout">Connection timeout in milliseconds</param>
        /// <param name="httpClient">Optional HTTP client</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Authenticated PveClient</returns>
        /// <exception cref="PveException">Thrown when no hosts are reachable or authentication fails</exception>
        public static async Task<PveClient> GetClientAndTryLoginAsync(string hostsAndPortHA,
                                                                      string username = null,
                                                                      string password = null,
                                                                      string apiToken = null,
                                                                      bool validateCertificate = true,
                                                                      ILoggerFactory loggerFactory = null,
                                                                      int timeout = DEFAULT_TIMEOUT,
                                                                      HttpClient httpClient = null,
                                                                      CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(hostsAndPortHA)) { throw new ArgumentException("Hosts and ports cannot be null or empty", nameof(hostsAndPortHA)); }

            var (client, endpoint) = await GetClientFromHAAsync(hostsAndPortHA, timeout, httpClient, cancellationToken);

            if (client == null) { throw new PveException("No reachable hosts found in the provided list"); }

            client.ValidateCertificate = validateCertificate;
            client.LoggerFactory = loggerFactory;

            if (!await AuthenticateAsync(client, username, password, apiToken))
            {
                var errorMessage = client.LastResult?.ReasonPhrase ?? "Authentication failed";
                throw new PveException($"Authentication failed for host {endpoint}: {errorMessage}");
            }

            loggerFactory?.CreateLogger<PveClient>()?.LogDebug("Successfully connected to Proxmox VE at {0}", endpoint);
            return client;
        }

        /// <summary>
        /// Get Client and try login with factory for advanced scenarios
        /// </summary>
        /// <typeparam name="T">Type derived from PveClient</typeparam>
        /// <param name="hostsAndPortHA">Comma-separated list of hosts</param>
        /// <param name="clientFactory">Factory function to create client instance</param>
        /// <param name="username">Username for authentication</param>
        /// <param name="password">Password for authentication</param>
        /// <param name="apiToken">API token for authentication</param>
        /// <param name="validateCertificate">Whether to validate SSL certificates</param>
        /// <param name="loggerFactory">Logger factory for logging</param>
        /// <param name="timeout">Connection timeout in milliseconds</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Authenticated client of type T</returns>
        public static async Task<T> GetClientAndTryLoginAsync<T>(string hostsAndPortHA,
                                                                 Func<string, int, T> clientFactory,
                                                                 string username = null,
                                                                 string password = null,
                                                                 string apiToken = null,
                                                                 bool validateCertificate = true,
                                                                 ILoggerFactory loggerFactory = null,
                                                                 int timeout = DEFAULT_TIMEOUT,
                                                                 CancellationToken cancellationToken = default(CancellationToken))
            where T : PveClient
        {
            if (string.IsNullOrWhiteSpace(hostsAndPortHA)) { throw new ArgumentException("Hosts and ports cannot be null or empty", nameof(hostsAndPortHA)); }

            var (client, endpoint) = await GetClientFromHAAsync(hostsAndPortHA, clientFactory, timeout, cancellationToken);

            if (client == null) { throw new PveException("No reachable hosts found in the provided list"); }

            client.ValidateCertificate = validateCertificate;
            client.LoggerFactory = loggerFactory;

            if (!await AuthenticateAsync(client, username, password, apiToken))
            {
                var errorMessage = client.LastResult?.ReasonPhrase ?? "Authentication failed";
                throw new PveException($"Authentication failed for host {endpoint}: {errorMessage}");
            }

            loggerFactory?.CreateLogger<T>()?.LogDebug("Successfully connected to Proxmox VE at {0}", endpoint);
            return client;
        }

        /// <summary>
        /// Get client from HA list with async connectivity check (generic version)
        /// </summary>
        /// <typeparam name="T">Type derived from PveClient</typeparam>
        /// <param name="hostsAndPortHA">Comma-separated list of hosts</param>
        /// <param name="clientFactory">Factory function to create client instance</param>
        /// <param name="timeout">Connection timeout in milliseconds</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Named tuple of client and endpoint, or (null, null) if no hosts are reachable</returns>
        public static async Task<(T client, HostEndpoint endpoint)> GetClientFromHAAsync<T>(string hostsAndPortHA,
                                                                                            Func<string, int, T> clientFactory,
                                                                                            int timeout = DEFAULT_TIMEOUT,
                                                                                            CancellationToken cancellationToken = default(CancellationToken))
            where T : PveClient
        {
            var endpoints = ParseHostEndpoints(hostsAndPortHA);
            var reachableEndpoint = await FindFirstReachableHostAsync(endpoints, timeout, cancellationToken);

            return reachableEndpoint != null
                ? (clientFactory(reachableEndpoint.Host, reachableEndpoint.Port), reachableEndpoint)
                : (null, null);
        }

        /// <summary>
        /// Get client from HA list with async connectivity check (non-generic overload for backward compatibility)
        /// </summary>
        /// <param name="hostsAndPortHA">Comma-separated list of hosts</param>
        /// <param name="timeout">Connection timeout in milliseconds</param>
        /// <param name="httpClient">Optional HTTP client</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Named tuple of client and endpoint, or (null, null) if no hosts are reachable</returns>
        public static async Task<(PveClient client, HostEndpoint endpoint)> GetClientFromHAAsync(string hostsAndPortHA,
                                                                                                 int timeout = DEFAULT_TIMEOUT,
                                                                                                 HttpClient httpClient = null,
                                                                                                 CancellationToken cancellationToken = default(CancellationToken))
            => await GetClientFromHAAsync(hostsAndPortHA,
                                          (host, port) => new PveClient(host, port, httpClient),
                                          timeout,
                                          cancellationToken);

        /// <summary>
        /// Parse host endpoints from string
        /// </summary>
        /// <param name="hostsAndPorts">Comma-separated host:port pairs</param>
        /// <returns>List of parsed endpoints</returns>
        public static List<HostEndpoint> ParseHostEndpoints(string hostsAndPorts)
        {
            if (string.IsNullOrWhiteSpace(hostsAndPorts)) { return []; }

            var endpoints = new List<HostEndpoint>();

            foreach (var hostAndPort in hostsAndPorts.Split(','))
            {
                var trimmed = hostAndPort.Trim();
                if (string.IsNullOrEmpty(trimmed)) { continue; }

                var parts = trimmed.Split(':');
                var host = parts[0].Trim();

                if (string.IsNullOrEmpty(host)) { continue; }

                var port = DEFAULT_PORT;
                if (parts.Length > 1 && int.TryParse(parts[1].Trim(), out int parsedPort))
                {
                    port = parsedPort;
                }

                endpoints.Add(new HostEndpoint(host, port));
            }

            return endpoints;
        }

        /// <summary>
        /// Find first reachable host from the list
        /// </summary>
        /// <param name="endpoints">List of endpoints to test</param>
        /// <param name="timeout">Connection timeout in milliseconds</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>First reachable endpoint or null</returns>
        public static async Task<HostEndpoint> FindFirstReachableHostAsync(IEnumerable<HostEndpoint> endpoints,
                                                                           int timeout = DEFAULT_TIMEOUT,
                                                                           CancellationToken cancellationToken = default(CancellationToken))
        {
            var errors = new List<string>();

            foreach (var endpoint in endpoints)
            {
                try
                {
                    if (await IsHostReachableAsync(endpoint, timeout, cancellationToken)) { return endpoint; }
                    errors.Add($"Host {endpoint} is not reachable");
                }
                catch (Exception ex)
                {
                    errors.Add($"Error testing host {endpoint}: {ex.Message}");
                }
            }

            if (errors.Any())
            {
                throw new PveException($"No reachable hosts found. Errors: {string.Join("; ", errors)}");
            }

            return null;
        }

        /// <summary>
        /// Test if a host is reachable
        /// </summary>
        /// <param name="endpoint">Endpoint to test</param>
        /// <param name="timeout">Connection timeout in milliseconds</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if host is reachable</returns>
        public static async Task<bool> IsHostReachableAsync(HostEndpoint endpoint,
                                                            int timeout = DEFAULT_TIMEOUT,
                                                            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                using var tcpClient = new TcpClient();
#if NET7_0_OR_GREATER
                // .NET 7+ supports CancellationToken in ConnectAsync
                using (var timeoutCts = new CancellationTokenSource(timeout))
                using (var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token))
                {
                    await tcpClient.ConnectAsync(endpoint.Host, endpoint.Port).ConfigureAwait(false);
                    return tcpClient.Connected;
                }
#else
                // .NET Standard 2.0 and older versions don't support CancellationToken in ConnectAsync
                var connectTask = tcpClient.ConnectAsync(endpoint.Host, endpoint.Port);
                var completedTask = await Task.WhenAny(connectTask, Task.Delay(timeout, cancellationToken));

                if (completedTask == connectTask)
                {
                    await connectTask; // Await to get any exceptions
                    return tcpClient.Connected;
                }
                else
                {
                    return false; // Timeout
                }
#endif
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw; // Rethrow if it's the user's cancellation token
            }
            catch
            {
                return false; // Timeout or connection refused
            }
        }

        /// <summary>
        /// Authenticate with the Proxmox VE API
        /// </summary>
        /// <param name="client">PVE client</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="apiToken">API token</param>
        /// <returns>True if authentication successful</returns>
        private static async Task<bool> AuthenticateAsync(PveClient client, string username, string password, string apiToken)
        {
            if (!string.IsNullOrEmpty(apiToken))
            {
                client.ApiToken = apiToken;
                var versionResult = await client.Version.Version();
                return versionResult.IsSuccessStatusCode;
            }

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password)) { return await client.LoginAsync(username, password); }

            throw new ArgumentException("Either API token or username/password must be provided");
        }
    }
}