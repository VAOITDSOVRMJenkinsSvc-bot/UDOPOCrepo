using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Specialized;
using UDO.LOB.Extensions.Configuration;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.LegacyPayments.Messages;
using VEIS.Core.Messages;
using VEIS.Messages.PaymentHistoryService;
using VEIS.Messages.PaymentInformationService;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Microsoft.Xrm.Tooling.Connector;

//using VIMT.PaymentHistoryService.Messages;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.Servicebus.Logging.CRM.Util;
//using VRM.Integration.UDO.Common;
//using VRM.Integration.UDO.LegacyPayments.Messages;

namespace UDO.LOB.LegacyPayments.Processors
{
    class UDOcreateUDOLegacyPaymentsProcessor
    {
        private const string LOG_CONFIG_FIELD = "mcs_createUDOLegacyPaymentData";
        private TimeTracker timer { get; set; }
        // OrganizationServiceProxy OrgServiceProxy;

        private CrmServiceClient OrgServiceProxy;
        private OrganizationWebProxyClient webProxyClient;
        private bool _debug { get; set; }

        private const string VEISBaseUrlAppSettingsKeyName = "VEISBaseUrl";

        private string LogBuffer { get; set; }

        private Uri veisBaseUri;

        private LogSettings logSettings { get; set; }

        private const string method = "UDOcreateUDOLegacyPaymentsProcessor";

        public IMessageBase Execute(UDOcreateLegacyPaymentsRequest request)
        {
            #region Start Timer

            if (request.LogTiming)
            {
                timer = new TimeTracker();
                timer.Restart();
            }

            #endregion

            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.Execute");

            UDOcreateLegacyPaymentsResponse response = new UDOcreateLegacyPaymentsResponse();
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
                timer.Stop();
                timer = null;
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

            LogHelper.LogInfo($">> {this.GetType().FullName}.Execute: Request:  {JsonHelper.Serialize<UDOcreateLegacyPaymentsRequest>(request)}");

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
                // prefix = payHistSSN_findPayHistoryBySSNRequest();
                if (request.LogTiming) timer.MarkStart("findPayHistoryBySSNResponse");
                var findPayHistoryBySSNRequest = new VEISpayHistSSN_findPayHistoryBySSNRequest
                {
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
                    mcs_ssn = String.IsNullOrEmpty(request.filenumber) ? request.ssn : request.filenumber
                };

                // REM: Invoke VEIS Endpoint
                // var findPayHistoryBySSNResponse = findPayHistoryBySSNRequest.SendReceive<VIMTpayHistSSN_findPayHistoryBySSNResponse>(MessageProcessType.Local);
                var findPayHistoryBySSNResponse = WebApiUtility.SendReceive<VEISpayHistSSN_findPayHistoryBySSNResponse>(findPayHistoryBySSNRequest, WebApiType.VEIS);
                if (request.LogTiming) timer.MarkStop("findPayHistoryBySSNResponse");
                progressString = "After VEIS EC Call";

                if (request.LogSoap || findPayHistoryBySSNResponse.ExceptionOccurred)
                {
                    if (findPayHistoryBySSNResponse.SerializedSOAPRequest != null || findPayHistoryBySSNResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findPayHistoryBySSNResponse.SerializedSOAPRequest + findPayHistoryBySSNResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISpayHistSSN_findPayHistoryBySSNRequest Request/Response {requestResponse}", true);
                    }
                }

                response.ExceptionMessage = findPayHistoryBySSNResponse.ExceptionMessage;
                response.ExceptionOccurred = findPayHistoryBySSNResponse.ExceptionOccurred;

                var requestCollection = new OrganizationRequestCollection();
                #region process response
                if (findPayHistoryBySSNResponse != null)
                {
                    // Replaced:  VIMTpayHistSSN_Info = VEISpayHistSSN_returnInfo
                    if (findPayHistoryBySSNResponse.VEISpayHistSSN_Info != null)
                    {

                        if (findPayHistoryBySSNResponse.VEISpayHistSSN_Info.VEISpayHistSSN_paymentRecordInfo != null)
                        {
                            var legacyPayments = findPayHistoryBySSNResponse.VEISpayHistSSN_Info.VEISpayHistSSN_paymentRecordInfo;


                            //instantiate the new Entity
                            Entity thisNewEntity = new Entity();
                            thisNewEntity.LogicalName = "udo_legacypaymenthistory";
                            thisNewEntity["udo_name"] = "Legacy Payment Summary";
                            if (request.ownerId != System.Guid.Empty)
                            {
                                thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                            }
                            if (!string.IsNullOrEmpty(findPayHistoryBySSNResponse.VEISpayHistSSN_Info.VEISpayHistSSN_paymentRecordInfo.mcs_fileNumber))
                            {
                                thisNewEntity["udo_filenumber"] = findPayHistoryBySSNResponse.VEISpayHistSSN_Info.VEISpayHistSSN_paymentRecordInfo.mcs_fileNumber;
                            }
                            if (!string.IsNullOrEmpty(findPayHistoryBySSNResponse.VEISpayHistSSN_Info.VEISpayHistSSN_paymentRecordInfo.mcs_fullName))
                            {
                                thisNewEntity["udo_fullname"] = findPayHistoryBySSNResponse.VEISpayHistSSN_Info.VEISpayHistSSN_paymentRecordInfo.mcs_fullName;
                            }
                            if (!string.IsNullOrEmpty(findPayHistoryBySSNResponse.VEISpayHistSSN_Info.VEISpayHistSSN_paymentRecordInfo.mcs_lastActivityDt))
                            {
                                thisNewEntity["udo_lastactivitydate"] = findPayHistoryBySSNResponse.VEISpayHistSSN_Info.VEISpayHistSSN_paymentRecordInfo.mcs_lastActivityDt;
                            }
                            if (!string.IsNullOrEmpty(findPayHistoryBySSNResponse.VEISpayHistSSN_Info.VEISpayHistSSN_paymentRecordInfo.mcs_lastFicheDt))
                            {
                                thisNewEntity["udo_lastfichedate"] = findPayHistoryBySSNResponse.VEISpayHistSSN_Info.VEISpayHistSSN_paymentRecordInfo.mcs_lastFicheDt;
                            }
                            if (!string.IsNullOrEmpty(findPayHistoryBySSNResponse.VEISpayHistSSN_Info.VEISpayHistSSN_paymentRecordInfo.mcs_payeeCode))
                            {
                                thisNewEntity["udo_payee"] = findPayHistoryBySSNResponse.VEISpayHistSSN_Info.VEISpayHistSSN_paymentRecordInfo.mcs_payeeCode;
                            }
                            if (!string.IsNullOrEmpty(findPayHistoryBySSNResponse.VEISpayHistSSN_Info.VEISpayHistSSN_paymentRecordInfo.mcs_priorFicheDt))
                            {
                                thisNewEntity["udo_priorfichedate"] = findPayHistoryBySSNResponse.VEISpayHistSSN_Info.VEISpayHistSSN_paymentRecordInfo.mcs_priorFicheDt;
                            }

                            if (request.UDOcreateUDOLegacyPaymentsRelatedEntitiesInfo != null)
                            {
                                foreach (var relatedItem in request.UDOcreateUDOLegacyPaymentsRelatedEntitiesInfo)
                                {
                                    thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                }
                            }
                            CreateRequest createPayments = new CreateRequest
                            {
                                Target = thisNewEntity
                            };
                            requestCollection.Add(createPayments);
                        }
                    }
                }
                if (request.LogTiming) timer.MarkStop("Processing Data");
                #endregion

                if (requestCollection.Count > 0)
                {
                    #region Execute Multiple

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

                    string logInfo = string.Format("Number of Legacy Payment Records Created: {0}", requestCollection.Count);
                    LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, method, logInfo);
                    #endregion
                }

