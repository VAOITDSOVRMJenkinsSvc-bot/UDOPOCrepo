
using VRM.Integration.UDO.Common;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Crm.Sdk.Messages;
using System;
using System.Collections.Generic;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Payments.Messages;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using Logger = VRM.Integration.Servicebus.Core.Logger;
using VIMT.PaymentInformationService.Messages;

namespace VRM.Integration.UDO.Payments.Processors
{
    class UDOgetPaymentDetailsProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOgetPaymentDetailsProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOgetPaymentDetailsRequest request)
        {
            UDOgetPaymentDetailsResponse response = new UDOgetPaymentDetailsResponse();
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
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOgetPaymentDetailsProcessor Processor, Connection Error", connectException.Message);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

            try
            {
                var retrievePaymentDetailRequest = new VIMTrtrpmtdtlretrievePaymentDetailRequest
                {
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
                    LegacyServiceHeaderInfo = new VIMT.PaymentInformationService.Messages.HeaderInfo
                   {
                       ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                       ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                       LoginName = request.LegacyServiceHeaderInfo.LoginName,
                       StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                   },
                };
                //TODO(NP): Update the VIMT call to VEIS
                var retrievePaymentDetailResponse = retrievePaymentDetailRequest.SendReceive<VIMTrtrpmtdtlretrievePaymentDetailResponse>(MessageProcessType.Local);
                progressString = "After VIMT EC Call";


                response.ExceptionMessage = retrievePaymentDetailResponse.ExceptionMessage;
                response.ExceptionOccured = retrievePaymentDetailResponse.ExceptionOccured;
                response.UDOgetPaymentDetailsInfo = new UDOgetPaymentDetails();
                var paymentAdjustmentCount = 0;
                var awardadjustmentCount = 0;
                var requestCollection = new OrganizationRequestCollection();

                if (retrievePaymentDetailResponse != null)
                {
                    if (retrievePaymentDetailResponse.VIMTrtrpmtdtlPaymentDetailResponseInfo != null)
                    {
                        #region info sent back in response
                        #region PaymentDetailsAwardAdjustment

                        Entity currentEntity = new Entity("udo_payment");
                        currentEntity.Id = request.udo_paymentId;

                        if (retrievePaymentDetailResponse.VIMTrtrpmtdtlPaymentDetailResponseInfo.VIMTrtrpmtdtlawardAdjustmentsInfo != null)
                        {
                            progressString = "Beginning Mapping Payment Details: Award Adjustment";

                            var awardAdjustmentResponse = retrievePaymentDetailResponse.VIMTrtrpmtdtlPaymentDetailResponseInfo.VIMTrtrpmtdtlawardAdjustmentsInfo;
                            progressString = "Map: Gross Award Amount";


                            //response.UDOgetPaymentDetailsInfo.udo_GrossAwardAmount = moneyStringFormat(awardAdjustmentResponse.mcs_grossAwardAmount.ToString());
                            currentEntity["udo_grossawardamount"] = moneyStringFormat(awardAdjustmentResponse.mcs_grossAwardAmount.ToString());
                            //response.UDOgetPaymentDetailsInfo.udo_NetAwardAmount =moneyStringFormat(awardAdjustmentResponse.mcs_netAwardAmount.ToString());
                            currentEntity["udo_netawardamount"] = moneyStringFormat(awardAdjustmentResponse.mcs_netAwardAmount.ToString());
                            if (awardAdjustmentResponse.mcs_awardEffectiveDate != null)
                            {
                                // response.UDOgetPaymentDetailsInfo.udo_AwardEffectiveDate = awardAdjustmentResponse.mcs_awardEffectiveDate;
                                currentEntity["udo_awardeffectivedate"] = awardAdjustmentResponse.mcs_awardEffectiveDate;
                            }
                            progressString = "Completed Mapping Payment Details: Award Adjustment";
                        }
                        #endregion

                        #region PaymentDetailsPaymentAdjustment
                        if (retrievePaymentDetailResponse.VIMTrtrpmtdtlPaymentDetailResponseInfo.VIMTrtrpmtdtlpaymentAdjustmentsInfo != null)
                        {
                            //Map Details: Payment Adjustment Information
                            progressString = "Beginning Mapping Payment Details: Payment Adjustment";

                            var paymentAdjustmentResponse = retrievePaymentDetailResponse.VIMTrtrpmtdtlPaymentDetailResponseInfo.VIMTrtrpmtdtlpaymentAdjustmentsInfo;

                            //response.UDOgetPaymentDetailsInfo.udo_GrossPaymentAmount = moneyStringFormat(paymentAdjustmentResponse.mcs_grossPaymentAmount.ToString());
                            currentEntity["udo_grosspaymentamount"] = moneyStringFormat(paymentAdjustmentResponse.mcs_grossPaymentAmount.ToString());
                            //response.UDOgetPaymentDetailsInfo.udo_NetPaymentAmount = moneyStringFormat(paymentAdjustmentResponse.mcs_netPaymentAmount.ToString());
                            currentEntity["udo_netpaymentamount"] = moneyStringFormat(paymentAdjustmentResponse.mcs_netPaymentAmount.ToString());
                            if (request.payeecodeid != Guid.Empty)
                            {
                                Entity payeeCode = new Entity();
                                payeeCode.LogicalName = "udo_payeecode";
                                payeeCode.Id = request.payeecodeid;
                                payeeCode["udo_grossamount"] = moneyStringFormat(paymentAdjustmentResponse.mcs_grossPaymentAmount.ToString());
                                payeeCode["udo_netamount"] = moneyStringFormat(paymentAdjustmentResponse.mcs_netPaymentAmount.ToString());
                                OrgServiceProxy.Update(TruncateHelper.TruncateFields(payeeCode, request.OrganizationName, request.UserId, request.LogTiming));
                                //   LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOgetPaymentDetailsProcessor", "payeeCode Updated");
                            }
                            if (request.udo_personId != Guid.Empty)
                            {
                                Entity person = new Entity();
                                person.LogicalName = "udo_person";
                                person.Id = request.udo_personId;
                                person["udo_grossamount"] = moneyStringFormat(paymentAdjustmentResponse.mcs_grossPaymentAmount.ToString());
                                person["udo_netamount"] = moneyStringFormat(paymentAdjustmentResponse.mcs_netPaymentAmount.ToString());
                                if (retrievePaymentDetailResponse.VIMTrtrpmtdtlPaymentDetailResponseInfo.VIMTrtrpmtdtlawardAdjustmentsInfo != null)
                                {
                                    if (retrievePaymentDetailResponse.VIMTrtrpmtdtlPaymentDetailResponseInfo.VIMTrtrpmtdtlawardAdjustmentsInfo.mcs_awardEffectiveDate != null)
                                    {
                                        person["udo_awardeffectivedate"] = retrievePaymentDetailResponse.VIMTrtrpmtdtlPaymentDetailResponseInfo.VIMTrtrpmtdtlawardAdjustmentsInfo.mcs_awardEffectiveDate;
                                    }
                                }
                                person["udo_paymentcomplete"] = true;
                                OrgServiceProxy.Update(TruncateHelper.TruncateFields(person, request.OrganizationName, request.UserId, request.LogTiming));
                                //   LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOgetPaymentDetailsProcessor", "person Updated");
                            }
                            progressString = "Completed Mapping Payment Details: Payment Adjustment";
                        }
                        #endregion

                        #region PaymentDetailsAwardReason
                        if (retrievePaymentDetailResponse.VIMTrtrpmtdtlPaymentDetailResponseInfo.VIMTrtrpmtdtlawardAdjustmentsInfo != null)
                        {

                            if (retrievePaymentDetailResponse.VIMTrtrpmtdtlPaymentDetailResponseInfo.VIMTrtrpmtdtlawardAdjustmentsInfo.VIMTrtrpmtdtlawardReasonListInfo != null)
                            {
                                //Map Details: Award Reason List
                                progressString = "Beginning Mapping Payment Details: Award Reasons";

                                var AwardReasons = retrievePaymentDetailResponse.VIMTrtrpmtdtlPaymentDetailResponseInfo.VIMTrtrpmtdtlawardAdjustmentsInfo.VIMTrtrpmtdtlawardReasonListInfo;

                                foreach (var ReasonInfo in AwardReasons)
                                {
                                    if (ReasonInfo.mcs_awardReasonText != null)
                                    {
                                        //If Reason is Empty, Do not Concat. a Comma
                                        if (response.UDOgetPaymentDetailsInfo.udo_Reason == null)
                                        {
                                            response.UDOgetPaymentDetailsInfo.udo_Reason = ReasonInfo.mcs_awardReasonText;
                                            currentEntity["udo_reason"] = ReasonInfo.mcs_awardReasonText;
                                            //    currentEntity["udo_ro"] = paymentDetails.udo_RO;
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
                        if (retrievePaymentDetailResponse.VIMTrtrpmtdtlPaymentDetailResponseInfo.VIMTrtrpmtdtlpaymentAdjustmentsInfo != null)
                        {
                            if (retrievePaymentDetailResponse.VIMTrtrpmtdtlPaymentDetailResponseInfo.VIMTrtrpmtdtlpaymentAdjustmentsInfo.VIMTrtrpmtdtlpaymentAdjustmentListInfo != null)
                            {
                                #region
                                var paymentAdjustmentList = retrievePaymentDetailResponse.VIMTrtrpmtdtlPaymentDetailResponseInfo.VIMTrtrpmtdtlpaymentAdjustmentsInfo.VIMTrtrpmtdtlpaymentAdjustmentListInfo;
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
                        if (retrievePaymentDetailResponse.VIMTrtrpmtdtlPaymentDetailResponseInfo.VIMTrtrpmtdtlawardAdjustmentsInfo != null)
                        {
                            if (retrievePaymentDetailResponse.VIMTrtrpmtdtlPaymentDetailResponseInfo.VIMTrtrpmtdtlawardAdjustmentsInfo.VIMTrtrpmtdtlawardAdjustmentListInfo != null)
                            {
                                var awardAdjustmentList = retrievePaymentDetailResponse.VIMTrtrpmtdtlPaymentDetailResponseInfo.VIMTrtrpmtdtlawardAdjustmentsInfo.VIMTrtrpmtdtlawardAdjustmentListInfo;
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
                                        thisNewEntity["udo_adjustmenteffectivedate"] = awardAdjustmentItem.mcs_adjustmentEffectiveDate;
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
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming));
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOgetPaymentDetailsProcessor Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Payment Detail Line Data";
                response.ExceptionOccured = true;
                return response;
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
