/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Collections;
using System.Reflection;
using Corsinvest.ProxmoxVE.Api.Metadata;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Metadata;

public class FlatCacheTests
{
    // A piece of apidoc.js with every field the model reads.
    private const string ApiDoc = """
        [{"path":"/nodes","text":"nodes","leaf":0,"info":{"GET":{"method":"GET","name":"index","description":"Cluster node index.",
            "parameters":{"additionalProperties":0},
            "returns":{"type":"array","links":[{"rel":"child","href":"{node}"}],"items":{"type":"object","properties":{
               "node":{"type":"string","description":"The cluster node name."},
               "maxmem":{"type":"integer","optional":1,"renderer":"bytes","description":"Number of available memory in bytes."},
               "cpu":{"type":"number","optional":1,"renderer":"fraction_as_percentage"},
               "uptime":{"type":"integer","optional":1,"renderer":"duration"}}}}}},
          "children":[{"path":"/nodes/{node}","text":"{node}","leaf":0,"children":[
            {"path":"/nodes/{node}/qemu-server","text":"qemu-server","leaf":1,"info":{"PUT":{"method":"PUT","name":"update_vm",
                "description":"Set virtual machine options.",
                "parameters":{"additionalProperties":0,"properties":{
                   "node":{"type":"string","format":"pve-node","description":"The cluster node name."},
                   "onboot":{"type":"boolean","optional":1,"default":0,"description":"Start at boot."},
                   "memory":{"type":"string","optional":1,"typetext":"<integer>","minimum":16,"maximum":4178944,
                             "verbose_description":"Memory properties, long text.","description":"Memory."},
                   "net[n]":{"type":"string","optional":1,"description":"Network device.",
                             "format":{"model":{"type":"string","enum":["e1000","virtio"],"default_key":1,"description":"Model."},
                                       "bridge":{"type":"string","optional":1,"format_description":"bridge","maxLength":64}}},
                   "tags":{"type":"array","optional":1,"items":{"type":"object","properties":{"name":{"type":"string"}}}}}},
                "returns":{"type":"null"}}}}]}]}]
        """;

    private static ClassApi FromApiDoc(string json)
    {
        var root = new ClassApi();
        foreach (var token in JArray.Parse(json)) { _ = new ClassApi(token, root); }
        return root;
    }

    private static ClassApi RoundTrip(ClassApi root)
        => GeneratorClassApi.BuildClassApiFromFlat(GeneratorClassApi.LoadFlatCache(GeneratorClassApi.BuildFlatCache(root))!);

    [Fact]
    public void Renderer_survives_the_cache()
    {
        var method = ClassApi.GetFromResource(RoundTrip(FromApiDoc(ApiDoc)), "/nodes")!.Methods.Single();
        Assert.Equal("bytes", method.ReturnParameters.Single(a => a.Name == "maxmem").Renderer);
        Assert.Equal("fraction_as_percentage", method.ReturnParameters.Single(a => a.Name == "cpu").Renderer);
    }

    [Fact]
    public void Cache_keeps_every_field_of_the_sample()
        => Assert.Equal(Describe(FromApiDoc(ApiDoc)), Describe(RoundTrip(FromApiDoc(ApiDoc))));

    [Fact]
    public async Task Cache_keeps_every_field_of_the_real_schema()
    {
        var root = await GeneratorClassApi.GenerateAsync();
        var expected = Describe(root);
        var actual = Describe(RoundTrip(root));

        Assert.Equal([], expected.Except(actual).Take(10));
        Assert.Equal([], actual.Except(expected).Take(10));
    }

    [Fact]
    public void Cache_of_another_format_version_is_not_loaded()
    {
        Assert.Null(GeneratorClassApi.LoadFlatCache("""{"formatVersion":1,"resources":{}}"""));
        Assert.NotNull(GeneratorClassApi.LoadFlatCache(GeneratorClassApi.BuildFlatCache(FromApiDoc(ApiDoc))));
        Assert.Contains($"\"formatVersion\":{GeneratorClassApi.FlatCacheFormatVersion}", GeneratorClassApi.BuildFlatCache(FromApiDoc(ApiDoc)));
    }

    // One line per resource, method, parameter and format, with every public property that is not a link to another node.
    private static List<string> Describe(ClassApi root)
    {
        var ret = new List<string>();
        void Node(ClassApi node)
        {
            if (!node.IsRoot) { ret.Add($"{node.Resource} {Props(node)}"); }
            foreach (var method in node.Methods)
            {
                ret.Add($"{node.Resource} {method.MethodType} {Props(method)}");
                foreach (var p in method.Parameters) { Param($"{node.Resource} {method.MethodType} param", p); }
                foreach (var p in method.ReturnParameters) { Param($"{node.Resource} {method.MethodType} return", p); }
            }

            foreach (var sub in node.SubClasses.OrderBy(a => a.Resource)) { Node(sub); }
        }

        void Param(string prefix, ParameterApi p)
        {
            ret.Add($"{prefix} {Props(p)}");
            foreach (var f in p.Formats) { ret.Add($"{prefix} {p.Name} format {Props(f)}"); }
            foreach (var i in p.Items) { Param($"{prefix} {p.Name} item", i); }
        }

        Node(root);
        return ret;
    }

    private static string Props(object item)
        => string.Join(" ", item.GetType()
                                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .Where(a => a.PropertyType == typeof(string)
                                            || a.PropertyType == typeof(string[])
                                            || a.PropertyType == typeof(List<string>)
                                            || (a.PropertyType != typeof(ClassApi) && !typeof(IEnumerable).IsAssignableFrom(a.PropertyType)))
                                .OrderBy(a => a.Name)
                                .Select(a => $"{a.Name}={Value(a.GetValue(item))}"));

    private static string Value(object value)
        => value switch
        {
            null => "null",
            string text => text,
            IEnumerable<string> list => "[" + string.Join(",", list) + "]",
            _ => value.ToString() ?? string.Empty,
        };
}
