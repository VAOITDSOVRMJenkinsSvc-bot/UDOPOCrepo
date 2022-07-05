using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Pfe.Xrm;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Tooling.Connector;

using System;
using System.Configuration;
using System.Web;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Caching;
using UDO.LOB.Extensions.Logging;
using System.Web.Hosting;
using System.Diagnostics;

namespace UDO.LOB.Extensions
{
    public static class ConnectionCache
    {
        #region Properties
        private static readonly string orgName;
        public static string OrgName
        {
            get
            {
                return orgName;
            }
            private set
            {

            }
        }

        private static readonly ConnectionManager connectManager;
        public static ConnectionManager ConnectManager
        {
            get
            {
                return connectManager;
            }
            private set
            {

            }
        }
        #endregion

        #region Constructor
        static ConnectionCache()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var query = assemblies.AsEnumerable()
                .Where(assembly =>
                            assembly.FullName.Split(',')[0].Split('.').ElementAtOrDefault(0) == "UDO" &&
                            assembly.FullName.Split(',')[0].Split('.').ElementAtOrDefault(3) != "Extensions" &&
                            assembly.FullName.Split(',')[0].Split('.').ElementAtOrDefault(2) != "Core" &&
                            assembly.FullName.Split(',')[0].Split('.').ElementAtOrDefault(2).Contains("Api"))
                .Select(x => x);

            var callingAssemby = query.FirstOrDefault();
            string callerName = callingAssemby.FullName.Split(',')[0].Split('.')[2].Replace("Api", "");

            var isRunningOnLocalMachine = HostingEnvironment.ApplicationPhysicalPath.StartsWith(@"C:\") && Debugger.IsAttached;

            connectManager = ConnectionManager.GetConnectionManager(callerName, isRunningOnLocalMachine);
        }
        #endregion

