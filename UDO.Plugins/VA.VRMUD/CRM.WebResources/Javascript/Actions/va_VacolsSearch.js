﻿var vacolsSearch = function (context) {
    this.context = context;
    this.actionMessage = new actionsMessage();
    this.actionMessage.name = 'vacolsSearch';
    this.actionMessage.documentElement = 'actionsMessage';
    this.actionMessage.stackTrace = '';
    this.webservices = new Array();
    this.analyzers = new Array();
    this.performAction = vacolsSearch_performAction;
    this.analyzeFindAppealsResult = analyzeFindAppealsResult;
    this.analyzeGetAppealResult = analyzeGetAppealResult;
}
vacolsSearch.prototype = new search;
vacolsSearch.prototype.constructor = vacolsSearch;
var vacolsSearch_performAction = function () {
    this.context.stackTrace = 'performAction();';
    this.hasErrors = false;

    if (!this.context.parameters['ssn']) this.context.parameters['ssn'] = Xrm.Page.getAttribute('va_ssn').getValue();

    this.webservices['findAppeals'] = new findAppeals(this.context);
    this.analyzers['findAppeals'] = this.analyzeFindAppealsResult;
    this.webservices['getAppeal'] = new getAppeal(this.context);
    this.analyzers['getAppeal'] = this.analyzeGetAppealResult;

    this.excuteSearchOpeartions(this.webservices);
    return !this.hasErrors;
}
var analyzeFindAppealsResult = function (parentObject) {
    var appealXml = Xrm.Page.getAttribute('va_findappealsresponse').getValue();

    if (appealXml && appealXml != '') {
        var appealXmlObject = _XML_UTIL.parseXmlObject(appealXml);

        if (appealXmlObject && appealXmlObject.xml && appealXmlObject.xml != '') {
            var tempXml = appealXmlObject;

            if (tempXml.selectSingleNode('//AppealKey')) {
                appealKey = tempXml.selectSingleNode('//AppealKey').text;
                parentObject.context.parameters['appealKey'] = appealKey;
                parentObject.webservices['getAppeal'].context = parentObject.context;
            }
        }
    }
    else {
        var warningMsg = new actionsMessage();
        warningMsg.documentElement = 'actionsMessage';
        warningMsg.stackTrace = '';
        warningMsg.errorFlag = true;
        warningMsg.description = 'Web service error: findAppeals did not return data';
        warningMsg.pushMessage();
    }

    return;
}
var analyzeGetAppealResult = function (parentObject) {
    var appealXml = Xrm.Page.getAttribute('va_findindividualappealsresponse').getValue();

    if (appealXml && appealXml != '') {
        var appealXmlObject = _XML_UTIL.parseXmlObject(appealXml);

        if (appealXmlObject && appealXmlObject.xml && appealXmlObject.xml != '') {
            var tempXml = appealXmlObject;

            if (parentObject.context.parameters['appealKey']) {
                appealKey = parentObject.context.parameters['appealKey'];

                var appealKeyNode;
                var appealKeyTextNode;

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

                if (tempXml != null && tempXml != undefined && tempXml.xml != '') {
                    Xrm.Page.getAttribute('va_findindividualappealsresponse').setValue(tempXml.xml);
                }
            }
            else {
                var warningMsg = new actionsMessage();
                warningMsg.documentElement = 'actionsMessage';
                warningMsg.stackTrace = '';
                warningMsg.errorFlag = true;
                warningMsg.description = 'Web service error: appealKey is necessary to process the data';
                warningMsg.pushMessage();
            }
        }
    }
    else {
        var warningMsg = new actionsMessage();
        warningMsg.documentElement = 'actionsMessage';
        warningMsg.stackTrace = '';
        warningMsg.errorFlag = true;
        warningMsg.description = 'Web service error: getAppeal did not return data';
        warningMsg.pushMessage();
    }

    return;
}