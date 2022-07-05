//=====================================================================================================
// START MapDContentionService
//=====================================================================================================
var mapDContentionService = function (context) {
// CSDev Left Intentionally Blank 
}
mapDContentionService.prototype = new webservice;
mapDContentionService.prototype.constructor = mapDContentionService;
//=====================================================================================================
// START Individual MapDContentionService Methods
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
//=====================================================================================================
//START findContentions
//EX Request:
//<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.mapd.benefits.vba.va.gov/">
//   <soapenv:Header/>
//   <soapenv:Body>
//      <ser:findContentions>
//         <!--Optional:-->
//         <claimId>?</claimId>
//      </ser:findContentions>
//   </soapenv:Body>
//</soapenv:Envelope>
//=====================================================================================================
var findContentions = function (context) {
// CSDev Left Intentionally Blank 
}
findContentions.prototype = new mapDContentionService;
findContentions.prototype.constructor = findContentions;
findContentions.prototype.buildSoapBodyInnerXml = function () {
// CSDev Left Intentionally Blank 
}
// END findContentions
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
//=====================================================================================================
//START findContentionsByPtcpntId
//EX Request:
//<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.mapd.benefits.vba.va.gov/">
//   <soapenv:Header/>
//   <soapenv:Body>
//      <ser:findContentionsByPtcpntId>
//         <!--Optional:-->
//         <ptcpntId>?</ptcpntId>
//      </ser:findContentionsByPtcpntId>
//   </soapenv:Body>
//</soapenv:Envelope>
//=====================================================================================================
var findContentionsByPtcpntId = function (context) {
// CSDev Left Intentionally Blank 
}
findContentionsByPtcpntId.prototype = new mapDContentionService;
findContentionsByPtcpntId.prototype.constructor = findContentionsByPtcpntId;
findContentionsByPtcpntId.prototype.buildSoapBodyInnerXml = function () {
// CSDev Left Intentionally Blank 
}
// END findContentionsByPtcpntId
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
// END Individual MapDContentionService Methods
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
// END MapDContentionService
//=====================================================================================================