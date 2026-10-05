/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Net;
using System.Text;
using Corsinvest.ProxmoxVE.Api.Extension.Utils;
using Corsinvest.ProxmoxVE.Api.Shared;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Common;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests;

/// <summary>
/// The typed reads written by hand and the helpers of the client that read more than one endpoint.
/// </summary>
public class ModelsExtensionsTests
{
    // answers by the end of the path; anything else is an empty object
    private static (PveClient Client, FakeHandler Handler) ClientAnswering(params (string PathEnd, string Json)[] answers)
    {
        var handler = new FakeHandler(request =>
        {
            var path = Uri.UnescapeDataString(request.RequestUri!.AbsolutePath);
            foreach (var (pathEnd, json) in answers)
            {
                if (path.EndsWith(pathEnd)) { return FakeHandler.Json(HttpStatusCode.OK, json); }
            }
            return FakeHandler.Json(HttpStatusCode.OK, """{"data":{}}""");
        });
        return (FakeHandler.Client(handler), handler);
    }

    private static string PathOf(HttpRequestMessage request) => Uri.UnescapeDataString(request.RequestUri!.AbsolutePath);
    private static System.Collections.Specialized.NameValueCollection QueryOf(HttpRequestMessage request)
        => System.Web.HttpUtility.ParseQueryString(request.RequestUri!.Query);

    private const string Log = """{"data":[{"n":2,"t":"second"},{"n":1,"t":"first"}]}""";

    [Fact]
    public async Task Log_of_a_task_is_read_as_lines()
    {
        const string upid = "UPID:pve01:0012A3F4:05C1B2D3:6720F1A0:vzdump:100:root@pam:";
        var (client, handler) = ClientAnswering(("/log", Log));

        var lines = await client.Nodes["pve01"].Tasks[upid].Log.GetAsync(limit: 500, start: 10);

        Assert.Equal(["first", "second"], lines);
        var request = handler.Requests.Single();
        Assert.Equal($"/api2/json/nodes/pve01/tasks/{upid}/log", PathOf(request));
        Assert.Equal("500", QueryOf(request)["limit"]);
        Assert.Equal("10", QueryOf(request)["start"]);
    }

    [Fact]
    public async Task Firewall_and_replication_logs_are_read_as_lines()
    {
        var (client, handler) = ClientAnswering(("/log", Log));

        Assert.Equal(["first", "second"], await client.Nodes["pve01"].Firewall.Log.GetAsync(limit: 10));
        Assert.Equal("/api2/json/nodes/pve01/firewall/log", PathOf(handler.Requests.Last()));

        Assert.Equal(["first", "second"], await client.Nodes["pve01"].Qemu[100].Firewall.Log.GetAsync());
        Assert.Equal("/api2/json/nodes/pve01/qemu/100/firewall/log", PathOf(handler.Requests.Last()));

        Assert.Equal(["first", "second"], await client.Nodes["pve01"].Lxc[200].Firewall.Log.GetAsync());
        Assert.Equal("/api2/json/nodes/pve01/lxc/200/firewall/log", PathOf(handler.Requests.Last()));

        Assert.Equal(["first", "second"], await client.Nodes["pve01"].Replication["100-0"].Log.GetAsync());
        Assert.Equal("/api2/json/nodes/pve01/replication/100-0/log", PathOf(handler.Requests.Last()));
    }

    [Fact]
    public async Task Journal_is_read_without_empty_lines_and_cursors()
    {
        var (client, handler) = ClientAnswering(("/journal", """{"data":["s=abc;i=1","Oct 01 10:00:00 pve01 pvedaemon: start","","   ","Oct 01 10:00:01 pve01 pvedaemon: end","s=abc;i=2"]}"""));

        var lines = await client.Nodes["pve01"].Journal.GetAsync(lastentries: 100);

        Assert.Equal(["Oct 01 10:00:00 pve01 pvedaemon: start", "Oct 01 10:00:01 pve01 pvedaemon: end"], lines);
        Assert.Equal("100", QueryOf(handler.Requests.Single())["lastentries"]);
    }

    [Fact]
    public async Task Journal_without_data_is_empty()
    {
        var (client, _) = ClientAnswering(("/journal", """{"data":null}"""));

        Assert.Empty(await client.Nodes["pve01"].Journal.GetAsync());
    }

