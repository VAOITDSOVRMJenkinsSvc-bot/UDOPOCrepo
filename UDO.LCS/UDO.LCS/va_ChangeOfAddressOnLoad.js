/// <reference path="../Intellisense/XrmPage-vsdoc.js" />

// This method will be call from CRM form
function OnLoad() {
// CSDev Left Intentionally Blank 
}
//Global Variables
var globalcadd = {};
globalcadd.bankdata = "";
globalcadd.cadddata = "";
globalcadd.idproof = "";
globalcadd.ca = false;
globalcadd.ca_a = "";
globalcadd.ca_r = "";
globalcadd.ca_t = "";
_changingAwardAddress = true;
_fromClaims = false;
_hasAppeals = false;
_participantId = '';

var awardAddresses_xmlObject = null;
var selectedAward_xmlObject = null;
var CountryList_xmlObject = null;
var vetFileNumber = null;
var vetSSN = null;
var veteranInformation_xml = null;
var veteranInformation_xmlObject = null;
var parent_page = null;

function onFormLoad() {
 // CSDev Left Intentionally Blank 
}

function DisableAll() {
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

function GetOptionValue(optionsetSchema, labelText) {
// CSDev Left Intentionally Blank 
}

function CopyAddress() {
 // CSDev Left Intentionally Blank 
}

function CopyAddress2() {
 // CSDev Left Intentionally Blank 
}

function va_mailingcountryChange() {
 // CSDev Left Intentionally Blank 
}

function va_routingnumberChange() {
  // CSDev Left Intentionally Blank 
}

function SetMandatoryFields() {
  // CSDev Left Intentionally Blank 
}

function ValidateMailingAddress() {
// CSDev Left Intentionally Blank 
}

function validateAddressUsingWS(addressType, addressParams, responseFieldName, scoreFieldName) {
// CSDev Left Intentionally Blank 
}

function ValidatePaymentAddress() {
// CSDev Left Intentionally Blank 
}

function ValidateFields() {
// CSDev Left Intentionally Blank 
}

function GetAddressArray(type) {
 // CSDev Left Intentionally Blank 
}

function TestForAllowedChars(text, nums) {
 // CSDev Left Intentionally Blank 
// CSDev Left Intentionally Blank 
}

function ValidatePaymentFields() {
 // CSDev Left Intentionally Blank 
}

function ProgressDlg(text) { this.text = text; }

ProgressDlg.prototype.ShowProgress = function (msg) {
// CSDev Left Intentionally Blank 
};

function RetrieveAwardAddresses(vetSearchCtx, opener) {
// CSDev Left Intentionally Blank 
}

// Not used in the OnLoad
function RetrieveAddressesFromParent(parentPage, opener) {
// CSDev Left Intentionally Blank 
}

function retrieveAppellantAddress() {
// CSDev Left Intentionally Blank 
}

function retrieveAppellantAddresses(vetSearchCtx) {
// CSDev Left Intentionally Blank 
}

function RetrievePersonalInfo(vetSearchCtx) {
// CSDev Left Intentionally Blank 
}

function FormatTelephone(telephoneNumber) {
// CSDev Left Intentionally Blank 
}

//On Change Phone
function fnphoneOnChange1() {
// CSDev Left Intentionally Blank 
}

function fnphoneOnChange2() {
// CSDev Left Intentionally Blank 
}

function convertDate(dateString) {
// CSDev Left Intentionally Blank 
}

function GetCountryList(vetSearchCtx) {
// CSDev Left Intentionally Blank 
}

function MilZipCodeChange() {
// CSDev Left Intentionally Blank 
}

function SetOptionSetFromValue(controlName, optionText) {
// CSDev Left Intentionally Blank 
}

function CPLink() {
// CSDev Left Intentionally Blank 
}

function EdLink() {
// CSDev Left Intentionally Blank 
}

function ViewScript() {
// CSDev Left Intentionally Blank 
}

function CADDIdProofingComplete() {
// CSDev Left Intentionally Blank 
}

function setHeaderData(page) {
 // CSDev Left Intentionally Blank 
}

function setHeaderFieldValue(fieldId, newText, textDecoration, underline, color, fontsize, cursor, tooltipText) {
// CSDev Left Intentionally Blank 
}

function getPhoneNumberFromCadd(parentPhoneNumber, phone1, phone2) {
// CSDev Left Intentionally Blank 
}

//ID Proofing: ToggleTabs (called from button or click of each checkbox)
function fnIDProof() {
// CSDev Left Intentionally Blank 
}

//set flag when any of the fields in the sections are changed for notes. Call this function on change from every field we are checking.

function setChangeCheckBox(field) {
// CSDev Left Intentionally Blank 
}

//If CADD intiated from Claim 
//    and Claim payee has any awards
//        Then disable rounting and account number fields

//VTRIGIILI 01-29-2015 - fixed typo in the spelling of opener that was
//         causing IE to throw errors.
function setRoutingAndAccountNumber(opener, Parent_Page) {
// CSDev Left Intentionally Blank 
}
