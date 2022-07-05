using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using UDO.LOB.Claims.Messages;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using VEIS.Core.Messages;
using VEIS.Messages.BenefitClaimService;
using VEIS.Messages.EBenefitsBnftClaimStatusService;

namespace UDO.LOB.Claims.Processors
{
    class UpdateUDOClaimsProcessor
    {
        private bool _debug { get; set; }

        private string LogBuffer { get; set; }

        private const string method = "UpdateUDOClaimsProcessor";
        
        public IMessageBase Execute(UDOUpdateUDOClaimsRequest request)
        {
            UDOUpdateUDOClaimsResponse response = new UDOUpdateUDOClaimsResponse { MessageId = request.MessageId };
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
            TraceLogger aiLogger = new TraceLogger("UpdateUDOClaimsProcessor.Execute", request);
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
                Entity thisEntity = new Entity();
                thisEntity.Id = request.udo_ClaimID;
                thisEntity.LogicalName = "udo_claim";

                var findBenefitClaimDetailsByBnftClaimIdRequest = new VEISfindBCDbyBCid_findBenefitClaimDetailsByBnftClaimIdRequest();
                findBenefitClaimDetailsByBnftClaimIdRequest.MessageId = request.MessageId;
                findBenefitClaimDetailsByBnftClaimIdRequest.LogTiming = request.LogTiming;
                findBenefitClaimDetailsByBnftClaimIdRequest.LogSoap = request.LogSoap;
                findBenefitClaimDetailsByBnftClaimIdRequest.Debug = request.Debug;
                findBenefitClaimDetailsByBnftClaimIdRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findBenefitClaimDetailsByBnftClaimIdRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findBenefitClaimDetailsByBnftClaimIdRequest.RelatedParentId = request.RelatedParentId;
                findBenefitClaimDetailsByBnftClaimIdRequest.UserId = request.UserId;
                findBenefitClaimDetailsByBnftClaimIdRequest.OrganizationName = request.OrganizationName;
                findBenefitClaimDetailsByBnftClaimIdRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo()
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,

                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };

                if (request.udo_benefitClaimID != string.Empty)
                {
                    if (Int64.TryParse(request.udo_benefitClaimID, out long newLong))
                        findBenefitClaimDetailsByBnftClaimIdRequest.mcs_bnftclaimid = newLong;
                }

                // REM: Invoke VEIS Endpoint
                var findBenefitClaimDetailsByBnftClaimIdResponse = WebApiUtility.SendReceive<VEISfindBCDbyBCid_findBenefitClaimDetailsByBnftClaimIdResponse>(findBenefitClaimDetailsByBnftClaimIdRequest, WebApiType.VEIS);
                progressString = "After VEIS EC Call for findBenefitClaimDetails...";

                if (request.LogSoap || findBenefitClaimDetailsByBnftClaimIdResponse.ExceptionOccurred)
                {
                    if (findBenefitClaimDetailsByBnftClaimIdResponse.SerializedSOAPRequest != null || findBenefitClaimDetailsByBnftClaimIdResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findBenefitClaimDetailsByBnftClaimIdResponse.SerializedSOAPRequest + findBenefitClaimDetailsByBnftClaimIdResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfindBCDbyBCid_findBenefitClaimDetailsByBnftClaimIdRequest Request/Response {requestResponse}", true);
                    }
                }
                response.ExceptionMessage = findBenefitClaimDetailsByBnftClaimIdResponse.ExceptionMessage;
                response.ExceptionOccured = findBenefitClaimDetailsByBnftClaimIdResponse.ExceptionOccurred;
                #region findBenefitClaimDetailsbyBnftClaimId
                if (findBenefitClaimDetailsByBnftClaimIdResponse.VEISfindBCDbyBCid_benefitClaimDetailsDTOInfo != null && findBenefitClaimDetailsByBnftClaimIdResponse.VEISfindBCDbyBCid_benefitClaimDetailsDTOInfo.VEISfindBCDbyBCid_bnftClaimLcStatusInfo != null)
                {
                    var bnftClaimLcStatus = findBenefitClaimDetailsByBnftClaimIdResponse.VEISfindBCDbyBCid_benefitClaimDetailsDTOInfo.VEISfindBCDbyBCid_bnftClaimLcStatusInfo;

                    //Updated to reference phase change datetime rather than phase name
                    var mostRecentPhaseType = "";
                    DateTime mostRecentChngDate = new DateTime(1);

                    foreach (var bnftClaimLcStatusItem in bnftClaimLcStatus)
                    {
                        if (!string.IsNullOrEmpty(bnftClaimLcStatusItem.mcs_phaseType))
                        {
                            DateTime phaseChgDate = DateTime.Parse(bnftClaimLcStatusItem.mcs_phaseChngdDt);
                            if(DateTime.Compare(mostRecentChngDate, phaseChgDate) < 0)
                            {
                                mostRecentPhaseType = bnftClaimLcStatusItem.mcs_phaseType;
                                mostRecentChngDate = phaseChgDate;
                            }
                        }
                    }
                    thisEntity["udo_phasetype"] = mostRecentPhaseType;

                    if (!string.IsNullOrEmpty(findBenefitClaimDetailsByBnftClaimIdResponse.VEISfindBCDbyBCid_benefitClaimDetailsDTOInfo.mcs_minEstClaimCompleteDt))
                    {
                        DateTime newDateTime;
                        if (DateTime.TryParse(findBenefitClaimDetailsByBnftClaimIdResponse.VEISfindBCDbyBCid_benefitClaimDetailsDTOInfo.mcs_minEstClaimCompleteDt, out newDateTime))
                        {
                            thisEntity["udo_minestclaimcompletedt"] = newDateTime;
                        }
                    }
                    if (!string.IsNullOrEmpty(findBenefitClaimDetailsByBnftClaimIdResponse.VEISfindBCDbyBCid_benefitClaimDetailsDTOInfo.mcs_maxEstClaimCompleteDt))
                    {
                        DateTime newDateTime;
                        if (DateTime.TryParse(findBenefitClaimDetailsByBnftClaimIdResponse.VEISfindBCDbyBCid_benefitClaimDetailsDTOInfo.mcs_maxEstClaimCompleteDt, out newDateTime))
                        {
                            thisEntity["udo_maxestclaimcompletedt"] = newDateTime;
                        }
                    }

                    thisEntity["udo_benefitstatuscomplete"] = true;
                }

