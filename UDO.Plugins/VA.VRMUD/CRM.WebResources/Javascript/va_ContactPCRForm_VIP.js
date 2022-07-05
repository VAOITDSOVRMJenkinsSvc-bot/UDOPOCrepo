/// <reference path="XrmPage-vsdoc.js" />
// This method will be call from CRM form
function OnLoad() {
    environmentConfigurations.initalize();
    commonFunctionsVip.initalize();
    // RU12 form
    onFormLoad();
}
function onFormLoad() {
    _isLoading = true;
    try { window.top.moveTo(1, 1); window.top.resizeTo(screen.availWidth - 1, screen.availHeight - 1); } catch (err) { }
    defineResponseAttributes();

    var mainResponseExists = (Xrm.Page.getAttribute("va_generalinformationresponse").getValue() && Xrm.Page.getAttribute("va_generalinformationresponse").getValue().length > 0);

    var rtContact = null;
    if (Xrm.Page.ui.getFormType() != CRM_FORM_TYPE_CREATE && mainResponseExists) {
        rtContact = executePostSearchOperations(true);

        if (rtContact && rtContact.fileNumber && rtContact.fileNumber.length > 0) {
            var user = GetUserSettingsForWebservice();

            if (user && user.fileNumber && rtContact.fileNumber == user.fileNumber) {
                alert('You do not have permission to view this record because it is your own');

                //                Xrm.Page.ui.tabs.forEach(function (tab, index) {
                //                    tab.sections.forEach(function (section, index) {
                //                        section.controls.forEach(function (control, index) {
                //                            try {control.setVisible(false);} catch (er) { }
                //                        });
                //                        section.setVisible(false);
                //                    });
                //                    tab.setVisible(false);
                //                });

                //                Xrm.Page.ui.navigation.items.forEach(function (item, index) {
                //                    try { item.setVisible(false); }catch (er) { }
                //                });

                try { Xrm.Page.ui.close(); }
                catch (er) { }
                finally { return; }
            }
            /*
            if (user && user.stationId && user.stationId == rtContact.BIRLSEmployeeStation) {
            alert('You do not have permission to view records for persons in your office (based on BIRLS employee station).');
            try {
            Xrm.Page.ui.close();
            }
            catch (er) { }
            finally { return; }
            }*/
        }
    }

    Xrm.Page.getAttribute('va_searchpathways').addOnChange(function () { // fireOnChange no longer needed as of RU12

        var checked = Xrm.Page.getAttribute("va_searchpathways").getValue();
        PathwaysSearchOnChange('tab_search', 'tab_search_pathwayoptions', checked);

    });
    var editable = (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_CREATE || Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_UPDATE);

    if (editable && (!Xrm.Page.getAttribute("va_searchtype").getValue() || parseInt(Xrm.Page.getAttribute("va_searchtype").getValue()) < 1)) {
        Xrm.Page.getAttribute("va_searchtype").setValue(1);
    }
    webResourceUrl = parent.Xrm.Page.context.getServerUrl() + '/WebResources/va_';

    window.GeneralToolbar = new InlineToolbar("va_search");
    GeneralToolbar.AddButton("btnConv", "Search ('Enter' to run)", "100%", SearchOnClick, webResourceUrl + 'find.png');


    Xrm.Page.getAttribute('va_searchtype').addOnChange(SearchTypeChange);

    Xrm.Page.getAttribute('va_moresearchoptions').addOnChange(MoreSearchOptions); // fireOnChange no longer needed as of RU12

    Xrm.Page.getAttribute('va_searchcorpall').addOnChange(SelectPathwaysSearch); // fireOnChange no longer needed as of RU12

    Xrm.Page.getAttribute('va_searchpathways').addOnChange(function () {

        var checked = Xrm.Page.getAttribute("va_searchpathways").getValue();
        PathwaysSearchOnChange('tab_search', 'tab_search_pathwayoptions', checked);

    });

    Xrm.Page.getAttribute('va_searchpathways').addOnChange(function () {
        var userSelection = Xrm.Page.getAttribute("va_findappealsby").getValue();
        //'These Values' == 953850002
        Xrm.Page.ui.controls.get('va_appealsssn').setVisible((userSelection == 953850002));
        Xrm.Page.ui.controls.get('va_appealslastname').setVisible((userSelection == 953850002));
        Xrm.Page.ui.controls.get('va_appealsfirstname').setVisible((userSelection == 953850002));
        Xrm.Page.ui.controls.get('va_appealsdateofbirth').setVisible((userSelection == 953850002));
        Xrm.Page.ui.controls.get('va_appealscity').setVisible((userSelection == 953850002));
        Xrm.Page.ui.controls.get('va_appealsstate').setVisible((userSelection == 953850002));
    });


    Xrm.Page.getAttribute('va_timezone').addOnChange(ShowLocalTime);

    ShowLocalTime();

    SearchTypeChange();
    MoreSearchOptions();
    TranslateSearchType();
    SelectPathwaysSearch();
    PathwaysSearchOnChange('tab_search', 'tab_search_pathwayoptions', false);
    AppealsSearchOnChange('tab_search', 'tab_search_appealsoptions', false);

    Xrm.Page.getControl('va_veteransensitivitylevel').setDisabled(true);
    Xrm.Page.getAttribute('va_veteransensitivitylevel').setSubmitMode('always');

    Xrm.Page.getControl('va_sensitivitylevelvalue').setDisabled(true);
    Xrm.Page.getAttribute('va_sensitivitylevelvalue').setSubmitMode('always');

    var doLoadCache = false;

    var autoSearch = (Xrm.Page.getAttribute("va_autosearch").getValue() == true);
    //debugger;
    if (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_UPDATE || Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_CREATE) {
        var ssn = Xrm.Page.getAttribute("va_ssn").getValue();
        if ((!ssn || ssn.length == 0) && Xrm.Page.getAttribute("va_searchtype").getValue() == 1) {
            var pid = Xrm.Page.getAttribute("va_participantid").getValue();
            if (pid && pid.length > 0) {
                Xrm.Page.getAttribute("va_searchtype").setValue(2);
                SearchTypeChange();
            }
        }
    }

    if (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_UPDATE && mainResponseExists) {
        /*
        var callDate = '';
        var lastCall = Xrm.Page.getAttribute("va_webserviceresponse").getValue();
        if (lastCall) callDate = '\n\nThe data was last refreshed on ' + lastCall.format("MM/dd/yyyy hh:mm");
        var promptText = "Contact history for this record contains cached data. Would you like to load Veteran's information cached in CRM without refreshing it from the system-of-record servers?" + callDate;
        if (autoSearch) { promptText += '\n\nIf you select Cancel, new search will be executed automatically.'; }
        if (confirm(promptText)) {
        _CORP_RECORD_COUNT = 1;
        _BIRLS_RECORD_COUNT = 1;
        doLoadCache = true;
        autoSearch = false;
        }
        else {
        // check autosearch
        if (autoSearch) {
        SearchOnClick();
        }
        }
        */
        SearchOnClick();
    }
    else {
        if (Xrm.Page.getAttribute("va_autosearch").getValue() == true) {
            SearchOnClick();
        }
    }

    for (var i = 0; i < _responseAttributes.length; i++) {
        Xrm.Page.getControl(_responseAttributes[i].getName()).setDisabled(false);

        if (!_allowToCashXMLResponses)
            _responseAttributes[i].setValue(null);
        else
            _responseAttributes[i].setSubmitMode('always');
    }

    $('form').bind('keypress', function (e) {
        if (e.keyCode == 13) {
            SearchOnClick();
        }
    });


    if (doLoadCache && !autoSearch) {
        LoadCachedDataVIP();
    }

    _isLoading = false;

}

