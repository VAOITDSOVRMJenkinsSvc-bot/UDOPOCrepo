using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.Payments.Messages;
using VEIS.Core.Messages;
using VEIS.Messages.PaymentInformationService;

/// <summary>
/// VIMT LOB Component for UDOcreatePaymentAdjustments,createPaymentAdjustments method, Processor.
/// </summary>
namespace UDO.LOB.Payments.Processors
{
    class UDOcreatePaymentAdjustmentsProcessor
    {
        private bool _debug { get; set; }

        private string LogBuffer { get; set; }

        private const string method = "UDOcreatePaymentAdjustmentsProcessor";

        public IMessageBase Execute(UDOcreatePaymentAdjustmentsRequest request)
        {
            //var request = message as createPaymentAdjustmentsRequest;
            UDOcreatePaymentAdjustmentsResponse response = new UDOcreatePaymentAdjustmentsResponse
            {
                MessageId = request.MessageId
            };

            var progressString = "Top of Processor";
            
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

            TraceLogger aiLogger = new TraceLogger($">> Entered {this.GetType().FullName}.createPaymentAdjustment", request);

            #region connect to CRM
            CrmServiceClient OrgServiceProxy = null;

            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
                aiLogger.LogEvent($"Authenticated and Connected to CRM", "2");
            }
            catch (Exception connectException)
            {
                aiLogger.LogException(connectException, "990");
                LogHelper.LogError(request.OrganizationName, request.UserId, "mcs_createPaymentAdjustments - UDOcreatePaymentAdjustmentsProcessor Processor, Progess:" + progressString, connectException);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccurred = true;
                return response;
            }
            #endregion

            progressString = "After Connection";

            try
            {
                var retrievePaymentDetailRequest = new VEISrtrpmtdtlretrievePaymentDetailRequest();
                retrievePaymentDetailRequest.MessageId = request.MessageId;
                retrievePaymentDetailRequest.LogTiming = request.LogTiming;
                retrievePaymentDetailRequest.LogSoap = request.LogSoap;
                retrievePaymentDetailRequest.Debug = request.Debug;
                retrievePaymentDetailRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                retrievePaymentDetailRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                retrievePaymentDetailRequest.RelatedParentId = request.RelatedParentId;
                retrievePaymentDetailRequest.UserId = request.UserId;
                retrievePaymentDetailRequest.OrganizationName = request.OrganizationName;

                retrievePaymentDetailRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };

                retrievePaymentDetailRequest.mcs_paymentid = request.PaymentId;
                retrievePaymentDetailRequest.mcs_fbtid = request.FbtId;

                var retrievePaymentDetailResponse = WebApiUtility.SendReceive<VEISrtrpmtdtlretrievePaymentDetailResponse>(retrievePaymentDetailRequest, WebApiType.VEIS);
                if (request.LogSoap || retrievePaymentDetailResponse.ExceptionOccurred)
                {
                    if (retrievePaymentDetailResponse.SerializedSOAPRequest != null || retrievePaymentDetailResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = retrievePaymentDetailResponse.SerializedSOAPRequest + retrievePaymentDetailResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"Request Request/Response {requestResponse}", true);
                    }
                }

                progressString = "After VEIS EC Call";
                var paymentadjustment = 0;
                var requestCollection = new OrganizationRequestCollection();
                response.ExceptionMessage = retrievePaymentDetailResponse.ExceptionMessage;
                response.ExceptionOccurred = retrievePaymentDetailResponse.ExceptionOccurred;

                if (response.ExceptionOccurred)
                {
                    throw new ApplicationException(retrievePaymentDetailResponse.ExceptionMessage);
                }

