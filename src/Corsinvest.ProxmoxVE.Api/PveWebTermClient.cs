/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */
using System.Net;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Extensions.Logging;

namespace Corsinvest.ProxmoxVE.Api;

/// <summary>
/// PVE Web Terminal Client
/// </summary>
/// <param name="client"></param>
/// <param name="node"></param>
public class PveWebTermClient(PveClient client, string node) : IAsyncDisposable
{
    private readonly ClientWebSocket _ws = new();
    private readonly CancellationTokenSource _cts = new();
    private readonly StringBuilder _outputBuffer = new();
    private Timer? _pingTimer;
    private bool _connected;
    private bool _loginConfirmed;
    private readonly SemaphoreSlim _bufferLock = new(1, 1);
    private ILogger<PveWebTermClient> _logger;

    private static readonly Regex _shellPromptRegex = new(@"root@\w+:~#\s*", RegexOptions.Compiled);
    private static readonly Regex _bracketedPasteEndRegex = new(@"\x1b\[\?2004h", RegexOptions.Compiled);

    private ILogger<PveWebTermClient> Logger => _logger ??= client.LoggerFactory.CreateLogger<PveWebTermClient>();

    /// <summary>
    /// Get Output buffer
    /// </summary>
    public async Task<string> GetOutputAsync()
    {
        await _bufferLock.WaitAsync();
        try
        {
            return _outputBuffer.ToString();
        }
        finally
        {
            _bufferLock.Release();
        }
    }

    /// <summary>
    /// Connect to the terminal
    /// </summary>
    /// <returns></returns>
    public async Task<bool> ConnectAsync()
    {
        // Get terminal proxy ticket
        var termproxy = await client.Nodes[node].Termproxy.Termproxy();
        var vncTicket = Convert.ToString(termproxy.Response.data.ticket);
        var port = Convert.ToInt32(termproxy.Response.data.port);
        var ticket = HttpUtility.UrlEncode(vncTicket);
        var uri = new Uri($"wss://{client.Host}:{client.Port}/api2/json/nodes/{node}/vncwebsocket?port={port}&vncticket={ticket}");

        _ws.Options.AddSubProtocol("binary");
        _ws.Options.SetRequestHeader("CSRFPreventionToken", client.CSRFPreventionToken);

        var cookies = new CookieContainer();
        cookies.Add(new Cookie("PVEAuthCookie", client.PVEAuthCookie, "/", client.Host) { Secure = true });
        _ws.Options.Cookies = cookies;

        if (!client.ValidateCertificate)
        {
#if NET7_0_OR_GREATER
            _ws.Options.RemoteCertificateValidationCallback = (_, _, _, _) => true;
#else
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
#endif
        }

        await _ws.ConnectAsync(uri, _cts.Token);

        var loginStr = $"{termproxy.Response.data.user}:{termproxy.Response.data.ticket}\n";
        var loginBytes = Encoding.UTF8.GetBytes(loginStr);
        await _ws.SendAsync(new ArraySegment<byte>(loginBytes), WebSocketMessageType.Binary, true, _cts.Token);

        _connected = true;
        _ = Task.Run(ReceiveLoop);
        StartPing(TimeSpan.FromSeconds(30));

        return await WaitForPromptAsync();
    }

