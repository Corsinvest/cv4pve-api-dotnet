/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Net;
using Corsinvest.ProxmoxVE.Api.Extension.Utils;
using Corsinvest.ProxmoxVE.Api.Shared;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Api;

/// <summary>
/// Login with OpenID, and what the client does when it opens the connection by itself.
/// </summary>
public class OpenIdAndConnectionTests
{
    private const string Ticket = """{"data":{"ticket":"PVE:user@oidc:TICKET","CSRFPreventionToken":"CSRF","username":"user@oidc"}}""";

    private static JObject Body(HttpRequestMessage request) => JObject.Parse(request.Content!.ReadAsStringAsync().Result);

    // a node with an OpenID realm: it gives the address of the provider, then a ticket for the code
    private static FakeHandler OpenIdHandler(string? authUrl = "https://idp.example.com/authorize?client_id=pve", bool loginOk = true)
        => new(request => request.RequestUri!.AbsolutePath switch
        {
            "/api2/json/access/openid/auth-url" => authUrl == null
                ? FakeHandler.Json(HttpStatusCode.InternalServerError, """{"data":null}""")
                : FakeHandler.Json(HttpStatusCode.OK, new JObject { ["data"] = authUrl }.ToString()),
            "/api2/json/access/openid/login" => loginOk
                ? FakeHandler.Json(HttpStatusCode.OK, Ticket)
                : FakeHandler.Json(HttpStatusCode.Unauthorized, """{"data":null}"""),
            _ => FakeHandler.Json(HttpStatusCode.OK, """{"data":null}"""),
        });

    [Fact]
    public async Task OpenId_code_is_exchanged_for_a_ticket()
    {
        var handler = OpenIdHandler();
        var client = FakeHandler.Client(handler);

        Assert.True(await client.LoginOpenIdAsync("CODE", "STATE", "http://localhost:1234/callback"));

        Assert.Equal("PVE:user@oidc:TICKET", client.PVEAuthCookie);
        Assert.Equal("CSRF", client.CSRFPreventionToken);
        var body = Body(handler.Requests.Single());
        Assert.Equal("CODE", (string?)body["code"]);
        Assert.Equal("STATE", (string?)body["state"]);
        Assert.Equal("http://localhost:1234/callback", (string?)body["redirect-url"]);
    }

    [Fact]
    public async Task OpenId_code_that_is_refused_leaves_the_client_without_a_ticket()
    {
        var client = FakeHandler.Client(OpenIdHandler(loginOk: false));

        Assert.False(await client.LoginOpenIdAsync("CODE", "STATE", "http://localhost:1234/callback"));
        Assert.Null(client.PVEAuthCookie);
    }

    // the browser of the test: it goes straight back to the local address the client listens on
    private static (Action<string> Open, Func<Task<string>> Page) Browser(FakeHandler handler, string query, List<string> opened)
    {
        Task<string> page = Task.FromResult(string.Empty);
        return (url =>
                {
                    opened.Add(url);
                    var redirectUrl = (string)Body(handler.Requests.Single(a => a.RequestUri!.AbsolutePath.EndsWith("/auth-url")))["redirect-url"]!;
                    page = Task.Run(async () =>
                    {
                        using var http = new HttpClient();
                        return await http.GetStringAsync(redirectUrl + query);
                    });
                },
                () => page);
    }

    [Fact]
    public async Task OpenId_login_opens_the_browser_and_waits_for_the_code()
    {
        var handler = OpenIdHandler();
        var client = FakeHandler.Client(handler);
        var opened = new List<string>();
        var (open, page) = Browser(handler, "?code=CODE%2F1&state=STATE", opened);

        Assert.True(await client.LoginOpenIdAsync("oidc", open, 30));

        Assert.Equal(["https://idp.example.com/authorize?client_id=pve"], opened);
        Assert.Contains("Authentication complete", await page());
        Assert.Equal("PVE:user@oidc:TICKET", client.PVEAuthCookie);

        var authUrl = Body(handler.Requests[0]);
        Assert.Equal("oidc", (string?)authUrl["realm"]);
        Assert.Matches(@"^http://localhost:\d+/callback$", (string?)authUrl["redirect-url"]);
        var login = Body(handler.Requests[1]);
        Assert.Equal("CODE/1", (string?)login["code"]);
        Assert.Equal("STATE", (string?)login["state"]);
        Assert.Equal((string?)authUrl["redirect-url"], (string?)login["redirect-url"]);
    }

    [Theory]
    [InlineData("?state=STATE")]
    [InlineData("?code=CODE")]
    [InlineData("?error=access_denied")]
    public async Task OpenId_answer_without_code_or_state_is_not_a_login(string query)
    {
        var handler = OpenIdHandler();
        var client = FakeHandler.Client(handler);
        var (open, page) = Browser(handler, query, []);

        Assert.False(await client.LoginOpenIdAsync("oidc", open, 30));

        Assert.Contains("Authentication complete", await page());
        Assert.DoesNotContain(handler.Requests, a => a.RequestUri!.AbsolutePath.EndsWith("/openid/login"));
        Assert.Null(client.PVEAuthCookie);
    }

