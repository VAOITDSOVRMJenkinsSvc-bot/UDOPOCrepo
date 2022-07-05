var corpSearch = function (context) {
    this.context = context;
    this.actionMessage = new actionsMessage();
    this.actionMessage.name = 'corpSearch';
    this.actionMessage.documentElement = 'actionsMessage';
    this.actionMessage.stackTrace = '';
    this.webservices = new Array();
    this.analyzers = new Array();
    this.performAction = corpSearch_performAction;
    this.analyzeFindIncomeExpenseResult = analyzeFindIncomeExpenseResult;
}
corpSearch.prototype = new search;
corpSearch.prototype.constructor = corpSearch;
var corpSearch_performAction = function () {
    this.context.stackTrace = 'performAction();';
    this.hasErrors = false;

    this.webservices['findIncomeExpense'] = new findIncomeExpense(this.context);
    this.analyzers['findIncomeExpense'] = this.analyzeFindIncomeExpenseResult;

    // 'OLD' pay history service is only used if flag is set (it's on by default)
    if (_searchOldPayments) {
        this.webservices['findPayHistoryBySSN'] = new findPayHistoryBySSN(this.context);
    }

    this.webservices['findMilitaryRecordByPtcpntId'] = new findMilitaryRecordByPtcpntId(this.context);
    this.webservices['findRatingData'] = new findRatingData(this.context);
    this.webservices['findDenialsByPtcpntId'] = new findDenialsByPtcpntId(this.context);
    this.webservices['retrievePaymentSummary'] = new retrievePaymentSummary(this.context);
    //this.webservices['findMonthOfDeath'] = new findMonthOfDeath(this.context);

    this.executeSearchOperations(this.webservices);
    UpdateSearchListObject({ name: 'corpSearch', complete: !this.hasErrors });
    return !this.hasErrors;
}
var analyzeFindIncomeExpenseResult = function (parentObject) {
    var incomeXml = Xrm.Page.getAttribute('va_findincomeexpenseresponse').getValue();

    if (incomeXml && incomeXml != '') {
        var incomeXmlObject = _XML_UTIL.parseXmlObject(incomeXml);

        if (incomeXmlObject && incomeXmlObject.xml && incomeXmlObject.xml != '') {
            if (parentObject.context.parameters['ptcpntVetId'] && parentObject.context.parameters['ptcpntBeneId']
                && parentObject.context.parameters['ptcpntRecipId'] && parentObject.context.parameters['awardTypeCd']) {
                var awardKey = parentObject.context.parameters['awardTypeCd'] + parentObject.context.parameters['ptcpntBeneId']
                    + parentObject.context.parameters['ptcpntRecipId'] + parentObject.context.parameters['ptcpntVetId'];

                if (awardKey && awardKey != '') {
                    var tempXml = incomeXmlObject;

                    awardKeyNode = tempXml.createElement("awardKey");
                    awardKeyTextNode = tempXml.createTextNode(awardKey);
                    awardKeyNode.appendChild(awardKeyTextNode);

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
            }
        }
    }

    return;
}