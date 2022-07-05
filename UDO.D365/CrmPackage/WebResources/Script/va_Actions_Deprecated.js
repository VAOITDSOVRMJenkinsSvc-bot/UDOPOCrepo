﻿/// <reference path="/Intellisense/XrmPage-vsdoc.js" />

/* 
EX context record {user: userRecord, findRecords: [], soapBodyInnerXml: [], webserviceRequestUrl: []}
*/

var actionContext = function () { }
actionContext.prototype.constructor = actionContext;
actionContext.prototype.user;
actionContext.prototype.fileNumber;
actionContext.prototype.participantId;
actionContext.prototype.refreshExtjs;

var action = function (context) {
    this.context = context;
}
action.prototype.performAction = function () { }

var veteranSearch = function (context) {
    this.context = context;
}
veteranSearch.prototype = new action;
veteranSearch.prototype.constructor = veteranSearch;

veteranSearch.prototype.performAction = function () {

    var findVeteranWebService;
    var findVeteranResponseXml;
    var findVeteranResponseXmlObject;

    var fileNumber = Xrm.Page.getAttribute('va_ssn').getValue();
    var participantId = Xrm.Page.getAttribute('va_participantid').getValue();

    if (fileNumber/* || participantId*/) {
        /*        if (participantId) {
        this.context.participantId = participantId;
        findVeteranWebService = new findVeteranByparticipantId(this.context);
        }
        else {      */
        this.context.fileNumber = fileNumber;
        findVeteranWebService = new findVeteranByFileNumber(this.context);
        // }
    }
    else {//Search is by name
        findVeteranWebService = new findVeteran(this.context);
    }

    findVeteranWebService.responseFieldSchema = 'va_findveteranresponse';
    findVeteranWebService.responseTimestamp = 'va_webserviceresponse';
    if (findVeteranWebService.executeRequest()) {
        findVeteranResponseXml = Xrm.Page.getAttribute(findVeteranWebService.responseFieldSchema).getValue();
        findVeteranResponseXmlObject = parseXmlObject(findVeteranResponseXml);

  
        var findCorpRecordCount =
                findVeteranResponseXmlObject.selectSingleNode('//numberOfRecords') != null ?
                findVeteranResponseXmlObject.selectSingleNode('//numberOfRecords').text : 1;

        //Store in Global for EXTJS processing
        _CORP_RECORD_COUNT = parseInt(findCorpRecordCount);

        var findBirlsRecordCount =
                findVeteranResponseXmlObject.selectSingleNode('//NUMBER_OF_RECORDS') != null ?
                findVeteranResponseXmlObject.selectSingleNode('//NUMBER_OF_RECORDS').text : 0;

        //Store in Global for EXTJS processing
        _BIRLS_RECORD_COUNT = parseInt(findBirlsRecordCount);


        if (findCorpRecordCount == 1) {
            if (fileNumber == null) {
                fileNumber = findVeteranResponseXmlObject.selectSingleNode('//fileNumber').text;
            }

            this.context.fileNumber = fileNumber;
            this.context.refreshExtjs = false;
            var findVeteranGeneralInfo = new veteranSearchGeneralAndBenefitInformation(this.context);
            findVeteranGeneralInfo.performAction();

            var rtContact = new contact();
            rtContact.parseCorpRecord(findVeteranResponseXmlObject);

            MarkAsRelatedVeteran(rtContact /*findVeteranResponseXmlObject*/);
            _iframeSrc = '';
            _iframeSrc = _IFRAME_SOURCE_SINGLE;
        }

        // update other UI
        crmForm.all.from.title = "Branch: Army\n2 open claims\nFlash 1: disabled veteran\nFlash 2: code x430";
        crmForm.all.from_c.title = "Branch: Army\n2 open claims\nFlash 1: disabled veteran\nFlash 2: code x430";

        // refresh iframe to apply data
        // toggle visibility
        Xrm.Page.ui.tabs.get('tab_search').sections.get('phone searchresults').setVisible(true);
        var iframeObject = Xrm.Page.getControl('IFRAME_search').setSrc(_iframesrc);
    }
}

var veteranSearchByFileNumber = function (context) {
    this.context = context;
}
veteranSearchByFileNumber.prototype = new action;
veteranSearchByFileNumber.prototype.constructor = veteranSearchByFileNumber;
veteranSearchByFileNumber.prototype.performAction = function () {

}

