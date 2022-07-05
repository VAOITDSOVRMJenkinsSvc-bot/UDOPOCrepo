using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;

namespace UDO.LOB.Extensions.Configuration
{
    public static class VEISConfiguration
    {
        public const string AADTenent = "AADTenent";
        public const string AADInstance = "AADInstance";
        public const string OAuthClientId = "OAuthClientId";
        public const string OAuthClientSecret = "OAuthClientSecret";
        public const string OAuthResourceId = "OAuthResourceId";
        public const string AzureKeyVaultUrl = "AzureKeyVaultUrl";
        public const string OcpApimSubscriptionKey = "Ocp-Apim-Subscription-Key";
        public const string OcpApimSubscriptionKeyS = "Ocp-Apim-Subscription-Key-S";
        public const string OcpApimSubscriptionKeyE = "Ocp-Apim-Subscription-Key-E";

        public const string ECUri = "ECUri";

        public static NameValueCollection GetConfigurationSettings()
        {
            return ConnectionCache.ConnectManager.VEISConfiguration;
        }
    }


    public static class LOBConfiguration
    {
        public const string AADTenent = "AADTenent";
        public const string AADInstance = "AADInstance";
        public const string OAuthClientId = "OAuthClientId";
        public const string OAuthClientSecret = "OAuthClientSecret";
        public const string OAuthResourceId = "OAuthResourceId";
        public const string AzureKeyVaultUrl = "AzureKeyVaultUrl";
        public const string LobApimUri = "LobApimUri";

        public static NameValueCollection GetConfigurationSettings()
        {
            //Read config settings from Connection Manager
            return ConnectionCache.ConnectManager.LOBConfiguration;
        }
    }
}