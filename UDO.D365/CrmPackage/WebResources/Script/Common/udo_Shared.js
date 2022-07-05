"use strict";

if (typeof UDO === 'undefined')
{ var UDO = { __namespace: true }; }

if (typeof (UDO.Shared) === "undefined") {
    UDO.Shared = {
        __namespace: true
    };
}

if (typeof (UDO.Shared.Constants) === "undefined") {
    UDO.Shared.Constants = {
        __namespace: true
    };
}

if (typeof UDO.Shared.RibbonCode === 'undefined') {
    UDO.Shared.RibbonCode = { __namespace: true };
}

// Constants
UDO.Shared.Constants.CREATE_FORM = 1;
UDO.Shared.Constants.UPDATE_FORM = 2;
UDO.Shared.Constants.INACTIVE_FORM = 4;
UDO.Shared.Constants.QUICK_CREATE_FORM = 5;
//UDO.Shared.Constants.OCCFM_FORM = "43132D83-9BC6-432A-B20C-2039CFC2E0C8"; // Id example

// Example
// UDO.Shared.Constants.Position =
//     {
//         SpecialtyCSR: "9f4fd2ce-48d9-e811-8134-1458d04d78b8",
//         Supervisor: "dbb3287d-c2ce-e711-8118-1458d04e2938",
//         SeniorSupervisor: "bff07490-c2ce-e711-8118-1458d04e2938",
//         CSR: "d4f035d9-c2ce-e711-8118-1458d04e2938",
//         Analyst: "6c2d8605-c3ce-e711-8118-1458d04e2938"
//     };

// Example
// UDO.Shared.Constants.InteractedWith_Options =
//     {
//         C6: [
//                 UDO.Shared.Constants.InteractedWith.Veteran,
//                 UDO.Shared.Constants.InteractedWith.MeaningfulRelationship,
//                 UDO.Shared.Constants.InteractedWith.VAEmployee,
//                 UDO.Shared.Constants.InteractedWith.Other,
//                 UDO.Shared.Constants.InteractedWith.Congressional,
//                 UDO.Shared.Constants.InteractedWith.VeteranRepresentative,
//                 UDO.Shared.Constants.InteractedWith.CommunityProviderOffice,
//                 UDO.Shared.Constants.InteractedWith.TPA
//         ]
//     }

//Variables
var _notifications = null;
//var _crmCommonJS = new CrmCommonJS.CrmCommon(2016);

// Example
UDO.Shared.UserLobs = null;

//CrmCommonJS Web API accessor 
//UDO.Shared.CrmCommonJS = _crmCommonJS;

//Function to handle UTC Dates
UDO.Shared.FormatDate = function (date, operator) {
    var dateParts = date.split("/");
    var utcDate;
    var isoDate;

    if (dateParts.length === 3) {
        if (isNumeric(dateParts[0]) && isNumeric(dateParts[1]) && isNumeric(dateParts[2])) {
            if (!isNaN(Date.parse(dateParts[2] + "-" + dateParts[0] + "-" + dateParts[1]))) {
                if (operator === "ge") {
                    utcDate = new Date(Date.UTC(dateParts[2], parseInt(dateParts[0]) - 1, parseInt(dateParts[1]), 12));
                }
                else if (operator === "lt") {
                    utcDate = new Date(Date.UTC(dateParts[2], parseInt(dateParts[0]) - 1, parseInt(dateParts[1]) + 1, 12));
                }

                isoDate = utcDate.toISOString();
                isoDate = isoDate.substring(0, 10);
            }
        }
    }

    return isoDate;
}

//function to validate the provided SSN 
UDO.Shared.ValidateSSN = function (ssn) {

    return (
      ssn.length === 9 &&
      ssn.match(/[0-9]{9}/)
     );
}

