using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.VASS.Plugins
{
    public class CreateVassTagsHistoryRecordRunner : PluginRunner
    {
        private IServiceProvider serviceProvider;
        public CreateVassTagsHistoryRecordRunner(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public void Execute()
        {
            try
            {
                string relationshipName = string.Empty;
                if (PluginExecutionContext.InputParameters.Contains("Relationship"))
                {
                    relationshipName = PluginExecutionContext.InputParameters["Relationship"].ToString();
                    TracingService.Trace($"Relationship Name :{relationshipName}");
                }

                if (PluginExecutionContext.MessageName == "Associate" && relationshipName == "udo_udo_vasstag_udo_interaction.Referenced")
                {
                    EntityReferenceCollection relatedEntities = null;
                    EntityReference relatedEntity = null;
                    EntityReference targetReference = null;

                    if (PluginExecutionContext.InputParameters.Contains("Target") && PluginExecutionContext.InputParameters["Target"] is EntityReference)
                    {
                        TracingService.Trace("Capture Interaction Details");
                        targetReference = (EntityReference)PluginExecutionContext.InputParameters["Target"];
                        TracingService.Trace($"The Interaction Id is {targetReference.Id}");
                        //Get Interaction Information
                        string fetchXmltoGetInteractionDetails = $@"<fetch top='1' no-lock='true' >
  <entity name='udo_interaction' >
     <attribute name='udo_veteranfirstname' />
    <attribute name='udo_veteranlastname' />
    <attribute name='udo_veteranssn' />
    <attribute name='udo_mheointeractionassignmentid' />
    <filter>
      <condition attribute='udo_interactionid' operator='eq' value='{targetReference.Id}' />
    </filter>
  </entity>
</fetch>";

                        FetchExpression fetchExpression = new FetchExpression(fetchXmltoGetInteractionDetails);
                      
                        string firstName = string.Empty;
                        string lastName = string.Empty;
                        string ssn = string.Empty;
                        EntityReference interactionAssignmentId = new EntityReference();
                        EntityCollection interactionInformation = OrganizationService.RetrieveMultiple(fetchExpression);
                        TracingService.Trace("FeatchXML in correct format and response recieved ");
                        if (interactionInformation.Entities != null)
                        {
                            foreach (var item in interactionInformation.Entities[0].Attributes)
                            {
                                TracingService.Trace($"Attribute Key {item.Key} Value {item.Value}");
                            }
                            if (interactionInformation.Entities[0].Contains("udo_veteranfirstname"))
                            {
                                firstName = interactionInformation.Entities[0].GetAttributeValue<string>("udo_veteranfirstname");
                            }
                            if (interactionInformation.Entities[0].Contains("udo_veteranlastname"))
                            {
                                lastName = interactionInformation.Entities[0].GetAttributeValue<string>("udo_veteranlastname");
                            }
                            if (interactionInformation.Entities[0].Contains("udo_veteranssn"))
                            {
                                ssn = interactionInformation.Entities[0].GetAttributeValue<string>("udo_veteranssn");
                            }
                            if (interactionInformation.Entities[0].Contains("udo_mheointeractionassignmentid"))
                            {
                                interactionAssignmentId = interactionInformation.Entities[0].GetAttributeValue<EntityReference>("udo_mheointeractionassignmentid");
                            }
                        }

                        if (PluginExecutionContext.InputParameters.Contains("RelatedEntities") && PluginExecutionContext.InputParameters["RelatedEntities"] is EntityReferenceCollection)
                        {

                            relatedEntities = PluginExecutionContext.InputParameters["RelatedEntities"] as EntityReferenceCollection;
                            relatedEntity = relatedEntities[0];
                            TracingService.Trace($"the tag id {relatedEntity.Id}");

                        }
                        Entity vassTagsAnalysis = new Entity("udo_vasstagsanalysis");
                        vassTagsAnalysis["udo_interaction"] = new EntityReference("udo_interaction", targetReference.Id);
                        vassTagsAnalysis["udo_tag"] = new EntityReference("udo_vasstag", relatedEntity.Id);
                        vassTagsAnalysis["udo_associatedby"] = new EntityReference("systemuser", PluginExecutionContext.InitiatingUserId);
                        vassTagsAnalysis["udo_associateddate"] = PluginExecutionContext.OperationCreatedOn;
                        vassTagsAnalysis["udo_veteranfirstname"] = firstName;
                        vassTagsAnalysis["udo_veteranlastname"] = lastName;
                        vassTagsAnalysis["udo_veteranssn"] = ssn;
                        vassTagsAnalysis["udo_interactionassignment"] = new EntityReference("udo_mheointeractionassignment", interactionAssignmentId.Id);
                        vassTagsAnalysis["udo_name"] = $"Tag Record : {PluginExecutionContext.OperationCreatedOn.ToString()}";
                        TracingService.Trace("All the values set");
                        OrganizationService.Create(vassTagsAnalysis);

                    }

                }
            }
            catch (Exception ex)
            {

                TracingService.Trace($"Exception : {ex.Message}");
            }
           
        }
    }
}
