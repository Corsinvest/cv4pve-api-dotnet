/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Net;
using Corsinvest.ProxmoxVE.Api.Extension.Shell;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Shell;

public class ApiRequestTests
{
    private const string Upid = "UPID:pve01:0011D941:2E861852:6ABB7E1A:qmstart:100:root@pam:";

    private static ApiCommand Command(MethodType method, string resource, params (string, object)[] parameters)
        => new(method, resource, parameters.ToDictionary(a => a.Item1, a => a.Item2));

    [Fact]
    public async Task Object_answer_is_success_with_data()
    {
        var client = FakeHandler.Client(new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK, """{"data":{"version":"8.4.21"}}""")));
        var response = await ApiRequest.ExecuteAsync(client, Command(MethodType.Get, "/version"));

        Assert.True(response.IsSuccess);
        Assert.Equal(200, response.StatusCode);
        Assert.Equal("8.4.21", ((IDictionary<string, object>)response.Data!)["version"]);
        Assert.Null(response.Upid);
        Assert.Empty(response.ParameterErrors);
    }

    [Fact]
    public async Task Rejected_parameter_is_listed()
    {
        var client = FakeHandler.Client(new FakeHandler(_ =>
        {
            var answer = FakeHandler.Json(HttpStatusCode.BadRequest,
                """{"data":null,"errors":{"foo":"property is not defined in schema and the schema does not allow additional properties"}}""");
            answer.ReasonPhrase = "Parameter verification failed.";
            return answer;
        }));
        var response = await ApiRequest.ExecuteAsync(client, Command(MethodType.Get, "/nodes/pve01/status", ("foo", "1")));

        Assert.False(response.IsSuccess);
        Assert.Equal(400, response.StatusCode);
        Assert.Equal("Parameter verification failed.", response.Error);
        Assert.StartsWith("property is not defined", response.ParameterErrors["foo"]);
    }

    [Fact]
    public async Task Status_200_with_errors_is_not_a_success()
    {
        var client = FakeHandler.Client(new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK, """{"data":null,"errors":{"vmid":"invalid"}}""")));
        var response = await ApiRequest.ExecuteAsync(client, Command(MethodType.Get, "/cluster/nextid"));

        Assert.False(response.IsSuccess);
        Assert.Equal("invalid", response.ParameterErrors["vmid"]);
    }

    [Fact]
    public async Task Answer_that_is_not_json_is_not_a_success()
    {
        var client = FakeHandler.Client(new FakeHandler(_ => FakeHandler.Text(HttpStatusCode.OK, "<html>proxy</html>")));
        var response = await ApiRequest.ExecuteAsync(client, Command(MethodType.Get, "/version"));

        Assert.False(response.IsSuccess);
        Assert.Equal(502, response.StatusCode);
        Assert.Null(response.Data);
    }

    [Fact]
    public async Task Parameters_are_sent()
    {
        var handler = new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK, """{"data":[]}"""));
        await ApiRequest.ExecuteAsync(FakeHandler.Client(handler), Command(MethodType.Get, "/cluster/resources", ("type", "vm")));

        Assert.Equal("/api2/json/cluster/resources?type=vm", handler.Requests.Single().RequestUri!.PathAndQuery);
    }

    [Fact]
    public async Task Task_id_is_returned_without_waiting()
    {
        var client = FakeHandler.Client(new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK, $$"""{"data":"{{Upid}}"}""")));
        var response = await ApiRequest.ExecuteAsync(client, Command(MethodType.Create, "/nodes/pve01/qemu/100/status/start"));

        Assert.Equal(Upid, response.Upid);
        Assert.Null(response.Task);
    }

    [Theory]
    [InlineData("OK", true)]
    [InlineData("OK: 1 warning", true)]
    [InlineData("command 'qm start' failed", false)]
    public async Task Wait_reports_how_the_task_ended(string exitStatus, bool succeeded)
    {
        var client = FakeHandler.Client(new FakeHandler(request => request.RequestUri!.AbsolutePath.EndsWith("/status")
            ? FakeHandler.Json(HttpStatusCode.OK, $$$"""{"data":{"status":"stopped","exitstatus":"{{{exitStatus}}}"}}""")
            : FakeHandler.Json(HttpStatusCode.OK, $$"""{"data":"{{Upid}}"}""")));

        var response = await ApiRequest.ExecuteAsync(client,
                                                    Command(MethodType.Create, "/nodes/pve01/qemu/100/status/start"),
                                                    new ApiWaitOptions(Interval: TimeSpan.FromMilliseconds(10)));

        Assert.Equal(new ApiTaskOutcome(true, exitStatus, succeeded), response.Task);
    }

    [Fact]
    public async Task Wait_stops_at_the_timeout()
    {
        var client = FakeHandler.Client(new FakeHandler(request => request.RequestUri!.AbsolutePath.EndsWith("/status")
            ? FakeHandler.Json(HttpStatusCode.OK, """{"data":{"status":"running"}}""")
            : FakeHandler.Json(HttpStatusCode.OK, $$"""{"data":"{{Upid}}"}""")));

        var response = await ApiRequest.ExecuteAsync(client,
                                                    Command(MethodType.Create, "/nodes/pve01/qemu/100/status/start"),
                                                    new ApiWaitOptions(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(10)));

        Assert.Equal(new ApiTaskOutcome(false, null, false), response.Task);
    }

    [Fact]
    public async Task Task_status_that_cannot_be_read_is_not_finished()
    {
        var client = FakeHandler.Client(new FakeHandler(request => request.RequestUri!.AbsolutePath.EndsWith("/status")
            ? FakeHandler.Json(HttpStatusCode.InternalServerError, """{"data":null}""")
            : FakeHandler.Json(HttpStatusCode.OK, $$"""{"data":"{{Upid}}"}""")));

        var response = await ApiRequest.ExecuteAsync(client,
                                                    Command(MethodType.Create, "/nodes/pve01/qemu/100/status/start"),
                                                    new ApiWaitOptions(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(10)));

        Assert.Equal(new ApiTaskOutcome(false, null, false), response.Task);
    }

    [Theory]
    [InlineData("")]
    [InlineData("""{"status":"ok"}""")]
    public async Task Success_status_without_data_is_not_a_success(string body)
    {
        var client = FakeHandler.Client(new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK, body)));
        var response = await ApiRequest.ExecuteAsync(client, Command(MethodType.Get, "/version"));

        Assert.False(response.IsSuccess);
        Assert.Contains("no data", response.Error);
    }

    [Fact]
    public async Task Cancelled_before_the_call_sends_nothing()
    {
        var handler = new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK, """{"data":null}"""));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => ApiRequest.ExecuteAsync(FakeHandler.Client(handler), Command(MethodType.Get, "/version"), null, new CancellationToken(true)));
        Assert.Empty(handler.Requests);
    }

    [Fact]
    public async Task Wait_without_limit_stops_when_cancelled()
    {
        var client = FakeHandler.Client(new FakeHandler(request => request.RequestUri!.AbsolutePath.EndsWith("/status")
            ? FakeHandler.Json(HttpStatusCode.OK, """{"data":{"status":"running"}}""")
            : FakeHandler.Json(HttpStatusCode.OK, $$"""{"data":"{{Upid}}"}""")));
        using var cancel = new CancellationTokenSource(TimeSpan.FromMilliseconds(200));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => ApiRequest.ExecuteAsync(client,
                                          Command(MethodType.Create, "/nodes/pve01/qemu/100/status/start"),
                                          new ApiWaitOptions(null, TimeSpan.FromMilliseconds(10)),
                                          cancel.Token));
    }
}
