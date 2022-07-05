using MCSPlugins;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Va.Udo.Crm.Plugins.Interaction
{
    public class InteractionCloseRunner : PluginRunner
    {
        public InteractionCloseRunner(IServiceProvider serviceProvider)
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
            return PluginExecutionContext.InputParameters["Target"] as Entity;
        }

        /// <summary>
        /// Debug field
        /// </summary>
        public override string McsSettingsDebugField
        {
            get { return String.Empty; }
        }
        /// <summary>
        /// 
        /// </summary>
        internal void Execute()
        {
            try
            {
                var entity = GetPrimaryEntity();
                if (entity == null || entity.LogicalName != "udo_interaction") return;

                if (!entity.Attributes.Contains("udo_status")) return;

                var status = entity.GetAttributeValue<bool>("udo_status");
                if (status) return;

                var fetchXml = string.Format(
                    @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' count='1'>"
                        + @"<entity name='contact'>"
                            + @"<attribute name='ownerid' />"
                            + @"<attribute name='udo_veteransensitivitylevel' />"
                            + @"<order attribute='udo_veteransensitivitylevel' descending='false' />"
                            + @"<filter type='and'>"
                                + @"<condition attribute='udo_veteransensitivitylevel' operator='not-null' />"
                            + @"</filter>"
                            + @"<link-entity name='udo_idproof' from='udo_veteran' to='contactid' alias='ag'>"
                                + @"<link-entity name='udo_interaction' from='udo_interactionid' to='udo_interaction' alias='ah'>"
                                    + @"<filter type='and'>"
                                        + @"<condition attribute='udo_interactionid' operator='eq' uitype='udo_interaction' value='{0}' />"
                                    + @"</filter>"
                                + @"</link-entity>"
                            + @"</link-entity>"
                        + @"</entity>"
                    + @"</fetch>", entity.Id);

                var result = OrganizationService.RetrieveMultiple(new FetchExpression(fetchXml));

                if (result != null && result.Entities.Count > 0)
                {
                    var owner = result.Entities[0].GetAttributeValue<EntityReference>("ownerid");

                    AssignRequest req = new AssignRequest()
                    {
                        Assignee = owner,
                        Target = entity.ToEntityReference()
                    };

                    OrganizationService.Execute(req);
                }
            }
            catch (Exception ex)
            {
                PluginError = true;
                Trace(string.Format("{0}", ex.Message));
            }
            finally
            {
                Trace("Entered Finally");
                SetupLogger();
                Trace("Set up logger done.");
                ExecuteFinally();
                Trace("Exit Finally");
            }
        }
    }
}