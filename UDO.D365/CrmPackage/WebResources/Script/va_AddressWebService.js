//=====================================================================================================
// START AddressWebService
//=====================================================================================================
var addressWebService = function (context) {
    this.context = context;
    this.webserviceRequestUrl = WebServiceURLRoot + 'AddressWebServiceBean/AddressWebService';

    this.prefix = 'add';
    this.prefixUrl = 'http://address.services.vetsnet.vba.va.gov/';
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
    this.context = context;

    this.serviceName = 'findAllPtcpntAddrsByPtcpntId';
    this.responseFieldSchema = 'va_findaddressresponse';
    this.responseTimestamp = 'va_webserviceresponse';

    this.wsMessage = new webServiceMessage();
    this.wsMessage.methodName = 'AddressWebService.findAllPtcpntAddrsByPtcpntId';
    this.wsMessage.serviceName = 'findAllPtcpntAddrsByPtcpntId';
    this.wsMessage.friendlyServiceName = 'Addresses';

    this.requiredSearchParameters = new Array();
    this.requiredSearchParameters['ptcpntId'] = null;
}
findAllPtcpntAddrsByPtcpntId.prototype = new addressWebService;
findAllPtcpntAddrsByPtcpntId.prototype.constructor = findAllPtcpntAddrsByPtcpntId;
findAllPtcpntAddrsByPtcpntId.prototype.buildSoapBodyInnerXml = function () {
    this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
    var innerXml;

    var ptcpntId = this.context.parameters['ptcpntId'];

    if (ptcpntId && ptcpntId != '') {
        innerXml = '<add:findAllPtcpntAddrsByPtcpntId><ptcpntId>' + ptcpntId
        + '</ptcpntId></add:findAllPtcpntAddrsByPtcpntId>';
    }
    else {
        this.wsMessage.errorFlag = true;
        this.wsMessage.description = 'There must be a Participant Id present for the request';
        return null;
    }

    return innerXml;
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