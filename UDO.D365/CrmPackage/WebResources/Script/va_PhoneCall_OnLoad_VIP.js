//<reference path="XrmPage-vsdoc.js" />

var FORM_TYPE_CREATE = 1;
var FORM_TYPE_UPDATE = 2;
var FORM_TYPE_READ_ONLY = 3;
var FORM_TYPE_DISABLED = 4;
var FORM_TYPE_QUICK_CREATE = 5;
var FORM_TYPE_BULK_EDIT = 6;
var FORM_TYPE_READ_OPTIMIZED = 11;
var exCon = null;
var formContext = null;
// This method will be call from CRM form
function OnLoad(execCon) {
    var exCon = execCon;
    formContext = exCon.getFormContext();
try {
        var createdBy = Xrm.Page.getControl("createdby");
        var udCreatedBy = Xrm.Page.getControl("udo_udcreatedby");
        var udCreatedByAttr = Xrm.Page.getAttribute("udo_udcreatedby");
        if (udCreatedByAttr.getValue()) {
            createdBy.setVisible(false);
            udCreatedBy.setVisible(true);
        }
        else {
            createdBy.setVisible(true);
            udCreatedBy.setVisible(false);
        }

    }
    catch (e) {
        alert("Encountered an error: " + e);
    }
    environmentConfigurations.initalize(exCon);
    commonFunctionsVip.initalize(exCon);
    ws.mapDDevelopmentNotes.initalize(exCon);
    // RU12 Form
    onFormLoad();
    // FNOD
    var xrmObject= Xrm.Page.context.getQueryStringParameters();
    var runfnod = xrmObject["set_fnod_disposition"];
    if (runfnod===true || runfnod==="true"  || runfnod===1) {
        _CreateClaimServiceRequest(null, 'FNOD', true, '0820a', null, Xrm.Page.getAttribute('va_disposition').getValue());
    }
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

function GetLoadExecutionContext() {
    return exCon !== null ? exCon : "";
}


function CCallWrapper(aObjectReference, aDelay, aMethodName, aArgument0, aArgument1, aArgument2, aArgument3, aArgument4, aArgument5, aArgument6, aArgument7, aArgument8, aArgument9) {
    this.mId = 'CCallWrapper_' + (CCallWrapper.mCounter++);
    this.mObjectReference = aObjectReference;
    this.mDelay = aDelay;
    this.mTimerId = 0;
    this.mMethodName = aMethodName;
    this.mArgument0 = aArgument0;
    this.mArgument1 = aArgument1;
    this.mArgument2 = aArgument2;
    this.mArgument3 = aArgument3;
    this.mArgument4 = aArgument4;
    this.mArgument5 = aArgument5;
    this.mArgument6 = aArgument6;
    this.mArgument7 = aArgument7;
    this.mArgument8 = aArgument8;
    this.mArgument9 = aArgument9;
    CCallWrapper.mPendingCalls[this.mId] = this;
}

CCallWrapper.prototype.execute = function () {
    this.mObjectReference[this.mMethodName](this.mArgument0, this.mArgument1, this.mArgument2, this.mArgument3, this.mArgument4, this.mArgument5, this.mArgument6, this.mArgument7, this.mArgument8, this.mArgument9);
    delete CCallWrapper.mPendingCalls[this.mId];
};

CCallWrapper.prototype.cancel = function () {
    clearTimeout(this.mTimerId);
    delete CCallWrapper.mPendingCalls[this.mId];
};

CCallWrapper.asyncExecute = function (/* CCallWrapper */callwrapper) {
    CCallWrapper.mPendingCalls[callwrapper.mId].mTimerId = setTimeout('CCallWrapper.mPendingCalls["' + callwrapper.mId + '"].execute()', callwrapper.mDelay);
};

CCallWrapper.mCounter = 0;
CCallWrapper.mPendingCalls = {};

function OpenCalls(par) {
    this.par = par;
}

_actions = null;
_WebServiceExecutionStatusLists = null;
_searchVariabless = null;

function onFormLoad() {
    //*** Load code for VIP instance of Phone Call
    _isLoading = true;

    Xrm.Page.getControl("va_noastatement").setDisabled(true);
    Xrm.Page.getAttribute("va_noastatement").setSubmitMode('always');

    var navItem = Xrm.Page.ui.navigation.items.get('nav_crme_phonecall_crme_dependentmaintenance_RegardingPhoneCall');
    if (navItem != null) {
        navItem.setVisible(isAddDependentEnabled_LeftNav());
    }

    // Set the version of VIP
    Xrm.Page.ui.tabs.get('tab_search').setLabel('Search - VIP ' + _currentEnv.Version);
    Xrm.Page.ui.tabs.get('tab_search').sections.get('tab_search_vrm').setLabel('Welcome to VRM UD - VIP ' + _currentEnv.Version);

    var noop = (Xrm.Page.getAttribute('subject').getValue() == 'NOOP'), ga = Xrm.Page.getAttribute, gc = Xrm.Page.getControl;

    if (!noop) {
        var mainResponseExists = (ga("va_generalinformationresponse").getValue() && ga("va_generalinformationresponse").getValue().length > 0);

        var rtContact = null;
        if (Xrm.Page.ui.getFormType() != CRM_FORM_TYPE_CREATE && mainResponseExists) {
            rtContact = executePostSearchOperations(true); // search for BIRLS as well

            if (rtContact && rtContact.fileNumber && rtContact.fileNumber.length > 0) {
                var user = GetUserSettingsForWebservice(exCon);

                if (user && user.fileNumber && rtContact.fileNumber == user.fileNumber) {
                    alert('You do not have permission to view this record because it is your own');
                    try {
                        Xrm.Page.ui.close();
                    }
                    catch (er) { }
                    finally { return; }
                }
                /*
                if (user && user.stationId && user.stationId == rtContact.BIRLSEmployeeStation) {
                alert("You do not have the permission to view records for persons in your own office (based on BIRLS employee station).\n\nVeteran's Employee Station Number (" + rtContact.BIRLSEmployeeStation + ") is identical to your PCR Station Number as specified on CRM User Settings screen.\nThe Phone Call screen will close.");                               
                try {
                Xrm.Page.ui.close();
                }
                catch (er) { }
                finally { return; }
                } */
            }
        }

        //See if user is an ECC user
        //TODO: Change to use udo_CRMCommonJS
        _isECC = (UserHasRole("ECC Case Manager") || UserHasRole("ECC Phone Tech"));

        defineResponseAttributes();
        if (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_UPDATE) { _formOpenTimeStamp = new Date(); }

        _completed = (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_COMPLETED_ACTIVITY);
        var editable = (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_CREATE || Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_UPDATE);

        // set initial values
        if (editable && (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_CREATE || !ga("va_numberofdispositions").getValue() || parseInt(ga("va_numberofdispositions").getValue()) < 1)) {
            ga("va_numberofdispositions").setValue(1);
        }
        if (editable && (!ga("va_searchtype").getValue() || parseInt(ga("va_searchtype").getValue()) < 1)) {
            ga("va_searchtype").setValue(1);
        }

        webResourceUrl = parent.Xrm.Page.context.getClientUrl() + '/WebResources/va_';

        if (!_completed) {
            _WebServiceExecutionStatusLists = ga('va_webserviceexecutionstatus').getValue();

            window.GeneralToolbar = new InlineToolbar("va_search");
            GeneralToolbar.AddButton("btnConv", "Search ('Enter' to run)", "100%", SearchOnClick, webResourceUrl + 'find.png');

            window.GeneralToolbar = new InlineToolbar("va_addissue");
            GeneralToolbar.AddButton("btnAddIssue", "Add Issue", "100%", AddIssue, webResourceUrl + 'add.png');
            GeneralToolbar.AddButton("btnAddIssue2", "Rem Issue", "100%", RemoveIssue, webResourceUrl + 'delete.png');

            window.GeneralToolbar = new InlineToolbar("va_supervisor");
            GeneralToolbar.AddButton("btnSuper", "Assign to Supervisor", "100%", AssignSupervisor, webResourceUrl + 'tux.png');

            window.GeneralToolbar = new InlineToolbar("va_emergency");
            GeneralToolbar.AddButton("btnSuper", "ID Proofing Complete", "100%", IdProofingComplete, null);

        }
        else {
            var fieldsNAInCompletedState = ["va_search", "va_addissue", "va_addissue", "va_supervisor"];
            for (var i = 0; i < fieldsNAInCompletedState.length; i++) { Xrm.Page.ui.controls.get(fieldsNAInCompletedState[i]).setVisible(false); }
        }


        window.GeneralToolbar = new InlineToolbar("va_viewcallscript");
        GeneralToolbar.AddButton("btnScript", "View Call Script", "70%", ViewCallScriptOnClick, webResourceUrl + 'status_online.png');

        window.GeneralToolbar = new InlineToolbar("va_viewcallscript2");
        GeneralToolbar.AddButton("btnScript2", "View Call Script", "100%", ViewCallScriptOnClick2, webResourceUrl + 'status_online.png');

        window.GeneralToolbar = new InlineToolbar("va_viewcallscript3");
        GeneralToolbar.AddButton("btnScript3", "View Call Script", "100%", ViewCallScriptOnClick3, webResourceUrl + 'status_online.png');

        window.GeneralToolbar = new InlineToolbar("va_viewcallscript4");
        GeneralToolbar.AddButton("btnScript4", "View Call Script", "100%", ViewCallScriptOnClick4, webResourceUrl + 'status_online.png');

        window.GeneralToolbar = new InlineToolbar("va_viewcallscript5");
        GeneralToolbar.AddButton("btnScript5", "View Call Script", "100%", ViewCallScriptOnClick5, webResourceUrl + 'status_online.png');

        window.GeneralToolbar = new InlineToolbar("va_flags");
        GeneralToolbar.AddButton("btnScript8", "ID Call Script", "100%", IDCallScriptOnClick, webResourceUrl + 'status_online.png');

        window.GeneralToolbar = new InlineToolbar("va_letterrequestscript");
        GeneralToolbar.AddButton("btnScript5", "Letter Request Script", "100%", LetterRequestScriptOnClick, webResourceUrl + 'status_online.png');

        window.GeneralToolbar = new InlineToolbar("va_copyaddress");
        GeneralToolbar.AddButton("btnScript7", "Copy Veteran’s Mailing Address from VIP", "100%", CopyAddressOnClick, webResourceUrl + 'status_online.png');

        window.parent.document.getElementById("crmContentPanel").style.top = "0px";

        // wire up events
        Xrm.Page.data.entity.attributes.get('va_numberofdispositions').addOnChange(NoOfDispositionsOnClick);
        Xrm.Page.getControl("va_numberofdispositions").setDisabled(true);
        ga("va_numberofdispositions").setSubmitMode('always');


        //Removed: Added picklist values to original Call Type and SubCall Type lists

        //        // Setup ECC Call types and sub types            
        //        ga('va_ecccalltype').setRequiredLevel(_isECC ? 'required' : 'none');
        //        ga('va_eccsubcalltype').setRequiredLevel(_isECC ? 'required' : 'none');
        //        if (Xrm.Page.ui.getFormType() !== CRM_FORM_TYPE_COMPLETED_ACTIVITY) {
        //            gc('va_eccsubcalltype').setDisabled(!ga('va_eccsubcalltype').getValue() && _isECC);
        //        }
        //        ga('va_disposition').setRequiredLevel(!_isECC ? 'required' : 'none');
        //        ga('va_dispositionsubtype').setRequiredLevel(!_isECC ? 'required' : 'none');

        //        ga('va_ecccalltype').addOnChange(function () {
        //            var vertName, callType, newSubject,
        //                options = _optionSetUtility.getSubOptionsFromOption(ga('va_ecccalltype').getText(), ga('va_eccsubcalltype').getOptions(), _eccCallTypeMappings);
        //            _optionSetUtility.loadOptionSet(gc('va_eccsubcalltype'), options);
        //            gc('va_eccsubcalltype').setDisabled(options.length === 0);

        //            // Update Subject Field
        //            callType = ga('va_ecccalltype').getValue() ? ga('va_ecccalltype').getText() : '';
        //            subject = ga('subject').getValue() ? ga('subject').getValue() : '';

        //            if (subject.indexOf('-') !== -1) {
        //                vertName = subject.substring(subject.indexOf('-'))
        //                newSubject = callType + ' ' + vertName;
        //            } else {
        //                newSubject = callType;
        //            }

        //            ga('subject').setValue(newSubject);
        //        });
        //        ga('va_eccsubcalltype').addOnChange(function () { });

        ga('va_disposition').addOnChange(function () { RefreshDispAndSubject("va_disposition", "va_dispositionsubtype", "va_dispositioncomments"); onChangeDispostionPostEvent(); });
        ga('va_disposition2').addOnChange(function () { RefreshDispAndSubject("va_disposition2", "va_dispositionsubtype2", "va_dispositioncomments2"); });
        ga('va_disposition3').addOnChange(function () { RefreshDispAndSubject("va_disposition3", "va_dispositionsubtype3", "va_dispositioncomments3"); });
        ga('va_disposition4').addOnChange(function () { RefreshDispAndSubject("va_disposition4", "va_dispositionsubtype4", "va_dispositioncomments4"); });
        ga('va_disposition5').addOnChange(function () { RefreshDispAndSubject("va_disposition5", "va_dispositionsubtype5", "va_dispositioncomments5"); });

        ga('va_dispositionsubtype').addOnChange(function () { CheckPriorCalls(); PopupScript("va_disposition", "va_dispositionsubtype"); });
        ga('va_dispositionsubtype2').addOnChange(function () { PopupScript("va_disposition2", "va_dispositionsubtype2"); });
        ga('va_dispositionsubtype3').addOnChange(function () { PopupScript("va_disposition3", "va_dispositionsubtype3"); });
        ga('va_dispositionsubtype4').addOnChange(function () { PopupScript("va_disposition4", "va_dispositionsubtype4"); });
        ga('va_dispositionsubtype5').addOnChange(function () { PopupScript("va_disposition5", "va_dispositionsubtype5"); });

        Xrm.Page.getAttribute('va_callerrelationtoveteran').addOnChange(function () { CallerRelationToVeteranOnClick(false); });
        Xrm.Page.getAttribute('va_searchtype').addOnChange(SearchTypeChange);
        Xrm.Page.getAttribute('regardingobjectid').addOnChange(SearchTypeChange);
        Xrm.Page.getAttribute('va_timezone').addOnChange(ShowLocalTime);
        Xrm.Page.getAttribute('va_moresearchoptions').addOnChange(MoreSearchOptions); // fireOnChange no longer needed as of RU12
        Xrm.Page.getAttribute('va_searchcorpall').addOnChange(SelectPathwaysSearch); // fireOnChange no longer needed as of RU12
        Xrm.Page.getAttribute('va_cobrowsesessionindicator').addOnChange(CheckCoBrowseIndicator);
        Xrm.Page.getAttribute('va_disposition').addOnChange(confirm0820Statement);

        Xrm.Page.data.entity.attributes.get('va_ssn').addOnChange(UpdateContactHistoryGrid);
        //VTRIGILI 2015-01-20 - Change event to fire same function as the search button
        //which more correctly fits what the screen says will happen
        //Xrm.Page.data.entity.attributes.get('va_ssn').addOnChange(UpdateContactHistoryGrid);
        Xrm.Page.data.entity.attributes.get('va_ssn').addOnChange(SearchOnClick);


        Xrm.Page.data.entity.attributes.get('va_searchpathways').addOnChange(function () {
            var checked = ga("va_searchpathways").getValue();
            PathwaysSearchOnChange('tab_search', 'tab_search_pathwayoptions', checked);
        });

        Xrm.Page.data.entity.attributes.get('va_findappealsby').addOnChange(function () {
            var userSelection = ga("va_findappealsby").getValue();
            //'These Values' == 953850002
            Xrm.Page.ui.controls.get('va_appealsssn').setVisible((userSelection == 953850002));
            Xrm.Page.ui.controls.get('va_appealslastname').setVisible((userSelection == 953850002));
            Xrm.Page.ui.controls.get('va_appealsfirstname').setVisible((userSelection == 953850002));
            Xrm.Page.ui.controls.get('va_appealsdateofbirth').setVisible((userSelection == 953850002));
            Xrm.Page.ui.controls.get('va_appealscity').setVisible((userSelection == 953850002));
            Xrm.Page.ui.controls.get('va_appealsstate').setVisible((userSelection == 953850002));
        });

        // set default issue and subissue
        if (editable) {
            if (!ga("va_disposition").getValue()) {
            }
            else if (!ga("subject").getValue()) { RefreshDispAndSubject("va_disposition", "va_dispositionsubtype", "va_dispositioncomments"); }
        }

        _dispStructureAndPopup = new Array();

        disableDispositionSubType();
        showAndHideDispositions();
        setupCreateFormOnLoad();
        SearchTypeChange();
        NoOfDispositionsOnClick();
        ShowLocalTime();
        ClaimsAddSectionVisibility();
        MoreSearchOptions();
        TranslateSearchType();
        SelectPathwaysSearch();
        CheckCoBrowseIndicator();
        PathwaysSearchOnChange('tab_search', 'tab_search_pathwayoptions', false);
        AppealsSearchOnChange('tab_search', 'tab_search_appealsoptions', false);

        UpdateContactHistoryGrid();
        PopupScript("va_disposition", "va_dispositionsubtype", false, true);

        if (ga("va_disposition2").getValue() && ga("va_dispositionsubtype2").getValue()) {
            RefreshDispAndSubject("va_disposition2", "va_dispositionsubtype2", "va_dispositioncomments2");
            ga("va_dispositionsubtype2").setValue(ga("va_dispositionsubtype2").getValue());
        }
        if (ga("va_disposition3").getValue() && ga("va_dispositionsubtype3").getValue()) {
            RefreshDispAndSubject("va_disposition3", "va_dispositionsubtype3", "va_dispositioncomments3");
            ga("va_dispositionsubtype3").setValue(ga("va_dispositionsubtype3").getValue());
        }
        if (ga("va_disposition4").getValue() && ga("va_dispositionsubtype4").getValue()) {
            RefreshDispAndSubject("va_disposition4", "va_dispositionsubtype4", "va_dispositioncomments4");
            ga("va_dispositionsubtype4").setValue(ga("va_dispositionsubtype4").getValue());
        }
        if (ga("va_disposition5").getValue() && ga("va_dispositionsubtype5").getValue()) {
            RefreshDispAndSubject("va_disposition5", "va_dispositionsubtype5", "va_dispositioncomments5");
            ga("va_dispositionsubtype5").setValue(ga("va_dispositionsubtype5").getValue());
        }
        Xrm.Page.getControl('va_servicerequestdate').setDisabled(true);

        Xrm.Page.getControl('va_veteransensitivitylevel').setDisabled(true);
        ga('va_veteransensitivitylevel').setSubmitMode('always');

        Xrm.Page.getControl('va_sensitivitylevelvalue').setDisabled(true);
        ga('va_sensitivitylevelvalue').setSubmitMode('always');

        Xrm.Page.getControl('va_dispositioncomments').setVisible(true);
        Xrm.Page.getControl('va_dispositioncomments').setDisabled(false);
        Xrm.Page.getControl('va_isaveteran').setDisabled(true);
        Xrm.Page.getControl('va_calldurationsec').setDisabled(true);

        for (var i = 0; i < _responseAttributes.length; i++) {
            document.getElementById(_responseAttributes[i].getName()).disabled = false;

            if (!_allowToCashXMLResponses)
                _responseAttributes[i].setValue(null);
            else
                _responseAttributes[i].setSubmitMode('always');
        }

        var recentCall = false;
        var doLoadCache = false;

        // when loading existing record, if ws response and regarding are set, ask if user wants to load existing data
        if (Xrm.Page.ui.getFormType() != CRM_FORM_TYPE_CREATE && mainResponseExists) {
            var callDate = '';
            var lastCall = ga("va_webserviceresponse").getValue();

            if (lastCall) {
                callDate = '\n\nThe data was last refreshed on ' + lastCall.format("MM/dd/yyyy hh:mm");
                var oldness = ((new Date()).getTime() - lastCall.getTime()) / 1000;
                if (oldness <= 1800) { recentCall = true; } // up to 30 min - no prompt
            }

            if (_completed ||
		recentCall ||
		window.status == '+' ||
		 confirm("Contact history for this record contains cached data.\n\nWould you like to load Veteran's information cached in CRM without refreshing it from the system-of-record servers?"
		+ callDate)) {
                _CORP_RECORD_COUNT = 1;
                _BIRLS_RECORD_COUNT = 1;
                //debugger
                // already visible Xrm.Page.ui.tabs.get('tab_search').sections.get('phone searchresults').setVisible(true);
                //Xrm.Page.ui.tabs.get('tab_search').sections.get('Categorize Call_section_idproofing').setVisible(true);
                Xrm.Page.ui.tabs.get('tab_search').sections.get('phonecall_section_idprotocol').setVisible(true);

                // new VIP function: pass launch to EXTJS
                doLoadCache = true;
                //LoadCachedDataVIP();


                rtContact = executePostSearchOperations();

                if (rtContact && rtContact != undefined) {
                    ShowFlagsAndTooltips(rtContact);
                }

                if (lastCall) {
                    Xrm.Page.ui.tabs.get('tab_search').sections.get('phone searchresults').setLabel('Search Results - Showing Data Cached on ' + lastCall.format("MM/dd/yyyy hh:mm"));
                }

            }
        }

        //VTRILIGI - 2015-01-23 The JQuery bind is not working in CRM2013
        //        unable to determine why, but addEvent does work so 
        //        switching to that code instead

        // $('form').bind('keypress', 
        document.addEventListener('keypress',
          function (e) {
              if (e.keyCode == 13) {
                  ActionOnEnter();
                  e.cancelBubble = true;
                  e.returnValue = false;
                  return false;
              }

          });

        //  $('form').bind('keydown', 
        document.addEventListener("keydown", function (e) {
            if (e.keyCode == 76) {
                if (e.ctrlKey) { // Ctrl-L 
                    // launch Links popup from RibbonActions
                    PCR();
                    e.cancelBubble = true;
                    e.returnValue = false;
                    return false;
                }
            }
        });

        // collapse tab
        Xrm.Page.ui.tabs.get("phonecall").setDisplayState("collapsed");

        // auto search check
        // do it if there's no cached xml and either ssn or pid are present
        var ssnIs = (ga("va_ssn").getValue() && ga("va_ssn").getValue().length > 0);
        if (editable && !mainResponseExists &&
			(ssnIs || (ga("va_participantid").getValue() && ga("va_participantid").getValue().length > 0))) {
            // set default issue and subissue

            if (!ga("subject").getValue()) { RefreshDispAndSubject("va_disposition", "va_dispositionsubtype", "va_dispositioncomments"); }
            if (!ga("subject").getValue()) { ga("subject").setValue('Other'); }

            // set search type
            var defaultSearchBy = 1; // ssn
            if (!ssnIs) defaultSearchBy = 2; // pid
            if (ga("va_searchtype").getValue() != defaultSearchBy) ga("va_searchtype").setValue(defaultSearchBy);

            SearchOnClick();
        }

        _doGetOpenCallCount = (!recentCall && window.status != '+');

    }   // if (!noop)
    else {
        Xrm.Page.getControl('IFRAME_search').setSrc('_static/blank.htm');
    }

    _loadTime = null;

    $(document).ready(function () {
        try {
            // record load time
            if (LoadStartTime != undefined && LoadStartTime != null) {
                _loadTime = (new Date()).getTime() - LoadStartTime;
                Xrm.Page.getControl('va_searchactionscompleted').setLabel('Search Actions (ttl: ' + _loadTime.toString() + ')');
            }

            if (noop) { ShowSectionsAfterStartCall(); return; }

            window.top.moveTo(0, 0);
            if (document.all) {
                window.top.resizeTo(screen.availWidth, screen.availHeight);
            }
            else if (document.layers || document.getElementById) {
                if (window.top.outerHeight < screen.availHeight || window.top.outerWidth < screen.availWidth) {
                    window.top.outerHeight = screen.availHeight;
                    window.top.outerWidth = screen.availWidth;
                }
            }
        } catch (mer) { }

        // show tabs or start counter
        if (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_CREATE) {
            _searchCounter = 0;
            // set default pathways search
            var dt = new Date(); dt.setMonth(dt.getMonth() - 6); ga('va_appointmentfromdate').setValue(dt);
            dt = new Date(); dt.setMonth(dt.getMonth() + 6); ga('va_appointmenttodate').setValue(dt);

            // show Start Call sections, hide others
            var startSection = Xrm.Page.ui.tabs.get("tab_search").sections.get("tab_search_section_startcall");
            startSection.setVisible(true);
            startSection.setLabel('Click on Start Call button below to start call timer and show Search, Call Details & other sections');
            window.GeneralToolbar = new InlineToolbar("va_fullnameca");
            GeneralToolbar.AddButton("btnScriptStartCall", "Start Call ('Enter')", "100%", StartCallTimer, null);
        }
        else {
            Xrm.Page.ui.tabs.get("tab_search").sections.get("tab_search_section_startcall").setVisible(false);
            CallerRelationToVeteranOnClick(false);
            _searchCounter = GetSearchCounter();
            ShowSectionsAfterStartCall();

            if (doLoadCache) {
                LoadCachedDataVIP();
            }

            // during save of new screen, if we detected that new SRs must be created, prompt and create them now
            //if (ga('va_srneeded').getValue() === true) {
            //    DetectIssueChangesandPromptSRCreation(true);
            //    ga('va_srneeded').setValue(false);
            //    ga('va_srneeded').setSubmitMode('always');
            //}
        }
        Xrm.Page.ui.refreshRibbon();

        Xrm.Page.ui.tabs.get("tab_search").sections.get("tab_search_vrm").setVisible(false);

        if (_doGetOpenCallCount) {
            GetCountOfOpenCalls();
        }
    });

    _isLoading = false;

    // COMMENTED OUT - CC 2015-12-23
    //var va_findpaymenthistoryresponse = Xrm.Page.getAttribute('va_findpaymenthistoryresponse').getValue();
    //if (va_findpaymenthistoryresponse != null) {
    //    va_findpaymenthistoryresponse = va_findpaymenthistoryresponse.substr(0, 300);
    //    Xrm.Page.getAttribute('va_findpaymenthistoryresponse').setValue(va_findpaymenthistoryresponse);
    //}

    //var va_retrievepaymentsummaryresponse = Xrm.Page.getAttribute('va_retrievepaymentsummaryresponse').getValue();
    //if (va_retrievepaymentsummaryresponse != null) {
    //    va_retrievepaymentsummaryresponse = va_retrievepaymentsummaryresponse.substr(0, 600);
    //    Xrm.Page.getAttribute('va_retrievepaymentsummaryresponse').setValue(va_retrievepaymentsummaryresponse);
    //}
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



function StartCallTimer() {
    ShowSectionsAfterStartCall();

    _formOpenTimeStamp = new Date();
    Xrm.Page.getAttribute("va_fullnameca").setValue(true);

    Xrm.Page.ui.tabs.get("tab_search").sections.get("tab_search_section_startcall").setVisible(false);
    Xrm.Page.getControl('va_ssn').setFocus();

    var w = document.getElementById('IFRAME_search').contentWindow;
    if (w) {
        var timerData = Xrm.Page.getAttribute("va_calldurationsec").getValue();
        if (timerData) { try { timerData = parseInt(timerData); } catch (pe) { } }
        for (var i = 0; i < 300; i++) {
            try {
                w._appStartTimer(timerData);
                i = 400;
            }
            catch (te) { }
        }
    }
}

function StopCallTimer() {
    var w = document.getElementById('IFRAME_search').contentWindow;
    if (w) { w._appStopTimer(); }
}

function ShowSectionsAfterStartCall() {
    Xrm.Page.ui.tabs.get("tab_search").sections.get("searchsection").setVisible(true);
    Xrm.Page.ui.tabs.get("tab_search").sections.get("subject").setVisible(true);
    Xrm.Page.ui.tabs.get("tab_search").sections.get("searchparams_ssn").setVisible(true);
    Xrm.Page.ui.tabs.get("tab_search").sections.get("callerdetails").setVisible(true);
    Xrm.Page.ui.tabs.get("tab_search").sections.get("phonecall_section_idprotocol").setVisible(true);
    Xrm.Page.ui.tabs.get("tab_search").sections.get("idproof_buttons").setVisible(true);
    Xrm.Page.ui.tabs.get("tab_search").sections.get("callerdetails2").setVisible(true);
    Xrm.Page.ui.tabs.get("tab_search").sections.get("callerdetails3").setVisible(true);     //(!_isECC);
    Xrm.Page.ui.tabs.get("tab_search").sections.get("searchparams2").setVisible(true);
    Xrm.Page.ui.tabs.get("tab_search").sections.get("searchparams3").setVisible(true);
    Xrm.Page.ui.tabs.get("tab_search").sections.get("searchparams4").setVisible(true);

    Xrm.Page.ui.tabs.get("Categorize Call").setVisible(true);
    Xrm.Page.ui.tabs.get("phonecall").setVisible(true);
    Xrm.Page.ui.tabs.get("tab_history").setVisible(true);
    Xrm.Page.ui.tabs.get("tab_sr").setVisible(true);
    Xrm.Page.ui.tabs.get("notes").setVisible(true);
    Xrm.Page.ui.tabs.get("tab_outreach").setVisible(true);
    Xrm.Page.ui.tabs.get("Web Service Response").setVisible(true);

    //ECC userRole
    if (_isECC == true) {
        //Xrm.Page.ui.tabs.get('tab_search').sections.get("ecccalltypessection").setVisible(true);
        Xrm.Page.ui.tabs.get("tab_search").sections.get("ecc").setVisible(true);
        Xrm.Page.ui.tabs.get("tab_search").sections.get("ecc_idproofing").setVisible(true);
        Xrm.Page.getAttribute("va_eccphonecall").setValue(1);
        Xrm.Page.getAttribute("va_eccphonecall").setSubmitMode("always");
    }
        //PCR
    else {
        //Remove EDU options for PCR roles into Caller Type
        //Caller relation to Veteran
        var optionsetControl = Xrm.Page.getControl('va_callerrelationtoveteran');
        optionsetControl.removeOption(953850009);
        optionsetControl.removeOption(953850010);
    }
    switchCallTypeOptions('va_disposition');
}
//For Disposition Picklist controls, including Issues 2-5, remove Call Type options based on ECC or PCR role
function switchCallTypeOptions(fieldname) {
    var optionsetControl = Xrm.Page.getControl(fieldname);
    //ECC userRole
    if (_isECC == true) {
        //Call Type -Remove PCR options
        optionsetControl.removeOption(953850000);   //Appeals
        optionsetControl.removeOption(953850001);   //Claim
        optionsetControl.removeOption(953850002);   //Correspondence and Forms
        optionsetControl.removeOption(953850011);   //eBenefits
        optionsetControl.removeOption(953850003);   //Fiduciary
        optionsetControl.removeOption(953850004);   //FNOD
        optionsetControl.removeOption(953850005);   //General Questions
        optionsetControl.removeOption(953850006);   //Medical
        optionsetControl.removeOption(953850007);   //Other
        optionsetControl.removeOption(953850008);   //Other Business Lines
        optionsetControl.removeOption(953850009);   //Other Benefits – Comp or Pension
        optionsetControl.removeOption(953850010);   //Payments / Debts
        optionsetControl.removeOption(953850016);   //SEP VSO
        optionsetControl.removeOption(953850012);   //Special Issues
        optionsetControl.removeOption(953850013);   //Suicide Call
        optionsetControl.removeOption(953850014);   //Threat Call
        optionsetControl.removeOption(953850015);   //Update Information
    }
        //PCR userRole
    else {
        //Call Type -Remove ECC options
        optionsetControl.removeOption(953850017);   //General Inquiry
        optionsetControl.removeOption(953850018);   //Claim Inquiry
        optionsetControl.removeOption(953850019);   //Monthly Certification
        optionsetControl.removeOption(953850020);   //Update Information //Could be a problem - since named the same thing (reuse the other?)
        optionsetControl.removeOption(953850021);   //Payment Inquiry
        optionsetControl.removeOption(953850022);   //Debt Inquiry
        optionsetControl.removeOption(953850023);   //Letters/Documents Request
        optionsetControl.removeOption(953850024);   //Major Event - Call Tracker
        optionsetControl.removeOption(953850025);   //Emergency/Priority
        optionsetControl.removeOption(953850026);   //Other //Could be a problem - since named the same thing (reuse the other?)
    }
}
function ActionOnEnter() {
    var startSection = Xrm.Page.ui.tabs.get("tab_search").sections.get("tab_search_section_startcall");
    if (startSection.getVisible()) {
        // beginning of call
        StartCallTimer();
    }
    else {
        Xrm.Page.getControl('va_callerfirstname').setFocus();
        Xrm.Page.getControl('va_ssn').setFocus();
        SearchOnClick();
    }
}
function GetCountOfOpenCalls() {
    // from the same owner
    var cnt = 0;
    var columns = ['ActivityId'];
    var filter = "StateCode/Value eq 0 and OwningUser/Id eq guid'" + Xrm.Page.context.getUserId() + "'";
    if (Xrm.Page.data.entity.getId()) filter += " and ActivityId ne guid'" + Xrm.Page.data.entity.getId() + "'";
    var restResults = CrmRestKit2011.ByQueryAll('PhoneCall', columns, filter);
    restResults.fail(
    function (error) {
    })
    .done(function (data) {
        var elm = document.getElementById('footer_phonenumber_d');
        cnt = data.length;
        if (elm) {
            var field = elm.childNodes[0];
            var msg = cnt.toString() + ' Open Call(s)';
            field.innerText = msg;
            field.title = msg + '\n' + field.title;
        }
        if (cnt > 1) {
            var cm = 'Please note that you have ' + cnt.toString() + ' open call(s)' + (_completed ? '' : ' besides this call');
            alert(cm);
        }
    });
    return;
}
function CheckPriorCalls() {
    try {
        // validate that there was already call with this person and disposition
        // Per Kathleen, do it without prompt, and only mark if prior call is within a week - see timeSpan <= week clause

        var id = LookupId(Xrm.Page.getAttribute("regardingobjectid")),
			disp = Xrm.Page.getAttribute("va_disposition").getValue(),
			dispSub = Xrm.Page.getAttribute("va_dispositionsubtype").getValue();

        if (_isLoading || !id || !disp || LookupId(Xrm.Page.getAttribute("va_relatedpriorphonecallid"))) { return; }

        var columns = ['Subject', 'ActivityId', 'CreatedOn'];
        var filter = "RegardingObjectId/Id eq guid'" + id.toString() + "' and va_Disposition/Value eq " + disp.toString() +
						  " and va_DispositionSubtype/Value eq " + (dispSub ? +dispSub : "null")
        var orderby = "&$orderby=" + encodeURIComponent("CreatedOn desc,Subject asc ");
        var query = Xrm.Page.context.getClientUrl() + 'XRMServices/2011/OrganizationData.svc' + "/" + "PhoneCallSet"
            + "?$select=" + columns.join(',') + "&$filter=" + encodeURIComponent(filter) + orderby;
        var calls = CrmRestKit2011.ByQueryUrl(query, true);
        this.window.focus();
        calls.fail(
            function (error) {
            })
        calls.done(function (data) {
            if (data && data.d.results && data.d.results.length > 0 && data.d.results[0].Subject) {
                var fieldValue = eval(data.d.results[0].CreatedOn);
                var dt = new Date(parseInt(fieldValue.toString().replace("/Date(", "").replace(")/", "")));
                var dateValue = dt.format("MM/dd/yyyy hh:mm");
                var timeSpan = ((new Date()).getTime() - dt.getTime()) / 1000;
                var week = 7 * 24 * 3600;
                if (timeSpan <= week) {
                    //Xrm.Page.getAttribute("va_thisisarepeatcall").setValue(true);
                    Xrm.Page.getAttribute('va_relatedpriorphonecallid').setValue([{ id: data.d.results[0].ActivityId, name: data.d.results[0].Subject, entityType: 'PhoneCall' }]);
                }
            }
        });
    }
    catch (e) {
        alert("Error occurred when checking for prior calls.\n" + e.description);
    }
}
// Regarding Onchange event
function RegardingChange() {
    if (_isLoading || !Xrm.Page.getAttribute("regardingobjectid").getValue()) return;

    var regardingType = Xrm.Page.getAttribute("regardingobjectid").getValue()[0].entityType;
    if (regardingType && regardingType.length > 0 && regardingType != 'contact') {
        alert('Please only use Veteran Contact records in Regarding field.');
        Xrm.Page.getAttribute("regardingobjectid").setValue(null);
    }
    CheckPriorCalls();
}

// show different search fields 
function SearchTypeChange() {
    //var ediOption = !Xrm.Page.getAttribute("va_searchby").getValue();
    var option = Xrm.Page.getAttribute("va_searchtype").getValue();

    if (!option) option = 1;

    switch (parseInt(option)) {
        case 2:
            Xrm.Page.ui.tabs.get('tab_search').sections.get('searchparams_ssn').setVisible(false);
            Xrm.Page.ui.tabs.get('tab_search').sections.get('searchparams_edipi').setVisible(false);
            Xrm.Page.ui.tabs.get('tab_search').sections.get('tab_search_section_moreparams').setVisible(false);
            Xrm.Page.ui.tabs.get('tab_search').sections.get('tab_search_section_pid').setVisible(true);
            break;
        case 3:
            Xrm.Page.ui.tabs.get('tab_search').sections.get('searchparams_ssn').setVisible(false);
            Xrm.Page.ui.tabs.get('tab_search').sections.get('searchparams_edipi').setVisible(true);
            Xrm.Page.ui.tabs.get('tab_search').sections.get('tab_search_section_moreparams').setVisible(false);
            Xrm.Page.ui.tabs.get('tab_search').sections.get('tab_search_section_pid').setVisible(false);
            break;
        case 1:
        default:
            if (!_isLoading) { Xrm.Page.ui.tabs.get('tab_search').sections.get('searchparams_ssn').setVisible(true); }
            MoreSearchOptions();
            Xrm.Page.ui.tabs.get('tab_search').sections.get('searchparams_edipi').setVisible(false);
            Xrm.Page.ui.tabs.get('tab_search').sections.get('tab_search_section_pid').setVisible(false);
            break;
    }
}

function ConfirmCallerIdentityOnClick() {
    Xrm.Page.getAttribute("va_calleridentityverified").setValue(true);
}

// load cached data
function LoadCachedDataVIP() {
    var w = document.getElementById('IFRAME_search').contentWindow;

    var useCache = true,
    xmlCachedData = [];

    for (var i = 0; i < _responseAttributes.length; i++) {
        var val = _responseAttributes[i].getValue();
        var responseXml = null;
        if (val && val.length > 0) { responseXml = _XML_UTIL.parseXmlObject(val); }
        xmlCachedData[_responseAttributes[i].getName()] = responseXml;
    }

    // instead of calling load function in extjs, wait till it loads and starts itself
    _cachedData = [];
    _cachedData['Environment'] = GetEnvironment();
    _cachedData['UserSettings'] = GetUserSettingsForWebservice(exCon);
    _cachedData['UseCache'] = useCache;
    _cachedData['Cache'] = xmlCachedData;
    var timerData = Xrm.Page.getAttribute("va_calldurationsec").getValue();
    if (timerData) {
        try { timerData = parseInt(timerData); } catch (pe) { }
    }
    _cachedData['TimerData'] = timerData;

    if (Xrm.Page.getAttribute('statecode') && Xrm.Page.getAttribute('statecode').getText() == 'Completed') {
        _cachedData['StopTimer'] = true;
    }

    // collect cached data from xml fields
    if (w && w != undefined && w._appOnCallStarted != null && w._appOnCallStarted != undefined) {
        w._appOnCallStarted(GetEnvironment(), GetUserSettingsForWebservice(exCon), useCache, xmlCachedData);
    }
    //    }
    //    else {
    //        alert('Failed to find VIP entry point.');
    //    }
}

_missingMapping = '';

// handle search for veteran
//TODO: move to shared phone/contact file
function SearchOnClick(unattended_search) {
    // if record is closed, do not search
    if (!(Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_CREATE || Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_UPDATE)) {
        return false;
    }

    if ((Xrm.Page.getAttribute("va_searchtype").getValue() == 1) && (Xrm.Page.getAttribute("va_ssn").getValue() == null | Xrm.Page.getAttribute("va_ssn").getValue() == '') &&
        (Xrm.Page.getAttribute("va_lastname").getValue() == null | Xrm.Page.getAttribute("va_lastname").getValue() == '')
    ) {
        alert('SSN/File No./Claim No. or Last Name fields must have a value for a Search');
        return;
    }

    var vetSearchCtx = new vrmContext(exCon);
    _UserSettings = GetUserSettingsForWebservice(exCon);
    vetSearchCtx.user = _UserSettings;

    var exists = (Xrm.Page.getAttribute("regardingobjectid").getValue() != null &&
		(Xrm.Page.getAttribute("va_calleridentityverified").getValue() || Xrm.Page.getAttribute("va_filessnverified").getValue()));
    var searchChanged = false, searchBySnn = false;
    switch (Xrm.Page.getAttribute("va_searchtype").getValue()) {
        case 2: // pid
            searchChanged = Xrm.Page.getAttribute("va_participantid").getIsDirty();
            break;
        case 1: // ssn
            searchBySnn = true;
            searchChanged = (
                                                                                                  (Xrm.Page.getAttribute("va_ssn").getValue() && Xrm.Page.getAttribute("va_ssn").getIsDirty()) ||
                                                                                                  (!Xrm.Page.getAttribute("va_ssn").getValue() &&
                                                                                                                                   ((Xrm.Page.getAttribute("va_firstname").getValue() && Xrm.Page.getAttribute("va_firstname").getIsDirty()) || (Xrm.Page.getAttribute("va_lastname").getValue() && Xrm.Page.getAttribute("va_lastname").getIsDirty()))
                                                                                                  )
                                                                                                  );
            break;
    }

    // Check "Find AppealsBy condition
    if (Xrm.Page.getAttribute("va_findappealsby").getValue() == 953850002) {
        var searchScore = 0;

        if (Xrm.Page.getAttribute("va_appealsssn").getValue() && Xrm.Page.getAttribute("va_appealsssn").getIsDirty()) searchScore += 3;
        if (Xrm.Page.getAttribute("va_appealslastname").getValue() && Xrm.Page.getAttribute("va_appealslastname").getIsDirty()) searchScore += 2;
        if (Xrm.Page.getAttribute("va_appealsfirstname").getValue() && Xrm.Page.getAttribute("va_appealsfirstname").getIsDirty()) searchScore += 1;
        if (Xrm.Page.getAttribute("va_appealsdateofbirth").getValue() && Xrm.Page.getAttribute("va_appealsdateofbirth").getIsDirty()) searchScore += 1;
        if (Xrm.Page.getAttribute("va_appealscity").getValue() && Xrm.Page.getAttribute("va_appealscity").getIsDirty()) searchScore += 1;
        if (Xrm.Page.getAttribute("va_appealsstate").getValue() && Xrm.Page.getAttribute("va_appealsstate").getIsDirty()) searchScore += 1;

        if (searchScore <= 2) {
            alert('Please specify more parameters to search Appeals by.');
            return false;
        }
    }

    if (exists && searchChanged) {
        if (!confirm("Veteran Identity had been confirmed and Veteran record had already been linked to this call.\nIf you will execute another search, the cached search data for currently linked Veteran will be erased.\n\nIf this call deals with more than one Veteran, click on 'Cancel' button, mark current call record 'Complete' by pressing 'Mark Complete' toolbar button, open a new Phone Call screen and execute a new search for another Veteran from the new screen.\n\nAre you sure you would like to execute a new search?")) {
            return false;
        }
    }

    // reset all search fields
    for (var i = 0; i < _responseAttributes.length; i++) {
        _responseAttributes[i].setValue(null);
    }
    _WebServiceExecutionStatusLists = '';

    // reset the collection where key is xml field name and value is response from the service
    // each service will trigger update to the collection'
    _serviceResultsCollection = [];

    for (var i = 0; i < _responseAttributes.length; i++) {
        _serviceResultsCollection[_responseAttributes[i].getName()] = null;
    }

    _endOfSearchReached = false;

    // translate search selection to bit flags
    TranslateSearchType();

    var ssn = (Xrm.Page.getAttribute("va_ssn").getValue() == null ? '' : Xrm.Page.getAttribute("va_ssn").getValue());
    ssn = ssn.trim();
    ssn = ssn.replace(new RegExp('-', 'g'), '').replace(new RegExp(' ', 'g'), '');
    if (ssn && ssn.length > 0) Xrm.Page.getAttribute('va_ssn').setValue(ssn);

    if (vetSearchCtx.user && vetSearchCtx.user.fileNumber && ssn == vetSearchCtx.user.fileNumber) {
        alert('You do not have permission to search this record because it is your own');
        try { Xrm.Page.ui.close(); } catch (err) { }
        finally { return false; }
    }

    // if search by ssn, reset PID
    if (searchBySnn) {
        Xrm.Page.getAttribute("va_participantid").setValue(null);
    }
    // reset names
    var tempContact = Xrm.Page.getAttribute('va_createcontact');

    if ((ssn && ssn.length > 0) && tempContact.getValue() == 0) {
        Xrm.Page.getAttribute('va_firstname').setValue(null);
        Xrm.Page.getAttribute('va_lastname').setValue(null);
        Xrm.Page.getAttribute('va_middleinitial').setValue(null);
        Xrm.Page.getAttribute("va_email").setValue(null);
        Xrm.Page.getAttribute('va_dobtext').setValue(null);
    }

    // apply search parameters to context
    if (!vetSearchCtx.SetSearchParameters()) { return false; }

    if (_searchCounter == 0) {
        SearchList.clear();
    }

    _VRMMESSAGE = new Array();
    _totalWebServiceExecutionTime = 0;

    Xrm.Page.ui.tabs.get("tab_search").setLabel('Search - VIP ' + _vrmVersion);
    var notificationsArea = document.getElementById('crmNotifications');
    if (notificationsArea != null) {
        // RU12 Compatability issues
        if (typeof notificationsArea.SetNotifications === 'undefined') {
            notificationsArea = $find('crmNotifications');
        }
        notificationsArea.SetNotifications(null, null);
    }

    // set the ECC Search VADIR Flag
    vetSearchCtx.doSearchVadir = Xrm.Page.getAttribute('va_eccphonecall').getValue();

    // handoff to EXTJS 
    _vipSearchContext = vetSearchCtx;

    var w = document.getElementById('IFRAME_search').contentWindow;
    if (w) {
        for (i = 0; i < 200; i++) {
            try {
                if (w._appSearch != undefined) {
                    w._appSearch(vetSearchCtx);
                    i = 200;
                }
            }
            catch (ser) {
                if (!confirm('Error occurred while executing search. Would you like to repeat search attempt?\n\n' + ser.message)) {
                    i = 200;
                }
            }
        }
    }
    else {
        alert('Failed to pass search context to the search container.');
    }

    WebServiceExecutionStatusList.clear();
}

_SearchFunction = SearchOnClick;





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
    //VADIR search always gets stopped here, so now creation of record...
    if (Xrm.Page.getAttribute('va_findpersonresponsevadir').getValue() !== null) {
        var rtContact = executePostSearchOperations(false)
        return;
    }
    if (missingFields) { return; }

    _endOfSearchReached = true;

    Xrm.Page.getAttribute("va_webserviceresponse").setValue(new Date());

    if (_missingMapping.length > 0) {
        // extjs has some left some fields unmapped
        //alert('!! Unmapped cache !!\n\n' + _missingMapping);
        //debugger;
    }

    _CORP_RECORD_COUNT = 1;
    _BIRLS_RECORD_COUNT = 1;

    var rtContact = executePostSearchOperations(true);  // will call Mark as Related
    // no need to call updateSearchResultsSection - it only updates section header
    UpdateContactHistoryGrid();

    // Validate Vets station ID
    var user = GetUserSettingsForWebservice(exCon);
    /*
    if (user && user.stationId && user.stationId == rtContact.BIRLSEmployeeStation) {
    alert("You do not have the permission to view records for persons in your own office (based on BIRLS employee station).\n\nVeteran's Employee Station Number (" + rtContact.BIRLSEmployeeStation + ") is identical to your PCR Station Number as specified on CRM User Settings screen.\nThe Phone Call screen will close.\n\nPlease select 'Discard my changes' option and click on OK button on the next pop-up.");
    try {
    Xrm.Page.ui.close();
    }
    catch (er) { }
    finally { return; }
    }*/
}
_VIPEndOfSearch = VIPEndOfSearch;

// called at the end of each async ws
function VIPEndOfServiceCall(xmlFieldName, success, requestXml, url, response, wsName, wsDuration) {
    //.length causes null pointer....
    _WebServiceExecutionStatusLists += (_WebServiceExecutionStatusLists && _WebServiceExecutionStatusLists.length > 0 ? "," : "") +
                "{'name':'" + wsName + "','executionTime':" + wsDuration + "}";

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
        if (xmlFieldName == 'NOCACHE') { _missingMapping += requestXml + '\n'; }
    }
}
_VIPEndOfServiceCall = VIPEndOfServiceCall;

