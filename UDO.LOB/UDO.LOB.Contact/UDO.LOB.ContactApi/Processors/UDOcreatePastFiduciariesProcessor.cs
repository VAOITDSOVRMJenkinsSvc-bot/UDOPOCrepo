using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;
using UDO.LOB.Contact.Messages;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using VEIS.Core.Messages;
using VEIS.Messages.ClaimantService;

/// <summary>
/// VIMT LOB Component for UDOcreatePastFiduciaries,createPastFiduciaries method, Processor.
/// </summary>
namespace UDO.LOB.Contact.Processors
{
    class UDOcreatePastFiduciariesProcessor
    {
        private bool _debug { get; set; }

        private string LogBuffer { get; set; }

        private const string method = "UDOcreatePastFiduciariesProcessor";
        
        public IMessageBase Execute(UDOcreatePastFiduciariesRequest request)
        {
            LogBuffer = string.Empty;
            _debug = request.Debug;

            UDOcreatePastFiduciariesResponse response = new UDOcreatePastFiduciariesResponse { MessageId = request?.MessageId };
            var progressString = "Top of Processor";

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }

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

            #region connect to CRM
            CrmServiceClient OrgServiceProxy = null;

            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

            progressString = "After Connection, Starting Request";
            var requestCollection = new OrganizationRequestCollection();

