/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Metadata;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests;

public class MethodApiParametersTests
{
    private static MethodApi Method(string parameters)
        => new(JToken.Parse($$"""{ "method": "POST", "name": "create", "description": "test", "parameters": {{parameters}} }"""),
               new ClassApi());

    private static ParameterApi Parameter(MethodApi method, string name) => method.Parameters.Single(a => a.Name == name);

    // Shape of POST /cluster/ha/rules in the Proxmox VE apidoc: a common part plus one variant chosen by 'type'.
    private const string HaRules = """
        {
          "allOf": [
            { "additionalProperties": 0, "properties": { "rule": { "type": "string", "optional": 0 } } },
            {
              "type-property": "type",
              "type-property-schema": { "type": "string", "enum": [ "node-affinity", "resource-affinity" ] },
              "oneOf": [
                {
                  "instance-type": "node-affinity",
                  "properties": {
                    "nodes": { "type": "string", "optional": 0 },
                    "resources": { "type": "string", "optional": 0 },
                    "strict": { "type": "boolean", "optional": 1 },
                    "comment": { "type": "string", "optional": 1 }
                  }
                },
                {
                  "instance-type": "resource-affinity",
                  "properties": {
                    "affinity": { "type": "string", "optional": 0, "enum": [ "positive", "negative" ] },
                    "resources": { "type": "string", "optional": 0 },
                    "comment": { "type": "string", "optional": 1 }
                  }
                }
              ]
            }
          ]
        }
        """;

    [Fact]
    public void AllOf_and_oneOf_parameters_are_read()
        => Assert.Equal(["affinity", "comment", "nodes", "resources", "rule", "strict", "type"],
                        Method(HaRules).Parameters.Select(a => a.Name).Order());

    [Theory]
    [InlineData("rule", false)]      // common part, required
    [InlineData("type", false)]      // chooses the variant
    [InlineData("resources", false)] // required in every variant
    [InlineData("nodes", true)]      // required in one variant only
    [InlineData("affinity", true)]   // required in one variant only
    [InlineData("strict", true)]
    [InlineData("comment", true)]
    public void Required_only_if_required_in_every_variant(string name, bool optional)
        => Assert.Equal(optional, Parameter(Method(HaRules), name).Optional);

    [Fact]
    public void Type_property_has_its_allowed_values()
        => Assert.Equal(["node-affinity", "resource-affinity"], Parameter(Method(HaRules), "type").EnumValues);

    [Fact]
    public void Variant_enum_is_kept()
        => Assert.Equal(["positive", "negative"], Parameter(Method(HaRules), "affinity").EnumValues);

    [Fact]
    public void Plain_properties_are_unchanged()
    {
        var method = Method("""
            { "properties": { "vmid": { "type": "integer", "optional": 0 }, "name": { "type": "string", "optional": 1 } } }
            """);
        Assert.Equal(["vmid", "name"], method.Parameters.Select(a => a.Name));
        Assert.False(Parameter(method, "vmid").Optional);
        Assert.True(Parameter(method, "name").Optional);
    }

    [Fact]
    public void No_parameters()
        => Assert.Empty(new MethodApi(JToken.Parse("""{ "method": "GET", "name": "index", "description": "test" }"""),
                                      new ClassApi()).Parameters);
}
