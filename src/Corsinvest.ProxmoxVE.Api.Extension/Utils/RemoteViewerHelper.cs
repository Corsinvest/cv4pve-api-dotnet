/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

#nullable enable

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using Corsinvest.ProxmoxVE.Api.Shared.Utils;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils;

/// <summary>
/// Helper for launching SPICE and VNC remote-viewer sessions against Proxmox VE.
/// </summary>
public static partial class RemoteViewerHelper
{
    [GeneratedRegex(@"^(http|https|)://.*$")]
    private static partial Regex HttpRegex();

    /// <summary>
    /// Prepares a SPICE .vv file for the given VM/CT and returns its path.
    /// If <paramref name="proxy"/> is null or whitespace, defaults to <c>client.Host</c>.
    /// </summary>
    public static async Task<(string? Error, string? FileName)>
        PrepareSpiceAsync(PveClient client,
                          string node,
                          VmType vmType,
                          long vmId,
                          string? proxy = null,
                          TextWriter? output = null)
    {
        if (string.IsNullOrWhiteSpace(proxy)) { proxy = client.Host; }

        var (success, reasonPhrase, content) = vmType switch
        {
            VmType.Qemu => await client.Nodes[node].Qemu[vmId].Spiceproxy.GetSpiceFileVVAsync(proxy),
            VmType.Lxc => await client.Nodes[node].Lxc[vmId].Spiceproxy.GetSpiceFileVVAsync(proxy),
            _ => throw new InvalidEnumArgumentException(),
        };

        if (!success) { return (reasonPhrase, null); }

        content = await ReplaceProxyInContentAsync(content, proxy, output);

        var fileName = Path.GetTempFileName().Replace(".tmp", ".vv");
        await File.WriteAllTextAsync(fileName, content);
        return (null, fileName);
    }

    /// <summary>
    /// Opens a VNC WebSocket tunnel and prepares a .vv file for the given VM/CT.
    /// The caller must dispose the returned <see cref="VncWebSocketBridge"/> after the viewer exits.
    /// </summary>
    public static async Task<(string? Error, string? FileName, VncWebSocketBridge? Bridge)>
        PrepareVncAsync(PveClient client, string node, VmType vmType, long vmId, TextWriter? output = null)
    {
        var vncResult = vmType switch
        {
            VmType.Qemu => await client.Nodes[node].Qemu[vmId].Vncproxy.Vncproxy(websocket: true),
            VmType.Lxc => await client.Nodes[node].Lxc[vmId].Vncproxy.Vncproxy(websocket: true),
            _ => throw new InvalidEnumArgumentException(),
        };

        if (!vncResult.IsSuccessStatusCode) { return (vncResult.ReasonPhrase, null, null); }

        var ticket = (string)vncResult.Response.data.ticket;
        var port = Convert.ToInt32(vncResult.Response.data.port);

        var vmTypeStr = vmType switch
        {
            VmType.Qemu => "qemu",
            VmType.Lxc => "lxc",
            _ => throw new InvalidEnumArgumentException(),
        };

        var wsUrl = NoVncHelper.GetWebsocketUrl(client.Host,
                                                client.Port,
                                                node,
                                                vmTypeStr,
                                                vmId,
                                                port,
                                                HttpUtility.UrlEncode(ticket));

        if (output != null) { await output.WriteLineAsync($"VNC WebSocket URL: {wsUrl}"); }

        var vmTypeStrvv = vmType switch
        {
            VmType.Qemu => "VM",
            VmType.Lxc => "CT",
            _ => throw new InvalidEnumArgumentException(),
        };

        return await CreateVncWebSocketBridgeAsync(client, wsUrl, ticket, $"VNC: {vmTypeStrvv} {vmId}", output);
    }

