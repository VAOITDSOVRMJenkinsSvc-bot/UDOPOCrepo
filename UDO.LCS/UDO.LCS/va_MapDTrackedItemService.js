//=====================================================================================================
// START MapDTrackedItemService
//=====================================================================================================
var mapDTrackedItemService = function (context) {
// CSDev Left Intentionally Blank 
}
mapDTrackedItemService.prototype = new webservice;
mapDTrackedItemService.prototype.constructor = mapDTrackedItemService;
//=====================================================================================================
// START Individual MapDTrackedItemService Methods
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
//=====================================================================================================
//START findUnsolEvdnce
//EX Request:
//<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.mapd.benefits.vba.va.gov/">
//   <soapenv:Header/>
//   <soapenv:Body>
//      <ser:findUnsolEvdnce>
//         <!--Optional:-->
//         <Claiment_ptpcpnt_id>?</Claiment_ptpcpnt_id>
//      </ser:findUnsolEvdnce>
//   </soapenv:Body>
//</soapenv:Envelope>
var findUnsolEvdnce = function (context) {
// CSDev Left Intentionally Blank 
}
findUnsolEvdnce.prototype = new mapDTrackedItemService;
findUnsolEvdnce.prototype.constructor = findUnsolEvdnce;
findUnsolEvdnce.prototype.buildSoapBodyInnerXml = function () {
// CSDev Left Intentionally Blank 
}
//END findUnsolEvdnce
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
//=====================================================================================================
//START findTrackedItems
//EX Request:
//<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.mapd.benefits.vba.va.gov/">
//   <soapenv:Header/>
//   <soapenv:Body>
//      <ser:findTrackedItems>
//         <!--Optional:-->
//         <claimId>?</claimId>
//      </ser:findTrackedItems>
//   </soapenv:Body>
//</soapenv:Envelope>
var findTrackedItems = function (context) {
// CSDev Left Intentionally Blank 
}
findTrackedItems.prototype = new mapDTrackedItemService;
findTrackedItems.prototype.constructor = findTrackedItems;
findTrackedItems.prototype.buildSoapBodyInnerXml = function () {
// CSDev Left Intentionally Blank 
}
//END findTrackedItems
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
// END Individual MapDTrackedItemService Methods
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
// END MapDTrackedItemService
//=====================================================================================================