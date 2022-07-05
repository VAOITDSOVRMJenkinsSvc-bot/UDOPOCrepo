using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Linq;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.Payments.Messages;
using VEIS.Core.Messages;
using VEIS.Messages.DdeftWebService;
using VEIS.Messages.PaymentInformationService;

namespace UDO.LOB.Payments.Processors
{
    class UDOcreatePaymentsProcessor
    {
        private bool _debug { get; set; }

        private string LogBuffer { get; set; }

        private const string method = "UDOcreatePaymentsProcessor";

        public IMessageBase Execute(UDOcreatePaymentsRequest request)
        {
            UDOcreatePaymentsResponse response = new UDOcreatePaymentsResponse
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

            TraceLogger aiLogger = new TraceLogger($">> Entered {this.GetType().FullName}.createPayments", request);

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
                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, connectException);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccurred = true;
                return response;
            }
            #endregion

            progressString = "After Connection";
            var paymentsCount = 0;
            try
            {
                response.DiagnosticsContext = request.DiagnosticsContext;

                var lastpaymentdate = System.DateTime.MinValue;
                var lastpaymentamount = "";
                var nextpaymentdate = System.DateTime.MinValue;
                var nextpaymentamount = "";
                var hasNextPayment = false;

                var retrievePaymentSummaryWithBDNRequest = new VEISrtvpmtsumbdnretrievePaymentSummaryWithBDNRequest();
                retrievePaymentSummaryWithBDNRequest.MessageId = request.MessageId;
                retrievePaymentSummaryWithBDNRequest.LogTiming = request.LogTiming;
                retrievePaymentSummaryWithBDNRequest.LogSoap = request.LogSoap;
                retrievePaymentSummaryWithBDNRequest.Debug = request.Debug;
                retrievePaymentSummaryWithBDNRequest.RelatedParentEntityName = request.RelatedParentEntityName;
                retrievePaymentSummaryWithBDNRequest.RelatedParentFieldName = request.RelatedParentFieldName;
                retrievePaymentSummaryWithBDNRequest.RelatedParentId = request.RelatedParentId;
                retrievePaymentSummaryWithBDNRequest.UserId = request.UserId;
                retrievePaymentSummaryWithBDNRequest.OrganizationName = request.OrganizationName;

                retrievePaymentSummaryWithBDNRequest.mcs_participantid = request.ParticipantId;
                retrievePaymentSummaryWithBDNRequest.mcs_filenumber = request.FileNumber;
                retrievePaymentSummaryWithBDNRequest.mcs_payeecode = request.PayeeCode;
                retrievePaymentSummaryWithBDNRequest.LegacyServiceHeaderInfo = new LegacyHeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
                // REM: Invoke VEIS Endpoint
                var retrievePaymentSummaryWithBDNResponse = WebApiUtility.SendReceive<VEISrtvpmtsumbdnretrievePaymentSummaryWithBDNResponse>(retrievePaymentSummaryWithBDNRequest, WebApiType.VEIS);
                if (request.LogSoap || retrievePaymentSummaryWithBDNResponse.ExceptionOccurred)
                {
                    if (retrievePaymentSummaryWithBDNResponse.SerializedSOAPRequest != null || retrievePaymentSummaryWithBDNResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = retrievePaymentSummaryWithBDNResponse.SerializedSOAPRequest + retrievePaymentSummaryWithBDNResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISrtvpmtsumbdnretrievePaymentSummaryWithBDNRequest Request/Response {requestResponse}", true);
                    }
                }
                progressString = "After VEIS EC Call";

                response.ExceptionMessage = retrievePaymentSummaryWithBDNResponse.ExceptionMessage;
                response.ExceptionOccurred = retrievePaymentSummaryWithBDNResponse.ExceptionOccurred;

                var snapshotCount = 0;
                Int64 topPaymentIdentifier = 0;
                Int64 topFbtId = 0;
                var TopPayment = false;
                var oktoloadPaymentDetails = false;
                var topPaymentID = Guid.Empty;
                var comerica = string.Empty;
                var requestCollection = new OrganizationRequestCollection();

                #region process payments
                if (retrievePaymentSummaryWithBDNResponse != null &&
                    retrievePaymentSummaryWithBDNResponse.VEISrtvpmtsumbdnrpaymentSummaryResponseVOInfo != null &&
                    retrievePaymentSummaryWithBDNResponse.VEISrtvpmtsumbdnrpaymentSummaryResponseVOInfo.VEISrtvpmtsumbdnrpaymentsInfo != null)
                {
                    System.Collections.Generic.List<UDOcreatePaymentsMultipleResponse> UDOcreatePaymentsArray = new System.Collections.Generic.List<UDOcreatePaymentsMultipleResponse>();
                    var paymentsInfo = retrievePaymentSummaryWithBDNResponse.VEISrtvpmtsumbdnrpaymentSummaryResponseVOInfo.VEISrtvpmtsumbdnrpaymentsInfo;

                    // Define Flags
                    bool isScheduled,
                        isCompensation,
                        hasCompensation = paymentsInfo.Any(p => IsCompensation(p)),
                        hasScheduled = paymentsInfo.Any(p => Convert.ToDateTime(p.mcs_scheduledDate) != System.DateTime.MinValue); //Datatype changed with VEISMessages update

                    var paymentsSorted = paymentsInfo.OrderByDescending(h => h.mcs_paymentDate);

                    foreach (var hPaymentVOItem in paymentsSorted)
                    {
                        #region foreach payment record

                        isScheduled = hasScheduled && (Convert.ToDateTime(hPaymentVOItem.mcs_scheduledDate) != System.DateTime.MinValue); //Datatype changed with VEISMessages update
                        isCompensation = hasCompensation && IsCompensation(hPaymentVOItem);

                        var responseIds = new UDOcreatePaymentsMultipleResponse();
                        var peopleEntity = new Entity
                        {
                            LogicalName = "udo_person",
                            Id = request.udo_personId
                        };

                        //instantiate the new Entity
                        var thisNewEntity = new Entity("udo_payment");

                        #region udo_name
                        thisNewEntity["udo_name"] = "Payment Summary";
                        #endregion

                        #region udo_index
                        if (isScheduled)
                        {
                            thisNewEntity["udo_index"] = paymentsCount + 1;
                        }
                        else
                        {
                            thisNewEntity["udo_index"] = paymentsCount;
                        }
                        #endregion

                        #region owner
                        if (request.ownerId != System.Guid.Empty)
                        {
                            thisNewEntity["ownerid"] = new EntityReference(request.ownerType, request.ownerId);
                        }
                        #endregion

                        #region record idnetifiers
                        if (hPaymentVOItem.VEISrtvpmtsumbdnrpaymentRecordIdentifierInfo != null)
                        {
                            if (hPaymentVOItem.VEISrtvpmtsumbdnrpaymentRecordIdentifierInfo.mcs_paymentID > 0)
                            {

                                thisNewEntity["udo_paymentidentifier"] = hPaymentVOItem.VEISrtvpmtsumbdnrpaymentRecordIdentifierInfo.mcs_paymentID.ToString();
                                responseIds.paymentId = hPaymentVOItem.VEISrtvpmtsumbdnrpaymentRecordIdentifierInfo.mcs_paymentID;
                                oktoloadPaymentDetails = true;
                            }

                            if (hPaymentVOItem.VEISrtvpmtsumbdnrpaymentRecordIdentifierInfo.mcs_transactionID > 0)
                            {
                                thisNewEntity["udo_transactionid"] = hPaymentVOItem.VEISrtvpmtsumbdnrpaymentRecordIdentifierInfo.mcs_transactionID.ToString();
                                topFbtId = hPaymentVOItem.VEISrtvpmtsumbdnrpaymentRecordIdentifierInfo.mcs_transactionID;
                                responseIds.ftbid = hPaymentVOItem.VEISrtvpmtsumbdnrpaymentRecordIdentifierInfo.mcs_transactionID;
                                oktoloadPaymentDetails = true;
                            }
                        }
                        #endregion

                        #region udo_authorizedddate
                        if (Convert.ToDateTime(hPaymentVOItem.mcs_authorizedDate) != System.DateTime.MinValue) //Datatype changed with VEISMessages update
                        {
                            thisNewEntity["udo_authorizeddate"] = Convert.ToDateTime(hPaymentVOItem.mcs_authorizedDate);
                        }
                        #endregion

                        #region last payment and next payment

                        if (Convert.ToDateTime(hPaymentVOItem.mcs_paymentDate) != System.DateTime.MinValue) //Datatype changed with VEISMessages update
                        {
                            #region hasPaymentDate
                            thisNewEntity["udo_paydate"] = Convert.ToDateTime(hPaymentVOItem.mcs_paymentDate);
                            snapshotCount += 1;

                            if (isCompensation && Convert.ToDateTime(hPaymentVOItem.mcs_paymentDate) > lastpaymentdate)
                            {
                                lastpaymentamount = moneyStringFormat(hPaymentVOItem.mcs_paymentAmount.ToString());
                                lastpaymentdate = Convert.ToDateTime(hPaymentVOItem.mcs_paymentDate);
                                TopPayment = true;
                                if (hPaymentVOItem.VEISrtvpmtsumbdnrpaymentRecordIdentifierInfo != null)
                                {
                                    if (hPaymentVOItem.VEISrtvpmtsumbdnrpaymentRecordIdentifierInfo.mcs_paymentID > 0)
                                    {
                                        topPaymentIdentifier = hPaymentVOItem.VEISrtvpmtsumbdnrpaymentRecordIdentifierInfo.mcs_paymentID;
                                        oktoloadPaymentDetails = true;
                                    }

                                    if (hPaymentVOItem.VEISrtvpmtsumbdnrpaymentRecordIdentifierInfo.mcs_transactionID > 0)
                                    {
                                        topFbtId = hPaymentVOItem.VEISrtvpmtsumbdnrpaymentRecordIdentifierInfo.mcs_transactionID;
                                        oktoloadPaymentDetails = true;
                                    }
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            #region noPaymentData
                            if (isCompensation && isScheduled)
                            {
                                snapshotCount += 1;
                                nextpaymentdate = Convert.ToDateTime(hPaymentVOItem.mcs_scheduledDate);
                                nextpaymentamount = moneyStringFormat(hPaymentVOItem.mcs_paymentAmount.ToString());
                                hasNextPayment = true;
                            }
                            thisNewEntity["udo_index"] = 0;  // forced to 0.
                            #endregion
                        }
                        #endregion

                        #region udo_paymenttype
                        if (hPaymentVOItem.mcs_paymentType != string.Empty)
                        {
                            thisNewEntity["udo_paymenttype"] = hPaymentVOItem.mcs_paymentType;
                        }
                        #endregion

                        #region udo_programtype
                        if (hPaymentVOItem.mcs_programType != string.Empty)
                        {
                            thisNewEntity["udo_programtype"] = hPaymentVOItem.mcs_programType;
                        }
                        #endregion

                        #region udo_recipient
                        if (hPaymentVOItem.mcs_recipientName != string.Empty)
                        {
                            thisNewEntity["udo_recipient"] = hPaymentVOItem.mcs_recipientName;
                        }
                        #endregion

                        #region udo_programtype (mcs_bdnRecordType)  udo_recipient (mcs_beneficiaryName) override
                        if (!String.IsNullOrEmpty(hPaymentVOItem.mcs_bdnRecordType))
                        {
                            thisNewEntity["udo_programtype"] = hPaymentVOItem.mcs_bdnRecordType;
                            thisNewEntity["udo_recipient"] = hPaymentVOItem.mcs_beneficiaryName;
                        }
                        #endregion

                        #region udo_scheduleddate
                        if (isScheduled)
                        {
                            thisNewEntity["udo_scheduleddate"] = Convert.ToDateTime(hPaymentVOItem.mcs_scheduledDate);
                        }
                        #endregion

                        #region udo_payeetype
                        if (!String.IsNullOrEmpty(hPaymentVOItem.mcs_payeeType))
                        {
                            thisNewEntity["udo_payeetype"] = hPaymentVOItem.mcs_payeeType;
                        }
                        #endregion

                        #region udo_amount
                        thisNewEntity["udo_amount"] = moneyStringFormat(hPaymentVOItem.mcs_paymentAmount.ToString());
                        #endregion

                        #region addressField, udo_routingnumber, udo_accountnumer, udo_accounttype, udo_bankname
                        var addressField = "";
                        var accountNumber = string.Empty;
                        var routingNumber = string.Empty;

                        if (hPaymentVOItem.VEISrtvpmtsumbdnraddressEFTInfo != null)
                        {
                            var addressEFTVO = hPaymentVOItem.VEISrtvpmtsumbdnraddressEFTInfo;

                            #region udo_routingnumber and udo_accountnumber
                            if (!String.IsNullOrEmpty(addressEFTVO.mcs_accountNumber))
                            {
                                accountNumber = addressEFTVO.mcs_accountNumber;

                                #region udo_routingnumber and mask comerica account number and routing number
                                if (!String.IsNullOrEmpty(addressEFTVO.mcs_routingNumber))
                                {
                                    #region findComerica
                                    if (comerica == string.Empty)
                                    {
                                        //only do this once
                                        var findComericaRequest = new VEISroutingTransitionNumberfindComericaRoutngTrnsitNbrRequest
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
                                            }
                                        };

                                        // REM: Invoke VEIS Endpoint
                                        var findComericaResponse = WebApiUtility.SendReceive<VEISroutingTransitionNumberfindComericaRoutngTrnsitNbrResponse>(findComericaRequest, WebApiType.VEIS);
                                        if (request.LogSoap || findComericaResponse.ExceptionOccurred)
                                        {
                                            if (findComericaResponse.SerializedSOAPRequest != null || findComericaResponse.SerializedSOAPResponse != null)
                                            {
                                                var requestResponse = findComericaResponse.SerializedSOAPRequest + findComericaResponse.SerializedSOAPResponse;
                                                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISroutingTransitionNumberfindComericaRoutngTrnsitNbrRequest Request/Response {requestResponse}", true);
                                            }
                                        }
                                        progressString = "After findBankNameByRoutingNumber EC Call";
                                        // VEIS Dependency missing definition
                                        comerica = findComericaResponse.comericaRoutngTrnsitNbr.Trim();
                                    }
                                    #endregion

                                    routingNumber = addressEFTVO.mcs_routingNumber.Trim();

                                    thisNewEntity["udo_routingnumber"] = routingNumber;
                                }
                                #endregion

                                thisNewEntity["udo_accountnumber"] = accountNumber;
                                addressField = addressEFTVO.mcs_accountNumber;
                            }
                            #endregion

                            #region udo_accounttype
                            if (addressEFTVO.mcs_accountType != string.Empty)
                            {
                                thisNewEntity["udo_accounttype"] = addressEFTVO.mcs_accountType;
                                addressField += " " + addressEFTVO.mcs_accountType;
                            }
                            #endregion

                            #region udo_bankname
                            if (addressEFTVO.mcs_bankName != string.Empty)
                            {
                                thisNewEntity["udo_bankname"] = addressEFTVO.mcs_bankName;
                                addressField += " " + addressEFTVO.mcs_bankName;
                            }
                            #endregion

                            #region append routingNumber to addressField
                            if (addressEFTVO.mcs_routingNumber != string.Empty)
                            {
                                addressField += " " + routingNumber;
                            }
                            #endregion
                        }
                        #endregion

                        #region udo_paymentdetailscomplete = !oktoloadPaymentDetails
                        if (!oktoloadPaymentDetails)
                        {
                            thisNewEntity["udo_paymentdetailscomplete"] = true;
                        }
                        #endregion

                        #region udo_returnpayment
                        if (hPaymentVOItem.VEISrtvpmtsumbdnrreturnPaymentInfo != null)
                        {
                            if (hPaymentVOItem.VEISrtvpmtsumbdnrreturnPaymentInfo.mcs_returnReason != null)
                            {
                                thisNewEntity["udo_returnpayment"] = hPaymentVOItem.VEISrtvpmtsumbdnrreturnPaymentInfo.mcs_returnReason;
                            }
                        }
                        #endregion

                        #region checkinfo
                        if (hPaymentVOItem.VEISrtvpmtsumbdnrcheckAddressInfo != null)
                        {
                            var addressCheckVO = hPaymentVOItem.VEISrtvpmtsumbdnrcheckAddressInfo;

                            var zipCode = addressCheckVO.mcs_zipCode;
                            peopleEntity["udo_zip"] = zipCode;
                            if (addressCheckVO.mcs_addressLine1 != string.Empty)
                            {
                                addressField = addressCheckVO.mcs_addressLine1;

                            }
                            if (addressCheckVO.mcs_addressLine2 != string.Empty)
                            {
                                addressField += " " + addressCheckVO.mcs_addressLine2;

                            }
                            if (addressCheckVO.mcs_addressLine3 != string.Empty)
                            {
                                addressField += " " + addressCheckVO.mcs_addressLine3;

                            }
                            if (addressCheckVO.mcs_addressLine4 != string.Empty)
                            {
                                addressField += " " + addressCheckVO.mcs_addressLine4;
                            }
                            if (addressCheckVO.mcs_addressLine5 != string.Empty)
                            {
                                addressField += " " + addressCheckVO.mcs_addressLine5;

                            }
                            if (addressCheckVO.mcs_addressLine6 != string.Empty)
                            {
                                addressField += " " + addressCheckVO.mcs_addressLine6;
                            }
                            if (addressCheckVO.mcs_addressLine7 != string.Empty)
                            {
                                addressField += " " + addressCheckVO.mcs_addressLine7;
                            }
                            if (addressCheckVO.mcs_zipCode != string.Empty)
                            {
                                //in some cases, the zip code appears in one of the previous address lines
                                //we check indexof to make sure that the zip isn't already in there
                                if (addressField.IndexOf(addressCheckVO.mcs_zipCode) == -1)
                                    addressField += " " + addressCheckVO.mcs_zipCode;
                            }


                            #region toppayment stuff for people entity
                            if (TopPayment)
                            {
                                //payment addresses start with name on line 1
                                #region request.PayeeCode != "00"
                                if (request.PayeeCode != "00")
                                {
                                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "payee code:" + request.PayeeCode, request.Debug);
                                    peopleEntity["udo_address1"] = string.Empty;
                                    peopleEntity["udo_address2"] = string.Empty;
                                    peopleEntity["udo_address3"] = string.Empty;
                                    peopleEntity["udo_city"] = string.Empty;
                                    peopleEntity["udo_state"] = string.Empty;
                                    peopleEntity["udo_country"] = string.Empty;
                                    peopleEntity["udo_zip"] = string.Empty;
                                    if (!request.IDProofOrchestration) peopleEntity["udo_paymentcomplete"] = true;

                                    if (addressCheckVO.mcs_addressLine2 != string.Empty)
                                    {
                                        peopleEntity["udo_address1"] = addressCheckVO.mcs_addressLine2;
                                    }
                                    if (addressCheckVO.mcs_addressLine3 != string.Empty)
                                    {
                                        var addTemp = addressCheckVO.mcs_addressLine3;
                                        if (addTemp.Contains(zipCode))
                                        {
                                            var cityIndex = addTemp.IndexOf(" ");

                                            peopleEntity["udo_city"] = addTemp.Substring(0, cityIndex);

                                            var theRest = addTemp.Substring(cityIndex + 1);
                                            var stateIndex = theRest.IndexOf(" ");
                                            if (stateIndex < 0)
                                            {
                                                peopleEntity["udo_state"] = theRest;
                                            }
                                            else
                                            {
                                                peopleEntity["udo_state"] = theRest.Substring(0, stateIndex);
                                            }
                                        }
                                        else
                                        {
                                            peopleEntity["udo_address2"] = addressCheckVO.mcs_addressLine3;
                                        }
                                    }
                                    if (addressCheckVO.mcs_addressLine4 != string.Empty)
                                    {
                                        var addTemp = addressCheckVO.mcs_addressLine4;
                                        if (addTemp.Contains(zipCode))
                                        {
                                            if (addTemp.Equals(zipCode))
                                            {
                                                var cityIndex = addTemp.IndexOf(" ");
                                                if (cityIndex > 0)
                                                {
                                                    peopleEntity["udo_city"] = addTemp.Substring(0, cityIndex);

                                                    var theRest = addTemp.Substring(cityIndex + 1);
                                                    var stateIndex = theRest.IndexOf(" ");

                                                    if (stateIndex < 0)
                                                    {
                                                        peopleEntity["udo_state"] = theRest;
                                                    }
                                                    else
                                                    {
                                                        peopleEntity["udo_state"] = theRest.Substring(0, stateIndex);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            peopleEntity["udo_address3"] = addressCheckVO.mcs_addressLine4;
                                        }
                                    }
                                    if (addressCheckVO.mcs_addressLine5 != string.Empty)
                                    {
                                        var addTemp = addressCheckVO.mcs_addressLine5;
                                        if (addTemp.Contains(zipCode))
                                        {
                                            if (addTemp.Equals(zipCode))
                                            {
                                                var cityIndex = addTemp.IndexOf(" ");
                                                if (cityIndex > 0)
                                                {
                                                    peopleEntity["udo_city"] = addTemp.Substring(0, cityIndex);

                                                    var theRest = addTemp.Substring(cityIndex + 1);
                                                    var stateIndex = theRest.IndexOf(" ");

                                                    if (stateIndex < 0)
                                                    {
                                                        peopleEntity["udo_state"] = theRest;
                                                    }
                                                    else
                                                    {
                                                        peopleEntity["udo_state"] = theRest.Substring(0, stateIndex);
                                                    }
                                                }
                                            }
                                        }

                                    }

                                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(request.MessageId, peopleEntity, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));

                                }
                                #endregion
                                else
                                {
                                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "did not update people, payee code:" + request.PayeeCode, request.Debug);
                                }
                            }
                            #endregion
                        }
                        #endregion

                        #region udo_address = addressField
                        thisNewEntity["udo_address"] = addressField;
                        #endregion

                        #region related entities
                        if (request.UDOcreatePaymentsRelatedEntitiesInfo != null)
                        {
                            foreach (var relatedItem in request.UDOcreatePaymentsRelatedEntitiesInfo)
                            {
                                thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                            }
                        }
                        #endregion

                        #region top payment
                        if (TopPayment)
                        {
                            thisNewEntity["udo_latestpayment"] = true;

                            if (!request.IDProofOrchestration) topPaymentID = OrgServiceProxy.Create(TruncateHelper.TruncateFields(request.MessageId, thisNewEntity, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));

                            TopPayment = false;
                        }
                        else
                        {
                            if (!request.IDProofOrchestration)
                            {
                                CreateRequest createPaymentData = new CreateRequest
                                {
                                    Target = thisNewEntity
                                };
                                requestCollection.Add(createPaymentData);
                            }
                        }
                        #endregion

                        paymentsCount++;
                        #endregion
                    }
                }
                #endregion

                #region Create records
                if (request.vetsnapshotId != Guid.Empty)
                {
                    Entity vetSnapShot = new Entity("udo_veteransnapshot")
                    {
                        Id = request.vetsnapshotId
                    };

                    if (!String.IsNullOrWhiteSpace(lastpaymentamount))
                    {
                        vetSnapShot["udo_amount"] = lastpaymentamount;
                        vetSnapShot["udo_lastpaiddate"] = lastpaymentdate.Month + "/" + lastpaymentdate.Day + "/" + lastpaymentdate.Year;

                    }
                    if (hasNextPayment)
                    {
                        vetSnapShot["udo_nextpaiddate"] = nextpaymentdate.Month + "/" + nextpaymentdate.Day + "/" + nextpaymentdate.Year; ;
                        vetSnapShot["udo_nextamount"] = nextpaymentamount;

                        response.NextAmount = nextpaymentamount;
                        response.NextScheduledPayDate = nextpaymentdate.Month + "/" + nextpaymentdate.Day + "/" + nextpaymentdate.Year;
                    }
                    vetSnapShot["udo_paymentscompleted"] = new OptionSetValue(752280002);
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(request.MessageId, vetSnapShot, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "Updated Snapshot", request.Debug);
                }

                if (!request.IDProofOrchestration)
                {
                    if (requestCollection.Count > 0)
                    {
                        var result = ExecuteMultipleHelper.ExecuteMultiple(OrgServiceProxy, requestCollection, request.OrganizationName, Guid.Empty, request.Debug);

                        if (_debug)
                        {
                            LogBuffer += result.LogDetail;
                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, LogBuffer, request.Debug);
                        }

                        if (result.IsFaulted)
                        {
                            LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, method, result.ErrorDetail);
                            response.ExceptionMessage = result.FriendlyDetail;
                            response.ExceptionOccurred = true;
                            return response;
                        }
                    }
                    string logInfo = string.Format("Payment Records Created: {0}", paymentsCount);
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, logInfo, request.Debug);
                }
                else
                {
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, "In Orchestration, no payments created", request.Debug);
                }
                #endregion

                if (paymentsCount == 0)
                {
                    if (request.vetsnapshotId != Guid.Empty)
                    {
                        Entity vetSnapShot = new Entity();
                        vetSnapShot.LogicalName = "udo_veteransnapshot";
                        vetSnapShot.Id = request.vetsnapshotId;
                        vetSnapShot["udo_paymentscompleted"] = new OptionSetValue(752280002);
                        OrgServiceProxy.Update(vetSnapShot);
                    }
                }
                else
                {
                    if (!request.IDProofOrchestration && oktoloadPaymentDetails)
                    {
                        if (request.PayeeCode == "00")
                        {
                            loadPayments(topPaymentID, request, topFbtId, topPaymentIdentifier);
                        }
                        else
                        {
                            loadPaymentsSync(topPaymentID, request, topFbtId, topPaymentIdentifier);
                        }
                    }
                }
                if (!request.IDProofOrchestration && request.udo_payeecodeId != System.Guid.Empty)
                {
                    var parent = new Entity();
                    parent.Id = request.udo_payeecodeId;
                    parent.LogicalName = "udo_payeecode";
                    parent["udo_paymentloadcomplete"] = true;
                    OrgServiceProxy.Update(parent);
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                aiLogger.LogException(ExecutionException, "999");

                LogHelper.LogError(request.MessageId, request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method, ExecutionException);
                response.ExceptionMessage = "Failed to Process Payment Data " + ExecutionException.Message;
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

        private static bool IsCompensation(string programType)
        {
            if (String.IsNullOrEmpty(programType)) return false;
            return programType.IndexOf("Compensation", StringComparison.InvariantCultureIgnoreCase) > -1
                || programType.IndexOf("Pension", StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        private static bool IsCompensation(VEISrtvpmtsumbdnrpaymentsMultipleResponse p)
        {
            return IsCompensation(p.mcs_bdnRecordType) || IsCompensation(p.mcs_programType);
        }
        private static void loadPayments(Guid paymentId, UDOcreatePaymentsRequest request, Int64 topFbtId, Int64 topPaymentIdentifier)
        {
            var veteranId = Guid.Empty;
            var vetSnapId = Guid.Empty;
            var paymentGuid = Guid.Empty;
            var personId = Guid.Empty;
            if (request.UDOcreatePaymentsRelatedEntitiesInfo != null)
            {
                foreach (var relatedItem in request.UDOcreatePaymentsRelatedEntitiesInfo)
                {
                    if (relatedItem.RelatedEntityName == "udo_veteransnapshot")
                    {
                        vetSnapId = relatedItem.RelatedEntityId;
                    }
                    if (relatedItem.RelatedEntityName == "udo_person")
                    {
                        personId = relatedItem.RelatedEntityId;
                    }
                    if (relatedItem.RelatedEntityName == "contact")
                    {
                        veteranId = relatedItem.RelatedEntityId;
                    }
                }
            }
            var veteranReference = new UDOcreateRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = veteranId,
                RelatedEntityName = "contact"
            };
            var udo_paymentReference = new UDOcreateRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_paymentid",
                RelatedEntityId = paymentId,
                RelatedEntityName = "udo_payment"
            };

            UDOgetPaymentDetailsAsyncRequest thisRequest = new UDOgetPaymentDetailsAsyncRequest();

            var references = new[] { veteranReference, udo_paymentReference };
            thisRequest.UDOcreateRelatedEntitiesInfo = references;

            thisRequest.LegacyServiceHeaderInfo = request.LegacyServiceHeaderInfo;
            thisRequest.vetsnapshotId = vetSnapId;
            thisRequest.udo_personId = personId;
            thisRequest.PaymentId = topPaymentIdentifier;
            thisRequest.FbtId = topFbtId;
            thisRequest.LogSoap = request.LogSoap;
            thisRequest.LogTiming = request.LogTiming;
            thisRequest.ownerId = request.ownerId;
            thisRequest.ownerType = request.ownerType;
            thisRequest.OrganizationName = request.OrganizationName;
            thisRequest.Debug = request.Debug;
            thisRequest.UserId = request.UserId;
            thisRequest.udo_paymentId = paymentId;
            thisRequest.LegacyServiceHeaderInfo = new UDOHeaderInfo
            {
                ///Header Info
                ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                LoginName = request.LegacyServiceHeaderInfo.LoginName,
                StationNumber = request.LegacyServiceHeaderInfo.StationNumber,
            };
        }
        private static void loadPaymentsSync(Guid paymentId, UDOcreatePaymentsRequest request, Int64 topFbtId, Int64 topPaymentIdentifier)
        {
            var veteranId = Guid.Empty;
            var vetSnapId = Guid.Empty;

            var personId = Guid.Empty;
            if (request.UDOcreatePaymentsRelatedEntitiesInfo != null)
            {
                foreach (var relatedItem in request.UDOcreatePaymentsRelatedEntitiesInfo)
                {
                    if (relatedItem.RelatedEntityName == "udo_veteransnapshot")
                    {
                        vetSnapId = relatedItem.RelatedEntityId;
                    }
                    if (relatedItem.RelatedEntityName == "udo_person")
                    {
                        personId = relatedItem.RelatedEntityId;
                    }
                    if (relatedItem.RelatedEntityName == "contact")
                    {
                        veteranId = relatedItem.RelatedEntityId;
                    }
                }
            }
            var veteranReference = new UDOcreateRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_veteranid",
                RelatedEntityId = veteranId,
                RelatedEntityName = "contact"
            };
            var udo_paymentReference = new UDOcreateRelatedEntitiesMultipleRequest()
            {
                RelatedEntityFieldName = "udo_paymentid",
                RelatedEntityId = paymentId,
                RelatedEntityName = "udo_payment"
            };

            UDOgetPaymentDetailsRequest thisRequest = new UDOgetPaymentDetailsRequest();
            thisRequest.MessageId = request.MessageId;
            var references = new[] { veteranReference, udo_paymentReference };
            thisRequest.UDOcreateRelatedEntitiesInfo = references;
            thisRequest.LegacyServiceHeaderInfo = request.LegacyServiceHeaderInfo;
            thisRequest.vetsnapshotId = vetSnapId;
            thisRequest.udo_personId = personId;
            thisRequest.PaymentId = topPaymentIdentifier;
            thisRequest.FbtId = topFbtId;
            thisRequest.LogSoap = request.LogSoap;
            thisRequest.LogTiming = request.LogTiming;
            thisRequest.ownerId = request.ownerId;
            thisRequest.ownerType = request.ownerType;
            thisRequest.OrganizationName = request.OrganizationName;
            thisRequest.Debug = request.Debug;
            thisRequest.UserId = request.UserId;
            thisRequest.udo_paymentId = paymentId;

            thisRequest.LegacyServiceHeaderInfo = new UDOHeaderInfo
            {
                ///Header Info
                ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                LoginName = request.LegacyServiceHeaderInfo.LoginName,
                StationNumber = request.LegacyServiceHeaderInfo.StationNumber,
            };

            var getPaymentsLogic = new UDOgetPaymentDetailsProcessor();
            var response = (UDOgetPaymentDetailsResponse)getPaymentsLogic.Execute(thisRequest);
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