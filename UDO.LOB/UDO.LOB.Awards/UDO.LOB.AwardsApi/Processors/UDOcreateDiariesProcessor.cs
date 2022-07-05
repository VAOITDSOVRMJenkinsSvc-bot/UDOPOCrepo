using global::UDO.LOB.Awards.Messages;
using global::UDO.LOB.Core;
using global::UDO.LOB.Extensions;
using global::UDO.LOB.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;
using VEIS.Messages.ClaimantService;

namespace UDO.LOB.Awards.Processors
{
    internal class UDOcreateDiariesProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOcreateDiariesProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOcreateDiariesRequest request)
        {
            //var request = message as createDiariesRequest;
            UDOcreateDiariesResponse response = new UDOcreateDiariesResponse();
            response.MessageId = request.MessageId;
            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = method,
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
                };
            }
            TraceLogger tLogger = new TraceLogger(method, request);
            string progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;
            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }

            #region connect to CRM
            CrmServiceClient OrgServiceProxy;
            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
                tLogger.LogEvent($"Authenticated and Connected to CRM", "2");
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

            progressString = "After Connection";

            try
            {
                VEISfgenFNfindGeneralInformationByFileNumberRequest findGeneralInformationByFileNumberRequest = new VEISfgenFNfindGeneralInformationByFileNumberRequest();
                findGeneralInformationByFileNumberRequest.LogTiming = request.LogTiming;
                findGeneralInformationByFileNumberRequest.LogSoap = request.LogSoap;
                findGeneralInformationByFileNumberRequest.Debug = request.Debug;
                findGeneralInformationByFileNumberRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findGeneralInformationByFileNumberRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findGeneralInformationByFileNumberRequest.RelatedParentId = request.RelatedParentId;
                findGeneralInformationByFileNumberRequest.UserId = request.UserId;
                findGeneralInformationByFileNumberRequest.OrganizationName = request.OrganizationName;

                findGeneralInformationByFileNumberRequest.mcs_filenumber = request.fileNumber;
                if (request.LegacyServiceHeaderInfo != null)
                {
                    findGeneralInformationByFileNumberRequest.LegacyServiceHeaderInfo = new VEIS.Core.Messages.LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                }

                //REM: Invoke VEIS WebApi 
                var findGeneralInformationByFileNumberResponse = WebApiUtility.SendReceive<VEISfgenFNfindGeneralInformationByFileNumberResponse>(findGeneralInformationByFileNumberRequest, WebApiType.VEIS);
                if (request.LogSoap || findGeneralInformationByFileNumberResponse.ExceptionOccurred)
                {
                    if (findGeneralInformationByFileNumberResponse.SerializedSOAPRequest != null || findGeneralInformationByFileNumberResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findGeneralInformationByFileNumberResponse.SerializedSOAPRequest + findGeneralInformationByFileNumberResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfgenFNfindGeneralInformationByFileNumberRequest Request/Response {requestResponse}", true);
                    }
                }

                progressString = "After VEIS Call";

                response.ExceptionMessage = findGeneralInformationByFileNumberResponse.ExceptionMessage;
                response.ExceptionOccured = findGeneralInformationByFileNumberResponse.ExceptionOccurred;

                int diariesCount = 0;
                OrganizationRequestCollection requestCollection = new OrganizationRequestCollection();

                if (findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.VEISfgenFNdiariesInfo != null)
                {
                    VEISfgenFNdiariesMultipleResponse[] diary = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.VEISfgenFNdiariesInfo;
                    foreach (VEISfgenFNdiariesMultipleResponse diaryItem in diary)
                    {
                        UDOcreateDiariesMultipleResponse responseIds = new UDOcreateDiariesMultipleResponse();
                        //instantiate the new Entity
                        Entity thisNewEntity = new Entity();
                        thisNewEntity.LogicalName = "udo_diaries";
                        if (request.ownerId != System.Guid.Empty)
                        {
                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                        }
                        if (diaryItem.mcs_date != string.Empty)
                        {
                            DateTime newDateTime;
                            string newDate = dateStringFormat(diaryItem.mcs_date);
                            if (DateTime.TryParse(newDate, out newDateTime))
                            {
                                thisNewEntity["udo_date"] = newDateTime;
                            }
                        }
                        if (diaryItem.mcs_description != string.Empty)
                        {
                            thisNewEntity["udo_desc"] = diaryItem.mcs_description.Trim();
                        }
                        if (diaryItem.mcs_reasonCd != string.Empty)
                        {
                            thisNewEntity["udo_reasoncd"] = diaryItem.mcs_reasonCd;
                        }
                        if (diaryItem.mcs_reasonName != string.Empty)
                        {
                            thisNewEntity["udo_reasonname"] = diaryItem.mcs_reasonName;
                        }
                        if (request.UDOcreateDiariesRelatedEntitiesInfo != null)
                        {
                            foreach (UDOcreateDiariesRelatedEntitiesMultipleRequest relatedItem in request.UDOcreateDiariesRelatedEntitiesInfo)
                            {
                                thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                            }
                        }
                        CreateRequest createData = new CreateRequest
                        {
                            Target = thisNewEntity
                        };
                        requestCollection.Add(createData);
                        diariesCount += 1;

                    }

                }
                #region Create records

                if (diariesCount > 0)
                {
                    var result = new ExecuteMultipleHelperResponse();
                    result.IsFaulted = false;
                    result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, Guid.Empty, request.Debug);
                    // result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, request.UserId, request.Debug);

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

                string logInfo = string.Format("Diary Records Created: {0}", diariesCount);
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "Diary Records Created", logInfo, request.Debug);

                #endregion

                //added to generated code
                if (request.AwardId != null)
                {
                    Entity parent = new Entity();
                    parent.Id = request.AwardId;
                    parent.LogicalName = "udo_award";
                    parent["udo_diariescomplete"] = true;
                    OrgServiceProxy.Update(parent);
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "UDOcreateDiariesProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Award Diary Data";
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
