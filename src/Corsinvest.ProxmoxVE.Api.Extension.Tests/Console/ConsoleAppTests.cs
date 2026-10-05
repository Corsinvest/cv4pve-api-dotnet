/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.CommandLine;
using System.Net;
using System.Reflection;
using Corsinvest.ProxmoxVE.Api.Console.Helpers;
using Corsinvest.ProxmoxVE.Api.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Console;

/// <summary>
/// What a tool does around its commands: arguments of the process, login, questions, update notice, exit code.
/// The arguments, the console and the cache of the update check are of the whole process: one test at a time.
/// </summary>
[Collection("Console")]
public sealed class ConsoleAppTests : IDisposable
{
    private const string Key = "012345678901234567890123";
    private const string Releases = """[{"tag_name":"v10.0.0-rc1","prerelease":true},{"tag_name":"v9.9.9","prerelease":false},{"tag_name":"v9.9.8","prerelease":false}]""";

    private readonly string _folder = Path.Combine(Path.GetTempPath(), "cv4pve-tests-" + Guid.NewGuid().ToString("N"));
    private readonly TextWriter _out = System.Console.Out;
    private readonly TextReader _in = System.Console.In;
    private readonly string _cacheFilePath = UpdateHelper.CacheFilePath;
    private readonly StringWriter _console = new();
    private readonly FakeHandler _gitHub = new(_ => FakeHandler.Json(HttpStatusCode.OK, Releases));

    public ConsoleAppTests()
    {
        Directory.CreateDirectory(_folder);
        System.Console.SetOut(_console);
        UpdateHelper.CacheFilePath = Path.Combine(_folder, "cache", "update-check.json");
        UpdateHelper.CreateHandler = () => _gitHub;
        UpdateHelper.CurrentVersion = () => "1.0.0";
        Args();
    }

    public void Dispose()
    {
        System.Console.SetOut(_out);
        System.Console.SetIn(_in);
        UpdateHelper.CacheFilePath = _cacheFilePath;
        UpdateHelper.CreateHandler = null;
        UpdateHelper.CurrentVersion = ConsoleHelper.GetCurrentVersionApp;
        CommandOptionExtension.CommandLineArgs = Environment.GetCommandLineArgs;
        Directory.Delete(_folder, true);
    }

    // the arguments of the process, after the name of the tool
    private static void Args(params string[] args) => CommandOptionExtension.CommandLineArgs = () => ["cv4pve-tool", .. args];

    private static RootCommand App() => ConsoleHelper.CreateApp("Test tool");

    private JObject Cache() => JObject.Parse(File.ReadAllText(UpdateHelper.CacheFilePath));

