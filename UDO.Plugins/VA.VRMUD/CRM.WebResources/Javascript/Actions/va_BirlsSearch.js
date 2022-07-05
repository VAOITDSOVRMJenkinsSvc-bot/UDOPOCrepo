﻿var birlsSearch = function (context) {
    this.context = context;
    this.actionMessage = new actionsMessage();
    this.actionMessage.name = 'birlsSearch';
    this.actionMessage.documentElement = 'actionsMessage';
    this.actionMessage.stackTrace = '';
    this.webservices = new Array();
    this.analyzers = new Array();
    this.performAction = birlsSearch_performAction;
    this.analyzeFindBirlsRecordResult = analyzeFindBirlsRecordResult;
}
birlsSearch.prototype = new search;
birlsSearch.prototype.constructor = birlsSearch;
var birlsSearch_performAction = function () {
    this.context.stackTrace = 'performAction();';
    this.hasErrors = false;

    var fileNumber;

    if (this.context.parameters['fileNumber'] && this.context.parameters['fileNumber'] != undefined
        && this.context.parameters['fileNumber'] != '') {
        fileNumber = this.context.parameters['fileNumber'];
    }
    else {
        fileNumber = Xrm.Page.getAttribute('va_ssn').getValue();
    }

    if (fileNumber && fileNumber != '') {
        this.context.parameters['fileNumber'] = fileNumber;
        this.webservices['findBirlsRecordByFileNumber'] = new findBirlsRecordByFileNumber(this.context);
        this.analyzers['findBirlsRecordByFileNumber'] = this.analyzeFindBirlsRecordResult;
    }
    else {
        this.webservices['findBirlsRecord'] = new findBirlsRecord(this.context);
        this.analyzers['findBirlsRecord'] = this.analyzeFindBirlsRecordResult;
    }

    this.executeSearchOperations(this.webservices);
    return !this.hasErrors;
}
var analyzeFindBirlsRecordResult = function (parentObject) {
    var birlsXml = Xrm.Page.getAttribute('va_findbirlsresponse').getValue();

    if (birlsXml && birlsXml != '') {
        var birlsXmlObject = _XML_UTIL.parseXmlObject(birlsXml);

        if (birlsXmlObject && birlsXmlObject.xml && birlsXmlObject.xml != '') {
            var findBirlsRecordCount = 0;
            var fileNumber; var ptcpntId; var ssn;

            if (birlsXmlObject.selectSingleNode('//NUMBER_OF_RECORDS')
                && birlsXmlObject.selectSingleNode('//NUMBER_OF_RECORDS').text
                && birlsXmlObject.selectSingleNode('//NUMBER_OF_RECORDS').text.length > 0) {
                findBirlsRecordCount = parseInt(birlsXmlObject.selectSingleNode('//NUMBER_OF_RECORDS').text);
            }

            if (birlsXmlObject.selectSingleNode('//CLAIM_NUMBER')
                && birlsXmlObject.selectSingleNode('//CLAIM_NUMBER').text
                && birlsXmlObject.selectSingleNode('//CLAIM_NUMBER').text.length > 0) {
                fileNumber = SingleNodeExists(birlsXmlObject, '//CLAIM_NUMBER') 
                    ? birlsXmlObject.selectSingleNode('//CLAIM_NUMBER').text : null;

                if(fileNumber && fileNumber != '') {
                    parentObject.context.parameters['fileNumber'] = fileNumber;
                }
            }

            if (birlsXmlObject.selectSingleNode('//SOC_SEC_NUMBER')
                && birlsXmlObject.selectSingleNode('//SOC_SEC_NUMBER').text
                && birlsXmlObject.selectSingleNode('//SOC_SEC_NUMBER').text.length > 0) {
                ssn = SingleNodeExists(birlsXmlObject, '//SOC_SEC_NUMBER') 
                    ? birlsXmlObject.selectSingleNode('//SOC_SEC_NUMBER').text : null;

                if(ssn && ssn != '') {
                    parentObject.context.parameters['ssn'] = ssn;
                }
            }            

            //Store in Global for EXTJS processing
            _BIRLS_RECORD_COUNT = findBirlsRecordCount;

            if (findBirlsRecordCount == 0) {
                var resNode = birlsXmlObject.selectSingleNode('//RETURN_CODE');
                var resMsg = null;
                if (resNode && resNode.text && resNode.text.length > 0) {
                    var txt = '';
                    if (birlsXmlObject.selectSingleNode('//RETURN_MESSAGE')
                        && birlsXmlObject.selectSingleNode('//RETURN_MESSAGE').text
                        && birlsXmlObject.selectSingleNode('//RETURN_MESSAGE').text.length > 0) {
                        txt = birlsXmlObject.selectSingleNode('//RETURN_MESSAGE').text;
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
                            warningMsg.xmlResponse = birlsXml;
                            warningMsg.pushMessage();
                            break;
                        default:
                            var warningMsg = new actionsMessage();
                            warningMsg.documentElement = 'actionMessage';
                            warningMsg.stackTrace = '';
                            warningMsg.errorFlag = true;
                            warningMsg.nodataFlag = true;
                            warningMsg.description = 'Find BIRLS Record Web Service call did not find any data matching input parameters.';
                            warningMsg.xmlResponse = birlsXml;
                            warningMsg.pushMessage();
                            break;
                    }
                }
            }
        }
    }

    return;
}