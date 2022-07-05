using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using VRM.Integration.Servicebus.Core;
using VRM.Integration.UDO.Payments.Messages;
using VRM.Integration.Servicebus.Logging.CRM.Util;
using System.Linq;
using System.Linq.Expressions;
using VRM.Integration.UDO.Common;
using VIMT.PaymentInformationService.Messages;

namespace VRM.Integration.UDO.Payments.Processors
{
    class UDOcreatePaymentsProcessor
    {
        private bool _debug { get; set; }
        private const string method = "UDOcreatePaymentsProcessor";
        private string LogBuffer { get; set; }

        public IMessageBase Execute(UDOcreatePaymentsRequest request)
        {

            //if (request.Debug)
            //{
            //    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreatePayments", request.SerializeToString());
            //}
            //var request = message as createPaymentsRequest;
            UDOcreatePaymentsResponse response = new UDOcreatePaymentsResponse();
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
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDOcreatePaymentsProcessor Processor, Connection Error", connectException.Message);
                response.ExceptionMessage = "Failed to get CRMConnection";
                response.ExceptionOccured = true;
                return response;
            }
            #endregion

            progressString = "After Connection";
            var paymentsCount = 0;
            try
            {
                var lastpaymentdate = System.DateTime.MinValue;
                var lastpaymentamount = "";
                var nextpaymentdate = System.DateTime.MinValue;
                var nextpaymentamount = "";
                var hasNextPayment = false;

                // prefix = rtvpmtsumbdnretrievePaymentSummaryWithBDNRequest();
                var retrievePaymentSummaryWithBDNRequest = new VIMTrtvpmtsumbdnretrievePaymentSummaryWithBDNRequest();
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
                retrievePaymentSummaryWithBDNRequest.LegacyServiceHeaderInfo = new VIMT.PaymentInformationService.Messages.HeaderInfo
                {
                    ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                    ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                    LoginName = request.LegacyServiceHeaderInfo.LoginName,
                    StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                };
                //TODO(NP): Update the VIMT call to VEIS
                var retrievePaymentSummaryWithBDNResponse = retrievePaymentSummaryWithBDNRequest.SendReceive<VIMTrtvpmtsumbdnretrievePaymentSummaryWithBDNResponse>(MessageProcessType.Local);
                progressString = "After VIMT EC Call";

                response.ExceptionMessage = retrievePaymentSummaryWithBDNResponse.ExceptionMessage;
                response.ExceptionOccured = retrievePaymentSummaryWithBDNResponse.ExceptionOccured;

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
                    retrievePaymentSummaryWithBDNResponse.VIMTrtvpmtsumbdnPaymentSummaryResponseInfo != null &&
                    retrievePaymentSummaryWithBDNResponse.VIMTrtvpmtsumbdnPaymentSummaryResponseInfo.VIMTrtvpmtsumbdnpaymentsInfo != null)
                {
                    System.Collections.Generic.List<UDOcreatePaymentsMultipleResponse> UDOcreatePaymentsArray = new System.Collections.Generic.List<UDOcreatePaymentsMultipleResponse>();
                    var paymentsInfo = retrievePaymentSummaryWithBDNResponse.VIMTrtvpmtsumbdnPaymentSummaryResponseInfo.VIMTrtvpmtsumbdnpaymentsInfo;

                    // Define Flags
                    bool isScheduled,
                        isCompensation,
                        hasCompensation = paymentsInfo.Any(p => IsCompensation(p)),
                        hasScheduled = paymentsInfo.Any(p => p.mcs_scheduledDate != System.DateTime.MinValue);

                    var paymentsSorted = paymentsInfo.OrderByDescending(h => h.mcs_paymentDate);

                    foreach (var hPaymentVOItem in paymentsSorted)
                    {
                        #region foreach payment record

                        isScheduled = hasScheduled && (hPaymentVOItem.mcs_scheduledDate != System.DateTime.MinValue);
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
                        if (hPaymentVOItem.VIMTrtvpmtsumbdnpaymentRecordIdentifierInfo != null)
                        {
                            if (hPaymentVOItem.VIMTrtvpmtsumbdnpaymentRecordIdentifierInfo.mcs_paymentID > 0)
                            {

                                thisNewEntity["udo_paymentidentifier"] = hPaymentVOItem.VIMTrtvpmtsumbdnpaymentRecordIdentifierInfo.mcs_paymentID.ToString();
                                responseIds.paymentId = hPaymentVOItem.VIMTrtvpmtsumbdnpaymentRecordIdentifierInfo.mcs_paymentID;
                                oktoloadPaymentDetails = true;
                            }

                            if (hPaymentVOItem.VIMTrtvpmtsumbdnpaymentRecordIdentifierInfo.mcs_transactionID > 0)
                            {
                                thisNewEntity["udo_transactionid"] = hPaymentVOItem.VIMTrtvpmtsumbdnpaymentRecordIdentifierInfo.mcs_transactionID.ToString();
                                // thisNewEntity["udo_ftbid" = Convert.ToInt32(hPaymentVOItem.VIMTrtvpmtsumbdnpaymentRecordIdentifierInfo.mcs_transactionID);
                                responseIds.ftbid = hPaymentVOItem.VIMTrtvpmtsumbdnpaymentRecordIdentifierInfo.mcs_transactionID;
                                oktoloadPaymentDetails = true;
                            }
                        }
                        #endregion

                        #region udo_authorizedddate
                        if (hPaymentVOItem.mcs_authorizedDate != System.DateTime.MinValue)
                        {
                            thisNewEntity["udo_authorizeddate"] = hPaymentVOItem.mcs_authorizedDate;
                        }
                        #endregion

                        #region last payment and next payment

                        if (hPaymentVOItem.mcs_paymentDate != System.DateTime.MinValue)
                        {
                            #region hasPaymentDate
                            thisNewEntity["udo_paydate"] = hPaymentVOItem.mcs_paymentDate;
                            // LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "Payment VetSnapshot", "paymentsCount:" + paymentsCount + "  date:" + hPaymentVOItem.mcs_paymentDate.ToShortDateString());
                            snapshotCount += 1;

                            if (isCompensation && hPaymentVOItem.mcs_paymentDate > lastpaymentdate)
                            {
                                lastpaymentamount = moneyStringFormat(hPaymentVOItem.mcs_paymentAmount.ToString());
                                lastpaymentdate = hPaymentVOItem.mcs_paymentDate;
                                TopPayment = true;
                                if (hPaymentVOItem.VIMTrtvpmtsumbdnpaymentRecordIdentifierInfo != null)
                                {
                                    if (hPaymentVOItem.VIMTrtvpmtsumbdnpaymentRecordIdentifierInfo.mcs_paymentID > 0)
                                    {
                                        topPaymentIdentifier = hPaymentVOItem.VIMTrtvpmtsumbdnpaymentRecordIdentifierInfo.mcs_paymentID;
                                        oktoloadPaymentDetails = true;
                                    }

                                    if (hPaymentVOItem.VIMTrtvpmtsumbdnpaymentRecordIdentifierInfo.mcs_transactionID > 0)
                                    {
                                        topFbtId = hPaymentVOItem.VIMTrtvpmtsumbdnpaymentRecordIdentifierInfo.mcs_transactionID;
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
                                // LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "Payment VetSnapshot", "paymentsCount:" + paymentsCount + "  mcs_scheduledDate:" + hPaymentVOItem.mcs_scheduledDate.ToShortDateString());

                                nextpaymentdate = hPaymentVOItem.mcs_scheduledDate;
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
                            //   LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "Create Payment", "Found BDNRecord");
                            thisNewEntity["udo_programtype"] = hPaymentVOItem.mcs_bdnRecordType;
                            thisNewEntity["udo_recipient"] = hPaymentVOItem.mcs_beneficiaryName;
                        }
                        #endregion

                        #region udo_scheduleddate
                        if (isScheduled)
                        {
                            thisNewEntity["udo_scheduleddate"] = hPaymentVOItem.mcs_scheduledDate;
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

                        if (hPaymentVOItem.VIMTrtvpmtsumbdnaddressEFTInfo != null)
                        {
                            var addressEFTVO = hPaymentVOItem.VIMTrtvpmtsumbdnaddressEFTInfo;

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
                                        var findComericaRequest = new VIMT.DdeftWebService.Messages.VIMTroutingTransitionNumberfindComericaRoutngTrnsitNbrRequest
                                        {
                                            LogTiming = request.LogTiming,
                                            LogSoap = request.LogSoap,
                                            Debug = request.Debug,
                                            RelatedParentEntityName = request.RelatedParentEntityName,
                                            RelatedParentFieldName = request.RelatedParentFieldName,
                                            RelatedParentId = request.RelatedParentId,
                                            UserId = request.UserId,
                                            OrganizationName = request.OrganizationName,
                                            LegacyServiceHeaderInfo = new VIMT.DdeftWebService.Messages.HeaderInfo
                                            {
                                                ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                                                ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                                                LoginName = request.LegacyServiceHeaderInfo.LoginName,
                                                StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                                            }
                                        };
                                        //TODO(NP): Update the VIMT call to VEIS
                                        var findComericaResponse = findComericaRequest.SendReceive<VIMT.DdeftWebService.Messages.VIMTroutingTransitionNumberfindComericaRoutngTrnsitNbrResponse>(MessageProcessType.Local);
                                        progressString = "After findBankNameByRoutingNumber EC Call";
                                        comerica = findComericaResponse.comericaRoutngTrnsitNbr.Trim();
                                    }
                                    #endregion

                                    routingNumber = addressEFTVO.mcs_routingNumber.Trim();

                                    // Kirk - removing masking
                                    //#region mask comerica routing number and accountnumber
                                    //if (string.Equals(comerica, routingNumber, StringComparison.InvariantCultureIgnoreCase))
                                    //{
                                    //    accountNumber = Tools.Mask(accountNumber);
                                    //    routingNumber = Tools.Mask(routingNumber, -4);
                                    //}
                                    //#endregion
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
                        if (hPaymentVOItem.VIMTrtvpmtsumbdnreturnPaymentInfo != null)
                        {
                            if (hPaymentVOItem.VIMTrtvpmtsumbdnreturnPaymentInfo.mcs_returnReason != null)
                            {
                                thisNewEntity["udo_returnpayment"] = hPaymentVOItem.VIMTrtvpmtsumbdnreturnPaymentInfo.mcs_returnReason;
                            }
                        }
                        #endregion

                        #region checkinfo
                        if (hPaymentVOItem.VIMTrtvpmtsumbdncheckAddressInfo != null)
                        {
                            var addressCheckVO = hPaymentVOItem.VIMTrtvpmtsumbdncheckAddressInfo;

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
                                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "Create Payments", "payee code:" + request.PayeeCode);
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

                                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "Create Payments", "Should be updating people with address from payments");
                                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(peopleEntity, request.OrganizationName, request.UserId, request.LogTiming));

                                }
                                #endregion
                                else
                                {
                                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "Create Payments", "did not update people, payee code:" + request.PayeeCode);
                                }
                            }
                            #endregion
                        }
                        #endregion

