using System;
using System.Security.Cryptography;
using System.Text;

namespace Corsinvest.ProxmoxVE.Api.Extension.Utils
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
    }
}