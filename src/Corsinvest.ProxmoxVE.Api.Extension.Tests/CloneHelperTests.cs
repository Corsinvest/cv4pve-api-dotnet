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
/// Unit tests for unified VM and container cloning and template helpers in <see cref="CloneHelper"/>.
/// </summary>
public class CloneHelperTests
{
    private const string UpidClone = "UPID:pve01:0001:qmclone:9000:root@pam:";
    private const string UpidTemplate = "UPID:pve01:0002:qmtemplate:100:root@pam:";
    private const string TaskFinishedOkJson = """{"data":{"status":"stopped","exitstatus":"OK"}}""";

    [Fact]
    public async Task GetNextVmIdAsync_ReturnsNextAvailableId()
    {
        var handler = new FakeHandler(req =>
        {
            if (req.RequestUri!.AbsolutePath.EndsWith("/cluster/nextid"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":"105"}""");
            }

            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var nextId = await CloneHelper.GetNextVmIdAsync(client);

        Assert.Equal(105, nextId);
    }

    [Fact]
    public async Task GetNextVmIdAsync_WithPreferredId_SendsVmidQueryParam()
    {
        HttpRequestMessage captured = null;
        var handler = new FakeHandler(req =>
        {
            captured = req;
            return FakeHandler.Json(HttpStatusCode.OK, """{"data":250}""");
        });

        var client = FakeHandler.Client(handler);
        var nextId = await CloneHelper.GetNextVmIdAsync(client, preferredVmId: 250);

        Assert.Equal(250, nextId);
        Assert.NotNull(captured);
        Assert.Contains("vmid=250", captured.RequestUri!.Query);
    }

    [Fact]
    public async Task GetNextVmIdAsync_ApiError_ThrowsPveResultException()
    {
        var handler = new FakeHandler(_ =>
            FakeHandler.Json(HttpStatusCode.BadRequest, """{"errors":{"vmid":"already exists"}}"""));

        var client = FakeHandler.Client(handler);

        await Assert.ThrowsAsync<PveResultException>(() => CloneHelper.GetNextVmIdAsync(client, preferredVmId: 100));
    }

    [Fact]
    public async Task CloneAsync_Qemu_AutoAllocatesNextVmId_AndAwaitsTask()
    {
        HttpRequestMessage cloneRequest = null;
        string cloneBody = null;
        var taskChecked = false;

        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/cluster/nextid"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":"101"}""");
            }

            if (req.Method == HttpMethod.Post && path.EndsWith("/nodes/pve01/qemu/9000/clone"))
            {
                cloneRequest = req;
                cloneBody = req.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
                return FakeHandler.Json(HttpStatusCode.OK, $$"""{"data":"{{UpidClone}}"}""");
            }

            if (path.Contains("/tasks/"))
            {
                taskChecked = true;
                return FakeHandler.Json(HttpStatusCode.OK, TaskFinishedOkJson);
            }

            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var (newVmId, result) = await CloneHelper.CloneAsync(
            client,
            "pve01",
            VmType.Qemu,
            9000,
            name: "web-01",
            targetStorage: "local-zfs",
            fullClone: true,
            format: "qcow2",
            wait: 1,
            timeout: 5000);

        Assert.Equal(101, newVmId);
        Assert.True(result.IsSuccessStatusCode);
        Assert.True(taskChecked);
        Assert.NotNull(cloneRequest);
        Assert.NotNull(cloneBody);
        Assert.Contains("\"newid\":101", cloneBody);
        Assert.Contains("\"name\":\"web-01\"", cloneBody);
        Assert.Contains("\"storage\":\"local-zfs\"", cloneBody);
        Assert.Contains("\"full\":1", cloneBody);
        Assert.Contains("\"format\":\"qcow2\"", cloneBody);
    }

    [Fact]
    public async Task CloneAsync_Lxc_WithExplicitNewVmId_MapsNameToHostname()
    {
        var nextIdCalled = false;
        string cloneBody = null;

        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/cluster/nextid"))
            {
                nextIdCalled = true;
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":"999"}""");
            }

            if (req.Method == HttpMethod.Post && path.EndsWith("/nodes/pve01/lxc/200/clone"))
            {
                cloneBody = req.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
                return FakeHandler.Json(HttpStatusCode.OK, $$"""{"data":"{{UpidClone}}"}""");
            }

            if (path.Contains("/tasks/"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, TaskFinishedOkJson);
            }

            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var (newVmId, result) = await CloneHelper.CloneAsync(
            client,
            "pve01",
            VmType.Lxc,
            200,
            newVmId: 205,
            name: "ct-app",
            fullClone: false,
            wait: 1,
            timeout: 5000);

