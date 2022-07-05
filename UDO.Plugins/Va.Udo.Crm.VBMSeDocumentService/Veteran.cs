using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRM.Integration.UDO.VBMS.Plugins
{
    public class Veteran
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FileNumber { get; set; }
        public string ClaimNumber { get; set; }
        public Guid docType { get; set; }
        public EntityReference RelatedObject { get; set; }
       
        
        public Veteran()
        {
            RelatedObject = null;
        }

        private string GetValue(Entity record, string attribute)
        {
            string result = null;
            if (!record.Contains(attribute)) return null;
            if (record[attribute] is AliasedValue)
            {
                result = ((AliasedValue)record[attribute]).Value.ToString().Trim();
            }
            else
            {
                result = record[attribute].ToString().Trim();
            }
            if (String.IsNullOrEmpty(result)) return null;
            return result;
        }
        private Guid GetGuidValue(Entity record, string attribute)
        {
            Guid result = Guid.Empty;
            if (!record.Contains(attribute)) return Guid.Empty;
            result = ((EntityReference)record[attribute]).Id;
            
            return result;
        }
        public Veteran(IOrganizationService service, EntityReference docReference)
        {
            var doc = service.Retrieve(docReference.LogicalName, docReference.Id, new ColumnSet("udo_claimnumber", "udo_filenumber", "udo_firstname", "udo_lastname", "udo_middlename", "udo_servicerequestid", "udo_lettergenerationid", "udo_vbmsuploadrole", "udo_vbmsdocumenttype", "udo_fnodid"));

            var srRef = doc.GetAttributeValue<EntityReference>("udo_servicerequestid");
            var lgRef = doc.GetAttributeValue<EntityReference>("udo_lettergenerationid");
            var fnodRef = doc.GetAttributeValue<EntityReference>("udo_fnodid");

            if (srRef == null && lgRef == null && fnodRef == null)
            {
                this.FirstName = GetValue(doc, "udo_firstname");
                this.MiddleName = GetValue(doc, "udo_middlename");
                this.LastName = GetValue(doc, "udo_lastname");
                this.FileNumber = GetValue(doc, "udo_filenumber");
                this.ClaimNumber = GetValue(doc, "udo_claimnumber");
                this.docType = GetGuidValue(doc, "udo_vbmsdocumenttype");
            }
            else if (srRef != null)
            {
                RelatedObject = srRef;
                // get data from serviceRequest

                var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                               @"<entity name='udo_servicerequest'>" +
                               @"<attribute name='udo_servicerequestid' />" +
                               @"<attribute name='udo_vbmsdoctype' />" +
                               @"<attribute name='udo_filenumber' />" +
                               @"<attribute name='udo_claimnumber' />" +
                               @"<filter type='and'>" +
                               @"<condition attribute='udo_servicerequestid' operator='eq' value='" + srRef.Id.ToString() + "' />" +
                               @"</filter>" +
                               @"<link-entity name='contact' from='contactid' to='udo_relatedveteranid' visible='false' link-type='outer' alias='vet'>" +
                               @"<attribute name='middlename' />" +
                               @"<attribute name='lastname' />" +
                               @"<attribute name='firstname' />" +
                               @"</link-entity>" +
                               @"</entity>" +
                               @"</fetch>";

                var vetInfo = service.RetrieveMultiple(new FetchExpression(fetchXml)).Entities[0];

                FirstName = GetValue(vetInfo, "vet.firstname");
                MiddleName = GetValue(vetInfo, "vet.middlename");
                LastName = GetValue(vetInfo, "vet.lastname");
                FileNumber = GetValue(vetInfo, "udo_filenumber");
                ClaimNumber = GetValue(vetInfo, "udo_claimnumber");
                docType = GetGuidValue(vetInfo, "udo_vbmsdoctype");

                doc["udo_firstname"] = FirstName;
                doc["udo_middlename"] = MiddleName;
                doc["udo_lastname"] = LastName;
                doc["udo_filenumber"] = FileNumber;
                doc["udo_claimnumber"] = ClaimNumber;
                service.Update(doc);
            }
            else if (lgRef != null)
            {
                RelatedObject = lgRef;
                // get data from lettergeneration

                var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                               @"<entity name='udo_lettergeneration'>" +
                               @"<attribute name='udo_lettergenerationid' />" +
                               @"<attribute name='udo_vbmsdoctype' />" +
                               @"<attribute name='udo_filenumber' />" +
                               @"<attribute name='udo_claimnumber' />" +
                               @"<filter type='and'>" +
                               @"<condition attribute='udo_lettergenerationid' operator='eq' value='" + lgRef.Id.ToString() + "' />" +
                               @"</filter>" +
                               @"<link-entity name='contact' from='contactid' to='udo_relatedveteranid' visible='false' link-type='outer' alias='vet'>" +
                               @"<attribute name='middlename' />" +
                               @"<attribute name='lastname' />" +
                               @"<attribute name='firstname' />" +
                               @"</link-entity>" +
                               @"</entity>" +
                               @"</fetch>";

                var vetInfo = service.RetrieveMultiple(new FetchExpression(fetchXml)).Entities[0];

                FirstName = GetValue(vetInfo, "vet.firstname");
                MiddleName = GetValue(vetInfo, "vet.middlename");
                LastName = GetValue(vetInfo, "vet.lastname");
                FileNumber = GetValue(vetInfo, "udo_filenumber");
                ClaimNumber = GetValue(vetInfo, "udo_claimnumber");
                docType = GetGuidValue(vetInfo, "udo_vbmsdoctype");

                doc["udo_firstname"] = FirstName;
                doc["udo_middlename"] = MiddleName;
                doc["udo_lastname"] = LastName;
                doc["udo_filenumber"] = FileNumber;
                doc["udo_claimnumber"] = ClaimNumber;
                service.Update(doc);
            }
            else if (fnodRef != null)
            {
                RelatedObject = fnodRef;

                // Get data from related fnod record
                // Retrieve: doctype, filenumber, claimnumber, related contact (vet), vet middlename, vet lastname, vet firstname, 

                // Create query
                QueryExpression fnodQuery = new QueryExpression
                {
                    EntityName = ("va_fnod"),
                    ColumnSet = new ColumnSet("va_filenumber", "udo_claimnumber")
                };

                // Add filter condition
                ConditionExpression con = new ConditionExpression("va_fnodid", ConditionOperator.Equal, fnodRef.Id.ToString());

                FilterExpression filter = new FilterExpression();
                filter.Conditions.Add(con);

                fnodQuery.Criteria.AddFilter(filter);

                // Add link entity
                LinkEntity fnod_contact = new LinkEntity()
                {
                    LinkFromEntityName = "va_fnod",
                    LinkToEntityName = "contact",
                    LinkFromAttributeName = "va_veterancontactid",
                    LinkToAttributeName = "contactid",
                    JoinOperator = JoinOperator.LeftOuter,
                    Columns = new ColumnSet("firstname", "middlename", "lastname"),
                    EntityAlias = "vet"
                };
                
                fnodQuery.LinkEntities.Add(fnod_contact);

                // Execute query
                Entity vetInfo = service.RetrieveMultiple(fnodQuery).Entities[0];

                // Extract data from query result
                FirstName = GetValue(vetInfo, "vet.firstname");
                MiddleName = GetValue(vetInfo, "vet.middlename");
                LastName = GetValue(vetInfo, "vet.lastname");
                FileNumber = GetValue(vetInfo, "va_filenumber");
                ClaimNumber = GetValue(vetInfo, "udo_claimnumber");

                // Get docType from VBMS Document record because FNOD has multiple doctype fields
                docType = GetGuidValue(doc, "udo_vbmsdocumenttype");

                // Update data values on VBMS Document
                doc["udo_firstname"] = FirstName;
                doc["udo_middlename"] = MiddleName;
                doc["udo_lastname"] = LastName;
                doc["udo_filenumber"] = FileNumber;
                doc["udo_claimnumber"] = ClaimNumber;
                service.Update(doc);
            }
        }
    }
}
