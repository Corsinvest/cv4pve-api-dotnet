/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using System.Security.Cryptography;
using System.Text;

namespace Corsinvest.ProxmoxVE.Api.Console.Helpers;

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
        tDes.Mode = CipherMode.CBC;
        tDes.Padding = PaddingMode.PKCS7;
        tDes.Key = Encoding.UTF8.GetBytes(key).Length >= 24 ? Encoding.UTF8.GetBytes(key)[..24] : throw new ArgumentException("Key must be at least 24 bytes for TripleDES");

        // The first 8 bytes are the IV
        var iv = new byte[8];
        Array.Copy(dataArray, 0, iv, 0, iv.Length);
        tDes.IV = iv;

        using var cTransform = tDes.CreateDecryptor();
        var resultArray = cTransform.TransformFinalBlock(dataArray, iv.Length, dataArray.Length - iv.Length);
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
        tDes.Mode = CipherMode.CBC;
        tDes.Key = Encoding.UTF8.GetBytes(key).Length >= 24 ? Encoding.UTF8.GetBytes(key)[..24] : throw new ArgumentException("Key must be at least 24 bytes for TripleDES");
        tDes.GenerateIV();
        tDes.Padding = PaddingMode.PKCS7;

        using var cTransform = tDes.CreateEncryptor();
        var resultArray = cTransform.TransformFinalBlock(dataArray, 0, dataArray.Length);
        tDes.Clear();

        // Combine IV and ciphertext
        var ivAndCipher = new byte[tDes.IV.Length + resultArray.Length];
        Array.Copy(tDes.IV, 0, ivAndCipher, 0, tDes.IV.Length);
        Array.Copy(resultArray, 0, ivAndCipher, tDes.IV.Length, resultArray.Length);

        return Convert.ToBase64String(ivAndCipher, 0, ivAndCipher.Length);
    }
}