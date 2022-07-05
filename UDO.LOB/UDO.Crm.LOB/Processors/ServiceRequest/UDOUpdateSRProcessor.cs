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
    class UDOUpdateSRProcessor
    {
        public delegate void ProgressSetter(string progress, params object[] args);
        /// <summary>
        /// UpdateProgress: This method is simple, it appends the log with information passed to it
        /// </summary>
        /// <param name="progress">A composite format string with a progress update.</param>
        /// <param name="args">The object(s) to format.</param>
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
        
        public IMessageBase Execute(UDOUpdateSRRequest request)
        {
            sr_log = new StringBuilder("SERVICEREQUEST Update Details:\r\n");
            UpdateProgress("Top of Process");

            //var request = message as InitiateSRRequest;
            UDOUpdateSRRepsonse response = new UDOUpdateSRRepsonse();
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
                OrgServiceProxy.CallerId = request.UserId;
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

            var serviceRequest = OrgServiceProxy.Retrieve("udo_servicerequest", request.udo_ServiceRequestId, new ColumnSet(new string[] { "udo_participantid", "udo_personid", "udo_pmc", 
                "udo_nokletter", "udo_21530", "udo_21534", "udo_401330", "udo_other", "udo_otherspecification", "udo_depfirstname", "udo_deplastname", "udo_srfirstname", "udo_srlastname", 
                "udo_ssn", "udo_issue", "udo_action", "udo_regionalofficeid", "udo_description", "udo_letterscreated", "udo_reqnumber", "udo_dateofmissingpayment", 
                "udo_amtofpayments", "udo_nameofreportingindividual", "udo_depdateofbirth", "udo_depssn", "udo_dateofdeath", "udo_srssn", "udo_relatedveteranid", "udo_servicerequestsid", 
                "udo_claimnumber"}));

            
            try
            {

                var pid = serviceRequest.GetAttributeValue<string>("udo_participantid");
                var personid = serviceRequest.GetAttributeValue<EntityReference>("udo_personid");
                if (String.IsNullOrEmpty(pid))
                {
                    UpdateProgress("No Participant ID, cannot create note.");
                }
                else if (personid==null || personid.Id==Guid.Empty)
                {
                    UpdateProgress("No Person ID");
                }
                else
                {

                    //UpdateProgress("Starting Note Creation Process");
                    //OrgServiceProxy.CallerId = request.UserId;


                    //TODO: Will need to comment out this section to generate a note.  The note will be created within the form.
                    //UpdateProgress("Step 1: Generate Message");
                    //var message = MapDNote.GenerateMapdNotes(UpdateProgress, request.OrganizationName, request.UserId, serviceRequest, "Update");
                    //UpdateProgress("Message Generated: {0}", message);

                    //UpdateProgress("Step 2: Create Note");
                    //var noteid = MapDNote.Create(UpdateProgress, request, serviceRequest, "Service Request Updated", message, OrgServiceProxy);
                    //UpdateProgress("Note Created: {0}", noteid);
                }

                sr_log.Insert(0, serviceRequest.DumpToString("serviceRequest") + "\r\n\r\n\r\n");

                if (request.Debug)
                {
                    var method = MethodInfo.GetThisMethod().ToString();
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method, sr_log.ToString());
                }
            }
            catch (Exception ExecutionException)
            {
                sr_log.Insert(0, "\r\n\r\nLog Details:");
                sr_log.Insert(0, ExecutionException.Message);
                sr_log.Insert(0, "EXECUTION EXCEPTION:\r\n");
                sr_log.AppendFormat("\r\nEXECUTION EXCEPTION: ");
                sr_log.AppendLine(ExecutionException.Message);
                sr_log.AppendLine("\r\nCALL STACK: ");
                sr_log.AppendLine(ExecutionException.StackTrace.ToString());
                var method = MethodInfo.GetThisMethod().ToString();
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method, sr_log.ToString());
            }
            return response;

        }
    }
}
