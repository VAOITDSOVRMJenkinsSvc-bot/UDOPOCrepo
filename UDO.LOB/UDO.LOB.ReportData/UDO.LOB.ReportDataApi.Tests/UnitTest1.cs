using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.WebServiceClient;
using System;
using System.Data.SqlClient;
using System.Xml;
using UDO.LOB.Extensions;

namespace UDO.LOB.ReportDataApi.Tests
{
    [TestClass]
    public class UnitTest1
    {

        private IOrganizationService OrgServiceProxy;
        private OrganizationWebProxyClient webProxyClient;

        [TestMethod]
        public void TestMethod1()
        {
            #region connect to CRM
            try
            {
                webProxyClient = CrmConnection.Connect<OrganizationWebProxyClient>();
               
                OrgServiceProxy = webProxyClient as IOrganizationService;
            }
            catch (Exception connectException)
            {
               
            }
            #endregion

            var createTable = "CREATE TABLE LetterGeneration (udo_lettergenerationid nvarchar(36) not null, transactioncurrencyid nvarchar(36) null, ";
            Entity entity = OrgServiceProxy.Retrieve("udo_letter",new Guid("116CF5C5-7FE1-E511-9438-0050568DF261"), new Microsoft.Xrm.Sdk.Query.ColumnSet(true));

           
            //this will only return data if we are NOT doing just dynamic only tables.
            string fetchXml = String.Format(entity["udo_fetchxml"].ToString(), "48D9B1B3-CA86-E911-A979-001DD8009F4B");
            Entity entity2 = null;
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





            EntityCollection entities = OrgServiceProxy.RetrieveMultiple(new FetchExpression(fetchXml));
            if (entities != null && entities.Entities.Count > 0) entity2 = entities.Entities[0];
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

                        datalist += "'" + Convert.ToDateTime(item.Value).ToShortDateString()+ "',";
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

                        datalist += "'" + ((Microsoft.Xrm.Sdk.EntityReference)item.Value).Name + "',";
                        break;

                    case "OptionSetValue":

                        datalist += "'" +  ((Microsoft.Xrm.Sdk.OptionSetValue)item.Value).Value + "',";
                        break;

                    default:
                        break;
                }
                
            }

            fieldnames = fieldnames.Substring(0,fieldnames.Length - 1);
            datalist = datalist.Substring(0,datalist.Length - 1);

            using (SqlConnection con = new SqlConnection("Data Source=localhost;Initial Catalog=UDOD365REPORTS;Integrated Security=True"))
            {
                con.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(
                        "Insert into LetterGeneration9 (" + fieldnames + ") VALUES (" + datalist + ")", con))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch
                {
                    Console.WriteLine("Table not created.");
                }
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
            d365FieldName = childNode3.Attributes[0].InnerText;
            if (childNode3.Attributes.Count == 1)
            {
                fieldName = childNode3.Attributes[0].InnerText;
            }
            else
            {
                fieldName = childNode3.Attributes[1].InnerText;
            }

            foreach (var metaDAttrib in thisEntityMetaData.EntityMetadata.Attributes)
            {
                if (d365FieldName.Equals(metaDAttrib.LogicalName, StringComparison.InvariantCultureIgnoreCase))
                {
                    createTable += fieldName;
                    if (fieldName.StartsWith("ic"))
                    {
                        var stopper = "now";
                    }
                    switch (metaDAttrib.AttributeType)
                    {
                        case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.String:
                            createTable += " nvarchar(" + ((Microsoft.Xrm.Sdk.Metadata.StringAttributeMetadata)metaDAttrib).DatabaseLength + ") null,";

                            break;
                        case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.DateTime:
                            createTable += " datetime null,";

                            break;
                        case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Boolean:
                            createTable += " BIT null,";

                            break;
                        case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Memo:
                            createTable += " varchar(MAX) null,";

                            break;
                        case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Money:
                            createTable += " DECIMAL null,";

                            break;
                        case Microsoft.Xrm.Sdk.Metadata.AttributeTypeCode.Picklist:
                            createTable += " nvarchar(500) null,";

                            break;
                        default:
                            break;

                    }

                }
            }

            return createTable;
        }

        
    }
}
