
namespace UDO.LOB.CADD.Processors
{
    using Microsoft.Pfe.Xrm;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Sdk.WebServiceClient;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using UDO.LOB.CADD.Messages;
    using UDO.LOB.Core;
    using UDO.LOB.Extensions;
    using UDO.LOB.Extensions.Logging;
    using VEIS.Core.Messages;
    using VEIS.Messages.AppealService;
    using VEIS.Messages.ClaimantService;
    using VEIS.Messages.DdeftWebService;
    using VEIS.Messages.PaymentInformationService;
    using VEIS.Messages.VeteranWebService;

    class UDOInitiateCADDProcessor
    {
        private IOrganizationService OrgServiceProxy;
        private OrganizationServiceProxyOptions organizationServiceProxyOptions;
        private bool _debug { get; set; }
        private string LogBuffer { get; set; }
        private const string method = "UDOInitiateCADDProcessor";

        public IMessageBase Execute(UDOInitiateCADDRequest request)
        {
            UDOInitiateCADDResponse response = new UDOInitiateCADDResponse
            {
                MessageId = request.MessageId,
                InnerExceptions = new List<UDOException>().ToArray()
            };



            var caddExceptions = new List<UDOException>();

            var progressString = "Top of Process or";

            if (request == null)
            {
                caddExceptions.Add(new UDOException()
                {
                    ExceptionMessage = "Called with no message",
                    ExceptionOccurred = true,
                    ExceptionCategory = "Appeal"
                });

                caddExceptions.Add(new UDOException()
                {
                    ExceptionMessage = "Called with no message",
                    ExceptionOccurred = true,
                    ExceptionCategory = "Claimant"
                });

                caddExceptions.Add(new UDOException()
                {
                    ExceptionMessage = "Called with no message",
                    ExceptionOccurred = true,
                    ExceptionCategory = "Payment"
                });

                caddExceptions.Add(new UDOException()
                {
                    ExceptionMessage = "Called with no message",
                    ExceptionOccurred = true,
                    ExceptionCategory = "Veteran"
                });

                response.ExceptionMessage = "Called with no message";
                response.ExceptionOccurred = true;
                response.InnerExceptions = caddExceptions.ToArray();

                return response;
            }

            if (request.DiagnosticsContext == null && request != null)
            {
                request.DiagnosticsContext = new DiagnosticsContext()
                {
                    AgentId = request.UserId,
                    MessageTrigger = method,
                    OrganizationName = request.OrganizationName,
                    StationNumber = request.LegacyServiceHeaderInfo != null ? request.LegacyServiceHeaderInfo.StationNumber : "NA",
                    IdProof = request.udo_IDProofId,
                    VeteranId = request.udo_veteranId
                };
            }
            TraceLogger aiLogger = new TraceLogger("UDOInitiateCADDProcessor.UDOInitiateCADDRequest", request);

            #region connect to CRM
            try
            {
                OrgServiceProxy = ConnectionCache.GetProxy();
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

            try
            {
                response.DiagnosticsContext = request.DiagnosticsContext;


                Entity thisNewEntity = new Entity();
                thisNewEntity.LogicalName = "va_bankaccount";

                #region Set Owner
                if (!String.IsNullOrEmpty(request.OwnerType) && request.OwnerId.HasValue)
                {
                    thisNewEntity["ownerid"] = new EntityReference(request.OwnerType, request.OwnerId.Value);
                }
                else
                {
                    thisNewEntity["ownerid"] = new EntityReference("systemuser", request.UserId);
                }
                #endregion

                #region Copy Basic Request Information
                thisNewEntity["va_payeetypecode"] = request.PayeeCode;
                thisNewEntity["udo_participantid"] = request.ptcpntId.ToString();
                if (request.udo_personId != Guid.Empty)
                {
                    thisNewEntity["udo_personid"] = new EntityReference("udo_person", request.udo_personId);
                }

                if (request.udo_snapshotid != Guid.Empty)
                {
                    thisNewEntity["udo_veteransnapshotid"] = new EntityReference("udo_veteransnapshot", request.udo_snapshotid);
                }

                // Set Default ptcpnt id's.  This can be overridden if an award is pulled using a filenumber.

                // If recip is omitted then it will update the parent veteran... 
                // terrible... because it could result in renaming the record.
                if (request.vetptcpntId != request.ptcpntId)
                {
                    thisNewEntity["va_participantbeneid"] = request.ptcpntId.ToString();
                    thisNewEntity["va_participantrecipid"] = request.ptcpntId.ToString();
                }

                thisNewEntity["va_participantvetid"] = request.vetptcpntId.ToString();
                // Set Veteran File Number.  All People - same vet file number.  Different participant id's
                thisNewEntity["va_filenumber"] = request.vetfileNumber;

                if (request.udo_IDProofId != Guid.Empty)
                {
                    thisNewEntity["udo_idproofid"] = new EntityReference("udo_idproof", request.udo_IDProofId);
                }

                #endregion

                #region findVeteranByPtcpntIdRequest
                // prefix = vetPctfindVeteranByPtcpntIdRequest();
                var findVeteranByPtcpntIdRequest = new VEISvetPctfindVeteranByPtcpntIdRequest
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
                    mcs_ptcpntid = request.ptcpntId,
                    LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    }
                };

                // REM: Invoke VEIS Endpoint
                // var findVeteranByPtcpntIdResponse = findVeteranByPtcpntIdRequest.SendReceive<VIMTvetPctfindVeteranByPtcpntIdResponse>(MessageProcessType.Local);
                //var findVeteranStopWatch = new Stopwatch();
                //findVeteranStopWatch.Start();
                var findVeteranByPtcpntIdResponse = WebApiUtility.SendReceive<VEISvetPctfindVeteranByPtcpntIdResponse>(findVeteranByPtcpntIdRequest, WebApiType.VEIS);
                //findVeteranStopWatch.Stop();
                //aiLogger.LogWebResponseDuration("VEISvetPctfindVeteranByPtcpntId", method, "010", findVeteranStopWatch.Elapsed.TotalMilliseconds);

                if (findVeteranByPtcpntIdRequest.LogSoap || findVeteranByPtcpntIdResponse.ExceptionOccurred)
                {
                    if (findVeteranByPtcpntIdResponse.SerializedSOAPRequest != null || findVeteranByPtcpntIdResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findVeteranByPtcpntIdResponse.SerializedSOAPRequest + findVeteranByPtcpntIdResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISbyRoutingTransitionNumberfindBankNameByRoutngTrnsitNbrRequest Request/Response {requestResponse}", true);
                    }
                }

                progressString = "After findVeteranByPtcpntIdRequest EC Call";
                if (findVeteranByPtcpntIdResponse.ExceptionOccurred)
                {
                    caddExceptions.Add(new UDOException()
                    {
                        ExceptionMessage = findVeteranByPtcpntIdResponse.ExceptionMessage,
                        ExceptionOccurred = true,
                        ExceptionCategory = "Veteran"
                    });

                    var udoException = new ApplicationException(findVeteranByPtcpntIdResponse.ExceptionMessage + " while calling findVeteranByPtcpntIdRequest EC");
                    throw udoException;
                }
                if (findVeteranByPtcpntIdResponse != null)
                {
                    if (findVeteranByPtcpntIdResponse.VEISvetPctreturnInfo != null)
                    {
                        if (findVeteranByPtcpntIdResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo != null)
                        {
                            #region veteran fields

                            var corpRecord = findVeteranByPtcpntIdResponse.VEISvetPctreturnInfo.VEISvetPctvetCorpRecordInfo;
                            /*
                            if (!string.IsNullOrEmpty(corpRecord.mcs_fileNumber))
                            {
                                // LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateCADD Processor", "request file number was:" + request.fileNumber);
                                request.fileNumber = corpRecord.mcs_fileNumber;
                                thisNewEntity["va_filenumber"] = request.fileNumber;
                                // LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateCADD Processor", "request file number NOW is:" + request.fileNumber);
                            }
                             */

                            // This code was commented out - doesn't look like it is used after it is set.
                            //if (!string.IsNullOrEmpty(corpRecord.mcs_ssn))
                            //{
                            //    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateCADD Processor", "request file number was:" + request.SSN);
                            //    request.SSN = corpRecord.mcs_ssn;
                            //    LogHelper.LogDebug(request.OrganizationName, request.Debug, request.UserId, "UDOInitiateCADD Processor", "request file number NOW is:" + request.SSN);
                            //}


                            if (!string.IsNullOrEmpty(corpRecord.mcs_dateOfBirth))
                            {
                                //DateTime newDateTime;
                                //if (DateTime.TryParse(corpRecord.mcs_dateOfBirth, out newDateTime))
                                //{
                                //    if (newDateTime > new System.DateTime(1900, 01, 01))
                                //    {
                                //        thisNewEntity["va_dateofbirth"] = newDateTime;
                                //    }
                                thisNewEntity["va_dateofbirthtext"] = corpRecord.mcs_dateOfBirth;
                                //}
                            }
                            PopulateField(thisNewEntity, "va_originalfirstname", corpRecord.mcs_firstName);
                            PopulateField(thisNewEntity, "va_originallastname", corpRecord.mcs_lastName);
                            PopulateField(thisNewEntity, "va_originalmiddlename", corpRecord.mcs_middleName);
                            PopulateField(thisNewEntity, "va_originalsuffix", corpRecord.mcs_suffixName);
                            PopulateField(thisNewEntity, "va_email", corpRecord.mcs_emailAddress);
                            PopulateField(thisNewEntity, "va_depositaccountnumber", corpRecord.mcs_eftAccountNumber);

                            var original_name = string.Empty;
                            if (thisNewEntity.Contains("va_originalfirstname"))
                            {
                                original_name += thisNewEntity["va_originalfirstname"].ToString() + " ";
                            }
                            if (thisNewEntity.Contains("va_originallastname"))
                            {
                                original_name += thisNewEntity["va_originallastname"].ToString();
                            }
                            var recordName = original_name.Trim();
                            if (!String.IsNullOrEmpty(recordName)) recordName += " - ";
                            recordName += DateTime.Now.ToShortDateString();
                            thisNewEntity["va_name"] = recordName;

                            thisNewEntity["udo_veteranid"] = new EntityReference("contact", request.udo_veteranId);

                            if (request.udo_veteranId != null)
                            {
                                string milServiceFetch = string.Format(
                                    "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' count='50'>" +
                                    "<entity name='udo_tourhistory'>" +
                                    "<attribute name='udo_releasedactiveduty'/>" +
                                    "<attribute name='udo_enteredactiveduty'/>" +
                                    "<filter type='and'>" +
                                    "<condition attribute='udo_veteranid' operator='eq' value='{0}' />" +
                                    "</filter>" +
                                    "</entity></fetch>", request.udo_veteranId.ToString());

                                var tours = OrgServiceProxy.RetrieveMultiple(new FetchExpression(milServiceFetch));

                                Entity latestTour = null;
                                DateTime latestRelease = new DateTime(1);

                                if (tours != null && tours.Entities != null && tours.Entities.Count > 0)
                                {
                                    foreach (var tour in tours.Entities)
                                    {
                                        DateTime thisRelease = new System.DateTime();
                                        var parsed = DateTime.TryParse(tour.GetAttributeValue<string>("udo_releasedactiveduty"), out thisRelease);

                                        if (parsed && DateTime.Compare(latestRelease, thisRelease) < 0)
                                        {
                                            latestRelease = thisRelease;
                                            latestTour = tour;
                                        }
                                    }

                                    if (latestTour != null)
                                    {
                                        thisNewEntity["udo_enteredactiveduty"] = latestTour.GetAttributeValue<String>("udo_enteredactiveduty");
                                        thisNewEntity["udo_releasedactiveduty"] = latestTour.GetAttributeValue<String>("udo_releasedactiveduty");
                                    }
                                }
                            }
                            thisNewEntity["va_accountownerid"] = new EntityReference("contact", request.udo_veteranId);

                            if (!string.IsNullOrEmpty(corpRecord.mcs_eftAccountType))
                            {
                                //953,850,000
                                var acctType = corpRecord.mcs_eftAccountType;
                                if (acctType.ToLower() == "checking")
                                {
                                    thisNewEntity["va_depositaccounttype"] = new OptionSetValue(953850000);
                                }
                                else
                                {
                                    if (acctType.ToLower() == "savings")
                                    {
                                        thisNewEntity["va_depositaccounttype"] = new OptionSetValue(953850001);
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(corpRecord.mcs_eftRoutingNumber))
                            {
                                thisNewEntity["va_routingnumber"] = corpRecord.mcs_eftRoutingNumber;
                                request.RoutingNumber = corpRecord.mcs_eftRoutingNumber.Trim();
                            }

                            #endregion

                            #region mailing address fields

                            PopulateField(thisNewEntity, "va_mailingaddress1", corpRecord.mcs_addressLine1);
                            PopulateField(thisNewEntity, "va_mailingaddress2", corpRecord.mcs_addressLine2);
                            PopulateField(thisNewEntity, "va_mailingaddress3", corpRecord.mcs_addressLine3);
                            PopulateField(thisNewEntity, "va_mailingaddresszipcode", corpRecord.mcs_zipCode);
                            PopulateField(thisNewEntity, "va_mailingcity", corpRecord.mcs_city);
                            PopulateField(thisNewEntity, "va_mailingstate", corpRecord.mcs_state);
                            PopulateField(thisNewEntity, "va_mailingcountry", corpRecord.mcs_country);

                            //if (!string.IsNullOrEmpty(corpRecord.mc))
                            //{
                            //                                thisNewEntity["va_mailingeffectivedate"] = corpRecord.mcs_e;
                            //}
                            #endregion

                            #region payment address fields
                            //TODO: These are always null in UD and in UDO and should not be.
                            PopulateField(thisNewEntity, "va_paymentaddress1", corpRecord.mcs_cpPaymentAddressLine1);
                            PopulateField(thisNewEntity, "va_paymentaddress2", corpRecord.mcs_cpPaymentAddressLine2);
                            PopulateField(thisNewEntity, "va_paymentaddress3", corpRecord.mcs_cpPaymentAddressLine3);
                            PopulateField(thisNewEntity, "va_paymentzipcode", corpRecord.mcs_cpPaymentZipCode);
                            PopulateField(thisNewEntity, "va_paymentcity", corpRecord.mcs_cpPaymentCity);
                            PopulateField(thisNewEntity, "va_paymentstate", corpRecord.mcs_cpPaymentState);
                            PopulateField(thisNewEntity, "va_paymentcountry", corpRecord.mcs_cpPaymentCountry);
                            PopulateField(thisNewEntity, "va_paymentforeignpostalcode", corpRecord.mcs_cpPaymentForeignZip);

                            #endregion

                            #region phone numbers
                            var area1 = "";
                            var area2 = "";
                            var phone1 = "";
                            var phone2 = "";
                            var phoneType1 = "";
                            var phoneType2 = "";
                            if (!string.IsNullOrEmpty(corpRecord.mcs_areaNumberOne))
                            {
                                area1 = corpRecord.mcs_areaNumberOne;
                            }
                            if (!string.IsNullOrEmpty(corpRecord.mcs_areaNumberTwo))
                            {
                                area2 = corpRecord.mcs_areaNumberTwo;
                            }
                            if (!string.IsNullOrEmpty(corpRecord.mcs_phoneNumberOne))
                            {
                                phone1 = corpRecord.mcs_phoneNumberOne;
                            }
                            if (!string.IsNullOrEmpty(corpRecord.mcs_phoneNumberTwo))
                            {
                                phone2 = corpRecord.mcs_phoneNumberTwo;
                            }
                            if (!string.IsNullOrEmpty(corpRecord.mcs_phoneTypeNameOne))
                            {
                                phoneType1 = corpRecord.mcs_phoneTypeNameOne;
                            }
                            if (!string.IsNullOrEmpty(corpRecord.mcs_phoneTypeNameTwo))
                            {
                                phoneType2 = corpRecord.mcs_phoneTypeNameTwo;
                            }

                            if (phoneType1 == "Nighttime" || phoneType1 == "NIGHTTIME")
                            {
                                thisNewEntity["va_phone1type"] = new OptionSetValue(953850001);
                            }
                            else
                            {
                                thisNewEntity["va_phone1type"] = new OptionSetValue(953850000);
                            }
                            if (phoneType2 == "Nighttime" || phoneType2 == "NIGHTTIME")
                            {
                                thisNewEntity["va_phone2type"] = new OptionSetValue(953850001);
                            }
                            else
                            {
                                thisNewEntity["va_phone2type"] = new OptionSetValue(953850000);
                            }
                            thisNewEntity["va_origarea1"] = area1;
                            thisNewEntity["va_area1"] = area1;
                            thisNewEntity["va_origarea2"] = area2;
                            thisNewEntity["va_area2"] = area2;
                            thisNewEntity["va_origphone1"] = phone1;
                            thisNewEntity["va_phone1"] = phone1;
                            thisNewEntity["va_origphone2"] = phone2;
                            thisNewEntity["va_phone2"] = phone2;
                            thisNewEntity["va_caddphone1"] = FormatTelephone(area1 + phone1);
                            thisNewEntity["va_caddphone2"] = FormatTelephone(area2 + phone2);
                            thisNewEntity["va_origphone2type"] = phoneType2;
                            thisNewEntity["va_origphone1type"] = phoneType2;

                            #endregion

                            thisNewEntity["va_addresstype"] = new OptionSetValue { Value = 953850000 };

                            var country = (corpRecord.mcs_country ?? string.Empty).ToUpperInvariant();
                            if (!string.IsNullOrEmpty(corpRecord.mcs_militaryPostOfficeTypeCode) && !string.IsNullOrEmpty(corpRecord.mcs_militaryPostalTypeCode))
                            {
                                thisNewEntity["va_addresstype"] = new OptionSetValue { Value = 953850002 };
                                //PopulateField(thisNewEntity, "udo_postaltypecode", corpRecord.mcs_militaryPostalTypeCode);
                                //PopulateField(thisNewEntity, "udo_postalofficetypecode", corpRecord.mcs_militaryPostOfficeTypeCode);
                                PopulateField(thisNewEntity, "va_verifycity", corpRecord.mcs_militaryPostOfficeTypeCode);
                                PopulateField(thisNewEntity, "va_verifystate", corpRecord.mcs_militaryPostalTypeCode);
                                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "Initiate CADD", "Address Type: Overseas Military", request.Debug);
                            }
                            else if ((country == "US" || country == "USA" || country == "U.S.A" || country == "UNITED STATES" || country == "UNITED STATES OF AMERICA") && (string.IsNullOrEmpty(corpRecord.mcs_militaryPostalTypeCode)) && (string.IsNullOrEmpty(corpRecord.mcs_militaryPostOfficeTypeCode)))
                            {
                                thisNewEntity["va_addresstype"] = new OptionSetValue { Value = 953850000 };
                                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "Initiate CADD", "Address Type: Domestic", request.Debug);
                            }
                            else if (!string.IsNullOrEmpty(country))
                            {
                                thisNewEntity["va_addresstype"] = new OptionSetValue { Value = 953850001 };
                                // set zip 
                                // va_paymentforeignpostalcode
                                thisNewEntity["va_verifyzipcode"] = thisNewEntity.GetAttributeValue<string>("va_paymentforeignpostalcode");
                                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "Initiate CADD", "Address Type: International", request.Debug);
                            }
                        }
                    }
                }
                #endregion

