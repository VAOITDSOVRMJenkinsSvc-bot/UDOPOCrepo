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
/// LOB Component for UDOcreateAwardAdjustment,createAwardAdjustment method, Processor.
/// </summary>
namespace UDO.LOB.Payments.Processors
{
    class UDOcreateAwardAdjustmentProcessor
    {
        private bool _debug { get; set; }

        private const string method = "UDOcreateAwardAdjustmentProcessor";

        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOcreateAwardAdjustmentRequest request)
        {
            //var request = message as createAwardAdjustmentRequest;
            UDOcreateAwardAdjustmentResponse response = new UDOcreateAwardAdjustmentResponse
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

            #region connect to CRM
            CrmServiceClient OrgServiceProxy = null; 

            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();

            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateAwardAdjustmentProcessor Processor, Progess:" + progressString, connectException);
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
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISrtrpmtdtlretrievePaymentDetailRequest Request/Response {requestResponse}", true);
                    }
                }
                progressString = "After VEIS EC Call";

                response.ExceptionMessage = retrievePaymentDetailResponse.ExceptionMessage;
                response.ExceptionOccurred = retrievePaymentDetailResponse.ExceptionOccurred;
                var awardadjustment = 0;
                var requestCollection = new OrganizationRequestCollection();

                if (retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo.VEISrtrpmtdtlawardAdjustmentsInfo.VEISrtrpmtdtlawardAdjustmentListInfo != null)
                {
                    var awardAdjustmentVO = retrievePaymentDetailResponse.VEISrtrpmtdtlpaymentDetailResponseVOInfo.VEISrtrpmtdtlawardAdjustmentsInfo.VEISrtrpmtdtlawardAdjustmentListInfo;
                    System.Collections.Generic.List<UDOcreateAwardAdjustmentMultipleResponse> UDOcreateAwardAdjustmentArray = new System.Collections.Generic.List<UDOcreateAwardAdjustmentMultipleResponse>();
                    foreach (var awardAdjustmentVOItem in awardAdjustmentVO)
                    {
                        var responseIds = new UDOcreateAwardAdjustmentMultipleResponse();
                        //instantiate the new Entity
                        Entity thisNewEntity = new Entity();
                        thisNewEntity.LogicalName = "udo_awardadjustment";
                        if (request.ownerId != System.Guid.Empty)
                        {
                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                        }
                        if (awardAdjustmentVOItem.mcs_allotmentType != string.Empty)
                        {
                            thisNewEntity["udo_alottmenttype"] = awardAdjustmentVOItem.mcs_allotmentType;
                        }
                        if (awardAdjustmentVOItem.mcs_allotmentRecipientName != string.Empty)
                        {
                            thisNewEntity["udo_alottmentrecipient"] = awardAdjustmentVOItem.mcs_allotmentRecipientName;
                        }
                        if (awardAdjustmentVOItem.mcs_alloteeRelationship != string.Empty)
                        {
                            thisNewEntity["udo_alloteerelationship"] = awardAdjustmentVOItem.mcs_alloteeRelationship;
                        }
                        if (awardAdjustmentVOItem.mcs_adjustmentType != string.Empty)
                        {
                            thisNewEntity["udo_adjustmenttype"] = awardAdjustmentVOItem.mcs_adjustmentType;
                        }
                        if (awardAdjustmentVOItem.mcs_adjustmentOperation != string.Empty)
                        {
                            thisNewEntity["udo_adjustmentoperation"] = awardAdjustmentVOItem.mcs_adjustmentOperation;
                        }
                        if (Convert.ToDateTime(awardAdjustmentVOItem.mcs_adjustmentEffectiveDate) != System.DateTime.MinValue) //Datatype changed with VEISMessages update
                        {
                            thisNewEntity["udo_adjustmenteffectivedate"] = Convert.ToDateTime(awardAdjustmentVOItem.mcs_adjustmentEffectiveDate);
                        }
                        thisNewEntity["udo_adjustmentamount"] = moneyStringFormat(awardAdjustmentVOItem.mcs_adjustmentAmount.ToString());
                        if (request.UDOcreateAwardAdjustmentRelatedEntitiesInfo != null)
                        {
                            foreach (var relatedItem in request.UDOcreateAwardAdjustmentRelatedEntitiesInfo)
                            {
                                thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                            }
                        }
                        CreateRequest createExamData = new CreateRequest
                        {
                            Target = thisNewEntity
                        };
                        requestCollection.Add(createExamData);
                        awardadjustment += 1;
                    }
                }
                if (awardadjustment > 0)
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
                                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, "UDOcreateAwardAdjustmentProcessor Processor, Progess:" + progressString + "FaultMessage" + responseItem.Fault.Message);
                                response.ExceptionMessage = "Failed to Map EC data to LOB";
                                response.ExceptionOccurred = true;
                            }
                        }
                    }
                    #endregion
                }

                string logInfo = string.Format("Number of AwardAdjustment Created: {0}", awardadjustment);
                LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, method, logInfo);


                //added to generated code
                if (request.udo_paymentId != System.Guid.Empty)
                {
                    var parent = new Entity();
                    parent.Id = request.udo_paymentId;
                    parent.LogicalName = "udo_payment";
                    parent["udo_awardadjustmentcomplete"] = true;
                    OrgServiceProxy.Update(parent);
                }
                return response;
            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateAwardAdjustmentProcessor Processor, Progess:" + progressString, connectException);
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