/**
* @param {string} formType - Identify entity friendly name
* @param {array} fieldNames - Pass an Array of field names to validate
* @param {string} numberType - For form notifications '{string} is invalid'
*/
UDO.Shared.ValidatePhoneOrFaxNumber = function (formType, fieldNames, numberType) {
    var validNumber = true;
    var errMsg = numberType + " Number must contain only 10 digits";
    var numberToCheck;
    var badNumber;
    var fieldCnt = fieldNames.length;
    for (i = 0; i < fieldCnt; i++) {
        badNumber = false;
        numberToCheck = UDO.Shared.GetFieldValue(fieldNames[i]);

        if (numberToCheck === null) {
            numberToCheck = "";
        }

        numberToCheck = UDO.Shared.UnPrettyFormatPhoneOrFaxNumber(numberToCheck);

        if (numberToCheck.replace(/\D/g, '').length < numberToCheck.length && numberToCheck.length > 0) {
            errMsg = numberType + " Number cannot contain letters";
            badNumber = true;
        } else if (numberToCheck.length === 0) {
            badNumber = false;
        }
        else {
            badNumber = numberToCheck.length !== 10 ? true: false;
        }

        if (badNumber) {
            validNumber = false; // There is at least one bad number

            UDO.Shared.FormContext.ui.setFormNotification(errMsg, "ERROR", "PHONEFAXERROR" + i);

            // Add additional field level notification as well
            if (formType === "Interaction" && numberType === "Phone") {
                UDO.Shared.FormContext.getControl("bah_phonenumber_text").setNotification("Phone Number must contain only 10 digits", "PHONELENGTHERROR");
            }
        } else {
            UDO.Shared.FormContext.ui.clearFormNotification("PHONEFAXERROR" + i);
            if (formType === "Interaction" && numberType === "Phone") {
                UDO.Shared.FormContext.getControl("bah_phonenumber_text").clearNotification("PHONELENGTHERROR");
            }

            UDO.Shared.SetFieldValue(fieldNames[i], UDO.Shared.PrettyFormatPhoneOrFaxNumber(numberToCheck));
        }
    }

    return validNumber;
}

UDO.Shared.PrettyFormatPhoneOrFaxNumber = function (num) {
    var prettyNum;
    num = num.replace(/\D/g, ''); // Remove all non-digits

    var match = num.match(/^(\d{3})(\d{3})(\d{4})$/);
    if (match) {
        prettyNum = '(' + match[1] + ')' + match[2] + '-' + match[3];
    } else {
        prettyNum = num;
    }

    return prettyNum;
}

UDO.Shared.UnPrettyFormatPhoneOrFaxNumber = function (num) {
    var nonPrettyNum;

    nonPrettyNum = num.replace("(", "").replace(")", "").replace("-", "");

    return nonPrettyNum;
}

//function to clear session storage
UDO.Shared.ClearSesssionStorage = function (storageItemName) {
    if (sessonStorage.getItem(storageItemName)) {
        sessionStorage.removeItem(storageItemName);
    }
}

UDO.Shared.GetCleanId = function (lookup) {
    var cleanId = lookup[0].id.replace("{", "").replace("}", "").toLowerCase();
    return cleanId;
}

// function that wires a change event to a field
UDO.Shared.SetOnChange = function (fieldName, delegate) {
    if (UDO.Shared.FormContext.getAttribute(fieldName) !== null) {
        UDO.Shared.FormContext.getAttribute(fieldName).addOnChange(delegate);
    }
}

//Function to remove a specific change event handler for a field
UDO.Shared.RemoveOnChange = function (fieldName, delegate) {
    if (UDO.Shared.FormContext.getAttribute(fieldName) !== null) {
        UDO.Shared.FormContext.getAttribute(fieldName).removeOnChange(delegate);
    }
}

// set the specified field as read-only (true/false)
UDO.Shared.SetReadOnly = function (fieldName, isReadOnly) {
    if (UDO.Shared.FormContext.getControl(fieldName) !== null) {
        UDO.Shared.FormContext.getControl(fieldName).setDisabled(isReadOnly);
    }
}

UDO.Shared.SetFieldValue = function (fieldName, value) {
    if (UDO.Shared.FormContext.getAttribute(fieldName) !== null) {
        return UDO.Shared.FormContext.getAttribute(fieldName).setValue(value);
    }
    return null;
}

