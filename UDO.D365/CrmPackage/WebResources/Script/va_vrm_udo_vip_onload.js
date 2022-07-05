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
var _WebServiceExecutionStatusLists = "Empty String";
var _missingMapping = "";
var _exCon = null;
var _formContext = null;

//This sets up the basic scaffolding that the VIP app needs to run. 
//TODO: Add parameter to handle search on load. 
function onFormLoad(execCon) {
    _exCon = execCon;
    _formContext = _exCon.getFormContext();
    commonFunctionsVip.initalize(_exCon); //VA code - sets up some needed golbals;
    _completed = (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_COMPLETED_ACTIVITY); // used to warn user about actions on closed/open records by VA code

    //set up the golbal varibles that VA code depends on
    var url = Xrm.Page.context.getServerUrl() + '/ISV/VIP/index.html';
    var orgName = Xrm.Page.context.getOrgUniqueName();
    url = url.replace(orgName + '/', '');
    _ValidateIDProofingForAddressChange = ValidateIDProofingForAddressChange; //makes this visible to VA Code
    _VIPEndOfServiceCall = VIPEndOfServiceCall; //makes this visible to VA Code
    _VIPEndOfSearch = VIPEndOfSearch;//makes this visible to VA Code



    // We do not call VIP, it calls itself. We just need to make sure "_cacheData" is ready before it is loaded. 
    Va.Udo.Crm.Scripts.Code.VT.LoadVIPCache();
    Xrm.Page.getControl('IFRAME_search').setSrc(url);

    //Set up the buttons on the page 
    Xrm.Page.getAttribute("va_ssn").addOnChange(Va.Udo.Crm.Scripts.Code.VT.Ssn_Search);
    Va.Udo.Crm.Scripts.Code.VT.addButton("va_searchtype", "Search", Va.Udo.Crm.Scripts.Code.VT.onButtonClick);
    Va.Udo.Crm.Scripts.Code.VT.addButton("va_callerfirstname", "Auto Fill", CopyAddressOnClick);
    Va.Udo.Crm.Scripts.Code.VT.addButton("va_autoload_vip", "Search and New", Va.Udo.Crm.Scripts.Code.VT.onNewButtonClick);

    //This code is to facilitate another entity calling in an autopiloting the search
    if (Xrm.Page.getAttribute('va_autoload_vip').getValue() == 1) {
        $('IFRAME_search').ready(Va.Udo.Crm.Scripts.Code.VT.auto_Search);
    }

                rtContact = executePostSearchOperations();

                if (rtContact && rtContact != undefined) {
                    ShowFlagsAndTooltips(rtContact);
                }



}


