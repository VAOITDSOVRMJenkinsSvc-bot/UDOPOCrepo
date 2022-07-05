﻿//=====================================================================================================
// START PaymentHistoryService
//=====================================================================================================
var paymentHistoryService = function (context) {
    this.context = context;
    this.webserviceRequestUrl = WebServiceURLRoot + 'PaymentHistoryWebServiceBean/PaymentHistoryWebService';
    this.useExternalUid = true;  // instructs to use <vaws:ExternalUid>aide\driz</vaws:ExternalUid> <vaws:ExternalKey>aide\driz</vaws:ExternalKey>

    this.prefix = 'pay';
    this.prefixUrl = 'http://paymenthistory.services.ebenefits.vba.va.gov/';
}
paymentHistoryService.prototype = new webservice;
paymentHistoryService.prototype.constructor = paymentHistoryService;
//=====================================================================================================
// START Individual PaymentHistoryService Methods
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
//=====================================================================================================
//START findPayHistoryBySSN
//EX Request:
var findPayHistoryBySSN = function (context) {
    this.context = context;

    this.serviceName = 'findPayHistoryBySSN';
    this.responseFieldSchema = 'va_findpaymenthistoryresponse';
    this.responseTimestamp = 'va_webserviceresponse';

    this.wsMessage = new webServiceMessage();
    this.wsMessage.methodName = 'PaymentHistoryService.findPayHistoryBySSN';
    this.wsMessage.serviceName = 'findPayHistoryBySSN';
    this.wsMessage.friendlyServiceName = 'Payment History';
    

    this.requiredSearchParameters = new Array();
    this.requiredSearchParameters['ssn'] = null;
    this.ignoreRequiredParMissingWarning = true;
}
findPayHistoryBySSN.prototype = new paymentHistoryService;
findPayHistoryBySSN.prototype.constructor = findPayHistoryBySSN;
findPayHistoryBySSN.prototype.buildSoapBodyInnerXml = function () {
    this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

    var innerXml;
    var ssn = this.context.parameters['ssn'];
    if (!ssn || ssn.length == 0) { ssn = this.context.parameters['fileNumber']; }

    if (ssn && ssn != '') {
        innerXml = '<pay:findPayHistoryBySSN><ssn>' + ssn +
        '</ssn></pay:findPayHistoryBySSN>';
    }
    else {
        this.wsMessage.errorFlag = true;
        this.wsMessage.description = 'There must be a FIle Number or SSN present for the Payment History request';
        return null;
    }

    return innerXml;
}
//END findPayHistoryBySSN
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
// END Individual PaymentHistoryService Methods
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
// END PaymentHistoryService
//=====================================================================================================


//=====================================================================================================
// START PaymentInformationService 
//=====================================================================================================
var paymentInfoService = function (context) {
    this.context = context;
    this.webserviceRequestUrl = WebServiceURLRoot + 'vrm-ws/PaymentInformationService'; //WebServiceURLRoot + 'http://bepcert.vba.va.gov/ PaymentInformationService';
    this.useExternalUid = true;  // instructs to use <vaws:ExternalUid>aide\driz</vaws:ExternalUid> <vaws:ExternalKey>aide\driz</vaws:ExternalKey>

    this.prefix = 'ws';
    this.prefixUrl = 'http://ws.vrm.benefits.vba.va.gov/';
}
paymentInfoService.prototype = new webservice;
paymentInfoService.prototype.constructor = paymentInfoService;

//START retrievePaymentSummary
var retrievePaymentSummary = function (context) {
    this.context = context;

    this.serviceName = 'retrievePaymentSummary';
    this.responseFieldSchema = 'va_retrievepaymentsummaryresponse';
    this.responseTimestamp = 'va_webserviceresponse';

    this.wsMessage = new webServiceMessage();
    this.wsMessage.methodName = 'PaymentInfoService.retrievePaymentSummary';
    this.wsMessage.serviceName = 'retrievePaymentSummary';
    this.wsMessage.friendlyServiceName = 'FAS Payment Info';


    this.requiredSearchParameters = new Array();
    this.requiredSearchParameters['ptcpntId'] = null;
}
retrievePaymentSummary.prototype = new paymentInfoService;
retrievePaymentSummary.prototype.constructor = retrievePaymentSummary;
retrievePaymentSummary.prototype.buildSoapBodyInnerXml = function () {
    this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

    var innerXml;
    var pid = this.context.parameters['ptcpntId'];

    if (pid && pid != '') {
        innerXml = '<ws:retrievePaymentSummary><ParticipantId>' + pid + '</ParticipantId></ws:retrievePaymentSummary>';
    }
    else {
        this.wsMessage.errorFlag = true;
        this.wsMessage.description = 'There must be a Participant Id present for the FAS Payment Info request';
        return null;
    }

    return innerXml;
}
//END retrievePaymentSummary

//START retrievePaymentDetail
var retrievePaymentDetail = function (context) {
    this.context = context;

    this.serviceName = 'retrievePaymentDetail';
    this.responseFieldSchema = 'va_retrievepaymentdetailresponse';
    this.responseTimestamp = 'va_webserviceresponse';

    this.wsMessage = new webServiceMessage();
    this.wsMessage.methodName = 'PaymentInfoService.retrievePaymentDetail';
    this.wsMessage.serviceName = 'retrievePaymentDetail';
    this.wsMessage.friendlyServiceName = 'FAS Payment Details';


    this.requiredSearchParameters = new Array();
    this.requiredSearchParameters['PaymentId'] = null;
    this.suppressProgressDlg = true;
}
retrievePaymentDetail.prototype = new paymentInfoService;
retrievePaymentDetail.prototype.constructor = retrievePaymentDetail;
retrievePaymentDetail.prototype.buildSoapBodyInnerXml = function () {
    this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
    var innerXml;

    var PaymentId = this.context.parameters['PaymentId'];

    if (PaymentId && PaymentId != '') {
        innerXml = '<ws:retrievePaymentDetail><PaymentId>' + PaymentId + '</PaymentId></ws:retrievePaymentDetail>';
    }
    else {
        this.wsMessage.errorFlag = true;
        this.wsMessage.description = 'There must be a Payment Id present for the FAS Payment Detail request';
        return null;
    }

    return innerXml;
}
//END retrievePaymentDetail

//=====================================================================================================
// START PaymentInformationService 
//=====================================================================================================