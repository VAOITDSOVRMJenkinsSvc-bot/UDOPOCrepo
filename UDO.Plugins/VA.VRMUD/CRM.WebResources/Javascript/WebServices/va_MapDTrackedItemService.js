//=====================================================================================================
// START MapDTrackedItemService
//=====================================================================================================
var mapDTrackedItemService = function (context) {
    this.context = context;
    this.webserviceRequestUrl = WebServiceURLRoot + 'TrackedItemService/TrackedItemService';
    this.prefix = 'ser';
    this.prefixUrl = 'http://services.mapd.benefits.vba.va.gov/';
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
    this.context = context;

    this.serviceName = 'findUnsolEvdnce';
    this.wsMessage = new webServiceMessage();
    this.wsMessage.methodName = 'MapDTrackedItemService.findUnsolEvdnce';
    this.wsMessage.serviceName = 'findUnsolEvdnce';
    this.wsMessage.friendlyServiceName = 'Unsolicited Evidence';
    this.responseFieldSchema = 'va_findunsolvedevidenceresponse';
    this.responseTimestamp = 'va_webserviceresponse';

    this.requiredSearchParameters = new Array();
    this.requiredSearchParameters['ptcpntId'] = null;
}
findUnsolEvdnce.prototype = new mapDTrackedItemService;
findUnsolEvdnce.prototype.constructor = findUnsolEvdnce;
findUnsolEvdnce.prototype.buildSoapBodyInnerXml = function () {
    this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
    var innerXml;

    var ptcpntId = this.context.parameters['ptcpntId'];

    if (ptcpntId && ptcpntId != '') {
        innerXml = '<ser:findUnsolEvdnce>'
            + '<Claiment_ptpcpnt_id>' + ptcpntId + '</Claiment_ptpcpnt_id></ser:findUnsolEvdnce>';
    }
    else {
        this.wsMessage.errorFlag = true;
        this.wsMessage.description = 'There must be a participant id present for the request';
        return null;
    }

    return innerXml;
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
    this.context = context;

    this.serviceName = 'findTrackedItems';
    this.wsMessage = new webServiceMessage();
    this.wsMessage.methodName = 'MapDTrackedItemService.findTrackedItems';
    this.wsMessage.serviceName = 'findTrackedItems';
    this.wsMessage.friendlyServiceName = 'Tracked Items';
    this.responseFieldSchema = 'va_findtrackeditemsresponse';
    this.responseTimestamp = 'va_webserviceresponse';

    this.requiredSearchParameters = new Array();
    this.requiredSearchParameters['claimId'] = null;
}
findTrackedItems.prototype = new mapDTrackedItemService;
findTrackedItems.prototype.constructor = findTrackedItems;
findTrackedItems.prototype.buildSoapBodyInnerXml = function () {
    this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
    var innerXml;

    var claimId = this.context.parameters['claimId'];

    if (claimId && claimId != '') {
        innerXml = '<ser:findTrackedItems>'
            + '<claimId>' + claimId + '</claimId></ser:findTrackedItems>';
    }
    else {
        this.wsMessage.errorFlag = true;
        this.wsMessage.description = 'There must be a participant id present for the request';
        return null;
    }

    return innerXml;
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