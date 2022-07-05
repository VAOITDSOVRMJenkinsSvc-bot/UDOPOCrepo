namespace UDO.LOB.ClaimEstablishment.Processors
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Sdk.WebServiceClient;
    using Microsoft.Xrm.Tooling.Connector;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using UDO.LOB.ClaimEstablishment.Messages;
    using UDO.LOB.Core;
    using UDO.LOB.Extensions;
    using UDO.LOB.Extensions.Logging;
    using VEIS.Core.Messages;
    using VEIS.Messages.BenefitClaimServiceV2;

    public class UDOClearClaimEstablishmentProcessor
    {
        private CrmServiceClient OrgServiceProxy;
        private bool _debug { get; set; }

        private const string method = "UDOClearClaimEstablishmentProcessor";

        private string LogBuffer { get; set; }

        string progressString = "Top of Processor";
        public string MachineName { get; set; }

        public UDOClearClaimEstablishmentProcessor()
        {
            MachineName = System.Environment.MachineName;
        }

        public IMessageBase Execute(UDOClearClaimEstablishmentRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.Execute");
            UDOClearClaimEstablishmentResponse response = new UDOClearClaimEstablishmentResponse { MessageId = request?.MessageId };
            var claimEstablishmentExceptions = new List<UDOException>();
            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccurred = true;
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
            TraceLogger tLogger = new TraceLogger(method, request);

            LogHelper.LogInfo($">> {this.GetType().FullName}.Execute: Request:  {JsonHelper.Serialize<UDOClearClaimEstablishmentRequest>(request)}");

            _debug = request.Debug;
            LogBuffer = string.Empty;

            string progressString = "Top of Processor";

            #region connect to CRM
            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
                tLogger.LogEvent($"Authenticated and Connected to CRM", "2");

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
                var entity = OrgServiceProxy.Retrieve("udo_claimestablishment", request.ClaimEstablishmentId,
                    new ColumnSet(true));

                var claimCleared = ClearBenefitClaim(OrgServiceProxy, request, entity);
                response.ClaimEstablishmentId = request.ClaimEstablishmentId;
                response.ExceptionOccurred = claimCleared.ExceptionOccurred;

                if (claimCleared.ExceptionOccurred)
                {
                    response.ExceptionMessage = claimCleared.ExceptionMessage;
                }
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

        private UDOClearClaimEstablishmentResponse ClearBenefitClaim(IOrganizationService orgServiceProxy, UDOClearClaimEstablishmentRequest request, Entity entity)
        {
            var common = new UDOClaimEstablishmentCommon();
            var responseExtractor = new UDOClaimEstablishmentCommon();

            UDOClearClaimEstablishmentResponse response = new UDOClearClaimEstablishmentResponse();
            var claimEstablishmentExceptions = new List<UDOException>();

            try
            {
                var typecode = orgServiceProxy.Retrieve("udo_claimestablishmenttypecode", entity.GetAttributeValue<EntityReference>("udo_endproduct").Id, new ColumnSet(true));
                var payeecode = orgServiceProxy.Retrieve("udo_claimestablishmentpayeecode", entity.GetAttributeValue<EntityReference>("udo_payeecodeid").Id, new ColumnSet(true));


                var reqBenefitClaimInput = new VEISReqclearBenefitClaimInputBCS2();

                reqBenefitClaimInput.mcs_bypassIndicator = "3";
                reqBenefitClaimInput.mcs_incremental = "001";
                reqBenefitClaimInput.mcs_pclrReasonCode = "65";
                reqBenefitClaimInput.mcs_pclrReasonText = string.Empty;

                if (entity.Attributes.Contains("udo_benefitclaimtype"))
                {
                    reqBenefitClaimInput.mcs_benefitClaimType = common.RetrieveOptionSetValueFromEntityField(orgServiceProxy, entity, "udo_benefitclaimtype").ToString();
                }

                if (typecode.Attributes.Contains("udo_typecode"))
                {
                    var tmpTypeCode = typecode.GetAttributeValue<string>("udo_typecode").ToString();
                    reqBenefitClaimInput.mcs_endProductCode = tmpTypeCode;
                    reqBenefitClaimInput.mcs_incremental = tmpTypeCode.Substring(0, 3);
                };

                reqBenefitClaimInput.mcs_fileNumber = common.RetrieveValueFromEntityField(entity, "udo_filenumber");
                reqBenefitClaimInput.mcs_payeeCode = common.RetrieveValueFromEntityField(payeecode, "udo_payeecode");

                var clearBenefitClaimRequest = new VEISclearBenefitClaimRequest();

                clearBenefitClaimRequest.Debug = request.Debug;

                clearBenefitClaimRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo();
                progressString = "insertBenefitClaimRequest.LegacyServiceHeaderInfo";
                clearBenefitClaimRequest.LegacyServiceHeaderInfo.ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName;
                progressString = "insertBenefitClaimRequest.LegacyServiceHeaderInfo.ApplicationName";
                clearBenefitClaimRequest.LegacyServiceHeaderInfo.ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine;
                progressString = "insertBenefitClaimRequest.LegacyServiceHeaderInfo.ClientMachine";
                clearBenefitClaimRequest.LegacyServiceHeaderInfo.LoginName = request.LegacyServiceHeaderInfo.LoginName;
                progressString = "insertBenefitClaimRequest.LegacyServiceHeaderInfo.LoginName";
                clearBenefitClaimRequest.LegacyServiceHeaderInfo.StationNumber = request.LegacyServiceHeaderInfo.StationNumber;
                progressString = "insertBenefitClaimRequest.LegacyServiceHeaderInfo.StationNumber";

                clearBenefitClaimRequest.LogSoap = request.LogSoap;
                clearBenefitClaimRequest.LogTiming = request.LogTiming;
                clearBenefitClaimRequest.OrganizationName = request.OrganizationName;
                clearBenefitClaimRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                clearBenefitClaimRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                clearBenefitClaimRequest.RelatedParentId = request.RelatedParentId;
                clearBenefitClaimRequest.UserId = request.UserId;
                clearBenefitClaimRequest.MessageId = request.MessageId;
                clearBenefitClaimRequest.VEISReqclearBenefitClaimInputBCS2Info = reqBenefitClaimInput;

                // REM: Invoke VEIS Endpoint
                // var clearBenefitClaimResponse = clearBenefitClaimRequest.SendReceive<VEISclearBenefitClaimResponse>(request.ProcessType);
                //REM: 
                var clearBenefitClaimResponse = WebApiUtility.SendReceive<VEISclearBenefitClaimResponse>(clearBenefitClaimRequest, WebApiType.VEIS);

                if (request.LogSoap || clearBenefitClaimResponse.ExceptionOccurred)
                {
                    if (clearBenefitClaimResponse.SerializedSOAPRequest != null || clearBenefitClaimResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = clearBenefitClaimResponse.SerializedSOAPRequest + clearBenefitClaimResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISclearBenefitClaimRequest Request/Response {requestResponse}", true);
                    }
                }

                if (clearBenefitClaimResponse.ExceptionOccured == false)
                {
                    if (clearBenefitClaimResponse.VEISbenefitClaimRecordBCS2Info != null)
                    {
                        response.UDObenefitClaimRecordBCS2Info = responseExtractor.ExtractVEISResponse(clearBenefitClaimResponse.VEISbenefitClaimRecordBCS2Info);
                    }
                }

                responseExtractor.UpdateCrmClaimEstablishment(clearBenefitClaimResponse.ExceptionOccured, clearBenefitClaimResponse.ExceptionMessage,
                    request.ClaimEstablishmentId, response.UDObenefitClaimRecordBCS2Info,
                    ClaimEstablishmentStatus.Cleared, orgServiceProxy);

            }
            catch (Exception executionException)
            {
                var stInfo = string.Empty;

                var st = new StackTrace(executionException, true);
                for (var i = 0; i < st.FrameCount; i++)
                {
                    var sf = st.GetFrame(i);
                    stInfo = stInfo +
                             string.Format("LOB Machine Name: {0}, Method: {1}, File: {2}, Line Number: {3}", MachineName, sf.GetMethod(), sf.GetFileName(),
                                 sf.GetFileLineNumber());
                    stInfo = stInfo + System.Environment.NewLine;
                }

                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, method, executionException);

                response.ExceptionMessage = $"{method}: Exception Message: {executionException.Message} Execution Progress : {progressString}";
                response.ExceptionOccurred = true;
                response.StackTrace = stInfo;

                if (claimEstablishmentExceptions.Count > 0) response.InnerExceptions = claimEstablishmentExceptions.ToArray();
                return response;
            }

            return response;
        }
    }
}