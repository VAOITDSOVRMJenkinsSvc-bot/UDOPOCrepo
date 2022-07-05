﻿//=====================================================================================================
// START RatingService
//=====================================================================================================
var ratingService = function (context) {
    this.context = context;
    this.webserviceRequestUrl = WebServiceURLRoot + 'RatingServiceBean/RatingWebService';

    this.prefix = 'ser';
    this.prefixUrl = 'http://services.share.benefits.vba.va.gov/';
}
ratingService.prototype = new webservice;
ratingService.prototype.constructor = ratingService;
//=====================================================================================================
// START Individual RatingService Methods
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
//=====================================================================================================
//START findRatingData
//EX Request:
//<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ser="http://services.share.benefits.vba.va.gov/">
//   <soapenv:Header/>
//   <soapenv:Body>
//      <ser:findRatingData>
//         <!--Optional:-->
//         <fileNumber>?</fileNumber>
//         <!--Optional:-->
//      </ser:findRatingData>
//   </soapenv:Body>
//</soapenv:Envelope>
var findRatingData = function (context) {
    this.context = context;

    this.serviceName = 'findRatingData';
    this.responseFieldSchema = 'va_findratingdataresponse';
    this.responseTimestamp = 'va_webserviceresponse';

    this.wsMessage = new webServiceMessage();
    this.wsMessage.methodName = 'RatingService.findRatingData';
    this.wsMessage.serviceName = 'findRatingData';
    this.wsMessage.friendlyServiceName = 'Rating Data';

    this.requiredSearchParameters = new Array();
    this.requiredSearchParameters['fileNumber'] = null;
}
findRatingData.prototype = new ratingService;
findRatingData.prototype.constructor = findRatingData;
findRatingData.prototype.buildSoapBodyInnerXml = function () {
    this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
    var innerXml;

    var fileNumber = this.context.parameters['fileNumber'];

    if (fileNumber && fileNumber != '') {
        innerXml = '<ser:findRatingData><fileNumber>' + fileNumber
        + '</fileNumber></ser:findRatingData>';
    }
    else {
        this.wsMessage.errorFlag = true;
        this.wsMessage.description = 'You must provide a File Number to perform the search';
        return null;
    }

    return innerXml;
}
//END findRatingData
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
//=====================================================================================================
// END Individual RatingService Methods
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
//=====================================================================================================
// END RatingService
//=====================================================================================================