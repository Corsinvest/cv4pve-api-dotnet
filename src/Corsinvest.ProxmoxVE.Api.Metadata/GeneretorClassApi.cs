/*
 * This file is part of the cv4pve-api-dotnet https://github.com/Corsinvest/cv4pve-api-dotnet,
 * Copyright (C) 2016 Corsinvest Srl
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
 
using System.Net.Http;
using System.Text;
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
        public static ClassApi Generate(string host = "pve.proxmox.com", int port = 443)
        {
            var classApi = new ClassApi();
            foreach (var token in JArray.Parse(GetJsonSchemaFromApiDoc(host, port))) { new ClassApi(token, classApi); }
            return classApi;
        }

        private static string GetJsonSchemaFromApiDoc(string host, int port)
        {
            var url = $"https://{host}:{port}/pve-docs/api-viewer/apidoc.js";
            var json = new StringBuilder();

            using (var httpClientHandler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            })
            using (var client = new HttpClient(httpClientHandler))
            using (var response = client.GetAsync(url).Result)
            {
                var data = response.Content.ReadAsStringAsync().Result;
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