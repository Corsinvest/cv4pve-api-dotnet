/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Net;
using Corsinvest.ProxmoxVE.Api.Extension.Utils;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests;

/// <summary>
/// Unit tests for safe VM and container destruction in <see cref="VmHelper"/> and <see cref="ClientExtension"/>.
/// </summary>
public class DestroyVmTests
{
    private const string UpidStop = "UPID:pve01:0001:qmstop:100:root@pam:";
    private const string UpidDestroy = "UPID:pve01:0002:qmdestroy:100:root@pam:";

    private const string TaskFinishedJson = """{"data":{"status":"stopped","exitstatus":"OK"}}""";
    private const string VmStoppedStatusJson = """{"data":{"status":"stopped","qmpstatus":"stopped"}}""";
    private const string VmRunningStatusJson = """{"data":{"status":"running","qmpstatus":"running"}}""";

    private const string ClusterResourcesJson = """
    {
        "data": [
            { "type": "qemu", "vmid": 100, "node": "pve01", "name": "test-vm", "status": "stopped" },
            { "type": "lxc", "vmid": 200, "node": "pve01", "name": "test-ct", "status": "stopped" }
        ]
    }
    """;

    [Fact]
    public async Task DestroyVmAsync_Qemu_StoppedVm_DestroysWithPurgeAndUnreferencedDisks()
    {
        HttpRequestMessage destroyRequest = null;
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/status/current"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, VmStoppedStatusJson);
            }
            if (req.Method == HttpMethod.Delete && path.Contains("/qemu/100"))
            {
                destroyRequest = req;
                return FakeHandler.Json(HttpStatusCode.OK, $$"""{"data":"{{UpidDestroy}}"}""");
            }
            if (path.Contains("/tasks/"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, TaskFinishedJson);
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var result = await VmHelper.DestroyVmAsync(client, "pve01", VmType.Qemu, 100, timeout: 5000);

        Assert.True(result.IsSuccessStatusCode);
        Assert.NotNull(destroyRequest);
        Assert.Equal(HttpMethod.Delete, destroyRequest.Method);
        Assert.Contains("purge=1", destroyRequest.RequestUri!.Query);
        Assert.Contains("destroy-unreferenced-disks=1", destroyRequest.RequestUri.Query);
    }

    [Fact]
    public async Task DestroyVmAsync_Lxc_StoppedContainer_DestroysWithPurgeAndUnreferencedDisks()
    {
        HttpRequestMessage destroyRequest = null;
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/status/current"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, VmStoppedStatusJson);
            }
            if (req.Method == HttpMethod.Delete && path.Contains("/lxc/200"))
            {
                destroyRequest = req;
                return FakeHandler.Json(HttpStatusCode.OK, $$"""{"data":"{{UpidDestroy}}"}""");
            }
            if (path.Contains("/tasks/"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, TaskFinishedJson);
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var result = await VmHelper.DestroyVmAsync(client, "pve01", VmType.Lxc, 200, timeout: 5000);

        Assert.True(result.IsSuccessStatusCode);
        Assert.NotNull(destroyRequest);
        Assert.Equal(HttpMethod.Delete, destroyRequest.Method);
        Assert.Contains("purge=1", destroyRequest.RequestUri!.Query);
        Assert.Contains("destroy-unreferenced-disks=1", destroyRequest.RequestUri.Query);
    }

    [Fact]
    public async Task DestroyVmAsync_RunningVm_StopsFirst_ThenDestroys()
    {
        var stopCalled = false;
        var destroyCalled = false;

        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/status/current"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, VmRunningStatusJson);
            }
            if (path.EndsWith("/status/stop"))
            {
                stopCalled = true;
                return FakeHandler.Json(HttpStatusCode.OK, $$"""{"data":"{{UpidStop}}"}""");
            }
            if (req.Method == HttpMethod.Delete && path.Contains("/qemu/100"))
            {
                destroyCalled = true;
                return FakeHandler.Json(HttpStatusCode.OK, $$"""{"data":"{{UpidDestroy}}"}""");
            }
            if (path.Contains("/tasks/"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, TaskFinishedJson);
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var result = await VmHelper.DestroyVmAsync(client, "pve01", VmType.Qemu, 100, stopIfRunning: true, timeout: 5000, stopTimeout: 5000);

        Assert.True(result.IsSuccessStatusCode);
        Assert.True(stopCalled, "Stop was not called for a running VM before destroy");
        Assert.True(destroyCalled, "Destroy was not called after stopping");
    }

    [Fact]
    public async Task DestroyVmAsync_Extension_ByVmId_ResolvesNodeAndTypeAutomatically()
    {
        var destroyCalled = false;
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.Contains("/cluster/resources"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, ClusterResourcesJson);
            }
            if (path.EndsWith("/status/current"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, VmStoppedStatusJson);
            }
            if (req.Method == HttpMethod.Delete && path.Contains("/qemu/100"))
            {
                destroyCalled = true;
                return FakeHandler.Json(HttpStatusCode.OK, $$"""{"data":"{{UpidDestroy}}"}""");
            }
            if (path.Contains("/tasks/"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, TaskFinishedJson);
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var result = await client.DestroyVmAsync(100, timeout: 5000);

        Assert.True(result.IsSuccessStatusCode);
        Assert.True(destroyCalled);
    }

    [Fact]
    public async Task DestroyVmAsync_Extension_ByVmId_NotFound_ThrowsArgumentException()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.Contains("/cluster/resources"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, ClusterResourcesJson);
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        await Assert.ThrowsAsync<ArgumentException>(() => client.DestroyVmAsync(999));
    }
}