function ProcessAndReportSearchResults(result) {
    var sensitiveAccessFail = false;
    Xrm.Page.getAttribute('va_pcrsensitivitylevelinsufficient').setValue(false);

    if (!result) {
        var errors = GetErrorMessages('\n', true);
        var onlyErrorIsNoData = true;
        if (_VRMMESSAGE && _VRMMESSAGE.length > 0) {
            for (var i = 0; i < _VRMMESSAGE.length; i++) {
                if (_VRMMESSAGE[i].errorFlag) { if (_VRMMESSAGE[i].nodataFlag != true) { onlyErrorIsNoData = false; break; } }
            }
        }
        var prompt = (!onlyErrorIsNoData ? 'One of the data retrieval web service calls reported an error in the middle-tier system.' : "One of data retrieval services reported 'No data had been found.'");
        var authFail = false;
        if (errors.toUpperCase().indexOf('AUTH_APL') != -1) {
            prompt = 'Authentication failed. Login name and other login attributes might be incorrect or login expired.';
            authFail = true;
            Xrm.Page.ui.tabs.get('tab_search').setLabel('Search - Authentication Failed');
            if (!Xrm.Page.getAttribute('subject').getValue()) Xrm.Page.getAttribute('subject').setValue('Auth Fail');
            if (!Xrm.Page.getAttribute('va_callerlastname').getValue()) {
                Xrm.Page.getAttribute('va_callerfirstname').setValue('Unknown');
                Xrm.Page.getAttribute('va_callerlastname').setValue('Caller');
            }
            if (!Xrm.Page.getAttribute('va_callerrelationtoveteran').getValue()) Xrm.Page.getAttribute('va_callerrelationtoveteran').setValue(953850006);
            if (!Xrm.Page.getAttribute('va_disposition').getValue()) {
                Xrm.Page.getAttribute('va_disposition').setValue(953850007); //Other
                Xrm.Page.getAttribute('va_dispositionsubtype').setValue(953850051); //N/A
            }
        }
        else {
            sensitiveAccessFail = IsSensitiveFileAccessFail();
            if (sensitiveAccessFail) {
                Xrm.Page.getAttribute('va_pcrsensitivitylevelinsufficient').setValue(true);
                Xrm.Page.ui.tabs.get('tab_search').setLabel('Search: Sensitive File - Access Violation!!!');
            }
        }

        var msg = prompt + ' Web Service(s) returned following information:\n\n' + errors;
        alert(msg);
        LogMessages(true, false, false);

        // if sensitive failed, prompt to assign to super
        if (sensitiveAccessFail) AssignSupervisor();

    }
    else {
        var warnings = GetWarningMessages('\n', true);
        var errors = GetErrorMessages('\n', true);
        if ((warnings && warnings.length > 0) || (errors && errors.length > 0)) {
            Xrm.Page.ui.tabs.get('tab_search').setLabel('Search - Warnings/Errors Reported');
            var msg = '';
            if (warnings && warnings.length > 0) msg = 'Warnings:\n' + warnings;
            if (errors && errors.length > 0) msg = (msg.length > 0 ? msg + '\n\n' : '') + 'Errors:\n' + errors;

            if (_logQueries) LogMessages((errors && errors.length > 0 ? true : false), (warnings && warnings.length > 0 ? true : false), false);
            alert('Some of the web services have executed with errors and/or warnings.\n\n' + msg);
        }
        else {
            Xrm.Page.ui.tabs.get('tab_search').setLabel('Search');
            if (_logQueries) LogMessages(false, false, true);
        }
    }

}

