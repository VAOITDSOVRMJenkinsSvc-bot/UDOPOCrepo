var awardSearch = function (context) {
    this.context = context;
    this.actionMessage = new actionsMessage();
    this.actionMessage.name = 'awardSearch';
    this.actionMessage.documentElement = 'actionsMessage';
    this.actionMessage.stackTrace = '';
    this.webservices = new Array();
    this.analyzers = new Array();
    this.performAction = awardSearch_performAction;
    this.analyzeFindOtherAwardInformationResult = analyzeFindOtherAwardInformationResult;
    this.analyzeFindIncomeExpenseResult = analyzeFindIncomeExpenseResult;
}
awardSearch.prototype = new search;
awardSearch.prototype.constructor = awardSearch;
var awardSearch_performAction = function () {
    this.context.stackTrace = 'performAction();';
    this.hasErrors = false;
    
    if (GetAwardCount() > 0) {
        var findOtherAwardInformationWebService = new findOtherAwardInformation(this.context);
        var success = CheckForRequiredParams(findOtherAwardInformationWebService, this.context);
        if (success) {
            this.webservices['findOtherAwardInformation'] = findOtherAwardInformationWebService;
            this.analyzers['findOtherAwardInformation'] = this.analyzeFindOtherAwardInformationResult;
        }

        var findIncomeExpenseWebService = new findIncomeExpense(this.context);
        success = CheckForRequiredParams(findIncomeExpenseWebService, this.context);
        if (success) {
            this.webservices['findIncomeExpense'] = findIncomeExpenseWebService
            this.analyzers['findIncomeExpense'] = this.analyzeFindIncomeExpenseResult;
        }

        this.executeSearchOperations(this.webservices);
        UpdateSearchListObject({ name: 'awardsSearch', complete: !this.hasErrors });
    }

    return !this.hasErrors;
}

function CheckForRequiredParams(webService, context) {
    for (var requiredParam in webService.requiredSearchParameters) {
        //if required param is in context, check next if next required param exists
        if (context.parameters[requiredParam] && context.parameters[requiredParam] != '' && context.parameters[requiredParam] != undefined) {
            continue;
        }
        else { //a required param does not exist in context - don't add to list of web services
            return false;
        }
    }
    return true;
}
var analyzeFindOtherAwardInformationResult = function (parentObject) {
    var awardXml = Xrm.Page.getAttribute('va_findotherawardinformationresponse').getValue();

    if (awardXml && awardXml != '') {
        var awardXmlObject = _XML_UTIL.parseXmlObject(awardXml);

        if (awardXmlObject && awardXmlObject.xml && awardXmlObject.xml != '') {
            if (parentObject.context.parameters['ptcpntVetId'] && parentObject.context.parameters['ptcpntBeneId']
                && parentObject.context.parameters['ptcpntRecipId'] && parentObject.context.parameters['awardTypeCd']) {
                var awardKey = parentObject.context.parameters['awardTypeCd'] + parentObject.context.parameters['ptcpntBeneId']
                    + parentObject.context.parameters['ptcpntRecipId'] + parentObject.context.parameters['ptcpntVetId'];

                if (awardKey && awardKey != '') {
                    var tempXml = awardXmlObject;

                    awardKeyNode = tempXml.createElement("awardKey");
                    awardKeyTextNode = tempXml.createTextNode(awardKey);
                    awardKeyNode.appendChild(awardKeyTextNode);



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
                            awardLineKeyNode = tempXml.createElement("awardLineKey");
                            awardLineKeyTextNode = tempXml.createTextNode(awardKey + j);
                            awardLineKeyNode.appendChild(awardLineKeyTextNode);

                            awardLinesNodes[j].appendChild(awardKeyNode.cloneNode(true));
                            awardLinesNodes[j].appendChild(awardLineKeyNode.cloneNode(true));

                            awardReasonsNodes = awardLinesNodes[j].selectNodes('awardReasons');
                            if (awardReasonsNodes) {
                                for (k = 0; k < awardReasonsNodes.length; k++) {
                                    awardReasonsNodes[k].appendChild(awardLineKeyNode.cloneNode(true));
                                }
                            }
                        }
                    }

                    if (tempXml != null && tempXml != undefined && tempXml.xml != '') {
                        Xrm.Page.getAttribute('va_findotherawardinformationresponse').setValue(tempXml.xml);
                    }
                }
            }
        }
    }
    return;
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

                    if (tempXml != null && tempXml != undefined && tempXml.xml != '') {
                        Xrm.Page.getAttribute('va_findincomeexpenseresponse').setValue(tempXml.xml);
                    }
                }
            }
        }
    }
    return;
}
function GetAwardCount() {
    var awardCount = 0;
    var awardXml = Xrm.Page.getAttribute('va_generalinformationresponse').getValue();

    if (awardXml && awardXml != undefined && awardXml != '') {
        var awardXmlObject = _XML_UTIL.parseXmlObject(awardXml);
        if (awardXmlObject && awardXmlObject != undefined && awardXmlObject.xml 
            && awardXmlObject.xml != undefined && awardXmlObject.xml != '') {
            if (SingleNodeExists(awardXmlObject, '//numberOfAwardBenes')) {
                awardCount = awardXmlObject.selectSingleNode('//numberOfAwardBenes').text;
            }
        }
    }

    return awardCount;
}