    [Fact]
    public async Task Metrics_are_asked_with_the_time_frame_and_the_consolidation()
    {
        var (client, handler) = ClientAnswering(("/rrddata", """{"data":[{"time":1767225600,"cpu":0.25,"maxcpu":4,"mem":1024,"maxmem":4096}]}"""));

        var qemu = await client.Nodes["pve01"].Qemu[100].Rrddata.GetAsync(RrdDataTimeFrame.Day, RrdDataConsolidation.Average);
        Assert.Equal("/api2/json/nodes/pve01/qemu/100/rrddata", PathOf(handler.Requests.Last()));
        Assert.Equal("day", QueryOf(handler.Requests.Last())["timeframe"]);
        Assert.Equal("AVERAGE", QueryOf(handler.Requests.Last())["cf"]);
        Assert.Equal(0.25, qemu.Single().CpuUsagePercentage);

        await client.Nodes["pve01"].Lxc[200].Rrddata.GetAsync(RrdDataTimeFrame.Week, RrdDataConsolidation.Maximum);
        Assert.Equal("/api2/json/nodes/pve01/lxc/200/rrddata", PathOf(handler.Requests.Last()));
        Assert.Equal("week", QueryOf(handler.Requests.Last())["timeframe"]);
        Assert.Equal("MAX", QueryOf(handler.Requests.Last())["cf"]);

        await client.Nodes["pve01"].Rrddata.GetAsync(RrdDataTimeFrame.Hour, RrdDataConsolidation.Average);
        Assert.Equal("/api2/json/nodes/pve01/rrddata", PathOf(handler.Requests.Last()));

        await client.Nodes["pve01"].Storage["local"].Rrddata.GetAsync(RrdDataTimeFrame.Year, RrdDataConsolidation.Average);
        Assert.Equal("/api2/json/nodes/pve01/storage/local/rrddata", PathOf(handler.Requests.Last()));
        Assert.Equal("year", QueryOf(handler.Requests.Last())["timeframe"]);

        await client.GetVmRrdDataAsync("pve01", VmType.Lxc, 200, RrdDataTimeFrame.Month, RrdDataConsolidation.Average);
        Assert.Equal("/api2/json/nodes/pve01/lxc/200/rrddata", PathOf(handler.Requests.Last()));
        Assert.Equal("month", QueryOf(handler.Requests.Last())["timeframe"]);
    }

    [Fact]
    public async Task Backups_are_read_from_every_active_storage_that_holds_them()
    {
        var (client, handler) = ClientAnswering(
            ("/storage", """{"data":[{"storage":"pbs01","type":"pbs","content":"backup","active":1,"enabled":1},{"storage":"nfs-off","type":"nfs","content":"backup","active":0,"enabled":1},{"storage":"local","type":"dir","content":"backup,iso","active":1,"enabled":1}]}"""),
            ("/storage/pbs01/content", """{"data":[{"volid":"pbs01:backup/vm/100/2026-10-01T00:00:00Z","content":"backup","vmid":100,"size":1024}]}"""),
            ("/storage/local/content", """{"data":[{"volid":"local:backup/vzdump-qemu-100-2026_10_02.vma.zst","content":"backup","vmid":100,"size":2048}]}"""));

        var backups = (await client.Nodes["pve01"].GetBackupsInAllStoragesAsync(100)).ToList();

        Assert.Equal(["pbs01:backup/vm/100/2026-10-01T00:00:00Z", "local:backup/vzdump-qemu-100-2026_10_02.vma.zst"], backups.Select(a => a.Volume));
        var paths = handler.Requests.Select(PathOf).ToList();
        Assert.DoesNotContain(paths, a => a.Contains("nfs-off"));
        Assert.Equal("backup", QueryOf(handler.Requests[0])["content"]);
        Assert.Equal("100", QueryOf(handler.Requests[1])["vmid"]);
        Assert.Equal("backup", QueryOf(handler.Requests[1])["content"]);
    }

    [Fact]
    public async Task Spice_file_is_built_from_the_answer()
    {
        var (client, handler) = ClientAnswering(("/spiceproxy", """{"data":{"type":"spice","host":"pvespiceproxy:abc","tls-port":61000,"password":"secret","proxy":"http://pve01:3128"}}"""));

        var (success, _, content) = await client.Nodes["pve01"].Qemu[100].Spiceproxy.GetSpiceFileVVAsync("pve01");

        Assert.True(success);
        Assert.Equal(HttpMethod.Post, handler.Requests.Single().Method);
        Assert.Equal("/api2/json/nodes/pve01/qemu/100/spiceproxy", PathOf(handler.Requests.Single()));
        var lines = content.Split(Environment.NewLine);
        Assert.Equal("[virt-viewer]", lines[0]);
        Assert.Contains("type=spice", lines);
        Assert.Contains("tls-port=61000", lines);
        Assert.Contains("proxy=http://pve01:3128", lines);

        await client.Nodes["pve01"].Lxc[200].Spiceproxy.GetSpiceFileVVAsync("pve01");
        Assert.Equal("/api2/json/nodes/pve01/lxc/200/spiceproxy", PathOf(handler.Requests.Last()));
    }

