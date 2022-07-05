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
    class UDOcreateUDOExamRequestProcessor
    {
        // OrganizationServiceProxy OrgServiceProxy;

        private CrmServiceClient OrgServiceProxy;
        private bool _debug { get; set; }

        private const string VEISBaseUrlAppSettingsKeyName = "VEISBaseUrl";

        private string LogBuffer { get; set; }

        private Uri veisBaseUri;

        private LogSettings logSettings { get; set; }

        private const string method = "UDOcreateUDOExamRequestProcessor";

        public IMessageBase Execute(UDOcreateUDOExamRequestRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.Execute");
            UDOcreateUDOExamRequestResponse response = new UDOcreateUDOExamRequestResponse();
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
                    StationNumber = request.HeaderInfo.StationNumber
                };
            }
            TraceLogger tLogger = new TraceLogger(method, request);

            LogHelper.LogInfo($">> {this.GetType().FullName}.Execute: Request:  {JsonHelper.Serialize<UDOcreateUDOExamRequestRequest>(request)}");

            #region connect to CRM
            try
            {
                tLogger.LogEvent($"Authenticated and Connected to CRM", "2");
                OrgServiceProxy = ConnectionCache.GetProxy();
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

                string mcs_in1_jsonString = "{\"filter:filter\": {\"-vhimVersion\": \"Vhim_4_00\"," +
                    "\"-xmlns:filter\": \"Filter\",\"filterId\": \"REQUESTS_AND_EXAMS_SINGLE_PATIENT_FILTER\"," +
                    "\"clientName\": \"VRM 1.0\"," +
                    "\"clientRequestInitiationTime\": " + Datestring + "," +
                    "\"patients\": { \"NationalId\": " + request.ICN + " }," +
                    "\"entryPointFilter\": {\"-queryName\": \"ExamRequest2507-Standardized\"," +
                    "\"domainEntryPoint\": \"ExamRequest2507\"}}}";

                // Replaced: VIMTpwsreadDataRequest = VEIS.PathWaysService.Messages.VEISpwsreadDataRequest
                var findExamRequestsRequest = new VEISpwsreadDataRequest
                {
                    // TODO: VEIS dependent missing mcs_transactionid
                    //       Removed to build but needed.
                    // mcs_transactionid = request.transactionId,
                    MessageId = request.MessageId,
                    LogTiming = request.LogTiming,
                    LogSoap = request.LogSoap,
                    Debug = request.Debug,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    // Replaced VIMT LegacyServiceHeaderInfo with HeaderInfo
                    LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = request.HeaderInfo.ApplicationName,
                        ClientMachine = request.HeaderInfo.ClientMachine,
                        LoginName = request.HeaderInfo.LoginName,
                        StationNumber = request.HeaderInfo.StationNumber
                    },
                    mcs_in0 = "RequestsAndExamsRead1",
                    // TODO: Convert XML to JSON
                    mcs_in1 = @"<?xml version='1.0' encoding='UTF-8'?>
                          <filter:filter xmlns:filter='Filter' vhimVersion='Vhim_4_00'>
                          <filterId>REQUESTS_AND_EXAMS_SINGLE_PATIENT_FILTER</filterId>
                          <clientName>VRM 1.0</clientName>
                          <clientRequestInitiationTime>" + Datestring + @"</clientRequestInitiationTime>
                          <patients>
                          <NationalId>" + request.ICN + @"</NationalId>
                          </patients>
                          <entryPointFilter queryName='ExamRequest2507-Standardized'>
                                  <domainEntryPoint>ExamRequest2507</domainEntryPoint>
                                    <startDate>1950-01-01</startDate>
                                    <endDate>2050-01-01</endDate>
                         </entryPointFilter>
                      </filter:filter> ",
                    mcs_in2 = "REQUESTS_AND_EXAMS_SINGLE_PATIENT_FILTER",
                    mcs_in3 = "SoapUI",
                };

                #endregion

                // REM: Invoke VEIS Endpoint
                var findExamRequestsResponse = WebApiUtility.SendReceive<VEISpwsreadDataResponse>(findExamRequestsRequest, WebApiType.VEIS);
                progressString = "After VEIS EC Call";

                response.ExceptionMessage = findExamRequestsResponse.ExceptionMessage;

                if (response.ExceptionOccurred)
                {
                    LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateUDOExamRequestProcessor Processor", "Exception Message calling EC: " + response.ExceptionMessage);
                    return response;
                }

                #region Map Response to Fields

                var examRequestCount = 0;
                if (findExamRequestsResponse.mcs_respData != null)
                {
                    var requestCollection = new OrganizationRequestCollection();

                    XDocument ApptXml = XDocument.Parse(findExamRequestsResponse.mcs_respData);

                    #region Old XML mapping
                    IEnumerable<XElement> ExamRequests =
                    (
                        from er in ApptXml.Root.Elements("patients").Elements("patient").Elements("examRequests").Elements("examRequest")
                        select er
                            );

                    if (ExamRequests != null)
                    {
                        foreach (XElement ExamRequest in ExamRequests)
                        {
                            var thisNewEntity = new Entity { LogicalName = "udo_examrequest" };
                            thisNewEntity["udo_name"] = "Exam Request Summary";
                            if (request.ownerId != System.Guid.Empty)
                            {
                                thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                            }

                            examRequestCount += 1;

                            if (ExamRequest.Elements("regionalOfficeNumber").Descendants("name").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_ro"] = (string)ExamRequest.Elements("regionalOfficeNumber").Descendants("name").FirstOrDefault();
                            }
                            if (ExamRequest.Elements("requestDate").Descendants("literal").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_requestdate"] = XmlDateTimeFormatter((string)ExamRequest.Elements("requestDate").Descendants("literal").FirstOrDefault());
                                thisNewEntity["udo_dateofrequest"] = DateTime.Parse(XmlDateTimeFormatter((string)ExamRequest.Elements("requestDate").Descendants("literal").FirstOrDefault())).ToCRMDateTime();
                            }
                            if (ExamRequest.Elements("requestor").Descendants("displayName").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_requester"] = (string)ExamRequest.Elements("requestor").Descendants("displayName").FirstOrDefault();
                            }
                            if (ExamRequest.Elements("priorityOfExam").Descendants("displayText").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_priority"] = (string)ExamRequest.Elements("priorityOfExam").Descendants("displayText").FirstOrDefault();
                            }
                            if (ExamRequest.Elements("status").Descendants("displayText").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_status"] = (string)ExamRequest.Elements("status").Descendants("displayText").FirstOrDefault();
                            }
                            if (ExamRequest.Descendants("otherDisabilitiesLine2").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_disabilities2"] = (string)ExamRequest.Descendants("otherDisabilitiesLine2").FirstOrDefault();
                            }
                            if (ExamRequest.Descendants("elapsedTime").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_elapsedtime"] = (string)ExamRequest.Descendants("elapsedTime").FirstOrDefault();
                            }
                            if (ExamRequest.Elements("claimFolderRequired").Descendants("displayText").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_claimfolderrequest"] = (string)ExamRequest.Elements("claimFolderRequired").Descendants("displayText").FirstOrDefault();
                            }
                            if (ExamRequest.Elements("routingLocation").Descendants("name").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_routinglocation"] = (string)ExamRequest.Elements("routingLocation").Descendants("name").FirstOrDefault();
                            }
                            if (request.UDOcreateUDOExamRelatedEntitiesInfo != null)
                            {
                                foreach (var relatedReference in request.UDOcreateUDOExamRelatedEntitiesInfo)
                                {
                                    thisNewEntity[relatedReference.RelatedEntityFieldName] = new EntityReference(relatedReference.RelatedEntityName, relatedReference.RelatedEntityId);
                                }
                            }

                            CreateRequest createExamRequestData = new CreateRequest
                            {
                                Target = thisNewEntity
                            };
                            requestCollection.Add(createExamRequestData);
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

                string logInfo = string.Format("Number of Exam Requests Created: {0}", examRequestCount);
                LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Exam Request Records Created", logInfo);

                #endregion

                if (request.udo_examId != System.Guid.Empty)
                {
                    var parent = new Entity();
                    parent.Id = request.RelatedParentId;
                    parent.LogicalName = request.RelatedParentEntityName;
                    parent["udo_examcompleted"] = true;
                    //OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                    OrgServiceProxy.Update(parent);
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                //LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOcreateExamRequests Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to process Exam Request Data";
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
            var output = "";
            if (input.Length < 8)
            {
                int hourTime = int.Parse(input.Substring(8, 2));
                var amPm = "AM";
                if (hourTime > 12)
                {
                    hourTime = hourTime - 12;
                    amPm = "PM";
                }
                output = input.Substring(4, 2) + "/" + input.Substring(6, 2) + "/" + input.Substring(0, 4) + " " + hourTime.ToString() + ":" + input.Substring(10, 2) + " " + amPm;
            }
            else output = input.Substring(4, 2) + "/" + input.Substring(6, 2) + "/" + input.Substring(0, 4);
            return output;
        }

        #endregion

        // REM: Invoke VEIS Endpoint
        private void InitProcessor(UDOcreateUDOExamRequestRequest request)
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
