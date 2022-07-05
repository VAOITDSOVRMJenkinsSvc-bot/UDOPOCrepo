/// <summary>Executes each of the validation checks for the "Dependent" (crme_dependent) entity form</summary>
/// <param name="execContext" type="Object">executionContext passed from the form event handler</param>
/// <returns type="Boolean">Returns true if the function executed successfully; false otherwise</returns>
function onLoad(execContext) {
    try {
        ValidateSsn(execContext);
        ValidateAgeSchool(execContext);
        ValidatePreviouslyMarried(execContext);
        ValidateRelationship(execContext);
        ValidateAgeSeriouslyDisabled(execContext);
        ValidateSpouseMarriageCountry(execContext);
        ValidateSpouseMailingCountry(execContext);
        // ValidateSchoolZip(execContext);
        //  ValidateAttendedSchoolZip(execContext);
    } catch (e) {
        handleError(e);
    }
}


/// <summary>Validates the "SSN" (crme_ssn) field</summary>
/// <param name="execContext" type="Object">executionContext passed from the form event handler</param>
/// <returns type="Boolean">Returns true if the function executed successfully; false otherwise</returns>
function ValidateSsn(execContext) {
    try {
        var formContext = execContext.getFormContext();
        var ssnControl = formContext.getControl("crme_ssn");
        var ssnNotificationId = "SsnValidationNotification";

        function validateNineDigitNumber(ssnString) {
            // Regex to check for 9 digit number
            var re = /^[0-9]{9}$/;
            return re.test(ssnString);
        }
        ssnControl.clearNotification(ssnNotificationId);

        if (formContext.getAttribute("crme_ssn").getValue() === null) {
            ssnControl.addNotification({
                messages: ["A missing or invalid SSN may result in a failed dependent maintenance submission"],
                notificationLevel: "RECOMMENDATION",
                uniqueId: ssnNotificationId
            });
        } else if (!validateNineDigitNumber(formContext.getAttribute("crme_ssn").getValue())) {
            ssnControl.addNotification({
                messages: ["SSN must be nine (9) digits"],
                notificationLevel: "ERROR",
                uniqueId: ssnNotificationId
            });
        } 
        return true;
    } catch (e) {
        handleError(e);
        return false;
    }
}


/// <summary>Validates the "Age 18-23 & In School"(crme_childage1823inschool) field</summary >
/// <param name="execContext" type="Object">executionContext passed from the form event handler</param>
/// <returns type="Boolean">Returns true if the function executed successfully; false otherwise</returns>
function ValidateAgeSchool(execContext) {
    try {
        var formContext = execContext.getFormContext();
        var ageSchoolControl = formContext.getControl("crme_childage1823inschool");
        var ageSchoolValue = formContext.getAttribute("crme_childage1823inschool").getValue();
        var ageSchoolNotificationId = "ageSchoolValidationNotification";

        if (formContext.ui.tabs.get("tab_General") == undefined || formContext.ui.tabs.get("tab_General") == null) {
            return true;
        }

        if (ageSchoolValue === true) {
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_TuitionPaidByGovt").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SchoolInfoManual").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SearchSchool").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SchoolInfo").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_StudentInfo").setVisible(true);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_Remarks").setVisible(true);
            var valRemarks = formContext.getAttribute("udo_incomeremarks").getValue();
            if (valRemarks === null) {
                formContext.getAttribute("udo_incomeremarks").setValue("No Remarks");
            }

            setSignatureFields("validate age school"); // Set 674 document signature fields
        } else {
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_TuitionPaidByGovt").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SchoolInfoManual").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SearchSchool").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_SchoolInfo").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_StudentInfo").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_PrevSchoolInfo").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_PrevSchoolInfoManual").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ReportOfIncome").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_section_ValueOfEstate").setVisible(false);
            formContext.ui.tabs.get("tab_General").sections.get("tab_General_Remarks").setVisible(false);
        }

        return true;
    } catch (e) {
        handleError(e);
        return false;
    }
}


/// <summary>Validates the "Previously Married" (crme_childpreviouslymarried) field</summary>
/// <param name="execContext" type="Object">executionContext passed from the form event handler</param>
/// <returns type="Boolean">Returns true if the function executed successfully; false otherwise</returns>
function ValidatePreviouslyMarried(execContext) {
    try {
        var formContext = execContext.getFormContext();
        var previouslyMarriedControl = formContext.getControl("crme_childpreviouslymarried");
        var previouslyMarriedValue = formContext.getAttribute("crme_childpreviouslymarried").getValue();
        var previouslyMarriedNotificationId = "previouslyMarriedValidationNotification";

        if (previouslyMarriedValue === true) {
            previouslyMarriedControl.addNotification({
                messages: ["Dependent will not be processed by RBPS if 'Previously Married' is 'Yes'"],
                notificationLevel: "RECOMMENDATION",
                uniqueId: previouslyMarriedNotificationId
            });
        } else {
            previouslyMarriedControl.clearNotification(previouslyMarriedNotificationId);
        }

        return true;
    } catch (e) {
        handleError(e);
        return false;
    }
}