// some field-level manipulation functions that provide null-checking
UDO.Shared.GetFieldValue = function (fieldName) {
    if (UDO.Shared.FormContext !== null && UDO.Shared.FormContext.getAttribute(fieldName) !== null) {
        return UDO.Shared.FormContext.getAttribute(fieldName).getValue();
    }
    return null;
}

UDO.Shared.SetRequired = function (fieldName, requiredLevel) {
    requiredLevel = requiredLevel === true ? "required"
        : requiredLevel === false ? "none"
        : requiredLevel;
       
    if (UDO.Shared.FormContext.getAttribute(fieldName) !== null)   {
        UDO.Shared.FormContext.getAttribute(fieldName).setRequiredLevel(requiredLevel);
    }
}

// set the specified field to Visible/hidden (true/false)
UDO.Shared.SetVisible = function (fieldName, isReadOnly) {
    if (UDO.Shared.FormContext.getControl(fieldName) !== null) {
        UDO.Shared.FormContext.getControl(fieldName).setVisible(isReadOnly);
    }
}

// set the cursor focus on specific attribute
UDO.Shared.SetFocus = function (fieldName) {
    if (UDO.Shared.FormContext.getControl(fieldName) !== null) {
        UDO.Shared.FormContext.getControl(fieldName).setFocus();
    }
}

// set submit mode on field
// valid values: always, never dirty
UDO.Shared.SetSubmitMode = function (fieldName, submitMode) {
    if (UDO.Shared.FormContext.getAttribute(fieldName) !== null) {
        UDO.Shared.FormContext.getAttribute(fieldName).setSubmitMode(submitMode);
    }
}

UDO.Shared.GetOptionSetValue = function (fieldName) {
    if (UDO.Shared.FormContext.getAttribute(fieldName) !== null && UDO.Shared.FormContext.getAttribute(fieldName).getSelectedOption() !== null) {
        return UDO.Shared.FormContext.getAttribute(fieldName).getSelectedOption().value;
    }
    return null;
}

UDO.Shared.GetOptionSetText = function (fieldName) {
    if (UDO.Shared.FormContext.getAttribute(fieldName) !== null && UDO.Shared.FormContext.getAttribute(fieldName).getSelectedOption() !== null) {
        return UDO.Shared.FormContext.getAttribute(fieldName).getSelectedOption().text;
    }
    return null;
}

//Function to loop all controls on form and set them to disabled
UDO.Shared.LockForm = function () {
    UDO.Shared.FormContext.data.entity.attributes.forEach(function (attribute, index) {
        var control = UDO.Shared.FormContext.getControl(attribute.getName());
        if (control) {
            control.setDisabled(true);
        }
    });
}

//Function to loop all controls on form and set them to Required No
UDO.Shared.SetAllFieldsNotRequired = function () {
    UDO.Shared.FormContext.data.entity.attributes.forEach(function (attribute, index) {
        attribute.setRequiredLevel("none");
    });
}

// utility function to write to the console (if it is available)
UDO.Shared.WriteToConsole = function (message) {
    if (window.console) {
        window.console.log(message);
    }
}

UDO.Shared.ExecuteWorkflow = function (workflowId, entityId) {
    return new Promise(function (resolve, reject) {
        var query = "workflows(" + workflowId.replace("}", "").replace("{", "") + ")/Microsoft.Dynamics.CRM.ExecuteWorkflow";
        var data = {
            "EntityId": entityId
        };

        var req = new XMLHttpRequest();
        req.open("POST", encodeURI(Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/" + query), true);
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.onreadystatechange = function () {
            if (this.readyState === 4 /* complete */) {
                if (req.status >= 200 && req.status < 300) {
                    req.onreadystatechange = null;
                    resolve(req);
                }
                else {
                    reject(new Error(req.status + " - " + req.statusText));
                }
            }
        };
        req.send(JSON.stringify(data));
    });
}

UDO.Shared.ExecuteWorkflowWithoutCallback = function (workflowId, entityId) {
    UDO.Shared.ExecuteWorkflow(workflowId, entityId).then(function (workflowData) {
        console.log(workflowData);
    }).catch(function (err) {
        console.log(err);
    });
}

UDO.Shared.GetCurrentRecordIdFormatted = function () {
    var id = UDO.Shared.FormContext.data.entity.getId();
    return id.replace(/{/gi, "").replace(/}/, "");
};

UDO.Shared.UpdateRecord = function (entityName, recordId, entityObject) {
    return new Promise(function (resolve, reject) {
        var req = new XMLHttpRequest();
        req.open("PATCH", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/" + entityName + "(" + recordId + ")", true);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 204) {
                    //Success - No Return Data - Do Something
                    resolve(req);
                } else {
                    reject(new Error(req.status + " - " + req.statusText));
                }
            }
        };
        req.send(JSON.stringify(entityObject));
    });
}

