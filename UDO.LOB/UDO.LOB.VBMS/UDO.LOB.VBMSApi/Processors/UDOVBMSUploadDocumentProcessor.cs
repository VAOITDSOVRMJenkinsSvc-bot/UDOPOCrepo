using UDO.LOB.Core;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.VBMS.Messages;

namespace UDO.LOB.VBMS.Processors
{
    internal class UDOVBMSUploadDocumentProcessor
    {
        public IMessageBase Execute(UDOVBMSUploadDocumentRequest request)
        {
            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "UDOVBMSUploadDocumentRequest", "UDOVBMSUploadDocumentProcessor - Preparing to upload document to VBMS", request.Debug);

            var response = new UDOVBMSUploadDocumentResponse();
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
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : string.Empty,
                    OrganizationName = request.OrganizationName
                };
            }

            UDOVBMSUploadDocumentAsyncRequest asyncRequest = new UDOVBMSUploadDocumentAsyncRequest()
            {
                MessageId = request.MessageId,
                Debug = request.Debug,
                LogSoap = request.LogSoap,
                LogTiming = request.LogTiming,
                udo_base64filecontents = request.udo_base64filecontents,
                udo_claimnumber = request.udo_claimnumber,
                udo_doctypeid = request.udo_doctypeid,
                udo_edipi = request.udo_edipi,
                udo_filename = request.udo_filename,
                udo_filenumber = request.udo_filenumber,
                udo_relatedentity = request.udo_relatedentity,
                udo_source = request.udo_source,
                udo_ssid = request.udo_ssid,
                udo_subject = request.udo_subject,
                udo_vbmsdocument = request.udo_vbmsdocument,
                udo_vet_firstname = request.udo_vet_firstname,
                udo_vet_lastname = request.udo_vet_lastname,
                udo_vet_middlename = request.udo_vet_middlename,
                UserId = request.UserId,
                OrganizationId = request.OrganizationId,
                OrganizationName = request.OrganizationName,
                RelatedParentEntityName = request.RelatedParentEntityName,
                RelatedParentFieldName = request.RelatedParentFieldName,
                RelatedParentId = request.RelatedParentId,
                udo_userName = request.udo_userName,
                udo_userRole = request.udo_userRole
            };

            if (request.LegacyServiceHeaderInfo != null)
            {
                asyncRequest.LegacyServiceHeaderInfo = new UDOHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
            }

            WebApiUtility.SendAsync(request: asyncRequest, webApiType: WebApiType.LOB);
            response.Processing = true;
            return response;
        }
    }
}
