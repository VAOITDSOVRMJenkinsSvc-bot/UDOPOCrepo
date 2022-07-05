using MCSPlugins;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDO.CustomActions.Notes.Plugins.Messages;
using UDO.LOB.Core;
using VRMRest;

namespace UDO.CustomActions.Notes.Plugins
{
    public class NotesRunner : UDOActionRunner
    {

        public NotesRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            TracingService.Trace("Base");

            EntityReference entity = (EntityReference)PluginExecutionContext.InputParameters["ParentEntityReference"];

            switch (entity.LogicalName)
            {
                case "crme_dependentmaintenance":
                    _debugField = "crme_dependentmaintenance";
                    _validEntities = new string[] { "crme_dependentmaintenance" };
                    break;
                case "va_fnod":
                    _debugField = "va_fnod";
                    _validEntities = new string[] { "va_fnod" };
                    break;
                case "udo_pointofinteraction":
                    _debugField = "udo_pointofinteraction";
                    _validEntities = new string[] { "udo_pointofinteraction" };
                    break;
                default:
                    break;
            }

            _logTimerField = "udo_contactlogtimer";
            _logSoapField = "udo_contactlogsoap";
            _vimtRestEndpointField = "crme_restendpointforvimt";
            _vimtTimeoutField = "udo_contacttimeout";
        }
        public override void DoAction()
        {
            _method = "DoAction";
            GetSettingValues();

            EntityReference entity = (EntityReference)PluginExecutionContext.InputParameters["ParentEntityReference"];
            TracingService.Trace("ID is: " + entity.Id);

            switch (entity.LogicalName)
            {
                case "crme_dependentmaintenance":
                    DependentMaintenanceNotes(entity);
                    break;
                case "va_fnod":
                    FNODNotes(entity);
                    break;
                case "udo_pointofinteraction":
                    POINotes(entity);
                    break;
                default:
                    break;
            }

            
        }
        private void SetOutputParameter(string key, object value)
        {
            if (PluginExecutionContext.OutputParameters.ContainsKey(key))
            {
                PluginExecutionContext.OutputParameters[key] = value;
                TracingService.Trace("Set Output Parameter: {0} = {1}", key, value);
                return;
            }
            PluginExecutionContext.OutputParameters.Add(key, value);
            TracingService.Trace("Set Output Parameter: {0} = {1}", key, value);
        }

