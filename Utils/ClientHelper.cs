using System;
using System.Net.Http;
using System.Text;

namespace EnterpriseVE.ProxmoxVE.Api.Utils
{
#pragma warning disable 1591
    public class ClientHelper
    {
        public const int DEFAULT_PORT = 8006;

        public static string CreateBaseUrl(string hostName, int port = DEFAULT_PORT) { return $"https://{hostName}:{port}"; }

        /// <summary>
        /// Return schema JSON forma from ApiDocs
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        public static string GetJsonSchemaFromApiDoc(string hostName, int port = ClientHelper.DEFAULT_PORT)
        {
            var url = $"{ClientHelper.CreateBaseUrl(hostName, port)}/pve-docs/api-viewer/apidoc.js";
            var json = new StringBuilder();

            using (var httpClientHandler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            })
            using (var client = new HttpClient(httpClientHandler))
            using (var response = client.GetAsync(url).Result)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                data = data.Substring(data.IndexOf("["));

                foreach (var line in data.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                {
                    json.Append(line);
                    if (line.Substring(0, 1) == "]") { break; }
                }
            }

            return json.ToString();
        }
    }
}