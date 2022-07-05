
namespace UDO.LOB.Extensions
{
    using Azure.Core;
    using Azure.Identity;
    using Azure.Security.KeyVault.Secrets;

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.ServiceModel;
    using System.Text;
    using System.Threading.Tasks;
    using UDO.LOB.Extensions.Logging;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Client;
    using Microsoft.Xrm.Sdk.WebServiceClient;
    using System.Web;

    public class CrmConnection
    {
        public OrganizationServiceProxy OrgServiceProxy { get; set; }

        private static IOrganizationService organizationService;

        public static OrganizationWebProxyClient ServiceProxyClient { get; private set; }

        public static AuthenticationResult authenticationResult { get; private set; }

        public CrmConnection()
        {
            if (ServiceProxyClient == null)
            {
                //NEW CODE TO GET SETTING FROM WEB APP CONFIGURATION SECTIONS
                var realRelativeSetting = "OrganizationServiceRelativeUrl";
                var realSetting = CrmConfiguration.BaseUrl;

                try
                {
                    var folder = HttpContext.Current?.Request.Url.Segments[1];

                    if (folder != null && folder != "api/")
                    {
                        folder = folder.Replace("/", string.Empty);
                        realSetting += "-" + folder;
                        realRelativeSetting += "-" + folder;
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.LogInfo($"CrmConnection :: Not able to find new folder:" + ex.Message);
                }

                string resource = ConfigurationManager.AppSettings[realSetting].ToString();
                string clientId = ConfigurationManager.AppSettings["ClientId"].ToString();
                string clientSecret = ConfigurationManager.AppSettings["ClientSecret"].ToString();
                string authority = ConfigurationManager.AppSettings["Authority"].ToString();

                //Ininitiate the Client Credential
                ClientCredential clientcred = new ClientCredential(clientId, clientSecret);

                //AuthenticationResult authenticationResult;


                // Create a AuthenticationContext using the Authenticating Authority
                AuthenticationContext authenticationContext = new AuthenticationContext(authority, false);
                AuthenticationResult authenticationResult = authenticationContext.AcquireTokenAsync(resource, clientcred).Result;
                string crmAccessToken = authenticationResult.AccessToken;
               
                string orgServiceRelativeUrl = ConfigurationManager.AppSettings[realRelativeSetting].ToString();
                Uri orgServiceUri = new Uri(new Uri(resource), orgServiceRelativeUrl);

                ServiceProxyClient = new OrganizationWebProxyClient(orgServiceUri, false)
                {
                    //Set the Auth Token 
                    HeaderToken = crmAccessToken
                };

            }
        }

        public IOrganizationService Connect()
        {
            if (organizationService == null)
            {             
                organizationService = ServiceProxyClient as IOrganizationService;
                LogHelper.LogInfo($"CrmConnection.Connect :: Initiated a new Connection to CRM");
            }

            return organizationService; 
        }


        private static object lockObject = "LOCK";

        private static KeyValuePair<string, DateTimeOffset> AuthToken;

        /// <summary>
        /// Create a new Connection to Dynamics CRM Online using a Client Id, Client Secret, OrganizationService Url.
        /// Returns an instance of OrganizationWebProxyClient
        /// Use the OrganizationWebProxyClient for impersonation scenarios to set the CallerId
        /// </summary>
        /// <typeparam name="T">OrganizationWebProxyClient</typeparam>
        /// <returns>OrganizationWebProxyClient</returns>
        public static T Connect<T>() where T: OrganizationWebProxyClient
        {
            lock (lockObject)
            {
                // Check if token expired and Refresh Token?
                if (AuthToken.Value != null && DateTimeOffset.Now.CompareTo(AuthToken.Value) >= 0)
                {
                    //NEW CODE TO GET SETTING FROM WEB APP CONFIGURATION SECTIONS
                    //NEW CODE TO GET SETTING FROM WEB APP CONFIGURATION SECTIONS
                    var realRelativeSetting = "OrganizationServiceRelativeUrl";
                    var realSetting = CrmConfiguration.BaseUrl;

                    try
                    {
                        var folder = HttpContext.Current?.Request.Url.Segments[1];

                        if (folder != null && folder != "api/")
                        {
                            folder = folder.Replace("/", string.Empty);
                            realSetting += "-" + folder;
                            realRelativeSetting += "-" + folder;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogInfo($"T Connect<T>() :: Not able to find new folder:" + ex.Message);
                    }
                    string resource = ConfigurationManager.AppSettings[realSetting].ToString();
                    string clientId = ConfigurationManager.AppSettings["ClientId"].ToString();
                    string clientSecret = ConfigurationManager.AppSettings["ClientSecret"].ToString();
                    string authority = ConfigurationManager.AppSettings["Authority"].ToString();

                    //Ininitiate the Client Credential
                    ClientCredential clientcred = new ClientCredential(clientId, clientSecret);

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    // Create a AuthenticationContext using the Authenticating Authority
                    AuthenticationContext authenticationContext = new AuthenticationContext(authority);
                    authenticationResult = authenticationContext.AcquireTokenAsync(resource, clientcred).Result;
                    string crmAccessToken = authenticationResult.AccessToken;
                    AuthToken = new KeyValuePair<string, DateTimeOffset>(crmAccessToken, authenticationResult.ExpiresOn.AddMinutes(-5));
                    LogHelper.LogInfo("Authentication token acquired.");
                    string orgServiceRelativeUrl = ConfigurationManager.AppSettings[realRelativeSetting].ToString();
                    Uri orgServiceUri = new Uri(new Uri(resource), orgServiceRelativeUrl);

                    ServiceProxyClient = new OrganizationWebProxyClient(orgServiceUri, false)
                    {
                        //Set the Auth Token 
                        HeaderToken = AuthToken.Key
                    };

                    organizationService = ServiceProxyClient as IOrganizationService;
                    LogHelper.LogInfo($"CrmConnection.Connect :: Initiated a new Connection to CRM  (LOGGING)");
                }

                return (T)ServiceProxyClient;
            }

        }
        public void CloseConnection()
        {            
            ServiceProxyClient.Close();
            ServiceProxyClient.Dispose();
        }

		/// <summary>
		/// Create a new Connection to Dynamics CRM Online using a Client Id, Client Secret, OrganizationService Url.
		/// Returns an instance of OrganizationWebProxyClient
		/// Use the OrganizationWebProxyClient for impersonation scenarios to set the CallerId
		/// 
		/// CSDev This uses a very specific first time touch Log Method with enabled proxy types.  Do not remove. 
		/// 
		/// </summary>
		/// <typeparam name="T">OrganizationWebProxyClient</typeparam>
		/// <returns>OrganizationWebProxyClient</returns>
		public static T Connect<T>(bool enableProxyTypes) where T : OrganizationWebProxyClient
		{
            LogHelper.LogInfo($"{MethodInfo.GetThisMethod().Method} :: getting lock for crm connection");

            lock (lockObject)
			{
                LogHelper.LogInfo($"{MethodInfo.GetThisMethod().Method} :: got lock for crm connection");

                // Check if token expired and Refresh Token?
                if (AuthToken.Value != null && DateTimeOffset.Now.CompareTo(AuthToken.Value) >= 0)
				{
                    //NEW CODE TO GET SETTING FROM WEB APP CONFIGURATION SECTIONS
                    //NEW CODE TO GET SETTING FROM WEB APP CONFIGURATION SECTIONS
                    var realRelativeSetting = "OrganizationServiceRelativeUrl";
                    var realSetting = CrmConfiguration.BaseUrl;

                    try
                    {
                        var folder = HttpContext.Current?.Request.Url.Segments[1];

                        if (folder != null && folder != "api/")
                        {
                            folder = folder.Replace("/", string.Empty);
                            realSetting += "-" + folder;
                            realRelativeSetting += "-" + folder;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogInfo($"T Connect<T>(bool enableProxyTypes):: Not able to find new folder:" + ex.Message);
                    }

                    string resource = ConfigurationManager.AppSettings[realSetting].ToString();
                    string clientId = ConfigurationManager.AppSettings["ClientId"].ToString();
                    string clientSecret = ConfigurationManager.AppSettings["ClientSecret"].ToString();
                    string authority = ConfigurationManager.AppSettings["Authority"].ToString();
                    //Ininitiate the Client Credential
                    ClientCredential clientcred = new ClientCredential(clientId, clientSecret);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    // Create a AuthenticationContext using the Authenticating Authority
                    AuthenticationContext authenticationContext = new AuthenticationContext(authority);
                    authenticationResult = authenticationContext.AcquireTokenAsync(resource, clientcred).Result;
                    string crmAccessToken = authenticationResult.AccessToken;
                    AuthToken = new KeyValuePair<string, DateTimeOffset>(crmAccessToken, authenticationResult.ExpiresOn.AddMinutes(-5));
                    string orgServiceRelativeUrl = ConfigurationManager.AppSettings[realRelativeSetting].ToString();
                    Uri orgServiceUri = new Uri(new Uri(resource), orgServiceRelativeUrl);

                    //csdev
                    //ServiceProxyClient = new OrganizationWebProxyClient(orgServiceUri, false)
                    ServiceProxyClient = new OrganizationWebProxyClient(orgServiceUri, enableProxyTypes)
					{
						//Set the Auth Token 
						HeaderToken = AuthToken.Key
					};
                    LogHelper.LogInfo("13");
                    organizationService = ServiceProxyClient as IOrganizationService;
					LogHelper.LogInfo($"CrmConnection.Connect :: Initiated a new Connection to CRM");
				}

				return (T)ServiceProxyClient;
			}

		}

		/// <summary>
		/// Create a new Connection to Dynamics CRM Online using a Client Id, Client Secret, OrganizationService Url.
		/// Returns an instance of OrganizationWebProxyClient
		/// Use the OrganizationWebProxyClient for impersonation scenarios to set the CallerId
		/// 
		/// CSDev This uses a very specific first time touch Log Method with enabled proxy types.  Do not remove. 
		/// Method cloned to FORCE ENABLE PROXY TYPES 
		/// 
		/// </summary>
		/// <typeparam name="T">OrganizationWebProxyClient</typeparam>
		/// <returns>OrganizationWebProxyClient</returns>
		public static T Connect<T>(bool enableProxyTypes, bool forceEnableProxyTypes) where T : OrganizationWebProxyClient
		{
			lock (lockObject)
			{
                //NEW CODE TO GET SETTING FROM WEB APP CONFIGURATION SECTIONS
                //NEW CODE TO GET SETTING FROM WEB APP CONFIGURATION SECTIONS
                var realRelativeSetting = "OrganizationServiceRelativeUrl";
                var realSetting = CrmConfiguration.BaseUrl;

                try
                {
                    var folder = HttpContext.Current?.Request.Url.Segments[1];



                    if (folder != null && folder != "api/")
                    {
                        folder = folder.Replace("/", string.Empty);
                        realSetting += "-" + folder;
                        realRelativeSetting += "-" + folder;
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.LogInfo($"CrmConnection.Connect :: Not able to find new folder:" + ex.Message);
                }

                // Check if token expired and Refresh Token?
                if (forceEnableProxyTypes || ( AuthToken.Value != null && DateTimeOffset.Now.CompareTo(AuthToken.Value) >= 0))
				{
                    string resource = ConfigurationManager.AppSettings[realSetting].ToString();
					string clientId = ConfigurationManager.AppSettings["ClientId"].ToString();
					string clientSecret = ConfigurationManager.AppSettings["ClientSecret"].ToString();
					string authority = ConfigurationManager.AppSettings["Authority"].ToString();

					//Ininitiate the Client Credential
					ClientCredential clientcred = new ClientCredential(clientId, clientSecret);

					ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

					// Create a AuthenticationContext using the Authenticating Authority
					AuthenticationContext authenticationContext = new AuthenticationContext(authority);
					authenticationResult = authenticationContext.AcquireTokenAsync(resource, clientcred).Result;
					string crmAccessToken = authenticationResult.AccessToken;
					AuthToken = new KeyValuePair<string, DateTimeOffset>(crmAccessToken, authenticationResult.ExpiresOn.AddMinutes(-5));

					string orgServiceRelativeUrl = ConfigurationManager.AppSettings[realRelativeSetting].ToString();
					Uri orgServiceUri = new Uri(new Uri(resource), orgServiceRelativeUrl);

					//csdev
					//ServiceProxyClient = new OrganizationWebProxyClient(orgServiceUri, false)
					ServiceProxyClient = new OrganizationWebProxyClient(orgServiceUri, enableProxyTypes)
					{
						//Set the Auth Token 
						HeaderToken = AuthToken.Key
					};

					organizationService = ServiceProxyClient as IOrganizationService;
					LogHelper.LogInfo($"CrmConnection.Connect :: Initiated a new Connection to CRM");
				}

				return (T)ServiceProxyClient;
			}

		}
	}
}