UDO.Shared.CreateRecord = function (entityName, entityObject) {
    return new Promise(function (resolve, reject) {
        var req = new XMLHttpRequest();
        req.open("POST", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/" + entityName, true);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 204) {
                    var uri = this.getResponseHeader("OData-EntityId");
                    var regExp = /\(([^)]+)\)/;
                    var matches = regExp.exec(uri);
                    var newEntityId = matches[1];
                    resolve(newEntityId);
                } else {
                    reject(new Error(req.status + " - " + req.statusText));
                }
            }
        };
        req.send(JSON.stringify(entityObject));
    });
}

UDO.Shared.GetFormContext = function (executionContext) {
    if (executionContext !== null && executionContext !== "undefined") {
        if (UDO.Shared.FormContext !== null && UDO.Shared.FormContext !== "undefined") {
            UDO.Shared.FormContext = executionContext.getFormContext();
        }
    }
}

UDO.Shared.CallAction = function (actionName, parameters) {
    return new Promise(function (resolve, reject) {
        var req = new XMLHttpRequest();
        req.open("POST", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/" + actionName, true);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    var result = JSON.parse(this.response);
                    console.log("Successful Action Call with Results: ");
                    console.log(result);
                    resolve(result);
                } else {
                    console.log("Error in action call: " + this.statusText);
                    reject(new Error(req.status + " - " + req.statusText));
                }
            }
        };
        req.send(JSON.stringify(parameters));
    });
}

UDO.Shared.ExecuteAction = function (request) {
    return new Promise(function (resolve, reject) {
        Xrm.WebApi.online.execute(request).then(
            function success(result) {
                result.json().then(
                    function (response) {
                        resolve(response);
                    }
                );
            },
            function (error) {
                reject(error.message);
            }
        );
    });
}

UDO.Shared.CallEntityAction = function (actionName, entityName, entityId, parameters) {
    return new Promise(function (resolve, reject) {
        var req = new XMLHttpRequest();
        req.open("POST", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/" + entityName + "s(" + entityId + ")/Microsoft.Dynamics.CRM." + actionName, true);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status >= 200 && this.status < 300) {
                    var result = '';

                    if (this.response !== '') {
                        result = JSON.parse(this.response);
                        console.log("Successful Action Call with Results: ");
                        console.log(result);
                    }

                    resolve(result);
                } else {
                    console.log("Error in action call: " + this.statusText);
                    reject(new Error(req.status + " - " + req.statusText));
                }
            }
        };
        
        // IE req http requires an empty JSON string to be passed in to Action if no body is found.
        if (typeof (parameters === 'undefined')) {
            parameters = {};
        }

        req.send(JSON.stringify(parameters));
    });
}

UDO.Shared.GetCurrentAppProperties = function () {
    return new Promise(function (resolve, reject) {
        var globalContext = Xrm.Utility.getGlobalContext();
        globalContext.getCurrentAppProperties().then(function (appProperties) {
            resolve(appProperties);
        }).catch(function (error) {
            reject(error);
        });
    });
}

