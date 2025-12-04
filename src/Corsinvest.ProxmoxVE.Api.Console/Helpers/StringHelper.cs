/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using System.Security.Cryptography;
using System.Text;

namespace Corsinvest.ProxmoxVE.Api.Console.Helpers;

internal static class StringHelper
{
    private const string VERSION_PREFIX = "v2:";

    public static string Decrypt(string data, string key)
    {
        if (data.StartsWith(VERSION_PREFIX))
        {
            // New format with version prefix - uses CBC mode with IV
            var actualData = data[VERSION_PREFIX.Length..];
            var dataArray = Convert.FromBase64String(actualData);

            // Extract IV from the beginning of the data
            var iv = new byte[8];
            Array.Copy(dataArray, 0, iv, 0, 8);

            // Extract encrypted content
            var encryptedData = new byte[dataArray.Length - 8];
            Array.Copy(dataArray, 8, encryptedData, 0, dataArray.Length - 8);

            using var tDes = TripleDES.Create();
            tDes.Mode = CipherMode.CBC;
            tDes.Key = Encoding.UTF8.GetBytes(key);
            tDes.IV = iv;
            tDes.Padding = PaddingMode.PKCS7;

            using var cTransform = tDes.CreateDecryptor();
            var resultArray = cTransform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            tDes.Clear();

            return Encoding.UTF8.GetString(resultArray);
        }
        else
        {
            // Old format without version prefix - uses ECB mode
            #pragma warning disable CA5351 // Accettabile per retrocompatibilità con formato dati esistenti
            var dataArray = Convert.FromBase64String(data);

            using var tDes = TripleDES.Create();
            tDes.Mode = CipherMode.ECB;
            tDes.Key = Encoding.UTF8.GetBytes(key);
            tDes.Padding = PaddingMode.PKCS7;

            using var cTransform = tDes.CreateDecryptor();
            var resultArray = cTransform.TransformFinalBlock(dataArray, 0, dataArray.Length);
            tDes.Clear();

            #pragma warning restore CA5351
            return Encoding.UTF8.GetString(resultArray);
        }
    }

    public static string Encrypt(string data, string key)
    {
        var dataArray = Encoding.UTF8.GetBytes(data);

        using var tDes = TripleDES.Create();
        tDes.Mode = CipherMode.CBC;
        tDes.Key = Encoding.UTF8.GetBytes(key);
        tDes.GenerateIV();
        tDes.Padding = PaddingMode.PKCS7;

        using var cTransform = tDes.CreateEncryptor();
        var resultArray = cTransform.TransformFinalBlock(dataArray, 0, dataArray.Length);

        // Combine IV and encrypted data
        var combined = new byte[tDes.IV.Length + resultArray.Length];
        Array.Copy(tDes.IV, 0, combined, 0, tDes.IV.Length);
        Array.Copy(resultArray, 0, combined, tDes.IV.Length, resultArray.Length);

        tDes.Clear();
        return VERSION_PREFIX + Convert.ToBase64String(combined, 0, combined.Length);
    }

    /// <summary>
    /// Get the version prefix used for new encrypted data
    /// </summary>
    /// <returns></returns>
    public static string GetVersionPrefix() => VERSION_PREFIX;
}