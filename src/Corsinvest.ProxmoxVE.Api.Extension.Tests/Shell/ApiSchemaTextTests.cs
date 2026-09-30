/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Extension.Shell;
using Corsinvest.ProxmoxVE.Api.Metadata;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Shell;

public class ApiSchemaTextTests
{
    private static IReadOnlyList<ApiMethodInfo> Methods()
        => ApiSchema.GetMethods(GeneratorClassApi.BuildClassApiFromFlat(GeneratorClassApi.LoadFlatCache("""
            {"/nodes":{"children":[{"name":"{node}","indexed":true}],"methods":{"get":{"returnType":"array"}}},
             "/nodes/{node}":{"keys":["node"],"children":[{"name":"status"}],"methods":{"get":{"returnType":"array"}}},
             "/nodes/{node}/status":{"keys":["node"],"methods":{
                "get":{"comment":"Read node status.","returnType":"object","returnParams":[{"name":"uptime","type":"integer","description":"Seconds."}]},
                "post":{"comment":"Reboot or shutdown a node.",
                        "params":[{"name":"node","type":"string"},
                                  {"name":"command","type":"string","enumValues":["reboot","shutdown"],"description":"Specify the command."},
                                  {"name":"force","type":"boolean","optional":true}]}}}}
            """)!), "/nodes/pve01/status")!;

    private static readonly string NL = Environment.NewLine;

    [Fact]
    public void Usage_lists_each_method_with_its_required_parameters()
        => Assert.Equal($"USAGE: get /nodes/pve01/status{NL}"
                        + $"USAGE: create /nodes/pve01/status --command <string> [OPTIONS]{NL}",
                        ApiSchemaText.Usage("/nodes/pve01/status", Methods(), false, false, TableGenerator.Output.Text));

    [Fact]
    public void Usage_with_the_key_value_style()
        => Assert.Equal($"USAGE: create /nodes/pve01/status command:<string> [OPTIONS]{NL}",
                        ApiSchemaText.Usage("/nodes/pve01/status", [.. Methods().Where(a => a.Method == MethodType.Create)],
                                            false, false, TableGenerator.Output.Text, optionStyle: false));

    [Fact]
    public void Verbose_usage_adds_description_and_parameters()
    {
        var text = ApiSchemaText.Usage("/nodes/pve01/status", [.. Methods().Where(a => a.Method == MethodType.Create)],
                                       true, false, TableGenerator.Output.Json);

        Assert.Equal($"USAGE: create /nodes/pve01/status --command <string> [OPTIONS]{NL}{NL}  Reboot or shutdown a node.{NL}"
                     + """[{"param":"command","type":"reboot,shutdown","description":"Specify the command."},{"param":"force","type":"boolean","description":""}]"""
                     + NL,
                     text);
    }

    [Fact]
    public void Usage_with_returns_adds_the_answer_fields()
    {
        var text = ApiSchemaText.Usage("/nodes/pve01/status", [.. Methods().Where(a => a.Method == MethodType.Get)],
                                       false, true, TableGenerator.Output.Json);

        Assert.Equal($"USAGE: get /nodes/pve01/status{NL}RETURNS:{NL}"
                     + """[{"param":"uptime","type":"integer","description":"Seconds."}]""",
                     text);
    }

    [Fact]
    public void Long_descriptions_wrap()
    {
        var parameters = ApiSchema.GetMethod(GeneratorClassApi.BuildClassApiFromFlat(GeneratorClassApi.LoadFlatCache("""
            {"/version":{"methods":{"put":{"params":[{"name":"note","type":"string","optional":true,
                "description":"A description long enough to be split on more than one line of the table."}]}}}}
            """)!), "/version", MethodType.Set)!.Parameters;

        Assert.Equal([["note", "string", "A description long enough to be split on more"],
                      ["", "", "than one line of the table."]],
                     ApiSchemaText.ParameterRows(parameters));
    }

    [Fact]
    public void List_shows_attributes_and_names()
        => Assert.Equal($"Dr---        pve01{NL}-r--c        status{NL}",
                        ApiSchemaText.List(new([new("pve01", true, false), new("status", false, true)], null)));

    [Fact]
    public void List_ends_with_the_error()
        => Assert.Equal($"-r---        qemu{NL}Permission check failed{NL}",
                        ApiSchemaText.List(new([new("qemu", false, false)], "Permission check failed")));
}
