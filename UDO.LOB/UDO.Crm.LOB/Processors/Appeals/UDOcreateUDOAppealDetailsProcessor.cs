// using CRM007.CRM.SDK.Core;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using VIMT.AppealService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Appeals.Messages;
using VRM.Integration.UDO.Common;
using Logger = VRM.Integration.Servicebus.Core.Logger;

namespace VRM.Integration.UDO.Appeals.Processors
{
    class createUDOAppealDetailsProcessor
    {
        private bool _debug { get; set; }
        private const string method = "createUDOAppealDetailsProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOcreateUDOAppealDetailsRequest request)
        {
            //var request = message as createUDOAppealDatesRequest;
            UDOcreateUDOAppealDetailsResponse response = new UDOcreateUDOAppealDetailsResponse();
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
                LogHelper.LogError(request.OrganizationName, request.UserId, "createUDOAppealDetailsProcessor Processor, Connection Error", connectException.Message);                
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

            progressString = "After Connection";

            try
            {
                // prefix = gtaplgetAppealRequest();
                var getAppealRequest = new VIMTgtaplgetAppealRequest()
                {
                    LogTiming = request.LogTiming,
                    LogSoap = request.LogSoap,
                    Debug = request.Debug,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    LegacyServiceHeaderInfo = new VIMT.AppealService.Messages.HeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    },
                    getappealrequestInfo = new VIMTgtaplgetappealrequest { mcs_AppealKey = request.AppealKey }
                };

                //non standard fields
                //TODO(NP): Update the VIMT call to VEIS
                var getAppealResponse = getAppealRequest.SendReceive<VIMTgtaplgetAppealResponse>(MessageProcessType.Local);
                progressString = "After VIMT EC Call";

                response.ExceptionMessage = getAppealResponse.ExceptionMessage;
                response.ExceptionOccured = getAppealResponse.ExceptionOccured;
                var requestCollection = new OrganizationRequestCollection();

                if (getAppealResponse != null)
                {
                    var appealUpdate = 0;
                    if (getAppealResponse.VIMTgtaplGetAppealResponseInfo != null)
                    {
                        if (getAppealResponse.VIMTgtaplGetAppealResponseInfo.VIMTgtaplAppealRecordInfo != null)
                        {
                            var AppealRecord = getAppealResponse.VIMTgtaplGetAppealResponseInfo.VIMTgtaplAppealRecordInfo;

                            var thisAppealRecord = new Entity();
                            thisAppealRecord.Id = request.udo_appealId;
                            thisAppealRecord.LogicalName = "udo_appeal";

                            #region appellantData

                            if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppellantInfo.VIMTgtaplAppellantAddressInfo.mcs_AppellantAddressZipCode))
                            {
                                thisAppealRecord["udo_zipcode"] = AppealRecord.VIMTgtaplAppellantInfo.VIMTgtaplAppellantAddressInfo.mcs_AppellantAddressZipCode;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppellantInfo.VIMTgtaplAppellantAddressInfo.mcs_AppellantWorkPhoneNumber))
                            {
                                thisAppealRecord["udo_workphone"] = AppealRecord.VIMTgtaplAppellantInfo.VIMTgtaplAppellantAddressInfo.mcs_AppellantWorkPhoneNumber;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppellantInfo.mcs_AppellantTitle))
                            {
                                thisAppealRecord["udo_title"] = AppealRecord.VIMTgtaplAppellantInfo.mcs_AppellantTitle;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppellantInfo.VIMTgtaplAppellantAddressInfo.mcs_AppellantAddressStateCode))
                            {
                                thisAppealRecord["udo_state"] = AppealRecord.VIMTgtaplAppellantInfo.VIMTgtaplAppellantAddressInfo.mcs_AppellantAddressStateCode;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppealVeteranInfo.mcs_VeteranSSN))
                            {
                                thisAppealRecord["udo_ssn"] = AppealRecord.VIMTgtaplAppealVeteranInfo.mcs_VeteranSSN;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppellantInfo.mcs_AppellantRelationshipToVeteranCode))
                            {
                                thisAppealRecord["udo_relationshiptovetdesc"] = AppealRecord.VIMTgtaplAppellantInfo.mcs_AppellantRelationshipToVeteranCode;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppellantInfo.VIMTgtaplAppellantAddressInfo.mcs_AppellantAddressLastModifiedDate))
                            {
                                DateTime newDateTime;
                                if (DateTime.TryParse(AppealRecord.VIMTgtaplAppellantInfo.VIMTgtaplAppellantAddressInfo.mcs_AppellantAddressLastModifiedDate, out newDateTime))
                                {
                                    thisAppealRecord["udo_addressmoddate"] = newDateTime.ToShortDateString();
                                }
                                //                                thisAppealRecord["udo_addressmoddate"] = AppealRecord.VIMTgtaplAppellantInfo.VIMTgtaplAppellantAddressInfo.mcs_AppellantAddressLastModifiedDate;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppellantInfo.VIMTgtaplAppellantAddressInfo.mcs_AppellantAddressLastModifiedByROName))
                            {
                                thisAppealRecord["udo_addressmodby"] = AppealRecord.VIMTgtaplAppellantInfo.VIMTgtaplAppellantAddressInfo.mcs_AppellantAddressLastModifiedByROName;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppellantInfo.VIMTgtaplAppellantAddressInfo.mcs_AppellantAddressLine2))
                            {
                                thisAppealRecord["udo_address2"] = AppealRecord.VIMTgtaplAppellantInfo.VIMTgtaplAppellantAddressInfo.mcs_AppellantAddressLine2;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppellantInfo.VIMTgtaplAppellantAddressInfo.mcs_AppellantAddressLine1))
                            {
                                thisAppealRecord["udo_address1"] = AppealRecord.VIMTgtaplAppellantInfo.VIMTgtaplAppellantAddressInfo.mcs_AppellantAddressLine1;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppellantInfo.VIMTgtaplAppellantAddressInfo.mcs_AppellantHomePhoneNumber))
                            {
                                thisAppealRecord["udo_homephone"] = AppealRecord.VIMTgtaplAppellantInfo.VIMTgtaplAppellantAddressInfo.mcs_AppellantHomePhoneNumber;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppellantInfo.VIMTgtaplAppellantAddressInfo.mcs_AppellantAddressCountryName))
                            {
                                thisAppealRecord["udo_country"] = AppealRecord.VIMTgtaplAppellantInfo.VIMTgtaplAppellantAddressInfo.mcs_AppellantAddressCountryName;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppellantInfo.VIMTgtaplAppellantAddressInfo.mcs_AppellantAddressCityName))
                            {
                                thisAppealRecord["udo_city"] = AppealRecord.VIMTgtaplAppellantInfo.VIMTgtaplAppellantAddressInfo.mcs_AppellantAddressCityName;
                            }

                            if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppellantInfo.VIMTgtaplAppellantAddressInfo.mcs_AppellantAddressNotes))
                            {
                                thisAppealRecord["udo_addressnotes"] = AppealRecord.VIMTgtaplAppellantInfo.VIMTgtaplAppellantAddressInfo.mcs_AppellantAddressNotes;
                            }
                            var appellantName = "";
                            if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppellantInfo.mcs_AppellantLastName))
                            {
                                appellantName = AppealRecord.VIMTgtaplAppellantInfo.mcs_AppellantLastName;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppellantInfo.mcs_AppellantFirstName))
                            {
                                appellantName += ", " + AppealRecord.VIMTgtaplAppellantInfo.mcs_AppellantFirstName;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppellantInfo.mcs_AppellantMiddleInitial))
                            {
                                appellantName += " " + AppealRecord.VIMTgtaplAppellantInfo.mcs_AppellantMiddleInitial;
                            }
                            thisAppealRecord["udo_appellantname"] = appellantName;
                            #endregion
                            #region veteranInfo Data
                            if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppealVeteranInfo.mcs_VeteranGender))
                            {
                                thisAppealRecord["udo_gender"] = AppealRecord.VIMTgtaplAppealVeteranInfo.mcs_VeteranGender;
                            }
                            if (AppealRecord.VIMTgtaplAppealVeteranInfo.mcs_FinalNoticeOfDeathDateSpecified)
                            {
                                thisAppealRecord["udo_fnoddate"] = AppealRecord.VIMTgtaplAppealVeteranInfo.mcs_FinalNoticeOfDeathDate;
                            }
                            if (AppealRecord.VIMTgtaplAppealVeteranInfo.mcs_BirthDateSpecified)
                            {
                                thisAppealRecord["udo_birthdate"] = AppealRecord.VIMTgtaplAppealVeteranInfo.mcs_BirthDate;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppealVeteranInfo.mcs_VeteranGender))
                            {
                                thisAppealRecord["udo_gender"] = AppealRecord.VIMTgtaplAppealVeteranInfo.mcs_VeteranGender;
                            }
                            var veteranName = "";
                            if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppealVeteranInfo.mcs_VeteranLastName))
                            {
                                veteranName = AppealRecord.VIMTgtaplAppealVeteranInfo.mcs_VeteranLastName;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppealVeteranInfo.mcs_VeteranFirstName))
                            {
                                veteranName += ", " + AppealRecord.VIMTgtaplAppealVeteranInfo.mcs_VeteranFirstName;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppealVeteranInfo.mcs_VeteranMiddleInitial))
                            {
                                veteranName += " " + AppealRecord.VIMTgtaplAppealVeteranInfo.mcs_VeteranMiddleInitial;
                            }
                            thisAppealRecord["udo_veteranname"] = veteranName;

                            #endregion

                            #region base Appeal Data
                            if (AppealRecord.mcs_ServiceOrganizationReceivedDateSpecified)
                            {
                                thisAppealRecord["udo_svcorgrecdate"] = AppealRecord.mcs_ServiceOrganizationReceivedDate;
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
                            if (AppealRecord.mcs_DecisionReviewOfficerElectedDate != System.DateTime.MinValue)
                            {
                                thisAppealRecord["udo_droelecteddate"] = AppealRecord.mcs_DecisionReviewOfficerElectedDate;
                            }
                            if (AppealRecord.mcs_DocketDateSpecified)
                            {
                                thisAppealRecord["udo_docketdate"] = AppealRecord.mcs_DocketDate;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.mcs_DocketNumber))
                            {
                                thisAppealRecord["udo_docketnumber"] = AppealRecord.mcs_DocketNumber;
                            }

                            if (AppealRecord.mcs_ChargeToCurrentLocationDateSpecified)
                            {
                                thisAppealRecord["udo_chrgtocurrentlocation"] = AppealRecord.mcs_ChargeToCurrentLocationDate;
                            }
                            if (AppealRecord.mcs_BVAReceivedDateSpecified)
                            {
                                thisAppealRecord["udo_bvareceivedate"] = AppealRecord.mcs_BVAReceivedDate;
                            }
                            if (!string.IsNullOrEmpty(AppealRecord.mcs_ActionTypeDescription))
                            {
                                thisAppealRecord["udo_appealactiontypedesc"] = AppealRecord.mcs_ActionTypeDescription;
                            }


                            #endregion
                            #region decision fields
                            if (AppealRecord.VIMTgtaplAppealDecisionInfo != null)
                            {
                                if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppealDecisionInfo.mcs_DecisionTeamCode))
                                {
                                    thisAppealRecord["udo_teamcode"] = AppealRecord.VIMTgtaplAppealDecisionInfo.mcs_DecisionTeamCode;
                                }
                                //LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateUDOAppealDetailsProcessor", AppealRecord.VIMTgtaplAppealDecisionInfo.mcs_DecisionDateSpecified.ToString());
                                //LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreateUDOAppealDetailsProcessor", AppealRecord.VIMTgtaplAppealDecisionInfo.mcs_DecisionDate.ToLongDateString());

                                if (AppealRecord.VIMTgtaplAppealDecisionInfo.mcs_DecisionDateSpecified)
                                {
                                    thisAppealRecord["udo_decisiondate"] = AppealRecord.VIMTgtaplAppealDecisionInfo.mcs_DecisionDate;
                                    thisAppealRecord["udo_date"] = AppealRecord.VIMTgtaplAppealDecisionInfo.mcs_DecisionDate;
                                }
                                if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppealDecisionInfo.mcs_DecisionRemandedToName))
                                {
                                    thisAppealRecord["udo_remandtoname"] = AppealRecord.VIMTgtaplAppealDecisionInfo.mcs_DecisionRemandedToName;
                                }
                                if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppealDecisionInfo.mcs_RemandedToCode))
                                {
                                    thisAppealRecord["udo_remandtocode"] = AppealRecord.VIMTgtaplAppealDecisionInfo.mcs_RemandedToCode;
                                }
                                if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppealDecisionInfo.mcs_HearingActionDescription))
                                {
                                    thisAppealRecord["udo_hearingactiondescription"] = AppealRecord.VIMTgtaplAppealDecisionInfo.mcs_HearingActionDescription;
                                }
                                if (!string.IsNullOrEmpty(AppealRecord.VIMTgtaplAppealDecisionInfo.mcs_DecisionDispositionDescription))
                                {
                                    thisAppealRecord["udo_dispositiondescription"] = AppealRecord.VIMTgtaplAppealDecisionInfo.mcs_DecisionDispositionDescription;
                                }
                            }
                            #endregion

                            appealUpdate += 1;
                            if (AppealRecord.VIMTgtaplHearingRequestInfo != null)
                            {
                                var HearingRecords = AppealRecord.VIMTgtaplHearingRequestInfo;
                                //var latestHearingRecordDate = HearingRecords.Max(h => h.mcs_HearingRequestScheduledDate);
                                //var HearingRequestItem = HearingRecords.Where(h => h.mcs_HearingRequestScheduledDate == latestHearingRecordDate).FirstOrDefault();
                                var HearingRequestItem = HearingRecords.OrderByDescending(h => h.mcs_HearingRequestScheduledDate).FirstOrDefault();

                                //foreach (var HearingRequestItem in HearingRecords)
                                //{
                                if (HearingRequestItem.mcs_HearingRequestClosedDateSpecified)
                                {
                                    thisAppealRecord["udo_closeddate"] = HearingRequestItem.mcs_HearingRequestClosedDate;
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
                                    thisAppealRecord["udo_requestdate"] = HearingRequestItem.mcs_HearingRequestRequestedDate;
                                }
                                if (HearingRequestItem.mcs_HearingRequestScheduledDateSpecified)
                                {
                                    thisAppealRecord["udo_scheduledate"] = HearingRequestItem.mcs_HearingRequestScheduledDate;
                                }
                                //}
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
                    if (getAppealResponse.VIMTgtaplGetAppealResponseInfo != null)
                    {
                        if (getAppealResponse.VIMTgtaplGetAppealResponseInfo.VIMTgtaplAppealRecordInfo != null)
                        {
                            if (getAppealResponse.VIMTgtaplGetAppealResponseInfo.VIMTgtaplAppealRecordInfo.VIMTgtaplAppealDateInfo != null)
                            {
                                var AppealDate = getAppealResponse.VIMTgtaplGetAppealResponseInfo.VIMTgtaplAppealRecordInfo.VIMTgtaplAppealDateInfo;

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
                                        thisNewEntity["udo_date"] = AppealDateItem.mcs_Date;
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
                    if (getAppealResponse.VIMTgtaplGetAppealResponseInfo != null)
                    {
                        if (getAppealResponse.VIMTgtaplGetAppealResponseInfo.VIMTgtaplAppealRecordInfo != null)
                        {
                            if (getAppealResponse.VIMTgtaplGetAppealResponseInfo.VIMTgtaplAppealRecordInfo.VIMTgtaplRepFeesInfo != null)
                            {
                                var AttorneyInformation = getAppealResponse.VIMTgtaplGetAppealResponseInfo.VIMTgtaplAppealRecordInfo.VIMTgtaplRepFeesInfo;

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
                                        thisNewEntity["udo_feedate"] = AttorneyInfo.mcs_FeeDate.ToString();
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
                    if (getAppealResponse.VIMTgtaplGetAppealResponseInfo != null)
                    {
                        if (getAppealResponse.VIMTgtaplGetAppealResponseInfo.VIMTgtaplAppealRecordInfo != null)
                        {
                            if (getAppealResponse.VIMTgtaplGetAppealResponseInfo.VIMTgtaplAppealRecordInfo.VIMTgtaplIssueInfo != null)
                            {
                                var AppealIssues = getAppealResponse.VIMTgtaplGetAppealResponseInfo.VIMTgtaplAppealRecordInfo.VIMTgtaplIssueInfo;

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
                                        thisNewEntity["udo_dispositiondate"] = AppealIssueItem.mcs_IssueDispositionDate;
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
                                    if (AppealIssueItem.VIMTgtaplRemandReasonInfo != null)
                                    {
                                        var RemandReasons = AppealIssueItem.VIMTgtaplRemandReasonInfo;
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
                    if (getAppealResponse.VIMTgtaplGetAppealResponseInfo != null)
                    {
                        if (getAppealResponse.VIMTgtaplGetAppealResponseInfo.VIMTgtaplAppealRecordInfo != null)
                        {
                            if (getAppealResponse.VIMTgtaplGetAppealResponseInfo.VIMTgtaplAppealRecordInfo.VIMTgtaplDiaryInfo != null)
                            {
                                var AppealDiaries = getAppealResponse.VIMTgtaplGetAppealResponseInfo.VIMTgtaplAppealRecordInfo.VIMTgtaplDiaryInfo;

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
                                        thisNewEntity["udo_duedate"] = AppealDiariesItem.mcs_DiarySuspenseDueDate;
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
                                        thisNewEntity["udo_assigneddate"] = AppealDiariesItem.mcs_AssignedToStaffMemberDate;
                                    }
                                    if (!string.IsNullOrEmpty(AppealDiariesItem.mcs_DiaryStatusDescription))
                                    {
                                        thisNewEntity["udo_status"] = AppealDiariesItem.mcs_DiaryStatusDescription;
                                    }
                                    if (AppealDiariesItem.mcs_DiaryClosedDateSpecified)
                                    {
                                        thisNewEntity["udo_closeddate"] = AppealDiariesItem.mcs_DiaryClosedDate;
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
                    if (getAppealResponse.VIMTgtaplGetAppealResponseInfo != null)
                    {
                        if (getAppealResponse.VIMTgtaplGetAppealResponseInfo.VIMTgtaplAppealRecordInfo != null)
                        {
                            if (getAppealResponse.VIMTgtaplGetAppealResponseInfo.VIMTgtaplAppealRecordInfo.VIMTgtaplAppealDecisionInfo != null)
                            {
                                if (getAppealResponse.VIMTgtaplGetAppealResponseInfo.VIMTgtaplAppealRecordInfo.VIMTgtaplAppealDecisionInfo.VIMTgtaplSpecialContentionsInfo != null)
                                {
                                    var SpecialContentions = getAppealResponse.VIMTgtaplGetAppealResponseInfo.VIMTgtaplAppealRecordInfo.VIMTgtaplAppealDecisionInfo.VIMTgtaplSpecialContentionsInfo;
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
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming));
                }

                return response;
            }

            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "createUDOAppealsDetailsProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Appeals Data";
                response.ExceptionOccured = true;
                return response;
            }
        }
    }
}