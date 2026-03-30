/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: MIT
 */

using System.Web;

namespace Corsinvest.ProxmoxVE.Api.Shared.Utils;

/// <summary>
/// Backup Helper
/// </summary>
public static class BackupHelper
{
    /// <summary>
    /// Url download file
    /// </summary>
    public static string GetDownloadFileUrl(string host, int port, string node, string storage, string volume, string filePath)
        => $"https://{host}:{port}/api2/json/nodes/{node}/storage/{storage}/file-restore/download?" +
           $"volume={HttpUtility.UrlEncode(volume)}&filepath={HttpUtility.UrlEncode(filePath)}";

    /// <summary>
    /// File name download file. IF folder add zip
    /// </summary>
    public static string GetDownloadFileName(string type, string fileName)
        => type == "d"
            ? fileName + ".zip"  //for folder add zip compression
            : fileName;
}