/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Net;
using Corsinvest.ProxmoxVE.Api.Extension.Utils;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests;

/// <summary>
/// The helpers that find a guest and act on it without knowing its node and type.
/// </summary>
public class ClientExtensionTests
{
    private const string Upid = "UPID:pve01:0012A3F4:05C1B2D3:6720F1A0:qmstart:100:root@pam:";

    private const string Resources = """
        {"data":[
            {"id":"node/pve02","type":"node","node":"pve02","status":"online","maxcpu":8,"cpu":0.1,"maxmem":1000,"mem":500},
            {"id":"node/pve01","type":"node","node":"pve01","status":"online","maxcpu":8,"cpu":0.5,"maxmem":1000,"mem":250},
            {"id":"lxc/200","type":"lxc","vmid":200,"name":"ct01","node":"pve02","status":"running"},
            {"id":"qemu/101","type":"qemu","vmid":101,"name":"db01","node":"pve02","status":"stopped","tags":"production;db"},
            {"id":"qemu/100","type":"qemu","vmid":100,"name":"Web01","node":"pve01","status":"running","tags":"production"},
            {"id":"storage/pve01/local","type":"storage","storage":"local","node":"pve01","status":"available"}
        ]}
        """;

    private readonly FakeHandler _handler;
    private readonly PveClient _client;

