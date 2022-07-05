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
/// VIMT LOB Component for UDOcreateUDOEvidence,createUDOEvidence method, Processor.
/// </summary>
namespace UDO.LOB.Claims.Processors
{
    class UDOcreateUDOEvidenceProcessor
    {
        private bool _debug { get; set; }

        private string LogBuffer { get; set; }

        private const string method = "UDOcreateUDOEvidenceProcessor";
        
        public IMessageBase Execute(UDOcreateUDOEvidenceRequest request)
        {
            UDOcreateUDOEvidenceResponse response = new UDOcreateUDOEvidenceResponse { MessageId = request.MessageId };
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
            TraceLogger aiLogger = new TraceLogger("UDOcreateUDOEvidenceProcessor.Execute", request);
            LogHelper.LogInfo(string.Format("Message Id:{0}, Type={2}, Recieved diagnostics message: {1}",
            request.MessageId,
            request.MessageId,
            GetType().FullName));

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
            var requestCollection = new OrganizationRequestCollection();

            try
            {
                var findUnsolEvdnceRequest = new VEISfindUnsolEvdnceRequest();
                findUnsolEvdnceRequest.LogTiming = request.LogTiming;
                findUnsolEvdnceRequest.LogSoap = request.LogSoap;
                findUnsolEvdnceRequest.Debug = request.Debug;
                findUnsolEvdnceRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findUnsolEvdnceRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findUnsolEvdnceRequest.RelatedParentId = request.RelatedParentId;
                findUnsolEvdnceRequest.UserId = request.UserId;
                findUnsolEvdnceRequest.OrganizationName = request.OrganizationName;

                findUnsolEvdnceRequest.mcs_claiment_ptpcpnt_id = request.Claiment_ptpcpnt_id;
                findUnsolEvdnceRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
                // REM: Invoke VEIS Endpoint
                var findUnsolEvdnceResponse = WebApiUtility.SendReceive<VEISfindUnsolEvdnceResponse>(findUnsolEvdnceRequest, WebApiType.VEIS);

                if (request.LogSoap || findUnsolEvdnceResponse.ExceptionOccurred)
                {
                    if (findUnsolEvdnceResponse.SerializedSOAPRequest != null || findUnsolEvdnceResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findUnsolEvdnceResponse.SerializedSOAPRequest + findUnsolEvdnceResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfindUnsolEvdnceRequest Request/Response {requestResponse}", true);
                    }
                }

                progressString = "After VEIS EC Call";

                response.ExceptionMessage = findUnsolEvdnceResponse.ExceptionMessage;
                response.ExceptionOccured = findUnsolEvdnceResponse.ExceptionOccurred;
                // Replaced: VIMTUnsolicitedEvidencetrkItemInfo = VEISunsolicitedEvidenceInfo
                if (findUnsolEvdnceResponse.VEISunsolicitedEvidenceInfo != null)
                {
                    var UnsolicitedEvidence = findUnsolEvdnceResponse.VEISunsolicitedEvidenceInfo;
                    System.Collections.Generic.List<UDOcreateUDOEvidenceMultipleResponse> UDOcreateUDOEvidenceArray = new System.Collections.Generic.List<UDOcreateUDOEvidenceMultipleResponse>();
                    foreach (var UnsolicitedEvidenceItem in UnsolicitedEvidence)
                    {
                        var responseIds = new UDOcreateUDOEvidenceMultipleResponse();
                        //instantiate the new Entity
                        Entity thisNewEntity = new Entity();
                        thisNewEntity.LogicalName = "udo_evidence";
                        if (request.ownerId != System.Guid.Empty)
                        {
                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                        }

                        if (UnsolicitedEvidenceItem.mcs_userName != string.Empty)
                        {
                            thisNewEntity["udo_lastupdatedby"] = UnsolicitedEvidenceItem.mcs_userName;
                        }
                        if (Convert.ToDateTime(UnsolicitedEvidenceItem.mcs_jrnDt) != System.DateTime.MinValue)
                        {
                            thisNewEntity["udo_journaldate"] = Convert.ToDateTime(UnsolicitedEvidenceItem.mcs_jrnDt);
                        }
                        if (Convert.ToDateTime(UnsolicitedEvidenceItem.mcs_rcvdDt) != System.DateTime.MinValue)
                        {
                            thisNewEntity["udo_datereceived"] = Convert.ToDateTime(UnsolicitedEvidenceItem.mcs_rcvdDt);
                        }
                        if (UnsolicitedEvidenceItem.mcs_descTxt != string.Empty)
                        {
                            thisNewEntity["udo_name"] = UnsolicitedEvidenceItem.mcs_descTxt;
                        }
                        if (request.UDOcreateUDOEvidenceRelatedEntitiesInfo != null)
                        {
                            foreach (var relatedItem in request.UDOcreateUDOEvidenceRelatedEntitiesInfo)
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

                    #region Log Results
                    string logInfo = string.Format("Evidence Records Created: {0}", requestCollection.Count());
                    LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Evidence Records Created", logInfo);
                    #endregion
                }

                //added to generated code
                if (request.udo_claimId != System.Guid.Empty)
                {
                    var parent = new Entity();
                    parent.Id = request.udo_claimId;
                    parent.LogicalName = "udo_claim";
                    parent["udo_evidencecomplete"] = true;
                    OrgServiceProxy.Update(parent);
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, method + " Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to process Evidence data";
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