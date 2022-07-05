//=====================================================================================================
// START MapDWorkAssignmentService
// Web Method List:  findCaseManagerByFileNumber
//
// EX:
//      var mapDWorkAssignmentServiceCtx = new actionContext();
//      mapDWorkAssignmentServiceCtx.user = GetUserSettingsForWebservice();
//
//      var findCaseManagerByFileNumber  = new  mapDWorkAssignmentService(mapDWorkAssignmentServiceCtx);
//      findCaseManagerByFileNumber.serviceName = 'findCaseManagerByFileNumber';
//      findCaseManagerByFileNumber.wsMessage.serviceName = 'findCaseManagerByFileNumber';
//      findCaseManagerByFileNumber.responseFieldSchema = 'va_findcasemanagerbyfilenumberresponse';
//      findCaseManagerByFileNumber.responseTimestamp = 'va_webserviceresponse';
//      findCaseManagerByFileNumber.executeRequest();
//=====================================================================================================
var mapDWorkAssignmentService = function (context) {
    this.context = context;
    this.webserviceRequestUrl = WebServiceURLRoot + 'WorkAssignmentService/WorkQueueService2';
    this.wsMessage.methodName = 'MapDWorkAssignmentService.mapDWorkAssignmentService';
    this.prefix = 'ser';
    this.prefixUrl = 'http://services.mapd.benefits.vba.va.gov/';
}
mapDWorkAssignmentService.prototype = new webservice;
mapDWorkAssignmentService.prototype.constructor = mapDWorkAssignmentService;
mapDWorkAssignmentService.prototype.buildSoapBodyInnerXml = function () {
    this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
    var innerXml;

    var fileNumber = this.context.fileNumber;

    if (fileNumber == null) {
        this.wsMessage.errorFlag = true;
        this.wsMessage.description = 'There must be a file number present for the request';
        return null;
    }

    innerXml = '<' + this.prefix + ':' + this.serviceName + '>'
    + '<fileNumber>' + fileNumber + '</fileNumber>'
    + '</' + this.prefix + ':' + this.serviceName + '>';

    return innerXml;
}
// END MapDWorkAssignmentService
//=====================================================================================================