function SearchTypeChange() {
    //var ediOption = !Xrm.Page.getAttribute("va_searchby").getValue();
    var option = Xrm.Page.getAttribute("va_searchtype").getValue();
    //debugger
    if (!option) option = 1;

    switch (parseInt(option)) {
        case 2:
            Xrm.Page.ui.tabs.get('tab_search').sections.get('general_section_id').setVisible(false);
            Xrm.Page.ui.tabs.get('tab_search').sections.get('searchparams_edipi').setVisible(false);
            Xrm.Page.ui.tabs.get('tab_search').sections.get('tab_search_section_moreparams').setVisible(false);
            Xrm.Page.ui.tabs.get('tab_search').sections.get('tab_search_section_pid').setVisible(true);
            break;
        case 3:
            Xrm.Page.ui.tabs.get('tab_search').sections.get('general_section_id').setVisible(false);
            Xrm.Page.ui.tabs.get('tab_search').sections.get('searchparams_edipi').setVisible(true);
            Xrm.Page.ui.tabs.get('tab_search').sections.get('tab_search_section_moreparams').setVisible(false);
            Xrm.Page.ui.tabs.get('tab_search').sections.get('tab_search_section_pid').setVisible(false);
            break;
        case 1:
        default:
            if (!_isLoading) { Xrm.Page.ui.tabs.get('tab_search').sections.get('general_section_id').setVisible(true); }
            MoreSearchOptions();
            Xrm.Page.ui.tabs.get('tab_search').sections.get('searchparams_edipi').setVisible(false);
            Xrm.Page.ui.tabs.get('tab_search').sections.get('tab_search_section_pid').setVisible(false);
            break;
    }
}

