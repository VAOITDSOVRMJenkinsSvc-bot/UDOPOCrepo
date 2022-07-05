using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Specialized;
using System.Linq;
using UDO.LOB.Extensions.Configuration;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.Claims.Messages;
using VEIS.Core.Messages;
using VEIS.Messages.BenefitClaimService;
using Microsoft.Xrm.Tooling.Connector;

namespace UDO.LOB.Claims.Processors
{
    class createUDOClaimsOrchProcessor
    {
        private bool _debug { get; set; }

        private const string method = "createUDOClaimsOrchProcessor";

        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOcreateUDOClaimsRequest request)
        {
            //var request = message as createUDOClaimsRequest;
            UDOcreateUDOClaimsResponse response = new UDOcreateUDOClaimsResponse();
            response.MessageId = request.MessageId;
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
                    IdProof = request.idProofId,
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : string.Empty
                };
            }
            TraceLogger aiLogger = new TraceLogger("createUDOClaimsOrchProcessor.Execute", request);

            aiLogger.LogTrace($">> Entered {this.GetType().FullName}.createClaims", "1");
            //var dataTruncation = new Truncate();

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

            // These are used throughout the claims processor for both building the veteran snapshot and updating the people list.
            VEISfindBenefitClaimResponse findBenefitClaimResponse = null;
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
                    var findBenefitClaimRequest = new VEISfindBenefitClaimRequest();
                    findBenefitClaimRequest.MessageId = request.MessageId;
                    findBenefitClaimRequest.Debug = request.Debug;
                    findBenefitClaimRequest.LogSoap = request.LogSoap;
                    findBenefitClaimRequest.LogTiming = request.LogTiming;
                    findBenefitClaimRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                    findBenefitClaimRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                    findBenefitClaimRequest.RelatedParentId = request.RelatedParentId;
                    findBenefitClaimRequest.UserId = request.UserId;
                    findBenefitClaimRequest.OrganizationName = request.OrganizationName;
                    findBenefitClaimRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,

                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };

                    //non standard fieldsMessageProcessType.Local
                    findBenefitClaimRequest.mcs_filenumber = request.fileNumber;
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "About to call findBenefitClaimRequest, request.fileNumber:" + request.fileNumber, request.Debug);

                    // REM: Invoke VEIS endpoint
                    findBenefitClaimResponse = WebApiUtility.SendReceive<VEISfindBenefitClaimResponse>(findBenefitClaimRequest, WebApiType.VEIS);

                    if (request.LogSoap || findBenefitClaimResponse.ExceptionOccurred)
                    {
                        if (findBenefitClaimResponse.SerializedSOAPRequest != null || findBenefitClaimResponse.SerializedSOAPResponse != null)
                        {
                            var requestResponse = findBenefitClaimResponse.SerializedSOAPRequest + findBenefitClaimResponse.SerializedSOAPResponse;
                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfindBenefitClaimRequest Request/Response {requestResponse}", true);
                        }
                    }

                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "After findBenefitClaimRequest", request.Debug);
                    progressString = "After VEIS EC for findBenefitClaim Call";

                    response.ExceptionMessage = findBenefitClaimResponse.ExceptionMessage;
                    response.ExceptionOccured = findBenefitClaimResponse.ExceptionOccurred;

                    response.VEISfindBenefitClaimRequestData = findBenefitClaimResponse;
                    #endregion

                    System.Collections.Generic.List<UDOcreateUDOClaimsMultipleResponse> UDOcreateUDOClaimsArray = new System.Collections.Generic.List<UDOcreateUDOClaimsMultipleResponse>();

                    // Replaced: VIMTbenefitClaimRecordbclmInfo = VEISbenefitClaimRecord1Info
                    // @Matt review this change. I looked at structure of message and VEIS added this returninfo, but VIMT did not have that.
                    if (findBenefitClaimResponse.VEISreturnInfo != null)
                    {
                        // Replaced: VIMTparticipantRecordbclmInfo = VEISparticipantRecordInfo
                        if (findBenefitClaimResponse.VEISreturnInfo.VEISparticipantRecordInfo != null)
                        {
                            #region VEISparticipantRecordbclmInfo != null
                            if (findBenefitClaimResponse.VEISreturnInfo.VEISparticipantRecordInfo.mcs_numberOfRecords != null)
                            {
                                if (findBenefitClaimResponse.VEISreturnInfo.VEISparticipantRecordInfo.mcs_numberOfRecords == "001")
                                {

                                    var participantRecord = findBenefitClaimResponse.VEISreturnInfo.VEISparticipantRecordInfo;
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
                                    if (findBenefitClaimResponse.VEISreturnInfo.VEISparticipantRecordInfo.mcs_claimantPersonOrOrganizationIndicator != string.Empty)
                                    {
                                        thisNewEntity["udo_personorgindicator"] = findBenefitClaimResponse.VEISreturnInfo.VEISparticipantRecordInfo.mcs_claimantPersonOrOrganizationIndicator;
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
                                    if (findBenefitClaimResponse.VEISreturnInfo.VEISparticipantRecordInfo.mcs_claimStationOfJurisdiction != string.Empty)
                                    {
                                        thisNewEntity["udo_claimstation"] = findBenefitClaimResponse.VEISreturnInfo.VEISparticipantRecordInfo.mcs_claimStationOfJurisdiction;
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

                                    // 5/14: Reviewed and left commented code as-is
                                    UDOcreateUDOClaimsArray.Add(responseIds);
                                    #endregion

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
                                    // Replaced: VEISbenefitClaimRecordbclmInfo.VEISlifeCycleRecordbclmInfo.VEISlifeCycleRecordsbclmInfo
                                    //          = VEISreturnInfo.VEISlifeCycleRecordInfo
                                    if (findBenefitClaimResponse.VEISreturnInfo.VEISlifeCycleRecordInfo != null)
                                    {
                                        //since this only works for the 1 claim scenario, we don't need the where stuff
                                        var shrinqbcLifeCycle = findBenefitClaimResponse.VEISreturnInfo.VEISlifeCycleRecordInfo.VEISlifeCycleRecordsInfo;

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
                                                if (shrinqbcLifeCycleItem.mcs_benefitClaimID != string.Empty && shrinqbcLifeCycleItem.mcs_lifeCycleStatusTypeName != string.Empty)
                                                {
                                                    thisLifecycle["udo_lifecycle"] = shrinqbcLifeCycleItem.mcs_benefitClaimID + " - " +
                                                                                     shrinqbcLifeCycleItem.mcs_lifeCycleStatusTypeName;
                                                }
                                                if (shrinqbcLifeCycleItem.mcs_reasonText != string.Empty)
                                                {
                                                    thisLifecycle["udo_explanation"] = shrinqbcLifeCycleItem.mcs_reasonText;
                                                }

                                                //Review with @Matt; 5/14/19: Reviewed with Matt; Commented code left as-is

                                                var parent = new Entity();
                                                parent.LogicalName = "udo_claim";
                                                parent["udo_lifecyclecomplete"] = true;
                                                // 5/14/19: Reviewed with Matt; Commented code left as-is; Added comments to NOT add the updateData object to update

                                            }
                                            #region Update records
                                            
                                            #endregion

                                    }
                                    }
                                    #endregion
                                    progressString = "after lifecycle creation";
                                }
                            }
                            requestCollection.Clear();

                            // Replaced: VIMTbenefitClaimRecordbclmInfo.VIMTparticipantRecordbclmInfo.VIMTselectionbclmInfo
                            //          = VEISreturnInfo.VEISparticipantRecordInfo.VEISselectionInfo
                            if (findBenefitClaimResponse.VEISreturnInfo.VEISparticipantRecordInfo.VEISselectionInfo != null)
                            {
                                var shrinqbcSelection = findBenefitClaimResponse.VEISreturnInfo.VEISparticipantRecordInfo.VEISselectionInfo;
                                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, $"Mapping Multiple Claim. VEISselectionInfo Count: {shrinqbcSelection.Length}", request.Debug);
                                foreach (var shrinqbcSelectionItem in shrinqbcSelection)
                                {
                                    #region Map multiple Claims
                                    var responseIds = new UDOcreateUDOClaimsMultipleResponse();
                                    //instantiate the new Entity
                                    Entity thisNewEntity = new Entity();
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
                                    // Replaced: VIMTbenefitClaimRecordbclmInfo.VIMTbenefitClaimRecord1bclmInfo = VEISreturnInfo.VEISbenefitClaimRecord1Info
                                    if (findBenefitClaimResponse.VEISreturnInfo.VEISbenefitClaimRecord1Info.mcs_claimStationOfJurisdiction != string.Empty)
                                    {
                                        thisNewEntity["udo_claimstation"] = findBenefitClaimResponse.VEISreturnInfo.VEISbenefitClaimRecord1Info.mcs_claimStationOfJurisdiction;
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
                                    
                                    #endregion findBenefitClaimDetailsbyBnftClaimId

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
                                    
                                }

                                createdcount = 0; //requestCollenction.Count();
                                string logInfo = string.Format("Claim Records Created: {0}", createdcount);
                                LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Claim Records Created", logInfo);
                                #endregion
                                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, 
                                    $"{method} Progress: {progressString}, {UDOcreateUDOClaimsArray.Count} records found and {createdcount} records created", request.Debug);

                                progressString = "after claim creation";
                            }

                            #endregion
                        }
                    }
                }
                else
                {
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "No filenumber!!, SnapshotID:" + request.vetsnapshotId, request.Debug);
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

                    OrgServiceProxy.Update(vetSnapShot);
                }

                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogDebug(request.OrganizationName, request.UserId, request.Debug, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method, $"{method} Progress: {progressString}", false);
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method, ExecutionException);
                response.ExceptionMessage = "Failed to Process Claims Data " + ExecutionException.Message;
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

        // Replaced: VIMTparticipantRecordbclm = VEISparticipantRecord
        private bool IsPendingClaim(VEISparticipantRecord claim)
        {
            var status = claim.mcs_statusTypeCode.ToLower() ?? string.Empty;
            return !(status == "clr" || status == "clsd" || status == "can");
        }

        // Replaced: VIMTselectionbclmMultipleResponse
        private bool IsPendingClaim(VEISselectionMultipleResponse claim)
        {
            var status = claim.mcs_statusTypeCode.ToLower() ?? string.Empty;
            return !(status == "clr" || status == "clsd" || status == "can");
        }
    }
}