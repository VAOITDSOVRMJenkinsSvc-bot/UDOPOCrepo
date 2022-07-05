using System;
using UDO.LOB.Extensions.Logging;

namespace UDO.LOB.Extensions
{
    public class KeyVaultSecretKeyNames
    {
        public KeyVaultSecretKeyNames(string assemblyUppercase, string environmentUppercase = null)
        {
            //NEW CODE TO GET SETTING FROM WEB APP CONFIGURATION SECTIONS - this is called from LOB, and on startup          
            RealCrmBaseUrlSetting = "CrmBaseUrl";
            RealClientIdSetting = "ClientId-" + assemblyUppercase;
            RealClientSecretSetting = "ClientSecret-" + assemblyUppercase;
            RealAuthoritySetting = "Authority";
            RealOrganizationServiceRelativeUrl = "OrganizationServiceRelativeUrl";

            //SSRS CONFIGURATION SETTINGS
            RealReportServerDbConnectionString = "ReportServerDbConnectionString"; //Changes based on virtual env
            RealReportFolder = "ReportFolder"; //Changes based on virtual env
            RealReportServerUserName = "ReportServerUserName";
            RealReportServerDomain = "ReportServerDomain";
            RealReportServerPw = "ReportServerPassword"; // Fortify does not take kindly to public stringiables named Password
    
            //VEIS CONFIGURATION SETTINGS
            RealECUriSetting = "ECUri";
            RealAADTenentSetting = "AADTenent";
            RealAADInstanceSetting = "AADInstance";
            RealOAuthClientIdSetting = "OAuthClientId";
            RealOAuthClientSecretSetting = "OAuthClientSecret";
            RealOAuthResourceIdSetting = "OAuthResourceId";
            RealAzureKeyVaultUrlSetting = "AzureKeyVaultUrl";
            RealOcpApimSubscriptionKeySetting = "Ocp-Apim-Subscription-Key";
            RealOcpApimSubscriptionKeySSetting = "Ocp-Apim-Subscription-Key-S";
            RealOcpApimSubscriptionKeyESetting = "Ocp-Apim-Subscription-Key-E";

            //LOB CONFIGURATION SETTING
            RealLobApimUriSetting = "LobApimUri";

            ModifyKeysByEnvironment(environmentUppercase);
        }

        //NEW CODE TO GET SETTING FROM WEB APP CONFIGURATION SECTIONS - this is called from LOB, and on startup          
        public string RealCrmBaseUrlSetting { get; set; }
        public string RealClientIdSetting { get; set; }
        public string RealClientSecretSetting { get; set; }
        public string RealAuthoritySetting { get; set; }
        public string RealOrganizationServiceRelativeUrl { get; set; }

        //SSRS CONFIGURATION SETTINGS
        public string RealReportServerDbConnectionString { get; set; }
        public string RealReportFolder { get; set; }
        public string RealReportServerUserName { get; set; }
        public string RealReportServerDomain { get; set; }
        public string RealReportServerPw { get; set; }

        //VEIS CONFIGURATION SETTINGS
        public string RealECUriSetting { get; set; }
        public string RealAADTenentSetting { get; set; }
        public string RealAADInstanceSetting { get; set; }
        public string RealOAuthClientIdSetting { get; set; }
        public string RealOAuthClientSecretSetting { get; set; }
        public string RealOAuthResourceIdSetting { get; set; }
        public string RealAzureKeyVaultUrlSetting { get; set; }
        public string RealOcpApimSubscriptionKeySetting { get; set; }
        public string RealOcpApimSubscriptionKeySSetting { get; set; }
        public string RealOcpApimSubscriptionKeyESetting { get; set; }

        //LOB CONFIGURATION SETTING
        public string RealLobApimUriSetting { get; set; }

        /// <summary>
        /// Modifies the keys by environment.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        private void ModifyKeysByEnvironment(string environmentUppercase = null)
        {
            try
            {
                if (string.IsNullOrEmpty(environmentUppercase))
                {
                    var virtualPath = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;

                    if (virtualPath != "/")
                    {
                        var folder = virtualPath.Replace("/", string.Empty);
                        RealCrmBaseUrlSetting += "-" + folder;
                        RealClientIdSetting += "-" + folder;
                        RealClientSecretSetting += "-" + folder;
                        RealLobApimUriSetting += "-" + folder;
                        RealECUriSetting += "-" + folder;
                        RealReportServerDbConnectionString += "-" + folder;
                        RealReportFolder += "-" + folder;
                    }
                    else if (System.Web.Hosting.HostingEnvironment.SiteName.Contains("prod"))
                    {
                        if (System.Web.Hosting.HostingEnvironment.SiteName.Contains("south"))
                        {
                            RealECUriSetting += "-south";
                            RealLobApimUriSetting += "-south";
                        }
                        else
                        {
                            RealECUriSetting += "-east";
                            RealLobApimUriSetting += "-east";
                        }
                    }
                }
                else
                {
                    if (environmentUppercase != "DEV" && environmentUppercase != "" && environmentUppercase.StartsWith("PROD") == false)
                    {
                        RealCrmBaseUrlSetting += "-" + environmentUppercase;
                        RealClientIdSetting += "-" + environmentUppercase;
                        RealClientSecretSetting += "-" + environmentUppercase;
                        RealLobApimUriSetting += "-" + environmentUppercase;
                        RealECUriSetting += "-" + environmentUppercase;
                        RealReportServerDbConnectionString += "-" + environmentUppercase;
                        RealReportFolder += "-" + environmentUppercase;
                    }
                    else if (environmentUppercase == "PRODEAST")
                    {
                        RealECUriSetting += "-east";
                        RealLobApimUriSetting += "-east";
                    }
                    else if (environmentUppercase == "PRODSOUTH")
                    {
                        RealECUriSetting += "-south";
                        RealLobApimUriSetting += "-south";
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogInfo("ConnectionManager:: Not able to find new folder: " + ex.Message);
            }
        }
    }
}
