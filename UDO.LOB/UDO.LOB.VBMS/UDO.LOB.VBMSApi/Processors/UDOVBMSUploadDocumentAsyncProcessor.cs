#region Using Directives
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.Notes.Messages;
using UDO.LOB.VBMS.Messages;
using VEIS.Core.Messages;
using VEIS.VBMSWebService.Messages;
#endregion

namespace UDO.LOB.VBMS.Processors
{
    public class ResponseData : MessageBase
    {
        public string ExceptionMessage { get; set; }
        public bool ExceptionOccurred { get; set; }
        public string udo_vbmsdcsid { get; set; }
        public string udo_vbmscategory { get; set; }
        public string udo_vbmsfilename { get; set; }
        public DateTime udo_vbmsuploaddate { get; set; }
        public bool udo_uploaded { get; set; }
        public string udo_uploadmessage { get; set; }
        public Guid? udo_doctypeid { get; set; }
    }

    public class UDOVBMSUploadDocumentAsyncProcessor
    {
        private bool _debug { get; set; }

        private static string _source = "CRM";

        private string _method { get; set; }
        private string _progresssString;
        private StringBuilder _log { get; set; }

        private Stopwatch _txnTimer;
        private Stopwatch _EntireTimer;
        private StringBuilder _logData;
        private StringBuilder _logTimerData;
        private string _token = String.Empty;
        private string _documentCategory { get; set; }

        public string progressString
        {
            get { return _progresssString; }
            set
            {
                _progresssString = value;
                if (_log != null) _log.AppendFormat("** Updating Progress: {0}\r\n", this._progresssString);
            }
        }

        public IMessageBase Execute(UDOVBMSUploadDocumentAsyncRequest request)
        {
            this._method = "UDOVBMSUploadDocumentAsyncProcessor";
            _txnTimer = Stopwatch.StartNew();
            _EntireTimer = Stopwatch.StartNew();
            _logData = new StringBuilder();
            _logTimerData = new StringBuilder();
            _log = new StringBuilder();
            progressString = "Top of Process";

            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, this._method, "UDOVBMSUploadDocumentAsyncProcessor - Uploading document to VBMS.", request.Debug);

            UDOVBMSUploadDocumentResponse vbmsUploadDocResponse = new UDOVBMSUploadDocumentResponse { MessageId = request.MessageId };

