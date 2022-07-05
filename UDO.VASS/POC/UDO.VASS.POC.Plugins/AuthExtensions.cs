using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UDO.VASS.POC.Plugins.Entities;

namespace UDO.VASS.POC.Plugins
{
    public static class AuthExtensions
    {
        public static HttpClient AddAuthHeader(this HttpClient httpClient, UDOSettings crmAuthInfo)
        {
            AzureAccessToken token = GetAccessToken(crmAuthInfo);

            // httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{token.token_type} {token.access_token}");
            httpClient.DefaultRequestHeaders.ConnectionClose = true;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"{token.token_type}", $" {token.access_token}");
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", crmAuthInfo.ApimSubscriptionKey);

            if (!string.IsNullOrEmpty(crmAuthInfo.ApimSubscriptionKeyS))
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key-S", crmAuthInfo.ApimSubscriptionKeyS);

            if (!string.IsNullOrEmpty(crmAuthInfo.ApimSubscriptionKeyE))
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key-E", crmAuthInfo.ApimSubscriptionKeyE);

            return httpClient;
        }

        private static AzureAccessToken GetAccessToken(UDOSettings crmAuthInfo)
        {
            AzureAccessToken token = null;
            using (WebClient client = new WebClient())
            {
                //This is the AADInstance in the webconfig
                string oauthUrl = $"https://login.microsoftonline.us/{crmAuthInfo.TenantId}/oauth2/token";
                string reqBody = $"grant_type=client_credentials&client_id={Uri.EscapeDataString(crmAuthInfo.ClientApplicationId)}&client_secret={Uri.EscapeDataString(crmAuthInfo.ClientSecret)}&resource={Uri.EscapeDataString(crmAuthInfo.ParentApplicationId)}";

                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                string response = client.UploadString(oauthUrl, reqBody);
                token = JsonHelper.Deserialize<AzureAccessToken>(response);
            }
            return token;
        }
    }
}
