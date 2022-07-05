/// <reference path="../Intellisense/XrmPage-vsdoc.js" />
// This method will be call from CRM form
function OnLoad() {
    // Reports for the ribbon button
    //reports.getReports(); //Added back into Button WR
    environmentConfigurations.initalize();
    commonFunctionsVip.initalize();
    ws.mapDDevelopmentNotes.initalize();
    // RU12 Form
    onFormLoad();
}
dateClosed = null;
reqStatus = null;
reqStatusVal = null;
_loading = true;
_parentPage = null;
_sendMailButtonClicked = false;
_sendEmailsThroughCode = null;
_runningEmailGenAfterAutoSave = null;
_originalRO = null;
_isECC = false;

// Begin Form onload event - retrieve data from VIP if necessary

function onFormLoad() {
    _runningEmailGenAfterAutoSave = false;
    _sendEmailsThroughCode = true;
    Xrm.Page.getControl("va_action").removeOption(953850008);

    var ro = (status == 'Sent' || status == 'Resolved' || status == 'Cancelled' || status == 'Queued to Send');

    if (Xrm.Page.ui.getFormType() != CRM_FORM_TYPE_CREATE) {
        var contactId = Xrm.Page.getAttribute('va_relatedveteranid').getValue() ? Xrm.Page.getAttribute('va_relatedveteranid').getValue()[0].id : null;

        if (contactId) {
            RestrictPCRAccess(contactId, function (restricAccess) {
                if (restricAccess) {
                    return;
                } else {
                    serviceRequest();
                }
            });
        } else {
            serviceRequest();
        }
    } else {
        var actionSelectedOption = Xrm.Page.getAttribute('va_action').getSelectedOption();
        if (actionSelectedOption != null) {
            var action = actionSelectedOption.text;
            if (action == 'Email Forms') {
                var sojrpo = Xrm.Page.getAttribute('va_sojrpo').getValue();
                if (sojrpo.indexOf('SOJ') != -1) {
                    var sojCode = sojrpo.substr(5, 3);

                    resultSet = CrmRestKit2011.ByQueryAll('va_regionaloffice', ['va_name', 'va_regionalofficeId'], "va_Code eq '" + sojCode + "' ", false);
                    resultSet.fail(function (err) {
                        UTIL.restKitError(err, 'Failed to retrieve regional office response:');
                    })
                    .done(function (data) {
                        if (data && data.length > 0) {
                            Xrm.Page.getAttribute('va_regionalofficeid').setValue([{ id: data[0].va_regionalofficeId, name: data[0].va_name, entityType: 'va_regionaloffice' }]);
                        }
                    });
                }
            }

        }
        serviceRequest();

        if ((Xrm.Page.getAttribute('va_disposition').getValue() == 953850085) || (Xrm.Page.getAttribute('va_disposition').getValue() == 953850217)) {
            Xrm.Page.getAttribute("va_action").setValue(1);
        }
    }
}

function serviceRequest() {

    /** Set the custom labels and the onchange events **/
    Xrm.Page.getControl('va_deathrelatedinformationchecklists').setLabel('Answered questions concerning possible benefit entitlements referring to "Death Related Information Checklist" work aid');
    Xrm.Page.getControl('va_benefitsstopfirstofmonth').setLabel('Advised the caller the benefits will be stopped the first of the month of death and that any payment issued following that date must be returned (if applicable)');
    Xrm.Page.getControl('va_willroutereportofdeath').setLabel('Will route this report of death to Regional Office of Jurisdiction or PMC via encrypted email for stop payment processing');

    //Adding Functions to OnChange:
    Xrm.Page.getAttribute('va_quickwriteid').addOnChange(QuickWriteChange);
    Xrm.Page.getAttribute('va_action').addOnChange(ServiceTypeChange);
    Xrm.Page.getAttribute('va_action').addOnChange(EmailtoVet);
    Xrm.Page.getAttribute('va_relatedveteranid').addOnChange(RelatedVetChange);
    Xrm.Page.getAttribute('va_soj').addOnChange(SOJChange);
    Xrm.Page.getAttribute('va_requeststatus').addOnChange(StatusChange);
    Xrm.Page.getAttribute("va_alltrackeditemsreceivedorclosed").addOnChange(va_AllTrackedItemsReceivedOrClosedChange);
    Xrm.Page.getAttribute('va_processedfnodinshare').addOnChange(ProcessedInShareChange);
    Xrm.Page.getAttribute('va_other').addOnChange(FnodOtherChange);

    fnMPCheckAll();
    //Set Service Request Number
    if (Xrm.Page.getAttribute('va_reqnumber').getValue() == null) {
        getUniqueRequestNumber();
    }
    //Set Date Opened
    setSRFields();


    if (Xrm.Page.getAttribute('va_servicerequesttype') != null) {
        if (Xrm.Page.getAttribute('va_servicerequesttype').getValue() === 'ECC') {
            Xrm.Page.ui.tabs.get("mailing_address_tab").sections.get("ecc_information").setVisible(true);
        }
    }

    _isECC = (UserHasRole("ECC Case Manager") || UserHasRole("ECC Phone Tech"));
    if (_isECC == true) {
        if (UserHasRole("ECC Case Manager")) {
            Xrm.Page.getAttribute('va_ecctitle').setValue('Case Manager');
        }
        else {
            Xrm.Page.getAttribute('va_ecctitle').setValue('Phone Tech');
        }
    }

    setRPOText();
    /** End event setting **/

    dateClosed = Xrm.Page.getAttribute("va_dateclosed").getValue();
    reqStatus = Xrm.Page.getAttribute("va_requeststatus").getSelectedOption().text;
    reqStatusVal = Xrm.Page.getAttribute("va_requeststatus").getValue();

    if (reqStatus == 'Sent') { Xrm.Page.getControl("va_requeststatus").setDisabled(true); }
    var webResourceUrl = Xrm.Page.context.getServerUrl() + '/WebResources/va_';

    // If new record, retrieve data from VIP application
    //    if (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_CREATE) {
    //        RetrieveServiceRequestData();
    //    }

    //Events to happen when Service Request is New or Existing
    if (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_CREATE || Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_UPDATE) {

        //Show Script button
        window.GeneralToolbar = new InlineToolbar("va_script");
        GeneralToolbar.AddButton("btnScript", "Show Script", "60%", ShowScript, webResourceUrl + 'status_online.png');

        //Send E-mail button
        window.GeneralToolbar = new InlineToolbar("va_robutton");
        GeneralToolbar.AddButton("btnGetRO", (_sendEmailsThroughCode ? "Send E-mail" : "Set Service Request as Sent"), "60%", SendEmailToRO, webResourceUrl + 'email_go.png');

        //Enclosures button
        window.GeneralToolbar = new InlineToolbar("va_enclosuresbutton");
        GeneralToolbar.AddButton("btnEnclo", "Populate Enclosures", "60%", PopulateEnclosures, webResourceUrl + 'arrow_refresh.png');

        //Multiple Payment button
        window.GeneralToolbar = new InlineToolbar("va_mpbutton");
        GeneralToolbar.AddButton("btnMP", "Filter Payments", "60%", fnMP, webResourceUrl + 'BankandroutingNoLookupIcon_App');

        //Copy veteran's address
        window.GeneralToolbar = new InlineToolbar("va_copymailingaddress");
        GeneralToolbar.AddButton("btnMP", "Copy Mailing Address", "60%", copyMailingAddress, '');

        //If SOJ field is blank, promt the user
        if (Xrm.Page.getAttribute('va_action').getValue() != null) {
            var action = Xrm.Page.getAttribute('va_action').getSelectedOption().text;
            if (action != 'Email Forms') {
                if (!Xrm.Page.getAttribute('va_regionalofficeid').getValue()) {
                    if (Xrm.Page.getAttribute('va_eccssn').getValue() != null) {
                        alert('Warning: RPO is empty.\n\nTo select RPO manually, click on the Lookup icon next to Send To box on the top of the screen.');
                    }
                    else {
                        alert('Warning: SOJ is empty.\n\nTo select SOJ manually, click on the Lookup icon next to Send To box on the top of the screen.');
                    }
                }
            }
        }
    } else { Xrm.Page.getControl("va_robutton").setVisible(false); }


    ServiceTypeChange();
    StatusChange();
    fnFNODtype();
    setVeteranName();

    _originalRO = UTIL.GetLookupId('va_regionalofficeid');
    _loading = false;

    //If loading after SAVE, check if need to resume Send E-mail op based on QS params
    if (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_UPDATE) {
        var sendEmail = false;
        if (window.parent && window.parent.SEMSet && window.parent.SEMSet) {
            var id = Xrm.Page.data.entity.getId().toString();
            for (var i = 0; i < window.parent.SEMSet.length; i++) {
                if (window.parent.SEMSet[i] == id) {
                    _runningEmailGenAfterAutoSave = true;
                    window.parent.SEMSet[i] = '';
                    sendEmail = true;
                    break;
                }
            }
        }
        if (sendEmail) {
            SendEmailToRO();
        }
    }
}
/*******************************END ONLOAD EVENT*****************************************/

