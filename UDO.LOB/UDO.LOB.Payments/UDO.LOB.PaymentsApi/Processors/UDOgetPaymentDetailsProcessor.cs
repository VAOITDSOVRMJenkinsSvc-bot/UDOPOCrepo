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

namespace UDO.LOB.Payments.Processors
{
    class UDOgetPaymentDetailsProcessor
    {
        private bool _debug { get; set; }

        private string LogBuffer { get; set; }

        private const string method = "UDOgetPaymentDetailsProcessor";

        public IMessageBase Execute(UDOgetPaymentDetailsRequest request)
        {
            UDOgetPaymentDetailsResponse response = new UDOgetPaymentDetailsResponse
            {
                MessageId = request.MessageId
            };

            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;
            
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

            TraceLogger aiLogger = new TraceLogger($">> Entered {this.GetType().FullName}.getPaymentDetails", request);

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
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOgetPaymentDetailsProcessor Processor, Connection Error", connectException.Message);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccurred = true;
                return response;
            }
            #endregion

            try
            {
                var retrievePaymentDetailRequest = new VEISrtrpmtdtlretrievePaymentDetailRequest
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
                    mcs_paymentid = request.PaymentId,
                    mcs_fbtid = request.FbtId,
                    LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    },
                };

                // REM: Invoke VEIS Endpoint
                var retrievePaymentDetailResponse = WebApiUtility.SendReceive<VEISrtrpmtdtlretrievePaymentDetailResponse>(retrievePaymentDetailRequest, WebApiType.VEIS);
                if (request.LogSoap || retrievePaymentDetailResponse.ExceptionOccurred)
                {
                    if (retrievePaymentDetailResponse.SerializedSOAPRequest != null || retrievePaymentDetailResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = retrievePaymentDetailResponse.SerializedSOAPRequest + retrievePaymentDetailResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISrtrpmtdtlretrievePaymentDetailRequest Request/Response {requestResponse}", true);
                    }
                }
                progressString = "After VEIS EC Call";

                response.ExceptionMessage = retrievePaymentDetailResponse.ExceptionMessage;
                response.ExceptionOccurred = retrievePaymentDetailResponse.ExceptionOccurred;

                if (response.ExceptionOccurred)
                {
                    throw new ApplicationException(retrievePaymentDetailResponse.ExceptionMessage);
                }

                response.UDOgetPaymentDetailsInfo = new UDOgetPaymentDetails();
                var paymentAdjustmentCount = 0;
                var awardadjustmentCount = 0;
                var requestCollection = new OrganizationRequestCollection();

