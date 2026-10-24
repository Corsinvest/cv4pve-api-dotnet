/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Net;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests;

public class ClientExtensionHaTests
{
    [Fact]
    public async Task CheckHaStatusAsync_ReturnsTrueWhenHealthy()
    {
        var handler = new FakeHandler(request =>
        {
            if (request.RequestUri!.AbsolutePath.EndsWith("/cluster/ha/status/current"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":[]}""");
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var isHealthy = await client.CheckHaStatusAsync();

        Assert.True(isHealthy);
    }

    [Fact]
    public async Task GetHaResourceAsync_ReturnsModelWhenFound()
    {
        var handler = new FakeHandler(request =>
        {
            if (request.RequestUri!.AbsolutePath.EndsWith("/cluster/ha/resources/vm:100"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """
                    {"data":{"sid":"vm:100","group":"ha-group-1","max_restart":2,"state":"started"}}
                    """);
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var resource = await client.GetHaResourceAsync("vm:100");

        Assert.NotNull(resource);
        Assert.Equal("vm:100", resource.Sid);
        Assert.Equal("ha-group-1", resource.Group);
    }

    [Fact]
    public async Task GetHaResourceAsync_ReturnsNullOnNotFound()
    {
        var handler = new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.NotFound, """{"errors":"resource does not exist"}"""));

        var client = FakeHandler.Client(handler);
        var resource = await client.GetHaResourceAsync(100, VmType.Qemu);

        Assert.Null(resource);
    }

    [Fact]
    public async Task GetHaResourceAsync_ThrowsOnServerError()
    {
        var handler = new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.InternalServerError, """{"message":"cluster database error"}"""));

        var client = FakeHandler.Client(handler);
        await Assert.ThrowsAsync<PveResultException>(() => client.GetHaResourceAsync(100, VmType.Qemu));
    }

    [Fact]
    public async Task CreateHaResourceAsync_SendsAllPve9Parameters()
    {
        HttpRequestMessage capturedRequest = null;
        string requestBody = null;

        var handler = new FakeHandler(request =>
        {
            capturedRequest = request;
            requestBody = request.Content?.ReadAsStringAsync().Result;
            return FakeHandler.Json(HttpStatusCode.OK, """{"data":null}""");
        });

        var client = FakeHandler.Client(handler);
        var result = await client.CreateHaResourceAsync(
            vmId: 100,
            vmType: VmType.Qemu,
            state: "started",
            comment: "production web vm",
            maxRestart: 3,
            maxRelocate: 2,
            autoRebalance: true,
            failback: true,
            group: "ha-rule-group"
        );

        Assert.True(result.IsSuccessStatusCode);
        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest.Method);
        Assert.EndsWith("/cluster/ha/resources", capturedRequest.RequestUri!.AbsolutePath);

        Assert.NotNull(requestBody);
        var body = JObject.Parse(requestBody);
        Assert.Equal("vm:100", (string)body["sid"]);
        Assert.Equal("started", (string)body["state"]);
        Assert.Equal("production web vm", (string)body["comment"]);
        Assert.Equal(3, (int)body["max_restart"]);
        Assert.Equal(2, (int)body["max_relocate"]);
        Assert.Equal(1, (int)body["auto-rebalance"]);
        Assert.Equal(1, (int)body["failback"]);
        Assert.Equal("ha-rule-group", (string)body["group"]);
    }

    [Fact]
    public async Task EnsureHaResourceAsync_CreatesWhenAbsent()
    {
        var handler = new FakeHandler(request =>
        {
            if (request.Method == HttpMethod.Post)
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":null}""");
            }
            return FakeHandler.Json(HttpStatusCode.BadRequest, "{}");
        });

        var client = FakeHandler.Client(handler);
        var result = await client.EnsureHaResourceAsync(100, VmType.Qemu, maxRestart: 2);

        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task EnsureHaResourceAsync_UpdatesWhenAlreadyExists()
    {
        var requests = new List<HttpRequestMessage>();

        var handler = new FakeHandler(request =>
        {
            requests.Add(request);
            if (request.Method == HttpMethod.Post)
            {
                return FakeHandler.Json(HttpStatusCode.BadRequest, """{"errors":{"sid":"resource 'vm:100' already exists"}}""");
            }
            if (request.Method == HttpMethod.Put)
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":null}""");
            }
            return FakeHandler.Json(HttpStatusCode.BadRequest, "{}");
        });

        var client = FakeHandler.Client(handler);
        var result = await client.EnsureHaResourceAsync(100, VmType.Qemu, state: "started", maxRestart: 3);

        Assert.True(result.IsSuccessStatusCode);
        Assert.Equal(2, requests.Count);
        Assert.Equal(HttpMethod.Post, requests[0].Method);
        Assert.Equal(HttpMethod.Put, requests[1].Method);
        Assert.EndsWith("/cluster/ha/resources/vm:100", requests[1].RequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task UpdateHaResourceAsync_SendsPutWithParameters()
    {
        HttpRequestMessage capturedRequest = null;
        string requestBody = null;

        var handler = new FakeHandler(request =>
        {
            capturedRequest = request;
            requestBody = request.Content?.ReadAsStringAsync().Result;
            return FakeHandler.Json(HttpStatusCode.OK, """{"data":null}""");
        });

        var client = FakeHandler.Client(handler);
        var result = await client.UpdateHaResourceAsync(
            vmId: 200,
            vmType: VmType.Lxc,
            state: "stopped",
            autoRebalance: false,
            failback: true
        );

        Assert.True(result.IsSuccessStatusCode);
        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Put, capturedRequest.Method);
        Assert.EndsWith("/cluster/ha/resources/ct:200", capturedRequest.RequestUri!.AbsolutePath);

        Assert.NotNull(requestBody);
        var body = JObject.Parse(requestBody);
        Assert.Equal("stopped", (string)body["state"]);
        Assert.Equal(0, (int)body["auto-rebalance"]);
        Assert.Equal(1, (int)body["failback"]);
    }

    [Fact]
    public async Task DeleteHaResourceAsync_SendsDeleteWithPurge()
    {
        HttpRequestMessage capturedRequest = null;
        string requestBody = null;

        var handler = new FakeHandler(request =>
        {
            capturedRequest = request;
            requestBody = request.Content?.ReadAsStringAsync().Result;
            return FakeHandler.Json(HttpStatusCode.OK, """{"data":null}""");
        });

        var client = FakeHandler.Client(handler);
        var success = await client.DeleteHaResourceAsync("vm:100", purge: true);

        Assert.True(success);
        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Delete, capturedRequest.Method);
        Assert.EndsWith("/cluster/ha/resources/vm:100", capturedRequest.RequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task DeleteHaResourceAsync_IgnoresNotFoundWhenConfigured()
    {
        var handler = new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.NotFound, """{"errors":"resource 'vm:100' does not exist"}"""));

        var client = FakeHandler.Client(handler);
        var success = await client.DeleteHaResourceAsync(100, VmType.Qemu, ignoreNotFound: true);

        Assert.True(success);
    }
}
