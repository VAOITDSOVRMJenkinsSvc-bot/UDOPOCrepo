//<reference path="XrmPage-vsdoc.js" />

var FORM_TYPE_CREATE = 1;
var FORM_TYPE_UPDATE = 2;
var FORM_TYPE_READ_ONLY = 3;
var FORM_TYPE_DISABLED = 4;
var FORM_TYPE_QUICK_CREATE = 5;
var FORM_TYPE_BULK_EDIT = 6;
var FORM_TYPE_READ_OPTIMIZED = 11;

// This method will be call from CRM form
function OnLoad() {
// CSDev Left Intentionally Blank 
}
/*
* Load code for VIP instance of Phone Call
*/
_completed = false;
_wsObjects = new Array();
_fnod = false;
_openCallCount = 0;
_doGetOpenCallCount = false;
_isECC = false;
var _va_crm_udo_pageAlertMessageCount = 0;


function CCallWrapper(aObjectReference, aDelay, aMethodName, aArgument0, aArgument1, aArgument2, aArgument3, aArgument4, aArgument5, aArgument6, aArgument7, aArgument8, aArgument9) {
// CSDev Left Intentionally Blank 
}

CCallWrapper.prototype.execute = function () {
// CSDev Left Intentionally Blank 
};

CCallWrapper.prototype.cancel = function () {
// CSDev Left Intentionally Blank 
};

CCallWrapper.asyncExecute = function (/* CCallWrapper */callwrapper) {
    // CSDev Left Intentionally Blank CCallWrapper.mPendingCalls[callwrapper.mId].mTimerId = setTimeout('CCallWrapper.mPendingCalls["' + callwrapper.mId + '"].execute()', callwrapper.mDelay);
};

CCallWrapper.mCounter = 0;
CCallWrapper.mPendingCalls = {};

function OpenCalls(par) {
    // CSDev Left Intentionally Blank this.par = par;
}

_actions = null;
_WebServiceExecutionStatusLists = null;
_searchVariabless = null;

function onFormLoad() {
// CSDev Left Intentionally Blank 
}
//---------------------------------END ONLOAD EVENT--------------------------------------
/*******************************************************************************************/

function ViewCallScriptOnClick() { PopupScript("va_disposition", "va_dispositionsubtype", true, false, true); }
function ViewCallScriptOnClick2() { PopupScript("va_disposition2", "va_dispositionsubtype2", true, false, true); }
function ViewCallScriptOnClick3() { PopupScript("va_disposition3", "va_dispositionsubtype3", true, false, true); }
function ViewCallScriptOnClick4() { PopupScript("va_disposition4", "va_dispositionsubtype4", true, false, true); }
function ViewCallScriptOnClick5() { PopupScript("va_disposition5", "va_dispositionsubtype5", true, false, true); }
function IDCallScriptOnClick() { ShowScriptWindow('IdVerification.html'); }
function LetterRequestScriptOnClick() { ShowScriptWindow('LetterRequest_RepeatCaller.html'); }


function CopyAddressOnClick() {
// CSDev Left Intentionally Blank 
}



function StartCallTimer() {
// CSDev Left Intentionally Blank 
}

function StopCallTimer() {
// CSDev Left Intentionally Blank 
}

function ShowSectionsAfterStartCall() {
// CSDev Left Intentionally Blank 
}
//For Disposition Picklist controls, including Issues 2-5, remove Call Type options based on ECC or PCR role
function switchCallTypeOptions(fieldname) {
// CSDev Left Intentionally Blank 
}
function ActionOnEnter() {
// CSDev Left Intentionally Blank 
}
function GetCountOfOpenCalls() {
// CSDev Left Intentionally Blank 
}
function CheckPriorCalls() {
// CSDev Left Intentionally Blank 
}
// Regarding Onchange event
function RegardingChange() {
// CSDev Left Intentionally Blank 
}

// show different search fields 
function SearchTypeChange() {
// CSDev Left Intentionally Blank 
}

function ConfirmCallerIdentityOnClick() {
   // CSDev Left Intentionally Blank  Xrm.Page.getAttribute("va_calleridentityverified").setValue(true);
}

// load cached data
function LoadCachedDataVIP() {
// CSDev Left Intentionally Blank 
}

_missingMapping = '';

// handle search for veteran
//TODO: move to shared phone/contact file
function SearchOnClick(unattended_search) {
 // CSDev Left Intentionally Blank 
}

