using Microsoft.Xrm.Sdk.Query;
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
/// <param name=none></param>
/// <returns>none</returns>
namespace UDO.LOB.Notes.Processors
{
    public class UDOUpdateNoteProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOUpdateNoteRequest";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOUpdateNoteRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.Execute"); //{JsonHelper.Serialize<UDOcreateAwardsRequest>(request)}");

            if (String.IsNullOrEmpty(request.udo_User)) { }

            var response = new UDOUpdateNoteResponse();
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

            LogHelper.LogInfo($">> {this.GetType().FullName}.Execute: Request:  {JsonHelper.Serialize<UDOUpdateNoteRequest>(request)}");

            try
            {
                #region Validate User Created Note.
                if (!String.IsNullOrEmpty(request.udo_User))
                {
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

                    using (OrgServiceProxy)
                    {
                        var crmUser = OrgServiceProxy.Retrieve("systemuser", request.UserId, new ColumnSet("va_pcrssn"));
                        if (crmUser.Contains("va_pcrssn"))
                        {
                            var pcrssn = crmUser["va_pcrssn"].ToString();
                            if (!String.IsNullOrEmpty(pcrssn) &&
                                !String.Equals(pcrssn, request.udo_User, StringComparison.InvariantCultureIgnoreCase))
                            {
                                throw new Exception("customOnly the original User can update a note.");
                            }
                        }
                    }
                }
                #endregion

                var updateNoteRequest = new VEISupdateupdateNoteRequest
                {
                    LogTiming = request.LogTiming,
                    LogSoap = request.LogSoap,
                    Debug = request.Debug,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName
                };
                var nowNow = DateTime.Now;

                var nowTime = string.Format("00", (nowNow.Day)) + "-" + string.Format("00", (nowNow.Month + 1)) + "-" +
                              string.Format("00", (nowNow.Day + 1)) + nowNow.ToUniversalTime();

                var tempString = DateTime.Now.ToString("yyyy-MM-ddTHH:MM:ss-4:00");

                var tempDate = DateTime.Parse(tempString);
                DateTime dtTime;

                updateNoteRequest.noteInfo = new VEISupdatenote
                {
                    mcs_jrnLctnId = request.udo_RO,
                    mcs_suspnsDt = tempDate,
                    mcs_suspnsDtSpecified = true,
                    mcs_txt = request.udo_Note,
                    mcs_userId = request.udo_User,
                    mcs_ptcpntId = request.udo_ParticipantID,
                    mcs_jrnDt = tempDate,
                    mcs_jrnDtSpecified = true,
                    mcs_modifdDt = tempDate,
                    mcs_modifdDtSpecified = true,
                    mcs_jrnUserId = request.udo_User
                };

                if (!string.IsNullOrEmpty(request.udo_dtTime))
                {
                    dtTime = DateTime.Parse(request.udo_dtTime);
                    updateNoteRequest.noteInfo.mcs_createDt = dtTime;
                    updateNoteRequest.noteInfo.mcs_noteId = "";
                    updateNoteRequest.noteInfo.mcs_createDtSpecified = true;
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method,
                        string.Format("The note was created on {0}", request.udo_dtTime));
                }
                else
                {
                    if (request.udo_LegacyNoteId.Length > 0)
                    {
                        updateNoteRequest.noteInfo.mcs_noteId = request.udo_LegacyNoteId;
                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId,
                           method, string.Format("Note ID: {0}", request.udo_LegacyNoteId));
                    }
                }
                if (request.udo_ClaimId != null)
                {
                    updateNoteRequest.noteInfo.mcs_bnftClmNoteTc = "CLMDVLNOTE";
                    updateNoteRequest.noteInfo.mcs_noteOutTn = "Claim Development Note";
                    updateNoteRequest.noteInfo.mcs_clmId = request.udo_ClaimId;
                }
                else
                {
                    updateNoteRequest.noteInfo.mcs_ptcpntNoteTc = "CLMNTCONTACT";
                    updateNoteRequest.noteInfo.mcs_noteOutTn = "Contact with Claimant";
                    updateNoteRequest.noteInfo.mcs_clmId = null;
                }

                updateNoteRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };

                if (!string.IsNullOrEmpty(request.udo_ClaimId))
                {
                    updateNoteRequest.noteInfo.mcs_callId = long.Parse(request.udo_ClaimId);
                }
                else
                {
                    updateNoteRequest.noteInfo.mcs_callId = 17;
                }
                LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method,
                    "Sending data to VIMT");

                //VIMT doesn't return data??
                var updateNoteResponse = WebApiUtility.SendReceive<VEISupdateupdateNoteResponse>(updateNoteRequest, WebApiType.VEIS);
                if (request.LogSoap || updateNoteResponse.ExceptionOccurred)
                {
                    if (updateNoteResponse.SerializedSOAPRequest != null || updateNoteResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = updateNoteResponse.SerializedSOAPRequest + updateNoteResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISupdateupdateNoteRequest Request/Response {requestResponse}", true);
                    }
                }

                progressString = "After VIMT EC Call";

                response.ExceptionMessage = updateNoteResponse.ExceptionMessage;
                response.ExceptionOccurred = updateNoteResponse.ExceptionOccurred;

                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, method + " Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Notes Data";
                response.ExceptionOccurred = true;
                return response;
            }
        }
    }
}