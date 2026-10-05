/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.ComponentModel;
using System.Net;
using Corsinvest.ProxmoxVE.Api.Extension.Utils;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Newtonsoft.Json;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests;

/// <summary>
/// The helpers that act on a guest of a known node and type: every one has a branch for VM and one for container.
/// </summary>
public class GuestHelpersTests
{
    private const string Upid = "UPID:pve01:0012A3F4:05C1B2D3:6720F1A0:qmsnapshot:100:root@pam:";

    private readonly FakeHandler _handler;
    private readonly PveClient _client;
    private readonly Dictionary<string, string> _answers = [];
    private readonly HashSet<string> _refused = [];

    public GuestHelpersTests()
    {
        _handler = new FakeHandler(request =>
        {
            var path = Uri.UnescapeDataString(request.RequestUri!.AbsolutePath);
            if (_refused.Any(path.EndsWith)) { return FakeHandler.Json(HttpStatusCode.InternalServerError, """{"data":null}"""); }
            foreach (var (pathEnd, json) in _answers)
            {
                if (path.EndsWith(pathEnd)) { return FakeHandler.Json(HttpStatusCode.OK, json); }
            }
            if (path.Contains("/tasks/")) { return FakeHandler.Json(HttpStatusCode.OK, """{"data":{"status":"stopped","exitstatus":"OK"}}"""); }
            return request.Method == HttpMethod.Get
                ? FakeHandler.Json(HttpStatusCode.OK, """{"data":{}}""")
                : FakeHandler.Json(HttpStatusCode.OK, "{\"data\":\"" + Upid + "\"}");
        });
        _client = FakeHandler.Client(_handler);
    }

    private static string PathOf(HttpRequestMessage request) => Uri.UnescapeDataString(request.RequestUri!.AbsolutePath);
    private static string Body(HttpRequestMessage request) => request.Content!.ReadAsStringAsync().Result;

    // the call that is not a read of the status of a task
    private HttpRequestMessage Call => _handler.Requests.Last(a => !PathOf(a).Contains("/tasks/"));

    [Theory]
    [InlineData(VmType.Qemu, "/api2/json/nodes/pve01/qemu/100/snapshot")]
    [InlineData(VmType.Lxc, "/api2/json/nodes/pve01/lxc/100/snapshot")]
    public async Task Snapshots_are_listed_from_the_oldest(VmType vmType, string path)
    {
        _answers["/snapshot"] = """{"data":[{"name":"current","description":"You are here!"},{"name":"b","snaptime":200},{"name":"a","snaptime":100}]}""";

        var snapshots = await SnapshotHelper.GetSnapshotsAsync(_client, "pve01", vmType, 100);

        Assert.Equal(path, PathOf(Call));
        Assert.Equal(["a", "b"], snapshots.Where(a => a.Name != "current").Select(a => a.Name));
    }

    [Fact]
    public async Task Snapshot_of_a_vm_can_keep_the_memory_and_the_one_of_a_container_cannot()
    {
        var result = await SnapshotHelper.CreateSnapshotAsync(_client, "pve01", VmType.Qemu, 100, "before", "Before update", true, 5000);
        Assert.True(result.IsSuccessStatusCode);
        Assert.Equal("/api2/json/nodes/pve01/qemu/100/snapshot", PathOf(Call));
        Assert.Equal(HttpMethod.Post, Call.Method);
        Assert.Contains("\"snapname\":\"before\"", Body(Call));
        Assert.Contains("\"description\":\"Before update\"", Body(Call));
        Assert.Contains("\"vmstate\":1", Body(Call));

        await SnapshotHelper.CreateSnapshotAsync(_client, "pve01", VmType.Lxc, 200, "before", "Before update", true, 5000);
        Assert.Equal("/api2/json/nodes/pve01/lxc/200/snapshot", PathOf(Call));
        Assert.DoesNotContain("vmstate", Body(Call));

        // the task of the snapshot is waited
        Assert.Contains(_handler.Requests, a => PathOf(a) == $"/api2/json/nodes/pve01/tasks/{Upid}/status");
    }

