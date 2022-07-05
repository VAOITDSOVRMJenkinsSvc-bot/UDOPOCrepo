﻿/// <reference path="/Intellisense/XrmPage-vsdoc.js" />
//=====================================================================================================
// START VeteranContentions
//=====================================================================================================
var veteranContentions = function (context) {
    this.context = context;
    this.actionMessage = new actionsMessage();
    this.actionMessage.name = 'veteranContentions';
    this.actionMessage.documentElement = 'actionsMessage';
    this.actionMessage.stackTrace = '';
    this.webservices = new Array();
    this.analyzers = new Array();
    this.performAction = veteranContentions_performAction;
}
veteranContentions.prototype = new search;
veteranContentions.prototype.constructor = veteranContentions;
var veteranContentions_performAction = function () {
    this.actionMessage.stackTrace = 'performAction();';

    var claimIDs = this.context.claimIdList;
    var contentionResponses = [];
    var executed = false;

    for (i = 0; i < claimIDs.length; i++) {
        var claimId = claimIDs[i];
        if (claimId && claimId != undefined && claimId != '') {
            this.context.parameters['claimId'] = claimId;

            this.webservices['findContentions'] = new findContentions(this.context);
            this.executeSearchOperations(this.webservices);


            if (!this.webservices['findContentions'].wsMessage.errorFlag) {
                var tempXml = _XML_UTIL.parseXmlObject(this.webservices['findContentions'].responseXml);
                contentionResponses.push(tempXml);

                if (!executed) { executed = true; }
            }
        }
    }

    if (executed) {
        WebServiceExecutionStatusList.add({ name: 'findContentions', executionTime: this.webservices['findContentions'].wsMessage.executionTime, claimId: claimIDs }, true);
    }

    var fullContentionResponse = _XML_UTIL.concatenateDocs(contentionResponses, 'ClaimContentions');

    Xrm.Page.getAttribute('va_findcontentionsresponse').setValue(fullContentionResponse.xml);
}
// END VeteranContentions
//=====================================================================================================
// START VeteranEvidence
//=====================================================================================================
var veteranEvidence = function (context) {
    this.context = context;
    this.actionMessage = new actionsMessage();
    this.actionMessage.name = 'veteranEvidence';
    this.actionMessage.documentElement = 'actionsMessage';
    this.actionMessage.stackTrace = '';
    this.webservices = [];
    this.analyzers = [];
    this.performAction = veteranEvidence_performAction;
}
veteranEvidence.prototype = new search;
veteranEvidence.prototype.constructor = veteranEvidence;
var veteranEvidence_performAction = function () {
    this.actionMessage.stackTrace = 'performAction();';

    var ptcpntIds = this.context.claimNoteIdList;
    var evidenceResponses = [];
    var executed = false;

    for (i = 0; i < ptcpntIds.length; i++) {
        var ptcpntIdObj = ptcpntIds[i];
        if (ptcpntIdObj && ptcpntIdObj != undefined && ptcpntIdObj.ptcpntId
            && ptcpntIdObj.ptcpntId != undefined && ptcpntIdObj.ptcpntId != '') {
            this.context.parameters['ptcpntId'] = ptcpntIdObj.ptcpntId;

            this.webservices['findUnsolEvdnce'] = new findUnsolEvdnce(this.context);
            this.executeSearchOperations(this.webservices);


            if (!this.webservices['findUnsolEvdnce'].wsMessage.errorFlag) {
                var tempXml = _XML_UTIL.parseXmlObject(this.webservices['findUnsolEvdnce'].responseXml);
                evidenceResponses.push(tempXml);

                if (!executed) { executed = true; }
            }
        }
    }

    if (executed) {
        WebServiceExecutionStatusList.add({ name: 'findUnsolEvdnce', executionTime: this.webservices['findUnsolEvdnce'].wsMessage.executionTime, claimId: ptcpntIds }, true);
    }

    var fullContentionResponse = _XML_UTIL.concatenateDocs(contentionResponses, 'Evidence');

    Xrm.Page.getAttribute('va_findunsolvedevidenceresponse').setValue(fullNoteResponse.xml);
}
// END VeteranContentions
//=====================================================================================================