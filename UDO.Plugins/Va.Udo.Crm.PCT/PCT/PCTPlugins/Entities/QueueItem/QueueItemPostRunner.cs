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
    //Purpose:  The purpose of this plugin is to manage queue history creation on all primary queueitem messages (addtoqueue, pickfromqueue, releasetoqueue, routetoqueue, removefromqueue

    public class QueueItemPostRunner : MCSPlugins.PluginRunner
    {
        #region Enums
        
        public enum ItemDisposition
        {
            RoutedToQueue= 0,
            PickedFromQueue= 1,
            ReturnedToQueue= 2,
            RemovedFromQueue= 3,
            OnHold= 4
        }

        public enum InteractionStatus
        {
            OnHold = 752280000,
            Active = 1
        }

        #endregion

        #region debug
        public override string McsSettingsDebugField
        {
            get { return "udo_queueitem"; }
        }
        #endregion

        private Entity GetQueueItem(ParameterCollection paramCollection)
        {
            if (paramCollection.Contains("QueueItemId"))
            {
                var id = (Guid) paramCollection["QueueItemId"];
                return OrganizationService.Retrieve("queueitem", id, new ColumnSet(new[] { "queueitemid" , "queueid", "objectid"}));
            }
            return null;
        }


        #region Constructor
        public QueueItemPostRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }
        #endregion

        #region Internal Methods/Properties
        private void CreateHistory(Entity queueItem, EntityReference interactionRef, DateTime start, EntityReference destination, ItemDisposition? disposition)
        {
            #region check, lock, and update history
            // check for existing history prior to creating a new one.
            var history = new Entity("udo_queuehistory");
            history["udo_lockcode"] = "NEWHISTORY";
            history["udo_title"] = interactionRef.Name;
            history["udo_queueitemid"] = queueItem.ToEntityReference();
            history["udo_interactionid"] = interactionRef;
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
            // create Queue History Line Item
            Entity lineItem = null;
            
            lineItem = new Entity("udo_queuehistorylineitem");
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

                #region set parameters, start logging
                //start the timing for the plugin
                var now = DateTime.UtcNow;
                ItemDisposition? disposition = null;
                var input = PluginExecutionContext.InputParameters;
                var output = PluginExecutionContext.OutputParameters;
                Entity QueueItem=null;
                EntityReference Destination=null;
                EntityReference interactionRef = null;
                #endregion

                #region switch parameters on message type
                switch (PluginExecutionContext.MessageName)
                {
                    case "AddToQueue":
                        //Logger.WriteDebugMessage("Request Type: AddToQueue Request");
                        TracingService.Trace("Request Type: AddToQueue Request");
                        // New Queue Item
                        var destinationQueueId = (Guid)input["DestinationQueueId"];
                        Destination = new EntityReference("queue", destinationQueueId);
                        QueueItem = GetQueueItem(output);
                        disposition = ItemDisposition.RoutedToQueue;
                        break;
                    case "RouteTo":
                        //Logger.WriteDebugMessage("Request Type: RouteTo Request");
                        TracingService.Trace("Request Type: RouteTo Request");
                        // Route existing Queue Item
                        QueueItem = GetQueueItem(input);
                        Destination = input["Target"] as EntityReference;
                        disposition = ItemDisposition.RoutedToQueue;
                        break;
                    case "PickFromQueue":
                        //Logger.WriteDebugMessage("Request Type: PickFromQueue Request");
                        TracingService.Trace("Request Type: PickFromQueue Request");
                        // Grab item from a queue
                        QueueItem = GetQueueItem(input);
                        var pickWorkerId = (Guid)input["WorkerId"];
                        Destination = new EntityReference("systemuser", pickWorkerId);
                        interactionRef = QueueItem.GetAttributeValue<EntityReference>("objectid");
                        var interactionStatus = OrganizationService.Retrieve("udo_interaction", interactionRef.Id, new ColumnSet(new[]{"statuscode"})).GetAttributeValue<OptionSetValue>("statuscode");
                        if(interactionStatus.Value == Convert.ToInt32(InteractionStatus.OnHold))
                            disposition = ItemDisposition.OnHold;
                        else disposition = ItemDisposition.PickedFromQueue;
                        break;
                    case "ReleaseToQueue":
                        //Logger.WriteDebugMessage("Request Type: ReleaseToQueue Request");
                        TracingService.Trace("Request Type: ReleaseToQueue Request");
                        // Release to queue (after working it)
                        QueueItem = GetQueueItem(input);
                        if (QueueItem != null)
                        {
                            Destination = QueueItem.GetAttributeValue<EntityReference>("queueid");
                        }
                        disposition = ItemDisposition.ReturnedToQueue;
                        break;
                    case "RemoveFromQueue":
                        //Logger.WriteDebugMessage("Request Type: RemoveFromQueueRequest");
                        TracingService.Trace("Request Type: RemoveFromQueueRequest");
                        QueueItem = GetQueueItem(input);
                        disposition = ItemDisposition.RemovedFromQueue;
                        break;
                    default:
                        // Unsupported binding
                        var bindingErrorMessage = String.Format("Unable to execute for request type: {0}", PluginExecutionContext.MessageName);
                        //Logger.WriteException(new ArgumentException(bindingErrorMessage));
                        TracingService.Trace(bindingErrorMessage);
                        return;
                }

                if (QueueItem.GetAttributeValue<EntityReference>("objectid").LogicalName != "udo_interaction") return;
                if(interactionRef == null) interactionRef = QueueItem.GetAttributeValue<EntityReference>("objectid");
                #endregion

                if (QueueItem == null) return;

                #region Get History Reference
                var historyFetch = "<fetch><entity name='udo_queuehistory'>" +
                           "<attribute name='udo_queuehistoryid'/>" +
                           "<attribute name='createdon'/>" +
                           "<filter type='and'>" +
                           "<condition attribute='udo_queueitemid' operator='eq' value='" + QueueItem.Id + "'/>" +
                           "</filter>" +
                           "<order attribute='createdon' descending = 'true' />" +
                           "</entity></fetch>";

                var histories = OrganizationService.RetrieveMultiple(new FetchExpression(historyFetch));
                
                EntityReference historyRef = null;
                if (histories.Entities != null && histories.Entities.Count != 0)
                {
                    historyRef = new EntityReference("udo_queuehistory", histories.Entities.FirstOrDefault().Id);
                }
                if (historyRef == null)
                {
                    historyFetch = "<fetch><entity name='udo_queuehistory'>" +
                            "<attribute name='udo_queuehistoryid'/>" +
                            "<filter type='and'>" +
                            "<condition attribute='udo_interactionid' operator='eq' value='" + QueueItem.GetAttributeValue<EntityReference>("objectid").Id + "'/>" +
                            "</filter></entity></fetch>";

                    var historyresults = OrganizationService.RetrieveMultiple(new FetchExpression(historyFetch));

                    if (historyresults == null) 
                    {
                        string exceptionMessage = "Error in History Results Retrieve Multiple; please try again.";
                        //Logger.WriteException(new MissingFieldException(exceptionMessage));
                        TracingService.Trace(exceptionMessage);
                    }
                    if (historyresults.Entities.Count == 0)
                    {
                        if (PluginExecutionContext.MessageName == "AddToQueue" || PluginExecutionContext.MessageName == "RouteTo")
                        {
                            CreateHistory(QueueItem, interactionRef, now, Destination, disposition);
                        }
                        else
                        {
                            string errorMessage = "Attempting " + PluginExecutionContext.MessageName + " without a Queue History record associated. This transaction will not be logged in history.";
                            //Logger.WriteException(new InvalidOperationException(errorMessage));
                            TracingService.Trace(errorMessage);
                        }
                        return;
                    }
                    else historyRef = historyresults.Entities.Select(u => new EntityReference("udo_queuehistory", (Guid)u["udo_queuehistoryid"])).First();
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
                    throw ex;
                }
                
                Entity lastLineItem = null;
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
                else
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
                        History["udo_duration_waittime"] = History.GetAttributeValue<int>("udo_duration_waittime") + Convert.ToInt32(duration.TotalMinutes);
                        History["udo_duration_waittime_seconds"] = History.GetAttributeValue<int>("udo_duration_waittime_seconds") + Convert.ToInt32(duration.TotalSeconds);
                    }
                    else if (lastLineItem.GetAttributeValue<OptionSetValue>("udo_disposition").Value == new OptionSetValue(Convert.ToInt32(ItemDisposition.PickedFromQueue)).Value)
                    {
                        History["udo_duration_worktime"] = History.GetAttributeValue<int>("udo_duration_worktime") + Convert.ToInt32(duration.TotalMinutes);
                        History["udo_duration_worktime_seconds"] = History.GetAttributeValue<int>("udo_duration_worktime_seconds") + Convert.ToInt32(duration.TotalSeconds);
                    }
                    History["udo_duration_totaltime"] = History.GetAttributeValue<int>("udo_duration_totaltime") + Convert.ToInt32(duration.TotalMinutes);
                    History["udo_duration_totaltime_seconds"] = History.GetAttributeValue<int>("udo_duration_totaltime_seconds") + Convert.ToInt32(duration.TotalSeconds);
                    History["udo_lockcode"] = string.Empty;
                    // Update History totals
                    OrganizationService.Update(History);
                }
                #endregion 

                #region Create New Line Item and update History
                //Only Create a new line if there is a Destination value (not RemoveFromQueue Action)
                if (Destination == null)
                {
                    return;
                }

                try
                {
                    var lockcode = locker.WaitAndLock();

                    EntityReference newLineItem = CreateHistoryItem(QueueItem, historyRef, Destination, disposition);

                    var history = new Entity(historyRef.LogicalName);
                    history.Id = historyRef.Id;
                    history["udo_lastlineitemid"] = newLineItem;
                    history["udo_lockcode"] = string.Empty;
                    OrganizationService.Update(history);
                }
                catch (Exception ex)
                {
                    //Logger.WriteException(ex);
                    TracingService.Trace(ex.Message + ex.StackTrace);
                    throw new InvalidPluginExecutionException("Error in record lock; unable to create a new history line item. Please try again.");
                }

                txnTimer.Stop();
                Logger.setMethod = "QueueItemPostRunner";
                //Logger.WriteTxnTimingMessage(String.Format("Timing for : {0}", GetType()), txnTimer.ElapsedMilliseconds);


                //End the timing for the plugin
                //Logger.WriteTxnTimingMessage(String.Format("Ending : {0}", GetType()));
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
        #endregion
    }
}

