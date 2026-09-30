/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Net;
using Corsinvest.ProxmoxVE.Api.Extension.Utils;
using Corsinvest.ProxmoxVE.Api.Metadata;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Shell;

#pragma warning disable CS0618 // the obsolete members are what is tested here
public class ApiExplorerHelperTests
{
    // Schema of /nodes/{node}/qemu/{vmid}/config naming "memory" and "net[n]" as return keys, as the real API does.
    private static ClassApi Schema()
        => GeneratorClassApi.BuildClassApiFromFlat(GeneratorClassApi.LoadFlatCache("""
            {"/nodes":{"children":[{"name":"{node}","indexed":true}],"methods":{"get":{"returnType":"array"}}},
             "/nodes/{node}":{"keys":["node"],"children":[{"name":"qemu","hasChildren":true}],"methods":{"get":{"returnType":"array"}}},
             "/nodes/{node}/qemu":{"keys":["node"],"children":[{"name":"{vmid}","indexed":true}],"methods":{"get":{"returnType":"array"}}},
             "/nodes/{node}/qemu/{vmid}":{"keys":["node","vmid"],"children":[{"name":"config"}],"methods":{"get":{"returnType":"array"}}},
             "/nodes/{node}/qemu/{vmid}/config":{"keys":["node","vmid"],"methods":{"get":{"returnType":"object",
                "returnParams":[{"name":"memory","type":"string","optional":true},{"name":"net[n]","type":"string","optional":true}]}}}}
            """)!);

    [Fact]
    public async Task Object_shows_numbered_keys()
    {
        var client = FakeHandler.Client(new FakeHandler(_ =>
            FakeHandler.Json(HttpStatusCode.OK, """{"data":{"memory":"4096","net0":"virtio=AA:BB,bridge=vmbr0","usb0":"spice"}}""")));

        var (code, text) = await ApiExplorerHelper.ExecuteAsync(client, Schema(), "/nodes/pve01/qemu/100/config", MethodType.Get,
                                                                 new Dictionary<string, object>());

        Assert.Equal(200, code);
        Assert.Contains("net0", text);
        Assert.Contains("usb0", text);
        Assert.Contains("memory", text);
    }

    [Fact]
    public async Task Verbose_on_an_error_shows_the_answer()
    {
        var client = FakeHandler.Client(new FakeHandler(_ =>
            FakeHandler.Json(HttpStatusCode.BadRequest, """{"data":null,"errors":{"foo":"property is not defined"}}""")));

        var (code, text) = await ApiExplorerHelper.ExecuteAsync(client, Schema(), "/nodes/pve01/qemu/100/config", MethodType.Get,
                                                                 new Dictionary<string, object> { ["foo"] = "1" }, verbose: true);

        Assert.Equal(400, code);
        Assert.Contains("\"foo\": \"property is not defined\"", text);
    }

    [Fact]
    public async Task Error_lists_the_parameters_as_before()
    {
        var client = FakeHandler.Client(new FakeHandler(_ =>
            FakeHandler.Json(HttpStatusCode.BadRequest, """{"data":null,"errors":{"foo":"property is not defined"}}""")));

        var (_, text) = await ApiExplorerHelper.ExecuteAsync(client, Schema(), "/nodes/pve01/qemu/100/config", MethodType.Get,
                                                              new Dictionary<string, object> { ["foo"] = "1" });

        Assert.Contains("foo : property is not defined", text);
    }

