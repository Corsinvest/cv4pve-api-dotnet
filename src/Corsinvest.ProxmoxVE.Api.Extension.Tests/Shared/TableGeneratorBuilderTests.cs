/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Dynamic;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Shared;

public class TableGeneratorBuilderTests
{
    private sealed record Snap(string Node, int VmId, int? Size, bool Running, DayOfWeek Day, object Extra);

    private static readonly Snap[] Snaps =
    [
        new("pve1", 100, 10, true, DayOfWeek.Monday, 5),
        new("pve2", 1005, null, false, DayOfWeek.Friday, "x"),
    ];

    private static string Separator(string markdown) => markdown.Split(Environment.NewLine)[1];

    private static ExpandoObject Row(params (string Key, object Value)[] values)
    {
        var ret = new ExpandoObject();
        foreach (var (key, value) in values) { ((IDictionary<string, object>)ret)[key] = value; }
        return ret;
    }

    [Fact]
    public void Title_from_the_member_and_same_output_as_rows_by_hand()
        => Assert.Equal(new TableGenerator("Node", "VmId").AddRow("pve1", 100).AddRow("pve2", 1005).ToText(),
                        TableGenerator.From(Snaps).Column(a => a.Node).Column(a => a.VmId).ToText());

    [Fact]
    public void Title_Format_and_When()
        => Assert.Equal("""[{"NODE":"PVE1"},{"NODE":"PVE2"}]""",
                        TableGenerator.From(Snaps)
                                      .Column(a => a.Node).Title("NODE").Format(v => ((string)v!).ToUpperInvariant())
                                      .Column(a => a.VmId).When(false)
                                      .ToJson());

    [Fact]
    public void Expression_that_is_not_a_member_needs_a_title()
    {
        var ex = Assert.Throws<ArgumentException>(() => TableGenerator.From(Snaps).Column(a => a.Running ? "X" : "").Build());
        Assert.StartsWith("Column title is required for '", ex.Message);

        Assert.Equal("""[{"VM STATUS":"X"},{"VM STATUS":""}]""",
                     TableGenerator.From(Snaps).Column(a => a.Running ? "X" : "").Title("VM STATUS").ToJson());
    }

    [Fact]
    public void Two_columns_with_the_same_title()
        => Assert.Equal("Column 'Node' is already defined.",
                        Assert.Throws<ArgumentException>(() => TableGenerator.From(Snaps).Column(a => a.Node).Column("Node").Build()).Message);

    [Fact]
    public void Alignment_of_each_kind_of_member()
    {
        string Sep(TableGenerator.ColumnBuilder<Snap> column) => Separator(column.ToMarkdown());

        Assert.EndsWith(":|", Sep(TableGenerator.From(Snaps).Column(a => a.VmId)));        // int
        Assert.EndsWith(":|", Sep(TableGenerator.From(Snaps).Column(a => a.Size)));        // int?
        Assert.EndsWith("-|", Sep(TableGenerator.From(Snaps).Column(a => a.Node)));        // string
        Assert.EndsWith("-|", Sep(TableGenerator.From(Snaps).Column(a => a.Running)));     // bool
        Assert.EndsWith("-|", Sep(TableGenerator.From(Snaps).Column(a => a.Day)));         // enum
        Assert.EndsWith("-|", Sep(TableGenerator.From(Snaps).Column(a => a.Extra)));       // object: 5 and "x"
        Assert.EndsWith(":|", Sep(TableGenerator.From(Snaps.Take(1)).Column(a => a.Extra))); // object: 5
        Assert.EndsWith("-|", Sep(TableGenerator.From(Snaps).Column(a => a.VmId).Format(v => $"#{v}")));
        Assert.EndsWith("-|", Sep(TableGenerator.From(Snaps).Column(a => a.VmId).Align(TableGenerator.Align.Left)));
        Assert.EndsWith(":|", Sep(TableGenerator.From(Array.Empty<Snap>()).Column(a => a.VmId)));
    }

    [Fact]
    public void Dynamic_rows_by_key()
    {
        IEnumerable<dynamic> rows = [Row(("node", "pve1"), ("maxmem", 1024L)), Row(("node", "pve2"))];

        Assert.Equal("""[{"NODE":"pve1","maxmem":1024,"mem":"1024"},{"NODE":"pve2","maxmem":null,"mem":"-"}]""",
                     TableGenerator.From(rows)
                                   .Column("node").Title("NODE")
                                   .Column("maxmem")
                                   .Column("maxmem").Title("mem").Format(v => v == null ? "-" : v.ToString())
                                   .ToJson());
    }

    [Fact]
    public void Dynamic_numbers_long_and_double_are_right()
    {
        IEnumerable<dynamic> rows = [Row(("cpu", 1L)), Row(("cpu", 0.5d))];
        Assert.Equal("|----:|", Separator(TableGenerator.From(rows).Column("cpu").ToMarkdown()));
    }

    [Fact]
    public void Dictionary_rows_by_key()
    {
        var rows = new List<Dictionary<string, object>> { new() { ["a"] = "x" }, new() { ["b"] = "y" } };
        Assert.Equal("""[{"a":"x","b":null},{"a":null,"b":"y"}]""",
                     TableGenerator.From(rows).Column("a").Column("b").ToJson());
    }

    [Fact]
    public void Typed_object_read_by_key_with_any_case()
        => Assert.Equal("""[{"vmid":100,"Node":"pve1"},{"vmid":1005,"Node":"pve2"}]""",
                        TableGenerator.From(Snaps).Column("vmid").Column("Node").ToJson());

    [Fact]
    public void Several_columns_by_key_at_once()
    {
        Assert.Equal(["Node", "VmId", "size"],
                     TableGenerator.From(Snaps).Columns("Node", "VmId").Column(a => a.Size).Title("size").Build().Columns);
        Assert.Equal(["Node", "VmId"], TableGenerator.From(Snaps).Column(a => a.Node).Columns("VmId").Build().Columns);
    }

    private sealed class TwoIds
    {
        public int Id { get; init; }
        public int ID { get; init; }
    }

    [Fact]
    public void Key_matching_two_properties_by_case_does_not_throw()
    {
        TwoIds[] items = [new() { Id = 1, ID = 2 }];
        Assert.Equal("""[{"ID":2}]""", TableGenerator.From(items).Column("ID").ToJson());                   // exact case wins
        Assert.Equal("""[{"id":1}]""", TableGenerator.From(items).Column("id").ToJson());                   // first declared
    }

    [Fact]
    public void Columns_found_at_run_time()
    {
        var builder = TableGenerator.From(Snaps);
        foreach (var key in new[] { "Node", "VmId" }) { builder.Column(key); }
        Assert.Equal(["Node", "VmId"], builder.Build().Columns);
    }
}
