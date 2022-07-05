using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System;
using VIMT.PaymentHistoryService.Messages;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using VRM.Integration.UDO.Common;
using VRM.Integration.UDO.LegacyPayments.Messages;

namespace VRM.Integration.UDO.LegacyPayments.Processors
{
    class UDOcreateUDOLegacyPaymentsDetailsProcessor
    {
        private const string LOG_CONFIG_FIELD = "mcs_createUDOLegacyPaymentData";
        private TimeTracker timer { get; set; }
        private bool _debug { get; set; }
        private string LogBuffer { get; set; }
        private const string method = "UDOcreateUDOLegacyPaymentsDetailsProcessor";

        public IMessageBase Execute(UDOcreateLegacyPaymentsDetailsRequest request)
        {
            #region Start Timer

            if (request.LogTiming)
            {
                timer = new TimeTracker();
                timer.Restart();
            }

            #endregion
            
            UDOcreateLegacyPaymentsDetailsResponse response = new UDOcreateLegacyPaymentsDetailsResponse();
            var progressString = "Top of Processor";
            LogBuffer = string.Empty;
            _debug = request.Debug;

            if (request == null)
            {
                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccured = true;
                timer.Stop();
                timer = null;
                return response;
            }

            #region connect to CRM
            OrganizationServiceProxy OrgServiceProxy;
            try
            {
                var CommonFunctions = new CRMConnect();

                OrgServiceProxy = CommonFunctions.ConnectToCrm(request.OrganizationName);

            }
            catch (Exception connectException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreateUDOLegacyPaymentsDetailsProcessor Processor, Connection Error", connectException.Message);                
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                timer.Stop();
                timer = null;
                return response;
            }
            progressString = "After Connection";
            #endregion

            try
            {
                // prefix = payHistSSN_findPayHistoryBySSNRequest();
                if (request.LogTiming) timer.MarkStart("findPayHistoryBySSNResponse");

                if (string.IsNullOrEmpty(request.filenumber))
                {
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method, "filenumber is null or empty");
                }
                if (string.IsNullOrEmpty(request.ssn))
                {
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, method, "ssn is null or empty");
                }
                var findPayHistoryBySSNRequest = new VIMTpayHistSSN_findPayHistoryBySSNRequest
                {
                    LogTiming = request.LogTiming,
                    LogSoap = request.LogSoap,
                    Debug = request.Debug,
                    RelatedParentEntityName = request.RelatedParentEntityName,
                    RelatedParentFieldName = request.RelatedParentFieldName,
                    RelatedParentId = request.RelatedParentId,
                    UserId = request.UserId,
                    OrganizationName = request.OrganizationName,
                    LegacyServiceHeaderInfo = new VIMT.PaymentHistoryService.Messages.HeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    },
                    mcs_ssn = String.IsNullOrEmpty(request.filenumber) ? request.ssn : request.filenumber
                };

                var findPayHistoryBySSNResponse = findPayHistoryBySSNRequest.SendReceive<VIMTpayHistSSN_findPayHistoryBySSNResponse>(MessageProcessType.Local);
                if (request.LogTiming) timer.MarkStop("findPayHistoryBySSNResponse");
                progressString = "After VIMT EC Call";

                response.ExceptionMessage = findPayHistoryBySSNResponse.ExceptionMessage;
                response.ExceptionOccured = findPayHistoryBySSNResponse.ExceptionOccured;

