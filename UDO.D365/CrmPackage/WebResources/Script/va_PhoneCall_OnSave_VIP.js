/// <reference path="/Intellisense/XrmPage-vsdoc.js" />
var formContext = null;
var exCon = null;
function Form_Onsave(executionObj) {
    formContext = executionObj.getFormContext();
    exCon = executionObj;
    try {
        //debugger;
        if (Xrm.Page.getAttribute('subject').getValue() == 'NOOP') { return true; }

        var editable = (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_CREATE || Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_UPDATE);
        if (editable) {
            //debugger;
            //If Caller Phone empty, then check No Phone Number Available
            if (!Xrm.Page.getAttribute('va_identifycallerphone').getValue()) {
                Xrm.Page.getAttribute('va_nophonenumberavailable').setValue(true);
            }
            if (_WebServiceExecutionStatusLists && _WebServiceExecutionStatusLists.length > 0 && _WebServiceExecutionStatusLists[0] != '[') {
                _WebServiceExecutionStatusLists = '[' + _WebServiceExecutionStatusLists + ']';
            }
            Xrm.Page.getAttribute('va_webserviceexecutionstatus').setValue(_WebServiceExecutionStatusLists);

            var ph = Xrm.Page.getAttribute("phonenumber").getValue();
            var curMC = Xrm.Page.getAttribute('va_ismarriedcaid').getValue();
            // if closing window or closing phone call, and duration is blank, populate

            // update subject for call disc
            if (Xrm.Page.getAttribute("va_callerwasdisconnected").getValue() === true) {
                var subjAttr = Xrm.Page.getAttribute("subject"), subValue = subjAttr.getValue();
                if (!subValue || (subValue && subValue.toUpperCase().indexOf('CALL DISCONNECTED') == -1)) {
                    subjAttr.setValue(subValue + ' - Call Disconnected');
                }
            }

            var timeVal = Xrm.Page.getAttribute("va_calldurationsec").getValue();
            var curDuration = 0;
            if (timeVal && parseInt(timeVal)) curDuration = parseInt(timeVal);

            // close   completed   save/new  deact.      

            var timeSpan = curDuration + ((new Date()).getTime() - _formOpenTimeStamp.getTime()) / 1000;
            Xrm.Page.getAttribute("va_calldurationsec").setValue(parseInt(timeSpan));
            Xrm.Page.getAttribute("va_calldurationsec").setSubmitMode('always');

            // update duration, using 5 minutes increments
            var durationMin = parseInt(timeSpan / 60);
            if (timeSpan > 0 && timeSpan <= 60) {
                durationMin = 1;
            }
            else {
                var remainder = durationMin % 5;
                if (remainder > 0) {
                    //durationMin -= remainder;
                    durationMin += (5 - remainder);
                }
            }

            Xrm.Page.getAttribute("actualdurationminutes").setValue(durationMin);
            Xrm.Page.getAttribute("actualdurationminutes").setSubmitMode('always');

            UpdateContactOutreachFields();

            //DetectIssueChangesandPromptSRCreation(false);
        }
        if (_scriptWindowHandle) {
            try { _scriptWindowHandle.close(); } catch (e) { }
        }
        window.status = '+';

        var repeatCall = Xrm.Page.getAttribute("va_thisisarepeatcall").getValue();
        var repeatCall_isDirty = Xrm.Page.getAttribute("va_thisisarepeatcall").getIsDirty();
        var abusiveCall = Xrm.Page.getAttribute("va_abusivecall").getValue();
        var abusiveCall_isDirty = Xrm.Page.getAttribute("va_abusivecall").getIsDirty();

        //if ((repeatCall && repeatCall_isDirty) || (abusiveCall && abusiveCall_isDirty)) {
        if (abusiveCall && abusiveCall_isDirty) {
            createMapdNote();
        }
    }
    catch (err) {
        alert("Error caught during Onsave event: " + err.Description);
    }
}



