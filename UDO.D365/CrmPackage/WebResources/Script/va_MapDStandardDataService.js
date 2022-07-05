//=====================================================================================================
// START MapDStandardDataService
// Web Method List:  getVenefitClaimStatusTypecodeList, getClaimSuspenseReasonTypeCodeList,
// getContentionClassificationTypeCodeList, getContentionTypeList, getSpecialIssueList
//
// EX:
//      var mapDStandardDataServiceCtx = new actionContext();
//      mapDStandardDataServiceCtx.user = GetUserSettingsForWebservice();
//
//      var getSpecialIssueList  = new  mapDStandardDataService(mapDStandardDataServiceCtx);
//      getSpecialIssueList.serviceName = 'getSpecialIssueList';
//      getSpecialIssueList.wsMessage.serviceName = 'getSpecialIssueList';
//      getSpecialIssueList.responseFieldSchema = 'va_getspecialissuelistresponse';
//      getSpecialIssueList.responseTimestamp = 'va_webserviceresponse';
//      getSpecialIssueList.executeRequest();
//=====================================================================================================
var mapDStandardDataService = function (context) {
    this.context = context;
    this.webserviceRequestUrl = WebServiceURLRoot + 'StandardDataService/StandardDataService';
    this.wsMessage.methodName = 'MapDStandardDataService.mapDStandardDataService'
    this.prefix = 'ser';
    this.prefixUrl = 'http://services.mapd.benefits.vba.va.gov/';
}
mapDStandardDataService.prototype = new webservice;
mapDStandardDataService.prototype.constructor = mapDStandardDataService;
mapDStandardDataService.prototype.buildSoapBodyInnerXml = function () {
    this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
    var innerXml;

    innerXml = '<' + this.prefix + ':' + this.serviceName + '/>';

    return innerXml;
}
// END MapDStandardDataService
//=====================================================================================================