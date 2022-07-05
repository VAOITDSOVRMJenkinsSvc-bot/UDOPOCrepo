﻿/// <reference path="/Intellisense/XrmPage-vsdoc.js" />
//=====================================================================================================
// START VeteranBenefitClaimHistoryInformation
//=====================================================================================================
var veteranBenefitClaimHistoryInformation = function (context) {
    this.context = context;
    this.actionMessage = new actionsMessage();
    this.actionMessage.name = 'veteranBenefitClaimHistoryInformation';
    this.actionMessage.documentElement = 'actionsMessage';
    this.actionMessage.stackTrace = '';
    this.webservices = new Array();
    this.analyzers = new Array();
    this.performAction = veteranBenefitClaimHistoryInformation_performAction;
}
veteranBenefitClaimHistoryInformation.prototype = new search;
veteranBenefitClaimHistoryInformation.prototype.constructor = veteranBenefitClaimHistoryInformation;
var veteranBenefitClaimHistoryInformation_performAction = function () {
    this.actionMessage.stackTrace = 'performAction();';
    
    var claimIDs = this.context.claimIdList;
    var benefitClaimDetailResponses = [];
    var executed = false;

    for (i = 0; i < claimIDs.length; i++) {
        this.context.parameters['claimId'] = claimIDs[i];

        this.webservices['findBenefitClaimDetail'] = new findBenefitClaimDetail(this.context);
        this.executeSearchOperations(this.webservices);
        /*
        var bnftClmDtl = new findBenefitClaimDetail(this.context);
        bnftClmDtl.serviceName = 'findBenefitClaimDetail';
        bnftClmDtl.wsMessage.serviceName = 'findBenefitClaimDetail';
        bnftClmDtl.wsMessage.friendlyServiceName = 'Benefit Claim Detail';
        bnftClmDtl.responseFieldSchema = 'va_findbenefitdetailresponse';
        bnftClmDtl.responseTimestamp = 'va_webserviceresponse';
        bnftClmDtl.executeRequest();
        */

        var tempXml = _XML_UTIL.parseXmlObject(this.webservices['findBenefitClaimDetail'].responseXml);

        var suspenseNodes = tempXml.selectNodes('//suspenceRecords');
        var benefitClaimNodes = tempXml.selectNodes('//benefitClaimID');
        var benefitClaimNode = benefitClaimNodes[0];

        for (j = 0; j < suspenseNodes.length; j++) {
            suspenseNodes[j].appendChild(benefitClaimNode.cloneNode(true));
        }

        if (!this.webservices['findBenefitClaimDetail'].wsMessage.errorFlag && !executed) {
            executed = true;
        }

        benefitClaimDetailResponses.push(tempXml);
    }

    if (executed) {
        WebServiceExecutionStatusList.add({ name: 'findBenefitClaimDetail', executionTime: this.webservices['findBenefitClaimDetail'].wsMessage.executionTime, claimId: claimIDs }, true);
    }

    var fullBenefitClaimDetailResponse = _XML_UTIL.concatenateDocs(benefitClaimDetailResponses, 'BenefitClaims');

    Xrm.Page.getAttribute('va_findbenefitdetailresponse').setValue(fullBenefitClaimDetailResponse.xml);

}
// END VeteranBenefitClaimHistoryInformation
//=====================================================================================================
// START veteranNotes
//=====================================================================================================
var veteranNotes = function (context) {
    this.context = context;
    this.actionMessage = new actionsMessage();
    this.actionMessage.name = 'veteranBenefitClaimHistoryInformation';
    this.actionMessage.documentElement = 'actionsMessage';
    this.actionMessage.stackTrace = '';
    this.webservices = new Array();
    this.analyzers = new Array();
    this.performAction = veteranNotes_performAction;
}
veteranNotes.prototype = new search;
veteranNotes.prototype.constructor = veteranNotes;
var veteranNotes_performAction = function () {
    this.actionMessage.stackTrace = 'performAction();';

    var noteIds = this.context.claimNoteIdList;
    var noteResponses = [];
    var addedToExecutionList = false;

    for (i = 0; i < noteIds.length; i++) {
        var noteId = noteIds[i];
        if (noteId && noteId != undefined && noteId.ptcpntId && noteId.ptcpntId != undefined && noteId.ptcpntId != '') {
            this.context.parameters['ptcpntId'] = noteId.ptcpntId;

            this.webservices['findDevelopmentNotes'] = new findDevelopmentNotes(this.context);
            this.executeSearchOperations(this.webservices);

            var tempXml = _XML_UTIL.parseXmlObject(this.webservices['findDevelopmentNotes'].responseXml);

            noteResponses.push(tempXml);

            if (!this.webservices['findDevelopmentNotes'].wsMessage.errorFlag && !addedToExecutionList) {
                addedToExecutionList = true;
                WebServiceExecutionStatusList.add({ name: 'findDevelopmentNotes', executionTime: this.webservices['findDevelopmentNotes'].wsMessage.executionTime }, true);
            }

        }
    }

    var fullNoteResponse = _XML_UTIL.concatenateDocs(noteResponses, 'DevelopmentNotes');

    Xrm.Page.getAttribute('va_finddevelopmentnotesresponse').setValue(fullNoteResponse.xml);

}
// END VeteranBenefitClaimHistoryInformation
//=====================================================================================================