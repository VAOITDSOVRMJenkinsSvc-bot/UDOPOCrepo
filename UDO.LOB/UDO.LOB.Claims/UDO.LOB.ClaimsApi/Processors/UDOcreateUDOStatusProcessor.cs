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
using VEIS.Messages.ClaimManagementService;

/// <summary>
/// VIMT LOB Component for UDOcreateUDOStatus,createUDOStatus method, Processor.
/// </summary>
namespace UDO.LOB.Claims.Processors
{
    class UDOcreateUDOStatusProcessor
    {
        private bool _debug { get; set; }

        private string LogBuffer { get; set; }

        private const string method = "UDOcreateUDOStatusProcessor";
        
        public IMessageBase Execute(UDOcreateUDOStatusRequest request)
        {
            UDOcreateUDOStatusResponse response = new UDOcreateUDOStatusResponse { MessageId = request.MessageId };
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
            TraceLogger aiLogger = new TraceLogger("UDOcreateUDOStatusProcessor.Execute", request);
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
                var findClaimStatusRequest = new VEISfndClmStatfindClaimStatusRequest();
                findClaimStatusRequest.LogTiming = request.LogTiming;
                findClaimStatusRequest.LogSoap = request.LogSoap;
                findClaimStatusRequest.Debug = request.Debug;
                findClaimStatusRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findClaimStatusRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findClaimStatusRequest.RelatedParentId = request.RelatedParentId;
                findClaimStatusRequest.UserId = request.UserId;
                findClaimStatusRequest.OrganizationName = request.OrganizationName;

                findClaimStatusRequest.mcs_claimid = request.claimId;

                findClaimStatusRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber,
                };

                var findClaimStatusResponse = WebApiUtility.SendReceive<VEISfndClmStatfindClaimStatusResponse>(findClaimStatusRequest, WebApiType.VEIS);

                if (request.LogSoap || findClaimStatusResponse.ExceptionOccurred)
                {
                    if (findClaimStatusResponse.SerializedSOAPRequest != null || findClaimStatusResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findClaimStatusResponse.SerializedSOAPRequest + findClaimStatusResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfndClmStatfindClaimStatusRequest Request/Response {requestResponse}", true);
                    }
                }

                progressString = "After VEIS EC Call";

                response.ExceptionMessage = findClaimStatusResponse.ExceptionMessage;
                response.ExceptionOccured = findClaimStatusResponse.ExceptionOccurred;
                
                if (findClaimStatusResponse.VEISfndClmStatClaimStatusclmMngmInfo !=null)
                {
                    if (findClaimStatusResponse.VEISfndClmStatClaimStatusclmMngmInfo.VEISfndClmStatclaimLifecycleStatusListclmMngmInfo != null)
                    {
                        var requestCollection = new OrganizationRequestCollection();
                        var count = 0;
                        var claimLifecycleStatus = findClaimStatusResponse.VEISfndClmStatClaimStatusclmMngmInfo.VEISfndClmStatclaimLifecycleStatusListclmMngmInfo;
                        foreach (var claimLifecycleStatusItem in claimLifecycleStatus)
                        {
                            count += 1;
                            var responseIds = new UDOcreateUDOStatusMultipleResponse();
                            //instantiate the new Entity
                            Entity thisNewEntity = new Entity();
                            thisNewEntity.LogicalName = "udo_claimstatus";
                            if (request.ownerId != System.Guid.Empty)
                            {
                                thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                            }
                            if (!string.IsNullOrEmpty(claimLifecycleStatusItem.mcs_jrnSttTc))
                            {
                                thisNewEntity["udo_status"] = claimLifecycleStatusItem.mcs_bnftClmLcSttTn;
                            }
                            if (!string.IsNullOrEmpty(claimLifecycleStatusItem.mcs_clmId))
                            {
                                thisNewEntity["udo_claimidstring"] = claimLifecycleStatusItem.mcs_clmId;
                            }
                            if (claimLifecycleStatusItem.mcs_chngdDtSpecified)
                            {
                                DateTime changedDate = DateTime.Parse(claimLifecycleStatusItem.mcs_chngdDt);
                                thisNewEntity["udo_changedate"] = changedDate;
                            }
                            if (!string.IsNullOrEmpty(claimLifecycleStatusItem.mcs_actionLctnId))
                            {
                                thisNewEntity["udo_actionlocation"] = claimLifecycleStatusItem.mcs_actionLctnId;
                            }
                            if (request.UDOcreateUDOStatusRelatedEntitiesInfo != null)
                            {
                                foreach (var relatedItem in request.UDOcreateUDOStatusRelatedEntitiesInfo)
                                {
                                    thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                }
                            }
                            CreateRequest createSuspenseRecord = new CreateRequest
                            {
                                Target = thisNewEntity
                            };
                            requestCollection.Add(createSuspenseRecord); 
                            
                        }
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

                        string logInfo = string.Format("Claim Status Records Created: {0}", requestCollection.Count());
                        LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Claim Status Records Created", logInfo);
                        #endregion

                    }
                }
                //added to generated code
                if (request.udo_claimId != System.Guid.Empty)
                {
                    var parent = new Entity();
                    parent.Id = request.udo_claimId;
                    parent.LogicalName = "udo_claim";
                    parent["udo_statuscomplete"] = true;
                    OrgServiceProxy.Update(parent);
                }
                return response;
            }
            catch (Exception ExecutionException)
            {

                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, method + " Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Claim Status Data " + ExecutionException.Message; 
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