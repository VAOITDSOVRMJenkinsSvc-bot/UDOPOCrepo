using MCSPlugins;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace Va.Udo.Crm.Plugins.Veteran
{
    public class VeteranRunner : PluginRunner
    {
        private const string _TeamName = "PCR";

        public VeteranRunner(IServiceProvider serviceProvider)
            : base(serviceProvider) { }

        /// <summary>
        /// Retrieves the primary entity
        /// </summary>
        /// <returns>Primary entity</returns>
        public override Microsoft.Xrm.Sdk.Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.InputParameters["Target"] as Entity;
        }

        /// <summary>
        /// Retrieves the secondary entity
        /// </summary>
        /// <returns></returns>
        public override Microsoft.Xrm.Sdk.Entity GetSecondaryEntity()
        {
            return PluginExecutionContext.PreEntityImages["pre"] as Entity;
        }

        /// <summary>
        /// Debug field
        /// </summary>
        public override string McsSettingsDebugField
        {
            get { return "udo_security"; }
        }


        internal void Execute()
        {
            var progress = string.Empty;
            try
            {
                //Logger.WriteDebugMessage("in security");
                TracingService.Trace("in security");

                var message = PluginExecutionContext.MessageName;

                if (!message.Equals("Create", StringComparison.InvariantCultureIgnoreCase) &&
                    !message.Equals("Update", StringComparison.InvariantCultureIgnoreCase))
                {
                    return; // unsupported message
                }

                var entity = GetPrimaryEntity();

                if (entity == null || !entity.LogicalName.Equals("Contact", StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }

                //Setup Defaults
                //var sensitivityLevel = 752280000;
                var sensitivityLevel = 0;

                if (PluginExecutionContext.MessageName.Equals("update", StringComparison.InvariantCultureIgnoreCase))
                {
                    progress = "Checking for update assignment";
                    if (!UpdateAssignment(entity, out sensitivityLevel))
                    {
                        //Logger.WriteDebugMessage("Owner is PCR or sensitivity levels match, getting out");
                        TracingService.Trace("Owner is PCR or sensitivity levels match, getting out");
                        return;
                    }
                }

                progress = "Get team for security level using business unit";

                var team = GetTeam(_TeamName, sensitivityLevel);
                
                if (team == null)
                {
                    throw new InvalidPluginExecutionException("Default team not configured ");
                }

                progress = "Updating assignment";
                //Logger.WriteDebugMessage("Updating assignment");
                TracingService.Trace("Updating assignment");
                //TODO: Update assignrequest
                AssignRequest req = new AssignRequest()
                {
                    Assignee = team,
                    Target = entity.ToEntityReference()
                };
                OrganizationService.Execute(req);
                progress = "Record assigned successfully";
            }
            catch (Exception ex)
            {
                //Logger.WriteDebugMessage("Updating assignment error:" + ex.Message + "boom: " + progress);
                TracingService.Trace("Updating assignment error:" + ex.Message + "boom: " + progress);
                new InvalidPluginExecutionException("boom: " + progress);
            }
        }

        private bool UpdateAssignment(Entity entity, out int sensitivityLevel)
        {
            sensitivityLevel = 0;

            var preEntity = GetSecondaryEntity();
            if (preEntity == null)
                throw new InvalidPluginExecutionException("preEntity is null");

            // Get Sensitivity Levels
            var pre_value = 0;
            var pre_sl_optionset = preEntity.GetAttributeValue<OptionSetValue>("udo_veteransensitivitylevel");
            if (pre_sl_optionset != null) pre_value = pre_sl_optionset.Value;

            var entity_sl_optionset = entity.GetAttributeValue<OptionSetValue>("udo_veteransensitivitylevel");
            if (entity_sl_optionset != null) sensitivityLevel = entity_sl_optionset.Value;

            var currentOwner = preEntity.GetAttributeValue<EntityReference>("ownerid");

            // If owner is not PCR or the pre and current sensitivity levels do not match, then return true
            return (currentOwner != null && currentOwner.Name != "PCR") || !sensitivityLevel.Equals(pre_value);
        }

        private EntityReference GetTeam(string name, int securitylevel)
        {
            var fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' count='1'>"
                      + @"<entity name='team'>"
                      + @"<attribute name='teamid' />"
                      + @"<filter type='and'>"
                      + @"<condition attribute='name' operator='eq' value='{0}' />"
                      + @"</filter>"
                      + @"<link-entity name='businessunit' from='businessunitid' to='businessunitid' alias='ad'>"
                      + @"<filter type='and'>"
                      + @"<condition attribute='udo_veteransensitivitylevel' operator='eq' value='{1}' />"
                      + @"</filter>"
                      + @"</link-entity>"
                      + @"</entity>"
                      + @"</fetch>";

            var query = String.Format(fetch, name, securitylevel);

            var fe = new FetchExpression(query);

            var result = base.OrganizationService.RetrieveMultiple(fe);

            if (result == null || result.Entities.Count == 0) return null;

            return result.Entities[0].ToEntityReference();
        }
    }
}
