namespace SecretsEncryptor
{
    using System;
    using System.IO;
    using System.Security.Cryptography;

    public static class UdoEncrypter
    {
        private static readonly byte[] _key = new byte[] { 213, 162, 133, 210, 22, 208, 116, 195, 171, 80, 10, 61, 118, 71, 129, 200, 188, 1, 33, 66, 150, 191, 42, 112, 122, 244, 42, 7, 79, 249, 56, 107 };
        private static readonly byte[] _iv = new byte[] { 159, 219, 44, 48, 167, 249, 112, 85, 249, 212, 208, 200, 216, 133, 33, 81 };

        public static string Encrypt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            SymmetricAlgorithm aesAlgorithm = null;
            var result = string.Empty;
            try
            {
                aesAlgorithm = SymmetricAlgorithm.Create("AES");
                aesAlgorithm.Key = _key;
                aesAlgorithm.IV = _iv;

                using (var memoryStream = new MemoryStream())
                {
                    var encryptor = aesAlgorithm.CreateEncryptor();
                    var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

                    using (var streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(value);
                    }

                    result = Convert.ToBase64String(memoryStream.ToArray(), Base64FormattingOptions.None);
                }
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

        public static string Decrypt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            SymmetricAlgorithm aesAlgorithm = null;
            var result = string.Empty;
            try
            {
                aesAlgorithm = SymmetricAlgorithm.Create("AES");
                aesAlgorithm.Key = _key;
                aesAlgorithm.IV = _iv;

                using (var memoryStream = new MemoryStream(Convert.FromBase64String(value)))
                {
                    var decryptor = aesAlgorithm.CreateDecryptor();
                    var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

                    using (var streamReader = new StreamReader(cryptoStream))
                    {
                        result = streamReader.ReadToEnd();
                    }
                }
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
    }
}
