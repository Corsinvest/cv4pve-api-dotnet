/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Net;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Api;

public class LoginTests
{
    private const string Ticket = """{"data":{"ticket":"PVE:root@pam:TICKET","CSRFPreventionToken":"CSRF","username":"root@pam"}}""";

    private static bool IsLogin(HttpRequestMessage request) => request.RequestUri!.AbsolutePath.EndsWith("/access/ticket");

    private static async Task<JObject> BodyAsync(HttpRequestMessage request)
        => JObject.Parse(await request.Content!.ReadAsStringAsync());

    private static FakeHandler AnswerTicket()
        => new(request => IsLogin(request)
                            ? FakeHandler.Json(HttpStatusCode.OK, Ticket)
                            : FakeHandler.Json(HttpStatusCode.OK, """{"data":{}}"""));

    [Fact]
    public async Task User_without_realm_logs_in_pam()
    {
        var handler = AnswerTicket();

        Assert.True(await FakeHandler.Client(handler).LoginAsync("root", "secret"));

        var request = handler.Requests.Single();
        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.Equal("https://pve01:8006/api2/json/access/ticket", request.RequestUri!.AbsoluteUri);
        var body = await BodyAsync(request);
        Assert.Equal("root", (string)body["username"]!);
        Assert.Equal("pam", (string)body["realm"]!);
        Assert.Equal("secret", (string)body["password"]!);
    }

    [Theory]
    [InlineData("automation@pve", "automation", "pve")]
    [InlineData("root@pam", "root", "pam")]
    [InlineData("john@example.com@ldap", "john@example.com", "ldap")]
    public async Task Realm_is_read_after_the_last_at(string user, string expectedUser, string expectedRealm)
    {
        var handler = AnswerTicket();

        await FakeHandler.Client(handler).LoginAsync(user, "secret");

        var body = await BodyAsync(handler.Requests.Single());
        Assert.Equal(expectedUser, (string)body["username"]!);
        Assert.Equal(expectedRealm, (string)body["realm"]!);
    }

    [Fact]
    public async Task Realm_can_be_given()
    {
        var handler = AnswerTicket();

        await FakeHandler.Client(handler).LoginAsync("automation", "secret", "pve", null);

        var body = await BodyAsync(handler.Requests.Single());
        Assert.Equal("automation", (string)body["username"]!);
        Assert.Equal("pve", (string)body["realm"]!);
    }

    [Fact]
    public async Task Ticket_is_sent_with_the_next_requests()
    {
        var handler = AnswerTicket();
        var client = FakeHandler.Client(handler);

        Assert.True(await client.LoginAsync("root@pam", "secret"));
        await client.GetAsync("/version");

        Assert.Equal("PVE:root@pam:TICKET", client.PVEAuthCookie);
        Assert.Equal("CSRF", client.CSRFPreventionToken);
        var request = handler.Requests.Last();
        Assert.Equal("PVEAuthCookie=PVE:root@pam:TICKET", request.Headers.GetValues("Cookie").Single());
        Assert.Equal("CSRF", request.Headers.GetValues("CSRFPreventionToken").Single());
    }

    [Fact]
    public async Task Wrong_password_returns_false_with_the_reason_in_the_last_result()
    {
        var handler = new FakeHandler(_ =>
        {
            var response = FakeHandler.Json(HttpStatusCode.Unauthorized, """{"data":null}""");
            response.ReasonPhrase = "authentication failure";
            return response;
        });
        var client = FakeHandler.Client(handler);

        Assert.False(await client.LoginAsync("root@pam", "wrong"));

        Assert.Equal(HttpStatusCode.Unauthorized, client.LastResult.StatusCode);
        Assert.Equal("authentication failure", client.LastResult.ReasonPhrase);
        Assert.True(string.IsNullOrEmpty(client.PVEAuthCookie));
    }

    [Theory]
    [InlineData("""{"data":null}""")]
    [InlineData("""{"data":{}}""")]
    [InlineData("""{"data":{"username":"root@pam"}}""")]
    [InlineData("")]
    [InlineData("<html>proxy login</html>")]
    public async Task Login_answered_without_a_ticket_is_not_a_login(string body)
    {
        var client = FakeHandler.Client(new FakeHandler(_ => FakeHandler.Text(HttpStatusCode.OK, body)));

        Assert.False(await client.LoginAsync("root@pam", "secret"));
        Assert.True(string.IsNullOrEmpty(client.PVEAuthCookie));
    }

