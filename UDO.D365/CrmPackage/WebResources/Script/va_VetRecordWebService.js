"use strict";

var ws = ws || {};
ws.vetRecord = {};

var _formContext = null;

ws.vetRecord.retrieveFormContext = function(executionContext) {
    if (executionContext && exCon.getFormContext) {
        return executionContext.getFormContext();
    }
    else {
        return null;
    }
}

// Function will be called by form code
ws.vetRecord.initalize = function (executionContext) {
    _formContext = retrieveFormContext(executionContext);
    //=====================================================================================================
    // START VetRecordService
    //=====================================================================================================
    var vetRecordWebService = function (context) {
        this.context = context;
        this.webserviceRequestUrl = WebServiceURLRoot + 'VetRecordServiceBean/VetRecordWebService';
        this.prefix = 'ser';
        this.prefixUrl = 'http://services.share.benefits.vba.va.gov/';
        //this.proxyServiceUrl = 'http://10.153.50.200/VA.VRMUD.RedirectService/RedirectSvc.asmx';
        };
    vetRecordWebService.prototype = new webservice(executionContext);
    vetRecordWebService.prototype.constructor = vetRecordWebService;
    window.vetRecordWebService = vetRecordWebService;
    //=====================================================================================================
    // START Individual VetRecordService Methods
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START findBirlsRecord
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:findBirlsRecord>
    //         <!--Optional:-->
    //         <veteranRecordInput>
    //            <commandName>SHAR</commandName>
    //            <commandType>I</commandType>
    //            <transactionName>BPNQ</transactionName>
    //            <!--Optional:-->
    //            <fileNumber>?</fileNumber>
    //            <!--Optional:-->
    //            <ssn>?</ssn>
    //            <!--Optional:-->
    //            <insuranceNumber>?</insuranceNumber>
    //            <!--Optional:-->
    //            <serviceNumber>?</serviceNumber>
    //            <!--Optional:-->
    //            <lastName>?</lastName>
    //            <!--Optional:-->
    //            <firstName>?</firstName>
    //            <!--Optional:-->
    //            <middleName>?</middleName>
    //            <!--Optional:-->
    //            <suffix>?</suffix>
    //            <!--Optional:-->
    //            <payeeNumber>00</payeeNumber>
    //            <!--Optional:-->
    //            <branchOfService>?</branchOfService>
    //            <!--Optional:-->
    //            <dateOfBirth>?</dateOfBirth>
    //            <!--Optional:-->
    //            <dateOfDeath>?</dateOfDeath>
    //            <!--Optional:-->
    //            <enteredOnDutyDate>?</enteredOnDutyDate>
    //            <!--Optional:-->
    //            <releasedActiveDutyDate>?</releasedActiveDutyDate>
    //            <!--Optional:-->
    //            <folderLocation>?</folderLocation>
    //            <userSSN>?</userSSN>
    //            <userFileNumber>?</userFileNumber>
    //            <userStationNumber>?</userStationNumber>
    //            <userID>?</userID>
    //            <userIPAddress>?</userIPAddress>
    //            <applicationName>VBMS</applicationName>
    //            <processName>BIRLS Inquiry</processName>
    //         </veteranRecordInput>
    //         <!--Optional:-->
    //      </ser:findBirlsRecord>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    var findBirlsRecord = function (context) {
        this.context = context;
        this.serviceName = 'findBirlsRecord';
        this.responseFieldSchema = 'va_findbirlsresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'VetRecordService.findBirlsRecord';
        this.wsMessage.serviceName = 'findBirlsRecord';
        this.wsMessage.friendlyServiceName = 'BIRLS Record';

        this.dataSourceForKey = new Array();
        this.dataSourceForKey['ssn'] = true;
    };
    findBirlsRecord.prototype = new vetRecordWebService;
    findBirlsRecord.prototype.constructor = findBirlsRecord;
    findBirlsRecord.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
        var innerXml;
    
        var firstName = (_formContext.data.entity.getEntityName() === 'contact' ? _formContext.getAttribute('firstname').getValue() : _formContext.getAttribute('va_firstname').getValue()); 
        var lastName = (_formContext.data.entity.getEntityName() === 'contact' ? _formContext.getAttribute('lastname').getValue() : _formContext.getAttribute('va_lastname').getValue()); 

        if ((firstName === null || firstName === '') || (lastName === null || lastName === '')) {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'You must provide at least First Name and Last Name to perform the search';
            return null;
        }

        var moreSearchOptions = (_formContext.getAttribute('va_moresearchoptions') && _formContext.getAttribute('va_moresearchoptions').getValue() === true); 
        var dobDate = (_formContext.getAttribute('va_dobtext') !== null && _formContext.getAttribute('va_dobtext').getValue() !== null) ? new Date(_formContext.getAttribute('va_dobtext').getValue()) : null;  

        var params = new Array();
        // BIRLS fields
        params['firstName'] = firstName;
        params['lastName'] = lastName;
        params['middleName'] = _formContext.getAttribute('va_middleinitial').getValue();    
        params['dateOfBirth'] = null; if (dobDate) params['dateOfBirth'] = dobDate.format("MMddyyyy");

        if (moreSearchOptions) {
            // BIRLS Only
            params['fileNumber'] = _formContext.getAttribute('va_ssn').getValue();
            params['ssn'] = _formContext.getAttribute('va_ssn').getValue();
            params['insuranceNumber'] = _formContext.getAttribute('va_insurancenumber').getValue();
            params['serviceNumber'] = _formContext.getAttribute('va_servicenumber').getValue();
            params['branchOfService'] = _formContext.getAttribute('va_branchofservice').getValue();
            params['dateOfDeath'] = null; if (_formContext.getAttribute('va_dod').getValue()) params['dateOfDeath'] = _formContext.getAttribute('va_dod').getValue().format("MMddyyyy");
            params['enteredOnDutyDate'] = null; if (_formContext.getAttribute('va_enteredondutydate').getValue()) params['enteredOnDutyDate'] = _formContext.getAttribute('va_enteredondutydate').getValue().format("MMddyyyy");
            params['releasedActiveDutyDate'] = null; if (_formContext.getAttribute('va_releasedactivedutydate').getValue()) params['releasedActiveDutyDate'] = _formContext.getAttribute('va_releasedactivedutydate').getValue().format("MMddyyyy");
            params['folderLocation'] = _formContext.getAttribute('va_folderlocation').getValue();
            params['suffix'] = _formContext.getAttribute('va_suffix').getValue();
            params['payeeNumber'] = _formContext.getAttribute('va_payeenumber').getValue();
        }

        params['userSSN'] = this.context.user.ssn;
        params['userFileNumber'] = this.context.user.fileNumber;
        params['userStationNumber'] = this.context.user.stationId;
        params['userID'] = this.context.user.userName;
        params['userIPAddress'] = this.context.user.clientMachine;

        innerXml = '<ser:findBirlsRecord><veteranRecordInput><commandName>SHAR</commandName><commandType>I</commandType>'
        + '<transactionName>BPNQ</transactionName>';

        for (var node in params) {
            if (params[node]) innerXml += ('<' + node + '>' + params[node] + '</' + node + '>');
        }

        innerXml += '<applicationName>VBMS</applicationName>'
        + '<processName>BIRLS Inquiry</processName></veteranRecordInput></ser:findBirlsRecord>';

        return innerXml;
    };
    window.findBirlsRecord = findBirlsRecord;
    //END findBirlsRecord
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START findBirlsRecordByFileNumber
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:findBirlsRecordByFileNumber>
    //         <!--Optional:-->
    //         <fileNumber>?</fileNumber>
    //      </ser:findBirlsRecordByFileNumber>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    var findBirlsRecordByFileNumber = function (context) {
        this.context = context;
        this.serviceName = 'findBirlsRecordByFileNumber';

        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'VetRecordService.findBirlsRecordByFileNumber';
        this.wsMessage.serviceName = 'findBirlsRecordByFileNumber';
        this.wsMessage.friendlyServiceName = 'BIRLS Record By File Number';
        this.responseFieldSchema = 'va_findbirlsresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['fileNumber'] = null;
    };
    findBirlsRecordByFileNumber.prototype = new vetRecordWebService;
    findBirlsRecordByFileNumber.prototype.constructor = findBirlsRecordByFileNumber;
    findBirlsRecordByFileNumber.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
        var innerXml;

        var fileNumber = this.context.parameters['fileNumber'];

        if (fileNumber && fileNumber !== '') {
            innerXml = '<ser:findBirlsRecordByFileNumber><fileNumber>' + fileNumber + '</fileNumber>'
        + '</ser:findBirlsRecordByFileNumber>';
            } else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'You must provide a File Number to perform the search';
            return null;
        }

        return innerXml;
    };
    window.findBirlsRecordByFileNumber = findBirlsRecordByFileNumber;
    //END findBirlsRecordByFileNumber
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START findCorporateRecord
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:findCorporateRecord>
    //         <!--Optional:-->
    //         <ptcpntSearchPSNInput>
    //            <!--Optional:-->
    //            <city>?</city>
    //            <!--Optional:-->
    //            <dateOfBirth>?</dateOfBirth>
    //            <!--Optional:-->
    //            <firstName>?</firstName>
    //            <!--Optional:-->
    //            <lastName>?</lastName>
    //            <!--Optional:-->
    //            <middleName>?</middleName>
    //            <!--Optional:-->
    //            <state>?</state>
    //            <!--Optional:-->
    //            <zipCode>?</zipCode>
    //         </ptcpntSearchPSNInput>
    //      </ser:findCorporateRecord>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    var findCorporateRecord = function (context) {
        this.context = context;
        this.serviceName = 'findCorporateRecord';

        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'VetRecordService.findCorporateRecord';
        this.wsMessage.serviceName = 'findCorporateRecord';
        this.wsMessage.friendlyServiceName = 'Corp Record';
        this.responseFieldSchema = 'va_findcorprecordresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.dataSourceForKey = new Array();
        this.dataSourceForKey["ssn"] = true;
        this.dataSourceForKey["fileNumber"] = true;
        this.dataSourceForKey["ptcpntId"] = true;
    };
    findCorporateRecord.prototype = new vetRecordWebService;
    findCorporateRecord.prototype.constructor = findCorporateRecord;
    findCorporateRecord.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
        var innerXml;

        var firstName = (_formContext.data.entity.getEntityName() === 'contact' ? _formContext.getAttribute('firstname').getValue() : _formContext.getAttribute('va_firstname').getValue());
        var lastName = (_formContext.data.entity.getEntityName() === 'contact' ? _formContext.getAttribute('lastname').getValue() : _formContext.getAttribute('va_lastname').getValue());

        if ((firstName === null || firstName === '') || (lastName === null || lastName === '')) {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'You must provide at least First Name and Last Name to perform the search';
            return null;
        }

        var moreSearchOptions = (_formContext.getAttribute('va_moresearchoptions') && _formContext.getAttribute('va_moresearchoptions').getValue() === true);
        var dobDate = (_formContext.getAttribute('.va_dobtext') !== null && _formContext.getAttribute('va_dobtext').getValue() !== null) ? new Date(_formContext.getAttribute('va_dobtext').getValue()) : null;

        var params = new Array();
        // CORP
        params['firstName'] = firstName;
        params['lastName'] = lastName;

        params['middleName'] = _formContext.getAttribute('.va_middleinitial').getValue();
        params['dateOfBirth'] = null; if (dobDate) params['dateOfBirth'] = dobDate.format("MMddyyyy");

        if (moreSearchOptions) {
            // CORP Only
            params['city'] = _formContext.getAttribute('va_citysearch').getValue();
            params['state'] = _formContext.getAttribute('va_statesearch').getValue();
            params['zipCode'] = _formContext.getAttribute('va_zipcodesearch').getValue();
        }

        innerXml = '<ser:findCorporateRecord><ptcpntSearchPSNInput>';
        for (var node in params) {
            if (params[node]) innerXml += ('<' + node + '>' + params[node] + '</' + node + '>');
        }
        innerXml += '</ptcpntSearchPSNInput></ser:findCorporateRecord>';

        return innerXml;
    };
    window.findCorporateRecord = findCorporateRecord;
    //END findCorporateRecord
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START findCorporateRecordByFileNumber
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:findCorporateRecordByFileNumber>
    //         <!--Optional:-->
    //         <fileNumber>?</fileNumber>
    //         <!--Optional:-->
    //      </ser:findCorporateRecordByFileNumber>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    var findCorporateRecordByFileNumber = function (context) {
        this.context = context;
        this.serviceName = 'findCorporateRecordByFileNumber';

        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'VetRecordService.findCorporateRecord';
        this.wsMessage.serviceName = 'findCorporateRecordByFileNumber';
        this.wsMessage.friendlyServiceName = 'Corp Record By File Number';
        this.responseFieldSchema = 'va_findcorprecordresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['fileNumber'] = null;
    };

    findCorporateRecordByFileNumber.prototype = new vetRecordWebService;
    findCorporateRecordByFileNumber.prototype.constructor = findCorporateRecordByFileNumber;
    findCorporateRecordByFileNumber.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
        var innerXml;

        var fileNumber = this.context.parameters['fileNumber'];

        if (fileNumber && fileNumber !== '') {
            innerXml = '<ser:findCorporateRecordByFileNumber><fileNumber>' + fileNumber + '</fileNumber>'
        + '</ser:findCorporateRecordByFileNumber>';
        }
        else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'You must provide a File Number to perform the search';
            return null;
        }

        return innerXml;
    };
    window.findCorporateRecordByFileNumber = findCorporateRecordByFileNumber;
    //END findCorporateRecordByFileNumber
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START findVeteranByFileNumber
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:findVeteranByFileNumber>
    //         <!--Optional:-->
    //         <fileNumber>?</fileNumber>
    //         <!--Optional:-->
    //      </ser:findVeteranByFileNumber>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    var findVeteranByFileNumber = function (context) {
        this.context = context;
        this.serviceName = 'findVeteranByFileNumber';

        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'VetRecordService.findVeteranByFileNumber';
        this.wsMessage.serviceName = 'findVeteranByFileNumber';
        this.wsMessage.friendlyServiceName = 'Veteran By File Number';
        this.responseFieldSchema = 'va_findveteranresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['fileNumber'] = null;
    };
    findVeteranByFileNumber.prototype = new vetRecordWebService;
    findVeteranByFileNumber.prototype.constructor = findVeteranByFileNumber;
    findVeteranByFileNumber.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
        var innerXml;

        var fileNumber = this.context.parameters['fileNumber'];

        if (fileNumber && fileNumber !== '') {
            innerXml = '<ser:findVeteranByFileNumber><fileNumber>' + fileNumber + '</fileNumber>'
        + '</ser:findVeteranByFileNumber>';
        }
        else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'You must provide a File Number to perform the search';
            return null;
        }

        return innerXml;
    };
    window.findVeteranByFileNumber = findVeteranByFileNumber;
    //END findVeteranByFileNumber
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START findVeteranByPtcpntId
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:findVeteranByPtcpntId>
    //         <!--Optional:-->
    //         <ptcpntId>?</ptcpntId>
    //         <!--Optional:-->
    //      </ser:findVeteranByPtcpntId>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    var findVeteranByPtcpntId = function (context) {
        this.context = context;
        this.serviceName = 'findVeteranByPtcpntId';

        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'VetRecordService.findVeteranByPtcpntId';
        this.wsMessage.serviceName = 'findVeteranByPtcpntId';
        this.wsMessage.friendlyServiceName = 'Veteran By Ptcpnt Id';
        this.responseFieldSchema = 'va_findveteranresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['ptcpntId'] = null;
    };
    findVeteranByPtcpntId.prototype = new vetRecordWebService;
    findVeteranByPtcpntId.prototype.constructor = findVeteranByPtcpntId;
    findVeteranByPtcpntId.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
        var innerXml;

        var ptcpntId = this.context.parameters['ptcpntId'];

        if (ptcpntId && ptcpntId !== '') {
            innerXml = '<ser:findVeteranByPtcpntId><ptcpntId>' + ptcpntId + '</ptcpntId>'
        + '</ser:findVeteranByPtcpntId>';
        }
        else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'You must provide a Ptcpnt Id to perform the search';
            return null;
        }

        return innerXml;
    };
    window.findVeteranByPtcpntId = findVeteranByPtcpntId;
    //END findVeteranByPtcpntId

    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START syncCorporateBirls
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:syncCorporateBirls>
    //         <fileNumber>?</fileNumber>
    //      </ser:syncCorporateBirls>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    var syncCorporateBirls = function (context) {
        this.context = context;
        this.serviceName = 'syncCorporateBirls';

        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'VetRecordService.syncCorporateBirls';
        this.wsMessage.serviceName = 'syncCorporateBirls';
        this.wsMessage.friendlyServiceName = 'Sync Corporate and BIRLS';
        this.responseFieldSchema = 'va_synccorpbirlsresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['fileNumber'] = null;
    };
    syncCorporateBirls.prototype = new vetRecordWebService;
    syncCorporateBirls.prototype.constructor = syncCorporateBirls;
    syncCorporateBirls.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
        var innerXml;

        var fileNumber = this.context.parameters['fileNumber'];

        if (fileNumber && fileNumber !== '') {
            innerXml = '<ser:syncCorporateBirls><fileNumber>' + fileNumber + '</fileNumber>' + '</ser:syncCorporateBirls>';
        }
        else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'You must provide a File Number to Sync Corporate and BIRLS';
            return null;
        }

        return innerXml;
    };
    window.syncCorporateBirls = syncCorporateBirls;
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    // END Individual VetRecordService Methods
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    // END VetRecordService
    //=====================================================================================================
};