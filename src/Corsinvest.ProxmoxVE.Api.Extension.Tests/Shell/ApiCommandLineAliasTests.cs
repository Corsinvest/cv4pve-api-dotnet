/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Net;
using Corsinvest.ProxmoxVE.Api.Extension.Shell;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Shell;

public class ApiCommandLineAliasTests
{
    private static readonly ApiAlias Start = new("do start vm", "Start a VM", "create /nodes/{node}/qemu/{vmid}/status/start");
    private static readonly ApiAlias Status = new("get vm status", "", "get /nodes/{node}/qemu/{vmid}/status/current");
    private static readonly ApiAlias GuestStatus = new("get guest status", "", "get /nodes/{node}/{vmtype}/{vmid}/status/current");
    private static readonly ApiAlias Snapshot = new("create vm snapshot", "",
                                                    "create /nodes/{node}/qemu/{vmid}/snapshot --snapname {snapname} --description {descr}");
    private static readonly ApiAlias DeleteSnapshot = new("delete vm snapshot", "", "delete /nodes/{node}/qemu/{vmid}/snapshot/{snapname}", true);

    private static Task<ApiCommandResult> Expand(ApiAlias alias, params string[] tokens) => ApiCommandLine.ExpandAliasAsync(alias, tokens);

    [Fact]
    public async Task Arguments_fill_the_placeholders()
    {
        var result = await Expand(Start, "pve01", "100");
        Assert.Null(result.Error);
        Assert.Equal(MethodType.Create, result.Command!.Method);
        Assert.Equal("/nodes/pve01/qemu/100/status/start", result.Command.Resource);
        Assert.Empty(result.Command.Parameters);
    }

    [Fact]
    public async Task Placeholders_in_alias_parameters_become_parameters_and_keep_spaces()
    {
        var result = await Expand(Snapshot, "pve01", "100", "before-update", "Before the update");
        Assert.Equal("/nodes/pve01/qemu/100/snapshot", result.Command!.Resource);
        Assert.Equal("before-update", result.Command.Parameters["snapname"]);
        Assert.Equal("Before the update", result.Command.Parameters["description"]);
    }

    [Fact]
    public async Task User_parameters_are_added_and_a_value_equal_to_an_argument_is_kept()
    {
        var result = await Expand(Status, "pve01", "1012", "--foo", "1012");
        Assert.Equal("/nodes/pve01/qemu/1012/status/current", result.Command!.Resource);
        Assert.Equal("1012", result.Command.Parameters["foo"]);
    }

    [Fact]
    public async Task Missing_arguments_are_listed()
        => Assert.Equal(new ApiCommandResult(null, ApiCommandError.MissingArguments, "{vmid}"), await Expand(Status, "pve01"));

    [Fact]
    public async Task Extra_argument_is_an_error()
        => Assert.Equal(new ApiCommandResult(null, ApiCommandError.UnexpectedArgument, "extra"), await Expand(Status, "pve01", "100", "extra"));

    [Fact]
    public async Task Alias_needing_confirmation_stops_without_yes()
        => Assert.Equal(new ApiCommandResult(null, ApiCommandError.ConfirmationRequired, "delete vm snapshot"),
                        await Expand(DeleteSnapshot, "pve01", "100", "s1"));

    [Theory]
    [InlineData("--yes")]
    [InlineData("-y")]
    public async Task Yes_confirms_and_is_not_sent(string yes)
    {
        var result = await Expand(DeleteSnapshot, "pve01", "100", "s1", yes);
        Assert.Equal("/nodes/pve01/qemu/100/snapshot/s1", result.Command!.Resource);
        Assert.Empty(result.Command.Parameters);
    }

    [Fact]
    public async Task Parameter_repeated_by_the_user_is_an_error()
        => Assert.Equal(ApiCommandError.InvalidParameter, (await Expand(Status, "pve01", "100", "--a", "1", "--a", "2")).Error);

    [Fact]
    public async Task Parameter_written_in_the_alias_and_by_the_user_is_an_error()
    {
        var result = await Expand(Snapshot, "pve01", "100", "s1", "d", "--snapname", "x");
        Assert.Equal(ApiCommandError.InvalidParameter, result.Error);
        Assert.Contains("snapname", result.Detail);
    }

