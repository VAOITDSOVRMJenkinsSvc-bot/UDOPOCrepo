//=====================================================================================================
// START MapDClaimManagementService
//=====================================================================================================
var mapDClaimManagementService = function (context) {
    this.context = context;
    this.webserviceRequestUrl = WebServiceURLRoot + 'ClaimManagementService/ClaimManagementService';
    this.prefix = 'ser';
    this.prefixUrl = 'http://services.mapd.benefits.vba.va.gov/';
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
    this.context = context;

    this.serviceName = 'findClaimStatus';
    this.wsMessage = new webServiceMessage();
    this.wsMessage.methodName = 'MapDClaimManagementService.findClaimStatus';
    this.wsMessage.serviceName = 'findClaimStatus';
    this.wsMessage.friendlyServiceName = 'Claim Status';
    this.responseFieldSchema = 'va_findclaimstatusresponse';
    this.responseTimestamp = 'va_webserviceresponse';

    this.requiredSearchParameters = new Array();
    this.requiredSearchParameters['claimId'] = null;
}
findClaimStatus.prototype = new mapDClaimManagementService;
findClaimStatus.prototype.constructor = mapDClaimManagementService;
findClaimStatus.prototype.buildSoapBodyInnerXml = function () {
    this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
    var innerXml;

    var claimId = this.context.parameters['claimId'];

    if (claimId && claimId != '') {
        innerXml = '<ser:findClaimStatus><claimId>' + claimId + '</claimId></ser:findClaimStatus>';
    }
    else {
        this.wsMessage.errorFlag = true;
        this.wsMessage.description = 'There must be a claim id present for the request';
        return null;
    }

    return innerXml;
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