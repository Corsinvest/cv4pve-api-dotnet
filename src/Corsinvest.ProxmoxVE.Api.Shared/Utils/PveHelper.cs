/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api.Shared.Models.Node;
using Corsinvest.ProxmoxVE.Api.Shared.Models.Vm;
using System.ComponentModel;
using System.Web;

namespace Corsinvest.ProxmoxVE.Api.Shared.Utils
{
    /// <summary>
    /// Node helper
    /// </summary>
    public static class PveHelper
    {
        /// <summary>
        /// Get NoVnc Console type
        /// </summary>
        /// <param name="vmType"></param>
        /// <returns></returns>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public static string GetNoVncConsoleType(VmType vmType)
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
        public static string GetNoVncConsoleUrl(string host, int port, string console, string node, long vmId, string vmName)
            => $"https://{host}:{port}/?console={console}&vmid={vmId}&vmname={vmName}&node={node}&resize=scale&novnc=1";

        /// <summary>
        /// Get download spice vv file
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="node"></param>
        /// <param name="vmId"></param>
        /// <returns></returns>
        public static string GetSpiceUrlFileVV(string host, int port, string node, long vmId)
            => $"https://{host}:{port}/api2/spiceconfig/nodes/{node}/qemu/{vmId}/spiceproxy";

        /// <summary>
        /// Get Vnc Websocket Url
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="node"></param>
        /// <param name="type"></param>
        /// <param name="vmId"></param>
        /// <param name="vncport"></param>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public static string GetVncWebsocketUrl(string host, int port, string node, string type, long vmId, int vncport, string ticket)
            => $"wss://{host}:{port}/api2/json/nodes/{node}/{type}/{vmId}/vncwebsocket?port={vncport}&vncticket={ticket}";

        /// <summary>
        /// Url download file
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="node"></param>
        /// <param name="storage"></param>
        /// <param name="volume"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetDownloadBackupFileUrl(string host, int port, string node, string storage, string volume, string filePath)
            => $"https://{host}:{port}/api2/json/nodes/{node}/storage/{storage}/file-restore/download?" +
               $"volume={HttpUtility.UrlEncode(volume)}&filepath={HttpUtility.UrlEncode(filePath)}";

        /// <summary>
        /// File name download file. IF folder add zip
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetDownloadBackupFileName(string type, string fileName)
            => type == "d"
                ? fileName + ".zip"  //for folder add zip compression
                : fileName;

        /// <summary>
        /// Decode level support
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static NodeLevel DecodeLevelSupport(string level)
            => level switch
            {
                "c" => NodeLevel.Community,
                "p" => NodeLevel.Premium,
                "b" => NodeLevel.Basic,
                "s" => NodeLevel.Standard,
                _ => NodeLevel.None,
            };
    }
}