    [Fact]
    public async Task Status_200_with_errors_still_returns_200()
    {
        var client = FakeHandler.Client(new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK, """{"data":null,"errors":{"x":"y"}}""")));
        var (code, _) = await ApiExplorerHelper.ExecuteAsync(client, Schema(), "/nodes/pve01/qemu/100/config", MethodType.Get,
                                                              new Dictionary<string, object>());
        Assert.Equal(200, code);
    }

    [Fact]
    public void Repeated_key_names_the_key()
    {
        var ex = Assert.Throws<ArgumentException>(() => ApiExplorerHelper.CreateParameterResource(["type:vm", "type:node"]));
        Assert.Equal("Parameter 'type' is given more than once.", ex.Message);
    }

    [Fact]
    public void Argument_tags_are_the_placeholders()
        => Assert.Equal(["node", "vmid"], ApiExplorerHelper.GetArgumentTags("get /nodes/{node}/qemu/{vmid}/config"));

    [Fact]
    public async Task List_text_shows_the_schema_columns_json_every_column()
    {
        var root = GeneratorClassApi.BuildClassApiFromFlat(GeneratorClassApi.LoadFlatCache("""
            {"/nodes":{"methods":{"get":{"returnType":"array","returnParams":[{"name":"node","type":"string"}]}}}}
            """)!);
        var client = FakeHandler.Client(new FakeHandler(_ =>
            FakeHandler.Json(HttpStatusCode.OK, """{"data":[{"node":"pve01","ssl_fingerprint":"AA:BB"}]}""")));

        var (_, text) = await ApiExplorerHelper.ExecuteAsync(client, root, "/nodes", MethodType.Get, new Dictionary<string, object>());
        var (_, json) = await ApiExplorerHelper.ExecuteAsync(client, root, "/nodes", MethodType.Get, new Dictionary<string, object>(),
                                                             output: TableGenerator.Output.Json);

        Assert.DoesNotContain("ssl_fingerprint", text);
        Assert.Contains("ssl_fingerprint", json);
    }

    [Fact]
    public void Alias_table_columns_as_before()
    {
        var aliases = new ApiExplorerHelper.AliasManager();

        var brief = Newtonsoft.Json.Linq.JArray.Parse(aliases.ToTable(false, TableGenerator.Output.Json));
        Assert.Equal(["name", "description", "sys"], ((Newtonsoft.Json.Linq.JObject)brief[0]).Properties().Select(a => a.Name));
        Assert.Equal("X", (string)brief[0]["sys"]!);

        var verbose = Newtonsoft.Json.Linq.JArray.Parse(aliases.ToTable(true, TableGenerator.Output.Json));
        Assert.Equal(["name", "description", "command", "args", "sys"],
                     ((Newtonsoft.Json.Linq.JObject)verbose[0]).Properties().Select(a => a.Name));
        Assert.Contains(verbose, a => (string)a["args"]! == "node");
    }

    private static ClassApi StatusSchema()
        => GeneratorClassApi.BuildClassApiFromFlat(GeneratorClassApi.LoadFlatCache("""
            {"/nodes":{"children":[{"name":"{node}","indexed":true}],"methods":{"get":{"returnType":"array","returnLinkHRef":"{node}"}}},
             "/nodes/{node}":{"keys":["node"],"children":[{"name":"status"}],"methods":{"get":{"returnType":"array"}}},
             "/nodes/{node}/status":{"keys":["node"],"methods":{"get":{"returnType":"object"},"post":{"comment":"Reboot or shutdown a node.",
                "params":[{"name":"node","type":"string"},{"name":"command","type":"string","enumValues":["reboot","shutdown"]},
                          {"name":"force","type":"boolean","optional":true}]}}}}
            """)!);

    [Fact]
    public void Usage_text_as_before()
        => Assert.Equal($"USAGE: get /nodes/pve01/status{Environment.NewLine}"
                        + $"USAGE: create /nodes/pve01/status --command <string> [OPTIONS]{Environment.NewLine}",
                        ApiExplorerHelper.Usage(StatusSchema(), "/nodes/pve01/status", TableGenerator.Output.Text, optionStyle: true));

    [Fact]
    public void Usage_of_one_method_and_unknown_path()
    {
        Assert.Equal($"USAGE: create /nodes/pve01/status command:<string> [OPTIONS]{Environment.NewLine}",
                     ApiExplorerHelper.Usage(StatusSchema(), "/nodes/pve01/status", TableGenerator.Output.Text, command: "create"));
        Assert.Equal($"no such resource '/nodex'{Environment.NewLine}",
                     ApiExplorerHelper.Usage(StatusSchema(), "/nodex", TableGenerator.Output.Text));
    }

    [Fact]
    public void Parameters_and_values_of_a_method()
    {
        Assert.Equal(["command", "force"], ApiExplorerHelper.GetMethodParameters(StatusSchema(), "/nodes/pve01/status", MethodType.Create));
        Assert.Equal(["reboot", "shutdown"], ApiExplorerHelper.GetMethodParameterEnumValues(StatusSchema(), "/nodes/pve01/status", MethodType.Create, "command"));
        Assert.Equal(["0", "1"], ApiExplorerHelper.GetMethodParameterEnumValues(StatusSchema(), "/nodes/pve01/status", MethodType.Create, "force"));
        Assert.Empty(ApiExplorerHelper.GetMethodParameters(StatusSchema(), "/nodes/pve01/status", MethodType.Delete));
    }

    [Fact]
    public async Task List_values_with_attributes()
    {
        var client = FakeHandler.Client(new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK, """{"data":[{"node":"pve01"}]}""")));

        var (values, error) = await ApiExplorerHelper.ListValuesAsync(client, StatusSchema(), "/nodes");
        Assert.Equal(string.Empty, error);
        Assert.Equal([("Dr---", "pve01")], values);

        (values, _) = await ApiExplorerHelper.ListValuesAsync(client, StatusSchema(), "/nodes/pve01");
        Assert.Equal([("-r--c", "status")], values);
    }
}
#pragma warning restore CS0618