//Retrieve data from VIP
//function RetrieveServiceRequestData() {
//    var searchIframe = null,
//        parentWindow;

//    if (window.parent && window.parent.opener && window.parent.opener.Xrm.Page.data) {
//        parentWindow = window.parent.opener;
//    }
//    else {
//        parentWindow = window.top.opener.parent;
//    }

//    if (parentWindow.Xrm.Page.data.entity.getEntityName() == "phonecall") {
//        searchIframe = parentWindow.document.getElementById('IFRAME_search').contentWindow;
//    }
//    else if (parentWindow.Xrm.Page.data.entity.getEntityName() == "contact") {
//        searchIframe = parentWindow.document.getElementById('IFRAME_ro').contentWindow;
//    }
//    if (searchIframe) {
//        searchIframe._appFireEvent('createcrmservicerequest', this);
//    }
//    else return null;
//}
function EmailtoVet() {
    if (Xrm.Page.getAttribute('va_action').getValue() != null) {
        if (Xrm.Page.getAttribute('va_action').getValue() == 953850004) {
            Xrm.Page.getAttribute('va_sendemailtoveteran').setValue(true);
        }
        else {
            Xrm.Page.getAttribute('va_sendemailtoveteran').setValue(false);
        }
    }
}

//This is the callback function that VIP calls to return the data
function ReturnServiceRequestData(data) {
    for (var d in data) {
        if (data[d] == null || data[d] == undefined) continue;
        if (Xrm.Page.getAttribute(d.toLowerCase()).getAttributeType() == "optionset") {
            Xrm.Page.getAttribute(d.toLowerCase()).setValue(data[d].Value);
        }
        else if (Xrm.Page.getAttribute(d.toLowerCase()).getAttributeType() == "datetime") {
            Xrm.Page.getAttribute(d.toLowerCase()).setValue(new Date(data[d]));
        }
        else if (Xrm.Page.getAttribute(d.toLowerCase()).getAttributeType() == "lookup") {
            Xrm.Page.getAttribute(d.toLowerCase()).setValue([{
                id: data[d].Id,
                name: data[d].Name,
                entityType: data[d].LogicalName
            }]);
        }
        else if (Xrm.Page.getAttribute(d.toLowerCase()).getAttributeType() == "money") {
            Xrm.Page.getAttribute(d.toLowerCase()).setValue(parseFloat(data[d].Value));
        }
        else {
            Xrm.Page.getAttribute(d.toLowerCase()).setValue(data[d]);
        }
    }
    Xrm.Page.getAttribute('va_originatingcallid').setSubmitMode('always');
    Xrm.Page.getAttribute('va_relatedveteranid').setSubmitMode('always');
}

function ShowHideOLB() {
    //var ctrl = Xrm.Page.ui.controls.get('WebResource_AttachReport');
    //ctrl.setVisible(!ctrl.getVisible());
    window.GeneralToolbar.RemoveButton("btnGetRO");
    _sendEmailsThroughCode = !_sendEmailsThroughCode;

    var webResourceUrl = Xrm.Page.context.getServerUrl() + '/WebResources/va_';
    window.GeneralToolbar.AddButton("btnGetRO",
	_sendEmailsThroughCode ? "Send E-mail" : "Set Service Request as Sent", "60%", SendEmailToRO, webResourceUrl + 'email_go.png');
}

function StatusChange() {
    // if sent or cancelled, cannot change record
    var status = Xrm.Page.getAttribute("va_requeststatus").getSelectedOption().text;

    var sending = (status == 'Sent' || status == 'Queued to Send');
    var ro = (sending || status == 'Resolved' || status == 'Cancelled');

    var controls = ["va_action", "va_address1", "va_address2", "va_alltrackeditemsreceivedorclosed", "va_branchofservice", "va_city", "va_claim", "va_claimdetails", "va_compensationclaim", "va_dateclosed", "va_dateoffirstnotice", "va_dateopened", "va_description", "va_email", "va_enroutetova", "va_inquireristheveteran", "va_nameofreportingindividual", "va_originatingcallid", "va_participantid", "va_payment", "va_pensionclaim", "va_placeofdeath", "va_quickwriteid", "va_readscript", "va_regionalofficeid", "va_relatedveteranid", "va_relationshipdetails", "va_relationtoveteran", "va_reqnumber", "va_responsiblecontact", "va_robutton", "va_soj", "va_ssn", "va_state", "va_subtypeofresponse", "va_issue", "va_typerofresponse", "va_update", "va_zipcode", "va_specialsituationid", "va_hasfiduciary", "va_haspoa", "va_fiduciarydata", "va_poadata", "va_benefitsstopped", "va_lookedupvetrecord", "va_deathrelatedinformationchecklist", "va_processedfnodinshare", "va_processedfnodinshareexplanation", "va_pmc", "va_nokletter", "va_21530", "va_21534", "va_401330", "va_other", "va_otherspecification", "va_dependentinformation", "va_dependentnames", "va_dependentaddresses"];

    for (var i = 0; i < controls.length; i++) {
        var cc = Xrm.Page.getControl(controls[i]);
        try {
            if (cc) {
                cc.setDisabled(ro);
            }
        } catch (ec) {
        }
    }

    // only admins can change status
    // If not 0820, PCRs can change it to Resolved, if current status allows
    var action = (Xrm.Page.getAttribute("va_action").getSelectedOption() == null ? '' : Xrm.Page.getAttribute("va_action").getSelectedOption().text);
    var vai0820 = (action == '0820' || action == '0820a' || action == '0820f' || action == '0820d' || action == '0820 & VAI' || action == 'VAI');
    if (!_loading) {
        var canChangeStatus = (UserHasRole("System Administrator") || UserHasRole("Supervisor") || (!vai0820 && reqStatus != 'Sent' && reqStatus != 'Queued to Send'));
        if (!canChangeStatus) {
            alert("Combination of your security role and current service request status does not allow you to modify status; use 'Send Email' button to send Service Request and attached reports via email.");
            Xrm.Page.getAttribute("va_requeststatus").setValue(reqStatusVal);
            return;
        }
    }

    if ((status == 'Resolved' || status == 'Cancelled')) {
        if (!Xrm.Page.getAttribute("va_dateclosed").getValue()) {
            Xrm.Page.getAttribute("va_dateclosed").setValue(new Date());
        }
        Xrm.Page.getControl("va_dateclosed").setDisabled(false);
    } else if (!_loading && (status == "Queued to Send" || status == "Sent")) {
        alert("'" + status + "' cannot be set manually; use 'Send Email' button instead and/or workflow processing rules.");
        Xrm.Page.getAttribute("va_requeststatus").setValue(reqStatusVal);
        ro = (reqStatus == 'Sent' || reqStatus == 'Resolved' || reqStatus == 'Cancelled');
        for (var i = 0; i < controls.length; i++) {
            var cc = Xrm.Page.getControl(controls[i]);
            if (cc) {
                cc.setDisabled(ro);
            }
        }
    } else {
        if (!_loading && reqStatus != 'Resolved' && reqStatus != 'Cancelled' && Xrm.Page.getAttribute("va_dateclosed").getValue()) {
            Xrm.Page.getAttribute("va_dateclosed").setValue(null);
            Xrm.Page.getAttribute("va_dateclosed").setSubmitMode('always');
        }
    }
}

function doesControlHaveAttribute(control) {
    var controlType = control.getControlType();
    return controlType != "iframe" && controlType != "webresource" && controlType != "subgrid";
}

