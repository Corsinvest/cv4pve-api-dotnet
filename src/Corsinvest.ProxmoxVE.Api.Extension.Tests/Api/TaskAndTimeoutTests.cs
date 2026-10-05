/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Diagnostics;
using System.Net;
using Corsinvest.ProxmoxVE.Api.Extension.Utils;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Api;

public class TaskAndTimeoutTests
{
    private const string Upid = "UPID:pve01:0012A3F4:05C1B2D3:6720F1A0:qmsnapshot:100:root@pam:";

    // waits as long as the caller allows, like a node that does not answer
    private sealed class HangingHandler : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
            return new(HttpStatusCode.OK);
        }
    }

    private static PveClient ClientAnswering(HttpStatusCode status, string json)
        => FakeHandler.Client(new FakeHandler(_ => FakeHandler.Json(status, json)));

    [Fact]
    public async Task Wait_on_a_result_refused_with_403_has_nothing_to_wait_for()
    {
        var client = ClientAnswering(HttpStatusCode.Forbidden, """{"data":null}""");
        var result = await client.CreateAsync("/nodes/pve01/qemu/100/snapshot");

        Assert.False(result.IsSuccessStatusCode);
        Assert.True(await client.WaitForTaskToFinishAsync(result));
    }

    [Fact]
    public async Task Wait_on_a_result_of_a_request_that_never_reached_the_node_has_nothing_to_wait_for()
    {
        var client = FakeHandler.Client(new FakeHandler(_ => throw new HttpRequestException("Connection refused")));
        var result = await client.CreateAsync("/nodes/pve01/qemu/100/snapshot");

        Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        Assert.True(await client.WaitForTaskToFinishAsync(result));
    }

    [Fact]
    public async Task Wait_on_a_result_refused_with_400_and_errors_has_nothing_to_wait_for()
    {
        var client = ClientAnswering(HttpStatusCode.BadRequest, """{"data":null,"errors":{"snapname":"invalid format"}}""");
        var result = await client.CreateAsync("/nodes/pve01/qemu/100/snapshot");

        Assert.True(result.ResponseInError);
        Assert.True(await client.WaitForTaskToFinishAsync(result));
    }

    [Fact]
    public async Task Wait_on_a_successful_result_without_a_task_has_nothing_to_wait_for()
    {
        var client = ClientAnswering(HttpStatusCode.OK, """{"data":null}""");
        var result = await client.SetAsync("/nodes/pve01/qemu/100/config");

        Assert.True(result.IsSuccessStatusCode);
        Assert.True(await client.WaitForTaskToFinishAsync(result));
    }

    [Fact]
    public async Task Snapshot_helper_returns_the_refused_result()
    {
        var client = ClientAnswering(HttpStatusCode.Forbidden, """{"data":null}""");

        var result = await SnapshotHelper.CreateSnapshotAsync(client, "pve01", VmType.Qemu, 100, "snap", "description", false, 1000);

        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
    }

    [Fact]
    public async Task Exit_status_of_a_running_task_is_null()
    {
        var client = ClientAnswering(HttpStatusCode.OK, """{"data":{"status":"running","upid":"UPID","node":"pve01"}}""".Replace("UPID", Upid));

        Assert.True(await client.TaskIsRunningAsync(Upid));
        Assert.Null(await client.GetExitStatusTaskAsync(Upid));
    }

    [Fact]
    public async Task Exit_status_of_a_stopped_task_is_read()
    {
        var client = ClientAnswering(HttpStatusCode.OK, """{"data":{"status":"stopped","exitstatus":"OK","upid":"UPID"}}""".Replace("UPID", Upid));

        Assert.Equal("OK", await client.GetExitStatusTaskAsync(Upid));
    }

    [Fact]
    public async Task Client_timeout_gives_408()
    {
        var client = new PveClient("pve01", 8006, new HttpClient(new HangingHandler())) { Timeout = TimeSpan.FromMilliseconds(200) };

        var result = await client.GetAsync("/version");

        Assert.Equal(HttpStatusCode.RequestTimeout, result.StatusCode);
    }

    [Fact]
    public async Task HttpClient_timeout_gives_408()
    {
        var httpClient = new HttpClient(new HangingHandler()) { Timeout = TimeSpan.FromMilliseconds(200) };
        var client = new PveClient("pve01", 8006, httpClient);

        var result = await client.GetAsync("/version");

        Assert.Equal(HttpStatusCode.RequestTimeout, result.StatusCode);
    }

    [Fact]
    public void Internal_HttpClient_leaves_the_timeout_to_the_client()
    {
        //the HttpClient would stop every request at 100 seconds, also with a longer Timeout of the client
        var client = new PveClient("pve01");

        Assert.Equal(Timeout.InfiniteTimeSpan, client.GetHttpClient().Timeout);
    }

    [Fact]
    public async Task Upload_stops_at_its_own_timeout()
    {
        var client = new PveClient("pve01", 8006, new HttpClient(new HangingHandler()));
        using var file = new MemoryStream([1, 2, 3]);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => client.UploadFileToStorageAsync("pve01", "local", "iso", file, "debian.iso", CancellationToken.None, secondsTimeout: 1));
    }

    [Fact]
    public async Task Wait_on_a_result_with_a_task_reads_the_status_of_the_task()
    {
        var handler = new FakeHandler(request => request.Method == HttpMethod.Post
            ? FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID"}""".Replace("UPID", Upid))
            : FakeHandler.Json(HttpStatusCode.OK, """{"data":{"status":"stopped","exitstatus":"OK"}}"""));
        var client = FakeHandler.Client(handler);
        var result = await client.CreateAsync("/nodes/pve01/qemu/100/snapshot");

        Assert.True(await client.WaitForTaskToFinishAsync(result, wait: 10));
        Assert.EndsWith("/nodes/pve01/tasks/" + Upid + "/status", Uri.UnescapeDataString(handler.Requests.Last().RequestUri!.AbsoluteUri));
    }

    [Fact]
    public async Task Wait_on_a_result_with_a_task_still_running_at_the_timeout_is_false()
    {
        var handler = new FakeHandler(request => request.Method == HttpMethod.Post
            ? FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID"}""".Replace("UPID", Upid))
            : FakeHandler.Json(HttpStatusCode.OK, """{"data":{"status":"running"}}"""));
        var client = FakeHandler.Client(handler);
        var result = await client.CreateAsync("/nodes/pve01/qemu/100/snapshot");

        Assert.False(await client.WaitForTaskToFinishAsync(result, wait: 10, timeout: 50));
    }

    [Fact]
    public async Task Host_that_does_not_answer_is_given_up_after_the_timeout()
    {
        // 192.0.2.0/24 is reserved for documentation (RFC 5737): nothing answers
        var endpoint = new ClientHelper.HostEndpoint("192.0.2.1", 8006);
        var watch = Stopwatch.StartNew();

        var reachable = await ClientHelper.IsHostReachableAsync(endpoint, timeout: 500);

        watch.Stop();
        Assert.False(reachable);
        Assert.True(watch.ElapsedMilliseconds < 3000, $"gave up after {watch.ElapsedMilliseconds} ms with a timeout of 500 ms");
    }
}