    [Theory]
    [InlineData(VmType.Qemu, "qemu")]
    [InlineData(VmType.Lxc, "lxc")]
    public async Task Snapshot_is_removed_read_changed_and_restored(VmType vmType, string type)
    {
        var root = $"/api2/json/nodes/pve01/{type}/100/snapshot/before";

        Assert.True((await SnapshotHelper.RemoveSnapshotAsync(_client, "pve01", vmType, 100, "before", 5000, true)).IsSuccessStatusCode);
        Assert.Equal((HttpMethod.Delete, root), (Call.Method, PathOf(Call)));
        Assert.Equal("?force=1", Call.RequestUri!.Query);

        await SnapshotHelper.RemoveSnapshotAsync(_client, "pve01", vmType, 100, "before", 5000);
        Assert.Equal(string.Empty, Call.RequestUri!.Query);

        await SnapshotHelper.GetConfigSnapshotAsync(_client, "pve01", vmType, 100, "before");
        Assert.Equal((HttpMethod.Get, root + "/config"), (Call.Method, PathOf(Call)));

        await SnapshotHelper.UpdateSnapshotAsync(_client, "pve01", vmType, 100, "before", "new text");
        Assert.Equal((HttpMethod.Put, root + "/config"), (Call.Method, PathOf(Call)));
        Assert.Contains("\"description\":\"new text\"", Body(Call));

        Assert.True((await SnapshotHelper.RollbackSnapshotAsync(_client, "pve01", vmType, 100, "before", 5000)).IsSuccessStatusCode);
        Assert.Equal((HttpMethod.Post, root + "/rollback"), (Call.Method, PathOf(Call)));
    }

    [Fact]
    public async Task Type_that_is_not_a_guest_is_refused()
    {
        const VmType other = (VmType)99;

        await Assert.ThrowsAsync<InvalidEnumArgumentException>(() => SnapshotHelper.GetSnapshotsAsync(_client, "pve01", other, 100));
        await Assert.ThrowsAsync<InvalidEnumArgumentException>(() => SnapshotHelper.CreateSnapshotAsync(_client, "pve01", other, 100, "a", "b", false, 1000));
        await Assert.ThrowsAsync<InvalidEnumArgumentException>(() => SnapshotHelper.RemoveSnapshotAsync(_client, "pve01", other, 100, "a", 1000));
        await Assert.ThrowsAsync<InvalidEnumArgumentException>(() => SnapshotHelper.GetConfigSnapshotAsync(_client, "pve01", other, 100, "a"));
        await Assert.ThrowsAsync<InvalidEnumArgumentException>(() => SnapshotHelper.UpdateSnapshotAsync(_client, "pve01", other, 100, "a", "b"));
        await Assert.ThrowsAsync<InvalidEnumArgumentException>(() => SnapshotHelper.RollbackSnapshotAsync(_client, "pve01", other, 100, "a", 1000));
        await Assert.ThrowsAsync<InvalidEnumArgumentException>(() => VmHelper.ChangeStatusVmAsync(_client, "pve01", other, 100, VmStatus.Start));
        await Assert.ThrowsAsync<InvalidEnumArgumentException>(() => VmHelper.ChangeStatusVmAsync(_client, "pve01", VmType.Qemu, 100, (VmStatus)99));
        await Assert.ThrowsAsync<InvalidEnumArgumentException>(() => VmHelper.ChangeStatusVmAsync(_client, "pve01", VmType.Lxc, 100, (VmStatus)99));
        await Assert.ThrowsAsync<InvalidEnumArgumentException>(() => _client.GetVmStatusAsync("pve01", other, 100));
        await Assert.ThrowsAsync<InvalidEnumArgumentException>(() => _client.GetVmConfigAsync("pve01", other, 100));
        await Assert.ThrowsAsync<InvalidEnumArgumentException>(() => _client.VmUnlockAsync("pve01", other, 100));
        await Assert.ThrowsAsync<InvalidEnumArgumentException>(
            () => _client.GetVmRrdDataAsync("pve01", other, 100, RrdDataTimeFrame.Day, RrdDataConsolidation.Average));
        await Assert.ThrowsAsync<InvalidEnumArgumentException>(() => RemoteViewerHelper.PrepareSpiceAsync(_client, "pve01", other, 100));
        await Assert.ThrowsAsync<InvalidEnumArgumentException>(() => RemoteViewerHelper.PrepareVncAsync(_client, "pve01", other, 100));
        Assert.Empty(_handler.Requests);
    }

    [Theory]
    [InlineData(VmStatus.Start, "start")]
    [InlineData(VmStatus.Stop, "stop")]
    [InlineData(VmStatus.Shutdown, "shutdown")]
    [InlineData(VmStatus.Reboot, "reboot")]
    [InlineData(VmStatus.Suspend, "suspend")]
    [InlineData(VmStatus.Resume, "resume")]
    [InlineData(VmStatus.Reset, "reset")]
    public async Task Status_of_a_guest_is_changed_with_the_call_of_its_type(VmStatus status, string action)
    {
        await VmHelper.ChangeStatusVmAsync(_client, "pve01", VmType.Qemu, 100, status);
        Assert.Equal((HttpMethod.Post, $"/api2/json/nodes/pve01/qemu/100/status/{action}"), (Call.Method, PathOf(Call)));

        if (status == VmStatus.Reset)
        {
            // a container has no reset
            await Assert.ThrowsAsync<InvalidEnumArgumentException>(() => VmHelper.ChangeStatusVmAsync(_client, "pve01", VmType.Lxc, 200, status));
            return;
        }

        await VmHelper.ChangeStatusVmAsync(_client, "pve01", VmType.Lxc, 200, status);
        Assert.Equal((HttpMethod.Post, $"/api2/json/nodes/pve01/lxc/200/status/{action}"), (Call.Method, PathOf(Call)));
    }

