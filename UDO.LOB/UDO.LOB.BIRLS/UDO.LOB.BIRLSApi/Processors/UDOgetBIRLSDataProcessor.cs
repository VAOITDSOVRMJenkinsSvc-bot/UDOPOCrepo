/// <summary>
/// UDOgetBIRLSDataProcessor
/// </summary>

namespace UDO.LOB.BIRLSApi.Processors
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Tooling.Connector;
    using System;
    using UDO.LOB.BIRLS.Messages;
    using UDO.LOB.Core;
    using UDO.LOB.Extensions;
    using UDO.LOB.Extensions.Logging;
    using VEIS.Core.Messages;
    using VEIS.Messages.VeteranWebService;

    class UDOgetBIRLSDataProcessor
    {
        private CrmServiceClient OrgServiceProxy;

        private bool _debug { get; set; }

        private const string method = "UDOgetBIRLSDataProcessor";

        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOgetBIRLSDataRequest request)
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
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA",
                };
            }
            TraceLogger tLogger = new TraceLogger(method, request);

            UDOgetBIRLSDataResponse response = new UDOgetBIRLSDataResponse { MessageId = request.MessageId };

            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request.LogSoap)
            {
                LogHelper.LogDebug(request.OrganizationName, request.LogSoap, request.UserId, "UDOgetBIRLSDataProcessor", JsonHelper.Serialize<UDOgetBIRLSDataRequest>(request));
            }

            if (request == null)
            {
                response.ExceptionMessage = $"{method} - invoked with no message";
                response.ExceptionOccured = true;
                return response;
            }
            if (request.DiagnosticsContext == null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    IdProof = request.IDProofId,
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : string.Empty
                };
            }
            TraceLogger aiLogger = new TraceLogger("UDOgetBIRLSDataProcessor.Execute", request);

            aiLogger.LogTrace($">> Entered {this.GetType().FullName}.getBIRLSData", "1");
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
            _debug = request.Debug;

            #region connect to CRM
            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
                tLogger.LogEvent($"Authenticated and Connected to CRM", "2");
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException);
                response.ExceptionMessage = $"{method}: Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }

            #endregion
            tLogger.LogEvent("Connected to CRM", "001");
            progressString += " >> After CRM Connection";

            try
            {
                // prefix = brlsFNfindBirlsRecordByFileNumberRequest();
                var findBirlsRecordByFileNumberRequest = new VEISbrlsFNfindBirlsRecordByFileNumberRequest();
                findBirlsRecordByFileNumberRequest.MessageId = request.MessageId;
                findBirlsRecordByFileNumberRequest.LogTiming = request.LogTiming;
                findBirlsRecordByFileNumberRequest.LogSoap = request.LogSoap;
                findBirlsRecordByFileNumberRequest.Debug = request.Debug;
                findBirlsRecordByFileNumberRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                findBirlsRecordByFileNumberRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                findBirlsRecordByFileNumberRequest.RelatedParentId = request.RelatedParentId;
                findBirlsRecordByFileNumberRequest.UserId = request.UserId;
                findBirlsRecordByFileNumberRequest.OrganizationName = request.OrganizationName;

                findBirlsRecordByFileNumberRequest.mcs_filenumber = request.fileNumber;

                findBirlsRecordByFileNumberRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                {
                    ///Header Info
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber,
                };

                // Replaced? SendReceive VIMT with VEIS
                var findBirlsRecordByFileNumberResponse = new VEISbrlsFNfindBirlsRecordByFileNumberResponse();
                findBirlsRecordByFileNumberResponse = WebApiUtility.SendReceive<VEISbrlsFNfindBirlsRecordByFileNumberResponse>(findBirlsRecordByFileNumberRequest, WebApiType.VEIS);
                // var findBirlsRecordByFileNumberResponse = findBirlsRecordByFileNumberRequest.SendReceive<VIMTbrlsFNfindBirlsRecordByFileNumberResponse>(MessageProcessType.Local);
                progressString += " >> After VEIS EC Call";

                if (findBirlsRecordByFileNumberRequest.LogSoap || findBirlsRecordByFileNumberResponse.ExceptionOccurred)
                {
                    if (findBirlsRecordByFileNumberResponse.SerializedSOAPRequest != null || findBirlsRecordByFileNumberResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findBirlsRecordByFileNumberResponse.SerializedSOAPRequest + findBirlsRecordByFileNumberResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(findBirlsRecordByFileNumberRequest.MessageId, findBirlsRecordByFileNumberRequest.OrganizationName, findBirlsRecordByFileNumberRequest.UserId, MethodInfo.GetThisMethod().Method, $"VEISbrlsFNfindBirlsRecordByFileNumberRequest Request/Response {requestResponse}", true);
                    }
                }

                response.ExceptionMessage = findBirlsRecordByFileNumberResponse.ExceptionMessage;
                response.ExceptionOccured = findBirlsRecordByFileNumberResponse.ExceptionOccurred;

                var folderCount = 0;
                var altNamesCount = 0;
                var milHistCount = 0;
                var insCount = 0;
                var svcDiagsCount = 0;
                var flashesCount = 0;
                var disclosuresCount = 0;
                var requestCollection = new OrganizationRequestCollection();
                if (findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo != null)
                {
                    #region main BIRLS Update
                    var responseIds = new UDOgetBIRLSDataMultipleResponse();
                    //instantiate the new Entity
                    Entity thisNewEntity = new Entity();
                    thisNewEntity.LogicalName = "udo_birls";
                    thisNewEntity.Id = request.udo_birlsId;

                    if (request.ownerId != System.Guid.Empty)
                    {
                        thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                    }

                    var namefield = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_FIRST_NAME;
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_MIDDLE_NAME))
                    {
                        namefield += " " + findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_MIDDLE_NAME;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_LAST_NAME))
                    {
                        namefield += " " + findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_LAST_NAME;
                    }
                    thisNewEntity["udo_name"] = namefield;

                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_VIETNAM_SERVICE_IND))
                    {
                        thisNewEntity["udo_vietnamservice"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_VIETNAM_SERVICE_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_VET_IS_BENE_IND))
                    {
                        thisNewEntity["udo_vetisbeneficiary"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_VET_IS_BENE_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_VET_HAS_BENE_IND))
                    {
                        thisNewEntity["udo_vethasbeneficiaries"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_VET_HAS_BENE_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_VERIFIED_SOC_SEC_IND))
                    {
                        thisNewEntity["udo_verifiedssn"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_VERIFIED_SOC_SEC_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_TRANSFER_TO_RESERVES_IND))
                    {
                        thisNewEntity["udo_transfertoreserves"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_TRANSFER_TO_RESERVES_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_SVC_MED_RECORD_IND))
                    {
                        thisNewEntity["udo_svcmedrecord"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_SVC_MED_RECORD_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_SOC_SEC_NUMBER))
                    {
                        thisNewEntity["udo_ssn"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_SOC_SEC_NUMBER;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_SPECIAL_LAW_CODE))
                    {
                        thisNewEntity["udo_speciallawcd"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_SPECIAL_LAW_CODE;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_SPECIAL_ADAPTIVE_HOUSING))
                    {
                        thisNewEntity["udo_specialadaptivehousing"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_SPECIAL_ADAPTIVE_HOUSING;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_SEX_CODE))
                    {
                        thisNewEntity["udo_sex"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_SEX_CODE;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_VERIFIED_SVC_DATA_IND3))
                    {
                        thisNewEntity["udo_service3verified"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_VERIFIED_SVC_DATA_IND3;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_VADS_IND3))
                    {
                        thisNewEntity["udo_service3vads"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_VADS_IND3;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_VERIFIED_SVC_DATA_IND2))
                    {
                        thisNewEntity["udo_service2verfied"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_VERIFIED_SVC_DATA_IND2;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_VADS_IND2))
                    {
                        thisNewEntity["udo_service2vads"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_VADS_IND2;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_VERIFIED_SVC_DATA_IND))
                    {
                        thisNewEntity["udo_service1verified"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_VERIFIED_SVC_DATA_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_VADS_IND))
                    {
                        thisNewEntity["udo_service1vads"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_VADS_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_SEPARATION_PAY))
                    {
                        thisNewEntity["udo_separationpay"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_SEPARATION_PAY;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_REENLISTED_IND))
                    {
                        thisNewEntity["udo_reenlisted"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_REENLISTED_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_REASON_FOR_TERM_DISALLOW))
                    {
                        thisNewEntity["udo_reasonfortermordisallowance"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_REASON_FOR_TERM_DISALLOW;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_PURPLE_HEART_IND))
                    {
                        thisNewEntity["udo_purpleheart"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_PURPLE_HEART_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DATE_OF_DMDC_RETIRE_PAY_P))
                    {
                        thisNewEntity["udo_priorpaydate"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DATE_OF_DMDC_RETIRE_PAY_P;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DMDC_RETIRE_PAY_SBP_AMT_P))
                    {
                        thisNewEntity["udo_priorpayamt"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DMDC_RETIRE_PAY_SBP_AMT_P;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_POW_NUMBER_OF_DAYS))
                    {
                        thisNewEntity["udo_powdays"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_POW_NUMBER_OF_DAYS;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_POW_NUMBER_OF_DAYS))
                    {
                        thisNewEntity["udo_powofdays"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_POW_NUMBER_OF_DAYS;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_POWER_OF_ATTY_CODE2))
                    {
                        thisNewEntity["udo_poacode2"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_POWER_OF_ATTY_CODE2;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_POWER_OF_ATTY_CODE1))
                    {
                        thisNewEntity["udo_poacode1"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_POWER_OF_ATTY_CODE1;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_PERSIAN_GULF_SVC_IND))
                    {
                        thisNewEntity["udo_persongulfservice"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_PERSIAN_GULF_SVC_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_PAYMENT))
                    {
                        thisNewEntity["udo_payment"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_PAYMENT;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_NUM_OF_SVC_CON_DIS))
                    {
                        thisNewEntity["udo_numofsvcconnecteddiags"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_NUM_OF_SVC_CON_DIS;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_MEDAL_OF_HONOR_IND))
                    {
                        thisNewEntity["udo_medalofhonor"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_MEDAL_OF_HONOR_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_LUMP_SUM_READJUSTMENT_PAY))
                    {
                        thisNewEntity["udo_lumpsumreadjustmentpay"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_LUMP_SUM_READJUSTMENT_PAY;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_IN_THEATER_START_DATE))
                    {
                        thisNewEntity["udo_intheaterstart"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_IN_THEATER_START_DATE;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_IN_THEATER_END_DATE))
                    {
                        thisNewEntity["udo_intheaterend"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_IN_THEATER_END_DATE;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_IN_THEATER_DAYS))
                    {
                        thisNewEntity["udo_intheaterdays"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_IN_THEATER_DAYS;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_INS_PREFIX))
                    {
                        thisNewEntity["udo_insuranceprefix"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_INS_PREFIX;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_INS_NUMBER))
                    {
                        thisNewEntity["udo_insurancefilenbr"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_INS_NUMBER;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_INCOMPETENT_IND))
                    {
                        thisNewEntity["udo_incompetent"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_INCOMPETENT_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_HOMELESS_VET_IND))
                    {
                        thisNewEntity["udo_homeless"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_HOMELESS_VET_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_HEADSTONE))
                    {
                        thisNewEntity["udo_headstone"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_HEADSTONE;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_GUARDIANSHIP_CASE_IND))
                    {
                        thisNewEntity["udo_guardianshipcase"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_GUARDIANSHIP_CASE_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_ENTITLEMENT_CODE))
                    {
                        thisNewEntity["udo_entitlementcd"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_ENTITLEMENT_CODE;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_EMPLOYEE_STATION_NUMBER))
                    {
                        thisNewEntity["udo_employeestationnum"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_EMPLOYEE_STATION_NUMBER;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_EMPLOYEE_STATION_NUMBER))
                    {
                        thisNewEntity["udo_employeestation"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_EMPLOYEE_STATION_NUMBER;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_EMPLOYEE_NUMBER))
                    {
                        thisNewEntity["udo_employeenum"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_EMPLOYEE_NUMBER;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DATE_OF_BIRTH))
                    {
                        thisNewEntity["udo_dob"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DATE_OF_BIRTH;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DISABILITY_SEVERANCE_PAY))
                    {
                        thisNewEntity["udo_disabilityseverancepay"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DISABILITY_SEVERANCE_PAY;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DISABILITY_IND))
                    {
                        thisNewEntity["udo_disability"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DISABILITY_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DIAGS_VERIFIED_IND))
                    {
                        thisNewEntity["udo_diagnosisverified"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DIAGS_VERIFIED_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DEATH_IN_SVC))
                    {
                        thisNewEntity["udo_deathinservice"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DEATH_IN_SVC;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DATE_OF_DEATH))
                    {
                        thisNewEntity["udo_dateofdeath"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DATE_OF_DEATH;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DATE_OF_UPDATE))
                    {
                        thisNewEntity["udo_datelastupdated"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DATE_OF_UPDATE;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DATE_OF_DMDC_RETIRE_PAY_C))
                    {
                        thisNewEntity["udo_currentpaydate"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DATE_OF_DMDC_RETIRE_PAY_C;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DMDC_RETIRE_PAY_SBP_AMT_C))
                    {
                        thisNewEntity["udo_currentpayamnt"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DMDC_RETIRE_PAY_SBP_AMT_C;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CONTESTED_DATA_IND))
                    {
                        thisNewEntity["udo_contesteddata"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CONTESTED_DATA_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CP_VET_CP_BENE_IND))
                    {
                        thisNewEntity["udo_comppen"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CP_VET_CP_BENE_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_COMBINED_DEGREE))
                    {
                        thisNewEntity["udo_combineddegree"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_COMBINED_DEGREE;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CLOTHING_ALLOWANCE))
                    {
                        thisNewEntity["udo_clothingallowance"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CLOTHING_ALLOWANCE;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CLAIM_FOLDER_LOCATION))
                    {
                        thisNewEntity["udo_claimfolderloc"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CLAIM_FOLDER_LOCATION;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CLAIM_NUMBER))
                    {
                        thisNewEntity["udo_claim"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CLAIM_NUMBER;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CH34_IND))
                    {
                        thisNewEntity["udo_ch34"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CH34_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CH32_903_IND))
                    {
                        thisNewEntity["udo_ch32903"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CH32_903_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CH32_BEN_IND))
                    {
                        thisNewEntity["udo_ch32bene"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CH32_BEN_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CH32_BANK_IND))
                    {
                        thisNewEntity["udo_ch32bank"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CH32_BANK_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CH31_IND))
                    {
                        thisNewEntity["udo_ch31"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CH31_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CH30_IND))
                    {
                        thisNewEntity["udo_ch30"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CH30_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CH106_IND))
                    {
                        thisNewEntity["udo_ch106"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CH106_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CAUSE_OF_DEATH))
                    {
                        thisNewEntity["udo_causeofdeath"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CAUSE_OF_DEATH;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CP_EFFCTVE_DATE_OF_TERM))
                    {
                        thisNewEntity["udo_cptermdate"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_CP_EFFCTVE_DATE_OF_TERM;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_BURIAL_FLAG_ISSUE_IND))
                    {
                        thisNewEntity["udo_burialflagissued"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_BURIAL_FLAG_ISSUE_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_BURIAL_AWARD_TRANSPORT))
                    {
                        thisNewEntity["udo_burialawardtransport"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_BURIAL_AWARD_TRANSPORT;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_BURIAL_AWD_SVC_CONNECT))
                    {
                        thisNewEntity["udo_burialawardsvcconnected"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_BURIAL_AWD_SVC_CONNECT;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_BURIAL_AWARD_PLOT))
                    {
                        thisNewEntity["udo_burialawardplot"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_BURIAL_AWARD_PLOT;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_BURIAL_AWD_NONSVC_CON))
                    {
                        thisNewEntity["udo_burialawardnonsvcconnected"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_BURIAL_AWD_NONSVC_CON;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_BANKRUPTCY_IND))
                    {
                        thisNewEntity["udo_bankruptcy"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_BANKRUPTCY_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_AUTOMOBILE_ALLOWANCE))
                    {
                        thisNewEntity["udo_automobileallowance"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_AUTOMOBILE_ALLOWANCE;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_APPEALS_IND))
                    {
                        thisNewEntity["udo_appeals"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_APPEALS_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_APPLICATION_FOR_PLOT))
                    {
                        thisNewEntity["udo_appforplot"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_APPLICATION_FOR_PLOT;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_ADD_DIA_IND))
                    {
                        thisNewEntity["udo_additionaldiagnostics"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_ADD_DIA_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_ADAPTIVE_EQUIPMENT))
                    {
                        thisNewEntity["udo_adaptiveequipment"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_ADAPTIVE_EQUIPMENT;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_TOTAL_ACTIVE_SERVICE_YEARS))
                    {
                        thisNewEntity["udo_activesvcyears"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_TOTAL_ACTIVE_SERVICE_YEARS;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_TOTAL_ACTIVE_SERVICE_MONTHS))
                    {
                        thisNewEntity["udo_activesvcmonths"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_TOTAL_ACTIVE_SERVICE_MONTHS;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_TOTAL_ACTIVE_SERVICE_DAYS))
                    {
                        thisNewEntity["udo_activesvcdays"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_TOTAL_ACTIVE_SERVICE_DAYS;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_ACTIVE_DUTY_TRAINING_IND))
                    {
                        thisNewEntity["udo_activedutytraining"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_ACTIVE_DUTY_TRAINING_IND;
                    }
                    if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_IND_901))
                    {
                        thisNewEntity["udo_901"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_IND_901;
                    }

                    if (request.udo_birlsId != Guid.Empty)
                    {
                        OrgServiceProxy.Update(TruncateHelper.TruncateFields(request.MessageId, thisNewEntity, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                        progressString += " >> Updated BIRLS record";
                    }

                    #endregion

                    #region folder data
                    if (findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.VEISbrlsFNFOLDERInfo != null)
                    {
                        var birlsRecordFOLDER = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.VEISbrlsFNFOLDERInfo;
                        foreach (var birlsRecordFOLDERItem in birlsRecordFOLDER)
                        {
                            //instantiate the new Entity
                            thisNewEntity = new Entity();
                            thisNewEntity.LogicalName = "udo_folderlocation";
                            if (request.ownerId != System.Guid.Empty)
                            {
                                thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                            }
                            if (!string.IsNullOrEmpty(birlsRecordFOLDERItem.mcs_FOLDER_TYPE))
                            {
                                thisNewEntity["udo_type"] = birlsRecordFOLDERItem.mcs_FOLDER_TYPE;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordFOLDERItem.mcs_DATE_OF_TRANSFER))
                            {
                                thisNewEntity["udo_transferdate"] = birlsRecordFOLDERItem.mcs_DATE_OF_TRANSFER;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordFOLDERItem.mcs_RELOCATION_INDICATOR))
                            {
                                thisNewEntity["udo_relocation"] = birlsRecordFOLDERItem.mcs_RELOCATION_INDICATOR;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordFOLDERItem.mcs_FOLDER_PRIOR_LOCATION))
                            {
                                thisNewEntity["udo_priorloc"] = birlsRecordFOLDERItem.mcs_FOLDER_PRIOR_LOCATION;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordFOLDERItem.mcs_NO_RECORD_IND))
                            {
                                thisNewEntity["udo_norecord"] = birlsRecordFOLDERItem.mcs_NO_RECORD_IND;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordFOLDERItem.mcs_NO_FOLDER_EST_REASON))
                            {
                                thisNewEntity["udo_nofolderreason"] = birlsRecordFOLDERItem.mcs_NO_FOLDER_EST_REASON;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordFOLDERItem.mcs_LOCATION_NUMBER))
                            {
                                thisNewEntity["udo_locationnbr"] = birlsRecordFOLDERItem.mcs_LOCATION_NUMBER;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordFOLDERItem.mcs_INSURANCE_FOLDER_TYPE))
                            {
                                thisNewEntity["udo_insurancefoldertype"] = birlsRecordFOLDERItem.mcs_INSURANCE_FOLDER_TYPE;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordFOLDERItem.mcs_IN_TRANSIT_TO_STATION))
                            {
                                thisNewEntity["udo_intransitloc"] = birlsRecordFOLDERItem.mcs_IN_TRANSIT_TO_STATION;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordFOLDERItem.mcs_DATE_OF_TRANSIT))
                            {
                                thisNewEntity["udo_intransitdate"] = birlsRecordFOLDERItem.mcs_DATE_OF_TRANSIT;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordFOLDERItem.mcs_DATE_OF_FLDR_RETIRE))
                            {
                                thisNewEntity["udo_folderretiredate"] = birlsRecordFOLDERItem.mcs_DATE_OF_FLDR_RETIRE;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordFOLDERItem.mcs_FARC_ACCESSION_NUM))
                            {
                                thisNewEntity["udo_farcaccessionnbr"] = birlsRecordFOLDERItem.mcs_FARC_ACCESSION_NUM;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordFOLDERItem.mcs_FOLDER_DESTROYED_IND))
                            {
                                thisNewEntity["udo_destroyed"] = birlsRecordFOLDERItem.mcs_FOLDER_DESTROYED_IND;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordFOLDERItem.mcs_FOLDER_CURRENT_LOCATION))
                            {
                                thisNewEntity["udo_currloc"] = birlsRecordFOLDERItem.mcs_FOLDER_CURRENT_LOCATION;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordFOLDERItem.mcs_BOX_SEQUENCE_NUMBER))
                            {
                                thisNewEntity["udo_boxsequencenbr"] = birlsRecordFOLDERItem.mcs_BOX_SEQUENCE_NUMBER;
                            }
                            //not mapped thisNewEntity["udo_rebuilt"]=??
                            if (request.UDOgetBIRLSDataRelatedEntitiesInfo != null)
                            {
                                foreach (var relatedItem in request.UDOgetBIRLSDataRelatedEntitiesInfo)
                                {
                                    thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                }
                            }
                            if ((!string.IsNullOrEmpty(birlsRecordFOLDERItem.mcs_FOLDER_TYPE)) && (!string.IsNullOrWhiteSpace(birlsRecordFOLDERItem.mcs_FOLDER_TYPE)))
                            {
                                CreateRequest createData = new CreateRequest
                                {
                                    Target = thisNewEntity
                                };
                                requestCollection.Add(createData);
                                folderCount += 1;
                            }
                        }
                    }
                    #endregion

                    #region Alternate Names
                    if (findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.VEISbrlsFNALTERNATE_NAMEInfo != null)
                    {
                        var birlsRecordALTERNATE_NAME = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.VEISbrlsFNALTERNATE_NAMEInfo;
                        foreach (var birlsRecordALTERNATE_NAMEItem in birlsRecordALTERNATE_NAME)
                        {
                            //instantiate the new Entity
                            thisNewEntity = new Entity();
                            thisNewEntity.LogicalName = "udo_alternatename";

                            if (!string.IsNullOrEmpty(birlsRecordALTERNATE_NAMEItem.mcs_ALT_NAME_SUFFIX))
                            {
                                thisNewEntity["udo_suffix"] = birlsRecordALTERNATE_NAMEItem.mcs_ALT_NAME_SUFFIX;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordALTERNATE_NAMEItem.mcs_ALT_MIDDLE_NAME))
                            {
                                thisNewEntity["udo_middlename"] = birlsRecordALTERNATE_NAMEItem.mcs_ALT_MIDDLE_NAME;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordALTERNATE_NAMEItem.mcs_ALT_LAST_NAME))
                            {
                                thisNewEntity["udo_lastname"] = birlsRecordALTERNATE_NAMEItem.mcs_ALT_LAST_NAME;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordALTERNATE_NAMEItem.mcs_ALT_FIRST_NAME))
                            {
                                thisNewEntity["udo_firstname"] = birlsRecordALTERNATE_NAMEItem.mcs_ALT_FIRST_NAME;
                            }
                            if (request.ownerId != System.Guid.Empty)
                            {
                                thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                            }
                            if (request.UDOgetBIRLSDataRelatedEntitiesInfo != null)
                            {
                                foreach (var relatedItem in request.UDOgetBIRLSDataRelatedEntitiesInfo)
                                {
                                    thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                }
                            }
                            if ((!string.IsNullOrEmpty(birlsRecordALTERNATE_NAMEItem.mcs_ALT_FIRST_NAME)) && (!string.IsNullOrWhiteSpace(birlsRecordALTERNATE_NAMEItem.mcs_ALT_FIRST_NAME)))
                            {
                                CreateRequest createData = new CreateRequest
                                {
                                    Target = thisNewEntity
                                };
                                requestCollection.Add(createData);
                                altNamesCount += 1;
                            }
                        }

                    }

                    #endregion

                    #region milHistory
                    if (findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.VEISbrlsFNSERVICEInfo != null)
                    {
                        var birlsRecordSERVICE = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.VEISbrlsFNSERVICEInfo;

                        foreach (var birlsRecordSERVICEItem in birlsRecordSERVICE)
                        {

                            //instantiate the new Entity
                            thisNewEntity = new Entity();
                            thisNewEntity.LogicalName = "udo_birlsmilitaryservice";
                            if (!string.IsNullOrEmpty(birlsRecordSERVICEItem.mcs_SERVICE_NUMBER_FILL))
                            {
                                thisNewEntity["udo_servicenumfill"] = birlsRecordSERVICEItem.mcs_SERVICE_NUMBER_FILL;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordSERVICEItem.mcs_SHORT_SERVICE_NUMBER))
                            {
                                thisNewEntity["udo_servicenum"] = birlsRecordSERVICEItem.mcs_SHORT_SERVICE_NUMBER;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordSERVICEItem.mcs_SEPARATION_REASON_CODE))
                            {
                                thisNewEntity["udo_sepreason"] = birlsRecordSERVICEItem.mcs_SEPARATION_REASON_CODE;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordSERVICEItem.mcs_RELEASED_ACTIVE_DUTY_DATE))
                            {
                                thisNewEntity["udo_raddate"] = dateStringFormat(birlsRecordSERVICEItem.mcs_RELEASED_ACTIVE_DUTY_DATE);
                            }
                            if (!string.IsNullOrEmpty(birlsRecordSERVICEItem.mcs_PAY_GRADE))
                            {
                                thisNewEntity["udo_paygrade"] = birlsRecordSERVICEItem.mcs_PAY_GRADE;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordSERVICEItem.mcs_NONPAY_DAYS))
                            {
                                thisNewEntity["udo_nonpaydays"] = birlsRecordSERVICEItem.mcs_NONPAY_DAYS;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordSERVICEItem.mcs_ENTERED_ON_DUTY_DATE))
                            {
                                thisNewEntity["udo_eoddate"] = dateStringFormat(birlsRecordSERVICEItem.mcs_ENTERED_ON_DUTY_DATE);
                            }
                            if (!string.IsNullOrEmpty(birlsRecordSERVICEItem.mcs_CHAR_OF_SVC_CODE))
                            {
                                thisNewEntity["udo_charservice"] = birlsRecordSERVICEItem.mcs_CHAR_OF_SVC_CODE;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordSERVICEItem.mcs_BRANCH_OF_SERVICE))
                            {
                                thisNewEntity["udo_branch"] = LongBranchOfService(birlsRecordSERVICEItem.mcs_BRANCH_OF_SERVICE);
                            }
                            if (request.ownerId != System.Guid.Empty)
                            {
                                thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                            }
                            if (request.UDOgetBIRLSDataRelatedEntitiesInfo != null)
                            {
                                foreach (var relatedItem in request.UDOgetBIRLSDataRelatedEntitiesInfo)
                                {
                                    thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                }
                            }
                            if ((!string.IsNullOrEmpty(birlsRecordSERVICEItem.mcs_BRANCH_OF_SERVICE)) && (!string.IsNullOrWhiteSpace(birlsRecordSERVICEItem.mcs_BRANCH_OF_SERVICE)))
                            {

                                CreateRequest createData = new CreateRequest
                                {
                                    Target = thisNewEntity
                                };
                                requestCollection.Add(createData);
                                milHistCount += 1;
                            }
                        }

                    }

                    #endregion

                    #region insurance Info
                    if (findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.VEISbrlsFNINSURANCE_POLICYInfo != null)
                    {
                        var birlsRecordINSURANCE_POLICY = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.VEISbrlsFNINSURANCE_POLICYInfo;
                        foreach (var birlsRecordINSURANCE_POLICYItem in birlsRecordINSURANCE_POLICY)
                        {
                            //instantiate the new Entity
                            thisNewEntity = new Entity();
                            thisNewEntity.LogicalName = "udo_insurance";
                            if (!string.IsNullOrEmpty(birlsRecordINSURANCE_POLICYItem.mcs_INS_POL_PREFIX))
                            {
                                thisNewEntity["udo_policyprefix"] = birlsRecordINSURANCE_POLICYItem.mcs_INS_POL_PREFIX;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordINSURANCE_POLICYItem.mcs_INS_POL_NUMBER))
                            {
                                thisNewEntity["udo_policynumber"] = birlsRecordINSURANCE_POLICYItem.mcs_INS_POL_NUMBER;
                            }
                            if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_INS_PREFIX))
                            {
                                thisNewEntity["udo_insuranceprefix"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_INS_PREFIX;
                            }
                            if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_INS_NUMBER))
                            {
                                thisNewEntity["udo_insurancenumber"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_INS_NUMBER;
                            }
                            if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DATE_OF_INS_LAPSED_PURGE))
                            {
                                thisNewEntity["udo_insurancelapsepurgedate"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DATE_OF_INS_LAPSED_PURGE;
                            }
                            if (!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_INSURANCE_JURIS))
                            {
                                thisNewEntity["udo_insurancejurisdiction"] = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_INSURANCE_JURIS;
                            }
                            if (request.ownerId != System.Guid.Empty)
                            {
                                thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                            }
                            if (request.UDOgetBIRLSDataRelatedEntitiesInfo != null)
                            {
                                foreach (var relatedItem in request.UDOgetBIRLSDataRelatedEntitiesInfo)
                                {
                                    thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                }
                            }
                            if (((insCount == 0) && ((!string.IsNullOrEmpty(birlsRecordINSURANCE_POLICYItem.mcs_INS_POL_PREFIX)) && (!string.IsNullOrWhiteSpace(birlsRecordINSURANCE_POLICYItem.mcs_INS_POL_PREFIX)))
                            || ((!string.IsNullOrEmpty(birlsRecordINSURANCE_POLICYItem.mcs_INS_POL_NUMBER)) && (!string.IsNullOrWhiteSpace(birlsRecordINSURANCE_POLICYItem.mcs_INS_POL_NUMBER))) ||
                            ((!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_INS_PREFIX)) && (!string.IsNullOrWhiteSpace(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_INS_PREFIX))) ||
                            ((!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_INS_NUMBER)) && (!string.IsNullOrWhiteSpace(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_INS_NUMBER))) ||
                            ((!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DATE_OF_INS_LAPSED_PURGE) && (!string.IsNullOrWhiteSpace(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_DATE_OF_INS_LAPSED_PURGE))) ||
                            ((!string.IsNullOrEmpty(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_INSURANCE_JURIS)) && (!string.IsNullOrWhiteSpace(findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.mcs_INSURANCE_JURIS)))
                                || ((insCount > 0) &&
                            ((!string.IsNullOrEmpty(birlsRecordINSURANCE_POLICYItem.mcs_INS_POL_PREFIX)) && (!string.IsNullOrWhiteSpace(birlsRecordINSURANCE_POLICYItem.mcs_INS_POL_PREFIX))) ||
                            ((!string.IsNullOrEmpty(birlsRecordINSURANCE_POLICYItem.mcs_INS_POL_NUMBER)) && (!string.IsNullOrWhiteSpace(birlsRecordINSURANCE_POLICYItem.mcs_INS_POL_NUMBER)))))))
                            {
                                CreateRequest createData = new CreateRequest
                                {
                                    Target = thisNewEntity
                                };
                                requestCollection.Add(createData);
                                insCount += 1;
                            }
                        }
                    }
                    #endregion

                    #region service Diagnostics
                    if (findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.VEISbrlsFNSERVICEDIAGNOSTICSInfo != null)
                    {
                        var birlsRecordSERVICEDIAGNOSTICS = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.VEISbrlsFNSERVICEDIAGNOSTICSInfo;
                        foreach (var birlsRecordSERVICEDIAGNOSTICSItem in birlsRecordSERVICEDIAGNOSTICS)
                        {
                            //instantiate the new Entity
                            thisNewEntity = new Entity();
                            thisNewEntity.LogicalName = "udo_servicediagnostics";
                            if (!string.IsNullOrEmpty(birlsRecordSERVICEDIAGNOSTICSItem.mcs_RECUR_SVC_CON_DISABILITY))
                            {
                                thisNewEntity["udo_serviceconnecteddisability"] = birlsRecordSERVICEDIAGNOSTICSItem.mcs_RECUR_SVC_CON_DISABILITY;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordSERVICEDIAGNOSTICSItem.mcs_SERVICE_PERCENT2))
                            {
                                thisNewEntity["udo_percentage2"] = birlsRecordSERVICEDIAGNOSTICSItem.mcs_SERVICE_PERCENT2;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordSERVICEDIAGNOSTICSItem.mcs_SERVICE_PERCENT1))
                            {
                                thisNewEntity["udo_percentage"] = birlsRecordSERVICEDIAGNOSTICSItem.mcs_SERVICE_PERCENT1;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordSERVICEDIAGNOSTICSItem.mcs_SERVICE_DIAGNOSTICS))
                            {
                                thisNewEntity["udo_diagnosticcode"] = birlsRecordSERVICEDIAGNOSTICSItem.mcs_SERVICE_DIAGNOSTICS;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordSERVICEDIAGNOSTICSItem.mcs_RECUR_ANALOGUS_CODE))
                            {
                                thisNewEntity["udo_analogouscode"] = birlsRecordSERVICEDIAGNOSTICSItem.mcs_RECUR_ANALOGUS_CODE;
                            }
                            if (request.ownerId != System.Guid.Empty)
                            {
                                thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                            }
                            if (request.UDOgetBIRLSDataRelatedEntitiesInfo != null)
                            {
                                foreach (var relatedItem in request.UDOgetBIRLSDataRelatedEntitiesInfo)
                                {
                                    thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                }
                            }
                            if ((!string.IsNullOrEmpty(birlsRecordSERVICEDIAGNOSTICSItem.mcs_SERVICE_DIAGNOSTICS)) && (!string.IsNullOrWhiteSpace(birlsRecordSERVICEDIAGNOSTICSItem.mcs_SERVICE_DIAGNOSTICS)))
                            {
                                CreateRequest createData = new CreateRequest
                                {
                                    Target = thisNewEntity
                                };
                                requestCollection.Add(createData);
                                svcDiagsCount += 1;
                            }
                        }
                    }
                    #endregion

                    #region birls flashes
                    if (findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.VEISbrlsFNFLASHInfo != null)
                    {
                        var birlsRecordFLASH = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.VEISbrlsFNFLASHInfo;
                        foreach (var birlsRecordFLASHItem in birlsRecordFLASH)
                        {
                            //instantiate the new Entity
                            thisNewEntity = new Entity();
                            thisNewEntity.LogicalName = "udo_birlsflash";
                            if (!string.IsNullOrEmpty(birlsRecordFLASHItem.mcs_FLASH_STATION))
                            {
                                thisNewEntity["udo_flashstation"] = birlsRecordFLASHItem.mcs_FLASH_STATION;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordFLASHItem.mcs_FLASH_ROUTING_SYMBOL))
                            {
                                thisNewEntity["udo_flashroutingsymbol"] = birlsRecordFLASHItem.mcs_FLASH_ROUTING_SYMBOL;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordFLASHItem.mcs_FLASH_CODE))
                            {
                                thisNewEntity["udo_flashcode"] = birlsRecordFLASHItem.mcs_FLASH_CODE;
                            }
                            if (request.ownerId != System.Guid.Empty)
                            {
                                thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                            }
                            if (request.UDOgetBIRLSDataRelatedEntitiesInfo != null)
                            {
                                foreach (var relatedItem in request.UDOgetBIRLSDataRelatedEntitiesInfo)
                                {
                                    thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                }
                            }
                            if ((!string.IsNullOrEmpty(birlsRecordFLASHItem.mcs_FLASH_STATION)) && (!string.IsNullOrWhiteSpace(birlsRecordFLASHItem.mcs_FLASH_STATION)))
                            {
                                CreateRequest createData = new CreateRequest
                                {
                                    Target = thisNewEntity
                                };
                                requestCollection.Add(createData);
                                flashesCount += 1;
                            }
                        }
                    }
                    #endregion

                    #region disclosures
                    if (findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.VEISbrlsFNRECURING_DISCLOSUREInfo != null)
                    {
                        var birlsRecordRECURING_DISCLOSURE = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo.VEISbrlsFNRECURING_DISCLOSUREInfo;
                        foreach (var birlsRecordRECURING_DISCLOSUREItem in birlsRecordRECURING_DISCLOSURE)
                        {
                            //instantiate the new Entity
                            thisNewEntity = new Entity();
                            thisNewEntity.LogicalName = "udo_disclosure";
                            if (!string.IsNullOrEmpty(birlsRecordRECURING_DISCLOSUREItem.mcs_RECUR_DISCLOSURE_YEAR))
                            {
                                thisNewEntity["udo_disclosureyear"] = birlsRecordRECURING_DISCLOSUREItem.mcs_RECUR_DISCLOSURE_YEAR;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordRECURING_DISCLOSUREItem.mcs_RECUR_DISCLOSURE_MONTH))
                            {
                                thisNewEntity["udo_disclosuremonth"] = birlsRecordRECURING_DISCLOSUREItem.mcs_RECUR_DISCLOSURE_MONTH;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordRECURING_DISCLOSUREItem.mcs_DATE_OF_DISCLOSURE))
                            {
                                thisNewEntity["udo_dateofdisclosure"] = birlsRecordRECURING_DISCLOSUREItem.mcs_DATE_OF_DISCLOSURE;
                            }
                            if (!string.IsNullOrEmpty(birlsRecordRECURING_DISCLOSUREItem.mcs_RECUR_DISCLOSURE_NUM))
                            {
                                thisNewEntity["udo_accountnumber"] = birlsRecordRECURING_DISCLOSUREItem.mcs_RECUR_DISCLOSURE_NUM;
                            }

                            if (request.ownerId != System.Guid.Empty)
                            {
                                thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                            }
                            if (request.UDOgetBIRLSDataRelatedEntitiesInfo != null)
                            {
                                foreach (var relatedItem in request.UDOgetBIRLSDataRelatedEntitiesInfo)
                                {
                                    thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                }
                            }
                            if ((!string.IsNullOrEmpty(birlsRecordRECURING_DISCLOSUREItem.mcs_RECUR_DISCLOSURE_NUM)) && (!string.IsNullOrWhiteSpace(birlsRecordRECURING_DISCLOSUREItem.mcs_RECUR_DISCLOSURE_NUM)))
                            {
                                CreateRequest createData = new CreateRequest
                                {
                                    Target = thisNewEntity
                                };
                                requestCollection.Add(createData);
                                disclosuresCount += 1;
                            }


                        }
                    }
                    #endregion
                }

                #region Create records

                if (requestCollection.Count > 0)
                {
                    var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, Guid.Empty, request.Debug);
                    progressString += " >> Completed ExecuteMultiple with associated data";
                    if (_debug)
                    {
                        LogBuffer += result.LogDetail;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, LogBuffer, _debug);
                    }

                    if (result.IsFaulted)
                    {
                        LogHelper.LogError(request.OrganizationName, request.UserId, method, result.ErrorDetail);
                        response.ExceptionMessage = result.FriendlyDetail;
                        response.ExceptionOccured = true;
                        return response;
                    }
                }

                string logInfo = string.Format("Folders Added:{0}; Alt Names Added:{1}; MilHist Added:{2}; Insurance Added:{3}; Svc Diags Added:{4}; Flashes Added:{5}; Disclosures Added:{6}",
                    folderCount, altNamesCount, milHistCount, insCount, svcDiagsCount, flashesCount, disclosuresCount);
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, logInfo, request.Debug);

                #endregion


                //added to generated code
                if (request.IDProofId != System.Guid.Empty)
                {
                    var parent = new Entity
                    {
                        Id = request.IDProofId,
                        LogicalName = "udo_idproof"
                    };
                    parent["udo_birlscomplete"] = true;
                    //OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                    OrgServiceProxy.Update(parent);
                    progressString += " >> Updated ID Proof record >> END";
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method, ExecutionException);
                response.ExceptionMessage = "Failed to Process BIRLS Data - " + ExecutionException.Message + System.Environment.NewLine + ExecutionException.StackTrace;
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

        private string dateStringFormat(string date)
        {
            //02101999
            //if (date == null) return null;
            if (string.IsNullOrEmpty(date) || string.IsNullOrWhiteSpace(date)) return null;
            if (date.Length == 10) return date;
            if (date.Length < 8) return date;



            date = date.Insert(2, "/");
            //02/101999
            date = date.Insert(5, "/");
            //02/10/1999
            return date;
        }

        private string phoneNumberFormat(string phonenumber)
        {
            //7034056868
            var returnNumber = "";

            if (phonenumber == null) return null;
            if (phonenumber.Length == 7)
            {
                returnNumber = phonenumber.Substring(0, 3) + "-" + phonenumber.Substring(4);
            }

            if (phonenumber.Length == 10)
            {
                returnNumber = "(" + phonenumber.Substring(0, 3) + ") " + phonenumber.Substring(3, 3) + "-" + phonenumber.Substring(6);
            }

            if (phonenumber.Length > 10)
            {
                returnNumber = "(" + phonenumber.Substring(0, 3) + ") " + phonenumber.Substring(3, 3) + "-" + phonenumber.Substring(6, 4) + " " + phonenumber.Substring(9);
            }
            return returnNumber;
        }
        private string LongBranchOfService(string branchcode)
        {
            //JS switch (branchcode.trim())
            switch (branchcode.Trim())
            {
                case "AF": return "AIR FORCE (AF)";
                case "A": return "ARMY (ARMY)";
                //ARMY AIR CORPS
                case "CG": return "COAST GUARD (CG)";
                case "CA": return "COMMONWEALTH ARMY (CA)";
                case "GCS": return "GUERRILLA AND COMBINATION SVC (GCS)";
                case "M": return "MARINES (M)";
                case "MM": return "MERCHANT MARINES (MM)";
                case "NOAA": return "NATIONAL OCEANIC & ATMOSPHERIC ADMINISTRATION (NOAA)";
                //NAVY (NAVY)
                case "PHS": return "PUBLIC HEALTH SVC (PHS)";
                case "RSS": return "REGULAR PHILIPPINE SCOUT (RSS)";
                //REGULAR PHILIPPINE SCOUT COMBINED WITH SPECIAL
                case "RPS": return "PHILIPPINE SCOUT OR COMMONWEALTH ARMY SVC (RPS)";
                case "SPS": return "SPECIAL PHILIPPINE SCOUTS (SPS)";
                case "WAC": return "WOMEN'S ARMY CORPS (WAC)";
            }
            return branchcode;
        }
    }
}