        private void DependentMaintenanceNotes(EntityReference entity)
        {
            string childName = string.Empty;
            string spouseName = string.Empty;
            string childRelationshipName = string.Empty;
            string marriageTerminationReason = string.Empty;

            string NoteTextAdd = "686c submitted through Dependent Maintenance to add: ";
            bool trackNoteTextAdd = false;
            bool trackNoteTextNewSchChild = false;
            bool trackNoteTextSpouseRemove = false;
            bool trackNoteText674 = false;
           

            string NoteText = string.Empty;
            string NoteText674 = "674 submitted through Dependent Maintenance to add: ";
            string NoteTextNewSchChild = "686c and 674 submitted through Dependent Maintenance to add:";
            string NoteTextSpouseRemove = "686c submitted through Dependent Maintenance to remove: ";

            string NoteTextSpouseAddRemove = "686c submitted through Dependent Maintenance to add and remove: ";
            bool trackNoteTextSpouseAddRemove = false;

            int childCount = 0;
            int spouseCount = 0;
            var veteranID = new Guid();
            var interactionID = new Guid();
            var personID = new Guid();
            string partipantIdContact = string.Empty;
            DateTime marriageEndDate = DateTime.MinValue;

            #region Reading dependent info
            TracingService.Trace("Fetching Dependent Records Detail");
            var dependentDetails = @"<fetch>
                                      <entity name='crme_dependent' >
                                        <attribute name='crme_dependentrelationship' />
                                        <attribute name='crme_lastname' />
                                        <attribute name='crme_firstname' />
                                        <attribute name='crme_childrelationship' />
                                        <attribute name='crme_maintenancetype' />
                                        <attribute name='crme_childage1823inschool' />
                                        <attribute name='udo_marriageenddate' />
                                        <attribute name='udo_howwasmarriageterminated' />
                                        <filter type='and' >
                                          <condition attribute='crme_dependentmaintenance' operator='eq' value = '" + entity.Id + @"' />
                                        </filter>
                                      </entity>
                                    </fetch>";

            EntityCollection dependents = OrganizationService.RetrieveMultiple(new FetchExpression(dependentDetails));
            TracingService.Trace("Finished Fetching Dependent Records Detail");
            TracingService.Trace("Dependent Result Count - " + dependents.Entities.Count);
            #endregion


            #region Reading veteran ID and ID Proof
            var veteranDetails = @"<fetch>
                                  <entity name='crme_dependentmaintenance'>
                                    <attribute name = 'regardingobjectid' /> 
                                     <attribute name = 'udo_idproofid' />
                                     <filter type = 'and' > 
                                       <condition attribute = 'activityid' operator= 'eq' value = '" + entity.Id + @"' />
                                         </filter >     
                                       </entity >
                                     </fetch >";


            EntityCollection veteran = OrganizationService.RetrieveMultiple(new FetchExpression(veteranDetails));

            if (veteran.Entities[0].Contains("regardingobjectid"))
            {
                EntityReference veteranEntity = veteran.Entities[0].GetAttributeValue<EntityReference>("regardingobjectid");
                veteranID = veteranEntity.Id;
            }
            TracingService.Trace("Finished Fetching Veteran");
            TracingService.Trace("Veteran ID is : " + veteranID);

            if (veteran.Entities[0].Contains("udo_idproofid"))
            {
                EntityReference interactionEntity = veteran.Entities[0].GetAttributeValue<EntityReference>("udo_idproofid");
                interactionID = interactionEntity.Id;
            }


            TracingService.Trace("Finished Fetching Interaction ID");
            TracingService.Trace("Interaction ID is : " + interactionID);
            #endregion


            #region Participant ID
            var fetchParticipant = @"<fetch>
                                          <entity name='contact' >
                                            <attribute name='udo_participantid' />
                                            <filter>
                                              <condition attribute='contactid' operator='eq' value='" + veteranID + @"' />
                                            </filter>
                                          </entity>
                                        </fetch>";

            TracingService.Trace("Fetching participant id for the veteran - " + veteranID);
            EntityCollection participantRes = OrganizationService.RetrieveMultiple(new FetchExpression(fetchParticipant));

            foreach (var res in participantRes.Entities)
            {
                partipantIdContact = res.Attributes["udo_participantid"].ToString();
            }

            TracingService.Trace("Participant Id is " + partipantIdContact);
            #endregion

            #region Person ID
            var fetchPerson = @"<fetch>
                                  <entity name='udo_person' >
                                    <attribute name='udo_personid' />
                                    <filter type='and' >
                                      <condition attribute='udo_idproofid' operator='eq' value='" + interactionID + @"' />
                                      <condition attribute='udo_ptcpntid' operator='eq' value='" + partipantIdContact + @"' />
                                    </filter>
                                  </entity>
                                </fetch>";

            TracingService.Trace("Fetching person id");
            EntityCollection fetchPresonCol = OrganizationService.RetrieveMultiple(new FetchExpression(fetchPerson));

            if (fetchPresonCol.Entities[0].Contains("udo_personid"))
            {
                personID = fetchPresonCol.Entities[0].GetAttributeValue<Guid>("udo_personid");
            }

            TracingService.Trace("Person Id is: " + personID);
            #endregion


            #region Parsing dependent and Building note for "add" dependents and non school kids.
            foreach (var objDep in dependents.Entities)
            {
                OptionSetValue maintenanceType = new OptionSetValue();
                if (objDep.Attributes.Contains("crme_maintenancetype"))
                {
                    maintenanceType = objDep.GetAttributeValue<OptionSetValue>("crme_maintenancetype");
                }

                bool isChildInSchool = objDep.GetAttributeValue<bool>("crme_childage1823inschool");

                //this means dependent is an add and not school child
                if (maintenanceType.Value == 935950000 && isChildInSchool == false)
                {
                    string firstName = objDep.Attributes["crme_firstname"].ToString();
                    string lastName = objDep.Attributes["crme_lastname"].ToString();

                    OptionSetValue relationshipType = new OptionSetValue();
                    if (objDep.Attributes.Contains("crme_dependentrelationship"))
                    {
                        relationshipType = objDep.GetAttributeValue<OptionSetValue>("crme_dependentrelationship");
                    }

                    if (relationshipType.Value == 935950001)
                    {
                        if(objDep.Attributes.Contains("udo_marriageenddate"))//ex-spouse
                        {

                           
                            TracingService.Trace("Marriage end date is stamped ");

                        }
                        else
                        {
                         //   var marrEndDate = objDep.GetAttributeValue<DateTime>("udo_marriageenddate");
                         //   TracingService.Trace("Marriage Date is: " + marrEndDate);
                         //   TracingService.Trace("Marriage Date is: " + marrEndDate);
                            spouseCount = spouseCount + 1;
                            //spouseName =  "Spouse " + firstName + " " + lastName;
                            if (spouseCount == 1)
                            {
                                spouseName = "Spouse " + firstName + " " + lastName;
                            }
                            else
                            {
                                spouseName = spouseName + ", " + "\n" + "Spouse " + firstName + " " + lastName;
                            }
                        }
                    }
                    else
                    {
                        OptionSetValue childRelationshipValue = new OptionSetValue();
                        if (objDep.Attributes.Contains("crme_childrelationship"))
                        {
                            childRelationshipValue = objDep.GetAttributeValue<OptionSetValue>("crme_childrelationship");
                        }

                        if (childRelationshipValue.Value == 935950000)
                        {
                            childRelationshipName = "Biological";
                        }
                        if (childRelationshipValue.Value == 935950001)
                        {
                            childRelationshipName = "Step Child";
                        }
                        if (childRelationshipValue.Value == 935950002)
                        {
                            childRelationshipName = "Adopted";
                        }
                        childCount = childCount + 1;
                        if (childCount == 1)
                        {
                            childName = childRelationshipName + " Child " + firstName + " " + lastName;
                        }
                        else
                        {
                            childName = childName + ", " + "\n" + childRelationshipName + " Child " + firstName + " " + lastName;
                        }

                    }

                }

            }

            TracingService.Trace("Spouse Name: " + spouseName);
            TracingService.Trace("Child Name: " + childName);

            if (spouseCount >= 1 || childCount >= 1)
            {
                trackNoteTextAdd = true;
            }


            if (spouseCount >= 1) //spouse exists
            {
                if (childCount >= 1)
                { //child and spouse exists
                    NoteTextAdd = NoteTextAdd + "\n" + spouseName + " and " + "\n" + childName;
                }
                else //child doesnt exist and only spouse exists
                {
                    NoteTextAdd = NoteTextAdd + "\n" + spouseName;
                }
            }

            if ((childCount >= 1) && spouseCount == 0) //child exists and spouse doesnt exist
            {
                NoteTextAdd = NoteTextAdd + "\n" + childName;
            }


            TracingService.Trace("Note Text for adding dependents (spouse and non school child) is: " + NoteTextAdd);

            #endregion

            childCount = 0;
            spouseCount = 0;
            childName = "";
            spouseName = "";

            #region Parsing dependents and building note for "add" dependents and school age kids.
            foreach (var objDep in dependents.Entities)
            {
                OptionSetValue maintenanceType = new OptionSetValue();
                if (objDep.Attributes.Contains("crme_maintenancetype"))
                {
                    maintenanceType = objDep.GetAttributeValue<OptionSetValue>("crme_maintenancetype");
                }

                bool isChildInSchool = objDep.GetAttributeValue<bool>("crme_childage1823inschool");

                if (maintenanceType.Value == 935950000 && isChildInSchool == true)
                {
                    string firstName = objDep.Attributes["crme_firstname"].ToString();
                    string lastName = objDep.Attributes["crme_lastname"].ToString();

                    OptionSetValue childRelationshipValue = new OptionSetValue();
                    if (objDep.Attributes.Contains("crme_childrelationship"))
                    {
                        childRelationshipValue = objDep.GetAttributeValue<OptionSetValue>("crme_childrelationship");
                    }

                    if (childRelationshipValue.Value == 935950000)
                    {
                        childRelationshipName = "Biological";
                    }
                    if (childRelationshipValue.Value == 935950001)
                    {
                        childRelationshipName = "Step Child";
                    }
                    if (childRelationshipValue.Value == 935950002)
                    {
                        childRelationshipName = "Adopted";
                    }

                    childCount = childCount + 1;
                    if (childCount == 1)
                    {
                        childName = "School age Child " + childRelationshipName + " " + firstName + " " + lastName;
                    }
                    else
                    {
                        childName = childName + ", " + "\n" + "School age Child " + childRelationshipName + " " + firstName + " " + lastName;
                    }
                }
            }

            TracingService.Trace("Child Name: " + childName);

            if (childCount >= 1)
            {
                trackNoteTextNewSchChild = true;
            }

            if (childCount >= 1)
            {
                NoteTextNewSchChild = NoteTextNewSchChild + "\n" + childName;
            }

            TracingService.Trace("Note Text for adding school age child is: " + NoteTextNewSchChild);
            #endregion
            childCount = 0;
            spouseCount = 0;
            childName = "";
            spouseName = "";

            #region Parsing "remove" spouse
            foreach (var objDep in dependents.Entities)
            {
                OptionSetValue maintenanceType = new OptionSetValue();
                if (objDep.Attributes.Contains("crme_maintenancetype"))
                {
                    maintenanceType = objDep.GetAttributeValue<OptionSetValue>("crme_maintenancetype");
                }
                if (maintenanceType.Value == 935950001)
                {
                    OptionSetValue marriageTermReason = new OptionSetValue();
                    if (objDep.Attributes.Contains("udo_howwasmarriageterminated"))
                    {
                        marriageTermReason = objDep.GetAttributeValue<OptionSetValue>("udo_howwasmarriageterminated");
                    }

                    if (marriageTermReason.Value == 752280000)
                    {
                        marriageTerminationReason = "Divorce";
                    }
                    if (marriageTermReason.Value == 752280001)
                    {
                        marriageTerminationReason = "Death";
                    }

                    string firstName = objDep.Attributes["crme_firstname"].ToString();
                    string lastName = objDep.Attributes["crme_lastname"].ToString();

                    spouseCount = spouseCount + 1;
                    if (spouseCount == 1)
                    {
                        spouseName = " Spouse " + firstName + " " + lastName + " due to " + marriageTerminationReason;
                    }
                    else
                    {
                        spouseName = spouseName + ", " + "\n" + " Spouse " + firstName + " " + lastName + " due to " + marriageTerminationReason;
                    }

                }

            }
            TracingService.Trace("Spouse Name: " + spouseName);

            if (spouseCount >= 1)
            {
                trackNoteTextSpouseRemove = true;
            }

            if (spouseCount >= 1)
            {
                NoteTextSpouseRemove = NoteTextSpouseRemove + "\n" + spouseName;
            }

            TracingService.Trace("Note Text for removing Spouse is: " + NoteTextSpouseRemove);

            #endregion
            childCount = 0;
            spouseCount = 0;
            childName = "";
            spouseName = "";
            #region Parsing dependents and building note for "edit" school age dependents.
            foreach (var objDep in dependents.Entities)
            {
                OptionSetValue maintenanceType = new OptionSetValue();
                if (objDep.Attributes.Contains("crme_maintenancetype"))
                {
                    maintenanceType = objDep.GetAttributeValue<OptionSetValue>("crme_maintenancetype");
                }
                bool isChildInSchool = objDep.GetAttributeValue<bool>("crme_childage1823inschool");

                if (maintenanceType.Value == 752280000 && isChildInSchool == true)
                {
                    string firstName = objDep.Attributes["crme_firstname"].ToString();
                    string lastName = objDep.Attributes["crme_lastname"].ToString();

                    OptionSetValue childRelationshipValue = new OptionSetValue();
                    if (objDep.Attributes.Contains("crme_childrelationship"))
                    {
                        childRelationshipValue = objDep.GetAttributeValue<OptionSetValue>("crme_childrelationship");
                    }

                    if (childRelationshipValue.Value == 935950000)
                    {
                        childRelationshipName = "Biological";
                    }
                    if (childRelationshipValue.Value == 935950001)
                    {
                        childRelationshipName = "Step Child";
                    }
                    if (childRelationshipValue.Value == 935950002)
                    {
                        childRelationshipName = "Adopted";
                    }

                    childCount = childCount + 1;
                    if (childCount == 1)
                    {
                        childName = "School age Child " + childRelationshipName + " " + firstName + " " + lastName;
                    }
                    else
                    {
                        childName = childName + ", " + "\n" + "School age Child " + childRelationshipName + " " + firstName + " " + lastName;
                    }

                }
            }
            TracingService.Trace("Child Name: " + childName);
            if (childCount >= 1)
            {
                trackNoteText674 = true;
            }
            if (childCount >= 1)
            {
                NoteText674 = NoteText674 + "\n" + childName;
            }

            TracingService.Trace("Note Text for editing school age child is: " + NoteText674);
            #endregion

            childCount = 0;
            spouseCount = 0;
            childName = "";
            spouseName = "";

            #region Parsing spouse which Add and Remove at the same time
            foreach (var objDep in dependents.Entities)
            {
                OptionSetValue maintenanceType = new OptionSetValue();
                if (objDep.Attributes.Contains("crme_maintenancetype"))
                {
                    maintenanceType = objDep.GetAttributeValue<OptionSetValue>("crme_maintenancetype");
                }

                if(maintenanceType.Value == 935950000)
                {
                    OptionSetValue relationshipType = new OptionSetValue();
                    if (objDep.Attributes.Contains("crme_dependentrelationship"))
                    {
                        relationshipType = objDep.GetAttributeValue<OptionSetValue>("crme_dependentrelationship");
                    }
                    if (relationshipType.Value == 935950001)
                    {
                        if (objDep.Attributes.Contains("udo_marriageenddate"))// stampoed as ex spouse as well
                        {
                            var marrEndDate = objDep.GetAttributeValue<DateTime>("udo_marriageenddate");
                            string firstName = objDep.Attributes["crme_firstname"].ToString();
                            string lastName = objDep.Attributes["crme_lastname"].ToString();

                            TracingService.Trace("Marriage Date is: " + marrEndDate);
                            TracingService.Trace("Spouse First Name is: " + firstName);
                            TracingService.Trace("Spouse Last  Name is: " + lastName);
                            spouseCount = spouseCount + 1;
                            //spouseName =  "Spouse " + firstName + " " + lastName;
                            if (spouseCount == 1)
                            {
                                spouseName = "Spouse " + firstName + " " + lastName;
                            }
                            else
                            {
                                spouseName = spouseName + ", " + "\n" + "Spouse " + firstName + " " + lastName;
                            }

                            

                        }
                        else
                        {
                            TracingService.Trace("Marriage end date is not stamped ");
                            //do nothing here as we have already handled it in add spouse
                        }

                    }
                    else
                    {
                        //do nothing for child as we have already handled it.
                    }
                }

            }

            TracingService.Trace("Spouse Name: " + spouseName);
            if (spouseCount >= 1)
            {
                trackNoteTextSpouseAddRemove = true;
            }
            if (spouseCount >= 1)
            {
                NoteTextSpouseAddRemove = NoteTextSpouseAddRemove + "\n" + spouseName;
            }

            TracingService.Trace("Note Text for adding and removing spouse at the same time is : " + NoteTextSpouseAddRemove);
            #endregion

            #region Construct Note Text
            NoteText = "\n";

            if (trackNoteTextAdd == true)
            {
                NoteText = NoteTextAdd + "\n" + "\n";
            }

            if (trackNoteTextSpouseRemove == true)
            {
                NoteText = NoteText +  NoteTextSpouseRemove + "\n" + "\n";
            }

            //if(trackNoteTextSpouseAddRemove == true)
            //{
            //    NoteText = NoteText + NoteTextSpouseAddRemove + "\n" + "\n";
            //}

            if (trackNoteTextNewSchChild == true)
            {
                NoteText = NoteText + NoteTextNewSchChild + "\n" + "\n";
            }

            if (trackNoteText674 == true)
            {
                NoteText = NoteText + NoteText674 + "\n";
            }

            #endregion

            TracingService.Trace("Value of final Note text is:" + NoteText);
            Entity currentUser = OrganizationService.Retrieve("systemuser", PluginExecutionContext.InitiatingUserId, new ColumnSet("va_filenumber", "va_stationnumber", "fullname"));



            #region createnote
            Entity note = new Entity("udo_note");

            note["udo_fromudo"] = true;
            note["udo_veteranid"] = new EntityReference("contact", veteranID);
            note["udo_participantid"] = partipantIdContact;
            note["udo_personid"] = new EntityReference("udo_person", personID);
            note["udo_notetext"] = NoteText;
            note["udo_name"] = "New Note";
            note["udo_datetime"] = DateTime.UtcNow.ToString();
            note["udo_ro"] = currentUser.GetAttributeValue<string>("va_stationnumber");
            note["udo_userid"] = currentUser.GetAttributeValue<string>("va_filenumber");
            note["udo_user"] = currentUser.GetAttributeValue<string>("fullname");

            TracingService.Trace("creating note");
            var noteId = OrganizationService.Create(note);
            #endregion
            TracingService.Trace("Note created. Note ID is: " + noteId);

            //set output parameters
            if (noteId != null)
            {
                SetOutputParameter("NoteID", noteId);
                SetOutputParameter("NoteContent", NoteText);
                SetOutputParameter("Exception", false);
            }


        }

