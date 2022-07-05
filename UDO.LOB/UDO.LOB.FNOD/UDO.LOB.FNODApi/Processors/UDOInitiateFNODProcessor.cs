using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.Pfe.Xrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using System.Threading.Tasks;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Configuration;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.FNOD.Messages;
using VEIS.Core.Messages;
using VEIS.Messages.AddressWebService;
using VEIS.Messages.AppealService;
using VEIS.Messages.BenefitClaimService;
using VEIS.Messages.ClaimantService;
using VEIS.Messages.RatingService;
using VEIS.Messages.VeteranWebService;
using System.Text;
using Microsoft.Xrm.Tooling.Connector;

//using Microsoft.Xrm.Sdk.Messages;
//using System.Runtime.Serialization;
//using VIMT.AddressWebService.Messages;
//using VIMT.BenefitClaimService.Messages;
//using VIMT.ClaimantWebService.Messages;
//using VIMT.RatingWebService.Messages;
//using VIMT.VeteranWebService.Messages;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Logging.CRM.Util;
//using VRM.Integration.UDO.FNOD.Messages;
//using VRM.Integration.UDO.Common;

namespace UDO.LOB.FNOD.Processors
{
    internal class UDOInitiateFNODProcessor
    {
        private TimeTracker timer { get; set; }
        // OrganizationServiceProxy OrgServiceProxy;

        private CrmServiceClient OrgServiceProxy;
        private bool _debug { get; set; }

        private const string VEISBaseUrlAppSettingsKeyName = "VEISBaseUrl";

        private string LogBuffer { get; set; }

        private Uri veisBaseUri;

        private LogSettings logSettings { get; set; }

        private const string method = "UDOInitiateFNODProcessor";

        public UDOInitiateFNODProcessor()
        {
            timer = new TimeTracker();
        }

        public IMessageBase Execute(UDOInitiateFNODRequest request)
        {
            #region Start Timer
            timer.Restart();
            #endregion

            string startInit = timer.MarkStart("Initialize Process");
            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Starting execution . . .");

            var response = new UDOInitiateFNODResponse();

            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            //REM: Init Processor to set the VEIS Config
            InitProcessor(request);

            if (request == null)
            {
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Request is null.");

                response.UDOInitiateFNODExceptions = new UDOInitiateFNODException
                {
                    ExceptionMessage = "Called with no message",
                    ExceptionOccurred = true
                };
                return response;
            }

            LogHelper.LogInfo(string.Format("Message Id:{0}, Type={2}, Recieved diagnostics message: {1}",
                request.MessageId,
                request.MessageId,
                GetType().FullName));

            if (request.DiagnosticsContext == null && request != null)
            {
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Setting request.DiagnosticsContext.");

                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = method,
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
                };
            }
            TraceLogger tLogger = new TraceLogger(method, request);

            LogHelper.LogInfo($">> {this.GetType().FullName}.Execute: Request:  {JsonHelper.Serialize<UDOInitiateFNODRequest>(request)}");

            #region connect to CRM
            try
            {
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Connecting to CRM.");

                OrgServiceProxy = ConnectionCache.GetProxy();
                tLogger.LogEvent($"Authenticated and Connected to CRM", "2");
            }
            catch (Exception connectException)
            {
                var method = MethodInfo.GetThisMethod().ToString(false);
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException);
                response.UDOInitiateFNODExceptions = new UDOInitiateFNODException
                {
                    ExceptionMessage = "Failed to get CRMConnection",
                    ExceptionOccurred = true
                };
                return response;
            }
            #endregion

            progressString = "After Connection";

            timer.MarkStop("Initialize Process");

            string ptcpntBeneId = null, ptcpntVetId = null, ptpcntRecipId = null;

            //var serviceProxy = OrgServiceProxy as OrganizationServiceProxy;
            //OrganizationWebProxyClient webProxyClient = CrmConnection.Connect<OrganizationWebProxyClient>();
            //if (webProxyClient != null)
            //{
            //    webProxyClient.CallerId = request.UserId;
            //}

            //OrgServiceProxy = webProxyClient as IOrganizationService;

            //OrganizationServiceProxy serviceProxy = (OrganizationServiceProxy)OrgServiceProxy;

            //OrganizationServiceProxy serviceProxy = new OrganizationServiceProxy()
            try
            {
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Instantiating new FNOD entity record.");

                //instantiate the new Entity
                Entity new_va_fnod_entity = new Entity("va_fnod");

                // Because FNOD pulls from the parent record, which is the contact record
                // the methods below follow the contact record's sources

                //TODO: These should use participantId instead of fileNumber whereever possible.

                #region If pid is null then findGeneralInformationByFileNumberRequest (generalInfoByFileNum)
                // This is to take the filenum and get the other ptcpnt id values.
                // Only do this if pid is empty.
                string startGenInfoByFileNum = timer.MarkStart("generalInfoByFileNum");
                // Replaced:  VIMTfgenFNfindGeneralInformationByFileNumberRequest = VEISfgenFNfindGeneralInformationByFileNumberRequest
                var findGeneralInformationByFileNumberRequest = new VEISfgenFNfindGeneralInformationByFileNumberRequest
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
                    //non standard fields
                    mcs_filenumber = request.vetfileNumber
                };

                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Sending VEISfgenFNfindGeneralInformationByFileNumberRequest.");

                // REM: Invoke Veis Endpoint
                // findGeneralInformationByFileNumberRequest.SendReceive<VIMTfgenFNfindGeneralInformationByFileNumberResponse>(MessageProcessType.Local);
                var findGeneralInformationByFileNumberResponse = WebApiUtility.SendReceive<VEISfgenFNfindGeneralInformationByFileNumberResponse>(findGeneralInformationByFileNumberRequest, WebApiType.VEIS);

                if (request.LogSoap || findGeneralInformationByFileNumberResponse.ExceptionOccurred)
                {
                    if (findGeneralInformationByFileNumberResponse.SerializedSOAPRequest != null || findGeneralInformationByFileNumberResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findGeneralInformationByFileNumberResponse.SerializedSOAPRequest + findGeneralInformationByFileNumberResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfgenFNfindGeneralInformationByFileNumberRequest Request/Response {requestResponse}", true);
                    }
                }

                // Replaced: VIMTfgenFNreturnclmsInfo = VEISfgenFNreturnInfo
                var generalInfoByFileNum = findGeneralInformationByFileNumberResponse.VEISfgenFNreturnInfo;
                timer.MarkStop("generalInfoByFileNum");
                long pid = 0;
                if (request.ptcpntId > 0) pid = request.ptcpntId;
                if (generalInfoByFileNum != null)
                {
                    ptcpntBeneId = generalInfoByFileNum.mcs_ptcpntBeneID;
                    ptcpntVetId = generalInfoByFileNum.mcs_ptcpntVetID;
                    ptpcntRecipId = generalInfoByFileNum.mcs_ptcpntRecipID;


                    if (long.TryParse(generalInfoByFileNum.mcs_ptcpntVetID, out pid))
                    {
                        request.ptcpntId = pid;
                    }
                }

                if (ptcpntVetId == null && request.ptcpntId > 0) ptcpntVetId = request.ptcpntId.ToString();
                if (ptcpntBeneId == null) ptcpntBeneId = ptcpntVetId;
                if (ptpcntRecipId == null) ptpcntRecipId = ptcpntVetId;

                progressString = "After VEIS EC Call - findGeneralInformationByFileNumber";
                #endregion

                #region findCorporateRecordByFileNumber (corpRecord)
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: findCorporateRecordByFileNumber.");