_SearchFunction = SearchOnClick;





function VIPEndOfSearch() {
 // CSDev Left Intentionally Blank 
}
_VIPEndOfSearch = VIPEndOfSearch;

// called at the end of each async ws
function VIPEndOfServiceCall(xmlFieldName, success, requestXml, url, response, wsName, wsDuration) {
  // CSDev Left Intentionally Blank 
}
_VIPEndOfServiceCall = VIPEndOfServiceCall;

function ProcessAndReportSearchResults(result) {
// CSDev Left Intentionally Blank 

}

function OnError(desc, data) {
// CSDev Left Intentionally Blank 
}
_SearchOnError = OnError;

function ContinueSearch() {
 // CSDev Left Intentionally Blank 
}

//Identify search individual
//function IdentifySearchIndividual(fileNumber, ptcpntId) {
//    //debugger
//    try {
//        var identifyIndividualCtx = new vrmContext();
//        identifyIndividualCtx.user = GetUserSettingsForWebservice();
//        identifyIndividualCtx.fileNumber = fileNumber;
//        identifyIndividualCtx.ptcpntId = ptcpntId;
//        identifyIndividualCtx.refreshExtjs = true;
//        _totalWebServiceExecutionTime = 0;
//        _VRMMESSAGE = new Array();

//        var vetIdentifyIndividualAction = new veteranIdentifyIndividual(identifyIndividualCtx);

//        prepareProgress();

//        var actions = [vetIdentifyIndividualAction];
//        var result = performCrmAction(actions);

//        if (result) {
//            _CORP_RECORD_COUNT = 1;
//            _BIRLS_RECORD_COUNT = 1;

//            updateSearchResultsSection();
//        }
//        CloseProgress();
//        //ProcessAndReportSearchResults(result);
//        UpdateContactHistoryGrid();
//    }
//    catch (e) {
//        CloseProgress();
//        alert("Error occurred during data retrieval.\n" + e.description + "\n\n" + GetErrorMessages('\n'));
//        LogMessages(true, false, false);
//    }
//}

function CallerRelationToVeteranOnClick(veteranIdentified) {
 // CSDev Left Intentionally Blank 

}

function ShowLocalTime() {
  // CSDev Left Intentionally Blank 
}

function AddIssue() {
 // CSDev Left Intentionally Blank 
}
function RemoveIssue() {
// CSDev Left Intentionally Blank 
}

function MoreSearchOptions() {
// CSDev Left Intentionally Blank 

}

function AssignSupervisor(desc) {
 // CSDev Left Intentionally Blank 
}

//function DetectIssueChangesandPromptSRCreation(handlingOpenEvent) {
//    var needSR = false;
//    var prompt = '';

//    // do nothing if call is closed
//    if (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_COMPLETED_ACTIVITY) { return; }

//    // when processing load event, we don't know which items triggered sr request, so just assume first issue, which is mandatory
//    for (var i = 0; i < 5; i++) {
//        var dispName = "va_disposition" + (i == 0 ? '' : (i + 1).toString());
//        var dispSubtypeName = "va_dispositionsubtype" + (i == 0 ? '' : (i + 1).toString());

//        var dispControl = Xrm.Page.getAttribute(dispName);
//        var disposition = dispControl.getSelectedOption();
//        var subtype = Xrm.Page.getAttribute(dispSubtypeName).getSelectedOption();

//        var isDirty = (handlingOpenEvent || dispControl.getIsDirty() || Xrm.Page.getAttribute(dispSubtypeName).getIsDirty());
//        if (disposition && isDirty) {
//            var scr = DispositionRequiresServiceRequest(dispControl.getValue(), Xrm.Page.getAttribute(dispSubtypeName).getValue());
//            if (scr && scr.va_RequiresServiceRequest == true) {
//                needSR = true;
//                prompt += "Issue #" + (i + 1).toString() + " - '" + subtype.text + "'\n";

