function createAndOpenServiceRequest(action) {
 // CSDev Left Intentionally Blank 
};

function USDCreateLetterOrServiceRequest(idpid, interactionid, vetid, targetEntity, searchType, data) {
 // CSDev Left Intentionally Blank 
}

// This method will be call from CRM form
/* function OnLoad() {
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

function onFormLoad(executionObj) {

    webResourceUrl = Xrm.Page.context.getClientUrl() + '/WebResources/va_';

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
                        alert('FNOD successful. Please proceed to PMC (if requested) - not MOD eligible.');
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
 */

var webResourceUrl = null, parent_page = null, page_opener = null,
   spouse_action_options = null, //to save dropdown options in case things get removed and need to be refreshed
   _country_list = [],
   _country_list_USA = 0,
   _doHaveCorpRecord = false,
   _ranFnodThisTime = false,
   _modPrompt = false,
   _redrawing_spouse_action = false;


function addbeforeunloadEvent(func) {
// CSDev Left Intentionally Blank 
}

function ValidateFnodCompletion() {
// CSDev Left Intentionally Blank 
}

function SetMandatoryFields(type) {
// CSDev Left Intentionally Blank 

}

function ExecuteFNOD() {
// CSDev Left Intentionally Blank 
}

function IssuePMC() {
// CSDev Left Intentionally Blank 
}

function CopyPMC() {
// CSDev Left Intentionally Blank 
}

function RetrieveAddressesFromParent(parent_page) {

// CSDev Left Intentionally Blank 
}

function CopyLastKnownAddressToSpouseFields() {
// CSDev Left Intentionally Blank 
}

function RetrieveVetInfo() {
// CSDev Left Intentionally Blank 

}

function DisableAll() {
// CSDev Left Intentionally Blank 
}

function DisableMODFields() {
 // CSDev Left Intentionally Blank 

}

function ExecuteMOD() {
 // CSDev Left Intentionally Blank 
}

function CheckDomIntRequireds(requireds) {
// CSDev Left Intentionally Blank 
}

function ExecuteCorpBIRLSSync(vetSearchCtx, alertOK) {
 // CSDev Left Intentionally Blank 
}

function ValidateMOD() {
// CSDev Left Intentionally Blank 
}

function GetCountryList(vetSearchCtx) {
// CSDev Left Intentionally Blank 

}

function RetrieveSpouseInfo(mod_xmlObject) {
// CSDev Left Intentionally Blank 
}

//This function updates GUI/enforces business rules based on the dropdown "If spouse info is incorrect, please select from the following"
function RedrawSpouseFields(mode) {
// CSDev Left Intentionally Blank 
}

function updateTreasuryFields() {
 // CSDev Left Intentionally Blank 
}

function ValidateMODAddress() {
    if (ValidateZipcode() == false) {
        return false;
    }

    var success = ValidateMODFields();
    if (success) {
        Va.Udo.Crm.Scripts.Popup.MsgBox("Validation Succeeded", Va.Udo.Crm.Scripts.Popup.PopupStyles.Information, "Validation Success");
    }
}

function ValidateZipcode() {
    var va_spousezipcode = Xrm.Page.getAttribute('va_spousezipcode').getValue();
    if (va_spousezipcode != null && va_spousezipcode.match(/[a-zA-Z]/)) {
        var message = 'Spouse/Last Known Address Zip Code field contains invalid alphabetical characters';
        Va.Udo.Crm.Scripts.Popup.MsgBox(message, Va.Udo.Crm.Scripts.Popup.PopupStyles.Exclamation, "Warning");
        return false;
    }
    return true;
}

function TestForAllowedChars(text, nums) {
 // CSDev Left Intentionally Blank 
}

function ValidateMODFields() {
  // CSDev Left Intentionally Blank 
}

function NN(s) { return (s == null ? '' : s); }

function NormalizeCountry(country) {
  // CSDev Left Intentionally Blank 
}

function usaaddress() {
// CSDev Left Intentionally Blank 
}

function setupForm(executionObj) {
  // CSDev Left Intentionally Blank
}