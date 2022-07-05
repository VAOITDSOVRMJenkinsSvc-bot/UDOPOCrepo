// This method will be call from CRM form
function OnLoad() {
    environmentConfigurations.initalize();
    commonFunctions.initalize();
    ws.vetRecord.initalize();
    ws.claimant.initalize();
    ws.benefitClaim.initalize();
    ws.shareStandardData.initalize();
    // RU12 Form
    onFormLoad(arguments[0]);
    Xrm.Page.getControl('va_findcorprecordresponse').setDisabled(false);
    Xrm.Page.getControl('va_generalinformationresponsebypid').setDisabled(false);
    Xrm.Page.getControl('va_synccorpandbirlsresponse').setDisabled(false);
    Xrm.Page.getControl('va_fnodrequest').setDisabled(false);
    Xrm.Page.getControl('va_fnodresponse').setDisabled(false);
    Xrm.Page.getControl('va_findmonthofdeathrequest').setDisabled(false);
    Xrm.Page.getControl('va_findmonthofdeathresponse').setDisabled(false);
    Xrm.Page.getControl('va_updatemonthofdeathrequest').setDisabled(false);
    Xrm.Page.getControl('va_updatemonthofdeathresponse').setDisabled(false);
    Xrm.Page.getControl('va_findpmcrequest').setDisabled(false);
    Xrm.Page.getControl('va_findpmcresponse').setDisabled(false);
    Xrm.Page.getControl('va_insertpmcrequest').setDisabled(false);
    Xrm.Page.getControl('va_insertpmcresponse').setDisabled(false);
}
var webResourceUrl = null, parent_page = null, page_opener = null,
	spouse_action_options = null, //to save dropdown options in case things get removed and need to be refreshed
	_doHaveCorpRecord = false,
	_ranFnodThisTime = false,
	_modPrompt = false;

function onFormLoad(executionObj) {

    webResourceUrl = parent.Xrm.Page.context.getServerUrl() + '/WebResources/va_';

    if (window.parent && window.parent.opener && window.parent.opener.Xrm.Page) {
        parent_page = window.parent.opener.Xrm.Page;
        page_opener = window.parent.opener;
        if (!parent_page.data && window.top && window.top.opener && window.top.opener.parent) {
            parent_page = window.top.opener.parent.Xrm.Page;
            page_opener = window.top.opener;
        }
    }

    if (!parent_page || !parent_page.getAttribute('va_ssn')) {
        alert("FNOD/MOD/PMC can only be opened from the Phone Call page.");
        return;
    }
    var getA = Xrm.Page.getAttribute;

    getA('va_filenumber').setSubmitMode('always');
    getA('va_ranfnod').setSubmitMode('always');
    getA('va_ransync').setSubmitMode('always');
    getA('va_ranpmc').setSubmitMode('always');
    getA('va_ranmod').setSubmitMode('always');
    getA('va_ranfindmod').setSubmitMode('always');
    getA('va_modeligible').setSubmitMode('always');
    getA('va_createdsr').setSubmitMode('always');
    getA('va_fnodrequeststatus').setSubmitMode('always');
    getA('va_pmcrequeststatus').setSubmitMode('always');
    getA('va_modrequeststatus').setSubmitMode('always');
    getA('va_veteranhassurvivingspouse').setSubmitMode('always');
    getA('va_survivingspouseisvalidformod').setSubmitMode('always');
    getA('va_typeofnotice').setSubmitMode('always');
    getA('va_spouserecordaction').setSubmitMode('always');
    getA('va_webserviceresponse').setSubmitMode('always');
    getA('va_findcorprecordresponse').setSubmitMode('always');
    getA('va_generalinformationresponsebypid').setSubmitMode('always');
    getA('va_fnodrequest').setSubmitMode('always');
    getA('va_fnodresponse').setSubmitMode('always');
    getA('va_findmonthofdeathrequest').setSubmitMode('always');
    getA('va_findmonthofdeathresponse').setSubmitMode('always');
    getA('va_updatemonthofdeathrequest').setSubmitMode('always');
    getA('va_synccorpandbirlsresponse').setSubmitMode('always');
    getA('va_updatemonthofdeathresponse').setSubmitMode('always');
    getA('va_findpmcrequest').setSubmitMode('always');
    getA('va_findpmcresponse').setSubmitMode('always');
    getA('va_insertpmcrequest').setSubmitMode('always');
    getA('va_insertpmcresponse').setSubmitMode('always');
    //getA('va_fnodresults').setSubmitMode('always');
    //getA('va_modresults').setSubmitMode('always');

    //"Hidden" fields
    getA('va_modprocesstype').setSubmitMode('always');
    getA('va_modsoj').setSubmitMode('always');
    getA('va_modtranid').setSubmitMode('always');
    getA('va_modvetptcpntid').setSubmitMode('always');
    getA('va_modspouseptcpntid').setSubmitMode('always');

    //Spouse Theory
    getA('va_spousefirstname').setSubmitMode('always');
    getA('va_spousemiddlename').setSubmitMode('always');
    getA('va_spouselastname').setSubmitMode('always');
    getA('va_spousesuffix').setSubmitMode('always');
    getA('va_spousessn').setSubmitMode('always');
    getA('va_spousedob').setSubmitMode('always');
    getA('va_spouseaddress2').setSubmitMode('always');
    getA('va_spouseaddress3').setSubmitMode('always');
    getA('va_spousecity').setSubmitMode('always');
    getA('va_spousestatelist').setSubmitMode('always');
    getA('va_spousezipcode').setSubmitMode('always');
    getA('va_spousecountry').setSubmitMode('always');
    getA('va_spousecountrylist').setSubmitMode('always');
    getA('va_spouseaddresstype').setSubmitMode('always');
    getA('va_spouseoverseasmilitarypostalcode').setSubmitMode('always');
    getA('va_spouseoverseasmilitarypostofficetypecode').setSubmitMode('always');
    getA('va_spouseforeignpostalcode').setSubmitMode('always');
    getA('va_provincename').setSubmitMode('always');
    getA('va_territoryname').setSubmitMode('always');

    //Treasury fields
    getA('va_treasuryaddress1').setSubmitMode('always');
    getA('va_treasuryaddress2').setSubmitMode('always');
    getA('va_treasuryaddress3').setSubmitMode('always');
    getA('va_treasuryaddress4').setSubmitMode('always');
    getA('va_treasuryaddress5').setSubmitMode('always');
    getA('va_treasuryaddress6').setSubmitMode('always');

    getA("va_name").setValue('FNOD/MOD/PMC Request');

    window.GeneralToolbar = new InlineToolbar("va_executefnod");
    GeneralToolbar.AddButton("btnFNOD", "Submit FNOD (irreversible)", "100%", ExecuteFNOD, "");

    window.GeneralToolbar = new InlineToolbar("va_executepmc");
    GeneralToolbar.AddButton("btnNewPMC", "Issue PMC", "100%", IssuePMC, "");

    window.GeneralToolbar = new InlineToolbar("va_copypmcfields");
    GeneralToolbar.AddButton("btnCopyPMC", "Copy Existing to New (optional)", "100%", CopyPMC, "");

    window.GeneralToolbar = new InlineToolbar("va_executemod");
    GeneralToolbar.AddButton("btnMOD", "Submit MOD/NOK/LETTERS", "100%", ExecuteMOD, "");

    window.GeneralToolbar = new InlineToolbar("va_validatemodeligibility");
    GeneralToolbar.AddButton("btnValidateMOD", "Validate MOD Eligibility", "100%", ValidateMOD, "");

    window.GeneralToolbar = new InlineToolbar("va_generatetreasuryaddress");
    GeneralToolbar.AddButton("btnGenerateTreasuryAddress", "Generate/Update Treasury Address", "100%", updateTreasuryFields, "");

    window.GeneralToolbar = new InlineToolbar("va_validatemodfields");
    GeneralToolbar.AddButton("btnValidateMODFields", "Validate MOD Address", "100%", ValidateMODAddress, "");

    window.GeneralToolbar = new InlineToolbar("va_copylastknownaddress");
    GeneralToolbar.AddButton("btnCopyLastKnownAddress", "Copy from Last Known Address", "100%", CopyLastKnownAddressToSpouseFields, "");

    window.GeneralToolbar = new InlineToolbar("va_retrieveinfo");
    GeneralToolbar.AddButton("btnUpdateVetData", "Refresh Veteran's Data", "100%", RetrieveVetInfo, "");

    var vetSearchCtx = new vrmContext();
    _UserSettings = GetUserSettingsForWebservice();
    vetSearchCtx.user = _UserSettings;

    GetCountryList(vetSearchCtx);

    var onSpouseCountryListChange = function () {
        var varMyValue = $("#va_spousecountrylist option:selected");
        if (varMyValue != null) {
            var cName = varMyValue.text();
            // Defect 96137 For Country code, other than USA, use the full name but make it Upper and lower e.g. France or Sri Lanka
            if (cName && cName.length > 0 && cName != 'US' && cName != 'USA') {
                cName = NormalizeCountry(cName);
            } // cName.toString().toUpperCase(); }
            Xrm.Page.getAttribute("va_spousecountry").setValue(cName);
        }
    };

    var onSpouseRecordActionChange = function () {

        var mode = '', selection = (Xrm.Page.getAttribute("va_spouserecordaction").getText() == null ? '' : Xrm.Page.getAttribute("va_spouserecordaction").getText());


        switch (selection) {
            case 'Add New Spouse':
                mode = 'Add';
                break;
            case 'Modify Existing Spouse':
                mode = 'Modify';
                break;
            case 'Send Next of Kin Letter':
                mode = 'NOK';
                break;
            default:
                mode = 'No selection';
                break;
        }

        RedrawSpouseFields(mode);
    };

    Xrm.Page.getAttribute("va_spousecountrylist").addOnChange(onSpouseCountryListChange);
    Xrm.Page.getAttribute("va_spouserecordaction").addOnChange(onSpouseRecordActionChange);

    //save dropdown choices into a global variable
    spouse_action_options = getA("va_spouserecordaction").getOptions();

    if (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_CREATE) {
        RetrieveVetInfo();
    }

    if (getA('va_ranfnod').getValue() === true) {
        Xrm.Page.getControl('va_dateofdeath').setDisabled(true);
        Xrm.Page.getControl('va_causeofdeath').setDisabled(true);

        // check MOD eligibility
        if (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_UPDATE && Xrm.Page.getAttribute('va_ranfindmod').getValue() != true) {
            // VALIDATE fnod completion
            if (ValidateFnodCompletion()) {
                var validateModResponse = ValidateMOD();

                switch (validateModResponse) {
                    case 'Eligible':
                        alert('FNOD successful. Please process MOD (for spouse) or NOK.');
                        break;
                    case 'Not Eligible':
                        alert('FNOD successful. Please proceed to PMC (if requested) - not MOD eligible');
                        break;
                    default:
                        break;
                }
            }
        }
    }

    if (getA('va_ranpmc').getValue() && getA('va_ranfnod').getValue() && getA('va_ranmod').getValue()) {
        DisableAll();
    }

    var createdon = getA('createdon').getValue();
    var today = new Date().setHours(0, 0, 0, 0);

    if (createdon) {
        createdon = createdon.setHours(0, 0, 0, 0);

        if (new Date(createdon) < new Date(today)) {
            DisableAll();
        }
    }

    if (getA('va_ranpmc').getValue() || getA('va_ranfnod').getValue() || getA('va_ranmod').getValue()) {
        Xrm.Page.ui.tabs.get('general_tab').sections.get('execution_results').setVisible(true);
    }
    else {
        Xrm.Page.ui.tabs.get('general_tab').sections.get('execution_results').setVisible(false);
    }

    //if MOD has already been run, hide buttons inlcuding Validate MOD, and show all MOD fields.
    if (getA('va_ranmod').getValue()) {
        DisableMODFields();
    }

    // on closing if ran fnod, but not MOD and either didn't check mod elig, or is elig, prompt to run MOD
    addbeforeunloadEvent(function () {
        if (!_ranFnodThisTime && !_modPrompt) {    // _ranFnodThisTime us true if we just executed FNOD, which auto triggers save
            var getA = Xrm.Page.getAttribute;
            var isClosing = true;

            if (isClosing && getA('va_ranfnod').getValue() === true && getA('va_ranmod').getValue() != true &&
                (getA('va_ranfindmod').getValue() != true || getA('va_modeligible').getValue() === true)) {
                _modPrompt = true;
                if (!confirm('You have executed FNOD but did not execute MOD. Please confirm that you would like to close this screen without executing MOD.\n\nTo close the screen, click OK. To return back to the screen and execute MOD, click CANCEL.')) {
                    // RU12 Remove event.returnValue = false;
                    var evArgs = executionObj.getEventArgs();
                    if (evArgs != null) { evArgs.preventDefault(); }
                    return false;
                }
            }
        }
    });
    //CloseProgress();   
}

function addbeforeunloadEvent(func) {
    var oldonbeforeunload = window.onbeforeunload;
    if (typeof window.onbeforeunload != 'function') {
        window.onbeforeunload = func;
    } else {
        window.onbeforeunload = function () {
            oldonbeforeunload();
            func();
        }
    }
}

