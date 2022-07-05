﻿/// <reference path="/Intellisense/XrmPage-vsdoc.js" />
//=====================================================================================================
// START VeteranSearch
//=====================================================================================================
var veteranSearch = function (context) {
    this.context = context;
    this.actionMessage.name = 'veteranSearch';
}
veteranSearch.prototype = new action;
veteranSearch.prototype.constructor = veteranSearch;
veteranSearch.prototype.performAction = function () {
    this.actionMessage.stackTrace += 'performAction()';

    var findCorpRecordService;
    var findVeteranWebService;

    var fileNumber = Xrm.Page.getAttribute('va_ssn').getValue();
    var participantId = Xrm.Page.getAttribute('va_participantid').getValue();
    var vetSSN = null;




    var findVeteranResponseXml;
    var findVeteranResponseXmlObject;



    var searchOption = 1;

    if (Xrm.Page.data.entity.getEntityName() == 'contact') { //contact screen
        if (participantId) {
            searchOption = 2; // search by pid
        } else {
            searchOption = 1; //search by filenumber
        }

    } else {                            //phone call screen has a drop down
        searchOption = Xrm.Page.getAttribute('va_searchtype').getSelectedOption().value; 
    }

    switch (parseInt(searchOption)) {
        case 2:
            // pid
            if (participantId) {
                if (Xrm.Page.data.entity.getEntityName() != 'contact') {
                    Xrm.Page.getAttribute('va_ssn').setValue(null);
                    Xrm.Page.getAttribute('va_firstname').setValue(null);
                    Xrm.Page.getAttribute('va_lastname').setValue(null);
                }
                this.context.ptcpntId = participantId;
                findVeteranWebService = new vetRecordWebServiceByParticipantId(this.context);
                findVeteranWebService.serviceName = 'findVeteranByPtcpntId';
                findVeteranWebService.wsMessage.serviceName = 'findVeteranByPtcpntId';
                findVeteranWebService.wsMessage.friendlyServiceName = 'Veteran by Participant Id';
            }
            else {
                alert('Participant ID field is blank.');
                return;
            }
            break;
        case 3:
            // edipi
            alert('Connection to the authorization store has not yet been granted. Please come back later.');
            return;
        case 1:
        default:
            // ssn + other params
            if (Xrm.Page.data.entity.getEntityName() != 'contact') { 
                Xrm.Page.getAttribute('va_participantid').setValue(null);
                Xrm.Page.getAttribute('va_edipi').setValue(null);
            }
            if (fileNumber) {
                this.context.fileNumber = fileNumber;
                findVeteranWebService = new vetRecordWebServiceByFileNumber(this.context);
                findVeteranWebService.serviceName = 'findVeteranByFileNumber';
                findVeteranWebService.wsMessage.serviceName = 'findVeteranByFileNumber';
                findVeteranWebService.wsMessage.friendlyServiceName = 'Veteran by File Number';
            }
            else {
                //Search is by name
                findVeteranWebService = new vetRecordWebServiceBySearchCriteria(this.context);
                findVeteranWebService.serviceName = 'findVeteran';
                findVeteranWebService.wsMessage.serviceName = 'findVeteran';
                findVeteranWebService.wsMessage.friendlyServiceName = 'Veteran by Name/SSN';
            }
            break;
    }

    findVeteranWebService.responseFieldSchema = 'va_findveteranresponse';
    findVeteranWebService.responseTimestamp = 'va_webserviceresponse';
    //debugger
    if (findVeteranWebService.executeRequest()) {
        findVeteranResponseXml = Xrm.Page.getAttribute(findVeteranWebService.responseFieldSchema).getValue();
        findVeteranResponseXmlObject = _XML_UTIL.parseXmlObject(findVeteranResponseXml);

        var findCorpRecordCount = 0;
        var findBirlsRecordCount = 0;
        if (findVeteranResponseXmlObject.selectSingleNode('//numberOfRecords') &&
                findVeteranResponseXmlObject.selectSingleNode('//numberOfRecords').text.length > 0)
            findCorpRecordCount = parseInt(findVeteranResponseXmlObject.selectSingleNode('//numberOfRecords').text);
        else {
            if (findVeteranResponseXmlObject.selectSingleNode('//vetCorpRecord')) findCorpRecordCount = 1;
        }

        if (findVeteranResponseXmlObject.selectSingleNode('//NUMBER_OF_RECORDS') &&
                findVeteranResponseXmlObject.selectSingleNode('//NUMBER_OF_RECORDS').text.length > 0)
            findBirlsRecordCount = parseInt(findVeteranResponseXmlObject.selectSingleNode('//NUMBER_OF_RECORDS').text);

        //Store in Global for EXTJS processing
        _CORP_RECORD_COUNT = parseInt(findCorpRecordCount);

        //Store in Global for EXTJS processing
        _BIRLS_RECORD_COUNT = parseInt(findBirlsRecordCount);
        // if no birls records, check if it got too many
        if (findBirlsRecordCount == 0) {
            var resNode = findVeteranResponseXmlObject.selectSingleNode('//RETURN_CODE');
            var resMsg = null;
            if (resNode) {
                var txt = '';
                if (findVeteranResponseXmlObject.selectSingleNode('//RETURN_MESSAGE')) {
                    txt = findVeteranResponseXmlObject.selectSingleNode('//RETURN_MESSAGE').text;
                }
                switch (resNode.text) {
                    case 'BPNQ0200':
                        var warningMsg = new actionsMessage();
                        warningMsg.documentElement = 'actionsMessage';
                        warningMsg.stackTrace = '';
                        warningMsg.warningFlag = true;
                        warningMsg.description = 'BIRLS Warning: ' + txt;
                        warningMsg.pushMessage();
                        break;
                    case 'BPNQ9900':
                        var warningMsg = new actionsMessage();
                        warningMsg.documentElement = 'actionsMessage';
                        warningMsg.stackTrace = '';
                        warningMsg.errorFlag = true;
                        warningMsg.description = 'BIRLS error: ' + txt;
                        warningMsg.xmlResponse = findVeteranResponseXml;
                        warningMsg.pushMessage();
                        break;
                    case 'BPNQ0100':
//                        var warningMsg = new actionsMessage();
//                        warningMsg.documentElement = 'actionsMessage';
//                        warningMsg.nodataFlag = true;
//                        warningMsg.description = txt;
//                        warningMsg.pushMessage();
                        break;
                    default:
                        break;
                }
            }
        }

        // TODO : analyze returns such as too many records in birls
        if (findCorpRecordCount == 1) {
            if (fileNumber == null) {
                fileNumber = findVeteranResponseXmlObject.selectSingleNode('//fileNumber') != null ? findVeteranResponseXmlObject.selectSingleNode('//fileNumber').text : null;
                //TODO:  Determine SSN processing
                vetSSN = findVeteranResponseXmlObject.selectSingleNode('//ssn') != null ? findVeteranResponseXmlObject.selectSingleNode('//ssn').text : null;

                if (vetSSN != fileNumber) {
                    var warningMsg = new actionsMessage();
                    warningMsg.documentElement = 'actionsMessage';
                    warningMsg.stackTrace = '';
                    warningMsg.warningFlag = true;
                    warningMsg.description = "Veteran's SSN is different from veteran's filenumber. Veteran's SSN was used for search.";
                    warningMsg.pushMessage();
                    fileNumber = vetSSN;
                }
            }

            if (participantId == null) {
                participantId = findVeteranResponseXmlObject.selectSingleNode('//ptcpntId') != null ? findVeteranResponseXmlObject.selectSingleNode('//ptcpntId').text : null;
            }

            this.context.fileNumber = fileNumber;
            this.context.ptcpntId = participantId;
            this.context.refreshExtjs = false;

            var findVeteranGeneralInfo = new veteranGeneralInformation(this.context);
            findVeteranGeneralInfo.performAction();

            if (_searchCorp) {
                var findVeteranBenefitAndClaim = new veteranGeneralInformationBenefitAndClaim(this.context);
                findVeteranBenefitAndClaim.performAction();
            }

            var findVeteranGeneralInfoResponseXml = null;
            var findVeteranGeneralInfoResponseXmlObject = null;

            findVeteranGeneralInfoResponseXml =
                    Xrm.Page.getAttribute('va_generalinformationresponsebypid').getValue();
            findVeteranGeneralInfoResponseXmlObject = _XML_UTIL.parseXmlObject(findVeteranGeneralInfoResponseXml);

            var POAFidXml = Xrm.Page.getAttribute('va_findfiduciarypoaresponse').getValue();
            var POAFidXmlObject = null;
            if (POAFidXml) {
                POAFidXmlObject = _XML_UTIL.parseXmlObject(POAFidXml);
            }

            var rtContact = new contact();
            rtContact.parseCorpRecord(findVeteranResponseXmlObject, findVeteranGeneralInfoResponseXmlObject, POAFidXmlObject);

            MarkAsRelatedVeteran(rtContact /*findVeteranResponseXmlObject*/);
            _iframeSrc = '';
            _iframeSrc = _IFRAME_SOURCE_SINGLE;
        }
        else {
            if (findCorpRecordCount == 0 && findBirlsRecordCount == 0) {
                this.actionMessage.errorFlag = true;
                this.actionMessage.nodataFlag = true;
                this.actionMessage.description = 'Find Veteran Web Service call did not find any data matching input parameters.';
                this.actionMessage.xmlResponse = findVeteranResponseXml;
                this.actionMessage.pushMessage();
                this.hasErrors = true;
                return false;
            }
        }
    }
    else {
        this.actionMessage.errorFlag = true;
        this.actionMessage.description = 'Find Veteran Web Service call did not execute correctly.';
        this.actionMessage.xmlResponse = findVeteranWebService.responseXml;
        this.actionMessage.pushMessage();
        return false;
    }

    this.actionMessage.errorFlag = false;
    this.actionMessage.description = 'Find Veteran Action completed successfully on ' + new Date();
    this.actionMessage.pushMessage();
    return true;
}
// END VeteranSearch
//=====================================================================================================