        private void FNODNotes(EntityReference entity)
        {
            TracingService.Trace("FNOD");

            string NoteText = "Note submitted through FNOD";

            var veteranID = new Guid();
            var interactionID = new Guid();
            var personID = new Guid();
            string partipantIdContact = string.Empty;

            #region Reading veteran ID and ID Proof
            var veteranDetails = @"<fetch>
                  <entity name='va_fnod' >
                    <attribute name='va_veterancontactid' />
                    <attribute name='udo_idproof' />
                    <attribute name='udo_summarynote' />
                    <filter>
                      <condition attribute='va_fnodid' operator= 'eq' value = '" + entity.Id + @"' />
                    </filter>
                  </entity>
                </fetch>";

            EntityCollection veteran = OrganizationService.RetrieveMultiple(new FetchExpression(veteranDetails));

            if (veteran.Entities[0].Contains("va_veterancontactid"))
            {
                EntityReference veteranEntity = veteran.Entities[0].GetAttributeValue<EntityReference>("va_veterancontactid");
                veteranID = veteranEntity.Id;
            }
            TracingService.Trace("Finished Fetching Veteran");
            TracingService.Trace("Veteran ID is : " + veteranID);

            if (veteran.Entities[0].Contains("udo_idproof"))
            {
                EntityReference interactionEntity = veteran.Entities[0].GetAttributeValue<EntityReference>("udo_idproof");
                interactionID = interactionEntity.Id;
            }

            if (veteran.Entities[0].Contains("udo_summarynote"))
            {
                string summaryNote = veteran.Entities[0].GetAttributeValue<string>("udo_summarynote");
                TracingService.Trace("Summary Note: " + summaryNote);
                NoteText = NoteText + "\n" + summaryNote;
            }

            TracingService.Trace("Finished Fetching Interaction ID");
            TracingService.Trace("Interaction ID is : " + interactionID);
            #endregion


            #region Participant ID
            var fetchParticipant = @"<fetch>
                                          <entity name='contact' >
                                            <attribute name='udo_participantid' />
                                            <filter>
                                              <condition attribute='contactid' operator='eq' value='" + veteranID + @"' />
                                            </filter>
                                          </entity>
                                        </fetch>";

            TracingService.Trace("Fetching participant id for the veteran - " + veteranID);
            EntityCollection participantRes = OrganizationService.RetrieveMultiple(new FetchExpression(fetchParticipant));

            foreach (var res in participantRes.Entities)
            {
                partipantIdContact = res.Attributes["udo_participantid"].ToString();
            }

            TracingService.Trace("Participant Id is " + partipantIdContact);
            #endregion

            #region Person ID
            var fetchPerson = @"<fetch>
                                  <entity name='udo_person' >
                                    <attribute name='udo_personid' />
                                    <filter type='and' >
                                      <condition attribute='udo_idproofid' operator='eq' value='" + interactionID + @"' />
                                      <condition attribute='udo_ptcpntid' operator='eq' value='" + partipantIdContact + @"' />
                                    </filter>
                                  </entity>
                                </fetch>";

            TracingService.Trace("Fetching person id");
            EntityCollection fetchPresonCol = OrganizationService.RetrieveMultiple(new FetchExpression(fetchPerson));

            if (fetchPresonCol.Entities[0].Contains("udo_personid"))
            {
                personID = fetchPresonCol.Entities[0].GetAttributeValue<Guid>("udo_personid");
            }

            TracingService.Trace("Person Id is: " + personID);
            #endregion

            Entity currentUser = OrganizationService.Retrieve("systemuser", PluginExecutionContext.InitiatingUserId, new ColumnSet("va_filenumber", "va_stationnumber", "fullname"));

            #region createnote
            Entity note = new Entity("udo_note");

            note["udo_fromudo"] = true;
            note["udo_veteranid"] = new EntityReference("contact", veteranID);
            note["udo_participantid"] = partipantIdContact;
            note["udo_personid"] = new EntityReference("udo_person", personID);
            note["udo_notetext"] = NoteText;
            note["udo_name"] = "New Note";
            note["udo_datetime"] = DateTime.UtcNow.ToString();
            note["udo_ro"] = currentUser.GetAttributeValue<string>("va_stationnumber");
            note["udo_userid"] = currentUser.GetAttributeValue<string>("va_filenumber");
            note["udo_user"] = currentUser.GetAttributeValue<string>("fullname");

            TracingService.Trace("creating note");
            var noteId = OrganizationService.Create(note);
            #endregion
            TracingService.Trace("Note created. Note ID is: " + noteId);

            //set output parameters
            if (noteId != null)
            {
                SetOutputParameter("NoteID", noteId);
                SetOutputParameter("NoteContent", NoteText);
                SetOutputParameter("Exception", false);
            }

        }