function ValidateFnodCompletion() {
    var maxRetries = 15,
        vetSearchCtx = new vrmContext();

    ShowProgress('Validating FNOD Completion (attempt 1 of ' + maxRetries.toString() + ')');

    // IMPORTANT: findGeneralInformationByFileNumber often does not report correct DOD after FNOD, but findGeneralInformationByPtcpntIds does
    // therefore, run findGeneralInformationByPtcpntIds

    if (_UserSettings == null || _UserSettings == undefined) { _UserSettings = GetUserSettingsForWebservice(); }
    vetSearchCtx.user = _UserSettings;

    var genInfoByPidXml = parent_page.getAttribute("va_generalinformationresponsebypid").getValue(),
        genInfoByPidXmlObject = genInfoByPidXml != null && genInfoByPidXml.length > 0 ? _XML_UTIL.parseXmlObject(genInfoByPidXml) : null,
        usefindGeneralInformationByPtcpntIds = (genInfoByPidXmlObject != null);

    if (usefindGeneralInformationByPtcpntIds) {
        // validate PID response has necessary fields
        var ptcpntBeneID = SingleNodeExists(genInfoByPidXmlObject, '//return/ptcpntBeneID') ? genInfoByPidXmlObject.selectSingleNode('//return/ptcpntBeneID').text : '',
            ptcpntRecipID = SingleNodeExists(genInfoByPidXmlObject, '//return/ptcpntRecipID') ? genInfoByPidXmlObject.selectSingleNode('//return/ptcpntRecipID').text : '',
            ptcpntVetID = SingleNodeExists(genInfoByPidXmlObject, '//return/ptcpntVetID') ? genInfoByPidXmlObject.selectSingleNode('//return/ptcpntVetID').text : '',
            awardTypeCode = SingleNodeExists(genInfoByPidXmlObject, '//return/awardTypeCode') ? genInfoByPidXmlObject.selectSingleNode('//return/awardTypeCode').text : '';

        if (ptcpntVetID.length > 0) {
            // use by PID
            vetSearchCtx.parameters['ptcpntVetId'] = ptcpntVetID;
            vetSearchCtx.parameters['ptcpntBeneId'] = ptcpntBeneID;
            vetSearchCtx.parameters['ptcpntRecipId'] = ptcpntRecipID;
            vetSearchCtx.parameters['awardTypeCd'] = awardTypeCode;
        }
        else {
            // use findGeneralInformationByFileNumber
            usefindGeneralInformationByPtcpntIds = false;
            vetSearchCtx.parameters['fileNumber'] = Xrm.Page.getAttribute('va_filenumber').getValue();
        }
    }

    var dodResponseNode = usefindGeneralInformationByPtcpntIds ? '//return/vetDeathDate' : '//return/DATE_OF_DEATH',
        serviceName = usefindGeneralInformationByPtcpntIds ? 'findGeneralInformationByPtcpntIds' : 'findGeneralInformationByFileNumber',
        vetInfo = usefindGeneralInformationByPtcpntIds ? new findGeneralInformationByPtcpntIds(vetSearchCtx) : new findGeneralInformationByFileNumber(vetSearchCtx),
        notificationSection = Xrm.Page.ui.tabs.get('general_tab').sections.get('execution_results');

    for (var i = 0; i < maxRetries; i++) {
        vetInfo.executeRequest();

        if (vetInfo.wsMessage.errorFlag) {
            // runtime error
            CloseProgress();
            alert('Error during the execution of ' + serviceName + ' service for Veteran.\nCannot determine the completion status of FNOD call.\n\n' + vetInfo.wsMessage.description);
            notificationSection.setLabel('Cannot determine the completion status of FNOD call; ' + serviceName + ' call failed');
            return false;
        }
        var xmlObject = _XML_UTIL.parseXmlObject(vetInfo.responseXml);
        var dod = SingleNodeExists(xmlObject, dodResponseNode) ? xmlObject.selectSingleNode(dodResponseNode).text : '';
        if (dod != null && dod.length > 0) {
            // FNOD completed, DOD is on recrod
            notificationSection.setLabel('FNOD/Memorial Certificate Execution Results: FNOD Date of Death found – MOD check executed');
            CloseProgress();
            return true;
        }
        // if we are here FNOD process didn't update DOD yet (or failed, we have no way to knowing which is true)
        // try to search CORP again
        ShowProgress('Validating FNOD Completion (attempt ' + (i + 2).toString() + ' of ' + maxRetries.toString() + ')');
    }
    // if we are here, we couldn't get DOD from corp after maxRetries
    // notify user
    notificationSection.setLabel('FNOD - Date of Death NOT FOUND. Wait a few minutes and refresh the screen or contact tech support.');

    CloseProgress();
    return false;
}

function SetMandatoryFields(type) {

    var fnod = ['va_dateofdeath', 'va_causeofdeath', 'va_fnodfirstname', 'va_fnodlastname'];
    var pmc = ['va_newpmcvetfirstname', 'va_newpmcvetlastname', 'va_newpmcrecipaddress1', 'va_newpmcrecipcity', 'va_newpmcrecipstate', 'va_newpmcrecipzip', 'va_newpmcreciprelationshiptovet', 'va_newpmcrecipsalutation'];
    var mod = ['va_survivingspouseisvalidformod', 'va_typeofnotice', 'va_spouseaddress1', 'va_treasuryaddress1', 'va_treasuryaddress2', 'va_treasuryaddress3']; //removed 'va_spousecity', 'va_spousecountry'
    var sets = Array();
    sets['FNOD'] = fnod;
    sets['PMC'] = pmc;
    sets['MOD'] = mod;

    if (type == 'reset') {
        for (var op in sets) {
            var arr = sets[op];
            for (var c in arr) {
                Xrm.Page.getAttribute(arr[c]).setSubmitMode('dirty');
                Xrm.Page.getAttribute(arr[c]).setRequiredLevel('none');
            }
        }
        return;
    }
    var arr = sets[type];
    if (!arr) return;

    for (c in arr) {
        Xrm.Page.getAttribute(arr[c]).setSubmitMode('always');
        Xrm.Page.getAttribute(arr[c]).setRequiredLevel('required');
    };
}

function ExecuteFNOD() {

    SetMandatoryFields('reset');
    SetMandatoryFields('FNOD');
    _ranFnodThisTime = false;

    if (Xrm.Page.getAttribute('va_ranfnod').getValue() || Xrm.Page.getAttribute('va_ranpmc').getValue()) {
        alert('You cannot run the FNOD process twice and/or after PMC has been issued. \n\nIf you need to rerun the FNOD, please create new FNOD window from FNOD section of the Phone Call screen.');
        return false;
    }

    var retrievedFirstName = Xrm.Page.getAttribute('va_birlsfirstname').getValue(); //va_firstname
    var retrievedLastName = Xrm.Page.getAttribute('va_birlslastname').getValue();   // va_lastname
    var enteredFirstName = Xrm.Page.getAttribute('va_fnodfirstname').getValue();
    var enteredLastName = Xrm.Page.getAttribute('va_fnodlastname').getValue();

    if (!enteredFirstName || !enteredLastName) {
        alert('First/last name of the veteran must be provided.');
        return;
    }

    retrievedFirstName = retrievedFirstName.toString().toUpperCase();
    retrievedLastName = retrievedLastName.toString().toUpperCase();
    enteredFirstName = enteredFirstName.toString().toUpperCase();
    enteredLastName = enteredLastName.toString().toUpperCase();

    if (enteredFirstName != retrievedFirstName || enteredLastName != retrievedLastName) {
        alert('The name you have entered does not match the name reported by BIRLS: \n\n' +
				   'Entered:  ' + enteredFirstName + ' ' + enteredLastName + '\n' +
				   'BIRLS:  ' + retrievedFirstName + ' ' + retrievedLastName + '\n\n' +
				   'First Notice of Death cannot be processed because names do not match.');
        return;
    }

    if (!Xrm.Page.getAttribute('va_dateofdeath').getValue() || !Xrm.Page.getAttribute('va_causeofdeath').getValue()) {
        alert('Date of death/cause of death must be provided.');
        return;
    }

    var dod = new Date(Date.parse(Xrm.Page.getAttribute('va_dateofdeath').getValue()));
    var today = new Date();

    if (dod > today) {
        alert('Date of death cannot be a future date.');
        return;
    }

    if (!confirm('WARNING: You are about to run the FNOD process for the selected veteran. This operation cannot be undone. Please click OK to confirm that you want to proceed.')) return;

    //if (!confirm('Please read the following script to the Veteran and click on OK button to confirm that the script was read:\n\n\nI need to read the following statement to you.\nI am a VA employee who is authorized to receive or request evidentiary information or statements that may result in a change in your VA benefits. The primary purpose for gathering this information or statement is to make an eligibility determination. It is subject to verification through computer matching programs with other agencies.')) return;

    var vetSearchCtx = new vrmContext();
    _UserSettings = GetUserSettingsForWebservice();
    vetSearchCtx.user = _UserSettings;

    vetSearchCtx.parameters['fileNumber'] = Xrm.Page.getAttribute('va_filenumber').getValue();

    // First step: run Sync process if user requested it or CORP doesn't exist
    if (!_doHaveCorpRecord || Xrm.Page.getAttribute('va_synccorpandbirls').getValue() == true) {
        if (Xrm.Page.getAttribute('va_synccorpandbirls').getValue() != true) { Xrm.Page.getAttribute('va_synccorpandbirls').setValue(true); }
        if (!ExecuteCorpBIRLSSync(vetSearchCtx, true)) {
            alert('FNOD unsuccessful. Please reattempt.');
            return;
        }
    }

    vetSearchCtx.parameters['dateOfDeath'] = dod.format('MM/dd/yyyy');

    var cause = '';

    switch (Xrm.Page.getAttribute('va_causeofdeath').getValue()) {
        case 953850000:
            cause = 'Combat';
            break;
        case 953850001:
            cause = 'Natural';
            break;
        case 953850002:
            cause = 'Other';
            break;
        case 953850003:
            cause = 'Unknown';
            break;
        default:
            cause = 'Unknown';
            break;
    }

    vetSearchCtx.parameters['causeOfDeath'] = cause;

    var runFNOD = new updateFirstNoticeOfDeath(vetSearchCtx);
    runFNOD.suppressProgressDlg = true;
    runFNOD.executeRequest();

    Xrm.Page.getAttribute('va_fnodrequest').setValue(NN(runFNOD.wsMessage.xmlRequest));
    Xrm.Page.getAttribute('va_fnodresponse').setValue(NN(runFNOD.responseXml));

    var dt = new Date();
    //CloseProgress();

    if (runFNOD.wsMessage.errorFlag) {
        Xrm.Page.getAttribute('va_fnodrequeststatus').setValue(runFNOD.wsMessage.description);
        alert('FNOD web service failed to execute.\n\nMid-tier components reported this error: ' + runFNOD.wsMessage.description);
        alert('FNOD unsuccessful. Please reattempt.');
        return false;
    }

    Xrm.Page.getAttribute('va_vetdateofdeath').setValue(Xrm.Page.getAttribute('va_dateofdeath').getValue());
    Xrm.Page.getAttribute('va_fnodrequeststatus').setValue("A First Notice of Death request has been processed successfully.");
    //Xrm.Page.getAttribute('va_fnodresults').setValue('Success (' + dt.toDateString() + ')');
    Xrm.Page.getAttribute('va_ranfnod').setValue(true);
    Xrm.Page.getAttribute('va_fnoddate').setValue(new Date());

    if (confirm('FNOD has been processed successfully.\n\nWould you like to create a new FNOD/0820a Service Request? Please click OK to confirm .')) {
        Xrm.Page.getAttribute('va_createdsr').setValue(true);
        page_opener._CreateClaimServiceRequest(null, 'FNOD', true, '0820a', null, parent_page.getAttribute('va_disposition').getValue());
    }
    _ranFnodThisTime = true;
    Xrm.Page.data.entity.save("save");

}

