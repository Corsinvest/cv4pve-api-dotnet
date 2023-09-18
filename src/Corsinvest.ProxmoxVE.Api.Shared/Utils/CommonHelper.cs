/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using System;
using System.IO;

namespace Corsinvest.ProxmoxVE.Api.Shared.Utils
{
    /// <summary>
    /// Application helper
    /// </summary>
    public static class CommonHelper
    {
        /// <summary>
        /// Get application data directory. If not exists create.
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static string GetApplicationDataDirectory(string appName)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Corsinvest", appName);
            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            return path;
        }
    }
}