/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.CommandLine;
using System.Diagnostics;
using System.Net;
using Corsinvest.ProxmoxVE.Api.Console.Helpers;
using Corsinvest.ProxmoxVE.Api.Extension.Scheduling;
using Corsinvest.ProxmoxVE.Api.Extension.Shell;
using Corsinvest.ProxmoxVE.Api.Extension.Utils;
using Corsinvest.ProxmoxVE.Api.Shared;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Node;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Console;

/// <summary>
/// What needs a keyboard, a browser or a terminal, done through the internal hooks of the helpers,
/// and the last branches of the parsers.
/// </summary>
[Collection("Console")]
public sealed class LastGapsTests : IDisposable
{
    private const string Key = "012345678901234567890123";

    private readonly string _folder = Path.Combine(Path.GetTempPath(), "cv4pve-tests-" + Guid.NewGuid().ToString("N"));
    private readonly TextWriter _out = System.Console.Out;
    private readonly StringWriter _console = new();
    private readonly Func<ConsoleKeyInfo> _readKey = ConsoleHelper.ReadKey;
    private readonly Action<ProcessStartInfo> _startProcess = MiscHelper.StartProcess;
    private readonly string _cacheFilePath = UpdateHelper.CacheFilePath;

    public LastGapsTests()
    {
        Directory.CreateDirectory(_folder);
        System.Console.SetOut(_console);
    }

    public void Dispose()
    {
        System.Console.SetOut(_out);
        ConsoleHelper.ReadKey = _readKey;
        MiscHelper.StartProcess = _startProcess;
        UpdateHelper.CacheFilePath = _cacheFilePath;
        UpdateHelper.CreateHandler = null;
        UpdateHelper.CurrentVersion = ConsoleHelper.GetCurrentVersionApp;
        UpdateHelper.IsOutputRedirected = () => System.Console.IsOutputRedirected;
        CommandOptionExtension.CommandLineArgs = Environment.GetCommandLineArgs;
        Directory.Delete(_folder, true);
    }

