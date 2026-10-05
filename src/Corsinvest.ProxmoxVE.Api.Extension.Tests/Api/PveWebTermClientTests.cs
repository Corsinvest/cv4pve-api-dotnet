/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Net;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace Corsinvest.ProxmoxVE.Api.Extension.Tests.Api;

/// <summary>
/// The terminal of a node, against a local server that speaks the protocol of the PVE terminal proxy:
/// the login line, then messages "0:length:text" for the keys.
/// </summary>
public class PveWebTermClientTests
{
    private const string Prompt = "root@pve01:~# ";
    private const string PasteStart = "\u001b[?2004l\r";
    private const string PasteEnd = "\u001b[?2004h";

    private sealed class Terminal : IAsyncDisposable
    {
        public LocalPveServer Server { get; } = new();
        public PveClient Client { get; }
        public PveWebTermClient Term { get; }

        /// <summary>The login line, then every command the terminal received.</summary>
        public List<string> Received { get; } = [];

        /// <summary>Pings received, and whether the client said that it was closing.</summary>
        public int Pings;
        public bool ClosedByClient;

        public Terminal(Func<string, string?> answer)
        {
            Server.Rest = request => request.Path switch
            {
                "/api2/json/access/ticket" => (200, "OK", """{"data":{"ticket":"PVE:root@pam:TICKET","CSRFPreventionToken":"CSRF-TOKEN","username":"root@pam"}}"""),
                "/api2/json/nodes/pve01/termproxy" => (200, "OK", """{"data":{"ticket":"PVEVNC:TERM+TICKET/==","port":5900,"user":"root@pam","upid":"UPID:pve01:0012A3F4:05C1B2D3:6720F1A0:vncshell::root@pam:"}}"""),
                _ => (501, "Not expected", """{"data":null}"""),
            };

            Server.Socket = async (socket, _, cancellationToken) =>
            {
                Received.Add(Encoding.UTF8.GetString((await LocalPveServer.ReceiveAsync(socket, cancellationToken))!));
                await LocalPveServer.SendTextAsync(socket, "OK", cancellationToken);
                await LocalPveServer.SendTextAsync(socket, "Linux pve01 6.8.12-4-pve\r\n" + Prompt, cancellationToken);

                var keys = new StringBuilder();
                while (await LocalPveServer.ReceiveAsync(socket, cancellationToken) is { } message)
                {
                    // "0:length:text" are keys, "2" is the ping
                    var parts = Encoding.UTF8.GetString(message).Split(':', 3);
                    if (parts.Length < 3)
                    {
                        Assert.Equal("2", parts[0]);
                        Interlocked.Increment(ref Pings);
                        continue;
                    }
                    Assert.Equal(Encoding.UTF8.GetByteCount(parts[2]), int.Parse(parts[1]));

                    if (parts[2] != "\n")
                    {
                        keys.Append(parts[2]);
                        continue;
                    }

                    // Enter: the pasted text is a command
                    var command = keys.ToString();
                    keys.Clear();
                    Assert.StartsWith("\u001b[200~", command);
                    Assert.EndsWith("\u001b[201~", command);
                    command = command[6..^6].Trim();
                    Received.Add(command);

                    var reply = answer(command);
                    if (reply != null) { await LocalPveServer.SendTextAsync(socket, reply, cancellationToken); }
                }

                // the loop ends without an error only when the client sends the close
                ClosedByClient = true;
            };

            Client = Server.Client();
            Term = new PveWebTermClient(Client, "pve01");
        }

        public async Task<Terminal> ConnectAsync()
        {
            Assert.True(await Client.LoginAsync("root@pam", "secret"));
            Assert.True(await Term.ConnectAsync());
            return this;
        }

        public async ValueTask DisposeAsync()
        {
            await Term.DisposeAsync();
            await Server.DisposeAsync();
        }
    }

    // a shell that runs the script of ExecuteCommandAsync: its output is given by the test
    private static Func<string, string?> Shell(Func<string, string?> scriptOutput)
        => command =>
        {
            if (command.StartsWith("chmod +x /tmp/script_")) { return Prompt; }
            var script = Regex.Match(command, @"^/tmp/script_([0-9a-f]{32})\.sh$");
            return script.Success ? scriptOutput(script.Groups[1].Value) : null;
        };

    private static string Output(string id, string stdOut, string stdErr, string exitCode)
        => $"/tmp/script_{id}.sh\r\n"
           + $"#CV4PVE_ADMIN_BEGIN_{id}\r\n{stdOut}#CV4PVE_ADMIN_STDERR_{id}\r\n{stdErr}#CV4PVE_ADMIN_EXITCODE_{id}\r\n{exitCode}\r\n#CV4PVE_ADMIN_END_{id}\r\n"
           + Prompt;

