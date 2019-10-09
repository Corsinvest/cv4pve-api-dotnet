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
        public static bool IsNumeric(string value) => int.TryParse(value, out var vmId);

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="useHashing"></param>
        /// <returns></returns>
        public static string Decrypt(string data, string key, bool useHashing)
        {
            var keyArray = UTF8Encoding.UTF8.GetBytes(key);
            var dataArray = Convert.FromBase64String(data);

            if (useHashing)
            {
                using (var md5 = new MD5CryptoServiceProvider())
                {
                    keyArray = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    md5.Clear();
                }
            }

            using (var tDes = new TripleDESCryptoServiceProvider())
            {
                tDes.Key = keyArray;
                tDes.Mode = CipherMode.ECB;
                tDes.Padding = PaddingMode.PKCS7;

                using (var cTransform = tDes.CreateDecryptor())
                {
                    var resultArray = cTransform.TransformFinalBlock(dataArray, 0, dataArray.Length);
                    tDes.Clear();

                    return UTF8Encoding.UTF8.GetString(resultArray);
                }
            }
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
            var keyArray = UTF8Encoding.UTF8.GetBytes(key);
            var dataArray = UTF8Encoding.UTF8.GetBytes(data);

            if (useHashing)
            {
                using (var md5 = new MD5CryptoServiceProvider())
                {
                    keyArray = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    md5.Clear();
                }
            }

            using (var tDes = new TripleDESCryptoServiceProvider())
            {
                tDes.Key = keyArray;
                tDes.Mode = CipherMode.ECB;
                tDes.Padding = PaddingMode.PKCS7;

                using (var cTransform = tDes.CreateEncryptor())
                {
                    var resultArray = cTransform.TransformFinalBlock(dataArray, 0, dataArray.Length);
                    tDes.Clear();

                    return Convert.ToBase64String(resultArray, 0, resultArray.Length);
                }
            }
        }

        /// <summary>
        /// Tokenize CommandLine to list
        /// </summary>
        /// <param name="commandLine"></param>
        /// <returns></returns>
        public static List<string> TokenizeCommandLineToList(string commandLine)
        {
            var tokens = new List<string>();
            var token = new StringBuilder(255);
            var sections = commandLine.Split(' ');

            for (int curPart = 0; curPart < sections.Length; curPart++)
            {
                // We are in a quoted section!!
                if (sections[curPart].StartsWith("\""))
                {
                    // remove leading "
                    token.Append(sections[curPart].Substring(1));
                    var quoteCount = 0;

                    // Step backwards from the end of the current section to find the count of quotes from the end.
                    // This will exclude looking at the first character, which was the " that got us here in the first place.
                    for (; quoteCount < sections[curPart].Length - 1; quoteCount++)
                    {
                        if (sections[curPart][sections[curPart].Length - 1 - quoteCount] != '"') { break; }
                    }

                    // if we didn't have a leftover " (i.e. the 2N+1), then we should 
                    // continue adding in the next section to the current token.
                    while (quoteCount % 2 == 0 && (curPart != sections.Length - 1))
                    {
                        quoteCount = 0;
                        curPart++;

                        //Step backwards from the end of the current token to find the count of quotes from the end.
                        for (; quoteCount < sections[curPart].Length; quoteCount++)
                        {
                            if (sections[curPart][sections[curPart].Length - 1 - quoteCount] != '"') { break; }
                        }

                        token.Append(' ').Append(sections[curPart]);
                    }

                    // remove trailing " if we had a leftover
                    // if we didn't have a leftover then we go to the end of the command line without an enclosing " 
                    // so it gets treated as a quoted argument anyway
                    if (quoteCount % 2 != 0) { token.Remove(token.Length - 1, 1); }
                    token.Replace("\"\"", "\"");
                }
                else
                {
                    //Not a quoted section so this is just a boring parameter
                    token.Append(sections[curPart]);
                }

                //strip whitespace (because).
                if (!string.IsNullOrEmpty(token.ToString().Trim())) { tokens.Add(token.ToString().Trim()); }

                token.Clear();
            }

            //return the array in the same format args[] usually turn up to main in.
            return tokens;
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
    }
}