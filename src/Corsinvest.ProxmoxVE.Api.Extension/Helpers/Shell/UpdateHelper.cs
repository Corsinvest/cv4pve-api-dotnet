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

namespace Corsinvest.ProxmoxVE.Api.Extension.Helpers.Shell
{
    /// <summary>
    /// Update helper
    /// </summary>
    public class UpdateHelper
    {
        /// <summary>
        /// Get last release asset from GitHub
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static (DateTimeOffset? PublishedAt, string BrowserDownloadUrl, string ReleaseNotes, Version Version)
                       GetLastReleaseAssetFromGitHub(string appName)
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
        /// Upgrade from last release
        /// </summary>
        /// <param name="assetUrl"></param>
        /// <param name="fullNameApplication"></param>
        public static void Upgrade(string assetUrl, string fullNameApplication)
        {
            using (var client = new WebClient())
            {
                var zipFile = Path.GetTempFileName();

                //download file
                client.DownloadFile(new Uri(assetUrl), zipFile);

                //unzip
                using (var file = File.OpenRead(zipFile))
                using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
                {
                    foreach (var entry in zip.Entries) { entry.ExtractToFile(fullNameApplication); }
                }

                File.Delete(zipFile);
            }
        }
    }
}