    [Fact]
    public async Task Failed_login_forgets_the_previous_ticket()
    {
        var ok = true;
        var handler = new FakeHandler(request => !IsLogin(request)
            ? FakeHandler.Json(HttpStatusCode.OK, """{"data":{}}""")
            : ok
                ? FakeHandler.Json(HttpStatusCode.OK, Ticket)
                : FakeHandler.Json(HttpStatusCode.Unauthorized, """{"data":null}"""));
        var client = FakeHandler.Client(handler);

        Assert.True(await client.LoginAsync("root@pam", "secret"));
        ok = false;
        Assert.False(await client.LoginAsync("root@pam", "wrong"));
        Assert.False(handler.Requests.Last().Headers.Contains("Cookie"), "the old ticket is not sent with a new login");

        await client.GetAsync("/version");
        Assert.False(handler.Requests.Last().Headers.Contains("Cookie"));
        Assert.False(handler.Requests.Last().Headers.Contains("CSRFPreventionToken"));
    }

    [Theory]
    [InlineData("123456", "totp:123456")]
    [InlineData("recovery:abcd-1234", "recovery:abcd-1234")]
    public async Task Second_factor_is_sent_in_a_second_call_with_the_challenge(string otp, string expectedPassword)
    {
        var handler = new FakeHandler(request =>
        {
            var body = JObject.Parse(request.Content!.ReadAsStringAsync().Result);
            return body.ContainsKey("tfa-challenge")
                ? FakeHandler.Json(HttpStatusCode.OK, Ticket)
                : FakeHandler.Json(HttpStatusCode.OK, """{"data":{"ticket":"PVE:!tfa!CHALLENGE","NeedTFA":1,"username":"root@pam"}}""");
        });
        var client = FakeHandler.Client(handler);

        Assert.True(await client.LoginAsync("root@pam", "secret", otp));

        Assert.Equal(2, handler.Requests.Count);
        var second = await BodyAsync(handler.Requests[1]);
        Assert.Equal(expectedPassword, (string)second["password"]!);
        Assert.Equal("PVE:!tfa!CHALLENGE", (string)second["tfa-challenge"]!);
        Assert.Equal("root", (string)second["username"]!);
        Assert.Equal("PVE:root@pam:TICKET", client.PVEAuthCookie);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Second_factor_requested_without_a_code_throws(string? otp)
    {
        var client = FakeHandler.Client(new FakeHandler(_ =>
            FakeHandler.Json(HttpStatusCode.OK, """{"data":{"ticket":"PVE:!tfa!CHALLENGE","NeedTFA":1}}""")));

        var ex = await Assert.ThrowsAsync<PveAuthenticationException>(() => client.LoginAsync("root@pam", "secret", otp));

        Assert.Contains("Two Factor", ex.Message);
        Assert.NotNull(ex.Result);
    }

    [Fact]
    public async Task Second_factor_refused_returns_false()
    {
        var handler = new FakeHandler(request =>
            JObject.Parse(request.Content!.ReadAsStringAsync().Result).ContainsKey("tfa-challenge")
                ? FakeHandler.Json(HttpStatusCode.Unauthorized, """{"data":null}""")
                : FakeHandler.Json(HttpStatusCode.OK, """{"data":{"ticket":"PVE:!tfa!CHALLENGE","NeedTFA":1}}"""));
        var client = FakeHandler.Client(handler);

        Assert.False(await client.LoginAsync("root@pam", "secret", "000000"));
        Assert.True(string.IsNullOrEmpty(client.PVEAuthCookie));
    }

    [Fact]
    public async Task Api_token_is_sent_in_the_authorization_header_without_a_login()
    {
        var handler = AnswerTicket();
        var client = FakeHandler.Client(handler);
        client.ApiToken = "automation@pve!app=aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee";

        await client.GetAsync("/version");

        var request = handler.Requests.Single();
        Assert.Equal("PVEAPIToken", request.Headers.Authorization!.Scheme);
        Assert.Equal("automation@pve!app=aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee", request.Headers.Authorization.Parameter);
        Assert.False(request.Headers.Contains("Cookie"));
    }
}
