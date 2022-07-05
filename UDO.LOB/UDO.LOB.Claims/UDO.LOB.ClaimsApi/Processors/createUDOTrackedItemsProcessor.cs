using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;
using UDO.LOB.Claims.Messages;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using VEIS.Core.Messages;
using VEIS.Messages.TrackedItemService;

/// <summary>
/// VIMT LOB Component for UDOcreateUDOTrackedItems,createUDOTrackedItems method, Processor.
/// </summary>
namespace UDO.LOB.Claims.Processors
{
    class UDOcreateUDOTrackedItemsProcessor
    {
        private bool _debug { get; set; }

        private string LogBuffer { get; set; }

        private const string method = "UDOcreateUDOTrackedItemsProcessor";
        
        public IMessageBase Execute(UDOcreateUDOTrackedItemsRequest request)
        {
            UDOcreateUDOTrackedItemsResponse response = new UDOcreateUDOTrackedItemsResponse { MessageId = request.MessageId };
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }
            if (request.DiagnosticsContext == null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : string.Empty
                };
            }
            TraceLogger aiLogger = new TraceLogger("UDOcreateUDOTrackedItemsProcessor.Execute", request);
            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = "UDOgetMilitaryInformationProcessor",
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
                };
            }

            #region connect to CRM
            CrmServiceClient OrgServiceProxy = null;
            try
            {

                OrgServiceProxy = ConnectionCache.GetProxy();
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, "UDOgetMilitaryInformationProcessor", connectException);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

            progressString = "After Connection";

            try
            {
                var findTrackedItemsRequest = new VEISfindTrackedItemsRequest();
                findTrackedItemsRequest.LogTiming = request.LogTiming;
                findTrackedItemsRequest.LogSoap = request.LogSoap;
                findTrackedItemsRequest.Debug = request.Debug;
                findTrackedItemsRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findTrackedItemsRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findTrackedItemsRequest.RelatedParentId = request.RelatedParentId;
                findTrackedItemsRequest.UserId = request.UserId;
                findTrackedItemsRequest.OrganizationName = request.OrganizationName;

                findTrackedItemsRequest.mcs_claimid = request.claimId;
                findTrackedItemsRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                   
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
                // REM: Invoke VEIS Endpoint
                var findTrackedItemsResponse = WebApiUtility.SendReceive<VEISfindTrackedItemsResponse>(findTrackedItemsRequest, WebApiType.VEIS);

                if (request.LogSoap || findTrackedItemsResponse.ExceptionOccurred)
                {
                    if (findTrackedItemsResponse.SerializedSOAPRequest != null || findTrackedItemsResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findTrackedItemsResponse.SerializedSOAPRequest + findTrackedItemsResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfindTrackedItemsRequest Request/Response {requestResponse}", true);
                    }
                }

                progressString = "After VEIS EC Call";
                var requestCollection = new OrganizationRequestCollection();

                response.ExceptionMessage = findTrackedItemsResponse.ExceptionMessage;
                response.ExceptionOccured = findTrackedItemsResponse.ExceptionOccurred;
                #region trackedItem Part
                int trackItemCount = 0;
                // Replaced: VIMTBenefitClaimtrkItemInfo.VIMTdvlpmtItemstrkItemInfo = VEISbenefitClaimInfo.VEISdvlpmtItemsInfo
                if (findTrackedItemsResponse.VEISbenefitClaimInfo.VEISdvlpmtItemsInfo != null)
                {
                    var developmentItem = findTrackedItemsResponse.VEISbenefitClaimInfo.VEISdvlpmtItemsInfo;
                    System.Collections.Generic.List<UDOcreateUDOTrackedItemsMultipleResponse> UDOcreateUDOTrackedItemsArray = new System.Collections.Generic.List<UDOcreateUDOTrackedItemsMultipleResponse>();
                    foreach (var developmentItemItem in developmentItem)
                    {
                        var responseIds = new UDOcreateUDOTrackedItemsMultipleResponse();
                        //instantiate the new Entity
                        Entity thisNewEntity = new Entity();
                        thisNewEntity.LogicalName = "udo_trackeditem";
                        if (request.ownerId != System.Guid.Empty)
                        {
                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                        }
                        if (Convert.ToDateTime(findTrackedItemsResponse.VEISbenefitClaimInfo.mcs_closedDt) != System.DateTime.MinValue)
                        {
                            thisNewEntity["udo_closeddate"] = findTrackedItemsResponse.VEISbenefitClaimInfo.mcs_closedDt;
                        }
                        if (developmentItemItem.mcs_recipient != string.Empty)
                        {
                            thisNewEntity["udo_receipient"] = developmentItemItem.mcs_recipient;
                        }
                        if (Convert.ToDateTime(developmentItemItem.mcs_receiveDt) != System.DateTime.MinValue)
                        {
                            thisNewEntity["udo_receiveddate"] = Convert.ToDateTime(developmentItemItem.mcs_receiveDt);
                        }
                        if (Convert.ToDateTime(developmentItemItem.mcs_reqDt) != System.DateTime.MinValue)
                        {
                            thisNewEntity["udo_requestdate"] = Convert.ToDateTime(developmentItemItem.mcs_reqDt);
                        }
                        if (Convert.ToDateTime(developmentItemItem.mcs_suspnsDt) != System.DateTime.MinValue)
                        {
                            thisNewEntity["udo_suspensedate"] = Convert.ToDateTime(developmentItemItem.mcs_suspnsDt);
                        }
                        if (developmentItemItem.mcs_shortNm != string.Empty)
                        {
                            thisNewEntity["udo_developmentactionletter"] = developmentItemItem.mcs_shortNm;
                        }
                        if (Convert.ToDateTime(developmentItemItem.mcs_inErrorDt) != System.DateTime.MinValue)
                        {
                            thisNewEntity["udo_inerrordate"] = Convert.ToDateTime(developmentItemItem.mcs_inErrorDt);
                        }
                        if (Convert.ToDateTime(developmentItemItem.mcs_followDt) != System.DateTime.MinValue)
                        {
                            thisNewEntity["udo_followupdate"] = Convert.ToDateTime(developmentItemItem.mcs_followDt);
                        }
                        if (Convert.ToDateTime(developmentItemItem.mcs_scndFlwDt) != System.DateTime.MinValue)
                        {
                            thisNewEntity["udo_secondfollowupdate"] = Convert.ToDateTime(developmentItemItem.mcs_scndFlwDt);
                        }
                        if (Convert.ToDateTime(developmentItemItem.mcs_acceptDt) != System.DateTime.MinValue)
                        {
                            thisNewEntity["udo_acceptdate"] = Convert.ToDateTime(developmentItemItem.mcs_acceptDt);
                        }

                        if (developmentItemItem.mcs_dvlpmtTc != string.Empty)
                        {
                            thisNewEntity["udo_devitemtc"] = developmentItemItem.mcs_dvlpmtTc;
                        }

                        if (request.UDOcreateUDOTrackedItemsRelatedEntitiesInfo != null)
                        {
                            foreach (var relatedItem in request.UDOcreateUDOTrackedItemsRelatedEntitiesInfo)
                            {
                                thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                            }
                        }
                        CreateRequest createData = new CreateRequest
                        {
                            Target = thisNewEntity
                        };
                        requestCollection.Add(createData);
                        trackItemCount += 1;
                    }
                }
                #endregion

                #region MAPDLetters Part
                int letterCount = 0;
                // Replaced: VIMTletterstrkItemInfo = VEISlettersInfo
                if (findTrackedItemsResponse.VEISbenefitClaimInfo.VEISlettersInfo != null)
                {
                    var letter = findTrackedItemsResponse.VEISbenefitClaimInfo.VEISlettersInfo;
                    foreach (var letterItem in letter)
                    {
                        //instantiate the new Entity
                        Entity thisNewEntity = new Entity();
                        thisNewEntity.LogicalName = "udo_mapdletter";
                        if (letterItem.mcs_nm != string.Empty)
                        {
                            thisNewEntity["udo_letter"] = string.Format("{0} - {1}", letterItem.mcs_nm, letterItem.mcs_docid);
                        }
                        if (request.ownerId != System.Guid.Empty)
                        {
                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                        }                        
                        if (Convert.ToDateTime(letterItem.mcs_dcmntDt) != System.DateTime.MinValue)
                        {
                            thisNewEntity["udo_dateissued"] = Convert.ToDateTime(letterItem.mcs_dcmntDt);
                        }
                        if (letterItem.mcs_ptcpntId != string.Empty)
                        {
                            thisNewEntity["udo_participantid"] = letterItem.mcs_ptcpntId;
                        }
                        if (letterItem.mcs_dvlpmtTc != string.Empty)
                        {
                            thisNewEntity["udo_typecode"] = letterItem.mcs_dvlpmtTc;
                        }
                        if (letterItem.mcs_nm != string.Empty)
                        {
                            thisNewEntity["udo_typeofletter"] = letterItem.mcs_nm;
                        }
                        if (letterItem.mcs_docid != string.Empty)
                        {
                            thisNewEntity["udo_documentid"] = letterItem.mcs_docid;
                        }
                        if (request.UDOcreateUDOTrackedItemsRelatedEntitiesInfo != null)
                        {
                            foreach (var relatedItem in request.UDOcreateUDOTrackedItemsRelatedEntitiesInfo)
                            {
                                thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                            }
                        }
                        
                        CreateRequest createData = new CreateRequest
                        {
                            Target = thisNewEntity
                        };
                        requestCollection.Add(createData);
                        letterCount += 1;
                    }
                }
                #endregion

                #region Create records

                if (requestCollection.Count() > 0)
                {
                    var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, Guid.Empty, request.Debug);

                    if (_debug)
                    {
                        LogBuffer += result.LogDetail;
                        LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, LogBuffer);
                    }

                    if (result.IsFaulted)
                    {
                        LogHelper.LogError(request.OrganizationName, request.UserId, method, result.ErrorDetail);
                        response.ExceptionMessage = result.FriendlyDetail;
                        response.ExceptionOccured = true;
                        return response;
                    }
                }

                string logInfo = string.Format("Tracked Item Records Created: {0}; MAPD Letter Records Created: {1}", trackItemCount, letterCount);
                LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "MAPD Letter and Tracked Item Records Created", logInfo);
                #endregion

                //added to generated code
                if (request.udo_claimId != null && request.udo_claimId != Guid.Empty)
                {
                    var parent = new Entity();
                    parent.Id = request.udo_claimId;
                    parent.LogicalName = "udo_claim";
                    parent["udo_trackeditemmapdcomplete"] = true;
                    OrgServiceProxy.Update(parent);
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName,request.MessageId, method + " Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Track Item/MAPD Letter Data"; 
                response.ExceptionOccured = true;
                return response;
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