                var requestCollection = new OrganizationRequestCollection();
                var paymentDataCount = 0;
                var returnedPaymentDataCount = 0;
                #region process data
                if (findPayHistoryBySSNResponse != null)
                {
                    if (findPayHistoryBySSNResponse.VIMTpayHistSSN_Info != null)
                    {
                        if (findPayHistoryBySSNResponse.VIMTpayHistSSN_Info.VIMTpayHistSSN_paymentRecordInfo != null)
                        {
                            var legacyPayments = findPayHistoryBySSNResponse.VIMTpayHistSSN_Info.VIMTpayHistSSN_paymentRecordInfo;

                            if (findPayHistoryBySSNResponse.VIMTpayHistSSN_Info.VIMTpayHistSSN_paymentRecordInfo.VIMTpayHistSSN_paymentsInfo != null)
                            {
                                var payments = legacyPayments.VIMTpayHistSSN_paymentsInfo;

                                #region payment Data
                                if (findPayHistoryBySSNResponse.VIMTpayHistSSN_Info.VIMTpayHistSSN_paymentRecordInfo.VIMTpayHistSSN_paymentAddressInfo != null)
                                {
                                    var addresses = legacyPayments.VIMTpayHistSSN_paymentAddressInfo;
                                    var dataExists = false;
                                    foreach (var paymentsItem in payments)
                                    {
                                        //instantiate the new Entity
                                        Entity thisNewEntity = new Entity();
                                        thisNewEntity.LogicalName = "udo_paymentdata";
                                        if (request.ownerId != System.Guid.Empty)
                                        {
                                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                        }
                                        if (!string.IsNullOrEmpty(paymentsItem.mcs_payCheckReturnFiche))
                                        {
                                            thisNewEntity["udo_paidby"] = paymentsItem.mcs_payCheckReturnFiche;
                                            dataExists = true;
                                        }
                                        if (!string.IsNullOrEmpty(paymentsItem.mcs_payCheckDt))
                                        {
                                            thisNewEntity["udo_paydate"] = paymentsItem.mcs_payCheckDt;
                                            DateTime udo_paydate_dt;
                                            if (DateTime.TryParse(paymentsItem.mcs_payCheckDt.Trim().ToDateStringFormat(), out udo_paydate_dt))
                                            {
                                                thisNewEntity["udo_paydate_dt"] = udo_paydate_dt.ToCRMDateTime();
                                                dataExists = true;
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(paymentsItem.mcs_payCheckType))
                                        {
                                            thisNewEntity["udo_type"] = paymentsItem.mcs_payCheckType;
                                            dataExists = true;
                                        }
                                        if (!string.IsNullOrEmpty(paymentsItem.mcs_payCheckID))
                                        {
                                            thisNewEntity["udo_checkid"] = paymentsItem.mcs_payCheckID;
                                            dataExists = true;
                                        }
                                        if (!string.IsNullOrEmpty(paymentsItem.mcs_payCheckAmount))
                                        {
                                            thisNewEntity["udo_amount"] = paymentsItem.mcs_payCheckAmount;
                                            dataExists = true;
                                        }
                                        foreach (var paymentAddressItem in addresses)
                                        {
                                            if (paymentAddressItem.mcs_addressID == paymentsItem.mcs_payCheckID)
                                            {
                                                if (!string.IsNullOrEmpty(paymentAddressItem.mcs_addressPayMethod))
                                                {
                                                    thisNewEntity["udo_paymethod"] = paymentAddressItem.mcs_addressPayMethod;
                                                    dataExists = true;
                                                }
                                                if (!string.IsNullOrEmpty(paymentAddressItem.mcs_addressRO))
                                                {
                                                    thisNewEntity["udo_ro"] = paymentAddressItem.mcs_addressRO;
                                                    dataExists = true;
                                                }
                                                if (!string.IsNullOrEmpty(paymentAddressItem.mcs_addressRoutingNum))
                                                {
                                                    thisNewEntity["udo_routing"] = paymentAddressItem.mcs_addressRoutingNum;
                                                    dataExists = true;
                                                }
                                                if (!string.IsNullOrEmpty(paymentAddressItem.mcs_addressZipCode))
                                                {
                                                    thisNewEntity["udo_routingcode"] = paymentAddressItem.mcs_addressZipCode;
                                                    dataExists = true;
                                                }
                                                if (!string.IsNullOrEmpty(paymentAddressItem.mcs_addressLine4))
                                                {
                                                    thisNewEntity["udo_address4"] = paymentAddressItem.mcs_addressLine4;
                                                    dataExists = true;
                                                }
                                                if (!string.IsNullOrEmpty(paymentAddressItem.mcs_addressLine3))
                                                {
                                                    thisNewEntity["udo_address3"] = paymentAddressItem.mcs_addressLine3;
                                                    dataExists = true;
                                                }
                                                if (!string.IsNullOrEmpty(paymentAddressItem.mcs_addressLine2))
                                                {
                                                    thisNewEntity["udo_address2"] = paymentAddressItem.mcs_addressLine2;
                                                    dataExists = true;
                                                }
                                                if (!string.IsNullOrEmpty(paymentAddressItem.mcs_addressLine1))
                                                {
                                                    thisNewEntity["udo_address1"] = paymentAddressItem.mcs_addressLine1;
                                                    dataExists = true;
                                                }
                                                if (!string.IsNullOrEmpty(paymentAddressItem.mcs_addressAccntType))
                                                {
                                                    thisNewEntity["udo_accounttype"] = paymentAddressItem.mcs_addressAccntType;
                                                    dataExists = true;
                                                }
                                                if (!string.IsNullOrEmpty(paymentAddressItem.mcs_addressAccntNum))
                                                {
                                                    thisNewEntity["udo_account"] = paymentAddressItem.mcs_addressAccntNum;
                                                    dataExists = true;
                                                }
                                            }
                                        }
                                        if (request.UDOcreateUDOLegacyPaymentsDetailsRelatedEntitiesInfo != null)
                                        {
                                            foreach (var relatedItem in request.UDOcreateUDOLegacyPaymentsDetailsRelatedEntitiesInfo)
                                            {
                                                thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                            }
                                        }
                                        if (dataExists)
                                        {
                                            CreateRequest createPaymentData = new CreateRequest
                                            {
                                                Target = thisNewEntity
                                            };
                                            requestCollection.Add(createPaymentData);
                                            paymentDataCount += 1;
                                        }
                                    }
                                }
                                #endregion
                            }

                            
                            if (findPayHistoryBySSNResponse.VIMTpayHistSSN_Info.VIMTpayHistSSN_paymentRecordInfo.VIMTpayHistSSN_returnPaymentsInfo != null)
                            {
                                #region returnedpayment data
                                var returnPayments = findPayHistoryBySSNResponse.VIMTpayHistSSN_Info.VIMTpayHistSSN_paymentRecordInfo.VIMTpayHistSSN_returnPaymentsInfo;
                                var retDataExists = false;
                                foreach (var returnPaymentsItem in returnPayments)
                                {
                                    //instantiate the new Entity
                                    Entity thisNewEntity = new Entity();
                                    thisNewEntity.LogicalName = "udo_returnedpaymentdata";
                                    if (request.ownerId != System.Guid.Empty)
                                    {
                                        thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                                    }
                                    if (!string.IsNullOrEmpty(returnPaymentsItem.mcs_returnedCheckType))
                                    {
                                        thisNewEntity["udo_type"] = returnPaymentsItem.mcs_returnedCheckType;
                                        retDataExists = true;
                                    }
                                    if (!string.IsNullOrEmpty(returnPaymentsItem.mcs_returnedCheckRO))
                                    {
                                        thisNewEntity["udo_ro"] = returnPaymentsItem.mcs_returnedCheckRO;
                                        retDataExists = true;
                                    }
                                    if (!string.IsNullOrEmpty(returnPaymentsItem.mcs_returnedCheckReason))
                                    {
                                        thisNewEntity["udo_reason"] = returnPaymentsItem.mcs_returnedCheckReason;
                                        retDataExists = true;
                                    }
                                    if (!string.IsNullOrEmpty(returnPaymentsItem.mcs_returnedCheckReturnFiche))
                                    {
                                        thisNewEntity["udo_paidby"] = returnPaymentsItem.mcs_returnedCheckReturnFiche;
                                        retDataExists = true;
                                    }
                                    if (!string.IsNullOrEmpty(returnPaymentsItem.mcs_returnedCheckIssueDt))
                                    {
                                        thisNewEntity["udo_issueddate"] = returnPaymentsItem.mcs_returnedCheckIssueDt;
                                        DateTime udo_issueddate_dt;
                                        if (DateTime.TryParse(returnPaymentsItem.mcs_returnedCheckIssueDt.Trim().ToDateStringFormat(), out udo_issueddate_dt))
                                        {
                                            thisNewEntity["udo_issueddate_dt"] = udo_issueddate_dt.ToCRMDateTime();
                                            retDataExists = true;
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(returnPaymentsItem.mcs_returnedCheckNum))
                                    {
                                        thisNewEntity["udo_checknum"] = returnPaymentsItem.mcs_returnedCheckNum;
                                        retDataExists = true;
                                    }
                                    if (!string.IsNullOrEmpty(returnPaymentsItem.mcs_returnedCheckCancelDt))
                                    {
                                        thisNewEntity["udo_canceldate"] = returnPaymentsItem.mcs_returnedCheckCancelDt;
                                        retDataExists = true;
                                        DateTime udo_canceldate_dt;
                                        if (DateTime.TryParse(returnPaymentsItem.mcs_returnedCheckCancelDt.Trim().ToDateStringFormat(), out udo_canceldate_dt))
                                        {
                                            thisNewEntity["udo_canceldate_dt"] = udo_canceldate_dt.ToCRMDateTime();
                                            retDataExists = true;
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(returnPaymentsItem.mcs_returnedCheckAmount))
                                    {
                                        thisNewEntity["udo_amount"] = returnPaymentsItem.mcs_returnedCheckAmount;
                                        retDataExists = true;
                                    }
                                    if (request.UDOcreateUDOLegacyPaymentsDetailsRelatedEntitiesInfo != null)
                                    {
                                        foreach (var relatedItem in request.UDOcreateUDOLegacyPaymentsDetailsRelatedEntitiesInfo)
                                        {
                                            thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                                        }
                                    }
                                    if (retDataExists)
                                    {
                                        CreateRequest createReturnPayments = new CreateRequest
                                        {
                                            Target = thisNewEntity
                                        };
                                        requestCollection.Add(createReturnPayments);
                                        returnedPaymentDataCount += 1;
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                }
                if (request.LogTiming) timer.MarkStop("Processing Data");
                #endregion

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

                    string logInfo = string.Format("Number of Payment Data Created: {0}, Number of Returned Payment Data Created: {1}", paymentDataCount, returnedPaymentDataCount);
                    LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, method, logInfo);
                    #endregion
                }
                else
                {
                    LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, method, "No Payment Data Found/Created");
                }

                if (request.udo_legacypaymenthistoryId != System.Guid.Empty)
                {
                    var parent = new Entity();
                    parent.Id = request.udo_legacypaymenthistoryId;
                    parent.LogicalName = "udo_legacypaymenthistory";
                    parent["udo_legacypaymentdatacomplete"] = true;
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming));
                }

                #region Stop Timer

                if (request.LogTiming)
                {
                    var elapsedMilliseconds = timer.LogDurations(request.OrganizationName, request.Debug, request.UserId, 
                        string.Format("{0}, Progress: {1}", method, progressString), true);
                    LogHelper.LogTiming(request.OrganizationName, request.Debug, request.UserId,
                        request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method,
                        method,
                        Convert.ToDecimal(elapsedMilliseconds));
                }
                timer = null;
                #endregion
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, "UDOcreateAwardLinesProcessor Processor, Progess:" + progressString, ExecutionException);
                if (request.LogTiming)
                {
                    timer.LogDurations(request.OrganizationName, request.Debug, request.UserId, method, true);
                    timer = null;
                }
                response.ExceptionMessage = "Failed to process Lagacy payment details data";
                response.ExceptionOccured = true;
                return response;
            }
        }
    }
}