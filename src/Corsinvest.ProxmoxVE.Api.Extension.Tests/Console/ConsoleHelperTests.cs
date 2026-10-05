/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.CommandLine;
using Corsinvest.ProxmoxVE.Api.Console.Helpers;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Console;

public class ConsoleHelperTests
{
    private const string Key = "012345678901234567890123";

    private static RootCommand App() => ConsoleHelper.CreateApp("Test tool");

    private static string[] Errors(Command command, params string[] args)
        => [.. command.Parse(args).Errors.Select(a => a.Message)];

    [Fact]
    public void App_has_the_connection_options_of_the_suite()
    {
        var app = App();
        var names = app.Options.Select(a => a.Name).ToArray();

        foreach (var name in new[] { "--host", "--api-token", "--username", "--password", "--validate-certificate", "--debug", "--log-level", "--dry-run" })
        {
            Assert.Contains(name, names);
        }
        Assert.True(app.GetHostOption().Required);
        Assert.Contains("Made in Italy", app.Description);
    }

    [Theory]
    [InlineData("--debug")]
    [InlineData("--log-level")]
    [InlineData("--dry-run")]
    public void Diagnostic_options_are_hidden_and_valid_in_every_subcommand(string name)
    {
        var option = App().Options.Single(a => a.Name == name);

        Assert.True(option.Hidden);
        Assert.True(option.Recursive);
    }

    [Fact]
    public void Api_token_is_enough_to_connect()
    {
        var app = App();
        var result = app.Parse(["--host", "pve01,pve02:8007", "--api-token", "automation@pve!app=secret"]);

        Assert.Empty(result.Errors);
        Assert.Equal("pve01,pve02:8007", result.GetValue(app.GetHostOption()));
        Assert.Equal("automation@pve!app=secret", result.GetValue(app.GetApiTokenOption()));
        Assert.False(result.GetValue(app.GetValidateCertificateOption()));
    }

    [Fact]
    public void User_and_password_are_enough_to_connect()
    {
        var app = App();
        var result = app.Parse(["--host=pve01", "--username=root@pam", "--password=secret", "--validate-certificate"]);

        Assert.Empty(result.Errors);
        Assert.Equal("root@pam", result.GetValue(app.GetUsernameOption()));
        Assert.Equal("secret", result.GetValue(app.GetPasswordOption()));
        Assert.True(result.GetValue(app.GetValidateCertificateOption()));
    }

    [Fact]
    public void Host_is_required()
        => Assert.Contains(Errors(App(), "--api-token", "a@pve!t=x"), a => a.Contains("--host"));

    [Fact]
    public void Host_without_token_or_user_is_refused()
        => Assert.Contains(Errors(App(), "--host", "pve01"), a => a.Contains("--username") && a.Contains("--api-token"));

    [Fact]
    public void User_without_password_is_refused()
        => Assert.Contains(Errors(App(), "--host", "pve01", "--username", "root@pam"), a => a.Contains("--password"));

    [Fact]
    public void Command_and_option_names_take_aliases_after_a_bar()
    {
        var app = App();
        var command = app.AddCommand("snap|s", "Snapshot the selected guests");
        var option = command.AddOption<string>("--name|-n", "Name of the snapshot");
        var argument = command.AddArgument("label", "Label of the run");

        Assert.Equal("snap", command.Name);
        Assert.Equal(["s"], command.Aliases);
        Assert.Equal("--name", option.Name);
        Assert.Equal(["-n"], option.Aliases);
        Assert.Same(option, command.GetOption<string>("-n"));

        var result = app.Parse(["--host", "pve01", "--api-token", "a@pve!t=x", "s", "-n", "before-update", "daily"]);
        Assert.Empty(result.Errors);
        Assert.Equal("before-update", result.GetValue(option));
        Assert.Equal("daily", result.GetValue(argument));
    }

