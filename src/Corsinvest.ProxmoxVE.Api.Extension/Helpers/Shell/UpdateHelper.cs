/*
 * This file is part of the cv4pve-api-dotnet https://github.com/Corsinvest/cv4pve-api-dotnet,
 * Copyright (C) 2016 Corsinvest Srl
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
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
        public static (DateTimeOffset? PublishedAt, string BrowserDownloadUrl, string ReleaseNotes, Version Version) GetLastReleaseAssetFromGitHub(string appName)
        {
            (DateTimeOffset? PublishedAt, string BrowserDownloadUrl, string ReleaseNotes, Version Version) ret = (null, null, null, null);

            using (var client = new WebClient())
            {
                client.Headers.Add("User-Agent", appName);
                var url = $"https://api.github.com/repos/Corsinvest/{appName}/releases";
                dynamic releases = JsonConvert.DeserializeObject<IList<ExpandoObject>>(client.DownloadString(url));
                dynamic lastRelease = null;
                foreach (var release in releases)
                {
                    if (!release.prerelease) { lastRelease = release; }
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