    [Fact]
    public async Task Spice_file_of_a_refused_request_is_empty_with_the_reason()
    {
        var client = FakeHandler.Client(new FakeHandler(_ =>
        {
            var response = FakeHandler.Json(HttpStatusCode.InternalServerError, """{"data":null}""");
            response.ReasonPhrase = "VM 100 not running";
            return response;
        }));

        var (success, reason, content) = await client.Nodes["pve01"].Qemu[100].Spiceproxy.GetSpiceFileVVAsync("pve01");

        Assert.False(success);
        Assert.Equal("VM 100 not running", reason);
        Assert.Equal(string.Empty, content);
    }

    [Fact]
    public async Task Permissions_are_read_as_privileges_of_each_path()
    {
        var (client, handler) = ClientAnswering(("/access/permissions", """{"data":{"/":{"Sys.Audit":1,"VM.Audit":1},"/vms/100":{"VM.PowerMgmt":1}}}"""));

        var permissions = await client.Access.Permissions.GetPermissionsAsync(userid: "automation@pve");

        Assert.Equal(["/", "/vms/100"], permissions.Keys);
        Assert.Equal(["Sys.Audit", "VM.Audit"], permissions["/"]);
        Assert.Equal(["VM.PowerMgmt"], permissions["/vms/100"]);
        Assert.Equal("automation@pve", QueryOf(handler.Requests.Single())["userid"]);
    }

    [Fact]
    public async Task Permissions_of_a_refused_request_are_reported_with_the_reason()
    {
        var client = FakeHandler.Client(new FakeHandler(_ =>
        {
            var response = FakeHandler.Json(HttpStatusCode.Forbidden, """{"data":null}""");
            response.ReasonPhrase = "Permission check failed (/access, Sys.Audit)";
            return response;
        }));

        var ex = await Assert.ThrowsAsync<PveResultException>(() => client.Access.Permissions.GetPermissionsAsync(userid: "other@pve"));

        Assert.Equal("Permission check failed (/access, Sys.Audit)", ex.Message);
    }

    [Fact]
    public async Task Cpu_level_is_read_for_every_node_that_is_online()
    {
        var (client, handler) = ClientAnswering(
            ("/cluster/resources", """{"data":[{"id":"node/pve01","type":"node","node":"pve01","status":"online"},{"id":"node/pve02","type":"node","node":"pve02","status":"offline"},{"id":"qemu/100","type":"qemu","vmid":100,"node":"pve01","status":"running"}]}"""),
            ("/nodes/pve01/status", """{"data":{"cpuinfo":{"flags":"cx16 lahf_lm popcnt sse4_1 sse4_2 ssse3 avx avx2 bmi1 bmi2 fma f16c abm movbe","cpus":8}}}"""));

        var levels = (await client.GetClusterCpuX86LevelsAsync()).ToList();

        Assert.Equal(["pve01"], levels.Select(a => a.Node));
        Assert.Equal("x86-64-v3", levels[0].Level.Name);
        Assert.DoesNotContain(handler.Requests, a => PathOf(a).Contains("pve02"));
    }

    [Theory]
    [InlineData("vv", WebConsoleType.Spice)]
    [InlineData("html5", WebConsoleType.NoVnc)]
    [InlineData("xtermjs", WebConsoleType.XtermJs)]
    [InlineData("applet", WebConsoleType.XtermJs)]
    public async Task Default_console_is_read_from_the_options_of_the_cluster(string console, WebConsoleType expected)
    {
        var (client, _) = ClientAnswering(("/cluster/options", "{\"data\":{\"console\":\"" + console + "\"}}"));

        Assert.Equal(expected, await client.GetDefaultWebConsoleAsync());
    }

