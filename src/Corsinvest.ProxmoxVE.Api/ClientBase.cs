using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api
{
    /// <summary>
    /// Proxmox VE Client Base
    /// </summary>
    public class ClientBase
    {
        private string _ticketCSRFPreventionToken;
        private string _ticketPVEAuthCookie;

        /// <summary>
        /// Costructor
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        public ClientBase(string hostname, int port = 8006)
        {
            Hostname = hostname;
            Port = port;
        }

        /// <summary>
        /// Get hostname configured. 
        /// </summary>
        public string Hostname { get; }

        /// <summary>
        /// Get port configured. 
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Get/Set the response type that is going to be returned when doing requests (json, png). 
        /// </summary>
        public ResponseType ResponseType { get; set; } = ResponseType.Json;

        /// <summary>
        /// Get/Set level console output debug. 
        /// 0 - nothing
        /// 1 - Url and method
        /// 2 - Url and method and result
        /// </summary>
        public int DebugLevel { get; set; } = 0;

        /// <summary>
        /// Returns the base URL used to interact with the Proxmox VE API. 
        /// </summary>
        public string GetApiUrl() => $"https://{Hostname}:{Port}/api2/{Enum.GetName(typeof(ResponseType), ResponseType).ToLower()}";

        /// <summary>
        /// Convert object to JSON.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="formatted"></param>
        public static string ObjectToJson(object obj, bool formatted = true)
            => JsonConvert.SerializeObject(obj, formatted ? Formatting.Indented : Formatting.None);

        /// <summary>
        /// Creation ticket from login.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="realm"></param>
        public bool Login(string userName, string password, string realm = "pam")
        {
            var ticket = Create($"/access/ticket",
                                new Dictionary<string, object>()
                                {
                                    {"password", password},
                                    {"username", userName},
                                    {"realm", realm},
                                });

            if (ticket.IsSuccessStatusCode)
            {
                _ticketCSRFPreventionToken = ticket.Response.data.CSRFPreventionToken;
                _ticketPVEAuthCookie = ticket.Response.data.ticket;
            }
            return ticket.IsSuccessStatusCode;
        }

        /// <summary>
        /// Creation ticket from login split username &lt;username&gt;@&lt;realm&gt;.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public bool Login(string userName, string password)
        {
            var realm = "pam";

            //check username
            var data = userName.Split('@');
            if (data.Length > 1)
            {
                userName = data[0];
                realm = data[1];
            }
            return Login(userName, password, realm);
        }

        /// <summary>
        /// Execute Execute method GET
        /// </summary>
        /// <param name="resource">Url request</param>
        /// <param name="parameters">Additional parameters</param>
        /// <returns>Result</returns>
        public Result Get(string resource, IDictionary<string, object> parameters = null) => ExecuteAction(resource, HttpMethod.Get, parameters);

        /// <summary>
        /// Execute Execute method POST
        /// </summary>
        /// <param name="resource">Url request</param>
        /// <param name="parameters">Additional parameters</param>
        /// <returns>Result</returns>
        public Result Create(string resource, IDictionary<string, object> parameters = null) => ExecuteAction(resource, HttpMethod.Post, parameters);

        /// <summary>
        /// Execute Execute method PUT
        /// </summary>
        /// <param name="resource">Url request</param>
        /// <param name="parameters">Additional parameters</param>
        /// <returns>Result</returns>
        public Result Set(string resource, IDictionary<string, object> parameters = null) => ExecuteAction(resource, HttpMethod.Put, parameters);

        /// <summary>
        /// Execute Execute method DELETE
        /// </summary>
        /// <param name="resource">Url request</param>
        /// <param name="parameters">Additional parameters</param>
        /// <returns>Result</returns>
        public Result Delete(string resource, IDictionary<string, object> parameters = null) => ExecuteAction(resource, HttpMethod.Delete, parameters);

        private Result ExecuteAction(string resource, HttpMethod method, IDictionary<string, object> parameters = null)
        {
            using (var handler = new HttpClientHandler()
            {
                CookieContainer = new CookieContainer(),
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            })
            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri(GetApiUrl());

                //load parameters
                var @params = new Dictionary<string, string>();
                if (parameters != null)
                {
                    foreach (var parameter in parameters.Where(a => a.Value != null))
                    {
                        var value = parameter.Value;
                        if (value is bool) { value = ((bool)value) ? 1 : 0; }
                        @params.Add(parameter.Key, value.ToString());
                    }
                }

                var uriString = GetApiUrl() + resource;
                if (method == HttpMethod.Get && @params.Count > 0)
                {
                    uriString += "?" + string.Join("&", @params.Select(a => $"{a.Key}={HttpUtility.UrlEncode(a.Value)}"));
                }

                if (DebugLevel >= 1)
                {
                    Console.Out.WriteLine($"Method: {method}, Url: {uriString}");
                    if (method != HttpMethod.Get)
                    {
                        Console.Out.WriteLine("Parameters:");
                        Console.Out.WriteLine(string.Join(Environment.NewLine,
                                                          @params.Select(a => $"{a.Key} : {a.Value}")));
                    }
                }

                var request = new HttpRequestMessage(method, new Uri(uriString));
                if (method != HttpMethod.Get) { request.Content = new FormUrlEncodedContent(@params); }

                //tiket login
                if (_ticketCSRFPreventionToken != null)
                {
                    handler.CookieContainer.Add(request.RequestUri, new Cookie("PVEAuthCookie", _ticketPVEAuthCookie));
                    request.Headers.Add("CSRFPreventionToken", _ticketCSRFPreventionToken);
                }

                var response = client.SendAsync(request).Result;
                dynamic result = null;

                if (DebugLevel >= 2)
                {
                    Console.Out.WriteLine($"StatusCode:          {response.StatusCode}");
                    Console.Out.WriteLine($"ReasonPhrase:        {response.ReasonPhrase}");
                    Console.Out.WriteLine($"IsSuccessStatusCode: {response.IsSuccessStatusCode}");
                }

                switch (ResponseType)
                {
                    case ResponseType.Json:
                        var stringContent = response.Content.ReadAsStringAsync().Result;
                        result = JsonConvert.DeserializeObject<ExpandoObject>(stringContent);
                        if (DebugLevel >= 2) { Console.Out.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented)); }
                        break;

                    case ResponseType.Png:
                        result = "data:image/png;base64," +
                                 Convert.ToBase64String(response.Content.ReadAsByteArrayAsync().Result);

                        if (DebugLevel >= 2) { Console.Out.WriteLine(result); }
                        break;

                    default: break;
                }

                if (result == null) { result = new ExpandoObject(); }

                if (DebugLevel > 0) { Console.Out.WriteLine("============================="); }

                LastResult = new Result(result,
                                        response.StatusCode,
                                        response.ReasonPhrase,
                                        response.IsSuccessStatusCode);

                return LastResult;
            }
        }

        /// <summary>
        /// Last result action
        /// </summary>
        /// <value></value>
        public Result LastResult { get; private set; }

        /// <summary>
        /// Add indexed parameter to parameters.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void AddIndexedParameter(Dictionary<string, object> parameters, string name, IDictionary<int, string> value)
        {
            if (value == null) { return; }
            foreach (var item in value) { parameters.Add(name + item.Key, item.Value); }
        }

        /// <summary>
        /// Wait for task to finish
        /// </summary>
        /// <param name="node">Node identifier</param>
        /// <param name="task">Task identifier</param>
        /// <param name="wait">Millisecond wait next check</param>
        /// <param name="timeOut">Millisecond timeout</param>
        /// <return>O Success</return>
        public int WaitForTaskToFinish(string node, string task, long wait = 500, long timeOut = 10000)
        {
            var isRunning = true;
            if (wait <= 0) { wait = 500; }
            if (timeOut < wait) { timeOut = wait + 5000; }
            var timeStart = DateTime.Now;
            var waitTime = DateTime.Now;
            while (isRunning && (timeStart - DateTime.Now).Milliseconds < timeOut)
            {
                if ((DateTime.Now - waitTime).TotalMilliseconds >= wait)
                {
                    waitTime = DateTime.Now;
                    isRunning = TaskIsRunning(node, task);
                }
            }

            //check timeout
            return (timeStart - DateTime.Now).Milliseconds < timeOut ? 0 : 1;
        }

        /// <summary>
        /// Get exists status task.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool TaskIsRunning(string node, string task) => ReadTaskStatus(node, task).Response.data.status == "running";

        /// <summary>
        /// Get exists status task.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        public string GetExitStatusTask(string node, string task) => ReadTaskStatus(node, task).Response.data.exitstatus;

        /// <summary>
        /// Read task status.
        /// </summary>
        /// <returns></returns>
        private Result ReadTaskStatus(string node, string task) => Get($"/nodes/{node}/tasks/{task}/status");
    }
}