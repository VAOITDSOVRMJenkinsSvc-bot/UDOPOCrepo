﻿/// <reference path="/Intellisense/XrmPage-vsdoc.js" />
//=====================================================================================================
// START VeteranGeneralInformationPK
//=====================================================================================================
var veteranGeneralInformationPK = function (context) {
    this.context = context;
    this.actionMessage.name = 'veteranGeneralInformationPK';
}
veteranGeneralInformationPK.prototype = new action;
veteranGeneralInformationPK.prototype.constructor = veteranGeneralInformationPK;
veteranGeneralInformationPK.prototype.performAction = function () {
    this.actionMessage.stackTrace = 'performAction();'
    var findGeneralInformationByPtcpntIdsResponseXml;
    var findGeneralInformationByPtcpntIdsResponseXmlObject;
    var ptcpntVetId; var ptcpntBeneId; var ptpcntRecipId; var awardTypeCd;

    var findGeneralInformationByPtcpntIds = new claimantWebServiceByParticipantIdsPK(this.context);
    findGeneralInformationByPtcpntIds.serviceName = 'findGeneralInformationByPtcpntIds';
    findGeneralInformationByPtcpntIds.wsMessage.serviceName = 'findGeneralInformationByPtcpntIds';
    findGeneralInformationByPtcpntIds.wsMessage.friendlyServiceName = 'General Information By Ptcpnt Ids';
    findGeneralInformationByPtcpntIds.responseFieldSchema = 'va_generalinformationresponsebypid';
    findGeneralInformationByPtcpntIds.responseTimestamp = 'va_webserviceresponse';
    findGeneralInformationByPtcpntIds.executeRequest();

    findGeneralInformationByPtcpntIdsResponseXml = Xrm.Page.getAttribute('va_generalinformationresponsebypid').getValue();

    if (findGeneralInformationByPtcpntIdsResponseXml == null || findGeneralInformationByPtcpntIdsResponseXml == '') {
        this.actionMessage.errorFlag = true;
        this.actionMessage.description = 'Find Veteran General Information By Participant Ids did not return data.';
        this.actionMessage.logMessage();
        return false;
    }

    if (this.context.ptcpntVetId && this.context.ptcpntBeneId && this.context.ptpcntRecipId && this.context.awardTypeCd) {
        ptcpntVetId = this.context.ptcpntVetId;
        ptcpntBeneId = this.context.ptcpntBeneId;
        ptpcntRecipId = this.context.ptpcntRecipId;
        awardTypeCd = this.context.awardTypeCd;

        var awardBenefitPK = awardTypeCd + '_' + ptcpntVetId + '_' + ptcpntBeneId + '_' + ptpcntRecipId;

        _AWARDBENEFIT_RESPONSE_OBJECT[awardBenefitPK] = findGeneralInformationByPtcpntIdsResponseXml;
    }
    else {
        this.actionMessage.errorFlag = true;
        this.actionMessage.xmlResponse = findGeneralInformationByPtcpntIdsResponseXml;
        this.actionMessage.description = 'Award Benefit Primary Key contains a null value.';
        this.actionMessage.pushMessage();
        return false;
    }
}
// END VeteranGeneralInformationPK
//=====================================================================================================