function IssuePMC() {
    SetMandatoryFields('reset');
    SetMandatoryFields('PMC');
    //debugger;
    if (!Xrm.Page.getAttribute('va_ranpmc').getValue()) {

        if (!confirm('I have verified that the Veteran had an honorable or general discharge. Please click OK to confirm.')) {
            return;
        } else {
            Xrm.Page.getAttribute('va_confirmedvetsdischarge').setValue(true);
        }

        var vetSearchCtx = new vrmContext();
        _UserSettings = GetUserSettingsForWebservice();
        vetSearchCtx.user = _UserSettings;

        // pad file number and ?use TD format
        // if 8 digits, first must be space
        // if less than 8 first space others fill with digits
        //		var fileNo = Xrm.Page.getAttribute('va_filenumber').getValue();
        //		if (fileNo) {
        //			if (fileNo.length > 9) {
        //				alert('Invalid File Number field - field is too long.');
        //				return;
        //			}
        //			if (fileNo.length < 9) {
        //				if (fileNo.length == 8) {
        //					fileNo = ' ' + fileNo;
        //				}
        //				else {
        //					// space, then leading 0 than number
        //					fileNo = ' ' + new Array(8 - fileNo.length + 1).join("0") + fileNo;
        //				}
        //			}

        //			// Need to find out if rotation is needed
        //			//fileNo = fileNo.substr(7, 2) + fileNo.substr(0, 7);
        //		}

        vetSearchCtx.parameters['fileNumber'] = Xrm.Page.getAttribute('va_filenumber').getValue();
        vetSearchCtx.parameters['veteranFirstName'] = Xrm.Page.getAttribute('va_newpmcvetfirstname').getValue();
        vetSearchCtx.parameters['veteranMiddleInitial'] = Xrm.Page.getAttribute('va_newpmcvetmiddleinitial').getValue();
        vetSearchCtx.parameters['veteranLastName'] = Xrm.Page.getAttribute('va_newpmcvetlastname').getValue();
        vetSearchCtx.parameters['veteranSuffixName'] = Xrm.Page.getAttribute('va_newpmcvetsuffix').getValue();
        vetSearchCtx.parameters['station'] = Xrm.Page.getAttribute('va_newpmcvetstation').getValue();
        vetSearchCtx.parameters['salutation'] = Xrm.Page.getAttribute('va_newpmcrecipsalutation').getValue();
        vetSearchCtx.parameters['title'] = Xrm.Page.getAttribute('va_newpmcrecipname').getValue();
        vetSearchCtx.parameters['addressLine1'] = Xrm.Page.getAttribute('va_newpmcrecipaddress1').getValue();
        vetSearchCtx.parameters['addressLine2'] = Xrm.Page.getAttribute('va_newpmcrecipaddress2').getValue();
        vetSearchCtx.parameters['city'] = Xrm.Page.getAttribute('va_newpmcrecipcity').getValue();
        vetSearchCtx.parameters['state'] = Xrm.Page.getAttribute('va_newpmcrecipstate').getValue();
        vetSearchCtx.parameters['zipCode'] = Xrm.Page.getAttribute('va_newpmcrecipzip').getValue();
        vetSearchCtx.parameters['realtionshipToVeteran'] = Xrm.Page.getAttribute('va_newpmcreciprelationshiptovet').getValue();

        var insertPMC = new insertPresidentialMemorialCertificate(vetSearchCtx);
        insertPMC.executeRequest();

        Xrm.Page.getAttribute('va_insertpmcrequest').setValue(NN(insertPMC.wsMessage.xmlRequest));

        CloseProgress();
        if (insertPMC.wsMessage.errorFlag) {
            Xrm.Page.getAttribute('va_pmcrequeststatus').setValue(insertPMC.wsMessage.description);
            alert('Insert PMC web service failed to execute.\n\nMid-tier components reported this error:\n\n' + insertPMC.wsMessage.description);
        }
        else {
            Xrm.Page.getAttribute('va_pmcrequeststatus').setValue("A Presidential Memorial Certificate has been processed successfully.");
            //var pmc_xmlObject = _XML_UTIL.parseXmlObject(insertPMC.responseXml);
            Xrm.Page.ui.tabs.get('general_tab').sections.get('execution_results').setVisible(true);
            DisableAll();
            Xrm.Page.getAttribute('va_ranpmc').setValue(true);
            Xrm.Page.data.entity.save("save");
        }
    }
    else {
        alert('This form cannot be modified after a new PMC has already been issued. Please create a new FNOD/PMC request.');
    }
}

function CopyPMC() {
    if (!Xrm.Page.getAttribute('va_ranpmc').getValue()) {
        Xrm.Page.getAttribute('va_newpmcvetfirstname').setValue(Xrm.Page.getAttribute('va_existingpmcvetfirstname').getValue());
        Xrm.Page.getAttribute('va_newpmcvetmiddleinitial').setValue(Xrm.Page.getAttribute('va_existingpmcvetmiddleinitial').getValue());
        Xrm.Page.getAttribute('va_newpmcvetlastname').setValue(Xrm.Page.getAttribute('va_existingpmcvetlastname').getValue());
        Xrm.Page.getAttribute('va_newpmcvetsuffix').setValue(Xrm.Page.getAttribute('va_existingpmcvetsuffix').getValue());
        Xrm.Page.getAttribute('va_newpmcvetstation').setValue(Xrm.Page.getAttribute('va_existingpmcvetstation').getValue());
        Xrm.Page.getAttribute('va_newpmcrecipsalutation').setValue(Xrm.Page.getAttribute('va_existingpmcrecipsalutation').getValue());
        Xrm.Page.getAttribute('va_newpmcrecipname').setValue(Xrm.Page.getAttribute('va_existingpmcrecipname').getValue());
        Xrm.Page.getAttribute('va_newpmcrecipaddress1').setValue(Xrm.Page.getAttribute('va_existingpmcrecipaddress1').getValue());
        Xrm.Page.getAttribute('va_newpmcrecipaddress2').setValue(Xrm.Page.getAttribute('va_existingpmcrecipaddress2').getValue());
        Xrm.Page.getAttribute('va_newpmcrecipcity').setValue(Xrm.Page.getAttribute('va_existingpmcrecipcity').getValue());
        Xrm.Page.getAttribute('va_newpmcrecipstate').setValue(Xrm.Page.getAttribute('va_existingpmcrecipstate').getValue());
        Xrm.Page.getAttribute('va_newpmcrecipzip').setValue(Xrm.Page.getAttribute('va_existingpmcrecipzip').getValue());
        Xrm.Page.getAttribute('va_newpmcreciprelationshiptovet').setValue(Xrm.Page.getAttribute('va_existingpmcreciprelationshiptovet').getValue());
    }
    else {
        alert('This form cannot be modified after a new PMC has been issued. Please create a new FNOD/PMC request.');
    }
}

function RetrieveAddressesFromParent(parent_page) {

    var addrXml = parent_page.getAttribute('va_findaddressresponse').getValue(),
		address_xmlObject = addrXml && addrXml.length > 0 ? _XML_UTIL.parseXmlObject(addrXml) : null,
		found_mailing = false,
		mailing_address = '',
		addressNodes = address_xmlObject ? address_xmlObject.selectNodes('//return') : null;

    if (addressNodes) {
        for (var i = 0; i < addressNodes.length; i++) {         //looping through addresses and
            if (addressNodes[i].selectSingleNode('ptcpntAddrsTypeNm').text == 'Mailing' && !found_mailing) {

                mailing_address += SingleNodeExists(addressNodes[i], 'addrsOneTxt') ? addressNodes[i].selectSingleNode('addrsOneTxt').text : '';
                mailing_address += SingleNodeExists(addressNodes[i], 'addrsTwoTxt') ? '\n' + addressNodes[i].selectSingleNode('addrsTwoTxt').text : '';
                mailing_address += SingleNodeExists(addressNodes[i], 'addrsThreeTxt') ? '\n' + addressNodes[i].selectSingleNode('addrsThreeTxt').text : '';
                mailing_address += SingleNodeExists(addressNodes[i], 'cityNm') ? '\n' + addressNodes[i].selectSingleNode('cityNm').text : '';
                mailing_address += SingleNodeExists(addressNodes[i], 'zipPrefixNbr') ? ', ' + addressNodes[i].selectSingleNode('zipPrefixNbr').text : '';
                var country = SingleNodeExists(addressNodes[i], 'cntryNm') ? addressNodes[i].selectSingleNode('cntryNm').text : '';

                mailing_address += country ? '\n' + country : '';

                mailing_address += SingleNodeExists(addressNodes[i], 'mltyPostOfficeTypeCd') ? '\n' + addressNodes[i].selectSingleNode('mltyPostOfficeTypeCd').text : ''; //APO
                mailing_address += SingleNodeExists(addressNodes[i], 'mltyPostalTypeCd') ? ' ' + addressNodes[i].selectSingleNode('mltyPostalTypeCd').text : ''; //AE

                found_mailing = true;

                Xrm.Page.getAttribute('va_lastknownaddress').setValue(mailing_address);
            }
        }
    }
}

function CopyLastKnownAddressToSpouseFields() {
    _fnod_.copyLastKnownAddressToSpouseFields(Xrm, parent_page.getAttribute('va_findaddressresponse').getValue());

    updateTreasuryFields();
    usaaddress();
}

