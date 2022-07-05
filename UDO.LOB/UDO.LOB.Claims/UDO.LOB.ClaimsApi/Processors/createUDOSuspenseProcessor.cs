using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;
using UDO.LOB.Claims.Messages;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using VEIS.Core.Messages;
using VEIS.Messages.BenefitClaimService;

namespace UDO.LOB.Claims.Processors
{
    class UDOcreateUDOSuspenseProcessor
    {
        private bool _debug { get; set; }

        private string LogBuffer { get; set; }

        private const string method = "UDOcreateUDOSuspenseProcessor";
        
        public IMessageBase Execute(UDOcreateUDOSuspenseRequest request)
        {
            UDOcreateUDOSuspenseResponse response = new UDOcreateUDOSuspenseResponse { MessageId = request.MessageId };
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
            TraceLogger aiLogger = new TraceLogger("UDOcreateUDOSuspenseProcessor.Execute", request);
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
                var findBenefitClaimDetailRequest = new VEISfbendtlfindBenefitClaimDetailRequest();
                findBenefitClaimDetailRequest.LogTiming = request.LogTiming;
                findBenefitClaimDetailRequest.LogSoap = request.LogSoap;
                findBenefitClaimDetailRequest.Debug = request.Debug;
                findBenefitClaimDetailRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findBenefitClaimDetailRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findBenefitClaimDetailRequest.RelatedParentId = request.RelatedParentId;
                findBenefitClaimDetailRequest.UserId = request.UserId;
                findBenefitClaimDetailRequest.OrganizationName = request.OrganizationName;

                findBenefitClaimDetailRequest.mcs_benefitclaimid = request.benefitClaimId;

                findBenefitClaimDetailRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                {
                    ///Header Info
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber,
                };

                // REM: Invoke VEIS Endpoint
                var findBenefitClaimDetailResponse = WebApiUtility.SendReceive<VEISfbendtlfindBenefitClaimDetailResponse>(findBenefitClaimDetailRequest, WebApiType.VEIS);
                progressString = "After VEIS EC Call";

                if (request.LogSoap || findBenefitClaimDetailResponse.ExceptionOccurred)
                {
                    if (findBenefitClaimDetailResponse.SerializedSOAPRequest != null || findBenefitClaimDetailResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findBenefitClaimDetailResponse.SerializedSOAPRequest + findBenefitClaimDetailResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfbendtlfindBenefitClaimDetailRequest Request/Response {requestResponse}", true);
                    }
                }

                response.ExceptionMessage = findBenefitClaimDetailResponse.ExceptionMessage;
                response.ExceptionOccured = findBenefitClaimDetailResponse.ExceptionOccurred;
                // Replaced: VIMTfbendtlbenefitClaimRecordbclmInfo = VEISfbendtlfreturnInfo
                if (findBenefitClaimDetailResponse.VEISfbendtlfreturnInfo != null)
                {
                    // Replaced: VIMTfbendtlsuspenceRecordbclmInfo = VEISfbendtlfsuspenceRecordInfo
                    if (findBenefitClaimDetailResponse.VEISfbendtlfreturnInfo.VEISfbendtlfsuspenceRecordInfo != null)
                    {
                        if (findBenefitClaimDetailResponse.VEISfbendtlfreturnInfo.VEISfbendtlfsuspenceRecordInfo.VEISfbendtlfsuspenceRecordsInfo != null)
                        {
                            var suspenseRecords = findBenefitClaimDetailResponse.VEISfbendtlfreturnInfo.VEISfbendtlfsuspenceRecordInfo.VEISfbendtlfsuspenceRecordsInfo;
                            var requestCollection = new OrganizationRequestCollection();
                            var count = 0;
                            foreach (var suspenseRecord in suspenseRecords)
                            {
                                count = ++count;
                                //instantiate the new Entity
                                Entity thisNewEntity = new Entity();
                                thisNewEntity.LogicalName = "udo_claimsuspense";
                                if (request.ownerId != System.Guid.Empty)
                                {
                                    thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                }
                                if (!string.IsNullOrEmpty(suspenseRecord.mcs_suspenceReasonText))
                                {
                                    thisNewEntity["udo_suspensereason"] = suspenseRecord.mcs_suspenceReasonText;
                                }
                                if (!string.IsNullOrEmpty(suspenseRecord.mcs_claimSuspenceDate))
                                {
                                    var newDate = new System.DateTime();
                                    if (DateTime.TryParse(suspenseRecord.mcs_claimSuspenceDate, out newDate))
                                    {
                                        thisNewEntity["udo_suspensedate"] = newDate;
                                    }
                                }
                                if (!string.IsNullOrEmpty(suspenseRecord.mcs_suspenceActionDate))
                                {
                                    var newDate = new System.DateTime();
                                    if (DateTime.TryParse(suspenseRecord.mcs_suspenceActionDate, out newDate))
                                    {
                                        thisNewEntity["udo_actioncompletedon"] = newDate;
                                    }
                                }
                                if (!string.IsNullOrEmpty(suspenseRecord.mcs_firstName) && !string.IsNullOrEmpty(suspenseRecord.mcs_lastName) && !string.IsNullOrEmpty(suspenseRecord.mcs_journalDate))
                                {
                                    thisNewEntity["udo_updatedby"] = suspenseRecord.mcs_lastName + ", " + suspenseRecord.mcs_firstName + " - " + suspenseRecord.mcs_journalDate;
                                }
                                if (request.UDOcreateUDOSuspenseRelatedEntitiesInfo != null)
                                {
                                    foreach (var relatedItem in request.UDOcreateUDOSuspenseRelatedEntitiesInfo)
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

                            if (requestCollection.Count > 0)
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

                            string logInfo = string.Format("Claim Suspense Records Created: {0}", requestCollection.Count);
                            LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Claim Suspense Records Created", logInfo);
                            #endregion
                        }
                    }
                }

                //added to generated code
                if (request.udo_claimId != System.Guid.Empty)
                {
                    var parent = new Entity();
                    parent.Id = request.udo_claimId;
                    parent.LogicalName = "udo_claim";
                    parent["udo_suspensecomplete"] = true;
                    OrgServiceProxy.Update(parent);
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, method + " Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Claim Suspense Data"; 
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