/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using System.ComponentModel;
using System.Dynamic;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api;

/// <summary>
/// Proxmox VE Client Base
/// </summary>
/// <param name="host"></param>
/// <param name="port"></param>
/// <param name="httpClient"></param>
public class PveClientBase(string host, int port = 8006, HttpClient? httpClient = null)
{
    private ILogger<PveClientBase> _logger = NullLoggerFactory.Instance.CreateLogger<PveClientBase>();
    private ILoggerFactory _loggerFactory;

    private HttpClient _httpClient;
    private readonly HttpClient? _externalHttpClient = httpClient;

    /// <summary>
    /// Logger Factory
    /// </summary>
    public ILoggerFactory LoggerFactory
    {
        get => _loggerFactory;
        set
        {
            _loggerFactory = value ?? NullLoggerFactory.Instance;
            _logger = _loggerFactory.CreateLogger<PveClientBase>();
        }
    }

    /// <summary>
    /// Host address of the Proxmox server.
    /// </summary>
    public string Host { get; } = host;

    /// <summary>
    /// Port number of the Proxmox API.
    /// </summary>
    public int Port { get; } = port;

    /// <summary>
    /// Optional timeout for HTTP requests.
    /// </summary>
    public TimeSpan? Timeout { get; set; }

    /// <summary>
    /// If true, validates the certificate of the Proxmox API server.
    /// </summary>
    public bool ValidateCertificate { get; set; } = false;

    /// <summary>
    /// Response type (Json, Png or Raw).
    /// </summary>
    public ResponseType ResponseType { get; set; } = ResponseType.Json;

    /// <summary>
    /// Gets the base URL for the Proxmox API.
    /// </summary>
    public string GetApiUrl() => $"{BaseAddress}/api2/{Enum.GetName(typeof(ResponseType), ResponseType)?.ToLower()}";

    /// <summary>
    /// BaseAddress
    /// </summary>
    public string BaseAddress => $"https://{Host}:{Port}";

    /// <summary>
    /// API Token format: USER@REALM!TOKENID=UUID
    /// </summary>
    public string ApiToken { get; set; }

    /// <summary>
    /// CSRF prevention token received after login.
    /// </summary>
    public string CSRFPreventionToken { get; private set; }

    /// <summary>
    /// Authentication cookie received after login.
    /// </summary>
    public string PVEAuthCookie { get; private set; }

    /// <summary>
    /// Logs in to the Proxmox API using username and password.
    /// </summary>
    public async Task<bool> LoginAsync(string userName, string password, string realm, string otp = null)
    {
        var result = await CreateAsync("/access/ticket", new Dictionary<string, object>
        {
            {"password", password},
            {"username", userName},
            {"realm", realm},
            {"otp", otp},
        });

        if (result.IsSuccessStatusCode)
        {
            var data = (IDictionary<string, object>)result.Response.data;
            if (data.ContainsKey("NeedTFA"))
                throw new PveAuthenticationException(result, "Missing Two Factor Authentication (TFA)");

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
    public async Task<bool> LoginAsync(string userName, string password, string opt = null)
    {
        _logger.LogDebug("Login: {userName}", userName);

        var realm = "pam";

        //check username
        var data = userName.Split('@');
        if (data.Length > 1)
        {
            userName = data[0];
            realm = data[1];
        }
        return await LoginAsync(userName, password, realm, opt);
    }

    /// <summary>
    /// Execute method GET
    /// </summary>
    /// <param name="resource">Url request</param>
    /// <param name="parameters">Additional parameters</param>
    /// <returns>Result</returns>
    public async Task<Result> GetAsync(string resource, IDictionary<string, object> parameters = null)
        => await ExecuteRequestAsync(resource, MethodType.Get, parameters);

    /// <summary>
    /// Execute method POST
    /// </summary>
    /// <param name="resource">Url request</param>
    /// <param name="parameters">Additional parameters</param>
    /// <returns>Result</returns>
    public async Task<Result> CreateAsync(string resource, IDictionary<string, object> parameters = null)
        => await ExecuteRequestAsync(resource, MethodType.Create, parameters);

    /// <summary>
    /// Execute method PUT
    /// </summary>
    /// <param name="resource">Url request</param>
    /// <param name="parameters">Additional parameters</param>
    /// <returns>Result</returns>
    public async Task<Result> SetAsync(string resource, IDictionary<string, object> parameters = null)
        => await ExecuteRequestAsync(resource, MethodType.Set, parameters);

    /// <summary>
    /// Execute method DELETE
    /// </summary>
    /// <param name="resource">Url request</param>
    /// <param name="parameters">Additional parameters</param>
    /// <returns>Result</returns>
    public async Task<Result> DeleteAsync(string resource, IDictionary<string, object> parameters = null)
        => await ExecuteRequestAsync(resource, MethodType.Delete, parameters);

    /// <summary>
    /// Get http client
    /// </summary>
    /// <returns></returns>
    public virtual HttpClient GetHttpClient()
    {
        if (_externalHttpClient != null) return _externalHttpClient;

        _httpClient ??= new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = !ValidateCertificate
                                                            ? (message, cert, chain, errors) => true
                                                            : null
        });

        return _httpClient;
    }

