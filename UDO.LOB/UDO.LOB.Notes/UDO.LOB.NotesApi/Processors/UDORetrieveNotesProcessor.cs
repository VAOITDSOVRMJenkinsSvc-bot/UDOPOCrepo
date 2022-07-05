using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.Notes.Messages;
using VEIS.Core.Messages;
using VEIS.Messages.DevelopmentNotesService;

namespace UDO.LOB.Notes.Processors
{
    public class UDORetrieveNotesProcessor
    {
        private string method { get; set; }
        private TimeTracker timer { get; set; }
        private bool _debug { get; set; }
        private string LogBuffer { get; set; }
        private struct DupeCheck
        {
            public string date;
            public string noteText;
            public string ptcpntId;
        }


        public UDORetrieveNotesProcessor()
        {
            timer = new TimeTracker();
        }

        /// <summary>
        /// Execute VRM.Integration.UDO.Notes.UDORetrieveNotesProcessor in synchronously mode.
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns></returns>
        public IMessageBase Execute(UDORetrieveNotesRequest request)
        {
            method = "UDORetrieveNotesProcessor";
            LogHelper.LogInfo($">> Entered {method} for UDORetrieveNotesRequest {request.MessageId}");
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
                response.ExceptionOccurred = true;
                return response;
            }
            TraceLogger tLogger = new TraceLogger(method, request);

            LogHelper.LogInfo($">> {this.GetType().FullName}.Execute: Request:  {JsonHelper.Serialize<UDORetrieveNotesRequest>(request)}");

            _debug = request.Debug;
            LogBuffer = string.Empty;

            #endregion

            #region connect to CRM
            CrmServiceClient OrgServiceProxy = null;

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
            LogHelper.LogInfo($"After Connection.");

            try
            {
                var currentPcrSSN = String.Empty;
                // If the VIMT LOB Service Account has acces to the va_pcrssn field, then use it 
                var crmUser = OrgServiceProxy.Retrieve("systemuser", request.UserId, new ColumnSet("va_pcrssn"));
                LogHelper.LogInfo($"CRM User: {crmUser.Id}");

                if (crmUser.Contains("va_pcrssn"))
                {
                    LogHelper.LogInfo($"CRM User va_pcrssn: {crmUser.Attributes["va_pcrssn"]} ");

                    currentPcrSSN = crmUser["va_pcrssn"].ToString().Trim();
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method, "Current PCR SSN: " + currentPcrSSN);
                }

                #region Get Related
                LogHelper.LogInfo(">> Entering getRelated");
                // Get person reference
                var personRef = getRelated(request, "udo_person");
                LogHelper.LogInfo($"Person Ref: {personRef.Id} ");

                //Set to load complete because notes are loading
                var person = OrgServiceProxy.Retrieve("udo_person", personRef.Id, new ColumnSet("udo_personid", "udo_notesstatus", "udo_name"));
                LogHelper.LogInfo($"Person: {person["udo_name"]} with Guid of, {person.Id}");

                if (person.Contains("udo_notesstatus") &&
                    ((OptionSetValue)person["udo_notesstatus"]).Value != 752280000)
                {
                    LogHelper.LogInfo($"We are already loading the notes or have already...");

                    // We are already loading the notes or have already...
                    LogHelper.LogInfo($"Logging response: {response}");
                    return response;
                }
                person["udo_notesstatus"] = new OptionSetValue(752280001);
                person["udo_notesloadcomplete"] = true;
                LogHelper.LogInfo($"Params passing to update CRM: {person.Id}, {request.OrganizationName}, {request.UserId}, {request.LogTiming}");