            if (request == null)
            {
                vbmsUploadDocResponse.ExceptionMessage = "Called with no message";
                vbmsUploadDocResponse.ExceptionOccurred = true;
                return vbmsUploadDocResponse;
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

            this._debug = request.Debug;
            LogHelper.LogInfo(request.OrganizationName, true, request.UserId, this._method, $"Request Debug on?  {_debug}");

            if (request.udo_filename.Substring(request.udo_filename.Length - 4).ToLower() != ".pdf")
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, this._method, "VBMS Document is not a PDF and it must be a PDF.");
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, this._method, "Document is not a pdf.", this._debug);
                throw new ArgumentException("Document is not a pdf.");
            }

            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = this._method,
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
                };
            }

            #region connect to CRM
            CrmServiceClient OrgServiceProxy = null;

            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, this._method, connectException);
                vbmsUploadDocResponse.ExceptionMessage = "Failed to get CRMConnection";
                vbmsUploadDocResponse.ExceptionOccurred = true;

                return vbmsUploadDocResponse;
            }
            #endregion

            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                VEISeFolderuploadDocumentResponse eFolderUploadDocResponse = UploadDocument((IOrganizationService)OrgServiceProxy, request);
                stopwatch.Stop();
                _logTimerData.AppendLine("UploadDocument:" + stopwatch.ElapsedMilliseconds);

                vbmsUploadDocResponse.udo_vbmscategory = this._documentCategory;
                vbmsUploadDocResponse.udo_doctypeid = request.udo_doctypeid;
                vbmsUploadDocResponse.udo_vbmsfilename = request.udo_filename;
                vbmsUploadDocResponse.mcs_errorTypeField = eFolderUploadDocResponse.mcs_errorTypeField;
                vbmsUploadDocResponse.mcs_errorCode = eFolderUploadDocResponse.mcs_errorCode;
                vbmsUploadDocResponse.mcs_errorId = eFolderUploadDocResponse.mcs_errorId;

                if (!eFolderUploadDocResponse.ExceptionOccurred && eFolderUploadDocResponse.VEISeFolderuploadDocumentResponseDataInfo != null)
                {
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, this._method, "VBMS Upload Successful", this._debug);

                    VEISeFolderuploadDocumentResponseData eFolderUploadResponseDataInfo = eFolderUploadDocResponse.VEISeFolderuploadDocumentResponseDataInfo;
                    vbmsUploadDocResponse.udo_vbmsdcsid = eFolderUploadResponseDataInfo.mcs_documentSeriesRefId;
                    vbmsUploadDocResponse.udo_doctypeid = request.udo_doctypeid;
                    vbmsUploadDocResponse.udo_vbmsfilename = request.udo_filename;
                    vbmsUploadDocResponse.udo_vbmscategory = this._documentCategory;

                    _txnTimer.Restart();
                    if (DateTime.TryParse(eFolderUploadResponseDataInfo.mcs_vbmsUploadDate, out DateTime result))
                        vbmsUploadDocResponse.udo_vbmsuploaddate = result;
                    vbmsUploadDocResponse.udo_uploaded = true;
                    vbmsUploadDocResponse.udo_uploadmessage = "Uploaded Successfully";
                    _txnTimer.Stop();
                    _logTimerData.AppendLine("Time Creating response:" + _txnTimer.ElapsedMilliseconds);
                    _txnTimer.Restart();

                    UpdateUDOVBMSDocument(request, vbmsUploadDocResponse, (IOrganizationService)OrgServiceProxy, true);

                    _txnTimer.Stop();
                    _logTimerData.AppendLine("UpdateUDOVBMSDocument:" + _txnTimer.ElapsedMilliseconds);
                }
                else
                {
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, this._method, "!!!!!!!! VBMS UPLOAD FAILED !!!!!!!!", this._debug);

                    vbmsUploadDocResponse.udo_uploaded = false;
                    vbmsUploadDocResponse.udo_uploadmessage = "Upload to VBMS was not successful.";
                    if (eFolderUploadDocResponse.ExceptionOccurred)
                    {
                        vbmsUploadDocResponse.ExceptionMessage = eFolderUploadDocResponse.ExceptionMessage;
                        vbmsUploadDocResponse.ExceptionOccurred = eFolderUploadDocResponse.ExceptionOccurred;
                        vbmsUploadDocResponse.udo_uploadmessage += "\r\n\r\n" + eFolderUploadDocResponse.ExceptionMessage;
                    }

                    _txnTimer.Restart();
                    UpdateUDOVBMSDocument(request, vbmsUploadDocResponse, (IOrganizationService)OrgServiceProxy, false);
                    _txnTimer.Restart();

                    LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, this._method, eFolderUploadDocResponse.ExceptionMessage);
                    LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, this._method, "SOAP Request/Response:" + eFolderUploadDocResponse.SerializedSOAPRequest + eFolderUploadDocResponse.SerializedSOAPResponse);
                }

                this._EntireTimer.Stop();
                LogHelper.LogTiming(request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "Entire Upload", (object)null, this._EntireTimer.ElapsedMilliseconds);
                this._logTimerData.AppendLine("Entire Upload:" + (object)this._EntireTimer.ElapsedMilliseconds);

                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, this._method, "UDOVBMSUploadDocumentProcessor, Timing \r\n\r\n" + this._logTimerData.ToString(), this._debug);
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, this._method, "UDOVBMSUploadDocumentProcessor, Debug" + this._logData.ToString(), this._debug);

                return vbmsUploadDocResponse;
            }
            catch (Exception executionException)
            {
                string detail = executionException.StackTrace;
                detail += "\r\n\r\n";
                if (executionException.InnerException != null)
                {
                    detail += String.Format("INNER EXCEPTION: {0}\r\n", executionException.InnerException.Message);
                }
                LogError(request, executionException.Message, executionException);
                
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, this._method, _log.ToString(), this._debug);
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, this._method, executionException);

                vbmsUploadDocResponse.ExceptionOccurred = true;
                vbmsUploadDocResponse.ExceptionMessage = "There was an error uploading the document: " + detail;

                return vbmsUploadDocResponse;
            }
            finally
            {
                if (OrgServiceProxy != null)
                {
                    OrgServiceProxy.Dispose();
                }
            }
        }

        #region Update Document Status

        internal enum DocStatus
        {
            New = 1,
            Uploaded = 752280000,
            UploadFailed = 752280001
        }

        #endregion

        #region Create Note
        private void CreateNote(IOrganizationService service, UDOVBMSUploadDocumentAsyncRequest request, ResponseData response)
        {
            progressString = "START: CreateNote";

            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, _method, $"{progressString} ", request.Debug);

            try
            {
                string body = "", title = "";

                Entity note = new Entity("udo_note");
                Entity relatedEntity = null;
                EntityReference relatedObj = request.udo_relatedentity;

                if (relatedObj.LogicalName.Equals("udo_servicerequest", StringComparison.InvariantCultureIgnoreCase))
                {
                    relatedEntity = service.Retrieve(relatedObj.LogicalName, relatedObj.Id,
                        new ColumnSet("udo_regionalofficeid", "udo_participantid", "udo_claimnumber",
                            "udo_reqnumber", "udo_relatedveteranid", "udo_personid", "udo_servicerequestsid"));

                    body += request.udo_filename + " Uploaded for Service Request ";
                    title = "File Uploaded to VBMS for Service Request";
                }
                else if (relatedObj.LogicalName.Equals("udo_lettergeneration", StringComparison.InvariantCultureIgnoreCase))
                {
                    relatedEntity = service.Retrieve(relatedObj.LogicalName, relatedObj.Id,
                        new ColumnSet("udo_regionalofficeid", "udo_participantid",
                            "udo_reqnumber", "udo_relatedveteranid", "udo_personid", "udo_idproofid", "udo_claimnumber"));
                    body += request.udo_filename + " Uploaded for Letter Generation ";
                    title = "File Uploaded to VBMS for Letter Generation";
                }
                else
                {
                    // No note when it isn't related to anything... where would we put it...
                    return;
                }

                body += relatedEntity["udo_reqnumber"].ToString();
                Log("udo_reqnumber: {0}", relatedEntity["udo_reqnumber"]);

                #region get Regional Office Code from udo_regionalofficeid
                if (relatedEntity != null && relatedEntity.Contains("udo_regionalofficeid"))
                {
                    EntityReference regionaloffice = relatedEntity.GetAttributeValue<EntityReference>("udo_regionalofficeid");
                    if (regionaloffice != null)
                    {
                        string regionalofficeFetch = "<fetch count='1'><entity name='va_regionaloffice'><attribute name='va_code'/><filter>" +
                                    "<condition attribute='va_regionalofficeid' operator='eq' value='" + regionaloffice.Id + "'/>" +
                                    "</filter></entity></fetch>";

                        EntityCollection regionalofficeResults = service.RetrieveMultiple(new FetchExpression(regionalofficeFetch));

                        if (regionalofficeResults != null && regionalofficeResults.Entities.Count > 0)
                        {
                            Entity regionalofficeEntity = regionalofficeResults.Entities[0];
                            note["udo_ro"] = regionalofficeEntity.GetAttributeValue<string>("va_code");
                            Log("udo_ro: {0}", note["udo_ro"]);
                        }
                    }
                }
                #endregion

                if (relatedEntity.Contains("udo_servicerequestsid"))
                {
                    note["udo_idproofid"] = relatedEntity.GetAttributeValue<EntityReference>("udo_servicerequestsid");
                }
                else if (relatedEntity.Contains("udo_idproofid"))
                {
                    note["udo_idproofid"] = relatedEntity.GetAttributeValue<EntityReference>("udo_idproofid");
                }
                Log("udo_idproofid: {0}", ((EntityReference)note["udo_idproofid"]).Id);

                note["udo_claimid"] = relatedEntity.GetAttributeValue<string>("udo_claimnumber");
                Log("udo_claimid: {0}", note["udo_claimid"]);

                note["udo_participantid"] = relatedEntity.GetAttributeValue<string>("udo_participantid");
                Log("udo_participantid: {0}", note["udo_participantid"]);

                note["udo_veteranid"] = relatedEntity.GetAttributeValue<EntityReference>("udo_relatedveteranid");
                Log("udo_veteranid: {0}", ((EntityReference)note["udo_veteranid"]).Id);

                note["udo_personid"] = relatedEntity.GetAttributeValue<EntityReference>("udo_personid");
                Log("udo_personid: {0}", ((EntityReference)note["udo_personid"]).Id);

                note["udo_fromudo"] = false;
                Log("udo_fromudo: {0} (Note will be created directly using LOB)", note["udo_fromudo"]);

                note["udo_name"] = title;
                Log("udo_name: {0}", note["udo_name"]);

                note["udo_notetext"] = body;
                Log("udo_notetext: {0}", note["udo_notetext"]);

                CreateNoteUsingLOB(request, note, response);

                progressString = "Creating note in CRM";
                service.Create(note);
            }
            catch (Exception ex)
            {
                progressString = "Failed creating note.";
                string detail = ex.StackTrace;
                detail += "\r\n\r\n";
                if (ex.InnerException != null)
                {
                    detail += String.Format("INNER EXCEPTION: {0}\r\n", ex.InnerException.Message);
                }
                LogError(request, ex.Message, ex);
            }
            progressString = "END: CreateNote";
        }

        private void CreateNoteUsingLOB(UDOVBMSUploadDocumentAsyncRequest request, Entity note, ResponseData response)
        {
            progressString = "START: CreateNoteUsingLOB";

            // Build LOB Request
            try
            {
                UDOCreateNoteRequest noteRequest = new UDOCreateNoteRequest()
                {
                    MessageId = Guid.NewGuid().ToString(),
                    Debug = request.Debug,
                    udo_ClaimId = note.GetAttributeValue<string>("udo_claimid"),
                    udo_Note = note.GetAttributeValue<string>("udo_notetext"),
                    udo_ParticipantID = note.GetAttributeValue<string>("udo_participantid"),
                    udo_RO = note.GetAttributeValue<string>("udo_ro"),
                    udo_DateTime = string.Empty,
                    LogSoap = request.LogSoap,
                    LogTiming = request.LogTiming,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName
                };

                UDOCreateNoteResponse noteResponse = WebApiUtility.SendReceive<UDOCreateNoteResponse>(noteRequest, WebApiType.LOB);

                if (noteResponse.UDOCreateNoteInfo != null)
                {
                    #region success
                    UDOCreateNoteResponseInfo createInfo = noteResponse.UDOCreateNoteInfo;
                    note["udo_editable"] = true; // allow edit of new note
                    note["udo_claimid"] = createInfo.udo_ClaimId;
                    note["udo_dttime"] = createInfo.udo_DateTime;
                    note["udo_notetext"] = createInfo.udo_Note;
                    note["udo_ro"] = createInfo.udo_RO;
                    note["udo_type"] = createInfo.udo_Type;
                    if (createInfo.udo_SuspenseDate != null &&
                        createInfo.udo_SuspenseDate >= new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                    {
                        note["udo_suspensedate"] = createInfo.udo_SuspenseDate;
                    }
                    note["udo_user"] = createInfo.udo_User;
                    note["udo_userid"] = createInfo.udo_UserId;
                    note["udo_legacynoteid"] = createInfo.udo_legacynoteid;

                    // Already created using LOB
                    note["udo_fromudo"] = false;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                progressString = "Failed creating note.";

                LogError(request, ex.Message, ex);
            }
            progressString = "END: CreateNoteUsingLOB";
        }

        private void CreateNote(IOrganizationService service, ITracingService tracer, EntityReference relatedObj, DateTime? uploadTime, string fileName)
        {
            Log("CerateNote: START");
        }
        #endregion

        private static void DeleteAttachmentFromNote(IOrganizationService service, Entity note)
        {
            note["documentbody"] = null;
            note["filename"] = null;
            note["isdocument"] = false;
            service.Update(note);
        }

        private void UpdateUDOVBMSDocument(UDOVBMSUploadDocumentAsyncRequest request, UDOVBMSUploadDocumentResponse response, IOrganizationService OrgServiceProxy, bool success)
        {
            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, this._method, "Updating VBMS Document in CRM...", this._debug);

            Entity entity = new Entity()
            {
                LogicalName = request.udo_vbmsdocument.LogicalName
            };
            entity.Id = request.udo_vbmsdocument.Id;

            if (success)
            {
                entity["udo_vbmsdcsid"] = response.udo_vbmsdcsid;
                entity["udo_vbmscategory"] = response.udo_vbmscategory;
                entity["udo_vbmsuploaddate"] =response.udo_vbmsuploaddate;
                entity["udo_vbmsfilename"] = response.udo_vbmsfilename;
            }
            else
            {
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, this._method, "..updating VBMS Document with error details in CRM", this._debug);

                entity["udo_errorcode"] = response.mcs_errorCode;
                entity["udo_errorid"] = response.mcs_errorId;
                entity["udo_errortypefield"] = response.mcs_errorTypeField;
                entity["udo_exceptionmessage"] = response.ExceptionMessage;
                entity["udo_exceptionoccured"] = response.ExceptionOccurred;
            }
            entity["statuscode"] = new OptionSetValue(success ? 752280000 : 752280001);
            OrgServiceProxy.Update(entity);

            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, this._method, "...finished updating VBMS Document in CRM", this._debug);
        }

        #region VEISUpload
        private static string GetHashCode(byte[] contents)
        {
            if (contents == null || contents.Length == 0) return string.Empty;
            SHA256CryptoServiceProvider provider = new SHA256CryptoServiceProvider();
            return BitConverter.ToString(provider.ComputeHash(contents)).Replace("-", "").ToLower();
        }

        private VEISeFolderinitializeUploadResponse VEISInitializeUpload(UDOVBMSUploadDocumentAsyncRequest request, string hashCode, string docType, string docCategory)
        {
            DateTime receivedDate = DateTime.Now; // Equates to UTC time on server
            if (receivedDate.Hour >= 0 && receivedDate.Hour < 5)
            {
                /* To prevent VBMS upload failure due to future server date after Midnight 
                   i.e. 7pm ET client upload should not be sent as 12:00am ET the following day */
                receivedDate = DateTime.UtcNow.Subtract(new TimeSpan(5, 0, 0));

                LogHelper.LogInfo(request.OrganizationName, true, request.UserId, this._method, "NOTE: VBMS Received Date adjusted post 7pm ET to prevent upload failure.  Received Date before: " + DateTime.Now.ToString() + " Received Date after used for upload: " + receivedDate.ToString());
            } else
            {
                LogHelper.LogInfo(request.OrganizationName, true, request.UserId, this._method, "VBMS Received Date does not need adjusting to prevent post-7pm ET upload failure.");
            }

            progressString = "START: VEISInitializeUpload";

            var initRequest = new VEISeFolderinitializeUploadRequest
            {
                OrganizationName = request.OrganizationName,
                Debug = request.Debug,
                LogSoap = request.LogSoap,
                LogTiming = request.LogTiming,
                UserId = request.UserId,
                RelatedParentEntityName = request.RelatedParentEntityName,
                RelatedParentFieldName = request.RelatedParentFieldName,
                RelatedParentId = request.RelatedParentId,
                MessageId = request.MessageId.ToString(),
                mcs_doctype = docType,
                mcs_filename = request.udo_filename,
                mcs_source = _source,
                mcs_veteranidentifierInfo = new VEISeFoldermcs_veteranidentifier
                {
                    mcs_fileNumber = request.udo_filenumber
                },
                mcs_contenthash = hashCode,
                mcs_vareceivedate = receivedDate
            };

            // If specified, set the source.
            if (!String.IsNullOrEmpty(request.udo_source))
            {
                initRequest.mcs_source = request.udo_source;
            }

            var vetInfo = new VEISeFoldermcs_veteranidentifier();
            if (!String.IsNullOrEmpty(request.udo_filenumber))
            {
                vetInfo.mcs_fileNumber = request.udo_filenumber;
            }
            else if (!String.IsNullOrEmpty(request.udo_ssid))
            {
                vetInfo.mcs_ssn = request.udo_ssid;
            }
            else if (!String.IsNullOrEmpty(request.udo_edipi))
            {
                vetInfo.mcs_edipi = request.udo_edipi;
            }
            initRequest.mcs_veteranidentifierInfo = vetInfo;

            var pos = initRequest.mcs_filename.LastIndexOf('.');
            if (pos != -1)
            {
                initRequest.mcs_filename = initRequest.mcs_filename.Insert(pos, Guid.NewGuid().ToString("D"));
                request.udo_filename = initRequest.mcs_filename; // copy back to request for use later
            }

            List<VEISeFoldermcs_versionmetadata> metadata = new List<VEISeFoldermcs_versionmetadata>();

            if (!String.IsNullOrEmpty(request.udo_subject))
            {
                // this should have been set retrieving the docType
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "VBMS.InitalizeUpload", "Subject: " + request.udo_subject, this._debug);

                metadata.Add(new VEISeFoldermcs_versionmetadata() { mcs_key = "subject", mcs_value = request.udo_subject });
            }

            if (!String.IsNullOrEmpty(request.udo_userRole))
            {
                // Set the userRole
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "VBMS.InitalizeUpload", "UserRole: " + request.udo_userRole, this._debug);
                metadata.Add(new VEISeFoldermcs_versionmetadata() { mcs_key = "userRole", mcs_value = request.udo_userRole });
            }

            if (!String.IsNullOrEmpty(request.udo_userName))
            {
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "VBMS.InitalizeUpload", "UserName: " + request.udo_userName, this._debug);
                metadata.Add(new VEISeFoldermcs_versionmetadata() { mcs_key = "userName", mcs_value = request.udo_userName });
            }

            if (metadata.Count > 0) initRequest.mcs_versionmetadataMultipleInfo = metadata.ToArray();

            VEISeFolderinitializeUploadResponse folderinitializeUploadResponse = WebApiUtility.SendReceive<VEISeFolderinitializeUploadResponse>(initRequest, WebApiType.VEIS);

            if (request.LogSoap)
            {
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"SOAP Request: {folderinitializeUploadResponse.SerializedSOAPRequest} Response: {folderinitializeUploadResponse.SerializedSOAPResponse}", true);
            }

            if (!folderinitializeUploadResponse.ExceptionOccurred && folderinitializeUploadResponse.VEISeFolderinitializeUploadResponseDataInfo != null)
            {
                _token = folderinitializeUploadResponse.VEISeFolderinitializeUploadResponseDataInfo?.mcs_uploadToken;

                //_token = String.Empty; // Mimic upload failure
            }
            else
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, this._method, folderinitializeUploadResponse.ExceptionMessage);
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, this._method, "SOAP Request/Response:" + folderinitializeUploadResponse.SerializedSOAPRequest + folderinitializeUploadResponse.SerializedSOAPResponse);
            }

            progressString = "END: VEISInitializeUpload";
            if (request.Debug)
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, "Upload Token: " + this._token, true);

            return folderinitializeUploadResponse;
        }

        private VEISeFolderuploadDocumentResponse VEISUploadDocument(UDOVBMSUploadDocumentAsyncRequest request, Byte[] contents, string token)
        {
            progressString = "START: VEISUploadDocument";
            var uploadRequest = new VEISeFolderuploadDocumentRequest
            {
                OrganizationName = request.OrganizationName,
                Debug = request.Debug,
                LogSoap = request.LogSoap,
                LogTiming = request.LogTiming,
                UserId = request.UserId,
                RelatedParentEntityName = request.RelatedParentEntityName,
                RelatedParentFieldName = request.RelatedParentFieldName,
                RelatedParentId = request.RelatedParentId,
                MessageId = request.MessageId.ToString(),
                mcs_filecontents = contents,
                mcs_uploadtoken = token
            };

            if (request.LegacyServiceHeaderInfo != null)
            {
                uploadRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo()
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,

                };
            }

            var response = WebApiUtility.SendReceive<VEISeFolderuploadDocumentResponse>(uploadRequest, WebApiType.VEIS);
            if (request.LogSoap)
            {
                var requestResponse = response.SerializedSOAPRequest + response.SerializedSOAPResponse;
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $" VEISeFolderuploadDocumentRequest Request/Response {requestResponse}", true);
            }

            progressString = "END: VEISUploadDocument";
            return response;
        }

        private VEISeFolderuploadDocumentResponse UploadDocument(IOrganizationService service, UDOVBMSUploadDocumentAsyncRequest request)
        {
            progressString = "START: UploadDocument";
            string fileName = request.udo_filename;
            string docType = "474";
            string docCategory = string.Empty;

            if (request.udo_doctypeid.HasValue && request.udo_doctypeid.Value != Guid.Empty)
            {
                Entity doctypeinfo = getDocumentTypeInfo(service, request.udo_doctypeid.Value);
                _txnTimer.Stop();
                _logTimerData.AppendLine("getDocumentTypeInfo:" + _txnTimer.ElapsedMilliseconds);
                _txnTimer.Restart();
                docType = doctypeinfo.GetAttributeValue<string>("udo_doctype");
                docCategory = doctypeinfo.GetAttributeValue<string>("udo_documentcategory");
                if (String.IsNullOrEmpty(request.udo_subject))
                {
                    request.udo_subject = doctypeinfo.GetAttributeValue<string>("udo_name");
                }
            }
            else
            {
                if (fileName.Contains("Informal Claim Letter")) docType = "489";
            }

            byte[] filecontents = Convert.FromBase64String(request.udo_base64filecontents);

            // Clean specials from Upload
            if (!String.IsNullOrEmpty(request.udo_subject))
                request.udo_subject = RemoveSpecialCharacters(request.udo_subject);

            VEISeFolderinitializeUploadResponse folderinitializeUploadResponse = VEISInitializeUpload(request, UDOVBMSUploadDocumentAsyncProcessor.GetHashCode(filecontents), docType, docCategory);

            if (this._token != String.Empty)
            {
                _txnTimer.Stop();
                _logTimerData.AppendLine("VEISInitializeUpload:" + _txnTimer.ElapsedMilliseconds);
                _txnTimer.Restart();

                VEISeFolderuploadDocumentResponse veisDocResponse = VEISUploadDocument(request, filecontents, this._token);

                if (veisDocResponse.ExceptionOccurred)
                {
                    LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, this._method, $"ExceptionMessage: {veisDocResponse.ExceptionMessage} EC Trace : {veisDocResponse.EcTraceLog}");
                    LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, this._method, "SOAP Request/Response:" + veisDocResponse.SerializedSOAPRequest + veisDocResponse.SerializedSOAPResponse);
                }

                _txnTimer.Stop();
                _logTimerData.AppendLine("VEISUploadDocument:" + _txnTimer.ElapsedMilliseconds);
                _txnTimer.Restart();
               
                // Response is currently not parsing the category.
                this._documentCategory = docCategory;

                progressString = "END: UploadDocument";

                return veisDocResponse;
            }
            else
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, this._method, $"ExceptionMessage: ########### Upload Token is empty.  Unable to proceed with VBMS Upload ###########");

                return new VEISeFolderuploadDocumentResponse
                {
                    ExceptionOccurred = true,
                    SerializedSOAPRequest = folderinitializeUploadResponse.SerializedSOAPRequest,
                    SerializedSOAPResponse = folderinitializeUploadResponse.SerializedSOAPResponse,
                    ExceptionMessage = "Missing upload token - " + folderinitializeUploadResponse.ExceptionMessage,
                    mcs_errorCode = folderinitializeUploadResponse.mcs_errorCode,
                    mcs_errorId = folderinitializeUploadResponse.mcs_errorId,
                    mcs_errorTypeField = folderinitializeUploadResponse.mcs_errorTypeField
                };
            }
        }
        #endregion

        #region Document Type
        public Entity getDocumentTypeInfo(IOrganizationService service, Guid doctypeid)
        {
            return service.Retrieve("udo_vbmsdoctype", doctypeid, new ColumnSet("udo_doctype", "udo_documentcategory", "udo_documentforupload", "udo_name", "udo_vbmsdoctypeid"));
        }
        #endregion

        private void LogError(UDOVBMSUploadDocumentAsyncRequest request, string errorMessage, Exception ex,
            string errorTitle = "EXECUTION EXCEPTION", string detailTitle = "CALL STACK")
        {
            string method = MethodInfo.GetThisMethod().ToString();
            _log.Insert(0, "\r\n\r\nLog Details:");
            _log.Insert(0, errorMessage);
            _log.Insert(0, errorTitle + ":\r\n");
            _log.AppendLine("\r\n" + errorTitle + ": ");
            _log.AppendLine(errorMessage);

            string details = ex.StackTrace;
            details += "\r\n\r\n";
            if (ex.InnerException != null)
            {
                details += String.Format("INNER EXCEPTION: {0}\r\n", ex.InnerException.Message);
            }

            if (details != "")
            {
                _log.AppendLine("\r\n" + detailTitle + ": ");
                _log.AppendLine(details);
            }

            LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName,
                  method, ex);
            LogHelper.LogDebug(request.OrganizationName, request.UserId, this._debug, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName,
                method, "Progress: " + progressString + ", Message: " + _log.ToString(), false);
        }

        private void Log(string messageFormat, params object[] args)
        {
            _log.AppendFormat(messageFormat + "\r\n", args);
        }

        private string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
