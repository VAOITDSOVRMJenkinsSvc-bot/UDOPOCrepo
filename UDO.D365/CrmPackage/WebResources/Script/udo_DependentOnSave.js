var executionContext = null;
var formContext = null;
var globalContext = Xrm.Utility.getGlobalContext();
var version = globalContext.getVersion();
var lib;
var webApi;
var formHelper;
var isSaveCloseClicked = false;
var validationPopUp = false;

window.parent.clearschoolinfo = clearschoolinfo;
window.parent.clearprevschoolinfo = clearprevschoolinfo;
window.parent.findschool = findschool;
window.parent.findprevschool = findprevschool;

function OnSave(exCon) {
    console.log("OnSave()");
    executionContext = exCon;
    instantiateCommonScripts(exCon);

    if (formContext == undefined || formContext == null) {
        formContext = exCon.getFormContext();
    }

    if (!isSaveCloseClicked) {
        performSave();
    }

    if ((isSaveCloseClicked == true) && (validationPopUp == false)) {
        if (parent.window.IsUSD) {
            console.log("Running CloseDependentForm USD event");
            window.open("http://event/?eventName=CloseDependentForm");
        }
    }
}

function performSave() {
    console.log("performSave()");
    exCon = executionContext;
    preventAutoSave(exCon);

    var eventArgs = exCon.getEventArgs();
    if (eventArgs.getSaveMode() !== 70) {
        checkReqSchFields();
        checkRequiredDOBField();

        var dependentRelationship = formContext.getAttribute("crme_dependentrelationship").getValue();
        if (dependentRelationship === 935950000) { // Child
            var isChildInSchool = formContext.getAttribute("crme_childage1823inschool").getValue();

            if (isChildInSchool === true) {
                setSignatureFields("perform save"); // Used on 674 document

                var dateFieldValue = new Date();
                var dateFieldValue = formContext.getAttribute("crme_dob").getValue();
                var year = dateFieldValue.getFullYear();
                var year18Bday = year + 18;
                var month = dateFieldValue.getMonth();
                var day = dateFieldValue.getDate();
                var childTurns18Date = new Date(year18Bday, month, day);
                dateFieldValue = formContext.getAttribute("udo_coursebegindate").getValue();
                year = dateFieldValue.getFullYear();
                month = dateFieldValue.getMonth();
                day = dateFieldValue.getDate();
                var courseBeginDate = new Date(year, month, day);

                // Calculate the 18th bday date of this child
                var prevSchoolNameValue = formContext.getAttribute("udo_attendedlastterm").getValue();
                if (prevSchoolNameValue === false) {
                    if (childTurns18Date < courseBeginDate) {
                        var date1 = childTurns18Date; // Remember, months are 0 based in JS
                        var date2 = courseBeginDate;
                        var year1 = date1.getFullYear();
                        var year2 = date2.getFullYear();
                        var month1 = date1.getMonth();
                        var month2 = date2.getMonth();
                        if (month1 === 0) { // Have to take into account
                            month1++;
                            month2++;
                        }
                        var numberOfMonths = (year2 - year1) * 12 + (month2 - month1) - 1;

                        if (numberOfMonths < 5) {
                            validationPopUp = true;
                            var msg = "\n\r" + "Prior school term is required for automatic processing, absence of information may cause a delay in processing the claim";
                            var title = "Please review the details";
                            var alertOptions = { height: 300, width: 600 };
                            var alertStrings = { title: title, text: msg, confirmButtonLabel: "OK" };
                            Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                                function success(result) {
                                    console.log("Alert dialog closed");
                                    console.log(eventArgs.getSaveMode());
                                    if (isSaveCloseClicked === true) {
                                        if (parent.window.IsUSD) {
                                            console.log("Running CloseDependentForm USD event");
                                            window.open("http://event/?eventName=CloseDependentForm");
                                        }
                                    }
                                },
                                function (error) {
                                    console.log(error.message);
                                }
                            );
                        }
                    }
                }
            }
        }
        else { // Spouse
            var maintenanceType = formContext.getAttribute("crme_maintenancetype").getValue();
            if (maintenanceType == 935950001) { // Remove
                validationPopUp = true;
                var msg = "\n\r" + "Determine if step children need to be removed and follow proper procedures";
                var title = "Please review the details";
                var alertOptions = { height: 300, width: 600 };
                var alertStrings = { title: title, text: msg, confirmButtonLabel: "OK" };
                Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                    function success(result) {
                        console.log("Alert dialog closed");
                        console.log(eventArgs.getSaveMode());
                        if (isSaveCloseClicked === true) {
                            if (parent.window.IsUSD) {
                                console.log("Running CloseDependentForm USD event");
                                window.open("http://event/?eventName=CloseDependentForm");
                            }
                        }
                    },
                    function (error) {
                        console.log(error.message);
                    }
                );
            }
        }
    }
}

