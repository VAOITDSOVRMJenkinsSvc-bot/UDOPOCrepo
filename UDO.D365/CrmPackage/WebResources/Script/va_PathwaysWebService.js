//=====================================================================================================
// START PathwaysWebService
//=====================================================================================================
var pathwaysWebService = function (context) {
    this.context = context;
    // test
    //this.webserviceRequestUrl = 'http://10.224.71.35:7251/repositories.med.va.gov/pathways';

    // prod
    //if (_isPROD == true) { this.webserviceRequestUrl = 'http://152.132.35.134:5035/repositories.med.va.gov/pathways'; }
    // PREPROD 'http://152.132.35.134:5035/repositories.med.va.gov/pathways'

    this.webserviceRequestUrl = _PathwaysServiceURLRoot;
    this.serviceDACUrl = _PathwaysDAC;
    this.prefix = 'pat';
    this.prefixUrl = 'http://repositories.med.va.gov/pathways';

    this.CDATA1 = "![CDATA[", this.CDATA1Rep = "_vrm.dctag_";
    this.CDATA2 = "]]", this.CDATA2Rep = "_vrm.dctag2_";

}
pathwaysWebService.prototype = new webservice;
pathwaysWebService.prototype.constructor = pathwaysWebService;
//=====================================================================================================
// START Individual PathwaysWebService Methods
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
//=====================================================================================================
//START isAlive
//EX Request:
//<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:pat="http://repositories.med.va.gov/pathways/">
//   <soapenv:Header/>
//   <soapenv:Body>
//      <pat:isAliveRequest/>
//   </soapenv:Body>
//</soapenv:Envelope>
var isAlive = function (context) {
    this.context = context;
    this.wsMessage.methodName = 'PathwaysWebService.isAlive';

    this.serviceName = 'isAlive';
    this.wsMessage.serviceName = 'isAlive';
    this.wsMessage.friendlyServiceName = 'Is Alive';
    this.responseFieldSchema = 'va_benefitclaimresponse';
    this.responseTimestamp = 'va_webserviceresponse';

    this.requiredSearchParameters = new Array();
    this.requiredSearchParameters['fileNumber'] = null;
}
isAlive.prototype = new pathwaysWebService;
isAlive.prototype.constructor = isAlive;
isAlive.prototype.buildSoapBodyInnerXml = function () {
    this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

    var innerXml = '<pat:isAliveRequest/>'

    return innerXml;
}
//END isAlive
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
//=====================================================================================================
//START readDataExam
//EX Request:
//<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:pat="http://repositories.med.va.gov/pathways/">
//   <soapenv:Header/>
//   <soapenv:Body>
//      <pat:readDataRequest>
//         <in0>ExamsRead1</in0>
//         <in1><![CDATA[<?xml version="1.0" encoding="UTF-8"?><filter:filter vhimVersion="Vhim_4_00" xmlns:filter="Filter"><filterId>REQUESTS_AND_EXAMS_SINGLE_PATIENT_FILTER</filterId><patients><NationalId>${#Project#PATIENTID_1}</NationalId></patients><entryPointFilter queryName="ExamRequests-Standardized"><domainEntryPoint>ExamRequests2507</domainEntryPoint></entryPointFilter><entryPointFilter queryName="Exams-Standardized"><domainEntryPoint>Exams2507</domainEntryPoint></entryPointFilter></filter:filter>]]></in1>
//         <in2>REQUESTS_AND_EXAMS_SINGLE_PATIENT_FILTER</in2>
//      </pat:readDataRequest>
//   </soapenv:Body>
//</soapenv:Envelope>
//=====================================================================================================
var readDataExamRequest = function (context) {
    this.context = context;

    this.serviceName = 'readDataExamRequest';
    this.wsMessage = new webServiceMessage();
    this.wsMessage.methodName = 'PathwaysWebService.readDataExamRequest';
    this.wsMessage.serviceName = 'readDataExamRequest';
    this.wsMessage.friendlyServiceName = 'Exam Request/Exam';
    this.responseFieldSchema = 'va_readdataexamresponse';
    this.responseTimestamp = 'va_webserviceresponse';

    this.requiredSearchParameters = new Array();
    this.requiredSearchParameters['nationalId'] = null;
}
readDataExamRequest.prototype = new pathwaysWebService;
readDataExamRequest.prototype.constructor = readDataExamRequest;
readDataExamRequest.prototype.buildSoapBodyInnerXml = function () {
    this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';
    debugger;
    var nationalId = this.context.parameters['nationalId']; /*'1012645531V508407';*/

    var timezone = '';
    var tzOffset = -new Date().getTimezoneOffset() / 60;
    if (Math.abs(tzOffset) < 10) { timezone += '-0' + Math.abs(tzOffset); }
    else { timezone += tzOffset; }
    timezone += ':00';

    var today = new Date().format('yyyy-MM-dd\'T\'HH:mm:ss') + 'Z'; // timezone;
    var clientRequestInitiationTime = '2001-12-17T09:30:47Z';
    clientRequestInitiationTime = today;

    var startDate = Xrm.Page.getAttribute('va_appointmentfromdate').getValue()
        ? new Date(Xrm.Page.getAttribute('va_appointmentfromdate').getValue()) : null;

    var endDate = Xrm.Page.getAttribute('va_appointmenttodate').getValue()
		? new Date(Xrm.Page.getAttribute('va_appointmenttodate').getValue()) : null;

    var startDateStr = null; var startDateObj;
    var endDateStr = null;

    if (startDate && startDate != undefined && startDate != NaN) {
        startDateStr = startDate.format('yyyy-MM-dd').toString();
    }
    else {
        startDateObj = new Date();
        startDateObj.setDate(startDateObj.getMonth() - 6);
        startDateStr = startDateObj.format('yyyy-MM-dd').toString();
        //Xrm.Page.getAttribute('va_appointmentfromdate').setValue(new Date(startDateStr));
    }

    if (endDate && endDate != undefined && endDate != NaN) { endDateStr = endDate.format('yyyy-MM-dd').toString(); }
    else {
        endDateStr = new Date().format('yyyy-MM-dd').toString();
        //Xrm.Page.getAttribute('va_appointmenttodate').setValue(new Date(endDateStr));
    }
    //debugger
    var innerXml = '<pat:readData><in0>RequestsAndExamsRead1</in0><in1><![CDATA[<?xml version="1.0" encoding="UTF-8"?>'
	+ '<filter:filter vhimVersion="Vhim_4_00" xmlns:filter="Filter"><filterId>REQUESTS_AND_EXAMS_SINGLE_PATIENT_FILTER</filterId>'
	+ '<clientName>VRM 1.0</clientName><clientRequestInitiationTime>' +
	clientRequestInitiationTime + '</clientRequestInitiationTime>'
	+ '<patients><NationalId>' + nationalId + '</NationalId></patients>' +

	'<entryPointFilter queryName="Exam-Standardized">' +
	'<domainEntryPoint>Exam2507</domainEntryPoint>' +
	 '<startDate>' + startDateStr + '</startDate><endDate>' + endDateStr + '</endDate>' +
	'</entryPointFilter>' +

	'<entryPointFilter queryName="ExamRequests-Standardized">'
	+ '<domainEntryPoint>ExamRequest2507</domainEntryPoint>' +
	'<startDate>' + startDateStr + '</startDate><endDate>' + endDateStr + '</endDate>' +
	'</entryPointFilter>' +
	'</filter:filter>]]></in1>'
	+ '<in2>REQUESTS_AND_EXAMS_SINGLE_PATIENT_FILTER</in2><in3>Request196AndExams_ReqId_06_23_0000</in3></pat:readData>';

    innerXml = innerXml.replace(this.CDATA1, this.CDATA1Rep).replace(this.CDATA2, this.CDATA2Rep);
    return innerXml;
}
// END readDataExam
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
//=====================================================================================================
//START readDataAppointment
//EX Request:
//<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:pat="http://repositories.med.va.gov/pathways/">
//   <soapenv:Header/>
//   <soapenv:Body>
//      <pat:readDataRequest>
//         <in0>AppointmentsRead1</in0>
//         <in1><![CDATA[<?xml version="1.0" encoding="UTF-8"?><filter:filter vhimVersion="Vhim_4_00" xmlns:filter="Filter"><filterId>APPOINTMENTS_SINGLE_PATIENT_FILTER</filterId><patients><NationalId>${#Project#PATIENTID_1}</NationalId></patients><entryPointFilter queryName="Appointments-Standardized"><domainEntryPoint>AppointmentEvent</domainEntryPoint></entryPointFilter></filter:filter>]]></in1>
//         <in2>APPOINTMENTS_SINGLE_PATIENT_FILTER</in2>
//      </pat:readDataRequest>
//   </soapenv:Body>
//</soapenv:Envelope>
//=====================================================================================================
var readDataAppointment = function (context) {
    this.context = context;

    this.serviceName = 'readDataAppointment';
    this.wsMessage = new webServiceMessage();
    this.wsMessage.methodName = 'PathwaysWebService.readDataAppointment';
    this.wsMessage.serviceName = 'readDataAppointment';
    this.wsMessage.friendlyServiceName = 'Appointments Request';
    this.responseFieldSchema = 'va_readdataappointmentresponse';
    this.responseTimestamp = 'va_webserviceresponse';
    this.ignoreHeader = true;
}
readDataAppointment.prototype = new pathwaysWebService;
readDataAppointment.prototype.constructor = readDataAppointment;
readDataAppointment.prototype.buildSoapBodyInnerXml = function () {
    this.wsMessage.stackTrace += 'buildSoapBodyInnerXml();';

    var nationalId = this.context.parameters['nationalId']; /*'1012646298V807838'*/;
    var startDate = Xrm.Page.getAttribute('va_appointmentfromdate').getValue()
		? new Date(Xrm.Page.getAttribute('va_appointmentfromdate').getValue()) : null;

    var endDate = Xrm.Page.getAttribute('va_appointmenttodate').getValue()
		? new Date(Xrm.Page.getAttribute('va_appointmenttodate').getValue()) : null;
    var startDateStr = null; var startDateObj;
    var endDateStr = null;

    if (startDate && startDate != undefined && startDate != NaN) { startDateStr = startDate.format('yyyy-MM-dd').toString(); }
    else {
        startDateObj = new Date();
        startDateObj.setDate(startDateObj.getMonth() - 6);
        startDateStr = startDateObj.format('yyyy-MM-dd').toString();
        //Xrm.Page.getAttribute('va_appointmentfromdate').setValue(new Date(startDateStr));
    }

    if (endDate && endDate != undefined && endDate != NaN) { endDateStr = endDate.format('yyyy-MM-dd').toString(); }
    else {
        endDateStr = new Date().format('yyyy-MM-dd').toString();
        //Xrm.Page.getAttribute('va_appointmenttodate').setValue(new Date(endDateStr));
    }
    //debugger
    var innerXml = '<pat:readData><in0>AppointmentsRead1</in0><in1><![CDATA[<?xml version="1.0" encoding="UTF-8"?>'
	+ '<filter:filter vhimVersion="Vhim_4_00" xmlns:filter="Filter"><filterId>APPOINTMENTS_SINGLE_PATIENT_FILTER</filterId>'
	+ '<clientName>VRM 1.0</clientName><clientRequestInitiationTime>2001-12-17T09:30:47Z</clientRequestInitiationTime>'
	+ '<patients><NationalId>' + nationalId + '</NationalId></patients><entryPointFilter queryName="Appointments-Standardized">'
	+ '<domainEntryPoint>Appointment</domainEntryPoint>'
	+ '<startDate>' + startDateStr + '</startDate><endDate>' + endDateStr + '</endDate>'
	+ '</entryPointFilter></filter:filter>]]></in1>'
	+ '<in2>APPOINTMENTS_SINGLE_PATIENT_FILTER</in2>'
	+ '<in3>Appointments_ReqId_06_23_0000</in3></pat:readData>';

    innerXml = innerXml.replace(this.CDATA1, this.CDATA1Rep).replace(this.CDATA2, this.CDATA2Rep);

    return innerXml;
}
// END readDataAppointment
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
// END Individual PathwaysWebService Methods
//=====================================================================================================
/*****************************************************************************************************/
/*****************************************************************************************************/
/*****************************************************************************************************/
// END PathwaysWebService
//=====================================================================================================