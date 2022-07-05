//=====================================================================================================
// START MapDContentionService
//=====================================================================================================
var mapDContentionService = function (context) {
    this.context = context;
    this.webserviceRequestUrl = WebServiceURLRoot + 'ContentionService/ContentionService';
    this.prefix = 'ser';
    this.prefixUrl = 'http://services.mapd.benefits.vba.va.gov/';
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
    this.context = context;

    this.serviceName = 'findContentions';
    this.wsMessage = new webServiceMessage();
    this.wsMessage.methodName = 'MapDContentionService.findContentions';
    this.wsMessage.serviceName = 'findContentions';
    this.wsMessage.friendlyServiceName = 'Contentions';
    this.responseFieldSchema = 'va_findcontentionsresponse';
    this.responseTimestamp = 'va_webserviceresponse';

    this.requiredSearchParameters = new Array();
    this.requiredSearchParameters['claimId'] = null;
}
findContentions.prototype = new mapDContentionService;
findContentions.prototype.constructor = findContentions;
findContentions.prototype.buildSoapBodyInnerXml = function () {
    this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
    var innerXml;

    var claimId = this.context.parameters['claimId'];

    if (claimId && claimId != '') {
        innerXml = '<ser:findContentions><claimId>' + claimId + '</claimId></ser:findContentions>';
    }
    else {
        this.wsMessage.errorFlag = true;
        this.wsMessage.description = 'There must be a claim id present for the request';
        return null;
    }

    return innerXml;
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
    this.context = context;

    this.serviceName = 'findContentionsByPtcpntId';
    this.wsMessage = new webServiceMessage();
    this.wsMessage.methodName = 'MapDContentionService.findContentionsByPtcpntId';
    this.wsMessage.serviceName = 'findContentionsByPtcpntId';
    this.wsMessage.friendlyServiceName = 'Contentions';
    this.responseFieldSchema = 'va_findcontentionsresponse';
    this.responseTimestamp = 'va_webserviceresponse';

    this.requiredSearchParameters = new Array();
    this.requiredSearchParameters['ptcpntId'] = null;
    this.ignoreRequiredParMissingWarning = true;
}
findContentionsByPtcpntId.prototype = new mapDContentionService;
findContentionsByPtcpntId.prototype.constructor = findContentionsByPtcpntId;
findContentionsByPtcpntId.prototype.buildSoapBodyInnerXml = function () {
    this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
    var innerXml;

    var ptcpntId = this.context.parameters['ptcpntId'];

    if (ptcpntId && ptcpntId != '') {
        innerXml = '<ser:findContentionsByPtcpntId><ptcpntId>' + ptcpntId + '</ptcpntId></ser:findContentionsByPtcpntId>';
    }
    else {
        this.wsMessage.errorFlag = true;
        this.wsMessage.description = 'There must be a participant id present for the request';
        return null;
    }

    return innerXml;
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