                #endregion findBenefitClaimDetailsbyBnftClaimId

                var findBenefitClaimDetailsByClaimIdRequest = new VEISfbendtlfindBenefitClaimDetailRequest();
                findBenefitClaimDetailsByClaimIdRequest.LogTiming = request.LogTiming;
                findBenefitClaimDetailsByClaimIdRequest.LogSoap = request.LogSoap;
                findBenefitClaimDetailsByClaimIdRequest.Debug = request.Debug;
                findBenefitClaimDetailsByClaimIdRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findBenefitClaimDetailsByClaimIdRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findBenefitClaimDetailsByClaimIdRequest.RelatedParentId = request.RelatedParentId;
                findBenefitClaimDetailsByClaimIdRequest.UserId = request.UserId;
                findBenefitClaimDetailsByClaimIdRequest.OrganizationName = request.OrganizationName;
                findBenefitClaimDetailsByClaimIdRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo()
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,

                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };

                if (request.udo_benefitClaimID != string.Empty)
                {
                    findBenefitClaimDetailsByClaimIdRequest.mcs_benefitclaimid = request.udo_benefitClaimID;
                }

                var findBenefitClaimDetailsByClaimIdResponse = WebApiUtility.SendReceive<VEISfbendtlfindBenefitClaimDetailResponse>(findBenefitClaimDetailsByClaimIdRequest, WebApiType.VEIS);
                progressString = "After VEIS EC Call for findBenefitClaimDetails...";

                if (request.LogSoap || findBenefitClaimDetailsByClaimIdResponse.ExceptionOccurred)
                {
                    if (findBenefitClaimDetailsByClaimIdResponse.SerializedSOAPRequest != null || findBenefitClaimDetailsByClaimIdResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findBenefitClaimDetailsByClaimIdResponse.SerializedSOAPRequest + findBenefitClaimDetailsByClaimIdResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfbendtlfindBenefitClaimDetailRequest Request/Response {requestResponse}", true);
                    }
                }

                response.ExceptionMessage = findBenefitClaimDetailsByBnftClaimIdResponse.ExceptionMessage;
                response.ExceptionOccured = findBenefitClaimDetailsByBnftClaimIdResponse.ExceptionOccurred;

                // Replaced: VIMTfbendtlbenefitClaimRecordbclmInfo = VEISfbendtlfreturnInfo
                if (findBenefitClaimDetailsByClaimIdResponse.VEISfbendtlfreturnInfo != null)
                {
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "updateUDOClaimsProcessor ", "Got BFitClaimResponse");

                    if (findBenefitClaimDetailsByClaimIdResponse.VEISfbendtlfreturnInfo != null)
                    {

                        // Replaced: VEISfbendtlbenefitClaimRecord1bclmInfo = 
                        if (!string.IsNullOrEmpty(findBenefitClaimDetailsByClaimIdResponse.VEISfbendtlfreturnInfo.VEISfbendtlfbenefitClaimRecord1Info.mcs_claimStationOfJurisdiction))
                        {
                            thisEntity["udo_claimstation"] = findBenefitClaimDetailsByClaimIdResponse.VEISfbendtlfreturnInfo.VEISfbendtlfbenefitClaimRecord1Info.mcs_claimStationOfJurisdiction;
                            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "updateUDOClaimsProcessor ", "Mapped ClaimStation");
                        }
                        if (!string.IsNullOrEmpty(findBenefitClaimDetailsByClaimIdResponse.VEISfbendtlfreturnInfo.VEISfbendtlfbenefitClaimRecord1Info.mcs_participantClaimantID))
                        {
                            thisEntity["udo_participantid"] = findBenefitClaimDetailsByClaimIdResponse.VEISfbendtlfreturnInfo.VEISfbendtlfbenefitClaimRecord1Info.mcs_participantClaimantID;
                            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "updateUDOClaimsProcessor ", "Mapped PID for Claimant");
                        }
                    }
                }

                if (request.udo_ClaimID != System.Guid.Empty)
                {
                    thisEntity.Id = request.udo_ClaimID;
                    thisEntity.LogicalName = "udo_claim";
                    OrgServiceProxy.Update(thisEntity);
                }

                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, method + " Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = ExecutionException.Message;
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