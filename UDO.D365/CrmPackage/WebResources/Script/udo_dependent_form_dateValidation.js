"use strict";

/// <summary>Executes each of the validation checks for the "Dependent" (crme_dependent) entity form</summary>
/// <param name="execContext" type="Object">executionContext passed from the form event handler</param>
/// <returns type="Boolean">Returns true if the function executed successfully; false otherwise</returns>
function onLoad(execContext) {
    try {
        instantiateCommonScripts(execContext);
        var formContext = execContext.getFormContext();
        ValidateDob(execContext);

        if (formContext.getAttribute("crme_dependentrelationship").getValue() === 935950001) {
            ValidateMarriageDate(execContext);

            if (formContext.getAttribute("udo_spousedetails").getValue() === 752280001) {
                ValidateMarriageEndDate(execContext);
            }
        }
    } catch (e) {
        handleError(e);
    }
}

/// <summary>Validates the "DOB" (crme_dob) field</summary>
/// <param name="execContext" type="Object">execContext passed from the form event handler</param>
/// <returns type="Boolean">Returns true if the function executed successfully; false otherwise</returns>
function ValidateDob(execContext) {
    try {
        var controlName = "crme_dob";
        var notificationId = "DobValidationNotification";
        var formContext = execContext.getFormContext();
        var control = formContext.getControl(controlName);

        var today = new Date();
        var dateFieldValue = new Date();
        dateFieldValue = formContext.getAttribute(controlName).getValue();

        control.clearNotification(notificationId);

        if (dateFieldValue !== null && dateFieldValue > today) {
            control.addNotification({
                messages: ["DOB cannot be in the future"],
                notificationLevel: "ERROR",
                uniqueId: notificationId
            });
        }

        return true;
    } catch (e) {
        handleError(e);
        return false;
    }
}

/// <summary>Validates the "Marriage Date" (crme_marriagedate) field</summary>
/// <param name="execContext" type="Object">execContext passed from the form event handler</param>
/// <returns type="Boolean">Returns true if the function executed successfully; false otherwise</returns>
function ValidateMarriageDate(execContext) {
    try {
        var controlName = "crme_marriagedate";
        var notificationId = "MarriageDateValidationNotification";
        var formContext = execContext.getFormContext();
        var control = formContext.getControl(controlName);

        if (control !== null) {

            var today = new Date();
            var dateFieldValue = new Date();
            dateFieldValue = formContext.getAttribute(controlName).getValue();

            control.clearNotification(notificationId);

            if (dateFieldValue !== null && dateFieldValue > today) {
                control.addNotification({
                    messages: ["Marriage Date cannot be in the future"],
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

/// <summary>Validates the "Marriage End Date" (crme_relationshipenddate) field</summary>
/// <param name="execContext" type="Object">execContext passed from the form event handler</param>
/// <returns type="Boolean">Returns true if the function executed successfully; false otherwise</returns>
function ValidateMarriageEndDate(execContext) {
    try {
        var controlName = "udo_marriageenddate";
        var notificationId = "MarriageEndDateValidationNotification";
        var formContext = execContext.getFormContext();
        var control = formContext.getControl(controlName);

        if (control !== null) {

            var today = new Date();
            var dateFieldValue = new Date();
            dateFieldValue = formContext.getAttribute(controlName).getValue();

            control.clearNotification(notificationId);

            if (dateFieldValue !== null && dateFieldValue > today) {
                control.addNotification({
                    messages: ["Marriage End Date cannot be in the future"],
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

/// <summary>Handles any errors passed to the function</summary>
/// <param name="errorObject" type="Object">Error object that needs to be handled</param>
/// <returns type="Boolean">Returns true if the function executed successfully; false otherwise</returns>
function handleError(errorObject) {
    console.log("An error occurred when performing Dependent Form Date Validation. Error Message: " + errorObject.message);
    throw (errorObject);
    return true;
}
