/*
 * This file is part of the cv4pve-api-dotnet https://github.com/Corsinvest/cv4pve-api-dotnet,
 *
 * This source file is available under two different licenses:
 * - GNU General Public License version 3 (GPLv3)
 * - Corsinvest Enterprise License (CEL)
 * Full copyright and license information is available in
 * LICENSE.md which is distributed with this source code.
 *
 * Copyright (C) 2016 Corsinvest Srl	GPLv3 and CEL
 */

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace Corsinvest.ProxmoxVE.Api.Shell.Helpers
{
    /// <summary>
    /// Update helper
    /// </summary>
    public class UpdateHelper
    {
        /// <summary>
        /// Get info web version
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static (string Info, bool IsNewVersion, string BrowserDownloadUrl) GetInfo(string appName)
        {
            var info = GetInfoLastReleaseAssetFromGitHub(appName);
            var currVer = ShellHelper.GetCurrentVersionApp();
            var isNewVersion = info.Version.ToString() != currVer;
            var msg = isNewVersion ?
                        $"New version available: {info.Version}" :
                        "You are already at the latest version";

            return ($@"===== In execution release:
Version:       {currVer}

===== Last release:
Version:       {info.Version}
Published At:  {info.PublishedAt}
Download Url:  {info.BrowserDownloadUrl}
Release Notes: {info.ReleaseNotes}

{msg}", isNewVersion, info.BrowserDownloadUrl);
        }

        /// <summary>
        /// Get info last release asset from GitHub
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static (DateTimeOffset? PublishedAt, string BrowserDownloadUrl, string ReleaseNotes, Version Version) GetInfoLastReleaseAssetFromGitHub(string appName)
        {
            (DateTimeOffset? PublishedAt,
             string BrowserDownloadUrl,
             string ReleaseNotes,
             Version Version) ret = (null, null, null, null);

            using (var client = new WebClient())
            {
                client.Headers.Add("User-Agent", appName);
                var url = $"https://api.github.com/repos/Corsinvest/{appName}/releases";
                dynamic releases = JsonConvert.DeserializeObject<IList<ExpandoObject>>(client.DownloadString(url));
                dynamic lastRelease = null;
                foreach (var release in releases)
                {
                    if (!release.prerelease)
                    {
                        lastRelease = release;
                        break;
                    }
                }

                if (lastRelease != null)
                {
                    ret.PublishedAt = lastRelease.published_at;
                    ret.ReleaseNotes = lastRelease.body;
                    ret.Version = new Version(lastRelease.tag_name.Substring(1));

                    var downloadEndWith = "";

                    //check platform
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) { downloadEndWith += "win-"; }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) { downloadEndWith += "linux-"; }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) { downloadEndWith += "osx-"; }

                    //check architecure
                    switch (RuntimeInformation.OSArchitecture)
                    {
                        case Architecture.Arm: downloadEndWith += "arm"; break;
                        case Architecture.Arm64: downloadEndWith += "arm64"; break;
                        case Architecture.X64: downloadEndWith += "x64"; break;
                        case Architecture.X86: downloadEndWith += "x86"; break;
                        default: break;
                    }

                    downloadEndWith += ".zip";

                    ret.BrowserDownloadUrl = ((IList<dynamic>)lastRelease.assets)
                                                    .Where(a => a.name.StartsWith(appName) &&
                                                                a.name.EndsWith(downloadEndWith))
                                                    .FirstOrDefault()?.browser_download_url;
                }
            }

            return ret;
        }

        /// <summary>
        /// Get file name app new
        /// </summary>
        /// <param name="fileNameApp"></param>
        /// <returns></returns>
        public static string GetFileNameAppNew(string fileNameApp)
            => Path.Combine(Path.GetDirectoryName(fileNameApp),
                            Path.GetFileNameWithoutExtension(fileNameApp) + "-new" +
                            Path.GetExtension(fileNameApp));

        /// <summary>
        /// Get file name app from new
        /// </summary>
        /// <param name="fileNameNew"></param>
        /// <returns></returns>
        public static string GetFileNameAppFromNew(string fileNameNew)
        {
            var filename = Path.GetFileNameWithoutExtension(fileNameNew);

            return Path.Combine(Path.GetDirectoryName(fileNameNew),
                                filename.Substring(0, filename.LastIndexOf("-new")) +
                                Path.GetExtension(fileNameNew));
        }

        /// <summary>
        /// Finish upgrade
        /// </summary>
        /// <param name="fileNameNew"></param>
        public static void UpgradeFinish(string fileNameNew)
        {
            var fileNameApp = GetFileNameAppFromNew(fileNameNew);
            File.Copy(fileNameNew, fileNameApp, true);
        }

        /// <summary>
        /// Prepare upgrade from last release
        /// </summary>
        /// <param name="assetUrl"></param>
        /// <param name="fileNameApp"></param>
        /// <returns>File name new version</returns>
        public static string UpgradePrepare(string assetUrl, string fileNameApp)
        {
            using var client = new WebClient();
            var zipFile = Path.GetTempFileName();

            //download file
            client.DownloadFile(new Uri(assetUrl), zipFile);

            var fileNameNew = GetFileNameAppNew(fileNameApp);
            if (File.Exists(fileNameNew)) { File.Delete(fileNameNew); }

            //unzip
            using var file = File.OpenRead(zipFile);
            using var zip = new ZipArchive(file, ZipArchiveMode.Read);
            foreach (var entry in zip.Entries) { entry.ExtractToFile(fileNameNew); }

            File.Delete(zipFile);

            return fileNameNew;
        }
    }
}