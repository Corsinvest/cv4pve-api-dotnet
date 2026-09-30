/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Shared;

public class TableGeneratorTests
{
    private static readonly string NL = Environment.NewLine;

    private static TableGenerator Nodes()
        => new TableGenerator("n", "vm").AddRow("pve1", 100).AddRow("pve22", 1005);

    private static string Lines(params string[] lines) => string.Concat(lines.Select(a => a + NL));

    [Fact]
    public void Text_pads_and_aligns_numbers_right_with_their_header()
        => Assert.Equal(Lines("+-------+------+",
                              "| n     |   vm |",
                              "+-------+------+",
                              "| pve1  |  100 |",
                              "| pve22 | 1005 |",
                              "+-------+------+"),
                        Nodes().ToText());

    [Fact]
    public void Markdown_marks_right_aligned_columns()
        => Assert.Equal(Lines("| n     |   vm |",
                              "|-------|-----:|",
                              "| pve1  |  100 |",
                              "| pve22 | 1005 |"),
                        Nodes().ToMarkdown());

    [Fact]
    public void Html_with_the_old_style_and_right_alignment()
        => Assert.Equal("<table style='width: 100%;border-collapse: collapse;border: 1px solid black;'>"
                        + "<thead><tr><th style='border: 1px solid black;'>n</th>"
                        + "<th style='border: 1px solid black;text-align: right;'>vm</th></tr></thead>"
                        + "<tbody><tr><td style='border: 1px solid black;'>pve1</td>"
                        + "<td style='border: 1px solid black;text-align: right;'>100</td></tr>"
                        + "<tr><td style='border: 1px solid black;'>pve22</td>"
                        + "<td style='border: 1px solid black;text-align: right;'>1005</td></tr></tbody></table>",
                        Nodes().ToHtml());

    [Fact]
    public void Json_keeps_value_types()
    {
        Assert.Equal("""[{"n":"pve1","vm":100},{"n":"pve22","vm":1005}]""", Nodes().ToJson());
        Assert.Equal(JsonConvert.SerializeObject(new[]
                     {
                         new Dictionary<string, object> { ["n"] = "pve1", ["vm"] = 100 },
                         new Dictionary<string, object> { ["n"] = "pve22", ["vm"] = 1005 },
                     }, Formatting.Indented),
                     Nodes().ToJson(true));
    }

    [Fact]
    public void To_picks_the_format()
    {
        Assert.Equal(Nodes().ToText(), Nodes().To(TableGenerator.Output.Text));
        Assert.Equal(Nodes().ToMarkdown(), Nodes().To(TableGenerator.Output.Markdown));
        Assert.Equal(Nodes().ToHtml(), Nodes().To(TableGenerator.Output.Html));
        Assert.Equal(Nodes().ToJson(), Nodes().To(TableGenerator.Output.Json));
        Assert.Equal(Nodes().ToJson(true), Nodes().To(TableGenerator.Output.JsonPretty));
        Assert.Throws<ArgumentOutOfRangeException>(() => Nodes().To((TableGenerator.Output)99));
    }

    [Fact]
    public void No_rows_prints_the_header()
        => Assert.Equal(Lines("+---+----+", "| a | bb |", "+---+----+", "+---+----+"),
                        new TableGenerator("a", "bb").ToText());

    [Fact]
    public void Null_is_an_empty_cell_and_a_null_column_is_left()
    {
        var table = new TableGenerator("k", "v").AddRow("a", null);
        Assert.Equal(Lines("+---+---+", "| k | v |", "+---+---+", "| a |   |", "+---+---+"), table.ToText());
        Assert.Equal("""[{"k":"a","v":null}]""", table.ToJson());
    }

    [Fact]
    public void Text_cell_on_more_lines()
        => Assert.Equal(Lines("+---+-------+",
                              "| k | v     |",
                              "+---+-------+",
                              "| a | l1    |",
                              "|   | line2 |",
                              "+---+-------+"),
                        new TableGenerator("k", "v").AddRow("a", "l1\r\nline2").ToText());

