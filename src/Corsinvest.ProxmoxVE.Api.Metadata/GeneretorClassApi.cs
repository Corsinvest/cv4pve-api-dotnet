/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json.Linq;
using System.Text;

namespace Corsinvest.ProxmoxVE.Api.Metadata;

/// <summary>
/// Generator class Api
/// </summary>
public static class GeneratorClassApi
{
    /// <summary>
    /// Generate class Api
    /// </summary>
    /// <param name="host"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    public static async Task<ClassApi> GenerateAsync(string host = "pve.proxmox.com", int port = 443)
    {
        var classApi = new ClassApi();
        foreach (var token in JArray.Parse(await GetJsonSchemaFromApiDocAsync(host, port))) { _ = new ClassApi(token, classApi); }
        return classApi;
    }

    /// <summary>
    /// Fetches the JSON schema from the Proxmox VE API documentation.
    /// </summary>
    /// <param name="host">The Proxmox VE host address.</param>
    /// <param name="port">The port number.</param>
    /// <returns>The JSON schema string extracted from the API documentation.</returns>
    public static async Task<string> GetJsonSchemaFromApiDocAsync(string host, int port)
    {
        var url = $"https://{host}:{port}/pve-docs/api-viewer/apidoc.js";
        var json = new StringBuilder();

#pragma warning disable S4830 // Server certificates should be verified during SSL/TLS connections
        using (var httpClientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
#pragma warning restore S4830 // Server certificates should be verified during SSL/TLS connections
        })
        using (var client = new HttpClient(httpClientHandler))
        using (var response = await client.GetAsync(url))
        {
            var data = await response.Content.ReadAsStringAsync();
            //start Json API
            data = data[data.IndexOf('[')..];

            foreach (var line in data.Split('\n'))
            {
                json.Append(line);
                //end Json API
                if (line[..1] == "]") { break; }
            }
        }

        return json.ToString();
    }
}