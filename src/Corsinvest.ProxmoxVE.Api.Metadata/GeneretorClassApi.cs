/*
 * SPDX-FileCopyrightText: 2019 Daniele Corsini <daniele.corsini@corsinvest.it>
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Corsinvest.ProxmoxVE.Api.Metadata
{
    /// <summary>
    /// Generator class Api
    /// </summary>
    public static class GeneretorClassApi
    {
        /// <summary>
        /// Generate class Api
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static async Task<ClassApi> Generate(string host = "pve.proxmox.com", int port = 443)
        {
            var classApi = new ClassApi();
            foreach (var token in JArray.Parse(await GetJsonSchemaFromApiDoc(host, port))) { _ = new ClassApi(token, classApi); }
            return classApi;
        }

        private static async Task<string> GetJsonSchemaFromApiDoc(string host, int port)
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
                data = data.Substring(data.IndexOf("["));

                foreach (var line in data.Split('\n'))
                {
                    json.Append(line);
                    //end Json API
                    if (line.Substring(0, 1) == "]") { break; }
                }
            }

            return json.ToString();
        }
    }
}