using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UDO.Crm.LOB.Extensions.CRM
{
    public static class XrmToolingTokenStore
    {
        public static Dictionary<string, AuthenticationResult> AccessTokens = new Dictionary<string, AuthenticationResult>();
    }

    public class XrmToolingAuthOverride : Microsoft.Xrm.Tooling.Connector.IOverrideAuthHookWrapper
    {
        private string _clientId;

        private string _clientSecret;
        private string _resource;
        private string _authority;

        public XrmToolingAuthOverride(string ClientId, string ClientSecret, string Resource, string Authority)
        {
            _clientId = ClientId;
            _clientSecret = ClientSecret;
            _resource = Resource;
            _authority = Authority;

        }
        public string GetAuthToken(Uri connectedUri)
        {
            AuthenticationResult result = GetAccessTokenFromAdal(connectedUri);

            if (result != null && result.AccessToken != null)
            {
                return result.AccessToken;
            }

            return string.Empty;
        }

        private AuthenticationResult GetAccessTokenFromAdal(Uri connectedUri)
        {
            AuthenticationResult result = null;
            Task.Run(async () =>
            {
                var authContext = new AuthenticationContext(_authority);
                var taskResult = await authContext.AcquireTokenAsync(_resource, new ClientCredential(_clientId, _clientSecret));
                result = taskResult;
            }).Wait();

            return result;
        }
    }
}