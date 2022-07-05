using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;
using System.Linq;
using Microsoft.Xrm.Sdk.Query;
using MCSHelperClass;
using MCSUtilities2011;
using System.Globalization;
using UDO.Model;
using System.Diagnostics;

namespace Va.Udo.Crm.SpecialSituation.Plugins
{
    //Purpose:  The purpose of this plugin is to manage the names of interactionspecialsituation entities and to update the interaction with a string concatenation of all interactionspecialsituation names related to it.

    public class InteractionSpecialSituationPreCreateDeleteRunner : MCSPlugins.PluginRunner
    {
        public override Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.InputParameters["Target"] as Entity;
        }

        #region Constructor
        public InteractionSpecialSituationPreCreateDeleteRunner(IServiceProvider serviceProvider)
            : base(serviceProvider){}
        #endregion

        #region debug
        public override string McsSettingsDebugField
        {
            get { return "udo_interaction"; }
        }
        #endregion

        #region Internal Methods/Properties

        internal EntityCollection GetSpecialSituations(Guid interactionId)
        {
            EntityCollection specialSituations = new EntityCollection();
            using (var xrm = new UDOContext(OrganizationService))
            {
                var getSits = from sit in xrm.udo_interactionspecialsituationSet
                              where sit.udo_InteractionId.Id == interactionId
                              select new udo_interactionspecialsituation
                              {
                                  udo_interactionspecialsituationId = sit.udo_interactionspecialsituationId,
                                  udo_name = sit.udo_name
                              };
                if (getSits != null)
                {
                    foreach (var sit in getSits)
                    {
                        Entity thisSit = new Entity();
                        thisSit.Id = sit.udo_interactionspecialsituationId.Value;
                        thisSit.LogicalName = "udo_interactionspecialsituation";
                        thisSit["udo_name"] = sit.udo_name;
                        specialSituations.Entities.Add(thisSit);
                    }
                }
                return specialSituations;
            }
        }

        internal void Execute()
        {
            try
            {
                Stopwatch txnTimer = Stopwatch.StartNew();
                #region logging and set parameters
                Entity entity = new Entity();
                if (PluginExecutionContext.MessageName == "Create")
                {
                    entity = GetPrimaryEntity();
                }
                else if (PluginExecutionContext.MessageName == "Delete")
                {
                    entity = PluginExecutionContext.PreEntityImages["PreDeleteImage"];
                }
                else return; //registered on wrong step
                EntityReference interaction = entity.GetAttributeValue<EntityReference>("udo_interactionid");
                EntityReference specialSituation = entity.GetAttributeValue<EntityReference>("udo_specialsituationid");
                if (interaction == null)
                {
                    throw new InvalidOperationException("No udo_interaction specified. An interaction must be associated for the interactionspecialsituation to be valid.");
                }
                #endregion

                #region Set interactionspecialsituation name
                if (PluginExecutionContext.MessageName == "Create")
                {
                    if (specialSituation != null)
                    {
                        //For some reason, on manual creates the entity reference is returning with an ID but empty Name. Retrieving name here.
                        entity["udo_name"] = OrganizationService.Retrieve("udo_specialsituation", specialSituation.Id, new ColumnSet(new[] { "udo_name" })).GetAttributeValue<string>("udo_name");
                    }
                    else if (entity.GetAttributeValue<string>("udo_other") != null)
                    {
                        entity["udo_name"] = entity.GetAttributeValue<string>("udo_other");
                    }
                    else
                    {
                        throw new InvalidOperationException("No Special Situation provided. A special situation (or Other) must be associated for the interactionspecialsituation to be valid.");
                    }
                }
                #endregion

                #region update interaction
                EntityCollection specialSituations = GetSpecialSituations(interaction.Id);
                Entity updateInteraction = OrganizationService.Retrieve("udo_interaction", interaction.Id, new ColumnSet(new[] { "udo_interactionid", "udo_hasspecialsituations", "udo_specialsituationstring" }));

                if (PluginExecutionContext.MessageName == "Delete" && specialSituations.Entities.Count() <= 1)
                {
                    updateInteraction["udo_hasspecialsituations"] = false;
                    updateInteraction["udo_specialsituationstring"] = String.Empty;
                    OrganizationService.Update(updateInteraction);
                    return;
                }
                string specialSituationString = null;
                if (PluginExecutionContext.MessageName == "Create")
                {
                    specialSituationString += entity.GetAttributeValue<string>("udo_name");
                }

                foreach (Entity sit in specialSituations.Entities)
                {
                    if (sit.Id != entity.Id)
                    {
                        if (specialSituationString != null) specialSituationString += ", ";
                        specialSituationString += sit.GetAttributeValue<string>("udo_name");
                    }
                }
                updateInteraction["udo_hasspecialsituations"] = true;
                updateInteraction["udo_specialsituationstring"] = specialSituationString;
                OrganizationService.Update(updateInteraction);

                txnTimer.Stop();
                Logger.setMethod = "InteractionSpecialSituationPreCreateDeleteRunner";
                //Logger.WriteTxnTimingMessage(String.Format("Timing for : {0}", GetType()), txnTimer.ElapsedMilliseconds);
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

