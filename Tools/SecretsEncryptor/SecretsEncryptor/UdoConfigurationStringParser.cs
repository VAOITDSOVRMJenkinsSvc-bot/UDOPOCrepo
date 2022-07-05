namespace SecretsEncryptor
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using Newtonsoft.Json;

    public static class UdoConfigurationStringParser
    {
        public static void Parse(string encryptedConfigurationString)
        {
            var notNullKeyVaultConnectionSettings = new InternalUdoKVHelperConfig();
            byte[] key = new byte[] { 213, 162, 133, 210, 22, 208, 116, 195, 171, 80, 10, 61, 118, 71, 129, 200, 188, 1, 33, 66, 150, 191, 42, 112, 122, 244, 42, 7, 79, 249, 56, 107 };
            byte[] iv = new byte[] { 159, 219, 44, 48, 167, 249, 112, 85, 249, 212, 208, 200, 216, 133, 33, 81 };

            SymmetricAlgorithm aesAlgorithm = null;
            try
            {
                aesAlgorithm = SymmetricAlgorithm.Create("AES");
                aesAlgorithm.Key = key;
                aesAlgorithm.IV = iv;

                using (var memoryStream = new MemoryStream(Convert.FromBase64String(encryptedConfigurationString)))
                {
                    var decryptor = aesAlgorithm.CreateDecryptor();
                    var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                    var streamReader = new StreamReader(cryptoStream);

                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        jsonTextReader.Read();
                        notNullKeyVaultConnectionSettings.SetUrl(jsonTextReader.ReadAsString());
                        notNullKeyVaultConnectionSettings.SetClientId(jsonTextReader.ReadAsString());
                        notNullKeyVaultConnectionSettings.SetClientSecret(jsonTextReader.ReadAsString());
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
        }
    }
}
