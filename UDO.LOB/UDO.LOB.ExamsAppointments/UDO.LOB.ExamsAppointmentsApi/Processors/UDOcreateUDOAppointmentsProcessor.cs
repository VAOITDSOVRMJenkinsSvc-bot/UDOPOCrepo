using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Linq;
using UDO.LOB.Core;
using UDO.LOB.ExamsAppointments.Messages;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Configuration;
using UDO.LOB.Extensions.Logging;
using VEIS.Core.Messages;
using VEIS.Messages.PathWaysService;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.Xrm.Tooling.Connector;

//using VIMT.PathwaysService.Messages;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Logging.CRM.Util;
//using VRM.Integration.UDO.Common;
//using VRM.Integration.UDO.ExamsAppointments.Messages;

namespace UDO.LOB.ExamsAppointments.Processors
{
    class UDOcreateUDOAppointmentsProcessor
    {
        private CrmServiceClient OrgServiceProxy;
        private bool _debug { get; set; }

        private const string VEISBaseUrlAppSettingsKeyName = "VEISBaseUrl";

        private string LogBuffer { get; set; }

        private Uri veisBaseUri;

        private LogSettings logSettings { get; set; }

        private const string method = "UDOcreateUDOAppointmentsProcessor";

        public IMessageBase Execute(UDOcreateUDOAppointmentsRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.Execute");
            UDOcreateUDOAppointmentsResponse response = new UDOcreateUDOAppointmentsResponse();
            response.MessageId = request.MessageId;
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            //REM: Init Processor to set the VEIS Config
            InitProcessor(request);

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

            LogHelper.LogInfo($">> {this.GetType().FullName}.Execute: Request:  {JsonHelper.Serialize<UDOcreateUDOAppointmentsRequest>(request)}");


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

            progressString = "After Connection";

            try
            {

                #region Create EC Request

                var Datestring = DateTime.Now.ToString("yyyy-MM-dd") + "T" + DateTime.Now.ToString("hh:mm:ss");

                string mcs_in1_jsonString = "\"filter: filter\": {\"-vhimVersion\": \"Vhim_4_00\"," +
                    "\"-xmlns:filter\": \"Filter\",\"filterId\": \"APPOINTMENTS_SINGLE_PATIENT_FILTER\"," +
                    "\"clientName\": \"VRM 1.0\"," +
                    "\"clientRequestInitiationTime\": " + Datestring + "," +
                    "\"patients\": { \"NationalId\": " + request.ICN + " }," +
                    "\"entryPointFilter\": {\"-queryName\": \"Appointment-Standardized\"," +
                    "\"domainEntryPoint\": \"Appointment\"," +
                    "\"startDate\": \"1950-01-01 \"," +
                    "\"endDate\": \"2050-01-01 \"}";

                // prefix = payHistSSN_findPayHistoryBySSNRequest();
                var findExamsAppointmentsRequest = new VEISpwsreadDataRequest
                {
                    // TODO: VEIS dependent missing mcs_transactionid
                    //       Removed to build but needed.
                    //mcs_transactionid = request.transactionId,
                    MessageId = request.MessageId,
                    LogTiming = request.LogTiming,
                    LogSoap = request.LogSoap,
                    Debug = request.Debug,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    },
                    mcs_in0 = "AppointmentsRead1",
                    mcs_in1 = @"<?xml version='1.0' encoding='UTF-8'?>
                          <filter:filter xmlns:filter='Filter' vhimVersion='Vhim_4_00'>
                          <filterId>APPOINTMENTS_SINGLE_PATIENT_FILTER</filterId>
                          <clientName>VRM 1.0</clientName>
                          <clientRequestInitiationTime>" + Datestring + @"</clientRequestInitiationTime>
                          <patients>
                          <NationalId>" + request.ICN + @"</NationalId>
                          </patients>
                          <entryPointFilter queryName='Appointment-Standardized'>
                                  <domainEntryPoint>Appointment</domainEntryPoint>
                                    <startDate>1950-01-01</startDate>
                                    <endDate>2050-01-01</endDate>
                         </entryPointFilter>
                      </filter:filter> ",
                    mcs_in2 = "APPOINTMENTS_SINGLE_PATIENT_FILTER",
                    mcs_in3 = "CDS3T",
                };

                #endregion

                // REM: Invoke VEIS Endpoint
                // var findExamAppointmentsResponse = findExamsAppointmentsRequest.SendReceive<VEISpwsreadDataResponse>(MessageProcessType.Local);
                var findExamAppointmentsResponse = WebApiUtility.SendReceive<VEISpwsreadDataResponse>(findExamsAppointmentsRequest, WebApiType.VEIS);
                progressString = "After VEIS EC Call";

                response.ExceptionMessage = findExamAppointmentsResponse.ExceptionMessage;
                response.ExceptionOccurred = findExamAppointmentsResponse.ExceptionOccurred;

                if (response.ExceptionOccurred)
                {
                    LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateAppointments Processor", "Exception Message calling EC: " + response.ExceptionMessage);
                    return response;
                }

                #region Map Response to Fields

                var appointmentCnt = 0;
                if (findExamAppointmentsResponse.mcs_respData != null)
                {
                    var requestCollection = new OrganizationRequestCollection();
                    XDocument ApptXml = XDocument.Parse(findExamAppointmentsResponse.mcs_respData);

                    var patients = ApptXml.Root.Elements("patients").Elements("patient");

                    var Appts =
                    (
                        from a in ApptXml.Root.Elements("patients").Elements("patient").Elements("appointments").Elements("appointment")
                        select a
                            );

                    #region Old XML mapping

                    IEnumerable<XElement> Appointments =
                    (
                        from a in ApptXml.Root.Elements("patients").Elements("patient").Elements("appointments").Elements("appointment")
                        select a
                            );

                    if (Appointments != null)
                    {
                        foreach (XElement Appointment in Appointments)
                        {
                            var thisNewEntity = new Entity { LogicalName = "udo_appointment" };
                            thisNewEntity["udo_name"] = "Appointment Summary";
                            if (request.ownerId != System.Guid.Empty)
                            {
                                thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                            }
                            appointmentCnt += 1;

                            if (Appointment.Elements("patient").Descendants("identity").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_patientid"] = (string)Appointment.Elements("patient").Descendants("identity").FirstOrDefault();
                            }
                            if (Appointment.Elements("appointmentDateTime").Descendants("literal").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_appointmentdate"] = XmlDateTimeFormatter((string)Appointment.Elements("appointmentDateTime").Descendants("literal").FirstOrDefault());
                                thisNewEntity["udo_dateofappointment"] = DateTime.Parse(XmlDateTimeFormatter((string)Appointment.Elements("appointmentDateTime").Descendants("literal").FirstOrDefault())).ToCRMDateTime();
                            }
                            if (Appointment.Elements("appointmentStatus").Descendants("displayText").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_appointmentstatus"] = (string)Appointment.Elements("appointmentStatus").Descendants("displayText").FirstOrDefault();
                            }
                            if (Appointment.Elements("location").Elements("institution").Descendants("name").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_institution"] = (string)Appointment.Elements("location").Elements("institution").Descendants("name").FirstOrDefault();
                            }
                            if (Appointment.Elements("location").Elements("indentifier").Descendants("name").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_location"] = (string)Appointment.Elements("location").Elements("indentifier").Descendants("name").FirstOrDefault();
                            }
                            if (Appointment.Elements("labDateTime").Descendants("literal").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_labdate"] = XmlDateTimeFormatter((string)Appointment.Elements("labDateTime").Descendants("literal").FirstOrDefault());
                            }
                            if (Appointment.Elements("status").Descendants("displayText").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_status"] = (string)Appointment.Elements("status").Descendants("displayText").FirstOrDefault();
                            }
                            if (Appointment.Elements("location").Descendants("telephone").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_telephone"] = (string)Appointment.Elements("location").Descendants("telephone").FirstOrDefault();
                            }
                            if (Appointment.Elements("appointmentType").Descendants("displayText").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_type"] = (string)Appointment.Elements("status").Descendants("displayText").FirstOrDefault();
                            }
                            if (request.UDOcreateUDOAppointmentRelatedEntitiesInfo != null)
                            {
                                foreach (var relatedReference in request.UDOcreateUDOAppointmentRelatedEntitiesInfo)
                                {
                                    thisNewEntity[relatedReference.RelatedEntityFieldName] = new EntityReference(relatedReference.RelatedEntityName, relatedReference.RelatedEntityId);
                                }
                            }

                            CreateRequest createAppointmentData = new CreateRequest
                            {
                                Target = thisNewEntity
                            };
                            requestCollection.Add(createAppointmentData);
                        }
                    }

                    #endregion

                    #endregion

                    #region Create records

                    if (requestCollection.Count() > 0)
                    {
                        var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, Guid.Empty, request.Debug);

                        if (_debug)
                        {
                            LogBuffer += result.LogDetail;
                            LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, LogBuffer);
                        }

                        if (result.IsFaulted)
                        {
                            LogHelper.LogError(request.OrganizationName, request.UserId, method, result.ErrorDetail);
                            response.ExceptionMessage = result.FriendlyDetail;
                            response.ExceptionOccurred = true;
                            return response;
                        }
                    }
                }