UDO.Shared.DoesUserHaveRoles = function (listOfRoles) {
    return new Promise(function (resolve, reject) {
        var userId = Xrm.Utility.getGlobalContext().userSettings.userId.replace("{", "").replace("}", "");

        var fetchXMLForUserRoles = '<fetch no-lock="true">' +
                                    '<entity name="role" >' +
                                    '<attribute name="name" />' +
                                    '<link-entity name="systemuserroles" from="roleid" to="roleid" intersect="true" >' +
                                        '<filter type="and" >' +
                                        '<condition attribute="systemuserid" operator="eq" value="' + userId + '" />' +
                                        '</filter>' +
                                    '</link-entity>' +
                                    '</entity>' +
                                '</fetch>';

        Xrm.WebApi.AddRequestHeader("Prefer", "odata.include-annotations=OData.Community.Display.V1.FormattedValue");
        Xrm.WebApi.RetrieveByFetchXml("roles", fetchXMLForUserRoles).then(function (roles) {

            for (var i = 0; i < roles.value.length; i++) {
                var role = roles.value[i];

                for (var j = 0; j < listOfRoles.length; j++) {
                    if (listOfRoles[j].toLowerCase() === role.name.toLowerCase()) {
                        resolve(true);
                    }
                }
            }

            // not resolved, role not found
            resolve(false);
        }).catch(function (error) {
            reject(error);
        });
    });
}

UDO.Shared.setReadOnlyOnMultipleFields = function(fieldList, bool) {
    for (var i = 0; i < fieldList.length; i++) {
        UDO.Shared.SetReadOnly(fieldList[i], bool);
    }
}

UDO.Shared.SetRecordStatus = function (recordId, entityName, state, status) {
    return new Promise(function (resolve, reject) {
        var entity = {};
        entity.statecode = state;
        entity.statuscode = status;

        var lastletter = entityName[entityName.length - 1]

        var req = new XMLHttpRequest();
        req.open("PATCH", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/" + entityName + "(" + recordId + ")", true);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 204) {
                    resolve();
                } else {
                    reject(this.statusText);
                }
            }
        };
        req.send(JSON.stringify(entity));
    });
}

UDO.Shared.GetEntitySetName = function (logicalName)   {
    return new Promise(function (resolve, reject) {
        var req = new XMLHttpRequest();
        req.open("GET", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.1/EntityDefinitions(LogicalName='" + logicalName + "')?$select=DisplayName,IsKnowledgeManagementEnabled,EntitySetName", true);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    var result = JSON.parse(this.response);
                    resolve(result.EntitySetName);
                } else {
                    reject(this.statusText);
                }
            }
        };
        req.send(JSON.stringify());
    });
}

UDO.Shared.ShowProgressIndicator = function(message) {
    Xrm.Utility.showProgressIndicator(message);
}

UDO.Shared.CloseProgressIndicator = function() {
    Xrm.Utility.closeProgressIndicator();
}

UDO.Shared.openAlertDialog = function (message, title, height, width) {
    var defaultHeight = 200;
    var defaultWidth = 450;

    if (message === null) {
        message = "";
    }
    if (title === null){
        title = "";
    }
    if (height === null) {
        height = defaultHeight;
    }
    if (width === null) {
        width = defaultWidth;
    }

    var alertOptions = { height: height, width: width };
    var alertStrings = { title: title, text: message, confirmButtonLabel: "OK" };
    return Xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
}

UDO.Shared.openConfirmDialog = function (message, title, height, width, confirmButtonLabel, cancelButtonLabel) {
    var defaultHeight = 200;
    var defaultWidth = 450;

    if (message === null) {
        message = "";
    }
    if (title === null){
        title = "";
    }
    if (height === null) {
        height = defaultHeight;
    }
    if (width === null) {
        width = defaultWidth;
    }
    if (confirmButtonLabel === null) {
        confirmButtonLabel = "OK";
    }
    if (cancelButtonLabel === null) {
        cancelButtonLabel = "Cancel";
    }

    var confirmOptions = { height: height, width: width };
    var confirmStrings = { title: title, text: message, confirmButtonLabel: confirmButtonLabel, cancelButtonLabel: cancelButtonLabel };
    return Xrm.Navigation.openConfirmDialog(confirmStrings,confirmOptions);
}
