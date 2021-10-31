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
using System.IO;

namespace Corsinvest.ProxmoxVE.Api.Extension.Helpers
{
    /// <summary>
    /// Application helper
    /// </summary>
    public class ApplicationHelper
    {
        /// <summary>
        /// Get application data directory. If not exists create.
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static string GetApplicationDataDirectory(string appName)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Corsinvest", appName);
            if (!Directory.Exists(path))
            {
                var dir = Directory.CreateDirectory(path);
                //dir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }

            return path;
        }
    }
}