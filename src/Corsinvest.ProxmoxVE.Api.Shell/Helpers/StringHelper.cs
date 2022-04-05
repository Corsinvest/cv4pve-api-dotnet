/*
 * SPDX-FileCopyrightText: 2022 Daniele Corsini <daniele.corsini@corsinvest.it>
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using System;
using System.Security.Cryptography;
using System.Text;

namespace Corsinvest.ProxmoxVE.Api.Shell.Helpers
{
    /// <summary>
    /// String helper
    /// </summary>
    internal static class StringHelper
    {
        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt(string data, string key)
        {
            var dataArray = Convert.FromBase64String(data);

            using var tDes = TripleDES.Create();
            tDes.Mode = CipherMode.ECB;
            tDes.Key = Encoding.UTF8.GetBytes(key);
            tDes.Padding = PaddingMode.PKCS7;

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
        /// <returns></returns>
        public static string Encrypt(string data, string key)
        {
            var dataArray = Encoding.UTF8.GetBytes(data);

            using var tDes = TripleDES.Create();
            tDes.Mode = CipherMode.ECB;
            tDes.Key = Encoding.UTF8.GetBytes(key);
            tDes.Padding = PaddingMode.PKCS7;

            using var cTransform = tDes.CreateEncryptor();
            var resultArray = cTransform.TransformFinalBlock(dataArray, 0, dataArray.Length);
            tDes.Clear();

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
    }
}