function disableFormFields(onOff) {
    Xrm.Page.ui.controls.forEach(function (control, index) {
        if (doesControlHaveAttribute(control)) {
            control.setDisabled(onOff);
        }
    });
}
//Multiple Payment functions
//MP Main function
function fnMP() {
    fnMPCheckAll();
    fnMPdate();
    if (Xrm.Page.getAttribute("va_mpend").getValue() < Xrm.Page.getAttribute("va_mpstart").getValue()) {
        //clear out the display
        varDisplay = 'Invalid date range. Please verify that the start date is before the end date.';
        Xrm.Page.getAttribute("va_mpdisplay").setValue(varDisplay);
    }
    else {
        fnMPsort();
    }
}

//MP checkbox function
function fnMPCheckAll() {
    if (Xrm.Page.getAttribute("va_mpall").getValue() == '1') {
        Xrm.Page.getControl("va_mpstart").setVisible(false);
        Xrm.Page.getControl("va_mpend").setVisible(false);
        fnMPsort();
    }
    else {
        Xrm.Page.getControl("va_mpstart").setVisible(true);
        Xrm.Page.getControl("va_mpend").setVisible(true);
    }
}
//MP default dates if nothing is entered(6 months ago - today)
function fnMPdate() {
    varToday = new Date();
    //If no start, yes end
    if ((Xrm.Page.getAttribute("va_mpstart").getValue() == null) && (Xrm.Page.getAttribute("va_mpend").getValue() !== null)) {
        varAdd = new Date(Xrm.Page.getAttribute("va_mpend").getValue());
        varAdd.setDate(1);
        varAdd.setMonth(varAdd.getMonth() - 6);
        Xrm.Page.getAttribute("va_mpstart").setValue(varAdd);
    }
    //If no start, no end (catch)
    if (Xrm.Page.getAttribute("va_mpstart").getValue() == null) {
        varBegin = new Date();
        varBegin.setDate(1);
        varBegin.setMonth(varToday.getMonth() - 6);
        Xrm.Page.getAttribute("va_mpstart").setValue(varBegin);
    }
    //If no end
    if (Xrm.Page.getAttribute("va_mpend").getValue() == null) {
        Xrm.Page.getAttribute("va_mpend").setValue(varToday);
    }
    //Validate start is before end
    if (Xrm.Page.getAttribute("va_mpend").getValue() < Xrm.Page.getAttribute("va_mpstart").getValue()) {
        varValid = new Date(Xrm.Page.getAttribute("va_mpend").getValue());
        //varValid.setDate(1);
        //Xrm.Page.getAttribute("va_mpstart").setValue(varValid);
        alert('Invalid date range. Please verify that the start date is before the end date.');
    }
}
//MP need to sort through the data now.
function fnMPsort() {
    varBegin = new Date(Xrm.Page.getAttribute("va_mpstart").getValue());
    varBegin.setHours(0);
    varEnd = new Date(Xrm.Page.getAttribute("va_mpend").getValue()).setHours(0);
    //varEnd.setHours(0);
    varDisplay = "";

    if (Xrm.Page.getAttribute("va_mpraw").getValue() !== null) {
        //var SampleString = "1/1/2011_$101.00,2/1/2011_$201.00,3/1/2011_$301.00,3/23/2011_$323.00,4/1/2011_$401.00,5/1/2011_$501.00";
        var SampleString = new String(Xrm.Page.getAttribute("va_mpraw").getValue());
        if (SampleString.substring(0, 9) == "undefined") {
            SampleString = SampleString.substring(9, SampleString.length);
        }

        var SampleString2 = SampleString.substring(0, SampleString.length - 1);
        //set array to the data to be assigned
        var checkArray = SampleString2.split("#");
        //split each selected check into [payment date, amount] array
        var checkArray2d = new Array();
        for (var i = 0; i < checkArray.length; i++) {
            checkArray2d[i] = checkArray[i].split("_");
        }

        //Filter if "all" check is not true
        if (Xrm.Page.getAttribute("va_mpall").getValue() == '0') {
            for (var i = 0; i < checkArray2d.length; i++) {
                varCurrPayDate = new Date(checkArray2d[i][0]).setHours(0);

                if ((varCurrPayDate >= varBegin) && (varCurrPayDate <= varEnd)) {
                    var varFormatDate = mpFormatDate(varCurrPayDate);
                    varDisplay += varFormatDate + "\t\t $" + parseFloat(checkArray2d[i][1]).toFixed(2) + "\n";
                }
            }
        }
        else {
            for (var i = 0; i < checkArray2d.length; i++) {

                var varFormatDate = mpFormatDate(checkArray2d[i][0]);
                varDisplay += varFormatDate + "\t\t $" + parseFloat(checkArray2d[i][1]).toFixed(2) + "\n";
            }
        }
    }
    else {
        varDisplay += "No Payment History on the Service Request record to search.  If there is Payment History on the Phone Call, try creating a new Service Request."
    }

    Xrm.Page.getAttribute("va_mpdisplay").setValue(varDisplay);
}
function mpFormatDate(varDate) {
    var varInput = new Date(varDate);
    var varFormat = "";
    var varM = varInput.getMonth() + 1;
    if (varM < 10) {
        varFormat = "0" + varM + "/";
    }
    else {
        varFormat = "" + varM + "/";
    }
    var varD = varInput.getDate();
    if (varD < 10) {
        varFormat += "0" + varD + "/";
    }
    else {
        varFormat += varD + "/";
    }
    var varY = varInput.getFullYear();
    varFormat += varY;
    return varFormat;
}

function PopulateEnclosures() {
    enclosures = '';
    var columns = ['va_externaldocumentid'];
    var filter = "(va_servicerequestid eq guid'" + Xrm.Page.data.entity.getId() + "')";
    CrmRestKit2011.ByQuery('va_va_servicerequest_va_externaldocument', columns, filter, false)
    .fail(function (err) {
        UTIL.restKitError(err, 'Failed to retrieve service request external document data');
    })
    .done(function (data) {
        if (data && data.d.results.length > 0) {
            for (var i = 0; i < data.d.results.length; i++) {
                var r = data.d.results[i].va_externaldocumentid;

                columns = ['va_name', 'va_DocumentLocation'];
                filter = "(va_externaldocumentId eq guid'" + r + "')";

                CrmRestKit2011.ByQuery('va_externaldocument', columns, filter, false)
                    .fail(function (err) {
                        UTIL.restKitError(err, 'Failed to retrieve external document data');
                    })
                    .done(function (data) {
                        if (data && data.d.results.length > 0) {
                            enclosures += (enclosures.length > 0 ? '\n' : '') + data.d.results[0].va_name;
                        }
                    });
            }
        }
    });

    if (Xrm.Page.getAttribute('va_enclosures').getValue() != enclosures) {
        Xrm.Page.getAttribute('va_enclosures').setValue(enclosures);
        // update data to make sure enclosures get on report
        if (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_UPDATE) {
            CrmRestKit2011.Update('va_servicerequest', Xrm.Page.data.entity.getId(), { va_Enclosures: 'enclosures' })
                .fail(function (err) {
                    UTIL.restKitError(err, 'Failed to update servicerequest enclosures');
                })
                .done(function (data, status, xhr) {
                });
        }
    }
}