    [Fact]
    public async Task Selections_offered_for_the_guests_follow_the_cluster()
    {
        var (client, _) = ClientAnswering(
            ("/cluster/resources", """{"data":[{"id":"node/pve02","type":"node","node":"pve02","status":"online"},{"id":"node/pve01","type":"node","node":"pve01","status":"online"},{"id":"node/pve03","type":"node","node":"pve03","status":"offline"},{"id":"/pool/customer1","type":"pool","pool":"customer1"},{"id":"qemu/100","type":"qemu","vmid":100,"name":"web01","node":"pve01","status":"running"},{"id":"lxc/200","type":"lxc","vmid":200,"name":"ct01","node":"pve02","status":"stopped"}]}"""),
            ("/cluster/options", """{"data":{"allowed-tags":["production","test"]}}"""));

        var all = (await client.GetVmIdsAsync(true, true, true, true, true, true)).ToList();

        Assert.Equal(["@all", "@all-pve01", "@all-pve02", "@node-pve01", "@node-pve02", "@pool-customer1", "@tag-production", "@tag-test", "100", "200", "ct01", "web01"], all);
        Assert.Equal(["@all"], await client.GetVmIdsAsync(true, false, false, false, false, false));
        Assert.Equal(["100", "200"], await client.GetVmIdsAsync(false, false, false, false, true, false));

        var keys = (await VmHelper.GetVmsJollyKeysAsync(client, true, true, true, true, true)).ToList();
        Assert.Contains("@all", keys);
        Assert.Contains("@pool-customer1", keys);
        Assert.Contains("@all-pve01", keys);
        Assert.DoesNotContain("@all-pve03", keys);
        Assert.Contains("web01", keys);
    }

    [Fact]
    public async Task Smart_data_that_cannot_be_read_is_one_attribute_with_the_error()
    {
        var client = FakeHandler.Client(new FakeHandler(_ =>
        {
            var response = FakeHandler.Json(HttpStatusCode.InternalServerError, """{"data":null}""");
            response.ReasonPhrase = "smartctl failed";
            return response;
        }));

        var smart = await client.GetDiskSmart("pve01", "/dev/sda");

        var attribute = Assert.Single(smart.Attributes);
        Assert.Equal("Error", attribute.Name);
        Assert.Equal("smartctl failed", attribute.Raw);
    }

