var ws = ws || {};
ws.benefitClaim = {};
// Function will be called by form onLoad event
ws.benefitClaim.initalize = function () {
    //=====================================================================================================
    // START BenefitClaimWebService
    //=====================================================================================================
    var benefitClaimWebService = function (context) {
        this.context = context;
        this.webserviceRequestUrl = WebServiceURLRoot + 'BenefitClaimServiceBean/BenefitClaimWebService';
        this.prefix = 'ser';
        this.prefixUrl = 'http://services.share.benefits.vba.va.gov/';
    };
    benefitClaimWebService.prototype = new webservice;
    benefitClaimWebService.prototype.constructor = benefitClaimWebService;
    window.benefitClaimWebService = benefitClaimWebService;
    //=====================================================================================================
    // START Individual BenefitClaimWebService Methods
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START findBenefitClaim
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:findBenefitClaim>
    //         <!--Optional:-->
    //         <fileNumber>?</fileNumber>
    //         <!--Optional:-->         
    //      </ser:findBenefitClaim>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    var findBenefitClaim = function (context) {
        this.context = context;

        this.serviceName = 'findBenefitClaim';
        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'BenefitClaimService.findBenefitClaim';
        this.wsMessage.serviceName = 'findBenefitClaim';
        this.wsMessage.friendlyServiceName = 'Benefit Claim';
        this.responseFieldSchema = 'va_benefitclaimresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['fileNumber'] = null;
    };
    findBenefitClaim.prototype = new benefitClaimWebService;
    findBenefitClaim.prototype.constructor = findBenefitClaim;
    findBenefitClaim.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

        var innerXml;
        var fileNumber = this.context.parameters['fileNumber'];

        if (fileNumber && fileNumber != '') {
            innerXml = '<ser:findBenefitClaim><fileNumber>' + fileNumber
                + '</fileNumber></ser:findBenefitClaim>';
        } else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'There must be a file number present for the request';
            return null;
        }

        return innerXml;
    };
    window.findBenefitClaim = findBenefitClaim;
    //END findBenefitClaim
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START findBenefitClaimDetail
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:findBenefitClaimDetail>
    //         <!--Optional:-->
    //         <benefitClaimId>?</benefitClaimId>
    //         <!--Optional:-->
    //      </ser:findBenefitClaimDetail>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    //=====================================================================================================
    var findBenefitClaimDetail = function (context) {
        this.context = context;

        this.serviceName = 'findBenefitClaimDetail';
        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'BenefitClaimService.findBenefitClaimDetail';
        this.wsMessage.serviceName = 'findBenefitClaimDetail';
        this.wsMessage.friendlyServiceName = 'Benefit Claim Detail';
        this.responseFieldSchema = 'va_findbenefitdetailresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['claimId'] = null;
        this.ignoreRequiredParMissingWarning = true;
    };
    findBenefitClaimDetail.prototype = new benefitClaimWebService;
    findBenefitClaimDetail.prototype.constructor = findBenefitClaimDetail;
    findBenefitClaimDetail.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

        var innerXml;
        var claimId = this.context.parameters['claimId'];

        if (claimId && claimId != '') {
            innerXml = '<ser:findBenefitClaimDetail><benefitClaimId>' + claimId
                + '</benefitClaimId></ser:findBenefitClaimDetail>';
        } else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'There must be a benefit claim id number present for the request';
            return null;
        }

        return innerXml;
    };
    window.findBenefitClaimDetail = findBenefitClaimDetail;
    // END findBenefitClaimDetail
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START updateBenefitClaimAddress
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:updateBenefitClaimAddress>
    //         <!--Optional:-->
    //         <cInput>
    //            <fileNumber>?</fileNumber>
    //            <payeeCode>00</payeeCode>
    //            <!--Optional:-->
    //            <transId>?</transId>
    //            <!--Optional:-->
    //            <johnsCnarData>?</johnsCnarData>
    //            <!--Optional:-->
    //            <corpAddressLine1>?</corpAddressLine1>
    //            <!--Optional:-->
    //            <corpAddressLine2>?</corpAddressLine2>
    //            <!--Optional:-->
    //            <corpAddressLine3>?</corpAddressLine3>
    //            <corpCity>?</corpCity>
    //            <corpState>?</corpState>
    //            <corpCountry>?</corpCountry>
    //            <corpZipCode>?</corpZipCode>
    //            <!--Optional:-->
    //            <corpForeignMailCode>?</corpForeignMailCode>
    //            <payeeLastName>?</payeeLastName>
    //            <payeeFirstName>?</payeeFirstName>
    //            <!--Optional:-->
    //            <payeeMiddleName>?</payeeMiddleName>
    //            <!--Optional:-->
    //            <payeeSuffixName>?</payeeSuffixName>
    //            <!--Optional:-->
    //            <payeeOrgName>?</payeeOrgName>
    //            <!--Optional:-->
    //            <payeeOrgType>?</payeeOrgType>
    //            <!--Optional:-->
    //            <payeeOrgTitle>?</payeeOrgTitle>
    //            <!--Optional:-->
    //            <fiduciary1LastName>?</fiduciary1LastName>
    //            <!--Optional:-->
    //            <fiduciary1FirstName>?</fiduciary1FirstName>
    //            <!--Optional:-->
    //            <fiduciary1MiddleName>?</fiduciary1MiddleName>
    //            <!--Optional:-->
    //            <fiduciary1SuffixName>?</fiduciary1SuffixName>
    //            <!--Optional:-->
    //            <fiduciary2LastName>?</fiduciary2LastName>
    //            <!--Optional:-->
    //            <fiduciary2FirstName>?</fiduciary2FirstName>
    //            <!--Optional:-->
    //            <fiduciary2MiddleName>?</fiduciary2MiddleName>
    //            <!--Optional:-->
    //            <fiduciary2SuffixName>?</fiduciary2SuffixName>
    //            <!--Optional:-->
    //            <fiduciaryOrgName>?</fiduciaryOrgName>
    //            <!--Optional:-->
    //            <fiduciaryOrgTitle>?</fiduciaryOrgTitle>
    //            <!--Optional:-->
    //            <fiduciaryOrgType>?</fiduciaryOrgType>
    //            <!--Optional:-->
    //            <fiduciaryPtcpntRelationship>?</fiduciaryPtcpntRelationship>
    //            <!--Optional:-->
    //            <fiduciaryPrepPhraseType>?</fiduciaryPrepPhraseType>
    //            <!--Optional:-->
    //            <payeeSalutation>?</payeeSalutation>
    //            <!--Optional:-->
    //            <cfidNameActionInd>?</cfidNameActionInd>
    //            <!--Optional:-->
    //            <mltyPostalTypeCd>?</mltyPostalTypeCd>
    //            <!--Optional:-->
    //            <mltyPostOfficeTypeCd>?</mltyPostOfficeTypeCd>
    //            <!--Optional:-->
    //            <provinceName>?</provinceName>
    //            <!--Optional:-->
    //            <territoryName>?</territoryName>
    //            <!--Optional:-->
    //            <corpPhoneTypeName1>?</corpPhoneTypeName1>
    //            <!--Optional:-->
    //            <corpPhoneNumber1>?</corpPhoneNumber1>
    //            <!--Optional:-->
    //            <corpAreaNumber1>?</corpAreaNumber1>
    //            <!--Optional:-->
    //            <corpDeletePhoneNumberInd1>?</corpDeletePhoneNumberInd1>
    //            <!--Optional:-->
    //            <corpPhoneTypeName2>?</corpPhoneTypeName2>
    //            <!--Optional:-->
    //            <corpPhoneNumber2>?</corpPhoneNumber2>
    //            <!--Optional:-->
    //            <corpAreaNumber2>?</corpAreaNumber2>
    //            <!--Optional:-->
    //            <corpDeletePhoneNumberInd2>?</corpDeletePhoneNumberInd2>
    //            <!--Optional:-->
    //            <competencyDecisionNumber>?</competencyDecisionNumber>
    //            <!--Optional:-->
    //            <cpPaymentAddressLine1>?</cpPaymentAddressLine1>
    //            <!--Optional:-->
    //            <cpPaymentAddressLine2>?</cpPaymentAddressLine2>
    //            <!--Optional:-->
    //            <cpPaymentAddressLine3>?</cpPaymentAddressLine3>
    //            <!--Optional:-->
    //            <cpPaymentCity>?</cpPaymentCity>
    //            <!--Optional:-->
    //            <cpPaymentZipCode>?</cpPaymentZipCode>
    //            <!--Optional:-->
    //            <cpPaymentState>?</cpPaymentState>
    //            <!--Optional:-->
    //            <cpPaymentCountryCode>?</cpPaymentCountryCode>
    //            <!--Optional:-->
    //            <cpPaymentForeignZipCode>?</cpPaymentForeignZipCode>
    //            <!--Optional:-->
    //            <cpPaymentPostalTypeCode>?</cpPaymentPostalTypeCode>
    //            <!--Optional:-->
    //            <cpPaymentPoTypeCode>?</cpPaymentPoTypeCode>
    //            <!--Optional:-->
    //            <eftRoutingNumber>?</eftRoutingNumber>
    //            <!--Optional:-->
    //            <eftAccountType>?</eftAccountType>
    //            <!--Optional:-->
    //            <eftAccountNumber>?</eftAccountNumber>
    //            <!--Optional:-->
    //            <terminateAddressInd>?</terminateAddressInd>
    //            <!--Optional:-->
    //            <treasuryMailingAddress1>?</treasuryMailingAddress1>
    //            <!--Optional:-->
    //            <treasuryMailingAddress2>?</treasuryMailingAddress2>
    //            <!--Optional:-->
    //            <treasuryMailingAddress3>?</treasuryMailingAddress3>
    //            <!--Optional:-->
    //            <treasuryMailingAddress4>?</treasuryMailingAddress4>
    //            <!--Optional:-->
    //            <treasuryMailingAddress5>?</treasuryMailingAddress5>
    //            <!--Optional:-->
    //            <treasuryMailingAddress6>?</treasuryMailingAddress6>
    //            <!--Optional:-->
    //            <treasuryPaymentAddress1>?</treasuryPaymentAddress1>
    //            <!--Optional:-->
    //            <treasuryPaymentAddress2>?</treasuryPaymentAddress2>
    //            <!--Optional:-->
    //            <treasuryPaymentAddress3>?</treasuryPaymentAddress3>
    //            <!--Optional:-->
    //            <treasuryPaymentAddress4>?</treasuryPaymentAddress4>
    //            <!--Optional:-->
    //            <treasuryPaymentAddress5>?</treasuryPaymentAddress5>
    //            <!--Optional:-->
    //            <treasuryPaymentAddress6>?</treasuryPaymentAddress6>
    //            <!--Optional:-->
    //            <ptcpntIdPayee>?</ptcpntIdPayee>
    //            <!--Optional:-->
    //            <ptcpntIdFiduciary1>?</ptcpntIdFiduciary1>
    //            <!--Optional:-->
    //            <ptcpntIdFiduciary2>?</ptcpntIdFiduciary2>
    //            <!--Optional:-->
    //            <ptcpntIdVet>?</ptcpntIdVet>
    //            <!--Optional:-->
    //            <ptcpntIdBene>?</ptcpntIdBene>
    //            <!--Optional:-->
    //            <ptcpntIdRecip>?</ptcpntIdRecip>
    //            <!--Optional:-->
    //            <awardTypeCd>?</awardTypeCd>
    //            <!--Optional:-->
    //            <competencyDecisionTypeCd>?</competencyDecisionTypeCd>
    //            <!--Optional:-->
    //            <fiduciaryDecisionCategoryType>?</fiduciaryDecisionCategoryType>
    //            <!--Optional:-->
    //            <tempCustodianInd>?</tempCustodianInd>
    //            <!--Optional:-->
    //            <fiduciaryFileCorporateLocation>?</fiduciaryFileCorporateLocation>
    //            <!--Optional:-->
    //            <transBypassCorporateInd>?</transBypassCorporateInd>
    //            <!--Optional:-->
    //            <corpZipPlusFour>?</corpZipPlusFour>
    //            <!--Optional:-->
    //            <group1ValidatedInd>?</group1ValidatedInd>
    //            <!--Optional:-->
    //            <cpPaymentZipPlusFour>?</cpPaymentZipPlusFour>
    //            <!--Optional:-->
    //            <cpPaymentGroup1ValidatedInd>?</cpPaymentGroup1ValidatedInd>
    //            <!--Optional:-->
    //            <emailAddress>?</emailAddress>
    //            <!--Optional:-->
    //            <addressType>?</addressType>
    //         </cInput>
    //         <!--Optional:-->
    //      </ser:updateBenefitClaimAddress>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    //=====================================================================================================
    var updateBenefitClaimAddress = function (context) {
        this.context = context;

        this.serviceName = 'updateBenefitClaimAddress';
        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'BenefitClaimService.updateBenefitClaimAddress';
        this.wsMessage.serviceName = 'updateBenefitClaimAddress';
        this.wsMessage.friendlyServiceName = 'Update Benefit Claim Address';
        this.responseFieldSchema = 'va_updateaddressresponse';
        this.responseTimestamp = 'va_webserviceresponse';
    };
    updateBenefitClaimAddress.prototype = new benefitClaimWebService;
    updateBenefitClaimAddress.prototype.constructor = updateBenefitClaimAddress;
    updateBenefitClaimAddress.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

        var innerXml = '';
        //    var fileNumber = this.context.fileNumber;
        //    var payeeCode = this.context.payeeCode;


        //    if (claimId == null) {
        //        this.wsMessage.errorFlag = true;
        //        this.wsMessage.description = 'There must be a benefit claim id number present for the request';
        //        return null;
        //    }

        var eftAccountType;

        //todo: replace if with Xrm.Page.getAttribute().getText()

        if (Xrm.Page.getAttribute('va_depositaccounttype').getValue() == '953850000') {
            eftAccountType = 'Checking';
        } else if (Xrm.Page.getAttribute('va_depositaccounttype').getValue() == '953850001') {
            eftAccountType = 'Savings';
        } else {
            eftAccountType = '';
        }

        var accountnumber = NN(Xrm.Page.getAttribute('va_depositaccountnumber').getValue());
        var routingnumber = NN(Xrm.Page.getAttribute('va_routingnumber').getValue());

        //if (NN(Xrm.Page.getAttribute('va_ca').getValue())) {
        //    accountnumber = NN(Xrm.Page.getAttribute('va_ca_a').getValue());
        //    routingnumber = NN(Xrm.Page.getAttribute('va_ca_r').getValue());
        //}
        if (NN(globalcadd.ca)) {
            accountnumber = NN(globalcadd.ca_a);
            routingnumber = NN(globalcadd.ca_r);
        }

        innerXml =
            '<ser:updateBenefitClaimAddress><cInput>' +
                '<fileNumber>' + NN(Xrm.Page.getAttribute('va_filenumber').getValue()) + '</fileNumber>' +
                '<payeeCode>' + NN(Xrm.Page.getAttribute('va_payeetypecode').getValue()) + '</payeeCode>' +
                '<payeeLastName>' + NN(Xrm.Page.getAttribute('va_originallastname').getValue()) + '</payeeLastName>' +
                '<payeeFirstName>' + NN(Xrm.Page.getAttribute('va_originalfirstname').getValue()) + '</payeeFirstName>' +
                '<payeeMiddleName>' + NN(Xrm.Page.getAttribute('va_originalmiddlename').getValue()) + '</payeeMiddleName>' +
                '<payeeSuffixName>' + NN(Xrm.Page.getAttribute('va_originalsuffix').getValue()) + '</payeeSuffixName>' +
                '<corpPhoneTypeName1>' + NN(Xrm.Page.getAttribute('va_phone1type').getText()) + '</corpPhoneTypeName1>' +
                '<corpPhoneNumber1>' + NN(Xrm.Page.getAttribute('va_phone1').getValue()) + '</corpPhoneNumber1>' +
                '<corpAreaNumber1>' + NN(Xrm.Page.getAttribute('va_area1').getValue()) + '</corpAreaNumber1>' +
                '<corpPhoneTypeName2>' + NN(Xrm.Page.getAttribute('va_phone2type').getText()) + '</corpPhoneTypeName2>' +
                '<corpPhoneNumber2>' + NN(Xrm.Page.getAttribute('va_phone2').getValue()) + '</corpPhoneNumber2>' +
                '<corpAreaNumber2>' + NN(Xrm.Page.getAttribute('va_area2').getValue()) + '</corpAreaNumber2>' +
                '<corpAddressLine1>' + NN(Xrm.Page.getAttribute('va_mailingaddress1').getValue()) + '</corpAddressLine1>' +
                '<corpAddressLine2>' + NN(Xrm.Page.getAttribute('va_mailingaddress2').getValue()) + '</corpAddressLine2>' +
                '<corpAddressLine3>' + NN(Xrm.Page.getAttribute('va_mailingaddress3').getValue()) + '</corpAddressLine3>' +
                '<corpCity>' + NN(Xrm.Page.getAttribute('va_mailingcity').getValue()) + '</corpCity>' +
                '<corpState>' + NN(Xrm.Page.getAttribute('va_mailingstate').getValue()) + '</corpState>' +
                '<corpCountry>' + NN(Xrm.Page.getAttribute('va_mailingcountry').getValue()) + '</corpCountry>' +
                '<corpZipCode>' + NN(Xrm.Page.getAttribute('va_mailingaddresszipcode').getValue()) + '</corpZipCode>' +
                '<cpPaymentAddressLine1>' + NN(Xrm.Page.getAttribute('va_paymentaddress1').getValue()) + '</cpPaymentAddressLine1>' +
                '<cpPaymentAddressLine2>' + NN(Xrm.Page.getAttribute('va_paymentaddress2').getValue()) + '</cpPaymentAddressLine2>' +
                '<cpPaymentAddressLine3>' + NN(Xrm.Page.getAttribute('va_paymentaddress3').getValue()) + '</cpPaymentAddressLine3>' +
                '<cpPaymentCity>' + NN(Xrm.Page.getAttribute('va_paymentcity').getValue()) + '</cpPaymentCity>' +
                '<cpPaymentZipCode>' + NN(Xrm.Page.getAttribute('va_paymentzipcode').getValue()) + '</cpPaymentZipCode>' +
                '<cpPaymentState>' + NN(Xrm.Page.getAttribute('va_paymentstate').getValue()) + '</cpPaymentState>' +
                '<cpPaymentCountryCode>' + NN(Xrm.Page.getAttribute('va_paymentcountry').getValue()) + '</cpPaymentCountryCode>' +
                '<eftRoutingNumber>' + routingnumber + '</eftRoutingNumber>' +
                '<eftAccountNumber>' + accountnumber + '</eftAccountNumber>';

        if (eftAccountType != '') {
            innerXml += '<eftAccountType>' + NN(eftAccountType) + '</eftAccountType>';
        }

        var addressType = ''; // per Cory, DOmestic - blank string
        var overSeasMailingPostCode = '', overSeasPaymentPostCode = '', intMailingZip = '', intPaymentZip = '';
        switch (Xrm.Page.getAttribute('va_addresstype').getValue()) {
            //International
            case 953850001:
                addressType = 'INT';
                intMailingZip = '<corpForeignMailCode>' +
                    (Xrm.Page.getAttribute('va_mailingforeignpostalcode').getValue() ? Xrm.Page.getAttribute('va_mailingforeignpostalcode').getValue() : '') +
                    '</corpForeignMailCode>';
                intPaymentZip = '<cpPaymentForeignZipCode>' +
                    (Xrm.Page.getAttribute('va_paymentforeignpostalcode').getValue() ? Xrm.Page.getAttribute('va_paymentforeignpostalcode').getValue() : '') +
                    '</cpPaymentForeignZipCode>';

                innerXml += (
                    '<provinceName>' + (Xrm.Page.getAttribute('va_provincename').getValue() ? Xrm.Page.getAttribute('va_provincename').getValue() : '') + '</provinceName>' +
                        '<territoryName>' + (Xrm.Page.getAttribute('va_territoryname').getValue() ? Xrm.Page.getAttribute('va_territoryname').getValue() : '') + '</territoryName>');
                break;
                //Overseas Military
            case 953850002:
                addressType = 'OVR';
                overSeasMailingPostCode = '<mltyPostalTypeCd>' +
                    (Xrm.Page.getAttribute('va_mailingmilitarypostaltypecode').getText() ? Xrm.Page.getAttribute('va_mailingmilitarypostaltypecode').getText() : '') +
                    '</mltyPostalTypeCd>' +
                    '<mltyPostOfficeTypeCd>' +
                    (Xrm.Page.getAttribute('va_mailingmilitarypostofficetypecode').getText() ? Xrm.Page.getAttribute('va_mailingmilitarypostofficetypecode').getText() : '') +
                    '</mltyPostOfficeTypeCd>' +
                    '<provinceName>' + (Xrm.Page.getAttribute('va_provincename').getValue() ? Xrm.Page.getAttribute('va_provincename').getValue() : '') + '</provinceName>' +
                    '<territoryName>' + (Xrm.Page.getAttribute('va_territoryname').getValue() ? Xrm.Page.getAttribute('va_territoryname').getValue() : '') + '</territoryName>';

                overSeasPaymentPostCode = '<cpPaymentPostalTypeCode>' +
                    (Xrm.Page.getAttribute('va_paymentmilitarypostaltypecode').getText() ? Xrm.Page.getAttribute('va_paymentmilitarypostaltypecode').getText() : '') +
                    '</cpPaymentPostalTypeCode>' +
                    '<cpPaymentPoTypeCode>' +
                    (Xrm.Page.getAttribute('va_paymentmilitarypostofficetypecode').getText() ? Xrm.Page.getAttribute('va_paymentmilitarypostofficetypecode').getText() : '') +
                    '</cpPaymentPoTypeCode>';
                break;
            default:
                break;
        }
        // per Cory, ptcpntIdRecip cannot be used, use ptcpntIdPayee instead
        innerXml +=
            '<ptcpntIdPayee>' + NN(Xrm.Page.getAttribute('va_participantrecipid').getValue()) + '</ptcpntIdPayee>' +
                '<ptcpntIdVet>' + (Xrm.Page.getAttribute('va_participantvetid').getValue() ? Xrm.Page.getAttribute('va_participantvetid').getValue() : '') + '</ptcpntIdVet>' +
                '<ptcpntIdBene>' + (Xrm.Page.getAttribute('va_participantbeneid').getValue() ? Xrm.Page.getAttribute('va_participantbeneid').getValue() : '') + '</ptcpntIdBene>';

        if (Xrm.Page.getAttribute('va_openedfromclaimtab').getValue()) {
            innerXml += '<ptcpntIdRecip>' + '</ptcpntIdRecip>';
        } else {
            innerXml += '<ptcpntIdRecip>' + (Xrm.Page.getAttribute('va_participantrecipid').getValue() ? Xrm.Page.getAttribute('va_participantrecipid').getValue() : '') + '</ptcpntIdRecip>';
        }

        innerXml +=
            '<awardTypeCd>' + NN(Xrm.Page.getAttribute('va_awardtypecode').getValue()) + '</awardTypeCd>' +
                '<emailAddress>' + NN(Xrm.Page.getAttribute('va_email').getValue()) + '</emailAddress>' +
                '<addressType>' + addressType + '</addressType>' +
                '<transBypassCorporateInd> </transBypassCorporateInd>' +
                intMailingZip +
                intPaymentZip +
                overSeasMailingPostCode +
                overSeasPaymentPostCode +
                '</cInput></ser:updateBenefitClaimAddress>';


        return innerXml;
    };
    window.updateBenefitClaimAddress = updateBenefitClaimAddress;
    // END updateBenefitClaimAddress



    //=====================================================================================================
    //START updateFirstNoticeOfDeath
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <q0:updateFirstNoticeOfDeath>
    //         <fileNumber>545646541</fileNumber>
    //         <dateOfDeath>11/12/2011</dateOfDeath>
    //         <causeOfDeath>UNKNOWN</causeOfDeath>
    //      </q0:updateFirstNoticeOfDeath>
    //   </soapenv:Body>
    //</soapenv:Envelope>

    var updateFirstNoticeOfDeath = function (context) {
        this.context = context;

        this.serviceName = 'updateFirstNoticeOfDeath';
        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'BenefitClaimService.updateFirstNoticeOfDeath';
        this.wsMessage.serviceName = 'updateFirstNoticeOfDeath';
        this.wsMessage.friendlyServiceName = 'First Notice of Death';
        this.responseFieldSchema = 'va_fnodresponse';
        this.responseTimestamp = 'va_webserviceresponse';
        this.prefix = 'q0';

        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['fileNumber'] = null;
        this.requiredSearchParameters['DateOfDeath'] = null;
        this.requiredSearchParameters['CauseOfDeath'] = null;

    };
    updateFirstNoticeOfDeath.prototype = new benefitClaimWebService;
    updateFirstNoticeOfDeath.prototype.constructor = updateFirstNoticeOfDeath;
    updateFirstNoticeOfDeath.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

        var innerXml;
        var fileNumber = this.context.parameters['fileNumber'];
        var DOD = this.context.parameters['dateOfDeath'];
        var Cause = this.context.parameters['causeOfDeath'];

        if (fileNumber && fileNumber != '' && DOD && DOD.length > 0 && Cause && Cause.length > 0) {
            innerXml = '<q0:updateFirstNoticeOfDeath><fileNumber>' + fileNumber
                + '</fileNumber><dateOfDeath>' + DOD
                + '</dateOfDeath><causeOfDeath>' + Cause
                + '</causeOfDeath></q0:updateFirstNoticeOfDeath>';
        } else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'There must be a file number present for the request';
            return null;
        }

        return innerXml;
    };
    window.updateFirstNoticeOfDeath = updateFirstNoticeOfDeath;
    //END updateFirstNoticeOfDeath



    //=====================================================================================================
    //START findPresidentialMemorialCertificate
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <q0:findPresidentialMemorialCertificate>
    //         <fileNumber>545646540</fileNumber>
    //      </q0:findPresidentialMemorialCertificate>
    //   </soapenv:Body>
    //</soapenv:Envelope>

    var findPresidentialMemorialCertificate = function (context) {
        this.context = context;

        this.serviceName = 'findPresidentialMemorialCertificate';
        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'BenefitClaimService.findPresidentialMemorialCertificate';
        this.wsMessage.serviceName = 'findPresidentialMemorialCertificate';
        this.wsMessage.friendlyServiceName = 'Presidential Memorial Certificate';
        this.responseFieldSchema = 'va_findpmcresponse';
        this.responseTimestamp = 'va_webserviceresponse';
        this.prefix = 'q0';

        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['fileNumber'] = null;
    };
    findPresidentialMemorialCertificate.prototype = new benefitClaimWebService;
    findPresidentialMemorialCertificate.prototype.constructor = findPresidentialMemorialCertificate;

    findPresidentialMemorialCertificate.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

        var innerXml;
        var fileNumber = this.context.parameters['fileNumber'];

        if (fileNumber && fileNumber != '') {
            innerXml = '<q0:findPresidentialMemorialCertificate><fileNumber>' + fileNumber + '</fileNumber></q0:findPresidentialMemorialCertificate>';
        } else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'There must be a file number present for the request';
            return null;
        }

        return innerXml;
    };
    window.findPresidentialMemorialCertificate = findPresidentialMemorialCertificate;
    //END updateFirstNoticeOfDeath


    //=====================================================================================================
    //START insertPresidentialMemorialCertificate
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <q0:insertPresidentialMemorialCertificate>
    //         <pmcInput>
    //            <fileNumber>545646540</fileNumber>
    //            <veteranName/>
    //            <veteranFirstName>Jake</veteranFirstName>
    //            <veteranMiddleInitial>R</veteranMiddleInitial>
    //            <veteranLastName>Depends</veteranLastName>
    //            <veteranSuffixName>SR</veteranSuffixName>
    //            <station>281</station>
    //            <salutation>Miss</salutation>
    //            <title>Jane Depends</title>
    //            <addressLine1>988 A St</addressLine1>
    //            <city>Austin</city>
    //            <state>TX</state>
    //            <zipCode>78782</zipCode>
    //            <realtionshipToVeteran>SPOUSE</realtionshipToVeteran>
    //         </pmcInput>
    ////      </q0:insertPresidentialMemorialCertificate>
    //   </soapenv:Body>
    //</soapenv:Envelope>

    var insertPresidentialMemorialCertificate = function (context) {
        this.context = context;

        this.serviceName = 'insertPresidentialMemorialCertificate';
        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'BenefitClaimService.insertPresidentialMemorialCertificate';
        this.wsMessage.serviceName = 'insertPresidentialMemorialCertificate';
        //this.wsMessage.friendlyServiceName = 'Presidential Memorial Certificate';
        this.responseFieldSchema = 'va_insertpmcresponse';
        this.responseTimestamp = 'va_webserviceresponse';
        this.prefix = 'q0';

        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['fileNumber'] = null;
    };
    insertPresidentialMemorialCertificate.prototype = new benefitClaimWebService;
    insertPresidentialMemorialCertificate.prototype.constructor = insertPresidentialMemorialCertificate;

    insertPresidentialMemorialCertificate.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

        var innerXml;
        var fileNumber = this.context.parameters['fileNumber'];
        var veteranFirstName = this.context.parameters['veteranFirstName'];
        var veteranMiddleInitial = this.context.parameters['veteranMiddleInitial'];
        var veteranLastName = this.context.parameters['veteranLastName'];
        var veteranSuffixName = this.context.parameters['veteranSuffixName'];
        var station = this.context.parameters['station'];
        var salutation = this.context.parameters['salutation'];
        var title = this.context.parameters['title'];
        var addressLine1 = this.context.parameters['addressLine1'];
        var addressLine2 = this.context.parameters['addressLine2'];
        var city = this.context.parameters['city'];
        var state = this.context.parameters['state'];
        var zipCode = this.context.parameters['zipCode'];
        var realtionshipToVeteran = this.context.parameters['realtionshipToVeteran'];

        if (fileNumber && veteranFirstName && veteranLastName && station && salutation && addressLine1 && city && state && zipCode && realtionshipToVeteran) {

            innerXml =
                '<q0:insertPresidentialMemorialCertificate><pmcInput>' +
                    '<fileNumber>' + NN(fileNumber) + '</fileNumber>' +
                    '<veteranFirstName>' + NN(veteranFirstName) + '</veteranFirstName>';

            if (veteranMiddleInitial && veteranMiddleInitial != '') {
                innerXml += '<veteranMiddleInitial>' + NN(veteranMiddleInitial) + '</veteranMiddleInitial>';
            }

            innerXml +=
                '<veteranLastName>' + NN(veteranLastName) + '</veteranLastName>';

            if (veteranSuffixName && veteranSuffixName != '') {
                innerXml += '<veteranSuffixName>' + NN(veteranSuffixName) + '</veteranSuffixName>';
            }

            innerXml +=
                '<station>' + NN(station) + '</station>' +
                    '<salutation>' + NN(salutation) + '</salutation>' +
                    '<title>' + NN(title) + '</title>' +
                    '<addressLine1>' + NN(addressLine1) + '</addressLine1>';

            if (addressLine2 && addressLine2 != '') {
                innerXml += '<addressLine2>' + NN(addressLine2) + '</addressLine2>';
            }

            innerXml +=
                '<city>' + city + '</city>' +
                    '<state>' + state + '</state>' +
                    '<zipCode>' + zipCode + '</zipCode>' +
                    '<realtionshipToVeteran>' + NN(realtionshipToVeteran) + '</realtionshipToVeteran>' +
                    '</pmcInput></q0:insertPresidentialMemorialCertificate>';
        } else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = "The following fields must be populated:  vet's first/lastname, recipient's salutation/address/city/state/zip/relationship to vet.";
            return null;
        }

        return innerXml;
    };
    window.insertPresidentialMemorialCertificate = insertPresidentialMemorialCertificate;
    //END updateFirstNoticeOfDeath

    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    // END Individual BenefitClaimWebService Methods
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    // END BenefitClaimWebService
    //=====================================================================================================
};

function NN(s) { return (s == null ? '' : s); }