using Microsoft.Xrm.Tooling.Connector;
using System;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.Notes.Messages;
using VEIS.Core.Messages;
using VEIS.Messages.DevelopmentNotesService;

/// <summary>
/// VIMT LOB Component for UDOCreateNote,CreateNote method, Processor.
/// </summary>
namespace UDO.LOB.Notes.Processors
{
    public class UDODeleteNoteProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDODeleteNoteProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDODeleteNoteRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.Execute");
            UDODeleteNoteResponse response = new UDODeleteNoteResponse() { MessageId = request.MessageId };
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

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

            LogHelper.LogInfo($">> {this.GetType().FullName}.Execute: Request:  {JsonHelper.Serialize<UDODeleteNoteRequest>(request)}");

            #region connect to CRM
            CrmServiceClient OrgServiceProxy = null;

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
                var deleteNoteRequest = new VEISdeletedeleteNoteRequest();
                deleteNoteRequest.LogTiming = request.LogTiming;
                deleteNoteRequest.LogSoap = request.LogSoap;
                deleteNoteRequest.Debug = request.Debug;
                deleteNoteRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                deleteNoteRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                deleteNoteRequest.RelatedParentId = request.RelatedParentId;
                deleteNoteRequest.UserId = request.UserId;
                deleteNoteRequest.OrganizationName = request.OrganizationName;

                var modifiedDate = DateTime.Now;
                deleteNoteRequest.VEISdeletedReqnoteInfo = new VEISdeletedReqnote
                {
                    mcs_noteOutTn = request.udo_Type,
                    mcs_suspnsDt = modifiedDate,
                    mcs_suspnsDtSpecified = true,
                    mcs_txt = request.udo_Note,
                    mcs_userId = request.udo_User,
                    mcs_ptcpntId = request.udo_ParticipantID,

                    mcs_jrnDt = modifiedDate,
                    mcs_jrnDtSpecified = true,
                    mcs_modifdDt = modifiedDate,
                    mcs_modifdDtSpecified = true
                };
                deleteNoteRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
                DateTime dtTime;
                if (!String.IsNullOrEmpty(request.udo_dtTime))
                {
                    dtTime = DateTime.Parse(request.udo_dtTime);
                    deleteNoteRequest.VEISdeletedReqnoteInfo.mcs_createDt = dtTime;
                    deleteNoteRequest.VEISdeletedReqnoteInfo.mcs_createDtSpecified = true;
                }
                else
                {
                    if (request.udo_LegacyNoteId.Length > 0)
                    {
                        deleteNoteRequest.VEISdeletedReqnoteInfo.mcs_noteId = request.udo_LegacyNoteId;
                    }
                }
                if (request.udo_ClaimId != null)
                {
                    deleteNoteRequest.VEISdeletedReqnoteInfo.mcs_bnftClmNoteTc = "CLMDVLNOTE";
                    deleteNoteRequest.VEISdeletedReqnoteInfo.mcs_noteOutTn = "Claim Development Note";
                    deleteNoteRequest.VEISdeletedReqnoteInfo.mcs_clmId = request.udo_ClaimId;
                }
                else
                {
                    deleteNoteRequest.VEISdeletedReqnoteInfo.mcs_ptcpntNoteTc = "CLMNTCONTACT";
                    deleteNoteRequest.VEISdeletedReqnoteInfo.mcs_noteOutTn = "Contact with Claimant";
                }


                if (!string.IsNullOrEmpty(request.udo_ClaimId))
                {
                    deleteNoteRequest.VEISdeletedReqnoteInfo.mcs_callId = Int64.Parse(request.udo_ClaimId);
                }

                var deleteNoteResponse = WebApiUtility.SendReceive<VEISdeletedeleteNoteResponse>(deleteNoteRequest, WebApiType.VEIS);
                if (request.LogSoap || deleteNoteResponse.ExceptionOccurred)
                {
                    if (deleteNoteResponse.SerializedSOAPRequest != null || deleteNoteResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = deleteNoteResponse.SerializedSOAPRequest + deleteNoteResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"Request Request/Response {requestResponse}", true);
                    }
                }

                progressString = "After VIMT EC Call";

                response.ExceptionMessage = deleteNoteResponse.ExceptionMessage;
                response.ExceptionOccurred = deleteNoteResponse.ExceptionOccurred;
                if (deleteNoteResponse.VEISdeletedVoidInfo != null)
                {

                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, method, $" Processor, Progess: {progressString}");
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, method, ExecutionException);
                response.ExceptionMessage = $"{method} Failed to Process Notes Data Deletion";
                response.ExceptionOccurred = true;
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
    }
}