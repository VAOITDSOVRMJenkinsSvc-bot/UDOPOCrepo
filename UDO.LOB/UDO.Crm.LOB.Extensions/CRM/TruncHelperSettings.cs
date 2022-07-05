using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Caching;
using UDO.LOB.Extensions.Logging;

namespace UDO.LOB.Extensions
{
    public class TruncHelperSettings
    {
        private volatile object syncRoot = new Object();
        private static readonly string TimestampFormat = "yyyy-MM-dd hh:mm:ss:fff";
        static Cache _settingsCache = new Cache();

        public TruncHelperSettings()
        {
            ForceMetadataRefresh = false;
            LogTruncations = true;
            SettingId = Guid.Empty;
        }

        public bool LogTruncations { get; set; }
        public bool ForceMetadataRefresh { get; set; }
        public Guid SettingId { get; set; }
        public DateTime ExpiresOn { get; set; }

        public void Update(OrganizationServiceProxy proxy)
        {
            Entity e = new Entity("mcs_setting");
            e.Id = SettingId;
            e.Attributes.Add("udo_trunchelper_logtrunc", LogTruncations);
            e.Attributes.Add("udo_trunchelper_metadatarefresh", ForceMetadataRefresh);
            proxy.Update(e);
        }

        public void Update(IOrganizationService organizationService)
        {
            Entity e = new Entity("mcs_setting");
            e.Id = SettingId;
            e.Attributes.Add("udo_trunchelper_logtrunc", LogTruncations);
            e.Attributes.Add("udo_trunchelper_metadatarefresh", ForceMetadataRefresh);
            organizationService.Update(e);
        }

        public void Load(OrganizationServiceProxy proxy)
        {
            var fetch = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' limit='1'>"
                        + "<entity name='mcs_setting'>"
                        + "<attribute name='mcs_settingid' />"
                        + "<attribute name='mcs_name' />"
                        + "<attribute name='udo_trunchelper_logtrunc' />"
                        + "<attribute name='udo_trunchelper_metadatarefresh' />"
                        + "<filter type='and'>"
                        + "<condition attribute='mcs_name' operator='eq' value='Active Settings' />"
                        + "</filter></entity></fetch>";

            var fe = new FetchExpression(fetch);

            var responses = proxy.RetrieveMultiple(fe);
            if (responses.Entities.Count > 0)
            {
                var entity = responses.Entities[0];
                TruncHelperSettings settings = new TruncHelperSettings();
                LogTruncations = entity.GetAttributeValue<bool>("udo_trunchelper_logtrunc");
                ForceMetadataRefresh = entity.GetAttributeValue<bool>("udo_trunchelper_metadatarefresh");
                SettingId = entity.Id;
            }
        }

        public TruncHelperSettings Load(IOrganizationService organizationService)
        {
            TruncHelperSettings settings = new TruncHelperSettings();
            if (_settingsCache != null && _settingsCache["TruncateSettings"] != null)
            {
                settings = (TruncHelperSettings)_settingsCache["TruncateSettings"];
            }
            else
            {
                var fetch = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' limit='1'>"
                        + "<entity name='mcs_setting'>"
                        + "<attribute name='mcs_settingid' />"
                        + "<attribute name='mcs_name' />"
                        + "<attribute name='udo_trunchelper_logtrunc' />"
                        + "<attribute name='udo_trunchelper_metadatarefresh' />"
                        + "<filter type='and'>"
                        + "<condition attribute='mcs_name' operator='eq' value='Active Settings' />"
                        + "</filter></entity></fetch>";

                var fe = new FetchExpression(fetch);

                var responses = organizationService.RetrieveMultiple(fe);

                LogHelper.LogInfo($"LOGINFO: [{DateTime.Now.ToString(TimestampFormat)}] {MethodInfo.GetThisMethod().Method} :: Invoked CRM: TruncateHelperSettings");

                if (responses.Entities.Count > 0)
                {
                    var entity = responses.Entities[0];
                    settings = new TruncHelperSettings()
                    {
                        LogTruncations = entity.GetAttributeValue<bool>("udo_trunchelper_logtrunc"),
                        ForceMetadataRefresh = entity.GetAttributeValue<bool>("udo_trunchelper_metadatarefresh"),
                        SettingId = entity.Id
                    };


                    LogTruncations = entity.GetAttributeValue<bool>("udo_trunchelper_logtrunc");
                    ForceMetadataRefresh = entity.GetAttributeValue<bool>("udo_trunchelper_metadatarefresh");
                    SettingId = entity.Id;
                    AddToCache(_settingsCache, "TruncateSettings", settings);
                }
            }
            return settings;
        }

        private void AddToCache(Cache cache, string key, object cacheValue)
        {
            if (cache != null && cache[key] == null)
            {
                cache.Add(key, cacheValue, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1440), CacheItemPriority.High, null);
            }
        }

    }
}