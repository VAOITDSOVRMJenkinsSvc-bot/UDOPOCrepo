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

namespace Va.Udo.Crm.Queue.Plugins
{
    //Purpose:  This plugin executes on SetState, SetStateDynamicEntity, and Update of Interaction to accordingly handle queue history updates for hold state and start of work.

    public class InteractionPrePostRunner : MCSPlugins.PluginRunner
    {
        #region Enums

        public enum ItemDisposition
        {
            RoutedToQueue= 0,
            PickedFromQueue= 1,
            ReturnedToQueue= 2,
            RemovedFromQueue= 3,
            OnHold = 4
        }

        public enum InteractionStatus
        {
            OnHold = 752280000,
            Active = 1,
            Complete = 752280001,
            WalkOut = 752280002
        }

        #endregion

        #region Constructor
        public InteractionPrePostRunner(IServiceProvider serviceProvider)
            : base(serviceProvider){}
        #endregion

        #region debug
        public override string McsSettingsDebugField
        {
            get { return "udo_queueitem"; }
        }
        #endregion

        #region Internal Methods/Properties

        private EntityCollection GetQueueItems(EntityReference Interaction)
        {
            //Logger.WriteDebugMessage("Getting QueueItems.");
            TracingService.Trace("Getting QueueItems.");
            if (Interaction != null)
            {
                if (Interaction.Id != null)
                {
                    var queueItemFetch = "<fetch><entity name='queueitem'>" +
                           "<attribute name='queueitemid'/>" +
                           "<filter type='and'>" +
                           "<condition attribute='objectid' operator='eq' value='" + Interaction.Id + "'/>" +
                           "</filter></entity></fetch>";

                    return OrganizationService.RetrieveMultiple(new FetchExpression(queueItemFetch));
                }
            }
            return null;
        }
        
        private void CreateHistory(Entity queueItem, EntityReference targetRef, DateTime start, EntityReference destination, ItemDisposition? disposition)
        {
            #region check target ref
            //Logger.WriteDebugMessage("Creating new History");
            TracingService.Trace("Creating new History");
            if (targetRef == null)
            {
                // No target object, can't create history
                //Logger.WriteDebugMessage("No Target Object Identified, unable to write History.");
                TracingService.Trace("No Target Object Identified, unable to write History.");
                return;
            }
            #endregion

            #region check, lock, and update history
            // check for existing history prior to creating a new one.
            var history = new Entity("udo_queuehistory");
            history["udo_lockcode"] = "NEWHISTORY";
            history["udo_title"] = queueItem.GetAttributeValue<string>("title");
            history["udo_queueitemid"] = queueItem.ToEntityReference();
            if (targetRef.LogicalName == "udo_interaction")
            {
                history["udo_interactionid"] = targetRef;
            }
            else
            {
                EntityReference target = null;
                if (queueItem.GetAttributeValue<EntityReference>("objectid").LogicalName == "udo_interaction")
                {
                    target = queueItem.GetAttributeValue<EntityReference>("objectid");
                }
                if (target != null)
                {
                    history["udo_interactionid"] = target;
                }
            }
            history["udo_channel"] = queueItem.GetAttributeValue<OptionSetValue>("udo_channel");
            history["udo_sensitivitylevel"] = queueItem.GetAttributeValue<OptionSetValue>("udo_sensitivitylevel") ?? new OptionSetValue(Convert.ToInt32(0));
            
            history.Id = OrganizationService.Create(history);

            var lineItem = CreateHistoryItem(queueItem, new EntityReference("udo_queuehistory", history.Id), destination, disposition);
            
            // Link and unlock queue history
            var updateHistory = new Entity("udo_queuehistory");
            updateHistory.Id = history.Id;
            updateHistory["udo_lastlineitemid"] = lineItem;
            updateHistory["udo_lockcode"] = string.Empty;
            OrganizationService.Update(updateHistory);

            #endregion
        }

