/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Net;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Api;

/// <summary>
/// A sample of the generated client: the path of the API is the path in the code, the HTTP method is
/// the one of the endpoint, the parameters keep the names of the API.
/// </summary>
public class GeneratedClientTests
{
    private readonly FakeHandler _handler = new(_ => FakeHandler.Json(HttpStatusCode.OK, """{"data":null}"""));
    private readonly PveClient _client;

    public GeneratedClientTests() => _client = FakeHandler.Client(_handler);

    private HttpRequestMessage Request => _handler.Requests.Single();
    private string Path => Request.RequestUri!.AbsolutePath;
    private System.Collections.Specialized.NameValueCollection Query => System.Web.HttpUtility.ParseQueryString(Request.RequestUri!.Query);
    private async Task<JObject> BodyAsync() => JObject.Parse(await Request.Content!.ReadAsStringAsync());

    [Fact]
    public async Task Version()
    {
        await _client.Version.Version();

        Assert.Equal(HttpMethod.Get, Request.Method);
        Assert.Equal("/api2/json/version", Path);
    }

    [Fact]
    public async Task Cluster_resources_with_a_filter()
    {
        await _client.Cluster.Resources.Resources(type: "vm");

        Assert.Equal(HttpMethod.Get, Request.Method);
        Assert.Equal("/api2/json/cluster/resources", Path);
        Assert.Equal("vm", Query["type"]);
    }

    [Fact]
    public async Task Indexers_are_the_values_of_the_path()
    {
        await _client.Nodes["pve01"].Qemu[100].Config.VmConfig(current: true);

        Assert.Equal(HttpMethod.Get, Request.Method);
        Assert.Equal("/api2/json/nodes/pve01/qemu/100/config", Path);
        Assert.Equal("1", Query["current"]);
    }

    [Fact]
    public async Task Containers_are_under_lxc()
    {
        await _client.Nodes["pve02"].Lxc[200].Status.Current.VmStatus();

        Assert.Equal("/api2/json/nodes/pve02/lxc/200/status/current", Path);
    }

    [Fact]
    public async Task Start_is_a_post_and_a_dash_in_a_parameter_name_is_kept()
    {
        await _client.Nodes["pve01"].Qemu[100].Status.Start.VmStart(force_cpu: "host", timeout: 30);

        Assert.Equal(HttpMethod.Post, Request.Method);
        Assert.Equal("/api2/json/nodes/pve01/qemu/100/status/start", Path);
        var body = await BodyAsync();
        Assert.Equal("host", (string)body["force-cpu"]!);
        Assert.Equal(30, (int)body["timeout"]!);
        Assert.Equal(2, body.Count);
    }

    [Fact]
    public async Task Update_of_the_configuration_is_a_put_with_indexed_parameters()
    {
        await _client.Nodes["pve01"].Qemu[100].Config.UpdateVm(cores: 4,
                                                               onboot: true,
                                                               netN: new Dictionary<int, string> { [0] = "virtio,bridge=vmbr0", [1] = "virtio,bridge=vmbr1" },
                                                               scsiN: new Dictionary<int, string> { [2] = "local-lvm:32" });

        Assert.Equal(HttpMethod.Put, Request.Method);
        Assert.Equal("/api2/json/nodes/pve01/qemu/100/config", Path);
        var body = await BodyAsync();
        Assert.Equal(4, (int)body["cores"]!);
        Assert.Equal(1, (int)body["onboot"]!);
        Assert.Equal("virtio,bridge=vmbr0", (string)body["net0"]!);
        Assert.Equal("virtio,bridge=vmbr1", (string)body["net1"]!);
        Assert.Equal("local-lvm:32", (string)body["scsi2"]!);
        Assert.Equal(5, body.Count);
    }

    [Fact]
    public async Task Update_of_the_configuration_as_a_task_is_a_post()
    {
        await _client.Nodes["pve01"].Qemu[100].Config.UpdateVmAsync(memory: "4096");

        Assert.Equal(HttpMethod.Post, Request.Method);
        Assert.Equal("/api2/json/nodes/pve01/qemu/100/config", Path);
    }

    [Fact]
    public async Task Snapshot_takes_its_required_parameter_first()
    {
        await _client.Nodes["pve01"].Qemu[100].Snapshot.Snapshot("before-update", description: "Before the update", vmstate: false);

        Assert.Equal(HttpMethod.Post, Request.Method);
        Assert.Equal("/api2/json/nodes/pve01/qemu/100/snapshot", Path);
        var body = await BodyAsync();
        Assert.Equal("before-update", (string)body["snapname"]!);
        Assert.Equal("Before the update", (string)body["description"]!);
        Assert.Equal(0, (int)body["vmstate"]!);
    }

    [Fact]
    public async Task Delete_of_a_snapshot_sends_its_parameter_in_the_query_string()
    {
        await _client.Nodes["pve01"].Qemu[100].Snapshot["before-update"].Delsnapshot(force: true);

        Assert.Equal(HttpMethod.Delete, Request.Method);
        Assert.Equal("/api2/json/nodes/pve01/qemu/100/snapshot/before-update", Path);
        Assert.Equal("1", Query["force"]);
    }

    [Fact]
    public async Task Rollback_of_a_snapshot()
    {
        await _client.Nodes["pve01"].Lxc[200].Snapshot["daily"].Rollback.Rollback();

        Assert.Equal(HttpMethod.Post, Request.Method);
        Assert.Equal("/api2/json/nodes/pve01/lxc/200/snapshot/daily/rollback", Path);
    }

    [Fact]
    public async Task Create_of_a_vm_sends_only_what_is_given()
    {
        await _client.Nodes["pve01"].Qemu.CreateVm(vmid: 105, name: "web02", memory: "2048");

        Assert.Equal(HttpMethod.Post, Request.Method);
        Assert.Equal("/api2/json/nodes/pve01/qemu", Path);
        var body = await BodyAsync();
        Assert.Equal(["memory", "name", "vmid"], body.Properties().Select(a => a.Name).Order());
    }

    [Fact]
    public async Task Task_status_is_under_the_node_of_the_task()
    {
        const string upid = "UPID:pve01:0012A3F4:05C1B2D3:6720F1A0:qmstart:100:root@pam:";

        await _client.Nodes["pve01"].Tasks[upid].Status.ReadTaskStatus();

        Assert.Equal($"/api2/json/nodes/pve01/tasks/{upid}/status", Uri.UnescapeDataString(Path));
    }

    [Fact]
    public async Task Storage_content_with_a_filter()
    {
        await _client.Nodes["pve01"].Storage["local"].Content.Index(content: "backup", vmid: 100);

        Assert.Equal("/api2/json/nodes/pve01/storage/local/content", Path);
        Assert.Equal("backup", Query["content"]);
        Assert.Equal("100", Query["vmid"]);
    }

    [Fact]
    public void A_node_of_the_tree_makes_no_call_until_a_method_is_called()
    {
        _ = _client.Nodes["pve01"].Qemu[100].Config;

        Assert.Empty(_handler.Requests);
    }
}
