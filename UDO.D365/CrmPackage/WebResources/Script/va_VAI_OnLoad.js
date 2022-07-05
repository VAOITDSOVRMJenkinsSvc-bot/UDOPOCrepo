// This method will be call from CRM form
var formContext;
var exCon;
var webApi;
function VAI_Onload(execContext) {
    // Reports for the ribbon button
    //reports.getReports(); //Added back into Button WR
    exCon = execContext;
    formContext = exCon.getFormContext();    
    var lib = new CrmCommonJS(version, execContext);
    webApi = lib.WebApi;
    UTIL.initialize();
    environmentConfigurations.initalize(exCon);
    commonFunctionsVip.initalize(exCon);
    ws.mapDDevelopmentNotes.initalize(exCon);
    GetListOfHolidays();
    // RU12 Form
    onFormLoad();
}
//VAI Form Onload JS
function onFormLoad() {
    formContext.getAttribute('va_type').addOnChange(dynamicType);
    formContext.getAttribute('va_requestorphone').addOnChange(formatVAIPhone);
    formContext.getAttribute('va_standardtextresponse').addOnChange(standardVAIResponseChange);
    checkboxClickHandler('va_sendemail', sendEmail);
    formContext.getAttribute('va_sendto').addOnChange(ValidateSojIsPilot);

    //Save SubType Value, Run Dynamic Picklist, Return Value to SubType
    var SavedSubTypeValue = Xrm.Page.getAttribute('va_subtype').getValue();
    formContext.getAttribute('va_type').fireOnChange();
    formContext.getAttribute('va_subtype').setValue(SavedSubTypeValue);
    if (!formContext.getAttribute('va_type').getValue()) {
        formContext.getControl('va_type').setFocus();
    }

    formContext.getAttribute('va_requestorphone').fireOnChange();
}

//set VAI due date 5 business days from today


function addBusinessDays(date, numberOfBusinessDays) {
    while (numberOfBusinessDays > 0) {
        date.setDate(date.getDate() + 1);
        if (isBusinessDay(date) == false) {
            numberOfBusinessDays--;
        }
        else {
            numberOfBusinessDays;
        }
    }
    return date;
}

function isBusinessDay(date) {
    var day = date.getDay();
    //return true if day is Sunday, Saturday or Holiday
    if (day == 0 || day == 6 || isHoliday(date)) {
        return true;
    }
    else {
        return false;
    }
}

function setDueDate() {
    var createdOnDate = new Date(context.getAttribute('va_dateopened').getValue());
    var dueDateAttribute = context.getAttribute('va_duedate');
    if (createdOnDate != null && dueDateAttribute.getValue() == null) {
        var dueDate = addBusinessDays(createdOnDate, 5);
        dueDateAttribute.setValue(dueDate);

    }
}


function dynamicType() {
    //First value is Main Picklist Option, followed by corresponding SubValues
    var typePicklist = new Array();
    typePicklist[0] = new Array(953850000, 953850000, 953850001, 953850002, 953850003, 953850004, 953850005);
    typePicklist[1] = new Array(953850001, 953850006, 953850005);
    typePicklist[2] = new Array(953850002, 953850007, 953850008, 953850005);
    typePicklist[3] = new Array(953850003, 953850005, 0);
    typePicklist[4] = new Array(953850004, 953850009, 953850005);
    typePicklist[5] = new Array(953850005, 953850010, 953850011, 953850005);
    typePicklist[6] = new Array(953850006, 953850012, 953850013);
    typePicklist[7] = new Array(953850007, 953850005, 0);

    UTIL.createDynamicPicklist('va_type', 'va_subtype', typePicklist);
}

//**Standard VAI Text Responses Population into Message Function
function standardVAIResponseChange() {
    var id = UTIL.GetLookupId('va_standardtextresponse');
    if (!id) return;
    var messageAttribute = formContext.getAttribute('va_message');

    var columns = ['va_VAIResponseText'];
    //CrmRestKit2011.Retrieve('va_vaitextresponse', id, columns)
    webApi.Retrieve(id, "va_vaitextreponse", columns, null)
        .fail(function (err) {
            UTIL.restKitError(err, 'Failed to retrieve standard VAI text response:');
        }
        )
        .done( function (data) {
                var message = data.va_VAIResponseText;
                var val = messageAttribute.getValue();

                // process substition tokens
                // each one looks like <!va_ssn!>
                message = UTIL.ReplaceFieldTokens(message);
                if (!message) message = '';

                messageAttribute.setValue((val ? val + ' \n' + message : message));
                formContext.getAttribute('va_standardtextresponse').setValue(null);
                formContext.getControl('va_message').setFocus();
        }
        )
}

