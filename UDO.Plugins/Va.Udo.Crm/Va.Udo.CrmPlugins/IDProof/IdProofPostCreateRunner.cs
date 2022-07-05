using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using System.Diagnostics;

namespace Va.Udo.Crm.Plugins.IDProof
{
    internal class IdProofPostCreateRunner : PluginRunner
    {
        public IdProofPostCreateRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }
        public override Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.InputParameters["Target"] as Entity;
        }
        public override Entity GetSecondaryEntity()
        {
            return PluginExecutionContext.InputParameters["Target"] as Entity;
        }
        public override string McsSettingsDebugField
        {
            get { return "udo_idproof"; }
        }

        private string progressStatus;

        internal void Execute()
        {
            try
            {
                progressStatus = "Top of the IdProofPostCreateRunner";
                //Logger.WriteDebugMessage("Start IdPrrofPostCreate");
                Trace("Start IdPrrofPostCreate");

                bool assignIDPROOF = false;
                Stopwatch txnTimer = Stopwatch.StartNew();
                var requestCollection = new OrganizationRequestCollection();

                var idproof = GetPrimaryEntity();
                progressStatus = "Retrieved IdProof Entity Record";
                
                var veteranReference = getIdProofVeteran(idproof);
                progressStatus = "Retrieved Veteran Reference from IdProof Entity Record";

                var owner = getIdProofOwner(idproof);
                progressStatus = "Retrieved Owner from IdProof Entity Record";

                var interactionReference = getInteraction(idproof);
                progressStatus = "Retrieved Interaction Entity Reference from IdProof Entity Record";

                var chatSession = getChatSession(interactionReference);
                progressStatus = "Retrieved Chat Session Reference from IdProof Entity Record";
                // Logger.WriteTxnTimingMessage("IDProof Pre Create After IDProofGets", txnTimer.ElapsedMilliseconds);

                if (owner.LogicalName != "team")
                {
                    owner = getOwner(veteranReference.Id);
                    assignIDPROOF = true;
                    progressStatus = "Owner not a Team, then retrieve Owner of the Veteran Reference";
                }
                //  Logger.WriteTxnTimingMessage("IDProof Pre After Owner", txnTimer.ElapsedMilliseconds);

                if (veteranReference != null)
                {
                    var newMilHist= new Entity { LogicalName = "udo_militaryservice" };
                    newMilHist["udo_veteranid"] = veteranReference;
                    newMilHist["udo_idproofid"] = idproof.ToEntityReference();
                    newMilHist["ownerid"] = owner;
                    newMilHist["udo_name"] = "Military History Summary";
                    OrganizationService.Create(newMilHist);
                    progressStatus = "Created the Miliary Service record";
                    
                    //CreateRequest createMilService = new CreateRequest
                    //{
                    //    Target = newMilHist,
                    //};
                    //requestCollection.Add(createMilService); 

                    var newExamAndAppointment = new Entity { LogicalName = "udo_examandappontment" };
                    newExamAndAppointment["udo_veteranid"] = veteranReference;
                    newExamAndAppointment["udo_idproofid"] = idproof.ToEntityReference();
                    newExamAndAppointment["ownerid"] = owner;
                    newExamAndAppointment["udo_name"] = "Exam and Appointment Summary";
                    OrganizationService.Create(newExamAndAppointment);
                    progressStatus = "Created the Exam and Appointment record";

                    //CreateRequest createExam = new CreateRequest
                    //{
                    //    Target = newExamAndAppointment,
                    //};
                    //requestCollection.Add(createExam);

                   
                    //CreateRequest createSnap = new CreateRequest
                    //{
                    //    Target = newSnapShot,
                    //};
                    //requestCollection.Add(createSnap);

                    var newRating = new Entity { LogicalName = "udo_rating" };
                    newRating["ownerid"] = owner;
                    newRating["udo_veteranid"] = veteranReference;
                    newRating["udo_name"] = "Ratings Summary";
                    newRating["udo_idproofid"] = idproof.ToEntityReference();
                    var ratingsGuid = OrganizationService.Create(newRating);
                    progressStatus = "Created the Rating record";
                    //CreateRequest createRating = new CreateRequest
                    //{
                    //    Target = newRating,
                    //};
                    //requestCollection.Add(createRating);
                    var newBIRLS = new Entity { LogicalName = "udo_birls" };
                    newBIRLS["ownerid"] = owner;
                    newBIRLS["udo_veteranid"] = veteranReference;
                    newBIRLS["udo_name"] = "BIRLS Summary";
                    newBIRLS["udo_idproofid"] = idproof.ToEntityReference();
                    var birlsGuid = OrganizationService.Create(newBIRLS);
                    progressStatus = "Created the Birls record";
                    
                    //snapshot creation moved to search
                    //var newSnapShot = new Entity { LogicalName = "udo_veteransnapshot" };
                    //newSnapShot["udo_veteranid"] = veteran;
                    //newSnapShot["udo_name"] = "Veteran Summary";
                    //newSnapShot["udo_idproofid"] = thisReference;
                    //newSnapShot["ownerid"] = owner;
                    //newSnapShot["udo_birlsguid"] = birlsGuid.ToString();
                    //newSnapShot["udo_ratingsguid"] = ratingsGuid.ToString();
                    //OrganizationService.Create(newSnapShot);

                    //Update the Chat Session lookup field to be the veteran on the ID Proof
                    if (chatSession != null) 
                    {
                        var chatSession_vet = chatSession.GetAttributeValue<EntityReference>("udo_veteranid");
                        if (chatSession_vet == null)
                        {
                            var updatedChat = new Entity("crme_chatcobrowsesessionlog");
                            
                            updatedChat.Id = chatSession.Id;

                            updatedChat["udo_veteranid"] = veteranReference;
                            updatedChat["udo_idproof"] = idproof.ToEntityReference();
                            //updatedChat["entitystate"] = EntityState.Changed;
                            //chatSession.OwnerId = owner;
                            OrganizationService.Update(updatedChat);
                            progressStatus = "Created the Chat Cobrowse Session record";
                        }
                        ////The AssignRequest is to change the owner to match that of the ID Proof
                        ////As of CRM 2016, the Owner property can be changed directly as part of the update
                        ////until then, it must be invoked separately
                        //AssignRequest req = new AssignRequest()
                        //{
                        //    Assignee = owner,
                        //    Target = new EntityReference(chatSession.LogicalName, chatSession.Id)
                        //};
                        //OrganizationService.Execute(req);
                    }

                    var shapshotId = getSnapshot(idproof.Id);
                    if (shapshotId != Guid.Empty)
                    {
                        Entity snapShot = new Entity("udo_veteransnapshot");
                        snapShot.Id = shapshotId;
                        snapShot["udo_birlsguid"] = birlsGuid.ToString();
                        snapShot["udo_ratingsguid"] = ratingsGuid.ToString();
                        OrganizationService.Update(snapShot);
                        progressStatus = "Created the Veteran Snapshot record";
                    }

                    if (assignIDPROOF)
                    {
                        var assignRequest = new AssignRequest
                        {
                            Target = idproof.ToEntityReference(),
                            Assignee = owner
                        };

                        OrganizationService.Execute(assignRequest);
                        progressStatus = "Assigned owner to the ID Proof record";
                    }
    
                }
                txnTimer.Stop();
                Logger.setMethod = "IDProof Pre Create Creations";
                //Logger.WriteTxnTimingMessage("IDProof Post Create Creation", txnTimer.ElapsedMilliseconds);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                PluginError = true;
                Logger.WriteToFile("Proogress Status: " + progressStatus + " - " + ex.Message + ex.StackTrace.ToString());
                Trace("Progress Status: " + progressStatus + " - " + ex.Message + ex.StackTrace.ToString());
                //cannot throw message as it messes up ID Proof stuff.
            }
            catch (Exception ex)
            {
                PluginError = true;
                Logger.WriteToFile("Proogress Status: " + progressStatus + " - " + ex.Message + ex.StackTrace.ToString());
                Trace("Progress Status: " + progressStatus + " - " + ex.Message + ex.StackTrace.ToString());
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

        internal Guid getSnapshot(Guid idproofid)
        {
            var qe = new QueryExpression("udo_veteransnapshot")
            {
                NoLock = true,
                TopCount = 1,
                ColumnSet = new ColumnSet("udo_idproofid"),
                Criteria =
                {
                    Conditions = {
                        new ConditionExpression("udo_idproofid", ConditionOperator.Equal, idproofid)
                    }
                }
            };

            var response = OrganizationService.RetrieveMultiple(qe);

            if (response == null || response.Entities.Count == 0) return Guid.Empty; //?

            return response.Entities[0].Id;
        }
        
        internal EntityReference getOwner(Guid veteranId)
        {
            try
            {
                Logger.setMethod = "getOwner";
                var veteran = OrganizationService.Retrieve("contact", veteranId, new ColumnSet(new[] { "ownerid" }));
                EntityReference thisOwner = (EntityReference) veteran["ownerid"];
                Logger.setMethod = "execute";

                return thisOwner;
            }
            catch (Exception ex)
            {
                PluginError = true;
                throw new InvalidPluginExecutionException("Unable to getOwner2 due to: {0}".Replace("{0}", ex.Message));
            }
        }
        
        internal EntityReference getIdProofVeteran(Entity target)

        {
            if (target != null)
            {
                if (target.Contains("udo_veteran"))
                {
                    return (EntityReference)target["udo_veteran"];
                }
            }
            return null;
        }

        internal EntityReference getIdProofOwner(Entity target)
        {
            if (target != null)
            {
               
                    return (EntityReference)target["ownerid"];
            }
            return null;
        }

        internal EntityReference getInteraction(Entity target)
        {
            if (target != null)
            {
                return (EntityReference)target["udo_interaction"];
            }
            return null;
        }

        internal Entity getChatSession(EntityReference interaction)
        {
            QueryExpression qe = new QueryExpression("crme_chatcobrowsesessionlog")
            {
                NoLock = true,
                TopCount = 1,
                ColumnSet = new ColumnSet(
                    "udo_interactionid", "udo_idproof", "udo_veteranid", "ownerid"
                    ),
                Criteria =
                {
                    Conditions =
                    { new ConditionExpression("udo_interactionid", ConditionOperator.Equal, interaction.Id)
                    }
                }
            };

            var results = OrganizationService.RetrieveMultiple(qe);

            if (results == null || results.Entities.Count == 0) return null;

            return results.Entities[0];
                                //udo_InteractionId = chat.udo_InteractionId,
                                //udo_IdProof = chat.udo_IdProof,
                                //udo_VeteranId = chat.udo_VeteranId,
                                //Id = chat.Id,
                                //OwnerId = chat.OwnerId
        }
    }
}
