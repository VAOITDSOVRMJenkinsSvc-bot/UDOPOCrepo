/**
 * Created by VHAISDBLANCW on 2/20/2015.
 */
/// <reference path="../Intellisense/XrmPage-vsdoc.js" />
/// <reference path="PhoneCall_Onload.js" />
/// <reference path="ContactPCRForm.js" />

_ViewContact = null;
_IntentToFile = null;
_claimStatus = null;
_paymentHistoryStatus = null;
_responseAttributes = null;
_responseAttributesWithAggregation = null;  // allow multiple nodes of same type eg Contentions
_male = 953850000;
_female = 953850001;
_unknown = 953850002;
noteId = null;
_birlsResult = false;
_searchCounter = 0;

/***************Phone Call***************/
var ClaimantInfo = {};
ClaimantInfo.AddressLine1 = "";
ClaimantInfo.AddressLine2 = "";
ClaimantInfo.AddressLine3 = "";
ClaimantInfo.Email = "";
ClaimantInfo.City = "";
ClaimantInfo.State = "";
ClaimantInfo.Zip = "";
ClaimantInfo.Country = "";
ClaimantInfo.FirstName = "";
ClaimantInfo.LastName = "";
ClaimantInfo.MiddleInitial = "";
ClaimantInfo.ParticipantId = "";
ClaimantInfo.Ssn = "";
ClaimantInfo.MilitaryPostalTypeCode = "";
ClaimantInfo.MilitaryPostOfficeTypeCode = "";

var VeteranInfo = {};
VeteranInfo.Ssn = "";
VeteranInfo.FileNumber = "";

function IntentToFile(selection) {
// CSDev Left Intentionally Blank 
}
_IntentToFile = IntentToFile;

function getClaimantInfo() {
// CSDev Left Intentionally Blank 
}

//=============================================
//  ViewContact()
//=============================================
function ViewContact(selection) {
// CSDev Left Intentionally Blank 
}
_ViewContact = ViewContact;
//=============================================
//  HardcodeXmlResponse()
//=============================================
function HardcodeXmlResponse() { // skipMinSearchFields
 // CSDev Left Intentionally Blank 

}
function HardcodeResponse(xmlString, field) {
// CSDev Left Intentionally Blank 
}
//=============================================
//  LookupId()
//=============================================
function LookupId(attr) {
// CSDev Left Intentionally Blank 
}
//=============================================
//  performCrmAction()
//=============================================
function performCrmAction(actions) {
// CSDev Left Intentionally Blank 
}
//=============================================
//  MarkAsRelatedVeteran()
//=============================================
function MarkAsRelatedVeteran(rtContact) {
 // CSDev Left Intentionally Blank 
    //CloseProgress();
}
//=============================================
//  ShowFlagsAndTooltips()
//=============================================
function ShowFlagsAndTooltips(rtContact) {
 // CSDev Left Intentionally Blank 
}
//=============================================
//  SetHeaderField()
// SetHeaderField('va_flags', 'bla', 'underline', 'solid 1px blue', 'blue', 'x-small', 'pointer');
//=============================================
function SetHeaderField(fieldId, newText, textDecoration, underline, color, fontsize, cursor, tooltipText, isFooter) {
// CSDev Left Intentionally Blank 
}
//=============================================
//  updateSearchResultsSection()
//=============================================
//function updateSearchResultsSection() {
//    _IFRAME_SOURCE_MULTIPLE = 'ISV/ext-4.0.1/VA' + _vrmVersion + '/VA-phone/phone-multiple.html';
//    _IFRAME_SOURCE_SINGLE = 'ISV/ext-4.0.1/VA' + _vrmVersion + '/VA-contact/contact.html';