                string startFindCorpRecord = timer.MarkStart("findCorporateRecordByFileNumber");
                // Replaced: VIMTcrpFNfindCorporateRecordByFileNumberRequest = VIMTcrpFNfindCorporateRecordByFileNumberRequest
                var findCorporateRecordByFileNumberRequest = new VEIScrpFNfindCorporateRecordByFileNumberRequest
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
                    //non standard fields
                    mcs_filenumber = request.vetfileNumber,
                };

                // REM: Invoke VEIS Endpoint
                // var findCorporateRecordByFileNumberResponse = findCorporateRecordByFileNumberRequest.SendReceive<VIMTcrpFNfindCorporateRecordByFileNumberResponse> (MessageProcessType.Local);
                var findCorporateRecordByFileNumberResponse = WebApiUtility.SendReceive<VEIScrpFNfindCorporateRecordByFileNumberResponse>(findCorporateRecordByFileNumberRequest, WebApiType.VEIS);

                if (request.LogSoap || findCorporateRecordByFileNumberResponse.ExceptionOccurred)
                {
                    if (findCorporateRecordByFileNumberResponse.SerializedSOAPRequest != null || findCorporateRecordByFileNumberResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findCorporateRecordByFileNumberResponse.SerializedSOAPRequest + findCorporateRecordByFileNumberResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEIScrpFNfindCorporateRecordByFileNumberRequest Request/Response {requestResponse}", true);
                    }
                }

                progressString = "After VEIS EC Call - findCorporateRecord";
                timer.MarkStop("findCorporateRecordByFileNumber");
                // Replaced: VIMTcrpFNreturn = VEIScrpFNreturn
                VEIScrpFNreturn corpRecord = findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo;

                // If there is no response, then return the response
                if (findCorporateRecordByFileNumberResponse.VEIScrpFNreturnInfo == null)
                {
                    response.UDOInitiateFNODExceptions = new UDOInitiateFNODException
                    {
                        ExceptionMessage = findCorporateRecordByFileNumberResponse.ExceptionMessage,
                        ExceptionOccurred = findCorporateRecordByFileNumberResponse.ExceptionOccurred
                    };
                    return response;
                }

                #endregion

                #region findBirlsRecordByFileNumber (birlsRecord)
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: findBirlsRecordByFileNumber.");

                string startFindBirls = timer.MarkStart("findBirlsRecordByFileNumber");
                // Replaced: VIMTbrlsFNfindBirlsRecordByFileNumberRequest = 
                var findBirlsRecordByFileNumberRequest = new VEISbrlsFNfindBirlsRecordByFileNumberRequest
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
                    mcs_filenumber = request.vetfileNumber
                };

                // REM: Invoke VEIS Endpoint
                // var findBirlsRecordByFileNumberResponse = findBirlsRecordByFileNumberRequest.SendReceive<VIMTbrlsFNfindBirlsRecordByFileNumberResponse>(MessageProcessType.Local);
                var findBirlsRecordByFileNumberResponse = WebApiUtility.SendReceive<VEISbrlsFNfindBirlsRecordByFileNumberResponse>(findBirlsRecordByFileNumberRequest, WebApiType.VEIS);
                progressString = "After VEIS EC Call - findBirlsRecord";

                if (request.LogSoap || findBirlsRecordByFileNumberResponse.ExceptionOccurred)
                {
                    if (findBirlsRecordByFileNumberResponse.SerializedSOAPRequest != null || findBirlsRecordByFileNumberResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findBirlsRecordByFileNumberResponse.SerializedSOAPRequest + findBirlsRecordByFileNumberResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISbrlsFNfindBirlsRecordByFileNumberRequest Request/Response {requestResponse}", true);
                    }
                }

                var birlsRecord = findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo;
                if (findBirlsRecordByFileNumberResponse.VEISbrlsFNreturnInfo == null)
                {
                    response.UDOInitiateFNODExceptions = new UDOInitiateFNODException
                    {
                        ExceptionMessage = findBirlsRecordByFileNumberResponse.ExceptionMessage,
                        ExceptionOccurred = findBirlsRecordByFileNumberResponse.ExceptionOccurred
                    };
                    return response;
                }
                timer.MarkStop("findBirlsRecordByFileNumber");
                #endregion

                #region findGeneralInformationByPtcpntIds (generalInfo)
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: findGeneralInformationByPtcpntIds.");

                // Replaced: VIMTfgenpidreturnclms = VEISfgenpidreturn
                VEISfgenpidreturn generalInfo = null;
                if (!String.IsNullOrEmpty(ptcpntVetId) || (request.ptcpntId > 0))
                {
                    string startFindGeneralInfoByPtcpntId = timer.MarkStart("findGeneralInformationByPtcpntIds");

                    // Replaced: VIMTfgenpidfindGeneralInformationByPtcpntIdsRequest = VEISfgenpidfindGeneralInformationByPtcpntIdRequest
                    var findGeneralInformationByPtcpntIdsRequest = new VEISfgenpidfindGeneralInformationByPtcpntIdsRequest
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
                        //non standard fields
                        // TODO: VEIS missing attributes
                        mcs_ptcpntvetid = String.IsNullOrEmpty(ptcpntVetId) ? request.ptcpntId.ToString() : ptcpntVetId,
                        mcs_ptcpntbeneid = ptcpntBeneId,

                    };

                    // REM: Invoke VEIS Endpoint
                    // var findGeneralInformationByPtcpntIdsResponse = findGeneralInformationByPtcpntIdsRequest.SendReceive<VIMTfgenpidfindGeneralInformationByPtcpntIdResponse>(MessageProcessType.Local);
                    var findGeneralInformationByPtcpntIdsResponse = WebApiUtility.SendReceive<VEISfgenpidfindGeneralInformationByPtcpntIdsResponse>(findGeneralInformationByPtcpntIdsRequest, WebApiType.VEIS);
                    // Replaced: VIMTfgenpidreturnclmsInfo = VEISfgenpidreturnInfo
                    generalInfo = findGeneralInformationByPtcpntIdsResponse.VEISfgenpidreturnInfo;
                    progressString = "After VEIS EC Call - findGeneralInformationByPtcpntIds";

                    if (request.LogSoap || findGeneralInformationByPtcpntIdsResponse.ExceptionOccurred)
                    {
                        if (findGeneralInformationByPtcpntIdsResponse.SerializedSOAPRequest != null || findGeneralInformationByPtcpntIdsResponse.SerializedSOAPResponse != null)
                        {
                            var requestResponse = findGeneralInformationByPtcpntIdsResponse.SerializedSOAPRequest + findGeneralInformationByPtcpntIdsResponse.SerializedSOAPResponse;
                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfgenpidfindGeneralInformationByPtcpntIdsRequest Request/Response {requestResponse}", true);
                        }
                    }

                    if (generalInfo == null)
                    {
                        response.UDOInitiateFNODExceptions = new UDOInitiateFNODException
                        {
                            ExceptionMessage = findGeneralInformationByPtcpntIdsResponse.ExceptionMessage,
                            ExceptionOccurred = findGeneralInformationByPtcpntIdsResponse.ExceptionOccurred
                        };
                        return response;
                    }

                    timer.MarkStop("findGeneralInformationByPtcpntIds");

                    ptcpntBeneId = generalInfo.mcs_ptcpntBeneID;
                    ptcpntVetId = generalInfo.mcs_ptcpntVetID;
                    ptpcntRecipId = generalInfo.mcs_ptcpntRecipID;
                }
                else
                {
                    // No pid...
                }
                #endregion

                #region Set Entity Field Response Values


                #region udo_idproofid
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: udo_idproofid.");

                if (request.udo_idproofId == Guid.Empty)
                {
                    throw new InvalidPluginExecutionException("customIDProof not valid.  Please IDProof.");
                }
                else
                {
                    new_va_fnod_entity["udo_idproof"] = new EntityReference("udo_idproof", request.udo_idproofId);
                }
                #endregion

                #region va_veterancontactid
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: va_veterancontactid.");

                if (request.udo_veteranId == Guid.Empty)
                {
                    throw new InvalidPluginExecutionException("customVeteran not valid.  Please use a valid veteran record.");
                }
                else
                {
                    new_va_fnod_entity["va_veterancontactid"] = new EntityReference("contact", request.udo_veteranId);
                }
                #endregion

                #region va_name [const]
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: va_name.");

                new_va_fnod_entity["va_name"] = "FNOD/MOD/PMC Request";
                #endregion

                #region udo_personid
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: udo_personid.");

                new_va_fnod_entity["udo_deceasedperson"] = new EntityReference("udo_person", request.udo_personId);
                #endregion

                #region va_filenumber [request]
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: va_filenumber.");

                new_va_fnod_entity["va_filenumber"] = request.vetfileNumber;

                #endregion

                #region va_firstname, va_newpmcvetfirstname [corpRecord, birlsRecord] and va_birlsfirstname [birlsRecord]
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: va_firstname, va_newpmcvetfirstname and va_birlsfirstname.");

                // VEIS Dependency missing attributes
                if (corpRecord != null && !string.IsNullOrEmpty(corpRecord.mcs_firstName))
                {
                    new_va_fnod_entity["va_firstname"] = corpRecord.mcs_firstName;
                    new_va_fnod_entity["va_newpmcvetfirstname"] = corpRecord.mcs_firstName;
                }
                else if (birlsRecord != null && !string.IsNullOrEmpty(birlsRecord.mcs_FIRST_NAME))
                {
                    new_va_fnod_entity["va_firstname"] = birlsRecord.mcs_FIRST_NAME;
                    new_va_fnod_entity["va_newpmcvetfirstname"] = birlsRecord.mcs_FIRST_NAME;
                }

                if (birlsRecord != null)
                {
                    new_va_fnod_entity["va_birlsfirstname"] = birlsRecord.mcs_FIRST_NAME;
                }
                #endregion

                #region newpmcvetmiddleinitial [coprRecord, birlsRecord]
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: newpmcvetmiddleinitial.");

                if (corpRecord != null && !string.IsNullOrEmpty(corpRecord.mcs_middleName))
                {
                    new_va_fnod_entity["va_newpmcvetmiddleinitial"] = corpRecord.mcs_middleName;
                }
                else if (birlsRecord != null && !string.IsNullOrEmpty(birlsRecord.mcs_MIDDLE_NAME))
                {
                    new_va_fnod_entity["va_newpmcvetmiddleinitial"] = birlsRecord.mcs_MIDDLE_NAME;
                }
                #endregion

                #region va_lastname, va_newpmcvetlastname [corpRecord, birlsRecord] and va_birlslastname [birlsRecord]
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: va_lastname, va_newpmcvetlastname and va_birlslastname.");

                if (corpRecord != null && !string.IsNullOrEmpty(corpRecord.mcs_lastName))
                {
                    new_va_fnod_entity["va_lastname"] = corpRecord.mcs_lastName;
                    new_va_fnod_entity["va_newpmcvetlastname"] = corpRecord.mcs_lastName;
                }
                else if (birlsRecord != null && !string.IsNullOrEmpty(birlsRecord.mcs_LAST_NAME))
                {
                    new_va_fnod_entity["va_lastname"] = birlsRecord.mcs_LAST_NAME;
                    new_va_fnod_entity["va_newpmcvetlastname"] = birlsRecord.mcs_LAST_NAME;
                }
                if (birlsRecord != null)
                {
                    new_va_fnod_entity["va_birlslastname"] = birlsRecord.mcs_LAST_NAME;
                }

                #endregion

                #region va_newpmcvetsuffix [corpRecord, birlsRecord]
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: va_newpmcvetsuffix.");

                if (corpRecord != null && !string.IsNullOrEmpty(corpRecord.mcs_suffixName))
                {
                    new_va_fnod_entity["va_newpmcvetsuffix"] = corpRecord.mcs_suffixName;
                }
                else if (birlsRecord != null && !string.IsNullOrEmpty(birlsRecord.mcs_NAME_SUFFIX))
                {
                    new_va_fnod_entity["va_newpmcvetsuffix"] = birlsRecord.mcs_NAME_SUFFIX;
                }

                #endregion

                #region va_dateofbirth [corpRecord, birlsRecord]
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: va_dateofbirth.");

                if (corpRecord != null || birlsRecord != null)
                {
                    string dateOfBirth = (corpRecord == null || String.IsNullOrEmpty(corpRecord.mcs_dateOfBirth)) ? birlsRecord.mcs_DATE_OF_BIRTH : corpRecord.mcs_dateOfBirth;
                    if (!string.IsNullOrEmpty(dateOfBirth))
                    {
                        new_va_fnod_entity["va_dateofbirthtext"] = dateOfBirth;  // If we need to store the date as text, shouldn't we also store the dateofdeath as text?
                        DateTime va_dateofbirth;
                        if (DateTime.TryParse(dateOfBirth, out va_dateofbirth))
                        {
                            new_va_fnod_entity["va_dateofbirth"] = va_dateofbirth.ToCRMDateTime();
                        }
                    }
                }
                #endregion

                #region va_dateofdeath [birlsRecord, generalinfo]
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: va_dateofdeath.");

                string dod = (birlsRecord != null) ? birlsRecord.mcs_DATE_OF_DEATH : string.Empty;
                if (String.IsNullOrEmpty(dod) && generalInfo != null) dod = generalInfo.mcs_vetDeathDate;
                if (!string.IsNullOrEmpty(dod))
                {
                    DateTime va_dateofdeath;
                    if (DateTime.TryParse(dod, out va_dateofdeath))
                    {
                        new_va_fnod_entity["va_dateofdeath"] = va_dateofdeath.ToCRMDateTime();
                    }
                }


                #endregion

                #region va_sex [generalInfo, birlsRecord]
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: va_sex.");

                var gender = "";
                if (generalInfo == null || string.IsNullOrEmpty(generalInfo.mcs_vetSex))
                {
                    gender = birlsRecord.mcs_SEX_CODE;
                }
                else
                {
                    if (generalInfo != null)
                        gender = generalInfo.mcs_vetSex;
                }

                switch (gender)
                {
                    case "M":
                        new_va_fnod_entity["va_sex"] = new OptionSetValue(953850000);
                        break;
                    case "F":
                        new_va_fnod_entity["va_sex"] = new OptionSetValue(953850001);
                        break;
                    default:
                        new_va_fnod_entity["va_sex"] = new OptionSetValue(953850002);
                        break;
                }

                #endregion

                #region va_soj [generalInfo, birlsRecord]
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: va_soj.");

                if (generalInfo != null && !string.IsNullOrEmpty(generalInfo.mcs_stationOfJurisdiction))
                {
                    new_va_fnod_entity["va_soj"] = generalInfo.mcs_stationOfJurisdiction;
                }
                else if (birlsRecord != null && !string.IsNullOrEmpty(birlsRecord.mcs_CLAIM_FOLDER_LOCATION))
                {
                    new_va_fnod_entity["va_soj"] = birlsRecord.mcs_CLAIM_FOLDER_LOCATION;
                }

                #endregion

                #region va_poa [generalInfo]
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: va_poa.");

                if (generalInfo != null && !string.IsNullOrEmpty(generalInfo.mcs_powerOfAttorney))
                {
                    new_va_fnod_entity["va_poa"] = generalInfo.mcs_powerOfAttorney;
                }
                else
                {
                    var poa_exception = new UDOInitiateFNODException();
                    //new_va_fnod_entity["va_poa"] = getPOA(OrgServiceProxy, request, out poa_exception);
                    //new_va_fnod_entity["va_poa"] = getPOA(serviceProxy, request, out poa_exception);
                    new_va_fnod_entity["va_poa"] = getPOA(OrgServiceProxy, request, out poa_exception);
                    if (poa_exception.ExceptionOccurred)
                    {
                        //todo: add exception to response
                    }

                }



                #endregion

                #region va_birlsfileno [birlsRecord]
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: va_birlsfileno.");

                if (birlsRecord != null && !string.IsNullOrEmpty(birlsRecord.mcs_CLAIM_NUMBER))
                {
                    new_va_fnod_entity["va_birlsfileno"] = birlsRecord.mcs_CLAIM_NUMBER;
                }

                #endregion

                #region va_folderlocation [birlsRecord]
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: va_folderlocation.");

                if (birlsRecord != null && birlsRecord.VEISbrlsFNFOLDERInfo != null)
                {
                    var claimFolder = birlsRecord.VEISbrlsFNFOLDERInfo.Where(
                        folder =>
                            string.Equals(folder.mcs_FOLDER_TYPE, "CLAIM", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (claimFolder != null)
                    {
                        new_va_fnod_entity["va_folderlocation"] = claimFolder.mcs_FOLDER_CURRENT_LOCATION;
                    }
                }
                #endregion

                #region va_birlsmultiperiodservice [birlsRecord]
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: va_birlsmultiperiodservice.");

                if (birlsRecord != null)
                {
                    new_va_fnod_entity["va_birlsmultiperiodservice"] = (birlsRecord.VEISbrlsFNSERVICEInfo.Length > 1);
                }
                #endregion

                #region va_birlbos and va_bilscharacterofservice [birlsRecord]
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: va_birlbos and va_bilscharacterofservice.");

                // This gets the most recent service.
                // This should be able to replace the JScript reference that called va_findmilitaryrecordbyptcpntidresponse
                if (birlsRecord != null && birlsRecord.VEISbrlsFNSERVICEInfo != null)
                {
                    var latestServiceInfo = birlsRecord.VEISbrlsFNSERVICEInfo.OrderByDescending(x =>
                    {
                        DateTime d;
                        DateTime.TryParse(x.mcs_RELEASED_ACTIVE_DUTY_DATE, out d);
                        return d;
                    }).FirstOrDefault();

                    if (latestServiceInfo != null)
                    {
                        if (!string.IsNullOrEmpty(latestServiceInfo.mcs_BRANCH_OF_SERVICE))
                        {
                            new_va_fnod_entity["va_birlsbos"] = latestServiceInfo.mcs_BRANCH_OF_SERVICE;
                        }
                        if (!string.IsNullOrEmpty(latestServiceInfo.mcs_CHAR_OF_SVC_CODE))
                        {
                            new_va_fnod_entity["va_birlscharacterofservice"] = latestServiceInfo.mcs_CHAR_OF_SVC_CODE;
                        }
                    }
                }
                #endregion

                #region va_birlsservice1verified [birlsRecord]
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: va_birlsservice1verified.");

                if (birlsRecord != null && !string.IsNullOrEmpty(birlsRecord.mcs_VERIFIED_SVC_DATA_IND))
                {
                    new_va_fnod_entity["va_birlsservice1verified"] = birlsRecord.mcs_VERIFIED_SVC_DATA_IND;
                }

                #endregion

                #region va_awardcode [generalInfoByFileNum]
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: va_awardcode.");

                if (generalInfoByFileNum != null && !string.IsNullOrEmpty(generalInfoByFileNum.mcs_awardTypeCode))
                {
                    new_va_fnod_entity["va_awardcode"] = generalInfoByFileNum.mcs_awardTypeCode;
                }
                else
                {
                    var awards = generalInfoByFileNum.VEISfgenFNawardBenesInfo;
                    if (awards != null && awards.Length > 1)
                    {
                        foreach (var award in awards)
                        {
                            if (!award.mcs_awardTypeCd.Trim().Equals("CPL", StringComparison.InvariantCultureIgnoreCase) ||
                                !award.mcs_payeeCd.Trim().Equals("00"))
                            {
                                continue;
                            }
                            new_va_fnod_entity["va_awardcode"] = award.mcs_awardTypeCd;
                            break;
                        }
                    }
                    else if (awards != null && awards.Length == 1)
                    {
                        new_va_fnod_entity["va_awardcode"] = awards[0].mcs_awardTypeCd;
                    }
                }
                #endregion

                #region va_awardstatus [generalInfoByFileNum]
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: va_awardstatus.");

                if (generalInfoByFileNum != null && !string.IsNullOrEmpty(generalInfoByFileNum.mcs_payStatusTypeName))
                {
                    new_va_fnod_entity["va_awardatatus"] = generalInfoByFileNum.mcs_payStatusTypeName;
                }

                #endregion

                #region findDependents (va_listofdependents)
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: findDependents.");

                string startFindDependents = timer.MarkStart("findDependents");
                var findDependentsRequest = new VEISfedpfindDependentsRequest
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
                    mcs_filenumber = request.vetfileNumber
                };

                // REM: Invoke VEIS Endpoint
                // var findDependentsResponse = findDependentsRequest.SendReceive<VIMTfedpfindDependentsResponse>(MessageProcessType.Local);
                var findDependentsResponse = WebApiUtility.SendReceive<VEISfedpfindDependentsResponse>(findDependentsRequest, WebApiType.VEIS);
                progressString = "After VEIS EC Call - findDependents";

                if (request.LogSoap || findDependentsResponse.ExceptionOccurred)
                {
                    if (findDependentsResponse.SerializedSOAPRequest != null || findDependentsResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findDependentsResponse.SerializedSOAPRequest + findDependentsResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfedpfindDependentsRequest Request/Response {requestResponse}", true);
                    }
                }

                response.UDOInitiateFNODExceptions = new UDOInitiateFNODException
                {
                    ExceptionMessage = findDependentsResponse.ExceptionMessage,
                    ExceptionOccurred = findDependentsResponse.ExceptionOccurred
                };
                timer.MarkStop("findDependents");

                #endregion

                #region va_listofdependents [findDependents]
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: va_listofdependents.");

                if (findDependentsResponse.VEISfedpreturnInfo != null && findDependentsResponse.VEISfedpreturnInfo.VEISfedppersonsInfo != null)
                {
                    var dependents = SecurityTools.ConvertToSecureString(string.Empty);
                    foreach (var dependent in findDependentsResponse.VEISfedpreturnInfo.VEISfedppersonsInfo)
                    {
                        dependents = SecurityTools.Append(dependents, string.Format("Name: {0} {1}     Relationship: {2}     DOB: {3}",
                            dependent.mcs_firstName, dependent.mcs_lastName,
                            dependent.mcs_relationship, dependent.mcs_dateOfBirth));
                        if (!string.IsNullOrEmpty(dependent.mcs_dateOfDeath))
                        {
                            dependents = SecurityTools.Append(dependents, string.Format("     DOD: {0}", dependent.mcs_dateOfDeath));
                        }
                        if (!string.IsNullOrEmpty(dependent.mcs_ssn))
                        {
                            dependents = SecurityTools.Append(dependents, string.Format("     SSN: {0}", dependent.mcs_ssn));
                        }
                        //// DEFECT 305719 asks to remove the Has Awards block.
                        //// If spouse, alive and has a SSN, display his/her Awards
                        //if (!string.IsNullOrEmpty(dependent.mcs_ssn) && string.IsNullOrEmpty(dependent.mcs_dateOfDeath) &&
                        //    string.Equals(dependent.mcs_relationship, "Spouse",
                        //        StringComparison.InvariantCultureIgnoreCase))
                        //{
                        //    var findSpouseRequest = new VIMTfgenFNfindGeneralInformationByFileNumberRequest
                        //    {
                        //        LogTiming = request.LogTiming,
                        //        LogSoap = request.LogSoap,
                        //        Debug = request.Debug,
                        //        RelatedParentEntityName = request.RelatedParentEntityName,
                        //        RelatedParentFieldName = request.RelatedParentFieldName,
                        //        RelatedParentId = request.RelatedParentId,
                        //        UserId = request.UserId,
                        //        OrganizationName = request.OrganizationName,
                        //        mcs_filenumber = dependent.mcs_ssn
                        //    };

                        //    var findSpouseResponse =
                        //        findSpouseRequest.SendReceive<VIMTfgenFNfindGeneralInformationByFileNumberResponse>(
                        //            MessageProcessType.Local);
                        //    var spouseInfo = findSpouseResponse.VIMTfgenFNreturnclmsInfo;
                        //    progressString = "After VIMT EC Call - findGeneralInformation (Spouse)";
                        //    response.UDOInitiateFNODExceptions = new UDOInitiateFNODException{
                        //        ExceptionMessage = findSpouseResponse.ExceptionMessage,
                        //        ExceptionOccured = findSpouseResponse.ExceptionOccured
                        //    };

                        //    if (spouseInfo != null)
                        //    {
                        //        var numberOfAwardBenes = 0;
                        //        if (!string.IsNullOrEmpty(spouseInfo.mcs_numberOfAwardBenes))
                        //        {
                        //            int.TryParse(spouseInfo.mcs_numberOfAwardBenes, out numberOfAwardBenes);
                        //        }
                        //        dependents += string.Format("     Has Awards: {0}",
                        //            (numberOfAwardBenes > 0) ? "Yes" : "No");
                        //    }
                        //}

                        dependents = SecurityTools.Append(dependents, "\r\n");
                    }
                    //dependents += "***************************************************************************************************\r\n";

                    new_va_fnod_entity["va_listofdependents"] = SecurityTools.ConvertToUnsecureString(dependents);
                }
                timer.MarkStop("Aggregating Dependents");

                #endregion

                #endregion

                #region Set PMC Caller Info
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: Set PMC Caller Info.");

                var interactionFetch = "<fetch count='1'>" +
                    "<entity name='udo_interaction'>" +
                    "<attribute name='udo_firstname'/>" +
                    "<attribute name='udo_lastname'/>" +
                    "<attribute name='udo_relationship'/>" +
                    "<link-entity name='udo_idproof' to='udo_interactionid' from='udo_interaction'>" +
                    "<link-entity name='contact' to='udo_veteran' from='contactid' alias='contact'>" +
                    "<attribute name='ownerid'/>" +
                    "</link-entity>" +
                    "<attribute name='udo_idproofid'/>" +
                    "<filter type='and'>" +
                    "<condition attribute='udo_idproofid' operator='eq' value='" + request.udo_idproofId + "'/>" +
                    "</filter></link-entity></entity></fetch>";
                // TODO: Replace CallerId
                //OrgServiceProxy.CallerId = request.UserId;

                //webProxyClient.CallerId = request.UserId;
                //var interactionResult = OrgServiceProxy.RetrieveMultiple(new FetchExpression(interactionFetch));
                var interactionResult = OrgServiceProxy.RetrieveMultiple(new FetchExpression(interactionFetch));

                if (interactionResult != null && interactionResult.Entities.Count() > 0)
                {
                    var interaction = interactionResult.Entities[0];
                    string caller = string.Empty;
                    if (interaction.Contains("udo_firstname")) caller = interaction["udo_firstname"] + " ";
                    if (interaction.Contains("udo_lastname")) caller += interaction["udo_lastname"];

                    new_va_fnod_entity["va_newpmcrecipname"] = caller;
                    if (interaction.Contains("udo_relationship"))
                    {
                        new_va_fnod_entity["va_newpmcreciprelationshiptovet"] = interaction.FormattedValues["udo_relationship"].ToString();
                    }

                    var aliasValue = interaction.GetAttributeValue<AliasedValue>("contact.ownerid");
                    if (aliasValue != null)
                    {
                        var owner = aliasValue.Value as EntityReference;
                        if (owner != null)
                        {
                            new_va_fnod_entity["ownerid"] = owner;
                        }
                    }
                }
                #endregion

                #region findPresidentialMemorialCertificate (pmcInfo)
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: findPresidentialMemorialCertificate.");

                try
                {
                    progressString = "findPresidentialMemorialCertificate (pmcInfo)";
                    string startFindPresidentialMemorialCert = timer.MarkStart("findPresidentialMemorialCertificate");
                    // prefix = findPresidentialMemorialCertificateRequest();
                    var findPresidentialMemorialCertificateRequest = new VEISfindPresidentialMemorialCertificateRequest
                    {
                        LogTiming = request.LogTiming,
                        LogSoap = request.LogSoap,
                        Debug = request.Debug,
                        RelatedParentEntityName = request.RelatedParentEntityName,
                        RelatedParentFieldName = request.RelatedParentFieldName,
                        RelatedParentId = request.RelatedParentId,
                        UserId = request.UserId,
                        OrganizationName = request.OrganizationName,
                        mcs_filenumber = request.vetfileNumber
                    };

                    if (request.LegacyServiceHeaderInfo != null)
                    {
                        findPresidentialMemorialCertificateRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                        {
                            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                        };
                    }

                    // REM: Invoke VEIS Endpoint
                    // var findPresidentialMemorialCertificateResponse = findPresidentialMemorialCertificateRequest.SendReceive<VEISfindPresidentialMemorialCertificateResponse>(MessageProcessType.Local);
                    var findPresidentialMemorialCertificateResponse = WebApiUtility.SendReceive<VEISfindPresidentialMemorialCertificateResponse>(findPresidentialMemorialCertificateRequest, WebApiType.VEIS);
                    progressString = "After VEIS EC Call - findPresidentialMemorial";

                    if (request.LogSoap || findPresidentialMemorialCertificateResponse.ExceptionOccurred)
                    {
                        if (findPresidentialMemorialCertificateResponse.SerializedSOAPRequest != null || findPresidentialMemorialCertificateResponse.SerializedSOAPResponse != null)
                        {
                            var requestResponse = findPresidentialMemorialCertificateResponse.SerializedSOAPRequest + findPresidentialMemorialCertificateResponse.SerializedSOAPResponse;
                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfindPresidentialMemorialCertificateRequest Request/Response {requestResponse}", true);
                        }
                    }

                    // Replaced: VIMTFindPresidentialMemorialCertificateResponseReturnbclm = VEISreturn
                    VEISreturn pmcInfo = null;

                    if (findPresidentialMemorialCertificateResponse != null &&
                        // Replaced: VIMTFindPresidentialMemorialCertificateResponseReturnbclmInfo
                        findPresidentialMemorialCertificateResponse.VEISreturnInfo != null)
                    {
                        pmcInfo =
                        findPresidentialMemorialCertificateResponse.VEISreturnInfo;
                    }

                    response.UDOInitiateFNODExceptions = new UDOInitiateFNODException
                    {
                        ExceptionMessage = findPresidentialMemorialCertificateResponse.ExceptionMessage,
                        ExceptionOccurred = findPresidentialMemorialCertificateResponse.ExceptionOccurred
                    };
                    timer.MarkStop("findPresidentialMemorialCertificate");

                    #region pmc fields [pmcInfo]

                    if (pmcInfo != null)
                    {
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_veteranFirstName))
                        {
                            new_va_fnod_entity["va_existingpmcvetfirstname"] = pmcInfo.mcs_veteranFirstName;
                        }
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_veteranLastName))
                        {
                            new_va_fnod_entity["va_existingpmcvetlastname"] = pmcInfo.mcs_veteranLastName;
                        }
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_veteranMiddleInitial))
                        {
                            new_va_fnod_entity["va_existingpmcvetmiddleinitial"] = pmcInfo.mcs_veteranMiddleInitial;
                        }
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_veteranSuffixName))
                        {
                            new_va_fnod_entity["va_existingpmcvetsuffix"] = pmcInfo.mcs_veteranSuffixName;
                        }
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_station))
                        {
                            new_va_fnod_entity["va_existingpmcvetstation"] = pmcInfo.mcs_station;
                        }
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_salutation))
                        {
                            new_va_fnod_entity["va_existingpmcrecipsalutation"] = pmcInfo.mcs_salutation;
                        }
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_state))
                        {
                            new_va_fnod_entity["va_existingpmcrecipstate"] = pmcInfo.mcs_state;
                        }
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_zipCode))
                        {
                            new_va_fnod_entity["va_existingpmcrecipzip"] = pmcInfo.mcs_zipCode;
                        }
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_realtionshipToVeteran))
                        {
                            new_va_fnod_entity["va_existingpmcreciprelationshiptovet"] = pmcInfo.mcs_realtionshipToVeteran;
                        }
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_city))
                        {
                            new_va_fnod_entity["va_existingpmcrecipcity"] = pmcInfo.mcs_city;
                        }
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_title))
                        {
                            new_va_fnod_entity["va_existingpmcrecipname"] = pmcInfo.mcs_title;
                        }
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_addressLine1))
                        {
                            new_va_fnod_entity["va_existingpmcrecipaddress1"] = pmcInfo.mcs_addressLine1;
                        }
                        if (!string.IsNullOrEmpty(pmcInfo.mcs_addressLine2))
                        {
                            new_va_fnod_entity["va_existingpmcrecipaddress2"] = pmcInfo.mcs_addressLine2;
                        }
                        // PMC Station
                        var pmc_station = (pmcInfo.mcs_station ?? string.Empty).Trim();

                        var vaFolder = string.Empty;

                        if (new_va_fnod_entity.Contains("va_folderlocation"))
                        {
                            vaFolder = (string)new_va_fnod_entity["va_folderlocation"];
                        }

                        if (!String.IsNullOrEmpty(pmc_station) && !pmc_station.Equals("0"))
                        {
                            new_va_fnod_entity["va_existingpmcvetstation"] = pmc_station;
                        }
                        else if (String.IsNullOrEmpty(vaFolder))
                        {
                            new_va_fnod_entity["va_existingpmcvetstation"] = "10";
                        }
                        else
                        {
                            new_va_fnod_entity["va_existingpmcvetstation"] = vaFolder.Trim();
                        }
                        new_va_fnod_entity["va_newpmcvetstation"] = new_va_fnod_entity["va_existingpmcvetstation"];
                    }

                    #endregion
                }
                catch (Exception ExecutionException)
                {
                    //LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ExecutionException);
                    var method = MethodInfo.GetThisMethod().ToString(false);
                    LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, ExecutionException);
                    response.UDOInitiateFNODExceptions = new UDOInitiateFNODException
                    {
                        ExceptionMessage = "Failed to get CRMConnection",
                        ExceptionOccurred = true
                    };
                    //TODO: UNCOMMENT RETURN - THIS BYPASSES ANY FAILS FOR PMC.
                    return response;
                }
                #endregion

                #region findAllPtcpntAddrsByPtcpntIdRequest (addresses)
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: findAllPtcpntAddrsByPtcpntIdRequest.");

                VEISfallpidaddpidfreturnMultipleResponse[] addresses = null;
                if (pid > 0)
                {
                    string startFindAllPtcpntAddrs = timer.MarkStart("findAllPtcpntAddrsByPtcpntIdRequest");
                    var findAllPtcpntAddrsByPtcpntIdRequest = new VEISfallpidaddpidfindAllPtcpntAddrsByPtcpntIdRequest
                    {
                        LogTiming = request.LogTiming,
                        LogSoap = request.LogSoap,
                        Debug = request.Debug,
                        RelatedParentEntityName = request.RelatedParentEntityName,
                        RelatedParentFieldName = request.RelatedParentFieldName,
                        RelatedParentId = request.RelatedParentId,
                        UserId = request.UserId,
                        OrganizationName = request.OrganizationName,
                        mcs_ptcpntid = pid
                    };

                    if (request.LegacyServiceHeaderInfo != null)
                    {
                        findAllPtcpntAddrsByPtcpntIdRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                        {
                            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                        };
                    }

                    // REM: Invoke VEIS Endpoint
                    // var findAllPtcpntAddrsByPtcpntIdResponse = findAllPtcpntAddrsByPtcpntIdRequest.SendReceive<VIMTfallpidaddpidfindAllPtcpntAddrsByPtcpntIdResponse>(MessageProcessType.Local);
                    var findAllPtcpntAddrsByPtcpntIdResponse = WebApiUtility.SendReceive<VEISfallpidaddpidfindAllPtcpntAddrsByPtcpntIdResponse>(findAllPtcpntAddrsByPtcpntIdRequest, WebApiType.VEIS);
                    progressString = "After VEIS EC Call - findAllPtcpntAddrsByPtcpntIdRequest";

                    if (request.LogSoap || findAllPtcpntAddrsByPtcpntIdResponse.ExceptionOccurred)
                    {
                        if (findAllPtcpntAddrsByPtcpntIdResponse.SerializedSOAPRequest != null || findAllPtcpntAddrsByPtcpntIdResponse.SerializedSOAPResponse != null)
                        {
                            var requestResponse = findAllPtcpntAddrsByPtcpntIdResponse.SerializedSOAPRequest + findAllPtcpntAddrsByPtcpntIdResponse.SerializedSOAPResponse;
                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfallpidaddpidfindAllPtcpntAddrsByPtcpntIdRequest Request/Response {requestResponse}", true);
                        }
                    }

                    response.UDOInitiateFNODExceptions = new UDOInitiateFNODException
                    {
                        ExceptionMessage = findAllPtcpntAddrsByPtcpntIdResponse.ExceptionMessage,
                        ExceptionOccurred = findAllPtcpntAddrsByPtcpntIdResponse.ExceptionOccurred
                    };

                    // Replaced: VIMTfallpidaddpidreturnInfo = VEISfallpidaddpidfreturnInfo
                    addresses = findAllPtcpntAddrsByPtcpntIdResponse.VEISfallpidaddpidfreturnInfo;

                    timer.MarkStop("findAllPtcpntAddrsByPtcpntIdRequest");

                }
                #endregion

                #region va_lastknownaddress [addresses]
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: va_lastknownaddress.");

                if (addresses != null && addresses.Length > 0)
                {
                    bool updatePMC = !new_va_fnod_entity.Contains("va_newpmcrecipaddress1") ||
                        String.IsNullOrEmpty((string)new_va_fnod_entity["va_newpmcrecipaddress1"]);

                    var address = addresses.Where(a =>
                                  String.Equals(a.mcs_ptcpntAddrsTypeNm, "Mailing", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (address != null)
                    {
                        var addressBlock = address.mcs_addrsOneTxt;
                        new_va_fnod_entity["udo_lastaddress1"] = address.mcs_addrsOneTxt;
                        if (updatePMC)
                        {
                            new_va_fnod_entity["va_newpmcrecipaddress1"] = address.mcs_addrsOneTxt;
                        }
                        new_va_fnod_entity["udo_lastaddresstype"] = new OptionSetValue(953850000);
                        if (!string.IsNullOrWhiteSpace(address.mcs_addrsTwoTxt))
                        {
                            addressBlock += "\r\n" + address.mcs_addrsTwoTxt;
                            new_va_fnod_entity["udo_lastaddress2"] = address.mcs_addrsTwoTxt;
                            if (updatePMC)
                            {
                                new_va_fnod_entity["va_newpmcrecipaddress2"] = address.mcs_addrsTwoTxt;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(address.mcs_addrsThreeTxt))
                        {
                            new_va_fnod_entity["udo_lastaddress3"] = address.mcs_addrsThreeTxt;
                            addressBlock += "\r\n" + address.mcs_addrsThreeTxt;
                        }
                        if (!string.IsNullOrWhiteSpace(address.mcs_cityNm))
                        {
                            addressBlock += "\r\n" + address.mcs_cityNm;
                            new_va_fnod_entity["udo_lastcity"] = address.mcs_cityNm;
                            if (updatePMC)
                            {
                                new_va_fnod_entity["va_newpmcrecipcity"] = address.mcs_cityNm;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(address.mcs_postalCd))
                        {
                            addressBlock += ", " + address.mcs_postalCd;
                            new_va_fnod_entity["udo_laststate"] = address.mcs_postalCd;
                            if (updatePMC)
                            {
                                new_va_fnod_entity["va_newpmcrecipstate"] = address.mcs_postalCd;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(address.mcs_frgnPostalCd))
                        {
                            new_va_fnod_entity["udo_lastforeignpostalcode"] = address.mcs_frgnPostalCd;
                            new_va_fnod_entity["udo_lastaddresstype"] = new OptionSetValue(953850001);
                            if (updatePMC)
                            {
                                new_va_fnod_entity["va_newpmcrecipzip"] = new_va_fnod_entity["udo_lastforeignpostalcode"];
                            }
                        }
                        // UDO REM: Below section already commented out. 
                        /* 
            xrm.Page.getAttribute('va_spousestatelist').setValue(address.state);
            xrm.Page.getAttribute('va_spousezipcode').setValue(address.zip);
            xrm.Page.getAttribute('va_spousecountry').setValue(address.country);
            xrm.Page.getAttribute('va_spouseforeignpostalcode').setValue(address.forgeinPostalCode);
            xrm.Page.getAttribute('va_spouseoverseasmilitarypostalcode').setValue(address.mltyPostalType);
            xrm.Page.getAttribute('va_spouseoverseasmilitarypostofficetypecode').setValue(address.mltyPostOfficeType);
            xrm.Page.getAttribute('va_spouseaddresstype').setValue(address.spouseAddressType);
                         * */
                        if (!string.IsNullOrWhiteSpace(address.mcs_zipPrefixNbr))
                        {
                            addressBlock += " " + address.mcs_zipPrefixNbr;
                            new_va_fnod_entity["udo_lastzipcode"] = address.mcs_zipPrefixNbr;
                            if (updatePMC)
                            {
                                new_va_fnod_entity["va_newpmcrecipzip"] = address.mcs_zipPrefixNbr;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(address.mcs_cntryNm))
                        {
                            addressBlock += "\r\n" + address.mcs_cntryNm;
                            new_va_fnod_entity["udo_lastcountry"] = address.mcs_cntryNm;
                        }
                        if (!string.IsNullOrWhiteSpace(address.mcs_mltyPostOfficeTypeCd))
                        {
                            new_va_fnod_entity["udo_lastaddresstype"] = new OptionSetValue(953850002);
                            addressBlock += "\r\n" + address.mcs_mltyPostOfficeTypeCd;

                            OptionSetValue mltyPostOfficeType = null;
                            switch (address.mcs_mltyPostalTypeCd.ToUpper())
                            {
                                case "APO": mltyPostOfficeType = new OptionSetValue(953850000); break;
                                case "DPO": mltyPostOfficeType = new OptionSetValue(953850001); break;
                                case "FPO": mltyPostOfficeType = new OptionSetValue(953850002); break;
                            }
                            new_va_fnod_entity["udo_lastoverseasmilitarypostofficetypeco"] = mltyPostOfficeType;
                            if (updatePMC)
                            {
                                new_va_fnod_entity["va_newpmcrecipcity"] = address.mcs_mltyPostOfficeTypeCd.ToUpper();
                            }

                        }
                        if (!string.IsNullOrWhiteSpace(address.mcs_mltyPostalTypeCd))
                        {
                            addressBlock += " " + address.mcs_mltyPostalTypeCd;
                            OptionSetValue mltyPostalType = null;
                            switch (address.mcs_mltyPostalTypeCd.ToUpper())
                            {
                                case "AA": mltyPostalType = new OptionSetValue(953850000); break;
                                case "AE": mltyPostalType = new OptionSetValue(953850001); break;
                                case "AP": mltyPostalType = new OptionSetValue(953850002); break;
                            }
                            new_va_fnod_entity["udo_lastoverseasmilitarypostalcode"] = mltyPostalType;
                            if (updatePMC)
                            {
                                new_va_fnod_entity["va_newpmcrecipstate"] = address.mcs_mltyPostalTypeCd.ToUpper();
                            }
                        }

                        var lastAddressType = 0;
                        if (!String.IsNullOrWhiteSpace(address.mcs_mltyPostalTypeCd) ||
                            !String.IsNullOrWhiteSpace(address.mcs_mltyPostOfficeTypeCd))
                        {
                            // Military
                            lastAddressType = 953850002;
                        }
                        else
                        {
                            if (address.mcs_cntryNm != "USA" || String.IsNullOrWhiteSpace(address.mcs_cntryNm) ||
                                address.mcs_cntryNm == "US")
                            {
                                // Domestic
                                lastAddressType = 953850000;
                            }
                            else
                            {
                                // International
                                if (updatePMC)
                                {
                                    new_va_fnod_entity["va_newpmcrecipstate"] =
                                    (new_va_fnod_entity["va_newpmcrecipstate"] + " " + address.mcs_cntryNm).Trim();
                                }
                                lastAddressType = 953850001;
                            }
                        }
                        if (lastAddressType != 0)
                        {
                            new_va_fnod_entity["udo_lastoverseasmilitarypostalcode"] = new OptionSetValue(lastAddressType);
                        }

                        new_va_fnod_entity["va_lastknownaddress"] = addressBlock;
                    }
                }
                #endregion
                timer.MarkStop("Processing Addresses");

                #region findRatingDatatRequest (ratingInfo)
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: findRatingDatatRequest.");

                // Replaced: VIMTfnrtngdtdisabilityRatingRecord = VEISVIMTfnrtngdtdisabilityRatingRecordInfo
                VEISVIMTfnrtngdtdisabilityRatingRecord ratingInfo = null;
                string startFindRatingData = timer.MarkStart("findRatingDatatRequest");
                // prefix = fnrtngdtfindRatingDataRequest();
                var findRatingDataRequest = new VEISfnrtngdtfindRatingDataRequest
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
                    mcs_filenumber = request.vetfileNumber
                };

                // REM: Invoke VEIS Endpoint
                // Replaced: var findRatingDataResponse = findRatingDataRequest.SendReceive<VIMTfnrtngdtfindRatingDataResponse>(MessageProcessType.Local);
                var findRatingDataResponse = WebApiUtility.SendReceive<VEISfnrtngdtfindRatingDataResponse>(findRatingDataRequest, WebApiType.VEIS);
                progressString = "After VEIS EC Call";

                if (request.LogSoap || findRatingDataResponse.ExceptionOccurred)
                {
                    if (findRatingDataResponse.SerializedSOAPRequest != null || findRatingDataResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findRatingDataResponse.SerializedSOAPRequest + findRatingDataResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfnrtngdtfindRatingDataRequest Request/Response {requestResponse}", true);
                    }
                }

                // Replaced: VIMTfnrtngdtInfo = VEISVIMTfnrtngdtreturnInfo
                if (findRatingDataResponse != null && findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo != null
                    && findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtdisabilityRatingRecordInfo != null)
                {
                    ratingInfo = findRatingDataResponse.VEISVIMTfnrtngdtreturnInfo.VEISVIMTfnrtngdtdisabilityRatingRecordInfo;
                }

                response.UDOInitiateFNODExceptions = new UDOInitiateFNODException
                {
                    ExceptionMessage = findRatingDataResponse.ExceptionMessage,
                    ExceptionOccurred = findRatingDataResponse.ExceptionOccurred
                };
                timer.MarkStop("findRatingDatatRequest");
                #endregion

                #region va_awardratings [ratingInfo]
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: va_awardratings.");

                if (ratingInfo != null)
                {
                    if (!string.IsNullOrEmpty(ratingInfo.mcs_serviceConnectedCombinedDegree))
                    {
                        new_va_fnod_entity["va_awardratings"] = ratingInfo.mcs_serviceConnectedCombinedDegree;
                    }
                }

                #endregion

                #region findMilitaryRecordByPtcpntId [militaryTours]
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: findMilitaryRecordByPtcpntId.");

                // Replaced: VIMTfmirecpicmilitaryPersonToursclmsMultipleResponse = VEISfmirecpicmilitaryPersonToursMultipleResponse
                VEISfmirecpicmilitaryPersonToursMultipleResponse[] militaryTours = null;
                if (request.ptcpntId > 0)
                {
                    string startFindMilitaryRecord = timer.MarkStart("findMilitaryRecordByPtcpntId");
                    // prefix = fmirecpicfindMilitaryRecordByPtcpntIdRequest();
                    var findMilitaryRecordByPtcpntIdRequest = new VEISfmirecpicfindMilitaryRecordByPtcpntIdRequest
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
                        mcs_ptcpntid = request.ptcpntId.ToString()
                    };

                    // REM: Invoke VEIS Endpoint
                    // Replaced: var findMilitaryRecordByPtcpntIdResponse = findMilitaryRecordByPtcpntIdRequest.SendReceive<VIMTfmirecpicfindMilitaryRecordByPtcpntIdResponse>(MessageProcessType.Local);
                    var findMilitaryRecordByPtcpntIdResponse = WebApiUtility.SendReceive<VEISfmirecpicfindMilitaryRecordByPtcpntIdResponse>(findMilitaryRecordByPtcpntIdRequest, WebApiType.VEIS);
                    progressString = "After VEIS EC Call - findMilitaryRecordByPtcpntIdRequest";

                    if (request.LogSoap || findMilitaryRecordByPtcpntIdResponse.ExceptionOccurred)
                    {
                        if (findMilitaryRecordByPtcpntIdResponse.SerializedSOAPRequest != null || findMilitaryRecordByPtcpntIdResponse.SerializedSOAPResponse != null)
                        {
                            var requestResponse = findMilitaryRecordByPtcpntIdResponse.SerializedSOAPRequest + findMilitaryRecordByPtcpntIdResponse.SerializedSOAPResponse;
                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfmirecpicfindMilitaryRecordByPtcpntIdRequest Request/Response {requestResponse}", true);
                        }
                    }

                    militaryTours = findMilitaryRecordByPtcpntIdResponse.VEISfmirecpicreturnInfo == null ? null :
                        findMilitaryRecordByPtcpntIdResponse.VEISfmirecpicreturnInfo
                        .VEISfmirecpicmilitaryPersonToursInfo;

                    response.UDOInitiateFNODExceptions = new UDOInitiateFNODException
                    {
                        ExceptionMessage = findMilitaryRecordByPtcpntIdResponse.ExceptionMessage,
                        ExceptionOccurred = findMilitaryRecordByPtcpntIdResponse.ExceptionOccurred
                    };
                    timer.MarkStop("findMilitaryRecordByPtcpntId");
                }
                #endregion

                #region va_corpmilitarydischargetype, va_corpmilitaryseparationreason, va_corpverified [militaryTours]
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: va_corpmilitarydischargetype, va_corpmilitaryseparationreason, va_corpverified.");

                if (militaryTours != null)
                {
                    foreach (var tour in militaryTours)
                    {
                        if (!string.IsNullOrEmpty(tour.mcs_militarySeperationReasonTypeName))
                        {
                            new_va_fnod_entity["va_corpmilitaryseparationreason"] = tour.mcs_militarySeperationReasonTypeName;
                        }
                        if (
                            !string.IsNullOrEmpty(
                                tour.mcs_mpDischargeCharTypeName))
                        {
                            new_va_fnod_entity["va_corpmilitarydischargetype"] =
                                tour.mcs_mpDischargeCharTypeName;
                        }
                        if (!string.IsNullOrEmpty(tour.mcs_verifiedInd))
                        {
                            new_va_fnod_entity["va_corpverified"] =
                                tour.mcs_verifiedInd;
                        }
                    }
                }
                #endregion

                #region Insurance Policy Information
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Entered code region: Insurance Policy Information.");

                int policyCount = 0;
                string policyString = "";
                if (birlsRecord.VEISbrlsFNINSURANCE_POLICYInfo != null)
                {
                    foreach (var insurancePolicy in birlsRecord.VEISbrlsFNINSURANCE_POLICYInfo)
                    {
                        // Check if insurance policy is active (lapsed/purge date is blank) & bring over logic from UDOgetBIRLSDataProcessor
                        if (!string.IsNullOrEmpty(birlsRecord.mcs_DATE_OF_INS_LAPSED_PURGE))
                        {
                            continue;
                        }
                        else if ((policyCount == 0) && ((!string.IsNullOrEmpty(insurancePolicy.mcs_INS_POL_PREFIX)) && (!string.IsNullOrWhiteSpace(insurancePolicy.mcs_INS_POL_PREFIX))) || ((!string.IsNullOrEmpty(insurancePolicy.mcs_INS_POL_NUMBER)) && (!string.IsNullOrWhiteSpace(insurancePolicy.mcs_INS_POL_NUMBER))) || ((!string.IsNullOrEmpty(birlsRecord.mcs_INS_PREFIX)) && (!string.IsNullOrWhiteSpace(birlsRecord.mcs_INS_PREFIX))) || ((!string.IsNullOrEmpty(birlsRecord.mcs_INS_NUMBER)) && (!string.IsNullOrWhiteSpace(birlsRecord.mcs_INS_NUMBER))) || ((!string.IsNullOrEmpty(birlsRecord.mcs_DATE_OF_INS_LAPSED_PURGE) && (!string.IsNullOrWhiteSpace(birlsRecord.mcs_DATE_OF_INS_LAPSED_PURGE))) || ((!string.IsNullOrEmpty(birlsRecord.mcs_INSURANCE_JURIS)) && (!string.IsNullOrWhiteSpace(birlsRecord.mcs_INSURANCE_JURIS))) || ((policyCount > 0) && ((!string.IsNullOrEmpty(insurancePolicy.mcs_INS_POL_PREFIX)) && (!string.IsNullOrWhiteSpace(insurancePolicy.mcs_INS_POL_PREFIX))) || ((!string.IsNullOrEmpty(insurancePolicy.mcs_INS_POL_NUMBER)) && (!string.IsNullOrWhiteSpace(insurancePolicy.mcs_INS_POL_NUMBER))))))
                        {
                            policyCount++;

                            string policyPrefix = "None";
                            string policyNumber = "None";
                            string insurancePrefix = "None";
                            string insuranceNumber = "None";

                            if (!string.IsNullOrEmpty(insurancePolicy.mcs_INS_POL_PREFIX) && !string.IsNullOrWhiteSpace(insurancePolicy.mcs_INS_POL_PREFIX))
                            {
                                policyPrefix = insurancePolicy.mcs_INS_POL_PREFIX;
                            }

                            if (!string.IsNullOrEmpty(insurancePolicy.mcs_INS_POL_NUMBER) && !string.IsNullOrWhiteSpace(insurancePolicy.mcs_INS_POL_NUMBER))
                            {
                                policyNumber = insurancePolicy.mcs_INS_POL_NUMBER;
                            }

                            if (!string.IsNullOrEmpty(birlsRecord.mcs_INS_PREFIX) && !string.IsNullOrWhiteSpace(birlsRecord.mcs_INS_PREFIX))
                            {
                                insurancePrefix = birlsRecord.mcs_INS_PREFIX;
                            }

                            if (!string.IsNullOrEmpty(birlsRecord.mcs_INS_NUMBER) && !string.IsNullOrWhiteSpace(birlsRecord.mcs_INS_NUMBER))
                            {
                                insuranceNumber = birlsRecord.mcs_INS_NUMBER;
                            }

                            policyString += $"{Environment.NewLine}{policyCount.ToString()}.) Policy Prefix: {policyPrefix}, Policy #: {policyNumber}, Insurance Prefix: {insurancePrefix}, Insurance #: {insuranceNumber}";
                        }
                    }
                }
                policyString = $"This Veteran has {policyCount.ToString()} active Insurance Policies (accurate as of {DateTime.UtcNow.ToString("g")} UTC).{policyString}";
                new_va_fnod_entity["udo_insurancepolicies"] = policyString;
                #endregion

                #region Set udo_regionalofficeid (SOJ)
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Setting udo_regionalofficeid (SOJ) field on FNOD record.");

                // Code brought over and refactored from UDOInitiateLettersProcessor
                if (!string.IsNullOrEmpty(birlsRecord.mcs_CLAIM_FOLDER_LOCATION))
                {
                    var soj = getSojId(birlsRecord.mcs_CLAIM_FOLDER_LOCATION);

                    if (soj != null)
                    {
                        new_va_fnod_entity["udo_regionalofficeid"] = soj;

                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "SOJ found.");
                    }
                }
                #endregion

                // TODO: REM: CallerId 
                //      Legacy code had CallerId = UserId so the context would not run as service account but as the user/callerId
                //OrgServiceProxy.CallerId = Guid.Empty;
                //webProxyClient.CallerId = Guid.Empty;
                //var fnodid = OrgServiceProxy.Create(TruncateHelper.TruncateFields(new_va_fnod_entity, request.OrganizationName, request.UserId, request.LogTiming));
                var fnodid = OrgServiceProxy.Create(TruncateHelper.TruncateFields(request.MessageId, new_va_fnod_entity, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                response.newUDOInitiateFNODId = fnodid;

                #region Stop Timer
                LogHelper.LogTiming(request.OrganizationName, request.Debug, request.UserId,
                    request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method,
                    method, timer.ElapsedMilliseconds);
                // TODO: Resolve Timer Error
                //Convert.ToDecimal(timer.ElapsedMilliseconds));

                //var o = request.OrganizationName;
                //var db = request.Debug;
                //var u = request.UserId;
                //var m = method;
                //var s = true;
                //timer.LogDurations(o, db, u, m, s);
                timer.LogDurations(request.OrganizationName, request.Debug, request.UserId, method, true);

                #endregion

                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.Execute", "Returning response object.");

                return response;
            }
            catch (Exception ExecutionException)
            {
                var method = MethodInfo.GetThisMethod().ToString(false);
                //LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ExecutionException);
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, ExecutionException);
                timer.LogDurations(request.OrganizationName, request.Debug, request.UserId, method, true);
                response.UDOInitiateFNODExceptions = new UDOInitiateFNODException
                {
                    ExceptionMessage = "Failed to process FNOD Data",
                    ExceptionOccurred = true
                };
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

        // Removed: static 
        private string getPOA(IOrganizationService OrgServiceProxy, UDOInitiateFNODRequest request, out UDOInitiateFNODException response)
        {
            var progressString = "getPOAFIDData Start";
            //var serviceProxy = OrgServiceProxy as OrganizationServiceProxy;
            response = new UDOInitiateFNODException { ExceptionMessage = String.Empty, ExceptionOccurred = false };
            try
            {
                #region Get POA from person
                var fetch = "<fetch count='1'><entity name='udo_person'>" +
                          "<attribute name='udo_poa'/>" +
                          "<filter type='and'>" +
                          "<condition attribute='udo_personid' operator='eq' value='" + request.udo_personId.ToString() + "'/>" +
                          "</filter></entity></fetch>";

                // TODO: REM: CallerId
                //serviceProxy.CallerId = request.UserId;
                var people = OrgServiceProxy.RetrieveMultiple(new FetchExpression(fetch));

                if (people.Entities.Count == 1 && people[0].Contains("udo_poa"))
                {

                    if (!String.IsNullOrEmpty((string)people[0]["udo_poa"])) return (string)people[0]["udo_poa"];
                }

                progressString = "After Fetch";
                #endregion

                #region findPOA
                //if this doesn't contain anything, don't go asking for it!
                if (!string.IsNullOrEmpty(request.fileNumber))
                {

                    progressString = "Finding POA - findAllFiduciaryPoaRequest";
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateFNODProcessor.getPOAFIDData", progressString);

                    var findAllFiduciaryPoaRequest = new VEISafidpoafindAllFiduciaryPoaRequest();
                    findAllFiduciaryPoaRequest.LogTiming = request.LogTiming;
                    findAllFiduciaryPoaRequest.LogSoap = request.LogSoap;
                    findAllFiduciaryPoaRequest.Debug = request.Debug;
                    findAllFiduciaryPoaRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                    findAllFiduciaryPoaRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                    findAllFiduciaryPoaRequest.RelatedParentId = request.RelatedParentId;
                    findAllFiduciaryPoaRequest.UserId = request.UserId;
                    findAllFiduciaryPoaRequest.OrganizationName = request.OrganizationName;

                    //non standard fields
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
                    // REM: Invoke VEIS Endpoint
                    // Replaced: var findAllFiduciaryPoaResponse = findAllFiduciaryPoaRequest.SendReceive<VIMTafidpoafindAllFiduciaryPoaResponse>(MessageProcessType.Local);
                    var findAllFiduciaryPoaResponse = WebApiUtility.SendReceive<VEISafidpoafindAllFiduciaryPoaResponse>(findAllFiduciaryPoaRequest, WebApiType.VEIS);

                    response.ExceptionMessage = findAllFiduciaryPoaResponse.ExceptionMessage;
                    response.ExceptionOccurred = findAllFiduciaryPoaResponse.ExceptionOccurred;
                    // Replaced: VIMTafidpoareturnclmsInfo = VEISafidpoareturnInfo
                    if (findAllFiduciaryPoaResponse.VEISafidpoareturnInfo != null)
                    {
                        // Replaced: VIMTafidpoacurrentPowerOfAttorneyclmsInfo = VEISafidpoacurrentPowerOfAttorneyInfo
                        if (findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo != null)
                        {
                            if (!string.IsNullOrEmpty(findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_personOrgName))
                            {
                                var poa = findAllFiduciaryPoaResponse.VEISafidpoareturnInfo.VEISafidpoacurrentPowerOfAttorneyInfo.mcs_personOrgName;

                                #region Update Person

                                var person = new Entity("udo_person");
                                person["udo_personid"] = request.udo_personId;
                                person["udo_poa"] = poa;
                                // TODO: REM: CallerId
                                //serviceProxy.CallerId = request.UserId;
                                OrgServiceProxy.Update(TruncateHelper.TruncateFields(request.MessageId, person, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));

                                #endregion

                                return poa;
                            }

                        }
                    }

                }
                #endregion

                return string.Empty;
            }
            catch (Exception ExecutionException)
            {
                var method = MethodInfo.GetThisMethod().ToString(false);
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, ExecutionException);
                //LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to get POAFIDDATA";
                response.ExceptionOccurred = true;
                return string.Empty;
            }
        }
        // REM: Invoke VEIS Endpoint
        private void InitProcessor(UDOInitiateFNODRequest request)
        {
            try
            {
                if (logSettings == null)
                {
                    logSettings = new LogSettings
                    {
                        CallingMethod = method,
                        Org = request.OrganizationName,
                        UserId = request.UserId
                    };
                }
                NameValueCollection veisConfigurations = VEISConfiguration.GetConfigurationSettings();
                veisBaseUri = new Uri(veisConfigurations.Get(VEISConfiguration.ECUri));
            }
            catch
            {
                // TODO: Handle any exceptions
            }
        }

        // Code brought over and refactored from UDOInitiateLettersProcessor
        private EntityReference getSojId(string stationCode)
        {
            EntityReference thisEntRef = new EntityReference();

            QueryExpression expression = new QueryExpression()
            {
                ColumnSet = new ColumnSet("va_regionalofficeid", "va_name"),
                EntityName = "va_regionaloffice",
                Criteria = {
                    Filters = {

                        new FilterExpression()
                        {
                            Conditions = {
                                new ConditionExpression("va_code", ConditionOperator.Equal, stationCode)
                            }
                        }
                    }
                }
            };

            EntityCollection results = OrgServiceProxy.RetrieveMultiple(expression);
            if (results.Entities.Count > 0)
            {
                if (results.Entities[0].Attributes.Contains("va_regionalofficeid"))
                {
                    Entity soj = results[0];
                    thisEntRef.Id = soj.Id;
                    thisEntRef.LogicalName = expression.EntityName;
                    thisEntRef.Name = soj.GetAttributeValue<string>("va_name");
                }
            }
            else
            {
                return null;
            }

            return thisEntRef;
        }
    }
}