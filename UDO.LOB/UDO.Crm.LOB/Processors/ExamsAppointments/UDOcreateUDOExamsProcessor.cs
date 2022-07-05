using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using VIMT.PathwaysService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Common;
using VRM.Integration.UDO.ExamsAppointments.Messages;

namespace VRM.Integration.UDO.ExamsAppointments.Processors
{
    class UDOcreateUDOExamsProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOcreateUDOExamsProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOcreateUDOExamRequest request)
        {
            UDOcreateUDOExamResponse response = new UDOcreateUDOExamResponse();
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                return response;
            }

            OrganizationServiceProxy OrgServiceProxy;

            #region connect to CRM
            try
            {
                var CommonFunctions = new CRMConnect();

                OrgServiceProxy = CommonFunctions.ConnectToCrm(request.OrganizationName);

            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateUDOExamProcessor Processor, Connection Error", connectException.Message);                
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

            progressString = "After Connection";

            try
            {
                #region Create EC Request

                var Datestring = DateTime.Now.ToString("yyyy-MM-dd") + "T" + DateTime.Now.ToString("hh:mm:ss");
                
                var findExamRequestsRequest = new VIMTpwsreadDataRequest
                {
                    mcs_transactionid = request.transactionId,
                    LogTiming = request.LogTiming,
                    LogSoap = request.LogSoap,
                    Debug = request.Debug,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    LegacyServiceHeaderInfo = new VIMT.PathwaysService.Messages.HeaderInfo
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
                         </entryPointFilter>
                         </filter:filter> ",
                    mcs_in2 = "REQUESTS_AND_EXAMS_SINGLE_PATIENT_FILTER",
                    mcs_in3 = "CDS3T",
                };

                #endregion

                var findExamRequestsResponse = findExamRequestsRequest.SendReceive<VIMTpwsreadDataResponse>(MessageProcessType.Local);
                progressString = "After VIMT EC Call";

                response.ExceptionMessage = findExamRequestsResponse.ExceptionMessage;
                response.ExceptionOccured = findExamRequestsResponse.ExceptionOccured;

                if (response.ExceptionOccured)
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

                    #region Create records

                    if (requestCollection.Count() > 0)
                    {
                        var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, request.UserId, request.Debug);

                        if (_debug)
                        {
                            LogBuffer += result.LogDetail;
                            LogHelper.LogDebug(request.OrganizationName, _debug, request.UserId, method, LogBuffer);
                        }

                        if (result.IsFaulted)
                        {
                            LogHelper.LogError(request.OrganizationName, request.UserId, method, result.ErrorDetail);
                            response.ExceptionMessage = result.FriendlyDetail;
                            response.ExceptionOccured = true;
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
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming));
                }

                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOcreateExamRequests Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to process Exams Data";
                response.ExceptionOccured = true;
                return response;
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
    }
}