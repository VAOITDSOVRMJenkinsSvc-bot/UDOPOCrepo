using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRM.CRME.Plugin.DependentMaintenance
{
    public class SettingsHelper
    {

        public static CrmeSettings GetSettingValues(IOrganizationService organizationService, IPluginExecutionContext pluginExecutionContext)
        {
            CrmeSettings crmeSettings = null;

            QueryByAttribute query = new QueryByAttribute
            {
                ColumnSet = new ColumnSet("crme_restendpointforvimt"),
                EntityName = "mcs_setting"
            };
            query.AddAttributeValue("mcs_name", "Active Settings");

            EntityCollection results = organizationService.RetrieveMultiple(query);
            if (results.Entities.Count > 0)
            {
                crmeSettings = new CrmeSettings();

                crmeSettings.RestEndPointForVimt = results.Entities[0]["crme_restendpointforvimt"].ToString();
            }

            if(crmeSettings != null) {
                crmeSettings.LogSettings = new VRMRest.LogSettings() { Org = pluginExecutionContext.OrganizationName, ConfigFieldName = "REST", UserId = pluginExecutionContext.InitiatingUserId };
            }
            
            return crmeSettings;
        }

    }

    public class CrmeSettings
    {
        public string RestEndPointForVimt { get; set; }
        public VRMRest.LogSettings LogSettings { get; set; }
    }

}
