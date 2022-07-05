using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VIMT.BenefitClaimService.Messages;
using VIMT.ClaimantWebService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Claims.Messages;
using VRM.Integration.UDO.Common;

namespace VRM.Integration.UDO.Claims.Processors
{
    class createUDOClaimsProcessor
    {
        private bool _debug { get; set; }
        private const string method = "createUDOClaimsProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOcreateUDOClaimsRequest request)
        {
            //var request = message as createUDOClaimsRequest;
            UDOcreateUDOClaimsResponse response = new UDOcreateUDOClaimsResponse();
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
            //var dataTruncation = new Truncate();

            #region connect to CRM
            try
            {
                var CommonFunctions = new CRMConnect();

                OrgServiceProxy = CommonFunctions.ConnectToCrm(request.OrganizationName);

            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateUDOClaimsProcessor Processor, Connection Error", connectException.Message);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

            progressString = "After Connection";

            // These are used throughout the claims processor for both building the veteran snapshot and updating the people list.
            VIMTfindBenefitClaimResponse findBenefitClaimResponse = null;
            var pendingClaims = 0;

            var requestCollection = new OrganizationRequestCollection();

            try
            {
                #region set idproof if empty
                if (request.idProofId == Guid.Empty && request.RelatedParentEntityName == "udo_idproof")
                {
                    request.idProofId = request.RelatedParentId;
                }
                else if (request.idProofId == Guid.Empty && request.UDOcreateUDOClaimsRelatedEntitiesInfo != null)
                {
                    foreach (var related in request.UDOcreateUDOClaimsRelatedEntitiesInfo)
                    {
                        if (related.RelatedEntityName.ToLower() == "udo_idproof")
                        {
                            request.idProofId = related.RelatedEntityId;
                        }
                    }
                }
                #endregion


                if (request.fileNumber != null)
                {
                    #region main claim calls

                    // prefix = findBenefitClaimRequest();
                    var findBenefitClaimRequest = new VIMTfindBenefitClaimRequest();
                    findBenefitClaimRequest.Debug = request.Debug;
                    findBenefitClaimRequest.LogSoap = request.LogSoap;
                    findBenefitClaimRequest.LogTiming = request.LogTiming;
                    findBenefitClaimRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                    findBenefitClaimRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                    findBenefitClaimRequest.RelatedParentId = request.RelatedParentId;
                    findBenefitClaimRequest.UserId = request.UserId;
                    findBenefitClaimRequest.OrganizationName = request.OrganizationName;
                    findBenefitClaimRequest.LegacyServiceHeaderInfo = new VIMT.BenefitClaimService.Messages.HeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,

                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };

                    //non standard fieldsMessageProcessType.Local
                    findBenefitClaimRequest.mcs_filenumber = request.fileNumber;
                    //LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateClaimsProcessor Processor", "About to call findBenefitClaimRequest, request.fileNumber:" + request.fileNumber);
                    // TODO(TN): Comment to remediate
                    findBenefitClaimResponse = new VIMTfindBenefitClaimResponse();
                    // findBenefitClaimResponse = findBenefitClaimRequest.SendReceive<VIMTfindBenefitClaimResponse>(MessageProcessType.Local);
                    //LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateClaimsProcessor Processor", "After findBenefitClaimRequest");
                    progressString = "After VIMT EC for findBenefitClaim Call";

                    response.ExceptionMessage = findBenefitClaimResponse.ExceptionMessage;
                    response.ExceptionOccured = findBenefitClaimResponse.ExceptionOccured;

                    response.VIMTfindBenefitClaimRequestData = findBenefitClaimResponse;
                    #endregion

                    System.Collections.Generic.List<UDOcreateUDOClaimsMultipleResponse> UDOcreateUDOClaimsArray = new System.Collections.Generic.List<UDOcreateUDOClaimsMultipleResponse>();

                    if (findBenefitClaimResponse.VIMTbenefitClaimRecordbclmInfo != null)
                    {
                        //LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateClaimsProcessor Processor", "findBenefitClaimResponse.VIMTbenefitClaimRecordbclmInfo != null");
                        if (findBenefitClaimResponse.VIMTbenefitClaimRecordbclmInfo.VIMTparticipantRecordbclmInfo != null)
                        {
                            //LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateClaimsProcessor Processor", "findBenefitClaimResponse.VIMTbenefitClaimRecordbclmInfo.VIMTparticipantRecordbclmInfo != null");

                            #region VIMTparticipantRecordbclmInfo != null
                            if (findBenefitClaimResponse.VIMTbenefitClaimRecordbclmInfo.VIMTparticipantRecordbclmInfo.mcs_numberOfRecords != null)
                            {
                                if (findBenefitClaimResponse.VIMTbenefitClaimRecordbclmInfo.VIMTparticipantRecordbclmInfo.mcs_numberOfRecords == "001")
                                {
                                    //  LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateClaimsProcessor Processor", "Mapping Single Claim");
                                    var participantRecord = findBenefitClaimResponse.VIMTbenefitClaimRecordbclmInfo.VIMTparticipantRecordbclmInfo;
                                    #region Map single Claim
                                    var responseIds = new UDOcreateUDOClaimsMultipleResponse();
                                    //instantiate the new Entity
                                    Entity thisNewEntity = new Entity();
                                    thisNewEntity.LogicalName = "udo_claim";
                                    // thisNewEntity["udo_name"] = "Claim Summary";
                                    if (request.ownerId != System.Guid.Empty)
                                    {
                                        thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                    }
                                    if (participantRecord.mcs_programTypeCode != string.Empty)
                                    {
                                        thisNewEntity["udo_programtypecode"] = participantRecord.mcs_programTypeCode;
                                    }
                                    if (findBenefitClaimResponse.VIMTbenefitClaimRecordbclmInfo.VIMTparticipantRecordbclmInfo.mcs_claimantPersonOrOrganizationIndicator != string.Empty)
                                    {
                                        thisNewEntity["udo_personorgindicator"] = findBenefitClaimResponse.VIMTbenefitClaimRecordbclmInfo.VIMTparticipantRecordbclmInfo.mcs_claimantPersonOrOrganizationIndicator;
                                    }
                                    if (participantRecord.mcs_payeeTypeCode != string.Empty)
                                    {
                                        thisNewEntity["udo_payeetypecode"] = participantRecord.mcs_payeeTypeCode;
                                    }

                                    if (participantRecord.mcs_organizationTitleTypeName != string.Empty)
                                    {
                                        thisNewEntity["udo_organizationpersontitle"] = participantRecord.mcs_organizationTitleTypeName;
                                    }
                                    if (participantRecord.mcs_organizationName != string.Empty)
                                    {
                                        thisNewEntity["udo_organizationname"] = participantRecord.mcs_organizationName;
                                    }
                                    if (participantRecord.mcs_claimReceiveDate != string.Empty)
                                    {
                                        DateTime newDateTime;
                                        if (DateTime.TryParse(participantRecord.mcs_claimReceiveDate, out newDateTime))
                                        {
                                            thisNewEntity["udo_lastactiondate"] = newDateTime;
                                        }
                                    }
                                    if (participantRecord.mcs_endProductTypeCode != string.Empty)
                                    {
                                        thisNewEntity["udo_epc"] = participantRecord.mcs_endProductTypeCode;
                                    }
                                    if (participantRecord.mcs_claimReceiveDate != string.Empty)
                                    {
                                        DateTime newDateTime;
                                        if (DateTime.TryParse(participantRecord.mcs_claimReceiveDate, out newDateTime))
                                        {
                                            thisNewEntity["udo_dateofclaim"] = newDateTime;
                                        }
                                    }
                                    if (participantRecord.mcs_claimTypeCode != string.Empty)
                                    {
                                        thisNewEntity["udo_claimtypecode"] = participantRecord.mcs_claimTypeCode;
                                    }
                                    if (participantRecord.mcs_statusTypeCode != string.Empty)
                                    {
                                        thisNewEntity["udo_claimstatus"] = participantRecord.mcs_statusTypeCode;
                                        if (IsPendingClaim(participantRecord)) pendingClaims++;
                                    }
                                    if (findBenefitClaimResponse.VIMTbenefitClaimRecordbclmInfo.VIMTparticipantRecordbclmInfo.mcs_claimStationOfJurisdiction != string.Empty)
                                    {
                                        thisNewEntity["udo_claimstation"] = findBenefitClaimResponse.VIMTbenefitClaimRecordbclmInfo.VIMTparticipantRecordbclmInfo.mcs_claimStationOfJurisdiction;
                                    }
                                    if (participantRecord.mcs_benefitClaimID != string.Empty)
                                    {
                                        int newInteger;
                                        if (Int32.TryParse(participantRecord.mcs_benefitClaimID, out newInteger))
                                        {
                                            thisNewEntity["udo_claimidentifier"] = newInteger;
                                            long newLong = Convert.ToInt64(newInteger);
                                            responseIds.newUDOcreateUDOClaimsIdentifier = newLong;
                                        }
                                        thisNewEntity["udo_claimidstring"] = participantRecord.mcs_benefitClaimID;
                                    }
                                    if (participantRecord.mcs_claimantLastName != string.Empty)
                                    {
                                        thisNewEntity["udo_claimantlastname"] = participantRecord.mcs_claimantLastName;
                                    }
                                    if (participantRecord.mcs_claimantFirstName != string.Empty)
                                    {
                                        thisNewEntity["udo_claimantfirstname"] = participantRecord.mcs_claimantFirstName;
                                    }
                                    if (!string.IsNullOrEmpty(participantRecord.mcs_claimantMiddleName))
                                    {
                                        thisNewEntity["udo_claimantmiddlename"] = participantRecord.mcs_claimantMiddleName;
                                    }
                                    if (!string.IsNullOrEmpty(participantRecord.mcs_claimantSuffix))
                                    {
                                        thisNewEntity["udo_claimantsuffix"] = participantRecord.mcs_claimantSuffix;
                                    }
                                    if (participantRecord.mcs_claimTypeName != string.Empty)
                                    {
                                        thisNewEntity["udo_claimdescription"] = participantRecord.mcs_claimTypeName;
                                    }


                                    if (participantRecord.mcs_claimReceiveDate != string.Empty)
                                    {
                                        DateTime newDateTime;
                                        if (DateTime.TryParse(participantRecord.mcs_claimReceiveDate, out newDateTime))
                                        {
                                            var daysDiff = DateTime.Today - newDateTime;
                                            thisNewEntity["udo_dayssinceinception"] = daysDiff.Days.ToString();
                                        }
                                    }
                                    if (participantRecord.mcs_claimReceiveDate != string.Empty)
                                    {
                                        DateTime newDateTime;
                                        if (DateTime.TryParse(participantRecord.mcs_claimReceiveDate, out newDateTime))
                                        {
                                            var daysDiff = DateTime.Today - newDateTime;
                                            thisNewEntity["udo_dayspending"] = daysDiff.Days.ToString();
                                        }
                                    }
                                    if (request.UDOcreateUDOClaimsRelatedEntitiesInfo != null)
                                    {
                                        foreach (var relatedItem in request.UDOcreateUDOClaimsRelatedEntitiesInfo)
                                        {
                                            thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                        }
                                    }

                                    var claimId = OrgServiceProxy.Create(TruncateHelper.TruncateFields(thisNewEntity, request.OrganizationName, request.UserId, request.LogTiming));
                                    responseIds.newUDOcreateUDOClaimsId = claimId;
                                    UDOcreateUDOClaimsArray.Add(responseIds);
                                    #endregion
                                    //  LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateUDOClaimsProcessor Processor, Progess:" + progressString, "1 record found and created");

                                    #region Create Lifecycles for a single Claim

                                    #region Set Veteran Reference
                                    var veteranReference = new EntityReference();
                                    foreach (var reference in request.UDOcreateUDOClaimsRelatedEntitiesInfo)
                                    {
                                        if (reference.RelatedEntityName == "contact")
                                        {
                                            veteranReference.LogicalName = reference.RelatedEntityName;
                                            veteranReference.Id = reference.RelatedEntityId;
                                        }
                                    }
                                    #endregion
                                    //RC - eric - while you could do it here, you are delaying the creation of all claims.  I would move this down below just like you have evidence and other things
                                    //so you loop through the ID"s you need, but after claims are created.  The reason is because the claims is the first grid the user will see.
                                    if (findBenefitClaimResponse.VIMTbenefitClaimRecordbclmInfo.VIMTlifeCycleRecordbclmInfo.VIMTlifeCycleRecordsbclmInfo != null)
                                    {
                                        //since this only works for the 1 claim scenario, we don't need the where stuff
                                        //var shrinqbcLifeCycle = findBenefitClaimResponse.VIMTbenefitClaimRecordbclmInfo.VIMTlifeCycleRecordbclmInfo.VIMTlifeCycleRecordsbclmInfo.Where(lc => lc.mcs_benefitClaimID == claim.newUDOcreateUDOClaimsIdentifier.ToString());
                                        var shrinqbcLifeCycle = findBenefitClaimResponse.VIMTbenefitClaimRecordbclmInfo.VIMTlifeCycleRecordbclmInfo.VIMTlifeCycleRecordsbclmInfo;

                                        if (shrinqbcLifeCycle != null)
                                        {
                                            foreach (var shrinqbcLifeCycleItem in shrinqbcLifeCycle)
                                            {
                                                var thisLifecycle = new Entity { LogicalName = "udo_lifecycle" };

                                                var claimReference = new EntityReference
                                                {
                                                    LogicalName = "udo_claim",
                                                    Id = responseIds.newUDOcreateUDOClaimsId
                                                };

                                                thisLifecycle["udo_claimid"] = claimReference;

                                                if (veteranReference != null)
                                                {
                                                    thisLifecycle["udo_veteran"] = veteranReference;
                                                }

                                                if (shrinqbcLifeCycleItem.mcs_lifeCycleStatusTypeName != string.Empty)
                                                {
                                                    thisLifecycle["udo_status"] = shrinqbcLifeCycleItem.mcs_lifeCycleStatusTypeName;
                                                }
                                                if (shrinqbcLifeCycleItem.mcs_stationofJurisdiction != string.Empty)
                                                {
                                                    thisLifecycle["udo_claimstation"] = shrinqbcLifeCycleItem.mcs_stationofJurisdiction;
                                                }
                                                if (shrinqbcLifeCycleItem.mcs_changedDate != string.Empty)
                                                {
                                                    DateTime newDateTime;
                                                    if (DateTime.TryParse(shrinqbcLifeCycleItem.mcs_changedDate, out newDateTime))
                                                    {
                                                        thisLifecycle["udo_changedate"] = newDateTime;
                                                    }
                                                }
                                                if (shrinqbcLifeCycleItem.mcs_actionStationNumber != string.Empty)
                                                {
                                                    thisLifecycle["udo_actionstation"] = shrinqbcLifeCycleItem.mcs_actionStationNumber;
                                                }
                                                if (shrinqbcLifeCycleItem.mcs_actionFirstName != string.Empty)
                                                {
                                                    thisLifecycle["udo_actionperson"] = shrinqbcLifeCycleItem.mcs_actionLastName + " " + shrinqbcLifeCycleItem.mcs_actionFirstName + " , " + shrinqbcLifeCycleItem.mcs_actionMiddleName;
                                                }
                                                if (shrinqbcLifeCycleItem.mcs_statusReasonTypeName != string.Empty)
                                                {
                                                    thisLifecycle["udo_pcanpclrreason"] = shrinqbcLifeCycleItem.mcs_statusReasonTypeName;
                                                }
                                                if (shrinqbcLifeCycleItem.mcs_benefitClaimID != string.Empty &&
                                            shrinqbcLifeCycleItem.mcs_lifeCycleStatusTypeName != string.Empty)
                                                {
                                                    thisLifecycle["udo_lifecycle"] = shrinqbcLifeCycleItem.mcs_benefitClaimID + " - " +
                                                                                     shrinqbcLifeCycleItem.mcs_lifeCycleStatusTypeName;
                                                }
                                                if (shrinqbcLifeCycleItem.mcs_reasonText != string.Empty)
                                                {
                                                    thisLifecycle["udo_explanation"] = shrinqbcLifeCycleItem.mcs_reasonText;
                                                }

                                                OrgServiceProxy.Create(TruncateHelper.TruncateFields(thisLifecycle, request.OrganizationName, request.UserId, request.LogTiming));

                                                var parent = new Entity();
                                                parent.Id = claimId;
                                                parent.LogicalName = "udo_claim";
                                                parent["udo_lifecyclecomplete"] = true;
                                                //OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming));
                                                UpdateRequest updateData = new UpdateRequest
                                                {
                                                    Target = parent
                                                };
                                                requestCollection.Add(updateData);

                                            }
                                            #region Update records

                                            if (requestCollection.Count() > 0)
                                            {
                                                var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, request.UserId, request.Debug);

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

                                            string logInfo = string.Format("Claim Records Updated: {0}", requestCollection.Count());
                                            LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Claim Records Updated", logInfo);
                                            #endregion
                                        }
                                    }
                                    #endregion
                                    progressString = "after lifecycle creation";
                                }
                            }
                            requestCollection.Clear();

                            if (findBenefitClaimResponse.VIMTbenefitClaimRecordbclmInfo.VIMTparticipantRecordbclmInfo.VIMTselectionbclmInfo != null)
                            {
                                var shrinqbcSelection = findBenefitClaimResponse.VIMTbenefitClaimRecordbclmInfo.VIMTparticipantRecordbclmInfo.VIMTselectionbclmInfo;
                                // LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateClaimsProcessor Processor", "Mapping Multiple Claim");
                                foreach (var shrinqbcSelectionItem in shrinqbcSelection)
                                {
                                    #region Map multiple Claims
                                    var responseIds = new UDOcreateUDOClaimsMultipleResponse();
                                    //instantiate the new Entity
                                    Entity thisNewEntity = new Entity();
                                    //  thisNewEntity["udo_name"] = "Claim Summary";
                                    thisNewEntity.LogicalName = "udo_claim";
                                    if (request.ownerId != System.Guid.Empty)
                                    {
                                        thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                    }

                                    if (shrinqbcSelectionItem.mcs_programTypeCode != string.Empty)
                                    {
                                        thisNewEntity["udo_programtypecode"] = shrinqbcSelectionItem.mcs_programTypeCode;
                                    }
                                    if (!string.IsNullOrEmpty(shrinqbcSelectionItem.mcs_personOrOrganizationIndicator))
                                    {
                                        thisNewEntity["udo_personorgindicator"] = shrinqbcSelectionItem.mcs_personOrOrganizationIndicator;
                                    }
                                    if (shrinqbcSelectionItem.mcs_payeeTypeCode != string.Empty)
                                    {
                                        thisNewEntity["udo_payeetypecode"] = shrinqbcSelectionItem.mcs_payeeTypeCode;
                                    }

                                    if (shrinqbcSelectionItem.mcs_organizationTitleTypeName != string.Empty)
                                    {
                                        thisNewEntity["udo_organizationpersontitle"] = shrinqbcSelectionItem.mcs_organizationTitleTypeName;
                                    }
                                    if (shrinqbcSelectionItem.mcs_organizationName != string.Empty)
                                    {
                                        thisNewEntity["udo_organizationname"] = shrinqbcSelectionItem.mcs_organizationName;
                                    }
                                    if (shrinqbcSelectionItem.mcs_lastActionDate != string.Empty)
                                    {
                                        DateTime newDateTime;
                                        if (DateTime.TryParse(shrinqbcSelectionItem.mcs_lastActionDate, out newDateTime))
                                        {
                                            thisNewEntity["udo_lastactiondate"] = newDateTime;
                                        }
                                    }
                                    if (shrinqbcSelectionItem.mcs_endProductTypeCode != string.Empty)
                                    {
                                        thisNewEntity["udo_epc"] = shrinqbcSelectionItem.mcs_endProductTypeCode;
                                    }
                                    if (shrinqbcSelectionItem.mcs_claimReceiveDate != string.Empty)
                                    {
                                        DateTime newDateTime;
                                        if (DateTime.TryParse(shrinqbcSelectionItem.mcs_claimReceiveDate, out newDateTime))
                                        {
                                            thisNewEntity["udo_dateofclaim"] = newDateTime;
                                        }
                                    }
                                    if (shrinqbcSelectionItem.mcs_claimTypeCode != string.Empty)
                                    {
                                        thisNewEntity["udo_claimtypecode"] = shrinqbcSelectionItem.mcs_claimTypeCode;
                                    }
                                    if (shrinqbcSelectionItem.mcs_statusTypeCode != string.Empty)
                                    {
                                        thisNewEntity["udo_claimstatus"] = shrinqbcSelectionItem.mcs_statusTypeCode;
                                        if (IsPendingClaim(shrinqbcSelectionItem)) pendingClaims++;
                                    }
                                    if (findBenefitClaimResponse.VIMTbenefitClaimRecordbclmInfo.VIMTbenefitClaimRecord1bclmInfo.mcs_claimStationOfJurisdiction != string.Empty)
                                    {
                                        thisNewEntity["udo_claimstation"] = findBenefitClaimResponse.VIMTbenefitClaimRecordbclmInfo.VIMTbenefitClaimRecord1bclmInfo.mcs_claimStationOfJurisdiction;
                                    }
                                    if (shrinqbcSelectionItem.mcs_benefitClaimID != string.Empty)
                                    {
                                        int newInteger;
                                        if (Int32.TryParse(shrinqbcSelectionItem.mcs_benefitClaimID, out newInteger))
                                        {
                                            thisNewEntity["udo_claimidentifier"] = newInteger;
                                            long newLong = Convert.ToInt64(newInteger);
                                            responseIds.newUDOcreateUDOClaimsIdentifier = newLong;
                                        }
                                        thisNewEntity["udo_claimidstring"] = shrinqbcSelectionItem.mcs_benefitClaimID;

                                    }
                                    if (shrinqbcSelectionItem.mcs_claimantLastName != string.Empty)
                                    {
                                        thisNewEntity["udo_claimantlastname"] = shrinqbcSelectionItem.mcs_claimantLastName;
                                    }
                                    if (shrinqbcSelectionItem.mcs_claimantFirstName != string.Empty)
                                    {
                                        thisNewEntity["udo_claimantfirstname"] = shrinqbcSelectionItem.mcs_claimantFirstName;
                                    }
                                    if (!string.IsNullOrEmpty(shrinqbcSelectionItem.mcs_claimantMiddleName))
                                    {
                                        thisNewEntity["udo_claimantmiddlename"] = shrinqbcSelectionItem.mcs_claimantMiddleName;
                                    }
                                    if (!string.IsNullOrEmpty(shrinqbcSelectionItem.mcs_claimantSuffix))
                                    {
                                        thisNewEntity["udo_claimantsuffix"] = shrinqbcSelectionItem.mcs_claimantSuffix;
                                    }
                                    if (shrinqbcSelectionItem.mcs_claimTypeName != string.Empty)
                                    {
                                        thisNewEntity["udo_claimdescription"] = shrinqbcSelectionItem.mcs_claimTypeName;
                                    }


                                    if (shrinqbcSelectionItem.mcs_claimReceiveDate != string.Empty)
                                    {
                                        DateTime newDateTime;
                                        if (DateTime.TryParse(shrinqbcSelectionItem.mcs_claimReceiveDate, out newDateTime))
                                        {
                                            var daysDiff = DateTime.Today - newDateTime;
                                            thisNewEntity["udo_dayssinceinception"] = daysDiff.Days.ToString();
                                        }
                                    }
                                    if (shrinqbcSelectionItem.mcs_claimReceiveDate != string.Empty)
                                    {
                                        DateTime newDateTime;
                                        if (DateTime.TryParse(shrinqbcSelectionItem.mcs_claimReceiveDate, out newDateTime))
                                        {
                                            var daysDiff = DateTime.Today - newDateTime;
                                            thisNewEntity["udo_dayspending"] = daysDiff.Days.ToString();
                                        }
                                    }
                                    if (request.UDOcreateUDOClaimsRelatedEntitiesInfo != null)
                                    {
                                        foreach (var relatedItem in request.UDOcreateUDOClaimsRelatedEntitiesInfo)
                                        {
                                            thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                        }
                                    }

                                    #region findBenefitClaimDetailsbyBnftClaimId - moved to update processor
                                    //if (!response.ExceptionOccured)
                                    //{
                                    //    // prefix = findBCDbyBCid_findBenefitClaimDetailsByBnftClaimIdRequest();
                                    //    var findBenefitClaimDetailsByBnftClaimIdRequest = new VIMTfindBCDbyBCid_findBenefitClaimDetailsByBnftClaimIdRequest();
                                    //    findBenefitClaimDetailsByBnftClaimIdRequest.LogTiming = request.LogTiming;
                                    //    findBenefitClaimDetailsByBnftClaimIdRequest.LogSoap = request.LogSoap;
                                    //    findBenefitClaimDetailsByBnftClaimIdRequest.Debug = request.Debug;
                                    //    findBenefitClaimDetailsByBnftClaimIdRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                                    //    findBenefitClaimDetailsByBnftClaimIdRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                                    //    findBenefitClaimDetailsByBnftClaimIdRequest.RelatedParentId = request.RelatedParentId;
                                    //    findBenefitClaimDetailsByBnftClaimIdRequest.UserId = request.UserId;
                                    //    findBenefitClaimDetailsByBnftClaimIdRequest.OrganizationName = request.OrganizationName;
                                    //    findBenefitClaimDetailsByBnftClaimIdRequest.LegacyServiceHeaderInfo = new VIMT.EBenefitsBnftClaimStatusWebService.Messages.HeaderInfo()
                                    //    {
                                    //        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                                    //        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                                    //        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                                    //        Password = request.LegacyServiceHeaderInfo.Password,
                                    //        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                                    //    };

                                    //    if (shrinqbcSelectionItem.mcs_benefitClaimID != string.Empty)MessageProcessType.Local
                                    //    {
                                    //        long newLong;
                                    //        if (Int64.TryParse(shrinqbcSelectionItem.mcs_benefitClaimID, out newLong))
                                    //            findBenefitClaimDetailsByBnftClaimIdRequest.mcs_bnftclaimid = newLong;
                                    //    }
                                    //    //findBenefitClaimDetailsByBnftClaimIdRequest.mcs_filenumber = request.fileNumber;

                                          // (TN): Already commented out in original code
                                    //    var findBenefitClaimDetailsByBnftClaimIdResponse = findBenefitClaimDetailsByBnftClaimIdRequest.SendReceive<VIMTfindBCDbyBCid_findBenefitClaimDetailsByBnftClaimIdResponse>(MessageProcessType.Local);
                                    //    progressString = "After VIMT EC Call for findBenefitClaimDetails...";

                                    //    response.ExceptionMessage = findBenefitClaimDetailsByBnftClaimIdResponse.ExceptionMessage;
                                    //    response.ExceptionOccured = findBenefitClaimDetailsByBnftClaimIdResponse.ExceptionOccured;

                                    //    //findBenefitClaimDetailsByBnftClaimIdResponse.VIMTfindBCDbyBCid_benefitClaimDetailsDTOInfo will be null if Exception occurred
                                    //    if (findBenefitClaimDetailsByBnftClaimIdResponse.VIMTfindBCDbyBCid_benefitClaimDetailsDTOInfo != null && findBenefitClaimDetailsByBnftClaimIdResponse.VIMTfindBCDbyBCid_benefitClaimDetailsDTOInfo.VIMTfindBCDbyBCid_bnftClaimLcStatusInfo != null)
                                    //    {
                                    //        var bnftClaimLcStatus = findBenefitClaimDetailsByBnftClaimIdResponse.VIMTfindBCDbyBCid_benefitClaimDetailsDTOInfo.VIMTfindBCDbyBCid_bnftClaimLcStatusInfo;

                                    //        //Added to generated to implement logic to select the most advanced phase type in the returned phaseType for the DTO
                                    //        System.Collections.Generic.List<string> returnedPhaseTypes = new List<string>();
                                    //        var mostAdvancedPhaseType = "";
                                    //        foreach (var bnftClaimLcStatusItem in bnftClaimLcStatus)
                                    //        {
                                    //            if (!string.IsNullOrEmpty(bnftClaimLcStatusItem.mcs_phaseType))
                                    //            {
                                    //                returnedPhaseTypes.Add(bnftClaimLcStatusItem.mcs_phaseType);
                                    //            }
                                    //        }
                                    //        var phaseTypes = new string[] { "Claim Received", "Under Review", "Gathering of Evidence", "Review of Evidence", "Preparation for Decision", "Pending Decision Approval", "Preparation for Notification", "Complete" };
                                    //        for (int i = 7; i >= 0; i--)
                                    //        {
                                    //            if (returnedPhaseTypes.Contains(phaseTypes[i]))
                                    //            {
                                    //                mostAdvancedPhaseType = phaseTypes[i];
                                    //                break;
                                    //            }
                                    //        }
                                    //        thisNewEntity["udo_phasetype"] = mostAdvancedPhaseType;
                                    //        if (!string.IsNullOrEmpty(findBenefitClaimDetailsByBnftClaimIdResponse.VIMTfindBCDbyBCid_benefitClaimDetailsDTOInfo.mcs_minEstClaimCompleteDt))
                                    //        {
                                    //            DateTime newDateTime;
                                    //            if (DateTime.TryParse(findBenefitClaimDetailsByBnftClaimIdResponse.VIMTfindBCDbyBCid_benefitClaimDetailsDTOInfo.mcs_minEstClaimCompleteDt, out newDateTime))
                                    //            {
                                    //                thisNewEntity["udo_minestclaimcompletedt"] = newDateTime;
                                    //            }
                                    //        }
                                    //        if (!string.IsNullOrEmpty(findBenefitClaimDetailsByBnftClaimIdResponse.VIMTfindBCDbyBCid_benefitClaimDetailsDTOInfo.mcs_maxEstClaimCompleteDt))
                                    //        {
                                    //            DateTime newDateTime;
                                    //            if (DateTime.TryParse(findBenefitClaimDetailsByBnftClaimIdResponse.VIMTfindBCDbyBCid_benefitClaimDetailsDTOInfo.mcs_maxEstClaimCompleteDt, out newDateTime))
                                    //            {
                                    //                thisNewEntity["udo_maxestclaimcompletedt"] = newDateTime;
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                    #endregion findBenefitClaimDetailsbyBnftClaimId

                                    //responseIds.newUDOcreateUDOClaimsId = OrgServiceProxy.Create(TruncateHelper.TruncateFields(thisNewEntity, request.OrganizationName, request.UserId, request.LogTiming));
                                    //UDOcreateUDOClaimsArray.Add(responseIds);
                                    CreateRequest createData = new CreateRequest
                                    {
                                        Target = thisNewEntity
                                    };
                                    requestCollection.Add(createData);

                                    #endregion
                                }

                                #region Create records

                                if (requestCollection.Count() > 0)
                                {
                                    string logInfo = string.Format("about to create claims: {0}", requestCollection.Count());
                                    LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Claim Records Created", logInfo);

                                    var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, request.UserId, request.Debug);

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

                                string logInfo2 = string.Format("Claim Records Created: {0}", requestCollection.Count());
                                LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Claim Records Created", logInfo2);
                                #endregion
                                //   LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateUDOClaimsProcessor Processor, Progess:" + progressString, UDOcreateUDOClaimsArray.Count + " records found and created");

                                progressString = "after claim creation";
                            }
                            //response.UDOcreateUDOClaimsInfo = UDOcreateUDOClaimsArray.ToArray();
                            #endregion
                        }
                        //if (pendingClaims > 0)
                        //{
                        //  //  LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateClaimsProcessor", "(>0) More than 0 pending claims found.");
                        //}
                        //else
                        //{
                        //  //  LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateClaimsProcessor", "No pending claims found.");
                        //}
                    }
                }
                else
                {
                    LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateUDOClaimsProcessor Processor", "No filenumber!!");
                }
                #region Log Results
                string logMsg = string.Format("Claim Records Created: {0}", requestCollection.Count());
                LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Claim Records Created", logMsg);
                #endregion

                var dependentId = Guid.Empty;

                if (request.UDOcreateUDOClaimsRelatedEntitiesInfo != null)
                {
                    foreach (var related in request.UDOcreateUDOClaimsRelatedEntitiesInfo)
                    {
                        if (related.RelatedEntityName.ToLower() == "udo_dependant")
                        {
                            dependentId = related.RelatedEntityId;
                        }
                    }
                }

                if (request.vetsnapshotId != Guid.Empty && dependentId == Guid.Empty)
                {
                    Entity vetSnapShot = new Entity();
                    vetSnapShot.LogicalName = "udo_veteransnapshot";
                    vetSnapShot.Id = request.vetsnapshotId;
                    vetSnapShot["udo_pendingclaims"] = pendingClaims.ToString() + " open claim(s)";
                    vetSnapShot["udo_claimscompleted"] = new OptionSetValue(752280002);
                    vetSnapShot["udo_claimscomplete"] = true;

                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(vetSnapShot, request.OrganizationName, request.UserId, request.LogTiming));
                }

                if (request.idProofId != Guid.Empty && dependentId == Guid.Empty)
                {
                    var idProof = new Entity();
                    idProof.Id = request.idProofId;
                    idProof.LogicalName = "udo_idproof";
                    idProof["udo_claimintegration"] = new OptionSetValue(752280002);
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(idProof, request.OrganizationName, request.UserId, request.LogTiming));
                }
                else
                {
                    // LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateClaimsProcessor Processor", "no idProofId found");
                }

                if (dependentId != Guid.Empty)
                {
                    var dependent = new Entity("udo_dependant");
                    dependent.Id = dependentId;
                    dependent["udo_claimscomplete"] = true;
                    OrgServiceProxy.Update(dependent);
                }

                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOcreateUDOClaimsProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Claims Data";
                response.ExceptionOccured = true;
                if (request.idProofId != Guid.Empty)
                {
                    var idProof = new Entity();
                    idProof.Id = request.idProofId;
                    idProof.LogicalName = "udo_idproof";
                    idProof["udo_claimintegration"] = new OptionSetValue(752280003);
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(idProof, request.OrganizationName, request.UserId, request.LogTiming));
                }


                return response;
            }
        }

        private bool IsPendingClaim(VIMTparticipantRecordbclm claim)
        {
            var status = claim.mcs_statusTypeCode.ToLower() ?? string.Empty;
            return !(status == "clr" || status == "clsd" || status == "can");
        }

        private bool IsPendingClaim(VIMTselectionbclmMultipleResponse claim)
        {
            var status = claim.mcs_statusTypeCode.ToLower() ?? string.Empty;
            return !(status == "clr" || status == "clsd" || status == "can");
        }
    }
}