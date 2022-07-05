﻿//=====================================================================================================
// START MapDDocumentService
//=====================================================================================================
var mapDDocumentService = function (context) {
    this.context = context;
    this.webserviceRequestUrl = WebServiceURLRoot + 'DocumentService/DocumentService';
    this.prefix = 'ser';
    this.prefixUrl = 'http://services.mapd.benefits.vba.va.gov/';
}
mapDDocumentService.prototype = new webservice;
mapDDocumentService.prototype.constructor = mapDDocumentService;
//=====================================================================================================
// START Individual MapDDocumentService Methods
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
//=====================================================================================================
//START findClaimantLetters
//EX Request:
//<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.mapd.benefits.vba.va.gov/">
//   <soapenv:Header/>
//   <soapenv:Body>
//      <ser:findClaimantLetters>
//         <!--Optional:-->
//         <documentId>?</documentId>
//      </ser:findClaimantLetters>
//   </soapenv:Body>
//</soapenv:Envelope>
var findClaimantLetters = function (context) {
    this.context = context;

    this.serviceName = 'findClaimantLetters';
    this.wsMessage = new webServiceMessage();
    this.wsMessage.methodName = 'MapDDocumentService.findClaimantLetters';
    this.wsMessage.serviceName = 'findClaimantLetters';
    this.responseFieldSchema = 'va_findclaimantlettersresponse';
    this.responseTimestamp = 'va_webserviceresponse';

    this.requiredSearchParameters = new Array();
    this.requiredSearchParameters['documentId'] = null;
}
findClaimantLetters.prototype = new mapDDocumentService;
findClaimantLetters.prototype.constructor = findClaimantLetters;
findClaimantLetters.prototype.buildSoapBodyInnerXml = function () {
    this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
    var innerXml;

    var documentId = this.context.parameters['documentId'];

    if (documentId && documentId != '') {
        innerXml = '<ser:findClaimantLetters>'
            + '<documentId>' + documentId + '</documentId></ser:findClaimantLetters>';
    }
    else {
        this.wsMessage.errorFlag = true;
        this.wsMessage.description = 'There must be a document id present for the request';
        return null;
    }

    return innerXml;
}
//END findClaimantLetters

//START findThirdPartyLetters 
//      <ser:findThirdPartyLetters>
//         <documentId>?</documentId>
//      </ser:findThirdPartyLetters>
var findThirdPartyLetters = function (context) {
    this.context = context;

    this.serviceName = 'findThirdPartyLetters';
    this.wsMessage = new webServiceMessage();
    this.wsMessage.methodName = 'MapDDocumentService.findThirdPartyLetters';
    this.wsMessage.serviceName = 'findThirdPartyLetters';
    this.responseFieldSchema = 'va_findclaimantlettersresponse';
    this.responseTimestamp = 'va_webserviceresponse';

    this.requiredSearchParameters = new Array();
    this.requiredSearchParameters['documentId'] = null;
}
findThirdPartyLetters.prototype = new mapDDocumentService;
findThirdPartyLetters.prototype.constructor = findThirdPartyLetters;
findThirdPartyLetters.prototype.buildSoapBodyInnerXml = function () {
    this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
    var innerXml;

    var documentId = this.context.parameters['documentId'];

    if (documentId && documentId != '') {
        innerXml = '<ser:findThirdPartyLetters>' + '<documentId>' + documentId + '</documentId></ser:findThirdPartyLetters>';
    }
    else {
        this.wsMessage.errorFlag = true;
        this.wsMessage.description = 'There must be a document id present for the request';
        return null;
    }

    return innerXml;
}
//END findThirdPartyLetters

//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
// END Individual MapDDocumentService Methods
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
// END MapDDocumentService
//=====================================================================================================