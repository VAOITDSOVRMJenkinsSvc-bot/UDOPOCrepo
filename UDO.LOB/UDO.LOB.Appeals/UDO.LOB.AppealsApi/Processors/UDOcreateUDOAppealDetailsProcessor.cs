using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;
using UDO.LOB.Appeals.Messages;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using VEIS.Core.Messages;
using VEIS.Messages.AppealService;

namespace UDO.LOB.Appeals.Processors
{
    class createUDOAppealDetailsProcessor
    {
        private bool _debug { get; set; }

        private const string method = "createUDOAppealDetailsProcessor";

        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOcreateUDOAppealDetailsRequest request)
        {
            UDOcreateUDOAppealDetailsResponse response = new UDOcreateUDOAppealDetailsResponse
            {
                MessageId = request.MessageId
            };
            if (request.DiagnosticsContext == null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = method,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA",                   
                };
            }
            TraceLogger tLogger = new TraceLogger(method, request);
            var progressString = "Top of Processor";

            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
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
                LogHelper.LogError(request.OrganizationName, request.UserId, "createUDOAppealDetailsProcessor Processor, Connection Error", connectException.Message);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion
            tLogger.LogEvent("Connected to CRM", "001");
            progressString = "After Connection";

            try
            {
                var getAppealRequest = new VEISgtaplgetAppealRequest()
                {
                    LogTiming = request.LogTiming,
                    LogSoap = request.LogSoap,
                    Debug = request.Debug,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    },
                    VEISgtaplReqgetAppealCriteriaInfo = new VEISgtaplReqgetAppealCriteria { mcs_AppealKey = request.AppealKey }
                };

                //non standard fields
                var getAppealResponse = WebApiUtility.SendReceive<VEISgtaplgetAppealResponse>(getAppealRequest, WebApiType.VEIS);
                if (request.LogSoap || getAppealResponse.ExceptionOccurred)
                {
                    if (getAppealResponse.SerializedSOAPRequest != null || getAppealResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = getAppealResponse.SerializedSOAPRequest + getAppealResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISgtaplgetAppealRequest Request/Response {requestResponse}", true);
                    }
                }

                tLogger.LogEvent($"Web Service Call VEISgtaplgetAppealResponse. Exception Occured:{getAppealResponse.ExceptionOccurred}", "002");
                response.ExceptionMessage = getAppealResponse.ExceptionMessage;
                response.ExceptionOccured = getAppealResponse.ExceptionOccurred;
                var requestCollection = new OrganizationRequestCollection();

