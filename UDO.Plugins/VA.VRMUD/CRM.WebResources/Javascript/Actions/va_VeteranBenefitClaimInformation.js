﻿/// <reference path="/Intellisense/XrmPage-vsdoc.js" />
//=====================================================================================================
// START VeteranGeneralInformationBenefitAndClaim
//=====================================================================================================
var veteranGeneralInformationBenefitAndClaim = function (context) {
    this.context = context;
    this.actionMessage = new actionsMessage();
    this.actionMessage.name = 'veteranGeneralInformation';
    this.actionMessage.documentElement = 'actionsMessage';
    this.actionMessage.stackTrace = '';
    this.webservices = new Array();
    this.analyzers = new Array();
    this.performAction = veteranGeneralInformationBenefitAndClaim_performAction;
}
veteranGeneralInformationBenefitAndClaim.prototype = new search;
veteranGeneralInformationBenefitAndClaim.prototype.constructor = veteranGeneralInformationBenefitAndClaim;
var veteranGeneralInformationBenefitAndClaim_performAction = function () {
    this.actionMessage.stackTrace = 'performAction();';

    //Create instance of fiduciary web service
    this.webservices['findAllFiduciaryPoa'] = new findAllFiduciaryPoa(this.context);

    if(!_searchCorpMin) {
        //Create instance of claim web service
        this.webservices['findBenefitClaim'] = new findBenefitClaim(this.context);

        //Create instance of dependent web service
        this.webservices['findDependents'] = new findDependents(this.context);

        //Create instance of AllRelationships web service
        this.webservices['findAllRelationships'] = new findAllRelationships(this.context);

        //Create instance of DevelopmentNotes web service
        this.webservices['findDevelopmentNotes'] = new findDevelopmentNotes(this.context);

        //Create instance of Tracked Item - Find Unsolicited Evidence
        this.webservices['findUnsolEvdnce'] = new findUnsolEvdnce(this.context);

        //Create instance of PaymentHistory web service
        this.webservices['findPayHistoryBySSN'] = new findPayHistoryBySSN(this.context);

        //Create instance of MilitaryService web service
        this.webservices['findMilitaryRecordByPtcpntId'] = new findMilitaryRecordByPtcpntId(this.context);

        //Create instance of findRatingData web service
        this.webservices['findRatingData'] = new findRatingData(this.context);

        //Create instance of findDenialsByPtcpntId web service
        this.webservices['findDenialsByPtcpntId'] = new findDenialsByPtcpntId(this.context);


        this.webservices['findMonthOfDeath'] = new findMonthOfDeath(this.context);

        //Create instance of findAllPtcpntAddrsByPtcpntId web service
        this.webservices['findAllPtcpntAddrsByPtcpntId'] = new findAllPtcpntAddrsByPtcpntId(this.context);
    }

    this.executeSearchOperations(this.webservices);
    return !this.hasErrors;
}
// END VeteranGeneralInformationBenefitAndClaim
//=====================================================================================================