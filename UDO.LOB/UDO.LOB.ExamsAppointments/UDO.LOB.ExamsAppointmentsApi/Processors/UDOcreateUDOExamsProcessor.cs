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
using Newtonsoft.Json.Linq;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.Xrm.Tooling.Connector;

//using VIMT.PathwaysService.Messages;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Logging.CRM.Util;
//using VRM.Integration.UDO.Common;
//using VRM.Integration.UDO.ExamsAppointments.Messages;

namespace UDO.LOB.ExamsAppointments.Processors
{
    class UDOcreateUDOExamsProcessor
    {
        // OrganizationServiceProxy OrgServiceProxy;

        private CrmServiceClient OrgServiceProxy = null;
        private bool _debug { get; set; }

        private const string VEISBaseUrlAppSettingsKeyName = "VEISBaseUrl";

        private string LogBuffer { get; set; }

        private Uri veisBaseUri;

        private LogSettings logSettings { get; set; }

        private const string method = "UDOcreateUDOExamsProcessor";

        public IMessageBase Execute(UDOcreateUDOExamRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.Execute"); //{JsonHelper.Serialize<UDOcreateAwardsRequest>(request)}");

            UDOcreateUDOExamResponse response = new UDOcreateUDOExamResponse();
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

            LogHelper.LogInfo($">> {this.GetType().FullName}.Execute: Request:  {JsonHelper.Serialize<UDOcreateUDOExamRequest>(request)}");

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

                string mcs_in1_jsonString = "{\"filter:filter\": {\"-vhimVersion\": \"Vhim_4_00\"," +
                    "\"-xmlns:filter\": \"Filter\"," +
                    "\"filterId\": \"REQUESTS_AND_EXAMS_SINGLE_PATIENT_FILTER\"," +
                    "\"clientName\": \"VRM 1.0\"," +
                    "\"clientRequestInitiationTime\": " + Datestring + "," +
                    "\"patients\": { \"NationalId\": " + request.ICN + " }," +
                    "\"entryPointFilter\": {\"-queryName\": \"Exam2507-Standardized\"," +
                    "\"domainEntryPoint\": \"Exam2507\"}}}";

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
                    LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    },
                    mcs_in0 = "RequestsAndExamsRead1",
                    mcs_in1 = @"<?xml version='1.0' encoding='UTF-8'?>
                          <filter:filter xmlns:filter='Filter' vhimVersion='Vhim_4_00'>
                          <filterId>REQUESTS_AND_EXAMS_SINGLE_PATIENT_FILTER</filterId>
                          <clientName>VRM 1.0</clientName>
                          <clientRequestInitiationTime>" + Datestring + @"</clientRequestInitiationTime>
                          <patients>
                          <NationalId>" + request.ICN + @"</NationalId>
                          </patients>
                          <entryPointFilter queryName='Exam2507-Standardized'>
                                  <domainEntryPoint>Exam2507</domainEntryPoint>
                                    <startDate>1950-01-01</startDate>
                                    <endDate>2050-01-01</endDate>
                         </entryPointFilter>
                      </filter:filter> ",

                    mcs_in2 = "REQUESTS_AND_EXAMS_SINGLE_PATIENT_FILTER",
                    mcs_in3 = "SoapUI",
                };

                #endregion
                var findExamRequestsResponse = WebApiUtility.SendReceive<VEISpwsreadDataResponse>(findExamRequestsRequest, WebApiType.VEIS);
                progressString = "After VEIS EC Call";

                response.ExceptionMessage = findExamRequestsResponse.ExceptionMessage;
                response.ExceptionOccurred = findExamRequestsResponse.ExceptionOccurred;

                if (response.ExceptionOccurred)
                {
                    LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateUDOExamsProcessor Processor", "Exception Message calling EC: " + response.ExceptionMessage);
                    return response;
                }

                #region Map Response to Fields

                var examCount = 0;

                if (findExamRequestsResponse.mcs_respData != null)
                {
                    var requestCollection = new OrganizationRequestCollection();

                    XDocument ApptXml = XDocument.Parse(findExamRequestsResponse.mcs_respData);
                    #region Old XML mapping

                    IEnumerable<XElement> Exams =
                    (
                        from e in ApptXml.Root.Elements("patients").Elements("patient").Elements("exams").Elements("exam")
                        select e
                            );

                    if (Exams != null)
                    {
                        foreach (XElement Exam in Exams)
                        {
                            var thisNewEntity = new Entity { LogicalName = "udo_exam" };
                            thisNewEntity["udo_name"] = "Exam Summary";
                            if (request.ownerId != System.Guid.Empty)
                            {
                                thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                            }
                            examCount += 1;
                            if (Exam.Descendants("printName").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_examdescription"] = (string)Exam.Descendants("printName").FirstOrDefault();
                            }
                            if (Exam.Descendants("examReferenceNumber").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_refnumber"] = (string)Exam.Descendants("examReferenceNumber").FirstOrDefault();
                            }
                            if (Exam.Elements("examType").Descendants("code").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_code"] = (string)Exam.Elements("examType").Descendants("code").FirstOrDefault();
                            }
                            if (Exam.Elements("status").Descendants("displayText").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_status"] = (string)Exam.Elements("status").Descendants("displayText").FirstOrDefault();
                            }
                            if (Exam.Elements("feeExam").Descendants("displayText").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_feeexam"] = (string)Exam.Elements("feeExam").Descendants("displayText").FirstOrDefault();
                            }
                            if (Exam.Elements("examPlace").Descendants("displayText").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_examplace"] = (string)Exam.Elements("examPlace").Descendants("displayText").FirstOrDefault();
                            }
                            if (Exam.Elements("request2507").Descendants("identity").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_request2507id"] = (string)Exam.Elements("request2507").Descendants("identity").FirstOrDefault();
                            }
                            if (Exam.Elements("request2507").Descendants("namespaceId").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_request2507"] = (string)Exam.Elements("request2507").Descendants("namespaceId").FirstOrDefault();
                            }
                            if (Exam.Elements("patient").Descendants("identity").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_patientid"] = (string)Exam.Elements("patient").Descendants("identity").FirstOrDefault();
                            }
                            if (Exam.Elements("dateOfExam").Descendants("literal").FirstOrDefault() != null)
                            {
                                thisNewEntity["udo_dateofexam"] = XmlDateTimeFormatter((string)Exam.Descendants("literal").FirstOrDefault());
                                thisNewEntity["udo_examdate"] = DateTime.Parse(XmlDateTimeFormatter((string)Exam.Descendants("literal").FirstOrDefault())).ToCRMDateTime();
                            }
                            if (request.UDOcreateUDOExamRelatedEntitiesInfo != null)
                            {
                                foreach (var relatedReference in request.UDOcreateUDOExamRelatedEntitiesInfo)
                                {
                                    thisNewEntity[relatedReference.RelatedEntityFieldName] = new EntityReference(relatedReference.RelatedEntityName, relatedReference.RelatedEntityId);
                                }
                            }

                            CreateRequest createExamData = new CreateRequest
                            {
                                Target = thisNewEntity
                            };
                            requestCollection.Add(createExamData);
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

                string logInfo = string.Format("Number of Exams Created: {0}", examCount);
                LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Exam Records Created", logInfo);

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
                // LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOcreateExamRequests Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to process Exams Data";
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
        private void InitProcessor(UDOcreateUDOExamRequest request)
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