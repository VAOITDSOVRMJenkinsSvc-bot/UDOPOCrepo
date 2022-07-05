﻿/// <reference path="/Intellisense/XrmPage-vsdoc.js" />
//=====================================================================================================
// START VeteranGeneralInformation
//=====================================================================================================
var veteranGeneralInformation = function (context) {
    this.context = context;
    this.actionMessage.name = 'veteranGeneralInformation';
}
veteranGeneralInformation.prototype = new action;
veteranGeneralInformation.prototype.constructor = veteranGeneralInformation;
veteranGeneralInformation.prototype.performAction = function () {
    this.actionMessage.stackTrace = 'performAction();';

    var findGeneralInformationByFileNumberResponseXml;
    var findGeneralInformationByFileNumberResponseXmlObject;
    var ptcpntVetIdXpath; var ptcpntBeneIdXpath; var ptpcntRecipIdXpath; var awardTypeCdXpath;
    var ptcpntVetId; var ptcpntRecipId; var ptcpntBeneId; var awardTypeCd;

    //Create instance of general info web service
    var findGeneralInformationByFileNumber = new claimantWebServiceByFileNumber(this.context);
    findGeneralInformationByFileNumber.serviceName = 'findGeneralInformationByFileNumber';
    findGeneralInformationByFileNumber.wsMessage.serviceName = 'findGeneralInformationByFileNumber';
    findGeneralInformationByFileNumber.wsMessage.friendlyServiceName = 'General Information By File Number';
    findGeneralInformationByFileNumber.responseFieldSchema = 'va_generalinformationresponse';
    findGeneralInformationByFileNumber.responseTimestamp = 'va_webserviceresponse';

    if (!findGeneralInformationByFileNumber.executeRequest()) {
        return null;
    }

    findGeneralInformationByFileNumberResponseXml = Xrm.Page.getAttribute('va_generalinformationresponse').getValue();

    if (findGeneralInformationByFileNumberResponseXml == null || findGeneralInformationByFileNumberResponseXml == '') {
        this.actionMessage.errorFlag = true;
        this.actionMessage.description = 'Find Veteran General Information By File Number did not return data.';
        this.actionMessage.logMessage();
        return false;
    }

    findGeneralInformationByFileNumberResponseXmlObject =
        _XML_UTIL.parseXmlObject(findGeneralInformationByFileNumberResponseXml);

    if (findGeneralInformationByFileNumberResponseXmlObject) {
        var findGeneralInformationByFileNumberCount =
            findGeneralInformationByFileNumberResponseXmlObject.selectSingleNode('//numberOfAwardBenes') != null ?
            findGeneralInformationByFileNumberResponseXmlObject.selectSingleNode('//numberOfAwardBenes').text : 0;

        _AWARDBENEFIT_RECORD_COUNT = parseInt(findGeneralInformationByFileNumberCount);

        var tempXml = findGeneralInformationByFileNumberResponseXmlObject;

        if (_AWARDBENEFIT_RECORD_COUNT == 1) {
            ptcpntVetIdXpath = '//ptcpntVetID';
            ptcpntBeneIdXpath = '//ptcpntBeneID';
            ptpcntRecipIdXpath = '//ptcpntRecipID';
            awardTypeCdXpath = '//awardTypeCode';
        }
        else {
            ptcpntVetIdXpath = '//awardBenes/awardBenePK/ptcpntVetId';
            ptcpntBeneIdXpath = '//awardBenes/awardBenePK/ptcpntBeneId';
            ptpcntRecipIdXpath = '//awardBenes/awardBenePK/ptcpntRecipId';
            awardTypeCdXpath = '//awardBenes/awardBenePK/awardTypeCd';
        }

        ptcpntVetId =
            findGeneralInformationByFileNumberResponseXmlObject.selectSingleNode(ptcpntVetIdXpath) != null ?
                findGeneralInformationByFileNumberResponseXmlObject.selectSingleNode(ptcpntVetIdXpath).text : null;

        ptcpntBeneId =
            findGeneralInformationByFileNumberResponseXmlObject.selectSingleNode(ptcpntBeneIdXpath) != null ?
                findGeneralInformationByFileNumberResponseXmlObject.selectSingleNode(ptcpntBeneIdXpath).text : null;

        ptcpntRecipId =
            findGeneralInformationByFileNumberResponseXmlObject.selectSingleNode(ptpcntRecipIdXpath) != null ?
                findGeneralInformationByFileNumberResponseXmlObject.selectSingleNode(ptpcntRecipIdXpath).text : null;

        awardTypeCd =
            findGeneralInformationByFileNumberResponseXmlObject.selectSingleNode(awardTypeCdXpath) != null ?
                findGeneralInformationByFileNumberResponseXmlObject.selectSingleNode(awardTypeCdXpath).text : null;

        var awardKey = awardTypeCd + ptcpntBeneId + ptcpntRecipId + ptcpntVetId;

        this.context.ptcpntVetId = ptcpntVetId;
        this.context.ptcpntBeneId = ptcpntBeneId;
        this.context.ptcpntRecipId = ptcpntRecipId;
        this.context.awardTypeCd = awardTypeCd;

        awardKeyNode = tempXml.createElement("awardKey");
        awardKeyTextNode = tempXml.createTextNode(awardKey);
        awardKeyNode.appendChild(awardKeyTextNode);

        if (IsSingleAward()) {
            var genInfoNodes = tempXml.selectNodes('//return');
            if (genInfoNodes) {
                for (j = 0; j < genInfoNodes.length; j++) {
                    genInfoNodes[j].appendChild(awardKeyNode.cloneNode(true));
                }
            }
        }
        else {
            var genInfoAwardBeneNodes = tempXml.selectNodes('//awardBenes');
            if (genInfoAwardBeneNodes) {
                for (j = 0; j < genInfoAwardBeneNodes.length; j++) {
                    ptcpntVetId =
                        genInfoAwardBeneNodes[j].selectSingleNode('ptcpntVetId') != null ?
                            genInfoAwardBeneNodes[j].selectSingleNode('ptcpntVetId').text : null;

                    ptcpntBeneId =
                        genInfoAwardBeneNodes[j].selectSingleNode('ptcpntBeneId') != null ?
                            genInfoAwardBeneNodes[j].selectSingleNode('ptcpntBeneId').text : null;

                    ptcpntRecipId =
                        genInfoAwardBeneNodes[j].selectSingleNode('ptcpntRecipId') != null ?
                            genInfoAwardBeneNodes[j].selectSingleNode('ptcpntRecipId').text : null;

                    awardTypeCd =
                        genInfoAwardBeneNodes[j].selectSingleNode('awardTypeCd') != null ?
                            genInfoAwardBeneNodes[j].selectSingleNode('awardTypeCd').text : null;

                    var key = awardTypeCd + ptcpntBeneId + ptcpntRecipId + ptcpntVetId;

                    var keyNode = tempXml.createElement("awardKey");
                    var keyTextNode = tempXml.createTextNode(key);
                    keyNode.appendChild(keyTextNode);

                    genInfoAwardBeneNodes[j].appendChild(keyNode.cloneNode(true));
                }

            }
        }

        if (tempXml != null && tempXml != undefined && tempXml.xml != '') {
            Xrm.Page.getAttribute('va_generalinformationresponse').setValue(tempXml.xml);
        }

        var findGeneralInformationByPtcpntIdsService = new findGeneralInformationByPtcpntIds(this.context);
        findGeneralInformationByPtcpntIdsService.serviceName = 'findGeneralInformationByPtcpntIds';
        findGeneralInformationByPtcpntIdsService.wsMessage.serviceName = 'findGeneralInformationByPtcpntIds';
        findGeneralInformationByPtcpntIdsService.wsMessage.friendlyServiceName = 'General Information By Ptcpnt Ids';
        findGeneralInformationByPtcpntIdsService.responseFieldSchema = 'va_generalinformationresponsebypid';
        findGeneralInformationByPtcpntIdsService.responseTimestamp = 'va_webserviceresponse';
        findGeneralInformationByPtcpntIdsService.executeRequest();

        if (findGeneralInformationByPtcpntIdsService.responseXml
            && findGeneralInformationByPtcpntIdsService.responseXml != '') {
            tempXml = _XML_UTIL.parseXmlObject(findGeneralInformationByPtcpntIdsService.responseXml);

            var genInfoPidNodes = tempXml.selectNodes('//return');

            if (genInfoPidNodes) {
                for (j = 0; j < genInfoPidNodes.length; j++) {
                    genInfoPidNodes[j].appendChild(awardKeyNode.cloneNode(true));
                }
            }

            if (tempXml != null && tempXml != undefined && tempXml.xml != '') {
                Xrm.Page.getAttribute('va_generalinformationresponsebypid').setValue(tempXml.xml);
            }
        }
        else {
            var warningMsg = new actionsMessage();
            warningMsg.documentElement = 'actionsMessage';
            warningMsg.stackTrace = '';
            warningMsg.errorFlag = true;
            warningMsg.description = 'Web service error: findGeneralInformationByPtcpntIds did not return data';
            warningMsg.pushMessage();
        }

        if (!_searchCorpMin) {
            if (_searchCorp) {
                var findOtherAwardInformationService = new findOtherAwardInformation(this.context);
                findOtherAwardInformationService.serviceName = 'findOtherAwardInformation';
                findOtherAwardInformationService.wsMessage.serviceName = 'findOtherAwardInformation';
                findOtherAwardInformationService.wsMessage.friendlyServiceName = 'Other Award Information';
                findOtherAwardInformationService.responseFieldSchema = 'va_findotherawardinformationresponse';
                findOtherAwardInformationService.responseTimestamp = 'va_webserviceresponse';
                findOtherAwardInformationService.executeRequest();

                if (findOtherAwardInformationService.responseXml
                    && findOtherAwardInformationService.responseXml != '') {
                    tempXml = _XML_UTIL.parseXmlObject(findOtherAwardInformationService.responseXml);

                    var awardInfoNodes = tempXml.selectNodes('//awardInfo');
                    var awardLinesNodes = tempXml.selectNodes('//awardLines');
                    var accountBalancesNodes = tempXml.selectNodes('//accountBalances');
                    var deductionsNodes = tempXml.selectNodes('//deductions');
                    var receivablesNodes = tempXml.selectNodes('//receivables');
                    var awardReasonsNodes = tempXml.selectNodes('//awardReasons');

                    if (awardInfoNodes) {
                        for (j = 0; j < awardInfoNodes.length; j++) {
                            awardInfoNodes[j].appendChild(awardKeyNode.cloneNode(true));
                        }
                    }

                    if (accountBalancesNodes) {
                        for (j = 0; j < accountBalancesNodes.length; j++) {
                            accountBalancesNodes[j].appendChild(awardKeyNode.cloneNode(true));
                        }
                    }

                    if (deductionsNodes) {
                        for (j = 0; j < deductionsNodes.length; j++) {
                            deductionsNodes[j].appendChild(awardKeyNode.cloneNode(true));
                        }
                    }

                    if (receivablesNodes) {
                        for (j = 0; j < receivablesNodes.length; j++) {
                            receivablesNodes[j].appendChild(awardKeyNode.cloneNode(true));
                        }
                    }

                    if (awardLinesNodes) {
                        for (j = 0; j < awardLinesNodes.length; j++) {
                            awardLinesNodes[j].appendChild(awardKeyNode.cloneNode(true));
                        }
                    }

                    if (awardReasonsNodes) {
                        for (j = 0; j < awardReasonsNodes.length; j++) {
                            awardReasonsNodes[j].appendChild(awardKeyNode.cloneNode(true));
                        }
                    }

                    if (tempXml != null && tempXml != undefined && tempXml.xml != '') {
                        Xrm.Page.getAttribute('va_findotherawardinformationresponse').setValue(tempXml.xml);
                    }
                }
                else {
                    var warningMsg = new actionsMessage();
                    warningMsg.documentElement = 'actionsMessage';
                    warningMsg.stackTrace = '';
                    warningMsg.errorFlag = true;
                    warningMsg.description = 'Web service error: findOtherAwardInformation did not return data';
                    warningMsg.pushMessage();
                }

                var findIncomeExpensesByPtcptId = new findIncomeExpense(this.context);
                findIncomeExpensesByPtcptId.serviceName = 'findIncomeExpense';
                findIncomeExpensesByPtcptId.wsMessage.serviceName = 'findIncomeExpense';
                findIncomeExpensesByPtcptId.wsMessage.friendlyServiceName = 'Income/Expense';
                findIncomeExpensesByPtcptId.responseFieldSchema = 'va_findincomeexpenseresponse';
                findIncomeExpensesByPtcptId.responseTimestamp = 'va_webserviceresponse';
                findIncomeExpensesByPtcptId.executeRequest();

                if (findIncomeExpensesByPtcptId.responseXml
                    && findIncomeExpensesByPtcptId.responseXml != '') {
                    tempXml = _XML_UTIL.parseXmlObject(findIncomeExpensesByPtcptId.responseXml);

                    var incomeSummaryRecordNodes = tempXml.selectNodes('incomeSummaryRecords');
                    var incomeNodes = tempXml.selectNodes('//incomeRecords');
                    var expenseNodes = tempXml.selectNodes('//expenseRecords');

                    if (incomeSummaryRecordNodes) {
                        for (j = 0; j < incomeSummaryRecordNodes.length; j++) {
                            incomeSummaryRecordNodes[j].appendChild(awardKeyNode.cloneNode(true));
                        }
                    }

                    if (incomeNodes) {
                        for (j = 0; j < incomeNodes.length; j++) {
                            incomeNodes[j].appendChild(awardKeyNode.cloneNode(true));
                        }
                    }

                    if (expenseNodes) {
                        for (j = 0; j < expenseNodes.length; j++) {
                            expenseNodes[j].appendChild(awardKeyNode.cloneNode(true));
                        }
                    }

                    if (tempXml != null && tempXml != undefined && tempXml.xml != '') {
                        Xrm.Page.getAttribute('va_findincomeexpenseresponse').setValue(tempXml.xml);
                    }
                }
                else {
                    var warningMsg = new actionsMessage();
                    warningMsg.documentElement = 'actionsMessage';
                    warningMsg.stackTrace = '';
                    warningMsg.errorFlag = true;
                    warningMsg.description = 'Web service error: findIncomeExpense did not return data';
                    warningMsg.pushMessage();
                }
            }

            if (_searchVadir) {
                var fndAppeals = new findAppeals(this.context);
                fndAppeals.serviceName = 'findAppeals';
                fndAppeals.wsMessage.serviceName = 'findAppeals';
                fndAppeals.wsMessage.friendlyServiceName = 'Appeals';
                fndAppeals.responseFieldSchema = 'va_findappealsresponse';
                fndAppeals.responseTimestamp = 'va_webserviceresponse';
                fndAppeals.executeRequest();

                var appealKeyNode;
                var appealKeyTextNode;

                if (fndAppeals.responseXml && fndAppeals.responseXml != '') {
                    tempXml = _XML_UTIL.parseXmlObject(fndAppeals.responseXml);

                    if (tempXml != null && tempXml != undefined && tempXml.xml != '') {
                        Xrm.Page.getAttribute('va_findappealsresponse').setValue(tempXml.xml);
                    }

                    if (tempXml.selectSingleNode('//AppealKey')) {
                        appealKey = tempXml.selectSingleNode('//AppealKey').text;
                        this.context.appealKey = appealKey;
                        appealKeyNode = tempXml.createElement("AppealKey");
                        appealKeyTextNode = tempXml.createTextNode(appealKey);
                        appealKeyNode.appendChild(appealKeyTextNode);
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

                if (appealKeyNode) {
                    var getAppl = new getAppeal(this.context);
                    getAppl.serviceName = 'getAppeal';
                    getAppl.wsMessage.serviceName = 'getAppeal';
                    getAppl.wsMessage.friendlyServiceName = 'Appeals Detail';
                    getAppl.responseFieldSchema = 'va_findindividualappealsresponse';
                    getAppl.responseTimestamp = 'va_webserviceresponse';
                    getAppl.executeRequest();

                    if (getAppl.responseXml && getAppl.responseXml != '') {
                        tempXml = _XML_UTIL.parseXmlObject(getAppl.responseXml);

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
                        warningMsg.description = 'Web service error: getAppeal did not return data';
                        warningMsg.pushMessage();
                    }
                }
            }
        }

        if (findGeneralInformationByPtcpntIdsService.responseXml == null
            || findGeneralInformationByPtcpntIdsService.responseXml == ''
            || findGeneralInformationByPtcpntIdsService.responseXml == undefined) {
            Xrm.Page.getAttribute('va_generalinformationresponsebypid').setValue(findGeneralInformationByFileNumberResponseXml);
        }


    }
}
// END VeteranGeneralInformation
//=====================================================================================================