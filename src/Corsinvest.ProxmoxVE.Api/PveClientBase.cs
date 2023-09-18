/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Corsinvest.ProxmoxVE.Api
{
    /// <summary>
    /// Proxmox VE Client Base
    /// </summary>
    public class PveClientBase
    {
        private ILogger<PveClientBase> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public PveClientBase(string host, int port = 8006)
        {
            Host = host;
            Port = port;
            _logger = NullLoggerFactory.Instance.CreateLogger<PveClientBase>();
        }

        private ILoggerFactory _loggerFactory;
        /// <summary>
        /// Logger Factory
        /// </summary>
        public ILoggerFactory LoggerFactory
        {
            get => _loggerFactory;
            set
            {
                _loggerFactory = value;
                _logger = _loggerFactory.CreateLogger<PveClientBase>();
            }
        }

        /// <summary>
        /// Host
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// Port
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Get/Set the response type that is going to be returned when doing requests (json, png).
        /// </summary>
        public ResponseType ResponseType { get; set; } = ResponseType.Json;

        /// <summary>
        /// Returns the base URL used to interact with the Proxmox VE API.
        /// </summary>
        public string GetApiUrl() => $"https://{Host}:{Port}/api2/{Enum.GetName(typeof(ResponseType), ResponseType).ToLower()}";

        /// <summary>
        /// Api Token format USER@REALM!TOKENID=UUID
        /// </summary>
        public string ApiToken { get; set; }

        /// <summary>
        /// Ticket CSRFPreventionToken
        /// </summary>
        public string CSRFPreventionToken { get; private set; }

        /// <summary>
        /// Ticket PVEAuthCookie
        /// </summary>
        public string PVEAuthCookie { get; private set; }

        /// <summary>
        /// Creation ticket from login.
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="password">The secret password. This can also be a valid ticket.</param>
        /// <param name="realm">You can optionally pass the realm using this parameter.
        /// Normally the realm is simply added to the username &lt;username&gt;@&lt;relam&gt;.</param>
        /// <param name="otp">One-time password for Two-factor authentication.</param>
        public async Task<bool> Login(string userName, string password, string realm, string otp = null)
        {
            var result = await Create("/access/ticket",
                                      new Dictionary<string, object>
                                      {
                                          {"password", password},
                                          {"username", userName},
                                          {"realm", realm},
                                          {"otp", otp},
                                      });

            if (result.IsSuccessStatusCode)
            {
                if (((IDictionary<string, object>)result.Response.data).ContainsKey("NeedTFA"))
                {
                    throw new PveExceptionAuthentication(result, "Couldn't authenticate user: missing Two Factor Authentication (TFA)");
                }

                CSRFPreventionToken = result.Response.data.CSRFPreventionToken;
                PVEAuthCookie = result.Response.data.ticket;
            }
            return result.IsSuccessStatusCode;
        }

        /// <summary>
        /// Creation ticket from login username &lt;username&gt;@&lt;realm&gt;.
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="password">The secret password. This can also be a valid ticket.</param>
        /// <param name="opt">One-time password for Two-factor authentication.</param>
        public async Task<bool> Login(string userName, string password, string opt = null)
        {
            _logger.LogDebug($"Login: {userName}");

            var realm = "pam";

            //check username
            var data = userName.Split('@');
            if (data.Length > 1)
            {
                userName = data[0];
                realm = data[1];
            }
            return await Login(userName, password, realm, opt);
        }

        /// <summary>
        /// Execute Execute method GET
        /// </summary>
        /// <param name="resource">Url request</param>
        /// <param name="parameters">Additional parameters</param>
        /// <returns>Result</returns>
        public async Task<Result> Get(string resource, IDictionary<string, object> parameters = null)
            => await ExecuteAction(resource, MethodType.Get, parameters);

        /// <summary>
        /// Execute Execute method POST
        /// </summary>
        /// <param name="resource">Url request</param>
        /// <param name="parameters">Additional parameters</param>
        /// <param name="content"></param>
        /// <returns>Result</returns>
        public async Task<Result> Create(string resource, IDictionary<string, object> parameters = null)
            => await ExecuteAction(resource, MethodType.Create, parameters);

        /// <summary>
        /// Execute Execute method PUT
        /// </summary>
        /// <param name="resource">Url request</param>
        /// <param name="parameters">Additional parameters</param>
        /// <returns>Result</returns>
        public async Task<Result> Set(string resource, IDictionary<string, object> parameters = null)
            => await ExecuteAction(resource, MethodType.Set, parameters);

        /// <summary>
        /// Het http client
        /// </summary>
        /// <returns></returns>
        public HttpClient GetHttpClient()
        {
#pragma warning disable S4830 // Server certificates should be verified during SSL/TLS connections
            var handler = new HttpClientHandler()
            {
                CookieContainer = new CookieContainer(),
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
#pragma warning restore S4830 // Server certificates should be verified during SSL/TLS connections

            var client = new HttpClient(handler);

            //ticket login
            if (CSRFPreventionToken != null)
            {
                handler.CookieContainer.Add(new Cookie("PVEAuthCookie", PVEAuthCookie, "/", Host));
                client.DefaultRequestHeaders.Add("CSRFPreventionToken", CSRFPreventionToken);
            }

            if (!string.IsNullOrWhiteSpace(ApiToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("PVEAPIToken", ApiToken);
            }

            return client;
        }

        /// <summary>
        /// Execute Execute method DELETE
        /// </summary>
        /// <param name="resource">Url request</param>
        /// <param name="parameters">Additional parameters</param>
        /// <returns>Result</returns>
        public async Task<Result> Delete(string resource, IDictionary<string, object> parameters = null)
            => await ExecuteAction(resource, MethodType.Delete, parameters);

        private async Task<Result> ExecuteAction(string resource,
                                                 MethodType methodType,
                                                 IDictionary<string, object> parameters = null)
        {
            using var client = GetHttpClient();

            var httpMethod = methodType switch
            {
                MethodType.Get => HttpMethod.Get,
                MethodType.Set => HttpMethod.Put,
                MethodType.Create => HttpMethod.Post,
                MethodType.Delete => HttpMethod.Delete,
                _ => throw new ArgumentOutOfRangeException(nameof(methodType)),
            };

            //load parameters
            var @params = new Dictionary<string, string>();
            if (parameters != null)
            {
                foreach (var parameter in parameters.Where(a => a.Value != null))
                {
                    var value = parameter.Value;
                    if (value is bool valueBool) { value = valueBool ? 1 : 0; }
                    @params.Add(parameter.Key, value.ToString());
                }
            }

            var uriString = GetApiUrl() + resource;
            if ((httpMethod == HttpMethod.Get || httpMethod == HttpMethod.Delete) && @params.Count > 0)
            {
                uriString += "?" + string.Join("&", @params.Select(a => $"{a.Key}={HttpUtility.UrlEncode(a.Value)}"));
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"Method: {httpMethod}, Url: {uriString}");
                if (httpMethod != HttpMethod.Get)
                {
                    _logger.LogDebug("Parameters:" +
                                     Environment.NewLine +
                                     string.Join(Environment.NewLine, @params.Select(a => $"{a.Key} : {a.Value}")));
                }
            }

            var request = new HttpRequestMessage(httpMethod, new Uri(uriString));
            if (httpMethod != HttpMethod.Get && httpMethod != HttpMethod.Delete)
            {
                request.Content = new FormUrlEncodedContent(@params);
            }

            var response = await client.SendAsync(request);
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"StatusCode:          {response.StatusCode}" +
                                 Environment.NewLine +
                                 $"ReasonPhrase:        {response.ReasonPhrase}" +
                                 Environment.NewLine +
                                 $"IsSuccessStatusCode: {response.IsSuccessStatusCode}");
            }

            dynamic result = null;
            switch (ResponseType)
            {
                case ResponseType.Json:
                    result = JsonConvert.DeserializeObject<ExpandoObject>(await response.Content.ReadAsStringAsync());
                    if (_logger.IsEnabled(LogLevel.Trace))
                    {
                        _logger.LogTrace(JsonConvert.SerializeObject(result, Formatting.Indented) as string);
                    }
                    break;

                case ResponseType.Png:
                    result = "data:image/png;base64," + Convert.ToBase64String(await response.Content.ReadAsByteArrayAsync());
                    if (_logger.IsEnabled(LogLevel.Trace)) { _logger.LogTrace(result as string); }
                    break;

                case ResponseType.Response: result = response; break;

                default: throw new InvalidEnumArgumentException();
            }

            result ??= new ExpandoObject();

            LastResult = new Result(result,
                                    response.StatusCode,
                                    response.ReasonPhrase,
                                    response.IsSuccessStatusCode,
                                    resource,
                                    parameters,
                                    methodType,
                                    ResponseType);

            return LastResult;
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
        public static void AddIndexedParameter(Dictionary<string, object> parameters,
                                               string name,
                                               IDictionary<int, string> value)
        {
            if (value == null) { return; }
            foreach (var item in value) { parameters.Add(name + item.Key, item.Value); }
        }

        /// <summary>
        /// Get node from task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static string GetNodeFromTask(string task) => task.Split(':')[1];

        /// <summary>
        /// Wait for task to finish
        /// </summary>
        /// <param name="result"></param>
        /// <param name="wait"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<bool> WaitForTaskToFinish(Result result, int wait = 500, long timeout = 10000)
            => !(result != null && !result.ResponseInError && timeout > 0) ||
                    await WaitForTaskToFinish(result.ToData(), wait, timeout);

        /// <summary>
        /// Wait for task to finish
        /// </summary>
        /// <param name="task">Task identifier</param>
        /// <param name="wait">Millisecond wait next check</param>
        /// <param name="timeout">Millisecond timeout</param>
        /// <return></return>
        public async Task<bool> WaitForTaskToFinish(string task, int wait = 500, long timeout = 10000)
        {
            var isRunning = true;
            if (wait <= 0) { wait = 500; }
            if (timeout < wait) { timeout = wait + 5000; }
            var timeStart = DateTime.Now;

            while (isRunning && (DateTime.Now - timeStart).Milliseconds < timeout)
            {
                Thread.Sleep(wait);
                isRunning = await TaskIsRunning(task);
            }

            //check timeout
            return (DateTime.Now - timeStart).Milliseconds < timeout;
        }

        /// <summary>
        /// Get exists status task.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task<bool> TaskIsRunning(string task)
            => (await ReadTaskStatus(task)).Response.data.status == "running";

        /// <summary>
        /// Get exists status task.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task<string> GetExitStatusTask(string task)
            => (await ReadTaskStatus(task)).Response.data.exitstatus;

        /// <summary>
        /// Read task status.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private async Task<Result> ReadTaskStatus(string task)
            => await Get($"/nodes/{GetNodeFromTask(task)}/tasks/{task}/status");
    }
}