﻿//=====================================================================================================
// START MapDDocumentService
//=====================================================================================================
var mapDDocumentService = function (context) {
// CSDev Left Intentionally Blank 
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
// CSDev Left Intentionally Blank 
}
findClaimantLetters.prototype = new mapDDocumentService;
findClaimantLetters.prototype.constructor = findClaimantLetters;
findClaimantLetters.prototype.buildSoapBodyInnerXml = function () {
// CSDev Left Intentionally Blank 
}
//END findClaimantLetters

//START findThirdPartyLetters 
//      <ser:findThirdPartyLetters>
//         <documentId>?</documentId>
//      </ser:findThirdPartyLetters>
var findThirdPartyLetters = function (context) {
// CSDev Left Intentionally Blank 
}
findThirdPartyLetters.prototype = new mapDDocumentService;
findThirdPartyLetters.prototype.constructor = findThirdPartyLetters;
findThirdPartyLetters.prototype.buildSoapBodyInnerXml = function () {
// CSDev Left Intentionally Blank 
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