function SendEmailToRO() {
    if (Xrm.Page.ui.getFormType() == 1) {
        alert('You have to save the form before proceeding.');
        return;
    }

    if (Xrm.Page.getAttribute("va_requeststatus").getSelectedOption().text == 'Sent') {
        if (!confirm('Current Request Status is "Sent". Please confirm that you would like to send an email again.')) {
            return;
        }
    }
    var roId = UTIL.GetLookupId('va_regionalofficeid');

    // control TO USE SERVER-SIDE SENDING
    if (_sendEmailsThroughCode) {
        //    if (!confirm('Send e-mail to the selected recipient?\n\nScreen will close and e-mail message will be created.\n\nReopen the Service Request to see e-mails created.')) return;
    }

    var action = Xrm.Page.getAttribute("va_action").getSelectedOption().text;
    var is0820 = (action == '0820' || action == '0820a' || action == '0820f' || action == '0820d' || action == '0820 & VAI' || action == 'VAI' || action == 'Email Forms');

    Xrm.Page.getControl("va_update").setDisabled(false);
    Xrm.Page.getAttribute("va_update").setSubmitMode('always');
    if (is0820) {
        var script0 = 'Please read the following script to the Veteran and click on OK button to confirm that the script was read:\n\n\n I need to read the following statement to you.\n\nI am a VA employee who is authorized to receive or request evidentiary information or statements that may result in a change in your VA benefits. The primary purpose for gathering this information or statement is to make an eligibility determination. It is subject to verification through computer matching programs with other agencies.';
        if (action == '0820d') script0 += '\n\nIf the original check is found or received, you must return the original check to the Treasury Department and await receipt of the replacement check. If both checks are negotiated, then you will be responsible for the duplicate payment. You will receive a letter from Debt Management Center with instructions concerning collection.';

        // send mail could be running after automatic save. In this case, script was already prompted
        if (!_runningEmailGenAfterAutoSave) {
            if (!confirm(script0)) return;
        }

        var rs = Xrm.Page.getAttribute('va_readscript').getValue();
        if (rs == null || rs == false) Xrm.Page.getAttribute('va_readscript').setValue(true);

        //Console App will pick these service requests, run report and email them out.
        if (_sendEmailsThroughCode) {
            // uncomment to use server side queue manager
            //Xrm.Page.getAttribute("va_requeststatus").setValue(953850007); //Queued to Send
        }
    }

    if (_sendEmailsThroughCode) {
        /*
		next line is needed if we use server queue manager and email router
		var val = Xrm.Page.getAttribute("va_update").getValue();
		if (!val)
		val = true;
		else
		val = !val;
		*/
        //if (confirm('Create Outlook e-mail message?')) {
        _sendMailButtonClicked = CreateOutlookEmail2();
        if (!_sendMailButtonClicked) return;
        //}
    }

    var close = true;
    //if (!_sendEmailsThroughCode) {
    close = confirm('Would you like to mark this record as Sent and close it?');
    if (close) {
        _sendMailButtonClicked = true;
        Xrm.Page.getAttribute("va_requeststatus").setValue(953850006); //Sent
    }
    //}
    if (close) {
        Xrm.Page.data.entity.save("saveandclose");
    }
}