        private EntityReference CreateHistoryItem(Entity queueItem, EntityReference queueHistory, EntityReference destination, ItemDisposition? disposition)
        {
            #region build Line Item
            
            var lineItem = new Entity("udo_queuehistorylineitem");
            lineItem["udo_queuehistoryid"] = new EntityReference("udo_queuehistory", queueHistory.Id);
            lineItem["udo_index"] = 1;
            lineItem["udo_queueid"] = queueItem.GetAttributeValue<EntityReference>("queueid");
            var history = OrganizationService.Retrieve("udo_queuehistory", queueHistory.Id, new ColumnSet(new[] { "udo_title" }));
            lineItem["udo_name"] = history.GetAttributeValue<string>("udo_title");

            if (disposition.HasValue)
            {
                lineItem["udo_disposition"] = new OptionSetValue(Convert.ToInt32(disposition.Value)); // In Queue 
            }

            var name = string.Empty;
            if (lineItem["udo_queueid"] != null)
            {
                var queueRef = lineItem.GetAttributeValue<EntityReference>("udo_queueid");
                if (!String.IsNullOrEmpty(queueRef.Name))
                {
                    name = queueRef.Name;
                }
                else
                {
                    var queue = OrganizationService.Retrieve("queue", queueRef.Id, new ColumnSet(new[] { "name" }));
                    name = queue.GetAttributeValue<string>("name");
                }
            }
            lineItem["udo_responsibleparty"] = name;
            
            var destinationName = string.Empty;
            if (destination != null)
            {
                if (destination.LogicalName == "queue")
                {
                    lineItem["udo_queueid"] = destination;
                    if (!String.IsNullOrEmpty(destination.Name))
                    {
                        destinationName = destination.Name;
                    }
                    else
                    {
                        var queue = OrganizationService.Retrieve("queue", destination.Id, new ColumnSet(new[] { "name" }));
                        destinationName = queue.GetAttributeValue<string>("name");
                    }
                }

                if (destination.LogicalName == "systemuser")
                {
                    lineItem["udo_workerid"] = destination;
                    if (!String.IsNullOrEmpty(destination.Name))
                    {
                        destinationName = destination.Name;
                    }
                    else
                    {
                        var worker = OrganizationService.Retrieve("systemuser", destination.Id, new ColumnSet(new[] { "fullname" }));
                        destinationName = worker.GetAttributeValue<string>("fullname");
                    }
                }
                lineItem["udo_responsibleparty"] = destinationName;
            }
            lineItem.Id = OrganizationService.Create(lineItem);
            return new EntityReference("udo_queuehistorylineitem", lineItem.Id);
            #endregion
        }
        
