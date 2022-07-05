/// <reference path="/Intellisense/XrmPage-vsdoc.js" />
//=====================================================================================================
// START VeteranAwardInformation
//=====================================================================================================
var veteranAwardInformation = function (context) {
    this.context = context;
    this.actionMessage.name = 'veteranAwardItems';
}

debugger;

veteranAwardInformation.prototype = new action;
veteranAwardInformation.prototype.constructor = veteranAwardInformation;
veteranAwardInformation.prototype.performAction = function () {
    this.actionMessage.stackTrace = 'performAction();';
    this.context.updateProgress = true;
    
    var awardIDs = this.context.awardIdList;
    var awardResponses = new Array();
    var incomeExpenseResponses = new Array();
    var genInfoResponses = new Array();

    var xmlDoc;
    var xmlEl;
    var xmlText;

    for (i = 0; i < awardIDs.length; i++) {
        var awards = awardIDs[i];
        this.context.awardTypeCd = awards.awardTypeCd;
        this.context.ptcpntVetId = awards.ptcpntVetId;
        this.context.ptcpntBeneId = awards.ptcpntBeneId;
        this.context.ptcpntRecipId = awards.ptcpntRecipId;

        var findGeneralInformationByPtcpntIdsService = new findGeneralInformationByPtcpntIds(this.context);
        findGeneralInformationByPtcpntIdsService.serviceName = 'findGeneralInformationByPtcpntIds';
        findGeneralInformationByPtcpntIdsService.wsMessage.serviceName = 'findGeneralInformationByPtcpntIds';
        findGeneralInformationByPtcpntIdsService.wsMessage.friendlyServiceName = 'General Information By Ptcpnt Ids';
        findGeneralInformationByPtcpntIdsService.responseFieldSchema = 'va_generalinformationresponsebypid';
        findGeneralInformationByPtcpntIdsService.responseTimestamp = 'va_webserviceresponse';
        findGeneralInformationByPtcpntIdsService.executeRequest();
        findGeneralInformationByPtcpntIdsService.context.updateProgress = false;

        genInfoTempXml = _XML_UTIL.parseXmlObject(findGeneralInformationByPtcpntIdsService.responseXml);
        genInfoResponses.push(genInfoTempXml);

        var findOtherAwardInformationService = new findOtherAwardInformation(this.context);
        findOtherAwardInformationService.serviceName = 'findOtherAwardInformation';
        findOtherAwardInformationService.wsMessage.serviceName = 'findOtherAwardInformation';
        findOtherAwardInformationService.wsMessage.friendlyServiceName = 'Other Award Information';
        findOtherAwardInformationService.responseFieldSchema = 'va_findotherawardinformationresponse';
        findOtherAwardInformationService.responseTimestamp = 'va_webserviceresponse';
        findOtherAwardInformationService.executeRequest();
        findOtherAwardInformationService.context.updateProgress = false;

        awardTempXml = _XML_UTIL.parseXmlObject(findOtherAwardInformationService.responseXml);

//        xmlDoc = awardTempXml;
//        for (a in awards) {
//            xmlEl = xmlDoc.createElement(a);
//            xmlText = xmlDoc.createTextNode(awards[a]);
//            xmlEl.appendChild(xmlText);

//            xmlDoc = xmlDoc.getElementsByTagName('return')[0];
//            xmlDoc.appendChild(xmlEl);
//            xmlDoc = _XML_UTIL.parseXmlObject(xmlDoc.xml);
//        }

//        awardTempXml = xmlDoc;
        awardResponses.push(awardTempXml);

        var findIncomeExpensesByPtcptId = new findIncomeExpense(this.context);
        findIncomeExpensesByPtcptId.serviceName = 'findIncomeExpense ';
        findIncomeExpensesByPtcptId.wsMessage.serviceName = 'findIncomeExpense ';
        findIncomeExpensesByPtcptId.wsMessage.friendlyServiceName = 'Income/Expense';
        findIncomeExpensesByPtcptId.responseFieldSchema = 'va_findincomeexpenseresponse';
        findIncomeExpensesByPtcptId.responseTimestamp = 'va_webserviceresponse';
        findIncomeExpensesByPtcptId.executeRequest();
        findIncomeExpensesByPtcptId.context.updateProgress = false;

        incomeTempXml = _XML_UTIL.parseXmlObject(findIncomeExpensesByPtcptId.responseXml);
        incomeExpenseResponses.push(incomeTempXml);
    }

    var fullGenInfoResponse = _XML_UTIL.concatenateDocs(genInfoResponses, 'GeneralInfoResponses');
    var fullAwardResponse = _XML_UTIL.concatenateDocs(awardResponses, 'AwardResponses');
    var fullIncomeExpenseResponse = _XML_UTIL.concatenateDocs(incomeExpenseResponses, 'IncomeExpenseResponses');

    Xrm.Page.getAttribute('va_generalinformationresponsebypid').setValue(fullGenInfoResponse.xml);
    Xrm.Page.getAttribute('va_findotherawardinformationresponse').setValue(fullAwardResponse.xml);
    Xrm.Page.getAttribute('va_findincomeexpenseresponse').setValue(fullIncomeExpenseResponse.xml);
}
// END VeteranAwardInformation
//=====================================================================================================