/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using System.ComponentModel;

namespace Corsinvest.ProxmoxVE.Api.Shared.Utils;

/// <summary>
/// NoVnc Helper
/// </summary>
public static class NoVncHelper
{
    /// <summary>
    /// Get NoVnc Console type
    /// </summary>
    public static string GetConsoleType(VmType vmType)
        => vmType switch
        {
            VmType.Qemu => "kvm",
            VmType.Lxc => "lxc",
            _ => throw new InvalidEnumArgumentException(),
        };

    /// <summary>
    /// Get urn NoVncConsole
    /// </summary>
    public static string GetConsoleUrl(string host,
                                   int port,
                                   string console,
                                   string node,
                                   long vmId,
                                   string vmName,
                                   bool noVnc,
                                   bool xtermJs,
                                   string parameters = null)
    {
        var url = $"https://{host}:{port}/?console={console}&vmid={vmId}&vmname={vmName}&node={node}";
        if (noVnc) { url += "&novnc=1"; }
        if (xtermJs) { url += "&xtermjs=1"; }
        if (!string.IsNullOrEmpty(parameters)) { url += parameters; }
        return url;
    }

    /// <summary>
    /// Get Vnc Websocket Url VM/CT
    /// </summary>
    public static string GetWebsocketUrl(string host, int port, string node, string type, long vmId, int vncPort, string ticket)
        => $"wss://{host}:{port}/api2/json/nodes/{node}/{type}/{vmId}/vncwebsocket?port={vncPort}&vncticket={ticket}";

    /// <summary>
    /// Get Vnc Websocket Url Node
    /// </summary>
    public static string GetWebsocketUrl(string host, int port, string node, int vncPort, string ticket)
        => $"wss://{host}:{port}/api2/json/nodes/{node}/vncwebsocket?port={vncPort}&vncticket={ticket}";
}