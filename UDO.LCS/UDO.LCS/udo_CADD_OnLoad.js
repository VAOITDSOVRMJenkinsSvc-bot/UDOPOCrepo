/// <reference path="XrmPageTemplate.js" />
/// <reference path="../../../../../../../../../../analysis/ud_150506_final/va.vrmud/crm.webresources/javascript/va_commonfunctions.js" />

var Countries = new function () {
    //CSDev: Intentionally Left BLank
};

function OnLoad() {
    //CSDev: Intentionally Left BLank
}

//Global Variables
var globalcadd = {
    bankdata: "",
    cadddata: "",
    idproof: "",
    ca: false,
    ca_a: "",
    ca_r: "",
    ca_t: ""
};

var _changingAwardAddress = true;
var _fromClaims = false;
var _hasAppeals = false;
var _participantId = '';

var _addressTypes = { Domestic: 953850000, International: 953850001, Overseas: 953850002 };

var awardAddresses_xmlObject = null;
var selectedAward_xmlObject = null;
var vetFileNumber = null;
var vetSSN = null;
var veteranInformation_xml = null;
var veteranInformation_xmlObject = null;
var parent_page = null;


function onFormLoad() {
 // CSDev Left Intentionally Blank 

}

function CADDIdProofingComplete() {
    // CSDev Left Intentionally Blank 

}

function convertDate(dateString) {
  // CSDev Left Intentionally Blank 
}

function MilZipCodeChange() {
  // CSDev Left Intentionally Blank 
}

function DisableAll() {
  // CSDev Left Intentionally Blank
}

function CopyAddress() {
 // CSDev Left Intentionally Blank 
}

function ValidateFields() {
 // CSDev Left Intentionally Blank 
}

function ValidatePaymentFields() {
 // CSDev Left Intentionally Blank 
}

function va_routingnumberChange() {

// CSDev Left Intentionally Blank 
}

function CopyAddress2() {
// CSDev Left Intentionally Blank 
}

function va_mailingcountryChange() {
// CSDev Left Intentionally Blank 
}

function SetPaymentMandatoryFields(addressType) {
// CSDev Left Intentionally Blank 
}

function SetMailingMandatoryFields(addressType) {
// CSDev Left Intentionally Blank 
}

function SetMandatoryFields() {
// CSDev Left Intentionally Blank 
}

function GetAddressArray(type) {
  // CSDev Left Intentionally Blank 
}

function TestForAllowedChars(text, nums) {
// CSDev Left Intentionally Blank 
}

function MailingState_Onchange() {
// CSDev Left Intentionally Blank 
}

function AppellantState_Onchange() {
// CSDev Left Intentionally Blank 
}

function PaymentState_Onchange() {
// CSDev Left Intentionally Blank 
}

function SetAddlOptionSetFields() {
// CSDev Left Intentionally Blank 
}

function GetOptionValue(optionsetSchema, labelText) {
// CSDev Left Intentionally Blank 
}

function SetOptionSetFromValue(controlName, optionText) {
// CSDev Left Intentionally Blank 
}

function setChangeCheckBox(field) {
// CSDev Left Intentionally Blank 
}

//On Change Phone
function fnphoneOnChange1() {
// CSDev Left Intentionally Blank 
}

function fnphoneOnChange2() {
// CSDev Left Intentionally Blank 
}

function FormatTelephone(telephoneNumber) {
// CSDev Left Intentionally Blank 
}

//ID Proofing: ToggleTabs (called from button or click of each checkbox)
function fnIDProof() {
 // CSDev Left Intentionally Blank 
}



// -----------------------------------------------------------------------------------------------------
// SS - Should move to common functions later
// -----------------------------------------------------------------------------------------------------

///This function simplifies setting the option set value.
function SetOptionSetValue(optionsetAttribute, optionText) {
// CSDev Left Intentionally Blank 
}

function changeForm(name) {
// CSDev Left Intentionally Blank 
}
