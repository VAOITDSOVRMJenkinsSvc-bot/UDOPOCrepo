// using CRM007.CRM.SDK.Core;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using VRM.Integration.UDO.Common;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.ServiceRequest.Messages;
using Logger = VRM.Integration.Servicebus.Core.Logger;
using Microsoft.Xrm.Sdk.Query;
using System.Text;
using VIMT.VeteranWebService.Messages;
using System.Security;
using VRM.Integration.UDO.Helpers;

namespace VRM.Integration.UDO.ServiceRequest.Processors
{
    class UDOCloneSRProcessor
    {
        internal void UpdateProgress(string progress, params object[] args)
        {
            try
            {

                var method = MethodInfo.GetCallingMethod(false).ToString(true);
                string progressString = progress;
                if (args.Length > 0) progressString = string.Format(progress, args);
                if (sr_log == null) sr_log = new StringBuilder();
                sr_log.AppendFormat("Progress:[{0}]: {1}\r\n", method, progressString);
            }
            catch (Exception ex)
            {
                // This should not happen - if it does, then the log is not updated.
            }
        }
        public StringBuilder sr_log { get; set; }

        public IMessageBase Execute(UDOCloneSRRequest request)
        {
            sr_log = new StringBuilder("SERVICEREQUEST Clone Details:\r\n");
            UpdateProgress("Top of Process");

            //var request = message as InitiateSRRequest;
            UDOCloneSRResponse response = new UDOCloneSRResponse();
            //set multiple message exception response

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }

            #region connect to CRM
            OrganizationServiceProxy OrgServiceProxy;
            try
            {
                var CommonFunctions = new CRMConnect();

                OrgServiceProxy = CommonFunctions.ConnectToCrm(request.OrganizationName);
            }
            catch (Exception connectException)
            {
                var method = MethodInfo.GetThisMethod().ToString();
                LogHelper.LogError(request.OrganizationName, request.UserId, method + ", Connection Error", connectException.Message);
                response.ExceptionOccured = true;
                response.ExceptionMessage = "Failed to get CRMConnection";

                return response;
            }
            #endregion

            UpdateProgress("After Connection to CRM");

            try
            {
                #region Get Record
                OrgServiceProxy.CallerId = request.UserId;
                var sr = OrgServiceProxy.Retrieve("udo_servicerequest", request.udo_ServiceRequestId, new ColumnSet(true));
                #endregion

                sr["udo_reqnumber"] = GenerateRequestNumber(OrgServiceProxy, sr).ToUnsecureString();

                sr["udo_sendnotestomapd"] = false;
                sr["udo_notecreated"] = false;
                
                #region Save Copy
                sr.Id = Guid.Empty;
                sr.Attributes.Remove("udo_servicerequestid");
                
                OrgServiceProxy.CallerId = request.UserId;
                response.udo_ServiceRequestId = OrgServiceProxy.Create(sr);
                #endregion

                //var noteid = MapDNote.Create(sr, "Copy Service Request", message, OrgServiceProxy, request.UserId);
                try
                {
                    UpdateProgress("Create Note");
                    OrgServiceProxy.CallerId = request.UserId;
                    var message = MapDNote.GenerateMapdNotes(UpdateProgress, request.OrganizationName, request.UserId, sr, "Copy");
                    var noteid = MapDNote.Create(UpdateProgress, request, sr, "Copy Service Request", message, OrgServiceProxy);
                }
                catch (Exception ex)
                {
                    var method = MethodInfo.GetThisMethod().ToString();
                    var errormessage = String.Format("Error: {0}\r\n{1}\r\n\r\nCALL STACK:{2}",
                        "Could not create note for service request.", ex.Message, ex.StackTrace);

                    LogHelper.LogError(request.OrganizationName, request.UserId, method, errormessage);
                }

                return response;
            }
            catch (Exception ExecutionException)
            {
                var method = MethodInfo.GetThisMethod().ToString();
                var message = ExecutionException.Message + "\r\n\r\nCALL STACK:" + ExecutionException.StackTrace;

                LogHelper.LogError(request.OrganizationName, request.UserId, method, message);

                response.ExceptionOccured = true;
                response.ExceptionMessage = "There was an error creating the Service Request.";
                return response;
            }
        }


        private static void PopulateField(Entity thisNewEntity, string fieldName, string fieldValue)
        {
            if (!string.IsNullOrEmpty(fieldValue))
            {
                thisNewEntity[fieldName] = fieldValue;
            }
        }

        private static void PopulateFieldfromEntity(Entity thisNewEntity, string fieldName, Entity sourceEntity, string fieldValue)
        {
            if (sourceEntity.Attributes.Contains(fieldValue))
            {
                thisNewEntity[fieldName] = sourceEntity[fieldValue];
            }

        }

        private static void MapFields(Entity source, string[] sourceCols, Entity dst, string[] dstCols)
        {
            for (int i = 0; i < dstCols.Length; i++)
            {
                var srckey = sourceCols[i];
                var dstkey = dstCols[i];
                // if the destination key is empty, there is nothing to map.
                if (!String.IsNullOrEmpty(dstkey) && source.Contains(srckey))
                {
                    dst[dstkey] = source[srckey];
                }
            }
        }

