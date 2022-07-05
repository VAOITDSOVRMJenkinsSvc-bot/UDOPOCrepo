//=====================================================================================================
// START EbenefitsBnftClaimStatusService
//=====================================================================================================
var ebenefitsBnftClaimStatusService = function (context) {
    this.context = context;
    this.webserviceRequestUrl = WebServiceURLRoot + 
        'EbenefitsBnftClaimStatusServiceBean/EBenefitsBnftClaimStatusWebService';
}
ebenefitsBnftClaimStatusService.prototype = new webservice;
ebenefitsBnftClaimStatusService.prototype.constructor = ebenefitsBnftClaimStatusService;
//=====================================================================================================
// START Individual EbenefitsBnftClaimStatusService Methods
//=====================================================================================================
// START findBenefitClaimsStatusBySSN
//=====================================================================================================
var findBenefitClaimsStatusBySSN = function (context) {
    this.context = context;
    this.wsMessage.methodName = 'EbenefitsBnftClaimStatusService.findBenefitClaimsStatusBySSN';
}
findBenefitClaimsStatusBySSN.prototype = new ebenefitsBnftClaimStatusService;
findBenefitClaimsStatusBySSN.prototype.constructor = findBenefitClaimsStatusBySSN;
findBenefitClaimsStatusBySSN.prototype.buildSoapBodyInnerXml = function () {
    this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

    var innerXml;
    var fileNumber = this.context.fileNumber;

    if (fileNumber == null) {
        this.wsMessage.errorFlag = true;
        this.wsMessage.description = 'There must be a file number present for the request';
        return null;
    }

    innerXml = '<q0:findBenefitClaimsStatusBySSN><fileNumber>' + fileNumber
        + '</fileNumber></q0:findBenefitClaimsStatusBySSN>';

    return innerXml;
}
// END findBenefitClaimsStatusBySSN
//=====================================================================================================
// START findBenefitClaimDetailsByBnftClaimId
//=====================================================================================================
var findBenefitClaimDetailsByBnftClaimId = function (context) {
    this.context = context;
    this.wsMessage.methodName = 'EbenefitsBnftClaimStatusService.findBenefitClaimDetailsByBnftClaimId';
}
findBenefitClaimDetailsByBnftClaimId.prototype = new ebenefitsBnftClaimStatusService;
findBenefitClaimDetailsByBnftClaimId.prototype.constructor = findBenefitClaimDetailsByBnftClaimId;
findBenefitClaimDetailsByBnftClaimId.prototype.buildSoapBodyInnerXml = function () {
    this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

    var innerXml;
    var benefitClaimId = this.context.benefitClaimId;

    if (benefitClaimId == null) {
        this.wsMessage.errorFlag = true;
        this.wsMessage.description = 'There must be a benefit claim Id present for the request';
        return null;
    }

    innerXml = '<q0:findBenefitClaimDetailsByBnftClaimId><benefitClaimId>' + fileNumber
        + '</benefitClaimId></q0:findBenefitClaimDetailsByBnftClaimId>';

    return innerXml;
}
// END findBenefitClaimDetailsByBnftClaimId
//=====================================================================================================
// END Individual EbenefitsBnftClaimStatusService Methods
//=====================================================================================================
// END EbenefitsBnftClaimStatusService
//=====================================================================================================