Va.Udo.Crm.Scripts.Code.VT = {
    auto_Search: function () {
        var w = document.getElementById('IFRAME_search').contentWindow;

        //make sure the VIP is ready before we begin the search
        if (typeof w._appSearch == "undefined") {
            window.setTimeout(Va.Udo.Crm.Scripts.Code.VT.auto_Search, 1000);
        } else {
            Xrm.Page.getAttribute('va_autoload_vip').setValue(0);
            Xrm.Page.getAttribute("va_ssn").fireOnChange();
        }

    },
    Ssn_Search: function (context) {

        //setup some golbal varibles that VA code needs. 
        TranslateSearchType(); //in va_PhoneAndContactSharedFunctions.js

        var ssn = '';
        var controlName = context.getEventSource().getName();
        var field = Xrm.Page.getAttribute(controlName).getValue();

        if (field != null) {
            ssn = field;
            ssn = ssn.trim();
            ssn = ssn.replace(new RegExp('-', 'g'), '').replace(new RegExp(' ', 'g'), '');
        }
        if (ssn && ssn.length > 0) Xrm.Page.getAttribute('va_ssn').setValue(ssn);

        //build the search context that VA code needs
        var vetSearchCtx = new vrmContext(_exCon);
        _UserSettings = GetUserSettingsForWebservice(_exCon);
        vetSearchCtx.user = _UserSettings;
        vetSearchCtx.environment = GetEnvironment();
        vetSearchCtx.doSearchVadir = Xrm.Page.getAttribute('va_eccphonecall').getValue();
        vetSearchCtx.SetSearchParameters();

        // Pass data through to the EXTJS frame work 
        _vipSearchContext = vetSearchCtx;
        var w = document.getElementById('IFRAME_search').contentWindow;
        w._appSearch(vetSearchCtx);

    },

    //This function just collects the various attributes that make up the cache and
    //packages them up for the VIP Application
    LoadVIPCache: function () {
        //debugger;
        _XML_UTIL = new XmlUtilities(); // in va_XmlUtilities.js

        //fetch the fields into the attributes array
        //in the VA code _responseAttributes was defined 
        //in the golbal context, so I replicated that
        //behavior here.
        this.defineResponseAttributes();

        var useCache = true,
            xmlCachedData = [];


        for (var i = 0; i < _responseAttributes.length; i++) {
            var val = _responseAttributes[i].getValue();
            var responseXml = null;
            if (val && val.length > 0) { responseXml = _XML_UTIL.parseXmlObject(val); }
            xmlCachedData[_responseAttributes[i].getName()] = responseXml;
        }
        //debugger;

        window._currentEnv = window._currentEnv || environmentConfigurations.get();

        //this is the actual cache that the VIP application will read
        //It has to be available to the golbal context as VIP is loading
        //it through the parent context.
        _cachedData = [];
        _cachedData['Environment'] = GetEnvironment();
        _cachedData['UserSettings'] = GetUserSettingsForWebservice(_exCon);
        _cachedData['UseCache'] = useCache;
        _cachedData['Cache'] = xmlCachedData;
        _cachedData['TimerData'] = null;

    },
    addButton: function (attributename, buttonText, onClickAction) {
        if (document.getElementById(attributename) != null) {
            var sFieldID = "field_" + attributename;
            var elementID = document.getElementById(attributename + "_d");
            var div = document.createElement("div");
            div.style.width = "20%";
            div.style.textAlign = "center";
            div.style.display = "inline";
            elementID.appendChild(div, elementID);
            div.innerHTML = '<button id="' + sFieldID + '"  type="button" style="margin-left: 4px; width: 20%;" >' + buttonText + '</button>';
            document.getElementById(sFieldID).onclick = onClickAction;
        }
    },
    onNewButtonClick: function () {
        var parameters = {};
        //Set the the basic fields and the auto load flag
        parameters["va_ssn"] = Xrm.Page.getAttribute("va_ssn").getValue();
        parameters["va_autoload_vip"] = "true";
        parameters["va_searchtype"] = Xrm.Page.getAttribute("va_searchtype").getValue();
        parameters["va_calleridentityverified"] = Xrm.Page.getAttribute("va_calleridentityverified").getValue();
        parameters["va_filessnverified"] = Xrm.Page.getAttribute("va_filessnverified").getValue();
        parameters["va_bosverified"] = Xrm.Page.getAttribute("va_bosverified").getValue();
        parameters["va_eccphonecall"] = Xrm.Page.getAttribute("va_eccphonecall").getValue();
        parameters["va_searchcorpall"] = Xrm.Page.getAttribute("va_searchcorpall").getValue();
        // Open the window - entity, entity id (null for new), prefilled values
        Xrm.Utility.openEntityForm("va_testvipentity", null, parameters);
    },
    onButtonClick: function () {
        var choice = Xrm.Page.getAttribute("va_searchtype").getValue();
        switch (choice) {
            case 3: //EDIPI
                // alert("EDIPI Selected.");
                Xrm.Page.getAttribute("va_ssn").fireOnChange();
                break;
            case 2: //PID
                //alert("PID Selected.");
                Xrm.Page.getAttribute("va_ssn").fireOnChange();
                break;
            case 1: //SSN
                //alert("SSN Selected.");
            default:
                Xrm.Page.getAttribute("va_ssn").fireOnChange();
        }
    },

    //This function was lifted directly from the VA code
    //It simply puts all the Entity fields in to an array
    //to be paresed and added to the cache later
    defineResponseAttributes: function () {
        //Please make sure the order of array elements matches the order of fields on Phone/Contact forms
        _responseAttributes = [
            Xrm.Page.getAttribute("va_webserviceresponse"),
            Xrm.Page.getAttribute("va_findcorprecordresponse"),
            Xrm.Page.getAttribute("va_findbirlsresponse"),
            Xrm.Page.getAttribute("va_findveteranresponse"),
            Xrm.Page.getAttribute("va_generalinformationresponse"),
            Xrm.Page.getAttribute("va_generalinformationresponsebypid"),
            Xrm.Page.getAttribute("va_findaddressresponse"),
            Xrm.Page.getAttribute("va_benefitclaimresponse"),
            Xrm.Page.getAttribute("va_findbenefitdetailresponse"),
            Xrm.Page.getAttribute("va_findclaimstatusresponse"),
            Xrm.Page.getAttribute("va_findclaimantlettersresponse"),
            Xrm.Page.getAttribute("va_findcontentionsresponse"),
            Xrm.Page.getAttribute("va_finddependentsresponse"),
            Xrm.Page.getAttribute("va_findallrelationshipsresponse"),
            Xrm.Page.getAttribute("va_finddevelopmentnotesresponse"),
            Xrm.Page.getAttribute("va_findfiduciarypoaresponse"),
            Xrm.Page.getAttribute("va_findmilitaryrecordbyptcpntidresponse"),
            Xrm.Page.getAttribute("va_findpaymenthistoryresponse"),
            Xrm.Page.getAttribute("va_findtrackeditemsresponse"),
            Xrm.Page.getAttribute("va_findunsolvedevidenceresponse"),
            Xrm.Page.getAttribute("va_finddenialsresponse"),
            Xrm.Page.getAttribute("va_findawardcompensationresponse"),
            Xrm.Page.getAttribute("va_findotherawardinformationresponse"),
            Xrm.Page.getAttribute("va_findmonthofdeathresponse"),
            Xrm.Page.getAttribute("va_findincomeexpenseresponse"),
            Xrm.Page.getAttribute("va_findratingdataresponse"),
            Xrm.Page.getAttribute("va_findappealsresponse"),
            Xrm.Page.getAttribute("va_findindividualappealsresponse"),
            Xrm.Page.getAttribute("va_appellantaddressresponse"),
            Xrm.Page.getAttribute("va_updateappellantaddressresponse"),
            Xrm.Page.getAttribute("va_createnoteresponse"),
            Xrm.Page.getAttribute("va_findreasonsbyrbaissueidresponse"),
            Xrm.Page.getAttribute("va_isaliveresponse"),
            Xrm.Page.getAttribute("va_mviresponse"),
            Xrm.Page.getAttribute("va_readdataexamresponse"),
            Xrm.Page.getAttribute("va_readdataappointmentresponse"),
            Xrm.Page.getAttribute("va_awardfiduciaryresponse"),
            Xrm.Page.getAttribute("va_retrievepaymentsummaryresponse"),
            Xrm.Page.getAttribute("va_retrievepaymentdetailresponse"),
            Xrm.Page.getAttribute("va_getregistrationstatus"),
            Xrm.Page.getAttribute("va_findgetdocumentlist"),
            Xrm.Page.getAttribute("va_findpersonresponsevadir"),
            Xrm.Page.getAttribute("va_getcontactinfovadir")
        ];

        //_responseAttributesWithAggregation = [
        ////"va_generalinformationresponsebypid", // we only care about selected one, which is the one cached
        //	"va_findbenefitdetailresponse",
        //	"va_findclaimstatusresponse",
        //	"va_findclaimantlettersresponse",
        //	"va_findcontentionsresponse",
        //	"va_findindividualappealsresponse",
        //	"va_retrievepaymentdetailresponse"
        //];
    }
}