//                var act = scr.va_DefaultSRAction;
//                var defaultAction = null; if (act) defaultAction = act.Value;
//                // Special Case: if FNOD and reporting for Non-Vet Bene, action is 0820a
//                if (_fnod && Xrm.Page.getAttribute('va_fnodreportingfor').getSelectedOption().text == 'Non-Veteran Beneficiary') {
//                    defaultAction = 953850001;
//                }
//                if (Xrm.Page.ui.getFormType() != CRM_FORM_TYPE_CREATE && confirm("Would you like to create a new Service Request for: " + "Issue #" + (i + 1).toString() + " - '" + subtype.text + "'?")) {
//                    /*This calls the new service request process*/
//                    var w = document.getElementById('IFRAME_search').contentWindow;
//                    if (w != undefined && w._appFireEvent != undefined) {
//                        w._appFireEvent('createphonecallservicerequest', { defaultType: defaultAction });
//                    }
//                    else {
//                        var createSrEvent = new FireCreateSrEvent();
//                        var callwrapper = new CCallWrapper(createSrEvent, 1000, 'get', { defaultType: defaultAction });
//                        CCallWrapper.asyncExecute(callwrapper);
//                    }
//                    _CreateClaimServiceRequest(null, 'Claim', true, null, defaultAction, dispControl.getValue());
//                    /********************************************/
//                }
//            }
//        }

//        if (handlingOpenEvent) return;
//    }

//    // pcr-terminated call requires SR too
//    if (Xrm.Page.getAttribute('va_pcrterminated').getIsDirty() && Xrm.Page.getAttribute('va_pcrterminated').getValue() == true) {
//        needSR = true;
//        prompt += "PCR-Terminated Call\n";
//        if (Xrm.Page.ui.getFormType() != CRM_FORM_TYPE_CREATE && confirm("Would you like to create a new Service Request for issue 'PCR Disconnected Call'?")) {
//            /*This calls the new service request process*/
//            w = document.getElementById('IFRAME_search').contentWindow;
//            if (w != undefined && w._appFireEvent != undefined) {
//                w._appFireEvent('createphonecallservicerequest', { defaultType: '953850008' });
//            }
//            else {
//                createSrEvent = new FireCreateSrEvent();
//                callwrapper = new CCallWrapper(createSrEvent, 1000, 'get', { defaultType: '953850008' });
//                CCallWrapper.asyncExecute(callwrapper);
//            }
//            _CreateClaimServiceRequest(null, 'Claim', true, 'Other', null, Xrm.Page.getAttribute('va_disposition').getValue());
//            /********************************************/
//        }
//    }
//    if (needSR && Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_CREATE && !handlingOpenEvent) {
//        // set the flag that SRs needed, flag wil be processed during form open
//        Xrm.Page.getAttribute('va_srneeded').setValue(true);
//        Xrm.Page.getAttribute('va_srneeded').setSubmitMode('always');
//    }
//}

function FireCreateSrEvent(par) {
   // CSDev Left Intentionally Blank  this.par = par;
}

FireCreateSrEvent.prototype.get = function (srData) {
   // CSDev Left Intentionally Blank 
};

function DispositionRequiresServiceRequest(disposition, sub) {
    // CSDev Left Intentionally Blank 
}

//Validate ID Proofing checks when attempting to initiate a CADD
function ValidateIDProofingForAddressChange() {
// CSDev Left Intentionally Blank 
}
_ValidateIDProofingForAddressChange = ValidateIDProofingForAddressChange;

//Phone Call ID Proofing
function IdProofingComplete() {
 // CSDev Left Intentionally Blank 
}

function ProgressDlg(text) { this.text = text; }
// CSDev Left Intentionally Blank 
};

function UpdateContactHistoryGrid() {
// CSDev Left Intentionally Blank 

}

function UpdateSubGrid(gridName, fetchXml) {
 // CSDev Left Intentionally Blank 
}

//Call this from CADD on Save
function RedrawCADDfields(pid) {
// CSDev Left Intentionally Blank 
}
_RedrawCADDfields = RedrawCADDfields;

function SelectPathwaysSearch() {
    // CSDev Left Intentionally Blank PathwaysSearchOnChange('tab_search', 'tab_search_pathwayoptions', Xrm.Page.getAttribute("va_searchcorpall").getValue());
}

function formatTelephone(telephoneField) {
 // CSDev Left Intentionally Blank 
}

function onChangeDispostionPostEvent() {
 // CSDev Left Intentionally Blank    callTypeFnodChange();
}

function callTypeFnodChange() {
 // CSDev Left Intentionally Blank 
}

function CheckCoBrowseIndicator() {
// CSDev Left Intentionally Blank 
}

function removeCallTypeValues() {
 // CSDev Left Intentionally Blank 
}

function removeIssueValues() {
 // CSDev Left Intentionally Blank 
}

function confirm0820Statement() {
  // CSDev Left Intentionally Blank 
}