    /// <summary>
    /// Creates an HttpRequestMessage with appropriate headers.
    /// </summary>
    public HttpRequestMessage CreateHttpRequestMessage(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);
        if (!string.IsNullOrWhiteSpace(ApiToken)) { request.Headers.Authorization = new AuthenticationHeaderValue("PVEAPIToken", ApiToken); }
        if (!string.IsNullOrWhiteSpace(CSRFPreventionToken)) { request.Headers.Add("CSRFPreventionToken", CSRFPreventionToken); }
        if (!string.IsNullOrWhiteSpace(PVEAuthCookie)) { request.Headers.Add("Cookie", $"PVEAuthCookie={PVEAuthCookie}"); }
        return request;
    }

    /// <summary>
    /// Execute Request.
    /// </summary>
    /// <param name="resource"></param>
    /// <param name="methodType"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    protected virtual async Task<Result> ExecuteRequestAsync(string resource,
                                                             MethodType methodType,
                                                             IDictionary<string, object> parameters = null)
    {
        var httpMethod = methodType switch
        {
            MethodType.Get => HttpMethod.Get,
            MethodType.Set => HttpMethod.Put,
            MethodType.Create => HttpMethod.Post,
            MethodType.Delete => HttpMethod.Delete,
            _ => throw new ArgumentOutOfRangeException(nameof(methodType)),
        };

        //load parameters
        var @params = new Dictionary<string, object>();
        if (parameters != null)
        {
            foreach (var parameter in parameters.Where(a => a.Value != null))
            {
                var value = parameter.Value;
                if (value is bool valueBool) { value = valueBool ? 1 : 0; }
                @params.Add(parameter.Key, value);
            }
        }

        var uriString = GetApiUrl() + resource;
        if ((httpMethod == HttpMethod.Get || httpMethod == HttpMethod.Delete) && @params.Count > 0)
        {
            uriString += "?" + string.Join("&", @params.Select(a => $"{a.Key}={HttpUtility.UrlEncode(a.Value.ToString())}"));
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Method: {httpMethod}, Url: {uriString}", httpMethod, uriString);
            if (httpMethod != HttpMethod.Get)
            {
                _logger.LogDebug("Parameters: {parameters}", string.Join(Environment.NewLine, @params.Select(a => $"{a.Key} : {a.Value}")));
            }
        }

        var request = CreateHttpRequestMessage(httpMethod, uriString);

        if (httpMethod != HttpMethod.Get && httpMethod != HttpMethod.Delete)
        {
            request.Content = new StringContent(JsonConvert.SerializeObject(@params), Encoding.UTF8, "application/json");
        }

        using var cts = Timeout.HasValue
                ? new CancellationTokenSource(Timeout.Value)
                : new CancellationTokenSource();

        HttpResponseMessage response = null!;
        dynamic result = null;

        try
        {
            response = await GetHttpClient().SendAsync(request, cts.Token);

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
        }
        catch (TaskCanceledException ex) when (!cts.Token.IsCancellationRequested)
        {
            _logger.LogError(ex, ex.Message);
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("StatusCode: {StatusCode} ReasonPhrase: {ReasonPhrase} IsSuccessStatusCode: {IsSuccessStatusCode}",
                             response.StatusCode,
                             response.ReasonPhrase,
                             response.IsSuccessStatusCode);
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
    /// Adds indexed parameters to a dictionary.
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
    /// Extracts the node name from a task identifier.
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public static string GetNodeFromTask(string task) => task.Split(':')[1];

    /// <summary>
    /// Waits for a background task to finish.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="wait"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public async Task<bool> WaitForTaskToFinishAsync(Result result, int wait = 500, long timeout = 10000)
        => !(result != null && !result.ResponseInError && timeout > 0) ||
                await WaitForTaskToFinishAsync(result.ToData(), wait, timeout);

    /// <summary>
    /// Waits for a background task to finish by its ID.
    /// </summary>
    /// <param name="task">Task identifier</param>
    /// <param name="wait">Millisecond wait next check</param>
    /// <param name="timeout">Millisecond timeout</param>
    /// <return></return>
    public async Task<bool> WaitForTaskToFinishAsync(string task, int wait = 500, long timeout = 10000)
    {
        var isRunning = true;
        if (wait <= 0) { wait = 500; }
        if (timeout < wait) { timeout = wait + 5000; }
        var timeStart = DateTime.Now;

        while (isRunning && (DateTime.Now - timeStart).Milliseconds < timeout)
        {
            Thread.Sleep(wait);
            isRunning = await TaskIsRunningAsync(task);
        }

        //check timeout
        return (DateTime.Now - timeStart).Milliseconds < timeout;
    }

    /// <summary>
    /// Checks whether a task is still running.
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public async Task<bool> TaskIsRunningAsync(string task)
        => (await ReadTaskStatusAsync(task)).Response.data.status == "running";

    /// <summary>
    /// Gets the exit status of a task.
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public async Task<string> GetExitStatusTaskAsync(string task)
        => (await ReadTaskStatusAsync(task)).Response.data.exitstatus;

    /// <summary>
    /// Reads the current status of a task.
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    private async Task<Result> ReadTaskStatusAsync(string task)
        => await GetAsync($"/nodes/{GetNodeFromTask(task)}/tasks/{task}/status");
}