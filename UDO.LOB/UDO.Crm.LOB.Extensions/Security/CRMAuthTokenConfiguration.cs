using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using UDO.LOB.Extensions.Logging;

namespace UDO.LOB.Core
{
	public class CRMAuthTokenConfiguration
    {
        public string ClientApplicationId { get; set; }
        public string ClientSecret { get; set; }
        public string ParentApplicationId { get; set; }
        public string TenantId { get; set; }
        public string ApimSubscriptionKey { get; set; }
        public string ApimSubscriptionKeyE {get; set;}
        public string ApimSubscriptionKeyS {get; set;}
    }

    public static class AuthExtensions
    {
        private static AuthenticationResult azureAuthToken = null;
        private static ReaderWriterLockSlim tokenLock = new ReaderWriterLockSlim();

        public static HttpClient AddAuthHeader(this HttpClient httpClient, CRMAuthTokenConfiguration crmAuthInfo)
        {
            AzureAccessToken token = GetAccessToken(crmAuthInfo);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"{token.token_type}", $" {token.access_token}");
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", crmAuthInfo.ApimSubscriptionKey);
            if (!string.IsNullOrEmpty(crmAuthInfo.ApimSubscriptionKeyS))
            {
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key-S", crmAuthInfo.ApimSubscriptionKeyS);
            }

            if (!string.IsNullOrEmpty(crmAuthInfo.ApimSubscriptionKeyE))
            {
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key-E", crmAuthInfo.ApimSubscriptionKeyE);
            }

            httpClient.DefaultRequestHeaders.ConnectionClose = true;
            return httpClient;
        }

        public static WebClient AddAuthHeader(this WebClient webClient, CRMAuthTokenConfiguration crmAuthInfo)
        {
            AzureAccessToken token = GetAccessToken(crmAuthInfo);
            webClient.Headers[HttpRequestHeader.Authorization] = $"{token.token_type} {token.access_token}";
            return webClient;
        }

        private static AzureAccessToken GetAccessToken(CRMAuthTokenConfiguration crmAuthInfo)
        {
            try
            {
                AzureAccessToken token = new AzureAccessToken();

                // Attempt to read in access token
                tokenLock.EnterReadLock();
                try
                {
                    if ((azureAuthToken != null) && ((int)azureAuthToken.ExpiresOn.Subtract(DateTime.UtcNow).TotalMinutes > 5))
                    {
                        // Populate AzureAccessToken object
                        token.access_token = azureAuthToken.AccessToken;
                        token.token_type = "Bearer";
                        token.expires_on = azureAuthToken.ExpiresOn.ToString();
                        token.resource = azureAuthToken.Scopes.FirstOrDefault();
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.LogError(string.Empty, Guid.Empty, "GetAccessToken", ex.Message);
                    throw ex;
                }
                finally
                {
                    tokenLock.ExitReadLock();
                }

                if (token.access_token == null)
                {
                    tokenLock.EnterWriteLock();
                    try
                    {
                        if ((azureAuthToken == null) || ((int)azureAuthToken.ExpiresOn.Subtract(DateTime.UtcNow).TotalMinutes <= 5))
                        {
                            // Build authentication object using MSAL.NET
                            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(crmAuthInfo.ClientApplicationId)
                                .WithClientSecret(crmAuthInfo.ClientSecret)
                                .WithAuthority(new Uri($"https://login.microsoftonline.us/{crmAuthInfo.TenantId}"))
                                .Build();

                            string[] scopes = new string[]
                            {
                            $"{crmAuthInfo.ParentApplicationId}/.default"
                            };

                            // Retrieve access token
                            Task<AuthenticationResult> task = app.AcquireTokenForClient(scopes).ExecuteAsync();
                            task.Wait();
                            azureAuthToken = task.Result;
                        }

                        // Populate AzureAccessToken object
                        token.access_token = azureAuthToken.AccessToken;
                        token.token_type = "Bearer";
                        token.expires_on = azureAuthToken.ExpiresOn.ToString();
                        token.resource = azureAuthToken.Scopes.FirstOrDefault();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogError(string.Empty, Guid.Empty, "GetAccessToken", ex.Message);
                        throw ex;
                    }
                    finally
                    {
                        tokenLock.ExitWriteLock();
                    }
                }

                return token;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(string.Empty, Guid.Empty, "GetAccessToken", ex.Message);
                throw ex;
            }
        }
    }
}