function RetrieveVetInfo() {

    //debugger;
    // mark that CORP doesn't exist
    _doHaveCorpRecord = false;

    veteranInformation_xml = parent_page.getAttribute('va_findcorprecordresponse').getValue();
    if (veteranInformation_xml != null) { veteranInformation_xmlObject = _XML_UTIL.parseXmlObject(veteranInformation_xml); }

    generalInfoResponseByPid_xml = parent_page.getAttribute("va_generalinformationresponsebypid").getValue();
    generalInfoResponseByPid_xmlObject = null;
    if (generalInfoResponseByPid_xml != null) { generalInfoResponseByPid_xmlObject = _XML_UTIL.parseXmlObject(generalInfoResponseByPid_xml); }

    birls_xml = parent_page.getAttribute("va_findbirlsresponse").getValue();
    var birls_xmlObject = null;
    if (birls_xml != null) { birls_xmlObject = _XML_UTIL.parseXmlObject(birls_xml); }

    dependents_xml = parent_page.getAttribute("va_finddependentsresponse").getValue();
    dependents_xmlObject = null;
    if (dependents_xml != null) { dependents_xmlObject = _XML_UTIL.parseXmlObject(dependents_xml); }

    var vetFileNumber = SingleNodeExists(veteranInformation_xmlObject, '//return/fileNumber')
					? veteranInformation_xmlObject.selectSingleNode('//return/fileNumber').text : null;

    // mark that CORP does exist
    if (vetFileNumber && vetFileNumber.length > 0) { _doHaveCorpRecord = true; }

    var vetFirstName = SingleNodeExists(veteranInformation_xmlObject, '//return/firstName')
					? veteranInformation_xmlObject.selectSingleNode('//return/firstName').text : null;

    var vetLastName = SingleNodeExists(veteranInformation_xmlObject, '//return/lastName')
					? veteranInformation_xmlObject.selectSingleNode('//return/lastName').text : null;

    var vetDOB = SingleNodeExists(veteranInformation_xmlObject, '//return/dateOfBirth')
					? veteranInformation_xmlObject.selectSingleNode('//return/dateOfBirth').text : null;

    var vetDOD = SingleNodeExists(birls_xmlObject, '//return/DATE_OF_DEATH')
					? birls_xmlObject.selectSingleNode('//return/DATE_OF_DEATH').text : null;

    if (!vetDOD) {
        vetDOD = SingleNodeExists(generalInfoResponseByPid_xmlObject, '//return/vetDeathDate')
					? generalInfoResponseByPid_xmlObject.selectSingleNode('//return/vetDeathDate').text : null;
        if (vetDOD) {
            Xrm.Page.getAttribute('va_vetdateofdeath').setValue(new Date(FormatExtjsDate(vetDOD)));
        }
    } else {
        Xrm.Page.getAttribute('va_vetdateofdeath').setValue(new Date(vetDOD));
    }

    var vetSex = SingleNodeExists(generalInfoResponseByPid_xmlObject, '//return/vetSex')
					? generalInfoResponseByPid_xmlObject.selectSingleNode('//return/vetSex').text : null;

    var vetSOJ = SingleNodeExists(generalInfoResponseByPid_xmlObject, '//return/stationOfJurisdiction')
					? generalInfoResponseByPid_xmlObject.selectSingleNode('//return/stationOfJurisdiction').text : null;

    var vetPOA = SingleNodeExists(generalInfoResponseByPid_xmlObject, '//return/powerOfAttorney')
					? generalInfoResponseByPid_xmlObject.selectSingleNode('//return/powerOfAttorney').text : null;

    // if no corp, get data from BIRLS
    var bilrsFirstName = '', birlsLastName = '', birlsFileNo = '';
    if (birls_xmlObject) {
        if (SingleNodeExists(birls_xmlObject, '//return/CLAIM_NUMBER')) {
            Xrm.Page.getAttribute('va_birlsfileno').setValue(birls_xmlObject.selectSingleNode('//return/CLAIM_NUMBER').text);
            if (!vetFileNumber || vetFileNumber.length == 0) {
                vetFileNumber = birls_xmlObject.selectSingleNode('//return/CLAIM_NUMBER').text;
            }
        }

        if ((!vetFileNumber || vetFileNumber.length == 0) && SingleNodeExists(birls_xmlObject, '//return/SOC_SEC_NUMBER')) {
            vetFileNumber = birls_xmlObject.selectSingleNode('//return/SOC_SEC_NUMBER').text;
        }

        if (SingleNodeExists(birls_xmlObject, '//return/FIRST_NAME')) {
            Xrm.Page.getAttribute('va_birlsfirstname').setValue(birls_xmlObject.selectSingleNode('//return/FIRST_NAME').text);
            if (!vetFirstName || vetFirstName.length == 0) {
                vetFirstName = birls_xmlObject.selectSingleNode('//return/FIRST_NAME').text;
            }
        }
        if (SingleNodeExists(birls_xmlObject, '//return/LAST_NAME')) {
            Xrm.Page.getAttribute('va_birlslastname').setValue(birls_xmlObject.selectSingleNode('//return/LAST_NAME').text);
            if (!vetLastName || vetLastName.length == 0) {
                vetLastName = birls_xmlObject.selectSingleNode('//return/LAST_NAME').text;
            }
        }
        if ((!vetDOB || vetDOB.length == 0) && SingleNodeExists(birls_xmlObject, '//return/DATE_OF_BIRTH')) {
            vetDOB = birls_xmlObject.selectSingleNode('//return/DATE_OF_BIRTH').text;
        }
        if ((!vetSex || vetSex.length == 0) && SingleNodeExists(birls_xmlObject, '//return/SEX_CODE')) {
            vetSex = birls_xmlObject.selectSingleNode('//return/SEX_CODE').text;
        }
        if ((!vetSOJ || vetSOJ.length == 0) && SingleNodeExists(birls_xmlObject, '//return/FOLDER/FOLDER_CURRENT_LOCATION')) {
            vetSOJ = birls_xmlObject.selectSingleNode('//return/SEX_CODE').text;
        }
    }

    Xrm.Page.getAttribute('va_filenumber').setValue(vetFileNumber);
    Xrm.Page.getAttribute('va_firstname').setValue(vetFirstName);
    Xrm.Page.getAttribute('va_lastname').setValue(vetLastName);
    //    var dobD = null;
    //    try { if (vetDOB) { dobD = new Date(vetDOB); } } catch (der) {
    //        //debugger; 
    //    }
    //    Xrm.Page.getAttribute('va_dateofbirth').setValue(dobD);
    //Addition of new DOB Text Field:  RTC 108417 - Handle Dates before 1900.
    Xrm.Page.getAttribute('va_dateofbirthtext').setValue(vetDOB);
    Xrm.Page.getAttribute('va_dateofbirthtext').setSubmitMode('always');

    if (vetSex == 'M') {
        Xrm.Page.getAttribute('va_sex').setValue(953850000);
    } else if (vetSex == 'F') {
        Xrm.Page.getAttribute('va_sex').setValue(953850001);
    } else {
        Xrm.Page.getAttribute('va_sex').setValue(953850002);
    }

    Xrm.Page.getAttribute('va_soj').setValue(vetSOJ);
    Xrm.Page.getAttribute('va_poa').setValue(vetPOA);


    var dependents = '';
    returnNode = null;
    if (dependents_xmlObject) {
        returnNode = dependents_xmlObject.selectNodes('//return');
        var dependentNodes = returnNode && returnNode[0] && returnNode[0].childNodes ? returnNode[0].childNodes : null;

        if (returnNode && dependentNodes) {
            for (var i = 0; i < dependentNodes.length; i++) {         //looping through dependents
                if (dependentNodes[i].nodeName == 'persons') {
                    dependents += 'Name: ' + dependentNodes[i].selectSingleNode('firstName').text + ' ' + dependentNodes[i].selectSingleNode('lastName').text;
                    dependents += '     Relationship: ' + dependentNodes[i].selectSingleNode('relationship').text;
                    dependents += '     DOB: ' + dependentNodes[i].selectSingleNode('dateOfBirth').text;
                    if (dependentNodes[i].selectSingleNode('dateOfDeath').text != '') { dependents += '     DOD: ' + dependentNodes[i].selectSingleNode('dateOfDeath').text; }
                    if (dependentNodes[i].selectSingleNode('ssn').text != '') { dependents += '     SSN: ' + dependentNodes[i].selectSingleNode('ssn').text; }

                    //If spouse, alive and has a SSN, display his/her Awards
                    if ((dependentNodes[i].selectSingleNode('ssn').text != '') && (dependentNodes[i].selectSingleNode('dateOfDeath').text == '') && (dependentNodes[i].selectSingleNode('relationship').text == 'Spouse')) {

                        var spouseSearchCtx = new vrmContext();
                        _UserSettings = GetUserSettingsForWebservice();
                        spouseSearchCtx.user = _UserSettings;

                        spouseSearchCtx.parameters['fileNumber'] = dependentNodes[i].selectSingleNode('ssn').text; //'225789999';//

                        var spouseInfo = new findGeneralInformationByFileNumber(spouseSearchCtx);
                        spouseInfo.executeRequest();

                        if (!spouseInfo.wsMessage.errorFlag) {
                            var spouseInfo_xmlObject = _XML_UTIL.parseXmlObject(spouseInfo.responseXml);
                            var awardsExist = SingleNodeExists(spouseInfo_xmlObject, '//return/numberOfAwardBenes') ? spouseInfo_xmlObject.selectSingleNode('//return/numberOfAwardBenes').text : '';
                            if (awardsExist != '' & awardsExist > 0) {
                                dependents += '     Has Awards: Yes';
                            } else {
                                dependents += '     Has Awards?: No';
                            }
                        }
                    }

                    dependents += '\n';
                    //                    if (i != dependentNodes.length && dependentNodes.length > 1) { dependents += '***************************************************************************************************\n'; }
                }
            }
        }
    }

    Xrm.Page.getAttribute('va_listofdependents').setValue(dependents);

    var folderlocation = '';
    returnNode = birls_xmlObject.selectNodes('//return/folders');
    birlsNodes = returnNode[0].childNodes;

    if (returnNode) {
        for (var i = 0; i < birlsNodes.length; i++) {         //looping through folders
            if (birlsNodes[i].nodeName === 'FOLDER') {
                if (birlsNodes[i].selectSingleNode('FOLDER_TYPE').text === 'CLAIM') {
                    folderlocation = birlsNodes[i].selectSingleNode('FOLDER_CURRENT_LOCATION').text;
                    break;
                }
            }
        }
    }

    Xrm.Page.getAttribute('va_folderlocation').setValue(folderlocation);

    Xrm.Page.getAttribute('va_lastknownaddress').setValue(_fnod_.parseAddress(_fnod_.getAddress(parent_page.getAttribute('va_findaddressresponse').getValue(), 'Mailing')));

    var vetSearchCtx = new vrmContext();
    _UserSettings = GetUserSettingsForWebservice();
    vetSearchCtx.user = _UserSettings;
    vetSearchCtx.parameters['fileNumber'] = Xrm.Page.getAttribute('va_filenumber').getValue();
    var findPMC = new findPresidentialMemorialCertificate(vetSearchCtx);
    findPMC.executeRequest();

    Xrm.Page.getAttribute('va_findpmcrequest').setValue(NN(findPMC.wsMessage.xmlRequest));

    CloseProgress();

    if (findPMC.wsMessage.errorFlag) {
        alert('Find PMC web service failed to refresh information based on the file Number provided.\n\nMid-tier components reported this error: \n\n' + findPMC.wsMessage.description);
        return;
    }
    else {
        var pmc_xmlObject = _XML_UTIL.parseXmlObject(findPMC.responseXml);

        var pmc_vetFirstName = SingleNodeExists(pmc_xmlObject, '//return/veteranFirstName')
				? pmc_xmlObject.selectSingleNode('//return/veteranFirstName').text : null;

        var pmc_vetMiddleInitial = SingleNodeExists(pmc_xmlObject, '//return/veteranMiddleInitial')
				? pmc_xmlObject.selectSingleNode('//return/veteranMiddleInitial').text : null;

        var pmc_vetLastName = SingleNodeExists(pmc_xmlObject, '//return/veteranLastName')
				? pmc_xmlObject.selectSingleNode('//return/veteranLastName').text : null;

        var pmc_vetSuffixName = SingleNodeExists(pmc_xmlObject, '//return/veteranSuffixName')
				? pmc_xmlObject.selectSingleNode('//return/veteranSuffixName').text : null;

        var pmc_station = SingleNodeExists(pmc_xmlObject, '//return/station')
				? pmc_xmlObject.selectSingleNode('//return/station').text : null;

        var pmc_recipSalutation = SingleNodeExists(pmc_xmlObject, '//return/salutation')
				? pmc_xmlObject.selectSingleNode('//return/salutation').text : null;

        var pmc_recipTitle = SingleNodeExists(pmc_xmlObject, '//return/title')
				? pmc_xmlObject.selectSingleNode('//return/title').text : null;

        var pmc_recipAddressLine1 = SingleNodeExists(pmc_xmlObject, '//return/addressLine1')
				? pmc_xmlObject.selectSingleNode('//return/addressLine1').text : null;

        var pmc_recipAddressLine2 = SingleNodeExists(pmc_xmlObject, '//return/addressLine2')
				? pmc_xmlObject.selectSingleNode('//return/addressLine2').text : null;

        var pmc_recipCity = SingleNodeExists(pmc_xmlObject, '//return/city')
				? pmc_xmlObject.selectSingleNode('//return/city').text : null;

        var pmc_recipState = SingleNodeExists(pmc_xmlObject, '//return/state')
				? pmc_xmlObject.selectSingleNode('//return/state').text : null;

        var pmc_recipZip = SingleNodeExists(pmc_xmlObject, '//return/zipCode')
				? pmc_xmlObject.selectSingleNode('//return/zipCode').text : null;

        var pmc_recipRelationshipToVet = SingleNodeExists(pmc_xmlObject, '//return/realtionshipToVeteran')
				? pmc_xmlObject.selectSingleNode('//return/realtionshipToVeteran').text : null;

        Xrm.Page.getAttribute('va_existingpmcvetfirstname').setValue(pmc_vetFirstName);
        Xrm.Page.getAttribute('va_existingpmcvetmiddleinitial').setValue(pmc_vetMiddleInitial);
        Xrm.Page.getAttribute('va_existingpmcvetlastname').setValue(pmc_vetLastName);
        Xrm.Page.getAttribute('va_existingpmcvetsuffix').setValue(pmc_vetSuffixName);
        Xrm.Page.getAttribute('va_existingpmcrecipsalutation').setValue(pmc_recipSalutation);
        Xrm.Page.getAttribute('va_existingpmcrecipname').setValue(pmc_recipTitle);
        Xrm.Page.getAttribute('va_existingpmcrecipaddress1').setValue(pmc_recipAddressLine1);
        Xrm.Page.getAttribute('va_existingpmcrecipaddress2').setValue(pmc_recipAddressLine2);
        Xrm.Page.getAttribute('va_existingpmcrecipcity').setValue(pmc_recipCity);
        Xrm.Page.getAttribute('va_existingpmcrecipstate').setValue(pmc_recipState);
        Xrm.Page.getAttribute('va_existingpmcrecipzip').setValue(pmc_recipZip);
        Xrm.Page.getAttribute('va_existingpmcreciprelationshiptovet').setValue(pmc_recipRelationshipToVet);

        if ((pmc_station) && (pmc_station != '0')) {
            Xrm.Page.getAttribute('va_existingpmcvetstation').setValue(pmc_station);
            Xrm.Page.getAttribute('va_newpmcvetstation').setValue(pmc_station);
        }
        else if ((Xrm.Page.getAttribute('va_folderlocation').getValue() != null) && ((pmc_station == null) || (pmc_station == '0'))) {
            Xrm.Page.getAttribute('va_existingpmcvetstation').setValue(folderlocation);
            Xrm.Page.getAttribute('va_newpmcvetstation').setValue(folderlocation);
        }
        else if ((Xrm.Page.getAttribute('va_folderlocation').getValue() == null) && ((pmc_station == null) || (pmc_station == '0'))) {
            Xrm.Page.getAttribute('va_existingpmcvetstation').setValue('10');
            Xrm.Page.getAttribute('va_newpmcvetstation').setValue('10');
        }

    }

}

function DisableAll() {
    Xrm.Page.ui.controls.forEach(function (control, index) {
        control.setDisabled(true);
    });


}

function DisableMODFields() {
    Xrm.Page.ui.tabs.get("mod_tab").sections.get("mod_tab_section_3").setVisible(false);
    Xrm.Page.ui.tabs.get("mod_tab").sections.get("spouse_section0").setVisible(true);
    Xrm.Page.ui.tabs.get("mod_tab").sections.get("spouse_section1").setVisible(true);
    Xrm.Page.ui.tabs.get("mod_tab").sections.get("spouse_section2").setVisible(true);
    Xrm.Page.ui.tabs.get("mod_tab").sections.get("spouse_section3").setVisible(true);
    Xrm.Page.ui.tabs.get("mod_tab").sections.get("spouse_section4").setVisible(true);
    Xrm.Page.ui.tabs.get("mod_tab").sections.get("spouse_section5").setVisible(true);
    Xrm.Page.ui.tabs.get("mod_tab").sections.get("spouse_section5b").setVisible(true);

    var ta = Xrm.Page.ui.tabs.get("treasuryadd");
    ta.setVisible(true);
    ta.sections.get("spouse_section6").setVisible(true);
    ta.sections.get("spouse_section7").setVisible(true);

    Xrm.Page.ui.tabs.get("mod_tab").sections.get("spouse_section5a").setVisible(false);
    Xrm.Page.ui.tabs.get("mod_tab").sections.get("mod_button_section").setVisible(false);

    Xrm.Page.getControl('va_spousefirstname').setDisabled(true);
    Xrm.Page.getControl('va_spousemiddlename').setDisabled(true);
    Xrm.Page.getControl('va_spouselastname').setDisabled(true);
    Xrm.Page.getControl('va_spousesuffix').setDisabled(true);
    Xrm.Page.getControl('va_spousessn').setDisabled(true);
    Xrm.Page.getControl('va_spousedob').setDisabled(true);
    Xrm.Page.getControl('va_spouseaddress1').setDisabled(true);
    Xrm.Page.getControl('va_spouseaddress2').setDisabled(true);
    Xrm.Page.getControl('va_spouseaddress3').setDisabled(true);
    Xrm.Page.getControl('va_spousecity').setDisabled(true);
    Xrm.Page.getControl('va_spousestatelist').setDisabled(true);
    Xrm.Page.getControl('va_spousezipcode').setDisabled(true);
    Xrm.Page.getControl('va_spousecountry').setDisabled(true);
    Xrm.Page.getControl('va_spousecountrylist').setDisabled(true);
    Xrm.Page.getControl('va_spouseaddresstype').setDisabled(true);
    Xrm.Page.getControl('va_spouseoverseasmilitarypostalcode').setDisabled(true);
    Xrm.Page.getControl('va_spouseoverseasmilitarypostofficetypecode').setDisabled(true);
    Xrm.Page.getControl('va_spouseforeignpostalcode').setDisabled(true);
    Xrm.Page.getControl('va_provincename').setDisabled(true);
    Xrm.Page.getControl('va_territoryname').setDisabled(true);
    Xrm.Page.getControl('va_isnationalorstatecemetery').setDisabled(true);
}