//    var orgname = Xrm.Page.context.getOrgUniqueName();
//    var scriptRoot = Xrm.Page.context.getServerUrl().replace(orgname, '');
//    var sourceURL = '';
//    var searchIndividual;
//    // TODO: enhance logic in the line below by checking SSN from both ends when both birls and corp return single record
//    if (_CORP_RECORD_COUNT > 1 || _BIRLS_RECORD_COUNT > 1) {
//        sourceURL = '/' + _IFRAME_SOURCE_MULTIPLE;
//        searchIndividual = '';
//        if (Xrm.Page.getAttribute('va_firstname').getValue() && Xrm.Page.getAttribute('va_lastname').getValue()) {
//            searchIndividual = Xrm.Page.getAttribute('va_firstname').getValue() + ' '
//			+ Xrm.Page.getAttribute('va_lastname').getValue();
//        }
//        else {
//            if (_birlsResult) {
//                searchIndividual = GetBirlsSectionName();
//            }
//        }
//        searchIndividual += '. Corp: ' + _CORP_RECORD_COUNT + '; BIRLS: ' + _BIRLS_RECORD_COUNT;
//    }
//    else {
//        sourceURL = '/' + _IFRAME_SOURCE_SINGLE;
//        if (Xrm.Page.data.entity.getEntityName() == 'contact') {
//            if (Xrm.Page.getAttribute('firstname').getValue() && Xrm.Page.getAttribute('lastname').getValue()) {
//                searchIndividual = Xrm.Page.getAttribute('firstname').getValue() + ' '
//					+ Xrm.Page.getAttribute('lastname').getValue();
//            }
//            else {
//                if (_birlsResult) {
//                    searchIndividual = GetBirlsSectionName();
//                }
//            }
//        } else {
//            if (Xrm.Page.getAttribute('va_firstname').getValue() && Xrm.Page.getAttribute('va_lastname').getValue()) {
//                searchIndividual = Xrm.Page.getAttribute('va_firstname').getValue() + ' '
//					+ Xrm.Page.getAttribute('va_lastname').getValue();
//            }
//            else {
//                if (_birlsResult) {
//                    searchIndividual = GetBirlsSectionName();
//                }
//            }
//        }
//        var ssn = Xrm.Page.getAttribute('va_ssn').getValue();
//        if (ssn) { searchIndividual += ' (' + ssn + ')'; }
//    }
//    var totalExecutionTime = _totalWebServiceExecutionTime;
//    var executionSeconds = (totalExecutionTime / 1000);
//    var searchLabelText = 'Search Results for ' + searchIndividual
//		+ '; Execution time:  ' + executionSeconds + ' seconds';
//    if (Xrm.Page.data.entity.getEntityName() == 'contact') {            //contact screen
//        Xrm.Page.getControl('IFRAME_ro').setSrc(_iframesrc);
//        Xrm.Page.ui.tabs.get('tab_search').sections.get('search_results').setVisible(true);
//    } else {
//        Xrm.Page.ui.tabs.get('tab_search').sections.get('Categorize Call_section_idproofing').setVisible(true);
//        Xrm.Page.ui.tabs.get('tab_search').sections.get('phonecall_section_idprotocol').setVisible(true);
//        Xrm.Page.ui.tabs.get('tab_search').sections.get('callerdetails').setVisible(true);
//        Xrm.Page.ui.tabs.get('tab_search').sections.get('phone searchresults').setVisible(true);
//        Xrm.Page.ui.tabs.get('tab_search').sections.get('phone searchresults').setLabel(searchLabelText);
//        Xrm.Page.getControl('IFRAME_search').setSrc(sourceURL);
//        //if (1 == 0 && _FrameLoader) {_FrameLoader(); }
//    }
//}
//=============================================
//  GetWarningMessages()
//=============================================
function GetWarningMessages(separator, verbose) {
 // CSDev Left Intentionally Blank 
}
function FMM(msg) {
// CSDev Left Intentionally Blank 
}
//=============================================
//  GetErrorMessages()
//=============================================
function GetErrorMessages(separator, verbose) {
  // CSDev Left Intentionally Blank 
}
function IsSensitiveFileAccessFail() {
   // CSDev Left Intentionally Blank 
}
//=============================================
//  LogMessages()
//=============================================
function LogMessages(error, warning, summaryOnly) {
// CSDev Left Intentionally Blank 
}
//=============================================
//  CreateQueryLogEntry()
//=============================================
function CreateQueryLogEntry(cols) {
// CSDev Left Intentionally Blank 
}
//=============================================
//  GetQueryXML()
//=============================================
function GetQueryXML() {
// CSDev Left Intentionally Blank 
}
//=============================================
//  CreateClaimServiceRequest()
//=============================================
function CreateClaimServiceRequest(selection, srType, alreadyPrompted, defaultActionText, defaultActionId, callIssue, doOpen, doAlert) {
 // CSDev Left Intentionally Blank 
}
_CreateClaimServiceRequest = CreateClaimServiceRequest;
function FormatPhone(areaCode, phoneNumber) {
// CSDev Left Intentionally Blank 
}

