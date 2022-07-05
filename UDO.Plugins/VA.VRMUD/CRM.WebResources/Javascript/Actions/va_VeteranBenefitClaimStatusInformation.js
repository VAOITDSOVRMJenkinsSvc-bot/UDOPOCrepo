﻿/// <reference path="/Intellisense/XrmPage-vsdoc.js" />
//=====================================================================================================
// START VeteranBenefitClaimStatusInformation
//=====================================================================================================
var veteranBenefitClaimStatusInformation = function (context) {
    this.context = context;
    this.actionMessage = new actionsMessage();
    this.actionMessage.name = 'veteranBenefitClaimHistoryInformation';
    this.actionMessage.documentElement = 'actionsMessage';
    this.actionMessage.stackTrace = '';
    this.webservices = new Array();
    this.analyzers = new Array();
    this.performAction = veteranBenefitClaimStatusInformation_performAction;
}
veteranBenefitClaimStatusInformation.prototype = new search;
veteranBenefitClaimStatusInformation.prototype.constructor = veteranBenefitClaimStatusInformation;
var veteranBenefitClaimStatusInformation_performAction = function () {
    this.actionMessage.stackTrace = 'performAction();';

    var claimIDs = this.context.claimIdList;
    var benefitClaimDetailResponses = [];
    var executed = false;

    for (i = 0; i < claimIDs.length; i++) {
        this.context.parameters['claimId'] = claimIDs[i];

        this.webservices['findClaimStatus'] = new findClaimStatus(this.context);
        this.executeSearchOperations(this.webservices);

        /*
        var clmSts = new findClaimStatus(this.context);
        clmSts.serviceName = 'findClaimStatus';
        clmSts.wsMessage.serviceName = 'findClaimStatus';
        clmSts.responseFieldSchema = 'va_findclaimstatusresponse';
        clmSts.responseTimestamp = 'va_webserviceresponse';
        clmSts.executeRequest();
        */

        var tempXml = _XML_UTIL.parseXmlObject(this.webservices['findClaimStatus'].responseXml);
        benefitClaimDetailResponses.push(tempXml);

        if (!this.webservices['findClaimStatus'].wsMessage.errorFlag && !executed) {
            executed = true;
        }
    }

    if (executed) {
        WebServiceExecutionStatusList.add({ name: 'findClaimStatus', executionTime: this.webservices['findClaimStatus'].wsMessage.executionTime, claimId: claimIDs }, true);
    }
    var fullBenefitClaimDetailResponse = _XML_UTIL.concatenateDocs(benefitClaimDetailResponses, 'BenefitClaims');

    Xrm.Page.getAttribute('va_findclaimstatusresponse').setValue(fullBenefitClaimDetailResponse.xml);

}
// END VeteranBenefitClaimStatusInformation
//=====================================================================================================