    [Fact]
    public void Html_escapes_values_and_turns_newlines_into_br()
        => Assert.Contains("<td style='border: 1px solid black;'>&lt;b&gt;&amp;</td>"
                           + "<td style='border: 1px solid black;'>a<br>b</td>",
                           new TableGenerator("k", "v").AddRow("<b>&", "a\r\nb").ToHtml());

    [Fact]
    public void Markdown_escapes_pipes_and_turns_newlines_into_br()
        => Assert.Equal(Lines("| k    | v      |",
                              "|------|--------|",
                              "| a\\|b | x<br>y |"),
                        new TableGenerator("k", "v").AddRow("a|b", "x\ny").ToMarkdown());

    [Fact]
    public void Markdown_escapes_titles_too()
        => Assert.Equal(Lines("| a\\|b | c<br>d |",
                              "|------|--------|",
                              "| x    | y      |"),
                        new TableGenerator("a|b", "c\nd").AddRow("x", "y").ToMarkdown());

    [Fact]
    public void Text_title_on_more_lines()
        => Assert.Equal(Lines("+-------+-----+",
                              "| VM    | ram |",
                              "| NAME  |     |",
                              "+-------+-----+",
                              "| web01 |   4 |",
                              "+-------+-----+"),
                        new TableGenerator("VM\nNAME", "ram").AddRow("web01", 4).ToText());

    [Fact]
    public void Alignment_from_values()
    {
        // the separator line of a one-column Markdown table ends with ":|" when the column is right aligned
        static string Separator(TableGenerator table) => table.ToMarkdown().Split(Environment.NewLine)[1];

        Assert.EndsWith(":|", Separator(new TableGenerator("a").AddRow(1L).AddRow(2.5d)));   // long and double
        Assert.EndsWith("-|", Separator(new TableGenerator("a").AddRow(1).AddRow("x")));    // mixed
        Assert.EndsWith("-|", Separator(new TableGenerator("a").AddRow("100")));            // number as string
        Assert.EndsWith("-|", Separator(new TableGenerator("a").AddRow(true)));
        Assert.EndsWith("-|", Separator(new TableGenerator("a").AddRow(DayOfWeek.Monday)));
        Assert.EndsWith("-|", Separator(new TableGenerator("a").AddRow(null)));             // all null
        Assert.EndsWith(":|", Separator(new TableGenerator("a").AddRow(1).AddRow(null)));
        Assert.EndsWith("-|", Separator(new TableGenerator().AddColumn("a", TableGenerator.Align.Left).AddRow(1)));
        Assert.EndsWith(":|", Separator(new TableGenerator().AddColumn("a", TableGenerator.Align.Right).AddRow("x")));
    }

    [Fact]
    public void No_columns()
    {
        var table = new TableGenerator();
        Assert.Equal(string.Empty, table.ToText());
        Assert.Equal(string.Empty, table.ToMarkdown());
        Assert.Equal(string.Empty, table.ToHtml());
        Assert.Equal("[]", table.ToJson());
    }

    [Fact]
    public void Errors_say_what_is_wrong()
    {
        Assert.Equal("Row has 1 values, the table has 2 columns.",
                     Assert.Throws<ArgumentException>(() => new TableGenerator("a", "b").AddRow(1)).Message);
        Assert.Equal("Column 'a' is already defined.",
                     Assert.Throws<ArgumentException>(() => new TableGenerator("a").AddColumn("a")).Message);
        Assert.Equal("Columns cannot be added after rows.",
                     Assert.Throws<ArgumentException>(() => new TableGenerator("a").AddRow(1).AddColumn("b")).Message);
    }

    [Fact]
    public void Columns_and_rows_are_readable()
    {
        var table = new TableGenerator("a", "b").AddRows([["x", 1], ["y", 2]]);
        Assert.Equal(["a", "b"], table.Columns);
        Assert.Equal(2, table.Rows.Count);
        Assert.Equal(["y", 2], table.Rows[1]);
    }
}
