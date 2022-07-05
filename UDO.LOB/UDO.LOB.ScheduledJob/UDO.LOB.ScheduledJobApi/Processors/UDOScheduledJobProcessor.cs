using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.ScheduledJob.Messages;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.Xrm.Tooling.Connector;

namespace UDO.LOB.ScheduledJob.Processors
{
	class UDOScheduledJobProcessor 
	{
        public string progressString = "Top of Processor";

        private CrmServiceClient OrgServiceProxy;
        private OrganizationWebProxyClient webProxyClient;

        public IMessageBase Execute(UDOScheduledJobRequest request)
        {
            LogHelper.LogInfo("Entered ScheduledJob LOB");
            #region pass to async method and return
            ContinueProcessor(request);
            UDOScheduledJobResponse response = new UDOScheduledJobResponse()
            {
                MessageId = request.MessageId,
                Received = true
            };

            return response;
        }
        #endregion
        private void ContinueProcessor(UDOScheduledJobRequest request)
        {
            LogHelper.LogInfo(string.Format("Message Id:{0}, Type={1}", request.MessageId, GetType().FullName));

            #region connect to CRM
            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
                LogHelper.LogInfo("Connected to CRM");
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, $"Exception during CRM Connect: {connectException.Message}", connectException);
                LogHelper.LogError(request.OrganizationName, request.UserID, "udo_scheduledjob", "UDOScheduledJob Processor, Progress:" + progressString, connectException);
			}
			#endregion

			progressString = "After Connection";

			try
			{
                #region retrieve queues
                var QueueFetch = "<fetch>" +
                                  "<entity name='queue' >" +
                                    "<all-attributes/>" +
                                    "<filter>" +
                                      "<condition attribute='udo_markfordailycleanup' operator='eq' value='1' />" +
                                    "</filter>" +
                                  "</entity>" +
                                "</fetch>";

                EntityCollection queues = OrgServiceProxy.RetrieveMultiple(new FetchExpression(QueueFetch));
                var requestCollection = new OrganizationRequestCollection();
                progressString = "After queue fetch";
                if (queues.Entities.Count > 0)
                {
                    #region Retrieve queue items
                    foreach (var Q in queues.Entities)
                    {
                        var QIFetch = "<fetch>" +
                                      "<entity name='queueitem' >" +
                                        "<all-attributes/>" +
                                        "<filter>" +
                                          "<condition attribute='queueid' operator='eq' value='" + Q.Id + "' />" +
                                          "<condition attribute='statecode' operator='eq' value='0' />" +
                                        "</filter>" +
                                      "</entity>" +
                                    "</fetch>";

                        EntityCollection queueItems = OrgServiceProxy.RetrieveMultiple(new FetchExpression(QIFetch));
                        LogHelper.LogInfo(string.Format("{0} queue items found in queue {1}", queueItems.Entities.Count, Q.Id));
                        if (queueItems.Entities.Count > 0)
                        {
                            foreach (var QI in queueItems.Entities)
                            {
                                try
                                {
                                    if(QI.GetAttributeValue<EntityReference>("objectid").LogicalName == "udo_interaction")
                                    {
                                        //REM: Changed from SetSate to Update
                                        //var deactivateRequest = new SetStateRequest()
                                        //{
                                        //    EntityMoniker = QI.GetAttributeValue<EntityReference>("objectid"),
                                        //    State = new OptionSetValue(1),
                                        //    Status = new OptionSetValue(752280002)
                                        //};

                                        QI.Attributes["statecode"] = new OptionSetValue(1);
                                        QI.Attributes["statuscode"] = new OptionSetValue(752280002);

                                        LogHelper.LogInfo(string.Format("Deactivating Interaction {0}", QI.GetAttributeValue<EntityReference>("udo_originatinginteractionid")));

                                        OrgServiceProxy.Update(QI);
                                        //requestCollection.Add(deactivateRequest);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogHelper.LogError(request.OrganizationName, request.UserID, "UDOScheduledJobProcessor", ex);
                                }
                            }
                        }
                        else
                        {
                            LogHelper.LogDebug(request.OrganizationName, true, request.UserID, "UDOScheduledJobProcessor", string.Format("No queue items found to purge from queue {0}", Q.Id));
                        }
                    }
                    #endregion
                }
                else
                {
                    LogHelper.LogDebug(request.OrganizationName, true, request.UserID, "UDOScheduledJobProcessor", "No queues found marked for scheduled purge.");
                }
                //progressString = "All items added to requestcollection.";
                #endregion
                //#region executemultiple
                //ExecuteMultipleHelperResponse result;
                //if (requestCollection.Count > 0)
                //{
                //    result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, request.UserID, true);
                //    progressString = "After execute multiple";
                //#endregion
                    #region Recreate the Config Record
                    var scheduledJob = new Entity("udo_scheduledjob");
                    scheduledJob["udo_name"] = request.JobName;
                    OrgServiceProxy.Create(scheduledJob);
                    #endregion
                //}
            }
			catch (Exception connectException)
			 {
                LogHelper.LogError(request.OrganizationName, request.UserId, $"Exception during CRM Connect: {connectException.Message}", connectException);
                LogHelper.LogError(request.OrganizationName, request.UserId, "udo_scheduledjob", "UDOScheduledJobCleanup Processor, Progress:" + progressString, connectException);
			}
            finally
            {
                if (OrgServiceProxy != null)
                {
                    OrgServiceProxy.Dispose();
                }
            }
        }
	}
}