                if (request.idProofId != Guid.Empty)
                {
                    var idProof = new Entity();
                    idProof.Id = request.idProofId;
                    idProof.LogicalName = "udo_idproof";
                    idProof["udo_legacypaymentintegration"] = new OptionSetValue(752280002);
                    //OrgServiceProxy.Update(TruncateHelper.TruncateFields(idProof, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                    OrgServiceProxy.Update(idProof);
                }
                else
                {
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method, "no idProofId found");
                }

                #region Stop Timer

                if (request.LogTiming)
                {
                    var elapsedMilliseconds = timer.LogDurations(request.OrganizationName, request.Debug, request.UserId,
                        string.Format("{0}, Progress: {1}", method, progressString), true);
                    LogHelper.LogTiming(request.OrganizationName, request.Debug, request.UserId,
                        request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method,
                        method, timer.ElapsedMilliseconds);
                    //Convert.ToDecimal(elapsedMilliseconds));
                }
                #endregion
                timer = null;
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "UDOcreateUDOLegacyPaymentsProcessor Processor, Progess:" + progressString, ExecutionException);

                if (request.LogTiming)
                {
                    timer.LogDurations(request.OrganizationName, request.Debug, request.UserId, method, true);
                    timer = null;
                }
                response.ExceptionMessage = "Failed to process Legacy payment Data";
                response.ExceptionOccurred = true;
                if (request.idProofId != Guid.Empty)
                {
                    var idProof = new Entity();
                    idProof.Id = request.idProofId;
                    idProof.LogicalName = "udo_idproof";
                    idProof["udo_legacypaymentintegration"] = new OptionSetValue(752280003);
                    //OrgServiceProxy.Update(TruncateHelper.TruncateFields(idProof, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                    OrgServiceProxy.Update(idProof);
                }
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
        // REM: Invoke VEIS Endpoint
        private void InitProcessor(UDOcreateLegacyPaymentsRequest request)
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