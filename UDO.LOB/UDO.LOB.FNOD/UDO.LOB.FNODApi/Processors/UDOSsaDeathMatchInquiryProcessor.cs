using System;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.FNOD.Messages;
using VEIS.Core.Messages;
using VEIS.Messages.SsaInquiry;


namespace UDO.LOB.FNOD.Processors
{
    internal class UDOSsaDeathMatchInquiryProcessor
    {
        private TimeTracker timer { get; set; }
        private bool _debug { get; set; }
        private const string METHOD = "UDOSsaDeathMatchInquiryProcessor";

        public UDOSsaDeathMatchInquiryProcessor()
        {
            timer = new TimeTracker();
        }

        public IMessageBase Execute(UDOSsaDeathMatchInquiryRequest request)
        {
            #region Initialize Processor Metadata
            // Start timer
            timer.Restart();

            // Create response object (also sets default values via constructor)
            var response = new UDOSsaDeathMatchInquiryResponse();

            // Set debug
            _debug = request.Debug;
            LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, METHOD, "Debug is set to true.");

            // Confirm request is not null
            if (request == null)
            {
                response.SsaDeathMatchException.SetValues(true, "Called with no message");
                return response;
            }

            // Log MessageId and Type
            LogHelper.LogInfo(string.Format("Message Id:{0}, Type={2}, Recieved diagnostics message: {1}",
                request.MessageId,
                request.MessageId,
                GetType().FullName));

            // Set request diagnostics
            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = METHOD,
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
                };
            }
            #endregion

            #region Main Business Logic
            try
            {
                // Build request for EC
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, METHOD, "Building SSA Inquiry request for EC.");
                var ssaInquiryRequest = new VEISfSsaInquiryRequest
                {
                    UserId = request.UserId,
                    RelatedParentId = request.RelatedParentId,
                    OrganizationName = request.OrganizationName,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    LogTiming = request.LogTiming,
                    LogSoap = request.LogSoap,
                    Debug = request.Debug,
                    LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    },
                    mcs_SsaInquiryInput = new SsaInquiryInput
                    {
                        // Add required fields here
                        mcs_fileNumberField = request.mcs_SsaInquiryInput.mcs_fileNumberField,
                        mcs_dobField = request.mcs_SsaInquiryInput.mcs_dobField,
                        mcs_firstNameField = request.mcs_SsaInquiryInput.mcs_firstNameField,
                        mcs_lastNameField = request.mcs_SsaInquiryInput.mcs_lastNameField,
                        mcs_vetFileNumberField = request.mcs_SsaInquiryInput.mcs_vetFileNumberField,
                        mcs_reasonField = "Verify DOD"
                    }
                };

                // Send SSA Inquiry Request to EC
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, METHOD, "Sending SSA Inquiry request to EC.");
                var UDOSsaDeathMatchInquiryResponse = WebApiUtility.SendReceive<VEISSsaInquiryResponse>(ssaInquiryRequest, WebApiType.VEIS);


                if (request.LogSoap || UDOSsaDeathMatchInquiryResponse.ExceptionOccurred)
                {
                    if (UDOSsaDeathMatchInquiryResponse.SerializedSOAPRequest != null || UDOSsaDeathMatchInquiryResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = UDOSsaDeathMatchInquiryResponse.SerializedSOAPRequest + UDOSsaDeathMatchInquiryResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfSsaInquiryRequest Request/Response {requestResponse}", true);
                    }
                }

                // Parse response from EC
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, METHOD, "Parsing SSA Inquiry response from EC.");
                if (UDOSsaDeathMatchInquiryResponse.SsaInquiry != null && (!string.IsNullOrEmpty(UDOSsaDeathMatchInquiryResponse.SsaInquiry.mcs_dateOfDeathField) || !string.IsNullOrEmpty(UDOSsaDeathMatchInquiryResponse.SsaInquiry.mcs_dodField)))
                {
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, METHOD, "Setting response.DateOfDeath value.");
                    response.DateOfDeath = true;
                }

                response.SsaDeathMatchException.ExceptionOccurred = UDOSsaDeathMatchInquiryResponse.ExceptionOccurred;
                response.SsaDeathMatchException.ExceptionMessage = UDOSsaDeathMatchInquiryResponse.ExceptionMessage;
                response.SoapRequestString = UDOSsaDeathMatchInquiryResponse.SerializedSOAPRequest;
                response.SoapResponseString = UDOSsaDeathMatchInquiryResponse.SerializedSOAPResponse;
                response.RelatedParentId = request.RelatedParentId;
                response.RelatedParentEntityName = request.RelatedParentEntityName;
                response.RelatedParentFieldName = request.RelatedParentFieldName;


                // Log timings
                LogHelper.LogTiming(request.OrganizationName, request.Debug, request.UserId,
                    request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, METHOD,
                    METHOD, timer.ElapsedMilliseconds);
                timer.LogDurations(request.OrganizationName, request.Debug, request.UserId, METHOD, true);

                // Return response to D365 Custom Action
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, METHOD, "Returning response to D365.");
                return response;
            }
            catch (Exception ex)
            {
                var method = MethodInfo.GetThisMethod().ToString(false);
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method, ex);
                timer.LogDurations(request.OrganizationName, request.Debug, request.UserId, method, true);
                response.SsaDeathMatchException.SetValues(true, "Failed to process SSA Inquiry");
                return response;
            }
            #endregion
        }
    }
}