function CreateOutlookEmail2() {
    var reportName = ''; // "0820-AB10 Informal Claims Letter - Single"; //set this to the report you are trying to download
    var action = null;
    var dispSubType = Xrm.Page.getAttribute("va_disposition").getText();
    if (dispSubType == undefined) dispSubType = '';
    var arrFiles = null;

    action = Xrm.Page.getAttribute("va_action").getSelectedOption().text;
    switch (action) {
        case "0820":
            reportName = "27-0820 - Report of General Information";
            break;
        case "0820 & VAI":
            reportName = "27-0820 - Report of General Information";
            break;
        case "0820a":
            reportName = "27-0820a - Report of First Notice of Death";
            break;
        case "0820d":
            reportName = "27-0820d - Report of Non-Receipt of Payment";
            break;
        case "0820f":
            reportName = "27-0820f - Report of Month of Death";
            break;
        case "0820ab-10":
            reportName = "0820-AB10 Informal Claims Letter";
            break;
        default:
            break;
    }
    var doCreatePDF = true;

    var isAB10 = false;
    if (dispSubType != undefined && dispSubType != null && dispSubType.length > 0 && dispSubType.toUpperCase() == "CLAIM:INFORMAL - AB-10 LETTER") isAB10 = true;

    if ((!reportName || reportName.length == 0) && !isAB10) {
        doCreatePDF = false;
        //		if (!confirm('According to the selected Action value, no reports need to be generated. Would you like to continue the process and generate new e-mail without Report attachments?\n\n ')) {
        //			return false;
        //		}
    }


    var isDirty = Xrm.Page.data.entity.getIsDirty();

    // get forms attached to email
    var attachmentList = '',
        attachmentListx = '',
		enclosures = '';
    var attachUrlList = new Array();
    // get attachments

    var columns = ['va_externaldocumentid'];
    var filter = "(va_servicerequestid eq guid'" + Xrm.Page.data.entity.getId() + "')";
    CrmRestKit2011.ByQuery('va_va_servicerequest_va_externaldocument', columns, filter, false)
        .fail(function (err) {
            UTIL.restKitError(err, 'Failed to retrieve service request external document data');
        })
        .done(function (data) {
            if (data && data.d.results.length > 0) {
                for (var i = 0; i < data.d.results.length; i++) {
                    var r = data.d.results[i].va_externaldocumentid;

                    columns = ['va_name', 'va_DocumentLocation'];
                    filter = "(va_externaldocumentId eq guid'" + r + "')";

                    CrmRestKit2011.ByQuery('va_externaldocument', columns, filter, false)
                        .fail(function (err) {
                            UTIL.restKitError(err, 'Failed to retrieve external document data');
                        })
                        .done(function (data) {
                            if (data && data.d.results.length > 0) {
                                if (data.d.results[0].va_name != 'Home Loans' && data.d.results[0].va_name != 'Education'
                                    && data.d.results[0].va_name != 'VR&E' && data.d.results[0].va_name != 'Life Insurance' && data.d.results[0].va_name != 'Pension') {
                                    attachmentList += ("<a href='" + data.d.results[0].va_DocumentLocation + "'>" + data.d.results[0].va_name + "</a><br/>");
                                    attachUrlList.push(data.d.results[0].va_DocumentLocation);
                                }
                                else {
                                    attachmentListx += ("<a href='" + data.d.results[0].va_DocumentLocation + "'>" + data.d.results[0].va_name + "</a><br/>");
                                }
                                enclosures += (enclosures.length > 0 ? '\n' : '') + data.d.results[0].va_name;
                            }
                        });
                }
            }
        });

    if (Xrm.Page.getAttribute('va_enclosures').getValue() != enclosures) {
        Xrm.Page.getAttribute('va_enclosures').setValue(enclosures);
        // update data to make sure enclosures get on report
        if (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_UPDATE) {
            CrmRestKit2011.Update('va_servicerequest', Xrm.Page.data.entity.getId(), { va_Enclosures: 'enclosures' }, false)
                .fail(function (err) {
                    UTIL.restKitError(err, 'Failed to update servicerequest enclosures');
                })
				.done(function (data, status, xhr) {
				});
        }
    }

    if (doCreatePDF) {
        if (isDirty) {
            if (confirm('Some of the data on the screen has changed and was not saved to the database. Would you like to save the record before sending e-mail?\n\nIf you select OK, the screen will reload after save, and sending e-mail will resume.\n\nSelect Cancel to skip saving and go ahead with email generation.')) {
                // update parent to restart after save
                // encodeURIComponent('a') window.location.href
                var sem = new Array();
                sem.push(Xrm.Page.data.entity.getId().toString());
                window.parent.SEMSet = sem;
                Xrm.Page.data.entity.save("save");
                return false;
            }
        }

        // call ws to get the attachment file names from shared folder
        var names = '';
        var request = null;
        try {
            request = new ActiveXObject('Microsoft.XMLHTTP');
        } catch (err) { }

        if ((request == null) && window.XMLHttpRequest) {
            request = new XMLHttpRequest();
        } else if (request == null) {
            alert('Exception: Failed to create XML HTTP Object. Failed to execute web service request');
            return false;
        }

        var serviceUri = _currentEnv.RepWS;
        var env = '<?xml version="1.0" encoding="utf-8"?><soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">' + '<soap12:Body><DownloadReport xmlns="http://tempuri.org/">' + '<serviceRequestId>' + Xrm.Page.data.entity.getId() + '</serviceRequestId>' + '<action>' + action.replace('&', '&amp;') + '</action>' + '<dispSubType>' + dispSubType.replace('&', '&amp;') + '</dispSubType>' + '</DownloadReport></soap12:Body></soap12:Envelope>';

        try {
            ShowProgress('Generating Report Output');

            request.open('POST', serviceUri, false);
            request.setRequestHeader('SOAPAction', '');
            request.setRequestHeader('Content-Type', 'text/xml; charset=utf-8');
            request.setRequestHeader('Content-Length', env.length);

            request.send(env);
        } catch (rex) {
            request = null;
            alert('Call to the Web Service to generate the report had failed: ' + rex.description);
            return false;
        } finally {
            CloseProgress();
        }

        names = request.responseText;
        if (names) {
            var rx = parseXmlObject(names);
            var resNode = rx.selectSingleNode('//DownloadReportResult');
            if (!resNode) {
                names = 'Exception: No response from Report Generation WS.';
            } else {
                names = resNode.text;
            }
        }
        request = null;

        if (!names || names.length == 0 || names.indexOf('Exception') != -1) {
            alert('Call to Report Generation Web Service had failed or no reports had been generated.\n\n' + names);
            return false;
        }
        arrFiles = names.split(',');
    }

    var to = '';

    // get RO email
    var sendToRO = true,
		sendToVet = (Xrm.Page.getAttribute('va_sendemailtoveteran').getValue()),
		sojAddress = '';
    var roId = UTIL.GetLookupId('va_regionalofficeid');
    if (!roId) {
        roId = _originalRO;
        sendToRO = false;

        var userId = Xrm.Page.getAttribute('va_pcrofrecordid').getValue()[0].id;
        var siteId = GetAttributeValueFromID('systemuser', userId, 'siteid', 'systemuserid');

        var siteName = GetAttributeValueFromID('site', siteId, 'name', 'siteid'),
			siteAddress1 = GetAttributeValueFromID('site', siteId, 'address1_line1', 'siteid'),
			siteAddress2 = GetAttributeValueFromID('site', siteId, 'address1_line2', 'siteid'),
			siteAddress3 = GetAttributeValueFromID('site', siteId, 'address1_line3', 'siteid'),
			siteCity = GetAttributeValueFromID('site', siteId, 'address1_city', 'siteid'),
			siteState = GetAttributeValueFromID('site', siteId, 'address1_stateorprovince', 'siteid'),
			siteZip = GetAttributeValueFromID('site', siteId, 'address1_postalcode', 'siteid'),
			siteCountry = GetAttributeValueFromID('site', siteId, 'address1_country', 'siteid'),
			siteFax = GetAttributeValueFromID('site', siteId, 'address1_fax', 'siteid');

        sojAddress = (siteName && siteName.length > 0 ? siteName + '<br/>' : '') + (siteAddress1 && siteAddress1.length > 0 ? siteAddress1 + '<br/>' : '') + (siteAddress2 && siteAddress2.length > 0 ? siteAddress2 + '<br/>' : '') + (siteAddress3 && siteAddress3.length > 0 ? siteAddress3 + '<br/>' : '') + (siteCity && siteCity.length > 0 ? siteCity + ', ' : '') + (siteState && siteState.length > 0 ? siteState + ' ' : '') + (siteZip && siteZip.length > 0 ? siteZip + '<br/>' : '') + (siteCountry && siteCountry.length > 0 ? siteCountry + '<br/>' : '') + (siteFax && siteFax.length > 0 ? '<br/>FAX: ' + siteFax : '');
    }

    if (roId) {
        var columns = ['EmailAddress', 'va_Address1', 'va_Address2', 'va_Address3', 'va_City', 'va_FaxNumber', 'va_State', 'va_ZipCode', 'va_name', 'va_Alias'];
        var filter = "(va_regionalofficeId eq guid'" + roId + "')";
        CrmRestKit2011.Retrieve('va_regionaloffice', roId, columns, false)
            .fail(function (err) {
                UTIL.restKitError(err, 'Failed to retrieve regional office data');
            })
            .done(function (data) {
                if (data) {
                    var r = data.d;
                    to = r.EmailAddress;
                    sojAddress = (r.va_Alias && r.va_Alias.length > 0 ? r.va_Alias + '<br/>' : '') + (r.va_Address1 && r.va_Address1.length > 0 ? r.va_Address1 + '<br/>' : '') + (r.va_Address2 && r.va_Address2.length > 0 ? r.va_Address2 + '<br/>' : '') + (r.va_Address3 && r.va_Address3.length > 0 ? r.va_Address3 + '<br/>' : '') + (r.va_City && r.va_City.length > 0 ? r.va_City + ', ' : '') + (r.va_State && r.va_State.length > 0 ? r.va_State + ' ' : '') + (r.va_ZipCode && r.va_ZipCode.length > 0 ? r.va_ZipCode : '') + (r.va_FaxNumber && r.va_FaxNumber.length > 0 ? '<br/>FAX: ' + r.va_FaxNumber : '');
                }
            });
    }

    var s = '<BR/>';
    var newReq = (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_CREATE ? "New " : "");
    var veteran = (Xrm.Page.getAttribute('va_relatedveteranid').getValue() ? Xrm.Page.getAttribute('va_relatedveteranid').getValue()[0].name : 'Unknown');

    var issue = Xrm.Page.getAttribute('va_issue').getText();
    var subject = newReq + "Request for" /*+ "  " + veteran*/
	+ ": " + (!dispSubType || dispSubType.length == 0 ? issue : dispSubType);

    if (sendToVet) {
        if (action == 'Email Forms') {
            subject = 'Requested VA Forms';
        }
        else {
            subject = 'Requested Information from VA';
        }
    }

    var body = '';
    var nonEmergency = (action == 'Non Emergency Email');
    var PCRName = Xrm.Page.getAttribute('va_pcrofrecordid').getValue()[0].name;
    var PCRId = Xrm.Page.getAttribute('va_pcrofrecordid').getValue()[0].id;
    var StationNumber = GetAttributeValueFromID('systemuser', PCRId, 'va_stationnumber', 'systemuserid');
    var PCRNameArray = PCRName.split(',');
    var PCRNameFullName = '';
    if (PCRNameArray.length > 0) {
        PCRNameFullName = PCRNameArray[1] + ' ' + PCRNameArray[0];
    }
    var Description = Xrm.Page.getAttribute('va_description').getValue();
    if (Description == null) {  //ensure no 'null' in email body
        Description = '';
    }

    // For non-emergency email to Vet, using special body
    if ((!nonEmergency || (nonEmergency && sendToVet == false)) && action != 'Email Forms') {
        if (action == '0820' || action == '0820a' || action == '0820f') {
            body = PCRNameFullName + s + 'Station Number ' + StationNumber;
        }
        else {
            if (nonEmergency && sendToVet == false) {
                var cfirstname = Xrm.Page.getAttribute("va_firstname").getValue();
                cfirstname = cfirstname == null ? "" : cfirstname;
                var clastname = Xrm.Page.getAttribute("va_lastname").getValue();
                clastname = clastname == null ? "" : clastname;
                var mfirstname = Xrm.Page.getAttribute("va_srfirstname").getValue();
                mfirstname = mfirstname == null ? "" : mfirstname;
                var mlastname = Xrm.Page.getAttribute("va_srlastname").getValue();
                mlastname = mlastname == null ? "" : mlastname;
                var filenumber = Xrm.Page.getAttribute("va_filenumber").getValue();
                filenumber = filenumber == null ? "" : filenumber;
                var ssn = Xrm.Page.getAttribute("va_ssn").getValue();
                ssn = ssn == null ? "" : ssn;
                var phone = Xrm.Page.getAttribute("va_dayphone").getValue();
                phone = phone == null ? "N/A" : phone;

                body = "A " + newReq + "service request has been submitted for the Veteran " + veteran + s + s + (to && to.length > 0 ? "RO Email Address: " + to + s : "") + "Service Request Number: " + Xrm.Page.getAttribute("va_reqnumber").getValue() + s + "Service Request Action: " + action + s + "Service Request Type: " + issue + s;
                body += "Description: " + s;
                body += "Callers Name: " + cfirstname + " " + clastname + s;
                body += "Veteran/Beneficiary's Name (if different): " + mfirstname + " " + mlastname + s;
                body += "File or Social Security Number: " + (filenumber == "" ? ssn : filenumber) + s;
                body += "Phone Number(s): " + phone + s;
                body += "Best time to reach caller: anytime" + s;
                body += "Brief message: " + s + Description.replace(/\n/g, s) + s + s;
                body += "Attachments: " + (arrFiles ? arrFiles.length : attachUrlList.length);
            }
            else {
                body = Description.replace(/\n/g, s);
            }
        }
    }
    else {
        if (attachmentList.length > 0) {
            attachmentList = "Per your request, we have attached the following forms for your use: <br/>" + attachmentList + "<br/><br/>";
        }
        else {
            // if no attachments, use Description
            attachmentList = Description.replace(/\n/g, s) + s + s;
        }
        //body = attachmentList + "Please do not respond to this message.  Unfortunately, replies to this message will be routed to a mailbox which is unable to receive replies.<br/><br/>" + "Please be aware that you can change your direct deposit, create a benefits letter, check the status of your claim, obtain a copy of your DD 214, and access additional VA benefit information via eBenefits at www.ebenefits.va.gov. To register for an account, you can speak with an eBenefits specialist by dialing 1-800-827-1000 (please press #7) or visit your local VA regional office.<br/><br/>" + "We are happy to help you with any questions or concerns you may have.  If you have questions or need additional assistance, please see our contact information listed below.<br/><br/>" + "How to Contact VA:<br/><br/>" + "Online: <br/>" + "<a href='http://www.va.gov'>www.va.gov</a> or <a href='http://www.ebenefits.va.gov'>www.ebenefits.va.gov</a><br/><br/>" + "By Phone:<br/>" + "1-800-827-1000<br/>711 (Federal Relay Service for Hearing Impaired)<br/><br/>" + "By Mail: <br/>" + sojAddress;
        body = attachmentList + "Please do not respond to this message.  Unfortunately, replies to this message will be routed to a mailbox which is unable to receive replies.<br/><br/>" + "Please be aware that you can change your direct deposit, create a benefits letter, check the status of your claim, obtain a copy of your DD 214, and access additional VA benefit information via eBenefits at www.ebenefits.va.gov. To register for an account, you can speak with an eBenefits specialist by dialing 1-800-827-1000 (please press #7) or visit your local VA regional office.<br/><br/>" + "We are happy to help you with any questions or concerns you may have.  If you have questions or need additional assistance, please see our contact information listed below.<br/><br/>" + "How to Contact VA:<br/><br/>" + "Online: <br/>" + "<a href='http://www.va.gov'>www.va.gov</a> or <a href='http://www.ebenefits.va.gov'>www.ebenefits.va.gov</a><br/><br/>" + "By Phone:<br/>" + "1-800-827-1000<br/>711 (Federal Relay Service for Hearing Impaired)<br/><br/>" + "By Mail: <br/>";

        if (attachmentListx.length > 0) {
            body += attachmentListx;
        }
        else {
            if (roId) {
                var serverUrl = Xrm.Page.context.getServerUrl() + '/XRMServices/2011/OrganizationData.svc/';
                var va_regionalofficeid = Xrm.Page.getAttribute('va_regionalofficeid').getValue();
                var va_letteraddressing = Xrm.Page.getAttribute('va_letteraddressing').getValue();

                if (va_letteraddressing == 953850001) { // Pension
                    var $select = '$select = va_PensionCenterId';
                    var $filter = '$filter = va_regionalofficeId eq guid\'' + va_regionalofficeid[0].id + '\'';
                    var odataSelect = serverUrl + 'va_regionalofficeSet?' + $select + '&' + $filter;
                }
                else {
                    var $select = '$select = va_IntakeCenterId';
                    var $filter = '$filter = va_regionalofficeId eq guid\'' + va_regionalofficeid[0].id + '\'';
                    var odataSelect = serverUrl + 'va_regionalofficeSet?' + $select + '&' + $filter;
                }

                $.ajax({
                    type: "GET",
                    contentType: "application/json; charset=utf-8",
                    datatype: "json",
                    url: odataSelect,
                    async: false,
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("Accept", "application/json");
                    },
                    success: function (data, textStatus, xhr) {
                        if (data.d.results.length == 1) {
                            sojAddress = getSOJAddress(data, va_letteraddressing);
                        }
                    },
                    error: function (xhr, textStatus, errorThrown) {
                    }
                });

                body += sojAddress;
            }
        }
    }

    var toLine = '', ccLine = '';
    var emailofveteran = Xrm.Page.getAttribute('va_emailofveteran').getValue()

    if (sendToVet && emailofveteran != null && action == 'Email Forms') {
        toLine = emailofveteran;
    } else if (sendToVet && sendToRO) {
        toLine = to;
        ccLine = Xrm.Page.getAttribute('va_emailofveteran').getValue();
    } else if (sendToVet && !sendToRO) {
        toLine = Xrm.Page.getAttribute('va_emailofveteran').getValue();
    } else if (!sendToVet && sendToRO) {
        toLine = to;
    }

    var emailOptions = {
        subject: subject,
        to: toLine,
        cc: ccLine,
        isHtml: true,
        body: body,
        files: arrFiles || [], // RU12 fixed bug
        attachements: attachUrlList
    };

    UTIL.openOutlookEmail(emailOptions);
}

