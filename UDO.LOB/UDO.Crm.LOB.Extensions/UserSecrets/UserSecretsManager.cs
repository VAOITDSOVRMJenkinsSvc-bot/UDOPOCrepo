namespace UDO.LOB.Extensions
{
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Text.Json;
    using Microsoft.Extensions.Configuration;

    public static class UserSecretsManager
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const string UserSecretsId = "1e01257a-3171-4d0a-a517-045dd867cf1f";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static ConcurrentDictionary<string, string> _secrets = new ConcurrentDictionary<string, string>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public const string UdoAzureEnvironmentKey = "UdoAzureEnvironment";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public const string UdoConfigurationKey = "UdoConfiguration";

        private static JsonDocumentOptions _jsonDocumentOptions = new JsonDocumentOptions
        {
            AllowTrailingCommas = false,
            CommentHandling = JsonCommentHandling.Skip
        };

        /// <summary>
        /// Gets the user secrets.
        /// </summary>
        /// <returns></returns>
        public static ConcurrentDictionary<string, string> GetUserSecrets()
        {
            try
            {
                var keyValuePairs = new ConfigurationBuilder()
                    .AddUserSecrets(UserSecretsId)
                    .Build()
                    .AsEnumerable();

                _secrets.Clear();

                foreach (var keyValuePair in keyValuePairs)
                {
                    _secrets[keyValuePair.Key] = keyValuePair.Value;
                }

                return _secrets;
            }
            catch
            {
                // do not throw an exception in this case,
                // just assume that the user is not using UserSecrets
                // and therefore is likely running in Azure, so use KeyVault.
                return null;
            }
        }

        /// <summary>
        /// Gets the decrypted value by key.
        /// </summary>
        /// <param name="encryptedSecrets">The encrypted secrets.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string GetDecryptedValueByKey(ConcurrentDictionary<string, string> encryptedSecrets, string key)
        {
            var decryptedValues = GetDecryptedEnvironmentSecrets(encryptedSecrets);
            var returnValue = string.Empty;

            using (var jsonDocument = JsonDocument.Parse(decryptedValues, _jsonDocumentOptions))
            {
                var record = jsonDocument.RootElement;
                returnValue = record.GetNullableString(key);
            }

            return returnValue;
        }

        /// <summary>
        /// Gets the decrypted values.
        /// </summary>
        /// <param name="encryptedSecrets">The encrypted secrets.</param>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        public static UserSecretsKeys GetDecryptedValues(ConcurrentDictionary<string, string> encryptedSecrets, KeyVaultSecretKeyNames keys)
        {
            var decryptedValues = GetDecryptedEnvironmentSecrets(encryptedSecrets);
            var userSecretKeys = new UserSecretsKeys();

            using (var jsonDocument = JsonDocument.Parse(decryptedValues, _jsonDocumentOptions))
            {
                var json = jsonDocument.RootElement;
                userSecretKeys.ClientId = json.GetNullableString(keys.RealClientIdSetting);
                userSecretKeys.ClientSecret = json.GetNullableString(keys.RealClientSecretSetting);
                userSecretKeys.CrmBaseUrl = json.GetNullableString(keys.RealCrmBaseUrlSetting);
                userSecretKeys.Authority = json.GetNullableString(keys.RealAuthoritySetting);
                userSecretKeys.OrganizationServiceRelativeUrl = json.GetNullableString(keys.RealOrganizationServiceRelativeUrl);
                userSecretKeys.ReportServerDbConnectionString = json.GetNullableString(keys.RealReportServerDbConnectionString);
                userSecretKeys.ReportFolder = json.GetNullableString(keys.RealReportFolder);
                userSecretKeys.ReportServerUserName = json.GetNullableString(keys.RealReportServerUserName);
                userSecretKeys.ReportServerDomain = json.GetNullableString(keys.RealReportServerDomain);
                userSecretKeys.ReportServerPassword = json.GetNullableString(keys.RealReportServerPw);
                userSecretKeys.ECUri = json.GetNullableString(keys.RealECUriSetting);
                userSecretKeys.AADTenent = json.GetNullableString(keys.RealAADTenentSetting);
                userSecretKeys.AADInstance = json.GetNullableString(keys.RealAADInstanceSetting);
                userSecretKeys.OAuthClientId = json.GetNullableString(keys.RealOAuthClientIdSetting);
                userSecretKeys.OAuthClientSecret = json.GetNullableString(keys.RealOAuthClientSecretSetting);
                userSecretKeys.OAuthResourceId = json.GetNullableString(keys.RealOAuthResourceIdSetting);
                userSecretKeys.AzureKeyVaultUrl = json.GetNullableString(keys.RealAzureKeyVaultUrlSetting);
                userSecretKeys.LobApimUri = json.GetNullableString(keys.RealLobApimUriSetting);
                userSecretKeys.OcpApimSubscriptionKeyE = json.GetNullableString(keys.RealOcpApimSubscriptionKeyESetting);
                userSecretKeys.OcpApimSubscriptionKeyS = json.GetNullableString(keys.RealOcpApimSubscriptionKeySSetting);
                userSecretKeys.OcpApimSubscriptionKey = json.GetNullableString(keys.RealOcpApimSubscriptionKeySetting);
            }

            return userSecretKeys;
        }

        private static string GetDecryptedEnvironmentSecrets(ConcurrentDictionary<string, string> encryptedSecrets)
        {
            var environment = encryptedSecrets[UdoAzureEnvironmentKey];
            var encryptedValue = encryptedSecrets[environment + UdoConfigurationKey];

            var decryptedValue = UdoConfigurationEncryptor.Decrypt(encryptedValue);

            return decryptedValue;
        }

#if COMMENT_OUT
        private static string GetUserSecretsFilePathInternal()
        {
            var userSecretsFilePath = PathHelper.GetSecretsPathFromSecretsId(UserSecretsId);

            if (!File.Exists(userSecretsFilePath))
            {
                // required UDO User Secrets file secrets.json doe not exist on this machine
                // if the directory doesn't exist
                // attempt to create it
                var userSecretsDirectoryPath = Path.GetDirectoryName(userSecretsFilePath);
                if (!Directory.Exists(userSecretsDirectoryPath))
                {
                    try
                    {
                        Directory.CreateDirectory(userSecretsDirectoryPath);
                    }
                    catch
                    {
                        // don't error out if can't create the directory due to permission issues
                        // we want the actual exception below to be thrown instead as
                        // that is the real issue
                    }
                }

                throw new FileNotFoundException(
                    "The required UDO user secrets file \" + " +
                    Path.GetFileName(userSecretsFilePath) +
                    "\" does not exist on this machine at \"" +
                    userSecretsDirectoryPath +
                    "\"");
            }

            return userSecretsFilePath;
        }
#endif
    }
}
