using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using VIMT.DevelopmentNotesService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Common;
using VRM.Integration.UDO.Notes.Messages;
using MPT = VRM.Integration.Servicebus.Core.MessageProcessType;


namespace VRM.Integration.UDO.Notes.Processors
{
    public class UDORetrieveNotesProcessor
    {
        private string method { get; set; }
        private TimeTracker timer { get; set; }
        private bool _debug { get; set; }
        private string LogBuffer { get; set; }

        public UDORetrieveNotesProcessor()
        {
            timer = new TimeTracker();
        }

        /// <summary>
        /// Execute VRM.Integration.UDO.Notes.UDORetrieveNotesProcessor in Async mode.
        /// </summary>
        /// <param name="request">Async Request</param>
        /// <returns></returns>
        public IMessageBase Execute(UDORetrieveNotesAsyncRequest request)
        {
            method = "UDORetrieveNotesAsyncRequest";

            #region Start Timer

            timer.Restart();

            #endregion

            #region build response and validate request is not null

            var response = new UDORetrieveNotesResponse();
            var progressString = "Top of Async Processor";

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }

            #endregion

            #region get OrgServiceProxy

            OrganizationServiceProxy OrgServiceProxy;
            try
            {
                var CommonFunctions = new CRMConnect();
                
                OrgServiceProxy = CommonFunctions.ConnectToCrm(request.OrganizationName);
                OrgServiceProxy.CallerId = request.UserId;
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

                var personRef = getRelated(request, "udo_person");

                if (request.End == -1) request.End = request.Notes.Length;
                timer.MarkStart("CreateNotes");
                var person = new Entity("udo_person");
                person["udo_personid"] = personRef.Id;

                if (request.isRelatedPerson)
                {
                    //Set to load complete because notes are loading
                    OrgServiceProxy.CallerId = request.UserId;
                    person = OrgServiceProxy.Retrieve("udo_person", personRef.Id, new ColumnSet("udo_personid", "udo_notesstatus"));
                    if (person.Contains("udo_notesstatus") &&
                        ((OptionSetValue)person["udo_notesstatus"]).Value != 752280000)
                    {
                        // We are already loading the notes or have already...
                        return response;
                    }
                    person["udo_notesstatus"] = new OptionSetValue(752280001);
                    person["udo_notesloadcomplete"] = true;
                    OrgServiceProxy.CallerId = Guid.Empty;
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(person, request.OrganizationName, request.UserId, request.LogTiming));
                }

                var notes = request.Notes;
                if (notes == null)
                {
                    if (request.isRelatedPerson)
                    {
                        notes = getNotes(request);
                    }
                }

                OrgServiceProxy.CallerId = Guid.Empty;
                var createResponse = CreateNotes(OrgServiceProxy, request, notes, request.Start, request.End, person, request.CurrentPcrSSN);
                if (createResponse.ExceptionOccured) response = createResponse;
                timer.MarkStop("CreateNotes", "Create Notes: {0} to {1}", request.Start, request.End);

