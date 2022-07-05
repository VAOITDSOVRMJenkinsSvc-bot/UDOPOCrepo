using MCSPlugins;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;


namespace Va.Udo.Crm.Plugins.Request
{
    public class RequestPostCreateRunner : PluginRunner
    {
                /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceProvider"></param>
        internal RequestPostCreateRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        /// <summary>
        /// Retrieves the primary entity
        /// </summary>
        /// <returns>Primary entity</returns>
        public override Microsoft.Xrm.Sdk.Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.InputParameters.ContainsKey("Target") ? PluginExecutionContext.InputParameters["Target"] as Entity : null;
        }

        /// <summary>
        /// Retrieves the secondary entity
        /// </summary>
        /// <returns></returns>
        public override Microsoft.Xrm.Sdk.Entity GetSecondaryEntity()
        {
            return PluginExecutionContext.InputParameters["Target"] as Entity;
        }

        /// <summary>
        /// Debug field
        /// </summary>
        public override string McsSettingsDebugField
        {
            get { return ""; }
        }


        /// <summary>
        /// Sets the repeat call field if the veteran has called for the same request type, subtype in the last two weeks
        /// </summary>
        internal void Execute()
        {
            var progressStatus = "Start: RequestPostCreateRunner";
            Trace("Start: RequestPostCreateRunner");
            Trace("Getting Pre-Entity.");
            var entity = GetPrimaryEntity();
            if (entity == null)
                throw new InvalidPluginExecutionException("Target entity is null");


            Trace("Target is not null.");
            if (!string.Equals(entity.LogicalName, "udo_request", StringComparison.InvariantCultureIgnoreCase))
                throw new InvalidPluginExecutionException("This plugin is not configured for udo_request.");
            Entity image = PluginExecutionContext.PostEntityImages["PostCreateImage"];

            Entity veteran = null;
            Entity person = null;
            ///TODO: Get ID Proof ID from Target
            var idProofRef = entity.GetAttributeValue<EntityReference>("udo_idproof");

            //Get Veteran id from Target and Retrieve Contact for SSN and Participant ID
            var veteranRef = entity.GetAttributeValue<EntityReference>("udo_veteran");

            //Get Request Type and Sub Types
            var requestTypeRef = entity.GetAttributeValue<EntityReference>("udo_type");
            var requestSubTypeRef = entity.GetAttributeValue<EntityReference>("udo_subtype");

            try
            {

                var requestType = OrganizationService.Retrieve(requestTypeRef.LogicalName, requestTypeRef.Id, new ColumnSet("udo_name"));
                var requestSubType = OrganizationService.Retrieve(requestSubTypeRef.LogicalName, requestSubTypeRef.Id, new ColumnSet("udo_name"));


                if (veteranRef != null)
                {
                    var fetchVeteranXml = String.Format(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                                          <entity name='contact'>
                                                            <attribute name='fullname' />
                                                            <attribute name='contactid' />
                                                            <attribute name='udo_ssn' />
                                                            <attribute name='udo_participantid' />
                                                            <order attribute='fullname' descending='false' />
                                                            <filter type='and'>
                                                              <condition attribute='contactid' operator='eq' value='{0}' />
                                                            </filter>
                                                          </entity>
                                                        </fetch>", veteranRef.Id);

                    progressStatus = "Retrieving Veteran record";
                    var retrievedVeteranResults = OrganizationService.RetrieveMultiple(new FetchExpression(fetchVeteranXml));
                    progressStatus = "Retrieved Veteran record";
                    veteran = retrievedVeteranResults.Entities.FirstOrDefault();
                }

                ///TODO: Get udo_person record based on SSN and ID Proof ID
                if (idProofRef != null && veteran != null)
                {
                    var fetchXML = String.Format(@"<?xml version='1.0'?>
                                                <fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0'>
                                                <entity name='udo_person'>
                                                <attribute name='udo_personid'/>
                                                <attribute name='udo_name'/>
                                                <attribute name='createdon'/>
                                                <attribute name='udo_vetlastname'/>
                                                <attribute name='udo_vetfirstname'/>
                                                <attribute name='udo_veteranid'/>
                                                <attribute name='udo_ssn'/>
                                                <filter type='and'>
                                                <condition attribute='udo_idproofid' value='{0}' operator='eq'/>
                                                <condition attribute='udo_ssn' value='{1}' operator='eq'/>
                                                </filter>
                                                </entity>
                                                </fetch>", idProofRef.Id, veteran.GetAttributeValue<string>("udo_ssn"));

                    progressStatus = "Retrieving Person record";
                    var retrievePersonResults = OrganizationService.RetrieveMultiple(new FetchExpression(fetchXML));
                    progressStatus = "Retrieved Person record";

                    person = retrievePersonResults.Entities.FirstOrDefault();
                }
                else
                    ///Need to bail we have no way of retrieving the person to create the note with.
                    return;


                if (person == null)
                    ///Need to bail we have no person record to create not with.
                    return;

                Entity contact = new Entity("contact");
                contact.Id = entity.GetAttributeValue<EntityReference>("udo_veteran").Id;
                contact["udo_lastcalldatetime"] = entity.GetAttributeValue<DateTime>("createdon");
                contact["udo_lastcalltype"] = image.GetAttributeValue<EntityReference>("udo_type");
                contact["udo_lastcallsubtype"] = image.GetAttributeValue<EntityReference>("udo_subtype");
                //  contact["udo_lastcalltype"] = requestType.GetAttributeValue<string>("udo_name");
                //contact["udo_lastcallsubtype"] = requestSubType.GetAttributeValue<string>("udo_name");

                progressStatus = "Updating Veteran record";
                OrganizationService.Update(contact);
                progressStatus = "Updated Veteran record";

                ///Build Note to create based on Request
                var newNote = new Entity("udo_note");
                newNote["udo_fromudo"] = true;
                newNote["udo_veteranid"] = veteranRef;
                newNote["udo_personid"] = person.ToEntityReference();
                newNote["udo_participantid"] = veteran.GetAttributeValue<string>("udo_participantid");
                newNote["udo_notetext"] = String.Format(@" UDO: {0} / {1} for {2}", requestType.GetAttributeValue<string>("udo_name"), requestSubType.GetAttributeValue<string>("udo_name"), veteran.GetAttributeValue<string>("fullname"));

                progressStatus = "Creating Note";
                OrganizationService.Create(newNote);
                progressStatus = "Created Note";

            }
            catch (Exception ex)
            {
                PluginError = true;
                Logger.WriteToFile("Va.Udo.Crm.Plugins.Request.RequestPostCreateRunner" + Environment.NewLine + Environment.NewLine + 
                    "Proogress Status: " + progressStatus + " - " + ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace.ToString());
                //Trace the error
                Trace(string.Format("{0}", ex.Message));
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
    }
}