        Assert.False(nextIdCalled);
        Assert.Equal(205, newVmId);
        Assert.True(result.IsSuccessStatusCode);
        Assert.NotNull(cloneBody);
        Assert.Contains("\"newid\":205", cloneBody);
        Assert.Contains("\"hostname\":\"ct-app\"", cloneBody);
        Assert.Contains("\"full\":0", cloneBody);
    }

    [Fact]
    public async Task CloneAsync_Lxc_WithFormat_ThrowsArgumentException()
    {
        var client = FakeHandler.Client(new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK, "{}")));

        await Assert.ThrowsAsync<ArgumentException>(() => CloneHelper.CloneAsync(
            client,
            "pve01",
            VmType.Lxc,
            200,
            newVmId: 205,
            format: "qcow2"));
    }

    [Fact]
    public async Task CloneAsync_ApiFailure_ReturnsFailureWithoutPollingTask()
    {
        var taskChecked = false;
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/nodes/pve01/qemu/9000/clone"))
            {
                return FakeHandler.Json(HttpStatusCode.InternalServerError, """{"data":null}""");
            }

            if (path.Contains("/tasks/"))
            {
                taskChecked = true;
            }

            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var (newVmId, result) = await CloneHelper.CloneAsync(
            client,
            "pve01",
            VmType.Qemu,
            9000,
            newVmId: 102,
            wait: 1,
            timeout: 5000);

        Assert.Equal(102, newVmId);
        Assert.False(result.IsSuccessStatusCode);
        Assert.False(taskChecked);
    }

    [Fact]
    public async Task CloneAsync_TaskNonOkExitStatus_ThrowsPveResultException()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/nodes/pve01/qemu/9000/clone"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, $$"""{"data":"{{UpidClone}}"}""");
            }

            if (path.Contains("/tasks/"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":{"status":"stopped","exitstatus":"storage 'fast-nvme' is not available"}}""");
            }

            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);

        await Assert.ThrowsAsync<PveResultException>(() => CloneHelper.CloneAsync(
            client,
            "pve01",
            VmType.Qemu,
            9000,
            newVmId: 102,
            wait: 1,
            timeout: 5000));
    }

    [Fact]
    public async Task CloneAsync_TaskTimeout_ThrowsTimeoutException()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (path.EndsWith("/nodes/pve01/qemu/9000/clone"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, $$"""{"data":"{{UpidClone}}"}""");
            }

            if (path.Contains("/tasks/"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":{"status":"running"}}""");
            }

            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);

        await Assert.ThrowsAsync<TimeoutException>(() => CloneHelper.CloneAsync(
            client,
            "pve01",
            VmType.Qemu,
            9000,
            newVmId: 102,
            wait: 10,
            timeout: 30));
    }

    [Fact]
    public async Task ConvertToTemplateAsync_Qemu_WithTaskUpid_AwaitsAndVerifiesTask()
    {
        var taskChecked = false;
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (req.Method == HttpMethod.Post && path.EndsWith("/nodes/pve01/qemu/100/template"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, $$"""{"data":"{{UpidTemplate}}"}""");
            }

            if (path.Contains("/tasks/"))
            {
                taskChecked = true;
                return FakeHandler.Json(HttpStatusCode.OK, TaskFinishedOkJson);
            }

            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var result = await CloneHelper.ConvertToTemplateAsync(client, "pve01", VmType.Qemu, 100, wait: 1, timeout: 5000);

        Assert.True(result.IsSuccessStatusCode);
        Assert.True(taskChecked);
    }

    [Fact]
    public async Task ConvertToTemplateAsync_Lxc_WithNullData_CompletesWithoutPollingTask()
    {
        var taskChecked = false;
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (req.Method == HttpMethod.Post && path.EndsWith("/nodes/pve01/lxc/200/template"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":null}""");
            }

            if (path.Contains("/tasks/"))
            {
                taskChecked = true;
            }

            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var result = await CloneHelper.ConvertToTemplateAsync(client, "pve01", VmType.Lxc, 200, wait: 1, timeout: 5000);

        Assert.True(result.IsSuccessStatusCode);
        Assert.False(taskChecked);
    }

    [Fact]
    public async Task ConvertToTemplateAsync_Lxc_WithDisk_ThrowsArgumentException()
    {
        var client = FakeHandler.Client(new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK, "{}")));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            CloneHelper.ConvertToTemplateAsync(client, "pve01", VmType.Lxc, 200, disk: "rootfs"));
    }

    [Fact]
    public async Task CloneAsync_ZeroTimeout_ReturnsImmediatelyWithoutPollingTask()
    {
        var taskChecked = false;
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (req.Method == HttpMethod.Post && path.EndsWith("/nodes/pve01/qemu/9000/clone"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, $$"""{"data":"{{UpidClone}}"}""");
            }

            if (path.Contains("/tasks/"))
            {
                taskChecked = true;
            }

            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var (newVmId, result) = await CloneHelper.CloneAsync(
            client,
            "pve01",
            VmType.Qemu,
            9000,
            newVmId: 103,
            timeout: 0);

        Assert.Equal(103, newVmId);
        Assert.True(result.IsSuccessStatusCode);
        Assert.False(taskChecked);
    }

    [Fact]
    public async Task CloneAsync_TaskWarningsExitStatus_Succeeds()
    {
        var handler = new FakeHandler(req =>
        {
            var path = req.RequestUri!.AbsolutePath;
            if (req.Method == HttpMethod.Post && path.EndsWith("/nodes/pve01/qemu/9000/clone"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, $$"""{"data":"{{UpidClone}}"}""");
            }

            if (path.Contains("/tasks/"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":{"status":"stopped","exitstatus":"WARNINGS: 1"}}""");
            }

            return FakeHandler.Json(HttpStatusCode.NotFound, "{}");
        });

        var client = FakeHandler.Client(handler);
        var (newVmId, result) = await CloneHelper.CloneAsync(
            client,
            "pve01",
            VmType.Qemu,
            9000,
            newVmId: 104,
            wait: 1,
            timeout: 5000);

        Assert.Equal(104, newVmId);
        Assert.True(result.IsSuccessStatusCode);
    }
}
