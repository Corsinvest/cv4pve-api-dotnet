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