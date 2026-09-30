/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Dynamic;
using Corsinvest.ProxmoxVE.Api.Extension.Shell;
using Corsinvest.ProxmoxVE.Api.Metadata;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Shell;

public class ApiSchemaTableTests
{
    // /nodes/{node}/qemu/{vmid}/config naming only "memory" and "net[n]" as return keys, as the real schema does.
    private static ClassApi Schema()
        => GeneratorClassApi.BuildClassApiFromFlat(GeneratorClassApi.LoadFlatCache("""
            {"/nodes":{"children":[{"name":"{node}","indexed":true}],"methods":{"get":{"returnType":"array"}}},
             "/nodes/{node}":{"keys":["node"],"children":[{"name":"qemu","hasChildren":true}],"methods":{"get":{"returnType":"array"}}},
             "/nodes/{node}/qemu":{"keys":["node"],"children":[{"name":"{vmid}","indexed":true}],"methods":{"get":{"returnType":"array"}}},
             "/nodes/{node}/qemu/{vmid}":{"keys":["node","vmid"],"children":[{"name":"config"},{"name":"rules"},{"name":"nets"},{"name":"disks"}],"methods":{"get":{"returnType":"array"}}},
             "/nodes/{node}/qemu/{vmid}/rules":{"keys":["node","vmid"],"methods":{"get":{"returnType":"array",
                "returnParams":[{"name":"pos","type":"integer"}]}}},
             "/nodes/{node}/qemu/{vmid}/disks":{"keys":["node","vmid"],"methods":{"get":{"returnType":"array",
                "returnParams":[{"name":"name","type":"string"},{"name":"size","type":"integer","renderer":"bytes"},
                                {"name":"cpu","type":"number","renderer":"fraction_as_percentage"}]}}},
             "/nodes/{node}/qemu/{vmid}/nets":{"keys":["node","vmid"],"methods":{"get":{"returnType":"array",
                "returnParams":[{"name":"id","type":"integer"},{"name":"net[n]","type":"string","optional":true}]}}},
             "/nodes/{node}/qemu/{vmid}/config":{"keys":["node","vmid"],"methods":{"get":{"returnType":"object",
                "returnParams":[{"name":"memory","type":"string","optional":true},{"name":"net[n]","type":"string","optional":true}]}}}}
            """)!);

    private static ExpandoObject Object(params (string Key, object Value)[] values)
    {
        var ret = new ExpandoObject();
        foreach (var (key, value) in values) { ((IDictionary<string, object>)ret)[key] = value; }
        return ret;
    }

    [Fact]
    public void Object_shows_every_key_even_those_the_schema_does_not_name()
        => Assert.Equal("""[{"key":"memory","value":"4096"},{"key":"net0","value":"virtio,bridge=vmbr0"},{"key":"usb0","value":"spice"}]""",
                        ApiSchema.ToTable(Object(("memory", "4096"), ("net0", "virtio,bridge=vmbr0"), ("usb0", "spice")),
                                          Schema(), "/nodes/pve01/qemu/100/config")!.ToJson());

    [Fact]
    public void List_of_strings_is_one_column()
        => Assert.Equal("""[{"value":"a"},{"value":"b"}]""",
                        ApiSchema.ToTable(new List<object> { "a", "b" }, Schema(), "/nodes")!.ToJson());

    [Fact]
    public void Object_hides_nothing()
    {
        ApiSchema.ToTable(Object(("memory", "4096"), ("usb0", "spice")), Schema(), "/nodes/pve01/qemu/100/config", new ApiTableOptions(), out var hidden);
        Assert.Empty(hidden);
    }

    [Fact]
    public void List_shows_the_schema_columns_and_tells_the_hidden_ones()
    {
        var rule = Object(("pos", 0L), ("action", "ACCEPT"), ("dport", "22"));
        var table = ApiSchema.ToTable(new List<object> { rule }, Schema(), "/nodes/pve01/qemu/100/rules", new ApiTableOptions(), out var hidden);

        Assert.Equal("""[{"pos":0}]""", table!.ToJson());
        Assert.Equal(["action", "dport"], hidden);
    }

    [Fact]
    public void List_with_all_columns()
    {
        var rule = Object(("pos", 0L), ("action", "ACCEPT"), ("dport", "22"));
        var table = ApiSchema.ToTable(new List<object> { rule }, Schema(), "/nodes/pve01/qemu/100/rules", new ApiTableOptions(AllColumns: true), out var hidden);

        Assert.Equal("""[{"pos":0,"action":"ACCEPT","dport":"22"}]""", table!.ToJson());
        Assert.Empty(hidden);
    }

    [Fact]
    public void List_matches_numbered_keys_of_the_schema()
    {
        var item = Object(("id", 1L), ("net10", "c"), ("net1", "b"), ("net0", "a"), ("foo", "x"));
        var table = ApiSchema.ToTable(new List<object> { item }, Schema(), "/nodes/pve01/qemu/100/nets", new ApiTableOptions(), out var hidden);

        Assert.Equal("""[{"id":1,"net0":"a","net1":"b","net10":"c"}]""", table!.ToJson());
        Assert.Equal(["foo"], hidden);
    }

    [Fact]
    public void List_is_sorted_by_the_first_column_and_a_missing_key_is_empty()
        => Assert.Equal("""[{"pos":1,"action":null},{"pos":2,"action":"DROP"}]""",
                        ApiSchema.ToTable(new List<object> { Object(("pos", 2L), ("action", "DROP")), Object(("pos", 1L)) },
                                          Schema(), "/nodes/pve01/qemu/100/rules", new ApiTableOptions(AllColumns: true))!.ToJson());

    [Fact]
    public void Single_value_and_nothing_are_not_a_table()
    {
        Assert.Null(ApiSchema.ToTable(107L, Schema(), "/cluster/nextid"));
        Assert.Null(ApiSchema.ToTable(null, Schema(), "/nodes"));
    }

    [Fact]
    public void Empty_object_or_list_prints_nothing()
    {
        Assert.Equal(string.Empty, ApiSchema.ToTable(new ExpandoObject(), Schema(), "/nodes/pve01/qemu/100/config")!.ToText());
        Assert.Equal(string.Empty, ApiSchema.ToTable(new List<object>(), Schema(), "/nodes")!.ToText());
    }

    private static List<object> Disks() => [Object(("name", "a"), ("size", 1536L), ("cpu", 0.5d))];

    [Fact]
    public void Values_are_human_readable_and_aligned_right()
    {
        var table = ApiSchema.ToTable(Disks(), Schema(), "/nodes/pve01/qemu/100/disks")!;

        Assert.Equal("""[{"cpu":"50%","name":"a","size":"1.50 KiB"}]""", table.ToJson());
        Assert.Equal("|----:|------|---------:|", table.ToMarkdown().Split(Environment.NewLine)[1]);
    }

    [Fact]
    public void Values_as_the_api_returns_them()
        => Assert.Equal("""[{"cpu":0.5,"name":"a","size":1536}]""",
                        ApiSchema.ToTable(Disks(), Schema(), "/nodes/pve01/qemu/100/disks", new ApiTableOptions(HumanReadable: false))!.ToJson());
}