//******************************************************************
//**
//**  THE FOLLOWING FUNCTIONS ARE COPIED OUT OF va_PhoneCalleOnLoad_VIP.js
//**
//******************************************************************

function CopyAddressOnClick() {
    //debugger;
    //gets all the nodes with address type Mailing and selects the one with the most recent effective date.

    var vipaddressresponse = Xrm.Page.getAttribute('va_findaddressresponse').getValue();
    if (vipaddressresponse == null) return;

    var vipaddressresponseXML = _XML_UTIL.parseXmlObject(vipaddressresponse);
    var parentNode = vipaddressresponseXML.selectSingleNode('//findAllPtcpntAddrsByPtcpntIdResponse');
    if (parentNode === null) return;

    var address;
    var mostrecentdate;
    for (var i = 0; i < parentNode.childNodes.length; i++) {
        var childNode = parentNode.childNodes[i];
        if (childNode.nodeName === "return" && childNode.nodeType === 1) {
            var addrType = childNode.selectSingleNode('ptcpntAddrsTypeNm');
            var efctvDt = childNode.selectSingleNode('efctvDt').text;
            //return if mailing address doesn't exist

            if (addrType.text == 'Mailing' && (mostrecentdate == null || efctvDt > mostrecentdate)) {
                mostrecentdate = efctvDt;
                address = new Object();

                //check if addrsOne exists
                var addressOne = childNode.selectSingleNode('addrsOneTxt');
                address.addrsOne = addressOne != null ? addressOne.text : '';

                //check if addrstwo exists
                var addressTwo = childNode.selectSingleNode('addrsTwoTxt');
                address.addrsTwo = addressTwo != null ? addressTwo.text : '';

                //check if addrsThreeTxt exists
                var addressThree = childNode.selectSingleNode('addrsThreeTxt');
                address.addrsThree = addressThree != null ? addressThree.text : '';

                //check if cityNm exists
                var City = childNode.selectSingleNode('cityNm');
                address.cityNm = City != null ? City.text : '';

                //check if state exists
                var State = childNode.selectSingleNode('postalCd');
                address.stateNm = State != null ? State.text : '';

                //check if zip exists
                var ZipCode = childNode.selectSingleNode('zipPrefixNbr');
                address.zip = ZipCode != null ? ZipCode.text : '';

                //check if country exists
                var Country = childNode.selectSingleNode('cntryNm');
                address.countryNm = Country != null ? Country.text : '';
            }
        }
    }

    Xrm.Page.getAttribute('va_calleraddress1').setValue(address.addrsOne);
    Xrm.Page.getAttribute('va_calleraddress2').setValue(address.addrsTwo);
    Xrm.Page.getAttribute('va_calleraddress3').setValue(address.addrsThree);
    Xrm.Page.getAttribute('va_callercity').setValue(address.cityNm);
    Xrm.Page.getAttribute('va_callerstate').setValue(address.stateNm);
    Xrm.Page.getAttribute('va_callerzipcode').setValue(address.zip);
    Xrm.Page.getAttribute('va_callercountry').setValue(address.countryNm);

}




