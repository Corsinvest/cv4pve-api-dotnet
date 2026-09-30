/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Extension.Shell;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Shell;

public class ApiCommandLineParametersTests
{
    private static Dictionary<string, string> Parse(params string[] tokens)
        => ApiCommandLine.ParseParameters(tokens).Parameters.ToDictionary(a => a.Key, a => a.Value);

    [Fact]
    public void Key_value_pairs()
        => Assert.Equal(new Dictionary<string, string> { ["memory"] = "4096", ["cores"] = "2" },
                        Parse("--memory", "4096", "--cores", "2"));

    [Fact]
    public void Key_equals_value_splits_at_the_first_equals()
        => Assert.Equal(new Dictionary<string, string> { ["usb0"] = "spice", ["name"] = "a=b" },
                        Parse("--usb0=spice", "--name=a=b"));

    [Fact]
    public void Flag_in_the_middle_does_not_shift_values()
        => Assert.Equal(new Dictionary<string, string> { ["force"] = "true", ["memory"] = "4096" },
                        Parse("--force", "--memory", "4096"));

    [Fact]
    public void Flag_at_the_end_is_true() => Assert.Equal("true", Parse("--memory", "4096", "--force")["force"]);

    [Theory]
    [InlineData("-1")]
    [InlineData("-")]
    public void Value_starting_with_one_dash_is_a_value(string value) => Assert.Equal(value, Parse("--limit", value)["limit"]);

    [Fact]
    public void Tokens_not_taken_as_values_are_positional()
    {
        var (parameters, positional) = ApiCommandLine.ParseParameters(["pve01", "100", "--timeout", "100"]);
        Assert.Equal(["pve01", "100"], positional);
        Assert.Equal("100", parameters.Single().Value);
    }

    [Fact]
    public void Repeated_key_is_an_error()
    {
        var ex = Assert.Throws<ArgumentException>(() => ApiCommandLine.ParseParameters(["--type", "vm", "--type", "node"]));
        Assert.Equal("Parameter '--type' is given more than once.", ex.Message);
    }

    [Fact]
    public void Placeholders_in_order_including_value_lists()
        => Assert.Equal(["realm", "pam|pve|ldap"],
                        ApiCommandLine.GetPlaceholders("create /access/domains --realm {realm} --type {pam|pve|ldap}"));
}
