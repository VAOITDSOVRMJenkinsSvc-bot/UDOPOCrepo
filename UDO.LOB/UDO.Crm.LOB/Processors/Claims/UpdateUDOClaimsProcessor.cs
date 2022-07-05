using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using VIMT.BenefitClaimService.Messages;
using VIMT.EBenefitsBnftClaimStatusWebService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Claims.Messages;
using VRM.Integration.UDO.Common;
using System.Linq;

namespace VRM.Integration.UDO.Claims.Processors
{
    class UpdateUDOClaimsProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UpdateUDOClaimsProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOUpdateUDOClaimsRequest request)
        {
            //var request = message as createUDOClaimsRequest;
            UDOUpdateUDOClaimsResponse response = new UDOUpdateUDOClaimsResponse();
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }

            OrganizationServiceProxy OrgServiceProxy;

            #region connect to CRM
            try
            {
                var CommonFunctions = new CRMConnect();

                OrgServiceProxy = CommonFunctions.ConnectToCrm(request.OrganizationName);

            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, string.Format("{0} Processor, Connection Error", method), connectException.Message);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

            progressString = "After Connection";

            try
            {
                var noData = true;

                Entity thisEntity = new Entity();
                thisEntity.Id = request.udo_ClaimID;
                thisEntity.LogicalName = "udo_claim";

                var findBenefitClaimDetailsByBnftClaimIdRequest = new VIMTfindBCDbyBCid_findBenefitClaimDetailsByBnftClaimIdRequest();
                findBenefitClaimDetailsByBnftClaimIdRequest.LogTiming = request.LogTiming;
                findBenefitClaimDetailsByBnftClaimIdRequest.LogSoap = request.LogSoap;
                findBenefitClaimDetailsByBnftClaimIdRequest.Debug = request.Debug;
                findBenefitClaimDetailsByBnftClaimIdRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findBenefitClaimDetailsByBnftClaimIdRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findBenefitClaimDetailsByBnftClaimIdRequest.RelatedParentId = request.RelatedParentId;
                findBenefitClaimDetailsByBnftClaimIdRequest.UserId = request.UserId;
                findBenefitClaimDetailsByBnftClaimIdRequest.OrganizationName = request.OrganizationName;
                findBenefitClaimDetailsByBnftClaimIdRequest.LegacyServiceHeaderInfo = new VIMT.EBenefitsBnftClaimStatusWebService.Messages.HeaderInfo()
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,

                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };

                if (request.udo_benefitClaimID != string.Empty)
                {
                    long newLong;
                    if (Int64.TryParse(request.udo_benefitClaimID, out newLong))
                        findBenefitClaimDetailsByBnftClaimIdRequest.mcs_bnftclaimid = newLong;
                }

                // TODO(TN): Comment o remediate
                var findBenefitClaimDetailsByBnftClaimIdResponse = new VIMTfindBCDbyBCid_findBenefitClaimDetailsByBnftClaimIdResponse();
                // var findBenefitClaimDetailsByBnftClaimIdResponse = findBenefitClaimDetailsByBnftClaimIdRequest.SendReceive<VIMTfindBCDbyBCid_findBenefitClaimDetailsByBnftClaimIdResponse>(MessageProcessType.Local);
                progressString = "After VIMT EC Call for findBenefitClaimDetails...";

                response.ExceptionMessage = findBenefitClaimDetailsByBnftClaimIdResponse.ExceptionMessage;
                response.ExceptionOccured = findBenefitClaimDetailsByBnftClaimIdResponse.ExceptionOccured;
                #region findBenefitClaimDetailsbyBnftClaimId
                //findBenefitClaimDetailsByBnftClaimIdResponse.VIMTfindBCDbyBCid_benefitClaimDetailsDTOInfo will be null if Exception occurred
                if (findBenefitClaimDetailsByBnftClaimIdResponse.VIMTfindBCDbyBCid_benefitClaimDetailsDTOInfo != null && findBenefitClaimDetailsByBnftClaimIdResponse.VIMTfindBCDbyBCid_benefitClaimDetailsDTOInfo.VIMTfindBCDbyBCid_bnftClaimLcStatusInfo != null)
                {
                    var bnftClaimLcStatus = findBenefitClaimDetailsByBnftClaimIdResponse.VIMTfindBCDbyBCid_benefitClaimDetailsDTOInfo.VIMTfindBCDbyBCid_bnftClaimLcStatusInfo;

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

                    if (!string.IsNullOrEmpty(findBenefitClaimDetailsByBnftClaimIdResponse.VIMTfindBCDbyBCid_benefitClaimDetailsDTOInfo.mcs_minEstClaimCompleteDt))
                    {
                        DateTime newDateTime;
                        if (DateTime.TryParse(findBenefitClaimDetailsByBnftClaimIdResponse.VIMTfindBCDbyBCid_benefitClaimDetailsDTOInfo.mcs_minEstClaimCompleteDt, out newDateTime))
                        {
                            thisEntity["udo_minestclaimcompletedt"] = newDateTime;
                        }
                    }
                    if (!string.IsNullOrEmpty(findBenefitClaimDetailsByBnftClaimIdResponse.VIMTfindBCDbyBCid_benefitClaimDetailsDTOInfo.mcs_maxEstClaimCompleteDt))
                    {
                        DateTime newDateTime;
                        if (DateTime.TryParse(findBenefitClaimDetailsByBnftClaimIdResponse.VIMTfindBCDbyBCid_benefitClaimDetailsDTOInfo.mcs_maxEstClaimCompleteDt, out newDateTime))
                        {
                            thisEntity["udo_maxestclaimcompletedt"] = newDateTime;
                        }
                    }

                    thisEntity["udo_benefitstatuscomplete"] = true;
                    noData = false;
                }

                #endregion findBenefitClaimDetailsbyBnftClaimId

                var findBenefitClaimDetailsByClaimIdRequest = new VIMTfbendtlfindBenefitClaimDetailRequest();
                findBenefitClaimDetailsByClaimIdRequest.LogTiming = request.LogTiming;
                findBenefitClaimDetailsByClaimIdRequest.LogSoap = request.LogSoap;
                findBenefitClaimDetailsByClaimIdRequest.Debug = request.Debug;
                findBenefitClaimDetailsByClaimIdRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findBenefitClaimDetailsByClaimIdRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findBenefitClaimDetailsByClaimIdRequest.RelatedParentId = request.RelatedParentId;
                findBenefitClaimDetailsByClaimIdRequest.UserId = request.UserId;
                findBenefitClaimDetailsByClaimIdRequest.OrganizationName = request.OrganizationName;
                findBenefitClaimDetailsByClaimIdRequest.LegacyServiceHeaderInfo = new VIMT.BenefitClaimService.Messages.HeaderInfo()
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

                // TODO(TN): Comment to remediate
                var findBenefitClaimDetailsByClaimIdResponse = new VIMTfbendtlfindBenefitClaimDetailResponse();
                // var findBenefitClaimDetailsByClaimIdResponse = findBenefitClaimDetailsByClaimIdRequest.SendReceive<VIMTfbendtlfindBenefitClaimDetailResponse>(MessageProcessType.Local);
                progressString = "After VIMT EC Call for findBenefitClaimDetails...";

                response.ExceptionMessage = findBenefitClaimDetailsByBnftClaimIdResponse.ExceptionMessage;
                response.ExceptionOccured = findBenefitClaimDetailsByBnftClaimIdResponse.ExceptionOccured;

                if (findBenefitClaimDetailsByClaimIdResponse.VIMTfbendtlbenefitClaimRecordbclmInfo != null)
                {
                    // LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "updateUDOClaimsProcessor ", "Got BFitClaimResponse");

                    if (findBenefitClaimDetailsByClaimIdResponse.VIMTfbendtlbenefitClaimRecordbclmInfo != null)
                    {

                        if (!string.IsNullOrEmpty(findBenefitClaimDetailsByClaimIdResponse.VIMTfbendtlbenefitClaimRecordbclmInfo.VIMTfbendtlbenefitClaimRecord1bclmInfo.mcs_claimStationOfJurisdiction))
                        {
                            thisEntity["udo_claimstation"] = findBenefitClaimDetailsByClaimIdResponse.VIMTfbendtlbenefitClaimRecordbclmInfo.VIMTfbendtlbenefitClaimRecord1bclmInfo.mcs_claimStationOfJurisdiction;
                            // LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "updateUDOClaimsProcessor ", "Mapped ClaimStation");
                        }
                        if (!string.IsNullOrEmpty(findBenefitClaimDetailsByClaimIdResponse.VIMTfbendtlbenefitClaimRecordbclmInfo.VIMTfbendtlbenefitClaimRecord1bclmInfo.mcs_participantClaimantID))
                        {
                            thisEntity["udo_participantid"] = findBenefitClaimDetailsByClaimIdResponse.VIMTfbendtlbenefitClaimRecordbclmInfo.VIMTfbendtlbenefitClaimRecord1bclmInfo.mcs_participantClaimantID;
                            //  LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "updateUDOClaimsProcessor ", "Mapped PID for Claimant");
                        }
                    }
                }

                if (request.udo_ClaimID != System.Guid.Empty)
                {
                    thisEntity.Id = request.udo_ClaimID;
                    thisEntity.LogicalName = "udo_claim";
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(thisEntity, request.OrganizationName, request.UserId, request.LogTiming));
                }

                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Award Line Data";
                response.ExceptionOccured = true;
                return response;
            }
        }

    }
}