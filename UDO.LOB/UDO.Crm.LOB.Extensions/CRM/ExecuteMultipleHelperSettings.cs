using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Net;
using System.Web.Caching;
using UDO.LOB.Extensions.Logging;

namespace UDO.LOB.Extensions
{
    public enum ProcessMethodType
    {
        PFELibrary,
        Direct
    }

    public class ExecuteMultipleHelperSettings
    {
        static Cache _settingsCache = new Cache();
        public ExecuteMultipleHelperSettings()
        {
            BatchSize = 10;
            MinBatches = 3;
            MaxDegreeOfParallelism = 25;
            Loaded = false;
            ProcessMethod = ProcessMethodType.PFELibrary;
            OrganizationServiceProxyUri = null;
        }

        public ProcessMethodType ProcessMethod;

        public int BatchSize { get; set; }

        public int MinBatches { get; set; }

        public int MaxDegreeOfParallelism { get; set; }

        public ConnectionSettings DefaultConnectionSettings { get; set; }

        public ConnectionSettings CRMConnectionSettings { get; set; }

        public bool Loaded { get; set; }

        public DateTime ExpiresOn { get; set; }

        public static ExecuteMultipleHelperSettings Default
        {
            get
            {
                return new ExecuteMultipleHelperSettings();
            }
        }

        public Uri OrganizationServiceProxyUri {get;set;}
        public string ApplyConnectionSettings(Uri uri)
        {
            if (uri == null) uri = OrganizationServiceProxyUri;

            var result = "";
            if (CRMConnectionSettings != null)
            {
                var sp = ServicePointManager.FindServicePoint(uri);
                result += CRMConnectionSettings.UpdateServicePoint(sp);
            }
            if (DefaultConnectionSettings != null)
                result += DefaultConnectionSettings.UpdateServicePointManager();
            return result;
        }

        public static ExecuteMultipleHelperSettings LoadFromCRM(IOrganizationService proxy)
        {
            ExecuteMultipleHelperSettings settings = new ExecuteMultipleHelperSettings();
            if (_settingsCache != null && _settingsCache["ExecuteMultipleSettings"] != null)
            {
                settings = (ExecuteMultipleHelperSettings)_settingsCache["ExecuteMultipleSettings"];
            }
            else
            {
                var fetch = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' limit='1'>"
                        + "<entity name='mcs_setting'>"
                        + "<attribute name='mcs_settingid' />"
                        + "<attribute name='mcs_name' />"
                        + "<attribute name='udo_udousenaglealgorithm' />"
                        + "<attribute name='udo_udoexpect100continue' />"
                        + "<attribute name='udo_udoconnectionlimit' />"
                        + "<attribute name='udo_em_requestsperbatch' />"
                        + "<attribute name='udo_em_minbatches' />"
                        + "<attribute name='udo_em_processmethod' />"
                        + "<attribute name='udo_em_maxdegreeofparallelism' />"
                        + "<attribute name='udo_defaultconnectionlimit' />"
                        + "<filter type='and'>"
                        + "<condition attribute='mcs_name' operator='eq' value='Active Settings' />"
                        + "</filter></entity></fetch>";

                var fe = new FetchExpression(fetch);


                var responses = proxy.RetrieveMultiple(fe);
                LogHelper.LogInfo($"{MethodInfo.GetThisMethod().Method} :: Invoked CRM: ExecuteMultipleHelperSettings");
                if (responses.Entities.Count > 0)
                {
                    var configentity = responses.Entities[0];

                    settings.CRMConnectionSettings = new ConnectionSettings();
                    if (configentity.Contains("udo_udousenaglealgorithm"))
                    {
                        settings.CRMConnectionSettings.UseNagleAlgorithm = (((OptionSetValue)configentity["udo_udousenaglealgorithm"]).Value == 1);
                    }
                    if (configentity.Contains("udo_udoexpect100continue"))
                    {
                        settings.CRMConnectionSettings.Expect100Continue = (((OptionSetValue)configentity["udo_udoexpect100continue"]).Value == 1);
                    }
                    if (configentity.Contains("udo_udoconnectionlimit"))
                    {
                        settings.CRMConnectionSettings.ConnectionLimit = (int)configentity["udo_udoconnectionlimit"];
                    }
                    settings.DefaultConnectionSettings = new ConnectionSettings();
                    if (configentity.Contains("udo_defaultconnectionlimit"))
                    {
                        settings.DefaultConnectionSettings.ConnectionLimit = (int)configentity["udo_defaultconnectionlimit"];
                    }
                    if (configentity.Contains("udo_em_maxdegreeofparallelism"))
                    {
                        settings.MaxDegreeOfParallelism = (int)configentity["udo_em_maxdegreeofparallelism"];
                    }
                    if (configentity.Contains("udo_em_requestsperbatch"))
                    {
                        settings.BatchSize = (int)configentity["udo_em_requestsperbatch"];
                    }
                    if (configentity.Contains("udo_em_minbatches"))
                    {
                        settings.MinBatches = (int)configentity["udo_em_minbatches"];
                    }
                    if (configentity.Contains("udo_em_processmethod"))
                    {
                        ProcessMethodType method = ProcessMethodType.PFELibrary;
                        var osv = configentity.GetAttributeValue<OptionSetValue>("udo_em_processmethod");
                        if (osv.Value == 752280001) method = ProcessMethodType.Direct;
                        settings.ProcessMethod = method;
                    }
                }
                else
                {
                    settings = Default;                    
                }

                var expiresOn = DateTime.UtcNow;
                var mintoadd = 15 - (expiresOn.Minute % 15);
                if (mintoadd < 1) mintoadd = 15;
                expiresOn.AddMinutes(mintoadd);
                settings.ExpiresOn = expiresOn;

                if (proxy != null)
                {
                    CrmServiceClient proxyCrm = (CrmServiceClient)proxy;
                    if (proxyCrm.CrmConnectOrgUriActual != null)
                    {
                        settings.OrganizationServiceProxyUri = proxyCrm.CrmConnectOrgUriActual;
                    }
                }

                settings.ApplyConnectionSettings(null);
                AddToCache(_settingsCache, "ExecuteMultipleSettings", settings);
            }
                
            return settings;
        }

        private static void AddToCache(Cache cache, string key, object cacheValue)
        {
            if (cache != null && cache[key] == null)
            {
                cache.Add(key, cacheValue, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1440), CacheItemPriority.High, null);
            }
        }

        public bool IsExpired()
        {
            return (DateTime.Now >= this.ExpiresOn);
        }
    }
}