function OnError(desc, data) {
    // tab_search
    if (desc && desc.indexOf('\n') >= 0) { desc = desc.substring(0, desc.indexOf('\n') - 1); }
    Xrm.Page.ui.tabs.get("tab_search").setLabel('Search Error Detected: ' + desc);
    var msg = desc;
    if (data == undefined || !data || data.length == 0) { return; }

    // detect SensitiveFileAccessFail
    for (var i = 0; i < data.length; i++) {
        if (data[i].indexOf('SENS1') >= 0) {
            Xrm.Page.getAttribute('va_pcrsensitivitylevelinsufficient').setValue(true);
            //AssignSupervisor('Sensitive Level Violation');
            //msg = 'Sensitive Level Violation';
            break;
        }
    }

    var notificationsArea = document.getElementById('crmNotifications');
    if (notificationsArea != null) {
        // RU12 Compatability issues
        if (typeof notificationsArea.AddNotification === 'undefined') {
            notificationsArea = $find('crmNotifications');
        }
        notificationsArea.AddNotification('mep1', 1, desc, msg);
    }

}
_SearchOnError = OnError;

function ContinueSearch() {
    try {
        if (!Xrm.Page.getAttribute("regardingobjectid").getValue()) { alert('Cannot continue search because initial (CORP Min) search either was not run or it had failed.'); return; }

        SearchOnClick(true);
    }
    catch (e) {
        CloseProgress();
        alert("Error occurred during data retrieval.\n" + e.description + "\n\n" + GetErrorMessages('\n'));
        LogMessages(true, false, false);
    }
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
    var isSelf = false;
    var unknown = false;
    var tab = Xrm.Page.ui.tabs.get("tab_search");
    var section = tab.sections.get("callerdetails");
    if (section && section != undefined) { section.setLabel("Caller and Call Type"); }
    //tab.setLabel('Identify Caller');
    veteranIdentified = (Xrm.Page.getAttribute("regardingobjectid").getValue() != null);

    if (Xrm.Page.getAttribute('va_callerrelationtoveteran').getSelectedOption()) {
        isSelf = (Xrm.Page.getAttribute('va_callerrelationtoveteran').getSelectedOption().text == 'Self');
    }
    if (isSelf) {
        // do not update names durign load event
        if (!_isLoading) {
            if (!veteranIdentified) {
                if (Xrm.Page.getAttribute("va_callerfirstname").getValue())
                    Xrm.Page.getAttribute("va_firstname").setValue(Xrm.Page.getAttribute("va_callerfirstname").getValue());
                else {
                    if (Xrm.Page.getAttribute("va_firstname").getValue()) Xrm.Page.getAttribute("va_callerfirstname").setValue(Xrm.Page.getAttribute("va_firstname").getValue());
                }
                if (Xrm.Page.getAttribute("va_callerlastname").getValue())
                    Xrm.Page.getAttribute("va_lastname").setValue(Xrm.Page.getAttribute("va_callerlastname").getValue());
                else {
                    if (Xrm.Page.getAttribute("va_lastname").getValue()) Xrm.Page.getAttribute("va_callerlastname").setValue(Xrm.Page.getAttribute("va_lastname").getValue());
                }
            }
            else {
                var nameSet = false;
                if (Xrm.Page.getAttribute("va_firstname").getValue()) { Xrm.Page.getAttribute("va_callerfirstname").setValue(Xrm.Page.getAttribute("va_firstname").getValue()); nameSet = true; }
                if (Xrm.Page.getAttribute("va_lastname").getValue()) { Xrm.Page.getAttribute("va_callerlastname").setValue(Xrm.Page.getAttribute("va_lastname").getValue()); nameSet = true; }
                if (nameSet) section.setLabel("Caller and Call Type: First and Last Name are set to Veteran's F/L names from Search Parameters section");
            }
        }

        // get fiduciary and POA information
        var hasPOA = false, hasFid = false;
        var rtContact = new contact();

        var POAFidXml = Xrm.Page.getAttribute('va_findfiduciarypoaresponse').getValue();
        if (POAFidXml) {
            var POAFidXmlObject = _XML_UTIL.parseXmlObject(POAFidXml);
            rtContact.getFidPOAData(POAFidXmlObject);
            var poaArr = rtContact.currentPowerOfAttorney;
            var fidArr = rtContact.currentFiduciary;
            if (poaArr) { hasPOA = true; }
            if (fidArr) { hasFid = true; }
        //} else {
        //    //2015-06-05 VTRIGILI - Throw the alert only once even though this block is called more than once
        //    if (_va_crm_udo_pageAlertMessageCount == 0) {
        //        alert("The webpage did not properly load. Some functions may not work. Please refresh the page to try again.");
        //    }
        //    _va_crm_udo_pageAlertMessageCount++;
        }
        // set caption of CAller details section if self and has POA
        // per Rich, POA doesn't need notice on screen
        if ( /*hasPOA ||*/hasFid) { section.setLabel('Caller and Call Type: ALERT! CALL ABOUT SELF; CALLER HAS FIDUCIARY!'); }
    }
    else {
        var callerRelationToVeteran = (Xrm.Page.getAttribute("va_callerrelationtoveteran").getSelectedOption() == null ? '' : Xrm.Page.getAttribute("va_callerrelationtoveteran").getSelectedOption().text);
        if (callerRelationToVeteran == 'Unknown Caller') { // must check whether object exists first then check text property

            Xrm.Page.getAttribute("va_callerfirstname").setValue('Unknown');
            Xrm.Page.getAttribute("va_callerlastname").setValue('Caller');
            unknown = true;
            section.setLabel("Caller and Call Type: First and Last Name are set to 'Unknown Caller'");
        }
    }


    if (!_isLoading) { Xrm.Page.ui.tabs.get("phonecall").setVisible(!isSelf); }

}