    [Fact]
    public async Task Console_page_is_asked_with_the_ticket_of_the_client()
    {
        var handler = new FakeHandler(request => request.RequestUri!.AbsolutePath.EndsWith("/access/ticket")
            ? FakeHandler.Json(HttpStatusCode.OK, """{"data":{"ticket":"PVE:root@pam:TICKET","CSRFPreventionToken":"CSRF"}}""")
            : FakeHandler.Text(HttpStatusCode.OK, "<html>console</html>"));
        var client = FakeHandler.Client(handler);
        Assert.True(await client.LoginAsync("root@pam", "secret"));

        using var response = await VmHelper.GetConsoleNoVncAsync(client, "pve02", 100, "web01", VmType.Qemu, true, false);

        var request = handler.Requests.Last();
        Assert.Equal("https://pve01:8006/?console=kvm&vmid=100&vmname=web01&node=pve02&novnc=1", request.RequestUri!.AbsoluteUri);
        Assert.Equal("PVEAuthCookie=PVE:root@pam:TICKET", request.Headers.GetValues("Cookie").Single());
        Assert.Equal("<html>console</html>", await response.Content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData("backup")]
    [InlineData("images")]
    [InlineData("")]
    public async Task Upload_of_a_content_that_is_not_an_image_or_a_template_is_refused(string content)
    {
        var (client, handler) = ClientAnswering();
        using var file = new MemoryStream([1, 2, 3]);

        await Assert.ThrowsAsync<PveException>(() => client.UploadFileToStorageAsync("pve01", "local", content, file, "x.iso", CancellationToken.None));
        Assert.Empty(handler.Requests);
    }

    [Fact]
    public async Task File_is_uploaded_as_a_form_also_after_other_requests()
    {
        string? body = null;
        var handler = new FakeHandler(request =>
        {
            if (request.RequestUri!.AbsolutePath.EndsWith("/upload")) { body = request.Content!.ReadAsStringAsync().Result; }
            return FakeHandler.Json(HttpStatusCode.OK, """{"data":"UPID:pve01:0012A3F4:05C1B2D3:6720F1A0:imgcopy::root@pam:"}""");
        });
        var client = FakeHandler.Client(handler);
        client.ApiToken = "automation@pve!app=secret";
        await client.GetAsync("/version");   // the client has already made a request, as after a login
        using var file = new MemoryStream(Encoding.UTF8.GetBytes("ISO-CONTENT"));

        var result = await client.UploadFileToStorageAsync("pve01", "local", "iso", file, "debian.iso", CancellationToken.None, secondsTimeout: 30);

        Assert.True(result.IsSuccessStatusCode);
        Assert.StartsWith("UPID:", result.ToData<string>());
        Assert.Equal("/nodes/pve01/storage/local/upload", result.RequestResource);
        var request = handler.Requests.Last();
        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.Equal("/api2/json/nodes/pve01/storage/local/upload", PathOf(request));
        Assert.Equal("PVEAPIToken", request.Headers.Authorization!.Scheme);
        Assert.Equal("multipart/form-data", request.Content!.Headers.ContentType!.MediaType);
        Assert.Contains("ISO-CONTENT", body);
        Assert.Contains("debian.iso", body);
        Assert.Contains("name=\"content\"", body);
    }

    [Fact]
    public async Task Health_score_of_nodes_guests_and_storages()
    {
        var (client, _) = ClientAnswering(("/cluster/resources", """
            {"data":[
                {"id":"node/pve01","type":"node","node":"pve01","status":"online","cpu":0.5,"maxcpu":8,"mem":250,"maxmem":1000,"disk":50,"maxdisk":100},
                {"id":"qemu/100","type":"qemu","vmid":100,"node":"pve01","status":"running","cpu":0.2,"maxcpu":2,"mem":500,"maxmem":1000},
                {"id":"qemu/101","type":"qemu","vmid":101,"node":"pve01","status":"stopped","cpu":0,"maxcpu":2,"mem":0,"maxmem":1000},
                {"id":"storage/pve01/local","type":"storage","storage":"local","node":"pve01","status":"available","disk":30,"maxdisk":100},
                {"id":"storage/pve01/empty","type":"storage","storage":"empty","node":"pve01","status":"available","disk":0,"maxdisk":0},
                {"id":"/pool/customer1","type":"pool","pool":"customer1"}
            ]}
            """));

        var resources = (await client.GetResourcesAsync(ClusterResourceType.All)).ToDictionary(a => a.Id);

        Assert.Equal(60, resources["node/pve01"].HealthScoreCalculated);          // 100 - (50*0.4 + 25*0.4 + 50*0.2)
        Assert.Equal(65, resources["qemu/100"].HealthScoreCalculated);            // 100 - (20*0.5 + 50*0.5)
        Assert.Null(resources["qemu/101"].HealthScoreCalculated);                 // not running: nothing to measure
        Assert.Equal(70, resources["storage/pve01/local"].HealthScoreCalculated); // 100 - 30
        Assert.Equal(100, resources["storage/pve01/empty"].HealthScoreCalculated);
        Assert.Null(resources["/pool/customer1"].HealthScoreCalculated);
    }

    [Fact]
    public async Task Resources_are_asked_by_type()
    {
        var (client, handler) = ClientAnswering(("/cluster/resources", """{"data":[{"id":"node/pve01","type":"node","node":"pve01","status":"online"}]}"""));

        Assert.Single(await client.GetResourcesAsync("node"));
        Assert.Equal("node", QueryOf(handler.Requests.Last())["type"]);

        await client.GetResourcesAsync(ClusterResourceType.Storage);
        Assert.Equal("storage", QueryOf(handler.Requests.Last())["type"]);

        await client.GetResourcesAsync(ClusterResourceType.All);
        Assert.Null(QueryOf(handler.Requests.Last())["type"]);

        // pools and sdn are in the answer of every resource, not a type that can be asked
        await Assert.ThrowsAsync<System.ComponentModel.InvalidEnumArgumentException>(() => client.GetResourcesAsync(ClusterResourceType.Pool));
    }

    [Fact]
    public async Task Picture_is_returned_as_a_data_address()
    {
        byte[] png = [0x89, 0x50, 0x4E, 0x47, 1, 2, 3];
        var handler = new FakeHandler(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(png) });
        var client = FakeHandler.Client(handler);
        client.ResponseType = ResponseType.Png;

        var result = await client.GetAsync("/nodes/pve01/rrd", new Dictionary<string, object> { ["ds"] = "cpu", ["timeframe"] = "day" });

        Assert.True(result.IsSuccessStatusCode);
        Assert.Equal("data:image/png;base64," + Convert.ToBase64String(png), (string)result.Response);
        Assert.Equal("/api2/png/nodes/pve01/rrd", handler.Requests.Single().RequestUri!.AbsolutePath);
    }
}
