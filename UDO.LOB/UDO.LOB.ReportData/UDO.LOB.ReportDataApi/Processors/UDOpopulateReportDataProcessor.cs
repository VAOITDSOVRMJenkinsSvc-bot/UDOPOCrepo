using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Collections.Specialized;
using UDO.LOB.Extensions.Configuration;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.ReportData.Messages;
using VEIS.Core.Messages;
using VEIS.Messages.RatingService;
using Microsoft.Xrm.Sdk.WebServiceClient;
using System.Data.SqlClient;
using System.Xml;
using Microsoft.Xrm.Sdk.Query;
/// <summary>
/// VIMT LOB Component for UDOUDOcreateSMCRatings,UDOcreateSMCRatings method, Processor.
/// </summary>
namespace UDO.LOB.ReportData.Processors
{
    public class UDOpopulateReportDataProcessor
    {
        private IOrganizationService OrgServiceProxy;
        private OrganizationWebProxyClient webProxyClient;
        private bool _debug { get; set; }
        private const string method = "UDOpopulateReportDataProcessor";
        private string LogBuffer { get; set; }
       
        private List<string> fetchFields;
        private List<string> sqlFields;
        public IMessageBase Execute(UDOpopulateReportDataRequest request)
        {
         
            UDOpopulateReportDataResponse response = new UDOpopulateReportDataResponse();
            response.MessageId = request.MessageId;

            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccurred = true;
                return response;
            }

            #region connect to CRM
            try
            {
                webProxyClient = CrmConnection.Connect<OrganizationWebProxyClient>();
                if (request.UserId != Guid.Empty)
                {
                    webProxyClient.CallerId = request.UserId;
                }
                OrgServiceProxy = webProxyClient as IOrganizationService;

            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, method, connectException);
                response.ExceptionMessage = $" {method}: Failed to get CRMConnection";
                response.ExceptionOccurred = true;
                return response;
            }
            #endregion
            Entity entity = OrgServiceProxy.Retrieve("udo_letter", new Guid("116CF5C5-7FE1-E511-9438-0050568DF261"), new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
            fetchFields = new List<string>();

            //this will only return data if we are NOT doing just dynamic only tables.
            string fetchXml = String.Format(entity["udo_fetchxml"].ToString(), "48D9B1B3-CA86-E911-A979-001DD8009F4B");
            Entity entity2 = null;
            progressString = "After Connection";
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

                        datalist += "'" + ((Microsoft.Xrm.Sdk.EntityReference)item.Value).Name + "',";
                        break;

                    case "OptionSetValue":

                        datalist += "'" + ((Microsoft.Xrm.Sdk.OptionSetValue)item.Value).Value + "',";
                        break;

                    default:
                        break;
                }

            }

            fieldnames = fieldnames.Substring(0, fieldnames.Length - 1);
            datalist = datalist.Substring(0, datalist.Length - 1);
            using (SqlConnection con = new SqlConnection(ConnectionCache.ConnectManager.SSRSConfiguration["ReportServerDbConnectionString"]))
            {
                con.Open();
                try
                {
                    string commandText = String.Format(@"IF NOT EXISTS (SELECT [udo_lettergenerationid] FROM letterGeneration WHERE [udo_lettergenerationid] = '{0}')
                                                            BEGIN
                                                                INSERT INTO LetterGeneration ({1}) VALUES ({2});
                                                            END", request.udo_lettergenerationid, fieldnames, datalist);

                    using (SqlCommand command = new SqlCommand(commandText, con))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch(Exception ex)
                {
                    response.ExceptionMessage = ex.Message;
                    response.ExceptionOccurred = true;
                }
            }

            return response;
        }

        private static string dateStringFormat(string date)
        {
            if (date == null) return null;
            if (date.Length == 10) return date;
            if (date.Length < 8) return date;

            date = date.Insert(2, "/");
            date = date.Insert(5, "/");
            return date;
        }
    }
}