                #region Stop Timer
                if (request.LogTiming)
                {
                    LogHelper.LogTiming(request.OrganizationName, request.Debug, request.UserId,
                        request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method,
                        method,
                        Convert.ToDecimal(timer.ElapsedMilliseconds));
                }
                if (request.Debug) {
                    timer.LogDurations(request.OrganizationName, request.Debug, request.UserId, method, true);
                }
                timer.Stop();
                #endregion

                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Retrieve Notes Data";
                response.ExceptionOccured = true;
                return response;
            }
        }

        /// <summary>
        /// Execute VRM.Integration.UDO.Notes.UDORetrieveNotesProcessor in synchronously mode.
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns></returns>
        public IMessageBase Execute(UDORetrieveNotesRequest request)
        {
            method = "UDORetrieveNotesProcessor";
            int syncLimit = request.LoadSize;  // 100 is one page.
            if (syncLimit == 0) syncLimit = 100;

            #region Start Timer

            timer.Restart();

            #endregion

            #region build response and validate request is not null

            var response = new UDORetrieveNotesResponse();
            var progressString = "Top of Processor";

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }

            #endregion

            #region get OrgServiceProxy

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
                var currentPcrSSN = String.Empty;
                // If the VIMT LOB Service Account has acces to the va_pcrssn field, then use it 
                var crmUser = OrgServiceProxy.Retrieve("systemuser", request.UserId, new ColumnSet("va_pcrssn"));
                if (crmUser.Contains("va_pcrssn"))
                {
                    currentPcrSSN = crmUser["va_pcrssn"].ToString().Trim();
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method, "Current PCR SSN: " + currentPcrSSN);
                }

                // Get person reference
                var personRef = getRelated(request, "udo_person");

                //Set to load complete because notes are loading
                var person = OrgServiceProxy.Retrieve("udo_person", personRef.Id, new ColumnSet("udo_personid", "udo_notesstatus"));
                
                if (person.Contains("udo_notesstatus") &&
                    ((OptionSetValue)person["udo_notesstatus"]).Value != 752280000)
                {
                    // We are already loading the notes or have already...
                    return response;
                }
                person["udo_notesstatus"] = new OptionSetValue(752280001);
                person["udo_notesloadcomplete"] = true;
                OrgServiceProxy.CallerId = request.UserId;
                OrgServiceProxy.Update(TruncateHelper.TruncateFields(person, request.OrganizationName, request.UserId, request.LogTiming));


                // There should be no notes there, but sometimes, notes are created 
                // by other events and they need to be flushed from crm.
                timer.MarkStart("deleteNotes");
                OrgServiceProxy.CallerId = request.UserId;
                deleteNotes(request, OrgServiceProxy, person.Id);
                timer.MarkStop("deleteNotes");

                #region findDevelopmentNotes
                timer.MarkStart("findDevelopmentNotesResponse");
                // prefix = finddevfindDevelopmentNotesRequest();

                var notes = getNotes(request);

                if (notes == null || notes.Length == 0)
                {
                    person["udo_notesstatus"] = new OptionSetValue(752280002); //complete
                    OrgServiceProxy.CallerId = request.UserId;
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(person, request.OrganizationName, request.UserId, request.LogTiming));
                    response.RecordCount = 0;
                    return response;
                }

                timer.MarkStop("findDevelopmentNotesResponse");

                #endregion

                #region Sort Notes

                // Sort notes descending by mcs_createDt
                Array.Sort(notes,
                    (x, y) => y.mcs_createDt.CompareTo(x.mcs_createDt));
                timer.MarkStop("Sort Notes");

                #endregion
                OrgServiceProxy.CallerId = request.UserId;
                var createResponse = CreateNotes(OrgServiceProxy, request, notes, 0, syncLimit, person, currentPcrSSN);
                if (createResponse.ExceptionOccured) response = createResponse;
                timer.MarkStop("CreateNotes", String.Format("Create Notes: 1 to {0} notes", syncLimit));

                #region Asyncrhonously Create Notes
                if (notes.Length > syncLimit)
                {
                    //Build Async Request
                    var asyncRequest = new UDORetrieveNotesAsyncRequest(request)
                    {
                        Start = syncLimit,
                        End = -1,
                        Notes = notes,
                        CurrentPcrSSN = currentPcrSSN,
                        isRelatedPerson = false,
                        MessageId = Guid.NewGuid().ToString()
                    };


                    //MessageProcessType.Local - Don't want it changed by a replace.
                    asyncRequest.SendAsync(MessageProcessType.Local);  //TODO: Defect 238062
                    timer.MarkStop("Sending Async Create Notes");
                }
                #endregion

                // Set total count
                response.RecordCount = notes.Length;

                // Log Count
                //var logInfo = string.Format("Number of Note Retrieved: {0}", response.RecordCount);
                //LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, method, logInfo);

                #region Stop Timer

                LogHelper.LogTiming(request.OrganizationName, request.Debug, request.UserId,
                    request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method,
                    method,
                    Convert.ToDecimal(timer.ElapsedMilliseconds));
                timer.LogDurations(request.OrganizationName, request.Debug, request.UserId, method, true);

                #endregion

                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process/Retrieve Notes Data"; 
                timer.LogDurations(request.OrganizationName, request.Debug, request.UserId, method, true);
                response.ExceptionOccured = true;
                return response;
            }
        }

        private VIMTfinddevfoundNoteMultipleResponse[] getNotes(UDORetrieveNotesRequest request)
        {
            var findDevelopmentNotesRequest = new VIMTfinddevfindDevelopmentNotesRequest();
            findDevelopmentNotesRequest.LogTiming = request.LogTiming;
            findDevelopmentNotesRequest.LogSoap = request.LogSoap;
            findDevelopmentNotesRequest.Debug = request.Debug;
            findDevelopmentNotesRequest.RelatedParentEntityName = request.RelatedParentEntityName;
            findDevelopmentNotesRequest.RelatedParentFieldName = request.RelatedParentFieldName;
            findDevelopmentNotesRequest.RelatedParentId = request.RelatedParentId;
            findDevelopmentNotesRequest.UserId = request.UserId;
            findDevelopmentNotesRequest.OrganizationName = request.OrganizationName;

            findDevelopmentNotesRequest.mcs_ptcpntid = request.ptcpntId;
            findDevelopmentNotesRequest.mcs_claimid = request.claimid;

            var findDevelopmentNotesResponse =
                findDevelopmentNotesRequest.SendReceive<VIMTfinddevfindDevelopmentNotesResponse>(
                    MessageProcessType.Local);

            //response.ExceptionMessage = findDevelopmentNotesResponse.ExceptionMessage;
            //response.ExceptionOccured = findDevelopmentNotesResponse.ExceptionOccured;
            if (findDevelopmentNotesResponse.ExceptionOccured)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, method + ", getNotes",
                    new Exception(findDevelopmentNotesResponse.ExceptionMessage));
            }
            if (findDevelopmentNotesResponse.VIMTfinddevfoundNoteInfo == null)
            {
                return null;
            }
            var notes = findDevelopmentNotesResponse.VIMTfinddevfoundNoteInfo;
            return notes;
        }
        private EntityReference getRelated(UDORetrieveNotesRequest request, string logicalname)
        {
            foreach (var related in request.RelatedEntities)
            {
                //new EntityReference(related.RelatedEntityName, related.RelatedEntityId);
                if (related.RelatedEntityName.ToLower().Equals(logicalname.ToLower()))
                {
                    return new EntityReference(related.RelatedEntityName, related.RelatedEntityId);
                }
            }
            return null;
        }

        private void deleteNotes(UDORetrieveNotesRequest request, OrganizationServiceProxy OrgServiceProxy, Guid personId)
        {
            var method = this.method + ".deleteNotes";
            var progressString = "Deleting Notes";

            //var queryPersonsNotes = new QueryExpression("udo_note");
            //queryPersonsNotes.Criteria = new FilterExpression();
            //queryPersonsNotes.Criteria.AddCondition("udo_personid", ConditionOperator.Equal, personId);
            //queryPersonsNotes.ColumnSet = new ColumnSet("udo_noteid");

            var queryPersonsNotes = new FetchExpression(
                          @"<fetch><entity name='udo_note'><attribute name='udo_noteid' /><filter type='and'>" +
                          @"<condition attribute='udo_personid' operator='eq' value='" + personId.ToString() + "' />" +
                          @"</filter></entity></fetch>");

            try
            {
                EntityCollection notes;
                do
                {
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method, "Before Query");
                    notes = OrgServiceProxy.RetrieveMultiple(queryPersonsNotes);
                    var deleteNotes = new OrganizationRequestCollection();
                    foreach (var note in notes.Entities)
                    {
                        var delRequest = new OrganizationRequest
                        {
                            RequestName = "udo_note_ForceDelete"
                        };
                        delRequest.Parameters.Add("Target", note.ToEntityReference());
                        delRequest.Parameters.Add("AuthorizationKey", "udo_delete_now");
                        LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method,
                            "New Delete Request: udo_note_ForceDelete\r\n" +
                            "Target: " + note.Id.ToString() + "\r\n" +
                            "AuthorizationKey: udo_delete_now");
                        deleteNotes.Add(delRequest);
                    }
                    OrgServiceProxy.CallerId = Guid.Empty;

                    //LogHelper.LogDebug(request.OrganizationName, true, request.UserId, method, String.Format("{0} deleteRequest are being sent to execute multiple", deleteNotes.Count));
                    ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, deleteNotes, request.OrganizationName, request.UserId, request.Debug, true, 100);
                } while (notes.MoreRecords);
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ExecutionException);
                // do nothing... we still need to continue if there was an error, so just log it and move on.
            }
        }

        /// <summary>
        /// Create the Notes in CRM found in the notes(start to end).
        /// </summary>
        /// <param name="orgServiceProxy">A Connected CRM Organization Service Proxy</param>
        /// <param name="request">The Request</param>
        /// <param name="notes">An Array of Notes from findNotes</param>
        /// <param name="start">Start Position</param>
        /// <param name="end">End Position (-1 to go until end of notes)</param>
        private UDORetrieveNotesResponse CreateNotes(OrganizationServiceProxy orgServiceProxy, UDORetrieveNotesRequest request,
            VIMTfinddevfoundNoteMultipleResponse[] notes, int start, int end, Entity person, string currentPcrSSN)
        {
            var response = new UDORetrieveNotesResponse();

            var method = this.method + ".CreateNotes";
            var loadedAllNotes = false;
            var progressString = "Creating Notes";

            try
            {
                //var owner = request.owner;
                EntityReference owner = null;
                if (request.OwnerId.HasValue && request.OwnerId.Value != Guid.Empty)
                {
                    owner = new EntityReference(request.OwnerType, request.OwnerId.Value);
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "Create Notes",
                    String.Format("CurrentPcrSSN: {0}\r\nOwner: {0} ({1})", currentPcrSSN, owner.Id, owner.LogicalName));
                }
                else
                {
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "Create Notes",
                    String.Format("CurrentPcrSSN: {0}\r\nOwner: Not specified", currentPcrSSN));
                }

                var creates = new ConcurrentBag<CreateRequest>();
                try
                {
                    if (end == -1 || end >= notes.Length)
                    {
                        end = notes.Length;
                        loadedAllNotes = true;
                    }

                    Parallel.For(start, end, (i) =>
                    {
                        var noteItem = notes[i];
                        var noteEntity = ConvertToEntity(request, notes[i], owner, currentPcrSSN);
                        if (noteEntity != null)
                        {
                            creates.Add(new CreateRequest { Target = noteEntity });
                        }
                    });
                }
                catch (AggregateException ae)
                {
                    foreach (var ex in ae.InnerExceptions)
                    {
                        LogHelper.LogError(request.OrganizationName, request.UserId, "CreateNotes", ex);
                    }
                }
                timer.MarkStop("Build Create Requests");

                progressString = "ExecuteMultiple";

                var emResponse = ExecuteMultipleHelper.ExecuteMultiple(orgServiceProxy, creates, request.OrganizationName, request.UserId, request.Debug);

                if (request.Debug)
                {
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method, emResponse.LogDetail);
                }
                if (emResponse.IsFaulted)
                {
                    LogHelper.LogError(request.OrganizationName, request.UserId, method, emResponse.ErrorDetail);
                    
                    response.ExceptionMessage = emResponse.FriendlyDetail;
                    response.ExceptionOccured = true;
                    return response; 
                }

                creates = null;

                var updates = new List<UpdateRequest>();
                if (loadedAllNotes)
                {
                    if (person.Contains("udo_notesstatus"))
                    {
                        person["udo_notesstatus"] = new OptionSetValue(752280002);  // Loaded all
                    }
                    else
                    {
                        person.Attributes.Add("udo_notesstatus", new OptionSetValue(752280002));
                    }
                    orgServiceProxy.Update(TruncateHelper.TruncateFields(person, request.OrganizationName, request.UserId, request.LogTiming));
                }

                timer.MarkStop("Execute Multiple");
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ExecutionException);
            }

            return response;
        }

        /// <summary>
        /// Convert a noteItem from findNotes to a CRM udo_note Entity.
        /// </summary>
        /// <param name="request">The Request</param>
        /// <param name="noteItem">The note object from findNotes</param>
        /// <returns></returns>
        private Entity ConvertToEntity(UDORetrieveNotesRequest request, VIMTfinddevfoundNoteMultipleResponse noteItem, EntityReference owner, string currentPcrSSN)
        {
            var progressString = "Converting Note to CRM Entity";
            var method = this.method + ".ConvertToEntity";

            //instantiate the new Entity
            var newEntity = new Entity("udo_note");
            newEntity["udo_name"] = "Note Detail";
            newEntity["udo_fromudo"] = false;
            newEntity["ownerid"] = owner;

            try
            {
                if (!string.IsNullOrEmpty(noteItem.mcs_clmId))
                {
                    newEntity["udo_claimid"] = noteItem.mcs_clmId.Truncate(100);
                }
                if (noteItem.mcs_createDtSpecified)
                {
                    var createDate = noteItem.mcs_createDt.ToCRMDateTime();
                    if (!noteItem.mcs_createDt.Equals(createDate))
                    {
                        newEntity["udo_datetime"] = noteItem.mcs_createDt.ToString();
                        newEntity["udo_createdtstring"] = noteItem.mcs_createDt.ToString("yyyy-MM-ddTHH:mm:ss-4:00");
                    }
                    newEntity["udo_dttime"] = createDate;
                    newEntity["udo_createdt"] = createDate;

                    var last24hours = (createDate != null && createDate >= DateTime.Now.AddHours(-24));

                    var notePcr = (noteItem.mcs_userId ?? string.Empty).Trim();
                    var sameuser = currentPcrSSN.Equals(notePcr, StringComparison.InvariantCultureIgnoreCase);

                    newEntity["udo_editable"] = last24hours && sameuser;
                }
                else
                {
                    newEntity["udo_dttime"] = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                }
                if (!string.IsNullOrEmpty(noteItem.mcs_txt))
                {
                    newEntity["udo_notetext"] = noteItem.mcs_txt.Truncate(99000);
                }
                if (!string.IsNullOrEmpty(noteItem.mcs_ptcpntId))
                {
                    newEntity["udo_participantid"] = noteItem.mcs_ptcpntId.Truncate(100);
                }
                if (noteItem.mcs_suspnsDtSpecified)
                {
                    newEntity["udo_suspensedate"] = noteItem.mcs_suspnsDt.ToCRMDateTime();
                }
                if (!string.IsNullOrEmpty(noteItem.mcs_noteOutTn))
                {
                    newEntity["udo_type"] = noteItem.mcs_noteOutTn.Truncate(100);
                }
                var userName = noteItem.mcs_jrnUserId.Trim();

                if (!string.IsNullOrEmpty(noteItem.mcs_userNm))
                {
                    userName = String.Format("{0} ({1})", noteItem.mcs_userNm.Trim(), userName);
                }
                if (String.IsNullOrEmpty(userName)) userName = "<Unknown User>";
                newEntity["udo_user"] = userName.Truncate(100);

                if (!string.IsNullOrEmpty(noteItem.mcs_userId))
                {
                    newEntity["udo_userid"] = noteItem.mcs_userId.Truncate(100);
                }

                if (!string.IsNullOrEmpty(noteItem.mcs_jrnLctnId))
                {
                    newEntity["udo_ro"] = noteItem.mcs_jrnLctnId.Truncate(100);
                }
                if (request.RelatedEntities != null)
                {
                    foreach (var relatedItem in request.RelatedEntities)
                    {
                        newEntity[relatedItem.RelatedEntityFieldName] =
                            new EntityReference(relatedItem.RelatedEntityName,
                                relatedItem.RelatedEntityId);
                    }
                }
                if (!string.IsNullOrEmpty(noteItem.mcs_noteId))
                {
                    newEntity["udo_legacynoteid"] = noteItem.mcs_noteId.Truncate(100);
                }
                if (!newEntity.Contains("udo_user"))
                {
                }
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ExecutionException);
            }
            return newEntity;
        }
    }
}