                #region retrievePaymentSummaryWithBDNRequest
                if (!String.IsNullOrEmpty(request.PayeeCode))
                {
                    // prefix = rtvpmtsumbdnretrievePaymentSummaryWithBDNRequest();
                    var retrievePaymentSummaryWithBDNRequest = new VEISrtvpmtsumbdnretrievePaymentSummaryWithBDNRequest
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
                        mcs_filenumber = request.vetfileNumber, //request.fileNumber,
                        mcs_participantid = request.ptcpntId,
                        mcs_payeecode = request.PayeeCode,
                        LegacyServiceHeaderInfo = new LegacyHeaderInfo
                        {
                            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                        }
                    };

                    // REM: Invoke VEIS Endpoint
                    // var retrievePaymentSummaryWithBDNResponse = retrievePaymentSummaryWithBDNRequest.SendReceive<VIMTrtvpmtsumbdnretrievePaymentSummaryWithBDNResponse>(MessageProcessType.Local);
                    //var rtvpmtsumbdnretrievePaymentSummaryStopWatch = new Stopwatch();
                    //rtvpmtsumbdnretrievePaymentSummaryStopWatch.Start();
                    var retrievePaymentSummaryWithBDNResponse = WebApiUtility.SendReceive<VEISrtvpmtsumbdnretrievePaymentSummaryWithBDNResponse>(retrievePaymentSummaryWithBDNRequest, WebApiType.VEIS);
                    //rtvpmtsumbdnretrievePaymentSummaryStopWatch.Stop();
                    //aiLogger.LogWebResponseDuration("VEISrtvpmtsumbdnretrievePaymentSummaryWithBDN", method, "020", rtvpmtsumbdnretrievePaymentSummaryStopWatch.Elapsed.TotalMilliseconds);

