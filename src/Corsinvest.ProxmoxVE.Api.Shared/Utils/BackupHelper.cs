/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using System.Web;

namespace Corsinvest.ProxmoxVE.Api.Shared.Utils
{
    /// <summary>
    /// Backup Helper
    /// </summary>
    public static class BackupHelper
    {
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
        public static string GetDownloadFileUrl(string host, int port, string node, string storage, string volume, string filePath)
            => $"https://{host}:{port}/api2/json/nodes/{node}/storage/{storage}/file-restore/download?" +
               $"volume={HttpUtility.UrlEncode(volume)}&filepath={HttpUtility.UrlEncode(filePath)}";

        /// <summary>
        /// File name download file. IF folder add zip
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetDownloadFileName(string type, string fileName)
            => type == "d"
                ? fileName + ".zip"  //for folder add zip compression
                : fileName;
    }
}