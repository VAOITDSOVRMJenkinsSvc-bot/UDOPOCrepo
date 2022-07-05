/// <reference path="../Intellisense/XrmPage-vsdoc.js" />
// This method will be call from CRM form
function OnLoad() {
// CSDev Left Intentionally Blank 
}
dateClosed = null;
reqStatus = null;
reqStatusVal = null;
_loading = true;
_parentPage = null;
_sendMailButtonClicked = false;
_sendEmailsThroughCode = null;
_runningEmailGenAfterAutoSave = null;
_originalRO = null;
_isECC = false;

// Begin Form onload event - retrieve data from VIP if necessary

function onFormLoad() {
// CSDev Left Intentionally Blank 
}

function serviceRequest() {
// CSDev Left Intentionally Blank 
}
/*******************************END ONLOAD EVENT*****************************************/

//Retrieve data from VIP
//function RetrieveServiceRequestData() {
//    var searchIframe = null,
//        parentWindow;

//    if (window.parent && window.parent.opener && window.parent.opener.Xrm.Page.data) {
//        parentWindow = window.parent.opener;
//    }
//    else {
//        parentWindow = window.top.opener.parent;
//    }

//    if (parentWindow.Xrm.Page.data.entity.getEntityName() == "phonecall") {
//        searchIframe = parentWindow.document.getElementById('IFRAME_search').contentWindow;
//    }
//    else if (parentWindow.Xrm.Page.data.entity.getEntityName() == "contact") {
//        searchIframe = parentWindow.document.getElementById('IFRAME_ro').contentWindow;
//    }
//    if (searchIframe) {
//        searchIframe._appFireEvent('createcrmservicerequest', this);
//    }
//    else return null;
//}
function EmailtoVet() {
// CSDev Left Intentionally Blank 
}

//This is the callback function that VIP calls to return the data
function ReturnServiceRequestData(data) {
 // CSDev Left Intentionally Blank 
}

function ShowHideOLB() {
// CSDev Left Intentionally Blank 
}

function StatusChange() {
   // CSDev Left Intentionally Blank 
}

function doesControlHaveAttribute(control) {
// CSDev Left Intentionally Blank 
}

function disableFormFields(onOff) {
// CSDev Left Intentionally Blank 
}
//Multiple Payment functions
//MP Main function
function fnMP() {
// CSDev Left Intentionally Blank 
}

//MP checkbox function
function fnMPCheckAll() {
// CSDev Left Intentionally Blank 
}
//MP default dates if nothing is entered(6 months ago - today)
function fnMPdate() {
// CSDev Left Intentionally Blank 
}
//MP need to sort through the data now.
function fnMPsort() {
 // CSDev Left Intentionally Blank 
}
function mpFormatDate(varDate) {
// CSDev Left Intentionally Blank 
}

function PopulateEnclosures() {
 // CSDev Left Intentionally Blank 
}


function SendEmailToRO() {
// CSDev Left Intentionally Blank 
}


function CreateOutlookEmail2() {
 // CSDev Left Intentionally Blank 
}

function getSOJAddress(data, letteraddressing) {
 // CSDev Left Intentionally Blank 
}

//To Do: Change to getValue() and use number instead of TEXT.
// change visibility of sections on the form based on type
function ServiceTypeChange() {
 // CSDev Left Intentionally Blank 
}

function RelatedVetChange() {
// CSDev Left Intentionally Blank 
}

//=============================================
// START FUNCTION performCrmAction()
//=============================================
function performCrmAction(actions) {
// CSDev Left Intentionally Blank 
}
// END FUNCTION performCrmAction()
//=============================================

function QuickWriteChange() {
 // CSDev Left Intentionally Blank 
}

function SOJChange() {
 // CSDev Left Intentionally Blank 
}

function va_AllTrackedItemsReceivedOrClosedChange() {
  // CSDev Left Intentionally Blank 
}

function FormatVeteranPhone(areaCode, phoneNumber) {
 // CSDev Left Intentionally Blank 
}

function ProcessedInShareChange() {
 // CSDev Left Intentionally Blank 
}

function FnodOtherChange() {
   // CSDev Left Intentionally Blank 
}

function FormatPathwaysDate(dateStr) {
 // CSDev Left Intentionally Blank 
}

function FormatAwardDate(date) {
 // CSDev Left Intentionally Blank 
}

function RestrictPCRAccess(id, callback) {
    // CSDev Left Intentionally Blank 
}

function ShowScript() {
 // CSDev Left Intentionally Blank 
}

function fnFNODtype() {
 // CSDev Left Intentionally Blank 
}

function setRPOText() {
  // CSDev Left Intentionally Blank 
}

function getUniqueRequestNumber() {
 // CSDev Left Intentionally Blank 
}

function setSRFields() {
// CSDev Left Intentionally Blank 
}

//set Vet on SR when SR is created from SR section of phone call
function setVeteranName() {
  // CSDev Left Intentionally Blank 
}

function getManager() {
  // CSDev Left Intentionally Blank 
}

function copyMailingAddress() {
  // CSDev Left Intentionally Blank 
}