        private void POINotes(EntityReference entity)
        {
            TracingService.Trace("POI");

            string NoteText = "Note submitted through POI";

            var veteranID = new Guid();
            var interactionID = new Guid();
            var personID = new Guid();
            string partipantIdContact = string.Empty;
            var entityId = entity.Id;

            if(entityId != Guid.Empty)
            {
                #region Reading veteran ID and ID Proof
                var veteranDetails = @"<fetch>
                  <entity name='udo_pointofinteraction' >
                    <attribute name = 'udo_requestedaction' />
                    <attribute name = 'udo_description' />
                    <attribute name = 'statecode' />
                    <attribute name = 'statuscode' />
                    <attribute name = 'udo_rejectionreason' />
                    <attribute name = 'udo_rejectiondescription' />
                    <attribute name = 'udo_previousstatus' />
                    <filter>
                      <condition attribute='udo_pointofinteractionid' operator= 'eq' value = '" + entity.Id + @"' />
                    </filter>
                    <link-entity name='contact' from='contactid' to='udo_veteran' link-type='inner' >
                      <attribute name='contactid' />
                    </link-entity>
                    <link-entity name='udo_idproof' from='udo_idproofid' to='udo_idproof' link-type='inner' >
                      <attribute name='udo_idproofid' />
                    </link-entity>
                  </entity>
                </fetch>";

                EntityCollection veteranCollection = OrganizationService.RetrieveMultiple(new FetchExpression(veteranDetails));
                Entity poi = veteranCollection.Entities[0];
                if (poi.Contains("contact1.contactid"))
                {
                    //EntityReference veteranEntity = veteran.Entities[0].GetAttributeValue<EntityReference>("contactid");
                    veteranID = (Guid)((AliasedValue)poi.Attributes["contact1.contactid"]).Value;
                }
                TracingService.Trace("Finished Fetching Veteran");
                TracingService.Trace("Veteran ID is : " + veteranID);

                if (poi.Contains("udo_idproof2.udo_idproofid"))
                {
                    // EntityReference interactionEntity = veteran.Entities[0].GetAttributeValue<EntityReference>("udo_idproofid");
                    interactionID = (Guid)((AliasedValue)poi.Attributes["udo_idproof2.udo_idproofid"]).Value;
                }

                if (poi.Contains("udo_requestedaction"))
                {
                    string requestedAction = poi.FormattedValues["udo_requestedaction"];

                    TracingService.Trace("Summary Note: " + requestedAction);
                    NoteText = NoteText + "\n" + "Requested Action: " + requestedAction;
                }

                if (poi.Contains("udo_description"))
                {
                    string summaryNote = poi.GetAttributeValue<string>("udo_description");
                    TracingService.Trace("Summary Note: " + summaryNote);
                    NoteText = NoteText + "\n" + "Description: " + summaryNote;
                }

                string statusReason = poi.FormattedValues["statuscode"];
                if (poi.Contains("udo_previousstatus") && poi.GetAttributeValue<string>("udo_previousstatus").Contains("Rejected") 
                    && statusReason == "Pending")
                {
                    TracingService.Trace("Status Reason: Resubmitted");
                    NoteText = NoteText + "\nStatus: Resubmitted";      
                }
                else if (statusReason == "Pending")
                {
                    TracingService.Trace("Status Reason: " + statusReason);
                    NoteText = NoteText + "\n" + "Status: Submitted";
                }
                else
                {
                    TracingService.Trace("Status Reason: " + statusReason);
                    NoteText = NoteText + "\n" + "Status: " + statusReason;
                }
                

                if (statusReason == "Rejected - Additional Action Needed")
                {
                    string rejectionReason = poi.FormattedValues["udo_rejectionreason"];
                    string rejectionDescription = poi.GetAttributeValue<string>("udo_rejectiondescription");
                    TracingService.Trace("Rejection Reason: " + rejectionReason + "\n" + "Rejection Description: " + rejectionDescription);

                    NoteText = NoteText + "\n" + "Rejection Reason: " + rejectionReason + "\n" + "Rejection Description: " + rejectionDescription;

                }

                TracingService.Trace("Finished Fetching Interaction ID");
                TracingService.Trace("Interaction ID is : " + interactionID);
                #endregion
            }

            //I don't believe this section will ever be called, but I will leave the code here just in case
            //Potential TODO - remove this block of code
            else //this means POI ID is blank
            {
                EntityReference veteranEntity = (EntityReference)PluginExecutionContext.InputParameters["VeteranPOI"];
                TracingService.Trace("Veteran ID is: " + veteranEntity.Id);
                EntityReference idproofEntity = (EntityReference)PluginExecutionContext.InputParameters["IdProofPOI"];
                TracingService.Trace("Id Proof ID is: " + idproofEntity.Id);

                String description = (String)PluginExecutionContext.InputParameters["POI_Description"];
                String requestedAction = (String)PluginExecutionContext.InputParameters["POIRequestedAction"];

               
                veteranID = veteranEntity.Id;
                interactionID = idproofEntity.Id;
                string requestedActionText = "";

                switch (requestedAction)
                {
                    case "751880000":
                        requestedActionText = "Claim Establishment";
                        break;
                    case "751880001":
                        requestedActionText = "DOB correction";
                        break;
                    case "751880002":
                        requestedActionText = "Name correction";
                        break;
                    case "751880003":
                        requestedActionText = "Service Information correction";
                        break;
                    case "751880004":
                        requestedActionText = "Stop Award";
                        break;
                    case "751880005":
                        requestedActionText = "Start Award";
                        break;
                    case "751880006":
                        requestedActionText = "Resume Award";
                        break;
                    case "751880007":
                        requestedActionText = "Denied Claim for failure to report to C&P Exam - COVID 19";
                        break;
                    default:
                        break;
                }

                NoteText = NoteText + "\n" + "Requested Action: " + requestedActionText;

                NoteText = NoteText + "\n" + "Description: " + description;


            }

                        #region Participant ID
            var fetchParticipant = @"<fetch>
                                          <entity name='contact' >
                                            <attribute name='udo_participantid' />
                                            <filter>
                                              <condition attribute='contactid' operator='eq' value='" + veteranID + @"' />
                                            </filter>
                                          </entity>
                                        </fetch>";

            TracingService.Trace("Fetching participant id for the veteran - " + veteranID);
            EntityCollection participantRes = OrganizationService.RetrieveMultiple(new FetchExpression(fetchParticipant));

            foreach (var res in participantRes.Entities)
            {
                partipantIdContact = res.Attributes["udo_participantid"].ToString();
            }

            TracingService.Trace("Participant Id is " + partipantIdContact);
            #endregion

            #region Person ID
            var fetchPerson = @"<fetch>
                                  <entity name='udo_person' >
                                    <attribute name='udo_personid' />
                                    <filter type='and' >
                                      <condition attribute='udo_idproofid' operator='eq' value='" + interactionID + @"' />
                                      <condition attribute='udo_ptcpntid' operator='eq' value='" + partipantIdContact + @"' />
                                    </filter>
                                  </entity>
                                </fetch>";

            TracingService.Trace("Fetching person id");
            EntityCollection fetchPresonCol = OrganizationService.RetrieveMultiple(new FetchExpression(fetchPerson));

            if (fetchPresonCol.Entities[0].Contains("udo_personid"))
            {
                personID = fetchPresonCol.Entities[0].GetAttributeValue<Guid>("udo_personid");
            }

            TracingService.Trace("Person Id is: " + personID);
            #endregion

            Entity currentUser = OrganizationService.Retrieve("systemuser", PluginExecutionContext.InitiatingUserId, new ColumnSet("va_filenumber", "va_stationnumber", "fullname"));

            #region createnote
            Entity note = new Entity("udo_note");

            note["udo_fromudo"] = true;
            note["udo_veteranid"] = new EntityReference("contact", veteranID);
            note["udo_participantid"] = partipantIdContact;
            note["udo_personid"] = new EntityReference("udo_person", personID);
            note["udo_notetext"] = NoteText;
            note["udo_name"] = "New Note";
            note["udo_datetime"] = DateTime.UtcNow.ToString();
            note["udo_ro"] = currentUser.GetAttributeValue<string>("va_stationnumber");
            note["udo_userid"] = currentUser.GetAttributeValue<string>("va_filenumber");
            note["udo_user"] = currentUser.GetAttributeValue<string>("fullname");

            TracingService.Trace("creating note");
            var noteId = OrganizationService.Create(note);
            #endregion
            TracingService.Trace("Note created. Note ID is: " + noteId);

            //set output parameters
            if (noteId != null)
            {
                SetOutputParameter("NoteID", noteId);
                SetOutputParameter("NoteContent", NoteText);
                SetOutputParameter("Exception", false);
            }

        }
    }
}