function ShowLocalTime() {
    var tzOption = Xrm.Page.getAttribute("va_timezone").getSelectedOption();
    var displaySection = Xrm.Page.ui.tabs.get('Categorize Call').sections.get('Categorize Call_section_moreinfo');
    var name = 'Additional Information';
    if (!tzOption) { displaySection.setLabel(name); return; }

    var localTime = getLocalTime(tzOption.text);
    if (!localTime) { displaySection.setLabel(name); return; }

    displaySection.setLabel(name + "; Contact's Date & Time: " + localTime);
}

function AddIssue() {
    var val = Xrm.Page.getAttribute("va_numberofdispositions").getValue(); if (!val) val = 1;
    if (val >= 5) { alert('No more than 5 issues are allowed.'); return; }
    if (!confirm('Please confirm that you would like to add an Issue')) return;
    Xrm.Page.getAttribute("va_numberofdispositions").setValue(++val);
    NoOfDispositionsOnClick();

    var control = 'va_disposition' + val.toString();
    switchCallTypeOptions(control);
    Xrm.Page.getControl(control).setFocus();
}
function RemoveIssue() {
    var val = Xrm.Page.getAttribute("va_numberofdispositions").getValue(); if (!val) val = 1;
    if (val <= 1) { alert('At least one issue is required.'); return; }
    if (!confirm('Please confirm that you would like to remove an Issue')) return;
    Xrm.Page.getAttribute("va_numberofdispositions").setValue(--val);
    NoOfDispositionsOnClick();
    var control = 'va_disposition' + (val > 1 ? val.toString() : '');
    Xrm.Page.getControl(control).setFocus();
}