    // the keys a person presses, then Enter
    private static void Type(params ConsoleKeyInfo[] keys)
    {
        var queue = new Queue<ConsoleKeyInfo>([.. keys, new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false)]);
        ConsoleHelper.ReadKey = queue.Dequeue;
    }

    private static ConsoleKeyInfo Char(char c) => new(c, char.IsLetter(c) ? Enum.Parse<ConsoleKey>(char.ToUpperInvariant(c).ToString()) : ConsoleKey.Oem1, char.IsUpper(c), false, false);
    private static readonly ConsoleKeyInfo Backspace = new('\b', ConsoleKey.Backspace, false, false, false);
    private static readonly ConsoleKeyInfo Left = new('\0', ConsoleKey.LeftArrow, false, false, false);
    private static readonly ConsoleKeyInfo Tab = new('\t', ConsoleKey.Tab, false, false, false);

    [Fact]
    public void Password_is_typed_without_being_shown()
    {
        Type(Char('s'), Char('e'), Char('x'), Backspace, Char('c'), Left, Tab, Char('!'), Char('R'));

        Assert.Equal("sec!R", ConsoleHelper.ReadPassword());
        Assert.Equal("***\b \b***", _console.ToString());
    }

    [Fact]
    public void Password_can_be_empty_and_backspace_on_nothing_does_nothing()
    {
        Type(Backspace, Backspace);

        Assert.Equal(string.Empty, ConsoleHelper.ReadPassword());
        Assert.Equal(string.Empty, _console.ToString());
    }

    [Fact]
    public void Password_file_that_does_not_exist_is_written_with_the_password_typed()
    {
        var fileName = Path.Combine(_folder, "password.txt");
        CommandOptionExtension.CommandLineArgs = () => ["cv4pve-tool", "--password", "file:" + fileName];
        var app = ConsoleHelper.CreateApp("Test tool");
        Type(Char('p'), Char('w'));

        Assert.Equal("pw", app.GetPasswordFromOption());

        Assert.StartsWith("Password:", _console.ToString());
        var stored = File.ReadAllText(fileName);
        Assert.DoesNotContain("pw", stored);
        Assert.Equal("pw", StringHelper.Decrypt(stored, Key));

        // the next run reads the file and asks nothing
        ConsoleHelper.ReadKey = () => throw new InvalidOperationException("no question expected");
        Assert.Equal("pw", app.GetPasswordFromOption());
    }

    [Fact]
    public void Browser_is_opened_with_the_program_of_the_system()
    {
        ProcessStartInfo? started = null;
        MiscHelper.StartProcess = a => started = a;

        MiscHelper.OpenBrowser("https://pve01:8006/?console=kvm&vmid=100");

        Assert.NotNull(started);
        if (OperatingSystem.IsWindows())
        {
            Assert.Equal("https://pve01:8006/?console=kvm&vmid=100", started.FileName);
            Assert.True(started.UseShellExecute);
        }
        else
        {
            Assert.Equal(OperatingSystem.IsMacOS() ? "open" : "xdg-open", started.FileName);
            Assert.Equal("https://pve01:8006/?console=kvm&vmid=100", started.Arguments);
        }
    }

    [Fact]
    public void Data_folder_of_a_tool_is_created_in_the_profile_of_the_user()
    {
        var appName = "cv4pve-tests-" + Guid.NewGuid().ToString("N");
        var parent = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Corsinvest");
        var parentExisted = Directory.Exists(parent);
        try
        {
            var path = CommonHelper.GetApplicationDataDirectory(appName);

            Assert.Equal(Path.Combine(parent, appName), path);
            Assert.True(Directory.Exists(path));
            Assert.Equal(path, CommonHelper.GetApplicationDataDirectory(appName));
        }
        finally
        {
            Directory.Delete(Path.Combine(parent, appName));
            if (!parentExisted) { Directory.Delete(parent); }
        }
    }

    [Fact]
    public async Task Notice_of_a_new_version_is_written_only_on_a_terminal()
    {
        UpdateHelper.CacheFilePath = Path.Combine(_folder, "update-check.json");
        UpdateHelper.CreateHandler = () => new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK, """[{"tag_name":"v9.9.9","prerelease":false}]"""));
        UpdateHelper.CurrentVersion = () => "1.0.0";

        async Task PrintAsync()
        {
            var (task, cts) = UpdateHelper.StartCheck("cv4pve-tool");
            await task.WaitAsync(TimeSpan.FromSeconds(10));
            UpdateHelper.PrintIfNewVersion("cv4pve-tool", task, cts);
        }

        UpdateHelper.IsOutputRedirected = () => true;
        await PrintAsync();
        Assert.Equal(string.Empty, _console.ToString());

        UpdateHelper.IsOutputRedirected = () => false;
        await PrintAsync();
        Assert.Equal(Environment.NewLine + "*** New version available: 9.9.9 (current: 1.0.0) ***" + Environment.NewLine, _console.ToString());

        // an error while the notice is prepared never stops the tool
        UpdateHelper.IsOutputRedirected = () => throw new IOException("no console");
        await PrintAsync();
    }

    [Fact]
    public async Task Update_check_that_cannot_write_its_cache_does_not_stop_the_tool()
    {
        // the folder of the cache cannot be created: a file has its name
        var blocker = Path.Combine(_folder, "blocker");
        File.WriteAllText(blocker, "file");
        UpdateHelper.CacheFilePath = Path.Combine(blocker, "update-check.json");
        UpdateHelper.CreateHandler = () => new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK, "[]"));
        UpdateHelper.CurrentVersion = () => "1.0.0";

        var (task, cts) = UpdateHelper.StartCheck("cv4pve-tool");
        await task.WaitAsync(TimeSpan.FromSeconds(10));
        UpdateHelper.PrintIfNewVersion("cv4pve-tool", task, cts);

        Assert.Null(UpdateHelper.GetNotice("cv4pve-tool"));
    }

    [Fact]
    public void Single_options_can_be_added_to_a_command()
    {
        var command = new Command("test");

        Assert.True(command.HostOption().Required);
        Assert.Equal("--username", command.UsernameOption().Name);
        Assert.Equal("--vmid", command.VmIdOption().Name);
        Assert.Equal(["--host", "--username", "--vmid"], command.Options.Select(a => a.Name));
        Assert.Equal(100, command.Parse(["--host", "pve01", "--vmid", "100"]).GetValue(command.GetOption<int>("--vmid")));
    }

    [Theory]
    [InlineData("mon")]
    [InlineData("mon..fri")]
    [InlineData("01-15")]
    [InlineData("2026-01-15")]
    [InlineData("mon,xyz 10:00")]
    [InlineData("fri..mon 10:00")]
    [InlineData("1-2-3-4 10:00")]
    [InlineData("10..5:00")]
    [InlineData("0..30:00")]
    [InlineData("10:50..70")]
    [InlineData("utc")]
    public void Schedule_that_is_not_complete_or_not_valid_is_refused(string input)
    {
        var ex = Assert.Throws<PveCalendarParseException>(() => PveCalendarEvent.Parse(input));

        Assert.Contains(input, ex.Message);
    }

    [Fact]
    public void Schedule_with_stars_and_ranges_and_one_that_never_happens()
    {
        var every = PveCalendarEvent.Parse("*:10..12");
        Assert.Equal(24, every.Hours.Count);
        Assert.Equal([10, 11, 12], every.Minutes.Order());

        // the 30th of February
        Assert.Null(PveCalendarEvent.Parse("02-30 00:00").NextOccurrence(new DateTime(2026, 1, 1)));
    }

    [Theory]
    [InlineData("put /cluster/options", MethodType.Set)]
    [InlineData("set /cluster/options", MethodType.Set)]
    [InlineData("post /cluster/backup", MethodType.Create)]
    [InlineData("create /cluster/backup", MethodType.Create)]
    [InlineData("delete /pools/test", MethodType.Delete)]
    [InlineData("GET /version", MethodType.Get)]
    public async Task Alias_can_name_the_method_as_http_or_as_the_clients_do(string command, MethodType method)
    {
        var result = await ApiCommandLine.ExpandAliasAsync(new ApiAlias("test", "", command), []);

        Assert.Null(result.Error);
        Assert.Equal(method, result.Command!.Method);
        Assert.Equal(command.Split(' ')[1], result.Command.Resource);
    }

    [Theory]
    [InlineData("get")]
    [InlineData("list /nodes")]
    [InlineData("/nodes get")]
    public async Task Alias_without_a_method_and_a_path_is_refused(string command)
    {
        var result = await ApiCommandLine.ExpandAliasAsync(new ApiAlias("broken", "", command), []);

        Assert.Equal(ApiCommandError.InvalidParameter, result.Error);
        Assert.Equal($"Alias 'broken' has no valid method and path: '{command}'.", result.Detail);
    }

    [Theory]
    [InlineData("web%01", false)]
    [InlineData("w%b-01", false)]
    [InlineData("%web%", true)]
    public void Name_with_a_wildcard_in_the_middle_matches_nothing(string pattern, bool expected)
        => Assert.Equal(expected, VmHelper.CheckIdOrName(new Corsinvest.ProxmoxVE.Api.Shared.Models.Cluster.ClusterResource { VmId = 100, Name = "web-01", Type = "qemu" }, pattern));

    [Fact]
    public async Task Search_of_a_reachable_host_stops_when_the_caller_cancels()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        // the cancellation is reported as the reason why no host was found
        var ex = await Assert.ThrowsAsync<PveException>(
            () => ClientHelper.FindFirstReachableHostAsync([new ClientHelper.HostEndpoint("127.0.0.1", 8006)], 2000, cancellation.Token));

        Assert.StartsWith("No reachable hosts found. Errors: Error testing host 127.0.0.1:8006: ", ex.Message);
        Assert.Null(await ClientHelper.FindFirstReachableHostAsync([], 2000));
    }

    [Fact]
    public void Bridge_without_ports_and_without_a_gateway_is_drawn_too()
    {
        var svg = NetworkDiagramBuilder.BuildSvg(
            [
                new("pve01", new NodeNetwork { Interface = "eth0", Type = "eth", Active = true }),
                new("pve01", new NodeNetwork { Interface = "vmbr0", Type = "bridge", Active = true, BridgePorts = "eth0" }),
                new("pve01", new NodeNetwork { Interface = "vmbr9", Type = "bridge", Active = true, Comments = "isolated" }),
            ],
            [],
            [new(100, "lab01", "pve01", "qemu", "running", null, new Corsinvest.ProxmoxVE.Api.Shared.Models.Vm.VmNetwork { Id = "net0", Bridge = "vmbr9" })],
            [],
            new("cv4pve-diag", "https://example.com", "1.0.0"));

        Assert.Contains("vmbr9 · isolated", svg);
        Assert.Contains("VM 100 · lab01", svg);
        // a guest of an isolated bridge is not a gateway
        Assert.DoesNotContain("#FF9100\" stroke", svg.Split("Node: pve01")[1]);
    }
}