    [Theory]
    [InlineData("1", true)]
    [InlineData("100", true)]
    [InlineData("0", false)]
    [InlineData("101", false)]
    public void Range_validator_accepts_only_the_values_in_the_range(string value, bool valid)
    {
        var command = new RootCommand();
        command.AddOption<int>("--keep", "Snapshots to keep").AddValidatorRange(1, 100);

        Assert.Equal(valid, Errors(command, "--keep", value).Length == 0);
    }

    [Fact]
    public void File_and_directory_validators_want_something_that_exists()
    {
        var file = Path.GetTempFileName();
        try
        {
            var command = new RootCommand();
            command.ScriptFileOption();
            command.AddOption<string>("--dir", "A directory").AddValidatorExistDirectory();

            Assert.Empty(Errors(command, "--script", file, "--dir", Path.GetTempPath()));
            Assert.NotEmpty(Errors(command, "--script", file + ".missing"));
            Assert.NotEmpty(Errors(command, "--dir", file + ".missing"));
        }
        finally
        {
            File.Delete(file);
        }
    }

    [Fact]
    public void Shared_options_have_the_names_of_the_suite()
    {
        var command = new RootCommand();

        Assert.Equal("--vmid", command.VmIdsOrNamesOption().Name);
        Assert.Equal("--timeout", command.TimeoutOption().Name);
        Assert.Equal("--verbose", command.VerboseOption().Name);

        var output = command.TableOutputOption();
        Assert.Equal("--output", output.Name);
        Assert.Equal(["-o"], output.Aliases);
        Assert.Equal(TableGenerator.Output.Text, command.Parse([]).GetValue(output));
        Assert.Equal(TableGenerator.Output.Markdown, command.Parse(["-o", "Markdown"]).GetValue(output));
    }

    [Fact]
    public void Title_is_written_under_the_logo()
    {
        var text = ConsoleHelper.MakeLogoAndTitle("Test tool");

        Assert.StartsWith(ConsoleHelper.Logo, text);
        Assert.EndsWith("(Made in Italy)", text);
        Assert.Contains("Test tool", text);
    }

    [Fact]
    public void Logger_factory_logs_the_client_at_the_level_asked()
    {
        using var factory = ConsoleHelper.CreateLoggerFactory<ConsoleHelperTests>(LogLevel.Debug);
        var clientLogger = factory.CreateLogger(typeof(PveClientBase).FullName!);
        var otherLogger = factory.CreateLogger("System.Net.Http");

        Assert.True(clientLogger.IsEnabled(LogLevel.Debug));
        Assert.False(clientLogger.IsEnabled(LogLevel.Trace));
        Assert.False(otherLogger.IsEnabled(LogLevel.Information));
        Assert.True(otherLogger.IsEnabled(LogLevel.Warning));
    }

    [Theory]
    [InlineData("secret")]
    [InlineData("")]
    [InlineData("pass with spaces and àèìòù €")]
    public void Password_written_to_a_file_is_read_back(string password)
    {
        var stored = StringHelper.Encrypt(password, Key);

        Assert.StartsWith(StringHelper.GetVersionPrefix(), stored);
        if (password.Length > 0) { Assert.DoesNotContain(password, stored); }
        Assert.Equal(password, StringHelper.Decrypt(stored, Key));
    }

    [Fact]
    public void Same_password_is_stored_differently_each_time()
        => Assert.NotEqual(StringHelper.Encrypt("secret", Key), StringHelper.Encrypt("secret", Key));

    [Fact]
    public void Password_file_of_the_old_format_is_still_read()
    {
        // "old-secret" as the versions before the 'v2:' prefix wrote it: a file written then must still open
        const string old = "cJ8ZEGCBIyvy7bWXdmmfxQ==";

        Assert.Equal("old-secret", StringHelper.Decrypt(old, Key));
    }

    [Theory]
    [InlineData("plain text password")]
    [InlineData("v2:not-base64")]
    public void File_that_is_not_an_encrypted_password_is_refused(string content)
        => Assert.ThrowsAny<Exception>(() => StringHelper.Decrypt(content, Key));
}
