using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRM.Integration.UDO.Common;
using VRM.Integration.UDO.Common.Messages;
using VRM.Integration.UDO.Notes.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Letters.Messages;
using VRM.Integration.UDO.ServiceRequest.Messages;


namespace VRM.Integration.UDO.Helpers
{
    public class MapDNote
    {
        public delegate void ProgressSetter(string progress, params object[] args);

        public string Name { get; set; }
        public string ParticipantId { get; set; }
        public string ClaimNumber { get; set; }
        public Guid VeteranId { get; set; }
        public Guid PersonId { get; set; }
        public Guid IdProofId { get; set; }
        public string NoteText { get; set; }
        public string RegionalOfficeCode { get; set; }

        private const bool FromUDO = true;

        public MapDNote()
        {
        }

        public static Guid Create(ProgressSetter UpdateProgress, UDORequestBase request, Entity entity, string name, string noteText, OrganizationServiceProxy orgService)
        {
            if (UpdateProgress != null) UpdateProgress("MapDNote: Create: Start");
            var callerId = request.UserId;
            MapDNote note = new MapDNote();
            var headerInfo = request.LegacyServiceHeaderInfo;
            note.CreateFromEntity(UpdateProgress, entity, name, noteText, orgService, callerId);

            if (note.VeteranId != Guid.Empty)
            {
                Entity crmNote = note.BuildNote(UpdateProgress);

	            note.CreateUsingLOB(UpdateProgress, headerInfo, crmNote, request.LogSoap, request.LogTiming, request.UserId, request.OrganizationName, request.Debug);

                //crmNote["udo_fromudo"] = true;
                orgService.CallerId = Guid.Empty;
                var id = orgService.Create(crmNote);
                if (UpdateProgress != null) UpdateProgress("MapDNote: Create: End");
                return id;
            }

            return Guid.Empty;
        }

        public static Guid Create(ProgressSetter UpdateProgress, Entity entity, string name, string noteText, OrganizationServiceProxy orgService, Guid callerId)
        {
            if (UpdateProgress != null) UpdateProgress("MapDNote: Create(2): Start");
            MapDNote note = new MapDNote();
            note.CreateFromEntity(UpdateProgress, entity, name, noteText, orgService, callerId);

            var id = note.Create(UpdateProgress, orgService, callerId);
            if (UpdateProgress != null) UpdateProgress("MapDNote: Create(2): End");
            return id;
        }

        public static Guid Create(ProgressSetter UpdateProgress, EntityReference entityReference, string name, string noteText, OrganizationServiceProxy orgService, Guid callerId)
        {
            if (UpdateProgress != null) UpdateProgress("MapDNote: Create(3): Start");
            MapDNote note = new MapDNote();
            note.CreateFromEntityReference(UpdateProgress, entityReference, name, noteText, orgService, callerId);
            var id = note.Create(UpdateProgress, orgService, callerId);
            if (UpdateProgress != null) UpdateProgress("MapDNote: Create(3): End");
            return id;
        }