                        #region udo_address = addressField
                        thisNewEntity["udo_address"] = addressField;
                        #endregion

                        //not mapped thisNewEntity["udo_processingstation"]=??
                        //checkTraceNumber + return Reason
                        //not mapped thisNewEntity["udo_ro"]=??
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
                            // LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "Payment Create", "Creating Top Payment Record");
                            thisNewEntity["udo_latestpayment"] = true;

                            if (!request.IDProofOrchestration) topPaymentID = OrgServiceProxy.Create(TruncateHelper.TruncateFields(thisNewEntity, request.OrganizationName, request.UserId, request.LogTiming));

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
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(vetSnapShot, request.OrganizationName, request.UserId, request.LogTiming));
                    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "Create Payments", "Updated Snapshot");
                }

                if (!request.IDProofOrchestration)
                {
                    if (requestCollection.Count > 0)
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
                    string logInfo = string.Format("Payment Records Created: {0}", paymentsCount);
                    LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Payment Records Created", logInfo);
                }
                else
                {
                    LogHelper.LogInfo(request.OrganizationName, request.Debug, request.UserId, "Payment Request", "In Orchestration, no payments created");
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
                        OrgServiceProxy.Update(TruncateHelper.TruncateFields(vetSnapShot, request.OrganizationName, request.UserId, request.LogTiming));
                        // LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "Payment Create", "snapshot should be updated,paymentsCount == 0");
                    }
                }
                else
                {
                    //   LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "Payment Create", "about to load payments");
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
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(parent, request.OrganizationName, request.UserId, request.LogTiming));
                }
                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName, method + " Processor, Progess:" + progressString, ExecutionException);
                response.ExceptionMessage = "Failed to Process Payment Data";
                response.ExceptionOccured = true;
                return response;
            }
        }

        private static bool IsCompensation(string programType)
        {
            if (String.IsNullOrEmpty(programType)) return false;
            return programType.IndexOf("Compensation", StringComparison.InvariantCultureIgnoreCase) > -1
                || programType.IndexOf("Pension", StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        private static bool IsCompensation(VIMTrtvpmtsumbdnpaymentsMultipleResponse p)
        {
            return IsCompensation(p.mcs_bdnRecordType) || IsCompensation(p.mcs_programType);
        }
        private static void loadPayments(Guid paymentId, UDOcreatePaymentsRequest request, Int64 topFbtId, Int64 topPaymentIdentifier)
        {
            //    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreatePaymentsProcessor Processor", "loadPaymentDetails Async");
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
            thisRequest.LegacyServiceHeaderInfo = new VRM.Integration.UDO.Messages.HeaderInfo
            {
                ///Header Info
                ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                LoginName = request.LegacyServiceHeaderInfo.LoginName,
                StationNumber = request.LegacyServiceHeaderInfo.StationNumber,
            };
            thisRequest.SendAsync(MessageProcessType.Local);
            //  LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreatePaymentsProcessor Processor", "loadPaymentDetails Async sent");

        }
        private static void loadPaymentsSync(Guid paymentId, UDOcreatePaymentsRequest request, Int64 topFbtId, Int64 topPaymentIdentifier)
        {
            //  LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreatePaymentsProcessor Processor", "loadPaymentDetails sync");
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

            thisRequest.LegacyServiceHeaderInfo = new VRM.Integration.UDO.Messages.HeaderInfo
            {
                ///Header Info
                ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                LoginName = request.LegacyServiceHeaderInfo.LoginName,
                StationNumber = request.LegacyServiceHeaderInfo.StationNumber,
            };

            var getPaymentsLogic = new UDOgetPaymentDetailsProcessor();
            var response = (UDOgetPaymentDetailsResponse)getPaymentsLogic.Execute(thisRequest);
            //TODO(NP): Already commented in code; Check this again
            //var response = thisRequest.SendReceive<UDOgetPaymentDetailsResponse>(MessageProcessType.Local);
            //   LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOcreatePaymentsProcessor Processor", "loadPaymentDetails returned");



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