function ExecuteMOD() {
    //debugger;

    if (ValidateZipcode() == false) {
        return false;
    }

    var vetSearchCtx = new vrmContext(), ga = Xrm.Page.getAttribute, _UserSettings = GetUserSettingsForWebservice(),
	    gc = Xrm.Page.getControl;

    if (!_fnod_.validate([{
        action: 'zipCodeExists',
        parameters: [ga('va_spouseaddresstype').getValue(), ga('va_spousezipcode').getValue()]
    }
    ])) return;

    vetSearchCtx.user = _UserSettings;

    vetSearchCtx.parameters['cntrl_Mod_Tran_Id'] = Xrm.Page.getAttribute('va_modtranid').getValue(); //2041

    var noticetypeSelection = Xrm.Page.getAttribute("va_typeofnotice").getSelectedOption().text,
		spouserecordactionSelection = NN(ga('va_spouserecordaction').getText());
    /*
    Per Jinmay Patel :Here are four possible values for spouse change indicator. 
 
    A - Add a Spouse
    M - Modify Spouse Info along with address (Name, DOB, SSN and address)
    N - Next of Kin processing
    blank or do not populate - No change to spouse Info but changes allowed to address only (In other words, no changes allowed for spouse name, DOB, SSN)
    */

    switch (spouserecordactionSelection) {
        case 'Add New Spouse':
            vetSearchCtx.parameters['spouseChangeInd'] = 'A';
            break;
        case 'Modify Existing Spouse':
            vetSearchCtx.parameters['spouseChangeInd'] = 'M';
            break;
        case 'Send Next of Kin Letter':
            vetSearchCtx.parameters['spouseChangeInd'] = 'N';
            break;
        default:
            vetSearchCtx.parameters['spouseChangeInd'] = '';
            break;
    }

    //no longer used
    //var VetLetArray = ["Telephone FNOD", "Personal Interview FNOD", "Insurance PCR FNOD", "Death Match"]; //valid choices for the Vet letter to be sent  

    if (spouserecordactionSelection == 'Send Next of Kin Letter') {
        vetSearchCtx.parameters['mod_Letter_Type_Cd'] = 'NEXTKINPLUS';
    } else {
        vetSearchCtx.parameters['mod_Letter_Type_Cd'] = 'VET';
    }

    var modProcessType = '';
    var spouseValid = ga('va_survivingspouseisvalidformod').getValue();

    if (vetSearchCtx.parameters['mod_Letter_Type_Cd'] == 'NEXTKINPLUS') {
        modProcessType = 'LMOD';
    } else {
        if (spouseValid) {
            modProcessType = 'GMOD';
        } else {
            modProcessType = 'GMOD'; //modProcessType = 'LMOD';
        }
    }

    if (modProcessType == undefined || modProcessType == null || modProcessType == "") {
        alert('Error: Unable to determine MOD Process Type (GMOD or LMOD). Please contact the support team to research this issue.');
        return false;
    }

    if (modProcessType === 'GMOD') {
        // Defect 87024 For GMOD the mod_Letter_Type_Cd should always be PAYPLUS
        vetSearchCtx.parameters['mod_Letter_Type_Cd'] = 'PAYPLUS';

        // Defect 102179 The following fields are required for GMOD : First Name, Last Name, SSN and DOB
        var fields = [
			{ name: 'First Name', value: 'va_spousefirstname' },
			{ name: 'Last Name', value: 'va_spouselastname' },
			{ name: 'SSN', value: 'va_spousessn' },
			{ name: 'Date of Birth', value: 'va_spousedob' }
        ];

        for (var field in fields) {
            var fieldName = fields[field].value, val = ga(fieldName).getValue();
            if (val == null || val.length == 0) {
                alert('Field ' + fields[field].name + ' is required when Process Type is No change to Spouse, Add a spouse, or Modify a spouse.');
                Xrm.Page.getControl(fieldName).setDisabled(false);
                Xrm.Page.getControl(fieldName).setFocus();
                return false;
            }
        }
        var ssn = (ga('va_spousessn').getValue());
        ssn = ssn ? ssn.trim() : '';
        ssn.replace(new RegExp('-', 'g'), '').replace(new RegExp(' ', 'g'), '');
        if (!ssn || ssn.length != 9) {
            alert('SSN is required when Process Type is No change to Spouse, Add a spouse, or Modify a spouse. Please make sure it contains 9 digits.');
            Xrm.Page.getControl('va_spousessn').setDisabled(false);
            Xrm.Page.getControl('va_spousessn').setFocus();
            return false;
        }
    }

    ga('va_modprocesstype').setValue(modProcessType);
    vetSearchCtx.parameters['mod_Procs_Type_Cd'] = modProcessType;  //e.g., GMOD

    var isNationalOrStateCemetery = ga('va_isnationalorstatecemetery').getValue();
    if (isNationalOrStateCemetery) {
        isNationalOrStateCemetery = 'Y';
    }
    else {
        isNationalOrStateCemetery = 'N';
    }

    vetSearchCtx.parameters['isNationalOrStateCemetery'] = isNationalOrStateCemetery;

    //If not just letter, spouse's info must be verified
    if (ga('va_survivingspouseisvalidformod').getValue() == false && ga('va_modprocesstype').getValue() != 'LMOD') {
        alert('Please confirm that the surviving spouse is eligible for the MOD transaction.');
        return false;
    }

    vetSearchCtx.parameters['fileNumber'] = ga('va_filenumber').getValue();
    vetSearchCtx.parameters['spouseSSNNumber'] = ga('va_spousessn').getValue();
    var msj = ga('va_modsoj').getValue();
    if (msj && msj.length >= 3) { msj = msj.substring(0, 3); }
    vetSearchCtx.parameters['stationNumber'] = msj;
    vetSearchCtx.parameters['veteranParticipantID'] = ga('va_modvetptcpntid').getValue();

    if (modProcessType == 'LMOD') { // This means Send Next of Kin Letter
        vetSearchCtx.parameters['beneParticipantID'] = '';
    }
    else {
        vetSearchCtx.parameters['beneParticipantID'] = ga('va_modspouseptcpntid').getValue();
    }

    if (modProcessType == 'GMOD' && vetSearchCtx.parameters['mod_Letter_Type_Cd'] == '') {
        vetSearchCtx.parameters['letterRecipientID'] = ''; //payment only, no letters
    } else {
        vetSearchCtx.parameters['letterRecipientID'] = ga('va_modvetptcpntid').getValue();
    }

    vetSearchCtx.parameters['spouseFirstName'] = ga('va_spousefirstname').getValue();
    vetSearchCtx.parameters['spouseMiddleName'] = ga('va_spousemiddlename').getValue();
    vetSearchCtx.parameters['spouseLastName'] = ga('va_spouselastname').getValue();
    vetSearchCtx.parameters['normalizedAddressLine1'] = ga('va_spouseaddress1').getValue();
    vetSearchCtx.parameters['normalizedAddressLine2'] = NN(ga('va_spouseaddress2').getValue());
    vetSearchCtx.parameters['normalizedAddressLine3'] = NN(ga('va_spouseaddress3').getValue());


    var temp1 = '';
    switch (ga('va_spouseoverseasmilitarypostofficetypecode').getValue()) {
        case 953850000:
            temp1 = "APO";
            break;
        case 953850001:
            temp1 = "DPO";
            break;
        case 953850002:
            temp1 = "FPO";
            break;
        default:
            break;
    }
    vetSearchCtx.parameters['militaryPostalOfficeTypeCode'] = temp1;

    temp1 = '';
    switch (ga('va_spouseoverseasmilitarypostalcode').getValue()) {
        case 953850000:
            temp1 = "AA";
            break;
        case 953850001:
            temp1 = "AE";
            break;
        case 953850002:
            temp1 = "AP";
            break;
        default:
            break;
    }
    vetSearchCtx.parameters['militaryPostalTypeCode'] = temp1;

    vetSearchCtx.parameters['provinceName'] = NN(ga('va_provincename').getValue());
    vetSearchCtx.parameters['territoryName'] = NN(ga('va_territoryname').getValue());

    vetSearchCtx.parameters['stateCode'] = (Xrm.Page.getAttribute("va_spousestatelist").getSelectedOption() == null ? '' : Xrm.Page.getAttribute("va_spousestatelist").getSelectedOption().text);
    vetSearchCtx.parameters['zipCode'] = NN(ga('va_spousezipcode').getValue());

    var spouseDOB = ga('va_spousedob').getValue();
    if (spouseDOB && spouseDOB.toString().length > 0) { spouseDOB = new Date(Date.parse(spouseDOB)).format('MM/dd/yyyy'); }
    vetSearchCtx.parameters['spouseBirthDate'] = spouseDOB;
    vetSearchCtx.parameters['cityName'] = NN(ga('va_spousecity').getValue());

    // make country code 3 digits
    var country = ga('va_spousecountry').getValue();
    if (country == null) {
        country = '';
    }
    var cU = country.toUpperCase(),
		isUs = (cU === 'US' || cU === 'USA' || cU === 'U.S.A.' || cU === 'UNITED STATES' || cU === 'UNITED STATES OF AMERICA'),
	    isDomestic = (isUs === true || country === '');

    if (country && cU === 'US') { country = 'USA'; }
    if (country && country.length > 0 && country.length < 3) {
        alert('Please make sure that Country Name is at least 3 characters long.');
        return false;
    }
    if (isUs) { country = country.toUpperCase(); }
    else if (country) {
        // Defect 96137 For Country code, other than USA, use the full name but make it Upper and lower e.g. France or Sri Lanka
        country = NormalizeCountry(country);
    }

    // Issue 87240, do not allow NONE for country code
    if (country.toUpperCase() === 'NONE') {
        alert('Invalid country code.  Please use blank field instead of NONE.');
        return false;
    }
    if (country && country.length > 0 && country.length < 3) {
        alert('Please make sure that Country Name is at least 3 characters long.');
        return false;
    }

    //ticket 104329
    //if (!isUs) { vetSearchCtx.parameters['spouseForeignPostalCode'] = NN(ga('va_spouseforeignpostalcode').getValue()); }
    vetSearchCtx.parameters['spouseForeignPostalCode'] = '';

    var requireds, domOrIntlAddress = (isUs || ga('va_spouseaddresstype').getText() === 'Domestic' || ga('va_spouseaddresstype').getText() === 'International');
    /*
        //ticket 99328 - removing city/country from the list of required fields for non us addresses    
        if (domOrIntlAddress) {
            //ticket 105650 - city is not mandatory for all addresses    
            requireds = ['va_spouseaddress1', 'va_spousecity', 'va_spousecountry'];
        } else {
            requireds = ['va_spouseaddress1'];
        }
    */
    if (ga('va_spouseaddresstype').getText() === 'Domestic') {
        requireds = ['va_spouseaddress1', 'va_spousecity', 'va_spousecountry', 'va_spousestatelist'];
    } else if (ga('va_spouseaddresstype').getText() === 'International') {
        requireds = ['va_spouseaddress1', 'va_spousecity', 'va_spousecountry'];
    } else if (ga('va_spouseaddresstype').getText() === 'Overseas Military') {
        requireds = ['va_spouseaddress1', 'va_spouseoverseasmilitarypostalcode', 'va_spouseoverseasmilitarypostofficetypecode'];
    } else {
        requireds = ['va_spouseaddress1'];
    }


    //   if (domOrIntlAddress) {
    var error = CheckDomIntRequireds(requireds);

    if (error != null) {
        alert(error);
        return false;
    }
    //    }
    //    else {
    //        for (var i = 0; i < requireds.length; i++) {
    //            if (!ga(requireds[i]).getValue() || ga(requireds[i]).getValue().length == 0) {
    //                alert('Following fields are required for MOD:\n\nAddress 1');
    //                gc(requireds[i]).setDisabled(false);
    //                return false;
    //            }
    //        }
    //    }

    //If treasury fields are blank, update them  *** commented out to allow auto update of treasury fields when update mod is clicked, per 152560
    //requireds = [ga('va_treasuryaddress1').getValue(), ga('va_treasuryaddress2').getValue(), ga('va_treasuryaddress3').getValue()];
    //for (i = 0; i < requireds.length; i++) {
    //    if (!requireds[i] || requireds[i].length == 0) {
    updateTreasuryFields();
    //        break;
    //    }
    //}

    //let's check the fields again
    requireds = [ga('va_treasuryaddress1').getValue(), ga('va_treasuryaddress2').getValue(), ga('va_treasuryaddress3').getValue()];
    for (i = 0; i < requireds.length; i++) {
        if (!requireds[i] || requireds[i].length == 0) {
            gc('va_treasuryaddress1').setDisabled(false);
            gc('va_treasuryaddress2').setDisabled(false);
            gc('va_treasuryaddress3').setDisabled(false);
            alert('Unable to generate values the following required fields:\n\Treasury Address 1; Treasury Address 2; Treasury Address 3 \n\n\Please populate the fields manually.');
            return false;
        }
    }

    // prompt changed on 10/26/12
    //var modPrompt = "WARNING: You are about to run the MOD process for the selected veteran. Please read the following IDs back to the caller and confirm: \n\n Veteran's File No: " + NN(ga('va_filenumber').getValue()) + "\n Spouse's SSN: " + NN(ga('va_spousessn').getValue()) + "\n\nThe address you provided will be used for letters and MOD awards. Click OK to process or Cancel to make any changes and then Click on submit MOD/NOK/Letters button.";
    var modPrompt = 'The address entered will be used for the MOD payment (widow’s address) or NOK Letter (Veteran’s address of record). Click "OK"  to process or "Cancel" to make any changes. After making any changes, Click the "Update Treasury Address" and "Submit MOD/NOK/Letters" buttons';
    if (!confirm(modPrompt)) {
        return false;
    }

    vetSearchCtx.parameters['countryName'] = country;

    vetSearchCtx.parameters['treasuryMailingAddressLine1'] = NN(ga('va_treasuryaddress1').getValue());
    vetSearchCtx.parameters['treasuryMailingAddressLine2'] = NN(ga('va_treasuryaddress2').getValue());
    vetSearchCtx.parameters['treasuryMailingAddressLine3'] = NN(ga('va_treasuryaddress3').getValue());
    vetSearchCtx.parameters['treasuryMailingAddressLine4'] = NN(ga('va_treasuryaddress4').getValue());
    vetSearchCtx.parameters['treasuryMailingAddressLine5'] = NN(ga('va_treasuryaddress5').getValue());
    vetSearchCtx.parameters['treasuryMailingAddressLine6'] = NN(ga('va_treasuryaddress6').getValue());

    var updateMod = new updateMonthOfDeath(vetSearchCtx);
    updateMod.executeRequest();
    CloseProgress();

    ga('va_updatemonthofdeathrequest').setValue(NN(updateMod.wsMessage.xmlRequest));
    ga('va_updatemonthofdeathresponse').setValue(NN(updateMod.wsMessage.xmlResponse));

    var dt = new Date();

    var modError = updateMod.wsMessage.errorFlag, desc = '', resp = updateMod.wsMessage.xmlResponse;
    if (modError) {
        desc = 'Mid-tier components reported this error: ' + NN(updateMod.wsMessage.description);
    } else if (resp && resp.length > 0) {
        if (resp.indexOf('Month of Death not processed') >= 0 || resp.indexOf('ns2:ShareException') >= 0) {
            modError = true;
            desc = 'Please review MOD Response box in Web Service Response section for the detailed error description.';
        }
    }

    if (modError) {
        ga('va_modrequeststatus').setValue(NN(updateMod.wsMessage.description).substring(0, 2000));
        //ga('va_modresults').setValue('Failure: (' + dt.toDateString() + ') ' + updateMOD.wsMessage.description);
        alert('Update MOD web service failed to execute.\n\n' + desc);
        return false;
    }

    ga('va_modrequeststatus').setValue("A Month of Death record has been processed successfully.");
    //ga('va_modresults').setValue('Success (' + dt.toDateString() + ')');
    Xrm.Page.ui.tabs.get('general_tab').sections.get('execution_results').setVisible(true);
    DisableAll();
    ga('va_ranmod').setValue(true);

    alert("A Month of Death record has been processed successfully. Please click OK to save the record.");

    Xrm.Page.data.entity.save("save");
    return true;
}

