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
using System.Xml.Linq;
using System.Diagnostics;

namespace Va.Udo.Crm.Queue.Plugins
{
    //Purpose:  This plugin executes on Assign of Interaction to ensure it is not incorrectly assigned away from PCR team by queue management

    public class InteractionPreAssignRunner : MCSPlugins.PluginRunner
    {
        private const string _TeamName = "PCR";
        #region Constructor
        public InteractionPreAssignRunner(IServiceProvider serviceProvider)
            : base(serviceProvider){}
        #endregion

        #region debug
        public override string McsSettingsDebugField
        {
            get { return "udo_interaction"; }
        }
        #endregion

        #region Internal Methods/Properties
        
        internal void Execute()
        {
            try
            {
                if (PluginExecutionContext.Depth > 2) return;

                Stopwatch txnTimer = Stopwatch.StartNew();

                #region logging and parameters
                var messageType = PluginExecutionContext.MessageName;
                EntityReference veteranRef = new EntityReference();
                EntityReference assignee = null;
                Entity updateTarget = null;
                Entity image = new Entity();
                if (messageType == "Update")
                {
                    updateTarget = (Entity)PluginExecutionContext.InputParameters["Target"];
                    if (!updateTarget.Contains("ownerid")) return;
                    if(PluginExecutionContext.InputParameters.Contains("Assignee"))
                    {
                        assignee = (EntityReference)PluginExecutionContext.InputParameters["Assignee"];
                    }
                    image = PluginExecutionContext.PreEntityImages["PreAssignImage"];
                }
                else if (messageType == "Assign")
                {
                    assignee = (EntityReference)PluginExecutionContext.InputParameters["Assignee"];
                    image = PluginExecutionContext.PreEntityImages["PreAssignImage"];
                }

                OptionSetValue sensitivityLevel = new OptionSetValue();
                #endregion

                using (var xrm = new UDOContext(OrganizationService))
                #region check for queueitem
                {
                    var QueueItems = from qItems in xrm.QueueItemSet
                                     where qItems.ObjectId.Id == image.GetAttributeValue<Guid>("udo_interactionid")
                                     select new
                                     {
                                         queueitemid = qItems.Id,
                                         udo_sensitivitylevel = qItems.udo_sensitivitylevel //va_veteransensitivitylevel
                                     };
                    bool gotQueueItems = false;
                    
                    foreach (var item in QueueItems)
                    {
                        gotQueueItems = true;
                        sensitivityLevel = item.udo_sensitivitylevel;
                    }
                    if (gotQueueItems)
                    {
                        assignBySL(sensitivityLevel, assignee, updateTarget);
                    }
                #endregion

                #region idproof on interaction, retrieve Veteran record owner as PCRTeam
                    else if (image.GetAttributeValue<Guid>("udo_interactionid") != null)
                    {
                        var proof = from idproof in xrm.udo_idproofSet
                                    where idproof.udo_Interaction.Id == image.GetAttributeValue<Guid>("udo_interactionid")
                                    select new
                                    {
                                        udo_Veteran = idproof.udo_Veteran,
                                    };
                        if (proof.FirstOrDefault() != null)
                        {
                            veteranRef = proof.FirstOrDefault().udo_Veteran;
                            var vet = from vetset in xrm.ContactSet
                                      where vetset.ContactId == veteranRef.Id
                                      select new
                                      {
                                          OwnerId = vetset.OwnerId
                                      };
                            if (assignee != null)
                            {
                                assignee = vet.FirstOrDefault().OwnerId;
                            }
                            else if (updateTarget != null)
                            {
                                updateTarget["ownerid"] = vet.FirstOrDefault().OwnerId;
                            }
                            else TracingService.Trace("No update target identified."); //Logger.WriteDebugMessage("No update target identified.");
                        }
                #endregion

                        #region no idproof
                        else
                        {
                            assignBySL(new OptionSetValue(0), assignee, updateTarget);
                        }
                        #endregion
                    }
                }
                txnTimer.Stop();
                Logger.setMethod = "InteractionPreAssignRunner";
                //Logger.WriteTxnTimingMessage(String.Format("Timing for : {0}", GetType()), txnTimer.ElapsedMilliseconds); 
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

        private void assignBySL(OptionSetValue sensitivityLevel, EntityReference assignee, Entity updateTarget)
        {
            using (var xrm = new UDOContext(OrganizationService))
            {
                var webresources = from wresources in xrm.WebResourceSet
                                   where wresources.Name == "udo_SensitivityLevelOptionsetConversion"
                                   select new
                                   {
                                       Id = wresources.Id,
                                       Content = wresources.Content
                                   };
                bool gotResources = false;
                foreach (var resource in webresources)
                {
                    gotResources = true;
                }
                if (gotResources)
                {
                    byte[] byteContent = Convert.FromBase64String(webresources.FirstOrDefault().Content);
                    string docContent = System.Text.Encoding.UTF8.GetString(byteContent);
                    string byteOrderMark = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.UTF8.GetPreamble());
                    if (docContent.StartsWith(byteOrderMark))
                    {
                        docContent = docContent.Remove(0, byteOrderMark.Length);

                    }
                    XDocument xmlDocument = XDocument.Parse(docContent);
                    if (sensitivityLevel != null)
                    {

                        var translatedValue = from translate in xmlDocument.Descendants("item")
                                              where translate.Attribute("id").Value.Equals(sensitivityLevel.Value.ToString())
                                              select new
                                              {
                                                  udo_veteransensitivitylevel = translate.Descendants("udo_veteransensitivitylevel").Attributes("value")
                                              };

                        int newValue = int.Parse(translatedValue.FirstOrDefault().udo_veteransensitivitylevel.FirstOrDefault().Value);
                        Entity bizUnit = GetBusinessUnitBySensitivityLevel(newValue);
                        Entity newOwner = GetDefaultTeamForBusinessUnit(bizUnit.Id);
                        if (assignee != null)
                        {
                            assignee = newOwner.ToEntityReference();
                        }
                        else if (updateTarget != null)
                        {
                            updateTarget["ownerid"] = newOwner.ToEntityReference();
                        }
                        else TracingService.Trace("No update target identified."); // Logger.WriteDebugMessage("No update target identified.");
                    }
                }
                else throw new InvalidPluginExecutionException("Unable to locate XML Reference document");
            }
        }

        private Entity GetBusinessUnitBySensitivityLevel(int level)
        {

            QueryExpression expression = new QueryExpression()
            {
                EntityName = BusinessUnit.EntityLogicalName,
                Criteria =
                {
                    Filters = 
                    {
                        new FilterExpression()
                        {
                            Conditions = 
                            { 
                               
                                new ConditionExpression("udo_veteransensitivitylevel", ConditionOperator.Equal, level)
                            }
                        }
                    }
                }
            };

            var result = base.OrganizationService.RetrieveMultiple(expression);
            if (result.Entities.Count() == 0)
            {
                throw new InvalidPluginExecutionException("Business Unit Not Found");
            }

            return result.Entities[0];
        }

        private Entity GetDefaultTeamForBusinessUnit(Guid buID)
        {
            QueryExpression expression = new QueryExpression()
            {
                EntityName = Team.EntityLogicalName,
                Criteria =
                {
                    Filters = 
                    {
                        
                        new FilterExpression()
                        {
                          Conditions = 
                            { 
                                
                                new ConditionExpression("name", ConditionOperator.Equal, _TeamName),
                                new ConditionExpression("businessunitid", ConditionOperator.Equal, buID)
                            }
                        }
                    }
                }
            };

            var result = base.OrganizationService.RetrieveMultiple(expression);

            if (result.Entities.Count() > 0)
                return result.Entities[0];
            else
                return null;
        }

        public override Entity GetPrimaryEntity()
        {
            return (Entity)PluginExecutionContext.InputParameters["Target"];
        }

        public override Entity GetSecondaryEntity()
        {
            return (Entity)PluginExecutionContext.InputParameters["Target"];
        }
        #endregion
    }
}