                    if (retrievePaymentSummaryWithBDNRequest.LogSoap || retrievePaymentSummaryWithBDNResponse.ExceptionOccurred)
                    {
                        if (retrievePaymentSummaryWithBDNResponse.SerializedSOAPRequest != null || retrievePaymentSummaryWithBDNResponse.SerializedSOAPResponse != null)
                        {
                            var requestResponse = retrievePaymentSummaryWithBDNResponse.SerializedSOAPRequest + retrievePaymentSummaryWithBDNResponse.SerializedSOAPResponse;
                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISrtvpmtsumbdnretrievePaymentSummaryWithBDNRequest Request/Response {requestResponse}", true);
                        }
                    }

                    progressString = "After retrievePaymentSummaryWithBDNRequest EC Call";
                    if (retrievePaymentSummaryWithBDNResponse != null)
                    {
                        if (retrievePaymentSummaryWithBDNResponse.ExceptionOccurred)
                        {
                            caddExceptions.Add(new UDOException()
                            {
                                ExceptionMessage = retrievePaymentSummaryWithBDNResponse.ExceptionMessage,
                                ExceptionOccurred = true,
                                ExceptionCategory = "Payment"
                            });

                            var udoException = new ApplicationException(retrievePaymentSummaryWithBDNResponse.ExceptionMessage + " while calling retrievePaymentSummaryWithBDNRequest EC");
                            throw udoException;
                        }
                        // Replaced: VEISrtvpmtsumbdnPaymentSummaryResponseInfo.VEISrtvpmtsumbdnpaymentsInfo = VEISrtvpmtsumbdnrpaymentSummaryResponseVOInfo.VEISrtvpmtsumbdnrpaymentsInfo
                        if (retrievePaymentSummaryWithBDNResponse.VEISrtvpmtsumbdnrpaymentSummaryResponseVOInfo.VEISrtvpmtsumbdnrpaymentsInfo != null)
                        {
                            // Replaced: VEISrtvpmtsumbdnPaymentSummaryResponseInfo.VEISrtvpmtsumbdnpaymentsInfo = 
                            var addressEFTVO = retrievePaymentSummaryWithBDNResponse.VEISrtvpmtsumbdnrpaymentSummaryResponseVOInfo.VEISrtvpmtsumbdnrpaymentsInfo;
                            // TODO: SerializeToString needs corrected. JSON is string, I do not see where in VEIS SerializeToString was replaced. 
                            //LogHelper.LogDebug(request.OrganizationName, true, request.UserId, "CADD", addressEFTVO.SerializeToString());
                            //LogHelper.LogDebug(request.OrganizationName, true, request.UserId, "CADD", "Program Types: " + addressEFTVO.Select(a => a.mcs_programType).Distinct().SerializeToString());
                            var latestPayments = addressEFTVO.OrderByDescending(p => p.mcs_paymentDate).Where(
                                o => o != null && !String.IsNullOrWhiteSpace(o.mcs_programType) &&
                                    (o.mcs_programType.Equals("compensation", StringComparison.InvariantCultureIgnoreCase) ||
                                    o.mcs_programType.Equals("pension", StringComparison.InvariantCultureIgnoreCase)));

                            if (latestPayments.Any())
                            {
                                var latestPayment = latestPayments.First();
                                if (latestPayment.VEISrtvpmtsumbdnraddressEFTInfo != null)
                                {
                                    var address = latestPayment.VEISrtvpmtsumbdnraddressEFTInfo;
                                    PopulateField(thisNewEntity, "va_account", address.mcs_accountNumber);
                                }
                                if (latestPayment.mcs_paymentAmountSpecified)
                                {
                                    thisNewEntity["va_netamount"] = new Money(latestPayment.mcs_paymentAmount);
                                }
                                if (latestPayment.mcs_paymentDateSpecified)
                                {
                                    PopulateDateField(thisNewEntity, "va_paydate", latestPayment.mcs_paymentDate);
                                }
                            }
                        }
                    }
                }
                #endregion