    [Fact]
    public async Task Connection_logs_in_with_the_ticket_of_the_terminal_and_waits_for_the_prompt()
    {
        await using var terminal = await new Terminal(_ => null).ConnectAsync();

        Assert.Equal("root@pam:PVEVNC:TERM+TICKET/==\n", terminal.Received[0]);
        Assert.Contains(Prompt, await terminal.Term.GetOutputAsync());
        Assert.DoesNotContain("OK", (await terminal.Term.GetOutputAsync()).Split('\n')[0]);

        var socket = terminal.Server.Requests.Single(a => a.Headers.ContainsKey("Sec-WebSocket-Key"));
        Assert.Equal("/api2/json/nodes/pve01/vncwebsocket", socket.Path);
        Assert.Equal("5900", socket.Query["port"]);
        Assert.Equal("PVEVNC:TERM+TICKET/==", socket.Query["vncticket"]);
        Assert.Equal("binary", socket.Headers["Sec-WebSocket-Protocol"]);
        Assert.Equal("CSRF-TOKEN", socket.Headers["CSRFPreventionToken"]);
        Assert.Contains("PVEAuthCookie=PVE:root@pam:TICKET", socket.Headers["Cookie"]);
    }

    [Fact]
    public async Task Refused_terminal_is_reported_with_the_reason()
    {
        var client = FakeHandler.Client(new FakeHandler(_ =>
        {
            var response = FakeHandler.Json(HttpStatusCode.Forbidden, """{"data":null}""");
            response.ReasonPhrase = "Permission check failed (/nodes/pve01, Sys.Console)";
            return response;
        }));
        await using var term = new PveWebTermClient(client, "pve01");

        var ex = await Assert.ThrowsAsync<PveResultException>(term.ConnectAsync);

        Assert.Equal("Permission check failed (/nodes/pve01, Sys.Console)", ex.Message);
    }