    private void WriteCache(string app, string latestVersion, DateTime lastCheck)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(UpdateHelper.CacheFilePath)!);
        File.WriteAllText(UpdateHelper.CacheFilePath,
                          new JObject { [app] = new JObject { ["LastCheck"] = lastCheck, ["LatestVersion"] = latestVersion } }.ToString());
    }

    private static async Task CheckAsync(string app)
    {
        var (task, cts) = UpdateHelper.StartCheck(app);
        await task.WaitAsync(TimeSpan.FromSeconds(10));
        cts.Dispose();
    }

    [Fact]
    public void Diagnostic_options_are_read_from_the_arguments_of_the_process()
    {
        var app = App();

        Assert.False(app.DebugIsActive());
        Assert.False(app.DryRunIsActive());
        Assert.Equal(LogLevel.Warning, app.GetLogLevelFromDebug());

        Args("--host", "pve01", "--debug", "--dry-run");
        Assert.True(app.DebugIsActive());
        Assert.True(app.DryRunIsActive());
        Assert.Equal(LogLevel.Debug, app.GetLogLevelFromDebug());

        Args("--debug", "--log-level", "Trace");
        Assert.Equal(LogLevel.Trace, app.GetLogLevelFromDebug());

        Args("--log-level", "Error");
        Assert.Equal(LogLevel.Error, app.GetLogLevelFromDebug());
    }

    [Fact]
    public void Password_is_read_from_the_option_or_from_its_file()
    {
        var app = App();
        Assert.Null(app.GetPasswordFromOption());

        Args("--password", " secret ");
        Assert.Equal("secret", app.GetPasswordFromOption());

        var encrypted = Path.Combine(_folder, "password.txt");
        File.WriteAllText(encrypted, StringHelper.Encrypt("from-file", Key));
        Args("--password", "file:" + encrypted);
        Assert.Equal("from-file", app.GetPasswordFromOption());

        // a file written by hand has the password as it is
        var plain = Path.Combine(_folder, "plain.txt");
        File.WriteAllText(plain, "written by hand\n");
        Args("--password", "file:" + plain);
        Assert.Equal("written by hand", app.GetPasswordFromOption());
    }

    [Fact]
    public async Task Tool_logs_in_with_the_api_token_of_the_arguments()
    {
        await using var server = new LocalPveServer { Rest = _ => (200, "OK", """{"data":{"version":"8.4.1","release":"8.4"}}""") };
        Args("--host", $"127.0.0.1:{server.Port}", "--api-token", "automation@pve!app=secret", "--username", "ignored@pam", "--password", "ignored");

        var client = await App().ClientTryLoginAsync(NullLoggerFactory.Instance);

        Assert.Equal("127.0.0.1", client.Host);
        Assert.Equal(server.Port, client.Port);
        Assert.Equal("automation@pve!app=secret", client.ApiToken);
        Assert.False(client.ValidateCertificate);
        var request = server.Requests.Last();
        Assert.Equal("/api2/json/version", request.Path);
        Assert.Equal("PVEAPIToken automation@pve!app=secret", request.Headers["Authorization"]);
        Assert.DoesNotContain(server.Requests, a => a.Path.EndsWith("/access/ticket"));
    }

    [Fact]
    public async Task Tool_logs_in_with_user_and_password_of_the_arguments()
    {
        await using var server = new LocalPveServer
        {
            Rest = request => request.Path.EndsWith("/access/ticket")
                                ? (200, "OK", """{"data":{"ticket":"PVE:root@pam:TICKET","CSRFPreventionToken":"CSRF"}}""")
                                : (200, "OK", """{"data":null}"""),
        };
        Args("--host", $"127.0.0.1:{server.Port}", "--username", "root@pam", "--password", "secret");

        var client = await App().ClientTryLoginAsync(NullLoggerFactory.Instance);

        Assert.Equal("PVE:root@pam:TICKET", client.PVEAuthCookie);
        Assert.Null(client.ApiToken);
        var login = JObject.Parse(server.Requests.Single(a => a.Path.EndsWith("/access/ticket")).Body);
        Assert.Equal("root", (string?)login["username"]);
        Assert.Equal("pam", (string?)login["realm"]);
        Assert.Equal("secret", (string?)login["password"]);
    }

    [Fact]
    public async Task Tool_with_refused_credentials_stops_with_the_reason()
    {
        await using var server = new LocalPveServer { Rest = _ => (401, "authentication failure", """{"data":null}""") };
        Args("--host", $"127.0.0.1:{server.Port}", "--username", "root@pam", "--password", "wrong");

        var ex = await Assert.ThrowsAsync<PveException>(() => App().ClientTryLoginAsync(NullLoggerFactory.Instance));

        Assert.Equal($"Authentication failed for host 127.0.0.1:{server.Port}: authentication failure", ex.Message);
    }

    [Theory]
    [InlineData("y\n", false, true)]
    [InlineData("YES\n", false, true)]
    [InlineData(" n \n", true, false)]
    [InlineData("No\n", true, false)]
    [InlineData("\n", true, true)]
    [InlineData("\n", false, false)]
    [InlineData("", true, true)]
    [InlineData("maybe\nwhat\ny\n", false, true)]
    public void Question_takes_yes_no_or_the_default(string typed, bool defaultAnswer, bool expected)
    {
        System.Console.SetIn(new StringReader(typed));

        Assert.Equal(expected, ConsoleHelper.ReadYesNo("Continue?", defaultAnswer));

        var written = _console.ToString();
        Assert.StartsWith(defaultAnswer ? "Continue? [Y/n] " : "Continue? [y/N] ", written);
        Assert.Equal(typed.StartsWith("maybe"), written.Contains("Invalid response 'maybe'. Please answer 'y' or 'n' or CTRL+C to exit."));
    }

    [Fact]
    public void Version_of_the_tool_is_the_one_of_the_program_that_runs()
        => Assert.Equal(Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion,
                        ConsoleHelper.GetCurrentVersionApp());

    [Theory]
    [InlineData("short", 42)]
    [InlineData("a title that is longer than the forty-seven characters of the logo", 1)]
    public void Title_is_followed_by_the_origin(string title, int spaces)
        => Assert.EndsWith($"{title}{new string(' ', spaces)}(Made in Italy)", ConsoleHelper.MakeLogoAndTitle(title));

    [Fact]
    public async Task Exit_code_of_the_tool_is_the_one_of_its_command()
    {
        var app = App();
        app.SetAction(_ => 7);

        Assert.Equal(7, await app.ExecuteAppAsync(Login, NullLogger.Instance));
        Assert.DoesNotContain("ERROR", _console.ToString());
    }

    [Fact]
    public async Task Error_of_a_command_is_written_and_the_tool_ends_with_1()
    {
        var app = App();
        app.SetAction(Fail);

        Assert.Equal(1, await app.ExecuteAppAsync(Login, NullLogger.Instance));

        Assert.Contains("ERROR: VM 100 not found", _console.ToString());
        Assert.DoesNotContain("EXCEPTION", _console.ToString());
    }

    [Fact]
    public async Task Error_of_a_command_is_written_with_its_details_when_debug_is_on()
    {
        Args("--debug");
        var app = App();
        app.SetAction(Fail);

        Assert.Equal(1, await app.ExecuteAppAsync([.. Login, "--debug"], NullLogger.Instance));

        var written = _console.ToString();
        Assert.Contains("ERROR: VM 100 not found", written);
        Assert.Contains("================ EXCEPTION ================", written);
        Assert.Contains(typeof(PveException).FullName!, written);
    }

    private static readonly string[] Login = ["--host", "pve01", "--api-token", "automation@pve!app=secret"];

    private static int Fail(ParseResult _) => throw new PveException("VM 100 not found");

    [Fact]
    public async Task Update_check_asks_the_releases_and_keeps_the_last_one_that_is_not_a_preview()
    {
        await CheckAsync("cv4pve-tool");

        var request = _gitHub.Requests.Single();
        Assert.Equal("https://api.github.com/repos/Corsinvest/cv4pve-tool/releases", request.RequestUri!.AbsoluteUri);
        Assert.Equal("cv4pve-app", request.Headers.UserAgent.ToString());
        Assert.Equal("9.9.9", (string?)Cache()["cv4pve-tool"]!["LatestVersion"]);
        Assert.True(DateTime.UtcNow - (DateTime)Cache()["cv4pve-tool"]!["LastCheck"]! < TimeSpan.FromMinutes(1));

        Assert.Equal("*** New version available: 9.9.9 (current: 1.0.0) ***", UpdateHelper.GetNotice("cv4pve-tool"));
        Assert.Null(UpdateHelper.GetNotice("cv4pve-other"));
    }

    [Theory]
    [InlineData("9.9.9")]
    [InlineData("9.10.0")]
    [InlineData("10.0.0")]
    [InlineData("not a version")]
    public async Task No_notice_when_the_tool_is_not_older(string current)
    {
        UpdateHelper.CurrentVersion = () => current;

        await CheckAsync("cv4pve-tool");

        Assert.Null(UpdateHelper.GetNotice("cv4pve-tool"));
    }

    [Fact]
    public async Task Update_check_of_today_is_not_repeated_and_each_tool_has_its_own()
    {
        WriteCache("cv4pve-tool", "2.0.0", DateTime.UtcNow.AddHours(-23));

        await CheckAsync("cv4pve-tool");
        Assert.Empty(_gitHub.Requests);
        Assert.Equal("*** New version available: 2.0.0 (current: 1.0.0) ***", UpdateHelper.GetNotice("cv4pve-tool"));

        await CheckAsync("cv4pve-other");
        Assert.Single(_gitHub.Requests);
        Assert.Equal("2.0.0", (string?)Cache()["cv4pve-tool"]!["LatestVersion"]);
        Assert.Equal("9.9.9", (string?)Cache()["cv4pve-other"]!["LatestVersion"]);
    }

    [Fact]
    public async Task Update_check_of_yesterday_is_repeated()
    {
        WriteCache("cv4pve-tool", "2.0.0", DateTime.UtcNow.AddHours(-25));

        await CheckAsync("cv4pve-tool");

        Assert.Single(_gitHub.Requests);
        Assert.Equal("9.9.9", (string?)Cache()["cv4pve-tool"]!["LatestVersion"]);
    }

    [Fact]
    public async Task Update_check_that_fails_keeps_the_version_it_knew()
    {
        var failing = new FakeHandler(_ => throw new HttpRequestException("no network"));
        UpdateHelper.CreateHandler = () => failing;
        WriteCache("cv4pve-tool", "2.0.0", DateTime.UtcNow.AddDays(-3));

        await CheckAsync("cv4pve-tool");
        Assert.Single(failing.Requests);
        Assert.Equal("2.0.0", (string?)Cache()["cv4pve-tool"]!["LatestVersion"]);

        // asked again tomorrow, not at every run
        await CheckAsync("cv4pve-tool");
        Assert.Single(failing.Requests);

        await CheckAsync("cv4pve-new");
        Assert.Equal(string.Empty, (string?)Cache()["cv4pve-new"]!["LatestVersion"]);
        Assert.Null(UpdateHelper.GetNotice("cv4pve-new"));
    }

    [Fact]
    public async Task Tool_updated_after_the_last_check_gets_no_notice_of_its_own_version()
    {
        WriteCache("cv4pve-tool", "2.0.0", DateTime.UtcNow.AddHours(-1));
        UpdateHelper.CurrentVersion = () => "2.1.0";

        await CheckAsync("cv4pve-tool");

        Assert.Empty(_gitHub.Requests);
        Assert.Equal("2.1.0", (string?)Cache()["cv4pve-tool"]!["LatestVersion"]);
        Assert.Null(UpdateHelper.GetNotice("cv4pve-tool"));
    }

    [Fact]
    public async Task Cache_that_cannot_be_read_is_written_again()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(UpdateHelper.CacheFilePath)!);
        File.WriteAllText(UpdateHelper.CacheFilePath, "{ not json");
        Assert.Null(UpdateHelper.GetNotice("cv4pve-tool"));

        await CheckAsync("cv4pve-tool");

        Assert.Equal("9.9.9", (string?)Cache()["cv4pve-tool"]!["LatestVersion"]);
    }

    [Fact]
    public async Task Tool_does_not_wait_for_a_slow_update_check()
    {
        var slow = new SlowHandler();
        UpdateHelper.CreateHandler = () => slow;
        var (task, cts) = UpdateHelper.StartCheck("cv4pve-tool");
        await slow.Started.Task.WaitAsync(TimeSpan.FromSeconds(10));

        UpdateHelper.PrintIfNewVersion("cv4pve-tool", task, cts);

        // the check is stopped, and nothing is written on a console that is not a terminal
        await task.WaitAsync(TimeSpan.FromSeconds(10));
        Assert.True(slow.Cancelled);
        Assert.Equal(string.Empty, _console.ToString());
    }

    private sealed class SlowHandler : HttpMessageHandler
    {
        public TaskCompletionSource Started { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public bool Cancelled { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Started.TrySetResult();
            try
            {
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Cancelled = true;
                throw;
            }
            return new(HttpStatusCode.OK);
        }
    }
}
