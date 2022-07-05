﻿var claimSearch = function (context) {
    this.context = context;
    this.actionMessage = new actionsMessage();
    this.actionMessage.name = 'claimSearch';
    this.actionMessage.documentElement = 'actionsMessage';
    this.actionMessage.stackTrace = '';
    this.webservices = new Array();
    this.analyzers = new Array();
    this.performAction = claimSearch_performAction;
    this.analyzeFindBenefitClaimResult = analyzeFindBenefitClaimResult;
    this.analyzeFindBenefitClaimDetailResult = analyzeFindBenefitClaimDetailResult;
}
claimSearch.prototype = new search;
claimSearch.prototype.constructor = claimSearch;
var claimSearch_performAction = function () {
    this.context.stackTrace = 'performAction();';
    this.hasErrors = false;
    
    this.webservices['findBenefitClaim'] = new findBenefitClaim(this.context);
    this.analyzers['findBenefitClaim'] = this.analyzeFindBenefitClaimResult;

    

    /*
    this.webservices['findClaimStatus'] = new findClaimStatus(this.context);
    this.webservices['findTrackedItems'] = new findTrackedItems(this.context);
    */
    
    this.webservices['findDevelopmentNotes'] = new findDevelopmentNotes(this.context);
    


    this.executeSearchOperations(this.webservices);

    UpdateSearchListObject({ name: 'claimsSearch', complete: !this.hasErrors });

    return !this.hasErrors;
}
var analyzeFindBenefitClaimResult = function (parentObject) {
    var claimXml = Xrm.Page.getAttribute('va_benefitclaimresponse').getValue();

    //SS -- 3/1/12
    debugger;

    if (claimXml && claimXml != '') {
        var claimXmlObject = _XML_UTIL.parseXmlObject(claimXml);

        if (claimXmlObject && claimXmlObject.xml && claimXmlObject.xml != '') {
            var claimId;

            if (claimXmlObject.selectSingleNode('//benefitClaimID')
                && claimXmlObject.selectSingleNode('//benefitClaimID').text
                && claimXmlObject.selectSingleNode('//benefitClaimID').text.length > 0) {
                claimId = claimXmlObject.selectSingleNode('//benefitClaimID').text;
                parentObject.context.parameters['claimId'] = claimId;

                parentObject.webservices['findBenefitClaimDetail'] = new findBenefitClaimDetail(parentObject.context);
                parentObject.analyzers['findBenefitClaimDetail'] = parentObject.analyzeFindBenefitClaimDetailResult;
                parentObject.webservices['findUnsolEvdnce'] = new findUnsolEvdnce(parentObject.context);
                parentObject.webservices['findContentionsByPtcpntId'] = new findContentionsByPtcpntId(parentObject.context);

                //                parentObject.webservices['findBenefitClaimDetail'].context = parentObject.context;
                //                parentObject.webservices['findDevelopmentNotes'].context = parentObject.context;
                //                parentObject.webservices['findUnsolEvdnce'].context = parentObject.context;
                //                parentObject.webservices['findContentionsByPtcpntId'].context = parentObject.context;

            }
            else {
                _CLAIM_RECORD_COUNT = 0;
                // TODO how to terminate properly
                //parentObject.context.endState = true;
                //parentObject.hasErrors = false;
                return;
            }
        }
    }

    return;
}
var analyzeFindBenefitClaimDetailResult = function (parentObject) {
    var claimXml = Xrm.Page.getAttribute('va_findbenefitdetailresponse').getValue();

    if (claimXml && claimXml != '') {
        var claimXmlObject = _XML_UTIL.parseXmlObject(claimXml);

        if (claimXmlObject && claimXmlObject.xml && claimXmlObject.xml != '') {
            var tempXml = claimXmlObject;

            var suspenseNodes = tempXml.selectNodes('//suspenceRecords');
            var benefitClaimNodes = tempXml.selectNodes('//benefitClaimID');
            var benefitClaimNode = benefitClaimNodes[0];

            for (j = 0; j < suspenseNodes.length; j++) {
                suspenseNodes[j].appendChild(benefitClaimNode.cloneNode(true));
            }

            if (tempXml && tempXml.xml && tempXml.xml != '') {
                Xrm.Page.getAttribute('va_findbenefitdetailresponse').setValue(tempXml.xml);
            }
        }
    }

    return;
}