    [Fact]
    public async Task Before_the_connection_nothing_can_be_sent()
    {
        await using var term = new PveWebTermClient(FakeHandler.Client(new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK, "{}"))), "pve01");

        await Assert.ThrowsAsync<InvalidOperationException>(() => term.SendCommandAsync("ls"));
        Assert.False(await term.WaitForPromptAsync(150));
        Assert.Equal(string.Empty, await term.GetOutputAsync());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Empty_command_is_refused(string? command)
    {
        await using var term = new PveWebTermClient(FakeHandler.Client(new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK, "{}"))), "pve01");

        await Assert.ThrowsAsync<ArgumentException>(() => term.ExecuteCommandAsync(command!));
    }

    [Fact]
    public async Task Command_gives_output_errors_and_exit_code()
    {
        await using var terminal = await new Terminal(Shell(id => Output(id, "hello\r\nworld\r\n", "warning: disk\r\n", "3"))).ConnectAsync();

        var (stdOut, stdErr, exitCode) = await terminal.Term.ExecuteCommandAsync("echo hello; echo world");

        Assert.Equal("hello\r\nworld", stdOut);
        Assert.Equal("warning: disk", stdErr);
        Assert.Equal(3, exitCode);

        // the command is written into a script that keeps output and errors apart, then the script runs
        var script = terminal.Received.Single(a => a.StartsWith("#!/bin/bash"));
        var id = Regex.Match(script, "script_([0-9a-f]{32})").Groups[1].Value;
        Assert.Contains($"(echo hello; echo world) 1>/tmp/stdout_{id}.txt 2>/tmp/stderr_{id}.txt", script);
        Assert.Contains("trap 'rm -f ", script);
        Assert.Equal([$"cat > /tmp/script_{id}.sh <<'#CV4PVE_ADMIN_EOF_{id}'", script, $"#CV4PVE_ADMIN_EOF_{id}", $"chmod +x /tmp/script_{id}.sh", $"/tmp/script_{id}.sh"],
                     terminal.Received.Skip(1));
    }

    [Fact]
    public async Task Command_without_output_and_errors()
    {
        await using var terminal = await new Terminal(Shell(id => Output(id, "", "", "0"))).ConnectAsync();

        Assert.Equal((string.Empty, string.Empty, 0), await terminal.Term.ExecuteCommandAsync("true"));
    }

    [Fact]
    public async Task Two_commands_one_after_the_other()
    {
        var run = 0;
        await using var terminal = await new Terminal(Shell(id => Output(id, $"run {++run}\r\n", "", "0"))).ConnectAsync();

        Assert.Equal("run 1", (await terminal.Term.ExecuteCommandAsync("first")).StdOut);
        Assert.Equal("run 2", (await terminal.Term.ExecuteCommandAsync("second")).StdOut);
    }

    [Fact]
    public async Task Command_that_does_not_end_is_a_timeout()
    {
        await using var terminal = await new Terminal(Shell(_ => null)).ConnectAsync();

        Assert.Equal((string.Empty, "[ERROR] Timeout during execution", -1), await terminal.Term.ExecuteCommandAsync("sleep 1000", 300));
    }

    [Fact]
    public async Task Output_without_the_markers_is_an_error()
    {
        await using var terminal = await new Terminal(Shell(_ => "bash: no such file\r\n" + Prompt)).ConnectAsync();

        var (stdOut, stdErr, exitCode) = await terminal.Term.ExecuteCommandAsync("ls");

        Assert.Contains("bash: no such file", stdOut);
        Assert.Equal("[ERROR] BEGIN marker not found", stdErr);
        Assert.Equal(-1, exitCode);
    }

    [Fact]
    public async Task Output_cut_before_the_end_is_an_error()
    {
        await using var terminal = await new Terminal(Shell(id => $"#CV4PVE_ADMIN_BEGIN_{id}\r\nhello\r\n" + Prompt)).ConnectAsync();

        var (_, stdErr, exitCode) = await terminal.Term.ExecuteCommandAsync("ls");

        Assert.Equal("[ERROR] END marker not found", stdErr);
        Assert.Equal(-1, exitCode);
    }

    [Fact]
    public async Task Output_without_the_parts_is_an_error()
    {
        await using var terminal = await new Terminal(Shell(id => $"#CV4PVE_ADMIN_BEGIN_{id}\r\nhello\r\n#CV4PVE_ADMIN_END_{id}\r\n" + Prompt)).ConnectAsync();

        var (stdOut, stdErr, exitCode) = await terminal.Term.ExecuteCommandAsync("ls");

        Assert.Equal("hello", stdOut);
        Assert.StartsWith("[ERROR] Parsing output failed", stdErr);
        Assert.Equal(-1, exitCode);
    }

    [Fact]
    public async Task Exit_code_that_is_not_a_number_is_an_error()
    {
        await using var terminal = await new Terminal(Shell(id => Output(id, "hello\r\n", "oops\r\n", "?"))).ConnectAsync();

        Assert.Equal(("hello", "oops\n[ERROR] Exit code parse failed", -1), await terminal.Term.ExecuteCommandAsync("ls"));
    }

    // a shell with one file: sha256sum and dd in blocks of 128K, as DownloadFileAsync asks
    private static Func<string, string?> FileShell(byte[] content, string? hash = null, Func<int, string, string>? block = null)
        => command =>
        {
            if (command.StartsWith("sha256sum "))
            {
                hash ??= Convert.ToHexString(SHA256.HashData(content)).ToLowerInvariant();
                return PasteStart + (hash.Length == 0 ? string.Empty : $"{hash}  /root/file.bin\r\n") + PasteEnd + Prompt;
            }

            var dd = Regex.Match(command, @"^dd if=(\S+) bs=(\d+)K skip=(\d+) count=1 2>/dev/null \| base64 -w 0$");
            if (!dd.Success) { return null; }

            var size = int.Parse(dd.Groups[2].Value) * 1024;
            var index = int.Parse(dd.Groups[3].Value);
            var data = Convert.ToBase64String(content.Skip(index * size).Take(size).ToArray());
            return PasteStart + (block?.Invoke(index, data) ?? data) + PasteEnd + Prompt;
        };

    private static async Task<((bool Success, string? ErrorMessage) Result, byte[] Content)> DownloadAsync(Terminal terminal, string remotePath = "/root/file.bin")
    {
        var fileName = Path.GetTempFileName();
        try
        {
            (bool, string?) result;
            await using (var stream = File.Create(fileName))
            {
                result = await terminal.Term.DownloadFileAsync(remotePath, stream, timeoutMs: 3000);
            }
            return (result, await File.ReadAllBytesAsync(fileName));
        }
        finally
        {
            File.Delete(fileName);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(128 * 1024)]
    [InlineData(300_000)]
    public async Task File_is_downloaded_in_blocks_and_checked(int size)
    {
        var content = new byte[size];
        new Random(size).NextBytes(content);
        await using var terminal = await new Terminal(FileShell(content)).ConnectAsync();

        var (result, downloaded) = await DownloadAsync(terminal);

        Assert.Equal((true, null), result);
        Assert.Equal(content, downloaded);
        Assert.Equal("sha256sum \"/root/file.bin\" 2>/dev/null", terminal.Received[1]);
        // every block and one more, empty, that ends the file
        Assert.Equal((size / (128 * 1024)) + 1 + (size % (128 * 1024) == 0 ? 0 : 1), terminal.Received.Count(a => a.StartsWith("dd if=/root/file.bin bs=128K")));
    }

    [Fact]
    public async Task File_with_a_different_hash_is_refused()
    {
        var wrong = new string('0', 64);
        await using var terminal = await new Terminal(FileShell([1, 2, 3], wrong)).ConnectAsync();

        var (result, _) = await DownloadAsync(terminal);

        Assert.False(result.Success);
        Assert.StartsWith($"SHA256 mismatch. Remote: {wrong}, Local: 039058c6", result.ErrorMessage);
    }

    [Fact]
    public async Task File_that_does_not_exist_is_refused()
    {
        await using var terminal = await new Terminal(FileShell([], hash: string.Empty)).ConnectAsync();

        Assert.Equal((false, "Could not retrieve remote SHA256 hash."), (await DownloadAsync(terminal)).Result);
    }

    [Fact]
    public async Task Block_that_is_not_base64_is_refused()
    {
        await using var terminal = await new Terminal(FileShell([1, 2, 3], block: (_, _) => "not base64!")).ConnectAsync();

        Assert.Equal((false, "Invalid base64 data received."), (await DownloadAsync(terminal)).Result);
    }

    [Fact]
    public async Task Answer_without_the_marks_of_the_terminal_is_refused()
    {
        await using var terminal = await new Terminal(_ => "something\r\n" + Prompt).ConnectAsync();

        Assert.Equal((false, "SHA256 command output not found."), (await DownloadAsync(terminal)).Result);
    }

    [Fact]
    public async Task Download_that_gets_no_answer_is_a_timeout()
    {
        await using var terminal = await new Terminal(_ => null).ConnectAsync();
        var fileName = Path.GetTempFileName();
        try
        {
            await using var stream = File.Create(fileName);

            Assert.Equal((false, "Timeout waiting for shell prompt after sha256sum command."), await terminal.Term.DownloadFileAsync("/root/file.bin", stream, timeoutMs: 300));
        }
        finally
        {
            File.Delete(fileName);
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Download_without_a_path_is_refused(string remotePath)
    {
        await using var term = new PveWebTermClient(FakeHandler.Client(new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK, "{}"))), "pve01");

        Assert.Equal((false, "Invalid remote path."), await term.DownloadFileAsync(remotePath, null!));
    }

    [Fact]
    public async Task Download_before_the_connection_is_an_error()
    {
        await using var term = new PveWebTermClient(FakeHandler.Client(new FakeHandler(_ => FakeHandler.Json(HttpStatusCode.OK, "{}"))), "pve01");

        Assert.Equal((false, "Error during download: WebSocket not connected."), await term.DownloadFileAsync("/root/file.bin", null!));
    }

    [Fact]
    public async Task Disconnection_closes_the_connection_to_the_node()
    {
        var terminal = await new Terminal(_ => null).ConnectAsync();

        await terminal.Term.DisconnectAsync();

        await terminal.Server.SocketClosed.WaitAsync(TimeSpan.FromSeconds(10));
        Assert.True(terminal.ClosedByClient);
        await terminal.Server.DisposeAsync();
    }

    [Fact]
    public async Task Certificate_of_the_node_is_checked_when_asked()
    {
        await using var server = new LocalPveServer();
        var client = server.Client();
        client.ValidateCertificate = true;

        var result = await client.Version.Version();

        // the certificate of the test server is self-signed: the request does not leave
        Assert.False(result.IsSuccessStatusCode);
        Assert.Empty(server.Requests);
    }

    [Fact]
    public async Task Open_terminal_is_kept_alive_with_pings()
    {
        var terminal = new Terminal(_ => null);
        terminal.Term.PingInterval = TimeSpan.FromMilliseconds(50);
        await using (await terminal.ConnectAsync())
        {
            for (var i = 0; i < 100 && Volatile.Read(ref terminal.Pings) < 3; i++) { await Task.Delay(50); }

            Assert.True(Volatile.Read(ref terminal.Pings) >= 3);
        }

        // no ping after the disconnection
        var pings = Volatile.Read(ref terminal.Pings);
        await Task.Delay(200);
        Assert.Equal(pings, Volatile.Read(ref terminal.Pings));
    }
}