function FormatPathwaysDate(dateStr) {
// CSDev Left Intentionally Blank 
}
function FormatAwardDate(date) {
// CSDev Left Intentionally Blank 
}
function GetNumber(string) {
// CSDev Left Intentionally Blank 
}

//=============================================
//  GetSOJ() - Returns the current vet's SOJ guid and name from CRM (award selection necessary if multiple awards).
//=============================================
function GetSOJ(sojVal) {
// CSDev Left Intentionally Blank 
}

//=============================================
//  ChangeOfAddressOnClick()
//=============================================
function ChangeOfAddressOnClick(selection) {
// CSDev Left Intentionally Blank 
}


_ChangeOfAddressOnClick = ChangeOfAddressOnClick;
function TranslateSearchType(continueSearch) {
 // CSDev Left Intentionally Blank 
}

function PathwaysSearchOnChange(tabName, sectionName, showSection) {
// CSDev Left Intentionally Blank 
}
function AppealsSearchOnChange(tabName, sectionName, showSection) {
// CSDev Left Intentionally Blank 
}

function IsSingleAward() {
// CSDev Left Intentionally Blank 
}
function executePostSearchOperations(searchForBIRLS) {
 // CSDev Left Intentionally Blank 
}

function defineResponseAttributes() {
// CSDev Left Intentionally Blank 
}
function QueryDevNotes(columns, filter) {
// CSDev Left Intentionally Blank 
}
_QueryDevNotes = QueryDevNotes;
function CreateDevNoteLogEntry(createNoteDetailresponseXml, devNoteText) {
// CSDev Left Intentionally Blank 
}
_CreateDevNoteLogEntry = CreateDevNoteLogEntry;
// TODO: complete
function ValidatePermissionToEditNote(noteId) {
}
_ValidatePermissionToEditNote = ValidatePermissionToEditNote;
function GetTransactionCurrencyInfo() {
// CSDev Left Intentionally Blank 
}
function IsFileNumber(number) {
// CSDev Left Intentionally Blank 
}
function GetBirlsSectionName() {
// CSDev Left Intentionally Blank 
}
function GetServerUrl() {
// CSDev Left Intentionally Blank 
}
function UpdateSearchOptionsObject(ctx) {
// CSDev Left Intentionally Blank 
}

function UpdateSearchListObject(searchObj) {
   // CSDev Left Intentionally Blank  SearchList.add(searchObj, true);
}

function JSONStore() {
    // CSDev Left Intentionally Blank this.rawData;
};

JSONStore.prototype = {
// CSDev Left Intentionally Blank 
};

// RU12 the class is built wrong

var CRMJSONStore = function (formAttributeName) {
// CSDev Left Intentionally Blank 
};
CRMJSONStore.prototype = new JSONStore;
CRMJSONStore.prototype.constructor = CRMJSONStore;

CRMJSONStore.prototype.read = function () {
// CSDev Left Intentionally Blank 
};

CRMJSONStore.prototype.overWrite = function (index, data, append) {
 // CSDev Left Intentionally Blank 
};

CRMJSONStore.prototype.write = function (data, append) {
  // CSDev Left Intentionally Blank 
};

CRMJSONStore.prototype.clear = function () {
// CSDev Left Intentionally Blank 
};

var WebServiceExecutionStatusList =
    (function () {
// CSDev Left Intentionally Blank 
    }());

var SearchOptionsList =
    (function () {
// CSDev Left Intentionally Blank 
    }());

var SearchList =
    (function () {
// CSDev Left Intentionally Blank 
    }());

var SearchIn =
    (function () {
// CSDev Left Intentionally Blank 
    }());

function SearchActionsComplete(searchObj) {
// CSDev Left Intentionally Blank 
}

function GetSearchCounter() {
// CSDev Left Intentionally Blank 
}


function getPhoneNumberFromContact(rtContact) {
// CSDev Left Intentionally Blank 
}

function parseContactPhoneNumber(area, number) {
// CSDev Left Intentionally Blank 
}

function formatContactPhoneNumber(phoneNumber, isDomestic) {
// CSDev Left Intentionally Blank 
}

