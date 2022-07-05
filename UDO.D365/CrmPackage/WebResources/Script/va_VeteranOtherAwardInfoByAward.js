/// <reference path="/Intellisense/XrmPage-vsdoc.js" />
//=====================================================================================================
// START veteranOtherAwardInfoByAward
//=====================================================================================================
var veteranOtherAwardInfoByAward = function (context) {
    this.context = context;
    this.actionMessage = new actionsMessage();
    this.actionMessage.name = 'veteranAwardItems';
    this.actionMessage.documentElement = 'actionsMessage';
    this.actionMessage.stackTrace = '';
    this.webservices = new Array();
    this.analyzers = new Array();
    this.performAction = veteranOtherAwardInfoByAward_performAction;
}
veteranOtherAwardInfoByAward.prototype = new search;
veteranOtherAwardInfoByAward.prototype.constructor = veteranOtherAwardInfoByAward;
var veteranOtherAwardInfoByAward_performAction = function () {
    this.actionMessage.stackTrace = 'performAction();';

    var awardIDs = this.context.awardIdList;
    var awardResponses = new Array();

    for (i in awardIDs) {
        var awards = awardIDs[i];
        this.context.parameters['awardTypeCd'] = awards.awardTypeCd;
        this.context.parameters['ptcpntVetId'] = awards.ptcpntVetId;
        this.context.parameters['ptcpntBeneId'] = awards.ptcpntBeneId;
        this.context.parameters['ptcpntRecipId'] = awards.ptcpntRecipId;

        var awardKey = this.context.parameters['awardTypeCd'] + this.context.parameters['ptcpntBeneId'] + this.context.parameters['ptcpntRecipId'] + this.context.parameters['ptcpntVetId'];

        this.webservices['findOtherAwardInformation'] = new findOtherAwardInformation(this.context);
        this.executeSearchOperations(this.webservices);

        /*
        var findOtherAwardInformationService = new findOtherAwardInformation(this.context);
        findOtherAwardInformationService.executeRequest();
        */

        if (this.webservices['findOtherAwardInformation'].responseXml && this.webservices['findOtherAwardInformation'].responseXml != '') {
            var tempXml = _XML_UTIL.parseXmlObject(this.webservices['findOtherAwardInformation'].responseXml);

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

            awardResponses.push(tempXml);
        }
    }

    if (awardResponses) {
        var fullAwardResponse = _XML_UTIL.concatenateDocs(awardResponses, 'AwardResponses');

        if (fullAwardResponse && fullAwardResponse.xml && fullAwardResponse.xml != '') {
            Xrm.Page.getAttribute('va_findotherawardinformationresponse').setValue(fullAwardResponse.xml);
        }
    }
}
// END veteranOtherAwardInfoByAward
//=====================================================================================================