//=====================================================================================================
// START AddressWebService
//=====================================================================================================
var addressWebService = function (context) {
// CSDev Left Intentionally Blank 
}
addressWebService.prototype = new webservice;
addressWebService.prototype.constructor = addressWebService;
//=====================================================================================================
// START Individual AddressWebService Methods
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
//=====================================================================================================
// START findAllPtcpntAddrsByPtcpntId
//EX Request:
//<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:add="http://address.services.vetsnet.vba.va.gov/">
//   <soapenv:Header/>
//   <soapenv:Body>
//      <add:findAllPtcpntAddrsByPtcpntId>
//         <!--Optional:-->
//         <ptcpntId>?</ptcpntId>
//      </add:findAllPtcpntAddrsByPtcpntId>
//   </soapenv:Body>
//</soapenv:Envelope>
var findAllPtcpntAddrsByPtcpntId = function (context) {
// CSDev Left Intentionally Blank 
}
findAllPtcpntAddrsByPtcpntId.prototype = new addressWebService;
findAllPtcpntAddrsByPtcpntId.prototype.constructor = findAllPtcpntAddrsByPtcpntId;
findAllPtcpntAddrsByPtcpntId.prototype.buildSoapBodyInnerXml = function () {
// CSDev Left Intentionally Blank 
}
// END findAllPtcpntAddrsByPtcpntId
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
// END Individual AddressWebService Methods
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
// END AddressWebService
//=====================================================================================================