                if (retrievePaymentDetailResponse != null)
                {
                    if (retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo != null)
                    {
                        #region info sent back in response
                        #region PaymentDetailsAwardAdjustment

                        Entity currentEntity = new Entity("udo_payment");
                        currentEntity.Id = request.udo_paymentId;

                        if (retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo.VEISrtrpmtdtlawardAdjustmentsInfo != null)
                        {
                            progressString = "Beginning Mapping Payment Details: Award Adjustment";

                            var awardAdjustmentResponse = retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo.VEISrtrpmtdtlawardAdjustmentsInfo;
                            progressString = "Map: Gross Award Amount";

                            currentEntity["udo_grossawardamount"] = moneyStringFormat(awardAdjustmentResponse.mcs_grossAwardAmount.ToString());
                            currentEntity["udo_netawardamount"] = moneyStringFormat(awardAdjustmentResponse.mcs_netAwardAmount.ToString());
                            if (awardAdjustmentResponse.mcs_awardEffectiveDate != null)
                            {
                                DateTime tryDate;
                                if (DateTime.TryParse(awardAdjustmentResponse.mcs_awardEffectiveDate, out tryDate))
                                {
                                    if (tryDate != DateTime.MinValue)
                                    {
                                        currentEntity["udo_awardeffectivedate"] = tryDate;
                                    }
                                }
                            }
                            progressString = "Completed Mapping Payment Details: Award Adjustment";
                        }
                        #endregion

                        #region PaymentDetailsPaymentAdjustment
                        if (retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo.VEISrtrpmtdtlpaymentAdjustmentsInfo != null)
                        {
                            //Map Details: Payment Adjustment Information
                            progressString = "Beginning Mapping Payment Details: Payment Adjustment";

                            var paymentAdjustmentResponse = retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo.VEISrtrpmtdtlpaymentAdjustmentsInfo;

                            currentEntity["udo_grosspaymentamount"] = moneyStringFormat(paymentAdjustmentResponse.mcs_grossPaymentAmount.ToString());
                            currentEntity["udo_netpaymentamount"] = moneyStringFormat(paymentAdjustmentResponse.mcs_netPaymentAmount.ToString());
                            if (request.payeecodeid != Guid.Empty)
                            {
                                Entity payeeCode = new Entity();
                                payeeCode.LogicalName = "udo_payeecode";
                                payeeCode.Id = request.payeecodeid;
                                payeeCode["udo_grossamount"] = moneyStringFormat(paymentAdjustmentResponse.mcs_grossPaymentAmount.ToString());
                                payeeCode["udo_netamount"] = moneyStringFormat(paymentAdjustmentResponse.mcs_netPaymentAmount.ToString());
                                OrgServiceProxy.Update(TruncateHelper.TruncateFields(request.MessageId, payeeCode, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                            }
                            if (request.udo_personId != Guid.Empty)
                            {
                                Entity person = new Entity();
                                person.LogicalName = "udo_person";
                                person.Id = request.udo_personId;
                                person["udo_grossamount"] = moneyStringFormat(paymentAdjustmentResponse.mcs_grossPaymentAmount.ToString());
                                person["udo_netamount"] = moneyStringFormat(paymentAdjustmentResponse.mcs_netPaymentAmount.ToString());
                                if (retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo.VEISrtrpmtdtlawardAdjustmentsInfo != null)
                                {
                                    if (retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo.VEISrtrpmtdtlawardAdjustmentsInfo.mcs_awardEffectiveDate != null)
                                    {
                                        DateTime tryDate;
                                        if (DateTime.TryParse(retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo.VEISrtrpmtdtlawardAdjustmentsInfo.mcs_awardEffectiveDate, out tryDate))
                                        {
                                            if (tryDate != DateTime.MinValue)
                                            {
                                                person["udo_awardeffectivedate"] = tryDate;
                                            }
                                        }
                                    }
                                }
                                person["udo_paymentcomplete"] = true;
                                OrgServiceProxy.Update(TruncateHelper.TruncateFields(request.MessageId, person, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                            }
                            progressString = "Completed Mapping Payment Details: Payment Adjustment";
                        }
                        #endregion

                        #region PaymentDetailsAwardReason
                        if (retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo.VEISrtrpmtdtlawardAdjustmentsInfo != null)
                        {

                            if (retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo.VEISrtrpmtdtlawardAdjustmentsInfo.VEISrtrpmtdtlawardReasonListInfo != null)
                            {
                                //Map Details: Award Reason List
                                progressString = "Beginning Mapping Payment Details: Award Reasons";

                                var AwardReasons = retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo.VEISrtrpmtdtlawardAdjustmentsInfo.VEISrtrpmtdtlawardReasonListInfo;

                                foreach (var ReasonInfo in AwardReasons)
                                {
                                    if (ReasonInfo.mcs_awardReasonText != null)
                                    {
                                        //If Reason is Empty, Do not Concat. a Comma
                                        if (response.UDOgetPaymentDetailsInfo.udo_Reason == null)
                                        {
                                            response.UDOgetPaymentDetailsInfo.udo_Reason = ReasonInfo.mcs_awardReasonText;
                                            currentEntity["udo_reason"] = ReasonInfo.mcs_awardReasonText;
                                        }
                                        //If Reason has First Reason Populated, Add and Concat.
                                        else
                                        {
                                            response.UDOgetPaymentDetailsInfo.udo_Reason = response.UDOgetPaymentDetailsInfo.udo_Reason + " , " + ReasonInfo.mcs_awardReasonText;
                                            currentEntity["udo_reason"] = response.UDOgetPaymentDetailsInfo.udo_Reason + " , " + ReasonInfo.mcs_awardReasonText;
                                        }
                                    }
                                }
                                progressString = "Completed Mapping Payment Details: Award Reasons";
                            }
                        }

                        #endregion

                        UpdateRequest updatePaymentData = new UpdateRequest
                        {
                            Target = currentEntity
                        };
                        requestCollection.Add(updatePaymentData);
                        #endregion

                        #region CreatePaymentAdjustments
                        if (retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo.VEISrtrpmtdtlpaymentAdjustmentsInfo != null)
                        {
                            if (retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo.VEISrtrpmtdtlpaymentAdjustmentsInfo.VEISrtrpmtdtlpaymentAdjustmentListInfo != null)
                            {
                                #region
                                var paymentAdjustmentList = retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo.VEISrtrpmtdtlpaymentAdjustmentsInfo.VEISrtrpmtdtlpaymentAdjustmentListInfo;
                                System.Collections.Generic.List<UDOgetPaymentAdjustmentsMultipleResponse> UDOcreatePaymentAdjustmentsArray = new System.Collections.Generic.List<UDOgetPaymentAdjustmentsMultipleResponse>();
                                progressString = "Beginning Payment Adjustment Creation";

                                foreach (var paymentAdjustmentItem in paymentAdjustmentList)
                                {
                                    var responseIds = new UDOgetPaymentAdjustmentsMultipleResponse();
                                    Entity thisNewEntity = new Entity();
                                    thisNewEntity.LogicalName = "udo_paymentadjustment";
                                    if (request.ownerId != System.Guid.Empty)
                                    {
                                        thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                    }
                                    if (paymentAdjustmentItem.mcs_adjustmentType != null)
                                    {
                                        thisNewEntity["udo_adjustmenttype"] = paymentAdjustmentItem.mcs_adjustmentType;
                                    }
                                    if (paymentAdjustmentItem.mcs_adjustmentCategory != null)
                                    {
                                        thisNewEntity["udo_adjustmentcategory"] = paymentAdjustmentItem.mcs_adjustmentCategory;
                                    }
                                    if (paymentAdjustmentItem.mcs_adjustmentOperation != null)
                                    {
                                        thisNewEntity["udo_adjustmentoperation"] = paymentAdjustmentItem.mcs_adjustmentOperation;
                                    }

                                    thisNewEntity["udo_adjustmentamount"] = moneyStringFormat(paymentAdjustmentItem.mcs_adjustmentAmount.ToString());

                                    if (paymentAdjustmentItem.mcs_adjustmentReason != null)
                                    {
                                        thisNewEntity["udo_adjustmentreason"] = paymentAdjustmentItem.mcs_adjustmentReason;
                                    }
                                    if (paymentAdjustmentItem.mcs_adjustmentSource != null)
                                    {
                                        thisNewEntity["udo_adjustmentsource"] = paymentAdjustmentItem.mcs_adjustmentSource;
                                    }
                                    foreach (var relatedItem in request.UDOcreateRelatedEntitiesInfo)
                                    {
                                        thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                    }
                                    CreateRequest createExamData = new CreateRequest
                                    {
                                        Target = thisNewEntity
                                    };
                                    requestCollection.Add(createExamData);
                                    paymentAdjustmentCount += 1;

                                }
                                #endregion

                            }

                        }
                        #endregion

                        #region CreateAwardAdjustments
                        if (retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo.VEISrtrpmtdtlawardAdjustmentsInfo != null)
                        {
                            if (retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo.VEISrtrpmtdtlawardAdjustmentsInfo.VEISrtrpmtdtlawardAdjustmentListInfo != null)
                            {
                                var awardAdjustmentList = retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo.VEISrtrpmtdtlawardAdjustmentsInfo.VEISrtrpmtdtlawardAdjustmentListInfo;
                                System.Collections.Generic.List<UDOgetAwardAdjustmentsMultipleResponse> UDOcreateAwardAdjustmentsArray = new System.Collections.Generic.List<UDOgetAwardAdjustmentsMultipleResponse>();
                                progressString = "Beginning Award Adjustment Creation";

                                foreach (var awardAdjustmentItem in awardAdjustmentList)
                                {
                                    var responseIds = new UDOgetAwardAdjustmentsMultipleResponse();
                                    Entity thisNewEntity = new Entity();
                                    thisNewEntity.LogicalName = "udo_awardadjustment";
                                    if (request.ownerId != System.Guid.Empty)
                                    {
                                        thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                    }
                                    if (awardAdjustmentItem.mcs_adjustmentType != null)
                                    {
                                        thisNewEntity["udo_adjustmenttype"] = awardAdjustmentItem.mcs_adjustmentType;
                                    }
                                    if (awardAdjustmentItem.mcs_adjustmentEffectiveDate != null)
                                    {
                                        DateTime tryDate;
                                        if (DateTime.TryParse(awardAdjustmentItem.mcs_adjustmentEffectiveDate, out tryDate))
                                        {
                                            if (tryDate != DateTime.MinValue)
                                            {
                                                thisNewEntity["udo_adjustmenteffectivedate"] = tryDate;
                                            }
                                        }
                                    }
                                    if (awardAdjustmentItem.mcs_adjustmentOperation != null)
                                    {
                                        thisNewEntity["udo_adjustmentoperation"] = awardAdjustmentItem.mcs_adjustmentOperation;
                                    }

                                    thisNewEntity["udo_adjustmentamount"] = moneyStringFormat(awardAdjustmentItem.mcs_adjustmentAmount.ToString());

                                    if (awardAdjustmentItem.mcs_alloteeRelationship != null)
                                    {
                                        thisNewEntity["udo_alloteerelationship"] = awardAdjustmentItem.mcs_alloteeRelationship;
                                    }
                                    if (awardAdjustmentItem.mcs_allotmentRecipientName != null)
                                    {
                                        thisNewEntity["udo_alottmentrecipient"] = awardAdjustmentItem.mcs_allotmentRecipientName;
                                    }
                                    if (awardAdjustmentItem.mcs_allotmentType != null)
                                    {
                                        thisNewEntity["udo_alottmenttype"] = awardAdjustmentItem.mcs_allotmentType;
                                    }
                                    foreach (var relatedItem in request.UDOcreateRelatedEntitiesInfo)
                                    {
                                        thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                    }

                                    CreateRequest createExamData = new CreateRequest
                                    {
                                        Target = thisNewEntity
                                    };
                                    requestCollection.Add(createExamData);
                                    awardadjustmentCount += 1;
                                }

                            }
                        }
                        #endregion
                    }
                }

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
                    #endregion
                }

                string logInfo = string.Format("Number of Payment Adjustments Created: {0}, Number of Award Adjustments Created: {0}", paymentAdjustmentCount, awardadjustmentCount);
                LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Payment Detail Records Created", logInfo);

                if (request.udo_paymentId != System.Guid.Empty)
                {
                    var parent = new Entity();
                    parent.Id = request.udo_paymentId;
                    parent.LogicalName = "udo_payment";
                    parent["udo_paymentdetailscomplete"] = true;
                    OrgServiceProxy.Update(parent);
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                aiLogger.LogException(ExecutionException, "999");
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, request.MessageId, "UDOgetPaymentDetailsProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Payment Detail Line Data " + ExecutionException.Message;
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