function sendEmail(event) {
    if (!formContext.getAttribute('va_sendto') || formContext.getAttribute('va_sendto').getValue()==null) {
        alert('SOJ field needs to be populated before sending email.');
        formContext.getAttribute('va_sendemail').setValue(0);
        return;
    }
    var email = formContext.getAttribute('va_email').getValue(),
        sojid = UTIL.GetLookupId('va_sendto'),
        user = GetUserSettingsForWebservice(exCon).fullName,
        Message = (Xrm.Page.getAttribute('va_message').getValue() == null) ? '' : Xrm.Page.getAttribute('va_message').getValue(),
        check = event.getEventSource().getValue();
        //check = event.srcElement.checked,
        emailOptions,
        salutation = formContext.getAttribute('va_requestorsalutation').getText(),
        firstName = formContext.getAttribute('va_requestorfirstname').getValue(),
        lastName = formContext.getAttribute('va_requestorlastname').getValue(),
        fullSalutation = 'Dear ' + salutation + ' ' + firstName + ' ' + lastName + ',' + '\n\n',
        getManager = (GetUserSettingsForWebservice(exCon).manager == null || GetUserSettingsForWebservice(exCon).manager == '') ? user : GetUserSettingsForWebservice(exCon).manager,
        manager = UTIL.fullnameFormat(getManager).substring(1);
    var columns = ['va_Address1', 'va_Address2', 'va_Address3', 'va_City', 'va_State', 'va_ZipCode'];
    //CrmRestKit2011.Retrieve("va_regionaloffice", sojid, columns)
    webApi.Retrieve(sojid, "va_regionaloffice", columns, null)
        .fail(function (err) {

            UTIL.restKitError(err, 'Failed to retrieve standard SOJ response:');
        }
        )
        .done(
            function (data) {
                        var sojAddress1 = data.d.va_Address1,
                        sojAddress2 = data.d.va_Address2,
                        sojAddress3 = data.d.va_Address3,
                        sojCity = data.d.va_City,
                        sojState = data.d.va_State,
                        sojZip = data.d.va_ZipCode;
                        if (check) {
                            if (!email) {
                                alert('An email address is required to send the email!');
                                formContext.getAttribute('va_sendemail').setValue(0);
                                return;
                            }
                            else {
                                emailOptions = {
                                    subject: 'Requested Information from VA',
                                    to: email,
                                    body: fullSalutation + Message + '\n\nThank you for contacting us.  If at some future time we can help you on a different subject, you may submit a new inquiry at https://iris.va.gov, call us toll free at 1-800-827-1000, or visit our web site at https://www.va.gov.\n\n' + ((manager == '' || manager == null) ? ' ' : manager) + '\n' + 'Veterans Service Center Manager\n' + 'How to Contact VA:\n' + 'Online: https://www.va.gov\n' + 'By Phone: 1-800-829-4833 (TDD hearing Impaired)\n' + 'By Letter: US Department of Veterans Affairs\n' + ((sojAddress1 == '' || sojAddress1 == null) ? '' : sojAddress1 + '\n') + ((sojAddress2 == null || sojAddress2 == '') ? '' : sojAddress2 + '\n') + ((sojAddress3 == null || sojAddress3 == '') ? '' : sojAddress3 + '\n') + sojCity + ', ' + sojState + ' ' + sojZip,
                                    isHtml: false
                                };

                                UTIL.openOutlookEmail(emailOptions);
                            }
                        }
                    
            }
        )
}

function formatVAIPhone() {
    UTIL.formatTelephone('va_requestorphone');
}

function checkboxClickHandler(attr, callback) {
    formContext.getAttribute(attr).addOnChange(callback);
    //var element = document.getElementById(attr);
    //element.attachEvent('onclick', callback);
};

function ValidateSojIsPilot() {
    var sojEntity = Xrm.Page.getAttribute('va_sendto').getValue(),
        soj;
    
    if (sojEntity && sojEntity.length > 0) {
        soj = sojEntity[0];

        if (soj.keyValues.va_isvaipilot.value === 'No' || soj.keyValues.va_isvaipilot.value === '') {
            alert("This SOJ is not part of the VAI Pilot. Please exit CRM and use Right Now Web tool to submit VAI");
            Xrm.Page.getAttribute('va_sendto').setValue(null);
        }
    }
}

function setCurrentUser() {
    var currentUserFromForm = formContext.getAttribute('va_currentuser'),
        loggedUserId = Xrm.Utility.getGlobalContext().userSettings.userId;
        loggedUser = GetUserSettingsForWebservice(exCon).fullName;
    if (!currentUserFromForm) {
        if (currentUserFromForm.getValue()[0].id != loggedUserId) {
            currentUserFromForm.setValue([{ id: loggedUserId, name: loggedUser, entityType: 'systemuser' }]);
            currentUserFromForm.setSubmitMode("always");
        }
        else {
            return;
        }
    }
    else {
        currentUserFromForm.setValue([{ id: loggedUserId, name: loggedUser, entityType: 'systemuser' }]);
        currentUserFromForm.setSubmitMode("always");
    }
}