function CheckDomIntRequireds(requireds) {
    var ga = Xrm.Page.getAttribute;
    var gc = Xrm.Page.getControl;
    var missingFields = [];
    var error = null;

    for (var i = 0; i < requireds.length; i++) {
        if (!ga(requireds[i]).getValue() || ga(requireds[i]).getValue().length == 0) {
            missingFields.push(requireds[i]);
            gc(requireds[i]).setDisabled(false);
        }
    }

    if (missingFields.length > 0) {
        error = 'Following fields are required for MOD:\n\n';
        for (var i = 0; i < missingFields.length; i++) {
            if (i != 0) {
                error += '; ';
            }

            if (missingFields[i] == 'va_spouseaddress1') {
                error += 'Address 1';
            }
            else if (missingFields[i] == 'va_spousecity') {
                error += 'City';
            }
            else if (missingFields[i] == 'va_spousecountry') {
                error += 'Country';
            }
            else if (missingFields[i] == 'va_spouseoverseasmilitarypostalcode') {
                error += 'Overseas Military Postal Code';
            }
            else if (missingFields[i] == 'va_spouseoverseasmilitarypostofficetypecode') {
                error += 'Overseas Military Post Office Type Code';
            }
            else if (missingFields[i] == 'va_spousestatelist') {
                error += 'State';
            }
        }

        return error;
    }
}

function ExecuteCorpBIRLSSync(vetSearchCtx, alertOK) {
    //debugger;
    var sync = new syncCorporateBirls(vetSearchCtx);
    sync.suppressProgressDlg = true;
    sync.executeRequest();
    Xrm.Page.getAttribute('va_synccorpandbirlsresponse').setValue(sync.responseXml);


    if (sync.wsMessage.errorFlag) {
        alert('Sync Corp and BIRLS web service had failed to execute correctly.\n\nMid-tier components reported this error: ' + sync.wsMessage.description);
        return false;
    }

    Xrm.Page.getAttribute('va_ransync').setValue(true);
    if (alertOK) {
        var message = sync.wsMessage.description;
        var x = _XML_UTIL.parseXmlObject(sync.responseXml);
        if (SingleNodeExists(x, '//return')) {
            message = x.selectSingleNode('//return').text;
        }
    }
    return true;
}

function ValidateMOD() {

    //debugger;

    Xrm.Page.getAttribute('va_modeligible').setValue(false);

    var vetSearchCtx = new vrmContext();
    if (_UserSettings == null || _UserSettings == undefined) { _UserSettings = GetUserSettingsForWebservice(); }
    vetSearchCtx.user = _UserSettings;
    vetSearchCtx.parameters['fileNumber'] = Xrm.Page.getAttribute('va_filenumber').getValue();
    var findMOD = new findMonthOfDeath(vetSearchCtx);
    findMOD.executeRequest();
    CloseProgress();

    Xrm.Page.getAttribute('va_findmonthofdeathrequest').setValue(NN(findMOD.wsMessage.xmlRequest));

    if (findMOD.wsMessage.errorFlag) {
        alert('Find MOD web service failed to execute.\n\nMid-tier components reported this error: ' + findMOD.wsMessage.description);
        return '';
    }

    Xrm.Page.getControl('va_modeligible').setVisible(true);
    Xrm.Page.getAttribute('va_ranfindmod').setValue(true);

    var mod_xmlObject = _XML_UTIL.parseXmlObject(findMOD.responseXml);
    //debugger;

    //if response doesnt contain "Not eligible" and parsed xml has a node called eligibleIndicator set to Y, then continue as eligible
    var isEligible = (findMOD.responseXml.indexOf("Not Eligible") == -1 &&
	(SingleNodeExists(mod_xmlObject, '//return/eligibleIndicator') ? mod_xmlObject.selectSingleNode('//return/eligibleIndicator').text : null) == 'Y');

    if (!isEligible) {
        return 'Not Eligible';
    }

    Xrm.Page.getAttribute('va_modeligible').setValue(true);

    //If eligible - make some of the MOD fields manadatory
    SetMandatoryFields('reset');
    SetMandatoryFields('MOD');

    if ((SingleNodeExists(mod_xmlObject, '//return/spouseExistsIndicator') ? mod_xmlObject.selectSingleNode('//return/spouseExistsIndicator').text : null) == 'Y') {
        Xrm.Page.getAttribute('va_veteranhassurvivingspouse').setValue(true);
        Xrm.Page.getAttribute('va_survivingspouseisvalidformod').setValue(true);
        RedrawSpouseFields('No selection');
    } else {
        Xrm.Page.getAttribute('va_veteranhassurvivingspouse').setValue(false);
        Xrm.Page.getAttribute('va_survivingspouseisvalidformod').setValue(false);
        RedrawSpouseFields('NOK');
        Xrm.Page.getControl('va_spouserecordaction').removeOption(953850001); // if no spouse exists, remove Modify option
        Xrm.Page.getAttribute('va_spouserecordaction').setValue(953850002); //default to NOK
    }

    RetrieveSpouseInfo(mod_xmlObject);

    //retrieving key information from findMOD to be sent back when running UpdateMOD
    var monthOfDeathTranId = SingleNodeExists(mod_xmlObject, '//return/monthOfDeathTranId') ? mod_xmlObject.selectSingleNode('//return/monthOfDeathTranId').text : null;

    var soj = SingleNodeExists(mod_xmlObject, '//return/soj') ? mod_xmlObject.selectSingleNode('//return/soj').text : null;

    var vetPtcpntId = SingleNodeExists(mod_xmlObject, '//return/vetPtcpntId') ? mod_xmlObject.selectSingleNode('//return/vetPtcpntId').text : null;

    var spousePtcpntId = SingleNodeExists(mod_xmlObject, '//return/spousePtcpntId') ? mod_xmlObject.selectSingleNode('//return/spousePtcpntId').text : null;

    if (monthOfDeathTranId) { Xrm.Page.getAttribute('va_modtranid').setValue(monthOfDeathTranId); }
    if (soj) { Xrm.Page.getAttribute('va_modsoj').setValue(soj); }
    if (vetPtcpntId) { Xrm.Page.getAttribute('va_modvetptcpntid').setValue(vetPtcpntId); }
    if (spousePtcpntId) { Xrm.Page.getAttribute('va_modspouseptcpntid').setValue(spousePtcpntId); }

    Xrm.Page.ui.tabs.get("mod_tab").sections.get("spouse_section0").setVisible(true);
    Xrm.Page.ui.tabs.get("mod_tab").sections.get("spouse_section1").setVisible(true);
    Xrm.Page.ui.tabs.get("mod_tab").sections.get("spouse_section2").setVisible(true);
    Xrm.Page.ui.tabs.get("mod_tab").sections.get("spouse_section3").setVisible(true);
    Xrm.Page.ui.tabs.get("mod_tab").sections.get("spouse_section4").setVisible(true);
    Xrm.Page.ui.tabs.get("mod_tab").sections.get("spouse_section5").setVisible(true);
    Xrm.Page.ui.tabs.get("mod_tab").sections.get("spouse_section5a").setVisible(true);
    Xrm.Page.ui.tabs.get("mod_tab").sections.get("spouse_section5b").setVisible(true);

    var ta = Xrm.Page.ui.tabs.get("treasuryadd");
    ta.setVisible(true);
    ta.sections.get("spouse_section6").setVisible(true);
    ta.sections.get("spouse_section7").setVisible(true);

    Xrm.Page.ui.tabs.get("mod_tab").sections.get("mod_button_section").setVisible(true);

    return 'Eligible';
}

function GetCountryList(vetSearchCtx) {
    try {
        var getCountryList = new findCountries(vetSearchCtx);
        getCountryList.suppressProgressDlg = true;
        getCountryList.executeRequest();
        //CloseProgress();

        CountryList_xmlObject = _XML_UTIL.parseXmlObject(getCountryList.responseXml);
        returnNode = CountryList_xmlObject.selectNodes('//return');
        CountryListNodes = returnNode[0].childNodes;
        var oOption = document.createElement("option");;

        if (CountryListNodes) {
            for (var i = 0; i < CountryListNodes.length; i++) {         //looping through countries and
                if (CountryListNodes[i].nodeName == 'types') {  //making sure we dont check irrelevant nodes

                    oOption.value = parseInt(CountryListNodes[i].selectSingleNode('code').text);
                    oOption.text = CountryListNodes[i].selectSingleNode('name').text;

                    Xrm.Page.getControl('va_spousecountrylist').addOption(oOption);
                }
            }
        }
    }
    catch (ced) {
        //debugger;
    }

}

