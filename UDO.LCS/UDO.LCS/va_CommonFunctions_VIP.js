var commonFunctionsVip = {};


/**
* Global functions and constants for VIP version of CRMUD
*/
// Constants
CRM_FORM_TYPE_CREATE = 1;
CRM_FORM_TYPE_UPDATE = 2;
CRM_FORM_TYPE_COMPLETED_ACTIVITY = 4;
WEBSERVICE_RESPONSE = '';

_vipEntryPoint = null;
_vipSearchContext = null;
_VIPEndOfSearch = null;
_VIPEndOfServiceCall = null;
_GetEnvironment = null;
_cachedData = null; // global set of Environment, UserSetting, etc passed to extjs
_serviceResultsCollection = null; // collection of ws xml responses keyed to xml field name
_endOfSearchReached = false;
_usingIFD = false;
_KMRoot = null;
_IFRAME_SOURCE_SINGLE = null;
_IFRAME_SOURCE_MULTIPLE = null;
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
//*******************************************************************************
/* Inizalization function
 * must be called from the onload event of the form
 */
commonFunctionsVip.initalize = function () {
// CSDev Left Intentionally Blank 
};
// end of inializations

//=============================================
//  getContext()
//=============================================

function getContext() {
    // CSDev Left Intentionally Blank return _context;
}
//=============================================
//  getVrmMessage() 
//=============================================
_VRMMESSAGE = new Array();
function GetEnvironment() {
    // CSDev Left Intentionally Blank return _currentEnv;
}
_GetEnvironment = GetEnvironment;

function getCurrentEnvironment() {
    // CSDev Left Intentionally Blank return _currentEnv.name;
}

function getVrmVersion() {
   // CSDev Left Intentionally Blank  return _vrmVersion;
}

function getVrmMessage() {
    // CSDev Left Intentionally Blank return _VRMMESSAGE;
}

function prepareProgress() {
// CSDev Left Intentionally Blank 
}
function positionProgress() {
// CSDev Left Intentionally Blank 
}

function ShowProgress(text) {
 // CSDev Left Intentionally Blank 
}
function ShowProgressIEModeless(text) {
  // CSDev Left Intentionally Blank 
}
_ShowProgress = ShowProgress;

function CloseProgressIEModeless() {
  // CSDev Left Intentionally Blank 
}

function CloseProgress() {
   // CSDev Left Intentionally Blank 
}

function Minimize() {
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

function FormatPhone(oField) {
  // CSDev Left Intentionally Blank 
}

// RU12 Updated InlineToolbar button method
function InlineToolbar(containerId) {
 // CSDev Left Intentionally Blank 
}
//************************************************************************

FormatExtjsDate = function (str_date) {
// CSDev Left Intentionally Blank 
};
parseXmlObject = function (xmlString) {
// CSDev Left Intentionally Blank 
};

function getLocalTime(timeZone) {
// CSDev Left Intentionally Blank 
}

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



//formating phone numbers
function FormatTelephone(telephoneNumber) {
   // CSDev Left Intentionally Blank 
}
