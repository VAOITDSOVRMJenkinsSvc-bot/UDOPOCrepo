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
namespace UDO.LOB.Notes.Processors
{
    public class UDOCreateNoteProcessor
    {
        private bool _debug { get; set; }
        private string LogBuffer { get; set; }
        private const string method = "UDOCreateNoteProcessor";

        public IMessageBase Execute(UDOCreateNoteRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.Execute");
            var response = new UDOCreateNoteResponse() { MessageId = request.MessageId };
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = $" {method} Called with no message";
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

            LogHelper.LogInfo($">> {this.GetType().FullName}.Execute: Request:  {JsonHelper.Serialize<UDOCreateNoteRequest>(request)}");

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
                var createNoteRequest = new VEIScreatecreateNoteRequest
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

                DateTime dtTime = DateTime.Now;

                if (!string.IsNullOrEmpty(request.udo_DateTime))
                {
                    dtTime = DateTime.Parse(request.udo_DateTime);
                }

                //TODO: ptcpnt id, note text is null, user id
                //ptcpnt id and user id are required
                createNoteRequest.noteInfo = new VEIScreatenote
                {
                    mcs_createDt = dtTime,

                    mcs_createDtSpecified = true,
                    mcs_jrnLctnId = request.udo_RO,
                    mcs_suspnsDt = dtTime,
                    mcs_suspnsDtSpecified = true,
                    mcs_txt = request.udo_Note,
                    mcs_ptcpntId = request.udo_ParticipantID,
                    mcs_jrnDt = dtTime,
                    mcs_jrnDtSpecified = true,
                    mcs_modifdDt = dtTime,
                    mcs_modifdDtSpecified = true
                };

                // TODO: This section of comment code, needed?
                // If the VIMT LOB Service Account has acces to the va_pcrssn field, then use it 
                try
                {
                    if (string.IsNullOrEmpty(request.udo_User))
                    {
                        var crmUser = OrgServiceProxy.Retrieve("systemuser", request.UserId, new ColumnSet("va_pcrssn", "fullname", "va_filenumber"));

                        if (!crmUser.Contains("va_pcrssn"))
                        {
                            response.ExceptionOccurred = true;
                            response.ExceptionMessage = String.Format("The user ({0} : {1}) creating the note must have a participantId configured on their account.", crmUser.GetAttributeValue<string>("fullname"), crmUser.Id);
                            return response;
                        }
                        request.udo_User = crmUser["va_pcrssn"].ToString();
                    }

                    createNoteRequest.noteInfo.mcs_userId = request.udo_User;
                }
                catch (Exception ex)
                {
                    LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, method + " Processor, Progess:" + progressString, ex);
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method,
                        String.Format("Source: {0}\r\n\r\nError:{1}", ex.Source, ex.Message));
                }

                if (String.IsNullOrWhiteSpace(request.udo_ClaimId))
                {
                    createNoteRequest.noteInfo.mcs_ptcpntNoteTc = "CLMNTCONTACT";
                    createNoteRequest.noteInfo.mcs_noteOutTn = "Contact with Claimant";
                }
                else
                {
                    createNoteRequest.noteInfo.mcs_bnftClmNoteTc = "CLMDVLNOTE";
                    createNoteRequest.noteInfo.mcs_noteOutTn = "Claim Development Note";
                    createNoteRequest.noteInfo.mcs_clmId = request.udo_ClaimId;
                }

                if (request.LegacyServiceHeaderInfo != null)
                {
                    createNoteRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                }

                VEIScreatecreateNoteResponse createNoteResponse = null;

                try
                {
                    createNoteResponse = WebApiUtility.SendReceive<VEIScreatecreateNoteResponse>(createNoteRequest, WebApiType.VEIS);
                    if (request.LogSoap || createNoteResponse.ExceptionOccurred)
                    {
                        if (createNoteResponse.SerializedSOAPRequest != null || createNoteResponse.SerializedSOAPResponse != null)
                        {
                            var requestResponse = createNoteResponse.SerializedSOAPRequest + createNoteResponse.SerializedSOAPResponse;
                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEIScreatecreateNoteRequest Request/Response {requestResponse}", true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    var loginName = "Unknown WSLogin";
                    if (request.LegacyServiceHeaderInfo != null && !String.IsNullOrEmpty(request.LegacyServiceHeaderInfo.LoginName))
                    {
                        loginName = request.LegacyServiceHeaderInfo.LoginName;
                    }

                    var message = String.Format("WSLOGIN: {0}\r\nUSER PID:{1}\r\nCONTACT PID:{2}", loginName, request.udo_User, request.udo_ParticipantID);

                    // Exception occurred in sub call to dev notes
                    if (ex.Message.Contains("Password has expired"))
                    {
                        message = String.Format("ERROR: Unable to create note in MAPD.\r\nThe account ({0}) password has expired.\r\n{1}", loginName, message);
                        throw new Exception(message, ex);
                    }
                    else
                    {
                        message = String.Format("ERROR: Unable to create note in MAPD\r\n{0}", message);
                        throw new Exception(message, ex);
                    }
                }
                progressString = "After VIMT EC Call";

                response.ExceptionMessage = createNoteResponse.ExceptionMessage;
                response.ExceptionOccurred = createNoteResponse.ExceptionOccurred;

                if (createNoteResponse.VEIScreatecreatedNoteInfo != null)
                {
                    var createInfo = createNoteResponse.VEIScreatecreatedNoteInfo;
                    DateTime createDt = DateTime.SpecifyKind(DateTime.Parse(createInfo.mcs_createDt), DateTimeKind.Utc);

                    var userName = createInfo.mcs_jrnUserId.Trim();
                    if (!string.IsNullOrEmpty(createInfo.mcs_userNm))
                    {
                        userName = String.Format("{0} ({1})", createInfo.mcs_userNm.Trim(), userName);
                    }
                    if (String.IsNullOrEmpty(userName))
                    {
                        userName = "<Unknown User>";
                    }

                    response.UDOCreateNoteInfo = new UDOCreateNoteResponseInfo
                    {
                        udo_ClaimId = request.udo_ClaimId,
                        udo_DateTime = createInfo.mcs_createDt,
                        udo_Type = createInfo.mcs_noteOutTn,
                        udo_Note = createInfo.mcs_txt,
                        udo_RO = createInfo.mcs_jrnLctnId,
                        udo_User = userName,
                        udo_UserId = createInfo.mcs_ptcpntId,
                        udo_legacynoteid = createInfo.mcs_noteId
                    };

                    if (createInfo.mcs_suspnsDtSpecified)
                    {
                        response.UDOCreateNoteInfo.udo_SuspenseDate = DateTime.Parse(createInfo.mcs_suspnsDt).ToCRMDateTime();
                    }
                }

            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, method, $" Processor, Progess: {progressString}");
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, method, ExecutionException);
                response.ExceptionMessage = $"{method}: Failed to Process Notes Data";
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
    }
}