function ShowLocalTime() {
    var tzOption = Xrm.Page.getAttribute("va_timezone").getSelectedOption();
    var displaySection = Xrm.Page.ui.tabs.get('tab_vetaddress').sections.get('tab_vetaddress_section_contactinfo');
    if (!tzOption) { displaySection.setLabel('Name'); return; }

    var localTime = getLocalTime(tzOption.text);
    if (!localTime) { displaySection.setLabel('Name'); return; }

    displaySection.setLabel("Contact's Date & Time: " + localTime);
}

function MoreSearchOptions() {
    var val = false;

    if (Xrm.Page.getAttribute("va_moresearchoptions").getValue()) val = Xrm.Page.getAttribute("va_moresearchoptions").getValue();
    Xrm.Page.ui.tabs.get('tab_search').sections.get('tab_search_section_moreparams').setVisible(val);
    Xrm.Page.ui.tabs.get('tab_search').sections.get('tab_search_section_moreparams_birls').setVisible(val);

    AppealsSearchOnChange('tab_search', 'tab_search_appealsoptions', Xrm.Page.getAttribute("va_moresearchoptions").getValue());

}
function SelectPathwaysSearch() {
    PathwaysSearchOnChange('tab_search', 'tab_search_pathwayoptions', Xrm.Page.getAttribute("va_searchcorpall").getValue());
}

