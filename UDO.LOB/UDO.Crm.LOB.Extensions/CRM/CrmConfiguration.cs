using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UDO.LOB.Extensions.Logging;

namespace UDO.LOB.Extensions
{
    public class CrmConfiguration
    {
        public const string Tenant = "CRMTenent";
        public const string BaseUrl = "CrmBaseUrl";

        public static NameValueCollection GetConfigurationSettings()
        {

            //NEW CODE TO GET SETTING FROM WEB APP CONFIGURATION SECTIONS
            var realRelativeSetting = "OrganizationServiceRelativeUrl";
            var realSetting = CrmConfiguration.BaseUrl;

            try
            {
                var folder = HttpContext.Current?.Request.Url.Segments[1];

                //called from where connection is called on startup and execute

                if (folder != null && folder != "api/")
                {

                    folder = folder.Replace("/", string.Empty);
                    realSetting += "-" + folder;
                    realRelativeSetting += "-" + folder;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogInfo($"GetConfigurationSettings :: Not able to find new folder:" + ex.Message);

            }
            //Read config settings
            NameValueCollection configValueColl = new NameValueCollection
            {
                { CrmConfiguration.BaseUrl,
                    ConfigurationManager.AppSettings[realSetting] ?? ConfigurationManager.AppSettings[realSetting].ToString() }

            };

          
            return configValueColl;
        }
    }
}
