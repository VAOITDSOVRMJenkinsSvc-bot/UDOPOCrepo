using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace UDO.LOB.RatingsApi.Tests
{
    class UpdateTable2
    {
        private IOrganizationService OrgServiceProxy;

        private List<string> fetchFields;
        private List<string> sqlFields;
        private string sqlConString = ConfigurationManager.ConnectionStrings["localDbConnection"].ConnectionString;
        private string crmConString = ConfigurationManager.ConnectionStrings["CrmConnection"].ConnectionString;
        private string currentEntity;
        string fileName;
        string table;
        string sqlDbName = ConfigurationManager.AppSettings["SQLDbName"];

        public void UpdateTable(string letterId)
        {
            sqlFields = new List<string>();
            using (SqlConnection con = new SqlConnection(sqlConString))
            {
                con.Open();

                #region connect to CRM
                //TODO: Change this to use the Api CRM helper for connection
                try
                {

                    CrmServiceClient client = new CrmServiceClient(crmConString);
                    OrgServiceProxy = (IOrganizationService)client.OrganizationServiceProxy;
                }
                catch (Exception connectException)
                {

                }
                #endregion

                QueryExpression letterQuery = new QueryExpression("udo_letter");
                LinkEntity link = new LinkEntity("udo_letter", "udo_lettergeneration", "udo_letterid", "udo_letter", JoinOperator.Inner);
                link.LinkCriteria.AddCondition("udo_lettergenerationid", ConditionOperator.Equal, new Guid(letterId));                
                letterQuery.LinkEntities.Add(link);
                letterQuery.ColumnSet = new ColumnSet("udo_name", "udo_fetchxml");

                EntityCollection entityCollection = OrgServiceProxy.RetrieveMultiple(letterQuery);
                if(entityCollection.Entities.Count == 0)
                {
                    throw new Exception("No Records found");
                }
                
                fetchFields = new List<string>();
                
                fetchFields.Clear();
                sqlFields.Clear();
                Entity entity = entityCollection.Entities[0];
                string fetchXml = entity.GetAttributeValue<string>("udo_fetchxml");
                string report = entity.GetAttributeValue<string>("udo_name");
                if(string.IsNullOrEmpty(fetchXml) || string.IsNullOrEmpty(report))
                {
                    throw new Exception("No data in fields or fields not found");
                }

                //TODO: repotName and parameter can be used later when calling the SSRS services to reneder the report
                //We will need to determine how to format the parameter based on the report name 
                string parameter, reportName;
                if (report.Contains("-"))
                {
                    parameter = report.Substring(report.IndexOf('-') + 1).Trim();
                    reportName = report.Substring(0, report.IndexOf('-')).Trim();
                }
                
                fetchXml = string.Format(fetchXml, letterId);
                
                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.LoadXml(fetchXml);
                }
                catch (XmlException xe)
                {
                    throw xe;
                }


                if (doc.HasChildNodes)
                {
                    var node1 = doc.ChildNodes;
                    currentEntity = node1[0].ChildNodes[0].Attributes[0].Value;

                    switch (currentEntity)
                    {
                        case "udo_lettergeneration":
                            table = "LetterGeneration";
                            break;
                        case "udo_servicerequest":
                            table = "ServiceRequest";
                            break;
                        case "udo_lettergenerationdisability":
                            table = "LetterGenerationDisability";
                            break;
                        case "udo_interaction":
                            table = "Interaction";
                            break;
                        default:
                            table = currentEntity;
                            break;
                    }
                    if (!TableExists(table))
                    {
                        CreateInitialTable(table, fetchXml);
                    }
                    try
                    {
                        using (SqlCommand command = new SqlCommand(
                            $"SELECT COLUMN_NAME FROM {sqlDbName}.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{table}'", con))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        sqlFields.Add(Convert.ToString(reader.GetValue(i)));
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Table not created.");
                    }


                    foreach (XmlNode childNode in node1)
                    {
                        if (childNode.HasChildNodes)
                        {
                            var node2 = childNode.ChildNodes;


                            RetrieveEntityResponse thisEntityMetaData = getEntityMetaData(node2[0]);

                            ProcessDataNode(node2, thisEntityMetaData);
                        }
                    }
                }

                if (fetchFields.Count > 0)
                {
                    var alterTable = string.Empty;
                    foreach (var item in fetchFields)
                    {
                        alterTable += item;
                    }
                    alterTable = alterTable.Substring(0, alterTable.Length - 1);

                    switch (currentEntity)
                    {
                        case "udo_lettergeneration":
                            table = "LetterGeneration";
                            break;
                        case "udo_servicerequest":
                            table = "ServiceRequest";
                            break;
                        case "udo_lettergenerationdisability":
                            table = "LetterGenerationDisability";
                            break;
                        default:
                            table = currentEntity;
                            break;
                    }


                    alterTable = $"Alter Table dbo.{table} ADD " + alterTable + ";";


                    try
                    {
                        using (SqlCommand command = new SqlCommand(alterTable, con))
                        {
                            command.ExecuteNonQuery();
                            Console.WriteLine($"{table} Altererd Successfully");
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Alter Table Failed.");
                    }

                }

                EntityCollection entities;
                try
                {
                    entities = OrgServiceProxy.RetrieveMultiple(new FetchExpression(fetchXml));
                }
                catch (Exception retrieveException)
                {
                    throw retrieveException;
                }

                if (entities != null && entities.Entities.Count == 0)
                {
                    throw new Exception("No Records Found from fetchXml");
                }

                Entity entity2 = entities.Entities[0];
                var fieldnames = string.Empty;
                var datalist = string.Empty;
                char[] b = { '"' };
                foreach (var item in entity2.Attributes)
                {
                    fieldnames += item.Key + ",";

                    var test = item.Value;
                    var testType = test.GetType();
                    switch (testType.Name)
                    {
                        case "AliasedValue":
                            var aliasValue = ((Microsoft.Xrm.Sdk.AliasedValue)item.Value).Value;
                            var aliasType = aliasValue.GetType();
                            switch (aliasType.Name)
                            {
                                case "Money":
                                    datalist += "'" + ((Microsoft.Xrm.Sdk.Money)((Microsoft.Xrm.Sdk.AliasedValue)item.Value).Value).Value + "',";
                                    break;
                                case "String":

                                    datalist += "'" + ((Microsoft.Xrm.Sdk.AliasedValue)item.Value).Value + "',";
                                    break;
                                case "Guid":

                                    datalist += "'" + ((Microsoft.Xrm.Sdk.AliasedValue)item.Value).Value + "',";
                                    break;
                                case "Boolean":
                                    
                                    switch (((Microsoft.Xrm.Sdk.AliasedValue)item.Value).Value)
                                    {
                                        case "True":
                                            datalist += "'true',";
                                            break;
                                        case "False":
                                            datalist += "'false',";
                                            break;
                                        case "true":
                                            datalist += "'true',";
                                            break;
                                        case "false":
                                            datalist += "'false',";
                                            break;
                                        case false:
                                            datalist += "'false',";
                                            break;
                                        case true:
                                            datalist += "'true',";
                                            break;
                                        default:
                                            break;
                                    }
                                    fieldnames += $"{item.Key}Value,";
                                    datalist += ((BooleanManagedProperty)((AliasedValue)item.Value).Value).Value + ",";
                                    break;
                                case "DateTime":
                                    
                                    datalist += "'" + Convert.ToDateTime(((Microsoft.Xrm.Sdk.AliasedValue)item.Value).Value).ToShortDateString() + "',";
                                    break;
                                case "EntityReference":

                                    //  datalist += "'" + (Microsoft.Xrm.Sdk.EntityReference)((Microsoft.Xrm.Sdk.AliasedValue)item.Value.Name + "',";
                                    break;
                                default:
                                    datalist += "'" + ((Microsoft.Xrm.Sdk.AliasedValue)item.Value).Value + "',";
                                    break;
                            }

                            break;
                        case "Guid":

                            datalist += "'" + item.Value + "',";
                            break;
                        case "String":

                            datalist += "'" + item.Value + "',";
                            break;
                        case "DateTime":

                            datalist += "'" + Convert.ToDateTime(item.Value).ToShortDateString() + "',";
                            break;
                        case "Boolean":
                            switch (item.Value)
                            {
                                case "True":
                                    datalist += "'true',";
                                    break;
                                case "False":
                                    datalist += "'false',";
                                    break;
                                case "true":
                                    datalist += "'true',";
                                    break;
                                case "false":
                                    datalist += "'false',";
                                    break;
                                case false:
                                    datalist += "'false',";
                                    break;
                                case true:
                                    datalist += "'true',";
                                    break;
                                default:
                                    break;
                            }

                            break;
                        case "EntityReference":
                            datalist += "'" + ((Microsoft.Xrm.Sdk.EntityReference)item.Value).Id + "',";
                            fieldnames += item.Key + "Name";
                            datalist += "'" + ((EntityReference)item.Value).Name + "',";
                            break;

                        case "OptionSetValue":
                            //Swap the value and label to match MSCRMFetch behavior
                            datalist += $"'{entity2.FormattedValues[item.Key]}',";
                            fieldnames += item.Key + "Value,";
                            datalist += ((Microsoft.Xrm.Sdk.OptionSetValue)item.Value).Value + ",";
                            break;

                        default:
                            break;
                    }

                }

                fieldnames = fieldnames.Substring(0, fieldnames.Length - 1);
                datalist = datalist.Substring(0, datalist.Length - 1);


                try
                {
                    using (SqlCommand command = new SqlCommand(
                        $"Insert into {table}  (" + fieldnames + ") VALUES (" + datalist + ")", con))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                }
                
            }
        }

        private void CreateInitialTable(string tableName, string fetch)
        {
            CreateTable t = new CreateTable();
            t.CreateInitialTable(tableName, fetch);
        }

        private bool TableExists(string tableName)
        {
            
            SqlConnection con = new SqlConnection(sqlConString);
            using (SqlCommand command = new SqlCommand($"SELECT * FROM VEIS.Sys.tables WHERE name = '{tableName}';", con))
            {
                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                bool exists = reader.HasRows;
                command.Connection.Close();
                return exists;
            }

        }

        private void ProcessDataNode(XmlNodeList node2, RetrieveEntityResponse thisEntityMetaData)
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
                            ProcessDataNode(nodeChild, thisEntityMetaData);

                            break;
                        default:

                            var node3 = childNode2.ChildNodes;

                            foreach (XmlNode childNode3 in node3)
                            {
                                switch (childNode3.Name)
                                {
                                    case "attribute":
                                        identifyField(thisEntityMetaData, childNode3);

                                        break;
                                    case "filter":
                                        break;
                                    case "link-entity":
                                        if (childNode3.HasChildNodes)
                                        {
                                            thisEntityMetaData = getEntityMetaData(childNode3);
                                            var nodeChildAtt = childNode3.ChildNodes;
                                            ProcessDataNode(nodeChildAtt, thisEntityMetaData);
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
                            identifyField(thisEntityMetaData, childNode2);

                            break;
                        case "filter":
                            break;
                        case "link-entity":
                            if (childNode2.HasChildNodes)
                            {
                                thisEntityMetaData = getEntityMetaData(childNode2);
                                var nodeChild = childNode2.ChildNodes;
                                ProcessDataNode(nodeChild, thisEntityMetaData);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
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

        private void identifyField(RetrieveEntityResponse thisEntityMetaData, XmlNode childNode3)
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

            bool match = false;
            AttributeMetadata metaDAttrib = thisEntityMetaData.EntityMetadata.Attributes.FirstOrDefault(a => a.LogicalName == d365FieldName);
            if (metaDAttrib != null)
            {
                //Trap for picklist (OptionSets) and Lookup (EntityReference) fields so we can add both the value and label/name fields to the table
                if (metaDAttrib.AttributeTypeName != null && metaDAttrib.AttributeType == AttributeTypeCode.Picklist)
                {
                    string labelField = fieldName + "Label";
                    if (!sqlFields.Contains(labelField, StringComparer.CurrentCultureIgnoreCase))
                    {
                        AlterTable(labelField, metaDAttrib, true);
                    }
                } 
                else if(metaDAttrib.AttributeType.HasValue && metaDAttrib.AttributeType == AttributeTypeCode.Lookup)
                {
                    string labelField = fieldName + "Name";
                    if(!sqlFields.Contains(labelField, StringComparer.CurrentCultureIgnoreCase))
                    {
                        AlterTable(labelField, metaDAttrib, true);
                    }
                }
                if (!sqlFields.Contains(fieldName, StringComparer.CurrentCultureIgnoreCase))
                {
                    AlterTable(fieldName, metaDAttrib);
                }
            }
        }

        private void AlterTable(string fieldName, AttributeMetadata metaDAttrib, bool isLable = false)
        {
            var alterTable = fieldName;
            
            if (isLable)
            {
                alterTable += " nvarchar(500) null,";
            }
            else
            {
                switch (metaDAttrib.AttributeType)
                {
                    case AttributeTypeCode.String:
                        alterTable += " nvarchar(" + ((StringAttributeMetadata)metaDAttrib).MaxLength + ") null,";

                        break;
                    case AttributeTypeCode.DateTime:
                        alterTable += " datetime null,";

                        break;
                    case AttributeTypeCode.Boolean:
                        alterTable += " BIT DEFAULT(0),";

                        break;
                    case AttributeTypeCode.Memo:
                        alterTable += " varchar(MAX) null,";

                        break;
                    case AttributeTypeCode.Decimal:
                    case AttributeTypeCode.Money:
                        alterTable += " DECIMAL null,";

                        break;
                    case AttributeTypeCode.Picklist:
                        alterTable += " int null,";

                        break;
                    case AttributeTypeCode.Uniqueidentifier:
                        alterTable += " nvarchar(36) null,";
                        break;
                    default:
                        break;

                }
            }
            fetchFields.Add(alterTable);
        }
    }
}
