var _loading = true;
var _letterGenerationThroughCode = null;

var _isSystemAdmin = false;
CRM_FORM_TYPE_CREATE = 1;
CRM_FORM_TYPE_UPDATE = 2;
var sourceType = null;
var source = "";
var awardRequired = false;
var claimRequired = false;
var paymentRequired = false;
var ssrsReportName = '';
var reportId = '';
var formTabstoOpen = '';
var tmpLetterDisplay = '';

function OnLoad() {

// CSDev Left Intentionally Blank 
}

function onFormLoad() {
// CSDev Left Intentionally Blank 

}

function letters() {
// CSDev Left Intentionally Blank 
}

function ConfirmMarkAsSent() {
  // CSDev Left Intentionally Blank 
}

// action:
//    open: Open the generated letter as a report
//    upload: Upload the generated letter
//    download: Download the generated letter
function GenerateLetter(action, formatType) {
 // CSDev Left Intentionally Blank 
}

function ExecuteGenerateLetterAction(formatType, upload) {
 // CSDev Left Intentionally Blank 
}

//function GenerateLetterOnSave() {
//    Xrm.Page.getAttribute("udo_letterdisplay").setValue(tmpLetterDisplay);
//    window.open(serverUrl + source.replace("{0}", reportId) + "&p:LetterGenerationGUID=" + Xrm.Page.data.entity.getId());
//}


function PopulateEnclosures() {
      // CSDev Left Intentionally Blank 
}

function LetterChange() {

// CSDev Left Intentionally Blank 
}

// This method takes SecurityRole Name and context as parameters
function CheckUserRole(roleName, context) {
 // CSDev Left Intentionally Blank 
}

// Checks whether the security role exists in the system by using ODATA call
function FetchUserRoleIdWithName(roleName, context) {
 // CSDev Left Intentionally Blank 
}

function GenerateLetterCreated(letterName) {
// CSDev Left Intentionally Blank 
}

function retrieveUserReqCallBack(retrieveUserReq) {
   // CSDev Left Intentionally Blank 
}

function getReportID(reportName) {
   // CSDev Left Intentionally Blank 
}

var reports = {
 // CSDev Left Intentionally Blank 
};

function preFilterLookup() {
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

function GetLookupId(lookupAttributeName) {
 // CSDev Left Intentionally Blank 
}

function QuickWriteChange() {
 // CSDev Left Intentionally Blank 
}


// process substition tokens
// each one looks like <!va_ssn!> or <!udo_ssn!>
// will take input abe<!va_ssn!>asdf and replace with abe555667777asdf
function ReplaceFieldTokens(qw) {
   // CSDev Left Intentionally Blank 
}

function getManager() {
// CSDev Left Intentionally Blank 
}

function getVBMSRole() {
// CSDev Left Intentionally Blank 
}
