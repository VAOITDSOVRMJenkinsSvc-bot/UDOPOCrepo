using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Security;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Configuration;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.ServiceRequest.Messages;
using VEIS.Messages.VeteranWebService;
using Microsoft.Xrm.Tooling.Connector;

//using VRM.Integration.UDO.Common;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Logging.CRM.Util;
//using VRM.Integration.UDO.ServiceRequest.Messages;
//using Logger = VRM.Integration.Servicebus.Core.Logger;
//using VIMT.VeteranWebService.Messages;
//using VRM.Integration.UDO.Helpers;


namespace UDO.LOB.ServiceRequest.Processors
{
    class UDOUpdateSRProcessor
    {
        private bool _debug { get; set; }
        private string LogBuffer { get; set; }
        private CrmServiceClient OrgServiceProxy;

        private const string method = "UDOUpdateSRProcessor";

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
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.Execute");

            sr_log = new StringBuilder("SERVICEREQUEST Update Details:\r\n");
            UpdateProgress("Top of Process");

            //var request = message as InitiateSRRequest;
            UDOUpdateSRRepsonse response = new UDOUpdateSRRepsonse();
            //set multiple message exception response

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccurred = true;
                return response;
            }

            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = method,
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA"
                };
            }
            TraceLogger tLogger = new TraceLogger(method, request);

            LogHelper.LogInfo($">> {this.GetType().FullName}.Execute: Request:  {JsonHelper.Serialize<UDOUpdateSRRequest>(request)}");

            _debug = request.Debug;
            LogBuffer = string.Empty;
            #region connect to CRM
            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
                tLogger.LogEvent($"Authenticated and Connected to CRM", "2");
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccurred = true;
                return response;
            }
            #endregion
            try
            {
                var serviceRequest = OrgServiceProxy.Retrieve("udo_servicerequest", request.udo_ServiceRequestId, new ColumnSet(new string[] { "udo_participantid", "udo_personid", "udo_pmc",
                "udo_nokletter", "udo_21530", "udo_21534", "udo_401330", "udo_other", "udo_otherspecification", "udo_depfirstname", "udo_deplastname", "udo_srfirstname", "udo_srlastname",
                "udo_ssn", "udo_issue", "udo_action", "udo_regionalofficeid", "udo_description", "udo_letterscreated", "udo_reqnumber", "udo_dateofmissingpayment",
                "udo_amtofpayments", "udo_nameofreportingindividual", "udo_depdateofbirth", "udo_depssn", "udo_dateofdeath", "udo_srssn", "udo_relatedveteranid", "udo_servicerequestsid",
                "udo_claimnumber"}));





                var pid = serviceRequest.GetAttributeValue<string>("udo_participantid");
                var personid = serviceRequest.GetAttributeValue<EntityReference>("udo_personid");
                if (String.IsNullOrEmpty(pid))
                {
                    UpdateProgress("No Participant ID, cannot create note.");
                }
                else if (personid == null || personid.Id == Guid.Empty)
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
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, method, ExecutionException);
            }
            finally
            {
                if (OrgServiceProxy != null)
                {
                    OrgServiceProxy.Dispose();
                }
            }

            return response;
        }
    }
}