using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using VIMT.ClaimantWebService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Awards.Messages;
using VRM.Integration.UDO.Common;

namespace VRM.Integration.UDO.Awards.Processors
{
    class UDOretrieveAwardProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOretrieveAwardProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOretrieveAwardRequest request)
        {
            if (request != null && request.Debug)
            {
                LogHelper.LogDebug(request.OrganizationName,
                    request.Debug,
                    request.UserId,
                    MethodInfo.GetThisMethod().ToString(),
                    String.Format("Build Version: {0}", this.GetType().Assembly.GetName().Version.ToString())
                );
                
            }

            //var request = message as createAwardsRequest;
            UDOretrieveAwardResponse response = new UDOretrieveAwardResponse();
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request.LogSoap)
            {
                LogHelper.LogDebug(request.OrganizationName, request.LogSoap, request.UserId, "UDOretrieveAwardProcessor", request.SerializeToString());
            }

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
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOretrieveAwardProcessor Processor, Connection Error", connectException.Message);      
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

            progressString = "After Connection";

            try
            {
                var noData = true;

                Entity thisEntity = new Entity("udo_award");
                thisEntity.Id = request.AwardId;
                Entity vetSnapShot = new Entity();
                progressString = getAwardInformation(request, response, "Getting Award Information (getAwardInformation)", thisEntity, vetSnapShot);
                if (request.Debug) LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOretrieveAwardProcessor", progressString);

                if (!String.IsNullOrEmpty(request.ptcpntVetId))
                    progressString = getGeneralInfoByPtcpntIds(request, response, "Getting General Info By Participant IDs (getGeneralInfoByPtcpntIds)", thisEntity);
                else if (!String.IsNullOrEmpty(request.fileNumber))
                    progressString = getGeneralInfoByFNIds(request, response, "Getting General Info By FileNumber (getGeneralInfoByFNIds)", thisEntity);

                if (request.Debug) LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOretrieveAwardProcessor", progressString);
                progressString = getFidInformation(OrgServiceProxy, request, response, progressString, thisEntity);
                if (request.Debug) LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOretrieveAwardProcessor", progressString);

                OrgServiceProxy.Update(TruncateHelper.TruncateFields(thisEntity, request.OrganizationName, request.UserId, request.LogTiming));

                if (!request.AwardId.Equals(Guid.Empty) && noData)
                {
                    var parent = new Entity();
                    parent.Id = request.AwardId;
                    parent.LogicalName = "udo_award";
                    parent["udo_awardinfocomplete"] = true;
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming));
                }

                if (request.vetSnapShotId != Guid.Empty)
                {

                    vetSnapShot.LogicalName = "udo_veteransnapshot";
                    vetSnapShot.Id = request.vetSnapShotId;
                    if (thisEntity.Attributes.Contains("udo_paystatusname"))
                    {
                        vetSnapShot["udo_paymentstatus"] = thisEntity["udo_paystatusname"].ToString();
                    }

                    vetSnapShot["udo_awardscompleted"] = new OptionSetValue(752280002);
                    vetSnapShot["udo_awardscomplete"] = true;
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(vetSnapShot, request.OrganizationName, request.UserId, request.LogTiming));
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOretrieveAwardProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Award Detail Data";
                response.ExceptionOccured = true;

                return response;
            }
        }


        private static string getFidInformation(IOrganizationService OrgServiceProxy, UDOretrieveAwardRequest request, UDOretrieveAwardResponse response, string progressString, Entity thisNewEntity/*, Entity peopleEntity*/)
        {
            progressString = "After getFiduciary";
            
            var findFiduciaryRequest = new VIMTfidfindFiduciaryRequest();
            findFiduciaryRequest.LogTiming = request.LogTiming;
            findFiduciaryRequest.LogSoap = request.LogSoap;
            findFiduciaryRequest.Debug = request.Debug;
            findFiduciaryRequest.RelatedParentEntityName = request.RelatedParentEntityName;
            findFiduciaryRequest.RelatedParentFieldName = request.RelatedParentFieldName;
            findFiduciaryRequest.RelatedParentId = request.RelatedParentId;
            findFiduciaryRequest.UserId = request.UserId;
            findFiduciaryRequest.OrganizationName = request.OrganizationName;


            var fn = request.fileNumber;
            if (!String.IsNullOrEmpty(thisNewEntity.GetAttributeValue<string>("udo_payeessn")))
            {
                fn = thisNewEntity.GetAttributeValue<string>("udo_payeessn");
            }
            findFiduciaryRequest.mcs_filenumber = fn;

            if (request.LegacyServiceHeaderInfo != null)
            {
                findFiduciaryRequest.LegacyServiceHeaderInfo = new VIMT.ClaimantWebService.Messages.HeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
            }
            // TODO(TN): Commented to remediate
            var findFiduciaryResponse = new VIMTfidfindFiduciaryResponse();
            // var findFiduciaryResponse = findFiduciaryRequest.SendReceive<VIMTfidfindFiduciaryResponse>(MessageProcessType.Local);
            progressString = "After VIMTfidfindFiduciaryRequest EC Call";

            response.ExceptionMessage = findFiduciaryResponse.ExceptionMessage;
            response.ExceptionOccured = findFiduciaryResponse.ExceptionOccured;
            if (findFiduciaryResponse.VIMTfidreturnclmsInfo != null)
            {
                var fidInfo = findFiduciaryResponse.VIMTfidreturnclmsInfo;


                //peopleEntity["udo_fidexists"] = true;
                //now done for 00 in awards.
                //  progressString = updatePersonFidExists(OrgServiceProxy, request);

                //if (!string.IsNullOrEmpty(fidInfo.mcs_personOrganizationName))
                //{
                //    //thisNewEntity["udo_name"] = fidInfo.mcs_personOrganizationName;
                //}
                if (!string.IsNullOrEmpty(fidInfo.mcs_relationshipName))
                {
                    thisNewEntity["udo_fiduciaryrelationshipname"] = fidInfo.mcs_relationshipName;
                }
                if (!string.IsNullOrEmpty(fidInfo.mcs_personOrgName))
                {
                    thisNewEntity["udo_fiduciaryname"] = fidInfo.mcs_personOrgName;
                }
                if (!string.IsNullOrEmpty(fidInfo.mcs_endDate))
                {
                    DateTime newDateTime;
                    var newDate = dateStringFormat(fidInfo.mcs_endDate);
                    if (DateTime.TryParse(newDate, out newDateTime))
                    {
                        thisNewEntity["udo_fiduciaryenddate"] = newDateTime;
                    }
                }
                if (!string.IsNullOrEmpty(fidInfo.mcs_beginDate))
                {
                    DateTime newDateTime;
                    var newDate = dateStringFormat(fidInfo.mcs_beginDate);
                    if (DateTime.TryParse(newDate, out newDateTime))
                    {
                        thisNewEntity["udo_fiduciarybegindate"] = newDateTime;
                    }
                }
            }
            return progressString;
             
        }

        private static string updatePersonFidExists(IOrganizationService OrgServiceProxy, UDOretrieveAwardRequest request)
        {
            FetchExpression fe = new FetchExpression(
                @"<fetch mapping='logical' page='1' count='1'><entity name='udo_person'><attribute name='udo_personid' />" +
                @"<filter type='and'><condition attribute='udo_ptcpntid' operator='eq' value='" + request.ptcpntRecipId + "' /></filter>" +
                @"</entity></fetch>");

            var persons = OrgServiceProxy.RetrieveMultiple(fe);
            if (persons.Entities != null && persons.Entities.Count > 0)
            {
                var person = persons[0];
                person["udo_fidexists"] = true;
                OrgServiceProxy.Update(TruncateHelper.TruncateFields(person, request.OrganizationName, request.UserId, request.LogTiming));
            }

            return "updatePersonFidExists completed";
        }

        private static string getAwardInformation(UDOretrieveAwardRequest request, UDOretrieveAwardResponse response, string progressString, Entity entity, Entity vetSnapShot)
        {
            var findOtherAwardInformationRequest = new VIMTfoawdinfofindOtherAwardInformationRequest();
            findOtherAwardInformationRequest.LogTiming = request.LogTiming;
            findOtherAwardInformationRequest.LogSoap = request.LogSoap;
            findOtherAwardInformationRequest.Debug = request.Debug;
            findOtherAwardInformationRequest.RelatedParentEntityName = request.RelatedParentEntityName;
            findOtherAwardInformationRequest.RelatedParentFieldName = request.RelatedParentFieldName;
            findOtherAwardInformationRequest.RelatedParentId = request.RelatedParentId;
            findOtherAwardInformationRequest.UserId = request.UserId;
            findOtherAwardInformationRequest.OrganizationName = request.OrganizationName;

            findOtherAwardInformationRequest.mcs_ptcpntvetid = request.ptcpntVetId;
            findOtherAwardInformationRequest.mcs_ptcpntbeneid = /*String.IsNullOrEmpty(request.ptcpntBeneId) ? request.ptcpntVetId :*/ request.ptcpntBeneId;
            findOtherAwardInformationRequest.mcs_ptcpntrecipid = /*String.IsNullOrEmpty(request.ptcpntRecipId) ? request.ptcpntVetId :*/ request.ptcpntRecipId;
            findOtherAwardInformationRequest.mcs_awardtypecd = request.awardTypeCd;
            if (request.LegacyServiceHeaderInfo != null)
            {
                findOtherAwardInformationRequest.LegacyServiceHeaderInfo = new VIMT.ClaimantWebService.Messages.HeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
            }
            // TODO(TN): Commented to remediate
            var findOtherAwardInformationResponse = new VIMTfoawdinfofindOtherAwardInformationResponse();
            // var findOtherAwardInformationResponse = findOtherAwardInformationRequest.SendReceive<VIMTfoawdinfofindOtherAwardInformationResponse>(MessageProcessType.Local);
            progressString = "After VIMTfoawdinfofindOtherAwardInformationRequest EC Call";

            response.ExceptionMessage = findOtherAwardInformationResponse.ExceptionMessage;
            response.ExceptionOccured = findOtherAwardInformationResponse.ExceptionOccured;
            if (findOtherAwardInformationResponse.VIMTfoawdinforeturnclmsInfo != null)
            {
                if (findOtherAwardInformationResponse.VIMTfoawdinforeturnclmsInfo.VIMTfoawdinfoawardInfoclmsInfo != null)
                {
                    var awardInfo = findOtherAwardInformationResponse.VIMTfoawdinforeturnclmsInfo.VIMTfoawdinfoawardInfoclmsInfo;

                    if (!string.IsNullOrEmpty(awardInfo.mcs_auditRelatedAr))
                    {
                        entity["udo_auditrelatedar"] = awardInfo.mcs_auditRelatedAr;
                    }
                    //if (!string.IsNullOrEmpty(awardInfo.mcs_beneCd;
                    //if (!string.IsNullOrEmpty( awardInfo.mcs_beneFirstName;
                    //if (!string.IsNullOrEmpty(awardInfo.mcs_beneLastName;
                    //if (!string.IsNullOrEmpty(awardInfo.mcs_beneMiddleName;

                    if (!string.IsNullOrEmpty(awardInfo.mcs_beneName))
                    {
                        entity["udo_beneficiarytype"] = awardInfo.mcs_beneName;
                    }
                    //if (!string.IsNullOrEmpty(awardInfo.mcs_bnftCd;
                    if (!string.IsNullOrEmpty(awardInfo.mcs_bnftName))
                    {
                        entity["udo_benefittype"] = awardInfo.mcs_bnftName;
                    }
                    //if (!string.IsNullOrEmpty(awardInfo.mcs_fidType;
                    //if (!string.IsNullOrEmpty(awardInfo.mcs_frequencyCd;
                    if (!string.IsNullOrEmpty(awardInfo.mcs_frequencyName))
                    {
                        entity["udo_frequency"] = awardInfo.mcs_frequencyName;
                    }

                    if (!string.IsNullOrEmpty(awardInfo.mcs_igReferenceNbr))
                    {
                        entity["udo_igreferencenum"] = awardInfo.mcs_igReferenceNbr;
                    }

                    if (!string.IsNullOrEmpty(awardInfo.mcs_lastPaidDate))
                    {
                        entity["udo_lastpaiddate"] = dateStringFormat(awardInfo.mcs_lastPaidDate);
                    }

                    entity["udo_beneficiaryfullname"] = awardInfo.mcs_beneLastName + ", " + awardInfo.mcs_beneFirstName + " " + awardInfo.mcs_beneMiddleName;

                    //if (!string.IsNullOrEmpty(awardInfo.mcs_payStatusCd;
                    //if (!string.IsNullOrEmpty(awardInfo.mcs_payStatusName;
                    //if (!string.IsNullOrEmpty(awardInfo.mcs_recipName;

                    if (!string.IsNullOrEmpty(awardInfo.mcs_requestedFrequency))
                    {
                        entity["udo_requestedfrequency"] = awardInfo.mcs_requestedFrequency;
                    }
                    if (!string.IsNullOrEmpty(awardInfo.mcs_retroactiveDate))
                    {
                        DateTime newDateTime;
                        var newDate = dateStringFormat(awardInfo.mcs_retroactiveDate);
                        if (DateTime.TryParse(newDate, out newDateTime))
                        {
                            entity["udo_retroactivedate"] = newDateTime;
                        }
                    }
                    if (!string.IsNullOrEmpty(awardInfo.mcs_statusReasonCd))
                    {
                        entity["udo_statusreasoncode"] = awardInfo.mcs_statusReasonCd;
                    }
                    if (!string.IsNullOrEmpty(awardInfo.mcs_statusReasonDate))
                    {
                        DateTime newDateTime;
                        var newDate = dateStringFormat(awardInfo.mcs_statusReasonDate);
                        if (DateTime.TryParse(newDate, out newDateTime))
                        {
                            entity["udo_statusreasondate"] = newDateTime;

                        }
                        vetSnapShot["udo_effectivedate"] = awardInfo.mcs_statusReasonDate;
                    }
                    if (!string.IsNullOrEmpty(awardInfo.mcs_statusReasonName))
                    {
                        entity["udo_statusreasonname"] = awardInfo.mcs_statusReasonName;
                    }
                    if (!string.IsNullOrEmpty(awardInfo.mcs_vetFirstName))
                    {
                        entity["udo_vetfirstname"] = awardInfo.mcs_vetFirstName;
                        vetSnapShot["udo_firstname"] = awardInfo.mcs_vetFirstName;
                        //peopleEntity["udo_vetfirstname"] = awardInfo.mcs_vetFirstName;
                    }
                    if (!string.IsNullOrEmpty(awardInfo.mcs_vetLastName))
                    {
                        entity["udo_vetlastname"] = awardInfo.mcs_vetLastName;
                        vetSnapShot["udo_lastname"] = awardInfo.mcs_vetLastName;
                        //peopleEntity["udo_vetlastname"] = awardInfo.mcs_vetLastName;
                    }
                    //if (!string.IsNullOrEmpty(awardInfo.mcs_vetMiddleName;


                    //peopleEntity["udo_name"] = awardInfo.mcs_vetFirstName + " " + awardInfo.mcs_vetLastName;

                    if (!string.IsNullOrEmpty(awardInfo.mcs_payStatusCd))
                    {
                        entity["udo_paystatuscode"] = awardInfo.mcs_payStatusCd;
                    }
                    if (!string.IsNullOrEmpty(awardInfo.mcs_payStatusName))
                    {
                        entity["udo_paystatusname"] = awardInfo.mcs_payStatusName;
                    }

                }
            }

            return progressString;
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

        private static string moneyStringFormat(string thisField)
        {
            var returnField = "";
            try
            {
                Double newValue = 0;
                if (Double.TryParse(thisField, out newValue))
                {
                    returnField = string.Format("{0:C}", newValue);
                }
                else
                {
                    returnField = "$0.00";
                }
            }
            catch (Exception ex)
            {
                returnField = ex.Message;
            }
            return returnField;

        }

        private static string getGeneralInfoByFNIds(UDOretrieveAwardRequest request, UDOretrieveAwardResponse response, string progressString, Entity entity)
        {
             try
            {                
                var findGeneralInformationByFileNumberRequest = new VIMTfgenFNfindGeneralInformationByFileNumberRequest();
                findGeneralInformationByFileNumberRequest.LogTiming = request.LogTiming;
                findGeneralInformationByFileNumberRequest.LogSoap = request.LogSoap;
                findGeneralInformationByFileNumberRequest.Debug = request.Debug;
                findGeneralInformationByFileNumberRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findGeneralInformationByFileNumberRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findGeneralInformationByFileNumberRequest.RelatedParentId = request.RelatedParentId;
                findGeneralInformationByFileNumberRequest.UserId = request.UserId;
                findGeneralInformationByFileNumberRequest.OrganizationName = request.OrganizationName;
                if (request.LegacyServiceHeaderInfo != null)
                {
                    findGeneralInformationByFileNumberRequest.LegacyServiceHeaderInfo = new VIMT.ClaimantWebService.Messages.HeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                }

                findGeneralInformationByFileNumberRequest.mcs_filenumber = request.fileNumber;

                // TODO(TN): Commented to remediate
                var findGeneralInformationByFNResponse = new VIMTfgenpidfindGeneralInformationByPtcpntIdsResponse();
                // var findGeneralInformationByFNResponse = findGeneralInformationByFileNumberRequest.SendReceive<VIMTfgenpidfindGeneralInformationByPtcpntIdsResponse>(MessageProcessType.Local);
                progressString = "After VIMT EC Call";

                response.ExceptionMessage = findGeneralInformationByFNResponse.ExceptionMessage;
                response.ExceptionOccured = findGeneralInformationByFNResponse.ExceptionOccured;
                if (findGeneralInformationByFNResponse.VIMTfgenpidreturnclmsInfo != null)
                {

                    var generalInfo = findGeneralInformationByFNResponse.VIMTfgenpidreturnclmsInfo;

                    if (!string.IsNullOrEmpty(generalInfo.mcs_currentMonthlyRate))
                    {
                        entity["udo_currmonthlyrate"] = moneyStringFormat(generalInfo.mcs_currentMonthlyRate);
                    }

                    if (!string.IsNullOrEmpty(generalInfo.mcs_powerOfAttorney))
                    {
                        entity["udo_poa"] = generalInfo.mcs_powerOfAttorney;
                    }
                    if (!string.IsNullOrEmpty(generalInfo.mcs_payeeBirthDate))
                    {
                        entity["udo_payeedobstr"] = dateStringFormat(generalInfo.mcs_payeeBirthDate);
                    }
                    if (!string.IsNullOrEmpty(generalInfo.mcs_payeeSex))
                    {
                        entity["udo_payeesex"] = generalInfo.mcs_payeeSex;
                    }
                    if (!string.IsNullOrEmpty(generalInfo.mcs_payeeSSN))
                    {
                        entity["udo_payeessn"] = generalInfo.mcs_payeeSSN;
                    }
                    if (!string.IsNullOrEmpty(generalInfo.mcs_payeeName))
                    {
                        entity["udo_payeename"] = generalInfo.mcs_payeeName;
                    }
                    if (!string.IsNullOrEmpty(generalInfo.mcs_stationOfJurisdiction))
                    {
                        entity["udo_soj"] = generalInfo.mcs_stationOfJurisdiction;
                    }

                    if (!string.IsNullOrEmpty(generalInfo.mcs_paidThroughDate))
                    {
                        entity["udo_paidthrough"] = dateStringFormat(generalInfo.mcs_paidThroughDate);
                    }
                    if (!string.IsNullOrEmpty(generalInfo.mcs_clothingAllowanceTypeCode))
                    {
                        entity["udo_clothingallowancecode"] = generalInfo.mcs_clothingAllowanceTypeCode;
                    }
                    if (!string.IsNullOrEmpty(generalInfo.mcs_clothingAllowanceTypeName))
                    {
                        entity["udo_clothingallowancename"] = generalInfo.mcs_clothingAllowanceTypeName;
                    }
                    if (!string.IsNullOrEmpty(generalInfo.mcs_directDepositAccountID))
                    {
                        entity["udo_directdepositid"] = generalInfo.mcs_directDepositAccountID;
                    }
                    if (!string.IsNullOrEmpty(generalInfo.mcs_competencyDecisionTypeCode))
                    {
                        entity["udo_competency"] = generalInfo.mcs_competencyDecisionTypeCode + ":" + generalInfo.mcs_competencyDecisionTypeName;
                    }
                    if (!string.IsNullOrEmpty(generalInfo.mcs_fiduciaryDecisionTypeCode))
                    {
                        entity["udo_fiduciary"] = generalInfo.mcs_fiduciaryDecisionTypeCode + ":" + generalInfo.mcs_fiduciaryDecisionTypeName;
                    }
                    if (!string.IsNullOrEmpty(generalInfo.mcs_payeeTypeCode))
                    {
                        entity["udo_payeetype"] = generalInfo.mcs_payeeTypeCode + ":" + generalInfo.mcs_payeeTypeName;
                    }
                }
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Map EC data to LOB";
                response.ExceptionOccured = true;
            }

            return progressString;
        }        

        private static string getGeneralInfoByPtcpntIds(UDOretrieveAwardRequest request, UDOretrieveAwardResponse response, string progressString, Entity entity)
        {
            try
            {                
                var findGeneralInformationByPtcpntIdsRequest = new VIMTfgenpidfindGeneralInformationByPtcpntIdsRequest();
                findGeneralInformationByPtcpntIdsRequest.LogTiming = request.LogTiming;
                findGeneralInformationByPtcpntIdsRequest.LogSoap = request.LogSoap;
                findGeneralInformationByPtcpntIdsRequest.Debug = request.Debug;
                findGeneralInformationByPtcpntIdsRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findGeneralInformationByPtcpntIdsRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findGeneralInformationByPtcpntIdsRequest.RelatedParentId = request.RelatedParentId;
                findGeneralInformationByPtcpntIdsRequest.UserId = request.UserId;
                findGeneralInformationByPtcpntIdsRequest.OrganizationName = request.OrganizationName;
                if (request.LegacyServiceHeaderInfo != null)
                {
                    findGeneralInformationByPtcpntIdsRequest.LegacyServiceHeaderInfo = new VIMT.ClaimantWebService.Messages.HeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                }
                findGeneralInformationByPtcpntIdsRequest.mcs_ptcpntvetid = request.ptcpntVetId;
                findGeneralInformationByPtcpntIdsRequest.mcs_ptcpntbeneid = request.ptcpntBeneId;
                findGeneralInformationByPtcpntIdsRequest.mcs_ptpcntrecipid = request.ptcpntRecipId;
                findGeneralInformationByPtcpntIdsRequest.mcs_awardtypecd = request.awardTypeCd;

                // TODO(TN): Commented to remediate
                var findGeneralInformationByPtcpntIdsResponse = new VIMTfgenpidfindGeneralInformationByPtcpntIdsResponse();
                // var findGeneralInformationByPtcpntIdsResponse = findGeneralInformationByPtcpntIdsRequest.SendReceive<VIMTfgenpidfindGeneralInformationByPtcpntIdsResponse>(MessageProcessType.Local);
                progressString = "After VIMT EC Call";

                response.ExceptionMessage = findGeneralInformationByPtcpntIdsResponse.ExceptionMessage;
                response.ExceptionOccured = findGeneralInformationByPtcpntIdsResponse.ExceptionOccured;
                if (findGeneralInformationByPtcpntIdsResponse.VIMTfgenpidreturnclmsInfo != null)
                {

                    var generalInfo = findGeneralInformationByPtcpntIdsResponse.VIMTfgenpidreturnclmsInfo;

                    if (!string.IsNullOrEmpty(generalInfo.mcs_currentMonthlyRate))
                    {
                        entity["udo_currmonthlyrate"] = moneyStringFormat(generalInfo.mcs_currentMonthlyRate);
                    }

                    if (!string.IsNullOrEmpty(generalInfo.mcs_powerOfAttorney))
                    {
                        entity["udo_poa"] = generalInfo.mcs_powerOfAttorney;
                    }
                    if (!string.IsNullOrEmpty(generalInfo.mcs_payeeBirthDate))
                    {
                        entity["udo_payeedobstr"] = dateStringFormat(generalInfo.mcs_payeeBirthDate);
                    }
                    if (!string.IsNullOrEmpty(generalInfo.mcs_payeeSex))
                    {
                        entity["udo_payeesex"] = generalInfo.mcs_payeeSex;
                    }
                    if (!string.IsNullOrEmpty(generalInfo.mcs_payeeSSN))
                    {
                        entity["udo_payeessn"] = generalInfo.mcs_payeeSSN;
                    }
                    if (!string.IsNullOrEmpty(generalInfo.mcs_payeeName))
                    {
                        entity["udo_payeename"] = generalInfo.mcs_payeeName;
                    }
                    if (!string.IsNullOrEmpty(generalInfo.mcs_stationOfJurisdiction))
                    {
                        entity["udo_soj"] = generalInfo.mcs_stationOfJurisdiction;
                    }

                    if (!string.IsNullOrEmpty(generalInfo.mcs_paidThroughDate))
                    {
                        entity["udo_paidthrough"] = dateStringFormat(generalInfo.mcs_paidThroughDate);
                    }
                    if (!string.IsNullOrEmpty(generalInfo.mcs_clothingAllowanceTypeCode))
                    {
                        entity["udo_clothingallowancecode"] = generalInfo.mcs_clothingAllowanceTypeCode;
                    }
                    if (!string.IsNullOrEmpty(generalInfo.mcs_clothingAllowanceTypeName))
                    {
                        entity["udo_clothingallowancename"] = generalInfo.mcs_clothingAllowanceTypeName;
                    }
                    if (!string.IsNullOrEmpty(generalInfo.mcs_directDepositAccountID))
                    {
                        entity["udo_directdepositid"] = generalInfo.mcs_directDepositAccountID;
                    }
                    if (!string.IsNullOrEmpty(generalInfo.mcs_competencyDecisionTypeCode))
                    {
                        entity["udo_competency"] = generalInfo.mcs_competencyDecisionTypeCode + ":" + generalInfo.mcs_competencyDecisionTypeName;
                    }
                    if (!string.IsNullOrEmpty(generalInfo.mcs_fiduciaryDecisionTypeCode))
                    {
                        entity["udo_fiduciary"] = generalInfo.mcs_fiduciaryDecisionTypeCode + ":" + generalInfo.mcs_fiduciaryDecisionTypeName;
                    }
                    if (!string.IsNullOrEmpty(generalInfo.mcs_payeeTypeCode))
                    {
                        entity["udo_payeetype"] = generalInfo.mcs_payeeTypeCode + ":" + generalInfo.mcs_payeeTypeName;
                    }
                }
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Map EC data to LOB";
                response.ExceptionOccured = true;
            }

            return progressString;
        }

    }
}