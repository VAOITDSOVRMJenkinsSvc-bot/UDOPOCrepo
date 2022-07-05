//=====================================================================================================
// START MapDClaimManagementService
//=====================================================================================================
var mapDClaimManagementService = function (context) {
// CSDev Left Intentionally Blank 
}
mapDClaimManagementService.prototype = new webservice;
mapDClaimManagementService.prototype.constructor = mapDClaimManagementService;
//=====================================================================================================
// START Individual MapDClaimManagementService Methods
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
//=====================================================================================================
//START findClaimStatus
//EX Request:
//<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.mapd.benefits.vba.va.gov/">
//   <soapenv:Header/>
//   <soapenv:Body>
//      <ser:findClaimStatus>
//         <!--Optional:-->
//         <claimId>?</claimId>
//      </ser:findClaimStatus>
//   </soapenv:Body>
//</soapenv:Envelope>
var findClaimStatus = function (context) {
// CSDev Left Intentionally Blank 
}
findClaimStatus.prototype = new mapDClaimManagementService;
findClaimStatus.prototype.constructor = mapDClaimManagementService;
findClaimStatus.prototype.buildSoapBodyInnerXml = function () {
// CSDev Left Intentionally Blank 
}
// END findClaimStatus
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
// END Individual MapDClaimManagementService Methods
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
// END MapDClaimManagementService
//=====================================================================================================