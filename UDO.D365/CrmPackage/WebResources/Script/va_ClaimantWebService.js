"use strict";

var ws = ws || {};
ws.claimant = {};
var _formContext = null;

ws.claimant.retrieveFormContext = function (executionContext) {
    if (executionContext && exCon.getFormContext) {
        return executionContext.getFormContext();
    }
    else {
        return null;
    }
}

var _formContext = null;

ws.claimant.retrieveFormContext = function (executionContext) {
    if (executionContext && exCon.getFormContext) {
        return executionContext.getFormContext();
    }
    else {
        return null;
    }
}

// Function will be called by form onLoad event
ws.claimant.initalize = function (executionContext) {
    _formContext = retrieveFormContext(executionContext);
    //=====================================================================================================
    // START ClaimantWebService
    //=====================================================================================================
    var claimantWebService = function (context) {
        this.context = context;
        this.webserviceRequestUrl = WebServiceURLRoot + 'ClaimantServiceBean/ClaimantWebService';

        this.prefix = 'ser';
        this.prefixUrl = 'http://services.share.benefits.vba.va.gov/';
    };
    claimantWebService.prototype = new webservice(executionContext);
    claimantWebService.prototype.constructor = claimantWebService;
    window.claimantWebService = claimantWebService;
    //=====================================================================================================
    // START Individual ClaimantWebService Methods
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START findGeneralInformationByFileNumber
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:findGeneralInformationByFileNumber>
    //         <!--Optional:-->
    //         <fileNumber>?</fileNumber>
    //         <!--Optional:-->
    //      </ser:findGeneralInformationByFileNumber>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    //TODO: change from SOAP to REST
    var findGeneralInformationByFileNumber = function (context) {
        this.context = context;

        this.serviceName = 'findGeneralInformationByFileNumber';
        this.responseFieldSchema = 'va_generalinformationresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'ClaimantWebService.findGeneralInformationByFileNumber';
        this.wsMessage.serviceName = 'findGeneralInformationByFileNumber';
        this.wsMessage.friendlyServiceName = 'General Info By File Number';

        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['fileNumber'] = null;
    };
    findGeneralInformationByFileNumber.prototype = new claimantWebService;
    findGeneralInformationByFileNumber.prototype.constructor = findGeneralInformationByFileNumber;
    findGeneralInformationByFileNumber.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
        var innerXml;

        var fileNumber = this.context.parameters['fileNumber'];

        if (fileNumber && fileNumber !== '') {
            innerXml = '<ser:findGeneralInformationByFileNumber><fileNumber>' + fileNumber
                + '</fileNumber></ser:findGeneralInformationByFileNumber>';
        } else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'You must provide a File Number to perform the search';
            return null;
        }

        return innerXml;
    };
    window.findGeneralInformationByFileNumber = findGeneralInformationByFileNumber;
    //END findGeneralInformationByFileNumber
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START findGeneralInformationByPtcpntIds
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:findGeneralInformationByPtcpntIds>
    //         <!--Optional:-->
    //         <ptcpntVetId>?</ptcpntVetId>
    //         <!--Optional:-->
    //         <ptcpntBeneId>?</ptcpntBeneId>
    //         <!--Optional:-->
    //         <ptpcntRecipId>?</ptpcntRecipId>
    //         <!--Optional:-->
    //         <awardTypeCd>?</awardTypeCd>
    //         <!--Optional:-->
    //      </ser:findGeneralInformationByPtcpntIds>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    //TODO: change from SOAP to REST
    var findGeneralInformationByPtcpntIds = function (context) {
        this.context = context;

        this.serviceName = 'findGeneralInformationByPtcpntIds';
        this.responseFieldSchema = 'va_generalinformationresponsebypid';
        this.responseTimestamp = 'va_webserviceresponse';

        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'ClaimantService.findGeneralInformationByPtcpntIds';
        this.wsMessage.serviceName = 'findGeneralInformationByPtcpntIds';
        this.wsMessage.friendlyServiceName = 'General Info By Ptcpnt Id';


        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['ptcpntVetId'] = null;
        //this.requiredSearchParameters['ptcpntBeneId'] = null;
        //this.requiredSearchParameters['ptcpntRecipId'] = null;
        //this.requiredSearchParameters['awardTypeCd'] = null;
    };
    findGeneralInformationByPtcpntIds.prototype = new claimantWebService;
    findGeneralInformationByPtcpntIds.prototype.constructor = findGeneralInformationByPtcpntIds;
    findGeneralInformationByPtcpntIds.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

        var innerXml;
        var vetXml;
        var beneXml = '';
        var recipXml = '';
        var awardXml = '';

        var ptcpntVetId = this.context.parameters['ptcpntVetId'];
        var ptcpntBeneId = this.context.parameters['ptcpntBeneId'];
        var ptcpntRecipId = this.context.parameters['ptcpntRecipId'];
        var awardTypeCd = this.context.parameters['awardTypeCd'];

        if (ptcpntVetId && ptcpntVetId !== '') {
            vetXml = '<ptcpntVetId>' + ptcpntVetId + '</ptcpntVetId>';
        } else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'There must be a participant vet id present for the request';
            return null;
        }

        if (ptcpntBeneId && ptcpntBeneId !== '') {
            beneXml = '<ptcpntBeneId>' + ptcpntBeneId + '</ptcpntBeneId>';
        } else {
            //        this.wsMessage.errorFlag = true;
            //        this.wsMessage.description = 'There must be a participant beneficiary id present for the request';
            //        return null;
        }

        if (ptcpntRecipId && ptcpntRecipId !== '') {
            recipXml = '<ptpcntRecipId>' + ptcpntRecipId + '</ptpcntRecipId>';
        } else {
            //        this.wsMessage.errorFlag = true;
            //        this.wsMessage.description = 'There must be a participant recipient id present for the request';
            //        return null;
        }

        if (awardTypeCd && awardTypeCd !== '') {
            awardXml = '<awardTypeCd>' + awardTypeCd + '</awardTypeCd>';
        } else {
        }

        if ((vetXml && vetXml !== '')
        ) {
            innerXml = '<ser:findGeneralInformationByPtcpntIds>'
                + vetXml + beneXml + recipXml + awardXml
                + '</ser:findGeneralInformationByPtcpntIds>';
        }

        return innerXml;
    };
    window.findGeneralInformationByPtcpntIds = findGeneralInformationByPtcpntIds;
    //END findGeneralInformationByPtcpntIds
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START findOtherAwardInformation
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:findOtherAwardInformation>
    //         <!--Optional:-->
    //         <ptcpntVetId>?</ptcpntVetId>
    //         <!--Optional:-->
    //         <ptcpntBeneId>?</ptcpntBeneId>
    //         <!--Optional:-->
    //         <ptcpntRecipId>?</ptcpntRecipId>
    //         <!--Optional:-->
    //         <awardTypeCd>?</awardTypeCd>
    //         <!--Optional:-->       
    //      </ser:findOtherAwardInformation>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    //TODO: change from SOAP to REST
    var findOtherAwardInformation = function (context) {
        this.context = context;


        this.serviceName = 'findOtherAwardInformation';
        this.responseFieldSchema = 'va_findotherawardinformationresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'ClaimantService.findOtherAwardInformation';
        this.wsMessage.serviceName = 'findOtherAwardInformation';
        this.wsMessage.friendlyServiceName = 'Other Award Information';


        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['ptcpntVetId'] = null;
        this.requiredSearchParameters['ptcpntBeneId'] = null;
        this.requiredSearchParameters['ptcpntRecipId'] = null;
        this.requiredSearchParameters['awardTypeCd'] = null;
        this.ignoreRequiredParMissingWarning = true;
    };
    findOtherAwardInformation.prototype = new claimantWebService;
    findOtherAwardInformation.prototype.constructor = findOtherAwardInformation;
    findOtherAwardInformation.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

        var innerXml;
        var vetXml;
        var beneXml;
        var recipXml;
        var awardXml;

        var ptcpntVetId = this.context.parameters['ptcpntVetId'];
        var ptcpntBeneId = this.context.parameters['ptcpntBeneId'];
        var ptcpntRecipId = this.context.parameters['ptcpntRecipId'];
        var awardTypeCd = this.context.parameters['awardTypeCd'];

        if (ptcpntVetId && ptcpntVetId !== '') {
            vetXml = '<ptcpntVetId>' + ptcpntVetId + '</ptcpntVetId>';
        } else {
            this.wsMessage.warningFlag = true;
            this.wsMessage.description = 'There must be a participant vet id present for the request';
            return null;
        }

        if (ptcpntBeneId && ptcpntBeneId !== '') {
            beneXml = '<ptcpntBeneId>' + ptcpntBeneId + '</ptcpntBeneId>';
        } else {
            this.wsMessage.warningFlag = true;
            this.wsMessage.description = 'There must be a participant beneficiary id present for the request';
            return null;
        }

        if (ptcpntRecipId && ptcpntRecipId !== '') {
            recipXml = '<ptcpntRecipId>' + ptcpntRecipId + '</ptcpntRecipId>';
        } else {
            this.wsMessage.warningFlag = true;
            this.wsMessage.description = 'There must be a participant recipient id present for the request';
            return null;
        }

        if (awardTypeCd && awardTypeCd !== '') {
            awardXml = '<awardTypeCd>' + awardTypeCd + '</awardTypeCd>';

        } else {
            this.wsMessage.warningFlag = true;
            this.wsMessage.description = 'There must be an award type code present for the request';
            return null;
        }

        if ((vetXml && vetXml !== '')
            && (beneXml && beneXml !== '')
            && (recipXml && recipXml !== '')
            && (awardTypeCd && awardTypeCd !== '')) {
            innerXml = '<ser:findOtherAwardInformation>'
                + vetXml + beneXml + recipXml + awardXml
                + '</ser:findOtherAwardInformation>';
        }

        return innerXml;
    };
    window.findOtherAwardInformation = findOtherAwardInformation;
    //END findOtherAwardInformation
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START findIncomeExpense
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:findIncomeExpense>
    //         <!--Optional:-->
    //         <ptcpntVetId>?</ptcpntVetId>
    //         <!--Optional:-->
    //         <ptcpntBeneId>?</ptcpntBeneId>
    //         <!--Optional:-->
    //      </ser:findIncomeExpense>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    //TODO: change from SOAP to REST
    var findIncomeExpense = function (context) {
        this.context = context;

        this.serviceName = 'findIncomeExpense';
        this.responseFieldSchema = 'va_findincomeexpenseresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'ClaimantService.findIncomeExpense';
        this.wsMessage.serviceName = 'findIncomeExpense';
        this.wsMessage.friendlyServiceName = 'Income/Expense';

        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['ptcpntVetId'] = null;
    };
    findIncomeExpense.prototype = new claimantWebService;
    findIncomeExpense.prototype.constructor = findIncomeExpense;
    findIncomeExpense.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

        var innerXml;
        var ptcpntVetId = this.context.parameters['ptcpntVetId'];
        var ptcpntBeneId = this.context.parameters['ptcpntBeneId'];

        var vetXml;
        var beneXml = '';

        if (ptcpntVetId && ptcpntVetId !== '') {
            vetXml = '<ptcpntVetId>' + ptcpntVetId + '</ptcpntVetId>';
        } else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'There must be a participant vet id present for the request';
            return null;
        }

        if (ptcpntBeneId && ptcpntBeneId !== '') {
            beneXml = '<ptcpntBeneId>' + ptcpntBeneId + '</ptcpntBeneId> ';
        } else {
        }

        if ((vetXml && vetXml !== '')) {
            innerXml = '<ser:findIncomeExpense>' + vetXml + beneXml + '</ser:findIncomeExpense>';
        }

        return innerXml;
    };
    window.findIncomeExpense = findIncomeExpense;
    //END findIncomeExpense
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START findAllFiduciaryPoa
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:findAllFiduciaryPoa>
    //         <!--Optional:-->
    //         <fileNumber>?</fileNumber>
    //         <!--Optional:-->
    //      </ser:findAllFiduciaryPoa>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    //TODO: change from SOAP to REST
    var findAllFiduciaryPoa = function (context) {
        this.context = context;

        this.serviceName = 'findAllFiduciaryPoa';
        this.responseFieldSchema = 'va_findfiduciarypoaresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'ClaimantService.findAllFiduciaryPoa';
        this.wsMessage.serviceName = 'findAllFiduciaryPoa';
        this.wsMessage.friendlyServiceName = 'Fiduciary/Poa';

        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['fileNumber'] = null;
    };
    findAllFiduciaryPoa.prototype = new claimantWebService;
    findAllFiduciaryPoa.prototype.constructor = findAllFiduciaryPoa;
    findAllFiduciaryPoa.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

        var innerXml;
        var fileNumber = this.context.parameters['fileNumber'];

        if (fileNumber && fileNumber !== '') {
            innerXml = '<ser:findAllFiduciaryPoa><fileNumber>' + fileNumber
                + '</fileNumber></ser:findAllFiduciaryPoa>';
        } else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'There must be a file number present for the request';
            return null;
        }

        return innerXml;
    };
    window.findAllFiduciaryPoa = findAllFiduciaryPoa;
    //END findAllFiduciaryPoa
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START findAllFiduciaryPoaAward
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //<soapenv:Body>
    //      <ser:findFiduciary>
    //         <fileNumber>555668888</fileNumber>
    //       
    //      </ser:findFiduciary>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    //TODO: change from SOAP to REST
    var findFiduciary = function (context) {
        this.context = context;

        this.serviceName = 'findFiduciary';
        this.responseFieldSchema = 'va_awardfiduciaryresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'ClaimantService.findFiduciary';
        this.wsMessage.serviceName = 'findFiduciary';
        this.wsMessage.friendlyServiceName = 'Award Fiduciary';

        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['payeeSSN'] = null;
    };
    findFiduciary.prototype = new claimantWebService;
    findFiduciary.prototype.constructor = findFiduciary;
    findFiduciary.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

        var innerXml;
        var fileNumber = this.context.parameters['payeeSSN'];

        if (fileNumber && fileNumber !== '') {
            innerXml = '<ser:findFiduciary><fileNumber>' + fileNumber
                + '</fileNumber></ser:findFiduciary>';
        } else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'There must be a file number present for the request';
            return null;
        }

        return innerXml;
    };
    window.findFiduciary = findFiduciary;
    //END findFiduciary
    //=====================================================================================================
    //=====================================================================================================
    //START findDependents
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:findDependents>
    //         <!--Optional:-->
    //         <fileNumber>?</fileNumber>
    //         <!--Optional:-->
    //      </ser:findDependents>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    //TODO: change from SOAP to REST
    var findDependents = function (context) {
        this.context = context;

        this.serviceName = 'findDependents';
        this.responseFieldSchema = 'va_finddependentsresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'ClaimantService.findDependents';
        this.wsMessage.serviceName = 'findDependents';
        this.wsMessage.friendlyServiceName = 'Dependents';


        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['fileNumber'] = null;

        this.ignoredExceptionMessage = 'No records found.';
    };
    findDependents.prototype = new claimantWebService;
    findDependents.prototype.constructor = findDependents;
    findDependents.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

        var innerXml;
        var fileNumber = this.context.parameters['fileNumber'];

        if (fileNumber && fileNumber !== '') {
            innerXml = '<ser:findDependents><fileNumber>' + fileNumber
                + '</fileNumber></ser:findDependents>';
        } else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'There must be a file number present for the request';
            return null;
        }

        return innerXml;
    };
    window.findDependents = findDependents;
    //END findDependents
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //=====================================================================================================
    //=====================================================================================================
    //START findAllRelationships
    //TODO: change from SOAP to REST
    var findAllRelationships = function (context) {
        this.context = context;

        this.serviceName = 'findAllRelationships';
        this.responseFieldSchema = 'va_findallrelationshipsresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'ClaimantService.findAllRelationships';
        this.wsMessage.serviceName = 'findAllRelationships';
        this.wsMessage.friendlyServiceName = 'AllRelationships';


        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['ptcpntId'] = null;

        this.ignoredExceptionMessage = 'No records found.';
    };
    findAllRelationships.prototype = new claimantWebService;
    findAllRelationships.prototype.constructor = findAllRelationships;
    findAllRelationships.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

        var innerXml;
        var pid = this.context.parameters['ptcpntId'];

        if (pid && pid !== '') {
            innerXml = '<ser:findAllRelationships><ptcpntId>' + pid + '</ptcpntId></ser:findAllRelationships>';
        } else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'There must be a ptcpntId present for the request';
            return null;
        }

        return innerXml;
    };
    window.findAllRelationships = findAllRelationships;
    //END findDependents
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //START findMilitaryRecordByPtcpntId
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:findMilitaryRecordByPtcpntId>
    //         <!--Optional:-->
    //         <ptcpntId>?</ptcpntId>
    //         <!--Optional:-->
    //      </ser:findMilitaryRecordByPtcpntId>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    //TODO: change from SOAP to REST
    var findMilitaryRecordByPtcpntId = function (context) {
        this.context = context;

        this.serviceName = 'findMilitaryRecordByPtcpntId';
        this.responseFieldSchema = 'va_findmilitaryrecordbyptcpntidresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'ClaimantService.findMilitaryRecordByPtcpntId';
        this.wsMessage.serviceName = 'findMilitaryRecordByPtcpntId';
        this.wsMessage.friendlyServiceName = 'Military Record';

        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['ptcpntId'] = null;
    };
    findMilitaryRecordByPtcpntId.prototype = new claimantWebService;
    findMilitaryRecordByPtcpntId.prototype.constructor = findMilitaryRecordByPtcpntId;
    findMilitaryRecordByPtcpntId.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

        var innerXml;
        var ptcpntId = this.context.parameters['ptcpntId'];

        if (ptcpntId && ptcpntId !== '') {
            innerXml = '<ser:findMilitaryRecordByPtcpntId><ptcpntId>' + ptcpntId + '</ptcpntId>'
                + '</ser:findMilitaryRecordByPtcpntId>';
        } else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'There must be a participant id present for the request';
            return null;
        }


        return innerXml;
    };
    window.findMilitaryRecordByPtcpntId = findMilitaryRecordByPtcpntId;
    //END findMilitaryRecordByPtcpntId
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START findDenialsByPtcpntId
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:findDenialsByPtcpntId>
    //         <!--Optional:-->
    //         <ptcpntId>?</ptcpntId>
    //         <!--Optional:-->
    //      </ser:findDenialsByPtcpntId>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    //TODO: change from SOAP to REST
    var findDenialsByPtcpntId = function (context) {
        this.context = context;

        this.serviceName = 'findDenialsByPtcpntId';
        this.responseFieldSchema = 'va_finddenialsresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'ClaimantService.findDenialsByPtcpntId';
        this.wsMessage.serviceName = 'findDenialsByPtcpntId';
        this.wsMessage.friendlyServiceName = 'Denials';

        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['ptcpntId'] = null;
    };
    findDenialsByPtcpntId.prototype = new claimantWebService;
    findDenialsByPtcpntId.prototype.constructor = findDenialsByPtcpntId;
    findDenialsByPtcpntId.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

        var innerXml;
        var ptcpntId = this.context.parameters['ptcpntId'];

        if (ptcpntId && ptcpntId !== '') {
            innerXml = '<ser:findDenialsByPtcpntId><ptcpntId>' + ptcpntId + '</ptcpntId>'
                + '</ser:findDenialsByPtcpntId>';
        } else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'There must be a participant id present for the request';
            return null;
        }

        return innerXml;
    };
    window.findDenialsByPtcpntId = findDenialsByPtcpntId;
    //END findDenialsByPtcpntId
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START findMonthOfDeath
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:findMonthOfDeath>
    //         <!--Optional:-->
    //         <fileNumber>?</fileNumber>
    //         <!--Optional:-->
    //      </ser:findMonthOfDeath>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    //TODO: change from SOAP to REST
    var findMonthOfDeath = function (context) {
        this.context = context;

        this.serviceName = 'findMonthOfDeath';
        this.responseFieldSchema = 'va_findmonthofdeathresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'ClaimantService.findMonthOfDeath';
        this.wsMessage.serviceName = 'findMonthOfDeath';
        this.wsMessage.friendlyServiceName = 'Month Of Death';

        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['fileNumber'] = null;

        this.ignoredExceptionMessage = 'Not Eligible';
    };
    findMonthOfDeath.prototype = new claimantWebService;
    findMonthOfDeath.prototype.constructor = findMonthOfDeath;
    findMonthOfDeath.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

        var innerXml;
        var fileNumber = this.context.parameters['fileNumber'];

        if (fileNumber && fileNumber !== '') {
            innerXml = '<ser:findMonthOfDeath><fileNumber>' + fileNumber + '</fileNumber>'
                + '</ser:findMonthOfDeath>';
        } else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'There must be a File Number present for the request';
            return null;
        }

        return innerXml;
    };
    window.findMonthOfDeath = findMonthOfDeath;
    //END findMonthOfDeath
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START findReasonsByRbaIssueId
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:findReasonsByRbaIssueId>
    //         <!--Optional:-->
    //         <rbaIssueId>?</rbaIssueId>
    //         <!--Optional:-->
    //      </ser:findReasonsByRbaIssueId>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    //TODO: change from SOAP to REST
    var findReasonsByRbaIssueId = function (context) {
        this.context = context;

        this.serviceName = 'findReasonsByRbaIssueId';
        this.responseFieldSchema = 'va_findreasonsbyrbaissueidresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'ClaimantService.findReasonsByRbaIssueId';
        this.wsMessage.serviceName = 'findReasonsByRbaIssueId';
        this.wsMessage.friendlyServiceName = 'Reasons';

        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['rbaIssueId'] = null;
    };

    findReasonsByRbaIssueId.prototype = new claimantWebService;
    findReasonsByRbaIssueId.prototype.constructor = findReasonsByRbaIssueId;

    findReasonsByRbaIssueId.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

        var innerXml;
        var rbaIssueId = this.context.parameters['rbaIssueId'];

        if (rbaIssueId && rbaIssueId !== '') {
            innerXml = '<ser:findReasonsByRbaIssueId><rbaIssueId>' + rbaIssueId + '</rbaIssueId>'
                + '</ser:findReasonsByRbaIssueId>';
        } else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'There must be an rba issue id present for the request';
            return null;
        }

        return innerXml;
    };
    window.findReasonsByRbaIssueId = findReasonsByRbaIssueId;
    //END findReasonsByRbaIssueId
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START findAwardAddresses
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:findAwardAddresses>
    //         <!--Optional:-->
    //         <fileNumber>?</fileNumber>
    //         <!--Optional:-->
    //      </ser:findAwardAddresses>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    //TODO: change from SOAP to REST
    var findAwardAddresses = function (context) {
        this.context = context;

        this.serviceName = 'findAwardAddresses';
        this.responseFieldSchema = 'va_findawardaddressesresponse';
        this.responseTimestamp = 'va_webserviceresponse';

        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'ClaimantService.findAwardAddresses';
        this.wsMessage.serviceName = 'findAwardAddresses';
        this.wsMessage.friendlyServiceName = 'Award Addresses';

        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['fileNumber'] = null;
    };
    findAwardAddresses.prototype = new claimantWebService;
    findAwardAddresses.prototype.constructor = findAwardAddresses;
    findAwardAddresses.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

        var innerXml;
        var fileNumber = this.context.parameters['fileNumber'];

        if (fileNumber && fileNumber !== '') {
            innerXml = '<ser:findAwardAddresses><fileNumber>' + fileNumber + '</fileNumber></ser:findAwardAddresses>';

        } else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'There must be a File Number present for this request';
            return null;
        }


        return innerXml;
    };
    window.findAwardAddresses = findAwardAddresses;
    //END findAwardAddresses
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
    //START updateMonthOfDeath
    //EX Request:
    //<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
    //   <soapenv:Header/>
    //   <soapenv:Body>
    //      <ser:updateMonthOfDeath>
    //         <!--Optional:-->
    //         <fileNumber>?</fileNumber>
    //         <!--Optional:-->
    //      </ser:updateMonthOfDeath>
    //   </soapenv:Body>
    //</soapenv:Envelope>
    //TODO: change from SOAP to REST
    var updateMonthOfDeath = function (context) {
        this.context = context;

        this.serviceName = 'updateMonthOfDeath';
        this.responseFieldSchema = 'va_updatemonthofdeathresponse';
        this.responseTimestamp = 'va_webserviceresponse';
        this.prefix = 'ser';

        this.wsMessage = new webServiceMessage();
        this.wsMessage.methodName = 'ClaimantService.updateMonthOfDeath';
        this.wsMessage.serviceName = 'updateMonthOfDeath';
        this.wsMessage.friendlyServiceName = 'Month Of Death';

        this.requiredSearchParameters = new Array();
        this.requiredSearchParameters['fileNumber'] = null;

        this.ignoredExceptionMessage = 'Not Eligible';
    };
    updateMonthOfDeath.prototype = new claimantWebService;
    updateMonthOfDeath.prototype.constructor = updateMonthOfDeath;
    updateMonthOfDeath.prototype.buildSoapBodyInnerXml = function () {
        this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

        var innerXml;
        var cntrl_Mod_Tran_Id = this.context.parameters['cntrl_Mod_Tran_Id']; //2041
        var mod_Procs_Type_Cd = this.context.parameters['mod_Procs_Type_Cd']; //BMOD
        var isNationalOrStateCemetery = this.context.parameters["isNationalOrStateCemetery"];
        var mod_Letter_Type_Cd = this.context.parameters['mod_Letter_Type_Cd']; //VET
        var fileNumber = this.context.parameters['fileNumber'];
        var spouseSSNNumber = this.context.parameters['spouseSSNNumber'];
        var stationNumber = this.context.parameters['stationNumber'];
        var veteranParticipantID = this.context.parameters['veteranParticipantID'];
        var beneParticipantID = this.context.parameters['beneParticipantID'];
        var letterRecipientID = this.context.parameters['letterRecipientID'];
        var spouseFirstName = this.context.parameters['spouseFirstName'];
        var spouseMiddleName = this.context.parameters['spouseMiddleName'];
        var spouseLastName = this.context.parameters['spouseLastName'];
        var spouseSuffixName = this.context.parameters['spouseSuffixName'];
        var normalizedAddressLine1 = this.context.parameters['normalizedAddressLine1'];
        var normalizedAddressLine2 = this.context.parameters['normalizedAddressLine2'];
        var normalizedAddressLine3 = this.context.parameters['normalizedAddressLine3'];

        var treasuryMailingAddressLine1 = this.context.parameters['treasuryMailingAddressLine1'];
        var treasuryMailingAddressLine2 = this.context.parameters['treasuryMailingAddressLine2'];
        var treasuryMailingAddressLine3 = this.context.parameters['treasuryMailingAddressLine3'];
        var treasuryMailingAddressLine4 = this.context.parameters['treasuryMailingAddressLine4'];
        var treasuryMailingAddressLine5 = this.context.parameters['treasuryMailingAddressLine5'];
        var treasuryMailingAddressLine6 = this.context.parameters['treasuryMailingAddressLine6'];

        var militaryPostalTypeCode = this.context.parameters['militaryPostalTypeCode'];
        var militaryPostalOfficeTypeCode = this.context.parameters['militaryPostalOfficeTypeCode'];
        var provinceName = this.context.parameters['provinceName'];
        var territoryName = this.context.parameters['territoryName'];

        var spouseForeignPostalCode = this.context.parameters['spouseForeignPostalCode'];

        var stateCode = this.context.parameters['stateCode'];
        var zipCode = this.context.parameters['zipCode'];
        var spouseBirthDate = this.context.parameters['spouseBirthDate'];
        var cityName = this.context.parameters['cityName'];
        var spouseChangeInd = this.context.parameters['spouseChangeInd']; // M,A, or N for no change
        var countryName = this.context.parameters['countryName'];

        if (fileNumber && fileNumber !== '') { 
            innerXml = '<ser:updateMonthOfDeath><modUpdateInput>' +
                '<cntrl_Mod_Tran_Id>' + NN(cntrl_Mod_Tran_Id) + '</cntrl_Mod_Tran_Id>' +
                '<mod_Procs_Type_Cd>' + NN(mod_Procs_Type_Cd) + '</mod_Procs_Type_Cd>' +
                '<nationalStateCemetery>' + NN(isNationalOrStateCemetery) + '</nationalStateCemetery>' +
                '<mod_Letter_Type_Cd>' + NN(mod_Letter_Type_Cd) + '</mod_Letter_Type_Cd>' +
                '<fileNumber>' + NN(fileNumber) + '</fileNumber>' +
                '<spouseSSNNumber>' + NN(spouseSSNNumber) + '</spouseSSNNumber>' +
                '<stationNumber>' + NN(stationNumber) + '</stationNumber>' +
                '<veteranParticipantID>' + NN(veteranParticipantID) + '</veteranParticipantID>' +
                '<beneParticipantID>' + NN(beneParticipantID) + '</beneParticipantID>' +
                '<letterRecipientID>' + NN(letterRecipientID) + '</letterRecipientID>' +
                '<spouseFirstName>' + NN(spouseFirstName) + '</spouseFirstName>' +
                '<spouseMiddleName>' + NN(spouseMiddleName) + '</spouseMiddleName>' +
                '<spouseLastName>' + NN(spouseLastName) + '</spouseLastName>' +
                ((typeof spouseSuffixName !== 'undefined' && spouseSuffixName.length > 0) ?
                    ('<spouseSuffixName>' + NN(spouseSuffixName) + '</spouseSuffixName>') : '') +
                '<normalizedAddressLine1>' + NN(normalizedAddressLine1) + '</normalizedAddressLine1>';

            if (normalizedAddressLine2 || normalizedAddressLine3) {
                innerXml += '<normalizedAddressLine2>' + NN(normalizedAddressLine2) + '</normalizedAddressLine2>' +
                    '<normalizedAddressLine3>' + NN(normalizedAddressLine3) + '</normalizedAddressLine3>';
            }

            innerXml += '<treasuryMailingAddressLine1>' + NN(treasuryMailingAddressLine1) + '</treasuryMailingAddressLine1>' +
                '<treasuryMailingAddressLine2>' + NN(treasuryMailingAddressLine2) + '</treasuryMailingAddressLine2>' +
                '<treasuryMailingAddressLine3>' + NN(treasuryMailingAddressLine3) + '</treasuryMailingAddressLine3>';

            if (treasuryMailingAddressLine4) innerXml += '<treasuryMailingAddressLine4>' + NN(treasuryMailingAddressLine4) + '</treasuryMailingAddressLine4>';
            if (treasuryMailingAddressLine5) innerXml += '<treasuryMailingAddressLine5>' + NN(treasuryMailingAddressLine5) + '</treasuryMailingAddressLine5>';
            if (treasuryMailingAddressLine6) innerXml += '<treasuryMailingAddressLine6>' + NN(treasuryMailingAddressLine6) + '</treasuryMailingAddressLine6>';

            if (militaryPostalTypeCode) innerXml += '<militaryPostalTypeCode>' + NN(militaryPostalTypeCode) + '</militaryPostalTypeCode>';
            if (militaryPostalOfficeTypeCode) innerXml += '<militaryPostalOfficeTypeCode>' + NN(militaryPostalOfficeTypeCode) + '</militaryPostalOfficeTypeCode>';
            if (provinceName) innerXml += '<provinceName>' + NN(provinceName) + '</provinceName>';
            if (territoryName) innerXml += '<territoryName>' + NN(territoryName) + '</territoryName>';

            //this is per requirements.. when the data is updated, one must set the foreignMailCode field, when retrieved - read from spouseForeignPostalCode field
            if (spouseForeignPostalCode) innerXml += '<foreignMailCode>' + NN(spouseForeignPostalCode) + '</foreignMailCode>';

            spouseBirthDate = NN(spouseBirthDate);
            if (spouseBirthDate === '') {
                spouseBirthDate = '        ';
            }
            innerXml += '<stateCode>' + NN(stateCode) + '</stateCode>' +
                '<zipCode>' + NN(zipCode) + '</zipCode>' +
                '<spouseBirthDate>' + NN(spouseBirthDate) + '</spouseBirthDate>' +
                '<cityName>' + NN(cityName) + '</cityName>' +
                '<spouseChangeInd>' + NN(spouseChangeInd) + '</spouseChangeInd>' +
                '<countryName>' + NN(countryName) + '</countryName>' +
                '<group1OverrideInd/>' +
                '</modUpdateInput></ser:updateMonthOfDeath>';
        } else {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'There must be a File Number present for the request';
            return null;
        }

        return innerXml;
    };
    window.updateMonthOfDeath = updateMonthOfDeath;
    //END updateMonthOfDeath
    //=====================================================================================================
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    /*****************************************************************************************************/
    //=====================================================================================================
};

function NN(s) { return (s === null ? '' : s); }