    [Fact]
    public async Task Status_config_and_unlock_use_the_calls_of_the_type()
    {
        _answers["/status/current"] = """{"data":{"status":"running","vmid":100,"name":"web01"}}""";
        _answers["/config"] = """{"data":{"ostype":"l26","memory":"2048","hostname":"ct01"}}""";

        Assert.Equal("running", (await _client.GetVmStatusAsync("pve01", VmType.Qemu, 100)).Status);
        Assert.Equal("/api2/json/nodes/pve01/qemu/100/status/current", PathOf(Call));
        Assert.Equal("running", (await _client.GetVmStatusAsync("pve01", VmType.Lxc, 200)).Status);
        Assert.Equal("/api2/json/nodes/pve01/lxc/200/status/current", PathOf(Call));

        Assert.IsType<VmConfigQemu>(await _client.GetVmConfigAsync("pve01", VmType.Qemu, 100));
        Assert.Equal("/api2/json/nodes/pve01/qemu/100/config", PathOf(Call));
        Assert.IsType<VmConfigLxc>(await _client.GetVmConfigAsync("pve01", VmType.Lxc, 200));
        Assert.Equal("/api2/json/nodes/pve01/lxc/200/config", PathOf(Call));

        await _client.VmUnlockAsync("pve01", VmType.Qemu, 100);
        Assert.Equal((HttpMethod.Put, "/api2/json/nodes/pve01/qemu/100/config"), (Call.Method, PathOf(Call)));
        Assert.Contains("\"delete\":\"lock\"", Body(Call));
        Assert.Contains("\"skiplock\":1", Body(Call));

        await _client.VmUnlockAsync("pve01", VmType.Lxc, 200);
        Assert.Equal((HttpMethod.Put, "/api2/json/nodes/pve01/lxc/200/config"), (Call.Method, PathOf(Call)));
        Assert.Contains("\"delete\":\"lock\"", Body(Call));
        Assert.DoesNotContain("skiplock", Body(Call));
    }

    [Fact]
    public async Task Guests_of_a_pool_include_the_ones_of_its_nested_pools()
    {
        _answers["/cluster/resources"] = """
            {"data":[
                {"id":"qemu/100","type":"qemu","vmid":100,"name":"web01","node":"pve01","status":"running"},
                {"id":"qemu/101","type":"qemu","vmid":101,"name":"db01","node":"pve01","status":"running"},
                {"id":"lxc/200","type":"lxc","vmid":200,"name":"ct01","node":"pve02","status":"running"},
                {"id":"qemu/300","type":"qemu","vmid":300,"name":"other","node":"pve02","status":"running"}
            ]}
            """;
        var handler = new FakeHandler(request =>
        {
            var path = request.RequestUri!.AbsolutePath;
            var poolId = System.Web.HttpUtility.ParseQueryString(request.RequestUri.Query)["poolid"];
            if (path.EndsWith("/cluster/resources")) { return FakeHandler.Json(HttpStatusCode.OK, _answers["/cluster/resources"]); }
            return FakeHandler.Json(HttpStatusCode.OK, poolId switch
            {
                null => """{"data":[{"poolid":"customer"},{"poolid":"customer/web"},{"poolid":"customers"},{"poolid":"empty"}]}""",
                "customer" => """{"data":[{"poolid":"customer","members":[{"id":"qemu/101","type":"qemu","vmid":101},{"id":"storage/pve01/local","type":"storage"}]}]}""",
                "customer/web" => """{"data":[{"poolid":"customer/web","members":[{"id":"qemu/100","type":"qemu","vmid":100},{"id":"lxc/200","type":"lxc","vmid":200}]}]}""",
                "customers" => """{"data":[{"poolid":"customers","members":[{"id":"qemu/300","type":"qemu","vmid":300}]}]}""",
                _ => """{"data":[]}""",
            });
        });
        var client = FakeHandler.Client(handler);

        Assert.Equal([100, 101, 200], (await client.GetVmsAsync("@pool-customer")).Select(a => a.VmId).Order());
        Assert.Equal([100, 200], (await client.GetVmsAsync("@pool-CUSTOMER/WEB")).Select(a => a.VmId).Order());
        Assert.Empty(await client.GetVmsAsync("@pool-empty"));
        Assert.Empty(await client.GetVmsAsync("@pool-missing"));

        // the list of the pools is read once for a selection with more pools
        handler.Requests.Clear();
        Assert.Equal([100, 101, 200, 300], (await client.GetVmsAsync("@pool-customer,@pool-customers")).Select(a => a.VmId).Order());
        Assert.Single(handler.Requests, a => a.RequestUri!.AbsolutePath.EndsWith("/pools") && a.RequestUri.Query == string.Empty);
    }

