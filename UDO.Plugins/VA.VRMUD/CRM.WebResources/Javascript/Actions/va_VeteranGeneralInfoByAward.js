﻿/// <reference path="/Intellisense/XrmPage-vsdoc.js" />
//=====================================================================================================
// START veteranGeneralInfoByAward
//=====================================================================================================
var veteranGeneralInfoByAward = function (context) {
    this.context = context;
    this.actionMessage = new actionsMessage();
    this.actionMessage.name = 'veteranGeneralInfoByAward';
    this.actionMessage.documentElement = 'actionsMessage';
    this.actionMessage.stackTrace = '';
    this.webservices = new Array();
    this.analyzers = new Array();
    this.performAction = veteranGeneralInfoByAward_performAction;
}
veteranGeneralInfoByAward.prototype = new search;
veteranGeneralInfoByAward.prototype.constructor = veteranGeneralInfoByAward;
var veteranGeneralInfoByAward_performAction = function () {
    this.actionMessage.stackTrace = 'performAction();';

    var awardIDs = this.context.awardIdList;
    var genInfoResponses = new Array();

    for (i in awardIDs) {
        var awards = awardIDs[i];
        this.context.parameters['awardTypeCd'] = awards.awardTypeCd;
        this.context.parameters['ptcpntVetId'] = awards.ptcpntVetId;
        this.context.parameters['ptcpntBeneId'] = awards.ptcpntBeneId;
        this.context.parameters['ptcpntRecipId'] = awards.ptcpntRecipId;

        var awardKey = this.context.parameters['awardTypeCd'] + this.context.parameters['ptcpntBeneId'] + this.context.parameters['ptcpntRecipId'] + this.context.parameters['ptcpntVetId'];

        this.webservices['findGeneralInformationByPtcpntIds'] = new findGeneralInformationByPtcpntIds(this.context);
        this.executeSearchOperations(this.webservices);

        /*
        var findGeneralInformationByPtcpntIdsService = new findGeneralInformationByPtcpntIds(this.context);
        findGeneralInformationByPtcpntIdsService.executeRequest();
        */

        if (this.webservices['findGeneralInformationByPtcpntIds'].responseXml
            && this.webservices['findGeneralInformationByPtcpntIds'].responseXml != '') {
            var tempXml = _XML_UTIL.parseXmlObject(this.webservices['findGeneralInformationByPtcpntIds'].responseXml);

            var awardNodes = tempXml.selectNodes('//return');

            awardKeyNode = tempXml.createElement("awardKey");
            awardKeyTextNode = tempXml.createTextNode(awardKey);
            awardKeyNode.appendChild(awardKeyTextNode);

            for (j = 0; j < awardNodes.length; j++) {
                awardNodes[j].appendChild(awardKeyNode.cloneNode(true));
            }

            genInfoResponses.push(tempXml);
        }
    }

    if (genInfoResponses) {
        var fullGenInfoResponse = _XML_UTIL.concatenateDocs(genInfoResponses, 'GeneralInfoResponses');

        if (fullGenInfoResponse && fullGenInfoResponse.xml && fullGenInfoResponse.xml != '') {
            Xrm.Page.getAttribute('va_generalinformationresponsebypid').setValue(fullGenInfoResponse.xml);
        }
    }
}
// END veteranGeneralInfoByAward
//=====================================================================================================
//=====================================================================================================
// START fiduciaryInfoByAward
//=====================================================================================================
var fiduciaryInfoByAward = function (context) {
    this.context = context;
    this.actionMessage = new actionsMessage();
    this.actionMessage.name = 'fiduciaryInfoByAward';
    this.actionMessage.documentElement = 'actionsMessage';
    this.actionMessage.stackTrace = '';
    this.webservices = new Array();
    this.analyzers = new Array();
    this.performAction = fiduciaryInfoByAward_performAction;
}
fiduciaryInfoByAward.prototype = new search;
fiduciaryInfoByAward.prototype.constructor = fiduciaryInfoByAward;
var fiduciaryInfoByAward_performAction = function () {
    this.actionMessage.stackTrace = 'performAction();';

    var payeeList = this.context.payeeList;
    var fiduciaryResponses = [];

    for (i in payeeList) {
        var payee = payeeList[i];
        var awardKey = payee.awardKey;
        this.context.parameters['payeeSSN'] = payee.payeeSSN;

        this.webservices['findFiduciary'] = new findFiduciary(this.context);
        this.executeSearchOperations(this.webservices);

        if (this.webservices['findFiduciary'].responseXml
            && this.webservices['findFiduciary'].responseXml != '') {
            var tempXml = _XML_UTIL.parseXmlObject(this.webservices['findFiduciary'].responseXml);

            var awardNodes = tempXml.selectNodes('//return');

            awardKeyNode = tempXml.createElement("awardKey");
            awardKeyTextNode = tempXml.createTextNode(awardKey);
            awardKeyNode.appendChild(awardKeyTextNode);

            for (j = 0; j < awardNodes.length; j++) {
                awardNodes[j].appendChild(awardKeyNode.cloneNode(true));
            }

            fiduciaryResponses.push(tempXml);
        }
    }

    if (fiduciaryResponses) {
        var fullFiduciaryResponse = _XML_UTIL.concatenateDocs(fiduciaryResponses, 'FiduciaryResponses');

        if (fullFiduciaryResponse && fullFiduciaryResponse.xml && fullFiduciaryResponse.xml != '') {
            Xrm.Page.getAttribute('va_awardfiduciaryresponse').setValue(fullFiduciaryResponse.xml);
        }
    }
}
// END fiduciaryInfoByAward
//=====================================================================================================