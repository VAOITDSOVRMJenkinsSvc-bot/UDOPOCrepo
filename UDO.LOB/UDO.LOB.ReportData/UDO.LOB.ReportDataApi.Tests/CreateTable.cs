using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Xml;

namespace UDO.LOB.RatingsApi.Tests
{
    class CreateTable
    {
        IOrganizationService OrgServiceProxy;
        string dbConnection;
        string crmConnection;
        public void CreateInitialTable(string tableName, string fetchXml)
        {
            #region connect to CRM
            try
            {
                crmConnection = ConfigurationManager.ConnectionStrings["CrmConnection"].ConnectionString;
                dbConnection = ConfigurationManager.ConnectionStrings["localDbConnection"].ConnectionString;


                CrmServiceClient conn = new CrmServiceClient(crmConnection);
                OrgServiceProxy = conn.OrganizationWebProxyClient ?? (IOrganizationService)conn.OrganizationServiceProxy;
            }
            catch (Exception connectException)
            {
                throw connectException;
            }
            #endregion

            var createTable = $"CREATE TABLE dbo.{tableName} (udo_{tableName.ToLower()}id nvarchar(36) not null CONSTRAINT PK_{tableName} PRIMARY KEY CLUSTERED, transactioncurrencyid nvarchar(36) null, ";
            
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(fetchXml);

            if (doc.HasChildNodes)
            {
                var node1 = doc.ChildNodes;

                foreach (XmlNode childNode in node1)
                {
                    if (childNode.HasChildNodes)
                    {
                        var node2 = childNode.ChildNodes;
                        RetrieveEntityResponse thisEntityMetaData = getEntityMetaData(node2[0]);

                        createTable = ProcessDataNode(createTable, node2, thisEntityMetaData);
                    }
                }
            }
            createTable = createTable.Substring(0, createTable.Length - 1);

            createTable += ")";

            using (SqlConnection con = new SqlConnection(dbConnection))
            using (SqlCommand command = new SqlCommand(createTable, con))
            {
                con.Open();
                command.ExecuteNonQuery();
                con.Close();
            }
        }

        private string ProcessDataNode(string createTable, XmlNodeList node2, RetrieveEntityResponse thisEntityMetaData)
        {
            foreach (XmlNode childNode2 in node2)
            {
                if (childNode2.HasChildNodes)
                {

                    switch (childNode2.Name)
                    {
                        case "link-entity":

                            thisEntityMetaData = getEntityMetaData(childNode2);
                            var nodeChild = childNode2.ChildNodes;
                            createTable = ProcessDataNode(createTable, nodeChild, thisEntityMetaData);

                            break;
                        default:

                            var node3 = childNode2.ChildNodes;

                            foreach (XmlNode childNode3 in node3)
                            {
                                switch (childNode3.Name)
                                {
                                    case "attribute":
                                        createTable = getField(createTable, thisEntityMetaData, childNode3); break;
                                    case "filter":
                                        break;
                                    case "link-entity":
                                        if (childNode3.HasChildNodes)
                                        {
                                            thisEntityMetaData = getEntityMetaData(childNode3);
                                            var nodeChildAtt = childNode3.ChildNodes;
                                            createTable = ProcessDataNode(createTable, nodeChildAtt, thisEntityMetaData);
                                        }
                                        break;
                                    default:
                                        break;
                                }

                            }
                            break;
                    }
                }
                else
                {
                    switch (childNode2.Name)
                    {
                        case "attribute":
                            createTable = getField(createTable, thisEntityMetaData, childNode2); break;
                        case "filter":
                            break;
                        case "link-entity":
                            if (childNode2.HasChildNodes)
                            {
                                thisEntityMetaData = getEntityMetaData(childNode2);
                                var nodeChild = childNode2.ChildNodes;
                                createTable = ProcessDataNode(createTable, nodeChild, thisEntityMetaData);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            return createTable;
        }

        private RetrieveEntityResponse getEntityMetaData(XmlNode childNode2)
        {
            RetrieveEntityResponse thisEntityMetaData = new RetrieveEntityResponse();
            switch (childNode2.Name)
            {
                case "entity":
                    RetrieveEntityRequest retrieveEntityMetaData = new RetrieveEntityRequest
                    {
                        EntityFilters = Microsoft.Xrm.Sdk.Metadata.EntityFilters.Attributes,
                        RetrieveAsIfPublished = true,
                        LogicalName = childNode2.Attributes[0].Value
                    };

                    thisEntityMetaData = (RetrieveEntityResponse)OrgServiceProxy.Execute(retrieveEntityMetaData);
                    break;
                case "link-entity":
                    RetrieveEntityRequest retrieveEntityMetaData2 = new RetrieveEntityRequest
                    {
                        EntityFilters = Microsoft.Xrm.Sdk.Metadata.EntityFilters.Attributes,
                        RetrieveAsIfPublished = true,
                        LogicalName = childNode2.Attributes[0].Value
                    };

                    thisEntityMetaData = (RetrieveEntityResponse)OrgServiceProxy.Execute(retrieveEntityMetaData2);
                    break;
                default:
                    break;
            }
            return thisEntityMetaData;
        }

        private static string getField(string createTable, RetrieveEntityResponse thisEntityMetaData, XmlNode childNode3)
        {
            var fieldName = string.Empty;
            var d365FieldName = string.Empty;
            d365FieldName = childNode3.Attributes[0].InnerText.ToLower();
            if (childNode3.Attributes.Count == 1)
            {
                fieldName = childNode3.Attributes[0].InnerText;
            }
            else
            {
                fieldName = childNode3.Attributes[1].InnerText;
            }
            var metaDAttrib = thisEntityMetaData.EntityMetadata.Attributes.FirstOrDefault(d => d.LogicalName == d365FieldName);
            if (metaDAttrib != null)
            {                
                switch (metaDAttrib.AttributeType)
                {
                    case AttributeTypeCode.String:
                        createTable += $"{fieldName} nvarchar(" + ((StringAttributeMetadata)metaDAttrib).MaxLength + ") null,";

                        break;
                    case AttributeTypeCode.DateTime:
                        createTable += $"{fieldName}Value datetime null, {fieldName} nvarchar(25) null,";

                        break;
                    case AttributeTypeCode.Boolean:
                        createTable += $"{fieldName}Value BIT DEFAULT(0), {fieldName} NVARCHAR(50) null,";

                        break;
                    case AttributeTypeCode.Memo:
                        createTable += $"{fieldName} varchar(MAX) null,";

                        break;
                    case AttributeTypeCode.Decimal:
                        createTable += $"{fieldName} DECIMAL(18,2) DEFAULT(0)";
                        break;
                    case AttributeTypeCode.Money:
                        createTable += $"{fieldName} MONEY DEFAULT(0),";
                        break;
                    case AttributeTypeCode.Picklist:
                        //Create two fields: one for the integer value and one for the label
                        createTable += $"{fieldName}Value int null, {fieldName} nvarchar(500) null,";
                        break;
                    case AttributeTypeCode.Uniqueidentifier:
                        createTable += $"{fieldName} nvarchar(36) null,";
                        break;
                    case AttributeTypeCode.Lookup:
                        //Create two fields: one for the id value and one for the Name value
                        createTable += $"{fieldName} nvarchar(36) null, {fieldName}Name nvarchar(500) null,";
                        break;
                    default:
                        break;

                }
            }
            else
            {
                var stop = "Now";
            }
            return createTable;
        }
    }
}
