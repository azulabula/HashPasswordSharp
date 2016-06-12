﻿using System;
using System.Security.Cryptography;
using System.Text;

namespace JaSt.HashPasswordSharp.Library
{
    /// <summary>
    /// This utility class contains static methods for generating passwords from passphrases using different hashing algorithms.
    /// </summary>
    /// <remarks>Algorithm: Juergen Busch</remarks>
    public static class HashUtil
    {
        /// <summary>
        /// Enumeration defines all supported hashalgorithms in HashPasswordSharp.
        /// </summary>
        public enum SupportedHashAlgorithm { MD5, SHA1, SHA256, SHA384, SHA512 }

        /// <summary>
        /// Method for generating password from the input values.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="login"></param>
        /// <param name="passphrase"></param>
        /// <param name="algorithm"></param>
        /// <param name="characterSet"></param>
        /// <param name="maxPwLength"></param>
        /// <returns>Generated password</returns>
        public static string GeneratePassword(string host, string login, string passphrase, SupportedHashAlgorithm algorithm, string characterSet, int maxPwLength)
        {
            var hashAlgorithm = CryptoConfig.CreateFromName(algorithm.ToString()) as HashAlgorithm;

            var basestring = string.IsNullOrWhiteSpace(login) ? $"{passphrase}@{host}" : $"{login}@{host}#{passphrase}";
            var digest = CreateHash(hashAlgorithm, basestring);

            var digestLength = digest.Length;
            var sPassword = string.Empty;

            var pos = 0;
            var bitno = 0;
            var charSetLength = characterSet.Length;

            var maxBitCnt = (int)Math.Ceiling(Math.Log(charSetLength) / Math.Log(2));

            for (var i = 0; (i < maxPwLength) && (pos * 8 + bitno < digestLength * 8); ++i)
            {
                var part = 0;
                var bitCnt = maxBitCnt;
                var actPos = pos;
                var actBitno = bitno;

                for (var j = 0; (j < bitCnt) && (actPos * 8 + actBitno < digestLength * 8); ++j)
                {
                    part <<= 1;
                    part |= (digest[actPos] & (1 << actBitno)) != 0 ? 1 : 0;
                    if (++actBitno >= 8)
                    {
                        ++actPos;
                        actBitno = 0;
                    }
                }

                if (part >= charSetLength)
                {
                    part >>= 1;
                    --actBitno;
                    if (actBitno < 0)
                    {
                        --actPos;
                        actBitno = 7;
                    }
                }

                bitno = actBitno;
                pos = actPos;

                sPassword = sPassword + characterSet[part];
            }

            return sPassword;
        }

        /// <summary>
        /// This method creates a hash of the given string using the given hashalgorithm.
        /// </summary>
        /// <param name="hashAlgorithm"></param>
        /// <param name="basestring"></param>
        /// <returns>Hash from basestring</returns>
        private static byte[] CreateHash(HashAlgorithm hashAlgorithm, string basestring)
        {
            // byte array representation of that string
            var encodedPassword = new UTF8Encoding().GetBytes(basestring);

            // need MD5 to calculate the hash
            var hash = hashAlgorithm.ComputeHash(encodedPassword);

            return hash;
        }
    }
}
