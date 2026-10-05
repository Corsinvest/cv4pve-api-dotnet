/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Net;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Api;

public class RequestTests
{
    private static FakeHandler Ok(string json = """{"data":{}}""") => new(_ => FakeHandler.Json(HttpStatusCode.OK, json));

    // collects what the client writes to its logger
    private sealed class LogCollector : ILoggerProvider, ILogger
    {
        public List<string> Lines { get; } = [];
        public ILogger CreateLogger(string categoryName) => this;
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => Lines.Add($"{logLevel}: {formatter(state, exception)}");
        public void Dispose() { }
    }

    private static LogCollector LogOf(PveClient client)
    {
        var collector = new LogCollector();
        client.LoggerFactory = LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.Trace).AddProvider(collector));
        return collector;
    }

    [Fact]
    public async Task Get_sends_the_parameters_in_the_query_string_encoded()
    {
        var handler = Ok();

        await FakeHandler.Client(handler).GetAsync("/cluster/resources", new Dictionary<string, object> { ["type"] = "a&b=c #%" });

        var request = handler.Requests.Single();
        Assert.Equal(HttpMethod.Get, request.Method);
        Assert.Null(request.Content);
        Assert.Equal("/api2/json/cluster/resources", request.RequestUri!.AbsolutePath);
        Assert.Equal("a&b=c #%", System.Web.HttpUtility.ParseQueryString(request.RequestUri.Query)["type"]);
    }

    [Fact]
    public async Task Get_without_parameters_has_no_query_string()
    {
        var handler = Ok();

        await FakeHandler.Client(handler).GetAsync("/version");

        Assert.Equal("https://pve01:8006/api2/json/version", handler.Requests.Single().RequestUri!.AbsoluteUri);
    }

    [Fact]
    public async Task Delete_sends_the_parameters_in_the_query_string()
    {
        var handler = Ok();

        await FakeHandler.Client(handler).DeleteAsync("/nodes/pve01/qemu/100", new Dictionary<string, object> { ["purge"] = true });

        var request = handler.Requests.Single();
        Assert.Equal(HttpMethod.Delete, request.Method);
        Assert.Equal("https://pve01:8006/api2/json/nodes/pve01/qemu/100?purge=1", request.RequestUri!.AbsoluteUri);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Create_and_set_send_the_parameters_as_a_json_body(bool create)
    {
        var handler = Ok();
        var client = FakeHandler.Client(handler);
        var parameters = new Dictionary<string, object> { ["name"] = "caffè", ["cores"] = 2, ["onboot"] = true, ["protection"] = false, ["description"] = null! };

        _ = create
            ? await client.CreateAsync("/nodes/pve01/qemu", parameters)
            : await client.SetAsync("/nodes/pve01/qemu/100/config", parameters);

        var request = handler.Requests.Single();
        Assert.Equal(create ? HttpMethod.Post : HttpMethod.Put, request.Method);
        Assert.Equal(string.Empty, request.RequestUri!.Query);
        Assert.Equal("application/json", request.Content!.Headers.ContentType!.MediaType);
        var body = JObject.Parse(await request.Content.ReadAsStringAsync());
        Assert.Equal("caffè", (string)body["name"]!);
        Assert.Equal(2, (int)body["cores"]!);
        Assert.Equal(1, (int)body["onboot"]!);
        Assert.Equal(0, (int)body["protection"]!);
        Assert.False(body.ContainsKey("description"), "a null parameter is not sent");
    }

    [Fact]
    public async Task Indexed_parameters_are_added_with_the_index_as_suffix()
    {
        var parameters = new Dictionary<string, object>();

        PveClientBase.AddIndexedParameter(parameters, "net", new Dictionary<int, string> { [0] = "virtio,bridge=vmbr0", [2] = "virtio,bridge=vmbr1" });
        PveClientBase.AddIndexedParameter(parameters, "scsi", null);

        Assert.Equal(["net0", "net2"], parameters.Keys);
        Assert.Equal("virtio,bridge=vmbr1", parameters["net2"]);
    }

    [Fact]
    public void Api_url_follows_host_port_and_response_type()
    {
        var client = new PveClient("pve01.example.com", 443);
        Assert.Equal("https://pve01.example.com:443", client.BaseAddress);
        Assert.Equal("https://pve01.example.com:443/api2/json", client.GetApiUrl());

        client.ResponseType = ResponseType.Png;
        Assert.Equal("https://pve01.example.com:443/api2/png", client.GetApiUrl());
        Assert.Equal(8006, new PveClient("pve01").Port);
    }

    [Fact]
    public async Task Result_describes_the_request_and_the_answer()
    {
        var client = FakeHandler.Client(Ok("""{"data":{"version":"9.2"}}"""));
        var parameters = new Dictionary<string, object> { ["verbose"] = true };

        var result = await client.GetAsync("/version", parameters);

        Assert.True(result.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("/version", result.RequestResource);
        Assert.Same(parameters, result.RequestParameters);
        Assert.Equal(MethodType.Get, result.MethodType);
        Assert.Equal(ResponseType.Json, result.ResponseType);
        Assert.True(result.Duration >= TimeSpan.Zero);
        Assert.True(result.ResponseHasData);
        Assert.False(result.ResponseInError);
        Assert.Equal("9.2", (string)result.Response.data.version);
        Assert.Same(result, client.LastResult);
    }

    [Fact]
    public async Task Refused_parameters_are_in_the_result_and_no_exception_is_thrown()
    {
        var client = FakeHandler.Client(new FakeHandler(_ =>
        {
            var response = FakeHandler.Json(HttpStatusCode.BadRequest, """{"data":null,"errors":{"cores":"type check ('integer') failed - got 'abc'","name":"too long"}}""");
            response.ReasonPhrase = "Parameter verification failed.";
            return response;
        }));

        var result = await client.SetAsync("/nodes/pve01/qemu/100/config", new Dictionary<string, object> { ["cores"] = "abc" });

        Assert.False(result.IsSuccessStatusCode);
        Assert.Equal("Parameter verification failed.", result.ReasonPhrase);
        Assert.True(result.ResponseInError);
        Assert.Equal($"cores : type check ('integer') failed - got 'abc'{Environment.NewLine}name : too long", result.GetError());
    }

    [Fact]
    public async Task Error_without_refused_parameters_has_no_error_text()
    {
        var client = FakeHandler.Client(new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.Forbidden, """{"data":null}""")));

        var result = await client.GetAsync("/nodes/pve01/qemu/100/config");

        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        Assert.False(result.ResponseInError);
        Assert.Equal(string.Empty, result.GetError());
    }

    [Fact]
    public async Task Answer_without_a_body_is_a_result_without_data()
    {
        var client = FakeHandler.Client(new FakeHandler(_ => FakeHandler.Text(HttpStatusCode.NotImplemented, "")));

        var result = await client.GetAsync("/nothing");

        Assert.Equal(HttpStatusCode.NotImplemented, result.StatusCode);
        Assert.False(result.ResponseHasData);
        Assert.False(result.ResponseInError);
    }

    [Fact]
    public async Task Request_that_gets_no_answer_is_a_result_with_the_reason()
    {
        var client = FakeHandler.Client(new FakeHandler(_ => throw new HttpRequestException("No such host is known. (pve01:8006)")));

        var result = await client.GetAsync("/version");

        Assert.False(result.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        Assert.Equal("No such host is known. (pve01:8006)", result.ReasonPhrase);
        Assert.False(result.ResponseHasData);
    }

    [Fact]
    public async Task Request_completed_is_raised_for_every_request_also_when_it_fails()
    {
        var fail = false;
        var client = FakeHandler.Client(new FakeHandler(_ => fail
            ? throw new HttpRequestException("Connection refused")
            : FakeHandler.Json(HttpStatusCode.OK, """{"data":{}}""")));
        var seen = new List<Result>();
        client.RequestCompleted += (sender, result) =>
        {
            Assert.Same(client, sender);
            seen.Add(result);
        };

        var first = await client.GetAsync("/version");
        fail = true;
        var second = await client.GetAsync("/version");

        Assert.Equal([first, second], seen);
        Assert.False(second.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Png_answer_is_returned_as_a_data_uri_of_the_bytes()
    {
        byte[] png = [0x89, (byte)'P', (byte)'N', (byte)'G', 0x0d, 0x0a, 0x1a, 0x0a, 0xff, 0x00, 0xfe];
        var handler = new FakeHandler(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(png) });
        var client = FakeHandler.Client(handler);
        client.ResponseType = ResponseType.Png;

        var result = await client.GetAsync("/nodes/pve01/rrd", new Dictionary<string, object> { ["ds"] = "cpu", ["timeframe"] = "day" });

        Assert.Equal("/api2/png/nodes/pve01/rrd", handler.Requests.Single().RequestUri!.AbsolutePath);
        Assert.Equal("data:image/png;base64," + Convert.ToBase64String(png), (string)result.Response);
        Assert.Equal(ResponseType.Png, result.ResponseType);
        Assert.False(result.ResponseInError);
    }

    [Fact]
    public async Task Log_does_not_show_secrets()
    {
        var handler = new FakeHandler(request => request.RequestUri!.AbsolutePath.EndsWith("/access/ticket")
            ? FakeHandler.Json(HttpStatusCode.OK, """{"data":{"ticket":"PVE:root@pam:SECRETTICKET","CSRFPreventionToken":"SECRETCSRF","username":"root@pam"}}""")
            : request.RequestUri.AbsolutePath.Contains("/token/")
                ? FakeHandler.Json(HttpStatusCode.OK, """{"data":{"full-tokenid":"a@pve!t1","value":"SECRETTOKENVALUE","info":{"comment":"visible-comment"}}}""")
                : FakeHandler.Json(HttpStatusCode.OK, """{"data":{}}"""));
        var client = FakeHandler.Client(handler);
        var log = LogOf(client);

        Assert.True(await client.LoginAsync("root@pam", "SECRETPASSWORD"));
        await client.GetAsync("/nodes/pve01/qemu/100/vncwebsocket", new Dictionary<string, object> { ["port"] = 5900, ["vncticket"] = "SECRETQUERY" });
        await client.SetAsync("/access/password", new Dictionary<string, object> { ["userid"] = "a@pve", ["password"] = "SECRETNEWPASSWORD" });
        await client.CreateAsync("/access/users/a@pve/token/t1", new Dictionary<string, object> { ["comment"] = "visible-comment" });

        var text = string.Join(Environment.NewLine, log.Lines);
        Assert.NotEmpty(log.Lines);
        foreach (var secret in new[] { "SECRETPASSWORD", "SECRETTICKET", "SECRETCSRF", "SECRETQUERY", "SECRETNEWPASSWORD", "SECRETTOKENVALUE" })
        {
            Assert.False(text.Contains(secret), $"the log shows {secret}");
        }
        // what is not secret is still there
        Assert.Contains("root@pam", text);
        Assert.Contains("5900", text);
        Assert.Contains("visible-comment", text);
        Assert.Contains("/nodes/pve01/qemu/100/vncwebsocket", text);
    }
}