var veteranSearchGeneralAndBenefitInformation = function (context) {
    this.context = context;
}
veteranSearchGeneralAndBenefitInformation.prototype = new action;
veteranSearchGeneralAndBenefitInformation.prototype.constructor = veteranSearchGeneralAndBenefitInformation;
veteranSearchGeneralAndBenefitInformation.prototype.performAction = function () {
    //Create instance of general info web service
    var findVeteranGeneralInfoWebService = new findGeneralInformationByFileNumber(this.context);
    findVeteranGeneralInfoWebService.responseFieldSchema = 'va_generalinformationresponse';
    findVeteranGeneralInfoWebService.responseTimestamp = 'va_webserviceresponse';
    findVeteranGeneralInfoWebService.executeRequest();

    //Create instance of claim web service
    var findVeteranBenefitClaimWebService = new findBenefitClaim(this.context);
    findVeteranBenefitClaimWebService.responseFieldSchema = 'va_benefitclaimresponse';
    findVeteranBenefitClaimWebService.responseTimestamp = 'va_webserviceresponse';
    findVeteranBenefitClaimWebService.executeRequest();

    //Create instance of dependent web service
    var findVeteranDependentsWebService = new findDependents(this.context);
    findVeteranDependentsWebService.responseFieldSchema = 'va_finddependentsresponse';
    findVeteranDependentsWebService.responseTimestamp = 'va_webserviceresponse';
    findVeteranDependentsWebService.executeRequest();

    //Create instance of fiduciary web service
    var findVeteranFiduciaryPoaWebService = new findAllFiduciaryPoa(this.context);
    findVeteranFiduciaryPoaWebService.responseFieldSchema = 'va_findfiduciarypoaresponse';
    findVeteranFiduciaryPoaWebService.responseTimestamp = 'va_webserviceresponse';
    findVeteranFiduciaryPoaWebService.executeRequest();

    // refresh iframe to apply data
//    if (this.context.refreshExtjs != null && this.context.refreshExtjs) {
//        Xrm.Page.ui.tabs.get('tab_search').sections.get('phone searchresults').setVisible(true);

//        var orgname = Xrm.Page.context.getOrgUniqueName();
//        var scriptRoot = Xrm.Page.context.getServerUrl().replace(orgname, '');
//        var sourceURL = '';
//        sourceURL = scriptRoot + _IFRAME_SOURCE_SINGLE;
//        Xrm.Page.getControl('IFRAME_search').setSrc(sourceURL);
//    }
}

var veteranSearchIdentifyIndividual = function (context) {
    this.context = context;
}
veteranSearchIdentifyIndividual.prototype = new action;
veteranSearchIdentifyIndividual.prototype.constructor = veteranSearchIdentifyIndividual;
veteranSearchIdentifyIndividual.prototype.performAction = function () {
    //Update find veteran response xml
    var findVeteranWebService = new findVeteranByFileNumber(this.context);
    findVeteranWebService.responseFieldSchema = 'va_findveteranresponse';
    findVeteranWebService.responseTimestamp = 'va_webserviceresponse';
    findVeteranWebService.executeRequest();

    //Update or Create Contact
    var rtContact = new contact();
    rtContact.parseCorpRecord(parseXmlObject(Xrm.Page.getAttribute('va_findveteranresponse').getValue()));

    MarkAsRelatedVeteran(rtContact);
    //MarkAsRelatedVeteran(parseXmlObject(Xrm.Page.getAttribute('va_findveteranresponse').getValue()));

    //_iframeSrc = '';
    // refresh iframe to apply data
//    if (this.context.refreshExtjs != null && this.context.refreshExtjs) {
//        Xrm.Page.ui.tabs.get('tab_search').sections.get('phone searchresults').setVisible(true);

//        var orgname = Xrm.Page.context.getOrgUniqueName();
//        var scriptRoot = Xrm.Page.context.getServerUrl().replace(orgname, '');
//        var sourceURL = '';
//        sourceURL = scriptRoot + _IFRAME_SOURCE_SINGLE;
//        Xrm.Page.getControl('IFRAME_search').setSrc(sourceURL);
//    }
}

var shareGetRegionalOffices = function (context) {
    this.context = context;
}
shareGetRegionalOffices.prototype = new action;
shareGetRegionalOffices.prototype.constructor = shareGetRegionalOffices;
shareGetRegionalOffices.prototype.performAction = function () {
    var getRegionalOffices = new findRegionalOffices(this.context);
    getRegionalOffices.responseFieldSchema = 'va_roxml';
    getRegionalOffices.responseTimestamp = 'va_wstimestamp';
    getRegionalOffices.executeRequest();
}