// load cached data
function LoadCachedDataVIP() {
    var w = document.getElementById('IFRAME_ro').contentWindow;

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
    _cachedData['UserSettings'] = GetUserSettingsForWebservice();
    _cachedData['UseCache'] = useCache;
    _cachedData['Cache'] = xmlCachedData;
    /*
    var timerData = Xrm.Page.getAttribute("va_calldurationsec").getValue();
    if (timerData) {
    try { timerData = parseInt(timerData); } catch (pe) { }
    }
    */
    _cachedData['TimerData'] = 0;

    if (Xrm.Page.getAttribute('statecode') && Xrm.Page.getAttribute('statecode').getText() == 'Completed') {
        _cachedData['StopTimer'] = true;
    }

    // collect cached data from xml fields
    if (w && w != undefined && w._appOnCallStarted != null && w._appOnCallStarted != undefined) {
        w._appOnCallStarted(GetEnvironment(), GetUserSettingsForWebservice(), useCache, xmlCachedData);
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
        return;
    }

    var vetSearchCtx = new vrmContext();
    _UserSettings = GetUserSettingsForWebservice();
    vetSearchCtx.user = _UserSettings;

    var searchChanged = false;
    switch (Xrm.Page.getAttribute("va_searchtype").getValue()) {
        case "2": // pid
            searchChanged = Xrm.Page.getAttribute("va_participantid").getIsDirty();
            break;
        case "1": // ssn
            searchChanged = (
						  (Xrm.Page.getAttribute("va_ssn").getValue() && Xrm.Page.getAttribute("va_ssn").getIsDirty()) ||
						  (!Xrm.Page.getAttribute("va_ssn").getValue() &&
								   ((Xrm.Page.getAttribute("va_firstname").getValue() && Xrm.Page.getAttribute("va_firstname").getIsDirty()) || (Xrm.Page.getAttribute("va_lastname").getValue() && Xrm.Page.getAttribute("va_lastname").getIsDirty()))
						  )
						  );
            break;
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

    Xrm.Page.getControl('firstname').setFocus();
    Xrm.Page.getControl('va_ssn').setFocus();

    var ssn = Xrm.Page.getAttribute("va_ssn").getValue();
    ssn = (ssn) ? ssn.trim() : '';
    ssn = ssn.replace(new RegExp('-', 'g'), '').replace(new RegExp(' ', 'g'), '');
    if (ssn && ssn.length > 0) Xrm.Page.getAttribute('va_ssn').setValue(ssn);

    if (vetSearchCtx.user && vetSearchCtx.user.fileNumber && ssn == vetSearchCtx.user.fileNumber) {
        alert('You do not have permission to search this record because it is your own');
        try { Xrm.Page.ui.close(); } catch (err) { }
    }

    // apply search parameters to context
    if (!vetSearchCtx.SetSearchParameters()) { return; }

    if (_searchCounter == 0) {
        SearchList.clear();
    }

    _VRMMESSAGE = new Array();
    _totalWebServiceExecutionTime = 0;

    Xrm.Page.ui.tabs.get("tab_search").setLabel('Search - VIP ' + _vrmVersion);
    var notificationsArea = document.getElementById('crmNotifications');
    if (notificationsArea != null) {
        // RU12 Compatability issues crmNotifications
        if (typeof notificationsArea.SetNotifications === 'undefined') {
            notificationsArea = $find('crmNotifications');
        }
        notificationsArea.SetNotifications(null, null);
    }

    // set the ECC Search VADIR Flag
    vetSearchCtx.doSearchVadir = Xrm.Page.getAttribute('va_eccphonecall').getValue();

    // handoff to EXTJS 
    _vipSearchContext = vetSearchCtx;

    var w = document.getElementById('IFRAME_ro').contentWindow;
    if (w) {
        for (var i = 0; i < 200; i++) {
            try {
                w._appSearch(vetSearchCtx);
                i = 200;
            } catch (ser) { }
        }
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
    if (missingFields) { return; }

    _endOfSearchReached = true;

    Xrm.Page.getAttribute("va_webserviceresponse").setValue(new Date());

    if (_missingMapping.length > 0) {
        // extjs has some left some fields unmapped
        //alert('!! Unmapped cache !!\n\n' + _missingMapping);
        //debugger;
    }

    // todo: get counts
    _CORP_RECORD_COUNT = 1;
    _BIRLS_RECORD_COUNT = 1;

    var rtContact = executePostSearchOperations(true);  // will call Mark as Related
    // Validate Vets station ID
    var user = GetUserSettingsForWebservice();
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

            if (xmlFieldName == 'va_findgetdocumentlist') {
                var xml = response;
                xmlObject = _XML_UTIL.parseXmlObject(xml);
                xmlObject.selectNodes('//Envelope/Body/GetDocumentListResponse/dcmntRecord[rstrcdDcmntInd = "Y"]').removeAll();

                Xrm.Page.getAttribute(xmlFieldName).setValue(xmlObject.xml);
            }
            else if (xmlFieldName == 'va_findpaymenthistoryresponse') {
                var $xml = $($.parseXML(newVal));
                $xml.find('addressAccntNum').text('**********');
                $xml.find('addressZipCode').text('**********');
                $xml.find('addressLine1').text('**********');

                var xmlString = undefined;
                if (window.ActiveXObject) {
                    xmlString = $xml[0].xml;
                }

                if (xmlString === undefined) {
                    xmlString = (new XMLSerializer()).serializeToString($xml[0]);
                }

                Xrm.Page.getAttribute(xmlFieldName).setValue(xmlString);
            }
            else if (xmlFieldName == 'va_findgetdocumentlist') {
                var xml = response;
                xmlObject = _XML_UTIL.parseXmlObject(xml);
                xmlObject.selectNodes('//Envelope/Body/GetDocumentListResponse/dcmntRecord[rstrcdDcmntInd = "Y"]').removeAll();

                Xrm.Page.getAttribute(xmlFieldName).setValue(xmlObject.xml);

            }
            else if (xmlFieldName == 'va_retrievepaymentsummaryresponse') {
                var xml = response;
                xmlObject = _XML_UTIL.parseXmlObject(xml);
                var payNode = xmlObject.selectSingleNode('//PaymentSummaryResponse/payments');
                if ((payNode) && (payNode.hasChildNodes)) {
                    var va_crn = Xrm.Page.getAttribute('va_crn').getValue();

                    for (var i = 0; i < payNode.childNodes.length; i++) {
                        if (payNode.childNodes[i].selectSingleNode('addressEFT/routingNumber') != null) {
                            var routingNumber = payNode.childNodes[i].selectSingleNode('addressEFT/routingNumber').text;

                            if (routingNumber != null && routingNumber == va_crn) {
                                var len = routingNumber.length - 4;
                                var masked = '';
                                for (ii = 0; ii < len; ii++) {
                                    masked += '*';
                                }
                                masked += routingNumber.substr(len, 4);
                                routingNumber = masked;

                                payNode.childNodes[i].selectSingleNode('addressEFT/routingNumber').text = routingNumber;
                                payNode.childNodes[i].selectSingleNode('addressEFT/accountNumber').text = '**********';
                            }
                        }
                    }
                }

                Xrm.Page.getAttribute(xmlFieldName).setValue(xmlObject.xml);

                //var $xml = $($.parseXML(newVal));
                //$xml.find('accountNumber').text('**********');
                //$xml.find('routingNumber').text('**********');

                //var xmlString = undefined;
                //if (window.ActiveXObject) {
                //    xmlString = $xml[0].xml;
                //}

                //if (xmlString === undefined) {
                //    xmlString = (new XMLSerializer()).serializeToString($xml[0]);
                //}

                //Xrm.Page.getAttribute(xmlFieldName).setValue(xmlString);
            }
            else {
                Xrm.Page.getAttribute(xmlFieldName).setValue(newVal);
            }
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