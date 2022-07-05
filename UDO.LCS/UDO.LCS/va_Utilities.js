//Common Utilities should be first library loaded
var UTIL = {};

//Function to clean strings (Keeps Alpha, Numeric, Spaces)
//Use: UTIL.trimChars('va_address1', 'AlphaNum', 'Invalid Characters entered in the address.');
UTIL.trimChars = function(fieldName, pattern, message) {
// CSDev Left Intentionally Blank 
};

//Register Checkbox FireOnClick function
UTIL.registerCheckboxClick = function(attr) {
  // CSDev Left Intentionally Blank 
};

//format Telephone Numbers 
//(need to remove other phone functions FormatPhone/formatTelephone, from files such as CommonFunctions, CommonFunctions_VIP, PhoneCall_OnLoad_VIP)
UTIL.formatTelephone = function(telephoneField) {
  // CSDev Left Intentionally Blank 
};

UTIL.isInteger = function(s) {
 // CSDev Left Intentionally Blank 
};

UTIL.stripCharsInBag = function(s, bag) {
// CSDev Left Intentionally Blank 
};

UTIL.daysInFebruary = function(year) {
// CSDev Left Intentionally Blank 
};

UTIL.DaysArray = function(n) {
  // CSDev Left Intentionally Blank 
};

UTIL.isProperDate = function(fieldName) {
 // CSDev Left Intentionally Blank 
};

// process substition tokens
// each one looks like <!va_ssn!>
// will take input abe<!va_ssn!>asdf and replace with abe555667777asdf
UTIL.ReplaceFieldTokens = function(qw) {
  // CSDev Left Intentionally Blank 
};

UTIL.GetLookupId = function(lookupAttributeName) {
  // CSDev Left Intentionally Blank 
};

// Utility function to create or update the map-D notes
UTIL.mapdNote = function (opts) {
// CSDev Left Intentionally Blank 
};

// Function accepts a data an the format pattern
// pattern examples: 'MM-dd-yyyy h:mm:ss' gives you a date formated like so: 12-24-2013 12:30:34
UTIL.dateFormat = function (date, format) {
  // CSDev Left Intentionally Blank 
};

//Non-Sequential Dynamic Picklist
//Sample Array to pass:
//First value is Main Picklist Option, followed by corresponding SubValues
/*var typePicklist = new Array();
//typePicklist[0] = new Array(953850000, 953850000, 953850001, 953850002, 953850003, 953850004, 953850005);
//typePicklist[1] = new Array(953850001, 953850006, 953850005);
*/
UTIL.createDynamicPicklist = function(varPicklist, varSubPicklist, varRelationArray) {
  // CSDev Left Intentionally Blank 
};

UTIL.openOutlookEmail = function (opts) {
 // CSDev Left Intentionally Blank 
};

//function that splits and chages format of "Last Name, First Name" to "First Name Last Name"
//var FullName = Xrm.Page.getAttribute('createdby').getValue()[0].name;
UTIL.fullnameFormat = function (FullName){
// CSDev Left Intentionally Blank 
};

//Used to Create ServiceReqeest and VAI from VIP (for phone call and contact)
UTIL.CreateEntity = function (entityName, cols) {
// CSDev Left Intentionally Blank 
};
UTIL.restKitError = function (err, message) {
  // CSDev Left Intentionally Blank 
};