                if (getAppealResponse != null)
                {
                    var appealUpdate = 0;
                    if (getAppealResponse.VEISgtaplGetAppealResponseInfo != null)
                    {
                        if (getAppealResponse.VEISgtaplGetAppealResponseInfo.VEISgtaplAppealRecordInfo != null)
                        {
                            var AppealRecord = getAppealResponse.VEISgtaplGetAppealResponseInfo.VEISgtaplAppealRecordInfo;

                            var thisAppealRecord = new Entity();
                            thisAppealRecord.Id = request.udo_appealId;
                            thisAppealRecord.LogicalName = "udo_appeal";

                            #region appellantData

                            if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppellantInfo.VEISgtaplAppellantAddressInfo.mcs_AppellantAddressZipCode))
                            {
                                thisAppealRecord["udo_zipcode"] = AppealRecord.VEISgtaplAppellantInfo.VEISgtaplAppellantAddressInfo.mcs_AppellantAddressZipCode;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppellantInfo.VEISgtaplAppellantAddressInfo.mcs_AppellantWorkPhoneNumber))
                            {
                                thisAppealRecord["udo_workphone"] = AppealRecord.VEISgtaplAppellantInfo.VEISgtaplAppellantAddressInfo.mcs_AppellantWorkPhoneNumber;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppellantInfo.mcs_AppellantTitle))
                            {
                                thisAppealRecord["udo_title"] = AppealRecord.VEISgtaplAppellantInfo.mcs_AppellantTitle;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppellantInfo.VEISgtaplAppellantAddressInfo.mcs_AppellantAddressStateCode))
                            {
                                thisAppealRecord["udo_state"] = AppealRecord.VEISgtaplAppellantInfo.VEISgtaplAppellantAddressInfo.mcs_AppellantAddressStateCode;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppealVeteranInfo.mcs_VeteranSSN))
                            {
                                thisAppealRecord["udo_ssn"] = AppealRecord.VEISgtaplAppealVeteranInfo.mcs_VeteranSSN;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppellantInfo.mcs_AppellantRelationshipToVeteranCode))
                            {
                                thisAppealRecord["udo_relationshiptovetdesc"] = AppealRecord.VEISgtaplAppellantInfo.mcs_AppellantRelationshipToVeteranCode;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppellantInfo.VEISgtaplAppellantAddressInfo.mcs_AppellantAddressLastModifiedDate))
                            {
                                DateTime newDateTime;
                                if (DateTime.TryParse(AppealRecord.VEISgtaplAppellantInfo.VEISgtaplAppellantAddressInfo.mcs_AppellantAddressLastModifiedDate, out newDateTime))
                                {
                                    thisAppealRecord["udo_addressmoddate"] = newDateTime.ToString();
                                }
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppellantInfo.VEISgtaplAppellantAddressInfo.mcs_AppellantAddressLastModifiedByROName))
                            {
                                thisAppealRecord["udo_addressmodby"] = AppealRecord.VEISgtaplAppellantInfo.VEISgtaplAppellantAddressInfo.mcs_AppellantAddressLastModifiedByROName;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppellantInfo.VEISgtaplAppellantAddressInfo.mcs_AppellantAddressLine2))
                            {
                                thisAppealRecord["udo_address2"] = AppealRecord.VEISgtaplAppellantInfo.VEISgtaplAppellantAddressInfo.mcs_AppellantAddressLine2;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppellantInfo.VEISgtaplAppellantAddressInfo.mcs_AppellantAddressLine1))
                            {
                                thisAppealRecord["udo_address1"] = AppealRecord.VEISgtaplAppellantInfo.VEISgtaplAppellantAddressInfo.mcs_AppellantAddressLine1;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppellantInfo.VEISgtaplAppellantAddressInfo.mcs_AppellantHomePhoneNumber))
                            {
                                thisAppealRecord["udo_homephone"] = AppealRecord.VEISgtaplAppellantInfo.VEISgtaplAppellantAddressInfo.mcs_AppellantHomePhoneNumber;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppellantInfo.VEISgtaplAppellantAddressInfo.mcs_AppellantAddressCountryName))
                            {
                                thisAppealRecord["udo_country"] = AppealRecord.VEISgtaplAppellantInfo.VEISgtaplAppellantAddressInfo.mcs_AppellantAddressCountryName;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppellantInfo.VEISgtaplAppellantAddressInfo.mcs_AppellantAddressCityName))
                            {
                                thisAppealRecord["udo_city"] = AppealRecord.VEISgtaplAppellantInfo.VEISgtaplAppellantAddressInfo.mcs_AppellantAddressCityName;
                            }

                            if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppellantInfo.VEISgtaplAppellantAddressInfo.mcs_AppellantAddressNotes))
                            {
                                thisAppealRecord["udo_addressnotes"] = AppealRecord.VEISgtaplAppellantInfo.VEISgtaplAppellantAddressInfo.mcs_AppellantAddressNotes;
                            }
                            var appellantName = "";
                            if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppellantInfo.mcs_AppellantLastName))
                            {
                                appellantName = AppealRecord.VEISgtaplAppellantInfo.mcs_AppellantLastName;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppellantInfo.mcs_AppellantFirstName))
                            {
                                appellantName += ", " + AppealRecord.VEISgtaplAppellantInfo.mcs_AppellantFirstName;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppellantInfo.mcs_AppellantMiddleInitial))
                            {
                                appellantName += " " + AppealRecord.VEISgtaplAppellantInfo.mcs_AppellantMiddleInitial;
                            }
                            thisAppealRecord["udo_appellantname"] = appellantName;
                            #endregion
                            #region veteranInfo Data
                            if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppealVeteranInfo.mcs_VeteranGender))
                            {
                                thisAppealRecord["udo_gender"] = AppealRecord.VEISgtaplAppealVeteranInfo.mcs_VeteranGender;
                            }
                            if (AppealRecord.VEISgtaplAppealVeteranInfo.mcs_FinalNoticeOfDeathDateSpecified)
                            {
                                thisAppealRecord["udo_fnoddate"] = AppealRecord.VEISgtaplAppealVeteranInfo.mcs_FinalNoticeOfDeathDate;

                                DateTime newDateTime;
                                if (DateTime.TryParse(AppealRecord.VEISgtaplAppealVeteranInfo.mcs_FinalNoticeOfDeathDate, out newDateTime))
                                {
                                    thisAppealRecord["udo_fnoddate"] = newDateTime;
                                }
                            }
                            if (AppealRecord.VEISgtaplAppealVeteranInfo.mcs_BirthDateSpecified)
                            {
                                DateTime newDateTime;
                                if (DateTime.TryParse(AppealRecord.VEISgtaplAppealVeteranInfo.mcs_BirthDate, out newDateTime))
                                {
                                    thisAppealRecord["udo_birthdate"] = newDateTime;
                                }
                            }

                            if (!string.IsNullOrEmpty(AppealRecord.mcs_DecisionReviewOfficerElectedDate))
                            {
                                DateTime newDateTime;
                                if (DateTime.TryParse(AppealRecord.mcs_DecisionReviewOfficerElectedDate, out newDateTime))
                                {
                                    thisAppealRecord["udo_droelecteddate"] = newDateTime;
                                }
                            }

                            if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppealVeteranInfo.mcs_VeteranGender))
                            {
                                thisAppealRecord["udo_gender"] = AppealRecord.VEISgtaplAppealVeteranInfo.mcs_VeteranGender;
                            }
                            var veteranName = "";
                            if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppealVeteranInfo.mcs_VeteranLastName))
                            {
                                veteranName = AppealRecord.VEISgtaplAppealVeteranInfo.mcs_VeteranLastName;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppealVeteranInfo.mcs_VeteranFirstName))
                            {
                                veteranName += ", " + AppealRecord.VEISgtaplAppealVeteranInfo.mcs_VeteranFirstName;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppealVeteranInfo.mcs_VeteranMiddleInitial))
                            {
                                veteranName += " " + AppealRecord.VEISgtaplAppealVeteranInfo.mcs_VeteranMiddleInitial;
                            }
                            thisAppealRecord["udo_veteranname"] = veteranName;

                            #endregion

                            #region base Appeal Data
                            if (AppealRecord.mcs_ServiceOrganizationReceivedDateSpecified)
                            {
                                DateTime newDateTime;
                                if (DateTime.TryParse(AppealRecord.mcs_ServiceOrganizationReceivedDate, out newDateTime))
                                {
                                    thisAppealRecord["udo_svcorgrecdate"] = newDateTime;
                                }
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.mcs_ServiceOrganizationName))
                            {
                                thisAppealRecord["udo_svcorgname"] = AppealRecord.mcs_ServiceOrganizationName;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.mcs_ServiceOrganizationDescription))
                            {
                                thisAppealRecord["udo_svcorgdescription"] = AppealRecord.mcs_ServiceOrganizationDescription;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.mcs_RegionalOfficeName))
                            {
                                thisAppealRecord["udo_roname"] = AppealRecord.mcs_RegionalOfficeName;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.mcs_RegionalOfficeCode))
                            {
                                thisAppealRecord["udo_rocode"] = AppealRecord.mcs_RegionalOfficeCode;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.mcs_MedicalFacilityName))
                            {
                                thisAppealRecord["udo_medicalfacilityname"] = AppealRecord.mcs_MedicalFacilityName;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.mcs_MedicalFacilityCode))
                            {
                                thisAppealRecord["udo_medfacilitycode"] = AppealRecord.mcs_MedicalFacilityCode;
                            }

                            if (!string.IsNullOrEmpty(AppealRecord.mcs_CurrentFileStoredLocationDescription))
                            {
                                thisAppealRecord["udo_filestorelocdesc"] = AppealRecord.mcs_CurrentFileStoredLocationDescription;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.mcs_DecisionReviewOfficerReadyToRateIndicator))
                            {
                                thisAppealRecord["udo_droreadytorateindicator"] = AppealRecord.mcs_DecisionReviewOfficerReadyToRateIndicator;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.mcs_DecisionReviewOfficerPartialGrantOrDenialIndicator))
                            {
                                thisAppealRecord["udo_dropartialgrantdenialind"] = AppealRecord.mcs_DecisionReviewOfficerPartialGrantOrDenialIndicator;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.mcs_DecisionReviewOfficerInformalHearingIndicator))
                            {
                                thisAppealRecord["udo_droinformalhearingind"] = AppealRecord.mcs_DecisionReviewOfficerInformalHearingIndicator;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.mcs_DecisionReviewOfficerId))
                            {
                                thisAppealRecord["udo_droid"] = AppealRecord.mcs_DecisionReviewOfficerId;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.mcs_DecisionReviewOfficerFormalHearingIndicator))
                            {
                                thisAppealRecord["udo_droformalhearingind"] = AppealRecord.mcs_DecisionReviewOfficerFormalHearingIndicator;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.mcs_DecisionReviewOfficerElectedDate))
                            {
                                DateTime newDateTime;
                                if (DateTime.TryParse(AppealRecord.mcs_DecisionReviewOfficerElectedDate, out newDateTime))
                                {
                                    thisAppealRecord["udo_droelecteddate"] = newDateTime;
                                }
                            }
                            if (AppealRecord.mcs_DocketDateSpecified)
                            {
                                DateTime newDateTime;
                                if (DateTime.TryParse(AppealRecord.mcs_DocketDate, out newDateTime))
                                {
                                    thisAppealRecord["udo_docketdate"] = newDateTime;
                                }
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.mcs_DocketNumber))
                            {
                                thisAppealRecord["udo_docketnumber"] = AppealRecord.mcs_DocketNumber;
                            }

                            if (AppealRecord.mcs_ChargeToCurrentLocationDateSpecified)
                            {
                                DateTime newDateTime;
                                if (DateTime.TryParse(AppealRecord.mcs_ChargeToCurrentLocationDate, out newDateTime))
                                {
                                    thisAppealRecord["udo_chrgtocurrentlocation"] = newDateTime;
                                }
                            }
                            if (AppealRecord.mcs_BVAReceivedDateSpecified)
                            {
                                DateTime newDateTime;
                                if (DateTime.TryParse(AppealRecord.mcs_BVAReceivedDate, out newDateTime))
                                {
                                    thisAppealRecord["udo_bvareceivedate"] = newDateTime;
                                }
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.mcs_ActionTypeDescription))
                            {
                                thisAppealRecord["udo_appealactiontypedesc"] = AppealRecord.mcs_ActionTypeDescription;
                            }


                            #endregion
                            #region decision fields
                            if (AppealRecord.VEISgtaplAppealDecisionInfo != null)
                            {
                                if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppealDecisionInfo.mcs_DecisionTeamCode))
                                {
                                    thisAppealRecord["udo_teamcode"] = AppealRecord.VEISgtaplAppealDecisionInfo.mcs_DecisionTeamCode;
                                }

                                if (AppealRecord.VEISgtaplAppealDecisionInfo.mcs_DecisionDateSpecified)
                                {
                                    DateTime newDateTime;
                                    if (DateTime.TryParse(AppealRecord.VEISgtaplAppealDecisionInfo.mcs_DecisionDate, out newDateTime))
                                    {
                                        thisAppealRecord["udo_decisiondate"] = newDateTime;
                                        thisAppealRecord["udo_date"] = newDateTime;
                                    }
                                }
                                if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppealDecisionInfo.mcs_DecisionRemandedToName))
                                {
                                    thisAppealRecord["udo_remandtoname"] = AppealRecord.VEISgtaplAppealDecisionInfo.mcs_DecisionRemandedToName;
                                }
                                if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppealDecisionInfo.mcs_RemandedToCode))
                                {
                                    thisAppealRecord["udo_remandtocode"] = AppealRecord.VEISgtaplAppealDecisionInfo.mcs_RemandedToCode;
                                }
                                if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppealDecisionInfo.mcs_HearingActionDescription))
                                {
                                    thisAppealRecord["udo_hearingactiondescription"] = AppealRecord.VEISgtaplAppealDecisionInfo.mcs_HearingActionDescription;
                                }
                                if (!string.IsNullOrEmpty(AppealRecord.VEISgtaplAppealDecisionInfo.mcs_DecisionDispositionDescription))
                                {
                                    thisAppealRecord["udo_dispositiondescription"] = AppealRecord.VEISgtaplAppealDecisionInfo.mcs_DecisionDispositionDescription;
                                }
                            }
                            #endregion

                            appealUpdate += 1;
                            if (AppealRecord.VEISgtaplHearingRequestInfo != null)
                            {
                                var HearingRecords = AppealRecord.VEISgtaplHearingRequestInfo;
                                var HearingRequestItem = HearingRecords.OrderByDescending(h => h.mcs_HearingRequestScheduledDate).FirstOrDefault();

                                if (HearingRequestItem.mcs_HearingRequestClosedDateSpecified)
                                {
                                    DateTime newDateTime;
                                    if (DateTime.TryParse(HearingRequestItem.mcs_HearingRequestClosedDate, out newDateTime))
                                    {
                                        thisAppealRecord["udo_closeddate"] = newDateTime;
                                    }
                                }
                                if (!string.IsNullOrEmpty(HearingRequestItem.mcs_HearingRequestDispositionDescription))
                                {
                                    thisAppealRecord["udo_despositiondescription"] = HearingRequestItem.mcs_HearingRequestDispositionDescription;
                                }
                                if (!string.IsNullOrEmpty(HearingRequestItem.mcs_HearingRequestNotes))
                                {
                                    thisAppealRecord["udo_notes"] = HearingRequestItem.mcs_HearingRequestNotes;
                                }
                                if (!string.IsNullOrEmpty(HearingRequestItem.mcs_HearingRequestedTypeDescription))
                                {
                                    thisAppealRecord["udo_requesttypedescription"] = HearingRequestItem.mcs_HearingRequestedTypeDescription;
                                }
                                if (HearingRequestItem.mcs_HearingRequestRequestedDateSpecified)
                                {
                                    DateTime newDateTime;
                                    if (DateTime.TryParse(HearingRequestItem.mcs_HearingRequestRequestedDate, out newDateTime))
                                    {
                                        thisAppealRecord["udo_requestdate"] = newDateTime;
                                    }
                                }
                                if (HearingRequestItem.mcs_HearingRequestScheduledDateSpecified)
                                {
                                    DateTime newDateTime;
                                    if (DateTime.TryParse(HearingRequestItem.mcs_HearingRequestScheduledDate, out newDateTime))
                                    {
                                        thisAppealRecord["udo_scheduledate"] = newDateTime;
                                    }
                                }
                            }
                            UpdateRequest updateAppeals = new UpdateRequest
                            {
                                Target = thisAppealRecord
                            };
                            requestCollection.Add(updateAppeals);
                        }
                    }

                    #region Appeal Dates
                    var appealDatesCnt = 0;
                    if (getAppealResponse.VEISgtaplGetAppealResponseInfo != null)
                    {
                        if (getAppealResponse.VEISgtaplGetAppealResponseInfo.VEISgtaplAppealRecordInfo != null)
                        {
                            if (getAppealResponse.VEISgtaplGetAppealResponseInfo.VEISgtaplAppealRecordInfo.VEISgtaplAppealDateInfo != null)
                            {
                                var AppealDate = getAppealResponse.VEISgtaplGetAppealResponseInfo.VEISgtaplAppealRecordInfo.VEISgtaplAppealDateInfo;

                                foreach (var AppealDateItem in AppealDate)
                                {
                                    appealDatesCnt += 1;
                                    var thisNewEntity = new Entity();
                                    thisNewEntity.LogicalName = "udo_appealdates";
                                    thisNewEntity["udo_name"] = "Appeal Date Summary";
                                    if (request.ownerId != System.Guid.Empty)
                                    {
                                        thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                    }
                                    if (AppealDateItem.mcs_DateTypeDescription != string.Empty)
                                    {
                                        thisNewEntity["udo_datetypedescription"] = AppealDateItem.mcs_DateTypeDescription;
                                    }
                                    if (AppealDateItem.mcs_DateTypeCode != string.Empty)
                                    {
                                        thisNewEntity["udo_datetypecode"] = AppealDateItem.mcs_DateTypeCode;
                                    }
                                    if (AppealDateItem.mcs_DateSpecified)
                                    {
                                        DateTime newDateTime;
                                        if (DateTime.TryParse(AppealDateItem.mcs_Date, out newDateTime))
                                        {
                                            thisNewEntity["udo_date"] = newDateTime;
                                        }
                                    }
                                    if (request.UDOcreateUDOAppealDetailsRelatedEntitiesInfo != null)
                                    {
                                        foreach (var relatedItem in request.UDOcreateUDOAppealDetailsRelatedEntitiesInfo)
                                        {
                                            thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                        }
                                    }

                                    CreateRequest createAppealDates = new CreateRequest
                                    {
                                        Target = thisNewEntity
                                    };
                                    requestCollection.Add(createAppealDates);
                                }
                            }
                        }
                    }

                    #endregion

                    #region Attorney Information
                    var attyInfoCnt = 0;
                    if (getAppealResponse.VEISgtaplGetAppealResponseInfo != null)
                    {
                        if (getAppealResponse.VEISgtaplGetAppealResponseInfo.VEISgtaplAppealRecordInfo != null)
                        {
                            if (getAppealResponse.VEISgtaplGetAppealResponseInfo.VEISgtaplAppealRecordInfo.VEISgtaplRepFeesInfo != null)
                            {
                                var AttorneyInformation = getAppealResponse.VEISgtaplGetAppealResponseInfo.VEISgtaplAppealRecordInfo.VEISgtaplRepFeesInfo;

                                foreach (var AttorneyInfo in AttorneyInformation)
                                {
                                    attyInfoCnt += 1;
                                    var thisNewEntity = new Entity();
                                    thisNewEntity.LogicalName = "udo_attorneyinformation";
                                    thisNewEntity["udo_name"] = "Attorney Summary";
                                    if (request.ownerId != System.Guid.Empty)
                                    {
                                        thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                    }
                                    if (AttorneyInfo.mcs_BVAReceivedSpecified)
                                    {
                                        thisNewEntity["udo_bvareceived"] = AttorneyInfo.mcs_BVAReceived;
                                    }
                                    if (AttorneyInfo.mcs_DirectPay != string.Empty)
                                    {
                                        thisNewEntity["udo_directpay"] = AttorneyInfo.mcs_DirectPay;
                                    }
                                    if (AttorneyInfo.mcs_FeeDateSpecified)
                                    {
                                        thisNewEntity["udo_feedate"] = AttorneyInfo.mcs_FeeDate;
                                    }
                                    if (AttorneyInfo.mcs_FirstName != string.Empty)
                                    {
                                        thisNewEntity["udo_firstname"] = AttorneyInfo.mcs_FirstName;
                                    }
                                    if (AttorneyInfo.mcs_LastName != string.Empty)
                                    {
                                        thisNewEntity["udo_lastname"] = AttorneyInfo.mcs_LastName;
                                    }
                                    if (AttorneyInfo.mcs_MiddleInitial != string.Empty)
                                    {
                                        thisNewEntity["udo_middleinitial"] = AttorneyInfo.mcs_MiddleInitial;
                                    }
                                    if (request.UDOcreateUDOAppealDetailsRelatedEntitiesInfo != null)
                                    {
                                        foreach (var relatedItem in request.UDOcreateUDOAppealDetailsRelatedEntitiesInfo)
                                        {
                                            thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                        }
                                    }

                                    CreateRequest createAttorneyInfo = new CreateRequest
                                    {
                                        Target = thisNewEntity
                                    };
                                    requestCollection.Add(createAttorneyInfo);
                                }
                            }
                        }
                    }

                    #endregion

                    #region Appeal Issues (and Remand Reasons)
                    var applIssCnt = 0;
                    var remandRsnCnt = 0;
                    if (getAppealResponse.VEISgtaplGetAppealResponseInfo != null)
                    {
                        if (getAppealResponse.VEISgtaplGetAppealResponseInfo.VEISgtaplAppealRecordInfo != null)
                        {
                            if (getAppealResponse.VEISgtaplGetAppealResponseInfo.VEISgtaplAppealRecordInfo.VEISgtaplIssueInfo != null)
                            {
                                var AppealIssues = getAppealResponse.VEISgtaplGetAppealResponseInfo.VEISgtaplAppealRecordInfo.VEISgtaplIssueInfo;

                                foreach (var AppealIssueItem in AppealIssues)
                                {
                                    applIssCnt += 1;
                                    var thisNewEntity = new Entity();
                                    thisNewEntity.LogicalName = "udo_appealissues";
                                    thisNewEntity["udo_name"] = "Appeal Issues Summary";
                                    if (request.ownerId != System.Guid.Empty)
                                    {
                                        thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                    }
                                    thisNewEntity["udo_sequencenumber"] = AppealIssueItem.mcs_IssueSequenceNumber;
                                    if (AppealIssueItem.mcs_IssueProgramDescription != string.Empty)
                                    {
                                        thisNewEntity["udo_programdescription"] = AppealIssueItem.mcs_IssueProgramDescription;
                                    }
                                    if (AppealIssueItem.mcs_IssueLevel3Description != string.Empty)
                                    {
                                        thisNewEntity["udo_level3description"] = AppealIssueItem.mcs_IssueLevel3Description;
                                    }
                                    if (AppealIssueItem.mcs_IssueLevel2Description != string.Empty)
                                    {
                                        thisNewEntity["udo_level2"] = AppealIssueItem.mcs_IssueLevel2Description;
                                    }
                                    if (AppealIssueItem.mcs_IssueLevel1Description != string.Empty)
                                    {
                                        thisNewEntity["udo_level1description"] = AppealIssueItem.mcs_IssueLevel1Description;
                                    }
                                    if (AppealIssueItem.mcs_IssueDescription != string.Empty)
                                    {
                                        thisNewEntity["udo_issuedescription"] = AppealIssueItem.mcs_IssueDescription;
                                    }
                                    if (AppealIssueItem.mcs_IssueCodeDescription != string.Empty)
                                    {
                                        thisNewEntity["udo_issuecodedescription"] = AppealIssueItem.mcs_IssueCodeDescription;
                                    }
                                    if (AppealIssueItem.mcs_IssueDispositionDescription != string.Empty)
                                    {
                                        thisNewEntity["udo_dispositiondescription"] = AppealIssueItem.mcs_IssueDispositionDescription;
                                    }
                                    if (AppealIssueItem.mcs_IssueDispositionDateSpecified)
                                    {

                                        DateTime newDateTime;
                                        if (DateTime.TryParse(AppealIssueItem.mcs_IssueDispositionDate, out newDateTime))
                                        {
                                            thisNewEntity["udo_dispositiondate"] = newDateTime;
                                        }
                                    }
                                    if (AppealIssueItem.mcs_IssueNotes != string.Empty)
                                    {
                                        thisNewEntity["udo_issuenotes"] = AppealIssueItem.mcs_IssueNotes;
                                    }
                                    if (request.UDOcreateUDOAppealDetailsRelatedEntitiesInfo != null)
                                    {
                                        foreach (var relatedItem in request.UDOcreateUDOAppealDetailsRelatedEntitiesInfo)
                                        {
                                            thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                        }
                                    }

                                    #region Remand Reasons
                                    if (AppealIssueItem.VEISgtaplRemandReasonInfo != null)
                                    {
                                        var RemandReasons = AppealIssueItem.VEISgtaplRemandReasonInfo;
                                        var remandEntity = new Entity();
                                        remandEntity.LogicalName = "udo_appealremandreasons";
                                        if (request.ownerId != System.Guid.Empty)
                                        {
                                            remandEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                        }
                                        foreach (var RemandReasonItem in RemandReasons)
                                        {
                                            remandRsnCnt += 1;
                                            if (!string.IsNullOrEmpty(RemandReasonItem.mcs_RemandReasonDescription))
                                            {
                                                remandEntity["udo_reasondescription"] = RemandReasonItem.mcs_RemandReasonDescription;
                                            }
                                            if (!string.IsNullOrEmpty(RemandReasonItem.mcs_LastModifiedDate))
                                            {
                                                remandEntity["udo_modifieddate"] = RemandReasonItem.mcs_LastModifiedDate;
                                            }
                                            if (!string.IsNullOrEmpty(RemandReasonItem.mcs_LastModifiedByName))
                                            {
                                                remandEntity["udo_modifiedbyname"] = RemandReasonItem.mcs_LastModifiedByName;
                                            }
                                            if (RemandReasonItem.mcs_RemandIssueSequenceNumberSpecified)
                                            {
                                                remandEntity["udo_issuesequencenumber"] = RemandReasonItem.mcs_RemandIssueSequenceNumber;
                                            }
                                            if (!string.IsNullOrEmpty(RemandReasonItem.mcs_RemandReasonCertifiedToBVAIndicator))
                                            {
                                                remandEntity["udo_certtobvaindicator"] = RemandReasonItem.mcs_RemandReasonCertifiedToBVAIndicator;
                                            }
                                            if (request.UDOcreateUDOAppealDetailsRelatedEntitiesInfo != null)
                                            {
                                                foreach (var relatedItem in request.UDOcreateUDOAppealDetailsRelatedEntitiesInfo)
                                                {
                                                    remandEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                                }
                                            }

                                            CreateRequest createRemandReasons = new CreateRequest
                                            {
                                                Target = remandEntity
                                            };
                                            requestCollection.Add(createRemandReasons);
                                        }
                                    }
                                    #endregion

                                    CreateRequest createAppealIssues = new CreateRequest
                                    {
                                        Target = thisNewEntity
                                    };
                                    requestCollection.Add(createAppealIssues);
                                }
                            }
                        }
                    }

                    #endregion

                    #region Appeal Diaries
                    var applDiariesCnt = 0;
                    if (getAppealResponse.VEISgtaplGetAppealResponseInfo != null)
                    {
                        if (getAppealResponse.VEISgtaplGetAppealResponseInfo.VEISgtaplAppealRecordInfo != null)
                        {
                            if (getAppealResponse.VEISgtaplGetAppealResponseInfo.VEISgtaplAppealRecordInfo.VEISgtaplDiaryInfo != null)
                            {
                                var AppealDiaries = getAppealResponse.VEISgtaplGetAppealResponseInfo.VEISgtaplAppealRecordInfo.VEISgtaplDiaryInfo;

                                foreach (var AppealDiariesItem in AppealDiaries)
                                {
                                    applDiariesCnt += 1;
                                    var thisNewEntity = new Entity();
                                    thisNewEntity.LogicalName = "udo_appealdiaries";
                                    thisNewEntity["udo_name"] = "Appeal Diaries Summary";
                                    if (request.ownerId != System.Guid.Empty)
                                    {
                                        thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                    }
                                    if (!string.IsNullOrEmpty(AppealDiariesItem.mcs_ResponseNotesDescription))
                                    {
                                        thisNewEntity["udo_responsenotesdescription"] = AppealDiariesItem.mcs_ResponseNotesDescription;
                                    }
                                    if (!string.IsNullOrEmpty(AppealDiariesItem.mcs_RequestedActivityDescription))
                                    {
                                        thisNewEntity["udo_reqactivitydescription"] = AppealDiariesItem.mcs_RequestedActivityDescription;
                                    }
                                    if (AppealDiariesItem.mcs_DiarySuspenseDueDateSpecified)
                                    {
                                        DateTime newDateTime;
                                        if (DateTime.TryParse(AppealDiariesItem.mcs_DiarySuspenseDueDate, out newDateTime))
                                        {
                                            thisNewEntity["udo_duedate"] = newDateTime;
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(AppealDiariesItem.mcs_DiaryDescription))
                                    {
                                        thisNewEntity["udo_diarydescription"] = AppealDiariesItem.mcs_DiaryDescription;
                                    }
                                    if (AppealDiariesItem.mcs_DaysToCompleteDiaryItemQuantitySpecified)
                                    {
                                        thisNewEntity["udo_daystocompletestring"] = AppealDiariesItem.mcs_DaysToCompleteDiaryItemQuantity.ToString();
                                    }
                                    if (!string.IsNullOrEmpty(AppealDiariesItem.mcs_BVAorRODiaryIndicatorText))
                                    {
                                        thisNewEntity["udo_bvaro"] = AppealDiariesItem.mcs_BVAorRODiaryIndicatorText;
                                    }
                                    if (!string.IsNullOrEmpty(AppealDiariesItem.mcs_AssignedStaffMemberName))
                                    {
                                        thisNewEntity["udo_assignedto"] = AppealDiariesItem.mcs_AssignedStaffMemberName;
                                    }
                                    if (AppealDiariesItem.mcs_AssignedToStaffMemberDateSpecified)
                                    {
                                        DateTime newDateTime;
                                        if (DateTime.TryParse(AppealDiariesItem.mcs_AssignedToStaffMemberDate, out newDateTime))
                                        {
                                            thisNewEntity["udo_assigneddate"] = newDateTime;
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(AppealDiariesItem.mcs_DiaryStatusDescription))
                                    {
                                        thisNewEntity["udo_status"] = AppealDiariesItem.mcs_DiaryStatusDescription;
                                    }
                                    if (AppealDiariesItem.mcs_DiaryClosedDateSpecified)
                                    {
                                        DateTime newDateTime;
                                        if (DateTime.TryParse(AppealDiariesItem.mcs_DiaryClosedDate, out newDateTime))
                                        {
                                            thisNewEntity["udo_closeddate"] = newDateTime;
                                        }
                                    }
                                    if (request.UDOcreateUDOAppealDetailsRelatedEntitiesInfo != null)
                                    {
                                        foreach (var relatedItem in request.UDOcreateUDOAppealDetailsRelatedEntitiesInfo)
                                        {
                                            thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                        }
                                    }

                                    CreateRequest createAppealDiaries = new CreateRequest
                                    {
                                        Target = thisNewEntity
                                    };
                                    requestCollection.Add(createAppealDiaries);
                                }
                            }
                        }
                    }

                    #endregion

                    #region Special Contentions
                    var spcContCnt = 0;
                    if (getAppealResponse.VEISgtaplGetAppealResponseInfo != null)
                    {
                        if (getAppealResponse.VEISgtaplGetAppealResponseInfo.VEISgtaplAppealRecordInfo != null)
                        {
                            if (getAppealResponse.VEISgtaplGetAppealResponseInfo.VEISgtaplAppealRecordInfo.VEISgtaplAppealDecisionInfo != null)
                            {
                                if (getAppealResponse.VEISgtaplGetAppealResponseInfo.VEISgtaplAppealRecordInfo.VEISgtaplAppealDecisionInfo.VEISgtaplSpecialContentionsInfo != null)
                                {
                                    var SpecialContentions = getAppealResponse.VEISgtaplGetAppealResponseInfo.VEISgtaplAppealRecordInfo.VEISgtaplAppealDecisionInfo.VEISgtaplSpecialContentionsInfo;
                                    foreach (var SpecialContentionsItem in SpecialContentions)
                                    {
                                        spcContCnt += 1;
                                        //instantiate the new Entity
                                        Entity thisNewEntity = new Entity();
                                        thisNewEntity.LogicalName = "udo_appealspecialcontentions";
                                        thisNewEntity["udo_name"] = "Appeal Special Contentions Summary";
                                        if (request.ownerId != System.Guid.Empty)
                                        {
                                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                        }
                                        if (!string.IsNullOrEmpty(SpecialContentionsItem.mcs_ContentionIndicator))
                                        {
                                            thisNewEntity["udo_contentionindicator"] = SpecialContentionsItem.mcs_ContentionIndicator;
                                        }
                                        if (!string.IsNullOrEmpty(SpecialContentionsItem.mcs_ContentionDescription))
                                        {
                                            thisNewEntity["udo_contentiondescription"] = SpecialContentionsItem.mcs_ContentionDescription;
                                        }
                                        if (!string.IsNullOrEmpty(SpecialContentionsItem.mcs_ContentionCode))
                                        {
                                            thisNewEntity["udo_contentioncode"] = SpecialContentionsItem.mcs_ContentionCode;
                                        }
                                        if (request.UDOcreateUDOAppealDetailsRelatedEntitiesInfo != null)
                                        {
                                            foreach (var relatedItem in request.UDOcreateUDOAppealDetailsRelatedEntitiesInfo)
                                            {
                                                thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                            }
                                        }

                                        CreateRequest createSpecialContentions = new CreateRequest
                                        {
                                            Target = thisNewEntity
                                        };
                                        requestCollection.Add(createSpecialContentions);
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region Execute Multiple
                    if (requestCollection.Count() > 0)
                    {
                        var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, Guid.Empty, request.Debug);
                        tLogger.LogEvent("Award Details Execute Multiple compelte.", "003");
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
                            tLogger.LogException(new Exception(result.FriendlyDetail), "004");
                            return response;
                        }                       
                    }

                    string logInfo = string.Format("Appeal Updated:{0}; Appeal Dates  Added:{1}; Atty Info Added:{2}; Appeal Issues  Added:{3}; Remand Rsns Added:{4}; Appeal Diaries  Added:{5}; Special Contentions  Added:{6}", appealUpdate, appealDatesCnt, attyInfoCnt, applIssCnt, remandRsnCnt, applDiariesCnt, spcContCnt);
                    LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Appeal Records Processed", logInfo);

                    #endregion
                }

                if (request.udo_appealId != System.Guid.Empty)
                {
                    var parent = new Entity();
                    parent.Id = request.udo_appealId;
                    parent.LogicalName = "udo_appeal";
                    parent["udo_appealcomplete"] = true;
                    OrgServiceProxy.Update(parent);
                    tLogger.LogTrace("Update Appeal Status", "004");
                }

                return response;
            }

            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "createUDOAppealsDetailsProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Appeals Data";
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