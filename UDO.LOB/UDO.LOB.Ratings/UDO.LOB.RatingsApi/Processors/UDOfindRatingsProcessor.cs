using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.Ratings.Messages;
using VEIS.Core.Messages;
using VEIS.Messages.RatingService;

/// <summary>
/// VIMT LOB Component for UDOUDOcreateSMCRatings,UDOcreateSMCRatings method, Processor.
/// </summary>
namespace UDO.LOB.Ratings.Processors
{
    public class UDOfindRatingsProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOfindRatingsProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOfindRatingsRequest request)
        {
            UDOfindRatingsResponse response = new UDOfindRatingsResponse();
            response.MessageId = request.MessageId;
            if (request.DiagnosticsContext == null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = method,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA",
                    OrganizationName = request.OrganizationName
                };
            }

            TraceLogger tLogger = new TraceLogger(method, request);
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccurred = true;
                return response;
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
                response.ExceptionMessage = $" {method}: Failed to get CRMConnection";
                response.ExceptionOccurred = true;
                return response;
            }
            #endregion
            tLogger.LogEvent("Connected to CRM", "001");

            progressString = "After Connection";

            try
            {
                var findRatingDataRequest = new VEISfnrtngdtfindRatingDataRequest();
                findRatingDataRequest.MessageId = request.MessageId;
                findRatingDataRequest.LogTiming = request.LogTiming;
                findRatingDataRequest.LogSoap = request.LogSoap;
                findRatingDataRequest.Debug = request.Debug;
                findRatingDataRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findRatingDataRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findRatingDataRequest.RelatedParentId = request.RelatedParentId;
                findRatingDataRequest.UserId = request.UserId;
                findRatingDataRequest.OrganizationName = request.OrganizationName;
                findRatingDataRequest.mcs_filenumber = request.fileNumber;
                findRatingDataRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo 
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
                progressString = "Request FN# is: " + findRatingDataRequest.mcs_filenumber + ", Request Org is: " + findRatingDataRequest.OrganizationName + ", Request User ID is: " + findRatingDataRequest.UserId;

                var findRatingDataResponse = WebApiUtility.SendReceive<VEISfnrtngdtfindRatingDataResponse>(findRatingDataRequest, WebApiType.VEIS);
                if (request.LogSoap || findRatingDataResponse.ExceptionOccurred)
                {
                    if (findRatingDataResponse.SerializedSOAPRequest != null || findRatingDataResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findRatingDataResponse.SerializedSOAPRequest + findRatingDataResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfnrtngdtfindRatingDataRequest Request/Response {requestResponse}", true);
                    }
                }

                tLogger.LogEvent("Web Service Call VEISfnrtngdtfindRatingDataResponse", "002");
                progressString = "After VEIS EC Call";

                response.ExceptionMessage = findRatingDataResponse.ExceptionMessage;
                response.ExceptionOccurred = findRatingDataResponse.ExceptionOccurred;
                response.MessageId = request.MessageId;

                progressString = "Beginning Creation of Child Records";
                if (findRatingDataResponse != null)
                {
                    response.ExceptionMessage = findRatingDataResponse.ExceptionMessage;
                    response.ExceptionOccurred = findRatingDataResponse.ExceptionOccurred;

                    if (findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo != null)
                    {
                        var responseIds = new UDOfindRatingsResponse();
                        var requestCollection = new OrganizationRequestCollection();


                        var smcRatingsCount = 0;
                        #region SMC Ratings
                        if (findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtspecialMonthlyCompensationRatingRecordInfo != null)
                        {
                            if (findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtspecialMonthlyCompensationRatingRecordInfo.VEISVIMTfnrtngdtsmcRatingsInfo != null)
                            {
                                var smcRatings = findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtspecialMonthlyCompensationRatingRecordInfo.VEISVIMTfnrtngdtsmcRatingsInfo;
                                foreach (var rating in smcRatings)
                                {
                                    Entity thisNewEntity = new Entity();
                                    thisNewEntity.LogicalName = "udo_smcrating";

                                    if (request.ownerId != System.Guid.Empty)
                                    {
                                        thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                    }
                                    thisNewEntity["udo_name"] = "SMC Rating Summary";

                                    if (!String.IsNullOrEmpty(rating.mcs_anatomicalLossTypeName))
                                    {
                                        thisNewEntity["udo_anatomicalloss"] = rating.mcs_anatomicalLossTypeName;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_beginDate))
                                    {
                                        thisNewEntity["udo_begindate"] = dateStringFormat(rating.mcs_beginDate);
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_hospitalSMCTypeName))
                                    {
                                        thisNewEntity["udo_hospitalsmc"] = rating.mcs_hospitalSMCTypeName;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_lossUseTypeName))
                                    {
                                        thisNewEntity["udo_lossuse"] = rating.mcs_lossUseTypeName;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_otherLossTypeName))
                                    {
                                        thisNewEntity["udo_otherloss"] = rating.mcs_otherLossTypeName;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_ratingPercent))
                                    {
                                        thisNewEntity["udo_ratingpercent"] = rating.mcs_ratingPercent;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_smcTypeName))
                                    {
                                        thisNewEntity["udo_smc"] = rating.mcs_smcTypeName;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_supplementalDecisonTypeName))
                                    {
                                        thisNewEntity["udo_supplementaldecision"] = rating.mcs_supplementalDecisonTypeName;
                                    }
                                    if (request.UDOfindRatingsRelatedEntitiesInfo != null)
                                    {
                                        foreach (var relatedItem in request.UDOfindRatingsRelatedEntitiesInfo)
                                        {
                                            thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                        }
                                    }
                                    CreateRequest createSMC = new CreateRequest
                                    {
                                        Target = thisNewEntity
                                    };
                                    requestCollection.Add(createSMC);
                                    smcRatingsCount += 1;
                                }

                            }
                        }
                        #endregion

                        var otherRatingsCount = 0;
                        #region Other Ratings
                        if (findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtotherRatingRecordInfo != null)
                        {
                            if (findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtotherRatingRecordInfo.VEISVIMTfnrtngdtratings3Info != null)
                            {
                                var otherRatings = findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtotherRatingRecordInfo.VEISVIMTfnrtngdtratings3Info;

                                foreach (var rating in otherRatings)
                                {
                                    Entity thisNewEntity = new Entity();
                                    thisNewEntity.LogicalName = "udo_otherrating";
                                    if (request.ownerId != System.Guid.Empty)
                                    {
                                        thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                    }
                                    thisNewEntity["udo_name"] = "Other Rating Summary";

                                    if (!String.IsNullOrEmpty(rating.mcs_supplementalDecisionTypeName))
                                    {
                                        thisNewEntity["udo_supplementaldecision"] = rating.mcs_supplementalDecisionTypeName;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_ratingDate))
                                    {
                                        thisNewEntity["udo_ratingdate"] = dateStringFormat(rating.mcs_ratingDate);
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_beginDate))
                                    {
                                        thisNewEntity["udo_begindate"] = dateStringFormat(rating.mcs_beginDate);
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_endDate))
                                    {
                                        thisNewEntity["udo_enddate"] = dateStringFormat(rating.mcs_endDate);
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_disabilityTypeName))
                                    {
                                        thisNewEntity["udo_disability"] = rating.mcs_disabilityTypeName;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_decisionTypeName))
                                    {
                                        thisNewEntity["udo_decision"] = rating.mcs_decisionTypeName;
                                    }
                                    if (request.UDOfindRatingsRelatedEntitiesInfo != null)
                                    {
                                        foreach (var relatedItem in request.UDOfindRatingsRelatedEntitiesInfo)
                                        {
                                            thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                        }
                                    }
                                    CreateRequest createOther = new CreateRequest
                                    {
                                        Target = thisNewEntity
                                    };
                                    requestCollection.Add(createOther);
                                    otherRatingsCount += 1;
                                }
                            }
                        }
                        #endregion
                        var famRatingsCount = 0;
                        #region Family Member Ratings

                        if (findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtfamilyMemberRatingRecordInfo != null)
                        {
                            if (findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtfamilyMemberRatingRecordInfo.VEISVIMTfnrtngdtratings2Info != null)
                            {
                                var familyMemberRatings = findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtfamilyMemberRatingRecordInfo.VEISVIMTfnrtngdtratings2Info;

                                foreach (var rating in familyMemberRatings)
                                {
                                    Entity thisNewEntity = new Entity();
                                    thisNewEntity.LogicalName = "udo_familymemberrating";
                                    if (request.ownerId != System.Guid.Empty)
                                    {
                                        thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                    }

                                    thisNewEntity["udo_name"] = "Family Rating Summary";
                                    if (!String.IsNullOrEmpty(rating.mcs_beginDate))
                                    {
                                        thisNewEntity["udo_begindate"] = dateStringFormat(rating.mcs_beginDate);
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_decisionTypeName))
                                    {
                                        thisNewEntity["udo_decision"] = rating.mcs_decisionTypeName;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_disabilityTypeName))
                                    {
                                        thisNewEntity["udo_disability"] = rating.mcs_disabilityTypeName;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_endDate))
                                    {
                                        thisNewEntity["udo_enddate"] = dateStringFormat(rating.mcs_endDate);
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_familyMemberName))
                                    {
                                        thisNewEntity["udo_familymembername"] = rating.mcs_familyMemberName;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_ratingDate))
                                    {
                                        thisNewEntity["udo_ratingdate"] = dateStringFormat(rating.mcs_ratingDate);
                                    }
                                    if (request.UDOfindRatingsRelatedEntitiesInfo != null)
                                    {
                                        foreach (var relatedItem in request.UDOfindRatingsRelatedEntitiesInfo)
                                        {
                                            thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                        }
                                    }
                                    CreateRequest createFamily = new CreateRequest
                                    {
                                        Target = thisNewEntity
                                    };
                                    requestCollection.Add(createFamily);
                                    famRatingsCount += 1;
                                }

                            }
                        }

                        #endregion
                        var deathRatingsCount = 0;
                        #region Death Ratings
                        if (findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtdeathRatingRecordInfo != null)
                        {
                            if (findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtdeathRatingRecordInfo.VEISVIMTfnrtngdtratingsInfo != null)
                            {
                                var deathRatings = findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtdeathRatingRecordInfo.VEISVIMTfnrtngdtratingsInfo;

                                foreach (var rating in deathRatings)
                                {
                                    Entity thisNewEntity = new Entity();
                                    thisNewEntity.LogicalName = "udo_deathrating";
                                    if (request.ownerId != System.Guid.Empty)
                                    {
                                        thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                    }

                                    thisNewEntity["udo_name"] = "Death Rating Summary";
                                    if (!String.IsNullOrEmpty(rating.mcs_militaryServicePeriodTypeName))
                                    {
                                        thisNewEntity["udo_militaryserviceperiod"] = rating.mcs_militaryServicePeriodTypeName;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_personDeathCauseTypeName))
                                    {
                                        thisNewEntity["udo_persondeathcause"] = rating.mcs_personDeathCauseTypeName;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_ratingDate))
                                    {
                                        thisNewEntity["udo_ratingdate"] = dateStringFormat(rating.mcs_ratingDate);
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_ratingDecisionID))
                                    {
                                        thisNewEntity["udo_ratingdecisionid"] = rating.mcs_ratingDecisionID;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_serviceConnectedDeathDecisionTypeName))
                                    {
                                        thisNewEntity["udo_serviceconnecteddeathdecision"] = rating.mcs_serviceConnectedDeathDecisionTypeName;
                                    }
                                    if (request.UDOfindRatingsRelatedEntitiesInfo != null)
                                    {
                                        foreach (var relatedItem in request.UDOfindRatingsRelatedEntitiesInfo)
                                        {
                                            thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                        }
                                    }
                                    CreateRequest createDeath = new CreateRequest
                                    {
                                        Target = thisNewEntity
                                    };
                                    requestCollection.Add(createDeath);
                                    deathRatingsCount += 1;
                                }
                            }
                        }
                        #endregion
                        var disabilityRatingsCount = 0;
                        #region DisabilityRatings
                        if (findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtdisabilityRatingRecordInfo != null)
                        {
                            if (findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtdisabilityRatingRecordInfo.VEISVIMTfnrtngdtratings1Info != null)
                            {
                                List<UDOcreateDisabilityRatingsMultipleResponse> UDOcreateDisabilityRatingsArray = new List<UDOcreateDisabilityRatingsMultipleResponse>();
                                var disabilityRatings = findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtdisabilityRatingRecordInfo.VEISVIMTfnrtngdtratings1Info;
                                var disabilityDetails = findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtdisabilityRatingRecordInfo;

                                foreach (var rating in disabilityRatings)
                                {
                                    Entity thisNewEntity = new Entity();
                                    thisNewEntity.LogicalName = "udo_disabilityrating";
                                    if (request.ownerId != System.Guid.Empty)
                                    {
                                        thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                    }

                                    thisNewEntity["udo_name"] = "Disability Rating Summary";
                                    if (!String.IsNullOrEmpty(disabilityDetails.mcs_combinedDegreeEffectiveDate))
                                    {
                                        thisNewEntity["udo_combdegreeefctvdate"] = dateStringFormat(disabilityDetails.mcs_combinedDegreeEffectiveDate);
                                    }
                                    if (!String.IsNullOrEmpty(disabilityDetails.mcs_legalEffectiveDate))
                                    {
                                        thisNewEntity["udo_legaleffectivedate"] = dateStringFormat(disabilityDetails.mcs_legalEffectiveDate);
                                    }
                                    if (!String.IsNullOrEmpty(disabilityDetails.mcs_nonServiceConnectedCombinedDegree))
                                    {
                                        thisNewEntity["udo_nonsvcconncombineddegree"] = disabilityDetails.mcs_nonServiceConnectedCombinedDegree;
                                    }
                                    if (!String.IsNullOrEmpty(disabilityDetails.mcs_promulgationDate))
                                    {
                                        thisNewEntity["udo_promulgationdate"] = dateStringFormat(disabilityDetails.mcs_promulgationDate);
                                    }
                                    if (!String.IsNullOrEmpty(disabilityDetails.mcs_serviceConnectedCombinedDegree))
                                    {
                                        thisNewEntity["udo_svcconncombineddegree"] = disabilityDetails.mcs_serviceConnectedCombinedDegree;
                                    }

                                    if (!String.IsNullOrEmpty(rating.mcs_beginDate))
                                    {
                                        thisNewEntity["udo_begindate"] = dateStringFormat(rating.mcs_beginDate);
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_disabilityDecisionTypeCode))
                                    {
                                        thisNewEntity["udo_code"] = rating.mcs_disabilityDecisionTypeCode;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_combatIndicator))
                                    {
                                        thisNewEntity["udo_combatind"] = rating.mcs_combatIndicator;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_disabilityDecisionTypeName))
                                    {
                                        thisNewEntity["udo_description"] = rating.mcs_disabilityDecisionTypeName;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_diagnosticText))
                                    {
                                        thisNewEntity["udo_diagnostic"] = rating.mcs_diagnosticText;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_diagnosticPercent))
                                    {
                                        thisNewEntity["udo_diagnosticpercent"] = rating.mcs_diagnosticPercent;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_diagnosticTypeName))
                                    {
                                        thisNewEntity["udo_diagnostictype"] = rating.mcs_diagnosticTypeName;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_diagnosticTypeCode))
                                    {
                                        thisNewEntity["udo_diagnostictypecode"] = rating.mcs_diagnosticTypeCode;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_disabilityID))
                                    {
                                        thisNewEntity["udo_disabilityid"] = rating.mcs_disabilityID;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_bilateralTypeName))
                                    {
                                        thisNewEntity["udo_bilateral"] = rating.mcs_bilateralTypeName;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_bilateralTypeCode))
                                    {
                                        thisNewEntity["udo_bilateralcd"] = rating.mcs_bilateralTypeCode;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_diagnosticTypeName))
                                    {
                                        thisNewEntity["udo_diagnostictype"] = rating.mcs_diagnosticTypeName;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_diagnosticTypeCode))
                                    {
                                        thisNewEntity["udo_diagnostictypecode"] = rating.mcs_diagnosticTypeCode;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_endDate))
                                    {
                                        thisNewEntity["udo_enddate"] = dateStringFormat(rating.mcs_endDate);
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_futureExamDate))
                                    {
                                        thisNewEntity["udo_futureexamdate"] = dateStringFormat(rating.mcs_futureExamDate);
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_hyphenatedDiagnosticTypeName))
                                    {
                                        thisNewEntity["udo_hdiag"] = rating.mcs_hyphenatedDiagnosticTypeName;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_hyphenatedDiagnosticTypeCode))
                                    {
                                        thisNewEntity["udo_hdiagcd"] = rating.mcs_hyphenatedDiagnosticTypeCode;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_hyphenatedRelatedDisabilityTypeName))
                                    {
                                        thisNewEntity["udo_hrelateddisability"] = rating.mcs_hyphenatedRelatedDisabilityTypeName;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_hyphenatedRelatedDisabilityTypeCode))
                                    {
                                        thisNewEntity["udo_hrelateddiscd"] = rating.mcs_hyphenatedRelatedDisabilityTypeCode;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_lastExamDate))
                                    {
                                        thisNewEntity["udo_lastexamdate"] = dateStringFormat(rating.mcs_lastExamDate);
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_majorIndicator))
                                    {
                                        thisNewEntity["udo_majorind"] = rating.mcs_majorIndicator;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_militaryServicePeriodTypeName))
                                    {
                                        thisNewEntity["udo_milsvcperiod"] = rating.mcs_militaryServicePeriodTypeName;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_militaryServicePeriodTypeCode))
                                    {
                                        thisNewEntity["udo_milsvcperiodcd"] = rating.mcs_militaryServicePeriodTypeCode;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_paragraphTypeCode))
                                    {
                                        thisNewEntity["udo_paragraphtype"] = rating.mcs_paragraphTypeCode;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_paragraphTypeCode))
                                    {
                                        thisNewEntity["udo_paragraphtypecd"] = rating.mcs_paragraphTypeCode;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_previousServicePercent))
                                    {
                                        thisNewEntity["udo_previousservice"] = rating.mcs_previousServicePercent;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_relatedDisabilityTypeName))
                                    {
                                        thisNewEntity["udo_relateddisability"] = rating.mcs_relatedDisabilityTypeName;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_supplementalDecisionTypeName))
                                    {
                                        thisNewEntity["udo_supplmentaldec"] = rating.mcs_supplementalDecisionTypeName;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_supplementalDecisionTypeCode))
                                    {
                                        thisNewEntity["udo_supplementaldeccd"] = rating.mcs_supplementalDecisionTypeCode;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_withholdingTypeName))
                                    {
                                        thisNewEntity["udo_withholdingname"] = rating.mcs_withholdingTypeName;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_withholdingTypeCode))
                                    {
                                        thisNewEntity["udo_withholdingcd"] = rating.mcs_withholdingTypeCode;
                                    }
                                    if (!String.IsNullOrEmpty(rating.mcs_withholdingPercent))
                                    {
                                        thisNewEntity["udo_withholding"] = rating.mcs_withholdingPercent;
                                    }
                                    if (request.UDOfindRatingsRelatedEntitiesInfo != null)
                                    {
                                        foreach (var relatedItem in request.UDOfindRatingsRelatedEntitiesInfo)
                                        {
                                            thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                        }
                                    }
                                    CreateRequest createDisabilityRatings = new CreateRequest
                                    {
                                        Target = thisNewEntity
                                    };
                                    requestCollection.Add(createDisabilityRatings);
                                    disabilityRatingsCount += 1;

                                }
                            }
                        }
                        #endregion

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
                                response.ExceptionOccurred = true;
                                return response;
                            }
                        }

                        #endregion

                        #region Log Results
                        string logInfo = string.Format("smcRatingsCount: {0}, otherRatingsCount: {1}, famRatingsCount: {2}, deathRatingsCount: {3}, disabilityRatingsCount: {4} ",
                            smcRatingsCount,
                            otherRatingsCount,
                            famRatingsCount,
                            deathRatingsCount,
                            disabilityRatingsCount);

                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method, $"Ratings Records + Children Created. \r\n Details {logInfo}");

                        #endregion
                    }
                }
                //added to generated code
                if (request.udo_ratingId != System.Guid.Empty)
                {
                    var parent = new Entity();
                    parent.Id = request.udo_ratingId;
                    parent.LogicalName = "udo_rating";
                    parent["udo_ratingcomplete"] = true;
                    OrgServiceProxy.Update(parent);
                    tLogger.LogEvent("CRM UPDATE: LOADED RATINGS DATA", "003");
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, method, ExecutionException);
                response.ExceptionMessage = "Failed to Map EC data to LOB";
                response.ExceptionOccurred = true;
                tLogger.LogException(ExecutionException, "004");
                
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

        private static string dateStringFormat(string date)
        {
            if (date == null) return null;
            if (date.Length == 10) return date;
            if (date.Length < 8) return date;

            date = date.Insert(2, "/");
            date = date.Insert(5, "/");
            return date;
        }
    }
}
