/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Console.Helpers;
using Corsinvest.ProxmoxVE.Api.Metadata;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Console;

/// <summary>
/// Commands of the system started by a tool, and the parts of the API schema and of the shared helpers
/// that the other tests do not reach.
/// </summary>
public class ShellAndSchemaTests
{
    [Fact]
    public void Dry_run_starts_nothing_and_debug_shows_what_would_run()
    {
        var output = new StringWriter();
        var variables = new Dictionary<string, string> { ["CV4PVE_PHASE"] = "snap-create-pre", ["CV4PVE_VMID"] = "100" };

        var result = ShellHelper.Execute("command-that-does-not-exist --flag", true, variables, output, dryRun: true, debug: true);

        Assert.Equal((string.Empty, 0), result);
        Assert.Equal(["-------------------------------------------------------",
                      "CV4PVE_PHASE: snap-create-pre",
                      "CV4PVE_VMID: 100",
                      "-------------------------------------------------------",
                      "Run command: command-that-does-not-exist --flag"],
                     output.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));

        output = new StringWriter();
        Assert.Equal((string.Empty, 0), ShellHelper.Execute("command-that-does-not-exist", false, variables, output, dryRun: true, debug: false));
        Assert.Equal((string.Empty, 0), ShellHelper.Execute("command-that-does-not-exist", false, null!, output, dryRun: true, debug: false, waitForExit: true));
        Assert.Equal(string.Empty, output.ToString());
    }

    [Fact]
    public void Command_runs_and_its_output_is_read()
    {
        // a program without arguments that every system of the tests has
        var (standardOutput, exitCode) = ShellHelper.Execute("hostname", true, null!, TextWriter.Null, false, false, true);

        Assert.Equal(0, exitCode);
        Assert.False(string.IsNullOrWhiteSpace(standardOutput));

        Assert.Equal((string.Empty, 0), ShellHelper.Execute("hostname", false, null!, TextWriter.Null, false, false, true));
        Assert.False(string.IsNullOrWhiteSpace(ShellHelper.Execute("hostname", true, null!, TextWriter.Null, false, false).StandardOutput));
    }

    [Fact]
    public void Command_of_a_shell_gets_variables_quotes_and_exit_code()
    {
        // on Windows the command is the program itself, without a shell and without arguments
        if (OperatingSystem.IsWindows()) { return; }

        var variables = new Dictionary<string, string> { ["CV4PVE_VMID"] = "100" };

        Assert.Equal(("vm 100\n", 0), ShellHelper.Execute("echo vm $CV4PVE_VMID", true, variables, TextWriter.Null, false, false, true));
        Assert.Equal(("a  b\n", 0), ShellHelper.Execute("echo \"a  b\"", true, null!, TextWriter.Null, false, false, true));
        Assert.Equal((string.Empty, 3), ShellHelper.Execute("exit 3", false, null!, TextWriter.Null, false, false, true));
        // not waited: the exit code is not known
        Assert.Equal(0, ShellHelper.Execute("exit 3", false, null!, TextWriter.Null, false, false).ExitCode);
    }

    private const string ApiDoc = """
        [{"path":"/vm","text":"vm","leaf":1,"info":{
            "GET":{"method":"GET","name":"read","description":"Read.","parameters":{"additionalProperties":0},
                   "returns":{"type":"array","items":{"type":"object","properties":{
                        "size":{"type":"integer","renderer":"bytes"},
                        "name":{"type":"string"}}}}},
            "POST":{"method":"POST","name":"create","description":"Create.","parameters":{"additionalProperties":0,"properties":{
                        "net":{"type":"string","optional":1,"description":"Network device.",
                               "format":{"properties":{
                                    "model":{"type":"string","enum":["e1000","virtio"],"default_key":1,"description":"Model.","alias":"type",
                                             "format":"pve-model","format_description":"model","minimum":1,"maximum":9,"maxLength":64},
                                    "bridge":{"type":"string","optional":1}}}},
                        "name":{"type":"string","format":"dns-name"}}},
                    "returns":{"type":"null"}},
            "PUT":{"method":"PUT","name":"update","description":"Update.","parameters":{"additionalProperties":0},"returns":{"type":"null"}},
            "DELETE":{"method":"DELETE","name":"delete","description":"Delete.","parameters":{"additionalProperties":0},"returns":{"type":"null"}}}}]
        """;

    private static ClassApi Schema()
    {
        var root = new ClassApi();
        foreach (var token in JArray.Parse(ApiDoc)) { _ = new ClassApi(token, root); }
        return root;
    }

    private static MethodApi Method(ClassApi root, string type) => ClassApi.GetFromResource(root, "/vm")!.Methods.Single(a => a.MethodType == type);

    private static string Describe(ParameterFormatApi a)
        => string.Join("|", a.Name, a.Type, a.Description, a.Optional, a.Minimum, a.Maximum, a.DefaultKey, a.FormatDescription, a.Format, a.Alias, a.MaxLength, string.Join(",", a.EnumValues));

    [Fact]
    public void Properties_of_a_format_are_read_from_the_schema()
    {
        var net = Method(Schema(), "POST").Parameters.Single(a => a.Name == "net");

        Assert.Equal(["model|string|Model.|False|1|9|1|model|pve-model|type|64|e1000,virtio", "bridge|string||True||||||||"],
                     net.Formats.Select(Describe));
        // a format that is only a name has no properties
        Assert.Empty(Method(Schema(), "POST").Parameters.Single(a => a.Name == "name").Formats);
    }

    [Fact]
    public void Properties_of_a_format_survive_the_cache()
    {
        var cached = GeneratorClassApi.BuildClassApiFromFlat(GeneratorClassApi.LoadFlatCache(GeneratorClassApi.BuildFlatCache(Schema()))!);

        Assert.Equal(Method(Schema(), "POST").Parameters.Single(a => a.Name == "net").Formats.Select(Describe),
                     Method(cached, "POST").Parameters.Single(a => a.Name == "net").Formats.Select(Describe));
    }

    [Theory]
    [InlineData("GET", "get")]
    [InlineData("POST", "create")]
    [InlineData("PUT", "set")]
    [InlineData("DELETE", "delete")]
    public void Method_has_the_name_used_by_the_clients(string type, string expected)
        => Assert.Equal(expected, Method(Schema(), type).GetMethodTypeHumanized());

    [Fact]
    public void Value_that_cannot_be_rendered_stays_as_it_is()
    {
        var returns = Method(Schema(), "GET").ReturnParameters;
        var size = returns.Single(a => a.Name == "size");
        var name = returns.Single(a => a.Name == "name");
        var date = new DateTime(2026, 1, 1);

        Assert.Equal("R", size.GetAlignmentValue());
        Assert.Equal("L", name.GetAlignmentValue());
        Assert.Equal(date, size.RendererValue(date));
        Assert.Equal("text", size.RendererValue("text"));
        Assert.Equal("[1,2]", name.RendererValue(new List<int> { 1, 2 }));
        Assert.Equal("""{"a":1}""", name.RendererValue(Newtonsoft.Json.JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>("""{"a":1}""")!));
    }

    [Theory]
    [InlineData("1 MB", 1_000_000)]
    [InlineData("1 TB", 1_000_000_000_000)]
    [InlineData("1 PB", 1_000_000_000_000_000)]
    [InlineData("1 EB", 1_000_000_000_000_000_000)]
    [InlineData("1 PiB", 1_125_899_906_842_624)]
    [InlineData("1 EiB", 1_152_921_504_606_846_976)]
    public void Biggest_units_are_converted_to_bytes(string text, long bytes)
        => Assert.Equal(bytes, ByteHelper.ToBytes(text));

    [Fact]
    public void Names_used_by_the_api_are_constants()
    {
        Assert.Equal("pool", PveConstants.KeyApiPool);
        Assert.Equal("template", PveConstants.KeyApiTemplate);
        Assert.Equal("sdn", PveConstants.KeyApiSdn);
        Assert.Equal("online", PveConstants.StatusOnline);
        Assert.Equal("offline", PveConstants.StatusOffline);
        Assert.Equal("paused", PveConstants.StatusVmPaused);
        Assert.Equal("backup", PveConstants.StorageContentBackup);
    }

    private sealed record Row(string Node, long VmId);

    [Fact]
    public void Table_is_written_in_the_format_asked_from_any_step_of_its_definition()
    {
        Row?[] rows = [new("pve01", 100), null];

        var byKeys = TableGenerator.From(rows).Columns("Node", "VmId");
        Assert.Equal(byKeys.ToHtml(), byKeys.To(TableGenerator.Output.Html));
        Assert.Equal(byKeys.ToMarkdown(), byKeys.To(TableGenerator.Output.Markdown));
        Assert.Contains("pve01", byKeys.ToText());

        var byColumn = TableGenerator.From(rows).Column("Node");
        Assert.Equal(byColumn.ToHtml(), byColumn.To(TableGenerator.Output.Html));
        Assert.Equal(byColumn.ToText(), byColumn.To(TableGenerator.Output.Text));
        Assert.Contains("pve01", byColumn.ToHtml());
    }
}
