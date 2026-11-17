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
/// Unit tests for safe VM and container destruction in <see cref="VmHelper"/>.
/// </summary>
public class DestroyVmTests
{
    private const string UpidStop = "UPID:pve01:0001:qmstop:100:root@pam:";
    private const string UpidDestroy = "UPID:pve01:0002:qmdestroy:100:root@pam:";

    private const string TaskFinishedJson = """{"data":{"status":"stopped","exitstatus":"OK"}}""";
    private const string VmStoppedStatusJson = """{"data":{"status":"stopped","qmpstatus":"stopped"}}""";
    private const string VmRunningStatusJson = """{"data":{"status":"running","qmpstatus":"running"}}""";

    [Fact]
    public async Task DestroyVmAsync_DefaultFlags_DoesNotIncludePurgeOrDestroyUnreferencedDisks()
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
        Assert.DoesNotContain("purge=1", destroyRequest.RequestUri!.Query);
        Assert.DoesNotContain("destroy-unreferenced-disks=1", destroyRequest.RequestUri.Query);
    }

    [Fact]
    public async Task DestroyVmAsync_ExplicitFlags_DestroysWithPurgeAndUnreferencedDisks()
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
        var result = await VmHelper.DestroyVmAsync(client, "pve01", VmType.Qemu, 100, purge: true, destroyUnreferencedDisks: true, timeout: 5000);

        Assert.True(result.IsSuccessStatusCode);
        Assert.NotNull(destroyRequest);
        Assert.Equal(HttpMethod.Delete, destroyRequest.Method);
        Assert.Contains("purge=1", destroyRequest.RequestUri!.Query);
        Assert.Contains("destroy-unreferenced-disks=1", destroyRequest.RequestUri.Query);
    }

    [Fact]
    public async Task DestroyVmAsync_Lxc_StoppedContainer_DestroysSuccessfully()
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
    public async Task DestroyVmAsync_StopFails_ReturnsFailureAndDoesNotAttemptDestroy()
    {
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
                return FakeHandler.Json(HttpStatusCode.InternalServerError, """{"errors":{"status":"cannot stop locked vm"}}""");
            }
            if (req.Method == HttpMethod.Delete)
            {
                destroyCalled = true;
                return FakeHandler.Json(HttpStatusCode.OK, $$"""{"data":"{{UpidDestroy}}"}""");
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var result = await VmHelper.DestroyVmAsync(client, "pve01", VmType.Qemu, 100, stopIfRunning: true);

        Assert.False(result.IsSuccessStatusCode);
        Assert.False(destroyCalled, "Destroy must not be attempted when stop fails");
    }

    [Fact]
    public async Task DestroyVmAsync_StopTaskFails_ThrowsPveResultExceptionAndDoesNotAttemptDestroy()
    {
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
                return FakeHandler.Json(HttpStatusCode.OK, $$"""{"data":"{{UpidStop}}"}""");
            }
            if (path.Contains($"/tasks/{UpidStop}/status"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":{"status":"stopped","exitstatus":"command failed: exit code 1"}}""");
            }
            if (req.Method == HttpMethod.Delete)
            {
                destroyCalled = true;
                return FakeHandler.Json(HttpStatusCode.OK, $$"""{"data":"{{UpidDestroy}}"}""");
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var ex = await Assert.ThrowsAsync<PveResultException>(() =>
            VmHelper.DestroyVmAsync(client, "pve01", VmType.Qemu, 100, stopIfRunning: true, timeout: 5000, stopTimeout: 5000));

        Assert.Contains("command failed: exit code 1", ex.Message);
        Assert.False(destroyCalled, "Destroy must not be attempted when stop task exit status is an error");
    }

    [Fact]
    public async Task DestroyVmAsync_DestroyTaskFails_ThrowsPveResultException()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/status/current"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, VmStoppedStatusJson);
            }
            if (req.Method == HttpMethod.Delete && path.Contains("/qemu/100"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, $$"""{"data":"{{UpidDestroy}}"}""");
            }
            if (path.Contains($"/tasks/{UpidDestroy}/status"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":{"status":"stopped","exitstatus":"cannot remove volume: dataset busy"}}""");
            }
            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var ex = await Assert.ThrowsAsync<PveResultException>(() =>
            VmHelper.DestroyVmAsync(client, "pve01", VmType.Qemu, 100, timeout: 5000));

        Assert.Contains("cannot remove volume: dataset busy", ex.Message);
    }
}