    private sealed class Guest : ClusterResource, IClusterResourceVmOsInfo
    {
        public VmQemuAgentOsInfo VmQemuAgentOsInfo { get; set; } = null!;
        public string HostName { get; set; } = null!;
        public string OsVersion { get; set; } = null!;
        public VmOsType? OsType { get; set; }

        public static Guest Of(string type, long vmId, string status)
            => JsonConvert.DeserializeObject<Guest>($$"""{"id":"{{type}}/{{vmId}}","type":"{{type}}","vmid":{{vmId}},"node":"pve01","status":"{{status}}"}""")!;
    }

    [Fact]
    public async Task System_of_a_running_vm_is_asked_to_its_agent()
    {
        _answers["/qemu/100/config"] = """{"data":{"ostype":"win11","agent":"1"}}""";
        _answers["/agent/get-osinfo"] = """{"data":{"result":{"id":"debian","name":"Debian GNU/Linux","pretty-name":"Debian GNU/Linux 12 (bookworm)","version":"12 (bookworm)","version-id":"12","kernel-release":"6.1.0-18-amd64"}}}""";
        _answers["/agent/get-host-name"] = """{"data":{"result":{"host-name":"web01.example.com"}}}""";
        var guest = Guest.Of("qemu", 100, "running");

        await VmHelper.PopulateVmOsInfoAsync(_client, guest);

        Assert.Equal(VmOsType.Windows, guest.OsType);
        Assert.Equal("web01.example.com", guest.HostName);
        Assert.Equal("Debian GNU/Linux", guest.VmQemuAgentOsInfo.Result.Name);
        Assert.Equal(guest.VmQemuAgentOsInfo.Result.OsVersion, guest.OsVersion);
        Assert.Contains("12", guest.OsVersion);
        Assert.Equal(["/api2/json/nodes/pve01/qemu/100/config", "/api2/json/nodes/pve01/qemu/100/agent/get-osinfo", "/api2/json/nodes/pve01/qemu/100/agent/get-host-name"],
                     _handler.Requests.Select(PathOf));
    }

    [Fact]
    public async Task Vm_without_an_answer_of_the_agent_says_so_in_the_host_name()
    {
        _answers["/qemu/100/config"] = """{"data":{"ostype":"l26","agent":"enabled=1,fstrim_cloned_disks=1"}}""";
        _refused.Add("/agent/get-osinfo");
        var guest = Guest.Of("qemu", 100, "running");

        await VmHelper.PopulateVmOsInfoAsync(_client, guest);

        Assert.Equal("Error Agent data!", guest.HostName);
        Assert.Equal(VmOsType.Linux, guest.OsType);
    }

    [Theory]
    [InlineData("""{"data":{"ostype":"l26"}}""")]
    [InlineData("""{"data":{"ostype":"l26","agent":"0"}}""")]
    public async Task Vm_without_the_agent_says_so_in_the_host_name(string config)
    {
        _answers["/qemu/100/config"] = config;
        var guest = Guest.Of("qemu", 100, "running");

        await VmHelper.PopulateVmOsInfoAsync(_client, guest);

        Assert.Equal("Agent not enabled!", guest.HostName);
        Assert.Single(_handler.Requests);
    }

    [Fact]
    public async Task System_of_a_stopped_vm_is_the_one_of_its_config()
    {
        _answers["/qemu/100/config"] = """{"data":{"ostype":"win11","agent":"1"}}""";
        var guest = Guest.Of("qemu", 100, "stopped");

        await VmHelper.PopulateVmOsInfoAsync(_client, guest);

        Assert.Equal(VmOsType.Windows, guest.OsType);
        Assert.Contains("Windows", guest.OsVersion);
        Assert.Null(guest.HostName);
        Assert.Single(_handler.Requests);
    }

    [Fact]
    public async Task System_and_host_name_of_a_container_are_the_ones_of_its_config()
    {
        _answers["/lxc/200/config"] = """{"data":{"ostype":"debian","hostname":"ct01","arch":"amd64"}}""";
        var guest = Guest.Of("lxc", 200, "running");

        await VmHelper.PopulateVmOsInfoAsync(_client, guest);

        Assert.Equal("ct01", guest.HostName);
        Assert.Equal(VmOsType.Linux, guest.OsType);
        Assert.False(string.IsNullOrEmpty(guest.OsVersion));
        var request = _handler.Requests.Single();
        Assert.Equal("/api2/json/nodes/pve01/lxc/200/config", PathOf(request));
        Assert.Equal("?current=1", request.RequestUri!.Query);
    }
}