function RetrieveSpouseInfo(mod_xmlObject) {

    var spouseFirstName = SingleNodeExists(mod_xmlObject, '//return/spouseFirstName')
		? mod_xmlObject.selectSingleNode('//return/spouseFirstName').text : null;

    var spouseMiddleName = SingleNodeExists(mod_xmlObject, '//return/spouseMiddleName')
		? mod_xmlObject.selectSingleNode('//return/spouseMiddleName').text : null;

    var spouseLastName = SingleNodeExists(mod_xmlObject, '//return/spouseLastName')
		? mod_xmlObject.selectSingleNode('//return/spouseLastName').text : null;

    var spouseSuffix = SingleNodeExists(mod_xmlObject, '//return/spouseSuffix')
		? mod_xmlObject.selectSingleNode('//return/spouseSuffix').text : null;

    var spouseSsn = SingleNodeExists(mod_xmlObject, '//return/spouseSsn')
		? mod_xmlObject.selectSingleNode('//return/spouseSsn').text : null;

    var spouseDateOfBirth = SingleNodeExists(mod_xmlObject, '//return/spouseDateOfBirth')
		? mod_xmlObject.selectSingleNode('//return/spouseDateOfBirth').text : null;

    var nationalStateCemetery = SingleNodeExists(mod_xmlObject, '//return/nationalStateCemetery')
		? mod_xmlObject.selectSingleNode('//return/nationalStateCemetery').text : null; /* jg */


    var spouseAddressLine1 = SingleNodeExists(mod_xmlObject, '//return/spouseAddressLine1')
		? mod_xmlObject.selectSingleNode('//return/spouseAddressLine1').text : null;

    var spouseAddressLine2 = SingleNodeExists(mod_xmlObject, '//return/spouseAddressLine2')
		? mod_xmlObject.selectSingleNode('//return/spouseAddressLine2').text : null;

    var spouseAddressLine3 = SingleNodeExists(mod_xmlObject, '//return/spouseAddressLine3')
		? mod_xmlObject.selectSingleNode('//return/spouseAddressLine3').text : null;

    var spouseCity = SingleNodeExists(mod_xmlObject, '//return/spouseCity')
		? mod_xmlObject.selectSingleNode('//return/spouseCity').text : null;

    var spouseState = SingleNodeExists(mod_xmlObject, '//return/spouseState')
		? mod_xmlObject.selectSingleNode('//return/spouseState').text : null;

    var spouseZipCode = SingleNodeExists(mod_xmlObject, '//return/spouseZipCode')
		? mod_xmlObject.selectSingleNode('//return/spouseZipCode').text : null;

    var spouseCountryTypeName = SingleNodeExists(mod_xmlObject, '//return/spouseCountryTypeName')
		? mod_xmlObject.selectSingleNode('//return/spouseCountryTypeName').text : null;

    var spouseForeignPostalCode = SingleNodeExists(mod_xmlObject, '//return/spouseForeignPostalCode')
		? mod_xmlObject.selectSingleNode('//return/spouseForeignPostalCode').text : null;

    var spouseMilitaryPostOfficeCode = SingleNodeExists(mod_xmlObject, '//return/spouseMilitaryPostOfficeCode')
		? mod_xmlObject.selectSingleNode('//return/spouseMilitaryPostOfficeCode').text : null;  //APO, DPO, FPO

    var spouseMilitaryPostalTypeCode = SingleNodeExists(mod_xmlObject, '//return/spouseMilitaryPostalTypeCode')
		? mod_xmlObject.selectSingleNode('//return/spouseMilitaryPostalTypeCode').text : null; //AA, AE, AP

    var spouseProvinceName = SingleNodeExists(mod_xmlObject, '//return/spouseProvinceName')
		? mod_xmlObject.selectSingleNode('//return/spouseProvinceName').text : null;

    var spouseTeritoryName = SingleNodeExists(mod_xmlObject, '//return/spouseTeritoryName')
		? mod_xmlObject.selectSingleNode('//return/spouseTeritoryName').text : null;


    Xrm.Page.getAttribute('va_spousefirstname').setValue(NN(spouseFirstName));
    Xrm.Page.getAttribute('va_spousemiddlename').setValue(NN(spouseMiddleName));
    Xrm.Page.getAttribute('va_spouselastname').setValue(NN(spouseLastName));
    Xrm.Page.getAttribute('va_spousesuffix').setValue(NN(spouseSuffix));
    Xrm.Page.getAttribute('va_spousessn').setValue(NN(spouseSsn));
    var newDOB = null;
    if (spouseDateOfBirth && spouseDateOfBirth.length > 0) { newDOB = new Date(spouseDateOfBirth); }
    Xrm.Page.getAttribute('va_spousedob').setValue(newDOB);

    if (nationalStateCemetery == "N") {
        Xrm.Page.getAttribute('va_isnationalorstatecemetery').setValue(false); /* jg */
    }
    else {
        Xrm.Page.getAttribute('va_isnationalorstatecemetery').setValue(true); /* jg */
    }

    Xrm.Page.getAttribute('va_spouseaddress1').setValue(NN(spouseAddressLine1));
    Xrm.Page.getAttribute('va_spouseaddress2').setValue(NN(spouseAddressLine2));
    Xrm.Page.getAttribute('va_spouseaddress3').setValue(NN(spouseAddressLine3));
    Xrm.Page.getAttribute('va_spousecity').setValue(NN(spouseCity));
    //Xrm.Page.getAttribute('va_spousestate').setValue(spouseState); //useless field

    if (spouseState) {
        var stateOptions = Xrm.Page.getAttribute('va_spousestatelist').getOptions();

        var length = stateOptions.length;

        for (i = 0; i < length; i++) {
            if (stateOptions[i].text == spouseState) {
                Xrm.Page.getAttribute('va_spousestatelist').setValue(stateOptions[i].value);
                break;
            }
        }
    }

    Xrm.Page.getAttribute('va_spousezipcode').setValue(NN(spouseZipCode));
    //Xrm.Page.getAttribute('va_spousecountry').setValue(NN(spouseCountryTypeName));

    var cName = NN(spouseCountryTypeName);
    if (cName && cName.length > 0) { cName = cName.toString().toUpperCase(); }
    Xrm.Page.getAttribute('va_spousecountry').setValue(cName);

    Xrm.Page.getAttribute('va_spouseforeignpostalcode').setValue(NN(spouseForeignPostalCode));
    Xrm.Page.getAttribute('va_provincename').setValue(NN(spouseProvinceName));
    Xrm.Page.getAttribute('va_territoryname').setValue(NN(spouseTeritoryName));

    //Updating treasury fields

    updateTreasuryFields();

    //end of treasury field update

    var temp = null;

    switch (spouseMilitaryPostalTypeCode) {
        case 'AA':
            temp = 953850000;
            break;
        case 'AE':
            temp = 953850001;
            break;
        case 'AP':
            temp = 953850002;
            break;
        default:
            break;
    }

    if (temp) {
        Xrm.Page.getAttribute('va_spouseoverseasmilitarypostalcode').setValue(temp);  //AA, AE, AP
    }

    temp = null;

    switch (spouseMilitaryPostOfficeCode) {
        case 'APO':
            temp = 953850000;
            break;
        case 'DPO':
            temp = 953850001;
            break;
        case 'FPO':
            temp = 953850002;
            break;
        default:
            break;
    }

    if (temp) {
        Xrm.Page.getAttribute('va_spouseoverseasmilitarypostofficetypecode').setValue(temp);  //APO, DPO, FPO 
    }


    if (spouseMilitaryPostOfficeCode || spouseMilitaryPostalTypeCode) {
        Xrm.Page.getAttribute('va_spouseaddresstype').setValue(953850002); //overseas
    } else {

        if (spouseCountryTypeName == null || spouseCountryTypeName == '' || spouseCountryTypeName == 'USA') {
            Xrm.Page.getAttribute('va_spouseaddresstype').setValue(953850000); //domestic
        } else {
            Xrm.Page.getAttribute('va_spouseaddresstype').setValue(953850001); //international
        }
    }
}

//This function updates GUI/enforces business rules based on the dropdown "If spouse info is incorrect, please select from the following"
function RedrawSpouseFields(mode) {

    //Redraw dropdown, items might have been removed
    Xrm.Page.getControl('va_spouserecordaction').clearOptions();

    for (var i = 1; i <= spouse_action_options.length; i++) {
        if (spouse_action_options[spouse_action_options.length - i].value != "null") {
            Xrm.Page.getControl('va_spouserecordaction').addOption(spouse_action_options[spouse_action_options.length - i], i - 1);
        }
    }

    switch (mode) {
        case 'No selection':
            Xrm.Page.getControl('va_spousefirstname').setDisabled(true);
            Xrm.Page.getControl('va_spousemiddlename').setDisabled(true);
            Xrm.Page.getControl('va_spouselastname').setDisabled(true);
            Xrm.Page.getControl('va_spousesuffix').setDisabled(true);
            Xrm.Page.getControl('va_spousessn').setDisabled(true);
            Xrm.Page.getControl('va_spousedob').setDisabled(true);
            break;
        case 'Add':
            Xrm.Page.getControl('va_spousefirstname').setDisabled(false);
            Xrm.Page.getControl('va_spousemiddlename').setDisabled(false);
            Xrm.Page.getControl('va_spouselastname').setDisabled(false);
            Xrm.Page.getControl('va_spousesuffix').setDisabled(false);
            Xrm.Page.getControl('va_spousessn').setDisabled(false);
            Xrm.Page.getControl('va_spousedob').setDisabled(false);
            Xrm.Page.getAttribute('va_spouserecordaction').setValue(953850000); //set the value of the dropdown after the redraw
            break;
        case 'Modify':
            Xrm.Page.getControl('va_spousefirstname').setDisabled(false);
            Xrm.Page.getControl('va_spousemiddlename').setDisabled(false);
            Xrm.Page.getControl('va_spouselastname').setDisabled(false);
            Xrm.Page.getControl('va_spousesuffix').setDisabled(false);
            Xrm.Page.getControl('va_spousessn').setDisabled(false);
            Xrm.Page.getControl('va_spousedob').setDisabled(false);
            Xrm.Page.getAttribute('va_spouserecordaction').setValue(953850001); //set the value of the dropdown after the redraw
            break;
        case 'NOK':
            Xrm.Page.getAttribute('va_spousefirstname').setValue(null);
            Xrm.Page.getAttribute('va_spousemiddlename').setValue(null);
            Xrm.Page.getAttribute('va_spouselastname').setValue(null);
            Xrm.Page.getAttribute('va_spousesuffix').setValue(null);
            Xrm.Page.getAttribute('va_spousessn').setValue(null);
            Xrm.Page.getAttribute('va_spousedob').setValue(null);

            Xrm.Page.getControl('va_spousefirstname').setDisabled(true);
            Xrm.Page.getControl('va_spousemiddlename').setDisabled(true);
            Xrm.Page.getControl('va_spouselastname').setDisabled(true);
            Xrm.Page.getControl('va_spousesuffix').setDisabled(true);
            Xrm.Page.getControl('va_spousessn').setDisabled(true);
            Xrm.Page.getControl('va_spousedob').setDisabled(true);
            Xrm.Page.getAttribute('va_spouserecordaction').setValue(953850002); //set the value of the dropdown after the redraw
            break;
        default:
            break;
    }
}

