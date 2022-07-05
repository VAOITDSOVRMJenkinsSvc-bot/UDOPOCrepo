//=====================================================================================================
// START PathwaysWebService
//=====================================================================================================
var pathwaysWebService = function (context) {
// CSDev Left Intentionally Blank 

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
// CSDev Left Intentionally Blank 
}
isAlive.prototype = new pathwaysWebService;
isAlive.prototype.constructor = isAlive;
isAlive.prototype.buildSoapBodyInnerXml = function () {
// CSDev Left Intentionally Blank 
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
// CSDev Left Intentionally Blank 
}
readDataExamRequest.prototype = new pathwaysWebService;
readDataExamRequest.prototype.constructor = readDataExamRequest;
readDataExamRequest.prototype.buildSoapBodyInnerXml = function () {
 // CSDev Left Intentionally Blank 
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
   // CSDev Left Intentionally Blank 
}
readDataAppointment.prototype = new pathwaysWebService;
readDataAppointment.prototype.constructor = readDataAppointment;
readDataAppointment.prototype.buildSoapBodyInnerXml = function () {
 // CSDev Left Intentionally Blank 
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