function preventAutoSave(executionContext) {
    var eventArgs = executionContext.getEventArgs();
    if (eventArgs.getSaveMode() == 70 || eventArgs.getSaveMode() == 2) {
        eventArgs.preventDefault();
        console.log("Save mode is: " + eventArgs.getSaveMode());
    }
}

function checkReqSchFields() {
    if (formContext.getAttribute("crme_dependentrelationship").getValue() === 935950000) {
        if (formContext.getAttribute("crme_childage1823inschool").getValue() === true) {
            if (formContext.getAttribute("udo_schoolinfomanual").getValue() === false) {
                if (formContext.getAttribute("udo_schoolname").getValue() === null) {
                    formContext.getControl("udo_schoolname").setNotification("Please enter the School Name");
                    formContext.getControl("udo_schooladdressline1").setNotification("Please enter the School Address");
                    formContext.getControl("udo_schooladdresscity").setNotification("Please enter the School City");
                    formContext.getControl("udo_schooladdstate").setNotification("Please enter the School State");
                    formContext.getControl("udo_schooladdresszip").setNotification("Please enter the School Zip");
                    return;
                }
            }
            if (formContext.getAttribute("udo_attendedlastterm").getValue() === true) {
                if (formContext.getAttribute("udo_prevschoolinfomanual").getValue() === false) {
                    if (formContext.getAttribute("udo_attendedschool").getValue() === null) {
                        formContext.getControl("udo_attendedschool").setNotification("Please enter the School Name");
                        formContext.getControl("udo_attendedschooladdress1").setNotification("Please enter the School Address");
                        formContext.getControl("udo_attendedschoolcity").setNotification("Please enter the School City");
                        formContext.getControl("udo_attendedschstate").setNotification("Please enter the School State");
                        formContext.getControl("udo_attendedschoolzip").setNotification("Please enter the School Zip");
                        return;
                    }
                }
            }
        }
    }
}

function checkRequiredDOBField() {
    if (formContext.getAttribute("crme_dob").getValue() === null) {
        formContext.ui.setFormNotification("Dependent has no Date of Birth", "WARNING", "DOBEMPTY");
    }
    else {
        formContext.ui.clearFormNotification("DOBEMPTY");
    }
}

function btn_saveAndCloseClicked() {
    console.log("btn_saveAndCloseClicked()");
    
    performSave();
}

function setSignatureFields(eventName) {
    console.log("Setting 674 document Signature Fields triggered by [" + eventName + "]...");

    try {
        var id = globalContext.userSettings.userId;
        var columns = ['firstname', 'lastname', 'homephone', 'mobilephone'];
        var filter = "$filter=systemuserid eq '" + id + "'";

        webApi.RetrieveMultiple("systemuser", columns, filter)
            .then(function (data) {
                if (data.length > 0) {
                    // Used currently on 674 document
                    formHelper.setValue("udo_signaturefirstname", data[0].firstname);
                    formHelper.setValue("udo_signaturelastname", data[0].lastname);

                    retrieveVetInformation();

                    formHelper.setValue("udo_signaturedate", new Date());

                    console.log("674 document Signature Fields set on " + eventName);
                }
                else {
                    console.log("Could not find system user record: " + id);
                }
            });
    }
    catch (ex) {
        console.log("Error reading system user record.  Exception: " + ex);
    }
}

function retrieveVetInformation() {
    try {
        if (formContext.getAttribute("crme_dependentmaintenance").getValue() === undefined || formContext.getAttribute("crme_dependentmaintenance").getValue() === null) {
            console.log("Dependent Maintenance ID not available at this time.");
            return;
        }

        var dmId = formContext.getAttribute("crme_dependentmaintenance").getValue()[0].id.replace("{", "").replace("}", "");
        var columns = ['crme_daytimephone', 'crme_nighttimephone'];
        var filter = "$filter=activityid eq '" + dmId + "'";

        webApi.RetrieveMultiple("crme_dependentmaintenance", columns, filter)
            .then(function (data) {
                if (data.length > 0) {
                    formHelper.setValue("udo_signaturephone", data[0].crme_daytimephone);
                    formHelper.setValue("udo_signatureeveningphone", data[0].crme_nighttimephone);
                    formHelper.setValue("udo_signaturerelationship", "Parent");
                }
                else {
                    console.log("Could not find DM record: " + dmId);
                }
            });
        }
    catch (ex) {
        console.log("Error reading DM record.  Exception: " + ex);
    }
}