/// <summary>Validates the "Relationship" (crme_childrelationship) field</summary>
/// <param name="execContext" type="Object">executionContext passed from the form event handler</param>
/// <returns type="Boolean">Returns true if the function executed successfully; false otherwise</returns>
function ValidateRelationship(execContext) {
    try {
        var formContext = execContext.getFormContext();
        var relationshipControl = formContext.getControl("crme_childrelationship");
        var relationshipValue = formContext.getAttribute("crme_childrelationship").getValue();
        var relationshipNotificationId = "relationshipValidationNotification";

        if (relationshipValue === 935950002) {
            relationshipControl.addNotification({
                messages: ["Dependent will not be processed by RBPS if 'Relationship' is 'Adopted'"],
                notificationLevel: "RECOMMENDATION",
                uniqueId: relationshipNotificationId
            });
        } else {
            relationshipControl.clearNotification(relationshipNotificationId);
        }

        return true;
    } catch (e) {
        handleError(e);
        return false;
    }
}


/// <summary>Validates the Age (based on "DOB" (crme_dob) field) and the "Seriously Disabled (crme_childseriouslydisabled) fields</summary>
/// <param name="execContext" type="Object">executionContext passed from the form event handler</param>
/// <returns type="Boolean">Returns true if the function executed successfully; false otherwise</returns>
function ValidateAgeSeriouslyDisabled(execContext) {
    try {
        var formContext = execContext.getFormContext();
        var seriouslyDisabledControl = formContext.getControl("crme_childseriouslydisabled");
        var seriouslyDisabledValue = formContext.getAttribute("crme_childseriouslydisabled").getValue();
        var dobControl = formContext.getControl("crme_dob");
        var dobValue = formContext.getAttribute("crme_dob").getValue();
        var ageYears = 0;
        var seriouslyDisabledNotificationId = "seriouslyDisabledValidationNotification";
        var ageNotificationId = "ageValidationNotification";

        if (dobValue !== null) {
            var birthYear = dobValue.getFullYear();
            var birthMonth = dobValue.getMonth();
            var birthDay = dobValue.getDate();
            var currentDate = new Date();
            var currentYear = currentDate.getFullYear();
            var currentMonth = currentDate.getMonth();
            var currentDay = currentDate.getDate();
            ageYears = currentYear - birthYear;

            if ((birthMonth - currentMonth > 0) || ((birthMonth - currentMonth === 0) && (birthDay - currentDay > 0))) {
                ageYears--;
            }
        }

        if (seriouslyDisabledValue === true && ageYears > 18) {
            seriouslyDisabledControl.addNotification({
                messages: ["Dependent will not be processed by RBPS if Age is greater than 18 and 'Seriously Disabled' is 'Yes'"],
                notificationLevel: "RECOMMENDATION",
                uniqueId: seriouslyDisabledNotificationId
            });

            dobControl.addNotification({
                messages: ["Dependent will not be processed by RBPS if Age is greater than 18 and 'Seriously Disabled' is 'Yes'"],
                notificationLevel: "RECOMMENDATION",
                uniqueId: ageNotificationId
            });
        } else {
            seriouslyDisabledControl.clearNotification(seriouslyDisabledNotificationId);
            dobControl.clearNotification(ageNotificationId);
        }

        return true;
    } catch (e) {
        handleError(e);
        return false;
    }
}


/// <summary>Validates the Spouse Marriage "Country" (crme_marriagecountryid) field</summary>
/// <param name="execContext" type="Object">executionContext passed from the form event handler</param>
/// <returns type="Boolean">Returns true if the function executed successfully; false otherwise</returns>
function ValidateSpouseMarriageCountry(execContext) {
    try {
        var formContext = execContext.getFormContext();
        var marriageCountryControl = formContext.getControl("crme_marriagecountryid");
        var marriageCountryValue = formContext.getAttribute("crme_marriagecountryid").getValue();
        var marriageCountryNotificationId = "marriageCountryValidationNotification";

        if (marriageCountryValue === null || marriageCountryValue[0].name !== "USA") {
            marriageCountryControl.addNotification({
                messages: ["Dependent will not be processed by RBPS if Spouse's 'Country' is not 'USA'"],
                notificationLevel: "RECOMMENDATION",
                uniqueId: marriageCountryNotificationId
            });
        } else {
            marriageCountryControl.clearNotification(marriageCountryNotificationId);
        }

        return true;
    } catch (e) {
        handleError(e);
        return false;
    }
}

