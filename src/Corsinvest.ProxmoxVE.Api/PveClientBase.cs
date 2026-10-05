/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.ComponentModel;
using System.Dynamic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;

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
    /// Optional timeout for HTTP requests. Without a value the requests of the internal HttpClient stop after 100 seconds.
    /// An HttpClient passed to the constructor keeps also its own timeout.
    /// </summary>
    public TimeSpan? Timeout { get; set; }

    //the timeout of an HttpClient cannot change after its first request: the internal one has none and the time is counted here
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(100);

    /// <summary>
    /// If true, validates the certificate of the Proxmox API server.
    /// </summary>
    public bool ValidateCertificate { get; set; } = false;

    /// <summary>
    /// Response type (Json or Png).
    /// </summary>
    public ResponseType ResponseType { get; set; } = ResponseType.Json;

    /// <summary>
    /// Gets the base URL for the Proxmox API.
    /// </summary>
    public string GetApiUrl() => $"{BaseAddress}/api2/{Enum.GetName<ResponseType>(ResponseType)?.ToLower()}";

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
    /// <param name="userName">User name</param>
    /// <param name="password">The secret password. This can also be a valid ticket.</param>
    /// <param name="realm">Realm</param>
    /// <param name="otp">Second factor of a user with two-factor authentication: a TOTP code (e.g. 123456)
    /// or 'type:value' (e.g. recovery:abcd-1234).</param>
    /// <returns>True when Proxmox VE gave a ticket; when false the reason is in <see cref="LastResult"/>.</returns>
    /// <exception cref="PveAuthenticationException">The user needs a second factor and otp is missing.</exception>
    public async Task<bool> LoginAsync(string userName, string password, string realm, string otp = null)
    {
        //a new login does not send, and on failure does not keep, the ticket of the previous one
        ClearTicket();

        var result = await CreateAsync("/access/ticket", new Dictionary<string, object>
        {
            {"password", password},
            {"username", userName},
            {"realm", realm},
        });

        if (result.IsSuccessStatusCode && GetDataObject(result) is { } data && data.ContainsKey("NeedTFA"))
        {
            if (string.IsNullOrWhiteSpace(otp))
            {
                throw new PveAuthenticationException(result, "Missing Two Factor Authentication (TFA)");
            }

            //second step: the response to the challenge of the first one
            result = await CreateAsync("/access/ticket", new Dictionary<string, object>
            {
                {"password", GetTfaResponse(otp)},
                {"username", userName},
                {"realm", realm},
                {"tfa-challenge", data.TryGetValue("ticket", out var challenge) ? challenge : null},
            });
        }

        return StoreTicket(result);
    }

    /// <summary>
    /// 'data' of an answer when it is an object: null for a failed call, an empty body, a value or a list.
    /// </summary>
    private static IDictionary<string, object> GetDataObject(Result result)
        => result is { ResponseHasData: true } && result.ResponseToDictionary["data"] is IDictionary<string, object> data
            ? data
            : null;

    /// <summary>
    /// Forget the ticket of a previous login.
    /// </summary>
    private void ClearTicket()
    {
        CSRFPreventionToken = null;
        PVEAuthCookie = null;

        if (_internalHttpClientHandler?.CookieContainer != null)
        {
            foreach (Cookie cookie in _internalHttpClientHandler.CookieContainer.GetCookies(new Uri(BaseAddress)))
            {
                cookie.Expired = true;
            }
        }
    }

    /// <summary>
    /// Keep the ticket of a login answer. Logged only with a ticket: a success status alone
    /// (e.g. the page of a proxy, an answer without data) is not a login.
    /// </summary>
    private bool StoreTicket(Result result)
    {
        if (!result.IsSuccessStatusCode
            || GetDataObject(result) is not { } data
            || !data.TryGetValue("ticket", out var ticket) || ticket is not string { Length: > 0 } ticketText
            || !data.TryGetValue("CSRFPreventionToken", out var csrf) || csrf is not string { Length: > 0 } csrfText)
        {
            return false;
        }

        CSRFPreventionToken = csrfText;
        PVEAuthCookie = ticketText;

        // Add cookie to CookieContainer for proper authentication in subsequent requests
        _internalHttpClientHandler?.CookieContainer?.Add(new Uri(BaseAddress), new Cookie("PVEAuthCookie", PVEAuthCookie));

        return true;
    }

    /// <summary>
    /// Second factor as Proxmox VE expects it in the response to a TFA challenge: 'type:value'.
    /// A code without a type is a TOTP code.
    /// </summary>
    internal static string GetTfaResponse(string otp)
        => otp.Contains(':') ? otp : $"totp:{otp}";

    /// <summary>
    /// Verify OpenID authorization code and create a ticket.
    /// </summary>
    /// <param name="code">OpenID authorization code received from the callback</param>
    /// <param name="state">OpenID state received from the callback</param>
    /// <param name="redirectUrl">Same redirect URL used in GetOpenIdAuthUrlAsync</param>
    /// <returns>True if login succeeded</returns>
    public async Task<bool> LoginOpenIdAsync(string code, string state, string redirectUrl)
    {
        ClearTicket();

        var result = await CreateAsync("/access/openid/login", new Dictionary<string, object>
        {
            { "code", code },
            { "state", state },
            { "redirect-url", redirectUrl }
        });

        return StoreTicket(result);
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
        using var tcpListenerFreePort = new TcpListener(IPAddress.Loopback, 0);
        tcpListenerFreePort.Start();
        var port = ((IPEndPoint)tcpListenerFreePort.LocalEndpoint).Port;
        tcpListenerFreePort.Stop();

        var redirectUrl = $"http://localhost:{port}/callback";

        var result = await CreateAsync("/access/openid/auth-url", new Dictionary<string, object>
        {
            { "realm", realm },
            { "redirect-url", redirectUrl }
        });

        var authUrl = result.IsSuccessStatusCode && result.ResponseHasData ? result.ResponseToDictionary["data"] as string : null;
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
            await context.Response.OutputStream.WriteAsync(responseHtml);
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
    /// <returns>True when Proxmox VE gave a ticket; when false the reason is in <see cref="LastResult"/>.</returns>
    /// <exception cref="PveAuthenticationException">The user needs a second factor and opt is missing.</exception>
    public async Task<bool> LoginAsync(string userName, string password, string opt = null)
    {
        _logger.LogDebug("Login: {userName}", userName);

        var realm = "pam";

        //user@realm: the realm is what follows the last @
        var at = userName.LastIndexOf('@');
        if (at > 0)
        {
            realm = userName[(at + 1)..];
            userName = userName[..at];
        }
        return await LoginAsync(userName, password, realm, opt);
    }

    /// <summary>
    /// Execute method GET
    /// </summary>
    /// <param name="resource">Url request</param>
    /// <param name="parameters">Additional parameters</param>
    public Task<Result> GetAsync(string resource, IDictionary<string, object> parameters = null)
        => ExecuteRequestAsync(resource, MethodType.Get, parameters);

    /// <summary>
    /// Execute method POST
    /// </summary>
    /// <param name="resource">Url request</param>
    /// <param name="parameters">Additional parameters</param>
    public Task<Result> CreateAsync(string resource, IDictionary<string, object> parameters = null)
        => ExecuteRequestAsync(resource, MethodType.Create, parameters);

    /// <summary>
    /// Execute method PUT
    /// </summary>
    /// <param name="resource">Url request</param>
    /// <param name="parameters">Additional parameters</param>
    public Task<Result> SetAsync(string resource, IDictionary<string, object> parameters = null)
        => ExecuteRequestAsync(resource, MethodType.Set, parameters);

    /// <summary>
    /// Execute method DELETE
    /// </summary>
    /// <param name="resource">Url request</param>
    /// <param name="parameters">Additional parameters</param>
    public Task<Result> DeleteAsync(string resource, IDictionary<string, object> parameters = null)
        => ExecuteRequestAsync(resource, MethodType.Delete, parameters);

    /// <summary>
    /// Token source that cancels a request after the timeout: the one asked, the one of this client,
    /// or 100 seconds with the internal HttpClient. An HttpClient passed to the constructor keeps also its own timeout.
    /// </summary>
    /// <param name="timeout">Timeout of the request, null for the one of this client</param>
    /// <param name="cancellationToken">Token of the caller</param>
    public CancellationTokenSource CreateTimeoutTokenSource(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout ??= Timeout ?? (httpClient == null ? DefaultTimeout : null);
        if (timeout.HasValue) { cts.CancelAfter(timeout.Value); }
        return cts;
    }

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
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                ServerCertificateCustomValidationCallback = !ValidateCertificate
                                                                ? (_, _, _, _) => true
                                                                : null
            };
            _internalHttpClient = new HttpClient(_internalHttpClientHandler)
            {
                Timeout = System.Threading.Timeout.InfiniteTimeSpan
            };
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

        //the url without the query string is the one written in the log: the query repeats the parameters as they are
        var resourceUrl = GetApiUrl() + resource;
        var uriString = resourceUrl;
        if ((httpMethod == HttpMethod.Get || httpMethod == HttpMethod.Delete) && @params.Count > 0)
        {
            uriString += "?" + string.Join("&", @params.Select(a => $"{a.Key}={HttpUtility.UrlEncode(a.Value.ToString())}"));
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Method: {httpMethod}, Url: {uriString}", httpMethod, resourceUrl);
            if (@params.Count > 0)
            {
                _logger.LogDebug("Parameters: {parameters}", string.Join(Environment.NewLine, @params.Select(a =>
                    IsSensitive(a.Key)
                        ? $"{a.Key} : ****"
                        : $"{a.Key} : {a.Value}")));
            }
        }

        var request = CreateHttpRequestMessage(httpMethod, uriString);

        if (httpMethod != HttpMethod.Get && httpMethod != HttpMethod.Delete)
        {
            request.Content = new StringContent(JsonConvert.SerializeObject(@params), Encoding.UTF8, "application/json");
        }

        using var cts = CreateTimeoutTokenSource(Timeout);

        HttpResponseMessage response = null!;
        dynamic result = null;
        var sw = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            response = await GetHttpClient().SendAsync(request, cts.Token);
            sw.Stop();

            switch (ResponseType)
            {
                case ResponseType.Json:
                    var body = await response.Content.ReadAsStringAsync();
                    try
                    {
                        result = JsonConvert.DeserializeObject<ExpandoObject>(body);
                    }
                    catch (JsonException ex)
                    {
                        // Not an answer of the API (a proxy page, a path mangled by the shell…):
                        // keep the HTTP status and show the start of the body instead of a stack trace.
                        _logger.LogDebug(ex, "{Message}", ex.Message);
                        response = NotJsonResponse(response.StatusCode, body);
                        result = null;
                    }
                    if (_logger.IsEnabled(LogLevel.Trace))
                    {
                        _logger.LogTrace("{Json}", MaskSensitiveResponse((object)result, resource));
                    }
                    break;

                case ResponseType.Png:
                    result = "data:image/png;base64," + Convert.ToBase64String(await response.Content.ReadAsByteArrayAsync());
                    if (_logger.IsEnabled(LogLevel.Trace)) { _logger.LogTrace("{Data}", (string)result); }
                    break;

                default: throw new InvalidEnumArgumentException();
            }
        }
        catch (OperationCanceledException ex)
        {
            //timeout of this client (cts) or of the HttpClient: nothing else cancels the request
            sw.Stop();
            _logger.LogError(ex, "{Message}", ex.Message);

            response = new(HttpStatusCode.RequestTimeout)
            {
                ReasonPhrase = ex.Message,
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "{Message}", ex.Message);

            response = new(HttpStatusCode.InternalServerError)
            {
                ReasonPhrase = ex.Message,
            };
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("StatusCode: {StatusCode} ReasonPhrase: {ReasonPhrase} IsSuccessStatusCode: {IsSuccessStatusCode} Duration: {ElapsedMilliseconds}ms",
                             response.StatusCode,
                             response.ReasonPhrase,
                             response.IsSuccessStatusCode,
                             sw.ElapsedMilliseconds);
        }

        result ??= new ExpandoObject();

        LastResult = new Result(result,
                                response.StatusCode,
                                response.ReasonPhrase,
                                response.IsSuccessStatusCode,
                                resource,
                                parameters,
                                methodType,
                                ResponseType,
                                sw.Elapsed);

        RequestCompleted?.Invoke(this, LastResult);

        return LastResult;
    }

    private static readonly string[] SensitiveNames = ["password", "token", "ticket", "otp", "apitoken", "tfa-challenge"];

    private static bool IsSensitive(string name)
        => SensitiveNames.Any(a => name.Contains(a, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Answer as indented JSON for the log, without the secrets it carries: the ticket and the
    /// CSRF token of a login, the value of a new API token.
    /// </summary>
    internal static string MaskSensitiveResponse(object response, string resource)
    {
        if (response == null) { return "null"; }

        var json = Newtonsoft.Json.Linq.JToken.FromObject(response);
        if (json is Newtonsoft.Json.Linq.JObject root && root["data"] is Newtonsoft.Json.Linq.JObject data)
        {
            foreach (var property in data.Properties())
            {
                if (IsSensitive(property.Name) || (property.Name == "value" && resource.Contains("/token/")))
                {
                    property.Value = "****";
                }
            }
        }

        return json.ToString(Formatting.Indented);
    }

    /// <summary>
    /// Raised after each API request completes. Fires for both successful and failed requests
    /// (including transport-level failures, which are translated into a synthetic <see cref="Result"/>
    /// </summary>
    public event EventHandler<Result> RequestCompleted;

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
    /// Response for an answer that is not JSON: the HTTP status is kept (a success becomes 502, since the
    /// answer cannot be used) and the reason shows the start of the body.
    /// </summary>
    internal static HttpResponseMessage NotJsonResponse(HttpStatusCode statusCode, string body)
    {
        var start = (body.Length > 100 ? body[..100] + "…" : body).ReplaceLineEndings(" ").Trim();
        return new(((int)statusCode) is >= 200 and <= 299 ? HttpStatusCode.BadGateway : statusCode)
        {
            ReasonPhrase = $"The answer is not JSON (HTTP {(int)statusCode}): {start}",
        };
    }

    /// <summary>
    /// Extracts the node name from a task identifier.
    /// </summary>
    public static string GetNodeFromTask(string task) => task.Split(':')[1];

    /// <summary>
    /// Waits for the background task started by a call to finish.
    /// </summary>
    /// <param name="result">Result of the call.</param>
    /// <param name="wait">Millisecond wait next check</param>
    /// <param name="timeout">Millisecond timeout</param>
    /// <returns>True when the task is finished or there is nothing to wait for (the call failed or started no task),
    /// false when the task is still running at the timeout.</returns>
    /// <exception cref="PveResultException">Task status cannot be read.</exception>
    public async Task<bool> WaitForTaskToFinishAsync(Result result, int wait = 500, long timeout = 10000)
        => GetTaskFromResult(result) is not { } task
            || timeout <= 0
            || await WaitForTaskToFinishAsync(task, wait, timeout);

    /// <summary>
    /// Task identifier (UPID) returned by a call: null when the call failed or started no task.
    /// </summary>
    internal static string GetTaskFromResult(Result result)
        => result is { IsSuccessStatusCode: true, ResponseInError: false, ResponseHasData: true }
            && result.ResponseToDictionary["data"] is string task
            && task.StartsWith("UPID:")
                ? task
                : null;

    /// <summary>
    /// Waits for a background task to finish by its ID.
    /// </summary>
    /// <param name="task">Task identifier</param>
    /// <param name="wait">Millisecond wait next check</param>
    /// <param name="timeout">Millisecond timeout</param>
    /// <returns>True when the task is finished, false when it is still running at the timeout.</returns>
    /// <exception cref="PveResultException">Task status cannot be read.</exception>
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

        //finished, also when the last check came after the timeout
        return !isRunning;
    }

    /// <summary>
    /// Checks whether a task is still running.
    /// </summary>
    /// <exception cref="PveResultException">Task status cannot be read.</exception>
    public async Task<bool> TaskIsRunningAsync(string task)
        => EnsureTaskStatus(await ReadTaskStatusAsync(task), task).Response.data.status == "running";

    /// <summary>
    /// Gets the exit status of a task.
    /// </summary>
    /// <returns>Exit status ('OK', 'WARNINGS: n' or the error); null while the task is running.</returns>
    /// <exception cref="PveResultException">Task status cannot be read.</exception>
    public async Task<string> GetExitStatusTaskAsync(string task)
        => EnsureTaskStatus(await ReadTaskStatusAsync(task), task).ResponseToDictionary["data"] is IDictionary<string, object> data
            && data.TryGetValue("exitstatus", out var exitStatus)
                ? exitStatus as string
                : null;

    /// <summary>
    /// Reads the current status of a task.
    /// </summary>
    private Task<Result> ReadTaskStatusAsync(string task)
        => GetAsync($"/nodes/{GetNodeFromTask(task)}/tasks/{task}/status");

    /// <summary>
    /// Validates a task status result before accessing its dynamic members, so that an API
    /// failure is reported with the original HTTP status and Proxmox VE error envelope
    /// instead of a RuntimeBinderException on the missing 'data' member.
    /// </summary>
    private static Result EnsureTaskStatus(Result result, string task)
    {
        if (result == null) { throw new PveResultException(null, $"Read status of task '{task}' returned no result"); }

        if (result.InError() || !result.IsSuccessStatusCode || !result.ResponseHasData)
        {
            var detail = result.InError() ? result.GetError()
                            : !result.IsSuccessStatusCode ? result.ReasonPhrase
                            : "response does not contain 'data'";

            throw new PveResultException(result,
                $"Read status of task '{task}' failed ({(int)result.StatusCode} {result.ReasonPhrase}): {detail}");
        }

        return result;
    }
}
