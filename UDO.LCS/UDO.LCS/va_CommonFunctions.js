var commonFunctions = {};

// Contstants
CRM_FORM_TYPE_CREATE = 1;
CRM_FORM_TYPE_UPDATE = 2;
CRM_FORM_TYPE_COMPLETED_ACTIVITY = 4;
WEBSERVICE_RESPONSE = '';

_usingIFD = false;
_KMRoot = null;
_IFRAME_SOURCE_MULTIPLE = null;
_IFRAME_SOURCE_SINGLE = null;
_XML_UTIL = null;
_context = null;

// Global variables
// For backward compatability ********************
WebServiceURLRoot = null;
_vrmVersion = null;
_envName = null;
_MVIServiceURLRoot = null;
_MVIDAC = null;
_PathwaysServiceURLRoot = null;
_PathwaysDAC = null;
_VacolsServiceURLRoot = null;
_VacolsDAC = null;
_globalDACUrl = null;
_isPROD = null;
//***********************************************

_progressWindow = null;
_noticeDlg = null;
_allowToCashXMLResponses = true;    // if true will save xml responses in phone call, contact records. if false, will flush them on save
_scriptWindowHandle = null;
_logErrors = true;
_logWarnings = false;
_logQueries = true;
_per = false;
_suppressWSNotice = false;
_progressWindowUrl = '/WebResources/va_progress.htm';
_totalProgress = 0;
_progressInterval = 1;
_totalWebServiceExecutionTime = 0;
_WebServiceResponse = null;
_UserSettings = null;

commonFunctions.initalize = function () {
 // CSDev Left Intentionally Blank 
};
// End of initalization

function getContext() {
    // CSDev Left Intentionally Blank return _context;
}
//=============================================
//  getVrmMessage() 
//=============================================
_VRMMESSAGE = new Array();

function getCurrentEnvironment() {
    // CSDev Left Intentionally Blank return _currentEnv.name;
}

function getVrmVersion() {
    // CSDev Left Intentionally Blank return _vrmVersion;
}

function getVrmMessage() {
   // CSDev Left Intentionally Blank  return _VRMMESSAGE;
}

function prepareProgress() {
  // CSDev Left Intentionally Blank 
}
function positionProgress() {
// CSDev Left Intentionally Blank 
}

function ShowProgressHTML(text) {
// CSDev Left Intentionally Blank 
}
function ShowProgress(text) {
  // CSDev Left Intentionally Blank 
}
_ShowProgress = ShowProgress;

function CloseProgress() {
   // CSDev Left Intentionally Blank 
}

function CloseProgressHTML() {
  // CSDev Left Intentionally Blank 
}

function Minimize() {
// CSDev Left Intentionally Blank 
}

function UpdateClaimScriptText(claim, claimScriptWindowHandle) {
// CSDev Left Intentionally Blank 
}

function UserHasRole(roleName) {

// CSDev Left Intentionally Blank 
}
function GuidsAreEqual(guid1, guid2) {
   // CSDev Left Intentionally Blank 
}
function GetRequestObject() {
   // CSDev Left Intentionally Blank 
}

// OData: make sure that you include jquery1.4.1.min.js and json2.js as libraries first since this script relies on them
function SetDefaultCurrency() {
// CSDev Left Intentionally Blank 
}

function FormatPhone(oField) {
 // CSDev Left Intentionally Blank 
}

function formatNumber(context) {
// CSDev Left Intentionally Blank 

}

//Operation: Entity Clone
function Clone() {
 // CSDev Left Intentionally Blank 
}

// Operation: Inline toolbar and button
// RU12 Updated InlineToolbar button method
function InlineToolbar(containerId) {
 // CSDev Left Intentionally Blank 
}

//************************************************************************



Encoder = {
// CSDev Left Intentionally Blank 

}

FormatExtjsDate = function (str_date) {
// CSDev Left Intentionally Blank 
}

parseXmlObject = function (xmlString) {
// CSDev Left Intentionally Blank 
}

function getLocalTime(timeZone) {
// CSDev Left Intentionally Blank 
}

/// base64 encode/decode
/**
*
*  Base64 encode / decode
*  http://www.webtoolkit.info/
*
**/

var Base64 = {

// CSDev Left Intentionally Blank 

}

//***********************************************************************
// Hides buttons from  IFRAME view
function HideIframeButtons(Iframe, buttonTitles) {
// CSDev Left Intentionally Blank 
}

function getElementsByClassName(className, anchorNode) {
// CSDev Left Intentionally Blank 
}
//************************************************************************

function formatCurrency(num) {
// CSDev Left Intentionally Blank 
}

function formatXml(xml) {
// CSDev Left Intentionally Blank 
}


function SingleNodeExists(xmlObj, node_path) {
 // CSDev Left Intentionally Blank 
}

function MultipleNodesExist(xmlObj, node_path) {
// CSDev Left Intentionally Blank 
}


if (typeof (SDK) == "undefined")
{ SDK = { __namespace: true }; }
// Namespace container for functions in this library.
SDK.MetaData = {
 // CSDev Left Intentionally Blank 
};

function successRetrieveAllEntities(entityMetadataCollection) {
// CSDev Left Intentionally Blank 

}
function errorRetrieveAllEntities(error) {
// CSDev Left Intentionally Blank 
}

function retrieveAttributes() {
// CSDev Left Intentionally Blank 
}

function successRetrieveEntity(logicalName, entityMetadata) {
// CSDev Left Intentionally Blank 

}
function errorRetrieveEntity(error) {
// CSDev Left Intentionally Blank 
}

//function that splits and chages format of "Last Name, First Name" to "First Name Last Name"
//var FullName = Xrm.Page.getAttribute('createdby').getValue()[0].name;
function fullnameFormat(FullName) {
// CSDev Left Intentionally Blank 
}