// if outreach fields had changed, update matching contact
function UpdateContactOutreachFields() {
    if (!Xrm.Page.getAttribute("regardingobjectid").getValue() || Xrm.Page.getAttribute("regardingobjectid").getValue().length == 0) return;

    var rtContact = new contact();
    rtContact.id = Xrm.Page.getAttribute("regardingobjectid").getValue()[0].id;
    var isDirty = false;

    if (Xrm.Page.getAttribute("va_smspaymentnotices").getIsDirty()) { isDirty = true; rtContact.SMSPaymentNotices = Xrm.Page.getAttribute("va_smspaymentnotices").getValue(); }
    if (Xrm.Page.getAttribute("va_emailclaimnotices").getIsDirty()) { isDirty = true; rtContact.EmailClaimNotices = Xrm.Page.getAttribute("va_emailclaimnotices").getValue(); }
    if (Xrm.Page.getAttribute("va_emailpaymentnotices").getIsDirty()) { isDirty = true; rtContact.EmailPaymentNotices = Xrm.Page.getAttribute("va_emailpaymentnotices").getValue(); }
    if (Xrm.Page.getAttribute("va_preferredcontactmethod").getIsDirty()) { isDirty = true; rtContact.PreferredMethodofContact = Xrm.Page.getAttribute("va_preferredcontactmethod").getValue(); }

    if (Xrm.Page.getAttribute("va_preferredday").getIsDirty()) { isDirty = true; rtContact.PreferredDay = Xrm.Page.getAttribute("va_preferredday").getValue(); }
    if (Xrm.Page.getAttribute("va_preferredtime").getIsDirty()) { isDirty = true; rtContact.PreferredTime = Xrm.Page.getAttribute("va_preferredtime").getValue(); }
    if (Xrm.Page.getAttribute("va_selfservicenotifications").getIsDirty()) { isDirty = true; rtContact.SelfServiceNotifications = Xrm.Page.getAttribute("va_selfservicenotifications").getValue(); }
    if (Xrm.Page.getAttribute("va_email").getIsDirty()) { isDirty = true; rtContact.email = Xrm.Page.getAttribute("va_email").getValue(); }
    if (Xrm.Page.getAttribute("va_preferredphone").getIsDirty()) { isDirty = true; UTIL.formatTelephone(Xrm.Page.getAttribute("va_preferredphone")); rtContact.PreferredPhone = Xrm.Page.getAttribute("va_preferredphone").getValue(); }
    if (Xrm.Page.getAttribute("va_hasebenefitsaccount").getIsDirty()) { isDirty = true; rtContact.HasEbenefitsAccount = Xrm.Page.getAttribute("va_hasebenefitsaccount").getValue(); }
    if (Xrm.Page.getAttribute("va_timezone").getIsDirty()) { isDirty = true; rtContact.TimeZone = Xrm.Page.getAttribute("va_timezone").getValue(); }
    //if (Xrm.Page.getAttribute("va_preferredphone").getIsDirty()) {isDirty=true; rtContact.PreferredPhone = Xrm.Page.getAttribute("va_preferredphone").getValue();}
    if (Xrm.Page.getAttribute("va_preferredphonetype").getIsDirty()) { isDirty = true; rtContact.PreferredPhoneType = Xrm.Page.getAttribute("va_preferredphonetype").getValue(); }

    if (Xrm.Page.getAttribute("va_morningphone").getIsDirty()) { isDirty = true; UTIL.formatTelephone(Xrm.Page.getAttribute("va_morningphone")); rtContact.MorningPhone = Xrm.Page.getAttribute("va_morningphone").getValue(); }
    if (Xrm.Page.getAttribute("va_afternoonphone").getIsDirty()) { isDirty = true; UTIL.formatTelephone(Xrm.Page.getAttribute("va_afternoonphone")); rtContact.AfternoonPhone = Xrm.Page.getAttribute("va_afternoonphone").getValue(); }
    if (Xrm.Page.getAttribute("va_eveningphone").getIsDirty()) { isDirty = true; UTIL.formatTelephone(Xrm.Page.getAttribute("va_eveningphone")); rtContact.EveningPhone = Xrm.Page.getAttribute("va_eveningphone").getValue(); }

    if (!isDirty) return;
    rtContact.updateOutreachFields();
}

//create contact when search doesn't return data and contact doesn't exists
function tempContactnoContactFound() {
    if ((Xrm.Page.getAttribute('regardingobjectid').getValue() == null) && (Xrm.Page.getAttribute('va_createcontact').getValue() == true)) {
        var firstName = Xrm.Page.getAttribute("va_firstname").getValue();
        var lastName = Xrm.Page.getAttribute('va_lastname').getValue();
        var ssn = Xrm.Page.getAttribute("va_ssn").getValue();

        var context = Xrm.Page.context;
        var serverUrl = context.getClientUrl();
        var ODATA_ENDPOINT = "/XRMServices/2011/OrganizationData.svc";
        var CRMObject = new Object();
        ///////////////////////////////////////////////////////////// 
        // Specify the ODATA entity collection 
        var ODATA_EntityCollection = "/ContactSet";
        ///////////////////////////////////////////////////////////// 
        // Define attribute values for the CRM object you want created 
        CRMObject.FirstName = firstName;
        CRMObject.LastName = lastName;
        CRMObject.va_SSN = ssn;
        //Parse the entity object into JSON 
        var jsonEntity = window.JSON.stringify(CRMObject);
        //Asynchronous AJAX function to Create a CRM record using OData
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            url: serverUrl + ODATA_ENDPOINT + ODATA_EntityCollection,
            data: jsonEntity,
            async: �false,
            beforeSend: function (XMLHttpRequest) {
                //Specifying this header ensures that the results will be returned as JSON. 
                XMLHttpRequest.setRequestHeader("Accept", "application/json");
            },
            success: function (data, textStatus, XmlHttpRequest) {
                var NewContactCreated = data["d"];
                var RegardingName = Xrm.Page.getAttribute("regardingobjectid");
                if (RegardingName != null) {
                    var lookupValue = new Array();
                    lookupValue[0] = new Object();
                    lookupValue[0].id = NewContactCreated.ContactId;
                    lookupValue[0].name = lastName + ", " + firstName;
                    lookupValue[0].entityType = "contact";
                    RegardingName.setValue(lookupValue);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert("failed creating contact");
            }
        });
    }
}

function createMapdNote() {
    var note = '';
    var repeatCall = Xrm.Page.getAttribute("va_thisisarepeatcall").getValue();
    var repeatCall_isDirty = Xrm.Page.getAttribute("va_thisisarepeatcall").getIsDirty();
    var abusiveCall = Xrm.Page.getAttribute("va_abusivecall").getValue();
    var abusiveCall_isDirty = Xrm.Page.getAttribute("va_abusivecall").getIsDirty();

    //if (repeatCall && repeatCall_isDirty) {
    //    note += 'Repeat Caller script provided.';
    //}
    if (abusiveCall && abusiveCall_isDirty) {
        //    if (note != '') {
        //        note += ' ';
        //    }
        note += 'Caller became abusive toward PCR. Advised caller call would be disconnected if these actions continue. PCR disconnected call.';
    }

    var noteExists = false;
    ShowProgress((noteExists ? 'Updating' : 'Creating new') + ' Development Note');

    var mapdOptions = {
        nodeId: noteId,
        ptcpntId: Xrm.Page.getAttribute('va_participantid').getValue(),
        noteText: note
    };

    var mapdResults = UTIL.mapdNote(mapdOptions);

    CloseProgress();
}
