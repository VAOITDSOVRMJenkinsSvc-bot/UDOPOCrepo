using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using UDO.LOB.Awards.Messages;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using VEIS.Messages.ClaimantService;

namespace UDO.LOB.Awards.Processors
{
    internal class UDOretrieveAwardProcessor
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
            TraceLogger tLogger = new TraceLogger(method, request);

            UDORetrieveAwardResponse response = new UDORetrieveAwardResponse();
            response.MessageId = request.MessageId;

            string progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request.LogSoap)
            {
                LogHelper.LogDebug(request.OrganizationName, request.LogSoap, request.UserId, "UDOretrieveAwardProcessor", JsonHelper.Serialize<UDOretrieveAwardRequest>(request));
            }

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }

            #region connect to CRM
            CrmServiceClient OrgServiceProxy;
            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
                tLogger.LogEvent($"Authenticated and Connected to CRM", "2");
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

            progressString = "After Connection";

            try
            {
                bool noData = true;

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

                OrgServiceProxy.Update(TruncateHelper.TruncateFields(request.MessageId, thisEntity, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));

                if (!request.AwardId.Equals(Guid.Empty) && noData)
                {
                    Entity parent = new Entity();
                    parent.Id = request.AwardId;
                    parent.LogicalName = "udo_award";
                    parent["udo_awardinfocomplete"] = true;
                    //OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                    OrgServiceProxy.Update(parent);
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
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(request.MessageId, vetSnapShot, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "UDOretrieveAwardProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Award Detail Data";
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


        private static string getFidInformation(IOrganizationService OrgServiceProxy, UDOretrieveAwardRequest request, UDORetrieveAwardResponse response, string progressString, Entity thisNewEntity/*, Entity peopleEntity*/)
        {
            progressString = "After getFiduciary";

            //var findFiduciaryRequest = new VIMTfidfindFiduciaryRequest();
            VEISfidfindFiduciaryRequest findFiduciaryRequest = new VEISfidfindFiduciaryRequest();
            findFiduciaryRequest.LogTiming = request.LogTiming;
            findFiduciaryRequest.LogSoap = request.LogSoap;
            findFiduciaryRequest.Debug = request.Debug;
            findFiduciaryRequest.RelatedParentEntityName = request.RelatedParentEntityName;
            findFiduciaryRequest.RelatedParentFieldName = request.RelatedParentFieldName;
            findFiduciaryRequest.RelatedParentId = request.RelatedParentId;
            findFiduciaryRequest.UserId = request.UserId;
            findFiduciaryRequest.OrganizationName = request.OrganizationName;


            string fn = request.fileNumber;
            if (!String.IsNullOrEmpty(thisNewEntity.GetAttributeValue<string>("udo_payeessn")))
            {
                fn = thisNewEntity.GetAttributeValue<string>("udo_payeessn");
            }
            findFiduciaryRequest.mcs_filenumber = fn;

            if (request.LegacyServiceHeaderInfo != null)
            {
                findFiduciaryRequest.LegacyServiceHeaderInfo = new VEIS.Core.Messages.LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
            }

            // REM: Invoke VEIS WebApi
            VEISfidfindFiduciaryResponse findFiduciaryResponse = WebApiUtility.SendReceive<VEISfidfindFiduciaryResponse>(findFiduciaryRequest, WebApiType.VEIS);
            if (request.LogSoap || findFiduciaryResponse.ExceptionOccurred)
            {
                if (findFiduciaryResponse.SerializedSOAPRequest != null || findFiduciaryResponse.SerializedSOAPResponse != null)
                {
                    var requestResponse = findFiduciaryResponse.SerializedSOAPRequest + findFiduciaryResponse.SerializedSOAPResponse;
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfidfindFiduciaryRequest Request/Response {requestResponse}", true);
                }
            }

            progressString = "After VIMTfidfindFiduciaryRequest EC Call";

            response.ExceptionMessage = findFiduciaryResponse.ExceptionMessage;
            response.ExceptionOccured = findFiduciaryResponse.ExceptionOccurred;

            if (findFiduciaryResponse.VEISfidreturnInfo != null)
            {
                VEISfidreturn fidInfo = findFiduciaryResponse.VEISfidreturnInfo;

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
                    string newDate = dateStringFormat(fidInfo.mcs_endDate);
                    if (DateTime.TryParse(newDate, out newDateTime))
                    {
                        thisNewEntity["udo_fiduciaryenddate"] = newDateTime;
                    }
                }
                if (!string.IsNullOrEmpty(fidInfo.mcs_beginDate))
                {
                    DateTime newDateTime;
                    string newDate = dateStringFormat(fidInfo.mcs_beginDate);
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

            EntityCollection persons = OrgServiceProxy.RetrieveMultiple(fe);
            if (persons.Entities != null && persons.Entities.Count > 0)
            {
                Entity person = persons[0];
                person["udo_fidexists"] = true;
                //OrgServiceProxy.Update(TruncateHelper.TruncateFields(person, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                OrgServiceProxy.Update(person);
            }

            return "updatePersonFidExists completed";
        }

        private static string getAwardInformation(UDOretrieveAwardRequest request, UDORetrieveAwardResponse response, string progressString, Entity entity, Entity vetSnapShot)
        {
            // var findOtherAwardInformationRequest = new VIMTfoawdinfofindOtherAwardInformationRequest();
            //REM: 
            VEISfoawdinfofindOtherAwardInformationRequest findOtherAwardInformationRequest = new VEISfoawdinfofindOtherAwardInformationRequest();

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
                findOtherAwardInformationRequest.LegacyServiceHeaderInfo = new VEIS.Core.Messages.LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
            }

            // REM: Invoke VEIS 
            var findOtherAwardInformationResponse = WebApiUtility.SendReceive<VEISfoawdinfofindOtherAwardInformationResponse>(findOtherAwardInformationRequest, WebApiType.VEIS);
            if (request.LogSoap || findOtherAwardInformationResponse.ExceptionOccurred)
            {
                if (findOtherAwardInformationResponse.SerializedSOAPRequest != null || findOtherAwardInformationResponse.SerializedSOAPResponse != null)
                {
                    var requestResponse = findOtherAwardInformationResponse.SerializedSOAPRequest + findOtherAwardInformationResponse.SerializedSOAPResponse;
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfoawdinfofindOtherAwardInformationRequest Request/Response {requestResponse}", true);
                }
            }

            progressString = "After VIMTfoawdinfofindOtherAwardInformationRequest EC Call";

            response.ExceptionMessage = findOtherAwardInformationResponse.ExceptionMessage;
            response.ExceptionOccured = findOtherAwardInformationResponse.ExceptionOccurred;

            if (findOtherAwardInformationResponse.VEISfoawdinforeturnInfo != null)
            {
                if (findOtherAwardInformationResponse.VEISfoawdinforeturnInfo.VEISfoawdinfoawardInfoInfo != null)
                {
                    var awardInfo = findOtherAwardInformationResponse.VEISfoawdinforeturnInfo.VEISfoawdinfoawardInfoInfo;

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
                        string newDate = dateStringFormat(awardInfo.mcs_retroactiveDate);
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
                        string newDate = dateStringFormat(awardInfo.mcs_statusReasonDate);
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
            string returnField = "";
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

        private static string getGeneralInfoByFNIds(UDOretrieveAwardRequest request, UDORetrieveAwardResponse response, string progressString, Entity entity)
        {
            try
            {
                // var findGeneralInformationByFileNumberRequest = new VIMTfgenFNfindGeneralInformationByFileNumberRequest();
                //REM: VIMTfgenFNfindGeneralInformationByFileNumberRequest to VEISfgenFNfindGeneralInformationByFileNumberRequest
                VEISfgenFNfindGeneralInformationByFileNumberRequest findGeneralInformationByFileNumberRequest = new VEISfgenFNfindGeneralInformationByFileNumberRequest();

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
                    findGeneralInformationByFileNumberRequest.LegacyServiceHeaderInfo = new VEIS.Core.Messages.LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                }

                findGeneralInformationByFileNumberRequest.mcs_filenumber = request.fileNumber;

                //REM: Call to VEIS WebApi
                var findGeneralInformationByFNResponse = WebApiUtility.SendReceive<VEISfgenpidfindGeneralInformationByPtcpntIdsResponse>(findGeneralInformationByFileNumberRequest, WebApiType.VEIS);
                if (request.LogSoap || findGeneralInformationByFNResponse.ExceptionOccurred)
                {
                    if (findGeneralInformationByFNResponse.SerializedSOAPRequest != null || findGeneralInformationByFNResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findGeneralInformationByFNResponse.SerializedSOAPRequest + findGeneralInformationByFNResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfgenFNfindGeneralInformationByFileNumberRequest Request/Response {requestResponse}", true);
                    }
                }

                progressString = "After VEIS EC Call";

                response.ExceptionMessage = findGeneralInformationByFNResponse.ExceptionMessage;
                response.ExceptionOccured = findGeneralInformationByFNResponse.ExceptionOccurred;
                if (findGeneralInformationByFNResponse.VEISfgenpidreturnInfo != null)
                {

                    var generalInfo = findGeneralInformationByFNResponse.VEISfgenpidreturnInfo;

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
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, method + " Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Map EC data to LOB";
                response.ExceptionOccured = true;
            }

            return progressString;
        }

        private static string getGeneralInfoByPtcpntIds(UDOretrieveAwardRequest request, UDORetrieveAwardResponse response, string progressString, Entity entity)
        {
            try
            {
                //var findGeneralInformationByPtcpntIdsRequest = new VIMTfgenpidfindGeneralInformationByPtcpntIdsRequest();
                // REM: VIMTfgenpidfindGeneralInformationByPtcpntIdsRequest to VEISfgenpidfindGeneralInformationByPtcpntIdRequest
                var findGeneralInformationByPtcpntIdsRequest = new VEISfgenpidfindGeneralInformationByPtcpntIdsRequest();
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
                    findGeneralInformationByPtcpntIdsRequest.LegacyServiceHeaderInfo = new VEIS.Core.Messages.LegacyHeaderInfo
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
                

                //REM: Invoke VEIS WebApi 
                var findGeneralInformationByPtcpntIdsResponse = WebApiUtility.SendReceive<VEISfgenpidfindGeneralInformationByPtcpntIdsResponse>(findGeneralInformationByPtcpntIdsRequest, WebApiType.VEIS);
                if (request.LogSoap || findGeneralInformationByPtcpntIdsResponse.ExceptionOccurred)
                {
                    if (findGeneralInformationByPtcpntIdsResponse.SerializedSOAPRequest != null || findGeneralInformationByPtcpntIdsResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findGeneralInformationByPtcpntIdsResponse.SerializedSOAPRequest + findGeneralInformationByPtcpntIdsResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"Request Request/Response {requestResponse}", true);
                    }
                }

                progressString = "After VEIS EC Call";

                response.ExceptionMessage = findGeneralInformationByPtcpntIdsResponse.ExceptionMessage;
                response.ExceptionOccured = findGeneralInformationByPtcpntIdsResponse.ExceptionOccurred;
                if (findGeneralInformationByPtcpntIdsResponse.VEISfgenpidreturnInfo != null)
                {

                    var generalInfo = findGeneralInformationByPtcpntIdsResponse.VEISfgenpidreturnInfo;

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
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, method + " Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Map EC data to LOB";
                response.ExceptionOccured = true;
            }

            return progressString;
        }

    }
}