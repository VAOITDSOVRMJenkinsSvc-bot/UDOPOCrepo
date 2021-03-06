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
    class createUDOClaimsOrchProcessor
    {
        private bool _debug { get; set; }
        private const string method = "createUDOClaimsProcessor";
        private string LogBuffer { get; set; }
        private UDOcreateUDOClaimsResponse response;

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
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateUDOClaimsOrchProcessor Processor, Connection Error", connectException.Message);
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
            int createdcount = 0;

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
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "createUDOClaimsOrchProcessor Processor", "About to call findBenefitClaimRequest, request.fileNumber:" + request.fileNumber);

                    // TODO(TN): comment to remediate
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
                                    // LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "createUDOClaimsOrchProcessor Processor", "single claim");
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
                                        switch (participantRecord.mcs_statusTypeCode.ToLower())
                                        {
                                            case "clr":
                                                break;
                                            case "clsd":
                                                break;
                                            case "can":
                                                break;
                                            default:
                                                pendingClaims += 1;
                                                break;
                                        }
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

                                    //var claimId = OrgServiceProxy.Create(TruncateHelper.TruncateFields(thisNewEntity, request.OrganizationName, request.UserId, request.LogTiming));
                                    //responseIds.newUDOcreateUDOClaimsId = claimId;
                                    UDOcreateUDOClaimsArray.Add(responseIds);
                                    #endregion
                                    //  LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateUDOClaimsOrchProcessor Processor, Progess:" + progressString, "1 record found and created");

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

                                                //OrgServiceProxy.Create(TruncateHelper.TruncateFields(thisLifecycle, request.OrganizationName, request.UserId, request.LogTiming));

                                                var parent = new Entity();
                                                //parent.Id = claimId;
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

                                            /*
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
                                            */
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
                                    //LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "createUDOClaimsOrchProcessor Processor", "Multiple claims");
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
                                        switch (shrinqbcSelectionItem.mcs_statusTypeCode.ToLower())
                                        {
                                            case "clr":
                                                break;
                                            case "clsd":
                                                break;
                                            case "can":
                                                break;
                                            default:
                                                pendingClaims += 1;
                                                break;
                                        }
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
                                    //*********Disabling auto-cache from orch

                                    /*var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, request.UserId, request.Debug);

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
                                     */
                                }

                                createdcount = 0; //requestCollenction.Count();
                                string logInfo = string.Format("Claim Records Created: {0}", createdcount);
                                LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Claim Records Created", logInfo);
                                #endregion
                                //   LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateUDOClaimsOrchProcessor Processor, Progess:" + progressString, UDOcreateUDOClaimsArray.Count + " records found and created");

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
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateUDOClaimsOrchProcessor Processor", "No filenumber!!, SnapshotID:" + request.vetsnapshotId);
                }
                #region Log Results
                string logMsg = string.Format("Orch Claim Records Created: {0}", createdcount);
                LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Orch Claim Records Created", logMsg);
                #endregion
                if (request.vetsnapshotId != Guid.Empty)
                {
                    Entity vetSnapShot = new Entity();
                    vetSnapShot.LogicalName = "udo_veteransnapshot";
                    vetSnapShot.Id = request.vetsnapshotId;
                    vetSnapShot["udo_pendingclaims"] = pendingClaims.ToString() + " open claim(s)";
                    vetSnapShot["udo_claimscompleted"] = new OptionSetValue(752280002);
                    vetSnapShot["udo_claimscomplete"] = true;

                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(vetSnapShot, request.OrganizationName, request.UserId, request.LogTiming));
                }

                /*
                if (request.idProofId != Guid.Empty)
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
                */
                // Called after claims are created and veteran snapshot has been updated.
                // Update the people.
                //if (pendingClaims > 0)
                //{
                // call the DoPeopleEntity asynchronously
                //LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateClaimsProcessor", "before DoPeopleEntity.");
                //var result2 = DoPeopleEntity(OrgServiceProxy, request, findBenefitClaimResponse, requestCollection);
                //}
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOcreateUDOClaimsOrchProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Claims Data";
                response.ExceptionOccured = true;
                /*if (request.idProofId != Guid.Empty)
                {
                    var idProof = new Entity();
                    idProof.Id = request.idProofId;
                    idProof.LogicalName = "udo_idproof";
                    idProof["udo_claimintegration"] = new OptionSetValue(752280003);
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(idProof, request.OrganizationName, request.UserId, request.LogTiming));
                }
                */
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

        //private bool PeopleLoaded(OrganizationServiceProxy OrgServiceProxy, UDOcreateUDOClaimsRequest request, int depth = 0)
        //{
        //    if (depth > 16)
        //    {
        //        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "createUDOClaimsProcessor, DoPeopleEntity, PeopleLoaded", "People never loaded, tried 16 times..");
        //        return false;
        //    }
        //    var fetch = "<fetch page='1' count='1'><entity name='udo_idproof'><attribute name='udo_idproofid'/><filter type='and'>" +
        //                "<condition attribute='udo_idproofid' operator='eq' value='" + request.idProofId + "'/>" +
        //                "<condition attribute='udo_awardintegration' operator='eq' value='752280002'/>" +
        //                "</filter></entity></fetch>";

        //    var loaded = OrgServiceProxy.RetrieveMultiple(new FetchExpression(fetch)).Entities.Count > 0;

        //    if (!loaded)
        //    {
        //        Thread.Sleep(250);
        //        //  LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "createUDOClaimsProcessor, DoPeopleEntity, PeopleLoaded", "People note loaded yet... waiting 250ms...");
        //        return PeopleLoaded(OrgServiceProxy, request, depth + 1);
        //    }
        //    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "createUDOClaimsProcessor, DoPeopleEntity, PeopleLoaded", "People Loaded at depth: " + depth.ToString());
        //    return true;
        //}

        //private string GetString(Entity entity, string key)
        //{
        //    if (String.IsNullOrEmpty(key) || entity == null || !entity.Contains(key) ||
        //        String.IsNullOrEmpty(entity[key].ToString())) return string.Empty;
        //    return entity[key].ToString();
        //}

        //private async Task<bool> DoPeopleEntity(OrganizationServiceProxy OrgServiceProxy, UDOcreateUDOClaimsRequest request,
        //    VIMTfindBenefitClaimResponse findBenefitClaimResponse, OrganizationRequestCollection requestCollection)
        //{
        //    var peopleRequestCollection = new OrganizationRequestCollection();
        //    try {
        //    // Wait for people to load
        //    if (!(PeopleLoaded(OrgServiceProxy, request))) return false;

        //    #region Get a collection of unique claimPeople
        //    var claimPeople = 
        //        requestCollection.Select(req => req as CreateRequest).Where(o => o != null)
        //        .Select(req => req.Target).GroupBy(e => String.Join(" ", new[] {
        //               GetString(e, "udo_claimantfirstname"),
        //               GetString(e, "udo_claimantmiddlename"),
        //               GetString(e, "udo_claimantlastname"),
        //               GetString(e, "udo_claimantsuffix")})).Select(s=>new {
        //                   Name=s.Key, 
        //                   CrmName=String.Join(" ",
        //                       GetString(s.FirstOrDefault(), "udo_claimantfirstname"),
        //         //              GetString(s.FirstOrDefault(), "udo_claimantmiddlename"),
        //                       GetString(s.FirstOrDefault(), "udo_claimantlastname")
        //                   ).Trim(),
        //                   FirstName = GetString(s.FirstOrDefault(), "udo_claimantfirstname"),
        //                   MiddleName = GetString(s.FirstOrDefault(), "udo_claimantmiddlename"),
        //                   LastName = GetString(s.FirstOrDefault(), "udo_claimantlastname"),
        //                   Suffix= GetString(s.FirstOrDefault(), "udo_claimantsuffix"),
        //                   ClaimId=GetString(s.FirstOrDefault(),"udo_claimidstring"),
        //                   PendingClaims=s.Any(e=>{
        //                       var status = GetString(e,"udo_claimstatus").ToLower();
        //                       return !(status=="clr"||status=="clsd"||status=="can");
        //                   }),
        //                   PayeeCode = GetString(s.FirstOrDefault(),"udo_payeetypecode")
        //               }).Where(e=>!String.IsNullOrEmpty(e.ClaimId));
        //    #endregion

        //    #region Get udoPeople (udo_person) from CRM
        //    QueryByAttribute qa = new QueryByAttribute() {
        //        EntityName = "udo_person",
        //        Attributes = {"udo_idproofid"},
        //        ColumnSet = new ColumnSet("udo_personid","udo_first","udo_last", "udo_ptcpntid","udo_middle"),
        //        Values = {request.idProofId}
        //    };

        //    EntityCollection retrieved = OrgServiceProxy.RetrieveMultiple(qa);
        //    var udoPeople = retrieved.Entities.Select(e=>new {
        //        CrmName=String.Join(" ",GetString(e,"udo_first")/*,GetString(e,"udo_middle")*/,GetString(e,"udo_last")),
        //        PersonId=e["udo_personid"].ToString(),
        //        ParticipantId=GetString(e,"udo_ptcpntid")
        //    });
        //    #endregion

        //    #region Create peopleToPRocess: People that need to be updated vs created using claimPeople and udoPeople
        //    var peopleToProcess = from c in claimPeople
        //                          join u in udoPeople on c.CrmName equals u.CrmName into grp
        //                          from subu in grp.DefaultIfEmpty()
        //                          select new {
        //                              FullName = c.Name,
        //                              ClaimId = c.ClaimId,
        //                              FirstName = c.FirstName,
        //                              MiddleName = c.MiddleName,
        //                              LastName = c.LastName,
        //                              Suffix = c.Suffix,
        //                              PayeeCode = c.PayeeCode,
        //                              PendingClaims = c.PendingClaims,
        //                              PersonId = (subu==null) ? string.Empty : subu.PersonId,
        //                              ParticipantId = (subu==null) ? string.Empty : subu.ParticipantId
        //                          };
        //    #endregion

        //    Entity vetSnapshot = null;
        //    if (request.vetsnapshotId != System.Guid.Empty)
        //    {
        //        vetSnapshot = OrgServiceProxy.Retrieve("udo_veteransnapshot", request.vetsnapshotId, new ColumnSet("udo_ssn", "udo_veteranid", "udo_firstname", "udo_lastname", "udo_participantid"));
        //    }

        //    Entity veteran = null;
        //    Guid? veteranId = null;
        //    string vet_ptcpntid = string.Empty;

        //    #region Build peopleRequestCollection of Updates and Creates for udo_person

        //    foreach (var person in peopleToProcess)
        //    {
        //        Guid personId = Guid.Empty;
        //        if (!Guid.TryParse(person.PersonId, out personId)) personId = Guid.Empty;

        //        if (personId!=Guid.Empty)
        //        {
        //            #region Build Update Request
        //            var updateEntity = new Entity("udo_person");
        //            updateEntity["udo_personid"] = personId;
        //            //updateEntity.Id = personId;
        //            updateEntity["udo_payeecode"] = person.PayeeCode;
        //            updateEntity["udo_pendingclaimsexist"] = person.PendingClaims;
        //            var updateRequest = new UpdateRequest() { Target = updateEntity };
        //            peopleRequestCollection.Add(updateRequest);
        //            #endregion
        //        }
        //        else
        //        {
        //            #region Build Create Request
        //            var progressString = "Building Create Request";
        //            var peopleEntity = new Entity("udo_person");

        //            string ssn = null, vetssn = null, pid = null;

        //            // We are adding them for just a claim, they are a claimant or beneficiary...
        //            peopleEntity["udo_payeecode"] = person.PayeeCode;
        //            peopleEntity["udo_payeename"] = person.FullName;
        //            peopleEntity["udo_pendingclaimsexist"] = person.PendingClaims;

        //            // Should we also confirm that it is not the veteran by another comparison?  Currently the awards processor
        //            // adds the veteran person, so if there is a 00 here, it is for a non-veteran.
        //            if (person.PayeeCode.Trim().Equals("00"))
        //            {
        //                peopleEntity["udo_type"] = new OptionSetValue(752280003);
        //            }
        //            else
        //            {
        //                peopleEntity["udo_type"] = new OptionSetValue(752280002); // Beneficiary
        //            }

        //            #region findBenefitClaimDetailsbyClaimId
        //            var findBenefitClaimDetailsByClaimIdRequest = new VIMTfbendtlfindBenefitClaimDetailRequest
        //            {
        //                LogTiming = request.LogTiming,
        //                LogSoap = request.LogSoap,
        //                Debug = request.Debug,
        //                RelatedParentEntityName = request.RelatedParentEntityName,
        //                RelatedParentFieldName = request.RelatedParentFieldName,
        //                RelatedParentId = request.RelatedParentId,
        //                UserId = request.UserId,
        //                OrganizationName = request.OrganizationName,
        //                mcs_benefitclaimid = person.ClaimId,
        //                LegacyServiceHeaderInfo = new VIMT.BenefitClaimService.Messages.HeaderInfo()
        //                {
        //                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
        //                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
        //                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
        //                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
        //                }
        //            };

                      // (TN): Already commented out in original code
        //            var findBenefitClaimDetailsByClaimIdResponse = findBenefitClaimDetailsByClaimIdRequest.SendReceive<VIMTfbendtlfindBenefitClaimDetailResponse>(MessageProcessType.Local);
        //            progressString = "After VIMT EC Call for findBenefitClaimDetails...";

        //            if (findBenefitClaimDetailsByClaimIdResponse.VIMTfbendtlbenefitClaimRecordbclmInfo != null)
        //            {
        //                var claimRecord = findBenefitClaimDetailsByClaimIdResponse.VIMTfbendtlbenefitClaimRecordbclmInfo;
        //                if (claimRecord.VIMTfbendtlbenefitClaimRecord1bclmInfo != null)
        //                {
        //                    var claimRecordInfo = claimRecord.VIMTfbendtlbenefitClaimRecord1bclmInfo;

        //                    if (!string.IsNullOrEmpty(claimRecordInfo.mcs_participantClaimantID))
        //                    {
        //                        peopleEntity["udo_ptcpntid"] = pid = claimRecordInfo.mcs_participantClaimantID;
        //                        if (retrieved.Entities.Any(e=>e["udo_ptcpntid"].ToString().Equals(claimRecordInfo.mcs_participantClaimantID, StringComparison.OrdinalIgnoreCase)))
        //                            continue;
        //                        peopleEntity["udo_name"] = String.Join(" ",claimRecordInfo.mcs_claimantFirstName,
        //                            claimRecordInfo.mcs_claimantLastName);
        //                        peopleEntity["udo_first"] = claimRecordInfo.mcs_claimantFirstName;
        //                        peopleEntity["udo_last"] = claimRecordInfo.mcs_claimantLastName;
        //                        peopleEntity["udo_middle"] = claimRecordInfo.mcs_claimantMiddleName;
                                
        //                    }
        //                }
        //            }
                    
        //            if (!peopleEntity.Contains("udo_ptcpntid"))
        //            {
        //                var method = MethodInfo.GetThisMethod().ToString();
        //                LogHelper.LogError(request.OrganizationName, request.UserId, method, "Unable to get Participant Id from Claim");
        //                continue;
        //            }

        //            peopleEntity["udo_idproofid"] = new EntityReference("udo_idproof", request.idProofId);

        //            if (request.ownerId != System.Guid.Empty)
        //            {
        //                peopleEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
        //            }

        //            if (vetSnapshot != null)
        //            {
        //                peopleEntity["udo_vetsnapshotid"] = vetSnapshot.Id.ToString();
        //                peopleEntity["udo_veteransnapshotid"] = vetSnapshot.ToEntityReference();
        //                peopleEntity["udo_vetssn"] = vetssn = GetString(vetSnapshot,"udo_ssn");
        //                if (vetSnapshot.Contains("udo_veteranid") && vetSnapshot["udo_veteranid"] != null)
        //                {
        //                    var vetRef = (EntityReference)vetSnapshot["udo_veteranid"];
        //                    peopleEntity["udo_veteranid"] = vetRef;
        //                    veteranId = vetRef.Id;
        //                }
        //                peopleEntity["udo_vetfirstname"] = GetString(vetSnapshot, "udo_firstname");
        //                peopleEntity["udo_vetlastname"] = GetString(vetSnapshot, "udo_lastname");
        //                vet_ptcpntid = GetString(vetSnapshot, "udo_participantid");
        //            }
                    
        //            if (veteranId.HasValue &&
        //               (!peopleEntity.Contains("udo_vetssn")
        //               || String.IsNullOrEmpty(peopleEntity["udo_vetssn"].ToString())
        //               || String.IsNullOrEmpty(vet_ptcpntid)))
        //            {
        //                if (veteran != null)
        //                {
        //                    veteran = OrgServiceProxy.Retrieve("contact", veteranId.Value, new ColumnSet("udo_participantid", "udo_ssn"));
        //                }
        //                vet_ptcpntid = GetString(veteran, "udo_participantid");
        //                if (!peopleEntity.Contains("udo_vetssn") || String.IsNullOrEmpty(peopleEntity["udo_vetssn"].ToString())) {
        //                    peopleEntity["udo_vetssn"] = vetssn = GetString(veteran,"udo_ssn");
        //                }
        //            }
        //            //peopleEntity["udo_filenumber"] = request.fileNumber;
        //            //peopleEntity["udo_benefittypename"] =  ???
        //            //peopleEntity["udo_awardtypecode"] =  ???
        //            peopleEntity["udo_awardsexist"] = false;  // it would have been an update not a create if true
                    
        //            //TODO: Create Payee Code Id?
        //            #endregion

        //            //Use Participant Id to get info and build entity
        //            if (!String.IsNullOrEmpty(vet_ptcpntid))
        //            {
        //                var findGeneralInformationByPtcpntIdsRequest = new VIMTfgenpidfindGeneralInformationByPtcpntIdsRequest
        //                {
        //                    LogTiming = request.LogTiming,
        //                    LogSoap = request.LogSoap,
        //                    Debug = request.Debug,
        //                    OrganizationName = request.OrganizationName,
        //                    UserId = request.UserId,
        //                    RelatedParentEntityName = request.RelatedParentEntityName,
        //                    RelatedParentFieldName = request.RelatedParentFieldName,
        //                    RelatedParentId = request.RelatedParentId,
        //                    mcs_ptcpntvetid = vet_ptcpntid,
        //                    mcs_ptcpntbeneid = pid,
        //                    mcs_ptpcntrecipid = pid,
                            
        //                    LegacyServiceHeaderInfo = new VIMT.ClaimantWebService.Messages.HeaderInfo
        //                    {
        //                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
        //                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
        //                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
        //                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
        //                    }
        //                };

                          // (TN): Already commented out in original code
        //                var findGeneralInformationByPtcpntIdsResponse = findGeneralInformationByPtcpntIdsRequest.SendReceive<VIMTfgenpidfindGeneralInformationByPtcpntIdsResponse>(MessageProcessType.Local);

        //                if (findGeneralInformationByPtcpntIdsResponse.VIMTfgenpidreturnclmsInfo != null)
        //                {
        //                    var pidClaimInfo = findGeneralInformationByPtcpntIdsResponse.VIMTfgenpidreturnclmsInfo;

        //                    if (!String.IsNullOrEmpty(pidClaimInfo.mcs_payeeSSN))
        //                    {
        //                        peopleEntity["udo_ssn"] = ssn = pidClaimInfo.mcs_payeeSSN;
        //                    }
                            
        //                    if (!String.IsNullOrEmpty(pidClaimInfo.mcs_payeeBirthDate)) {
        //                        peopleEntity["udo_dobstr"] = pidClaimInfo.mcs_payeeBirthDate; 
                                
        //                        // Set the datetime of the dob
        //                        DateTime newDateTime;                                
        //                        if (DateTime.TryParse(pidClaimInfo.mcs_payeeBirthDate, out newDateTime))
        //                            peopleEntity["udo_dob"] = newDateTime.ToCRMDateTime();
        //                    }
        //                    peopleEntity["udo_gender"] = pidClaimInfo.mcs_payeeSex;
        //                    peopleEntity["udo_payeetypename"] = pidClaimInfo.mcs_payeeTypeName;
        //                    //var addressid = pidClaimInfo.mcs_paymentAddressID;
        //                }
        //            }

        //            var createRequest = new CreateRequest()
        //            {
        //                Target = peopleEntity
        //            };

        //            peopleRequestCollection.Add(createRequest);

        //            if (request.Debug) {
        //                var thisMethod = MethodInfo.GetThisMethod().ToString();
        //                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId,thisMethod,peopleEntity.DumpToString("New Person found in Claims"));
        //            }

        //            #endregion
        //        }
        //    }
        //    }
        //     catch (Exception ex) {
        //         var method = MethodInfo.GetThisMethod().ToString();
        //         LogHelper.LogError(request.OrganizationName, request.UserId, method, ex);
        //    }

        //    #endregion

        //    #region Execute Multiple
        //    if (peopleRequestCollection.Count > 0)
        //    {
        //        var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, peopleRequestCollection, request.OrganizationName, request.UserId, request.Debug);

        //        if (_debug)
        //        {
        //            LogBuffer += result.LogDetail;
        //            LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, LogBuffer);
        //        }

        //        if (result.IsFaulted)
        //        {
        //            LogHelper.LogError(request.OrganizationName, request.UserId, method, result.ErrorDetail);
        //            response.ExceptionMessage = result.FriendlyDetail;
        //            response.ExceptionOccured = true;
        //            //return response;
        //        }
        //    }

        //    #endregion

        //    return true;
        //}
    }
}