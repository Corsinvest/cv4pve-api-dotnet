/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Extension.Utils;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests;

public class VmJollyTests
{
    private static ClusterResource Vm(long vmId, string name, string node, string tags = "", string type = "qemu")
        => new() { Id = $"{type}/{vmId}", VmId = vmId, Name = name, Node = node, Tags = tags, Type = type };

    private static readonly ClusterResource[] Vms =
    [
        Vm(100, "web-01", "pve1", "prod;web"),
        Vm(101, "web-02", "pve1", "prod;web"),
        Vm(102, "db-01", "pve2", "prod"),
        Vm(150, "test-01", "pve2", "test"),
        Vm(200, "ct-01", "pve2", "", "lxc"),
        Vm(1006, "app-01", "pve3"),
        Vm(1007, "app-02", "pve3"),
    ];

    /// <summary>Pool "prod" holds 100 and 102, its nested pool "prod/db" holds 200.</summary>
    private static Task<IEnumerable<string>> Pools(string name)
        => Task.FromResult<IEnumerable<string>>(name.ToLower() switch
        {
            "prod" => ["qemu/100", "qemu/102", "lxc/200"],
            "prod/db" => ["lxc/200"],
            _ => [],
        });

    private static async Task<long[]> Select(string jolly)
        => [.. (await VmHelper.GetVmsFromJollyAsync(Vms, jolly, Pools)).Select(a => a.VmId).Order()];

    [Theory]
    [InlineData("@all", new long[] { 100, 101, 102, 150, 200, 1006, 1007 })]
    [InlineData("all", new long[] { 100, 101, 102, 150, 200, 1006, 1007 })]
    [InlineData("100,web-02", new long[] { 100, 101 })]
    [InlineData("100:150", new long[] { 100, 101, 102, 150 })]
    [InlineData("%app%", new long[] { 1006, 1007 })]
    [InlineData("@node-PVE1", new long[] { 100, 101 })]
    [InlineData("@all-pve3", new long[] { 1006, 1007 })]
    [InlineData("all-pve3", new long[] { 1006, 1007 })]
    [InlineData("@tag-WEB", new long[] { 100, 101 })]
    [InlineData("@pool-prod", new long[] { 100, 102, 200 })]
    [InlineData("@pool-PROD", new long[] { 100, 102, 200 })]
    public async Task Selects(string jolly, long[] expected)
        => Assert.Equal(expected, await Select(jolly));

    [Theory]
    [InlineData("@all,-100", new long[] { 101, 102, 150, 200, 1006, 1007 })]
    [InlineData("-100,@all", new long[] { 101, 102, 150, 200, 1006, 1007 })]
    [InlineData("@all,-@tag-prod", new long[] { 150, 200, 1006, 1007 })]
    [InlineData("@all,-@node-pve2,-web-01", new long[] { 101, 1006, 1007 })]
    [InlineData("100:199,-150", new long[] { 100, 101, 102 })]
    public async Task Excludes(string jolly, long[] expected)
        => Assert.Equal(expected, await Select(jolly));

    [Theory]
    [InlineData("1006,-150:200", new long[] { 1006 })]
    [InlineData("@node-pve3,-150:200", new long[] { 1006, 1007 })]
    [InlineData("1007,-1006:1006", new long[] { 1007 })]
    public async Task Excluded_range_only_removes(string jolly, long[] expected)
        => Assert.Equal(expected, await Select(jolly));

    [Theory]
    [InlineData("@all,-@pool-prod", new long[] { 101, 150, 1006, 1007 })]
    [InlineData("@pool-prod,-100", new long[] { 102, 200 })]
    [InlineData("@pool-prod,-@tag-prod", new long[] { 200 })]
    [InlineData("@pool-prod,100,102", new long[] { 100, 102, 200 })]
    public async Task Pool_combines_with_exclusions_and_duplicates(string jolly, long[] expected)
        => Assert.Equal(expected, await Select(jolly));

    [Fact]
    public async Task Each_guest_once()
        => Assert.Equal(new long[] { 100, 101 }, await Select("100,web-01,@tag-web,100:101"));

    [Theory]
    [InlineData("999")]
    [InlineData("@pool-missing")]
    [InlineData("@tag-missing")]
    public async Task Nothing_matches(string jolly)
        => Assert.Empty(await Select(jolly));
}