            try
            {
                 //if this doesn't contain anything, don't go asking for it!
                if (!string.IsNullOrEmpty(request.fileNumber))
                {
                    var findAllFiduciaryPoaRequest = new VEISafidpoafindAllFiduciaryPoaRequest();
                    findAllFiduciaryPoaRequest.LogTiming = request.LogTiming;
                    findAllFiduciaryPoaRequest.LogSoap = request.LogSoap;
                    findAllFiduciaryPoaRequest.Debug = request.Debug;
                    findAllFiduciaryPoaRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                    findAllFiduciaryPoaRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                    findAllFiduciaryPoaRequest.RelatedParentId = request.RelatedParentId;
                    findAllFiduciaryPoaRequest.UserId = request.UserId;
                    findAllFiduciaryPoaRequest.OrganizationName = request.OrganizationName;

                    findAllFiduciaryPoaRequest.mcs_filenumber = request.fileNumber;
                    if (request.LegacyServiceHeaderInfo != null)
                    {
                        findAllFiduciaryPoaRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                        {
                            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                        };
                    }
                    var findAllFiduciaryPoaResponse = WebApiUtility.SendReceive<VEISafidpoafindAllFiduciaryPoaResponse>(findAllFiduciaryPoaRequest, WebApiType.VEIS);
                    if (request.LogSoap || findAllFiduciaryPoaResponse.ExceptionOccurred)
                    {
                        if (findAllFiduciaryPoaResponse.SerializedSOAPRequest != null || findAllFiduciaryPoaResponse.SerializedSOAPResponse != null)
                        {
                            var requestResponse = findAllFiduciaryPoaResponse.SerializedSOAPRequest + findAllFiduciaryPoaResponse.SerializedSOAPResponse;
                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISafidpoafindAllFiduciaryPoaRequest Request/Response {requestResponse}", true);
                        }
                    }

                    progressString = "After VEIS EC Call";

                    response.ExceptionMessage = findAllFiduciaryPoaResponse.ExceptionMessage;
                    response.ExceptionOccured = findAllFiduciaryPoaResponse.ExceptionOccurred;

                    // Replaced: VIMTafidpoareturnclmsInfo = VEISafidpoareturnInfo
                    if (findAllFiduciaryPoaResponse.VEISafidpoareturnInfo != null)
                    {
                        System.Collections.Generic.List<UDOcreatePastFiduciariesMultipleResponse> UDOcreatePastFiduciariesArray = new System.Collections.Generic.List<UDOcreatePastFiduciariesMultipleResponse>();
                        var shrinqfPersonOrg3 = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoapastFiduciariesInfo;
                        if (shrinqfPersonOrg3 != null)
                        {
                            foreach (var shrinqfPersonOrg3Item in shrinqfPersonOrg3)
                            {
                                var responseIds = new UDOcreatePastFiduciariesMultipleResponse();
                                //instantiate the new Entity
                                Entity thisNewEntity = new Entity();
                                thisNewEntity.LogicalName = "udo_pastfiduciary";
                                if (request.ownerId != System.Guid.Empty)
                                {
                                    thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                }
                                else
                                {
                                    LogHelper.LogError(request.OrganizationName, request.UserId, "Create FID", "No Owner");
                                }
                                if (shrinqfPersonOrg3Item.mcs_personOrgName != string.Empty)
                                {
                                    thisNewEntity["udo_name"] = shrinqfPersonOrg3Item.mcs_personOrgName;
                                }
                                if (shrinqfPersonOrg3Item.mcs_veteranPtcpntID != string.Empty)
                                {
                                    thisNewEntity["udo_vetptcpntid"] = shrinqfPersonOrg3Item.mcs_veteranPtcpntID;
                                }
                                if (!string.IsNullOrEmpty(shrinqfPersonOrg3Item.mcs_personOrganizationCode))
                                {
                                    thisNewEntity["udo_cd"] = shrinqfPersonOrg3Item.mcs_personOrganizationCode;
                                }
                                if (shrinqfPersonOrg3Item.mcs_temporaryCustodianIndicator != string.Empty)
                                {
                                    thisNewEntity["udo_tempcustodian"] = shrinqfPersonOrg3Item.mcs_temporaryCustodianIndicator;
                                }
                                if (shrinqfPersonOrg3Item.mcs_statusCode != string.Empty)
                                {
                                    thisNewEntity["udo_status"] = shrinqfPersonOrg3Item.mcs_statusCode;
                                }
                                if (shrinqfPersonOrg3Item.mcs_relationshipName != string.Empty)
                                {
                                    thisNewEntity["udo_relationship"] = shrinqfPersonOrg3Item.mcs_relationshipName;
                                }
                                if (shrinqfPersonOrg3Item.mcs_rateName != string.Empty)
                                {
                                    thisNewEntity["udo_rate"] = shrinqfPersonOrg3Item.mcs_rateName;
                                }
                                if (shrinqfPersonOrg3Item.mcs_prepositionalPhraseName != string.Empty)
                                {
                                    thisNewEntity["udo_phrase"] = shrinqfPersonOrg3Item.mcs_prepositionalPhraseName;
                                }
                                if (shrinqfPersonOrg3Item.mcs_personOrgPtcpntID != string.Empty)
                                {
                                    thisNewEntity["udo_personorgptcpnt"] = shrinqfPersonOrg3Item.mcs_personOrgPtcpntID;
                                }
                                if (shrinqfPersonOrg3Item.mcs_personOrgName != string.Empty)
                                {
                                    thisNewEntity["udo_name"] = shrinqfPersonOrg3Item.mcs_personOrgName;
                                }
                                if (shrinqfPersonOrg3Item.mcs_personOrganizationName != string.Empty)
                                {
                                    thisNewEntity["udo_personorgname"] = shrinqfPersonOrg3Item.mcs_personOrganizationName;
                                }
                                if (shrinqfPersonOrg3Item.mcs_journalUserID != string.Empty)
                                {
                                    thisNewEntity["udo_jrnuser"] = shrinqfPersonOrg3Item.mcs_journalUserID;
                                }
                                if (shrinqfPersonOrg3Item.mcs_journalStatusTypeCode != string.Empty)
                                {
                                    thisNewEntity["udo_jrnstatus"] = shrinqfPersonOrg3Item.mcs_journalStatusTypeCode;
                                }
                                if (shrinqfPersonOrg3Item.mcs_journalObjectID != string.Empty)
                                {
                                    thisNewEntity["udo_jrnobj"] = shrinqfPersonOrg3Item.mcs_journalObjectID;
                                }
                                if (shrinqfPersonOrg3Item.mcs_journalLocationID != string.Empty)
                                {
                                    thisNewEntity["udo_jrnloc"] = shrinqfPersonOrg3Item.mcs_journalLocationID;
                                }
                                if (shrinqfPersonOrg3Item.mcs_journalDate != string.Empty)
                                {
                                    DateTime newDateTime;
                                    if (DateTime.TryParse(shrinqfPersonOrg3Item.mcs_journalDate, out newDateTime))
                                    {
                                        thisNewEntity["udo_jrndate"] = newDateTime;
                                    }
                                }
                                if (shrinqfPersonOrg3Item.mcs_healthcareProviderReleaseIndicator != string.Empty)
                                {
                                    thisNewEntity["udo_hcproviderrelease"] = shrinqfPersonOrg3Item.mcs_healthcareProviderReleaseIndicator;
                                }
                                if (!string.IsNullOrEmpty(shrinqfPersonOrg3Item.mcs_personOrOrganizationIndicator))
                                {
                                    thisNewEntity["udo_personorg"] = shrinqfPersonOrg3Item.mcs_personOrOrganizationIndicator;
                                }
                                if (shrinqfPersonOrg3Item.mcs_eventDate != string.Empty)
                                {
                                    DateTime newDateTime;
                                    if (DateTime.TryParse(shrinqfPersonOrg3Item.mcs_eventDate, out newDateTime))
                                    {
                                        thisNewEntity["udo_eventdate"] = newDateTime;
                                    }
                                }
                                if (shrinqfPersonOrg3Item.mcs_endDate != string.Empty)
                                {
                                    DateTime newDateTime;
                                    if (DateTime.TryParse(shrinqfPersonOrg3Item.mcs_endDate, out newDateTime))
                                    {
                                        thisNewEntity["udo_enddate"] = newDateTime;
                                    }
                                }
                                if (shrinqfPersonOrg3Item.mcs_beginDate != string.Empty)
                                {
                                    thisNewEntity["udo_begindate"] = shrinqfPersonOrg3Item.mcs_beginDate;
                                }
                                if (shrinqfPersonOrg3Item.mcs_personOrgAttentionText != string.Empty)
                                {
                                    thisNewEntity["udo_attn"] = shrinqfPersonOrg3Item.mcs_personOrgAttentionText;
                                }
                                if (request.UDOcreatePastFiduciariesRelatedEntitiesInfo != null)
                                {
                                    foreach (var relatedItem in request.UDOcreatePastFiduciariesRelatedEntitiesInfo)
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

                  
                        }
                    }
                    string logInfo = string.Format("Past Fiduciary Records queued to Create: {0}", requestCollection.Count());
                    LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Past Fiduciary Records Queued", logInfo);

                    if (findAllFiduciaryPoaResponse.VEISafidpoareturnInfo != null)
                    {
                        System.Collections.Generic.List<UDOcreatePastPOAMultipleResponse> UDOcreatePastPOAArray = new System.Collections.Generic.List<UDOcreatePastPOAMultipleResponse>();
                        var shrinqfPersonOrg4 = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoapastPowerOfAttorneysInfo;
                        if (shrinqfPersonOrg4 != null)
                        {
                            foreach (var shrinqfPersonOrg4Item in shrinqfPersonOrg4)
                            {
                                var responseIds = new UDOcreatePastPOAMultipleResponse();
                                //instantiate the new Entity
                                Entity thisNewEntity = new Entity();
                                thisNewEntity.LogicalName = "udo_pastpoa";
                                if (request.ownerId != System.Guid.Empty)
                                {
                                    thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                }
                                else
                                {
                                    LogHelper.LogError(request.OrganizationName, request.UserId, "Create POA", "No Owner");
                                }

                                if (shrinqfPersonOrg4Item.mcs_veteranPtcpntID != string.Empty)
                                {
                                    thisNewEntity["udo_vetptcpntid"] = shrinqfPersonOrg4Item.mcs_veteranPtcpntID;
                                }
                                if (shrinqfPersonOrg4Item.mcs_temporaryCustodianIndicator != string.Empty)
                                {
                                    thisNewEntity["udo_tempcustodian"] = shrinqfPersonOrg4Item.mcs_temporaryCustodianIndicator;
                                }
                                if (shrinqfPersonOrg4Item.mcs_statusCode != string.Empty)
                                {
                                    thisNewEntity["udo_status"] = shrinqfPersonOrg4Item.mcs_statusCode;
                                }
                                if (shrinqfPersonOrg4Item.mcs_relationshipName != string.Empty)
                                {
                                    thisNewEntity["udo_relationship"] = shrinqfPersonOrg4Item.mcs_relationshipName;
                                }
                                if (shrinqfPersonOrg4Item.mcs_rateName != string.Empty)
                                {
                                    thisNewEntity["udo_rate"] = shrinqfPersonOrg4Item.mcs_rateName;
                                }
                                if (shrinqfPersonOrg4Item.mcs_prepositionalPhraseName != string.Empty)
                                {
                                    thisNewEntity["udo_phase"] = shrinqfPersonOrg4Item.mcs_prepositionalPhraseName;
                                }
                                if (shrinqfPersonOrg4Item.mcs_personOrgPtcpntID != string.Empty)
                                {
                                    thisNewEntity["udo_personorgptcpnt"] = shrinqfPersonOrg4Item.mcs_personOrgPtcpntID;
                                }
                                if (shrinqfPersonOrg4Item.mcs_personOrganizationName != string.Empty)
                                {
                                    thisNewEntity["udo_personorgname"] = shrinqfPersonOrg4Item.mcs_personOrganizationName;
                                }
                                if (shrinqfPersonOrg4Item.mcs_personOrgName != string.Empty)
                                {
                                    thisNewEntity["udo_name"] = shrinqfPersonOrg4Item.mcs_personOrgName;
                                }
                                if (shrinqfPersonOrg4Item.mcs_personOrganizationCode != string.Empty)
                                {
                                    thisNewEntity["udo_personorg"] = shrinqfPersonOrg4Item.mcs_personOrganizationCode;
                                }
                                if (shrinqfPersonOrg4Item.mcs_journalUserID != string.Empty)
                                {
                                    thisNewEntity["udo_jrnuser"] = shrinqfPersonOrg4Item.mcs_journalUserID;
                                }
                                if (shrinqfPersonOrg4Item.mcs_journalStatusTypeCode != string.Empty)
                                {
                                    thisNewEntity["udo_jrnstatus"] = shrinqfPersonOrg4Item.mcs_journalStatusTypeCode;
                                }
                                if (shrinqfPersonOrg4Item.mcs_journalObjectID != string.Empty)
                                {
                                    thisNewEntity["udo_jrnobj"] = shrinqfPersonOrg4Item.mcs_journalObjectID;
                                }
                                if (shrinqfPersonOrg4Item.mcs_journalLocationID != string.Empty)
                                {
                                    thisNewEntity["udo_jrnloc"] = shrinqfPersonOrg4Item.mcs_journalLocationID;
                                }
                                if (shrinqfPersonOrg4Item.mcs_journalDate != string.Empty)
                                {
                                    DateTime newDateTime;
                                    if (DateTime.TryParse(shrinqfPersonOrg4Item.mcs_journalDate, out newDateTime))
                                    {
                                        thisNewEntity["udo_jrndate"] = newDateTime;
                                    }
                                }
                                if (shrinqfPersonOrg4Item.mcs_healthcareProviderReleaseIndicator != string.Empty)
                                {
                                    thisNewEntity["udo_hcproviderrelease"] = shrinqfPersonOrg4Item.mcs_healthcareProviderReleaseIndicator;
                                }
                                if (shrinqfPersonOrg4Item.mcs_eventDate != string.Empty)
                                {
                                    DateTime newDateTime;
                                    if (DateTime.TryParse(shrinqfPersonOrg4Item.mcs_eventDate, out newDateTime))
                                    {
                                        thisNewEntity["udo_eventdate"] = newDateTime;
                                    }
                                }
                                if (shrinqfPersonOrg4Item.mcs_endDate != string.Empty)
                                {
                                    DateTime newDateTime;
                                    if (DateTime.TryParse(shrinqfPersonOrg4Item.mcs_endDate, out newDateTime))
                                    {
                                        thisNewEntity["udo_enddate"] = newDateTime;
                                    }
                                }
                                if (shrinqfPersonOrg4Item.mcs_beginDate != string.Empty)
                                {
                                    DateTime newDateTime;
                                    if (DateTime.TryParse(shrinqfPersonOrg4Item.mcs_beginDate, out newDateTime))
                                    {
                                        thisNewEntity["udo_begindate"] = newDateTime;
                                    }
                                }
                                if (shrinqfPersonOrg4Item.mcs_personOrgAttentionText != string.Empty)
                                {
                                    thisNewEntity["udo_attn"] = shrinqfPersonOrg4Item.mcs_personOrgAttentionText;
                                }
                                if (request.UDOcreatePastFiduciariesRelatedEntitiesInfo != null)
                                {
                                    foreach (var relatedItem in request.UDOcreatePastFiduciariesRelatedEntitiesInfo)
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
                        }
                    }
                    logInfo = string.Format("Past POA Records queued to Create: {0}", requestCollection.Count());
                    LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Past POA Records Queued", logInfo);

                    LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, "starting on contact");
                    //RC new code - 09/2021 - feature_3023
                    if ((request.VeteranId != null) || (request.VeteranId != Guid.Empty))
                    {
                        //means we are here for the Veteran, not the dependent

                        Entity thisContact = new Entity();
                        thisContact.LogicalName = "contact";
                        thisContact.Id = request.VeteranId;

                        var dataExisted = false;
                        if (findAllFiduciaryPoaResponse.VEISafidpoareturnInfo != null)
                        {
                            var currentFiduciary = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentFiduciaryInfo;
                            if (currentFiduciary != null)
                            {
                                dataExisted = true;
                                #region map current FID data

                                if (!string.IsNullOrEmpty(currentFiduciary.mcs_personOrgName))
                                {
                                    //response.UDOgetContactRecordsInfo.udo_cfid2PersonOrgName =
                                    //response.UDOgetContactRecordsInfo.udo_FiduciaryAppointed =
                                    //currentFiduciary.mcs_personOrgName.TrimWhiteSpace();
                                    thisContact["udo_cfid2personorgname"] = currentFiduciary.mcs_personOrgName.TrimWhiteSpace();
                                    thisContact["udo_fiduciaryappointed"] = currentFiduciary.mcs_personOrgName.TrimWhiteSpace();
                                }
                                else
                                {
                                    thisContact["udo_cfid2personorgname"] = null;
                                    thisContact["udo_fiduciaryappointed"] = null;
                                }
                                if (!string.IsNullOrEmpty(currentFiduciary.mcs_prepositionalPhraseName))
                                {
                                    //response.UDOgetContactRecordsInfo.udo_cfidPrepositionalPhrase = currentFiduciary.mcs_prepositionalPhraseName;
                                    thisContact["udo_cfidprepositionalphrase"] = currentFiduciary.mcs_prepositionalPhraseName;
                                }
                                else
                                {
                                    thisContact["udo_cfidprepositionalphrase"] = null;

                                }
                                if (!string.IsNullOrEmpty(currentFiduciary.mcs_personOrganizationName))
                                {
                                    //response.UDOgetContactRecordsInfo.udo_personOrganizationName = currentFiduciary.mcs_personOrganizationName.TrimWhiteSpace();
                                    thisContact["udo_cfid2personorganizationname"] = currentFiduciary.mcs_personOrganizationName.TrimWhiteSpace();
                                }
                                else
                                {
                                    thisContact["udo_cfid2personorganizationname"] = null;

                                }
                                if (!string.IsNullOrEmpty(currentFiduciary.mcs_beginDate))
                                {
                                    DateTime newDateTime;
                                    if (DateTime.TryParse(currentFiduciary.mcs_beginDate, out newDateTime))
                                    {
                                        //response.UDOgetContactRecordsInfo.udo_cfidBeginDate = currentFiduciary.mcs_beginDate;
                                        thisContact["udo_cfidbegindate"] = newDateTime;
                                    }
                                    else
                                    {
                                        thisContact["udo_cfidbegindate"] = null;

                                    }
                                }
                                else
                                {
                                    thisContact["udo_cfidbegindate"] = null;

                                }
                                if (!string.IsNullOrEmpty(currentFiduciary.mcs_endDate))
                                {
                                    DateTime newDateTime;
                                    if (DateTime.TryParse(currentFiduciary.mcs_endDate, out newDateTime))
                                    {
                                        //response.UDOgetContactRecordsInfo.udo_cfidEndDate = currentFiduciary.mcs_endDate;
                                        thisContact["udo_cfidenddate"] = newDateTime;
                                    }
                                    else
                                    {
                                        thisContact["udo_cfidenddate"] = null;

                                    }
                                }
                                else
                                {
                                    thisContact["udo_cfidenddate"] = null;

                                }
                                if (!string.IsNullOrEmpty(currentFiduciary.mcs_eventDate))
                                {
                                    DateTime newDateTime;
                                    if (DateTime.TryParse(currentFiduciary.mcs_eventDate, out newDateTime))
                                    {
                                        //response.UDOgetContactRecordsInfo.udo_cfidEventDate = currentFiduciary.mcs_eventDate;
                                        thisContact["udo_cfideventdate"] = newDateTime;
                                    }
                                    else
                                    {
                                        thisContact["udo_cfideventdate"] = null;
                                    }
                                }
                                else
                                {
                                    thisContact["udo_cfideventdate"] = null;
                                }
                                if (!string.IsNullOrEmpty(currentFiduciary.mcs_healthcareProviderReleaseIndicator))
                                {
                                    //Valide N is correct for this IND field
                                    var thisValue = currentFiduciary.mcs_healthcareProviderReleaseIndicator;

                                    if (thisValue == "N")
                                    {
                                        //response.UDOgetContactRecordsInfo.udo_cfidHCProviderRelease = false;
                                        thisContact["udo_cfidhcproviderreleasedrp"] = new OptionSetValue(752280000);
                                    }
                                    else
                                    {
                                        //response.UDOgetContactRecordsInfo.udo_cfidHCProviderRelease = true;
                                        thisContact["udo_cfidhcproviderreleasedrp"] = new OptionSetValue(752280001);
                                    }
                                    //response.UDOgetContactRecordsInfo.udo_cfidHCProviderReleaseSpecified = true;

                                }
                                else
                                {
                                    thisContact["udo_cfidhcproviderreleasedrp"] = null;

                                }
                                if (!string.IsNullOrEmpty(currentFiduciary.mcs_journalDate))
                                {
                                    DateTime newDateTime;
                                    if (DateTime.TryParse(currentFiduciary.mcs_journalDate, out newDateTime))
                                    {
                                        //response.UDOgetContactRecordsInfo.udo_cfidJrnDate = currentFiduciary.mcs_journalDate;
                                        thisContact["udo_cfidjrndate"] = newDateTime;
                                    }
                                    else
                                    {
                                        thisContact["udo_cfidjrndate"] = null;
                                    }
                                }
                                else
                                {
                                    thisContact["udo_cfidjrndate"] = null;
                                }
                                if (!string.IsNullOrEmpty(currentFiduciary.mcs_journalLocationID))
                                {
                                    //response.UDOgetContactRecordsInfo.udo_cfidJrnLocID = currentFiduciary.mcs_journalLocationID;
                                    thisContact["udo_cfidjrnlocid"] = currentFiduciary.mcs_journalLocationID;
                                }
                                else
                                {
                                    thisContact["udo_cfidjrnlocid"] = null;
                                }
                                if (!string.IsNullOrEmpty(currentFiduciary.mcs_journalObjectID))
                                {
                                    //response.UDOgetContactRecordsInfo.udo_cfidJrnObjID = currentFiduciary.mcs_journalObjectID;
                                    thisContact["udo_cfidjrnobjid"] = currentFiduciary.mcs_journalObjectID;
                                }
                                else
                                {
                                    thisContact["udo_cfidjrnobjid"] = null; 
                                }
                                if (!string.IsNullOrEmpty(currentFiduciary.mcs_journalStatusTypeCode))
                                {
                                    //response.UDOgetContactRecordsInfo.udo_cfidJrnStatusType = currentFiduciary.mcs_journalStatusTypeCode;
                                    thisContact["udo_cfidjrnstatustype"] = currentFiduciary.mcs_journalStatusTypeCode;
                                }
                                else
                                {
                                    thisContact["udo_cfidjrnstatustype"] = null;
                                }
                                if (!string.IsNullOrEmpty(currentFiduciary.mcs_personOrgAttentionText))
                                {
                                    //response.UDOgetContactRecordsInfo.udo_cfidPersonOrgAttn = currentFiduciary.mcs_personOrgAttentionText;
                                    thisContact["udo_cfidpersonorgattn"] = currentFiduciary.mcs_personOrgAttentionText;
                                }
                                else
                                {
                                    thisContact["udo_cfidpersonorgattn"] = null;
                                }
                                if (!string.IsNullOrEmpty(currentFiduciary.mcs_personOrganizationCode))
                                {
                                    //response.UDOgetContactRecordsInfo.udo_cfidPersonOrgCode = currentFiduciary.mcs_personOrganizationCode;
                                    thisContact["udo_cfidpersonorgcode"] = currentFiduciary.mcs_personOrganizationCode;
                                }
                                else
                                {
                                    thisContact["udo_cfidpersonorgcode"] = null;
                                }
                                if (!string.IsNullOrEmpty(currentFiduciary.mcs_personOrgName))
                                {
                                    //response.UDOgetContactRecordsInfo.udo_cfidPersonOrgName = currentFiduciary.mcs_personOrgName.TrimWhiteSpace();
                                    thisContact["udo_cfidpersonorgname"] = currentFiduciary.mcs_personOrgName.TrimWhiteSpace();
                                }
                                else
                                {
                                    thisContact["udo_cfidpersonorgname"] = null;
                                }
                                if (!string.IsNullOrEmpty(currentFiduciary.mcs_personOrgPtcpntID))
                                {
                                    //response.UDOgetContactRecordsInfo.udo_cfidPersonOrgParticipantID = currentFiduciary.mcs_personOrgPtcpntID;
                                    thisContact["udo_cfidpersonorgparticipantid"] = currentFiduciary.mcs_personOrgPtcpntID;
                                }
                                else
                                {
                                    thisContact["udo_cfidpersonorgparticipantid"] = null;
                                }
                                if (!string.IsNullOrEmpty(currentFiduciary.mcs_personOrOrganizationIndicator))
                                {
                                    //Valide N is correct for this IND field
                                    var thisValue = currentFiduciary.mcs_personOrOrganizationIndicator;

                                    if (thisValue == "O")
                                    {
                                        //response.UDOgetContactRecordsInfo.udo_cfidPersonorOrg = false;
                                        thisContact["udo_cfidpersonororgdrp"] = new OptionSetValue(752280001);
                                    }
                                    else
                                    {
                                        //response.UDOgetContactRecordsInfo.udo_cfidPersonorOrg = true;
                                        thisContact["udo_cfidpersonororgdrp"] = new OptionSetValue(752280000);
                                    }
                                    //response.UDOgetContactRecordsInfo.udo_cfidPersonorOrgSpecified = true;
                                }
                                else
                                {
                                    thisContact["udo_cfidpersonororgdrp"] = null;
                                }
                                if (!string.IsNullOrEmpty(currentFiduciary.mcs_rateName))
                                {
                                    //response.UDOgetContactRecordsInfo.udo_cfidRateName = currentFiduciary.mcs_rateName;
                                    thisContact["udo_cfidratename"] = currentFiduciary.mcs_rateName;
                                }
                                else
                                {
                                    thisContact["udo_cfidratename"] = null;
                                }
                                if (!string.IsNullOrEmpty(currentFiduciary.mcs_relationshipName))
                                {
                                    //response.UDOgetContactRecordsInfo.udo_cfidRelationship = currentFiduciary.mcs_relationshipName;
                                    thisContact["udo_cfidrelationship"] = currentFiduciary.mcs_relationshipName;
                                }
                                else
                                {
                                    thisContact["udo_cfidrelationship"] = null;
                                }
                                if (!string.IsNullOrEmpty(currentFiduciary.mcs_statusCode))
                                {
                                    //response.UDOgetContactRecordsInfo.udo_cfidStatus = currentFiduciary.mcs_statusCode;
                                    thisContact["udo_cfidstatus"] = currentFiduciary.mcs_statusCode;
                                }
                                else
                                {
                                    thisContact["udo_cfidstatus"] = null;
                                }
                                if (!string.IsNullOrEmpty(currentFiduciary.mcs_temporaryCustodianIndicator))
                                {
                                    //response.UDOgetContactRecordsInfo.udo_cfidTempCustodian = currentFiduciary.mcs_temporaryCustodianIndicator;
                                    thisContact["udo_cfidtempcustodian"] = currentFiduciary.mcs_temporaryCustodianIndicator;
                                }
                                else
                                {
                                    thisContact["udo_cfidtempcustodian"] = null;
                                }
                                if (!string.IsNullOrEmpty(currentFiduciary.mcs_veteranPtcpntID))
                                {
                                    //response.UDOgetContactRecordsInfo.udo_cfidVetPtcpntID = currentFiduciary.mcs_veteranPtcpntID;
                                    thisContact["udo_cfidvetptcpntid"] = currentFiduciary.mcs_veteranPtcpntID;
                                }
                                else
                                {
                                    thisContact["udo_cfidvetptcpntid"] = null;
                                }
                                #endregion
                            }
                            else
                            {
                                    LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, "NO currentFID");

                            }

                            //LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.Debug, request.UserId, Guid.Empty, null, null, "Contact:Done with FID", null, txnTimerconn.ElapsedMilliseconds);
                            var currentPOA = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo;
                            if (currentPOA != null)
                            {
                                LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, "got currentPOA");

                                dataExisted = true;
                                var mapPOA = true;
                                #region map current POA data
                                if (!string.IsNullOrEmpty(currentPOA.mcs_endDate))
                                {
                                    //if there is an end date, we only add end data to contact but we take this data
                                    //and create a past POA record 
                                    
                                    DateTime newDateTime;
                                    if (DateTime.TryParse(currentPOA.mcs_endDate, out newDateTime))
                                    {
                                        LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, "end date on current");
                                        //response.UDOgetContactRecordsInfo.udo_cpoaEndDate = currentPOA.mcs_endDate;
                                        thisContact["udo_cpoaenddate"] = newDateTime;
                                        mapPOA = false;
                                        thisContact["udo_poa"] = null;
                                        thisContact["udo_cpoabegindate"] = null;
                                        thisContact["udo_cpoaeventdate"] = null;
                                        thisContact["udo_cpoahcproviderreleasedrp"] = null;
                                        thisContact["udo_cpoajrndate"] = null;
                                        thisContact["udo_cpoapersonororgdrp"] = null;
                                        thisContact["udo_cpoajrnlocid"] = null;
                                        thisContact["udo_cpoajrnobjid"] = null;
                                        thisContact["udo_cpoajrnstatustype"] = null;
                                        thisContact["udo_cpoajrnuserid"] = null;
                                        thisContact["udo_cpoaorgpersonname"] = null;
                                        thisContact["udo_cpoapersonorgattn"] = null;
                                        thisContact["udo_cpoapersonorgcode"] = null;
                                        thisContact["udo_cpoapersonorgname"] = null;
                                        thisContact["udo_cpoapersonorgparticipantid"] = null;
                                        thisContact["udo_cpoaprepositionalphrase"] = null;
                                        thisContact["udo_cpoaratename"] = null;
                                        thisContact["udo_cpoarelationship"] = null;
                                        thisContact["udo_cpoastatus"] = null;
                                        thisContact["udo_cpoatempcustodian"] = null;
                                        thisContact["udo_cpoavetptcptid"] = null;

                                        #region pastPOA
                                        Entity thisNewEntity = new Entity();
                                        thisNewEntity.LogicalName = "udo_pastpoa";
                                        if (request.ownerId != System.Guid.Empty)
                                        {
                                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                        }
                                        else
                                        {
                                            LogHelper.LogError(request.OrganizationName, request.UserId, "Create POA", "No Owner");
                                        }

                                        if (currentPOA.mcs_veteranPtcpntID != string.Empty)
                                        {
                                            thisNewEntity["udo_vetptcpntid"] = currentPOA.mcs_veteranPtcpntID;
                                        }
                                        if (currentPOA.mcs_temporaryCustodianIndicator != string.Empty)
                                        {
                                            thisNewEntity["udo_tempcustodian"] = currentPOA.mcs_temporaryCustodianIndicator;
                                        }
                                        if (currentPOA.mcs_statusCode != string.Empty)
                                        {
                                            thisNewEntity["udo_status"] = currentPOA.mcs_statusCode;
                                        }
                                        if (currentPOA.mcs_relationshipName != string.Empty)
                                        {
                                            thisNewEntity["udo_relationship"] = currentPOA.mcs_relationshipName;
                                        }
                                        if (currentPOA.mcs_rateName != string.Empty)
                                        {
                                            thisNewEntity["udo_rate"] = currentPOA.mcs_rateName;
                                        }
                                        if (currentPOA.mcs_prepositionalPhraseName != string.Empty)
                                        {
                                            thisNewEntity["udo_phase"] = currentPOA.mcs_prepositionalPhraseName;
                                        }
                                        if (currentPOA.mcs_personOrgPtcpntID != string.Empty)
                                        {
                                            thisNewEntity["udo_personorgptcpnt"] = currentPOA.mcs_personOrgPtcpntID;
                                        }
                                        if (currentPOA.mcs_personOrganizationName != string.Empty)
                                        {
                                            thisNewEntity["udo_personorgname"] = currentPOA.mcs_personOrganizationName;
                                        }
                                        if (currentPOA.mcs_personOrgName != string.Empty)
                                        {
                                            thisNewEntity["udo_name"] = currentPOA.mcs_personOrgName;
                                        }
                                        if (currentPOA.mcs_personOrganizationCode != string.Empty)
                                        {
                                            thisNewEntity["udo_personorg"] = currentPOA.mcs_personOrganizationCode;
                                        }
                                        if (currentPOA.mcs_journalUserID != string.Empty)
                                        {
                                            thisNewEntity["udo_jrnuser"] = currentPOA.mcs_journalUserID;
                                        }
                                        if (currentPOA.mcs_journalStatusTypeCode != string.Empty)
                                        {
                                            thisNewEntity["udo_jrnstatus"] = currentPOA.mcs_journalStatusTypeCode;
                                        }
                                        if (currentPOA.mcs_journalObjectID != string.Empty)
                                        {
                                            thisNewEntity["udo_jrnobj"] = currentPOA.mcs_journalObjectID;
                                        }
                                        if (currentPOA.mcs_journalLocationID != string.Empty)
                                        {
                                            thisNewEntity["udo_jrnloc"] = currentPOA.mcs_journalLocationID;
                                        }
                                        if (currentPOA.mcs_journalDate != string.Empty)
                                        {
                                            DateTime newDateTime2;
                                            if (DateTime.TryParse(currentPOA.mcs_journalDate, out newDateTime2))
                                            {
                                                thisNewEntity["udo_jrndate"] = newDateTime2;
                                            }
                                        }
                                        if (currentPOA.mcs_healthcareProviderReleaseIndicator != string.Empty)
                                        {
                                            thisNewEntity["udo_hcproviderrelease"] = currentPOA.mcs_healthcareProviderReleaseIndicator;
                                        }
                                        if (currentPOA.mcs_eventDate != string.Empty)
                                        {
                                            DateTime newDateTime2;
                                            if (DateTime.TryParse(currentPOA.mcs_eventDate, out newDateTime2))
                                            {
                                                thisNewEntity["udo_eventdate"] = newDateTime2;
                                            }
                                        }
                                        if (currentPOA.mcs_endDate != string.Empty)
                                        {
                                            DateTime newDateTime2;
                                            if (DateTime.TryParse(currentPOA.mcs_endDate, out newDateTime2))
                                            {
                                                thisNewEntity["udo_enddate"] = newDateTime2;
                                            }
                                        }
                                        if (currentPOA.mcs_beginDate != string.Empty)
                                        {
                                            DateTime newDateTime2;
                                            if (DateTime.TryParse(currentPOA.mcs_beginDate, out newDateTime2))
                                            {
                                                thisNewEntity["udo_begindate"] = newDateTime2;
                                            }
                                        }
                                        if (currentPOA.mcs_personOrgAttentionText != string.Empty)
                                        {
                                            thisNewEntity["udo_attn"] = currentPOA.mcs_personOrgAttentionText;
                                        }
                                        if (request.UDOcreatePastFiduciariesRelatedEntitiesInfo != null)
                                        {
                                            foreach (var relatedItem in request.UDOcreatePastFiduciariesRelatedEntitiesInfo)
                                            {
                                                thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                            }
                                        }

                                        CreateRequest createData = new CreateRequest
                                        {
                                            Target = thisNewEntity
                                        };
                                        requestCollection.Add(createData);
                                        #endregion
                                    }
                                    else
                                    {
                                        thisContact["udo_cpoaenddate"] =  null; 
                                    }
                                }
                                else
                                {
                                    thisContact["udo_cpoaenddate"] =  null;
                                    LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, "no end date on current");
                                }
                                if (mapPOA)
                                {
                                    if (!string.IsNullOrEmpty(currentPOA.mcs_beginDate))
                                    {
                                        DateTime newDateTime;
                                        if (DateTime.TryParse(currentPOA.mcs_beginDate, out newDateTime))
                                        {
                                            //response.UDOgetContactRecordsInfo.udo_cpoaBeginDate = currentPOA.mcs_beginDate;
                                            thisContact["udo_cpoabegindate"] = newDateTime;
                                        }
                                        else
                                        {
                                            thisContact["udo_cpoabegindate"] = null;
                                        }
                                    }
                                    else
                                    {
                                        thisContact["udo_cpoabegindate"] = null;
                                    }

                                    if (!string.IsNullOrEmpty(currentPOA.mcs_eventDate))
                                    {
                                        DateTime newDateTime;
                                        if (DateTime.TryParse(currentPOA.mcs_eventDate, out newDateTime))
                                        {
                                            //response.UDOgetContactRecordsInfo.udo_cpoaEventDate = currentPOA.mcs_eventDate;
                                            thisContact["udo_cpoaeventdate"] = newDateTime;    
                                        }
                                        else
                                        {
                                            thisContact["udo_cpoaeventdate"] = null;
                                        }

                                    }
                                    else
                                    {
                                        thisContact["udo_cpoaeventdate"] = null;
                                    }
                                    if (!string.IsNullOrEmpty(currentPOA.mcs_healthcareProviderReleaseIndicator))
                                    {
                                        //Valide N is correct for this IND field
                                        var thisValue = currentPOA.mcs_healthcareProviderReleaseIndicator;

                                        if (thisValue == "N")
                                        {
                                            //response.UDOgetContactRecordsInfo.udo_cpoaHCProviderRelease = false;
                                            thisContact["udo_cpoahcproviderreleasedrp"] = new OptionSetValue(752280001);
                                        }
                                        else
                                        {
                                            //response.UDOgetContactRecordsInfo.udo_cpoaHCProviderRelease = true;
                                            thisContact["udo_cpoahcproviderreleasedrp"] = new OptionSetValue(752280000);
                                        }
                                        //response.UDOgetContactRecordsInfo.udo_cpoaHCProviderReleaseSpecified = true;
                                    }
                                    else
                                    {
                                        thisContact["udo_cpoahcproviderreleasedrp"] = null;
                                    }

                                    if (!string.IsNullOrEmpty(currentPOA.mcs_journalDate))
                                    {
                                        DateTime newDateTime;
                                        if (DateTime.TryParse(currentPOA.mcs_journalDate, out newDateTime))
                                        {
                                            //response.UDOgetContactRecordsInfo.udo_cpoaJrnDate = currentPOA.mcs_journalDate;
                                            thisContact["udo_cpoajrndate"] = newDateTime;
                                        }
                                        else
                                        {
                                            thisContact["udo_cpoajrndate"] = null;
                                        }
                                    }
                                    else
                                    {
                                        thisContact["udo_cpoajrndate"] = null;
                                    }
                                    if (!string.IsNullOrEmpty(currentPOA.mcs_journalLocationID))
                                    {
                                        //response.UDOgetContactRecordsInfo.udo_cpoaJrnLocID = currentPOA.mcs_journalLocationID;
                                        thisContact["udo_cpoajrnlocid"] = currentPOA.mcs_journalLocationID;
                                    }
                                    else
                                    {
                                        thisContact["udo_cpoajrnlocid"] = null;
                                    }
                                    if (!string.IsNullOrEmpty(currentPOA.mcs_journalObjectID))
                                    {
                                        //response.UDOgetContactRecordsInfo.udo_cpoaJrnObjID = currentPOA.mcs_journalObjectID;
                                        thisContact["udo_cpoajrnobjid"] = currentPOA.mcs_journalObjectID;
                                    }
                                    else
                                    {
                                        thisContact["udo_cpoajrnobjid"] = null;
                                    }
                                    if (!string.IsNullOrEmpty(currentPOA.mcs_journalStatusTypeCode))
                                    {
                                        //response.UDOgetContactRecordsInfo.udo_cpoaJrnStatusType = currentPOA.mcs_journalStatusTypeCode;
                                        thisContact["udo_cpoajrnstatustype"] = currentPOA.mcs_journalStatusTypeCode;
                                    }
                                    else
                                    {
                                        thisContact["udo_cpoajrnstatustype"] = null;
                                    }
                                    if (!string.IsNullOrEmpty(currentPOA.mcs_journalUserID))
                                    {
                                        //response.UDOgetContactRecordsInfo.udo_cpoaJrnUserID = currentPOA.mcs_journalUserID;
                                        thisContact["udo_cpoajrnuserid"] = currentPOA.mcs_journalUserID;
                                    }
                                    else
                                    {
                                        thisContact["udo_cpoajrnuserid"] = null;
                                    }
                                    if (!string.IsNullOrEmpty(currentPOA.mcs_personOrgName))
                                    {
                                        //response.UDOgetContactRecordsInfo.udo_cpoaOrgPersonName = currentPOA.mcs_personOrgName;
                                        thisContact["udo_cpoaorgpersonname"] = currentPOA.mcs_personOrgName;
                                    }
                                    else
                                    {
                                        thisContact["udo_cpoaorgpersonname"] = null;
                                    }
                                    if (!string.IsNullOrEmpty(currentPOA.mcs_personOrgAttentionText))
                                    {
                                        //response.UDOgetContactRecordsInfo.udo_cpoaPersonOrgAttn = currentPOA.mcs_personOrgAttentionText;
                                        thisContact["udo_cpoapersonorgattn"] = currentPOA.mcs_personOrgAttentionText;
                                    }
                                    else
                                    {
                                        thisContact["udo_cpoapersonorgattn"] = null;
                                    }
                                    if (!string.IsNullOrEmpty(currentPOA.mcs_personOrganizationCode))
                                    {
                                        //response.UDOgetContactRecordsInfo.udo_cpoaPersonOrgCode = currentPOA.mcs_personOrganizationCode;
                                        thisContact["udo_cpoapersonorgcode"] = currentPOA.mcs_personOrganizationCode;
                                    }
                                    else
                                    {
                                        thisContact["udo_cpoapersonorgcode"] = null;
                                    }
                                    if (!string.IsNullOrEmpty(currentPOA.mcs_personOrganizationName))
                                    {
                                        //response.UDOgetContactRecordsInfo.udo_cpoaPersonOrgName = currentPOA.mcs_personOrganizationName;
                                        thisContact["udo_cpoapersonorgname"] = currentPOA.mcs_personOrganizationName;
                                    }
                                    else
                                    {
                                        thisContact["udo_cpoapersonorgname"] = null;
                                    }
                                    if (!string.IsNullOrEmpty(currentPOA.mcs_personOrgPtcpntID))
                                    {
                                        //response.UDOgetContactRecordsInfo.udo_cpoaPersonOrgParticipantID = currentPOA.mcs_personOrgPtcpntID;
                                        thisContact["udo_cpoapersonorgparticipantid"] = currentPOA.mcs_personOrgPtcpntID;
                                    }
                                    else
                                    {
                                        thisContact["udo_cpoapersonorgparticipantid"] = null;
                                    }
                                    if (!string.IsNullOrEmpty(currentPOA.mcs_personOrOrganizationIndicator))
                                    {
                                        //Valide N is correct for this IND field
                                        var thisValue = currentPOA.mcs_personOrOrganizationIndicator;

                                        if (thisValue == "O")
                                        {
                                            //response.UDOgetContactRecordsInfo.udo_cpoaPersonOrOrg = false;
                                            thisContact["udo_cpoapersonororgdrp"] = new OptionSetValue(752280001);
                                        }
                                        else
                                        {
                                            //response.UDOgetContactRecordsInfo.udo_cpoaPersonOrOrg = true;
                                            thisContact["udo_cpoapersonororgdrp"] = new OptionSetValue(752280000);
                                        }
                                        LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, "updatedpersonorg:" + thisValue);
                                        //response.UDOgetContactRecordsInfo.udo_cpoaPersonOrOrgSpecified = true;
                                    }
                                    else
                                    {
                                        thisContact["udo_cpoapersonororgdrp"] = null;
                                    }
                                    if (!string.IsNullOrEmpty(currentPOA.mcs_prepositionalPhraseName))
                                    {
                                        //response.UDOgetContactRecordsInfo.udo_cpoaPrepositionalPhrase = currentPOA.mcs_prepositionalPhraseName;
                                        thisContact["udo_cpoaprepositionalphrase"] = currentPOA.mcs_prepositionalPhraseName;
                                    }
                                    else
                                    {
                                        thisContact["udo_cpoaprepositionalphrase"] = null;
                                    }
                                    if (!string.IsNullOrEmpty(currentPOA.mcs_rateName))
                                    {
                                        //response.UDOgetContactRecordsInfo.udo_cpoaRateName = currentPOA.mcs_rateName;
                                        thisContact["udo_cpoaratename"] = currentPOA.mcs_rateName;
                                    }
                                    else
                                    {
                                        thisContact["udo_cpoaratename"] = null;
                                    }
                                    if (!string.IsNullOrEmpty(currentPOA.mcs_relationshipName))
                                    {
                                        //response.UDOgetContactRecordsInfo.udo_cpoaRelationship = currentPOA.mcs_relationshipName;
                                        thisContact["udo_cpoarelationship"] = currentPOA.mcs_relationshipName;
                                    }
                                    else
                                    {
                                        thisContact["udo_cpoarelationship"] = null;
                                    }
                                    if (!string.IsNullOrEmpty(currentPOA.mcs_statusCode))
                                    {
                                        //response.UDOgetContactRecordsInfo.udo_cpoaStatus = currentPOA.mcs_statusCode;
                                        thisContact["udo_cpoastatus"] = currentPOA.mcs_statusCode;
                                    }
                                    else
                                    {
                                        thisContact["udo_cpoastatus"] = null;
                                    }
                                    if (!string.IsNullOrEmpty(currentPOA.mcs_temporaryCustodianIndicator))
                                    {
                                        //response.UDOgetContactRecordsInfo.udo_cpoaTempCustodian = currentPOA.mcs_temporaryCustodianIndicator;
                                        thisContact["udo_cpoatempcustodian"] = currentPOA.mcs_temporaryCustodianIndicator;
                                    }
                                    else
                                    {
                                        thisContact["udo_cpoatempcustodian"] = null;
                                    }
                                    if (!string.IsNullOrEmpty(currentPOA.mcs_veteranPtcpntID))
                                    {
                                        //response.UDOgetContactRecordsInfo.udo_cpoaVetPtcptID = currentPOA.mcs_veteranPtcpntID;
                                        thisContact["udo_cpoavetptcptid"] = currentPOA.mcs_veteranPtcpntID;
                                    }
                                    else
                                    {
                                        thisContact["udo_cpoavetptcptid"] = null;
                                    }
                                   
                                }
                                #endregion
                            }
                            else
                            {
                                LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, "NO currentPOA");

                            }
                            thisContact["udo_fiduciarycomplete"] = true;
                            
                            //OrgServiceProxy.Update(TruncateHelper.TruncateFields(request.MessageId, thisContact, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                            var updateRequest = new UpdateRequest()
                            {
                                Target = thisContact
                            };
                            requestCollection.Add(updateRequest);
                            LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, "Added contact to update");
                            //LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.Debug, request.UserId, Guid.Empty, null, null, "Contact:Done with POA", null, txnTimerconn.ElapsedMilliseconds);
                        }
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
                    logInfo = string.Format("Contact Update, Past Fiduciary/POA Records Created: {0}", requestCollection.Count()-1);
                    LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Past Fiduciary Records Created", logInfo);

                    #endregion
                }

                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "UDOcreatePastFiduciariesProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Map EC data to LOB";
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
