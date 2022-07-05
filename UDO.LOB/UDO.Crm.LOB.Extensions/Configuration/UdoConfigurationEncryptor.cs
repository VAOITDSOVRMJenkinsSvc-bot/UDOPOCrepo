namespace UDO.LOB.Extensions
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Security.Cryptography;

    /// <summary>
    /// Static helper methods for encrypting and decrypting UDO configuration values.
    /// </summary>
    public static class UdoConfigurationEncryptor
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly static UdoSafe<byte[]> _iv = new UdoSafe<byte[]>(new byte[] { 159, 219, 44, 48, 167, 249, 112, 85, 249, 212, 208, 200, 216, 133, 33, 81 });

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly static UdoSafe<byte[]> _key = new UdoSafe<byte[]>(new byte[] { 213, 162, 133, 210, 22, 208, 116, 195, 171, 80, 10, 61, 118, 71, 129, 200, 188, 1, 33, 66, 150, 191, 42, 112, 122, 244, 42, 7, 79, 249, 56, 107 });

        /// <summary>
        /// Encrypts the specified UDO configuration value.
        /// If the value is null or an empty string "", then null is returned.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The encrypted UDO configuration value or null.</returns>
        public static string Encrypt(string value)
        {
            var trimmedNullableValue = value.AsTrimmedNullableString();
            if (trimmedNullableValue is null)
            {
                return null;
            }

            SymmetricAlgorithm aesAlgorithm = null;
            string result = null;
            try
            {
                aesAlgorithm = SymmetricAlgorithm.Create("AES");
                aesAlgorithm.Key = _key.Value;
                aesAlgorithm.IV = _iv.Value;
                var memoryStream = new MemoryStream();
                var encryptor = aesAlgorithm.CreateEncryptor();
                var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
                using (var streamWriter = new StreamWriter(cryptoStream))
                {
                    streamWriter.Write(trimmedNullableValue);
                }

                result = Convert.ToBase64String(memoryStream.ToArray(), Base64FormattingOptions.None);
            }
            finally
            {
                if (aesAlgorithm != null)
                {
                    aesAlgorithm.Clear();
                    aesAlgorithm.Dispose();
                }
            }

            return result;
        }

        /// <summary>
        /// Decrypts the specified UDO configuration value.
        /// If the value is null or an empty string "", then a null is returned.
        /// </summary>
        /// <param name="encryptedValue">The UDO configuration value. Note: can be null or empty string "".</param>
        /// <returns>The decrypted UDO configuration value or null.</returns>
        public static string Decrypt(string encryptedValue)
        {
            var stream = DecryptToStream(encryptedValue);
            if (stream == null)
            {
                return null;
            }

            using (var streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }

        /// <summary>
        /// Decrypts the specified UDO configuration value.
        /// If the value is null or an empty string "", then a null is returned.
        /// </summary>
        /// <param name="encryptedValue">The UDO configuration value. Note: can be null or empty string "".</param>
        /// <returns>The <see cref="Stream"/> that represents decrypted UDO configuration value or null.</returns>
        public static Stream DecryptToStream(string encryptedValue)
        {
            var trimmedNullableEncryptedValue = encryptedValue.AsTrimmedNullableString();
            if (trimmedNullableEncryptedValue is null)
            {
                return null;
            }

            SymmetricAlgorithm aesAlgorithm = null;
            try
            {
                aesAlgorithm = SymmetricAlgorithm.Create("AES");
                aesAlgorithm.Key = _key.Value;
                aesAlgorithm.IV = _iv.Value;
                var memoryStream = new MemoryStream(Convert.FromBase64String(trimmedNullableEncryptedValue));
                var decryptor = aesAlgorithm.CreateDecryptor();
                return new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            }
            finally
            {
                if (aesAlgorithm != null)
                {
                    aesAlgorithm.Clear();
                    aesAlgorithm.Dispose();
                }
            }
        }
    }
}
