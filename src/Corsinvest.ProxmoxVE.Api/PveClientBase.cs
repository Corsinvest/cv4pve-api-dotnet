/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Dynamic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Web;

namespace Corsinvest.ProxmoxVE.Api;

/// <summary>
/// Proxmox VE Client Base
/// </summary>
public class PveClientBase(string host, int port = 8006, HttpClient? httpClient = null)
{
    private ILogger<PveClientBase> _logger = NullLoggerFactory.Instance.CreateLogger<PveClientBase>();
    private ILoggerFactory _loggerFactory;

    private HttpClient _internalHttpClient;
    private HttpClientHandler _internalHttpClientHandler;

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
            {
                throw new PveAuthenticationException(result, "Missing Two Factor Authentication (TFA)");
            }

            CSRFPreventionToken = result.Response.data.CSRFPreventionToken;
            PVEAuthCookie = result.Response.data.ticket;

            // Add cookie to CookieContainer for proper authentication in subsequent requests
            if (_internalHttpClientHandler?.CookieContainer != null)
            {
                _internalHttpClientHandler.CookieContainer.Add(new Uri(BaseAddress), new Cookie("PVEAuthCookie", PVEAuthCookie));
            }
        }

        return result.IsSuccessStatusCode;
    }

    /// <summary>
    /// Verify OpenID authorization code and create a ticket.
    /// </summary>
    /// <param name="code">OpenID authorization code received from the callback</param>
    /// <param name="state">OpenID state received from the callback</param>
    /// <param name="redirectUrl">Same redirect URL used in GetOpenIdAuthUrlAsync</param>
    /// <returns>True if login succeeded</returns>
    public async Task<bool> LoginOpenIdAsync(string code, string state, string redirectUrl)
    {
        var result = await CreateAsync("/access/openid/login", new Dictionary<string, object>
        {
            { "code", code },
            { "state", state },
            { "redirect-url", redirectUrl }
        });

        if (result.IsSuccessStatusCode)
        {
            CSRFPreventionToken = result.Response.data.CSRFPreventionToken;
            PVEAuthCookie = result.Response.data.ticket;

            if (_internalHttpClientHandler?.CookieContainer != null)
            {
                _internalHttpClientHandler.CookieContainer.Add(new Uri(BaseAddress), new Cookie("PVEAuthCookie", PVEAuthCookie));
            }
        }

        return result.IsSuccessStatusCode;
    }


    /// <summary>
    /// Full OpenID login flow: starts a local HTTP listener, opens the browser,
    /// waits for the authorization callback, and exchanges the code for a ticket.
    /// </summary>
    /// <param name="realm">Authentication domain ID (OIDC realm configured in PVE)</param>
    /// <param name="openBrowser">Action to open the browser with the given URL</param>
    /// <param name="timeoutSeconds">Seconds to wait for the user to complete login</param>
    /// <returns>True if login succeeded</returns>
    public async Task<bool> LoginOpenIdAsync(string realm, Action<string> openBrowser, int timeoutSeconds = 60)
    {
#if NETSTANDARD2_0
        var tcpListenerFreePort = new TcpListener(IPAddress.Loopback, 0);
#else
        using var tcpListenerFreePort = new TcpListener(IPAddress.Loopback, 0);
#endif
        tcpListenerFreePort.Start();
        var port = ((IPEndPoint)tcpListenerFreePort.LocalEndpoint).Port;
        tcpListenerFreePort.Stop();

        var redirectUrl = $"http://localhost:{port}/callback";

        var result = await CreateAsync("/access/openid/auth-url", new Dictionary<string, object>
        {
            { "realm", realm },
            { "redirect-url", redirectUrl }
        });

        var authUrl = result.IsSuccessStatusCode ? (string)result.Response.data : null;
        if (string.IsNullOrEmpty(authUrl)) { return false; }

        var listener = new HttpListener();
        listener.Prefixes.Add($"http://localhost:{port}/");
        listener.Start();

        try
        {
            openBrowser(authUrl);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            var contextTask = listener.GetContextAsync();
            var completedTask = await Task.WhenAny(contextTask, Task.Delay(System.Threading.Timeout.Infinite, cts.Token));
            if (completedTask != contextTask) { return false; }

            var context = await contextTask;
            var query = context.Request.QueryString;
            var code = query["code"];
            var state = query["state"];

            var responseHtml = "<html><body><h2>Authentication complete. You can close this window.</h2></body></html>"u8.ToArray();
            context.Response.ContentType = "text/html";
            context.Response.ContentLength64 = responseHtml.Length;
#if NETSTANDARD2_0
            await context.Response.OutputStream.WriteAsync(responseHtml, 0, responseHtml.Length);
#else
            await context.Response.OutputStream.WriteAsync(responseHtml);
#endif
            context.Response.Close();

            return !string.IsNullOrEmpty(code)
                    && !string.IsNullOrEmpty(state)
                    && await LoginOpenIdAsync(code, state, redirectUrl);
        }
        finally
        {
            listener.Stop();
        }
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
    public async Task<Result> GetAsync(string resource, IDictionary<string, object> parameters = null)
    => await ExecuteRequestAsync(resource, MethodType.Get, parameters);

    /// <summary>
    /// Execute method POST
    /// </summary>
    /// <param name="resource">Url request</param>
    /// <param name="parameters">Additional parameters</param>
    public async Task<Result> CreateAsync(string resource, IDictionary<string, object> parameters = null)
    => await ExecuteRequestAsync(resource, MethodType.Create, parameters);

    /// <summary>
    /// Execute method PUT
    /// </summary>
    /// <param name="resource">Url request</param>
    /// <param name="parameters">Additional parameters</param>
    public async Task<Result> SetAsync(string resource, IDictionary<string, object> parameters = null)
    => await ExecuteRequestAsync(resource, MethodType.Set, parameters);

    /// <summary>
    /// Execute method DELETE
    /// </summary>
    /// <param name="resource">Url request</param>
    /// <param name="parameters">Additional parameters</param>
    public async Task<Result> DeleteAsync(string resource, IDictionary<string, object> parameters = null)
    => await ExecuteRequestAsync(resource, MethodType.Delete, parameters);

    /// <summary>
    /// Get http client
    /// </summary>
    public virtual HttpClient GetHttpClient()
    {
        if (httpClient != null) { return httpClient; }
        if (_internalHttpClient == null)
        {
            _internalHttpClientHandler = new HttpClientHandler
            {
                CookieContainer = new CookieContainer(),
                ServerCertificateCustomValidationCallback = !ValidateCertificate
                                                                ? (message, cert, chain, errors) => true
                                                                : null
            };
            _internalHttpClient = new HttpClient(_internalHttpClientHandler);
        }

        return _internalHttpClient;
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
                var sensitiveParams = new[] { "password", "token", "ticket", "otp", "apitoken" };
                _logger.LogDebug("Parameters: {parameters}", string.Join(Environment.NewLine, @params.Select(a =>
                {
                    var paramName = a.Key.ToLower();
                    return sensitiveParams.Any(p => paramName.Contains(p))
                        ? $"{a.Key} : ****"
                        : $"{a.Key} : {a.Value}";
                })));
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

            response = new(HttpStatusCode.RequestTimeout)
            {
                ReasonPhrase = ex.Message,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            response = new(HttpStatusCode.InternalServerError)
            {
                ReasonPhrase = ex.Message,
            };
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
    public Result LastResult { get; private set; }

    /// <summary>
    /// Adds indexed parameters to a dictionary.
    /// </summary>
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
    public static string GetNodeFromTask(string task) => task.Split(':')[1];

    /// <summary>
    /// Waits for a background task to finish.
    /// </summary>
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

        while (isRunning && (DateTime.Now - timeStart).TotalMilliseconds < timeout)
        {
            await Task.Delay(wait);
            isRunning = await TaskIsRunningAsync(task);
        }

        //check timeout
        return (DateTime.Now - timeStart).TotalMilliseconds < timeout;
    }

    /// <summary>
    /// Checks whether a task is still running.
    /// </summary>
    public async Task<bool> TaskIsRunningAsync(string task)
        => (await ReadTaskStatusAsync(task)).Response.data.status == "running";

    /// <summary>
    /// Gets the exit status of a task.
    /// </summary>
    public async Task<string> GetExitStatusTaskAsync(string task)
        => (await ReadTaskStatusAsync(task)).Response.data.exitstatus;

    /// <summary>
    /// Reads the current status of a task.
    /// </summary>
    private async Task<Result> ReadTaskStatusAsync(string task)
        => await GetAsync($"/nodes/{GetNodeFromTask(task)}/tasks/{task}/status");
}