                string logInfo = string.Format("Number of Appointments Created: {0}", appointmentCnt);
                LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Appointment Records Created", logInfo);

                #endregion

                if (request.udo_appointmentId != System.Guid.Empty)
                {
                    var parent = new Entity();
                    parent.Id = request.RelatedParentId;
                    parent.LogicalName = request.RelatedParentEntityName;
                    parent["udo_appointmentcompleted"] = true;

                    //OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                    OrgServiceProxy.Update(parent);
                }

                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateAppointments Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to process Appointment Data";
                response.ExceptionOccurred = true;
                return response;
            }
            finally
            {
                if (OrgServiceProxy != null)
                {
                    OrgServiceProxy.Dispose();
                }
            }
        }

        #region Helper Methods

        public string XmlDateTimeFormatter(string input)
        {
            int hourTime = int.Parse(input.Substring(8, 2));
            var amPm = "AM";
            if (hourTime > 12)
            {
                hourTime = hourTime - 12;
                amPm = "PM";
            }
            var output = input.Substring(4, 2) + "/" + input.Substring(6, 2) + "/" + input.Substring(0, 4) + " " + hourTime.ToString() + ":" + input.Substring(10, 2) + " " + amPm;
            return output;
        }

        #endregion
        // REM: Invoke VEIS Endpoint
        private void InitProcessor(UDOcreateUDOAppointmentsRequest request)
        {
            try
            {
                if (logSettings == null)
                {
                    logSettings = new LogSettings
                    {
                        CallingMethod = method,
                        Org = request.OrganizationName,
                        UserId = request.UserId
                    };
                }
                NameValueCollection veisConfigurations = VEISConfiguration.GetConfigurationSettings();
                veisBaseUri = new Uri(veisConfigurations.Get(VEISConfiguration.ECUri));
            }
            catch
            {
                // TODO: Handle any exceptions
            }
        }

    }
}