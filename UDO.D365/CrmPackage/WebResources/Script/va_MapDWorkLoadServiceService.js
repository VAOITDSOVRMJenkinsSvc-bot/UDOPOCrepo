//=====================================================================================================
// START MapDWorkLoadServiceService
// Web Method List:  findUsrWorkLoad
//
// EX:
//      var mapDWorkLoadServiceServiceCtx = new actionContext();
//      mapDWorkLoadServiceServiceCtx.user = GetUserSettingsForWebservice();
//
//      var findUsrWorkLoad  = new  mapDWorkLoadServiceService(mapDWorkLoadServiceServiceCtx);
//      findUsrWorkLoad.serviceName = 'findUsrWorkLoad';
//      findUsrWorkLoad.wsMessage.serviceName = 'findUsrWorkLoad';
//      findUsrWorkLoad.responseFieldSchema = 'va_findusrworkloadresponse';
//      findUsrWorkLoad.responseTimestamp = 'va_webserviceresponse';
//      findUsrWorkLoad.executeRequest();
//=====================================================================================================
var mapDWorkLoadServiceService = function (context) {
    this.context = context;
    this.webserviceRequestUrl = WebServiceURLRoot + 'WorkLoadService/WorkQueueService1';
    this.wsMessage.methodName = 'MapDWorkLoadServiceService.mapDWorkLoadServiceService';
    this.prefix = 'ser';
    this.prefixUrl = 'http://services.mapd.benefits.vba.va.gov/';
}
mapDWorkLoadServiceService.prototype = new webservice;
mapDWorkLoadServiceService.prototype.constructor = mapDWorkLoadServiceService;
mapDWorkLoadServiceService.prototype.buildSoapBodyInnerXml = function () {
    this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
    var innerXml;

    innerXml = '<' + this.prefix + ':' + this.serviceName + '/>';

    return innerXml;
}
// END MapDWorkLoadServiceService
//=====================================================================================================