//Validate ID Proofing checks when attempting to initiate a CADD
ValidateIDProofingForAddressChange = function () {
    //Verify if the Failed ID Proofing is checked
    if (Xrm.Page.getAttribute("va_failedidproofingforchangeofaddress").getValue()) {
        //Set focus at Failed ID Proofing and alert user
        Xrm.Page.getControl("va_failedidproofingforchangeofaddress").setFocus();
        alert("Cannot initiate CADD because 'Failed ID Proofing' checkbox is set.");
        return false;
    }
    //Check three ID Proofing values
    var attr = [Xrm.Page.getAttribute("va_calleridentityverified"),
		 Xrm.Page.getAttribute("va_filessnverified"),
		 Xrm.Page.getAttribute("va_bosverified")];
    for (var a in attr) {
        //If array of values are not checked
        if (!attr[a].getValue()) {
            //Set focus at Failed ID Proofing and alert user
            Xrm.Page.getControl("va_failedidproofingforchangeofaddress").setFocus();
            alert("Cannot initiate CADD because one of the following checkboxes are NOT set:\n\nOn 'ID Protocol Verification:'\n-Name Verified\n-File/SSN Verified\n-BOS Verified");
            return false;
        }
    }

    return true;
    }

// Copied from ContactPCRForm_VIP.js
// called at the end of each async ws
function VIPEndOfServiceCall(xmlFieldName, success, requestXml, url, response, wsName, wsDuration) {
    //.length causes null pointer....
    //_WebServiceExecutionStatusLists += (_WebServiceExecutionStatusLists && _WebServiceExecutionStatusLists.length > 0 ? "," : "") +
	// "{'name':'" + wsName + "','executionTime':" + wsDuration + "}";

    if (success) {
        // for most attributes like Birls, set value
        // for some like claim details, add to the existing value
        var addToExistingValue = false;
        for (var i in _responseAttributesWithAggregation) {
            if (_responseAttributesWithAggregation[i] == xmlFieldName) { addToExistingValue = true; break; }
        }
        var val = Xrm.Page.getAttribute(xmlFieldName).getValue();
        if (!(val != null && val.length > 0 && (response == null || response.length == 0))) {
            var newVal = response;
            if (addToExistingValue) {
                //newVal = val + newVal;
                newVal = (val && val != undefined) ? val + newVal : newVal;
            }
            Xrm.Page.getAttribute(xmlFieldName).setValue(newVal);
        }

        // add result to the collection to know if we got enough data to trigger form operations
        // such as connection to contact, flag displays etc
        //_serviceResultsCollection[xmlFieldName] = response;
        if (!_endOfSearchReached) { VIPEndOfSearch(); }
    }
    else {
        // TODO: how to handle failure
        if (xmlFieldName == 'NOCACHE') { _missingMapping += requestXml + '\n'; }
    }
}


