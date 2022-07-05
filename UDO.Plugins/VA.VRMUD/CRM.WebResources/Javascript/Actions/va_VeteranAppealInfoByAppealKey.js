﻿/// <reference path="/Intellisense/XrmPage-vsdoc.js" />
//=====================================================================================================
// START veteranAppealInfoByAppealKey
//=====================================================================================================
var veteranAppealInfoByAppealKey = function (context) {
    this.context = context;
    this.actionMessage = new actionsMessage();
    this.actionMessage.name = 'veteranAppealInfoByAppealKey';
    this.actionMessage.documentElement = 'actionsMessage';
    this.actionMessage.stackTrace = '';
    this.webservices = new Array();
    this.analyzers = new Array();
    this.performAction = veteranAppealInfoByAppealKey_performAction;
}
veteranAppealInfoByAppealKey.prototype = new search;
veteranAppealInfoByAppealKey.prototype.constructor = veteranAppealInfoByAppealKey;
var veteranAppealInfoByAppealKey_performAction = function () {
    this.actionMessage.stackTrace = 'performAction();';
    var appealKeys = this.context.appealKeyList;
    var appealResponses = new Array();

    for (i in appealKeys) {
        var appealKey = appealKeys[i];
        this.context.parameters['appealKey'] = appealKey;

        this.webservices['getAppeal'] = new getAppeal(this.context);
        this.executeSearchOperations(this.webservices);

//        var getApplService = new getAppeal(this.context);
//        getApplService.executeRequest();

        if (this.webservices['getAppeal'].responseXml && this.webservices['getAppeal'].responseXml != '') {
            tempXml = _XML_UTIL.parseXmlObject(this.webservices['getAppeal'].responseXml);
            appealKeyNode = tempXml.createElement("AppealKey");
            appealKeyTextNode = tempXml.createTextNode(appealKey);
            appealKeyNode.appendChild(appealKeyTextNode);

            var appellantRecordNodes = tempXml.selectNodes('//Appellant');
            var appellantAddressRecordNodes = tempXml.selectNodes('//AppellantAddress');
            var appealVetRecordNodes = tempXml.selectNodes('//AppealVeteran');
            var issueRecordNodes = tempXml.selectNodes('//Issue');
            var remandReasonRecordNodes = tempXml.selectNodes('//RemandReason');
            var diaryRecordNodes = tempXml.selectNodes('//Diary');
            var appealDecisionRecordNodes = tempXml.selectNodes('//AppealDecision');
            var specialContentionsRecordNodes = tempXml.selectNodes('//SpecialContentions');
            var appealDateRecordNodes = tempXml.selectNodes('//AppealDate');
            var hearingRequestRecordNodes = tempXml.selectNodes('//HearingRequest');

            if (appellantRecordNodes) {
                for (j = 0; j < appellantRecordNodes.length; j++) {
                    appellantRecordNodes[j].appendChild(appealKeyNode.cloneNode(true));
                }
            }

            if (appellantAddressRecordNodes) {
                for (j = 0; j < appellantAddressRecordNodes.length; j++) {
                    appellantAddressRecordNodes[j].appendChild(appealKeyNode.cloneNode(true));
                }
            }

            if (appealVetRecordNodes) {
                for (j = 0; j < appealVetRecordNodes.length; j++) {
                    appealVetRecordNodes[j].appendChild(appealKeyNode.cloneNode(true));
                }
            }

            if (issueRecordNodes) {
                for (j = 0; j < issueRecordNodes.length; j++) {
                    issueRecordNodes[j].appendChild(appealKeyNode.cloneNode(true));
                }
            }

            if (remandReasonRecordNodes) {
                for (j = 0; j < remandReasonRecordNodes.length; j++) {
                    remandReasonRecordNodes[j].appendChild(appealKeyNode.cloneNode(true));
                }
            }

            if (diaryRecordNodes) {
                for (j = 0; j < diaryRecordNodes.length; j++) {
                    diaryRecordNodes[j].appendChild(appealKeyNode.cloneNode(true));
                }
            }

            if (appealDecisionRecordNodes) {
                for (j = 0; j < appealDecisionRecordNodes.length; j++) {
                    appealDecisionRecordNodes[j].appendChild(appealKeyNode.cloneNode(true));
                }
            }

            if (specialContentionsRecordNodes) {
                for (j = 0; j < specialContentionsRecordNodes.length; j++) {
                    specialContentionsRecordNodes[j].appendChild(appealKeyNode.cloneNode(true));
                }
            }

            if (appealDateRecordNodes) {
                for (j = 0; j < appealDateRecordNodes.length; j++) {
                    appealDateRecordNodes[j].appendChild(appealKeyNode.cloneNode(true));
                }
            }

            if (hearingRequestRecordNodes) {
                for (j = 0; j < hearingRequestRecordNodes.length; j++) {
                    hearingRequestRecordNodes[j].appendChild(appealKeyNode.cloneNode(true));
                }
            }

            appealResponses.push(tempXml);
        }
    }

    if (appealResponses) {
        var fullAppInfoResponse = _XML_UTIL.concatenateDocs(appealResponses, 'AppealDetails');

        if (fullAppInfoResponse && fullAppInfoResponse.xml && fullAppInfoResponse.xml != '') {
            Xrm.Page.getAttribute('va_findindividualappealsresponse').setValue(fullAppInfoResponse.xml);
        }
    }
}
// END veteranAppealInfoByAppealKey
//=====================================================================================================