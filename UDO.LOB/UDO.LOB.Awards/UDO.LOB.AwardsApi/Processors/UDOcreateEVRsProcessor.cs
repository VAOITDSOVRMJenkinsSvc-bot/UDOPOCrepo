using global::UDO.LOB.Awards.Messages;
using global::UDO.LOB.Core;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using VEIS.Messages.ClaimantService;

namespace UDO.LOB.Awards.Processors
{
    class UDOcreateEVRsProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOcreateEVRsProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOcreateEVRsRequest request)
        {
            //var request = message as createEVRsRequest;
            UDOcreateEVRsResponse response = new UDOcreateEVRsResponse();
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
            var progressString = "Top of Processor";
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
            tLogger.LogEvent("Connect to CRM", "001");
            progressString = "After Connection";

            try
            {
                // prefix = fgenFNfindGeneralInformationByFileNumberRequest();
                var findGeneralInformationByFileNumberRequest = new VEISfgenFNfindGeneralInformationByFileNumberRequest();
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

                var findGeneralInformationByFileNumberResponse = WebApiUtility.SendReceive<VEISfgenFNfindGeneralInformationByFileNumberResponse>(findGeneralInformationByFileNumberRequest, WebApiType.VEIS);
                if (request.LogSoap || findGeneralInformationByFileNumberResponse.ExceptionOccurred)
                {
                    if (findGeneralInformationByFileNumberResponse.SerializedSOAPRequest != null || findGeneralInformationByFileNumberResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findGeneralInformationByFileNumberResponse.SerializedSOAPRequest + findGeneralInformationByFileNumberResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfgenFNfindGeneralInformationByFileNumberRequest Request/Response {requestResponse}", true);
                    }
                }

                tLogger.LogEvent("Web Service Call VEISfgenFNfindGeneralInformationByFileNumber", "002");
                progressString = "After VIMT EC Call";

                response.ExceptionMessage = findGeneralInformationByFileNumberResponse.ExceptionMessage;
                response.ExceptionOccured = findGeneralInformationByFileNumberResponse.ExceptionOccurred;

                var EVRCount = 0;
                var requestCollection = new OrganizationRequestCollection();

                if (findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.VEISfgenFNevrsInfo != null)
                {
                    var evr = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo.VEISfgenFNevrsInfo;
                    foreach (var evrItem in evr)
                    {
                        var responseIds = new UDOcreateEVRsMultipleResponse();
                        //instantiate the new Entity
                        Entity thisNewEntity = new Entity();
                        thisNewEntity.LogicalName = "udo_evr";
                        thisNewEntity["udo_name"] = "Appeal Summary";
                        if (request.ownerId != System.Guid.Empty)
                        {
                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                        }
                        if (evrItem.mcs_type != string.Empty)
                        {
                            thisNewEntity["udo_type"] = evrItem.mcs_type;
                        }
                        if (evrItem.mcs_status != string.Empty)
                        {
                            thisNewEntity["udo_status"] = evrItem.mcs_status;
                        }
                        if (evrItem.mcs_lastReported != string.Empty)
                        {
                            DateTime newDateTime;
                            var newDate = dateStringFormat(evrItem.mcs_lastReported);
                            if (DateTime.TryParse(newDate, out newDateTime))
                            {
                                thisNewEntity["udo_lastreported"] = newDateTime;
                            }
                        }
                        if (evrItem.mcs_exempt != string.Empty)
                        {
                            thisNewEntity["udo_exempt"] = evrItem.mcs_exempt;
                        }
                        if (evrItem.mcs_control != string.Empty)
                        {
                            thisNewEntity["udo_control"] = evrItem.mcs_control;
                        }
                        if (request.UDOcreateEVRsRelatedEntitiesInfo != null)
                        {
                            foreach (var relatedItem in request.UDOcreateEVRsRelatedEntitiesInfo)
                            {
                                thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                            }
                        }
                        CreateRequest createData = new CreateRequest
                        {
                            Target = thisNewEntity
                        };
                        requestCollection.Add(createData);
                        EVRCount += 1;

                    }

                }
                #region Create records

                if (EVRCount > 0)
                {

                    var result = new ExecuteMultipleHelperResponse();
                    result.IsFaulted = false;
                    result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, Guid.Empty, request.Debug);

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

                string logInfo = string.Format("EVR Records Created: {0}", EVRCount);
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "EVR Records Created", logInfo, request.Debug);
                #endregion

                tLogger.LogEvent("Execute Multiple Awards / CreateEVR", "003");
                //added to generated code
                if (request.AwardId != null)
                {
                    var parent = new Entity();
                    parent.Id = request.AwardId;
                    parent.LogicalName = "udo_award";
                    parent["udo_evrcomplete"] = true;
                    OrgServiceProxy.Update(parent);
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "UDOcreateEVRsProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Award EVR Data";
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