    /// <summary>
    /// Prepares a SPICE .vv file for the given node shell and returns its path.
    /// </summary>
    public static async Task<(string? Error, string? FileName)>
        PrepareSpiceAsync(PveClient client, string node, string? proxy = null, TextWriter? output = null)
    {
        if (string.IsNullOrWhiteSpace(proxy)) { proxy = client.Host; }

        var (success, reasonPhrase, content) = await client.Nodes[node].Spiceshell.GetSpiceFileVVAsync(proxy);
        if (!success) { return (reasonPhrase, null); }

        content = await ReplaceProxyInContentAsync(content, proxy, output);

        var fileName = Path.GetTempFileName().Replace(".tmp", ".vv");
        await File.WriteAllTextAsync(fileName, content);
        return (null, fileName);
    }

    /// <summary>
    /// Opens a VNC WebSocket tunnel for the given node shell and prepares a .vv file.
    /// The caller must dispose the returned <see cref="VncWebSocketBridge"/> after the viewer exits.
    /// </summary>
    public static async Task<(string? Error, string? FileName, VncWebSocketBridge? Bridge)>
        PrepareVncAsync(PveClient client, string node, TextWriter? output = null)
    {
        var vncResult = await client.Nodes[node].Vncshell.Vncshell(websocket: true);

        if (!vncResult.IsSuccessStatusCode) { return (vncResult.ReasonPhrase, null, null); }

        var ticket = (string)vncResult.Response.data.ticket;
        var port = Convert.ToInt32(vncResult.Response.data.port);

        var wsUrl = NoVncHelper.GetWebsocketUrl(client.Host,
                                                client.Port,
                                                node,
                                                port,
                                                HttpUtility.UrlEncode(ticket));

        if (output != null) { await output.WriteLineAsync($"VNC WebSocket URL: {wsUrl}"); }

        return await CreateVncWebSocketBridgeAsync(client, wsUrl, ticket, $"VNC: Node {node}", output);
    }

    private static async Task<(string? Error, string? FileName, VncWebSocketBridge? Bridge)>
        CreateVncWebSocketBridgeAsync(PveClient client,
                                      string wsUrl,
                                      string ticket,
                                      string title,
                                      TextWriter? output = null)
    {
        var bridge = new VncWebSocketBridge();
        bridge.Start(wsUrl, client.Host, client.PVEAuthCookie);

        var vvContent = $"""
            [virt-viewer]
            type=vnc
            host=127.0.0.1
            port={bridge.LocalPort}
            password={ticket}
            title={title}
            delete-this-file=1
            """;

        var fileName = Path.GetTempFileName().Replace(".tmp", ".vv");
        await File.WriteAllTextAsync(fileName, vvContent);

        if (output != null) { await output.WriteLineAsync($"VNC local port: {bridge.LocalPort}"); }

        return (null, fileName, bridge);
    }

    private static async Task<string> ReplaceProxyInContentAsync(string content, string proxy, TextWriter? output)
    {
        if (HttpRegex().IsMatch(proxy))
        {
            var lines = content.Split("\n");
            for (var i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("proxy="))
                {
                    lines[i] = $"proxy={proxy}";
                    break;
                }
            }
            content = string.Join("\n", lines);

            if (output != null)
            {
                await output.WriteLineAsync($"Replace Proxy: {proxy}");
                await output.WriteLineAsync(content);
            }
        }

        return content;
    }

    /// <summary>
    /// Launches remote-viewer with the given .vv file.
    /// </summary>
    public static int Launch(string viewerPath,
                             string fileName,
                             string viewerOptions,
                             bool waitForExit,
                             TextWriter? output = null)
    {
        var startInfo = new ProcessStartInfo
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = output == null,
        };

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            startInfo.FileName = "/bin/bash";
            startInfo.Arguments = $"-c \"{viewerPath} {fileName} {viewerOptions}\"";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            startInfo.FileName = $"\"{viewerPath}\"";
            startInfo.Arguments = $"\"{fileName}\" {viewerOptions}";
        }

        if (output != null)
        {
            output.WriteLine($"Run FileName: {startInfo.FileName}");
            output.WriteLine($"Run Arguments: {startInfo.Arguments}");
        }

        var process = new Process { StartInfo = startInfo };
        process.Start();
        if (waitForExit) { process.WaitForExit(); }
        return !process.HasExited || process.ExitCode == 0 ? 0 : 1;
    }
}
