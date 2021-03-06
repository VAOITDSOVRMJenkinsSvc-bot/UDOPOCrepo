using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using VIMT.DevelopmentNotesService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Common;
using VRM.Integration.UDO.Notes.Messages;
using HeaderInfo = VIMT.DevelopmentNotesService.Messages.HeaderInfo;

/// <summary>
/// VIMT LOB Component for UDOCreateNote,CreateNote method, Processor.
/// Code Generated by IMS on: 6/27/2015 2:48:16 PM
/// Version: 2015.06.02
/// </summary>
/// <param name=none></param>
/// <returns>none</returns>
namespace VRM.Integration.UDO.Notes.Processors
{
    public class UDOCreateNoteProcessor
    {
        private bool _debug { get; set; }
        private string LogBuffer { get; set; }
        private const string method = "UDOCreateNoteProcessor";
        public IMessageBase Execute(UDOCreateNoteRequest request)
        {
            //var request = message as CreateNoteRequest;
            var response = new UDOCreateNoteResponse();
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }


            #region connect to CRM

            OrganizationServiceProxy OrgServiceProxy;
            try
            {
                var CommonFunctions = new CRMConnect();

                OrgServiceProxy = CommonFunctions.ConnectToCrm(request.OrganizationName);
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, string.Format("{0} Processor, Connection Error", method), connectException.Message);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }

            #endregion

            progressString = "After Connection";

            try
            {
                var createNoteRequest = new VIMTcreatecreateNoteRequest
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
                //else
                //{
                //    dtTime = DateTime.Now; //By getting the time as it is currently, this avoids time zone issues
                //}

                createNoteRequest.noteInfo = new VIMTcreatenote
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
                
                // If the VIMT LOB Service Account has acces to the va_pcrssn field, then use it 
                try
                {
                    //var crmUser = OrgServiceProxy.Retrieve("systemuser", request.UserId, new ColumnSet("va_pcrssn", "firstname", "lastname", "va_wsloginname"));
                    //OrgServiceProxy.CallerId = request.UserId;
                    

                    //#region If the wsloginname doesn't have a parens, prepend name to username for the entry
                    //var firstname = crmUser.Contains("firstname") ? crmUser["firstname"].ToString().ToUpper() : String.Empty;
                    //var lastname = crmUser.Contains("lastname") ? crmUser["lastname"].ToString().ToUpper() : String.Empty;

                    //var name = lastname + (firstname.Length > 0 ? ", " : "") + firstname;

                    //string loginname = string.Empty, createdby = string.Empty;
                    ////var createdby=string.Empty;
                    //if (crmUser.Contains("va_wsloginname")) loginname = crmUser["va_wsloginname"].ToString();

                    //if (loginname.Contains("("))
                    //{
                    //    createdby = loginname;
                    //}
                    //else
                    //{
                    //    createdby = " (" + loginname + ")";
                    //    if (name.Length + createdby.Length > 100)
                    //    {
                    //        name = name.Substring(0, name.Length - (name.Length + createdby.Length - 101));
                    //    }
                    //    name = name + createdby;
                    //}
                    //#endregion

                    OrgServiceProxy.CallerId = Guid.Empty;
                    if (string.IsNullOrEmpty(request.udo_User))
                    {
                        var crmUser = OrgServiceProxy.Retrieve("systemuser", request.UserId, new ColumnSet("va_pcrssn", "fullname")); //, "firstname", "lastname", "va_wsloginname"));
                        if (!crmUser.Contains("va_pcrssn"))
                        {
                            response.ExceptionOccured = true;
                            response.ExceptionMessage = String.Format("The user ({0} : {1}) creating the note must have a participantId configured on their account.", crmUser.GetAttributeValue<string>("fullname"), crmUser.Id);
                            return response;
                        }
                        request.udo_User = crmUser["va_pcrssn"].ToString();
                    }
                    createNoteRequest.noteInfo.mcs_userId = request.udo_User;  //pcrssn
                    OrgServiceProxy.CallerId = request.UserId;
                    //createNoteRequest.noteInfo.mcs_userNm = name;              //name of the user that created the record
                    //createNoteRequest.noteInfo.mcs_jrnUserId = loginname;
                }
                catch (Exception ex)
                {
                    LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ex);
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
                    createNoteRequest.LegacyServiceHeaderInfo = new HeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    };
                }

                VIMTcreatecreateNoteResponse createNoteResponse = null;

                try
                {
                    createNoteResponse =
                        createNoteRequest.SendReceive<VIMTcreatecreateNoteResponse>(MessageProcessType.Local);
                }
                catch (Exception ex)
                {
                    var loginName = "Unknown WSLogin";
                    if (request.LegacyServiceHeaderInfo != null && !String.IsNullOrEmpty(request.LegacyServiceHeaderInfo.LoginName))
                    {
                        loginName = request.LegacyServiceHeaderInfo.LoginName;
                    }

                    var message = String.Format("WSLOGIN: {0}\r\nUSER PID:{1}\r\nCONTACT PID:{2}",loginName, request.udo_User, request.udo_ParticipantID);

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
                response.ExceptionOccured = createNoteResponse.ExceptionOccured;

                if (createNoteResponse.VIMTcreatecreatedNoteInfo != null)
                {
                    var createInfo = createNoteResponse.VIMTcreatecreatedNoteInfo;

                    var userName = createInfo.mcs_jrnUserId.Trim();
                    if (!string.IsNullOrEmpty(createInfo.mcs_userNm))
                    {
                        userName = String.Format("{0} ({1})", createInfo.mcs_userNm.Trim(), userName);
                    }
                    if (String.IsNullOrEmpty(userName)) userName = "<Unknown User>";

                    var createDt = (createInfo.mcs_createDtSpecified) ?
                        createInfo.mcs_createDt : dtTime;

                    response.UDOCreateNoteInfo = new UDOCreateNoteResponseInfo
                    {
                        udo_ClaimId = request.udo_ClaimId,
                        udo_DateTime = createDt.ToCRMDateTime(), //createInfo.mcs_createDt,
                        udo_Type = createInfo.mcs_noteOutTn,
                        udo_Note = createInfo.mcs_txt,
                        udo_RO = createInfo.mcs_jrnLctnId,
                        udo_User = userName,
                        udo_UserId = createInfo.mcs_userId,
                        udo_legacynoteid = createInfo.mcs_noteId
                    };

                    if (createInfo.mcs_suspnsDtSpecified)
                    {
                        response.UDOCreateNoteInfo.udo_SuspenseDate = createInfo.mcs_suspnsDt.ToCRMDateTime();
                    }
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Notes Data";
                response.ExceptionOccured = true;
                return response;
            }
        }
    }
}