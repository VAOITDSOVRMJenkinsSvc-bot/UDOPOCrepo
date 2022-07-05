using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;

namespace MCSPlugins
{
    public static class AuthExtensions
    {
        public static HttpClient AddAuthHeader(this HttpClient httpClient, CRMAuthTokenConfiguration crmAuthInfo)
        {
            AzureAccessToken token = GetAccessToken(crmAuthInfo);

            // httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{token.token_type} {token.access_token}");
            httpClient.DefaultRequestHeaders.ConnectionClose = true;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"{token.token_type}", $" {token.access_token}");
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", crmAuthInfo.ApimSubscriptionKey);
            
            if(!string.IsNullOrEmpty(crmAuthInfo.ApimSubscriptionKeyS))
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key-S", crmAuthInfo.ApimSubscriptionKeyS);

            if(!string.IsNullOrEmpty(crmAuthInfo.ApimSubscriptionKeyE))
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key-E", crmAuthInfo.ApimSubscriptionKeyE);

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


    //public class AzureAccessToken
    //{
    //    public string access_token { get; set; }
    //    public string token_type { get; set; }
    //    public string expires_in { get; set; }
    //    public string expires_on { get; set; }
    //    public string resource { get; set; }
    //}

    //public class CRMAuthTokenConfiguration
    //{

    //    public string ClientApplicationId { get; set; }
    //    public string ClientSecret { get; set; }
    //    public string ParentApplicationId { get; set; }
    //    public string TenantId { get; set; }
    //}

}