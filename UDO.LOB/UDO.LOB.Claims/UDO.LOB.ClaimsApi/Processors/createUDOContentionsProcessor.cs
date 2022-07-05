using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;
using UDO.LOB.Claims.Messages;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using VEIS.Core.Messages;
using VEIS.Messages.ContentionService;

/// <summary>
/// VIMT LOB Component for UDOcreateUdoContentions,createUdoContentions method, Processor.
/// </summary>
namespace UDO.LOB.Claims.Processors
{
    class UDOcreateUdoContentionsProcessor
    {
        private const string method = "UDOcreateUdoContentionsProcessor";

        private bool _debug { get; set; }
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOcreateUdoContentionsRequest request)
        {
            UDOcreateUdoContentionsResponse response = new UDOcreateUdoContentionsResponse { MessageId = request.MessageId };
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
            TraceLogger aiLogger = new TraceLogger("UDOcreateUdoContentionsProcessor.Execute", request);

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
                var findContentionsRequest = new VEISfcontfindContentionsRequest();
                findContentionsRequest.LogTiming = request.LogTiming;
                findContentionsRequest.LogSoap = request.LogSoap;
                findContentionsRequest.Debug = request.Debug;
                findContentionsRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findContentionsRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findContentionsRequest.RelatedParentId = request.RelatedParentId;
                findContentionsRequest.UserId = request.UserId;
                findContentionsRequest.OrganizationName = request.OrganizationName;
                findContentionsRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
                findContentionsRequest.mcs_claimid = request.claimId;

                // REM: Invoke VEIS Endpoint
                var findContentionsResponse = WebApiUtility.SendReceive<VEISfcontfindContentionsResponse>(findContentionsRequest, WebApiType.VEIS);

                if (request.LogSoap || findContentionsResponse.ExceptionOccurred)
                {
                    if (findContentionsResponse.SerializedSOAPRequest != null || findContentionsResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findContentionsResponse.SerializedSOAPRequest + findContentionsResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfcontfindContentionsRequest Request/Response {requestResponse}", true);
                    }
                }

                progressString = "After VEIS EC Call";
                var requestCollection = new OrganizationRequestCollection();
                response.ExceptionMessage = findContentionsResponse.ExceptionMessage;
                response.ExceptionOccured = findContentionsResponse.ExceptionOccurred;
                if (findContentionsResponse.VEISfcontbenefitClaimInfo.VEISfcontcontentionsInfo != null)
                {
                    var contention = findContentionsResponse.VEISfcontbenefitClaimInfo.VEISfcontcontentionsInfo;
                    System.Collections.Generic.List<UDOcreateUdoContentionsMultipleResponse> UDOcreateUdoContentionsArray = new System.Collections.Generic.List<UDOcreateUdoContentionsMultipleResponse>();
                    foreach (var contentionItem in contention)
                    {
                        var responseIds = new UDOcreateUdoContentionsMultipleResponse();
                        //instantiate the new Entity
                        Entity thisNewEntity = new Entity();
                        thisNewEntity.LogicalName = "udo_contention";
                        if (request.ownerId != System.Guid.Empty)
                        {
                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                        }

                        if (contentionItem.mcs_cntntnStatusTc != string.Empty)
                        {
                            //ok
                            thisNewEntity["udo_status"] = contentionItem.mcs_cntntnStatusTc;
                        }
                        if (contentionItem.mcs_dgnstcTc != string.Empty)
                        {
                            thisNewEntity["udo_diagnosticcode"] = contentionItem.mcs_dgnstcTc;
                        }
                        if (contentionItem.mcs_dgnstcTn != string.Empty)
                        {
                            thisNewEntity["udo_description"] = contentionItem.mcs_clmntTxt;
                        }
                        if (contentionItem.mcs_clsfcnTxt != string.Empty)
                        {
                            thisNewEntity["udo_contentionclassification"] = contentionItem.mcs_clsfcnTxt;
                        }
                        if (contentionItem.mcs_medInd != string.Empty)
                        {
                            thisNewEntity["udo_codesheetdiagnosis"] = contentionItem.mcs_medInd;
                        }
                        if (Convert.ToDateTime(findContentionsResponse.VEISfcontbenefitClaimInfo.mcs_claimRcvdDt) != System.DateTime.MinValue)
                        {
                            thisNewEntity["udo_claimreceived"] = Convert.ToDateTime(findContentionsResponse.VEISfcontbenefitClaimInfo.mcs_claimRcvdDt);
                        }
                        if (request.UDOcreateUdoContentionsRelatedEntitiesInfo != null)
                        {
                            foreach (var relatedItem in request.UDOcreateUdoContentionsRelatedEntitiesInfo)
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
                    string logInfo = string.Format("Contention Records Created: {0}", requestCollection.Count());
                    LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Contention Records Created", logInfo);
                    #endregion
                }

                //added to generated code
                if (request.udo_claimId != System.Guid.Empty)
                {
                    var parent = new Entity();
                    parent.Id = request.udo_claimId;
                    parent.LogicalName = "udo_claim";
                    parent["udo_contentioncomplete"] = true;
                    OrgServiceProxy.Update(parent);
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, method + " Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to process claim data";
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