                OrgServiceProxy.Update(TruncateHelper.TruncateFields(request.MessageId, person, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                LogHelper.LogInfo($"Person updated and truncated if necessary.");
                LogHelper.LogInfo("<< Exiting getRelated");
                #endregion Get Related

                #region Delete Existing Notes
                // There should be no notes there, but sometimes, notes are created 
                // by other events and they need to be flushed from crm.
                LogHelper.LogInfo($">> Entering deleteNotes");
                timer.MarkStart("deleteNotes");
                deleteNotes(request, OrgServiceProxy, person.Id);
                timer.MarkStop("deleteNotes");
                LogHelper.LogInfo($"<< Exiting deleteNotes");
                #endregion Delete Existing Notes

                #region findDevelopmentNotes
                timer.MarkStart("findDevelopmentNotesResponse");
                LogHelper.LogInfo(">> Entering UDORetreivenotesProcessor.getNotes.");
                var notes = getNotes(request);

                if (notes == null || notes.Length == 0)
                {
                    person["udo_notesstatus"] = new OptionSetValue(752280002); //complete
                    LogHelper.LogInfo($"Truncate.TruncateFields Params: {person.Id}, {request.OrganizationName}, {request.UserId}, {request.LogTiming}");
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(request.MessageId, person, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                    response.RecordCount = 0;
                    return response;
                }

                timer.MarkStop("findDevelopmentNotesResponse");
                LogHelper.LogInfo("<< Exiting UDORetreivenotesProcessor.getNotes.");
                #endregion

                #region Sort Notes
                LogHelper.LogInfo(">> Entering Sort Notes logic.");
                timer.MarkStop("Sort Notes");
                LogHelper.LogInfo("<< Exiting Sort Notes logic.");
                #endregion

                #region Create Notes
                LogHelper.LogInfo(">> Entering Create Notes logic.");
                var createResponse = CreateNotes(OrgServiceProxy, syncLimit, request, notes, person, currentPcrSSN);
                LogHelper.LogInfo($"Create Notes Response: {createResponse}");
                if (createResponse.ExceptionOccurred) response = createResponse;
                timer.MarkStop("CreateNotes", String.Format("Create Notes: 1 to {0} notes", syncLimit));

                LogHelper.LogInfo("<< Exiting Create Notes logic.");
                #endregion Create Notes
                // Set total count
                response.RecordCount = notes.Length;

                #region Stop Timer

                LogHelper.LogTiming(request.OrganizationName, request.LogTiming, request.UserId,
                    request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method,
                    method, timer.ElapsedMilliseconds);
                #endregion
                LogHelper.LogInfo($"<< Exited {method} for UDORetrieveNotesRequest {request.MessageId} for user {request.UserId} Elapsed Time: {timer.ElapsedMilliseconds} (ms)");
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, method + " Processor, Progess:" + progressString, ExecutionException);

                LogHelper.LogTiming(request.OrganizationName, request.LogTiming, request.UserId,
                   request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method,
                   method, timer.ElapsedMilliseconds);

                response.ExceptionMessage = "Failed to Process/Retrieve Notes Data";

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

        private VEISfinddevnoteMultipleResponse[] getNotes(UDORetrieveNotesRequest request)
        {
            LogHelper.LogInfo($">> Entering getNotes to return VEISfinddevnoteMultipleReponse[].");
            var findDevelopmentNotesRequest = new VEISfinddevfindDevelopmentNotesRequest();
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
            if (request.LegacyServiceHeaderInfo != null)
            {
                LogHelper.LogInfo($"Legacy Header Info is not null. Setting it on VEISReq Object");
                findDevelopmentNotesRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo()
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
            }
            else
            {
                LogHelper.LogInfo("Legacy Header Info is  null.");
            }

            var findDevelopmentNotesResponse = WebApiUtility.SendReceiveNotes<VEISfinddevfindDevelopmentNotesResponse>(findDevelopmentNotesRequest, WebApiType.VEIS);

            if (findDevelopmentNotesResponse.ExceptionOccurred)
            {
                LogHelper.LogInfo("Find development Notes Response received an error.");
                LogHelper.LogError(request.OrganizationName, request.UserId, method + ", getNotes",
                    new Exception(findDevelopmentNotesResponse.ExceptionMessage));
            }
            if (findDevelopmentNotesResponse.VEISfinddevnoteInfo == null)
            {
                LogHelper.LogInfo("Find development Notes Response returned null.");
                return null;
            }
            var notes = findDevelopmentNotesResponse.VEISfinddevnoteInfo;

            return notes;
        }

        private EntityReference getRelated(UDORetrieveNotesRequest request, string logicalname)
        {
            LogHelper.LogInfo($"Get Related Method Params: {request.LegacyServiceHeaderInfo}, {logicalname}");

            foreach (var related in request.RelatedEntities)
            {
                LogHelper.LogInfo($"Related Person: {related.RelatedEntityFieldName}");

                if (related.RelatedEntityName.ToLower().Equals(logicalname.ToLower()))
                {
                    LogHelper.LogInfo($"Related Person: {related.RelatedEntityId}");

                    return new EntityReference(related.RelatedEntityName, related.RelatedEntityId);
                }
            }
            return null;
        }

        private void deleteNotes(UDORetrieveNotesRequest request, IOrganizationService OrgServiceProxy, Guid personId)
        {
            var method = this.method + ".deleteNotes";
            var progressString = "Deleting Notes";
            LogHelper.LogInfo($"Deleting Notes");

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
                    LogHelper.LogInfo($"Notes EC to be deleted: {notes.TotalRecordCount.ToString()}");
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
                    LogHelper.LogInfo($"Deleting Notes completed.");

                    ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, deleteNotes, request.OrganizationName, Guid.Empty, request.Debug, true, 100);
                } while (notes.MoreRecords);
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, method + " Processor, Progess:" + progressString, ExecutionException);
                // do nothing... we still need to continue if there was an error, so just log it and move on.
            }
        }

        /// <summary>
        /// Create the Notes in CRM found in the notes(start to end).
        /// </summary>
        /// <param name="orgServiceProxy">A Connected CRM Organization Service Proxy</param>
        /// <param name="request">The Request</param>
        /// <param name="notes">An Array of Notes from findNotes</param>
        private UDORetrieveNotesResponse CreateNotes(IOrganizationService orgServiceProxy, int synclimit, UDORetrieveNotesRequest request, VEISfinddevnoteMultipleResponse[] notes, Entity person, string currentPcrSSN)
        {
            LogHelper.LogInfo($">> Initiating CreateNotes to retrieve UDORetrieveNotesResponse. CreateNotes Params = Response: {notes.Length}, {person.Id}, {currentPcrSSN}");
            string method = this.method + ".CreateNotes";
            var progressString = "Creating Notes";

            try
            {
                EntityReference owner = null;
                if (request.OwnerId.HasValue && request.OwnerId.Value != Guid.Empty)
                {
                    LogHelper.LogInfo($"Request OwnerId has value: {request.OwnerId.HasValue}");
                    owner = new EntityReference(request.OwnerType, request.OwnerId.Value);
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "Create Notes",
                    String.Format("CurrentPcrSSN: {0}\r\nOwner: {0} ({1})", currentPcrSSN, owner.Id, owner.LogicalName));
                }
                else
                {
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "Create Notes",
                    $"CurrentPcrSSN: {currentPcrSSN}\r\nOwner: Not specified");
                }

                try
                {
                    LogHelper.LogInfo($"Total Notes to be Created for {person["udo_name"]}: {notes.Length}");



                    // AsParallel - ran in parallel
                    // AsOrdered - rebuilt in order
                    var creates = (from noteItem in notes.AsParallel()
                                   orderby noteItem.mcs_createDt descending
                                   select ConvertToRequest(request, noteItem, owner, currentPcrSSN, person));

                    LogHelper.LogInfo($"Sorted Request Collection Built # of Requests: " + creates.Count());

                    if (creates.Count() > 0)
                    {
                        var r1 = ExecuteMultipleHelper.ExecuteMultiple(orgServiceProxy, creates.Take(synclimit), request.OrganizationName, Guid.Empty, request.Debug);

                        if (synclimit < creates.Count())
                        {
                            var r2 = ExecuteMultipleHelper.ExecuteMultiple(orgServiceProxy, creates.Skip(synclimit), request.OrganizationName, Guid.Empty, request.Debug);
                        }
                        LogHelper.LogInfo($"Created {creates.Count()} notes for {person["udo_name"]}");

                    }

                }
                catch (AggregateException ae)
                {
                    foreach (var ex in ae.InnerExceptions)
                    {
                        LogHelper.LogError(request.OrganizationName, request.UserId, "CreateNotes", ex);
                    }
                }
                timer.MarkStop("Build Create Requests");

                person["udo_notesstatus"] = new OptionSetValue(752280002);

                orgServiceProxy.Update(TruncateHelper.TruncateFields(request.MessageId, person, request.OrganizationName, request.UserId, request.LogTiming, orgServiceProxy));
                LogHelper.LogInfo($"Upated the 'Notes Status' on Person Record, {person["udo_name"]}");

                timer.MarkStop("Execute Multiple");
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, method + " Processor, Progess:" + progressString, ExecutionException);
            }

            return new UDORetrieveNotesResponse(); ;
        }

        private CreateRequest ConvertToRequest(UDORetrieveNotesRequest request, VEISfinddevnoteMultipleResponse noteItem, EntityReference owner, string currentPcrSSN, Entity person)
        {
            Entity noteEntity = ConvertToEntity(request, noteItem, owner, currentPcrSSN);
            noteEntity["udo_personid"] = person.ToEntityReference();
            return new CreateRequest() { Target = noteEntity };
        }

        /// <summary>
        /// Convert a noteItem from findNotes to a CRM udo_note Entity.
        /// </summary>
        /// <param name="request">The Request</param>
        /// <param name="noteItem">The note object from findNotes</param>
        /// <returns></returns>
        private Entity ConvertToEntity(UDORetrieveNotesRequest request, VEISfinddevnoteMultipleResponse noteItem, EntityReference owner, string currentPcrSSN)
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
                    DateTime createDate = DateTime.SpecifyKind(noteItem.mcs_createDt, DateTimeKind.Utc);
                    if (!noteItem.mcs_createDt.Equals(createDate))
                    {
                        newEntity["udo_datetime"] = createDate;
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
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, method + " Processor, Progess:" + progressString, ExecutionException);
            }
            return newEntity;
        }
    }
}