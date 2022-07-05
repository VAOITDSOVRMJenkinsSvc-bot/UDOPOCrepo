using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;
using System.Linq;
using Microsoft.Xrm.Sdk.Query;
using MCSHelperClass;
using MCSUtilities2011;
using System.Globalization;
using System.Diagnostics;

namespace Va.Udo.Crm.SpecialSituation.Plugins
{
    //Purpose:  The purpose of this plugin is to create the system table entries needed for services to work - this is done on create and may have to be updated on update depending on
    //           The business rule that is decided.

    public class InteractionSpecialSituationPostDisassociateRunner : MCSPlugins.PluginRunner
    {
        public override Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.InputParameters["Target"] as Entity;
        }
        #region Constructor
        public InteractionSpecialSituationPostDisassociateRunner(IServiceProvider serviceProvider)
            : base(serviceProvider){}
        #endregion

        #region debug
        public override string McsSettingsDebugField
        {
            get { return "udo_interaction"; }
        }
        #endregion

        #region Internal Methods/Properties
        internal EntityCollection GetSpecialSituations(EntityReference interaction)
        {
            //Logger.WriteDebugMessage("At the top of GetSpecialSituations");
            TracingService.Trace("At the top of GetSpecialSituations");

            try
            {
                string intSitFetch = "<fetch><entity name='udo_interactionspecialsituation'>" +
                           "<attribute name='udo_interactionspecialsituationid'/>" +
                           "<filter type='and'>" +
                           "<condition attribute='udo_interactionid' operator='eq' value='" + interaction.Id + "'/>" +
                           "</filter></entity></fetch>";

                return OrganizationService.RetrieveMultiple(new FetchExpression(intSitFetch));
            }
            catch
            {
                return null;
            }
        }
        
        internal void Execute()
        {
            try
            {
                Stopwatch txnTimer = Stopwatch.StartNew();
                #region logging and parameters
                Logger.setMethod = "Execute";
                var target = (EntityReference)PluginExecutionContext.InputParameters["Target"];
                var related = (EntityReferenceCollection)PluginExecutionContext.InputParameters["RelatedEntities"];
                #endregion 

                #region validate message type
                if (PluginExecutionContext.Depth > 2) return;

                if (!PluginExecutionContext.InputParameters.Contains("Relationship") ||
                    !PluginExecutionContext.InputParameters.Contains("Target") ||
                    !PluginExecutionContext.InputParameters.Contains("RelatedEntities"))
                {
                    return;
                }

                var relationship = (Relationship)PluginExecutionContext.InputParameters["Relationship"];

                if (relationship.SchemaName != "udo_udo_interaction_udo_interactionspecialsit")
                {
                    return; // not the type of relationship for this plugin..
                }
                #endregion

                #region delete interactionspecialsituation

                Entity interaction = new Entity();
                if (target.LogicalName == "udo_interaction")
                {
                    OrganizationService.Delete("udo_interactionspecialsituation", related.FirstOrDefault().Id);
                    interaction = OrganizationService.Retrieve("udo_interaction", target.Id, new ColumnSet(new[] { "udo_hasspecialsituations" }));
                }
                else
                {
                    OrganizationService.Delete("udo_interactionspecialsituation", target.Id);
                    interaction = OrganizationService.Retrieve("udo_interaction", related.FirstOrDefault().Id, new ColumnSet(new[] { "udo_hasspecialsituations" }));
                }
                #endregion 

                #region update interaction

                var specialSituations = GetSpecialSituations(new EntityReference("udo_interaction", interaction.Id));
                if (specialSituations == null || specialSituations.Entities.Count() == 0)
                {
                    interaction["udo_hasspecialsituations"] = false;
                    interaction["udo_specialsituationstring"] = String.Empty;
                    OrganizationService.Update(interaction);
                    return;
                }

                interaction["udo_hasspecialsituations"] = true;
                string situationString = null;
                foreach (Entity sit in specialSituations.Entities)
                {
                    if (situationString != null) situationString += ", ";
                    situationString += OrganizationService.Retrieve("udo_interactionspecialsituation", sit.Id, new ColumnSet(new[] { "udo_name" })).GetAttributeValue<string>("udo_name");
                }

                interaction["udo_specialsituationstring"] = situationString;
                OrganizationService.Update(interaction);
                #endregion 

            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Logger.WriteToFile(ex.Message);
                Logger.WriteToFile(ex.StackTrace);
                TracingService.Trace(ex.Message);
                TracingService.Trace(ex.StackTrace);
                throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
            }
            catch (Exception ex)
            {
                Logger.WriteToFile(ex.Message);
                Logger.WriteToFile(ex.StackTrace);
                TracingService.Trace(ex.Message);
                TracingService.Trace(ex.StackTrace);
                throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
            }
        }

        public override Entity GetSecondaryEntity()
        {
            return (Entity)PluginExecutionContext.InputParameters["Target"];
        }
        #endregion
    }
}

