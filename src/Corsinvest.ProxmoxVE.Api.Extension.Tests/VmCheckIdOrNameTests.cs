/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Extension.Utils;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests;

public class VmCheckIdOrNameTests
{
    private static ClusterResource Vm(long vmId, string name) => new() { VmId = vmId, Name = name };

    [Theory]
    [InlineData("%web%", "my-web-01", true)]
    [InlineData("%WEB%", "my-web-01", true)]
    [InlineData("%web%", "db-01", false)]
    [InlineData("web%", "web-01", true)]
    [InlineData("web%", "my-web", false)]
    [InlineData("%web", "my-web", true)]
    [InlineData("%web", "web-01", false)]
    public void Wildcard_matches_contains_starts_and_ends(string pattern, string name, bool expected)
        => Assert.Equal(expected, VmHelper.CheckIdOrName(Vm(100, name), pattern));

    [Theory]
    [InlineData("web-01", true)]
    [InlineData("WEB-01", true)]
    [InlineData("web", false)]
    public void Name_is_exact_and_ignores_case(string pattern, bool expected)
        => Assert.Equal(expected, VmHelper.CheckIdOrName(Vm(100, "web-01"), pattern));

    [Theory]
    [InlineData("105", true)]
    [InlineData("100:110", true)]
    [InlineData("105:105", true)]
    [InlineData("106:110", false)]
    [InlineData("100:abc", false)]
    public void Id_and_range_include_both_ends(string pattern, bool expected)
        => Assert.Equal(expected, VmHelper.CheckIdOrName(Vm(105, "web-01"), pattern));
}