// Copied from ContactPCRForm_VIP.js
// called at the end of each async ws
function VIPEndOfSearch() {
    var requiredFields = [
		 'va_findbirlsresponse',
		 'va_findcorprecordresponse',
		 'va_generalinformationresponse',
    //'va_generalinformationresponsebypid',
		 'va_findfiduciarypoaresponse'
    ];

    // analyze if we got enough data for end of search
    var missingFields = false;

    for (var i in requiredFields) {
        if (Xrm.Page.getAttribute(requiredFields[i]).getValue() == null) {
            missingFields = true;
            break;
        }
    }
    if (missingFields) { return; }

    _endOfSearchReached = true;

    Xrm.Page.getAttribute("va_webserviceresponse").setValue(new Date());

    //if (_missingMapping.length > 0) {
    //    // extjs has some left some fields unmapped
    //    //alert('!! Unmapped cache !!\n\n' + _missingMapping);
    //    //debugger;
    //}

    // todo: get counts
    _CORP_RECORD_COUNT = 1;
    _BIRLS_RECORD_COUNT = 1;

    var rtContact = executePostSearchOperations(true);  // will call Mark as Related
    // Validate Vets station ID
    var user = GetUserSettingsForWebservice(_exCon);
    /*
    if (user && user.stationId && user.stationId == rtContact.BIRLSEmployeeStation) {
    alert('You do not have permission to view records for persons in your office (based on BIRLS employee station).');
    try {
    Xrm.Page.ui.close();
    }
    catch (er) { }
    finally { return; }
    } */

}

