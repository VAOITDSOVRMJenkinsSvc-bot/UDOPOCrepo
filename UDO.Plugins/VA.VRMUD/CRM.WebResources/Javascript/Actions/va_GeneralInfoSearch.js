var genInfoSearch = function (context) {
    this.context = context;
    this.actionMessage = new actionsMessage();
    this.actionMessage.name = 'genInfoSearch';
    this.actionMessage.documentElement = 'actionsMessage';
    this.actionMessage.stackTrace = '';
    this.webservices = new Array();
    this.analyzers = new Array();
    this.performAction = genInfoSearch_performAction;
    this.analyzeFindGeneralInformationByFileNumberResult = analyzeFindGeneralInformationByFileNumberResult;
    this.analyzeFindGeneralInformationByPtcpntIdsResult = analyzeFindGeneralInformationByPtcpntIdsResult;
    this.analyzeFindFiduciaryResult = analyzeFindFiduciaryResult;
}
genInfoSearch.prototype = new search;
genInfoSearch.prototype.constructor = genInfoSearch;
var genInfoSearch_performAction = function () {
    this.context.stackTrace = 'performAction();';
    this.hasErrors = false;

    this.webservices['findGeneralInformationByFileNumber'] = new findGeneralInformationByFileNumber(this.context);
    this.analyzers['findGeneralInformationByFileNumber'] = this.analyzeFindGeneralInformationByFileNumberResult;

    this.webservices['findGeneralInformationByPtcpntIds'] = new findGeneralInformationByPtcpntIds(this.context);
    this.analyzers['findGeneralInformationByPtcpntIds'] = this.analyzeFindGeneralInformationByPtcpntIdsResult;

    this.analyzers['findFiduciary'] = this.analyzeFindFiduciaryResult;

    this.executeSearchOperations(this.webservices);

    UpdateSearchListObject({ name: 'genInfoSearch', complete: !this.hasErrors });

    return !this.hasErrors;
}
var analyzeFindGeneralInformationByFileNumberResult = function (parentObject) {
    var genXml = Xrm.Page.getAttribute('va_generalinformationresponse').getValue();

    if (genXml && genXml != '') {
        var genXmlObject = _XML_UTIL.parseXmlObject(genXml);
        ptcpntBeneId = null;
        ptcpntRecipId = null;
        awardTypeCd = null;
        ptcpntVetId = null;

        if (genXmlObject && genXmlObject.xml && genXmlObject.xml != '') {
            var recordCount = 0;

            if (genXmlObject.selectSingleNode('//numberOfAwardBenes')
                && genXmlObject.selectSingleNode('//numberOfAwardBenes').text
                && genXmlObject.selectSingleNode('//numberOfAwardBenes').text.length > 0) {
                recordCount = parseInt(genXmlObject.selectSingleNode('//numberOfAwardBenes').text);
            }

            _AWARDBENEFIT_RECORD_COUNT = recordCount;

            var ptcpntVetIdXpath; var ptcpntBeneIdXpath; var ptpcntRecipIdXpath; var awardTypeCdXpath;

            if (recordCount == 1) {
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

            if (genXmlObject.selectSingleNode(ptcpntVetIdXpath)
                && genXmlObject.selectSingleNode(ptcpntVetIdXpath).text
                && genXmlObject.selectSingleNode(ptcpntVetIdXpath).text.length > 0) {
                ptcpntVetId = genXmlObject.selectSingleNode(ptcpntVetIdXpath).text
            }

            if (genXmlObject.selectSingleNode(ptcpntBeneIdXpath)
                && genXmlObject.selectSingleNode(ptcpntBeneIdXpath).text
                && genXmlObject.selectSingleNode(ptcpntBeneIdXpath).text.length > 0) {
                ptcpntBeneId = genXmlObject.selectSingleNode(ptcpntBeneIdXpath).text
            }

            if (genXmlObject.selectSingleNode(ptpcntRecipIdXpath)
                && genXmlObject.selectSingleNode(ptpcntRecipIdXpath).text
                && genXmlObject.selectSingleNode(ptpcntRecipIdXpath).text.length > 0) {
                ptcpntRecipId = genXmlObject.selectSingleNode(ptpcntRecipIdXpath).text
            }

            if (genXmlObject.selectSingleNode(awardTypeCdXpath)
                && genXmlObject.selectSingleNode(awardTypeCdXpath).text
                && genXmlObject.selectSingleNode(awardTypeCdXpath).text.length > 0) {
                awardTypeCd = genXmlObject.selectSingleNode(awardTypeCdXpath).text
            }

            // check if no award
            if (ptcpntVetId && !ptcpntBeneId && !ptcpntRecipId && !awardTypeCd) {
                parentObject.context.parameters['ptcpntVetId'] = ptcpntVetId;
                parentObject.context.parameters['ptcpntBeneId'] = '';
                parentObject.context.parameters['ptcpntRecipId'] = '';
                parentObject.context.parameters['awardTypeCd'] = '';
                _AWARDBENEFIT_RECORD_COUNT = 0;
                // TODO how to terminate properly
                //parentObject.context.endState = true; DR: no need to stop here
                parentObject.hasErrors = false;

                Xrm.Page.getAttribute('va_generalinformationresponsebypid').setValue(genXml);

                UpdateSearchOptionsObject(parentObject.context);

                return;
            }

            if (ptcpntVetId && ptcpntBeneId && ptcpntRecipId && awardTypeCd) {
                parentObject.context.parameters['ptcpntVetId'] = ptcpntVetId;
                parentObject.context.parameters['ptcpntBeneId'] = ptcpntBeneId;
                parentObject.context.parameters['ptcpntRecipId'] = ptcpntRecipId;
                parentObject.context.parameters['awardTypeCd'] = awardTypeCd;

                parentObject.webservices['findGeneralInformationByPtcpntIds'].context = parentObject.context;

                var awardKey = awardTypeCd + ptcpntBeneId + ptcpntRecipId + ptcpntVetId;
                parentObject.context.parameters['awardKey'] = awardKey;

                var tempXml = genXmlObject;

                awardKeyNode = tempXml.createElement("awardKey");
                awardKeyTextNode = tempXml.createTextNode(awardKey);
                awardKeyNode.appendChild(awardKeyTextNode);

                if (SingleNodeExists(genXmlObject, '//payeeSSN')) {
                    parentObject.context.parameters['payeeSSN'] = genXmlObject.selectSingleNode('//payeeSSN').text;
                    parentObject.webservices['findFiduciary'] = new findFiduciary(parentObject.context);
                }

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

                if (tempXml && tempXml.xml && tempXml.xml != '') {
                    Xrm.Page.getAttribute('va_generalinformationresponse').setValue(tempXml.xml);
                }
                UpdateSearchOptionsObject(parentObject.context);
            }
            else {
                parentObject.actionMessage.errorFlag = true;
                parentObject.actionMessage.description = 'Find General Information By File Number Web Service call did not return the desired results.';
                parentObject.actionMessage.xmlResponse = genXml;
                parentObject.actionMessage.pushMessage();
                parentObject.endState = true;
            }
        }
    }

    return;
}
var analyzeFindGeneralInformationByPtcpntIdsResult = function (parentObject) {
    var genXml = Xrm.Page.getAttribute('va_generalinformationresponsebypid').getValue();

    if (genXml && genXml != '') {
        var genXmlObject = _XML_UTIL.parseXmlObject(genXml);

        if (genXmlObject && genXmlObject.xml && genXmlObject.xml != '') {
            if (parentObject.context.parameters['ptcpntVetId'] && parentObject.context.parameters['ptcpntBeneId']
                && parentObject.context.parameters['ptcpntRecipId'] && parentObject.context.parameters['awardTypeCd']) {
                var awardKey = parentObject.context.parameters['awardTypeCd'] + parentObject.context.parameters['ptcpntBeneId']
                    + parentObject.context.parameters['ptcpntRecipId'] + parentObject.context.parameters['ptcpntVetId'];

                if (awardKey) {
                    parentObject.context.parameters['awardKey'] = awardKey;

                    if (SingleNodeExists(genXmlObject, '//payeeSSN')) {
                        parentObject.context.parameters['payeeSSN'] = genXmlObject.selectSingleNode('//payeeSSN').text;
                        parentObject.webservices['findFiduciary'] = new findFiduciary(parentObject.context);
                    }

                    var tempXml = genXmlObject;

                    awardKeyNode = tempXml.createElement("awardKey");
                    awardKeyTextNode = tempXml.createTextNode(awardKey);
                    awardKeyNode.appendChild(awardKeyTextNode);

                    var genInfoPidNodes = tempXml.selectNodes('//return');

                    if (genInfoPidNodes) {
                        for (j = 0; j < genInfoPidNodes.length; j++) {
                            genInfoPidNodes[j].appendChild(awardKeyNode.cloneNode(true));
                        }
                    }

                    if (tempXml && tempXml.xml && tempXml.xml != '') {
                        Xrm.Page.getAttribute('va_generalinformationresponsebypid').setValue(tempXml.xml);
                    }
                }
                UpdateSearchOptionsObject(parentObject.context);
            }
        }
    }
    else {
        parentObject.actionMessage.errorFlag = true;
        parentObject.actionMessage.description = 'Find General Information By Ptcpnt Ids Web Service call did not return the desired results.';
        parentObject.actionMessage.xmlResponse = genXml;
        parentObject.actionMessage.pushMessage();
        parentObject.endState = true;
    }
    return;
}
var analyzeFindFiduciaryResult = function (parentObject) {
    var fiduciaryXml = Xrm.Page.getAttribute('va_awardfiduciaryresponse').getValue();
    if (fiduciaryXml && fiduciaryXml != undefined && fiduciaryXml != '') {
        var fiduciaryXmlObject = _XML_UTIL.parseXmlObject(fiduciaryXml);
        if (fiduciaryXmlObject && fiduciaryXmlObject != undefined && fiduciaryXmlObject.xml
            && fiduciaryXmlObject.xml != undefined && fiduciaryXmlObject.xml != '') {
            var awardKey = parentObject.context.parameters['awardKey'];
            tempXml = fiduciaryXmlObject;

            awardKeyNode = tempXml.createElement("awardKey");
            awardKeyTextNode = tempXml.createTextNode(awardKey);
            awardKeyNode.appendChild(awardKeyTextNode);

            var fiduciaryNodes = tempXml.selectNodes('//return');

            if (fiduciaryNodes) {
                for (j = 0; j < fiduciaryNodes.length; j++) {
                    fiduciaryNodes[j].appendChild(awardKeyNode.cloneNode(true));
                }
            }

            if (tempXml && tempXml.xml && tempXml.xml != '') {
                Xrm.Page.getAttribute('va_awardfiduciaryresponse').setValue(tempXml.xml);
            }
        }
    }
}