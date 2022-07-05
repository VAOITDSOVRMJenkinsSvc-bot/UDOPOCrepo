using Azure.Security.KeyVault.Secrets;
//using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk;
using System;
using System.Globalization;
using System.Threading.Tasks;
using UDO.LOB.Extensions.Logging;

namespace UDO.LOB.Core
{
    public class AzureAuthenticationHelper : IAzureAuthenticationHelper
    {
        /// <summary>
        /// The method for getting the authorization token from Azure AD
        /// </summary>
        /// <param name="clientId">The client ID of the AD application.</param>
        /// <param name="clientSecret">The client secret of the AD application</param>
        /// <param name="apiResourceId">The resource id of the API.</param>
        /// <param name="tenant">The Azure AD tenant</param>
        /// <param name="aadInstance">The Azure AD instance</param>
        /// <returns>
        /// Azure authorization result
        /// </returns>
        public async Task<AzureAuthResult> ObtainAuthTokenAsync(string clientId, string clientSecret, string apiResourceId, string tenant, string aadInstance)
        {
            var azureAuthResult = new AzureAuthResult();

            if (string.IsNullOrEmpty(tenant) || string.IsNullOrEmpty(aadInstance) || string.IsNullOrEmpty(apiResourceId) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentException(
                    string.Format(
                        "{0}, {1}, {2}, {3}, {4} must be present in configuration.",
                        tenant,
                        aadInstance,
                        apiResourceId,
                        clientId,
                        clientSecret));
            }

            if (!aadInstance.Contains("{0}"))
            {
                throw new ArgumentException("AAD instance configuration should have a placeholder for tenant");
            }

            var authority = string.Format(CultureInfo.InvariantCulture, aadInstance, tenant);
            var authContext = new AuthenticationContext(authority);
            var cred = new ClientCredential(clientId, clientSecret);
            try
            {
                var authResult = await authContext.AcquireTokenAsync(apiResourceId, cred).ConfigureAwait(false);
                azureAuthResult.AuthToken = authResult.AccessToken;
                azureAuthResult.TokenType = authResult.AccessTokenType;
                azureAuthResult.ExpiresOn = authResult.ExpiresOn;
            }
            catch(Exception ex)
            {
                LogHelper.LogError(string.Empty, Guid.Empty, "ObtainAuthTokenAsync", ex.Message);
                return null;
            }

            return azureAuthResult;
        }

        public async Task<string> ObtainSecretFromKeyVault(string KVSecretURL)
        {
            //AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();

            try
            {
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(string.Empty, Guid.Empty, "ObtainSecretFromKeyVault", $"Failed to obtain Secret from Key Vault. {ex.Message}");
                return null;
            } 
        }
    }
}
