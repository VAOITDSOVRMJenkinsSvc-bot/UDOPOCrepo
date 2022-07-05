using MCSPlugins;
using Microsoft.Xrm.Sdk;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using UDO.LOB.Core;
using UDO.LOB.RedirectSvc.Messages;
using VRMRest;

namespace UDO.CustomActions.RedirectSvc.Plugins.Processors.RedirectSvc
{
    public class UDORedirectSvcRunner : UDOActionRunner
    {
        #region Members
        protected UDOHeaderInfo _headerInfo;
        protected string _parententityname = string.Empty;
        protected Guid _parententityid = Guid.Empty;
        protected string _soaprequest = string.Empty;
        #endregion

        #region constructor
        public UDORedirectSvcRunner(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            // Set configuration values
            _logTimerField = "udo_redirectsvclogtimer";
            _logSoapField = "udo_redirectsvclogsoap";
            _debugField = "udo_redirectsvc";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_redirectsvctimeout";
            _validEntities = new string[] { "va_fnod" };
        }
        #endregion

        #region DoAction method
        public override void DoAction()
        {
            try
            {
                try
                {
                    string parentAppId = McsSettings.GetSingleSetting<string>("udo_oauthresourceid");
                    string clientAppId = McsSettings.GetSingleSetting<string>("udo_oauthclientid");
                    string clientSecret = McsSettings.GetSingleSetting<string>("udo_oauthclientsecret");
                    string tenentId = McsSettings.GetSingleSetting<string>("udo_aadtenent");
                    string apimsubscriptionkey = McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkey");
                    string apimsubscriptionkeyS = McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkeysouth");
                    string apimsubscriptionkeyE = McsSettings.GetSingleSetting<string>("udo_ocpapimsubscriptionkeyeast");

                    _crmAuthTokenConfig = new CRMAuthTokenConfiguration
                    {
                        ParentApplicationId = parentAppId,
                        ClientApplicationId = clientAppId,
                        ClientSecret = clientSecret,
                        TenantId = tenentId,
                        ApimSubscriptionKey = apimsubscriptionkey,
                        ApimSubscriptionKeyE = apimsubscriptionkeyE,
                        ApimSubscriptionKeyS = apimsubscriptionkeyS
                    };

                    AzureAccessToken token = GetAccessToken(_crmAuthTokenConfig);

                    var SoapRequest = PluginExecutionContext.InputParameters["SoapRequest"].ToString();
                    var RequestURL = PluginExecutionContext.InputParameters["RequestURL"].ToString();

                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(RequestURL));

                    webRequest.Headers.Add("Ocp-Apim-Subscription-Key", apimsubscriptionkey);
                    webRequest.Headers.Add("Ocp-Apim-Subscription-Key-E", apimsubscriptionkeyE);
                    webRequest.Headers.Add("Ocp-Apim-Subscription-Key-S", apimsubscriptionkeyS);
                    webRequest.Headers.Add("Authorization", "Bearer " + token.access_token);
                    webRequest.ContentType = "text/xml; charset=UTF-8";
                    webRequest.Accept = "application/xml, text/xml, */*";
                    webRequest.Method = "POST";

                    byte[] bytes = Encoding.UTF8.GetBytes(SoapRequest);
                    webRequest.ContentLength = bytes.Length;
                    using (Stream putStream = webRequest.GetRequestStream())
                    {
                        putStream.Write(bytes, 0, bytes.Length);
                    }

                    string streamData = null;

                    using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        // Need to check if our request was succesful before we try to read the result.
                        if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Accepted)
                        {
                            string errorMessage = $"Redirect Service request failed with status code {response.StatusCode}.";
                            PluginExecutionContext.OutputParameters["ResponseMessage"] = errorMessage;
                            PluginExecutionContext.OutputParameters["Exception"] = true;
                            Trace(errorMessage);

                            throw new Exception(errorMessage);
                        }

                        streamData = reader.ReadToEnd();
                    }

                    PluginExecutionContext.OutputParameters["ResponseMessage"] = streamData;

                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException(SetResponseMessage(ex.Message));
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(SetResponseMessage(ex.Message));
            }
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

        #endregion

        private bool GetInputParameters()
        {
            try
            {
                tracer.Trace("Retrieving SOAP Request from input parameters");
                _soaprequest = (string)PluginExecutionContext.InputParameters["SoapRequest"];

                if (string.IsNullOrEmpty(_soaprequest))
                {
                    _responseMessage = SetResponseMessage($"One or more of the required input parameters for Redirect Service was empty.");
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(SetResponseMessage($"One or more of the required input parameters for Redirect Service was missing. {e.Message}"));
            }
        }

        private string SetResponseMessage(string message)
        {
            const string FAILURE_MESSAGE = "Unable to perform Redirect Svc. Message: {0}; CorrelationId: {1}";
            return String.Format(FAILURE_MESSAGE, message, PluginExecutionContext.CorrelationId.ToString());
        }
    }
}