function getSOJAddress(data, letteraddressing) {
    var serverUrl = Xrm.Page.context.getServerUrl() + '/XRMServices/2011/OrganizationData.svc/';
    var sojAddress = '';

    if (letteraddressing == 953850001) { // Pension
        var $select = '$select = va_name, va_Address1, va_Address2, va_Address3, va_City, va_State, va_ZipCode, va_FaxNumber, va_LocalFax';
        var $filter = '$filter = va_pensioncenterId eq guid\'' + data.d.results[0].va_PensionCenterId.Id + '\'';
        var odataSelect = serverUrl + 'va_pensioncenterSet?' + $select + '&' + $filter;

        $.ajax({
            type: "GET",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            url: odataSelect,
            async: false,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Accept", "application/json");
            },
            success: function (data, textStatus, xhr) {
                if (data.d.results.length == 1) {
                    sojAddress += data.d.results[0].va_Address1 == null ? null : data.d.results[0].va_Address1 + '<br/>';
                    sojAddress += data.d.results[0].va_Address2 == null ? null : data.d.results[0].va_Address2 + '<br/>';
                    sojAddress += data.d.results[0].va_Address3 == null ? null : data.d.results[0].va_Address3 + '<br/>';
                    sojAddress += data.d.results[0].va_City == null ? null : data.d.results[0].va_City + ', ';
                    sojAddress += data.d.results[0].va_State == null ? null : data.d.results[0].va_State + ' ';
                    sojAddress += data.d.results[0].va_ZipCode == null ? null : data.d.results[0].va_ZipCode;
                }
            },
            error: function (xhr, textStatus, errorThrown) {
            }
        });
    }
    else {
        var $select = '$select = va_name, va_Address1, va_Address2, va_Address3, va_City, va_State, va_ZipCode, va_FaxNumber, va_LocalFax';
        var $filter = '$filter = va_intakecenterId eq guid\'' + data.d.results[0].va_IntakeCenterId.Id + '\'';
        var odataSelect = serverUrl + 'va_intakecenterSet?' + $select + '&' + $filter;

        $.ajax({
            type: "GET",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            url: odataSelect,
            async: false,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Accept", "application/json");
            },
            success: function (data, textStatus, xhr) {
                if (data.d.results.length == 1) {
                    sojAddress += data.d.results[0].va_Address1 == null ? null : data.d.results[0].va_Address1 + '<br/>';
                    sojAddress += data.d.results[0].va_Address2 == null ? null : data.d.results[0].va_Address2 + '<br/>';
                    sojAddress += data.d.results[0].va_Address3 == null ? null : data.d.results[0].va_Address3 + '<br/>';
                    sojAddress += data.d.results[0].va_City == null ? null : data.d.results[0].va_City + ', ';
                    sojAddress += data.d.results[0].va_State == null ? null : data.d.results[0].va_State + ' ';
                    sojAddress += data.d.results[0].va_ZipCode == null ? null : data.d.results[0].va_ZipCode;
                }
            },
            error: function (xhr, textStatus, errorThrown) {
            }
        });
    }

    return sojAddress;
}

//To Do: Change to getValue() and use number instead of TEXT.
// change visibility of sections on the form based on type
function ServiceTypeChange() {
    if (Xrm.Page.getAttribute('va_action').getValue() != null) {
        var action = Xrm.Page.getAttribute('va_action').getSelectedOption().text;
        if (!action) return;
        switch (action) {
            case 'Email Forms':
                Xrm.Page.ui.tabs.get("Tab_SRinfo").setVisible(false);
                Xrm.Page.ui.tabs.get("tab_contact").setVisible(false);
                Xrm.Page.ui.tabs.get("tab_dependent_information").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_Notes").setVisible(false);
                Xrm.Page.ui.tabs.get("tab_ai").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_General").sections.get("Tab_General_Vet").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_General").sections.get("Tab_General_section_4").setVisible(false);
                Xrm.Page.getAttribute('va_sendemailtoveteran').setValue(true);
                break;
            case '0820':
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820").setVisible(true);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820dPayment").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a2").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("0820a_StopPayment").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_VAI").setVisible(false);
                break;
            case '0820f':
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820").setVisible(true);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820dPayment").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a2").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("0820a_StopPayment").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_VAI").setVisible(false);
                break;
            case '0820d':
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820").setVisible(true);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820dPayment").setVisible(true);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_VAI").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a2").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("0820a_StopPayment").setVisible(false);
                break;
            case '0820a':
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820").setVisible(true);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820dPayment").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_VAI").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("0820a_StopPayment").setVisible(true);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a").setVisible(true);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a2").setVisible(true);
                ProcessedInShareChange();
                FnodOtherChange();
                break;
            case 'VAI':
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820dPayment").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a2").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("0820a_StopPayment").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_VAI").setVisible(true);
                break;
            case '0820 & VAI':
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820").setVisible(true);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820dPayment").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a2").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("0820a_StopPayment").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_VAI").setVisible(true);
                break;
            default:
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820dPayment").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a2").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("0820a_StopPayment").setVisible(false);
                Xrm.Page.ui.tabs.get("Tab_SRinfo").sections.get("Section_VAI").setVisible(false);
                break;
        }

        var afControls = [Xrm.Page.getControl("va_placeofdeath"),
            Xrm.Page.getControl("va_enroutetova"),
            Xrm.Page.getControl("va_dateofdeath")];
        var vis = (action == '0820a' || action == '0820f');
        for (var c in afControls) {
            afControls[c].setVisible(vis);
        }
    }
}

