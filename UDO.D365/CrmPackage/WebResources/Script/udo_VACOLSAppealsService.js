"use strict";

var ws = ws || {};
ws.appeals = {};

var _formContext = null;

// Function will be called by form onLoad event
ws.appeals.initalize = function (executionContext) {
    _formContext = executionContext.getFormContext();

    //=====================================================================================================
    // START AppealsService
    //=====================================================================================================
    var appealsWebService = function (context) {
        this.context = context;
        /*
        // TEST
        this.webserviceRequestUrl = 'http://vaausvrsapp80.aac.va.gov/VIERSService/AppealService/Appeal';

        // PROD
        if (_isPROD === true) { this.webserviceRequestUrl = 'https://vavdrapp1.aac.va.gov/VIERSService/AppealService/Appeal'; }
        // PREPROD 'https://vavdrapp2.aac.va.gov/VIERSService/AppealService/Appeal'

        //this.ignoreGlobalDACUrl = true;
        //this.serviceDACUrl = 'http://10.153.50.201/ISV/DAC/RedirectSvc.asmx'; //DEV
        //this.serviceDACUrl = 'http://10.153.95.69/ISV/DAC/RedirectSvc.asmx'; //INT
        */

        this.webserviceRequestUrl = _currentEnv.Vacols;
        this.serviceDACUrl = _currentEnv.VacolsDAC;
        this.prefix = 'app';
        this.prefixUrl = "http://www.va.gov/schema/AppealService";

    };
    appealsWebService.prototype = new webservice(executionContext, this.context);
    appealsWebService.prototype.constructor = appealsWebService;
    window.appealsWebService = appealsWebService;
    //=====================================================================================================
    // START Individual AppealsService Methods
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START findAppeals
    //=====================================================================================================
    var findAppeals = function (context) {
        this.context = context;
        this.wsMessage.methodName = 'AppealsService.findAppeals';

        this.serviceName = 'findAppeals';
        this.wsMessage.serviceName = 'findAppeals';
        this.wsMessage.friendlyServiceName = 'Appeals';
        this.responseFieldSchema = 'va_findappealsresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['fileNumber'] = null;
    };
    findAppeals.prototype = new appealsWebService;
    findAppeals.prototype.constructor = findAppeals;
    findAppeals.prototype.getSearchParameter = function () {
        // search by file no, ssn or user-provided value
        var searchPar = this.context.parameters['fileNumber'];

        var userSelection = _formContext.getAttribute('va_findappealsby').getSelectedOption();
        if (!userSelection || userSelection.length === 0) { return searchPar; }

        if (userSelection === 'SSN') { searchPar = this.context.parameters['ssn']; }
        else if (userSelection === 'This Value') { searchPar = _formContext.getAttribute('va_appealssearchtext').getValue(); }

        return searchPar;
    };
    findAppeals.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

        var innerXml;
        var fileNumber = this.getSearchParameter();

        if (fileNumber && fileNumber !== '') {
            innerXml = '<app:findAppeals><app:findAppealCriteria><AppellantID></AppellantID><SSN>' + fileNumber
                + '</SSN></app:findAppealCriteria></app:findAppeals>';
        } else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'There must be a file number present for the request';
            return null;
        }

        return innerXml;
    };
    window.findAppeals = findAppeals;
    //END findAppeals
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START getAppeal
    //=====================================================================================================
    var getAppeal = function (context) {
        this.context = context;
        this.wsMessage.methodName = 'AppealsService.getAppeal';

        this.serviceName = 'getAppeal';
        this.wsMessage.serviceName = 'getAppeal';
        this.wsMessage.friendlyServiceName = 'Appeals Detail';
        this.responseFieldSchema = 'va_findindividualappealsresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['appealKey'] = null;
        this.ignoreRequiredParMissingWarning = true;
    };
    getAppeal.prototype = new appealsWebService;
    getAppeal.prototype.constructor = getAppeal;
    getAppeal.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

        var innerXml;
        var appealKey = this.context.parameters['appealKey'];

        if (appealKey && appealKey !== '') {
            innerXml = '<app:getAppeal><app:getAppealCriteria><AppealKey>' + appealKey
                + '</AppealKey></app:getAppealCriteria></app:getAppeal>';
        } else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'There must be an appeal key present for the request';
            return null;
        }

        return innerXml;
    };
    window.getAppeal = getAppeal;
    //END getAppeal
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START getAppellantAddress
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:app="http://www.va.gov/schema/AppealService">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <app:getAppellantAddress>
    //         <!--Optional:-->
    //         <app:getAddressCriteria>
    //            <!--Optional:-->
    //            <AppellantID>?</AppellantID>
    //            <!--Optional:-->
    //            <SSN>?</SSN>
    //         </app:getAddressCriteria>
    //      </app:getAppellantAddress>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    //=====================================================================================================
    var getAppellantAddress = function (context) {
        this.context = context;
        this.wsMessage.methodName = 'AppealsService.getAppellantAddress';

        this.serviceName = 'getAppellantAddress';
        this.wsMessage.serviceName = 'getAppellantAddress';
        this.wsMessage.friendlyServiceName = 'Appellant Address';
        this.responseFieldSchema = 'va_appellantaddressresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.requiredSearchParameters = new Array();
    };
    getAppellantAddress.prototype = new appealsWebService;
    getAppellantAddress.prototype.constructor = getAppellantAddress;
    getAppellantAddress.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

        var innerXml = '<app:getAppellantAddress xmlns:app1="http://www.va.gov/schema/Appeals_LDM"><app:getAddressCriteria>',
            ssn = this.context.parameters['fileNumber'],
            firstName = this.context.parameters['appealsFirstName'],
            lastName = this.context.parameters['appealsLastName'];

        if (ssn && ssn !== '') {
            innerXml += '<SSN>' + ssn + '</SSN>';
        }

        if (firstName && firstName !== undefined && firstName.length > 0) {
            innerXml += '<FirstName app1:Partialflag="true">' + firstName + '</FirstName>';
        }

        if (lastName && lastName !== undefined && lastName.length > 0) {
            innerXml += '<LastName app1:Partialflag="true">' + lastName + '</LastName>';
        }

        innerXml += '</app:getAddressCriteria></app:getAppellantAddress>';

        return innerXml;
    };
    window.getAppellantAddress = getAppellantAddress;
    //END getAppellantAddress
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START getAppellantAddress
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:app="http://www.va.gov/schema/AppealService">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <app:updateAppellantAddress>
    //         <!--Optional:-->
    //         <app:updateAddressCriteria>
    //            <!--Optional:-->
    //            <AppellantID></AppellantID>
    //            <!--Optional:-->
    //            <SSN>555119977</SSN>
    //            <AppellantAddress>
    //               <!--Optional:-->
    //               <AppellantAddressLine1>1234 Main St.</AppellantAddressLine1>
    //               <!--Optional:-->
    //               <AppellantAddressLine2></AppellantAddressLine2>
    //               <!--Optional:-->
    //               <AppellantAddressCityName>Arlington</AppellantAddressCityName>
    //               <!--Optional:-->
    //               <AppellantAddressStateCode>VA</AppellantAddressStateCode>
    //               <!--Optional:-->
    //               <AppellantAddressZipCode>22203</AppellantAddressZipCode>
    //               <!--Optional:-->
    //               <AppellantAddressCountryName></AppellantAddressCountryName>
    //               <!--Optional:-->
    //               <AppellantAddressLastModifiedByROName></AppellantAddressLastModifiedByROName>
    //               <!--Optional:-->
    //               <AppellantAddressLastModifiedDate></AppellantAddressLastModifiedDate>
    //               <!--Optional:-->
    //               <AppellantAddressNotes></AppellantAddressNotes>
    //               <!--Optional:-->
    //               <AppellantWorkPhoneNumber></AppellantWorkPhoneNumber>
    //               <!--Optional:-->
    //               <AppellantHomePhoneNumber></AppellantHomePhoneNumber>
    //            </AppellantAddress>
    //         </app:updateAddressCriteria>
    //      </app:updateAppellantAddress>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    //=====================================================================================================
    var updateAppellantAddress = function (context) {
        this.context = context;
        this.wsMessage.methodName = 'AppealsService.updateAppellantAddress';

        this.serviceName = 'updateAppellantAddress';
        this.wsMessage.serviceName = 'updateAppellantAddress';
        this.wsMessage.friendlyServiceName = 'Appellant Address';
        this.responseFieldSchema = 'va_updateappellantaddressresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.requiredSearchParameters = new Array();
    };
    updateAppellantAddress.prototype = new appealsWebService;
    updateAppellantAddress.prototype.constructor = updateAppellantAddress;
    updateAppellantAddress.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

        var innerXml;
        var addressKey = this.context.parameters['addressKey'];

        var innerXml = ''
        if (addressKey && addressKey !== '') {
            innerXml = '<app:updateAppellantAddress><app:updateAddressCriteria><AppellantAddress><AddressKey>' + addressKey
                + '</AddressKey>';
        } else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'There must be a Address Key present for the request';
            return null;
        }

        innerXml += '<AppellantAddressLine1>' + NN(_formContext.getAttribute('va_apellantaddress1').getValue()) + '</AppellantAddressLine1>' +
            '<AppellantAddressLine2>' + NN(_formContext.getAttribute('va_apellantaddress2').getValue()) + '</AppellantAddressLine2>' +
            '<AppellantAddressCityName>' + NN(_formContext.getAttribute('va_apellantcity').getValue()) + '</AppellantAddressCityName>' +
            '<AppellantAddressStateCode>' + NN(_formContext.getAttribute('va_apellantstate').getValue()) + '</AppellantAddressStateCode>' +
            '<AppellantAddressZipCode>' + NN(_formContext.getAttribute('va_apellantzipcode').getValue()) + '</AppellantAddressZipCode>' +
            '<AppellantAddressCountryName>' + NN(_formContext.getAttribute('va_apellantcountry').getValue()) + '</AppellantAddressCountryName>' + '<AppellantAddressNotes>' + NN(_formContext.getAttribute('va_appellantaddressnotes').getValue()) + '</AppellantAddressNotes>' +
            '<AppellantWorkPhoneNumber>' + NN(_formContext.getAttribute('va_apellantworkphone').getValue()) + '</AppellantWorkPhoneNumber>' +
            '<AppellantHomePhoneNumber>' + NN(_formContext.getAttribute('va_apellanthomephone').getValue()) + '</AppellantHomePhoneNumber>' +
            '</AppellantAddress></app:updateAddressCriteria></app:updateAppellantAddress>';

        return innerXml;
    };
    window.updateAppellantAddress = updateAppellantAddress;
    //END getAppellantAddress
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    // END Individual AppealsService Methods
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    // END AppealsService
    //=====================================================================================================
    function NN(s) { return (s === null ? '' : s); }
};