function updateTreasuryFields() {
    var ta = Xrm.Page.ui.tabs.get("treasuryadd");
    ta.setVisible(true);
    ta.sections.get("spouse_section6").setVisible(true);
    ta.sections.get("spouse_section7").setVisible(true);

    var spouseFirstName = Xrm.Page.getAttribute('va_spousefirstname').getValue();
    var spouseMiddleName = Xrm.Page.getAttribute('va_spousemiddlename').getValue();
    var spouseLastName = Xrm.Page.getAttribute('va_spouselastname').getValue();
    var spouseSuffix = Xrm.Page.getAttribute('va_spousesuffix').getValue();
    var spouseSsn = Xrm.Page.getAttribute('va_spousessn').getValue();
    var spouseDateOfBirth = new Date(Xrm.Page.getAttribute('va_spousedob').getValue());
    var spouseAddressLine1 = Xrm.Page.getAttribute('va_spouseaddress1').getValue();
    var spouseAddressLine2 = Xrm.Page.getAttribute('va_spouseaddress2').getValue();
    var spouseAddressLine3 = Xrm.Page.getAttribute('va_spouseaddress3').getValue();
    var spouseCity = Xrm.Page.getAttribute('va_spousecity').getValue();
    var spouseState = (Xrm.Page.getAttribute("va_spousestatelist").getSelectedOption() == null ? '' : Xrm.Page.getAttribute("va_spousestatelist").getSelectedOption().text);
    var spouseZipCode = Xrm.Page.getAttribute('va_spousezipcode').getValue();
    var spouseCountryTypeName = Xrm.Page.getAttribute('va_spousecountry').getValue();
    if (spouseCountryTypeName && spouseCountryTypeName.length > 0) {
        spouseCountryTypeName = spouseCountryTypeName.toString().toUpperCase();
    }
    var spouseAddressType = (Xrm.Page.getAttribute("va_spouseaddresstype").getSelectedOption() == null ? '' : Xrm.Page.getAttribute("va_spouseaddresstype").getSelectedOption().text);
    var spouseMilitaryOfficeTypeCode = (Xrm.Page.getAttribute("va_spouseoverseasmilitarypostofficetypecode").getSelectedOption() == null ? '' : Xrm.Page.getAttribute("va_spouseoverseasmilitarypostofficetypecode").getSelectedOption().text); //APO
    var spouseMilitaryPostalCode = (Xrm.Page.getAttribute("va_spouseoverseasmilitarypostalcode").getSelectedOption() == null ? '' : Xrm.Page.getAttribute("va_spouseoverseasmilitarypostalcode").getSelectedOption().text); //AA

    var treasuryAddressArray = new Array(6);
    var addr2Present = false;
    var addr3Present = false;

    spouseFirstName = NN(spouseFirstName);
    spouseMiddleName = NN(spouseMiddleName);
    spouseLastName = NN(spouseLastName);

    //calculate full name
    var spouseFullName = "";
    spouseFullName = spouseFirstName;
    if (spouseMiddleName && spouseMiddleName.length > 0) spouseFullName += " " + spouseMiddleName;
    spouseFullName += " " + spouseLastName;

    spouseAddressLine1 = NN(spouseAddressLine1);
    spouseAddressLine2 = NN(spouseAddressLine2);
    spouseAddressLine3 = NN(spouseAddressLine3);
    spouseCity = NN(spouseCity);
    spouseState = NN(spouseState);
    spouseZipCode = NN(spouseZipCode);
    spouseCountryTypeName = NN(spouseCountryTypeName);

    //debugger;

    // per Wayne Dahlsing 6/14/12, zip code must NOT be in treasury fields
    var useZipCode = false;

    var j = 0;

    if ((!spouseFirstName && spouseFirstName.length == 0) || (!spouseLastName && spouseLastName.length == 0)) {
        spouseFullName = 'No Spouse';
    }

    treasuryAddressArray[j] = spouseFullName.substring(0, 20);

    if (spouseFullName && spouseFullName.length > 20) {
        j++;
        treasuryAddressArray[j] = spouseFullName.substring(20, 40);
    }

    j++;
    treasuryAddressArray[j] = spouseAddressLine1.substring(0, 20);

    if (spouseAddressLine1 && spouseAddressLine1.length > 20) {
        j++;
        treasuryAddressArray[j] = spouseAddressLine1.substring(20, 40);
    }

    if (spouseAddressLine2 && spouseAddressLine2.length > 0) addr2Present = true;
    if (spouseAddressLine3 && spouseAddressLine3.length > 0) addr3Present = true;

    // if either ADDR2 or ADDR3 are present, deal with formatting.
    if (addr2Present || addr3Present) {

        if (addr2Present && !addr3Present) {
            treasuryAddressArray[j + 1] = spouseAddressLine2.substring(0, 20);

            if (spouseAddressType == 'Overseas Military') {
                treasuryAddressArray[j + 2] = (spouseMilitaryOfficeTypeCode + " " + spouseMilitaryPostalCode).substring(0, 20);
                j = j + 2;
            } else {
                treasuryAddressArray[j + 2] = (spouseCity + " " + spouseState).substring(0, 20);
                //ticket 104329.. appending country name for int addresses
                if (spouseCountryTypeName && spouseCountryTypeName.length > 0 && spouseCountryTypeName != 'USA') {
                    treasuryAddressArray[j + 3] = spouseCountryTypeName;
                    j = j + 3;
                } else {
                    j = j + 2;
                }
            }
        }

        if (!addr2Present && addr3Present) {
            treasuryAddressArray[j + 1] = spouseAddressLine3.substring(0, 20);

            if (spouseAddressType == 'Overseas Military') {
                treasuryAddressArray[j + 2] = (spouseMilitaryOfficeTypeCode + " " + spouseMilitaryPostalCode).substring(0, 20);
                j = j + 2;
            } else {
                treasuryAddressArray[j + 2] = (spouseCity + " " + spouseState).substring(0, 20);
                //ticket 104329.. appending country name for int addresses
                if (spouseCountryTypeName && spouseCountryTypeName.length > 0 && spouseCountryTypeName != 'USA') {
                    treasuryAddressArray[j + 3] = spouseCountryTypeName;
                    j = j + 3;
                } else {
                    j = j + 2;
                }
            }
        }

        if (addr2Present && addr3Present) {
            treasuryAddressArray[j + 1] = spouseAddressLine2.substring(0, 20);
            treasuryAddressArray[j + 2] = spouseAddressLine3.substring(0, 20);

            if (spouseAddressType == 'Overseas Military') {
                treasuryAddressArray[j + 3] = (spouseMilitaryOfficeTypeCode + " " + spouseMilitaryPostalCode).substring(0, 20);
                j = j + 3;
            } else {
                treasuryAddressArray[j + 3] = (spouseCity + " " + spouseState).substring(0, 20);
                //ticket 104329.. appending country name for int addresses
                if (spouseCountryTypeName && spouseCountryTypeName.length > 0 && spouseCountryTypeName != 'USA') {
                    treasuryAddressArray[j + 4] = spouseCountryTypeName;
                    j = j + 4;
                } else {
                    j = j + 3;
                }
            }
        }

    } else {

        if (spouseAddressType == 'Overseas Military') {
            treasuryAddressArray[j + 1] = (spouseMilitaryOfficeTypeCode + " " + spouseMilitaryPostalCode).substring(0, 20);
            j = j + 1;
        } else {
            treasuryAddressArray[j + 1] = (spouseCity + " " + spouseState).substring(0, 20);
            //ticket 104329.. appending country name for int addresses
            if (spouseCountryTypeName && spouseCountryTypeName.length > 0 && spouseCountryTypeName != 'USA') {
                treasuryAddressArray[j + 2] = spouseCountryTypeName;
                j = j + 2;
            } else {
                j = j + 1;
            }
        }

    }

    if (j >= 6) {
        alert("Warning: Unable to convert treasury address to the 6x20 format. Please update it manually.");
    }

    Xrm.Page.getAttribute('va_treasuryaddress1').setValue(treasuryAddressArray[0]);
    Xrm.Page.getAttribute('va_treasuryaddress2').setValue(treasuryAddressArray[1]);
    Xrm.Page.getAttribute('va_treasuryaddress3').setValue(treasuryAddressArray[2]);
    Xrm.Page.getAttribute('va_treasuryaddress4').setValue(treasuryAddressArray[3]);
    Xrm.Page.getAttribute('va_treasuryaddress5').setValue(treasuryAddressArray[4]);
    Xrm.Page.getAttribute('va_treasuryaddress6').setValue(treasuryAddressArray[5]);
}

function ValidateMODAddress() {
    if (ValidateZipcode() == false) {
        return false;
    }

    var success = ValidateMODFields();
    if (success) {
        alert("Validation Succeeded");
    }
}

function ValidateZipcode() {
    var va_spousezipcode = Xrm.Page.getAttribute('va_spousezipcode').getValue();
    if (va_spousezipcode != null && va_spousezipcode.match(/[a-zA-Z]/)) {
        alert('Spouse/Last Known Address Zip Code field contains invalid alphabetical characters');
        return false;
    }

    return true;
}

function TestForAllowedChars(text, nums) {
    var chars = {
        1: "[~]+",
        2: "[`]+",
        3: "[!]+",
        4: "[@]+",
        5: "[#]+",
        6: "[$]+",
        7: "[%]+",
        8: "[\\^]+",
        9: "[&]+",
        10: "[*]+",
        11: "[(]+",
        12: "[)]+",
        13: "[_]+",
        14: "[-]+",
        15: "[+]+",
        16: "[=]+",
        17: "[|]+",
        18: "[\\\\]+",
        19: "[}]+",
        20: "[]]+",
        21: "[{]+",
        22: "[[]+",
        23: "[']+",
        24: "[\"]+",
        25: "[:]+",
        26: "[;]+",
        27: "[\\/]+",
        28: "[?]+",
        29: "[.]+",
        30: "[>]+",
        31: "[,]+",
        32: "[<]+",
        33: "[  ]{2,}"
    };


    for (var remove in nums) {
        chars[nums[remove]] = null;
    }

    var success;
    for (var invalid in chars) {
        if (chars[invalid] != null) {
            var myregex = new RegExp(chars[invalid]);
            success = !myregex.test(text);
        }
        if (success == false) break;
    }
    return success;
}

function ValidateMODFields() {
    var Errors = {};

    var fields = {
        va_spouseaddress1: 35,
        va_spouseaddress2: 35,
        va_spouseaddress3: 35,
        va_spousecity: 30,
        va_spousezipcode: 5
    };

    //Applies the max length restriction to fields.
    for (var field in fields) {
        var input = Xrm.Page.getAttribute(field).getValue();
        if (input && input.length > fields[field]) {
            Errors[Xrm.Page.getControl(field).getLabel()] = "Payment field length surpassed max value of " + fields[field] + ";";
            //Xrm.Page.getAttribute(field).setValue(input.substring(0, ValidationFields[field]));
        }
        if (Xrm.Page.getControl(field).getLabel() == "Zip Code") {
            var zipSize = Xrm.Page.getAttribute(field).getValue();
            if (zipSize && zipSize.length != 5) {
                if (Errors[Xrm.Page.getControl(field).getLabel()] != undefined) {
                    Errors[Xrm.Page.getControl(field).getLabel()] += "Field must be exactly 5 characters;";
                }
                else {
                    Errors[Xrm.Page.getControl(field).getLabel()] = "Field must be exactly 5 characters;";
                }
            }
        }
        if (Xrm.Page.getControl(field).getLabel() == "Address 1") {
            var success = TestForAllowedChars(Xrm.Page.getAttribute(field).getValue(), [14, 27]);
            if (success == false) {
                if (Errors[Xrm.Page.getControl(field).getLabel()] != undefined) {
                    Errors[Xrm.Page.getControl(field).getLabel()] += "Field can only contain alphanumeric characters, dashes, slashes, and single spaces;";
                }
                else {
                    Errors[Xrm.Page.getControl(field).getLabel()] = "Field can only contain alphanumeric characters, dashes, slashes, and single spaces;";
                }
            }
        }

        var success = TestForAllowedChars(Xrm.Page.getAttribute(field).getValue(), [33, 24, 5, 7, 9, 23, 11, 12, 15, 31, 14, 29, 27, 25, 4]);
        if (success == false) {
            if (Errors[Xrm.Page.getControl(field).getLabel()] != undefined) {
                Errors[Xrm.Page.getControl(field).getLabel()] += "Field contains a non-allowable character;";
            }
            else {
                Errors[Xrm.Page.getControl(field).getLabel()] = "Field contains a non-allowable character;";
            }
        }
    }

    if (parseInt(Xrm.Page.getAttribute("va_spousezipcode").getValue()) == NaN) {
        if (Errors[Xrm.Page.getControl("va_spousezipcode").getLabel()] != undefined) {
            Errors[Xrm.Page.getControl("va_spousezipcode").getLabel()] += 'Must be a number;';
        }
        else {
            Errors[Xrm.Page.getControl("va_spousezipcode").getLabel()] = 'Must be a number;';
        }
    }
    /*
    if (parseInt(Xrm.Page.getAttribute("va_paymentzipcode").getValue()) != Xrm.Page.getAttribute("va_paymentzipcode").getValue()) {
    if (Errors[Xrm.Page.getControl("va_paymentzipcode").getLabel()] != undefined) {
    Errors[Xrm.Page.getControl("va_paymentzipcode").getLabel()] += 'Contains an invalid character;';
    }
    else {
    Errors[Xrm.Page.getControl("va_paymentzipcode").getLabel()] = 'Contains an invalid character;';
    }
    }*/


    var text = "";
    for (var a in Errors) {
        text += a + ": " + Errors[a] + "\n";
    }
    if (text != "") {
        alert("Validation Failed: \n\n" + text);
        return false;
    }
    else {
        return true;
    }
}

function NN(s) { return (s == null ? '' : s); }

function NormalizeCountry(country) {
    var words = country.split(' '), parsedCountry = '';
    for (var i = 0, l = words.length; i < l; i++) {
        // skip parent in a word
        var s = words[i].toLowerCase(), index = (s.substr(0, 1) === '(' ? 1 : 0), s1 = s.substr(index, 1).toUpperCase();
        words[i] = (index > 0 ? '(' : '') + s1 + s.substr(index + 1, words[i].length - 1 - index);

        parsedCountry += words[i] + (i < l - 1 ? ' ' : '');
    }
    return parsedCountry;
}

function usaaddress() {
    var va_spouseaddresstype = Xrm.Page.getAttribute("va_spouseaddresstype").getValue();

    if (va_spouseaddresstype == 953850000) { //Domestic
        $('#va_spousecountrylist option:contains("USA")').attr('selected', 'selected');

        Xrm.Page.getControl('va_spousecity').setVisible(true);
        Xrm.Page.getControl('va_spousestatelist').setVisible(true);
        Xrm.Page.getControl('va_spousezipcode').setVisible(true);
        Xrm.Page.getAttribute("va_spousecountry").setValue("USA");
        Xrm.Page.getControl('va_spousecountry').setVisible(true);
        Xrm.Page.getControl('va_spousecountrylist').setVisible(true);
        Xrm.Page.getAttribute('va_spouseoverseasmilitarypostalcode').setValue(null);
        Xrm.Page.getControl('va_spouseoverseasmilitarypostalcode').setVisible(false);
        Xrm.Page.getAttribute('va_spouseoverseasmilitarypostofficetypecode').setValue(null);
        Xrm.Page.getControl('va_spouseoverseasmilitarypostofficetypecode').setVisible(false);
    }
    else if (va_spouseaddresstype == 953850001) { //International
        Xrm.Page.getControl('va_spousecity').setVisible(true);
        Xrm.Page.getAttribute('va_spousestatelist').setValue(null);
        Xrm.Page.getControl('va_spousestatelist').setVisible(false);
        Xrm.Page.getAttribute('va_spousezipcode').setValue(null);
        Xrm.Page.getControl('va_spousezipcode').setVisible(false);
        Xrm.Page.getControl('va_spousecountry').setVisible(true);
        Xrm.Page.getControl('va_spousecountrylist').setVisible(true);
        Xrm.Page.getAttribute('va_spouseoverseasmilitarypostalcode').setValue(null);
        Xrm.Page.getControl('va_spouseoverseasmilitarypostalcode').setVisible(false);
        Xrm.Page.getAttribute('va_spouseoverseasmilitarypostofficetypecode').setValue(null);
        Xrm.Page.getControl('va_spouseoverseasmilitarypostofficetypecode').setVisible(false);
    }
    else if (va_spouseaddresstype == 953850002) { //Overseas Military
        Xrm.Page.getAttribute('va_spousecity').setValue(null);
        Xrm.Page.getControl('va_spousecity').setVisible(false);
        Xrm.Page.getAttribute('va_spousestatelist').setValue(null);
        Xrm.Page.getControl('va_spousestatelist').setVisible(false);
        //Xrm.Page.getAttribute('va_spousezipcode').setValue(null);
        //Xrm.Page.getControl('va_spousezipcode').setVisible(false);
        Xrm.Page.getControl('va_spousezipcode').setVisible(true);
        Xrm.Page.getAttribute('va_spousecountry').setValue(null);
        Xrm.Page.getControl('va_spousecountry').setVisible(false);
        Xrm.Page.getAttribute('va_spousecountrylist').setValue(null);
        Xrm.Page.getControl('va_spousecountrylist').setVisible(false);
        Xrm.Page.getControl('va_spouseoverseasmilitarypostalcode').setVisible(true);
        Xrm.Page.getControl('va_spouseoverseasmilitarypostofficetypecode').setVisible(true);
    }
}