function RelatedVetChange() {
    // find ssn/part. id of related veteran
    var id = null;
    var attr = Xrm.Page.getAttribute("va_relatedveteranid");
    if (attr && attr.getValue()[0] && attr.getValue()[0].id.length > 0) id = attr.getValue()[0].id;
    if (!id) return;

    if (RestrictPCRAccess(id, function (restrictAccess) {
        if (restrictAccess)
            return;

        ShowProgress('Getting Contact Data & new Request No');

        CrmRestKit2011.ByQuery('Contact', ['va_SSN', 'va_ParticipantID', 'CreatedOn'], "ContactId eq guid'" + id + "' ")
        .done(function (priorSRs) {
            try {
        	    if (priorSRs && priorSRs.d.results && priorSRs.d.results.length > 0) {
        		    Xrm.Page.getAttribute("va_ssn").setValue(priorSRs.d.results[0].va_SSN);
        		    Xrm.Page.getAttribute("va_participantid").setValue(priorSRs.d.results[0].va_ParticipantID);

        // count number of prior SRs from this person
        		    var personId = Xrm.Page.getAttribute("va_ssn").getValue();
        		    var searchCol = 'va_SSN';
        		    if (!personId) {
        			    personId = Xrm.Page.getAttribute("va_participantid").getValue();
        			    searchCol = 'va_ParticipantID';
    }
        		    if (personId) {
        		        CrmRestKit2011.ByQuery('va_servicerequest', ['CreatedOn'], searchCol + " eq '" + personId + "' ")
        		        .done(function (priorSRs) {
                            var priorCount = 0;
                            if (priorSRs && priorSRs.d && priorSRs.d.results)
                                priorCount = priorSRs.d.results.length;

                            priorCount++;

                            Xrm.Page.getAttribute("va_reqnumber").setValue(personId + ": " + priorCount.toString());
    }).fail(function (err) {
        		            UTIL.restKitError(err, 'Failed to retrieve the prior SR count');
    });

    }
    }
    } catch (e) {
        	    alert("Error occurred.\n" + e.description);
    } finally {
        	    CloseProgress();
    }
    }).fail(function (err) {
            CloseProgress();
            UTIL.restKitError(err, 'Failed to retrieve the contact data and SR number');
    });
    }));
}

//=============================================
// START FUNCTION performCrmAction()
//=============================================
function performCrmAction(actions) {
    for (var i in actions) {
        var action = actions[i];
        action.performAction();
    }
}
// END FUNCTION performCrmAction()
//=============================================

function QuickWriteChange() {
    var id = UTIL.GetLookupId('va_quickwriteid');
    if (!id) return;
    var descAttribute = Xrm.Page.getAttribute('va_description');
    //if (descAttribute.getValue() && !confirm('Would you like to overwrite Description field with the selected Quick Write?')) return;

    var columns = ['va_QuickWriteText'];
    CrmRestKit2011.Retrieve('va_quickwrite', id, columns)
        .fail(function (err) {
            UTIL.restKitError(err, 'Failed to retrieve text.');
        })
         .done(function (data) {
             if (data) {
                 var qw = data.d.va_QuickWriteText;
                 var val = descAttribute.getValue();

                 // process substition tokens
                 // each one looks like <!va_ssn!>
                 qw = UTIL.ReplaceFieldTokens(qw);
                 if (!qw) qw = '';

                 descAttribute.setValue((val ? val + ' \n' + qw : qw));
                 Xrm.Page.getAttribute('va_quickwriteid').setValue(null);
                 Xrm.Page.getControl('va_description').setFocus();

                 getManager();
             }
         });
}

function SOJChange() {
    // auto-select RO with SOJ matching selected value
    var SOJ = Xrm.Page.getAttribute("va_soj").getValue();
    if (!SOJ) return;

    var columns = ['va_name', 'va_regionalofficeId'];
    var filter = "va_SpecialIssueJurisdiction/Value eq " + SOJ + " ";
    CrmRestKit2011.ByQuery('va_regionaloffice', columns, filter)
        .fail(function (err) {
            UTIL.restKitError(err, 'Failed to retrieve data');
        })
        .done(function (data) {
            if (data && data.d.results.length > 0) {
                var id = null;
                var attr = Xrm.Page.getAttribute("va_regionalofficeid");
                if (attr && attr.DataValue && attr.DataValue.length > 0) id = '{' + attr.DataValue[0].id.toString() + '}';
                if (id == data.d.results[0].va_regionalofficeId) return;

                Xrm.Page.getAttribute('va_regionalofficeid').setValue([{
                    id: data.d.results[0].va_regionalofficeId,
                    name: data.d.results[0].va_name,
                    entityType: 'va_RegionalOffice'
                }]);
                alert("SOJ field was automatically updated to '" + data.d.results[0].va_name + "' based on the selected SOJ.");
            }
        });
}

function va_AllTrackedItemsReceivedOrClosedChange() {
    if (Xrm.Page.getAttribute("va_alltrackeditemsreceivedorclosed").getValue()) {
        var desc = Xrm.Page.getAttribute("va_description").getValue();
        var text = "This claim has all tracked items received or closed and should have the status changed to 'Ready for Decision'.";
        if (!desc || desc.indexOf(text) == -1) {
            Xrm.Page.getAttribute('va_description').setValue(desc + '\n\n' + text);
        }
    }
}

function FormatVeteranPhone(areaCode, phoneNumber) {
    var concatPhoneNumber = '', sTmp = '';

    if (areaCode && areaCode != undefined && areaCode != '') {
        sTmp += areaCode;
    }
    if (phoneNumber && phoneNumber != undefined && phoneNumber != '') {
        sTmp += phoneNumber;
    }
    if (sTmp.length == 10) {
        concatPhoneNumber = "(" + sTmp.substr(0, 3) + ") " + sTmp.substr(3, 3) + "-" + sTmp.substr(6, 4);
    } else if (sTmp.length == 7) {
        concatPhoneNumber = sTmp.substr(0, 3) + '-' + sTmp.substr(3, 4);
    }
    return concatPhoneNumber;
}

function ProcessedInShareChange() {
    var showExplanation = Xrm.Page.getAttribute('va_processedfnodinshare').getValue() === false;
    Xrm.Page.getControl('va_processedfnodinshareexplanation').setVisible(showExplanation);
    if (!showExplanation) {
        Xrm.Page.getAttribute('va_processedfnodinshareexplanation').setValue(null);
    }
}

function FnodOtherChange() {
    Xrm.Page.getControl('va_otherspecification').setVisible((Xrm.Page.getAttribute("va_other").getValue() == true));
}

function FormatPathwaysDate(dateStr) {
    if (dateStr && dateStr != undefined && (dateStr.length == 8 || dateStr.length > 8)) {
        dateStr = dateStr.substr(0, 8);
        dateStr = dateStr.substr(4, 4) + dateStr.substr(0, 4);
    }

    return FormatAwardDate(dateStr);
}

function FormatAwardDate(date) {
    if (date && date.length == 8) {
        var month = date.substring(0, 2);
        var day = date.substring(2, 4);
        var year = date.substring(4, 8);
        return new Date(year, month - 1, day);
    } else return null;
}