        #region Methods
        public static CrmServiceClient GetProxy()
        {
            try
            {
                return ConnectManager.GetProxy();
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ConnectManager.BaseUrl, Guid.Empty, "ConnectionCache.GetProxy", ex.Message);
                throw ex;
            }
        }
        #endregion
    }

    public class ConnectionManager : IDisposable
    {
        private static ConcurrentDictionary<string, ConnectionManager> _connections = new ConcurrentDictionary<string, ConnectionManager>();
        #region Properties
        public OrganizationServiceManager OrgServiceManager;

        public string BaseUrl;

        public NameValueCollection VEISConfiguration;

        public NameValueCollection LOBConfiguration;

        public NameValueCollection SSRSConfiguration;

        public string LoggerName;

        private static Cache _settingsCache = HttpRuntime.Cache;

        private ExecuteMultipleHelperSettings _emhSettings = null;
        #endregion

        #region Methodss
        private CrmConnectionParms GetConnectionParms(string OrgName)
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
                LogHelper.LogInfo($"GetConnectionParms):: Not able to find new folder:" + ex.Message);

            }


            string resource = ConfigurationManager.AppSettings[realSetting].ToString();
            string orgServiceRelativeUrl = ConfigurationManager.AppSettings[realRelativeSetting].ToString();

            CrmConnectionParms connectionParms = new CrmConnectionParms()
            {
                Name = OrgName,
                OrganizationServiceUrl = string.Concat(resource, orgServiceRelativeUrl)
            };

            if (connectionParms == null)
            {
                throw new Exception(string.Format("ConnectionParms was not found for OrganizationProxy {0}", OrgName));
            }

            return connectionParms;
        }

        public static ConnectionManager GetByOrgServiceProxy(OrganizationServiceProxy proxy)
        {
            IEnumerable<KeyValuePair<string, ConnectionManager>> cons = _connections.Where(a => a.Value.ExecuteMultipleHelperSettings.OrganizationServiceProxyUri == proxy.ServiceManagement.CurrentServiceEndpoint.Address.Uri);
            if (cons.Count() > 0)
            {
                return cons.First().Value;
            }
            return null;
        }

        public CrmServiceClient GetProxy()
        {
            return OrgServiceManager.GetProxy();
        }

        public ExecuteMultipleHelperSettings ExecuteMultipleHelperSettings
        {
            get
            {
                if (_settingsCache != null && _settingsCache["ExecuteMultipleSettings"] != null)
                {
                    _emhSettings = (ExecuteMultipleHelperSettings)_settingsCache["ExecuteMultipleSettings"];
                }

                if (_emhSettings == null || _emhSettings.IsExpired())
                {
                    using (CrmServiceClient svc = this.GetProxy())
                    {
                        _emhSettings = ExecuteMultipleHelperSettings.LoadFromCRM(svc);
                    }
                }
                return _emhSettings;
            }
            set
            {
                _emhSettings = value;
            }
        }

        public ConnectionManager()
        {
            
        }

        public string getPasswordSafely()
        {
            return "topsecret";
        }

        public ConnectionManager(string assembly, bool isRunningOnLocalMachine)
        {
            var secrets = isRunningOnLocalMachine ? UserSecretsManager.GetUserSecrets() : null;
            var environment = secrets != null ? secrets[UserSecretsManager.UdoAzureEnvironmentKey] : null;
            var envForKeyVaultKeys = isRunningOnLocalMachine && !string.IsNullOrEmpty(environment) ? environment.ToUpper() : null;

            var keys = new KeyVaultSecretKeyNames(assembly.ToUpper(), envForKeyVaultKeys);

#if COMMENT_OUT
            //NEW CODE TO GET SETTING FROM WEB APP CONFIGURATION SECTIONS - this is called from LOB, and on startup          
            var realCrmBaseUrlSetting = "CrmBaseUrl";
            var realClientIdSetting = "ClientId-" + assembly.ToUpper();
            var realClientSecretSetting = "ClientSecret-" + assembly.ToUpper();
            var realAuthoritySetting = "Authority";
            var realOrganizationServiceRelativeUrl = "OrganizationServiceRelativeUrl";
            
            //SSRS CONFIGURATION SETTINGS
            var realReportServerDbConnectionString = "ReportServerDbConnectionString"; //Changes based on virtual env
            var realReportFolder = "ReportFolder"; //Changes based on virtual env
            var realReportServerUserName = "ReportServerUserName";
            var realReportServerDomain = "ReportServerDomain";
            var realReportServerPw = "ReportServerPassword"; // Fortify does not take kindly to variables named Password

            //VEIS CONFIGURATION SETTINGS
            string realECUriSetting = "ECUri";
            string realAADTenentSetting = "AADTenent";
            string realAADInstanceSetting = "AADInstance";
            string realOAuthClientIdSetting = "OAuthClientId";
            string realOAuthClientSecretSetting = "OAuthClientSecret";
            string realOAuthResourceIdSetting = "OAuthResourceId";
            string realAzureKeyVaultUrlSetting = "AzureKeyVaultUrl";
            string realOcpApimSubscriptionKeySetting = "Ocp-Apim-Subscription-Key";
            string realOcpApimSubscriptionKeySSetting = "Ocp-Apim-Subscription-Key-S";
            string realOcpApimSubscriptionKeyESetting = "Ocp-Apim-Subscription-Key-E";

            //LOB CONFIGURATION SETTING
            string realLobApimUriSetting = "LobApimUri";
            var virtualPath = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;

            try
            {                
                if (virtualPath != "/")
                {
                    var folder = virtualPath.Replace("/", string.Empty);
                    realCrmBaseUrlSetting += "-" + folder;
                    realClientIdSetting += "-" + folder;
                    realClientSecretSetting += "-" + folder;
                    realLobApimUriSetting += "-" + folder;
                    realECUriSetting += "-" + folder;
                    realReportServerDbConnectionString += "-" + folder;
                    realReportFolder += "-" + folder;
                }
                else if (System.Web.Hosting.HostingEnvironment.SiteName.Contains("prod"))
                {  
                    if (System.Web.Hosting.HostingEnvironment.SiteName.Contains("south"))
                    {
                        realECUriSetting += "-south";
                        realLobApimUriSetting += "-south";
                    }
                    else
                    {
                        realECUriSetting += "-east";
                        realLobApimUriSetting += "-east";
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogInfo("ConnectionManager:: Not able to find new folder: " + ex.Message);
            }
#endif

#region Log Setting Names
            LogHelper.LogInfo("realCrmBaseUrlSetting: " + keys.RealCrmBaseUrlSetting);
            LogHelper.LogInfo("realClientIdSetting: " + keys.RealClientIdSetting);
            LogHelper.LogInfo("realClientSecretSetting: " + keys.RealClientSecretSetting);
            LogHelper.LogInfo("realAuthoritySetting: " + keys.RealAuthoritySetting);
            LogHelper.LogInfo("realOrganizationServiceRelativeUrl: " + keys.RealOrganizationServiceRelativeUrl);
            LogHelper.LogInfo("realReportServerDbConnectionString: " + keys.RealReportServerDbConnectionString);
            LogHelper.LogInfo("realReportFolder: " + keys.RealReportFolder);
            LogHelper.LogInfo("realReportServerUserName: " + keys.RealReportServerUserName);
            LogHelper.LogInfo("realReportServerDomain: " + keys.RealReportServerDomain);
            LogHelper.LogInfo("realReportServerPassword: " + keys.RealReportServerPw);
            LogHelper.LogInfo("realECUriSetting: " + keys.RealECUriSetting);
            LogHelper.LogInfo("realAADTenentSetting: " + keys.RealAADTenentSetting);
            LogHelper.LogInfo("realAADInstanceSetting: " + keys.RealAADInstanceSetting);
            LogHelper.LogInfo("realOAuthClientIdSetting: " + keys.RealOAuthClientIdSetting);
            LogHelper.LogInfo("realOAuthClientSecretSetting: " + keys.RealOAuthClientSecretSetting);
            LogHelper.LogInfo("realOAuthResourceIdSetting: " + keys.RealOAuthResourceIdSetting);
            LogHelper.LogInfo("realAzureKeyVaultUrlSetting: " + keys.RealAzureKeyVaultUrlSetting);
            LogHelper.LogInfo("realOcpApimSubscriptionKeySetting: " + keys.RealOcpApimSubscriptionKeySetting);
            LogHelper.LogInfo("realOcpApimSubscriptionKeySSetting: " + keys.RealOcpApimSubscriptionKeySSetting);
            LogHelper.LogInfo("realOcpApimSubscriptionKeyESetting: " + keys.RealOcpApimSubscriptionKeyESetting);
            LogHelper.LogInfo("realLobApimUriSetting: " + keys.RealLobApimUriSetting);
#endregion

            var appSettings = ConfigurationManager.AppSettings;
            string keyVaultURI = appSettings["AzureKeyVault"] ?? "";

            string instanceUri;
            string resource;
            string clientId;
            string clientSecret;
            string authority;
            string OrganizationServiceRelativeUrl;
            string ReportServerDbConnectionString;
            string ReportFolder;
            string ReportServerUserName;
            string ReportServerDomain;
            string ReportServerPassword;
            string ECUri;
            string AADTenent;
            string AADInstance;
            string OAuthClientId;
            string OAuthClientSecret;
            string OAuthResourceId;
            string AzureKeyVaultUrl;
            string LobApimUri;
            string OcpApimSubscriptionKeyE;
            string OcpApimSubscriptionKeyS; 
            string OcpApimSubscriptionKey; 

            if (environment != null && isRunningOnLocalMachine) 
            {
                try
                {
                    LogHelper.LogInfo(" >>> Retrieving app settings with values stored in UserSecrets ...");

                    var keyValues = UserSecretsManager.GetDecryptedValues(secrets, keys);
                    instanceUri = keyValues.CrmBaseUrl;
                    resource = keyValues.CrmBaseUrl;
                    BaseUrl = keyValues.CrmBaseUrl;
                    clientId = keyValues.ClientId;
                    clientSecret = keyValues.ClientSecret;
                    authority = keyValues.Authority;
                    OrganizationServiceRelativeUrl = keyValues.OrganizationServiceRelativeUrl;
                    ReportServerDbConnectionString = keyValues.ReportServerDbConnectionString;
                    ReportFolder = keyValues.ReportFolder;
                    ReportServerUserName = keyValues.ReportServerUserName;
                    ReportServerDomain = keyValues.ReportServerDomain;
                    ReportServerPassword = keyValues.ReportServerPassword;
                    ECUri = keyValues.ECUri;
                    AADTenent = keyValues.AADTenent;
                    AADInstance = keyValues.AADInstance;
                    OAuthClientId = keyValues.OAuthClientId;
                    OAuthClientSecret = keyValues.OAuthClientSecret;
                    OAuthResourceId = keyValues.OAuthResourceId;
                    AzureKeyVaultUrl = keyValues.AzureKeyVaultUrl;
                    LobApimUri = keyValues.LobApimUri;
                    OcpApimSubscriptionKey = keyValues.OcpApimSubscriptionKey;
                    OcpApimSubscriptionKeyE = keyValues.OcpApimSubscriptionKeyE;
                    OcpApimSubscriptionKeyS = keyValues.OcpApimSubscriptionKeyS;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                try
                {
                    LogHelper.LogInfo(" >>> Retrieving app settings with values stored in Azure Key Vault...");

                    SecretClientOptions options = new SecretClientOptions()
                    {
                        Retry =
                        {
                            Delay = TimeSpan.FromSeconds(2),
                            MaxDelay = TimeSpan.FromSeconds(16),
                            MaxRetries = 5,
                            Mode = RetryMode.Exponential
                        }
                    };
                    var client = new SecretClient(new Uri(keyVaultURI), new DefaultAzureCredential(), options);

                    KeyVaultSecret crmBaseUrl = client.GetSecret(keys.RealCrmBaseUrlSetting);
                    LogHelper.LogInfo($" ... retrieved CRM Base URL from key vault: {crmBaseUrl.Value}");
                    instanceUri = crmBaseUrl.Value;
                    resource = crmBaseUrl.Value;
                    BaseUrl = crmBaseUrl.Value;

                    KeyVaultSecret clientIdKV = client.GetSecret(keys.RealClientIdSetting);
                    LogHelper.LogInfo($" ... retrieved clientId from key vault: {clientIdKV.Value}");
                    clientId = clientIdKV.Value;

                    KeyVaultSecret clientSecretKV = client.GetSecret(keys.RealClientSecretSetting);
                    LogHelper.LogInfo($" ... retrieved clientSecret from key vault: {clientSecretKV.Value}");
                    clientSecret = clientSecretKV.Value;

                    KeyVaultSecret authorityKV = client.GetSecret(keys.RealAuthoritySetting);
                    LogHelper.LogInfo($" ... retrieved authority from key vault: {authorityKV.Value}");
                    authority = authorityKV.Value;

                    KeyVaultSecret OrganizationServiceRelativeUrlKV = client.GetSecret(keys.RealOrganizationServiceRelativeUrl);
                    LogHelper.LogInfo($" ... retrieved Org Service Relative URL from key vault: {OrganizationServiceRelativeUrlKV.Value}");
                    OrganizationServiceRelativeUrl = OrganizationServiceRelativeUrlKV.Value;

                    KeyVaultSecret ReportServerDbConnectionStringKV = client.GetSecret(keys.RealReportServerDbConnectionString);
                    LogHelper.LogInfo($" ... retrieved Report Server Db Connection String from key vault: {ReportServerDbConnectionStringKV.Value}");
                    ReportServerDbConnectionString = ReportServerDbConnectionStringKV.Value;

                    KeyVaultSecret ReportFolderKV = client.GetSecret(keys.RealReportFolder);
                    LogHelper.LogInfo($" ... retrieved Report Folder from key vault: {ReportFolderKV.Value}");
                    ReportFolder = ReportFolderKV.Value;

                    KeyVaultSecret ReportServerUserNameKV = client.GetSecret(keys.RealReportServerUserName);
                    LogHelper.LogInfo($" ... retrieved Report Server User Name from key vault: {ReportServerUserNameKV.Value}");
                    ReportServerUserName = ReportServerUserNameKV.Value;

                    KeyVaultSecret ReportServerDomainKV = client.GetSecret(keys.RealReportServerDomain);
                    LogHelper.LogInfo($" ... retrieved Report Server Domain from key vault: {ReportServerDomainKV.Value}");
                    ReportServerDomain = ReportServerDomainKV.Value;

                    KeyVaultSecret ReportServerPasswordKV = client.GetSecret(keys.RealReportServerPw);
                    LogHelper.LogInfo($" ... retrieved Report Server Password from key vault: {ReportServerPasswordKV.Value}");
                    ReportServerPassword = ReportServerPasswordKV.Value;

                    KeyVaultSecret ECUriKV = client.GetSecret(keys.RealECUriSetting);
                    LogHelper.LogInfo($" ... retrieved EC Uri from key vault: {ECUriKV.Value}");
                    ECUri = ECUriKV.Value;

                    KeyVaultSecret AADTenentKV = client.GetSecret(keys.RealAADTenentSetting);
                    LogHelper.LogInfo($" ... retrieved AAD Tenent from key vault: {AADTenentKV.Value}");
                    AADTenent = AADTenentKV.Value;

                    KeyVaultSecret AADInstanceKV = client.GetSecret(keys.RealAADInstanceSetting);
                    LogHelper.LogInfo($" ... retrieved AAD Instance from key vault: {AADInstanceKV.Value}");
                    AADInstance = AADInstanceKV.Value;

                    KeyVaultSecret OAuthClientIdKV = client.GetSecret(keys.RealOAuthClientIdSetting);
                    LogHelper.LogInfo($" ... retrieved OAuth Client Id from key vault: {OAuthClientIdKV.Value}");
                    OAuthClientId = OAuthClientIdKV.Value;

                    KeyVaultSecret OAuthClientSecretKV = client.GetSecret(keys.RealOAuthClientSecretSetting);
                    LogHelper.LogInfo($" ... retrieved OAuth Client Secret from key vault: {OAuthClientSecretKV.Value}");
                    OAuthClientSecret = OAuthClientSecretKV.Value;

                    KeyVaultSecret OAuthResourceIdKV = client.GetSecret(keys.RealOAuthResourceIdSetting);
                    LogHelper.LogInfo($" ... retrieved OAuth Resource Id from key vault: {OAuthResourceIdKV.Value}");
                    OAuthResourceId = OAuthResourceIdKV.Value;

                    KeyVaultSecret AzureKeyVaultUrlKV = client.GetSecret(keys.RealAzureKeyVaultUrlSetting);
                    LogHelper.LogInfo($" ... retrieved Azure Key Vault Url from key vault: {AzureKeyVaultUrlKV.Value}");
                    AzureKeyVaultUrl = AzureKeyVaultUrlKV.Value;

                    KeyVaultSecret LobApimUriKV = client.GetSecret(keys.RealLobApimUriSetting);
                    LogHelper.LogInfo($" ... retrieved Lob Apim Uri from key vault: {LobApimUriKV.Value}");
                    LobApimUri = LobApimUriKV.Value;

                    KeyVaultSecret OcpApimSubscriptionKeyEKV = client.GetSecret(keys.RealOcpApimSubscriptionKeyESetting);
                    LogHelper.LogInfo($" ... retrieved Ocp Apim Subscription Key East from key vault: {OcpApimSubscriptionKeyEKV.Value}");
                    OcpApimSubscriptionKeyE = OcpApimSubscriptionKeyEKV.Value;

                    KeyVaultSecret OcpApimSubscriptionKeySKV = client.GetSecret(keys.RealOcpApimSubscriptionKeySSetting);
                    LogHelper.LogInfo($" ... retrieved Ocp Apim Subscription Key South from key vault: {OcpApimSubscriptionKeySKV.Value}");
                    OcpApimSubscriptionKeyS = OcpApimSubscriptionKeySKV.Value;

                    KeyVaultSecret OcpApimSubscriptionKeyKV = client.GetSecret(keys.RealOcpApimSubscriptionKeySetting);
                    LogHelper.LogInfo($" ... retrieved Ocp Apim Subscription Key from key vault: {OcpApimSubscriptionKeyKV.Value}");
                    OcpApimSubscriptionKey = OcpApimSubscriptionKeyKV.Value;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            try
            {
                CrmServiceClient selectedServiceClient = null;

                for (var i = 0; i <= 1; i++)
                {
                    try
                    {
                        // NP: Old Code from Sean
                        // Microsoft.Xrm.Tooling.Connector.CrmServiceClient.AuthOverrideHook = new Crm.LOB.Extensions.CRM.CrmServiceClientAuthOverride();

                        // NP: New Code with overloaded ctor

                        Microsoft.Xrm.Tooling.Connector.CrmServiceClient.AuthOverrideHook = new Crm.LOB.Extensions.CRM.XrmToolingAuthOverride(clientId, clientSecret, resource, authority);

                        selectedServiceClient = new CrmServiceClient(new Uri(instanceUri), useUniqueInstance: true);

                        if (selectedServiceClient.IsReady)
                        {
                            break;
                        }
                    }
                    catch (InvalidOperationException e)
                    {
                        LogHelper.LogError(new Uri(instanceUri).Host.Split('.')[0], Guid.Empty, "ConnectionManager", e.Message);
                        throw e;
                    }
                }

                OrgServiceManager = new OrganizationServiceManager(selectedServiceClient);

                VEISConfiguration = new NameValueCollection
                {
                    { "AADTenent", AADTenent ?? AADTenent.ToString() },
                    { "AADInstance", AADInstance ?? AADInstance},
                    { "OAuthClientId", OAuthClientId ?? OAuthClientId},
                    { "OAuthClientSecret", OAuthClientSecret ?? OAuthClientSecret},
                    { "OAuthResourceId", OAuthResourceId ?? OAuthResourceId},
                    { "AzureKeyVaultUrl", AzureKeyVaultUrl ?? AzureKeyVaultUrl},
                    { "ECUri", ECUri ?? ECUri},
                    { "Ocp-Apim-Subscription-Key",  OcpApimSubscriptionKey ?? OcpApimSubscriptionKey },
                    /* keys values below are empty in pre-prod enviornments */
                    { "Ocp-Apim-Subscription-Key-S", OcpApimSubscriptionKeyS ?? OcpApimSubscriptionKeyS },
                    { "Ocp-Apim-Subscription-Key-E",  OcpApimSubscriptionKeyE ?? OcpApimSubscriptionKeyE },
                    /****************************************/
                };

                LOBConfiguration = new NameValueCollection
                {
                    { "AADTenent", AADTenent ?? AADTenent.ToString() },
                    { "AADInstance", AADInstance ?? AADInstance},
                    { "OAuthClientId", OAuthClientId ?? OAuthClientId},
                    { "OAuthClientSecret", OAuthClientSecret ?? OAuthClientSecret},
                    { "OAuthResourceId", OAuthResourceId ?? OAuthResourceId},
                    { "AzureKeyVaultUrl", AzureKeyVaultUrl ?? AzureKeyVaultUrl},
                    { "LobApimUri", LobApimUri ?? LobApimUri}
                };

                SSRSConfiguration = new NameValueCollection
                {
                    { "ReportServerDbConnectionString", ReportServerDbConnectionString ?? ReportServerDbConnectionString },
                    { "ReportFolder", ReportFolder ?? ReportFolder},
                    { "ReportServerUserName", ReportServerUserName ?? ReportServerUserName},
                    { "ReportServerDomain", ReportServerDomain ?? ReportServerDomain},
                    { "ReportServerPassword", ReportServerPassword ?? ReportServerPassword}
                };
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ConnectionManager(Microsoft.Xrm.Sdk.WebServiceClient.OrganizationWebProxyClient organizationWebProxyClient)
        {
            CrmServiceClient selectedServiceClient = null;
            for (var i = 0; i <= 1; i++)
            {
                try
                {

                    selectedServiceClient = new CrmServiceClient(organizationWebProxyClient);

                    if (selectedServiceClient.IsReady)
                    {
                        break;
                    }
                }
                catch (InvalidOperationException e)
                {
                    LogHelper.LogError("UDO", Guid.Empty, "ConnectionManager", e.Message);
                }
            }

            OrgServiceManager = new OrganizationServiceManager(selectedServiceClient);

            if (_settingsCache != null && _settingsCache["ExecuteMultipleSettings"] != null)
            {
                _emhSettings = (ExecuteMultipleHelperSettings)_settingsCache["ExecuteMultipleSettings"];
            }

            if (_emhSettings == null || _emhSettings.IsExpired())
            {
                using (CrmServiceClient svc = this.GetProxy())
                {
                    _emhSettings = ExecuteMultipleHelperSettings.LoadFromCRM(svc);
                }
            }
        }

        public bool IsTokenExpired = true;

        public static ConnectionManager GetConnectionManager(string callerName, bool isRunningOnLocalMachine)
        {

            ConnectionManager manager = new ConnectionManager(callerName, isRunningOnLocalMachine);

            return manager;
        }

        public void Dispose()
        {
            if (OrgServiceManager != null)
            {
                OrgServiceManager.Dispose();
            }
        }
#endregion
    }

    internal class CrmConnectionParms
    {
        public string Organization { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string Domain { get; set; }
        public string OrganizationServiceUrl { get; set; }
        public string DiscoveryServiceUrl { get; set; }
        public string Name { get; set; }
        public string OrganizationId { get; set; }
        public string ServerName { get; set; }
    }
}