/// <summary>Validates the Spouse Mailing "Country" (crme_countryid1) field</summary>
/// <param name="execContext" type="Object">executionContext passed from the form event handler</param>
/// <returns type="Boolean">Returns true if the function executed successfully; false otherwise</returns>
function ValidateSpouseMailingCountry(execContext) {
    try {
        var formContext = execContext.getFormContext();
        var mailingCountryControl = formContext.getControl("crme_countryid1");
        var mailingCountryValue = formContext.getAttribute("crme_countryid").getValue();
        var mailingCountryNotificationId = "mailingCountryValidationNotification";

        if (mailingCountryValue === null || mailingCountryValue[0].name !== "USA") {
            mailingCountryControl.addNotification({
                messages: ["Dependent will not be processed by RBPS if Spouse's 'Country' is not 'USA'"],
                notificationLevel: "RECOMMENDATION",
                uniqueId: mailingCountryNotificationId
            });
        } else {
            mailingCountryControl.clearNotification(mailingCountryNotificationId);
        }

        return true;
    } catch (e) {
        handleError(e);
        return false;
    }
}


/// <summary>Handles any errors passed to the function</summary>
/// <param name="errorObject" type="Object">Error object that needs to be handled</param>
/// <returns type="Boolean">Returns true if the function executed successfully; false otherwise</returns>
function handleError(errorObject) {
    console.log("An error occurred when performing Dependent validation. Error Message: " + errorObject.message);
    throw (errorObject);
    return true;
}


function ValidateSchoolZip(execContext) {
    try {
        var formContext = execContext.getFormContext();
        var zipControl = formContext.getControl("udo_schooladdresszip");
        var zipNotificationId = "zipValidationNotification";

        function validateFiveDigitNumber(ssnString) {
            // Regex to check for 5 digit number
            var re = /^[0-9]{5}$/;
            return re.test(ssnString);
        }

        zipControl.clearNotification(zipNotificationId);

        if (!validateFiveDigitNumber(formContext.getAttribute("udo_schooladdresszip").getValue())) {
            zipControl.addNotification({
                messages: ["Zip must be five (5) digits"],
                notificationLevel: "ERROR",
                uniqueId: zipNotificationId
            });
        }

        return true;
    } catch (e) {
        handleError(e);
        return false;
    }
}


function ValidateAttendedSchoolZip(execContext) {
    try {
        var formContext = execContext.getFormContext();
        var zipControl = formContext.getControl("udo_attendedschoolzip");
        var zipNotificationId = "zipValidationNotification";

        function validateFiveDigitNumber(ssnString) {
            // Regex to check for 5 digit number
            var re = /^[0-9]{5}$/;
            return re.test(ssnString);
        }

        zipControl.clearNotification(zipNotificationId);

        if (!validateFiveDigitNumber(formContext.getAttribute("udo_attendedschoolzip").getValue())) {
            zipControl.addNotification({
                messages: ["Zip must be five (5) digits"],
                notificationLevel: "ERROR",
                uniqueId: zipNotificationId
            });
        }

        return true;
    } catch (e) {
        handleError(e);
        return false;
    }
}

function Validatedigits(execContext, numberOfDigits, controlName) {
    try {
        var formContext = execContext.getFormContext();
        var control = formContext.getControl(controlName);
        var notificationId = "ValidationNotification";
        var message = "";

        switch (controlName) {
            case "crme_childprimaryphone":
                message = "Child Primary phone must be ten (10) digits";
                break;
            case "crme_spouseprimaryphone":
                message = "Primary phone must be ten (10) digits";
                break;
            default:
                break;
        }

        function validateDigitNumber(string, numberOfDigits) {

            var re = new RegExp("^[0-9]{" + numberOfDigits + "}$");
            return re.test(string);
        }

        control.clearNotification(notificationId);


        if (formContext.getAttribute(controlName).getValue() !== null) {

            if (!validateDigitNumber(formContext.getAttribute(controlName).getValue(), numberOfDigits)) {
                control.addNotification({
                    messages: [message],
                    notificationLevel: "ERROR",
                    uniqueId: notificationId
                });
            }
        }
        return true;
    } catch (e) {
        handleError(e);
        return false;
    }
}