function RestrictPCRAccess(id, callback) {
    var restrictAccess = false;

    GetContactInformation(id, function (rtContact) {

        if (rtContact && rtContact.fileNumber && rtContact.fileNumber.length > 0) {
            var user = GetUserSettingsForWebservice();

            if (user && user.pcrId && rtContact.fileNumber == user.pcrId) {
                alert('You do not have permission to view this record because it is your own');

                Xrm.Page.ui.tabs.forEach(function (tab, index) {
                    tab.sections.forEach(function (section, index) {
                        section.controls.forEach(function (control, index) {
                            try {
                                control.setVisible(false);
                            } catch (er) { }
                        });
                        section.setVisible(false);
                    });
                    tab.setVisible(false);
                });

                Xrm.Page.ui.navigation.items.forEach(function (item, index) {
                    try {
                        item.setVisible(false);
                    } catch (er) { }
                });

                try {
                    window.close();
                } catch (er) { } finally {
                    restrictAccess = true;
                }
            }
        }
        callback(restrictAccess);
    });
}

function ShowScript() {
    var scriptSource = _KMRoot + 'jurisdictions_routing.html';

    if (!_scriptWindowHandle || _scriptWindowHandle.closed) {
        _scriptWindowHandle = window.open(scriptSource, "CallScript", "width=600,height=500,scrollbars=1,resizable=1");
    } else {
        _scriptWindowHandle.open(scriptSource, "CallScript", "width=600,height=500,scrollbars=1,resizable=1");
    }
}

function fnFNODtype() {
    if (Xrm.Page.getAttribute('va_issue').getValue() == '953850004') {
        Xrm.Page.ui.tabs.get("tab_ai").setDisplayState('collapsed');
        Xrm.Page.ui.tabs.get("tab_dependent_information").setDisplayState('collapsed');
    }
}

function setRPOText() {
    if (Xrm.Page.getAttribute('va_rpo').getValue() !== (953850004 || null)) {
        Xrm.Page.getAttribute('va_rpotext').setValue((Xrm.Page.getAttribute('va_rpo').getText()))
    }
}

function getUniqueRequestNumber() {
    var requestNumber = '[SET REQ NO]',
        vetId,
        ssn,
        phoneWindow;
    var now = new Date();
    var today = (now.getMonth() + 1) + '/' + now.getDate() + '/' + now.getFullYear() + ' ' + now.getHours() + ':' + ((now.getMinutes() < 10 ? '0' : '') + now.getMinutes());
    //va_primarycontactid - Contact

    if (window.parent && window.parent.opener && window.parent.opener.Xrm.Page.data.entity.getEntityName() == 'phonecall') {
        phoneWindow = window.parent.opener;
    }
    if (parent && parent.window && parent.window.parent && parent.window.parent.opener && parent.window.parent.opener.Xrm.Page.data && parent.window.parent.opener.Xrm.Page.data.entity.getEntityName() == 'contact') {
        phoneWindow = parent.window.parent.opener;
    }

    if (phoneWindow.Xrm.Page.getAttribute('regardingobjectid').getValue() != null) {
        vetId = phoneWindow.Xrm.Page.getAttribute('regardingobjectid').getValue()[0].id;
    }

    if (phoneWindow.Xrm.Page.getAttribute('va_ssn').getValue() != null) {
        ssn = phoneWindow.Xrm.Page.getAttribute('va_ssn').getValue();
    }

    if (vetId === null || vetId === undefined || ssn === null || ssn === undefined) {
        requestNumber = ((phoneWindow.Xrm.Page.getAttribute('va_ssn').getValue() != null) ? phoneWindow.Xrm.Page.getAttribute('va_ssn').getValue() + ': 1' : 'Blank Forms ' + today);
    }

    if (window.parent.opener && window.parent.opener.CrmRestKit2011 && vetId !== null && vetId !== undefined && ssn !== null && ssn !== undefined) {
        // search by vet id if present, or ssn if present, or generate generic req number if none are available

        var columns = ['va_reqnumber'];
        var filter = (vetId != null) ? "va_RelatedVeteranId/Id eq guid'" + vetId.toString() + "' " : "va_SSN eq '" + ssn.toString() + "' ";
        var orderby = "&$orderby=" + encodeURIComponent("CreatedOn desc ");
        var query = Xrm.Page.context.getServerUrl() + 'XRMServices/2011/OrganizationData.svc' + "/" + "va_servicerequestSet"
                    + "?$select=" + columns.join(',') + "&$filter=" + encodeURIComponent(filter) + orderby;
        var priorServiceRequests = CrmRestKit2011.ByQueryUrl(query, false);
        priorServiceRequests.fail(function (error) {
            UTIL.restKitError(err, 'Failed to prior service requests');
        })
        priorServiceRequests.done(function (data) {
            count = 0;

            if (ssn == null) { ssn = 'SR: '; }

            if (data.d.results && data.d.results.length > 0) {
                for (var i in data.d.results) {
                    var temp = data.d.results[i].va_reqnumber.split(":");
                    if (parseInt(temp[1]) >= count) {
                        count = parseInt(temp[1]) + 1;
                    }
                }
                requestNumber = ssn + ": " + count;
            } else {
                requestNumber = ssn + ": 1";
            }
        });
    }
    Xrm.Page.getAttribute('va_reqnumber').setValue(requestNumber);
}

function setSRFields() {
    var today = new Date();
    if (Xrm.Page.getAttribute('va_dateopened').getValue() == null) {
        Xrm.Page.getAttribute('va_dateopened').setValue(today);
    }
}

//set Vet on SR when SR is created from SR section of phone call
function setVeteranName() {
    //if SR is created from phonecall
    if ((Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_CREATE) && (Xrm.Page.getAttribute('va_originatingcallid') != null)) {
        var veteranSR = Xrm.Page.getAttribute('va_relatedveteranid');
        var origCallId = Xrm.Page.getAttribute('va_originatingcallid').getValue()[0].id;
        var vetId = GetAttributeValueFromID('phonecall', origCallId, 'regardingobjectid', 'activityid');
        var vetName = GetAttributeValueFromID('contact', vetId, 'fullname', 'contactid');

        if (vetId != null && veteranSR.getValue() == null) {

            var lookupValue = new Array();
            lookupValue[0] = new Object();
            lookupValue[0].id = vetId;
            lookupValue[0].name = vetName;
            lookupValue[0].entityType = 'contact';

            veteranSR.setValue(lookupValue);
        }
    }
}

function getManager() {
    var pcrOfRecordId = Xrm.Page.getAttribute('va_pcrofrecordid').getValue();

    var columns = ['ParentSystemUserId'];
    var filter = "(SystemUserId eq guid'" + pcrOfRecordId[0].id + "')";
    CrmRestKit2011.ByQuery('SystemUser', columns, filter, true)
        .fail(function (err) {
            UTIL.restKitError(err, 'Failed to retrieve manager id');
        })
        .done(function (data) {
            if (data && data.d.results.length == 1) {
                var parentSystemUserId = data.d.results[0].ParentSystemUserId.Id;

                if (parentSystemUserId == null) {
                    alert('It has been detected that you do not have a manager assigned. It is recommended a manager is assigned to you.');

                    var description = Xrm.Page.getAttribute('va_description');
                    description.setValue(description.getValue().replace('(Signature of NCCM)', ''));

                    return;
                }

                columns = ['FirstName', 'LastName'];
                filter = "(SystemUserId eq guid'" + parentSystemUserId + "')";

                CrmRestKit2011.ByQuery('SystemUser', columns, filter, true)
                    .fail(function (err) {
                        UTIL.restKitError(err, 'Failed to retrieve manager data');
                    })
                    .done(function (data) {
                        if (data && data.d.results.length == 1) {
                            manager = data.d.results[0].FirstName + ' ' + data.d.results[0].LastName;

                            var description = Xrm.Page.getAttribute('va_description');
                            description.setValue(description.getValue().replace('(Signature of NCCM)', 'Sincerely,<br/>' + manager));
                        }
                    });
            }
        });
}

function copyMailingAddress() {
    var ga = ga = Xrm.Page.getAttribute;

    ga("va_address1").setValue(ga("va_mailing_address1").getValue());
    ga("va_address2").setValue(ga("va_mailing_address2").getValue());
    ga("va_address3").setValue(ga("va_mailing_address3").getValue());
    ga("va_city").setValue(ga("va_mailing_city").getValue());
    ga("va_state").setValue(ga("va_mailing_state").getValue());
    ga("va_zipcode").setValue(ga("va_mailing_zip").getValue());
    ga("va_country").setValue(ga("va_mailingcountry").getValue());
    ga("va_email").setValue(ga("va_sremail").getValue());
    ga("va_phone").setValue(ga("va_dayphone").getValue());
}
