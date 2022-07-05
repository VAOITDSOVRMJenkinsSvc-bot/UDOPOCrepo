"use strict";

var ws = ws || {};
ws.shareStandardData = {};
// Function will be called by form code
ws.shareStandardData.initalize = function (context) {
    //=====================================================================================================
    // START ShareStandardDataService
    // Web Method List:  findPayeeCodes, findStates, findCountries, findEpcs, findMilitaryPostOfficeTypes,
    // findMilitaryPostalTypes, findSsnVerificationTypes, findCauseOfDeathTypes, findIntakeSites, findDiagnosticCodes,
    // findPcanReasonCodes, findPclrReasonCodes, findDiaryReasonCodes, findSsaInquiryReasonCodes,
    // findStationAddress, findFutureReasons, findSalutations, findSuffixes, findGulfWarRegistryPermits,
    // findBranchOfServiceTypes, findPowerOfAttorneys, findCharacterOfServiceTypes, findSeparationReasonCodes,
    // findPayGrades, findGender, findInsurancePrefixs, findInsuranceFolderTypes, findInsurancePolicyPrefixs,
    // findFolderTypes, findNoFolderEstablishedReasons, findAllStations, findNARAs, findStations, findProgramTypes
    //
    // EX:
    //      var shareStandardDataServiceCtx = new actionContext();
    //      shareStandardDataServiceCtx.user = GetUserSettingsForWebservice();
    //
    //      var findPayeeCodes  = new  shareStandardDataService(shareStandardDataServiceCtx);
    //      findPayeeCodes.serviceName = 'findPayeeCodes';
    //      findPayeeCodes.wsMessage.serviceName = 'findPayeeCodes';
    //      findPayeeCodes.responseFieldSchema = 'va_findpayeecodesresponse';
    //      findPayeeCodes.responseTimestamp = 'va_webserviceresponse';
    //      findPayeeCodes.executeRequest();
    //=====================================================================================================
    var shareStandardDataService = function (context) {
        this.context = context;
        this.webserviceRequestUrl = WebServiceURLRoot + 'ShareStandardDataServiceBean/ShareStandardDataWebService';
        this.wsMessage.methodName = 'ShareStandardDataService.shareStandardDataService';
    };
    shareStandardDataService.prototype = new webservice(context);
    shareStandardDataService.prototype.constructor = shareStandardDataService;
    shareStandardDataService.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
        var innerXml;

        innerXml = '<q0:' + this.serviceName + '></q0:' + this.serviceName + '>';

        return innerXml;
    };
    window.shareStandardDataService = shareStandardDataService;
    // END ShareStandardDataService
    //=====================================================================================================

    //=====================================================================================================
    // START findRegionalOffices 
    // Web Method List:  findRegionalOffices 
    //
    // EX:
    //      var findRegionalOfficesCtx = new vrmContext();
    //      findRegionalOfficesCtx.user = GetUserSettingsForWebservice();
    //
    //      var findROS = new  findRegionalOffices(findRegionalOfficesCtx);
    //      findROS.serviceName = 'findRegionalOffices';
    //      findROS.wsMessage.serviceName = 'findRegionalOffices';
    //      findROS.responseFieldSchema = 'va_roxml';
    //      findROS.responseTimestamp = 'va_wstimestamp';
    //      findROS.executeRequest();
    //=====================================================================================================
    var findRegionalOffices = function (context) {
        this.context = context;
        this.wsMessage.methodName = 'ShareStandardDataService.findRegionalOffices';
    };
    findRegionalOffices.prototype = new shareStandardDataService;
    findRegionalOffices.prototype.constructor = findRegionalOffices;
    findRegionalOffices.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
        var innerXml;

        innerXml = '<q0:findRegionalOffices></q0:findRegionalOffices>';

        return innerXml;
    };
    window.findRegionalOffices = findRegionalOffices;
    // END findRegionalOffices
    //=====================================================================================================
    // START findRegionalOffices 
    // Web Method List:  findRegionalOffices 
    //
    // EX:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:findCountries>
    //         
    //      </ser:findCountries>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    //=====================================================================================================
    var findCountries = function (context) {
        this.prefix = 'ser';
        this.prefixUrl = 'http://services.share.benefits.vba.va.gov/';

        this.context = context;
        this.serviceName = 'findCountries';

        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'ShareStandardDataService.findCountries';
        this.wsMessage.serviceName = 'findCountries';
        this.wsMessage.friendlyServiceName = 'Find Countries';
    };
    findCountries.prototype = new shareStandardDataService;
    findCountries.prototype.constructor = findCountries;
    findCountries.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
        var innerXml;

        innerXml = '<ser:findCountries></ser:findCountries>';

        return innerXml;
    };
    window.findCountries = findCountries;
    // END findRegionalOffices
    //=====================================================================================================
};