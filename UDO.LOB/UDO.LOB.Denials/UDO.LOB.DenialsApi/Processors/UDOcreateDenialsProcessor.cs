/// <summary>
/// UDOcreateDenialsProcessor
/// Execute VEISfednpctpfindDenialsByPtcpntIdRequest and creates multiple udo_denial records in CRM
/// </summary>
namespace UDO.LOB.Denials.Processors
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Tooling.Connector;
    using System;
    using System.Linq;
    using UDO.LOB.Core;
    using UDO.LOB.Denials.Messages;
    using UDO.LOB.Extensions;
    using UDO.LOB.Extensions.Logging;
    using VEIS.Core.Messages;
    using VEIS.Messages.ClaimantService;

    class UDOcreateDenialsProcessor
    {
        private CrmServiceClient OrgServiceProxy;
        private bool _debug { get; set; }
        private string LogBuffer { get; set; }
        private const string method = "UDOcreateDenialsProcessor";

        public IMessageBase Execute(UDOcreateDenialsRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.Execute");
            UDOcreateDenialsResponse response = new UDOcreateDenialsResponse();
            response.MessageId = request.MessageId;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccurred = true;
                return response;
            }
            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = "UDOcreateAwardsProcessor",
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
                };
            }
            TraceLogger tLogger = new TraceLogger(method, request);

            LogHelper.LogInfo($">> {this.GetType().FullName}.Execute: Request:  {JsonHelper.Serialize<UDOcreateDenialsRequest>(request)}");

            _debug = request.Debug;
            LogBuffer = string.Empty;

            string progressString = "Top of Processor";

            #region connect to CRM
            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
                tLogger.LogEvent($"Authenticated and Connected to CRM", "2");
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException);
                response.ExceptionMessage = $"{method}: Failed to get CRMConnection";
                response.ExceptionOccurred = true;
                return response;
            }
            #endregion

            progressString = "After Connection";

            try
            {
                // prefix = fednpctpfindDenialsByPtcpntIdRequest();
                var findDenialsByPtcpntIdRequest = new VEISfednpctpfindDenialsByPtcpntIdRequest();
                findDenialsByPtcpntIdRequest.MessageId = request.MessageId;
                findDenialsByPtcpntIdRequest.LogTiming = request.LogTiming;
                findDenialsByPtcpntIdRequest.LogSoap = request.LogSoap;
                findDenialsByPtcpntIdRequest.Debug = request.Debug;
                findDenialsByPtcpntIdRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findDenialsByPtcpntIdRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findDenialsByPtcpntIdRequest.RelatedParentId = request.RelatedParentId;
                findDenialsByPtcpntIdRequest.UserId = request.UserId;
                findDenialsByPtcpntIdRequest.OrganizationName = request.OrganizationName;
                if (request.LegacyServiceHeaderInfo != null)
                {
                    findDenialsByPtcpntIdRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                }
                findDenialsByPtcpntIdRequest.mcs_ptcpntid = request.ptcpntId;

                var findDenialsByPtcpntIdResponse = WebApiUtility.SendReceive<VEISfednpctpfindDenialsByPtcpntIdResponse>(findDenialsByPtcpntIdRequest, WebApiType.VEIS);
                progressString = "After VEIS EC Call";

                if (request.LogSoap || findDenialsByPtcpntIdResponse.ExceptionOccurred)
                {
                    if (findDenialsByPtcpntIdResponse.SerializedSOAPRequest != null || findDenialsByPtcpntIdResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findDenialsByPtcpntIdResponse.SerializedSOAPRequest + findDenialsByPtcpntIdResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfednpctpfindDenialsByPtcpntIdRequest Request/Response {requestResponse}", true);
                    }
                }

                var requestCollection = new OrganizationRequestCollection();

                response.MessageId = request.MessageId;
                response.ExceptionMessage = findDenialsByPtcpntIdResponse.ExceptionMessage;
                response.ExceptionOccurred = findDenialsByPtcpntIdResponse.ExceptionOccurred;
                if (findDenialsByPtcpntIdResponse.VEISfednpctpreturnInfo.VEISfednpctpdenialsInfo != null)
                {
                    var denial = findDenialsByPtcpntIdResponse.VEISfednpctpreturnInfo.VEISfednpctpdenialsInfo;
                    System.Collections.Generic.List<UDOcreateDenialsMultipleResponse> UDOcreateDenialsArray = new System.Collections.Generic.List<UDOcreateDenialsMultipleResponse>();
                    foreach (var denialItem in denial)
                    {
                        var responseIds = new UDOcreateDenialsMultipleResponse();
                        //instantiate the new Entity
                        Entity thisNewEntity = new Entity();
                        thisNewEntity.LogicalName = "udo_denial";
                        thisNewEntity["udo_name"] = "Denial Summary";
                        if (request.ownerId != System.Guid.Empty)
                        {
                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                        }
                        if (!string.IsNullOrEmpty(denialItem.mcs_awardTypeNm))
                        {
                            thisNewEntity["udo_awardtype"] = denialItem.mcs_awardTypeNm;
                        }
                        if (!string.IsNullOrEmpty(denialItem.mcs_adminDate))
                        {
                            thisNewEntity["udo_admindate"] = dateStringFormat(denialItem.mcs_adminDate);

                        }
                        if (!string.IsNullOrEmpty(denialItem.mcs_claimDate))
                        {
                            thisNewEntity["udo_claimdate"] = dateStringFormat(denialItem.mcs_claimDate);

                        }
                        if (!string.IsNullOrEmpty(denialItem.mcs_claimPayeeCd))
                        {
                            thisNewEntity["udo_claimpayeecode"] = denialItem.mcs_claimPayeeCd;
                        }
                        if (!string.IsNullOrEmpty(denialItem.mcs_claimTypeNm))
                        {
                            thisNewEntity["udo_claimtype"] = denialItem.mcs_claimTypeNm;
                        }
                        if (!string.IsNullOrEmpty(denialItem.mcs_claimTypeCd))
                        {
                            thisNewEntity["udo_claimtypecode"] = denialItem.mcs_claimTypeCd;
                        }
                        if (!string.IsNullOrEmpty(denialItem.mcs_decisionDate))
                        {
                            thisNewEntity["udo_decisiondate"] = dateStringFormat(denialItem.mcs_decisionDate);
                        }
                        if (!string.IsNullOrEmpty(denialItem.mcs_decisionType))
                        {
                            thisNewEntity["udo_decisiontype"] = denialItem.mcs_decisionType;
                        }
                        if (!string.IsNullOrEmpty(denialItem.mcs_programTypeCd))
                        {
                            thisNewEntity["udo_programtypecode"] = denialItem.mcs_programTypeCd;
                        }
                        if (!string.IsNullOrEmpty(denialItem.mcs_rbaId))
                        {
                            thisNewEntity["udo_rbaid"] = denialItem.mcs_rbaId;
                        }
                        if (!string.IsNullOrEmpty(denialItem.mcs_decisionNm))
                        {
                            thisNewEntity["udo_reasonpreview"] = denialItem.mcs_decisionNm;
                        }
                        //not mapped thisNewEntity["udo_fullreason"]=??


                        if (request.UDOcreateDenialsRelatedEntitiesInfo != null)
                        {
                            foreach (var relatedItem in request.UDOcreateDenialsRelatedEntitiesInfo)
                            {
                                thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                            }
                        }
                        CreateRequest createData = new CreateRequest
                        {
                            Target = thisNewEntity
                        };
                        requestCollection.Add(createData);
                    }

                    #region Create records

                    if (requestCollection.Count() > 0)
                    {
                        var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, Guid.Empty, request.Debug);

                        if (_debug)
                        {
                            LogBuffer += result.LogDetail;
                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, LogBuffer, _debug);
                        }

                        if (result.IsFaulted)
                        {
                            LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, result.ErrorDetail);
                            response.ExceptionMessage = result.FriendlyDetail;
                            response.ExceptionOccurred = true;
                            return response;
                        }
                    }
                    #endregion

                    string logInfo = string.Format("Denials Records Created: {0}", requestCollection.Count());
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, logInfo, request.Debug);
                }
                //added to generated code
                if (request.udo_idproof != Guid.Empty)
                {
                    var parent = new Entity();
                    parent.Id = request.udo_idproof;
                    parent.LogicalName = "udo_idproof";
                    parent["udo_denialscomplete"] = true;
                    // OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming));
                    OrgServiceProxy.Update(parent);
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method, ExecutionException);
                response.ExceptionMessage = "Failed to Map Denials EC data to LOB - " + ExecutionException.Message + " / " + ExecutionException.StackTrace;
                response.ExceptionOccurred = true;
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

        private static string dateStringFormat(string date)
        {
            if (date == null) return null;
            if (date.Length == 10) return date;
            if (date.Length < 8) return date;

            date = date.Insert(2, "/");
            date = date.Insert(5, "/");
            return date;
        }
    }

}