        private static EntityReference getSOJId(OrganizationServiceProxy OrgServiceProxy, string stationCode)
        {
            EntityReference thisEntRef = new EntityReference();

            QueryExpression expression = new QueryExpression()
            {
                ColumnSet = new ColumnSet("va_regionalofficeid", "va_name"),
                EntityName = "va_regionaloffice",
                Criteria =
                {
                    Filters = 
                    {
                        
                        new FilterExpression()
                        {
                          Conditions = 
                            { 
                                
                                new ConditionExpression("va_code", ConditionOperator.Equal, stationCode)
                            }
                        }
                    }
                }
            };


            EntityCollection results = OrgServiceProxy.RetrieveMultiple(expression);
            if (results.Entities.Count > 0)
            {
                if (results.Entities[0].Attributes.Contains("va_regionalofficeid"))
                {
                    Entity soj = results[0];
                    thisEntRef.Id = soj.Id;
                    thisEntRef.LogicalName = expression.EntityName;
                    thisEntRef.Name = soj.GetAttributeValue<string>("va_name");
                }
            }
            else
            {
                return null;
            }
            return thisEntRef;
        }

        private static void PopulateDateField(Entity thisNewEntity, string fieldName, string fieldValue)
        {
            if (!string.IsNullOrEmpty(fieldValue))
            {
                DateTime newDateTime;
                if (DateTime.TryParse(fieldValue, out newDateTime))
                {
                    if (newDateTime != System.DateTime.MinValue)
                    {
                        if (newDateTime == Tools.ToCRMDateTime(newDateTime))
                        {
                            thisNewEntity[fieldName] = newDateTime;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Generates the request number for the service request record
        /// </summary>
        /// <param name="serviceRequest"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        private SecureString GenerateRequestNumber(OrganizationServiceProxy orgProxy, Entity serviceRequest)
        {
            var today = DateTime.Now.ToString("MM/dd/yy H:mm:ss");
            var num = 1;

            var relatedVeteran = serviceRequest.GetAttributeValue<EntityReference>("udo_relatedveteranid");
            var secureSRSSN = serviceRequest.GetAttributeValue<string>("udo_srssn").ToSecureString();
            var secureSSN = serviceRequest.GetAttributeValue<string>("udo_ssn").ToSecureString();
            
            StringBuilder fetch = new StringBuilder();

            if (secureSRSSN.Length!=0) {
                fetch.Append("<condition attribute='udo_srssn' operator='eq' value='"+secureSRSSN.ToUnsecureString()+"'/>");
            } else if (relatedVeteran!=null) {
                fetch.Append("<condition attribute='udo_relatedveteranid' operator='eq' value='"+relatedVeteran.Id.ToString()+"'/>");
            } else if (secureSSN.Length!=0) {
                fetch.Append("<condition attribute='udo_ssn' operator='eq' value='"+secureSSN.ToUnsecureString()+"'/>");
            }

            if (fetch.Length>0) {
                fetch.Insert(0, "<fetch count='1'><entity name='udo_servicerequest'><attribute name='udo_reqnumber'/>" +
                                "<filter type='and'>");
                fetch.Append("</filter><order attribute='createdon' descending='true' /></entity></fetch>");


                //UpdateProgress("Fetch Query for Req Number:\r\n{0}", fetch);

                var results = orgProxy.RetrieveMultiple(new FetchExpression(fetch.ToString()));
                if (results != null && results.Entities.Count>0) {
                    var sr = results.Entities[0];
                    if (sr.Contains("udo_reqnumber")) {
                        var number = sr["udo_reqnumber"] as string;
                        if (number != null)
                        {
                            var reqparts = number.Split(':');
                            if (reqparts.Length > 1)
                            {
                                int lastnum = 0;
                                if (int.TryParse(reqparts[1].Trim(), out lastnum))
                                {
                                    num = lastnum + 1;
                                }
                            }
                        }
                    }
                }
            }

            SecureString requestNumber;
            if (secureSRSSN.Length!=0) {
                requestNumber = secureSRSSN.Copy();
            } else if (secureSSN.Length!=0) {
                requestNumber = secureSSN.Copy();
            } else {
                requestNumber = "SR".ToSecureString();
            }
            requestNumber = requestNumber.Append(" : ");
            requestNumber = requestNumber.Append(num.ToString());
            
            return requestNumber;
        }

        /// <summary>
        /// Gets the default currency for service request from the organization settings
        /// </summary>
        /// <param name="serviceRequest"></param>
        private static EntityReference GetDefaultCurrency(OrganizationServiceProxy orgService, UDOInitiateSRRequest request)
        {
            var fetch = @"<fetch count='1'><entity name='organization'><attribute name='basecurrencyid'/>" +
                        @"<link-entity name='systemuser' to='organizationid' from='organizationid'><filter>" +
                        @"<condition attribute='systemuserid' operator='eq' value='" + request.UserId.ToString() + "' />" +
                        @"</filter></link-entity></entity></fetch>";

            orgService.CallerId = request.UserId;
            var results = orgService.RetrieveMultiple(new FetchExpression(fetch));

            if (results != null && results.Entities.Count>0) {
                var org = results.Entities[0];
                return org.GetAttributeValue<EntityReference>("transactioncurrencyid");
            }
            return null;
        }
    }
}