        public Entity CreateUsingLOB(ProgressSetter UpdateProgress, UDOHeaderInfo HeaderInfo, Entity note, bool logSoap, bool logTimer, Guid UserId, string OrgName, bool debug)
        {
            // Build LOB Request
            if (UpdateProgress != null) UpdateProgress("MapDNote: CreateUsingLOB: Start");
            var request = new UDOCreateNoteRequest()
            {
                LegacyServiceHeaderInfo = HeaderInfo,
                MessageId = Guid.NewGuid().ToString(),
                Debug = debug,
                udo_ClaimId = note.GetAttributeValue<string>("udo_claimid"),
                udo_Note = note.GetAttributeValue<string>("udo_notetext"),
                udo_ParticipantID = note.GetAttributeValue<string>("udo_participantid"),
                udo_RO = note.GetAttributeValue<string>("udo_ro"),
                udo_DateTime = string.Empty,//DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                LogSoap = logSoap,
                LogTiming = logTimer,
                UserId = UserId,
                OrganizationName = OrgName
            };

            if (UpdateProgress != null) UpdateProgress("MapDNote: Note LOB Request: {0}", request.SerializeToString());
            //TODO(NP): Update the local call to create Note
            var response = request.SendReceive<UDOCreateNoteResponse>(MessageProcessType.Local);
            //               (uri, "UDOCreateNoteRequest", request, _logSettings);
            // Logger.WriteDebugMessage("back from EC");

            if (response.UDOCreateNoteInfo != null)
            {
                #region success
                if (UpdateProgress != null) UpdateProgress("MapDNote: Note Created Successfully");
                var createInfo = response.UDOCreateNoteInfo;
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
            if (UpdateProgress != null) UpdateProgress("MapDNote: CreateUsingLOB: End");
            return note;
        }

        public Guid Create(ProgressSetter UpdateProgress, OrganizationServiceProxy OrgService, Guid CallerId)
        {
            if (UpdateProgress != null) UpdateProgress("MapDNote: Create(4): Start");
            // This should be one read not two...
            if (!GetSettingsEnabled(UpdateProgress, OrgService, "UDO_SR_GenerateMapdNotesOnSRCreate", "UDO_SR_NotesIntegrationEnabled"))
            {
                return Guid.Empty;
            }

            var currentCaller = OrgService.CallerId;
            Entity note = BuildNote(UpdateProgress);

            OrgService.CallerId = CallerId;
            var id = OrgService.Create(note);
            OrgService.CallerId = currentCaller;
            if (UpdateProgress != null) UpdateProgress("MapDNote: Create(4): End");
            return id;
        }

        private Entity BuildNote(ProgressSetter UpdateProgress)
        {
            if (UpdateProgress != null) UpdateProgress("MapDNote: BuildNote: Start");
            Entity note = new Entity("udo_note");
            note.LogicalName = "udo_note";
            note["udo_name"] = Name;
            note["udo_claimid"] = ClaimNumber;
            note["udo_participantid"] = ParticipantId;
            note["udo_veteranid"] = new EntityReference("contact", VeteranId);
            note["udo_personid"] = new EntityReference("udo_person", PersonId);
            note["udo_idproofid"] = new EntityReference("udo_idproof", IdProofId);
            note["udo_notetext"] = NoteText;
            note["udo_fromudo"] = FromUDO;
            note["udo_ro"] = RegionalOfficeCode;
            if (UpdateProgress != null) UpdateProgress("MapDNote: BuildNote: Note: \r\n{0}", note.DumpToString());
            if (UpdateProgress != null) UpdateProgress("MapDNote: BuildNote: End");
            return note;
        }

        private void CreateFromEntityReference(ProgressSetter UpdateProgress, EntityReference entityReference, string name, string noteText, OrganizationServiceProxy OrgService, Guid CallerId)
        {
            if (UpdateProgress != null) UpdateProgress("MapDNote: CreateFromEntityReference: Start");
            var currentCaller = OrgService.CallerId;
            OrgService.CallerId = CallerId;
            var entity = OrgService.Retrieve(entityReference.LogicalName, entityReference.Id, new ColumnSet(true));
            OrgService.CallerId = currentCaller;
            CreateFromEntity(UpdateProgress, entity, name, noteText, OrgService, CallerId);
            if (UpdateProgress != null) UpdateProgress("MapDNote: CreateFromEntityReference: End");
        }

        private void CreateFromEntity(ProgressSetter UpdateProgress, Entity entity, string name, string noteText, OrganizationServiceProxy OrgService, Guid CallerId)
        {
            if (UpdateProgress != null) UpdateProgress("MapDNote: CreateFromEntity: Start");
            #region get Regional Office Code from udo_regionalofficeid
            if (entity.Contains("udo_regionalofficeid"))
            {
                var regionaloffice = entity.GetAttributeValue<EntityReference>("udo_regionalofficeid");
                if (regionaloffice != null)
                {
                    var regionalofficeFetch = "<fetch count='1'><entity name='va_regionaloffice'><attribute name='va_code'/><filter>" +
                                "<condition attribute='va_regionalofficeid' operator='eq' value='" + regionaloffice.Id + "'/>" +
                                "</filter></entity></fetch>";

                    var currentCaller = OrgService.CallerId;
                    OrgService.CallerId = CallerId;
                    var regionalofficeResults = OrgService.RetrieveMultiple(new FetchExpression(regionalofficeFetch));
                    OrgService.CallerId = currentCaller;

                    if (regionalofficeResults != null && regionalofficeResults.Entities.Count > 0)
                    {
                        var regionalofficeEntity = regionalofficeResults.Entities[0];
                        RegionalOfficeCode = regionalofficeEntity.GetAttributeValue<string>("va_code");
                    }
                }
            }
            #endregion

            ParticipantId = entity.GetAttributeValue<string>("udo_participantid");

            var veteranRef = entity.GetAttributeValue<EntityReference>("udo_relatedveteranid");
            VeteranId = veteranRef == null ? Guid.Empty : veteranRef.Id;

            var personRef = entity.GetAttributeValue<EntityReference>("udo_personid");
            PersonId = personRef == null ? Guid.Empty : personRef.Id;

            var idpRef = entity.GetAttributeValue<EntityReference>("udo_servicerequestsid");
            IdProofId = idpRef == null ? Guid.Empty : idpRef.Id;

            if (!String.IsNullOrEmpty(Name))
            {
                Name = name;
            }
            else if (entity.LogicalName.Equals("udo_servicerequest", StringComparison.InvariantCultureIgnoreCase))
            {
                Name = "Notes from Service Request";
            }
            else if (entity.LogicalName.Equals("udo_lettergeneration", StringComparison.InvariantCultureIgnoreCase))
            {
                Name = "Notes from Letter Generation";
            }
            else
            {
                Name = "Note";
            }

            ClaimNumber = entity.GetAttributeValue<string>("udo_claimnumber");
            NoteText = noteText;
            if (UpdateProgress != null) UpdateProgress("MapDNote: CreateFromEntity: End");
        }

        /// <summary>
        /// Generates the mapd notes content from the service request attribute values
        /// </summary>
        /// <param name="entity">The service request object</param>
        /// <param name="messageName">The event message name</param>
        /// <returns>Generated Mapd note String</returns>
        public static string GenerateMapdNotes(ProgressSetter UpdateProgress, string OrgName, Guid UserId, Entity entity, string messageName)
        {
            if (UpdateProgress != null) UpdateProgress("MapDNote: GenerateMapdNotes: Start");
            if (entity.LogicalName != "udo_servicerequest" && entity.LogicalName != "udo_lettergeneration")
            {
                throw new ArgumentException(String.Format("Expected Service Request or Letter Generation and received a type {0} instead.", entity.LogicalName));
            }

            var sentFormText = string.Empty;

            if (entity.GetAttributeValue<bool>("udo_pmc"))
                sentFormText += "PMC, ";

            if (entity.GetAttributeValue<bool>("udo_nokletter"))
                sentFormText += "NOK Letter, ";

            if (entity.GetAttributeValue<bool>("udo_21530"))
                sentFormText += "21-530, ";

            if (entity.GetAttributeValue<bool>("udo_21534"))
                sentFormText += "21-534, ";

            if (entity.GetAttributeValue<bool>("udo_401330"))
                sentFormText += "40-1330, ";

            if (entity.GetAttributeValue<bool>("udo_other"))
                sentFormText += entity.GetAttributeValue<string>("udo_otherspecification");

            var devNoteText = BuildDevNoteText(UpdateProgress, OrgName, UserId, entity, messageName, sentFormText);
            if (UpdateProgress != null) UpdateProgress("MapDNote: GenerateMapdNotes: End");
            return devNoteText;
        }

        /// <summary>
        /// Adding additional details to the mapd notes
        /// </summary>
        /// <param name="entity">The service request object</param>
        /// <param name="messageName">The event message name</param>
        /// <param name="sentFormText">The sent from information</param>
        /// <returns></returns>
        private static string BuildDevNoteText(ProgressSetter UpdateProgress, string OrgName, Guid UserId, Entity entity, string messageName, string sentFormText)
        {
            if (UpdateProgress != null) UpdateProgress("MapDNote: BuildDevNoteText: Start");
            if (entity.LogicalName != "udo_servicerequest" && entity.LogicalName != "udo_lettergeneration")
            {
                throw new ArgumentException(String.Format("Expected Service Request or Letter Generation and received a type {0} instead.", entity.LogicalName));
            }

            if (entity.LogicalName == "udo_servicerequest")
            {
                if (UpdateProgress != null) UpdateProgress("MapDNote: BuildDevNoteText: End");
                return BuildServiceRequestNote(UpdateProgress, OrgName, UserId, entity, messageName, sentFormText);
            }
            if (UpdateProgress != null) UpdateProgress("MapDNote: BuildDevNoteText: End");
            return BuildLetterNote(UpdateProgress, OrgName, UserId, entity, messageName, sentFormText);
        }

        private static string BuildLetterNote(ProgressSetter UpdateProgress, string OrgName, Guid UserId, Entity entity, string messageName, string sentFormText)
        {
            if (UpdateProgress != null) UpdateProgress("MapDNote: BuildLetterNote: Start");
            var attributes = UDO.Common.EntityCache.GetAttributes(OrgName, "udo_servicerequest", UserId);

            var reportedForFullName = string.Format("{0} {1}",
                entity.GetAttributeValue<string>("udo_depfirstname"),
                entity.GetAttributeValue<string>("udo_deplastname"));

            var fnodVeteranFullName = string.Format("{0} {1}",
                entity.GetAttributeValue<string>("udo_srfirstname"),
                entity.GetAttributeValue<string>("udo_srlastname"));

            StringBuilder devNoteText = new StringBuilder("Letter ");
            devNoteText.Append(entity.GetAttributeValue<string>("udo_letter"));

            if (messageName == "Create")
            {
                devNoteText.Append(" created. ");
            }
            else if (messageName == "Copy")
            {
                devNoteText.Append(" copied. ");
            }
            else
            {
                devNoteText.Append(" updated. ");
            }
            

            if (entity.GetAttributeValue<string>("udo_ssn").Length > 0)
                devNoteText.AppendFormat("File #: {0}", entity.GetAttributeValue<string>("udo_ssn"));

            var issue = entity.GetAttributeValue<OptionSetValue>("udo_issue");
            if (issue != null)
            {
                devNoteText.AppendFormat("Type: {0}. ", EntityCache.GetOptionSetLabel(OrgName, entity.LogicalName, UserId, "udo_issue", issue.Value));
            }

            var action = entity.GetAttributeValue<OptionSetValue>("udo_action");
            if (action != null)
            {
                devNoteText.AppendFormat("Action: {0}. ", EntityCache.GetOptionSetLabel(OrgName, entity.LogicalName, UserId, "udo_action", action.Value));
            }

            var regionaloffice = entity.GetAttributeValue<EntityReference>("udo_regionalofficeid");
            if (regionaloffice != null)
                devNoteText.AppendFormat("SOJ: {0}. ", regionaloffice.Name);

            var desc = entity.GetAttributeValue<string>("udo_description");
            if (!string.IsNullOrWhiteSpace(desc))
                devNoteText.AppendFormat("\n\nDesc.: {0}", desc);

            var letterscreated = entity.GetAttributeValue<string>("udo_letterscreated");
            if (!string.IsNullOrEmpty(letterscreated))
                devNoteText.AppendFormat("\n\nThe following letters were sent: {0}", letterscreated);

            if (UpdateProgress != null) UpdateProgress("MapDNote: BuildLetterNote: End");
            return devNoteText.ToString();
        }

        private static string BuildServiceRequestNote(ProgressSetter UpdateProgress, string OrgName, Guid UserId, Entity entity, string messageName, string sentFormText)
        {
            if (UpdateProgress != null) UpdateProgress("MapDNote: BuildServiceRequestNote: Start");
            var attributes = UDO.Common.EntityCache.GetAttributes(OrgName, "udo_servicerequest", UserId);

            var reportedForFullName = string.Format("{0} {1}",
                entity.GetAttributeValue<string>("udo_depfirstname"),
                entity.GetAttributeValue<string>("udo_deplastname"));

            var fnodVeteranFullName = string.Format("{0} {1}",
                entity.GetAttributeValue<string>("udo_srfirstname"),
                entity.GetAttributeValue<string>("udo_srlastname"));

            StringBuilder devNoteText = new StringBuilder("Service Request ");

            devNoteText.Append(entity.GetAttributeValue<string>("udo_reqnumber"));
            if (messageName == "Create")
            {
                devNoteText.Append(" created. ");
            }
            else if (messageName == "Copy")
            {
                devNoteText.Append(" copied. ");
            }
            else
            {
                devNoteText.Append(" updated. ");
            }
            
            if (entity.Contains("udo_ssn"))
                devNoteText.AppendFormat("File #: {0}. ", entity["udo_ssn"]);

            var issue = entity.GetAttributeValue<OptionSetValue>("udo_issue");
            if (issue != null)
            {
                devNoteText.AppendFormat("Type: {0}. ", EntityCache.GetOptionSetLabel(OrgName, entity.LogicalName, UserId, "udo_issue", issue.Value));
            }

            var action = entity.GetAttributeValue<OptionSetValue>("udo_action");
            if (action != null)
            {
                devNoteText.AppendFormat("Action: {0}. ", EntityCache.GetOptionSetLabel(OrgName, entity.LogicalName, UserId, "udo_action", action.Value));
            }

            var regionaloffice = entity.GetAttributeValue<EntityReference>("udo_regionalofficeid");
            if (regionaloffice != null)
                devNoteText.AppendFormat("SOJ: {0}. ", regionaloffice.Name);

            var desc = entity.GetAttributeValue<string>("udo_description");
            if (!string.IsNullOrWhiteSpace(desc))
                devNoteText.AppendFormat("\n\nDesc.: {0}", desc);

            var letterscreated = entity.GetAttributeValue<string>("udo_letterscreated");
            if (!string.IsNullOrEmpty(letterscreated))
                devNoteText.AppendFormat("\n\nThe following letters were sent: {0}", letterscreated);

            if (action != null)
            {
                if (action.Value == 953850002)
                {
                    //"0820d"
                    var dateofmissingpayment = entity.GetAttributeValue<DateTime?>("udo_dateofmissingpayment");
                    if (dateofmissingpayment.HasValue)
                    {
                        devNoteText.AppendFormat(".\n\nMissing Payment Date: {0}", dateofmissingpayment.Value.ToString("MM/dd/yyyy"));
                    }

                    var amountofpayments = entity.GetAttributeValue<Decimal?>("udo_amtofpayments");
                    if (amountofpayments != null)
                    {
                        devNoteText.AppendFormat(".\nMissing Payment Amt: ${0}", amountofpayments);
                    }
                }

                if (action.Value == 953850003)
                {
                    //"0820f"
                    devNoteText = new StringBuilder("0820f sent for MOD to ROJ ");

                    if (regionaloffice != null) devNoteText.AppendFormat("'{0}'\n", regionaloffice.Name);

                    var reportedby = entity.GetAttributeValue<string>("udo_nameofreportingindividual");
                    if (!string.IsNullOrEmpty(reportedby))
                        devNoteText.AppendFormat("Reported By '{0}'\n", reportedby);

                    if (!string.IsNullOrEmpty(reportedForFullName))
                        devNoteText.AppendFormat("Reported for '{0}', ", reportedForFullName);

                    var depdob = entity.GetAttributeValue<DateTime?>("udo_depdateofbirth");
                    if (depdob.HasValue)
                        devNoteText.AppendFormat("DOB: {0}, ", depdob.Value.ToString("MM/dd/yyyy"));

                    if (entity.Contains("udo_depssn"))
                        devNoteText.AppendFormat("SSN: {0}", entity["udo_depssn"]);

                    if (!string.IsNullOrWhiteSpace(desc))
                        devNoteText.AppendFormat("\n\nDesc.: {0}", desc);

                    if (!string.IsNullOrEmpty(letterscreated))
                        devNoteText.AppendFormat("\n\nThe following letters were sent: {0}", letterscreated);

                }

                if (action.Value == 953850001)
                {
                    //"0820a"
                    devNoteText = new StringBuilder("0820a sent for FNOD\n");

                    //if (regionaloffice != null) devNoteText.AppendFormat("'{0}'\n", regionaloffice.Name);

                    var reportedby = entity.GetAttributeValue<string>("udo_nameofreportingindividual");
                    if (!string.IsNullOrEmpty(reportedby))
                        devNoteText.AppendFormat("Reported By '{0}'\n", reportedby);

                    if (!string.IsNullOrEmpty(fnodVeteranFullName))
                        devNoteText.AppendFormat("Reported for '{0}', ", fnodVeteranFullName);

                    var depdod = entity.GetAttributeValue<DateTime?>("udo_dateofdeath");
                    if (depdod.HasValue)
                        devNoteText.AppendFormat("Date of Death: {0}, ", depdod.Value.ToString("MM/dd/yyyy"));

                    if (entity.Contains("udo_srssn"))
                        devNoteText.AppendFormat("SSN: {0}", entity["udo_srssn"]);

                    if (!string.IsNullOrWhiteSpace(sentFormText))
                        devNoteText.AppendFormat("Sent Forms:  {0}", sentFormText);

                    if (!string.IsNullOrWhiteSpace(desc))
                        devNoteText.AppendFormat("\n\nDesc.: {0}", desc);

                    if (!string.IsNullOrEmpty(letterscreated))
                        devNoteText.AppendFormat("\n\nThe following letters were sent: {0}", letterscreated);

                }
            }
            if (UpdateProgress != null) UpdateProgress("MapDNote: BuildServiceRequestNote: End");
            return devNoteText.ToString();
        }

        private bool GetSettingsEnabled(ProgressSetter UpdateProgress, OrganizationServiceProxy OrgService, params string[] systemSettingsKeys)
        {
            if (UpdateProgress != null) UpdateProgress("MapDNote: GetSettingsEnabled: Start");
            var fetch = "<fetch><entity name='va_systemsettings'><attribute name='va_description'/>" +
                        "<filter type='or'>";
            foreach (var key in systemSettingsKeys)
            {
                fetch += "<condition attribute='va_name' operator='eq' value='" + key.ToLowerInvariant() + "'/>";

            }
            fetch += "</filter></entity></fetch>";

            OrgService.CallerId = Guid.Empty;
            var results = OrgService.RetrieveMultiple(new FetchExpression(fetch));
            if (results != null && results.Entities.Count > 0)
            {
                foreach (var result in results.Entities)
                {
                    if (!result["va_description"].ToString().Equals("true", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (UpdateProgress != null)
                        {
                            UpdateProgress("MapDNote: GetSettingsEnabled: Not Enabled");
                            UpdateProgress("MapDNote: GetSettingsEnabled: End");
                        }
                        return false;
                    }
                }
            }
            if (UpdateProgress != null)
            {
                UpdateProgress("MapDNote: GetSettingsEnabled: Enabled");
                UpdateProgress("MapDNote: GetSettingsEnabled: End");
            }
                        
            return true;
        }
    }
}
