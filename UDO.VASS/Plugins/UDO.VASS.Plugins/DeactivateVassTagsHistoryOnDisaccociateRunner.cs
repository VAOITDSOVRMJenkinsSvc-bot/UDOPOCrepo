using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDO.VASS.Plugins
{
    public class DeactivateVassTagsHistoryOnDisaccociateRunner :PluginRunner
    {
        private IServiceProvider serviceProvider;

        public DeactivateVassTagsHistoryOnDisaccociateRunner(IServiceProvider serviceProvider) : base(serviceProvider)
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

                if (PluginExecutionContext.MessageName == "Disassociate" && relationshipName == "udo_udo_vasstag_udo_interaction.Referenced")
                {
                    EntityReferenceCollection relatedEntities = null;
                    EntityReference relatedEntity = null;
                    EntityReference targetReference = null;

                    if (PluginExecutionContext.InputParameters.Contains("Target") && PluginExecutionContext.InputParameters["Target"] is EntityReference)
                    {
                        TracingService.Trace("Capture Interaction Details");
                        targetReference = (EntityReference)PluginExecutionContext.InputParameters["Target"];
                        TracingService.Trace($"The Interaction Id is {targetReference.Id}");
                        if (PluginExecutionContext.InputParameters.Contains("RelatedEntities") && PluginExecutionContext.InputParameters["RelatedEntities"] is EntityReferenceCollection)
                        {
                            relatedEntities = PluginExecutionContext.InputParameters["RelatedEntities"] as EntityReferenceCollection;
                            relatedEntity = relatedEntities[0];
                            TracingService.Trace($"the tag id {relatedEntity.Id}");
                        }
                        string fetchXmltoGetVASSTagsHistory = $@"<fetch top='50' no-lock='true' >
  <entity name='udo_vasstagsanalysis' >
     <attribute name='udo_tag' />
    <attribute name='statecode' />
    <attribute name='udo_vasstagsanalysisid' />
    <filter>
      <condition attribute='statecode' operator='eq' value='0' />
      <condition attribute='udo_interaction' operator='eq' value='{targetReference.Id}' />
      <condition attribute='udo_tag' operator='eq' value='{relatedEntity.Id}' />
    </filter>
  </entity>
</fetch>";

                        FetchExpression fetchExpression = new FetchExpression(fetchXmltoGetVASSTagsHistory);
                        TracingService.Trace("no problem with xml query");
                        EntityCollection tagsInformation = OrganizationService.RetrieveMultiple(fetchExpression);

                        if (tagsInformation.Entities != null && tagsInformation.Entities.Count>0)
                        {
                            foreach (var item in tagsInformation.Entities[0].Attributes)
                            {
                                TracingService.Trace($"Attribute Key : {item.Key}  Attribute Value : {item.Value}");
                            }
                            if (tagsInformation.Entities[0].Contains("udo_vasstagsanalysisid"))
                            {
                                TracingService.Trace("Update state code");
                                var tagsHistoryId = tagsInformation.Entities[0].GetAttributeValue<Guid>("udo_vasstagsanalysisid");
                                Entity tagRecord = new Entity("udo_vasstagsanalysis", tagsHistoryId);
                                tagRecord["statecode"] = new OptionSetValue(1);
                                TracingService.Trace("Record status set to Inactive");
                                OrganizationService.Update(tagRecord);
                            }
                            else
                            {
                                TracingService.Trace("Tag record not found to deactivate the record");
                            }
                        }

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
