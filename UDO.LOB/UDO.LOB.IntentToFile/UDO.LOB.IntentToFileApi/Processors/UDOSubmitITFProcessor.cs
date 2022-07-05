

namespace UDO.LOB.IntentToFile.Processors
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Client;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Crm.Sdk.Messages;
    using System;
    using System.Collections.Specialized;
    using UDO.LOB.Core;
    using UDO.LOB.Extensions;
    using UDO.LOB.Extensions.Configuration;
    using UDO.LOB.Extensions.Logging;
    using UDO.LOB.IntentToFile.Messages;
    using VEIS.Core.Messages;
    using VEIS.Messages.IntentToFileWebService;
    using Microsoft.Xrm.Sdk.WebServiceClient;
    using Microsoft.Xrm.Tooling.Connector;

    public class UDOSubmitITFProcessor
    {
        private CrmServiceClient OrgServiceProxy;
        private bool _debug { get; set; }

        private string LogBuffer { get; set; }

        private const string method = "UDOSubmitITFProcessor";
        
        public IMessageBase Execute(UDOSubmitITFRequest request)
        {
            var response = new UDOSubmitITFResponse
            {
                MessageId = request.MessageId
            };

            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccurred = true;
                return response;
            }
            if (request.DiagnosticsContext == null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : string.Empty,
                };
            }
            TraceLogger aiLogger = new TraceLogger(method, request);

            // Replaced: Logger.Instance.Info
            LogHelper.LogInfo(string.Format("Message Id:{0}, Type={2}, Recieved diagnostics message: {1}",
            request.MessageId,
            request.MessageId,
            GetType().FullName));

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
            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccurred = true;
                return response;
            }
            #endregion

            progressString = "After Connection";

            try
            {
                #region - Insert intent to file

                // TODO: Replace SerializeToString not defined in UDO request
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "Execute", "Request - " + JsonHelper.Serialize<UDOSubmitITFRequest>(request));

                var fileIntentRequest = new VEISinsertInt2FileinsertIntentToFileRequest
                {
                    Debug = request.Debug,
                    LogSoap = request.LogSoap,
                    LogTiming = request.LogTiming,
                    OrganizationName = request.OrganizationName,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    // Replaced? VIMT.IntentToFileWebService.Messages.HeaderInfo()
                    LegacyServiceHeaderInfo = new LegacyHeaderInfo()
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    },

                    VEISinsertInt2FileReqintentToFileDTOInfo = new VEISinsertInt2FileReqintentToFileDTO 
                    {
                        mcs_itfTypeCd = request.Claimant.CompensationType,
                        mcs_ptcpntClmantIdSpecified = true,
                        mcs_ptcpntClmantId = Convert.ToInt64(request.Claimant.ClaimantParticipantId),
                        mcs_ptcpntVetIdSpecified = true,
                        mcs_ptcpntVetId = Convert.ToInt64(request.Claimant.VeteranParticipantId),
                        mcs_clmantFirstNm = request.Claimant.ClaimantFirstName,
                        mcs_clmantLastNm = request.Claimant.ClaimantLastName,
                        mcs_clmantMiddleNm = request.Claimant.ClaimantMiddleInitial,
                        mcs_clmantSsn = request.Claimant.ClaimantSsn,
                        mcs_vetFirstNm = request.Claimant.VeteranFirstName,
                        mcs_vetLastNm = request.Claimant.VeteranLastName,
                        mcs_vetMiddleNm = request.Claimant.VeteranMiddleInitial,
                        mcs_vetSsnNbr = request.Claimant.VeteranSsn,
                        mcs_vetFileNbr = request.Claimant.VeteranFileNumber,
                        mcs_genderCd = request.Claimant.VeteranGender,
                        mcs_vetBrthdyDt = ConvertToDateTime(request.Claimant.VeteranBirthDate),
                        mcs_clmantPhoneAreaNbr = request.Claimant.PhoneAreaCode,
                        mcs_clmantPhoneNbr = request.Claimant.Phone,
                        mcs_clmantEmailAddrsTxt = request.Claimant.Email,
                        mcs_clmantAddrsOneTxt = request.Claimant.AddressLine1,
                        mcs_clmantAddrsTwoTxt = request.Claimant.AddressLine2,
                        mcs_clmantAddrsUnitNbr = request.Claimant.AddressLine3,
                        mcs_clmantCityNm = request.Claimant.City,
                        mcs_clmantStateCd = request.Claimant.State,
                        mcs_clmantZipCd = request.Claimant.Zip,
                        mcs_clmantCntryNm = request.Claimant.Country,
                        mcs_jrnLctnId = request.Claimant.StationLocation,
                        mcs_createDt = DateTime.UtcNow,
                        mcs_rcvdDtSpecified = true,
                        mcs_rcvdDt = DateTime.UtcNow,
                        mcs_signtrInd = "Y",
                        mcs_submtrApplcnTypeCd = "CRM"
                    }
                };

                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "Execute", "fileIntentRequest - " + JsonHelper.Serialize<VEISinsertInt2FileinsertIntentToFileRequest>(fileIntentRequest));

                // REM: Invoke VEIS Endpoint
                // Replaced: var fileIntentResponse = fileIntentRequest.SendReceive<VIMTinsertInt2FileinsertIntentToFileResponse>(MessageProcessType.Local);
                var fileIntentResponse = WebApiUtility.SendReceive<VEISinsertInt2FileinsertIntentToFileResponse>(fileIntentRequest, WebApiType.VEIS);
                progressString = "After VEISinsertInt2FileinsertIntentToFileRequest EC Call";

                if (request.LogSoap || fileIntentResponse.ExceptionOccurred)
                {
                    if (fileIntentResponse.SerializedSOAPRequest != null || fileIntentResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = fileIntentResponse.SerializedSOAPRequest + fileIntentResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISinsertInt2FileinsertIntentToFileRequest Request/Response {requestResponse}", true);
                    }
                }

                #endregion

                response.ExceptionOccurred = fileIntentResponse.ExceptionOccurred;
                response.ExceptionMessage = fileIntentResponse.ExceptionMessage;
                response.MessageId = fileIntentResponse.MessageId;
                // TODO: replace SerializeToString
                // response.request = fileIntentRequest.SerializeToString();
                // response.response = fileIntentResponse.SerializeToString();

                if (fileIntentResponse.VEISinsertInt2FileintentToFileDTOInfo != null)
                {
                    if (!string.IsNullOrEmpty(fileIntentResponse.VEISinsertInt2FileintentToFileDTOInfo.mcs_jrnUserId))
                        response.jrnUserId = fileIntentResponse.VEISinsertInt2FileintentToFileDTOInfo.mcs_jrnUserId;

                    if (!string.IsNullOrEmpty(fileIntentResponse.VEISinsertInt2FileintentToFileDTOInfo.mcs_jrnLctnId))
                        response.jrnLctnId = fileIntentResponse.VEISinsertInt2FileintentToFileDTOInfo.mcs_jrnLctnId;

                    if (fileIntentResponse.VEISinsertInt2FileintentToFileDTOInfo.mcs_createDtSpecified)
                        response.createDt = fileIntentResponse.VEISinsertInt2FileintentToFileDTOInfo.mcs_createDt;

                    if (!string.IsNullOrEmpty(fileIntentResponse.VEISinsertInt2FileintentToFileDTOInfo.mcs_submtrApplcnTypeCd))
                        response.submtrApplcnTypeCd = fileIntentResponse.VEISinsertInt2FileintentToFileDTOInfo.mcs_submtrApplcnTypeCd;

                    if (fileIntentResponse.VEISinsertInt2FileintentToFileDTOInfo.mcs_intentToFileIdSpecified)
                        response.intentToFileId = fileIntentResponse.VEISinsertInt2FileintentToFileDTOInfo.mcs_intentToFileId.ToString();

                    response.IntentToFileDto = fileIntentResponse.VEISinsertInt2FileintentToFileDTOInfo;
                }
            }
            catch (Exception executionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, 
                    request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, 
                    "UDOSubmitITFProcessor Processor, Progess:" + progressString, executionException);
                response.ExceptionMessage = "Failed to process submit ITF";
                response.ExceptionOccurred = true;
            }
            finally
            {
                if (OrgServiceProxy != null)
                {
                    OrgServiceProxy.Dispose();
                }
            }

            return response;
        }

        private long ConvertToLong(string input)
        {
            long rtnInput = 0;

            if (long.TryParse(input, out rtnInput))
            {
                return rtnInput;
            }

            return 0;
        }

        private DateTime ConvertToDateTime(string inputDate)
        {
            DateTime chkDateTime = DateTime.MinValue;

            if (DateTime.TryParse(inputDate, out chkDateTime))
            {
                return chkDateTime;
            }

            return DateTime.MinValue;
        }
    }
}