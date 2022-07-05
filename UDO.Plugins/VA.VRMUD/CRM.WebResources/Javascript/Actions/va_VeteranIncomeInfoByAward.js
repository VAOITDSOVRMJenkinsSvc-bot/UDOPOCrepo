﻿/// <reference path="/Intellisense/XrmPage-vsdoc.js" />
//=====================================================================================================
// START veteranIncomeInfoByAward
//=====================================================================================================
var veteranIncomeInfoByAward = function (context) {
    this.context = context;
    this.actionMessage = new actionsMessage();
    this.actionMessage.name = 'veteranAwardItems';
    this.actionMessage.documentElement = 'actionsMessage';
    this.actionMessage.stackTrace = '';
    this.webservices = new Array();
    this.analyzers = new Array();
    this.performAction = veteranIncomeInfoByAward_performAction;
}
veteranIncomeInfoByAward.prototype = new search;
veteranIncomeInfoByAward.prototype.constructor = veteranIncomeInfoByAward;
var veteranIncomeInfoByAward_performAction = function () {
    this.actionMessage.stackTrace = 'performAction();';

    var awardIDs = this.context.awardIdList;
    var incomeExpenseResponses = new Array();

    for (i in awardIDs) {
        var awards = awardIDs[i];
        this.context.parameters['awardTypeCd'] = awards.awardTypeCd;
        this.context.parameters['ptcpntVetId'] = awards.ptcpntVetId;
        this.context.parameters['ptcpntBeneId'] = awards.ptcpntBeneId;
        this.context.parameters['ptcpntRecipId'] = awards.ptcpntRecipId;

        var awardKey = this.context.parameters['awardTypeCd'] + this.context.parameters['ptcpntBeneId'] + this.context.parameters['ptcpntRecipId'] + this.context.parameters['ptcpntVetId'];

        this.webservices['findIncomeExpense'] = new findIncomeExpense(this.context);
        this.executeSearchOperations(this.webservices);

        /*
        var findIncomeExpensesByPtcptId = new findIncomeExpense(this.context);
        findIncomeExpensesByPtcptId.executeRequest();
        */

        if (this.webservices['findIncomeExpense'].responseXml && this.webservices['findIncomeExpense'].responseXml != '') {
            tempXml = _XML_UTIL.parseXmlObject(this.webservices['findIncomeExpense'].responseXml);

            awardKeyNode = tempXml.createElement("awardKey");
            awardKeyTextNode = tempXml.createTextNode(awardKey);
            awardKeyNode.appendChild(awardKeyTextNode);

            var incomeSummaryRecordNodes = tempXml.selectNodes('//incomeSummaryRecords');

            for (j = 0; j < incomeSummaryRecordNodes.length; j++) {
                incomeSummaryKeyNode = tempXml.createElement("incomeSummaryKey");
                incomeSummaryTextNode = tempXml.createTextNode(awardKey + j);
                incomeSummaryKeyNode.appendChild(incomeSummaryTextNode);

                incomeSummaryRecordNodes[j].appendChild(awardKeyNode.cloneNode(true));
                incomeSummaryRecordNodes[j].appendChild(incomeSummaryKeyNode.cloneNode(true));

                incomeNodes = incomeSummaryRecordNodes[j].selectNodes('incomeRecords');
                expenseNodes = incomeSummaryRecordNodes[j].selectNodes('expenseRecords');

                if (incomeNodes) {
                    for (k = 0; k < incomeNodes.length; k++) {
                        incomeNodes[k].appendChild(incomeSummaryKeyNode.cloneNode(true));
                    }
                }

                if (expenseNodes) {
                    for (l = 0; l < expenseNodes.length; l++) {
                        expenseNodes[l].appendChild(incomeSummaryKeyNode.cloneNode(true));
                    }
                }
            }

            incomeExpenseResponses.push(tempXml);
        }
    }

    if (incomeExpenseResponses) {
        var fullIncomeExpenseResponse = _XML_UTIL.concatenateDocs(incomeExpenseResponses, 'IncomeExpenseResponses');

        if (fullIncomeExpenseResponse && fullIncomeExpenseResponse.xml && fullIncomeExpenseResponse.xml != '') {
            Xrm.Page.getAttribute('va_findincomeexpenseresponse').setValue(fullIncomeExpenseResponse.xml);
        }
    }
}
// END veteranIncomeInfoByAward
//=====================================================================================================