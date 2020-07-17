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
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Corsinvest.ProxmoxVE.Api.Extension.Helpers
{
    /// <summary>
    /// String helper
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// New line Unix
        /// </summary>
        /// <value></value>
        public static char NewLineUnix => '\n';

        /// <summary>
        /// New line Windows
        /// </summary>
        /// <value></value>
        public static string NewLineWindows => "\r\n";

        /// <summary>
        /// Is numeric
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumeric(string value) => int.TryParse(value, out _);

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="useHashing"></param>
        /// <returns></returns>
        public static string Decrypt(string data, string key, bool useHashing)
        {
            var keyArray = Encoding.UTF8.GetBytes(key);
            var dataArray = Convert.FromBase64String(data);

            if (useHashing)
            {
                using var md5 = new MD5CryptoServiceProvider();
                keyArray = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
                md5.Clear();
            }

            using var tDes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            using var cTransform = tDes.CreateDecryptor();
            var resultArray = cTransform.TransformFinalBlock(dataArray, 0, dataArray.Length);
            tDes.Clear();

            return Encoding.UTF8.GetString(resultArray);
        }

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="useHashing"></param>
        /// <returns></returns>
        public static string Encrypt(string data, string key, bool useHashing)
        {
            var keyArray = Encoding.UTF8.GetBytes(key);
            var dataArray = Encoding.UTF8.GetBytes(data);

            if (useHashing)
            {
                using var md5 = new MD5CryptoServiceProvider();
                keyArray = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
                md5.Clear();
            }

            using var tDes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            using var cTransform = tDes.CreateEncryptor();
            var resultArray = cTransform.TransformFinalBlock(dataArray, 0, dataArray.Length);
            tDes.Clear();

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// Get argument into command start "{" end "}"
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string[] GetArgumentTags(string command)
            => new Regex(@"{\s*(.+?)\s*}").Matches(command)
                                          .OfType<Match>()
                                          .Where(a => a.Success)
                                          .Select(a => a.Groups[1].Value)
                                          .ToArray();

        /// <summary>
        /// Create argument tag
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string CreateArgumentTag(string name) => "{" + name + "}";

        /// <summary>
        /// Quote value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Quote(string value) => $"\"{value}\"";
    }
}