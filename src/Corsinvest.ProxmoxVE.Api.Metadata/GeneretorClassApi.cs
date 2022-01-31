/*
 * This file is part of the cv4pve-api-dotnet https://github.com/Corsinvest/cv4pve-api-dotnet,
 *
 * This source file is available under two different licenses:
 * - GNU General Public License version 3 (GPLv3)
 * - Corsinvest Enterprise License (CEL)
 * Full copyright and license information is available in
 * LICENSE.md which is distributed with this source code.
 *
 * Copyright (C) 2016 Corsinvest Srl	GPLv3 and CEL
 */

using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Corsinvest.ProxmoxVE.Api.Metadata
{
    /// <summary>
    /// Generator class Api
    /// </summary>
    public class GeneretorClassApi
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
            foreach (var token in JArray.Parse(await GetJsonSchemaFromApiDoc(host, port))) { new ClassApi(token, classApi); }
            return classApi;
        }

        private static async Task<string> GetJsonSchemaFromApiDoc(string host, int port)
        {
            var url = $"https://{host}:{port}/pve-docs/api-viewer/apidoc.js";
            var json = new StringBuilder();

            using (var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
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