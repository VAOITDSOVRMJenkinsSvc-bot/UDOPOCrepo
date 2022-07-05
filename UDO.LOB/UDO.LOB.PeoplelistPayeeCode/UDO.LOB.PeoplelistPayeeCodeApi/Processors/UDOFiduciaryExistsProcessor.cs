using System;
using System.Globalization;
using System.Text;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.PeoplelistPayeeCode.Messages;
using VEIS.Core.Messages;
using VEIS.Messages.ClaimantService;

namespace UDO.LOB.PeoplelistPayeeCode.Processors
{
    internal class UDOFiduciaryExistsProcessor
    {
        private bool _debug { get; set; }

        private string LogBuffer { get; set; }

        private const string method = "UDOFiduciaryExistsProcessor";

        public StringBuilder sr_log { get; set; }

        public IMessageBase Execute(UDOFiduciaryExistsRequest request)
        {
            
            TraceLogger tLogger = new TraceLogger(method, request);
            sr_log = new StringBuilder("UDO Fiduciary Exists Log:");
            var response = new UDOFiduciaryExistsResponse { MessageId = request !=null ? request.MessageId : "-NA-" };

            #region Check for request

            if (request == null)
            {
                response.ExceptionMessage = $"ERROR: {method} called with no request message.";
                response.ExceptionOccurred = true;
                return response;
            }

            #endregion

            if (request.DiagnosticsContext == null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = method,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA",

                    OrganizationName = request.OrganizationName,

                };
            }

            try
            {
                if (request.LogSoap || response.ExceptionOccurred)
                {
                    var method = MethodInfo.GetThisMethod().ToString(false);
                    var requestMessage = "Request: \r\n\r\n" + JsonHelper.Serialize<UDOFiduciaryExistsRequest>(request);
                    LogHelper.LogInfo(request.OrganizationName, request.LogSoap, request.UserId, method, requestMessage);
                }

                var fidexist = false;

                var findFiduciaryRequest = new VEISfidfindFiduciaryRequest()
                {
                    MessageId = request.MessageId,
                    LogTiming = request.LogTiming,
                    LogSoap = request.LogSoap,
                    Debug = request.Debug,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName
                };

                var searchfor = request.fileNumber;

                if (String.IsNullOrEmpty(searchfor))
                {
                    response.ExceptionOccurred = true;
                    response.ExceptionMessage = "No search identifier provided in the request";
                    return response;
                }

                findFiduciaryRequest.mcs_filenumber = searchfor;

                if (request.LegacyServiceHeaderInfo != null)
                {
                    findFiduciaryRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                }

                var findFiduciaryResponse = WebApiUtility.SendReceive<VEISfidfindFiduciaryResponse>(findFiduciaryRequest, WebApiType.VEIS);
                if (request.LogSoap || findFiduciaryResponse.ExceptionOccurred)
                {
                    if (findFiduciaryResponse.SerializedSOAPRequest != null || findFiduciaryResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findFiduciaryResponse.SerializedSOAPRequest + findFiduciaryResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfidfindFiduciaryRequest Request/Response {requestResponse}", true);
                    }
                }

                tLogger.LogEvent($"Completed VEISfidfindFiduciaryResponse ", "001");

                if (findFiduciaryResponse.VEISfidreturnInfo != null)
                {
                    var fidInfo = findFiduciaryResponse.VEISfidreturnInfo;

                    DateTime fidEndDate = DateTime.Today.AddDays(5);

                    if (!String.IsNullOrEmpty(fidInfo.mcs_endDate))
                       fidEndDate = DateTime.ParseExact(fidInfo.mcs_endDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                    var currentDate = DateTime.Today;

                    if (!String.IsNullOrEmpty(fidInfo.mcs_personOrgName) && fidEndDate > currentDate) fidexist = true;
                }

                response.FiduciaryExists = fidexist;

                return response;
            }
            catch (Exception ex)
            {
                var method = String.Format("{0}:Find Fiduciary", MethodInfo.GetThisMethod().ToString(true));
                var message = String.Format("ERROR: {0}\r\nSOURCE: {1}\r\n\r\n", ex.Message, ex.Source);
                if (ex.InnerException != null)
                {
                    message += String.Format("Inner Exception: {0}\r\n\r\n", ex.InnerException.Message);
                }
                message += String.Format("Call Stack:\r\n{0}", ex.StackTrace);
                sr_log.Insert(0, message);
                message = sr_log.ToString();
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId,
                    request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, $"Method: {method} , Message: {message}", ex);
                response.ExceptionMessage = "Failed to Find Fiduciary info";
                response.ExceptionOccurred = true;
                tLogger.LogException(ex, "002");
                return response;
            }
        }
    }
}
