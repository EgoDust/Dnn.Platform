﻿

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ClientDependency.Core
{
    public static class StringExtensions
    {
        public static string EncodeTo64Url(this string toEncode)
        {
            string returnValue = EncodeTo64(toEncode);

            // returnValue is base64 = may contain a-z, A-Z, 0-9, +, /, and =.
            // the = at the end is just a filler, can remove
            // then convert the + and / to "base64url" equivalent
            //
            returnValue = returnValue.TrimEnd(new char[] { '=' });
            returnValue = returnValue.Replace("+", "-");
            returnValue = returnValue.Replace("/", "_");

            return returnValue;
        }

        public static string EncodeTo64(this string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.Default.GetBytes(toEncode);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }
        public static string DecodeFrom64Url(this string toDecode)
        {
            // see BaseFileRegistrationProvider.EncodeTo64Url
            //
            toDecode = toDecode.Replace("-", "+");
            toDecode = toDecode.Replace("_", "/");
            int rem = toDecode.Length % 4; // 0 (aligned), 1, 2 or 3 (not aligned)
            if (rem > 0)
                toDecode = toDecode.PadRight(toDecode.Length + 4 - rem, '='); // align

            return DecodeFrom64(toDecode);
        }

        public static string DecodeFrom64(this string toDecode)
        {
            byte[] toDecodeAsBytes = System.Convert.FromBase64String(toDecode);
            return Encoding.Default.GetString(toDecodeAsBytes);
        }

        /// <summary>
        /// Generates a hash of a string based on the FIPS compliance setting.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GenerateHash(this string str)
        {
            try
            {
                return CryptoConfig.AllowOnlyFipsAlgorithms
                    ? str.GenerateSha256Hash()
                    : str.GenerateMd5();
            }
            catch (Exception)
            {
                //default to MD5
                return str.GenerateMd5();
            }
        }

        /// <summary>
        /// Generate a SHA256 hash of a string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GenerateSha256Hash(this string str)
        {
            using (var hasher = new SHA256CryptoServiceProvider())
            {
                var byteArray = hasher.ComputeHash(Encoding.Unicode.GetBytes(str));
                return byteArray.Aggregate("", (current, b) => current + b.ToString("x2"));
            }
        }

        /// <summary>Generate a MD5 hash of a string
        /// </summary>
        public static string GenerateMd5(this string str)
        {
            using (var hasher = new MD5CryptoServiceProvider())
            {
                var byteArray = hasher.ComputeHash(Encoding.Unicode.GetBytes(str));
                return byteArray.Aggregate("", (current, b) => current + b.ToString("x2"));
            }
        }

        /// <summary>
        /// checks if the string ends with one of the strings specified. This ignores case.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool EndsWithOneOf(this string str, string[] ext)
        {
            var upper = str.ToUpper();
            bool isExt = false;
            foreach (var e in ext)
            {
                if (upper.EndsWith(e.ToUpper()))
                {
                    isExt = true;
                    break;
                }
            }
            return isExt;
        }

        public static string TextOrEmpty(this string text)
        {
            return text ?? string.Empty;
        }
    }
}
