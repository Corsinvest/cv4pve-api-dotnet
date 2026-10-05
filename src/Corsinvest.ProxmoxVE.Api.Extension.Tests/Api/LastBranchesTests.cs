/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Collections.Concurrent;
using System.Net;
using Corsinvest.ProxmoxVE.Api.Extension.Shell;
using Corsinvest.ProxmoxVE.Api.Metadata;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Api;

/// <summary>
/// Values that the types allow and the API does not have, and a terminal that loses its node.
/// </summary>
public class LastBranchesTests
{
    private sealed class RawClient(HttpClient httpClient) : PveClient("pve01", 8006, httpClient)
    {
        public Task<Result> SendAsync(MethodType methodType) => ExecuteRequestAsync("/version", methodType);
    }

    [Fact]
    public async Task Method_that_does_not_exist_is_refused_before_any_request()
    {
        var handler = new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK, """{"data":null}"""));

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => new RawClient(new HttpClient(handler)).SendAsync((MethodType)99));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => ApiRequest.ExecuteAsync(FakeHandler.Client(handler), new ApiCommand((MethodType)99, "/version", new Dictionary<string, object>())));
        Assert.Empty(handler.Requests);
    }

    [Fact]
    public async Task Response_type_that_does_not_exist_is_a_failed_result()
    {
        var client = FakeHandler.Client(new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK, """{"data":null}""")));
        client.ResponseType = (ResponseType)99;

        var result = await client.GetAsync("/version");

        Assert.False(result.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
    }

    [Fact]
    public async Task Resource_that_is_not_a_node_a_guest_or_a_storage_has_no_health_score()
    {
        var client = FakeHandler.Client(new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK,
            """{"data":[{"id":"sdn/pve01/zone1","type":"sdn","sdn":"zone1","node":"pve01","status":"ok"},{"id":"/pool/p1","type":"pool","pool":"p1"}]}""")));

        Assert.All(await client.GetResourcesAsync(ClusterResourceType.All), a => Assert.Null(a.HealthScoreCalculated));
    }

    [Theory]
    [InlineData("fraction_as_percentage", "R")]
    [InlineData("bytes", "R")]
    [InlineData("duration", "R")]
    [InlineData("timestamp", "R")]
    [InlineData("timestamp_gmt", "R")]
    [InlineData("other", "L")]
    public void Values_that_are_numbers_or_dates_are_aligned_to_the_right(string renderer, string alignment)
    {
        var root = new ClassApi();
        var doc = """[{"path":"/x","text":"x","leaf":1,"info":{"GET":{"method":"GET","name":"read","parameters":{"additionalProperties":0},"returns":{"type":"array","items":{"type":"object","properties":{"value":{"type":"integer","renderer":"RENDERER"}}}}}}}]""";
        foreach (var token in JArray.Parse(doc.Replace("RENDERER", renderer))) { _ = new ClassApi(token, root); }

        Assert.Equal(alignment, ClassApi.GetFromResource(root, "/x")!.Methods.Single().ReturnParameters.Single().GetAlignmentValue());
    }

    private sealed class Logs : ILoggerProvider, ILogger
    {
        public ConcurrentQueue<(LogLevel Level, string Message)> Entries { get; } = new();
        public ILogger CreateLogger(string categoryName) => this;
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => Entries.Enqueue((logLevel, formatter(state, exception)));
        public void Dispose() { }
    }

    [Fact]
    public async Task Terminal_that_loses_its_node_logs_the_error_and_can_be_disposed()
    {
        var logs = new Logs();
        using var loggerFactory = LoggerFactory.Create(a => a.SetMinimumLevel(LogLevel.Trace).AddProvider(logs));
        var drop = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await using var server = new LocalPveServer
        {
            Rest = request => request.Path.EndsWith("/termproxy")
                                ? (200, "OK", """{"data":{"ticket":"PVEVNC:T","port":5900,"user":"root@pam"}}""")
                                : (200, "OK", """{"data":{"ticket":"PVE:root@pam:TICKET","CSRFPreventionToken":"CSRF"}}"""),
            Socket = async (socket, _, cancellationToken) =>
            {
                await LocalPveServer.ReceiveAsync(socket, cancellationToken);
                await LocalPveServer.SendTextAsync(socket, "OK", cancellationToken);
                await LocalPveServer.SendTextAsync(socket, "root@pve01:~# ", cancellationToken);
                // the node goes away without closing the session
                await drop.Task;
                socket.Abort();
            },
        };
        var client = server.Client();
        client.LoggerFactory = loggerFactory;
        Assert.True(await client.LoginAsync("root@pam", "secret"));
        var term = new PveWebTermClient(client, "pve01") { PingInterval = TimeSpan.FromMilliseconds(50) };
        Assert.True(await term.ConnectAsync());

        drop.SetResult();
        for (var i = 0; i < 100 && !logs.Entries.Any(a => a.Message.Contains("[ReceiveLoop] unexpected error")); i++) { await Task.Delay(50); }

        Assert.Contains(logs.Entries, a => a.Level == LogLevel.Error && a.Message.Contains("[ReceiveLoop] unexpected error"));
        await Assert.ThrowsAnyAsync<Exception>(() => term.SendCommandAsync("ls"));
        Assert.False(await term.WaitForPromptAsync(0));

        await term.DisposeAsync();
        Assert.Contains(logs.Entries, a => a.Message == "Disconnected");
    }
}
