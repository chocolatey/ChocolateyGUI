// <copyright file="NugetEncryptionUtility.cs" company="Chocolatey">
// Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>

namespace ChocolateyGui.Common.Providers
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using NuGet;

    // ReSharper disable InconsistentNaming
    public static class NugetEncryptionUtility
    {
        private static readonly byte[] _entropyBytes = Encoding.UTF8.GetBytes("Chocolatey");

        public static string GenerateUniqueToken(string caseInsensitiveKey)
        {
            // SHA256 is case sensitive; given that our key is case insensitive, we upper case it
            var pathBytes = Encoding.UTF8.GetBytes(caseInsensitiveKey.ToUpperInvariant());
            var hashProvider = new CryptoHashProvider("SHA256");

            return Convert.ToBase64String(hashProvider.CalculateHash(pathBytes)).ToUpperInvariant();
        }

        public static string EncryptString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var decryptedByteArray = Encoding.UTF8.GetBytes(value);
            var encryptedByteArray = ProtectedData.Protect(decryptedByteArray, _entropyBytes, DataProtectionScope.LocalMachine);
            var encryptedString = Convert.ToBase64String(encryptedByteArray);
            return encryptedString;
        }

        public static string DecryptString(string encryptedString)
        {
            var encryptedByteArray = Convert.FromBase64String(encryptedString);
            var decryptedByteArray = ProtectedData.Unprotect(encryptedByteArray, _entropyBytes, DataProtectionScope.LocalMachine);
            return Encoding.UTF8.GetString(decryptedByteArray);
        }
    }

    // ReSharper restore InconsistentNaming
}