                if (retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo.VEISrtrpmtdtlpaymentAdjustmentsInfo.VEISrtrpmtdtlpaymentAdjustmentListInfo != null)
                {
                    var paymentAdjustmentVO = retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo.VEISrtrpmtdtlpaymentAdjustmentsInfo.VEISrtrpmtdtlpaymentAdjustmentListInfo;
                    System.Collections.Generic.List<UDOcreatePaymentAdjustmentsMultipleResponse> UDOcreatePaymentAdjustmentsArray = new System.Collections.Generic.List<UDOcreatePaymentAdjustmentsMultipleResponse>();
                    foreach (var paymentAdjustmentVOItem in paymentAdjustmentVO)
                    {
                        var responseIds = new UDOcreatePaymentAdjustmentsMultipleResponse();
                        //instantiate the new Entity
                        Entity thisNewEntity = new Entity();
                        thisNewEntity.LogicalName = "udo_paymentadjustment";
                        if (request.ownerId != System.Guid.Empty)
                        {
                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                        }
                        if (paymentAdjustmentVOItem.mcs_adjustmentType != string.Empty)
                        {
                            thisNewEntity["udo_adjustmenttype"] = paymentAdjustmentVOItem.mcs_adjustmentType;
                        }
                        if (paymentAdjustmentVOItem.mcs_adjustmentSource != string.Empty)
                        {
                            thisNewEntity["udo_adjustmentsource"] = paymentAdjustmentVOItem.mcs_adjustmentSource;
                        }
                        if (paymentAdjustmentVOItem.mcs_adjustmentReason != string.Empty)
                        {
                            thisNewEntity["udo_adjustmentreason"] = paymentAdjustmentVOItem.mcs_adjustmentReason;
                        }
                        if (paymentAdjustmentVOItem.mcs_adjustmentOperation != string.Empty)
                        {
                            thisNewEntity["udo_adjustmentoperation"] = paymentAdjustmentVOItem.mcs_adjustmentOperation;
                        }
                        if (paymentAdjustmentVOItem.mcs_adjustmentCategory != string.Empty)
                        {
                            thisNewEntity["udo_adjustmentcategory"] = paymentAdjustmentVOItem.mcs_adjustmentCategory;
                        }
                        thisNewEntity["udo_adjustmentamount"] = moneyStringFormat(paymentAdjustmentVOItem.mcs_adjustmentAmount.ToString());
                        if (request.UDOcreatePaymentAdjustmentsRelatedEntitiesInfo != null)
                        {
                            foreach (var relatedItem in request.UDOcreatePaymentAdjustmentsRelatedEntitiesInfo)
                            {
                                thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                            }
                        }
                        CreateRequest createExamData = new CreateRequest
                        {
                            Target = thisNewEntity
                        };
                        requestCollection.Add(createExamData);
                        paymentadjustment += 1;
                    }
                }
                if (paymentadjustment > 0)
                {
                    #region Execute Multiple

                    var emRequest = new ExecuteMultipleRequest
                    {
                        Requests = requestCollection,
                        Settings = new ExecuteMultipleSettings
                        {
                            ContinueOnError = false,
                            ReturnResponses = false,
                        }
                    };

                    ExecuteMultipleResponse responseWithNoResults = (ExecuteMultipleResponse)OrgServiceProxy.Execute(emRequest);
                    if (responseWithNoResults.Responses.Count > 0)
                    {
                        foreach (var responseItem in responseWithNoResults.Responses)
                        {
                            if (responseItem.Fault != null)
                            {
                                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, "Progress:" + progressString + " Fault Detail: " + responseItem.Fault.Message);
                                response.ExceptionMessage = "Failed to Map EC data to LOB";
                                response.ExceptionOccurred = true;
                            }
                        }
                    }
                    #endregion
                }

                string logInfo = string.Format("Number of Payment Adjustments Created: {0}", paymentadjustment);
                LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, method, logInfo);
                //added to generated code
                if (request.udo_paymentId != System.Guid.Empty)
                {
                    var parent = new Entity();
                    parent.Id = request.udo_paymentId;
                    parent.LogicalName = "udo_payment";
                    parent["udo_paymentadjustmentcomplete"] = true;
                    OrgServiceProxy.Update(parent);
                }
                return response;
            }
            catch (Exception connectException)
            {
                aiLogger.LogException(connectException, "999");

                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreatePaymentAdjustmentsProcessor Processor, Progess:" + progressString, connectException);
                response.ExceptionMessage = "Failed to Map EC data to LOB";
                response.ExceptionOccurred = true;
                if (request.udo_paymentId != System.Guid.Empty)
                {
                    var parent = new Entity();
                    parent.Id = request.udo_paymentId;
                    parent.LogicalName = "udo_payment";
                    OrgServiceProxy.Update(parent);
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
        private static string moneyStringFormat(string thisField)
        {
            var returnField = "";
            try
            {
                Double newValue = 0;
                if (Double.TryParse(thisField, out newValue))
                {
                    returnField = string.Format("{0:C}", newValue);
                }
                else
                {
                    returnField = "$0.00";
                }
            }
            catch (Exception ex)
            {
                returnField = ex.Message;
            }
            return returnField;
        }
    }
}