                #region getAppealAddress

                var getAppealAddressRequest = new VEISgetAppAddr_getAppellantAddressRequest
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
                    // Replaced: "getappellantaddressrequestInfo = new VIMTgetAppAddr_getappellantaddressrequest" 
                    //         == "VEISgetAppAddr_ReqgetAddressCriteriaInfo = new VEISgetAppAddr_getAppellantAddressRequest"
                    // getappellanaddressrequestinfo had definition does not exist error 
                    VEISgetAppAddr_ReqgetAddressCriteriaInfo = new VEISgetAppAddr_ReqgetAddressCriteria
                    {
                        // VEIS Dependency missing
                        mcs_SSN = request.vetfileNumber
                        /*,
                        searchfieldfirstnameInfo = new VIMTgetAppAddr_searchfieldfirstname { mcs_Value = request.appealFirstName, mcs_Partialflag = false },
                        searchfieldlastnameInfo = new VIMTgetAppAddr_searchfieldlastname { mcs_Value = request.appealLastName, mcs_Partialflag = false }
                        */
                    }
                };

                // REM: Invoke VEIS Endpoint
                // var getAppealAddressResponse = getAppealAddressRequest.SendReceive<VIMTgetAppAddr_getAppellantAddressResponse>(MessageProcessType.Local);
                //var getAppealAddressStopWatch = new Stopwatch();
                //getAppealAddressStopWatch.Start();
                var getAppealAddressResponse = WebApiUtility.SendReceive<VEISgetAppAddr_getAppellantAddressResponse>(getAppealAddressRequest, WebApiType.VEIS);
                //getAppealAddressStopWatch.Stop();
                //aiLogger.LogWebResponseDuration("VEISgetAppAddr_getAppellantAddress", method, "030", getAppealAddressStopWatch.Elapsed.TotalMilliseconds);

                if (getAppealAddressRequest.LogSoap || getAppealAddressResponse.ExceptionOccurred)
                {
                    if (getAppealAddressResponse.SerializedSOAPRequest != null || getAppealAddressResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = getAppealAddressResponse.SerializedSOAPRequest + getAppealAddressResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISgetAppAddr_getAppellantAddressRequest Request/Response {requestResponse}", true);
                    }
                }

                progressString = "After getAppealAddressRequest EC Call";

                if (getAppealAddressResponse.ExceptionOccurred)
                {
                    caddExceptions.Add(new UDOException()
                    {
                        ExceptionMessage = getAppealAddressResponse.ExceptionMessage,
                        ExceptionOccurred = true,
                        ExceptionCategory = "Appeal"
                    });

                    var udoException = new ApplicationException(getAppealAddressResponse.ExceptionMessage + " while calling getAppealAddressRequest EC");
                    throw udoException;
                }

                if (getAppealAddressResponse.VEISgetAppAddr_AppellantInfo != null)
                {

                    var latestAppellantAddress = getAppealAddressResponse.VEISgetAppAddr_AppellantInfo.OrderByDescending(a => a.VEISgetAppAddr_AppellantAddressInfo.mcs_AppellantAddressLastModifiedDate).FirstOrDefault();

                    if (latestAppellantAddress != null)
                    {
                        thisNewEntity["udo_appealsexist"] = true;  // without this set the appellant address is not updatable.
                        var latestAppellantAddressInfo = latestAppellantAddress.VEISgetAppAddr_AppellantAddressInfo;

                        PopulateField(thisNewEntity, "va_appellantaddresskey", latestAppellantAddressInfo.mcs_AddressKey);
                        PopulateField(thisNewEntity, "va_apellantaddress1", latestAppellantAddressInfo.mcs_AppellantAddressLine1);
                        PopulateField(thisNewEntity, "va_apellantaddress2", latestAppellantAddressInfo.mcs_AppellantAddressLine2);
                        PopulateField(thisNewEntity, "va_apellantcity", latestAppellantAddressInfo.mcs_AppellantAddressCityName);
                        PopulateField(thisNewEntity, "va_apellantstate", latestAppellantAddressInfo.mcs_AppellantAddressStateCode);
                        PopulateField(thisNewEntity, "va_apellantzipcode", latestAppellantAddressInfo.mcs_AppellantAddressZipCode);
                        PopulateField(thisNewEntity, "va_apellantcountry", latestAppellantAddressInfo.mcs_AppellantAddressCountryName);
                        PopulateField(thisNewEntity, "va_apellantworkphone", latestAppellantAddressInfo.mcs_AppellantWorkPhoneNumber);
                        PopulateField(thisNewEntity, "va_apellanthomephone", latestAppellantAddressInfo.mcs_AppellantHomePhoneNumber);

                        var first = "";
                        var middle = "";
                        var last = "";

                        if (!string.IsNullOrEmpty(latestAppellantAddress.mcs_AppellantFirstName))
                        {
                            first = latestAppellantAddress.mcs_AppellantFirstName + " ";
                        }
                        if (!string.IsNullOrEmpty(latestAppellantAddress.mcs_AppellantMiddleInitial))
                        {
                            middle = latestAppellantAddress.mcs_AppellantMiddleInitial[0] + ". ";
                        }
                        if (!string.IsNullOrEmpty(latestAppellantAddress.mcs_AppellantLastName))
                        {
                            last = latestAppellantAddress.mcs_AppellantLastName;
                        }

                        // If the new field is created, just replace the new field name with va_name 
                        //thisNewEntity["va_name"] = (first + middle + last).Trim() + " - " + DateTime.Now.ToShortDateString();

                    }
                }
                #endregion

                #region findAwardAddresses
                // prefix = fawdaddfindAwardAddressesRequest();
                var findAwardAddressesRequest = new VEISfawdaddfindAwardAddressesRequest
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
                    mcs_filenumber = request.vetfileNumber,
                    LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                        ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                        LoginName = request.LegacyServiceHeaderInfo.LoginName,
                        StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                    }
                };
                // REM: Invoke VEIS Endpoint
                // var findAwardAddressesResponse = findAwardAddressesRequest.SendReceive<VIMTfawdaddfindAwardAddressesResponse>(MessageProcessType.Local);
                //var findAwardAddressesStopWatch = new Stopwatch();
                //findAwardAddressesStopWatch.Start();
                var findAwardAddressesResponse = WebApiUtility.SendReceive<VEISfawdaddfindAwardAddressesResponse>(findAwardAddressesRequest, WebApiType.VEIS);
                //findAwardAddressesStopWatch.Stop();
                //aiLogger.LogWebResponseDuration("VEISfawdaddfindAwardAddresses", method, "040", findAwardAddressesStopWatch.Elapsed.TotalMilliseconds);

                if (findAwardAddressesRequest.LogSoap || findAwardAddressesResponse.ExceptionOccurred)
                {
                    if (findAwardAddressesResponse.SerializedSOAPRequest != null || findAwardAddressesResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findAwardAddressesResponse.SerializedSOAPRequest + findAwardAddressesResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISfawdaddfindAwardAddressesRequest Request/Response {requestResponse}", true);
                    }
                }

                progressString = "After findAwardAddressesRequest EC Call";
                if (findAwardAddressesResponse.ExceptionOccurred)
                {
                    caddExceptions.Add(new UDOException()
                    {
                        ExceptionMessage = findAwardAddressesResponse.ExceptionMessage,
                        ExceptionOccurred = true,
                        ExceptionCategory = "Claimant"
                    });

                    var udoException = new ApplicationException(findAwardAddressesResponse.ExceptionMessage + " while calling findAwardAddressesRequest EC");
                    throw udoException;
                }

                // Replaced: VIMTfawdaddreturnclmsInfo = VEISfawdaddreturnInfo
                if (findAwardAddressesResponse.VEISfawdaddreturnInfo != null)
                {
                    // Replaced: VIMTfawdaddawardAddressesclmsInfo = VEISfawdaddawardAddressesInfo
                    if (findAwardAddressesResponse.VEISfawdaddreturnInfo.VEISfawdaddawardAddressesInfo != null)
                    {
                        var shrinqaAwardAddress = findAwardAddressesResponse.VEISfawdaddreturnInfo.VEISfawdaddawardAddressesInfo;

                        if (shrinqaAwardAddress != null && shrinqaAwardAddress.Length > 0)
                        {

                            // Last award awarded to that ptcpntid (person) 
                            var latestAwardAddress = shrinqaAwardAddress.Where(
                                p => p.mcs_awardTypeCode == request.awardtypecode &&
                                     String.Equals(p.mcs_ptcpntRecipID.Trim(), request.ptcpntId.ToString())
                                ).FirstOrDefault();

                            if (latestAwardAddress != null)
                            {
                                PopulateField(thisNewEntity, "va_participantbeneid", latestAwardAddress.mcs_ptcpntBeneID);
                                PopulateField(thisNewEntity, "va_participantrecipid", latestAwardAddress.mcs_ptcpntRecipID);
                                PopulateField(thisNewEntity, "va_participantvetid", latestAwardAddress.mcs_ptcpntVetID);
                                PopulateField(thisNewEntity, "va_awardtypecode", latestAwardAddress.mcs_awardTypeCode);


                                PopulateField(thisNewEntity, "va_paystatus", latestAwardAddress.mcs_statusTypeCode);
                                PopulateField(thisNewEntity, "va_depositaccountnumber", latestAwardAddress.mcs_directDepositAccountNumber);
                                PopulateField(thisNewEntity, "va_routingnumber", latestAwardAddress.mcs_directDepositRoutingNumber);
                                if (thisNewEntity.Contains("va_routingnumber"))
                                {
                                    request.RoutingNumber = thisNewEntity["va_routingnumber"].ToString();
                                }

                                PopulateDateField(thisNewEntity, "va_depositbegindate", latestAwardAddress.mcs_directDepositBeginDate);
                                PopulateDateField(thisNewEntity, "va_depositenddate", latestAwardAddress.mcs_directDepositEndDate);
                                PopulateDateField(thisNewEntity, "va_recurringpayableeffectivedate", latestAwardAddress.mcs_recurringPayableEffectiveDate);
                                if (latestAwardAddress.mcs_directDepositAccountTypeName != null)
                                {
                                    if (latestAwardAddress.mcs_directDepositAccountTypeName.StartsWith("C"))
                                    {
                                        thisNewEntity["va_depositaccounttype"] = new OptionSetValue(953850000);
                                    }
                                    else
                                    {

                                        thisNewEntity["va_depositaccounttype"] = new OptionSetValue(953850001);
                                    }
                                }

                                if (!string.IsNullOrEmpty(latestAwardAddress.mcs_netRateAmount))
                                {
                                    thisNewEntity["va_grossamount"] = new Money(Convert.ToDecimal(latestAwardAddress.mcs_netRateAmount));
                                    thisNewEntity["va_netrateamount"] = new Money(Convert.ToDecimal(latestAwardAddress.mcs_netRateAmount));
                                }
                                else
                                {
                                    thisNewEntity["va_grossamount"] = new Money(0);
                                    thisNewEntity["va_netrateamount"] = new Money(0);
                                }
                                // Replaced: VIMTfawdaddmailingAddressclmsInfo = VEISfawdaddmailingAddressInfo
                                if (latestAwardAddress.VEISfawdaddmailingAddressInfo != null)
                                {
                                    #region set mailing address fields

                                    PopulateField(thisNewEntity, "va_mailingaddress1", latestAwardAddress.VEISfawdaddmailingAddressInfo.mcs_addressLine1);
                                    PopulateField(thisNewEntity, "va_mailingaddress2", latestAwardAddress.VEISfawdaddmailingAddressInfo.mcs_addressLine2);
                                    PopulateField(thisNewEntity, "va_mailingaddress3", latestAwardAddress.VEISfawdaddmailingAddressInfo.mcs_addressLine3);
                                    PopulateField(thisNewEntity, "va_mailingaddresszipcode", latestAwardAddress.VEISfawdaddmailingAddressInfo.mcs_zipPrefix);
                                    PopulateField(thisNewEntity, "va_mailingcity", latestAwardAddress.VEISfawdaddmailingAddressInfo.mcs_city);
                                    PopulateField(thisNewEntity, "va_mailingstate", latestAwardAddress.VEISfawdaddmailingAddressInfo.mcs_state);
                                    PopulateField(thisNewEntity, "va_mailingcountry", latestAwardAddress.VEISfawdaddmailingAddressInfo.mcs_countryTypeName);
                                    PopulateField(thisNewEntity, "va_provincename", latestAwardAddress.VEISfawdaddmailingAddressInfo.mcs_province);
                                    PopulateField(thisNewEntity, "va_territoryname", latestAwardAddress.VEISfawdaddmailingAddressInfo.mcs_territory);
                                    PopulateField(thisNewEntity, "va_mailingforeignpostalcode", latestAwardAddress.VEISfawdaddmailingAddressInfo.mcs_forignPostalCode);

                                    //PopulateField(thisNewEntity, "va_mailingmilitarypostaltypecode", latestAwardAddress.VIMTfawdaddmailingAddressclmsInfo.mcs_militaryPostalTypeCode);
                                    //PopulateField(thisNewEntity, "va_mailingmilitarypostofficetypecode", latestAwardAddress.VIMTfawdaddmailingAddressclmsInfo.mcs_militaryPostOfficeTypeCode);

                                    PopulateField(thisNewEntity, "udo_postaltypecode", latestAwardAddress.VEISfawdaddmailingAddressInfo.mcs_militaryPostalTypeCode);
                                    PopulateField(thisNewEntity, "udo_postalofficetypecode", latestAwardAddress.VEISfawdaddmailingAddressInfo.mcs_militaryPostOfficeTypeCode);
                                    PopulateDateField(thisNewEntity, "va_mailingeffectivedate", latestAwardAddress.VEISfawdaddmailingAddressInfo.mcs_effectiveDate);
                                    if (thisNewEntity.Attributes.Contains("udo_postalofficetypecode"))
                                    {
                                        PopulateField(thisNewEntity, "udo_milzipcode", latestAwardAddress.VEISfawdaddmailingAddressInfo.mcs_zipPrefix);
                                    }

                                    #endregion
                                }
                                if (latestAwardAddress.VEISfawdaddpaymentAddressInfo != null)
                                {
                                    #region set payment address fields
                                    PopulateDateField(thisNewEntity, "va_paymenteffectivedate", latestAwardAddress.VEISfawdaddpaymentAddressInfo.mcs_effectiveDate);
                                    PopulateField(thisNewEntity, "va_paymentaddress1", latestAwardAddress.VEISfawdaddpaymentAddressInfo.mcs_addressLine1);
                                    PopulateField(thisNewEntity, "va_paymentaddress2", latestAwardAddress.VEISfawdaddpaymentAddressInfo.mcs_addressLine2);
                                    PopulateField(thisNewEntity, "va_paymentaddress3", latestAwardAddress.VEISfawdaddpaymentAddressInfo.mcs_addressLine3);
                                    PopulateField(thisNewEntity, "va_paymentcity", latestAwardAddress.VEISfawdaddpaymentAddressInfo.mcs_city);
                                    PopulateField(thisNewEntity, "va_paymentstate", latestAwardAddress.VEISfawdaddpaymentAddressInfo.mcs_state);
                                    PopulateField(thisNewEntity, "va_paymentzipcode", latestAwardAddress.VEISfawdaddpaymentAddressInfo.mcs_zipPrefix);
                                    PopulateField(thisNewEntity, "va_paymentforeignpostalcode", latestAwardAddress.VEISfawdaddpaymentAddressInfo.mcs_forignPostalCode);
                                    PopulateField(thisNewEntity, "udo_paypostaltypecode", latestAwardAddress.VEISfawdaddpaymentAddressInfo.mcs_militaryPostalTypeCode);
                                    PopulateField(thisNewEntity, "udo_paypostofficetypecode", latestAwardAddress.VEISfawdaddpaymentAddressInfo.mcs_militaryPostOfficeTypeCode);
                                    PopulateDateField(thisNewEntity, "va_paymenteffectivedate", latestAwardAddress.VEISfawdaddpaymentAddressInfo.mcs_effectiveDate);
                                    if (thisNewEntity.Attributes.Contains("udo_paypostofficetypecode"))
                                    {
                                        PopulateField(thisNewEntity, "udo_paymilzip", latestAwardAddress.VEISfawdaddpaymentAddressInfo.mcs_zipPrefix);
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                    else
                    {
                        // no award.. no recip

                    }
                }

                #endregion

                #region findBankNameByRoutingNumber
                if (!string.IsNullOrEmpty(request.RoutingNumber) && ValidABARoutingNumber(request.RoutingNumber))
                {
                    var findBankNameByRoutingNumberRequest = new VEISbyRoutingTransitionNumberfindBankNameByRoutngTrnsitNbrRequest
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
                        mcs_routngtrnsitnbr = request.RoutingNumber,
                        LegacyServiceHeaderInfo = new LegacyHeaderInfo
                        {
                            ApplicationName = request.LegacyServiceHeaderInfo.ApplicationName,
                            ClientMachine = request.LegacyServiceHeaderInfo.ClientMachine,
                            LoginName = request.LegacyServiceHeaderInfo.LoginName,
                            StationNumber = request.LegacyServiceHeaderInfo.StationNumber
                        }
                    };
                    // REM: Invoke VEIS Endpoint
                    // var findBankNameByRoutingNumberResponse = findBankNameByRoutingNumberRequest.SendReceive<VIMT.DdeftWebService.Messages.VIMTbyRoutingTransitionNumberfindBankNameByRoutngTrnsitNbrResponse>(MessageProcessType.Local);
                    //var findBankNameByRoutingNumberStopWatch = new Stopwatch();
                    //findBankNameByRoutingNumberStopWatch.Start();
                    var findBankNameByRoutingNumberResponse = WebApiUtility.SendReceive<VEISbyRoutingTransitionNumberfindBankNameByRoutngTrnsitNbrResponse>(findBankNameByRoutingNumberRequest, WebApiType.VEIS);
                    //findBankNameByRoutingNumberStopWatch.Stop();
                    //aiLogger.LogWebResponseDuration("VEISbyRoutingTransitionNumberfindBankNameByRoutngTrnsitNbr", method, "050", findBankNameByRoutingNumberStopWatch.Elapsed.TotalMilliseconds);

                    if (findBankNameByRoutingNumberRequest.LogSoap || findBankNameByRoutingNumberResponse.ExceptionOccurred)
                    {
                        if (findBankNameByRoutingNumberResponse.SerializedSOAPRequest != null || findBankNameByRoutingNumberResponse.SerializedSOAPResponse != null)
                        {
                            var requestResponse = findBankNameByRoutingNumberResponse.SerializedSOAPRequest + findBankNameByRoutingNumberResponse.SerializedSOAPResponse;
                            LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISbyRoutingTransitionNumberfindBankNameByRoutngTrnsitNbrRequest Request/Response {requestResponse}", true);
                        }
                    }

                    progressString = "After findBankNameByRoutingNumber EC Call";

                    if (findBankNameByRoutingNumberResponse.ExceptionOccurred)
                    {
                        caddExceptions.Add(new UDOException()
                        {
                            ExceptionMessage = findBankNameByRoutingNumberResponse.ExceptionMessage,
                            ExceptionOccurred = true,
                            ExceptionCategory = "Ddeft"
                        });

                        var udoException = new ApplicationException(findBankNameByRoutingNumberResponse.ExceptionMessage + " while calling findBankNameByRoutingNumber EC");
                        throw udoException;
                    }

                    // Replaced: VIMTbyRoutingTransitionNumberInfo = VEISbyRoutingTransitionNumberreturnInfo
                    if (findBankNameByRoutingNumberResponse.VEISbyRoutingTransitionNumberreturnInfo != null)
                    {
                        var BankName = findBankNameByRoutingNumberResponse.VEISbyRoutingTransitionNumberreturnInfo;

                        if (!string.IsNullOrEmpty(BankName.mcs_bankName))
                        {
                            thisNewEntity["va_bankname"] = BankName.mcs_bankName;
                        }
                    }
                }
                else
                {
                    LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, "UDOInitiateCADDProcessor Processor, Progess:" + progressString, "No routing number", request.Debug);
                }
                #endregion

                #region findCommerica
                var findCommericaRequest = new VEISroutingTransitionNumberfindComericaRoutngTrnsitNbrRequest
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

                //var findCommericaStopWatch = new Stopwatch();
                //findCommericaStopWatch.Start();
                var findCommericaResponse = WebApiUtility.SendReceive<VEISroutingTransitionNumberfindComericaRoutngTrnsitNbrResponse>(findCommericaRequest, WebApiType.VEIS);
                //findCommericaStopWatch.Stop();
                //aiLogger.LogWebResponseDuration("VEISroutingTransitionNumberfindComericaRoutngTrnsitNbr", method, "060", findCommericaStopWatch.Elapsed.TotalMilliseconds);

                if (findCommericaRequest.LogSoap || findCommericaResponse.ExceptionOccurred)
                {
                    if (findCommericaResponse.SerializedSOAPRequest != null || findCommericaResponse.SerializedSOAPResponse != null)
                    {
                        var requestResponse = findCommericaResponse.SerializedSOAPRequest + findCommericaResponse.SerializedSOAPResponse;
                        LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, MethodInfo.GetThisMethod().Method, $"VEISroutingTransitionNumberfindComericaRoutngTrnsitNbrRequest Request/Response {requestResponse}", true);
                    }
                }

                progressString = "After findComericaRoutingTrsitNbr EC Call";

                if (findCommericaResponse.ExceptionOccurred)
                {
                    caddExceptions.Add(new UDOException()
                    {
                        ExceptionMessage = findCommericaResponse.ExceptionMessage,
                        ExceptionOccurred = true,
                        ExceptionCategory = "Ddeft"
                    });

                    var udoException = new ApplicationException(findCommericaResponse.ExceptionMessage + " while calling findComericaRoutingTrsitNbr EC");
                    throw udoException;
                }

                // TODO: VEIS Dependency missing comericaRoutngTrnsitNbr
                var comericaRoutingNumber = findCommericaResponse.comericaRoutngTrnsitNbr;
                PopulateField(thisNewEntity, "udo_crn", comericaRoutingNumber);
                #endregion

                #region Comerica Protection
                if (!String.IsNullOrEmpty(comericaRoutingNumber) && !String.IsNullOrEmpty(request.RoutingNumber) &&
                    String.Equals(request.RoutingNumber, comericaRoutingNumber, StringComparison.InvariantCultureIgnoreCase))
                {
                    // Kirk removing masking
                    // thisNewEntity["va_depositaccountnumber"] = Tools.Mask(thisNewEntity["va_depositaccountnumber"].ToString());

                    //mask all but right 4 for comerica routing numbers.
                    //thisNewEntity["va_routingnumber"] = Tools.Mask(request.RoutingNumber, -4);
                }
                #endregion

                #region Verify Address fields
                PopulateFieldfromEntity(thisNewEntity, "va_verifyaddress1", thisNewEntity, "va_mailingaddress1");
                PopulateFieldfromEntity(thisNewEntity, "va_verifyaddress2", thisNewEntity, "va_mailingaddress2");
                PopulateFieldfromEntity(thisNewEntity, "va_verifyaddress3", thisNewEntity, "va_mailingaddress3");
                PopulateFieldfromEntity(thisNewEntity, "va_verifyzipcode", thisNewEntity, "va_mailingaddresszipcode");
                PopulateFieldfromEntity(thisNewEntity, "va_verifycity", thisNewEntity, "va_mailingcity");
                PopulateFieldfromEntity(thisNewEntity, "va_verifystate", thisNewEntity, "va_mailingstate");
                PopulateFieldfromEntity(thisNewEntity, "va_verifycountry", thisNewEntity, "va_mailingcountry");
                #endregion

                #region Copy Related Entities
                if (request.RelatedEntities != null)
                {
                    foreach (var relatedItem in request.RelatedEntities)
                    {
                        thisNewEntity[relatedItem.RelatedEntityFieldName] = new EntityReference(relatedItem.RelatedEntityName, relatedItem.RelatedEntityId);
                    }
                }
                #endregion

                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, $"{progressString}  About to create Cadd", request.Debug);

                //OrgServiceProxy.CallerId = Guid.Empty;
                //organizationServiceProxyOptions = new OrganizationServiceProxyOptions();
                //organizationServiceProxyOptions.CallerId = Guid.Empty;
                response.CADDId = OrgServiceProxy.Create(TruncateHelper.TruncateFields(request.MessageId, thisNewEntity, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                LogHelper.LogDebug(request.MessageId, request.OrganizationName, request.UserId, method, $"{progressString}  CADD Created", request.Debug);

                //added to generated code
                if (request.va_bankaccountId != System.Guid.Empty)
                {
                    var parent = new Entity();
                    parent.Id = request.va_bankaccountId;
                    parent.LogicalName = "va_bankaccount";
                    parent["va_bankaccountcomplete"] = true;
                    //OrgServiceProxy.CallerId = Guid.Empty;
                    //organizationServiceProxyOptions.CallerId = Guid.Empty;
                    OrgServiceProxy.Update(TruncateHelper.TruncateFields(request.MessageId, parent, request.OrganizationName, request.UserId, request.LogTiming, OrgServiceProxy));
                }

                if (!response.ExceptionOccurred && caddExceptions.Count > 0)
                {
                    response.ExceptionOccurred = true;
                    response.ExceptionMessage = "Internal Exceptions Occurred";
                    response.InnerExceptions = caddExceptions.ToArray();
                }

                return response;
            }
            catch (Exception ExecutionException)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName,
                    request.MessageId, "UDOInitiateCADDProcessor Processor, Progess:" + progressString, ExecutionException);

                aiLogger.LogException(ExecutionException, "999");

                response.ExceptionMessage = ExecutionException.Message;
                response.ExceptionOccurred = true;

                if (caddExceptions.Count > 0) response.InnerExceptions = caddExceptions.ToArray();
                return response;
            }
        }

        private bool ValidABARoutingNumber(string routingnumber)
        {
            int[] d = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            if (routingnumber.Length != 9) return false;

            int num = 0;
            if (!Int32.TryParse(routingnumber, out num)) return false;

            var i = 8; // max position
            while (num > 0)
            {
                d[i] = num % 10; // get digit
                num = num / 10; // remove digit
                i--; //decrease position;
            }

            long checksum = 3 * (d[0] + d[3] + d[6]);
            checksum += 7 * (d[1] + d[4] + d[7]);
            checksum += d[2] + d[5] + d[8];

            return (checksum % 10) == 0;
        }

        private string FormatTelephone(string telephoneNumber)
        {
            var Phone = telephoneNumber;
            var ext = "";
            var result = "";

            if (0 != Phone.IndexOf('+'))
            {
                if (1 < Phone.LastIndexOf('x'))
                {
                    ext = Phone.Substring(Phone.LastIndexOf('x'));
                    Phone = Phone.Substring(0, Phone.LastIndexOf('x'));
                }
            }
            //Phone = Phone.Replace(/[^\d]/gi, "");
            result = Phone;
            if (7 == Phone.Length)
            {
                result = Phone.Substring(0, 3) + "-" + Phone.Substring(3);
            }
            if (10 == Phone.Length)
            {
                result = "(" + Phone.Substring(0, 3) + ") " + Phone.Substring(3, 3) + "-" + Phone.Substring(6);
            }
            if (0 < ext.Length)
            {
                result = result + " " + ext;
            }
            return result;

        }

        private void PopulateField(Entity thisNewEntity, string fieldName, string fieldValue)
        {
            if (!string.IsNullOrEmpty(fieldValue))
            {
                thisNewEntity[fieldName] = fieldValue;
            }

        }

        private void PopulateFieldfromEntity(Entity thisNewEntity, string fieldName, Entity sourceEntity, string fieldValue)
        {
            if (sourceEntity.Attributes.Contains(fieldValue))
            {
                thisNewEntity[fieldName] = sourceEntity[fieldValue];
            }

        }

        private void PopulateDateField(Entity thisNewEntity, string fieldName, string fieldValue)
        {
            if (!string.IsNullOrEmpty(fieldValue))
            {
                DateTime newDateTime;
                if (DateTime.TryParse(fieldValue, out newDateTime))
                {
                    thisNewEntity[fieldName] = newDateTime;
                }
            }
        }

    }
}
