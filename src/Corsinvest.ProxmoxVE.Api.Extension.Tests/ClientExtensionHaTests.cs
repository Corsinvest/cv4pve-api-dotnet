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
    public async Task GetHaResourceAsync_ReturnsModelWhenInList()
    {
        var handler = new FakeHandler(request =>
        {
            if (request.Method == HttpMethod.Get && request.RequestUri!.AbsolutePath.EndsWith("/cluster/ha/resources"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """
                    {"data":[{"sid":"vm:100","max_restart":2,"state":"started"},{"sid":"ct:200","state":"stopped"}]}
                    """);
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);

        var resourceBySid = await client.GetHaResourceAsync("vm:100");
        Assert.NotNull(resourceBySid);
        Assert.Equal("vm:100", resourceBySid.Sid);
        Assert.Equal("started", resourceBySid.State);
        Assert.Equal(2, resourceBySid.MaxRestart);

        var resourceByVmId = await client.GetHaResourceAsync(200, VmType.Lxc);
        Assert.NotNull(resourceByVmId);
        Assert.Equal("ct:200", resourceByVmId.Sid);
        Assert.Equal("stopped", resourceByVmId.State);
    }

    [Fact]
    public async Task GetHaResourceAsync_ReturnsNullWhenNotInList()
    {
        var requests = new List<HttpRequestMessage>();
        var handler = new FakeHandler(request =>
        {
            requests.Add(request);
            if (request.Method == HttpMethod.Get && request.RequestUri!.AbsolutePath.EndsWith("/cluster/ha/resources"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":[{"sid":"vm:200","state":"started"}]}""");
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var resource = await client.GetHaResourceAsync(100, VmType.Qemu);
        Assert.Null(resource);

        var resourceBySid = await client.GetHaResourceAsync("vm:100");
        Assert.Null(resourceBySid);

        var nullSid = await client.GetHaResourceAsync(null);
        Assert.Null(nullSid);

        var emptySid = await client.GetHaResourceAsync("   ");
        Assert.Null(emptySid);

        // Only the first two calls should have queried the API; null/whitespace should short-circuit
        Assert.Equal(2, requests.Count);
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
            failback: true
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
        Assert.Null(body["group"]);
    }

    [Fact]
    public async Task EnsureHaResourceAsync_CallsCreateWhenAbsent()
    {
        var requests = new List<HttpRequestMessage>();
        string createBody = null;

        var handler = new FakeHandler(request =>
        {
            requests.Add(request);
            if (request.Method == HttpMethod.Get && request.RequestUri!.AbsolutePath.EndsWith("/cluster/ha/resources"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":[]}""");
            }
            if (request.Method == HttpMethod.Post && request.RequestUri!.AbsolutePath.EndsWith("/cluster/ha/resources"))
            {
                createBody = request.Content?.ReadAsStringAsync().Result;
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":null}""");
            }
            return FakeHandler.Json(HttpStatusCode.BadRequest, "{}");
        });

        var client = FakeHandler.Client(handler);
        var result = await client.EnsureHaResourceAsync(
            vmId: 100,
            vmType: VmType.Qemu,
            state: "started",
            comment: "created resource",
            maxRestart: 2,
            maxRelocate: 1,
            autoRebalance: true,
            failback: false
        );

        Assert.True(result.IsSuccessStatusCode);
        Assert.Equal(2, requests.Count);
        Assert.Equal(HttpMethod.Get, requests[0].Method);
        Assert.Equal(HttpMethod.Post, requests[1].Method);
        Assert.EndsWith("/cluster/ha/resources", requests[1].RequestUri!.AbsolutePath);

        Assert.NotNull(createBody);
        var body = JObject.Parse(createBody);
        Assert.Equal("vm:100", (string)body["sid"]);
        Assert.Equal("started", (string)body["state"]);
        Assert.Equal("created resource", (string)body["comment"]);
        Assert.Equal(2, (int)body["max_restart"]);
        Assert.Equal(1, (int)body["max_relocate"]);
        Assert.Equal(1, (int)body["auto-rebalance"]);
        Assert.Equal(0, (int)body["failback"]);
        Assert.Null(body["group"]);

        requests.Clear();
        createBody = null;
        var resultWithoutState = await client.EnsureHaResourceAsync(100);
        Assert.True(resultWithoutState.IsSuccessStatusCode);
        Assert.Equal(2, requests.Count);
        Assert.Equal(HttpMethod.Get, requests[0].Method);
        Assert.Equal(HttpMethod.Post, requests[1].Method);
        Assert.EndsWith("/cluster/ha/resources", requests[1].RequestUri!.AbsolutePath);
        Assert.NotNull(createBody);
        var bodyWithoutState = JObject.Parse(createBody);
        Assert.Equal("vm:100", (string)bodyWithoutState["sid"]);
        Assert.False(bodyWithoutState.ContainsKey("state"));
        Assert.Null(bodyWithoutState["state"]);
    }

    [Fact]
    public async Task EnsureHaResourceAsync_CallsUpdateWhenPresent()
    {
        var requests = new List<HttpRequestMessage>();
        string updateBody = null;

        var handler = new FakeHandler(request =>
        {
            requests.Add(request);
            if (request.Method == HttpMethod.Get && request.RequestUri!.AbsolutePath.EndsWith("/cluster/ha/resources"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":[{"sid":"vm:100","state":"stopped"}]}""");
            }
            if (request.Method == HttpMethod.Put && request.RequestUri!.AbsolutePath.EndsWith("/cluster/ha/resources/vm:100"))
            {
                updateBody = request.Content?.ReadAsStringAsync().Result;
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":null}""");
            }
            return FakeHandler.Json(HttpStatusCode.BadRequest, "{}");
        });

        var client = FakeHandler.Client(handler);
        var result = await client.EnsureHaResourceAsync(
            vmId: 100,
            vmType: VmType.Qemu,
            state: "started",
            comment: "updated resource",
            maxRestart: 3,
            maxRelocate: 2,
            autoRebalance: false,
            failback: true
        );

        Assert.True(result.IsSuccessStatusCode);
        Assert.Equal(2, requests.Count);
        Assert.Equal(HttpMethod.Get, requests[0].Method);
        Assert.Equal(HttpMethod.Put, requests[1].Method);
        Assert.EndsWith("/cluster/ha/resources/vm:100", requests[1].RequestUri!.AbsolutePath);

        Assert.NotNull(updateBody);
        var body = JObject.Parse(updateBody);
        Assert.Equal("started", (string)body["state"]);
        Assert.Equal("updated resource", (string)body["comment"]);
        Assert.Equal(3, (int)body["max_restart"]);
        Assert.Equal(2, (int)body["max_relocate"]);
        Assert.Equal(0, (int)body["auto-rebalance"]);
        Assert.Equal(1, (int)body["failback"]);
        Assert.Null(body["group"]);
    }

    [Fact]
    public async Task EnsureHaResourceAsync_WhenResourceExistsAndStateNull_DoesNotSendStateInPut()
    {
        var requests = new List<HttpRequestMessage>();
        string updateBody = null;

        var handler = new FakeHandler(request =>
        {
            requests.Add(request);
            if (request.Method == HttpMethod.Get && request.RequestUri!.AbsolutePath.EndsWith("/cluster/ha/resources"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":[{"sid":"vm:100","state":"stopped"}]}""");
            }
            if (request.Method == HttpMethod.Put && request.RequestUri!.AbsolutePath.EndsWith("/cluster/ha/resources/vm:100"))
            {
                updateBody = request.Content?.ReadAsStringAsync().Result;
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":null}""");
            }
            return FakeHandler.Json(HttpStatusCode.BadRequest, "{}");
        });

        var client = FakeHandler.Client(handler);
        var result = await client.EnsureHaResourceAsync(
            vmId: 100,
            vmType: VmType.Qemu,
            comment: "maintenance mode"
        );

        Assert.True(result.IsSuccessStatusCode);
        Assert.Equal(2, requests.Count);
        Assert.Equal(HttpMethod.Get, requests[0].Method);
        Assert.Equal(HttpMethod.Put, requests[1].Method);
        Assert.EndsWith("/cluster/ha/resources/vm:100", requests[1].RequestUri!.AbsolutePath);

        Assert.NotNull(updateBody);
        var body = JObject.Parse(updateBody);
        Assert.False(body.ContainsKey("state"));
        Assert.Null(body["state"]);
        Assert.Equal("maintenance mode", (string)body["comment"]);

        requests.Clear();
        updateBody = null;
        var resultWithoutArgs = await client.EnsureHaResourceAsync(100);
        Assert.True(resultWithoutArgs.IsSuccessStatusCode);
        Assert.Equal(2, requests.Count);
        Assert.Equal(HttpMethod.Get, requests[0].Method);
        Assert.Equal(HttpMethod.Put, requests[1].Method);
        Assert.EndsWith("/cluster/ha/resources/vm:100", requests[1].RequestUri!.AbsolutePath);
        Assert.NotNull(updateBody);
        var bodyWithoutArgs = JObject.Parse(updateBody);
        Assert.False(bodyWithoutArgs.ContainsKey("state"));
        Assert.Null(bodyWithoutArgs["state"]);
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
            comment: "lxc container",
            maxRestart: 2,
            maxRelocate: 1,
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
        Assert.Equal("lxc container", (string)body["comment"]);
        Assert.Equal(2, (int)body["max_restart"]);
        Assert.Equal(1, (int)body["max_relocate"]);
        Assert.Equal(0, (int)body["auto-rebalance"]);
        Assert.Equal(1, (int)body["failback"]);
        Assert.Null(body["group"]);
    }

    [Fact]
    public async Task DeleteHaResourceAsync_WhenAbsent_WithIgnoreNotFound_ReturnsTrueWithoutCallingDelete()
    {
        var requests = new List<HttpRequestMessage>();

        var handler = new FakeHandler(request =>
        {
            requests.Add(request);
            if (request.Method == HttpMethod.Get && request.RequestUri!.AbsolutePath.EndsWith("/cluster/ha/resources"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":[]}""");
            }
            return FakeHandler.Json(HttpStatusCode.InternalServerError, "{}");
        });

        var client = FakeHandler.Client(handler);
        var success = await client.DeleteHaResourceAsync(100, VmType.Qemu, ignoreNotFound: true);

        Assert.True(success);
        Assert.Single(requests);
        Assert.Equal(HttpMethod.Get, requests[0].Method);
        Assert.EndsWith("/cluster/ha/resources", requests[0].RequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task DeleteHaResourceAsync_WhenPresent_CallsDeleteWithPurge()
    {
        var requests = new List<HttpRequestMessage>();

        var handler = new FakeHandler(request =>
        {
            requests.Add(request);
            if (request.Method == HttpMethod.Get && request.RequestUri!.AbsolutePath.EndsWith("/cluster/ha/resources"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":[{"sid":"vm:100"}]}""");
            }
            if (request.Method == HttpMethod.Delete && request.RequestUri!.AbsolutePath.EndsWith("/cluster/ha/resources/vm:100"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":null}""");
            }
            return FakeHandler.Json(HttpStatusCode.BadRequest, "{}");
        });

        var client = FakeHandler.Client(handler);
        var success = await client.DeleteHaResourceAsync("vm:100", purge: true, ignoreNotFound: true);

        Assert.True(success);
        Assert.Equal(2, requests.Count);
        Assert.Equal(HttpMethod.Get, requests[0].Method);
        Assert.Equal(HttpMethod.Delete, requests[1].Method);
        Assert.EndsWith("/cluster/ha/resources/vm:100", requests[1].RequestUri!.AbsolutePath);
        Assert.Contains("purge=1", requests[1].RequestUri!.Query);
    }

    [Fact]
    public async Task DeleteHaResourceAsync_WhenIgnoreNotFoundFalse_CallsDeleteDirectly()
    {
        var requests = new List<HttpRequestMessage>();

        var handler = new FakeHandler(request =>
        {
            requests.Add(request);
            if (request.Method == HttpMethod.Delete && request.RequestUri!.AbsolutePath.EndsWith("/cluster/ha/resources/vm:100"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":null}""");
            }
            return FakeHandler.Json(HttpStatusCode.BadRequest, "{}");
        });

        var client = FakeHandler.Client(handler);
        var success = await client.DeleteHaResourceAsync("vm:100", purge: false, ignoreNotFound: false);

        Assert.True(success);
        Assert.Single(requests);
        Assert.Equal(HttpMethod.Delete, requests[0].Method);
        Assert.EndsWith("/cluster/ha/resources/vm:100", requests[0].RequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task CreateHaResourceAsync_LxcContainer_FormatsCtPrefixAndOmitsNulls()
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
        var result = await client.CreateHaResourceAsync(vmId: 200, vmType: VmType.Lxc);

        Assert.True(result.IsSuccessStatusCode);
        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest.Method);
        Assert.EndsWith("/cluster/ha/resources", capturedRequest.RequestUri!.AbsolutePath);

        Assert.NotNull(requestBody);
        var body = JObject.Parse(requestBody);
        Assert.Equal("ct:200", (string)body["sid"]);
        Assert.False(body.ContainsKey("state"));
        Assert.Null(body["state"]);
        Assert.Null(body["comment"]);
        Assert.Null(body["max_restart"]);
        Assert.Null(body["max_relocate"]);
        Assert.Null(body["auto-rebalance"]);
        Assert.Null(body["failback"]);
        Assert.Null(body["group"]);
    }

    [Fact]
    public async Task DeleteHaResourceAsync_WhenIgnoreNotFoundFalse_ReturnsFalseOnServerError()
    {
        var handler = new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.InternalServerError, """{"errors":"cluster error"}"""));
        var client = FakeHandler.Client(handler);

        var success = await client.DeleteHaResourceAsync("vm:100", ignoreNotFound: false);
        Assert.False(success);
    }

    [Fact]
    public async Task DeleteHaResourceAsync_WhenSidNullOrWhitespace_ReturnsIgnoreNotFoundWithoutCallingApi()
    {
        var requests = new List<HttpRequestMessage>();
        var handler = new FakeHandler(request =>
        {
            requests.Add(request);
            return FakeHandler.Json(HttpStatusCode.OK, """{"data":[]}""");
        });
        var client = FakeHandler.Client(handler);

        var successNull = await client.DeleteHaResourceAsync(null, ignoreNotFound: true);
        Assert.True(successNull);

        var failNull = await client.DeleteHaResourceAsync(null, ignoreNotFound: false);
        Assert.False(failNull);

        var successWhitespace = await client.DeleteHaResourceAsync("   ", ignoreNotFound: true);
        Assert.True(successWhitespace);

        Assert.Empty(requests);
    }

    [Fact]
    public async Task DeleteHaResourceAsync_LxcContainer_DelegatesToCtPrefix()
    {
        var requests = new List<HttpRequestMessage>();
        var handler = new FakeHandler(request =>
        {
            requests.Add(request);
            if (request.Method == HttpMethod.Get)
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":[{"sid":"ct:200"}]}""");
            }
            if (request.Method == HttpMethod.Delete && request.RequestUri!.AbsolutePath.EndsWith("/cluster/ha/resources/ct:200"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":null}""");
            }
            return FakeHandler.Json(HttpStatusCode.BadRequest, "{}");
        });

        var client = FakeHandler.Client(handler);
        var success = await client.DeleteHaResourceAsync(200, VmType.Lxc, purge: true, ignoreNotFound: true);

        Assert.True(success);
        Assert.Equal(2, requests.Count);
        Assert.EndsWith("/cluster/ha/resources/ct:200", requests[1].RequestUri!.AbsolutePath);
    }
}
