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

using System.Collections.Generic;

namespace Corsinvest.ProxmoxVE.Api.Extension.Helpers
{
    /// <summary>
    /// Dynamic helper
    /// </summary>
    public class DynamicHelper
    {
        /// <summary>
        /// Check or create value
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        public static void CheckKeyOrCreate(dynamic obj, string key, object defaultValue = null)
        {
            var dic = (IDictionary<string, object>)obj;
            if (!(dic.ContainsKey(key))) { dic.Add(key, defaultValue); }
        }

        /// <summary>
        /// Get value
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static object GetValue(dynamic obj, string key, object defaultValue = null)
        {
            var dic = (IDictionary<string, object>)obj;
            return dic.TryGetValue(key, out var value) ? value : defaultValue;
        }
    }
}