function MoreSearchOptions() {
    var val = false;

    if (Xrm.Page.getAttribute("va_moresearchoptions").getValue()) val = Xrm.Page.getAttribute("va_moresearchoptions").getValue();
    Xrm.Page.ui.tabs.get('tab_search').sections.get('tab_search_section_moreparams').setVisible(val);
    Xrm.Page.ui.tabs.get('tab_search').sections.get('tab_search_section_moreparams_birls').setVisible(val);

    AppealsSearchOnChange('tab_search', 'tab_search_appealsoptions', Xrm.Page.getAttribute("va_moresearchoptions").getValue());

}

function AssignSupervisor(desc) {
    if (Xrm.Page.getAttribute("va_callerrequestedsupervisor").getValue() != true && Xrm.Page.getAttribute("va_pcrsensitivitylevelinsufficient").getValue() != true) {
        alert("Please mark 'Caller Requested Supervisor' and/or 'PCR Sensitivity Level Insufficient' boxes perior to assigning this phone call to Supervisor.");
        return;
    }
    var text = 'This record will be saved and closed and phone call will be assigned to Supervisor queue. Proceed?';
    if (desc && desc != undefined) { text = desc + '\n\n' + text; }
    if (!confirm(text)) { return; }

    Xrm.Page.data.entity.save("saveandclose");
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
    this.par = par;
}

