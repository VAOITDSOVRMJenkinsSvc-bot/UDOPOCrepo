//requires: Va_GetSystemUserProperties.js, Va_XmlUtilities.js, Va_CommonFunctions_VIP.js,
//          Va_EnvironmentalConfiguration.js, Va_CrmRestKit_2011.js, va_Context_Search.js
//          Va_PhoneAndContactSharedFunctions.js, va_PhoneCallGlobalObjects.js, Va_PhoneCalls_DispositionContorls.js 

//declare Namespaces
var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
Va.Udo.Crm.Scripts.Code = Va.Udo.Crm.Scripts.Code || {};

var _responseAttributes = null;
var _responseAttributesWithAggregation = null;  // allow multiple nodes of same type eg Contentions
var _male = 953850000;
var _female = 953850001;
var _unknown = 953850002;
var _XML_UTIL = null;
var _cachedData = null;
var _completed = false;
var _ValidateIDProofingForAddressChange = null;
var _VIPEndOfServiceCall = null;
var _VIPEndOfSearch = null;

//This sets up the basic scaffolding that the VIP app needs to run. 
//TODO: Add parameter to handle search on load. 
function onFormLoad() {
// CSDev Left Intentionally Blank 

}


Va.Udo.Crm.Scripts.Code.VT = {
    auto_Search: function () {
   // CSDev Left Intentionally Blank 

    },
    Ssn_Search: function (context) {
// CSDev Left Intentionally Blank 

    },

    //This function just collects the various attributes that make up the cache and
    //packages them up for the VIP Application
    LoadVIPCache: function () {
   // CSDev Left Intentionally Blank 

    },
    addButton: function (attributename, buttonText, onClickAction) {
   // CSDev Left Intentionally Blank 
    },
    onNewButtonClick: function () {
  // CSDev Left Intentionally Blank 
    },
    onButtonClick: function () {
   // CSDev Left Intentionally Blank 
    },

    //This function was lifted directly from the VA code
    //It simply puts all the Entity fields in to an array
    //to be paresed and added to the cache later
    defineResponseAttributes: function () {
        // CSDev Left Intentionally Blank 
    }
}



//******************************************************************
//**
//**  THE FOLLOWING FUNCTIONS ARE COPIED OUT OF va_PhoneCalleOnLoad_VIP.js
//**
//******************************************************************

function CopyAddressOnClick() {
 // CSDev Left Intentionally Blank 

}




//Validate ID Proofing checks when attempting to initiate a CADD
ValidateIDProofingForAddressChange = function () {
  // CSDev Left Intentionally Blank 
    }

// called at the end of each async ws
function VIPEndOfServiceCall(xmlFieldName, success, requestXml, url, response, wsName, wsDuration) {
  // CSDev Left Intentionally Blank 
}

function VIPEndOfSearch() {
  // CSDev Left Intentionally Blank 
}

