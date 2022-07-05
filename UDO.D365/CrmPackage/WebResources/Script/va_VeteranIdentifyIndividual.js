/// <reference path="/Intellisense/XrmPage-vsdoc.js" />
//Not Used.  Remove from repository.
var veteranIdentifyIndividual = function (context) {
    this.context = context;
    this.actionMessage = new actionsMessage();
    this.actionMessage.name = 'veteranIdentifyIndividual';
    this.actionMessage.documentElement = 'actionsMessage';
    this.actionMessage.stackTrace = '';
    this.webservices = new Array();
    this.analyzers = new Array();
    this.performAction = veteranIdentifyIndividual_performAction;
}
veteranIdentifyIndividual.prototype = new search;
veteranIdentifyIndividual.prototype.constructor = veteranIdentifyIndividual;
var veteranIdentifyIndividual_performAction = function () {
    this.actionMessage.stackTrace = 'performAction();';
    var findVeteranWebService;
    var findVeteranByFileNumberResponseXml;
    var findVeteranByFileNumberResponseXmlObject;
    var fileNumber; var participantId
    var POAFidXmlObject = null;

    fileNumber = this.context.fileNumber;
    //TODO: What to do if filenumber is different from ssn?
    participantId = this.context.ptcpntId;

    if (fileNumber) {
        findVeteranWebService = new vetRecordWebServiceByFileNumber(this.context);
        findVeteranWebService.serviceName = 'findVeteranByFileNumber';
        findVeteranWebService.wsMessage.serviceName = 'findVeteranByFileNumber';
        findVeteranWebService.wsMessage.friendlyServiceName = 'Veteran By File Number';
    }
    else if (participantId) {
        findVeteranWebService = new vetRecordWebServiceByParticipantId(this.context);
        findVeteranWebService.serviceName = 'findVeteranByPtcpntId';
        findVeteranWebService.wsMessage.serviceName = 'findVeteranByPtcpntId';
        findVeteranWebService.wsMessage.friendlyServiceName = 'Veteran By PtcpntId';
    }
    else {
        this.actionMessage.errorFlag = true;
        this.actionMessage.description = 'Either file number or participant id must be present for the search operation';
        this.actionMessage.pushMessage();
        return false;
    }

    findVeteranWebService.responseFieldSchema = 'va_findveteranresponse';
    findVeteranWebService.responseTimestamp = 'va_webserviceresponse';

    var additionalMsg = '';

    if (findVeteranWebService.executeRequest()) {

        findVeteranByFileNumberResponseXml = Xrm.Page.getAttribute('va_findveteranresponse').getValue();

        findVeteranByFileNumberResponseXmlObject = _XML_UTIL.parseXmlObject(findVeteranByFileNumberResponseXml);

        if (participantId == null) {
            participantId = findVeteranByFileNumberResponseXmlObject.selectSingleNode('//ptcpntId').text;
            this.context.ptcpntId = participantId;
        }

        var findVeteranGeneralInfo = new veteranGeneralInformation(this.context);
        findVeteranGeneralInfo.performAction();

        var findVeteranBenefitAndClaim = new veteranGeneralInformationBenefitAndClaim(this.context);
        findVeteranBenefitAndClaim.performAction();

        var findCorpRecordCount = 0;
        var findBirlsRecordCount = 0;
        if (findVeteranByFileNumberResponseXmlObject.selectSingleNode('//returnMessage') &&
                findVeteranByFileNumberResponseXmlObject.selectSingleNode('//returnMessage').text.length > 0)
            findCorpRecordCount = 1;

        if (findVeteranByFileNumberResponseXmlObject.selectSingleNode('//NUMBER_OF_RECORDS') &&
                findVeteranByFileNumberResponseXmlObject.selectSingleNode('//NUMBER_OF_RECORDS').text.length > 0)
            findBirlsRecordCount = parseInt(findVeteranByFileNumberResponseXmlObject.selectSingleNode('//NUMBER_OF_RECORDS').text);

        var findVeteranGeneralInfoResponseXml = null; var findVeteranGeneralInfoResponseXmlObject = null;
        findVeteranGeneralInfoResponseXml =
                    Xrm.Page.getAttribute('va_generalinformationresponsebypid').getValue();
        findVeteranGeneralInfoResponseXmlObject =
                    _XML_UTIL.parseXmlObject(findVeteranGeneralInfoResponseXml);

        var rtContact = new contact();
        var parseOK = false;
        if (findCorpRecordCount != 0) {
            var POAFidXml = Xrm.Page.getAttribute('va_findfiduciarypoaresponse').getValue();
            if (POAFidXml) {
                POAFidXmlObject = _XML_UTIL.parseXmlObject(POAFidXml);
            }

            parseOK = rtContact.parseCorpRecord(findVeteranByFileNumberResponseXmlObject, findVeteranGeneralInfoResponseXmlObject, POAFidXmlObject);
        }
        else {
            additionalMsg = '; NOTE: NO data for this person is found in CORPDB!';
            if (findBirlsRecordCount != 0)
                parseOK = rtContact.parseBIRLSRecord(findVeteranByFileNumberResponseXmlObject);
            else
                additionalMsg = '; NOTE: NO data for this person is found in neither CORPDB nor BIRLS!';
        }

        if (parseOK) {
            MarkAsRelatedVeteran(rtContact);
            _iframeSrc = '';
            _iframeSrc = _IFRAME_SOURCE_SINGLE;
        }
        else {
            this.actionMessage.errorFlag = true;
            this.actionMessage.description = 'Find Veteran By File Number did not find data.' + additionalMsg;
            this.actionMessage.xmlResponse = findVeteranGeneralInfoResponseXml;
            this.actionMessage.pushMessage();
            return false;
        }
    }
    else {
        this.actionMessage.errorFlag = true;
        this.actionMessage.description = 'Find Veteran By File Number Web Service call did not execute correctly.';
        this.actionMessage.xmlResponse = findVeteranWebService.responseXml;
        this.actionMessage.pushMessage();
        return false;
    }

    this.actionMessage.errorFlag = false;
    this.actionMessage.description = 'Identify Veteran Action completed successfully on ' + new Date() + additionalMsg;
    this.actionMessage.pushMessage();
    return true;
}