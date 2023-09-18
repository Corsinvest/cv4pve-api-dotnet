/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using System.ComponentModel;

namespace Corsinvest.ProxmoxVE.Api.Shared.Utils
{
    /// <summary>
    /// NoVnc Helper
    /// </summary>
    public static class NoVncHelper
    {
        /// <summary>
        /// Get NoVnc Console type
        /// </summary>
        /// <param name="vmType"></param>
        /// <returns></returns>
        /// <exception cref="InvalidEnumArgumentException"></exception>
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
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="console"></param>
        /// <param name="node"></param>
        /// <param name="vmId"></param>
        /// <param name="vmName"></param>
        /// <returns></returns>
        public static string GetConsoleUrl(string host, int port, string console, string node, long vmId, string vmName)
            => $"https://{host}:{port}/?console={console}&vmid={vmId}&vmname={vmName}&node={node}&resize=scale&novnc=1";

        /// <summary>
        /// Get Vnc Websocket Url VM/CT
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="node"></param>
        /// <param name="type"></param>
        /// <param name="vmId"></param>
        /// <param name="vncPort"></param>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public static string GetWebsocketUrl(string host, int port, string node, string type, long vmId, int vncPort, string ticket)
            => $"wss://{host}:{port}/api2/json/nodes/{node}/{type}/{vmId}/vncwebsocket?port={vncPort}&vncticket={ticket}";

        /// <summary>
        /// Get Vnc Websocket Url Node
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="node"></param>
        /// <param name="vncPort"></param>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public static string GetWebsocketUrl(string host, int port, string node, int vncPort, string ticket)
            => $"wss://{host}:{port}/api2/json/nodes/{node}/vncwebsocket?port={vncPort}&vncticket={ticket}";
    }
}