    [Fact]
    public async Task OpenId_login_refused_by_the_node_is_not_a_login()
    {
        var handler = OpenIdHandler(loginOk: false);
        var (open, _) = Browser(handler, "?code=CODE&state=STATE", []);

        Assert.False(await FakeHandler.Client(handler).LoginOpenIdAsync("oidc", open, 30));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task OpenId_realm_without_an_address_does_not_open_the_browser(string? authUrl)
    {
        var opened = new List<string>();

        Assert.False(await FakeHandler.Client(OpenIdHandler(authUrl)).LoginOpenIdAsync("oidc", opened.Add, 30));
        Assert.Empty(opened);
    }

    [Fact]
    public async Task OpenId_login_that_nobody_completes_ends_at_the_timeout()
    {
        var opened = new List<string>();

        Assert.False(await FakeHandler.Client(OpenIdHandler()).LoginOpenIdAsync("oidc", opened.Add, 1));
        Assert.Single(opened);
    }

    [Fact]
    public void Client_without_a_logger_factory_has_one_that_logs_nothing()
    {
        var client = new PveClient("pve01");

        Assert.Same(NullLoggerFactory.Instance, client.LoggerFactory);
        client.LoggerFactory = null!;
        Assert.Same(NullLoggerFactory.Instance, client.LoggerFactory);
    }

    [Fact]
    public async Task Cookie_of_the_session_is_sent_until_a_login_fails()
    {
        var loginOk = true;
        await using var server = new LocalPveServer();
        server.Rest = request => !request.Path.EndsWith("/access/ticket")
                                    ? (200, "OK", """{"data":{"version":"8.4.1"}}""")
                                    : loginOk
                                        ? (200, "OK", """{"data":{"ticket":"PVE:root@pam:TICKET","CSRFPreventionToken":"CSRF"}}""")
                                        : (401, "authentication failure", """{"data":null}""");
        var client = server.Client();

        Assert.True(await client.LoginAsync("root@pam", "secret"));
        Assert.True((await client.Version.Version()).IsSuccessStatusCode);
        Assert.Contains("PVEAuthCookie=PVE:root@pam:TICKET", server.Requests.Last().Headers["Cookie"]);

        loginOk = false;
        Assert.False(await client.LoginAsync("root@pam", "wrong"));
        Assert.Equal("authentication failure", client.LastResult.ReasonPhrase);
        await client.Version.Version();
        Assert.False(server.Requests.Last().Headers.ContainsKey("Cookie"));
    }

    private sealed class ToolClient(string host, int port) : PveClient(host, port)
    {
        public string Tool => "cv4pve-tool";
    }

    [Fact]
    public async Task Client_of_a_tool_is_created_by_its_factory_and_logged_in()
    {
        await using var server = new LocalPveServer { Rest = _ => (200, "OK", """{"data":{"version":"8.4.1"}}""") };
        var created = new List<(string Host, int Port)>();

        var client = await ClientHelper.GetClientAndTryLoginAsync($"127.0.0.1:{server.Port}",
                                                                  (host, port) =>
                                                                  {
                                                                      created.Add((host, port));
                                                                      return new ToolClient(host, port);
                                                                  },
                                                                  apiToken: "automation@pve!app=secret",
                                                                  validateCertificate: false,
                                                                  loggerFactory: NullLoggerFactory.Instance,
                                                                  timeout: 2000);

        Assert.Equal("cv4pve-tool", client.Tool);
        Assert.Equal([("127.0.0.1", server.Port)], created);
        Assert.Equal("automation@pve!app=secret", client.ApiToken);
        Assert.False(client.ValidateCertificate);
        Assert.Equal("/api2/json/version", server.Requests.Last().Path);
    }

    [Fact]
    public async Task Client_of_a_tool_with_refused_credentials_or_no_host_is_an_error()
    {
        await using var server = new LocalPveServer { Rest = _ => (401, "invalid token", """{"data":null}""") };
        static ToolClient Factory(string host, int port) => new(host, port);

        var ex = await Assert.ThrowsAsync<PveException>(
            () => ClientHelper.GetClientAndTryLoginAsync($"127.0.0.1:{server.Port}", Factory, apiToken: "automation@pve!app=wrong", validateCertificate: false, timeout: 2000));
        Assert.Equal($"Authentication failed for host 127.0.0.1:{server.Port}: invalid token", ex.Message);

        await Assert.ThrowsAsync<ArgumentException>(() => ClientHelper.GetClientAndTryLoginAsync(" ", Factory, apiToken: "automation@pve!app=secret"));

        using var closed = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 0);
        closed.Start();
        var port = ((IPEndPoint)closed.LocalEndpoint).Port;
        closed.Stop();
        ex = await Assert.ThrowsAsync<PveException>(
            () => ClientHelper.GetClientAndTryLoginAsync($"127.0.0.1:{port}", Factory, apiToken: "automation@pve!app=secret", timeout: 2000));
        Assert.Contains($"Host 127.0.0.1:{port} is not reachable", ex.Message);
    }
}