    private async Task ReceiveLoop()
    {
        var buffer = new byte[8192];

        try
        {
            while (!_cts.IsCancellationRequested && _ws.State == WebSocketState.Open)
            {
                var messageBuffer = new MemoryStream();
                WebSocketReceiveResult result;

                do
                {
                    result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                    messageBuffer.Write(buffer, 0, result.Count);
                } while (!result.EndOfMessage && !_cts.IsCancellationRequested);

                if (messageBuffer.Length == 0) { continue; }

                var text = Encoding.UTF8.GetString(messageBuffer.ToArray());

                if (text == "OK")
                {
                    _loginConfirmed = true;
                    continue;
                }

                if (text == "B") { continue; }

                await _bufferLock.WaitAsync();
                try
                {
                    _outputBuffer.Append(text);
                }
                finally
                {
                    _bufferLock.Release();
                }
            }
        }
        catch (OperationCanceledException) when (_cts.IsCancellationRequested)
        {
            Logger.LogDebug("ReceiveLoop canceled due to client disconnect.");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "[ReceiveLoop] unexpected error");
        }
    }

    private void StartPing(TimeSpan interval)
    {
        _pingTimer = new Timer(async _ =>
        {
            try
            {
                if (_ws.State == WebSocketState.Open)
                {
                    var ping = Encoding.UTF8.GetBytes("2");
                    await _ws.SendAsync(new ArraySegment<byte>(ping), WebSocketMessageType.Binary, true, _cts.Token);
                }
            }
            catch (OperationCanceledException) when (_cts.IsCancellationRequested)
            {
                Logger.LogDebug("Ping canceled due to stop.");
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "[Ping] access error");
            }
        }, null, interval, interval);
    }

    /// <summary>
    /// Send command
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task SendCommandAsync(string command)
    {
        if (!_connected) { throw new InvalidOperationException("WebSocket not connected."); }
        var finalCommand = $"\u001b[200~{command}\u001b[201~";
        await SendWithHeaderAsync(finalCommand);
        await SendWithHeaderAsync("\n"); // simulate Enter
    }

    private async Task SendWithHeaderAsync(string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        var header = Encoding.UTF8.GetBytes($"0:{bytes.Length}:");
        var message = new byte[header.Length + bytes.Length];

        Buffer.BlockCopy(header, 0, message, 0, header.Length);
        Buffer.BlockCopy(bytes, 0, message, header.Length, bytes.Length);

        await _ws.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Binary, true, _cts.Token);
    }

    /// <summary>
    /// Wait for prompt
    /// </summary>
    /// <param name="timeoutMs"></param>
    /// <returns></returns>
    public async Task<bool> WaitForPromptAsync(int timeoutMs = 10000)
    {
        var start = DateTime.UtcNow;
        while ((DateTime.UtcNow - start).TotalMilliseconds < timeoutMs)
        {
            if (_loginConfirmed
                && (_shellPromptRegex.IsMatch(await GetOutputAsync())
                    || _bracketedPasteEndRegex.IsMatch(await GetOutputAsync())))
            {
                return true;
            }
            await Task.Delay(100);
        }
        return false;
    }

    /// <summary>
    /// Execute command
    /// </summary>
    /// <param name="command"></param>
    /// <param name="timeoutMs"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<(string StdOut, string StdErr, int ExitCode)> ExecuteCommandAsync(string command, int timeoutMs = 20000)
    {
        if (string.IsNullOrWhiteSpace(command)) { throw new ArgumentException("Value cannot be null or whitespace.", nameof(command)); }

        var id = Guid.NewGuid().ToString("N");

        var stdoutFile = $"/tmp/stdout_{id}.txt";
        var stderrFile = $"/tmp/stderr_{id}.txt";
        var exitCodeFile = $"/tmp/exitcode_{id}.txt";
        var scriptFile = $"/tmp/script_{id}.sh";

        var beginMarker = $"#CV4PVE_ADMIN_BEGIN_{id}";
        var stderrMarker = $"#CV4PVE_ADMIN_STDERR_{id}";
        var exitCodeMarker = $"#CV4PVE_ADMIN_EXITCODE_{id}";
        var endMarker = $"#CV4PVE_ADMIN_END_{id}";
        var eofMarker = $"#CV4PVE_ADMIN_EOF_{id}";

        var scriptContent = $"""
#!/bin/bash
trap 'rm -f {stdoutFile} {stderrFile} {exitCodeFile} {scriptFile}' EXIT

({command}) 1>{stdoutFile} 2>{stderrFile}
echo $? > {exitCodeFile}
echo '{beginMarker}'
cat {stdoutFile}
echo '{stderrMarker}'
cat {stderrFile}
echo '{exitCodeMarker}'
cat {exitCodeFile}
echo '{endMarker}'
""";

        await SendCommandAsync($"cat > {scriptFile} <<'{eofMarker}'\n");
        await Task.Delay(50);

        await SendCommandAsync($"{scriptContent}\n");
        await Task.Delay(100);

        await SendCommandAsync($"{eofMarker}\n");
        await Task.Delay(50);
        if (!await WaitForPromptAsync(timeoutMs)) { return (string.Empty, "[ERROR] Timeout during execution", -1); }

        _outputBuffer.Clear();
        await SendCommandAsync($"chmod +x {scriptFile}\n");
        if (!await WaitForPromptAsync(timeoutMs)) { return (string.Empty, "[ERROR] Timeout during execution", -1); }

        _outputBuffer.Clear();
        await SendCommandAsync($"{scriptFile}\n");
        //await Task.Delay(50);
        if (!await WaitForPromptAsync(timeoutMs)) { return (string.Empty, "[ERROR] Timeout during execution", -1); }

        var rawOutput = await GetOutputAsync() ?? string.Empty;

        // Parsing
        var beginIndex = rawOutput.IndexOf(beginMarker);
        if (beginIndex < 0) { return (rawOutput, "[ERROR] BEGIN marker not found", -1); }

        var contentFromBegin = rawOutput[(beginIndex + beginMarker.Length + "\r\n".Length)..];
        var endIndex = contentFromBegin.IndexOf(endMarker);
        if (endIndex < 0) { return (rawOutput, "[ERROR] END marker not found", -1); }

        var content = contentFromBegin[..endIndex];
        var stderrIndex = content.IndexOf(stderrMarker, StringComparison.Ordinal);
        var exitCodeIndex = content.IndexOf(exitCodeMarker, StringComparison.Ordinal);

        if (stderrIndex < 0 || exitCodeIndex < 0 || exitCodeIndex <= stderrIndex)
        {
            return (content.Trim(), $"[ERROR] Parsing output failed: STDERR=({stderrIndex}), EXITCODE=({exitCodeIndex})", -1);
        }

        var stdout = content[..stderrIndex].Trim('\r', '\n');
        var stderr = content[(stderrIndex + stderrMarker.Length)..exitCodeIndex].Trim('\r', '\n');
        var exitCodeStr = content[(exitCodeIndex + exitCodeMarker.Length)..].Trim();

        if (!int.TryParse(exitCodeStr, out var exitCode))
        {
            stderr += "\n[ERROR] Exit code parse failed";
            exitCode = -1;
        }

        return (stdout, stderr, exitCode);
    }

    /// <summary>
    /// Disconnect from the terminal
    /// </summary>
    /// <returns></returns>
    public async Task DisconnectAsync()
    {
        Logger.LogDebug("Disconnecting WebSocket");
        _cts.Cancel();

        if (_ws.State == WebSocketState.Open)
        {
            await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnecting", CancellationToken.None);
        }

        _pingTimer?.Dispose();
        _ws.Dispose();
        Logger.LogDebug("Disconnected");
    }

    private async Task<Match> ExtractBracketedPasteContentAsync()
        => Regex.Match(await GetOutputAsync(), @"\x1B\[\?2004l\r(.*?)\r?\x1B\[\?2004h", RegexOptions.Singleline);

    /// <summary>
    /// Download file from remote path
    /// </summary>
    /// <param name="remotePath"></param>
    /// <param name="stream"></param>
    /// <param name="chunkSizeKB"></param>
    /// <param name="timeoutMs"></param>
    /// <returns></returns>
    public async Task<(bool Success, string? ErrorMessage)> DownloadFileAsync(string remotePath,
                                                                              FileStream stream,
                                                                              int chunkSizeKB = 128,
                                                                              int timeoutMs = 20000)
    {
        if (string.IsNullOrWhiteSpace(remotePath)) { return (false, "Invalid remote path."); }

        chunkSizeKB = Math.Max(128, chunkSizeKB);

        try
        {
            _outputBuffer.Clear();
            await SendCommandAsync($"sha256sum \"{remotePath}\" 2>/dev/null");
            if (!await WaitForPromptAsync(timeoutMs)) { return (false, "Timeout waiting for shell prompt after sha256sum command."); }

            var match = await ExtractBracketedPasteContentAsync();
            if (!match.Success) { return (false, "SHA256 command output not found."); }

            match = Regex.Match(match.Groups[1].Value, "([a-fA-F0-9]{64})");
            if (!match.Success) { return (false, "Could not retrieve remote SHA256 hash."); }
            var remoteHash = match.Groups[1].Value.ToLowerInvariant();

            using var sha256 = SHA256.Create();

            for (var chunkIndex = 0; ; chunkIndex++)
            {
                _outputBuffer.Clear();

                var command = $"dd if={remotePath} bs={chunkSizeKB}K skip={chunkIndex} count=1 2>/dev/null | base64 -w 0";

                await SendCommandAsync(command);
                if (!await WaitForPromptAsync(timeoutMs)) { return (false, "Timeout waiting for shell prompt after download chunk command."); }

                match = await ExtractBracketedPasteContentAsync();
                if (!match.Success) { return (false, "Download markers not found in output."); }

                var base64Data = match.Groups[1].Value;
                if (string.IsNullOrWhiteSpace(base64Data)) { break; }

                byte[] chunkBytes;
                try
                {
                    chunkBytes = Convert.FromBase64String(base64Data);
                }
                catch (FormatException)
                {
                    return (false, "Invalid base64 data received.");
                }

                if (chunkBytes.Length == 0) { break; }
                sha256.TransformBlock(chunkBytes, 0, chunkBytes.Length, null, 0);
#if NET7_0_OR_GREATER
                await stream.WriteAsync(chunkBytes);
#else
                await stream.WriteAsync(chunkBytes, 0, chunkBytes.Length);
#endif
            }

            sha256.TransformFinalBlock([], 0, 0);
            var localHash = ToHexStringLower(sha256.Hash!);
            if (localHash != remoteHash) { return (false, $"SHA256 mismatch. Remote: {remoteHash}, Local: {localHash}"); }

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Error during download: {ex.Message}");
        }
    }

    private static string ToHexStringLower(byte[] bytes)
    {
        var sb = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes) { sb.Append(b.ToString("x2")); }
        return sb.ToString();
    }

    /// <summary>
    /// Dispose async
    /// </summary>
    /// <returns></returns>
    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();
        _cts.Dispose();
    }
}
