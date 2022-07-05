using System;
using System.Linq;
using MCSPlugins;
using MCSUtilities2011;
using Microsoft.Xrm.Sdk;
using UDO.Model;
using System.ServiceModel;
using System.Security;
using System.Runtime.InteropServices;
using MCSHelperClass;
using Microsoft.Xrm.Sdk.Messages;

namespace Va.Udo.Crm.Letters.Plugins
{

    internal class PostLetterGenerationCreateUpdateRunner : PluginRunner
    {
        /// <summary>
        /// Alias of the image registered for the snapshot of the 
        /// primary entity"s attributes before the core platform operation executes.
        /// The image contains the following attributes:
        /// All Attributes
        /// </summary>
        private readonly string preImageAlias = "PreImage";

        /// <summary>
        /// Alias of the image registered for the snapshot of the 
        /// primary entity"s attributes after the core platform operation executes.
        /// The image contains the following attributes:
        /// All Attributes
        /// 
        /// Note: Only synchronous post-event and asynchronous registered plug-ins 
        /// have PostEntityImages populated.
        /// </summary>
        private readonly string postImageAlias = "PostImage";
        private SecureString letterSSN;
        public PostLetterGenerationCreateUpdateRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }
        
        public override Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.InputParameters["Target"] as Entity;
        }

        public Entity GetPrimaryEntityPostImage()
        {
            return (PluginExecutionContext.PostEntityImages != null && PluginExecutionContext.PostEntityImages.Contains(this.postImageAlias)) ? PluginExecutionContext.PostEntityImages[this.postImageAlias] as Entity : null;
        }

        public Entity GetPrimaryEntityPreImage()
        {
            return (PluginExecutionContext.PreEntityImages != null && PluginExecutionContext.PreEntityImages.Contains(this.preImageAlias)) ? PluginExecutionContext.PreEntityImages[this.preImageAlias] as Entity : null;
        }

        public override Entity GetSecondaryEntity()
        {
            return PluginExecutionContext.InputParameters["Target"] as Entity;
        }

        public override string McsSettingsDebugField
        {
            get { return "udo_lettergeneration"; }
        }

        internal void Execute(ITracingService tracingService)
        {
            try
            {
                Trace("Starting Letter Post Create Update Runner");
                var entity = GetPrimaryEntity();

                if (entity == null)
                {
                    throw new InvalidPluginExecutionException("Target entity is null");
                }

                var letter = GetPrimaryEntity().ToEntity<udo_lettergeneration>();

                var postImageRequest = GetPrimaryEntityPostImage();

                // Create the notes on Letter Generation Creation and also when the udo_SendNotestoMAPD is set to true
                //udo_SendNotestoMAPD is set to true when the Letter tab is closed   
                if (postImageRequest != null)
                {
                    var postImage = postImageRequest.ToEntity<udo_lettergeneration>();
                    bool shouldCreateNotes = false;
                    if (PluginExecutionContext.MessageName == "Create" && postImage.udo_RegionalOfficeId != null && GetSystemSettingValue("UDO_Letters_GenerateMapdNotesOnLetterCreate"))
                    {
                        shouldCreateNotes = true;
                    }
                    else if (PluginExecutionContext.MessageName != "Create" && letter.udo_SendNotestoMAPD != null && letter.udo_SendNotestoMAPD.Value)
                    {
                        var preImage = GetPrimaryEntityPreImage().ToEntity<udo_lettergeneration>();

                        if (preImage != null && preImage.udo_SendNotestoMAPD != null && !preImage.udo_SendNotestoMAPD.Value)
                        {
                            shouldCreateNotes = true;
                        }
                    }

                    EntityReference noteRef = null;
                    if (PluginExecutionContext.MessageName != "Create" && !shouldCreateNotes && !String.IsNullOrEmpty(letter.udo_Description))
                    {
                        var pre = GetPrimaryEntityPreImage();

                        noteRef = pre.GetAttributeValue<EntityReference>("udo_note");
                        shouldCreateNotes = true;
                    }

                    if (shouldCreateNotes && GetSystemSettingValue("UDO_Letters_NotesIntegrationEnabled"))
                    {
                        letterSSN = MCSHelper.ConvertToSecureString_new(letter.udo_SSN);
                        Trace("Generating Note");
                        var notes = MCSHelper.ConvertToSecureString_new(GenerateMapdNotes(postImage, PluginExecutionContext.MessageName, tracingService));
                        var debugNote = MCSHelper.ConvertToSecureString_new(string.Format("Saving Note: \r\n\r\n{0}", MCSHelper.ConvertToUnsecureString(notes)));

                        //tracingService.Trace(UDOHelper.ConvertToUnsecureString(debugNote));
                        CreateMapdNotes(notes, postImage, noteRef, PluginExecutionContext.MessageName, tracingService);
                    }

                    Trace("Ending Letter Post Create Update Runner");
                }
            }
            finally
            {
                Trace("Entered Finally");
                SetupLogger();
                Trace("Set up logger done.");
                ExecuteFinally();
                Trace("Exit Finally");
            }
        }

        private bool GetSystemSettingValue(string systemSettingsKey)
        {
            using (UDOContext context = new UDOContext(OrganizationService))
            {
                var shouldCreateNotes = (from settings in context.va_systemsettingsSet
                                         where settings.va_name == systemSettingsKey
                                         select settings.va_Description).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(shouldCreateNotes) && shouldCreateNotes.ToUpper() == "TRUE")
                {
                    return true;
                }
            }
            return false;
        }

        private void CreateMapdNotes(SecureString notes, udo_lettergeneration Letter, EntityReference existing, string pluginmessage, ITracingService tracingService)
        {
            string code = string.Empty;
            if (Letter.udo_RegionalOfficeId != null)
            {
                using (UDOContext context = new UDOContext(OrganizationService))
                {
                    code = (from ro in context.va_regionalofficeSet
                            where ro.va_regionalofficeId.Value == Letter.udo_RegionalOfficeId.Id
                            select ro.va_Code).FirstOrDefault();
                }
            }

            try
            {
                if (Letter.udo_PersonId == null)
                {
                    throw new Exception("The Letter does not have a personid.");
                }
                if (Letter.udo_RelatedVeteranId == null)
                {
                    throw new Exception("The Letter does not have a related veteran.");
                }

                var note = new udo_note
                {
                    udo_name = "Notes from Letter",
                    udo_ClaimId = Letter.udo_Claim,
                    udo_ParticipantID = Letter.udo_ParticipantID,
                    udo_VeteranId = Letter.udo_RelatedVeteranId,
                    udo_personId = Letter.udo_PersonId,
                    udo_idProofId = Letter.udo_IDProofId,
                    udo_NoteText = MCSHelper.ConvertToUnsecureString(notes),
                    udo_fromUDO = true,
                    udo_RO = code
                };

                if (existing != null)
                {
                    try
                    {
                        var newnote = new Entity("udo_note");
                        newnote.Id = existing.Id;
                        newnote.Attributes.Add("udo_fromudo", true);
                        newnote.Attributes.Add("udo_editable", true);
                        newnote.Attributes.Add("udo_notetext", MCSHelper.ConvertToUnsecureString(notes));

                        // ExecuteMultiple is used because ExecuteMultiple executes outside of the current
                        // database transaction, without using it, if notes throws an error, then the error
                        // cannot be handled in the current plugin.
                        Utilities.ExecuteRequestOutsideCurrentTransaction(new UpdateRequest { Target = newnote });
                        
                        //OrganizationService.Update(newnote);
                        
                        //note.Id = existing.Id;
                        //note.udo_editable = true;
                        //OrganizationService.Update(note);
                    }
                    catch (Exception e)
                    {
                        PluginError = true;
                        Trace("Error Updating Note: \r\n" +e.Message + "\r\n" + e.StackTrace);
                        SaveNoteAndUpdateLetter(Letter, pluginmessage, note);
                    }
                }
                else
                {

                    SaveNoteAndUpdateLetter(Letter, pluginmessage, note);
                }

                Trace("Note Created or Updated");
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                PluginError = true;
                Trace(ex.ToString());
            }
            catch (Exception ex)
            {
                PluginError = true;
                Trace(ex.ToString());
            }
        }

        private void SaveNoteAndUpdateLetter(udo_lettergeneration Letter, string pluginmessage, udo_note note)
        {
            try
            {
                // ExecuteMultiple is used because ExecuteMultiple executes outside of the current
                // database transaction, without using it, if notes throws an error, then the error
                // cannot be handled in the current plugin.
                Guid? id = null;
                var createResponse = Utilities.ExecuteRequestOutsideCurrentTransaction(new CreateRequest { Target = note }) as CreateResponse;
                if (createResponse != null) id = createResponse.id;
                
                if (id.HasValue && String.Equals(pluginmessage, "update", StringComparison.InvariantCultureIgnoreCase))
                {
                    var letter_with_note = new Entity("udo_lettergeneration");
                    letter_with_note.Id = Letter.Id;
                    letter_with_note["udo_note"] = new EntityReference("udo_note", id.Value);

                    //var rn = OrganizationService.Retrieve("udo_note",id,new Microsoft.Xrm.Sdk.Query.ColumnSet("udo_legacynoteid"));
                    //letter_with_note["udo_devnoteid"] = rn.GetAttributeValue<string>("udo_legacynoteid");

                    OrganizationService.Update(letter_with_note);
                }
            }
            catch (Exception)
            {
                PluginError = true;
                // Ignore error
            }
        }

        private string GenerateMapdNotes(udo_lettergeneration Letter, string messageName, ITracingService tracingService)
        {
            var sentFormText = string.Empty;
            if (Letter.udo_PMC != null && Letter.udo_PMC.Value)
            {
                sentFormText += "PMC, ";
            }
            if (Letter.udo_NOKLETTER != null && Letter.udo_NOKLETTER.Value)
            {
                sentFormText += "NOK Letter, ";
            }
            if (Letter.udo_21530 != null && Letter.udo_21530.Value)
            {
                sentFormText += "21-530, ";
            }
            if (Letter.udo_21534 != null && Letter.udo_21534.Value)
            {
                sentFormText += "21-534, ";
            }
            if (Letter.udo_401330 != null && Letter.udo_401330.Value)
            {
                sentFormText += "40-1330, ";
            }
            if (Letter.udo_Other != null && Letter.udo_Other.Value)
            {
                var specificationText = string.IsNullOrWhiteSpace(Letter.udo_OTHERSPECIFICATION) ? string.Empty : Letter.udo_OTHERSPECIFICATION;
                sentFormText += specificationText;
            }

            var devNoteText = MCSHelper.ConvertToSecureString_new(BuildDevNoteText(Letter, messageName, sentFormText, tracingService));

            return MCSHelper.ConvertToUnsecureString(devNoteText);
        }

        private string BuildDevNoteText(udo_lettergeneration Letter, string messageName, string sentFormText, ITracingService tracingService)
        {
            var reportedForFullName = string.Format("{0} {1}", Letter.udo_DepFirstName, Letter.udo_DepLastName);

            var fnodVeteranFullName = string.Format("{0} {1}", Letter.udo_SRFirstName, Letter.udo_SRLastName);

            var letterName = (Letter.udo_Letter == null) ? string.Empty : Letter.udo_Letter.Name;

            var devNoteText = MCSHelper.ConvertToSecureString_new("Letter " + letterName +
                ((messageName == "Create") ? " created. File #:" : " updated. File #:") +
                            (letterSSN != null ? MCSHelper.ConvertToUnsecureString(letterSSN) : string.Empty) +
                              (Letter.udo_issue != null
                                  ? string.Format("Type: {0}. ", Utilities.getOptionSetString(Letter.udo_issue.Value,
                                          udo_lettergeneration.EntityLogicalName, "udo_issue", tracingService))
                                  : string.Empty) +
                              (Letter.udo_action != null
                                  ? string.Format("Action: {0}. ", Utilities.getOptionSetString(Letter.udo_action.Value,
                                          udo_lettergeneration.EntityLogicalName, "udo_action", tracingService))
                                  : string.Empty) +
                              (Letter.udo_RegionalOfficeId != null
                                  ? string.Format("SOJ: {0}. ", Letter.udo_RegionalOfficeId.Name)
                                  : string.Empty) +
                              (string.IsNullOrWhiteSpace(Letter.udo_Description)
                                  ? string.Empty
                                  : string.Format("\n\nDesc.: {0}", Letter.udo_Description)) +
                              (string.IsNullOrWhiteSpace(Letter.udo_LettersCreated)
                                  ? string.Empty
                                  : string.Format("\n\nThe following letters were sent: {0}", Letter.udo_LettersCreated)));
            return MCSHelper.ConvertToUnsecureString(devNoteText);
        }
    }
}
