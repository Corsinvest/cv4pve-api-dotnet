/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Net;
using Corsinvest.ProxmoxVE.Api.Extension.Shell;
using Corsinvest.ProxmoxVE.Api.Metadata;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Shell;

public class ApiSchemaTests
{
    private static ClassApi Schema()
        => GeneratorClassApi.BuildClassApiFromFlat(GeneratorClassApi.LoadFlatCache("""
            {"/nodes":{"children":[{"name":"{node}","indexed":true}],"methods":{"get":{"comment":"Cluster node index.","returnType":"array","returnLinkHRef":"{node}"}}},
             "/nodes/{node}":{"keys":["node"],"children":[{"name":"qemu","hasChildren":true},{"name":"status"}],"methods":{"get":{"returnType":"array"}}},
             "/nodes/{node}/status":{"keys":["node"],"methods":{"get":{"returnType":"object"},"post":{"comment":"Reboot or shutdown a node.",
                "params":[{"name":"command","type":"string","enumValues":["reboot","shutdown"]}]}}},
             "/nodes/{node}/qemu":{"keys":["node"],"children":[{"name":"{vmid}","indexed":true}],"methods":{"get":{"returnType":"array"}}},
             "/nodes/{node}/qemu/{vmid}":{"keys":["node","vmid"],"children":[{"name":"config"}],"methods":{"get":{"returnType":"array"}}},
             "/nodes/{node}/qemu/{vmid}/config":{"keys":["node","vmid"],"methods":{
                "get":{"comment":"Get the VM configuration.","returnType":"object",
                       "returnParams":[{"name":"memory","type":"string","optional":true}]},
                "put":{"comment":"Set virtual machine options.",
                       "params":[{"name":"memory","type":"string","optional":true,"description":"Memory in MiB."},
                                 {"name":"onboot","type":"boolean","optional":true}]}}}}
            """)!);

    [Fact]
    public void Methods_of_a_path_without_the_path_keys()
    {
        var methods = ApiSchema.GetMethods(Schema(), "/nodes/pve01/qemu/100/config")!;

        Assert.Equal([MethodType.Get, MethodType.Set], methods.Select(a => a.Method));
        var set = methods.Single(a => a.Method == MethodType.Set);
        Assert.Equal("Set virtual machine options.", set.Description);
        Assert.Equal(["memory", "onboot"], set.Parameters.Select(a => a.Name));
        Assert.Equal(["memory"], methods.Single(a => a.Method == MethodType.Get).Returns.Select(a => a.Name));
    }

    [Fact]
    public void Unknown_path_has_no_methods() => Assert.Null(ApiSchema.GetMethods(Schema(), "/nodex"));

    [Fact]
    public void One_method()
    {
        Assert.Equal(MethodType.Create, ApiSchema.GetMethod(Schema(), "/nodes/pve01/status", MethodType.Create)!.Method);
        Assert.Null(ApiSchema.GetMethod(Schema(), "/nodes/pve01/status", MethodType.Delete));
    }

    [Fact]
    public void Allowed_values_from_the_list_or_a_boolean()
    {
        var status = ApiSchema.GetMethod(Schema(), "/nodes/pve01/status", MethodType.Create)!;
        Assert.Equal(["reboot", "shutdown"], ApiSchema.GetAllowedValues(status.Parameters.Single()));

        var config = ApiSchema.GetMethod(Schema(), "/nodes/pve01/qemu/100/config", MethodType.Set)!;
        Assert.Equal(["0", "1"], ApiSchema.GetAllowedValues(config.Parameters.Single(a => a.Name == "onboot")));
        Assert.Empty(ApiSchema.GetAllowedValues(config.Parameters.Single(a => a.Name == "memory")));
    }

    [Fact]
    public async Task Static_children_with_their_kind()
    {
        var result = await ApiSchema.GetChildrenAsync(FakeHandler.Client(new FakeHandler(_ => throw new InvalidOperationException("no call expected"))),
                                                      Schema(),
                                                      "/nodes/pve01");

        Assert.Null(result.Error);
        Assert.Equal([new ApiChild("qemu", true, false), new ApiChild("status", false, true)], result.Children);
    }

    [Fact]
    public async Task Indexed_children_are_read_from_the_cluster()
    {
        var client = FakeHandler.Client(new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK, """{"data":[{"node":"pve02"},{"node":"pve01"}]}""")));
        var result = await ApiSchema.GetChildrenAsync(client, Schema(), "/nodes");

        Assert.Equal(["pve01", "pve02"], result.Children.Select(a => a.Name));
        Assert.All(result.Children, a => Assert.True(a.HasChildren));
    }

    [Fact]
    public async Task Error_reading_indexed_children_is_returned()
    {
        var client = FakeHandler.Client(new FakeHandler(_ =>
        {
            var answer = FakeHandler.Json(HttpStatusCode.Forbidden, """{"data":null}""");
            answer.ReasonPhrase = "Permission check failed (/nodes, Sys.Audit)";
            return answer;
        }));
        var result = await ApiSchema.GetChildrenAsync(client, Schema(), "/nodes");

        Assert.Empty(result.Children);
        Assert.Equal("Permission check failed (/nodes, Sys.Audit)", result.Error);
    }

    [Fact]
    public async Task Unknown_path_has_no_children()
    {
        var result = await ApiSchema.GetChildrenAsync(FakeHandler.Client(new FakeHandler(_ => throw new InvalidOperationException())), Schema(), "/nodex");
        Assert.Equal("no such resource '/nodex'", result.Error);
    }

    [Fact]
    public async Task Indexed_numbers_are_sorted_as_numbers()
    {
        var root = GeneratorClassApi.BuildClassApiFromFlat(GeneratorClassApi.LoadFlatCache("""
            {"/nodes":{"children":[{"name":"{node}","indexed":true}],"methods":{"get":{"returnType":"array","returnLinkHRef":"{node}"}}},
             "/nodes/{node}":{"keys":["node"],"children":[{"name":"qemu","hasChildren":true}],"methods":{"get":{"returnType":"array"}}},
             "/nodes/{node}/qemu":{"keys":["node"],"children":[{"name":"{vmid}","indexed":true}],"methods":{"get":{"returnType":"array","returnLinkHRef":"{vmid}"}}},
             "/nodes/{node}/qemu/{vmid}":{"keys":["node","vmid"],"methods":{"get":{"returnType":"array"}}}}
            """)!);
        var client = FakeHandler.Client(new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK, """{"data":[{"vmid":100},{"vmid":99},{"vmid":1000}]}""")));

        var result = await ApiSchema.GetChildrenAsync(client, root, "/nodes/pve01/qemu");

        Assert.Equal(["99", "100", "1000"], result.Children.Select(a => a.Name));
    }

    [Fact]
    public async Task Children_stop_when_cancelled()
    {
        var handler = new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK, """{"data":[{"node":"pve01"}]}"""));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => ApiSchema.GetChildrenAsync(FakeHandler.Client(handler), Schema(), "/nodes", new CancellationToken(true)));
        Assert.Empty(handler.Requests);
    }
}