    [Fact]
    public async Task Guest_without_a_value_is_an_error()
        => Assert.Equal(ApiCommandError.InvalidParameter, (await Expand(Status, "--guest")).Error);

    private static PveClient ClusterWith(string resourcesJson)
        => FakeHandler.Client(new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK, resourcesJson)));

    private const string Resources = """
        {"data":[
          {"id":"qemu/100","type":"qemu","vmid":100,"name":"web01","node":"pve02","status":"running"},
          {"id":"lxc/200","type":"lxc","vmid":200,"name":"db01","node":"pve01","status":"running"}
        ]}
        """;

    [Theory]
    [InlineData("100")]
    [InlineData("WEB01")]
    public async Task Guest_by_id_or_name_fills_node_and_vmid(string guest)
    {
        var result = await ApiCommandLine.ExpandAliasAsync(Status, ["--guest", guest], ClusterWith(Resources));
        Assert.Equal("/nodes/pve02/qemu/100/status/current", result.Command!.Resource);
    }

    [Fact]
    public async Task Guest_fills_the_type_for_guest_aliases()
    {
        var result = await ApiCommandLine.ExpandAliasAsync(GuestStatus, ["-g", "db01"], ClusterWith(Resources));
        Assert.Equal("/nodes/pve01/lxc/200/status/current", result.Command!.Resource);
    }

    [Fact]
    public async Task Vm_alias_does_not_find_a_container()
        => Assert.Equal(new ApiCommandResult(null, ApiCommandError.GuestNotFound, "db01"),
                        await ApiCommandLine.ExpandAliasAsync(Status, ["--guest", "db01"], ClusterWith(Resources)));

    [Fact]
    public async Task Guest_on_an_alias_without_guest_placeholders_leaves_them_to_fill()
    {
        var alias = new ApiAlias("get node status", "", "get /nodes/{node}/status");
        var result = await ApiCommandLine.ExpandAliasAsync(alias, ["--guest", "100"], ClusterWith(Resources));
        Assert.Equal("/nodes/pve02/status", result.Command!.Resource);
    }

    [Fact]
    public async Task Guest_needs_a_client()
        => await Assert.ThrowsAsync<ArgumentNullException>(() => ApiCommandLine.ExpandAliasAsync(Status, ["--guest", "100"]));

    [Fact]
    public async Task Quoted_value_in_the_alias_command_stays_one_value()
    {
        var alias = new ApiAlias("snap", "", "create /nodes/{node}/qemu/{vmid}/snapshot --snapname {name} --description 'Before update'");
        var result = await Expand(alias, "pve01", "100", "s1");
        Assert.Equal("Before update", result.Command!.Parameters["description"]);
    }

    [Fact]
    public async Task Stray_value_in_the_alias_command_is_an_error()
    {
        var alias = new ApiAlias("bad", "", "get /nodes/{node}/status extra");
        Assert.Equal(ApiCommandError.InvalidParameter, (await Expand(alias, "pve01")).Error);
    }

    [Theory]
    [InlineData("100/../../stopall")]
    [InlineData("..")]
    [InlineData("100?x=1")]
    [InlineData("100#x")]
    [InlineData("100%2F")]
    public async Task Value_in_the_path_cannot_change_the_path(string vmid)
    {
        var result = await Expand(Status, "pve01", vmid);

        Assert.Null(result.Command);
        Assert.Equal(ApiCommandError.InvalidParameter, result.Error);
        Assert.Contains(vmid, result.Detail);
    }

    [Fact]
    public async Task Value_of_a_parameter_can_have_a_slash()
    {
        var alias = new ApiAlias("create vm snapshot", "", "create /nodes/{node}/qemu/{vmid}/snapshot --snapname {snapname}");
        var result = await Expand(alias, "pve01", "100", "a/b");

        Assert.Equal("a/b", result.Command!.Parameters["snapname"]);
    }

    [Fact]
    public async Task Guest_lookup_stops_when_cancelled()
    {
        var handler = new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK, """{"data":[]}"""));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => ApiCommandLine.ExpandAliasAsync(Status, ["--guest", "100"], FakeHandler.Client(handler), new CancellationToken(true)));
        Assert.Empty(handler.Requests);
    }
}
