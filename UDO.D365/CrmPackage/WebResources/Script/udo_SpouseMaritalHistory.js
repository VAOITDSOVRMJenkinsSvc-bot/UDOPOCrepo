var formContext = null;
var globalContext = Xrm.Utility.getGlobalContext();
var version = globalContext.getVersion();
var lib;
var webApi;
var formHelper;

function instantiateCommonScripts(executionContext) {
    lib = new CrmCommonJS.CrmCommon(version, executionContext);
    webApi = lib.WebApi;
    formHelper = new CrmCommonJS.FormHelper(executionContext);
}

function onLoad(executionContext) {
    instantiateCommonScripts(executionContext);
    formContext = executionContext.getFormContext();

    ValidateMarriageStartDate(executionContext);
    ValidateMarriageEndDate(executionContext);

    setFullName();
}

function onSave(executionContext) {
    setFullName();
}

function setFullName() {
    var fullName = "";
    var firstName = formContext.getAttribute("crme_firstname");
    var lastName = formContext.getAttribute("crme_lastname");

    if (firstName != undefined && fullName != null) {
        fullName = firstName.getValue();
    }

    if (lastName != undefined && lastName != null) {
        fullName = fullName + " " + lastName.getValue();
    }

    formContext.getAttribute("crme_fullname").setValue(fullName);
}

function marriageStartCountryOnChange(executionContext) {
    // Marriage Start Location is a field for the 686c form that shows concatenated City and State for US, and Country
    instantiateCommonScripts(executionContext);
    setLocation("crme_marriagestartlocation", "crme_marriagestartcountryid", "crme_marriagestartcity", "crme_marriagestartstateid");
}

function marriageEndCountryOnChange(executionContext) {
    // Marriage End Location is a field for the 686c form that shows concatenated City and State for US, and Country
    instantiateCommonScripts(executionContext);
    setLocation("crme_marriageendlocation", "crme_marriageendcountryid", "crme_marriageendcity", "crme_marriageendstateid");
}

function setLocation(locationField, countryField, cityField, stateField) {

    var countryAttribute = formHelper.getValue(countryField);
    if (countryAttribute !== null && countryAttribute !== '') {
        var thisCountry = countryAttribute;
        if (thisCountry[0].name === "USA" || thisCountry[0].name === "US") {
            var state = formHelper.getValue(stateField);
            var city = formHelper.getValue(cityField);
            var thisCity = "";
            var thisState = "";
            (city !== null && city !== '') ? thisCity = city.toUpperCase() : thisCity = "";
            (state !== null && state !== '') ? thisState = state[0].name.toUpperCase() : thisState = "";
            formHelper.setValue(locationField, thisCity + " " + thisState);
        } else {
            formHelper.setValue(locationField, thisCountry[0].name);
        }
    }
}

/// <summary>Validates the "Marriage End Date" (crme_marriageenddate) field</summary>
/// <param name="execContext" type="Object">execContext passed from the form event handler</param>
/// <returns type="Boolean">Returns true if the function executed successfully; false otherwise</returns>
function ValidateMarriageEndDate(execContext) {
    try {
        var controlName = "crme_marriageenddate";
        var notificationId = "MarriageEndDateValidationNotification";
        var formContext = execContext.getFormContext();
        var control = formContext.getControl(controlName);

        if (control !== null) {
            control.clearNotification(notificationId);

            var today = new Date();
            var dateFieldValue = new Date();
            dateFieldValue = formContext.getAttribute(controlName).getValue() || formHelper.getValue(controlName);

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

/// <summary>Validates the "Marriage Start Date" (crme_marriagestartdate) field</summary>
/// <param name="execContext" type="Object">execContext passed from the form event handler</param>
/// <returns type="Boolean">Returns true if the function executed successfully; false otherwise</returns>
function ValidateMarriageStartDate(execContext) {
    try {
        var controlName = "crme_marriagestartdate";
        var notificationId = "MarriageStartDateValidationNotification";
        var formContext = execContext.getFormContext();
        var control = formContext.getControl(controlName);

        if (control !== null) {
            control.clearNotification(notificationId);

            var today = new Date();
            var dateFieldValue = new Date();
            dateFieldValue = formContext.getAttribute(controlName).getValue() || formHelper.getValue(controlName);

            if (dateFieldValue !== null && dateFieldValue > today) {
                control.addNotification({
                    messages: ["Marriage Start Date cannot be in the future"],
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
    console.log("An error occurred when performing Spouse Marital History Validation. Error Message: " + errorObject.message);
    throw (errorObject);
    return true;
}