    public ClientExtensionTests()
    {
        _handler = new FakeHandler(request =>
        {
            var path = request.RequestUri!.AbsolutePath;
            if (path.EndsWith("/cluster/resources"))
            {
                var type = System.Web.HttpUtility.ParseQueryString(request.RequestUri.Query)["type"];
                var all = JObject.Parse(Resources);
                var wanted = type switch
                {
                    null => all["data"]!,
                    "vm" => new JArray(all["data"]!.Where(a => (string)a["type"]! is "qemu" or "lxc")),
                    _ => new JArray(all["data"]!.Where(a => (string)a["type"]! == type)),
                };
                return FakeHandler.Json(HttpStatusCode.OK, new JObject { ["data"] = wanted }.ToString());
            }
            if (request.Method == HttpMethod.Get && path.EndsWith("/snapshot"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":[{"name":"current","description":"You are here!"},{"name":"b","snaptime":200,"description":"second"},{"name":"a","snaptime":100,"description":"first"}]}""");
            }
            if (path.Contains("/tasks/"))
            {
                return FakeHandler.Json(HttpStatusCode.OK, """{"data":{"status":"stopped","exitstatus":"OK"}}""");
            }
            return request.Method == HttpMethod.Get
                ? FakeHandler.Json(HttpStatusCode.OK, """{"data":{}}""")
                : FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID"}""".Replace("UPID", Upid));
        });
        _client = FakeHandler.Client(_handler);
    }

    private HttpRequestMessage Last(Func<HttpRequestMessage, bool> filter) => _handler.Requests.Last(filter);
    private static string PathOf(HttpRequestMessage request) => Uri.UnescapeDataString(request.RequestUri!.AbsolutePath);

    [Fact]
    public async Task Guests_of_the_cluster_are_sorted_by_node_and_id()
    {
        var vms = (await _client.GetVmsAsync()).ToList();

        Assert.Equal([100L, 101L, 200L], vms.Select(a => a.VmId));
        Assert.Equal(["pve01", "pve02", "pve02"], vms.Select(a => a.Node));
        Assert.Equal([VmType.Qemu, VmType.Qemu, VmType.Lxc], vms.Select(a => a.VmType));
        Assert.True(vms[0].IsRunning);
        Assert.True(vms[1].IsStopped);
    }

    [Theory]
    [InlineData("100", 100)]
    [InlineData("web01", 100)]
    [InlineData("WEB01", 100)]
    [InlineData("ct01", 200)]
    public async Task Guest_is_found_by_id_or_name(string idOrName, long expected)
        => Assert.Equal(expected, (await _client.GetVmAsync(idOrName)).VmId);

    [Fact]
    public async Task Guest_is_found_by_numeric_id()
        => Assert.Equal("db01", (await _client.GetVmAsync(101)).Name);

    [Theory]
    [InlineData("999")]
    [InlineData("nothing")]
    public async Task Guest_that_does_not_exist_is_reported(string idOrName)
    {
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _client.GetVmAsync(idOrName));

        Assert.Contains(idOrName, ex.Message);
    }

    [Fact]
    public async Task Guests_are_selected_by_pattern()
    {
        Assert.Equal([100L, 101L], (await _client.GetVmsAsync("@tag-production")).Select(a => a.VmId));
        Assert.Equal([101L, 200L], (await _client.GetVmsAsync("@node-pve02")).Select(a => a.VmId));
        Assert.Equal([100L, 200L], (await _client.GetVmsAsync("@all,-@tag-db")).Select(a => a.VmId));
        Assert.Empty(await _client.GetVmsAsync("nothing"));
    }

    [Fact]
    public async Task Nodes_and_storages_are_read_from_the_resources()
    {
        Assert.Equal(["pve02", "pve01"], (await _client.GetNodesAsync()).Select(a => a.Node));
        Assert.Equal("pve01", (await _client.GetNodeAsync("pve01")).Node);
        Assert.Equal("local", (await _client.GetStorageAsync("local")).Storage);
        Assert.Single(await _client.GetStoragesAsync());

        await Assert.ThrowsAsync<ArgumentException>(() => _client.GetNodeAsync("pve09"));
        await Assert.ThrowsAsync<ArgumentException>(() => _client.GetStorageAsync("nothing"));
    }

    [Fact]
    public async Task Resources_of_one_kind_are_asked_with_its_filter()
    {
        await _client.GetResourcesAsync(ClusterResourceType.Node);
        Assert.Equal("?type=node", _handler.Requests.Last().RequestUri!.Query);

        await _client.GetResourcesAsync(ClusterResourceType.All);
        Assert.Equal(string.Empty, _handler.Requests.Last().RequestUri!.Query);
    }

    [Theory]
    [InlineData(100, VmStatus.Start, "/api2/json/nodes/pve01/qemu/100/status/start")]
    [InlineData(100, VmStatus.Shutdown, "/api2/json/nodes/pve01/qemu/100/status/shutdown")]
    [InlineData(100, VmStatus.Reset, "/api2/json/nodes/pve01/qemu/100/status/reset")]
    [InlineData(200, VmStatus.Stop, "/api2/json/nodes/pve02/lxc/200/status/stop")]
    [InlineData(200, VmStatus.Reboot, "/api2/json/nodes/pve02/lxc/200/status/reboot")]
    [InlineData(200, VmStatus.Suspend, "/api2/json/nodes/pve02/lxc/200/status/suspend")]
    [InlineData(200, VmStatus.Resume, "/api2/json/nodes/pve02/lxc/200/status/resume")]
    public async Task State_of_a_guest_is_changed_on_its_node_and_type(long vmId, VmStatus status, string expectedPath)
    {
        var result = await _client.ChangeStatusVmAsync(vmId, status);

        var request = Last(a => a.Method == HttpMethod.Post);
        Assert.Equal(expectedPath, PathOf(request));
        Assert.Equal(Upid, result.ToData<string>());
    }

    [Fact]
    public async Task State_can_be_given_as_text_without_regard_to_case()
    {
        await _client.ChangeStatusVmAsync(100, "shutdown");

        Assert.EndsWith("/qemu/100/status/shutdown", PathOf(Last(a => a.Method == HttpMethod.Post)));
    }

    [Fact]
    public async Task Reset_of_a_container_is_refused()
        => await Assert.ThrowsAnyAsync<ArgumentException>(() => _client.ChangeStatusVmAsync(200, VmStatus.Reset));

    [Fact]
    public async Task Status_and_configuration_are_read_from_the_type_of_the_guest()
    {
        await _client.GetVmStatusAsync("pve02", VmType.Lxc, 200);
        Assert.Equal("/api2/json/nodes/pve02/lxc/200/status/current", PathOf(_handler.Requests.Last()));

        await _client.GetVmConfigAsync("pve01", VmType.Qemu, 100);
        Assert.Equal("/api2/json/nodes/pve01/qemu/100/config", PathOf(_handler.Requests.Last()));
    }

    [Fact]
    public async Task Unlock_removes_the_lock_of_the_configuration()
    {
        await _client.VmUnlockAsync("pve02", VmType.Lxc, 200);

        var request = _handler.Requests.Last();
        Assert.Equal(HttpMethod.Put, request.Method);
        Assert.Equal("/api2/json/nodes/pve02/lxc/200/config", PathOf(request));
        Assert.Equal("lock", (string)JObject.Parse(await request.Content!.ReadAsStringAsync())["delete"]!);
    }

    [Theory]
    [InlineData(VmType.Qemu, 100, "pve01", "/api2/json/nodes/pve01/qemu/100/snapshot")]
    [InlineData(VmType.Lxc, 200, "pve02", "/api2/json/nodes/pve02/lxc/200/snapshot")]
    public async Task Snapshot_is_created_on_the_type_of_the_guest_and_its_task_is_waited(VmType type, long vmId, string node, string expectedPath)
    {
        var result = await SnapshotHelper.CreateSnapshotAsync(_client, node, type, vmId, "before-update", "Before the update", false, 2000);

        Assert.True(result.IsSuccessStatusCode);
        var create = Last(a => a.Method == HttpMethod.Post);
        Assert.Equal(expectedPath, PathOf(create));
        Assert.Equal("before-update", (string)JObject.Parse(await create.Content!.ReadAsStringAsync())["snapname"]!);
        Assert.EndsWith($"/tasks/{Upid}/status", PathOf(_handler.Requests.Last()));
    }

    [Fact]
    public async Task Snapshots_are_listed_from_the_oldest()
    {
        var names = (await SnapshotHelper.GetSnapshotsAsync(_client, "pve01", VmType.Qemu, 100)).Select(a => a.Name).ToList();

        Assert.True(names.IndexOf("a") < names.IndexOf("b"), string.Join(",", names));
    }

    [Fact]
    public async Task Snapshot_is_removed_rolled_back_and_updated_on_its_path()
    {
        await SnapshotHelper.RemoveSnapshotAsync(_client, "pve01", VmType.Qemu, 100, "old", 2000, force: true);
        var remove = Last(a => a.Method == HttpMethod.Delete);
        Assert.Equal("/api2/json/nodes/pve01/qemu/100/snapshot/old", PathOf(remove));
        Assert.Equal("?force=1", remove.RequestUri!.Query);

        await SnapshotHelper.RollbackSnapshotAsync(_client, "pve02", VmType.Lxc, 200, "daily", 2000);
        Assert.Equal("/api2/json/nodes/pve02/lxc/200/snapshot/daily/rollback", PathOf(Last(a => a.Method == HttpMethod.Post)));

        await SnapshotHelper.UpdateSnapshotAsync(_client, "pve01", VmType.Qemu, 100, "old", "new description");
        var update = Last(a => a.Method == HttpMethod.Put);
        Assert.Equal("/api2/json/nodes/pve01/qemu/100/snapshot/old/config", PathOf(update));
        Assert.Equal("new description", (string)JObject.Parse(await update.Content!.ReadAsStringAsync())["description"]!);
    }

    [Fact]
    public async Task Cluster_name_is_read_from_the_status()
    {
        var cluster = FakeHandler.Client(new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK,
            """{"data":[{"type":"cluster","name":"prod","nodes":2,"quorate":1},{"type":"node","name":"pve01","online":1,"ip":"10.0.0.1"},{"type":"node","name":"pve02","online":1,"ip":"10.0.0.2"}]}""")));
        var single = FakeHandler.Client(new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK,
            """{"data":[{"type":"node","name":"pve01","online":1,"ip":"10.0.0.1"}]}""")));

        Assert.Equal((Corsinvest.ProxmoxVE.Api.Shared.Models.Common.ClusterType.Cluster, "prod"), await cluster.GetClusterInfoAsync());
        Assert.Equal((Corsinvest.ProxmoxVE.Api.Shared.Models.Common.ClusterType.SingleNode, "pve01"), await single.GetClusterInfoAsync());
        Assert.Equal("10.0.0.2", (await cluster.GetHostAndIpAsync())["pve02"]);
    }
}