FireCreateSrEvent.prototype.get = function (srData) {
    var w = document.getElementById('IFRAME_search').contentWindow;
    if (w != undefined && w._appFireEvent != undefined) {
        w._appFireEvent('createphonecallservicerequest', srData);
    }
};

function DispositionRequiresServiceRequest(disposition, sub) {
    var res = false;
    var scripts = GetCallScripts(false);
    for (var i = 0; i < scripts.length; i++) {
        if (scripts[i].va_Issue && scripts[i].va_SubIssue && scripts[i].va_Issue.Value == disposition && scripts[i].va_SubIssue.Value == sub) {
            // Exception: if it's FNOD (which requires SR), but FNOD is for Vet, then DO NOT create SR.
            //   in this case, special FNOD screen is used.
            if (_fnod && Xrm.Page.getAttribute('va_fnodreportingfor').getSelectedOption().text == 'Veteran') {
                return null;
            }
            return scripts[i];
        }
    }
    return null;
}

//Validate ID Proofing checks when attempting to initiate a CADD
function ValidateIDProofingForAddressChange() {
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
_ValidateIDProofingForAddressChange = ValidateIDProofingForAddressChange;

//Phone Call ID Proofing
function IdProofingComplete() {
    var attr = [
                                Xrm.Page.getAttribute("va_calleridentityverified"),
                                Xrm.Page.getAttribute("va_filessnverified"),
                                Xrm.Page.getAttribute("va_bosverified")
    ];
    //Xrm.Page.getAttribute("va_fullnameca"), Xrm.Page.getAttribute("va_currentcheckamountcaid"), Xrm.Page.getAttribute("va_maritalstatus")]; 
    for (var a in attr) {
        attr[a].setValue(true);
    }
    if (_isECC == true) {
        Xrm.Page.getAttribute("va_addressverified").setValue(true);

        //Check if Caller Relation to Veteran is School Certifying Official
        if (Xrm.Page.getAttribute("va_callerrelationtoveteran") && Xrm.Page.getAttribute("va_callerrelationtoveteran").getValue() == 953850010) {
            Xrm.Page.getAttribute("va_schoolcertifyingofficial").setValue(true);
        }
    }
    alert('ID proofing boxes have been checked to indicate successful proofing, please proceed with the call.');
}

function ProgressDlg(text) { this.text = text; }

ProgressDlg.prototype.Preload = function () {
    if (!_progressWindow) {
        prepareProgress();
    }
    Minimize();
};

function UpdateContactHistoryGrid() {
    var ssn = Xrm.Page.getAttribute('va_ssn').getValue();
    if (!ssn) ssn = '';
    ssn = ssn.trim();
    ssn = ssn.replace(new RegExp('-', 'g'), '').replace(new RegExp(' ', 'g'), '');

    //Update the fetchXML that will be used by the grid.   
    var fetchXml = '<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="false">' +
  '<entity name="phonecall">' +
                                '<attribute name="subject" />' +
                                '<attribute name="regardingobjectid" />' +
                                '<attribute name="phonenumber" />' +
                                '<attribute name="prioritycode" />' +
                                '<attribute name="scheduledend" />' +
                                '<attribute name="statecode" />' +
                                '<attribute name="ownerid" />' +
                                '<attribute name="va_thisisarepeatcall" />' +
                                '<attribute name="va_email" />' +
                                '<attribute name="actualdurationminutes" />' +
                                '<attribute name="va_disposition" />' +
                                '<attribute name="va_callerlastname" />' +
                                '<attribute name="va_calleridentityverified" />' +
                                '<attribute name="va_callerfirstname" />' +
                                '<attribute name="createdon" />' +
                                '<attribute name="va_ssn" />' +
                                '<attribute name="va_relatedpriorphonecallid" />' +
                                '<attribute name="va_participantid" />' +
                                '<attribute name="va_isaveteran" />' +
                                '<attribute name="va_flags" />' +
                                '<attribute name="va_failedidproofingforchangeofaddress" />' +
                                '<attribute name="va_dispositionsubtype" />' +
                                '<attribute name="directioncode" />' +
                                '<attribute name="va_abusivecall" />' +
                                '<attribute name="va_inquirytype" />' +
                                '<attribute name="va_trainingrequest" />' +
                                '<attribute name="va_specialsituationid" />' +
                                '<attribute name="createdby" />' +
                                '<attribute name="activityid" />' +
                                '<order attribute="createdon" descending="true" />' +
                                '<order attribute="subject" descending="false" />' +
                                '<filter type="and">' +
                                   '<condition attribute="va_ssn" operator="eq" value="' + ssn + '" />' +
                                '</filter>' +
  '</entity>' +
'</fetch>';

    UpdateSubGrid('contacthistory', fetchXml);

}

function UpdateSubGrid(gridName, fetchXml) {
    //var leadGrid = (Xrm.Page.getControl(gridName)) ? Xrm.Page.getControl(gridName).getValue() : null;
    var leadGrid = document.getElementById(gridName);
    //If this method is called from the form OnLoad, make sure that the grid is loaded before proceeding   
    // RU12 check
    if (!leadGrid) {
        setTimeout('UpdateContactHistoryGrid()', 1000);
        return;
    }
    if (leadGrid.readyState != "complete") {
        //The subgrid hasn't loaded, wait 1 second and then try again      
        setTimeout('UpdateContactHistoryGrid()', 1000);
        return;
    }

    //Inject the new fetchXml   
    // RU12 Compatability issues
    if (typeof leadGrid.control.setParameter === 'function') {
        leadGrid.control.setParameter("fetchXml", fetchXml);
    } else {
        leadGrid.control.SetParameter("fetchXml", fetchXml);
    }
    //Force the subgrid to refresh   
    leadGrid.control.refresh();
}

//Call this from CADD on Save
function RedrawCADDfields(pid) {
    var w = document.getElementById('IFRAME_search').contentWindow;
    if (w) {
        try {
            w._appFireEvent('redrawcaddfields', pid);
        } catch (ser) { }
    }
}
_RedrawCADDfields = RedrawCADDfields;

function SelectPathwaysSearch() {
    PathwaysSearchOnChange('tab_search', 'tab_search_pathwayoptions', Xrm.Page.getAttribute("va_searchcorpall").getValue());
}

function formatTelephone(telephoneField) {
    var Phone = Xrm.Page.getAttribute(telephoneField).getValue();
    var ext = '';
    var result;
    if (Phone != null) {
        if (0 != Phone.indexOf('+')) {
            if (1 < Phone.lastIndexOf('x')) {
                ext = Phone.slice(Phone.lastIndexOf('x'));
                Phone = Phone.slice(0, Phone.lastIndexOf('x'));
            }

            Phone = Phone.replace(/[^\d]/gi, '');
            result = Phone;
            if (7 == Phone.length) {
                result = Phone.slice(0, 3) + '-' + Phone.slice(3)
            }
            if (10 == Phone.length) {
                result = '(' + Phone.slice(0, 3) + ') ' + Phone.slice(3, 6) + '-' + Phone.slice(6);
            }
            if (11 == Phone.length) {
                result = Phone.slice(0, 1) + ' (' + Phone.slice(1, 4) + ') ' + Phone.slice(4, 7) + '-' + Phone.slice(7);
            }
            if (0 < ext.length) {
                result = result + ' ' + ext;
            }
            Xrm.Page.getAttribute(telephoneField).setValue(result);
        }
    }
}

function onChangeDispostionPostEvent() {
    callTypeFnodChange();
}

function callTypeFnodChange() {
    if (Xrm.Page.getAttribute("va_disposition").getValue() === 953850004) {
        //Set Call Subtype to Death Checklist
        Xrm.Page.getAttribute("va_dispositionsubtype").setValue(953850034);
        //Set FNOD reporting for: Veteran
        Xrm.Page.getAttribute("va_fnodreportingfor").setValue(953850000);
    }
    else {
        //Set them to null
        Xrm.Page.getAttribute("va_dispositionsubtype").setValue(null);
        Xrm.Page.getAttribute("va_fnodreportingfor").setValue(null);
    }
}

function CheckCoBrowseIndicator() {
    var myAttribute1 = "va_cobrowsesessionindicator";
    var myControl1 = Xrm.Page.ui.controls.get(myAttribute1);
    var myCheckboxValue1 = myControl1.getAttribute().getValue();

    if (myCheckboxValue1 == false) {
        $('#va_cobrowsesessionid').val('').prop('disabled', true);
    }
    else {
        $('#va_cobrowsesessionid').prop('disabled', false);
    }
}

function removeCallTypeValues() {
    var FORM_TYPE_CREATE = 1;
    var FORM_TYPE_UPDATE = 2;
    var FORM_TYPE_READ_ONLY = 3;
    var FORM_TYPE_DISABLED = 4;
    var FORM_TYPE_QUICK_CREATE = 5;
    var FORM_TYPE_BULK_EDIT = 6;
    var FORM_TYPE_READ_OPTIMIZED = 11;

    var formType = Xrm.Page.ui.getFormType();

    var callType = Xrm.Page.getControl('va_disposition');

    if (formType == FORM_TYPE_CREATE) {
        callType.removeOption(953850005); // General Questions
        callType.removeOption(953850007); // Other
    }
    else {
        if (Xrm.Page.getAttribute('va_disposition').getValue() != 953850005) {
            callType.removeOption(953850005); // General Questions
        }
        if (Xrm.Page.getAttribute('va_disposition').getValue() != 953850007) {
            callType.removeOption(953850007); // Other
        }
    }
}

function removeIssueValues() {
    var FORM_TYPE_CREATE = 1;
    var FORM_TYPE_UPDATE = 2;
    var FORM_TYPE_READ_ONLY = 3;
    var FORM_TYPE_DISABLED = 4;
    var FORM_TYPE_QUICK_CREATE = 5;
    var FORM_TYPE_BULK_EDIT = 6;
    var FORM_TYPE_READ_OPTIMIZED = 11;

    var formType = Xrm.Page.ui.getFormType();

    var issue2 = Xrm.Page.getControl('va_disposition2');
    var issue3 = Xrm.Page.getControl('va_disposition3');
    var issue4 = Xrm.Page.getControl('va_disposition4');
    var issue5 = Xrm.Page.getControl('va_disposition5');

    if (formType == FORM_TYPE_CREATE) {
        issue2.removeOption(953850005); // General Questions
        issue2.removeOption(953850007); // Other
        issue3.removeOption(953850005); // General Questions
        issue3.removeOption(953850007); // Other
        issue4.removeOption(953850005); // General Questions
        issue4.removeOption(953850007); // Other
        issue5.removeOption(953850005); // General Questions
        issue5.removeOption(953850007); // Other
    }
    else {
        if (Xrm.Page.getAttribute('va_disposition2').getValue() != 953850005) {
            issue2.removeOption(953850005); // General Questions
        }
        if (Xrm.Page.getAttribute('va_disposition2').getValue() != 953850007) {
            issue2.removeOption(953850007); // Other
        }
        if (Xrm.Page.getAttribute('va_disposition3').getValue() != 953850005) {
            issue3.removeOption(953850005); // General Questions
        }
        if (Xrm.Page.getAttribute('va_disposition3').getValue() != 953850007) {
            issue3.removeOption(953850007); // Other
        }
        if (Xrm.Page.getAttribute('va_disposition4').getValue() != 953850005) {
            issue4.removeOption(953850005); // General Questions
        }
        if (Xrm.Page.getAttribute('va_disposition4').getValue() != 953850007) {
            issue4.removeOption(953850007); // Other
        }
        if (Xrm.Page.getAttribute('va_disposition5').getValue() != 953850005) {
            issue5.removeOption(953850005); // General Questions
        }
        if (Xrm.Page.getAttribute('va_disposition5').getValue() != 953850007) {
            issue5.removeOption(953850007); // Other
        }
    }
}

function confirm0820Statement() {
    if (Xrm.Page.getAttribute('va_disposition').getValue() == 953850032) {
        var isOK = confirm('I am a VA employee who is authorized to receive or request evidentiary information or statements that may result in a change in your VA benefits. The primary purpose for gathering this information or statement is to make an eligibility determination. It is subject to verification through computer matching programs with other agencies.');

        if (isOK) {
            Xrm.Page.getAttribute("va_noastatement").setValue(true);
            //Xrm.Page.getAttribute("va_noadisagree").setValue(false);

            var navItem = Xrm.Page.ui.navigation.items.get('nav_crme_phonecall_crme_dependentmaintenance_RegardingPhoneCall');
            if (navItem != null) {
                navItem.setVisible(isAddDependentEnabled_LeftNav());
            }

            //var btn = top.document.getElementById('phonecall|NoRelationship|Form|DependentMaintenance-Large');
            //if (btn != null) {
            //btn.disabled = !isAddDependentEnabled();
            //}

            Xrm.Page.ui.refreshRibbon();
        }
        else {
            Xrm.Page.getAttribute("va_noastatement").setValue(false);
            //Xrm.Page.getAttribute("va_noadisagree").setValue(true);

            var navItem = Xrm.Page.ui.navigation.items.get('nav_crme_phonecall_crme_dependentmaintenance_RegardingPhoneCall');
            if (navItem != null) {
                navItem.setVisible(false);
            }

            //var btn = top.document.getElementById('phonecall|NoRelationship|Form|DependentMaintenance-Large');
            //if (btn != null) {
            //btn.disabled = true;
            //}

            Xrm.Page.ui.refreshRibbon();

        }
    }
    else {
        //Xrm.Page.getAttribute("va_noastatement").setValue(false);
    }
}