        internal void Execute()
        {
            try
            {
                
                Stopwatch txnTimer = Stopwatch.StartNew();

                #region set parameters, start logging, branch for message type
                var now = DateTime.UtcNow;
                ItemDisposition? disposition = ItemDisposition.OnHold;
                OptionSetValue status = new OptionSetValue();
                Entity image = PluginExecutionContext.PreEntityImages["interactionImage"];
                Entity queueItem = GetQueueItems(new EntityReference("Udo_interaction", image.Id)).Entities.FirstOrDefault();
                if (queueItem == null) return;

                if (PluginExecutionContext.MessageName == "Update")
                {
                    Entity target = PluginExecutionContext.PostEntityImages["postImage"];
                    status = target.GetAttributeValue<OptionSetValue>("statuscode");
                    disposition = ItemDisposition.PickedFromQueue;
                    if (image.GetAttributeValue<OptionSetValue>("statuscode").Value != new OptionSetValue(Convert.ToInt32(InteractionStatus.OnHold)).Value)
                    {
                        return;
                    }
                    else if (target.GetAttributeValue<OptionSetValue>("statuscode").Value != new OptionSetValue(Convert.ToInt32(InteractionStatus.Active)).Value)
                    {
                        return;
                    }
                }
                else
                {
                    status = (OptionSetValue)PluginExecutionContext.InputParameters["Status"];
                    bool invalid = true;
                    //Release Hold on Interaction
                    if (image.GetAttributeValue<OptionSetValue>("statuscode") == new OptionSetValue(Convert.ToInt32(InteractionStatus.OnHold)) &&
                        status == new OptionSetValue(Convert.ToInt32(InteractionStatus.Active)))
                    {
                        disposition = ItemDisposition.PickedFromQueue;
                        invalid = false;
                    }
                    //Close Interaction
                    if ((image.GetAttributeValue<OptionSetValue>("statuscode") == new OptionSetValue(Convert.ToInt32(InteractionStatus.Active)) ||
                        image.GetAttributeValue<OptionSetValue>("statuscode") == new OptionSetValue(Convert.ToInt32(InteractionStatus.OnHold))) &&
                        (status == new OptionSetValue(Convert.ToInt32(InteractionStatus.Complete)) ||
                        status == new OptionSetValue(Convert.ToInt32(InteractionStatus.WalkOut))))
                    {
                        disposition = ItemDisposition.RemovedFromQueue;
                        invalid = false;
                    }

                    if (invalid == true) return;
                    #endregion
                }
                #endregion

                #region removefromqueue if interaction is closing
                EntityReference destination = new EntityReference();
                if (disposition == ItemDisposition.PickedFromQueue) destination = new EntityReference("systemuser", PluginExecutionContext.UserId);

                if (disposition == ItemDisposition.RemovedFromQueue)
                {
                    //Logger.WriteDebugMessage("Interaction Closing. Removing Queue Item from Queue.");
                    TracingService.Trace("Interaction Closing. Removing Queue Item from Queue.");
                    var removeQueueItemRequest = new OrganizationRequest();

                    removeQueueItemRequest.RequestName = "RemoveFromQueue";
                    removeQueueItemRequest.Parameters.Add("QueueItemId", queueItem.Id);

                    OrganizationService.Execute(removeQueueItemRequest);

                    return;
                }
                #endregion

                #region Get History Reference
                
                var historyFetch = "<fetch><entity name='udo_queuehistory'>" +
                           "<attribute name='udo_queuehistoryid'/>" +
                           "<filter type='and'>" +
                           "<condition attribute='udo_queueitemid' operator='eq' value='" + queueItem.Id + "'/>" +
                           "</filter>" +
                           "<order attribute='createdon' descending = 'true' />" +
                           "</entity></fetch>";

                var histories = OrganizationService.RetrieveMultiple(new FetchExpression(historyFetch));
                EntityReference historyRef = new EntityReference("udo_queuehistory", histories.Entities.FirstOrDefault().Id);

                if (historyRef == null)
                {//should not happen
                    CreateHistory(queueItem, new EntityReference("udo_interaction", image.Id), now, destination, disposition);
                }
                else 
                {
                    //Logger.WriteDebugMessage("{0}", historyRef.Id);
                }

                #endregion
                
                #region Lock the History Record and Get the LastLineItem reference (if one exists)
                // Wait until History can be locked and get last line item
                var locker = new EntityLock(OrganizationService, historyRef, "udo_lockcode", new[]{"udo_lastlineitemid"});
                
                EntityReference lastLineItemRef = null;
                try {
                    var lockcode = locker.WaitAndLock();
                    if (locker.Record!=null) {
                        lastLineItemRef = locker.Record.GetAttributeValue<EntityReference>("udo_lastlineitemid");
                    }
                } catch (Exception ex) {
                    //Logger.WriteDebugMessage("Error in File Lock");
                    //Logger.WriteException(ex);
                    TracingService.Trace("Error in File Lock" + ex.Message + ex.StackTrace);
                    throw ex;
                }

                if (lastLineItemRef == null)
                { // Should not happen

                    var lineItemFetch = "<fetch><entity name='udo_queuehistorylineitem'>" +
                           "<attribute name='udo_queuehistorylineitemid'/>" +
                           "<attribute name='createdon'/>" +
                           "<filter type='and'>" +
                           "<condition attribute='udo_queuehistoryid' operator='eq' value='" + historyRef.Id + "'/>" +
                           "</filter>" +
                           "<order attribute='createdon' descending = 'true' />" +
                           "</entity></fetch>";

                    var historyItems = OrganizationService.RetrieveMultiple(new FetchExpression(lineItemFetch));
                    if (historyItems == null || historyItems.Entities.Count == 0)
                    {
                        //Logger.WriteDebugMessage("No History Line Items identified for this History Ref. Skipping to Create.");
                        TracingService.Trace("No History Line Items identified for this History Ref. Skipping to Create.");
                    }
                    else lastLineItemRef = new EntityReference("udo_queuehistorylineitem", historyItems.Entities.FirstOrDefault().Id);
                }

                Entity lastLineItem = null;
                if (lastLineItemRef != null)
                {
                    lastLineItem = OrganizationService.Retrieve("udo_queuehistorylineitem", lastLineItemRef.Id,
                        new ColumnSet(new[] { "createdon", "udo_disposition" }));
                }
                #endregion 

                #region Update LastLineItem and History
                if (lastLineItem != null)
                {
                    lastLineItem["udo_endedon"] = now;
                    var start = lastLineItem.GetAttributeValue<DateTime>("createdon");

                    var duration = (now - start);

                    lastLineItem["udo_duration_seconds"] = duration.TotalSeconds;
                    lastLineItem["udo_duration_minutes"] = duration.TotalMinutes;
                    // Update previous record
                    OrganizationService.Update(lastLineItem);

                    Entity History = OrganizationService.Retrieve("udo_queuehistory", historyRef.Id, new ColumnSet(new[] { "udo_queuehistoryid", "udo_duration_waittime", 
                        "udo_duration_waittime_seconds", "udo_duration_worktime", "udo_duration_worktime_seconds", "udo_duration_totaltime", "udo_duration_totaltime_seconds", "udo_lockcode" }));

                    if (lastLineItem.GetAttributeValue<OptionSetValue>("udo_disposition").Value == new OptionSetValue(Convert.ToInt32(ItemDisposition.RoutedToQueue)).Value ||
                        lastLineItem.GetAttributeValue<OptionSetValue>("udo_disposition").Value == new OptionSetValue(Convert.ToInt32(ItemDisposition.ReturnedToQueue)).Value)
                    {
                        History["udo_duration_waittime"] = History.GetAttributeValue<int>("udo_duration_waittime") + duration.TotalMinutes;
                        History["udo_duration_waittime_seconds"] = History.GetAttributeValue<int>("udo_duration_waittime_seconds") + duration.TotalSeconds;
                    }
                    else if (lastLineItem.GetAttributeValue<OptionSetValue>("udo_disposition").Value == new OptionSetValue(Convert.ToInt32(ItemDisposition.PickedFromQueue)).Value)
                    {
                        History["udo_duration_worktime"] = History.GetAttributeValue<int>("udo_duration_worktime") + duration.TotalMinutes;
                        History["udo_duration_worktime_seconds"] = History.GetAttributeValue<int>("udo_duration_worktime_seconds") + duration.TotalSeconds;
                    }
                    History["udo_duration_totaltime"] = History.GetAttributeValue<int>("udo_duration_totaltime") + duration.TotalMinutes;
                    History["udo_duration_totaltime_seconds"] = History.GetAttributeValue<int>("udo_duration_totaltime_seconds");
                    History["udo_lockcode"] = string.Empty;
                    // Update History totals
                    OrganizationService.Update(History);
                }
                #endregion 

                #region Create New Line Item and update History
                //Only Create a new line if there is a Destination value (not RemoveFromQueue Action)

                try
                {
                    var lockcode = locker.WaitAndLock();

                    EntityReference newLineItem = CreateHistoryItem(queueItem, historyRef, destination, disposition);

                    var history = new Entity(historyRef.LogicalName);
                    history.Id = historyRef.Id;
                    history["udo_lastlineitemid"] = newLineItem;
                    history["udo_lockcode"] = string.Empty;
                    OrganizationService.Update(history);
                }
                catch (Exception ex)
                {
                    //Logger.WriteDebugMessage("Error in file lock while creating new line item. Last Line Item ID not updated.");
                    //Logger.WriteException(ex);
                    TracingService.Trace("Error in file lock while creating new line item. Last Line Item ID not updated.");
                    TracingService.Trace(ex.Message + ex.StackTrace);
                    throw new InvalidPluginExecutionException("Unable to create a new history line item. Please try again.");
                }

                //Logger.WriteDebugMessage("Ending");
                TracingService.Trace("Ending");

                txnTimer.Stop();
                Logger.setMethod = "InteractionPrePostRunner";
                //Logger.WriteTxnTimingMessage(String.Format("Timing for : {0}", GetType()), txnTimer.ElapsedMilliseconds);  
                //End the timing for the plugin
                //Logger.WriteTxnTimingMessage(String.Format("Ending : {0}", GetType()),);
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

        //For Create plugins there is no PRE image.
        //for anything else PRE images can exist on a pre-stage plugin but POST image does not.
        // For any other POST stage plugins PRE and POST exist.

        //since target only contains what was changed - the purpose of the helper is to be able to get a value of a field that may or may not have been updated.
        //For pre-stage plugins this can be difficult and require lots of coding as you want to use the target if the user updated the field and the property exists in the target - but if it
        //doesn't exist then the pre-image would hold the value.  That can be 4 lines of code, or more, for each attribute.

        //however for a post stage plugin if you want the post value then make the thisEntity and preEntity both contain the post image.
        //though not common it is possible for a post image to be different than the target.  
        //One scenario is phone number where maybe the phone number in target is 123-4567890 and the post it is 
        //(123) 456-7890.  This happens if a plugin changes the value prior to it getting to the db.

        public override Entity GetPrimaryEntity()
        {
            return (Entity)PluginExecutionContext.InputParameters["Target"];
        }

        public override Entity GetSecondaryEntity()
        {
            return (Entity)PluginExecutionContext.InputParameters["Target"];
        }
    }
}

