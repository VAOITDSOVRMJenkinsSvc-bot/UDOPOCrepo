var globalContext = Xrm.Utility.getGlobalContext();
var userSettings = globalContext.userSettings;
var guidForSensitivityWarnignBanner = "3CC79BF3-0EFB-44E5-9281-277295B41E7B";
var timelogId;

var VASS = VASS || {};
VASS.Interaction = VASS.Interaction || {};

function startTrackEvent(name, properties) {
    try {
        if (VASS.AppInsights.IsInitialized && VASS.AppInsights.startTrackEvent) {
            VASS.AppInsights.startTrackEvent(name, properties);
        }
    }
    catch (ex) {
        console.log("error occured during logging to app insights: " + ex.message)
    }

}
function stopTrackEvent(name, properties) {
    try {
        if (VASS.AppInsights.IsInitialized && VASS.AppInsights.stopTrackEvent) {
            VASS.AppInsights.stopTrackEvent(name, properties);
        }
    }
    catch (ex) {
        console.log("error occured during logging to app insights: " + ex.message)
    }
}

function trackException(ex) {
    try {
        if (VASS.AppInsights.IsInitialized && VASS.AppInsights.trackException) {
            VASS.AppInsights.trackException(ex);
        }
    }
    catch (ex) {
        console.log("error occured during logging to app insights: " + ex.message)
    }

}

function trackPageView(name, properties) {
    try {
        if (VASS.AppInsights.IsInitialized && VASS.AppInsights.trackPageView) {
            VASS.AppInsights.trackPageView(name, properties);
        }
    }
    catch (ex) {
        console.log("error occured during logging to app insights: " + ex.message)
    }

}

function MHEOSendEmail(primaryControl) {
    var subject = "Recap email";
    var to = primaryControl.getAttribute("udo_veteranemail").getValue();
    var body = encodeURIComponent("message goes here");

    window.location.href = "mailto:" + to + "?subject=" + subject + "&body=" + body;
}

function MHEOClickToCall(primaryControl) {
    alert("click to call placeholder");
}

function ContinueMHEOMarkComplete(primaryControl, dispositionValue) {
    var intrvl = primaryControl.getAttribute("udo_interactionmheointerval").getText();
    var confirmStrings = {};
    var confirmOptions = { height: 120, width: 260 };
    startTrackEvent("InteractionResolution", { "method": "MHEOMarkComplete", "description": "Agent clicks the resolve button. Disposition:" + dispositionValue });
    confirmStrings.title = "Confirm Resolve";
    confirmStrings.text = "Are you sure you want to resolve the " + intrvl + " interval?";
    Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions)
        .then(
            function (response) {
                if (response.confirmed) {
                    primaryControl.getAttribute("udo_mheocalldisposition").setValue(dispositionValue);
                    if (dispositionValue === 752280000) { //Successful call attempt
                        primaryControl.getAttribute("udo_benefitsdiscussed").setRequiredLevel("required");
                    }
                    else {
                        primaryControl.getAttribute("udo_benefitsdiscussed").setRequiredLevel("none");
                    }
                    primaryControl.data.save().then(function () { stopTrackEvent("InteractionResolution") });
                }
                else {
                    return;
                }
            },
            function (error) {
                Xrm.Navigation.openAlertDialog({ text: "Error resolving interaction" });
                trackException(error);

            });
}

function MHEOMarkComplete(primaryControl, dispositionValue, commandProperties) {
    if (commandProperties.SourceControlId !== null) {
        var sourceControlId = commandProperties.SourceControlId;
        if (sourceControlId === "udo_interaction|NoRelationship|Form|udo.udo_interaction.CloseSuccess.Button") {

            VASS.Interaction.DisableSuccessfullContactMade(primaryControl)
                .then(function success(result) {
                    if (result == false) {

                        var alertStrings = {
                            confirmButtonLabel: "OK",
                            text: "The resolution you selected is not applicable based on the latest call attempt.",
                            title: "Error Resolving Interaction"
                        };
                        Xrm.Navigation.openAlertDialog(alertStrings);
                        return;

                    } else {
                        ContinueMHEOMarkComplete(primaryControl, dispositionValue);
                        VASS.Interaction.ContinuelogEndTime(primaryControl);
                    }


                });
        } else if (sourceControlId === "udo_interaction|NoRelationship|Form|udo.udo_interaction.CloseFailure.Button") {
            VASS.Interaction.DisableFailedToMakeContact(primaryControl)
                .then(function success(result) {
                    if (result == false) {
                        var alertStrings = {
                            confirmButtonLabel: "OK",
                            text: "The resolution you selected is not applicable based on the latest call attempt.",
                            title: "Error Resolving Interaction"
                        };
                        Xrm.Navigation.openAlertDialog(alertStrings);
                        return;
                    } else {
                        ContinueMHEOMarkComplete(primaryControl, dispositionValue);
                        VASS.Interaction.ContinuelogEndTime(primaryControl);
                    }
                });
        } else {
            ContinueMHEOMarkComplete(primaryControl, dispositionValue);
            VASS.Interaction.ContinuelogEndTime(primaryControl);
        }
    }
}

function preventAutoSave(econtext) {
    var eventArgs = econtext.getEventArgs();
    if (eventArgs.getSaveMode() == 70) {
        eventArgs.preventDefault();       
    }
}
var emailUpdateNotificationId = "DCB34E7E-B45A-4BC0-9DC8-939D1C5CBD79";
function saveEmailNotification(context) {
    if (context != null) {
        var formContext = context.getFormContext();
        formContext.ui.setFormNotification("Please save the updated email address before a Call Attempt is logged.", "WARNING", emailUpdateNotificationId);
    }
}

function clearNotifications(context) {
    if (context != null) {
        var formContext = context.getFormContext();
        formContext.ui.clearFormNotification(emailUpdateNotificationId);
    }
}

var getVeteranUpdateNotificationId = "7E138512-1206-4A6F-9EB8-EF41EB385558";
function loadVeteranData(context) {
    try {
        var appInsightsProperties = { "method": "Load veterandata", "description": "Called on load of veteran" }
        var formContext = context.getFormContext();
        startTrackEvent("LoadVeteranData", appInsightsProperties);
        startTrackEvent("LoadVeteranData>RetrieveSettings", appInsightsProperties);
        Xrm.WebApi.retrieveMultipleRecords("mcs_setting", "?$select=udo_triggergetveteranupdateoninteraction&$top=1").then(
            function success(result) {
                stopTrackEvent("LoadVeteranData>RetrieveSettings", appInsightsProperties);
                if (result.entities.length > 0) {
                    setting = result.entities[0];
                    if (setting.udo_triggergetveteranupdateoninteraction) {

                        var contacts = formContext.getAttribute("udo_contactid").getValue();
                        if (contacts.length > 0) {
                            var contactId = contacts[0].id.replace('{', '').replace('}', '');

                            var target = {};
                            target.entityType = "contact";
                            target.id = contactId;

                            var req = {};
                            req.ParentEntityReference = target;
                            req.getMetadata = function () {
                                return {
                                    boundParameter: null,
                                    parameterTypes: {
                                        "ParentEntityReference": {
                                            typeName: "mscrm.contact",
                                            structuralProperty: 5
                                        }
                                    },
                                    operationType: 0,
                                    operationName: "udo_GetVeteranUpdates"
                                };
                            };
                            startTrackEvent("LoadVeteranData>CallGetVeteranUpdate");
                            Xrm.WebApi.online.execute(req).then(
                                function (data) {
                                    data.json().then(function (response) {
                                        if (response.DataIssue) {
                                            //formContext.ui.setFormNotification(response.ResponseMessage, "WARNING", getVeteranUpdateNotificationId);
                                            console.log("WARNING " + response.ResponseMessage);
                                        }
                                        else {
                                            //formContext.ui.setFormNotification(response.ResponseMessage, "INFO", getVeteranUpdateNotificationId);
                                            console.log("INFO " + response.ResponseMessage);
                                            formContext.data.refresh();
                                        }
                                        stopTrackEvent("LoadVeteranData>CallGetVeteranUpdate");
                                        stopTrackEvent("LoadVeteranData", appInsightsProperties);
                                    });
                                },
                                function (error) {
                                    //formContext.ui.setFormNotification("Failed to retrieve veteran data.", "ERROR", getVeteranUpdateNotificationId);
                                    console.log("ERROR " + error.message);
                                }
                            );
                        }
                    }
                    else {
                        //formContext.ui.setFormNotification("Get Veteran Update disabled on interaction", "INFO", getVeteranUpdateNotificationId);
                        console.log("INFO " + "Get Veteran Update disabled on interaction");
                    }
                }
            },
            function (error) {
                console.log("ERROR " + "Failed to retrieve settings.");
                //formContext.ui.setFormNotification("Failed to retrieve settings.", "ERROR", getVeteranUpdateNotificationId);
                // handle error conditions
            });
    }
    catch (ex) {
        //formContext.ui.setFormNotification("Failed to retrieve veteran data.", "ERROR", getVeteranUpdateNotificationId);
        console.log("ERROR " + ex.message);
    }
}




VASS.Interaction.validateFieldForCharacters = function (executionContext) {
    var formContext = executionContext.getFormContext();
    var pattern = /[^a-z ]/ig;
    var fieldName = executionContext.getEventSource().getName();
    var currentValue = formContext.getAttribute(fieldName).getValue();
    if (pattern.test(currentValue)) {
        formContext.getControl(fieldName).setNotification('Invalid characters, please enter only letters.');
    } else {
        formContext.getControl(fieldName).clearNotification();
    }
}


VASS.Interaction.validateFieldForNumbers = function (executionContext) {
    var formContext = executionContext.getFormContext()
    var pattern = /[^0-9,\-]/ig;
    var fieldName = executionContext.getEventSource().getName();
    var currentValue = formContext.getAttribute(fieldName).getValue();
    if (pattern.test(currentValue)) {
        formContext.getControl(fieldName).setNotification('Invalid characters, please enter only  numbers.');
    } else {
        formContext.getControl(fieldName).clearNotification();
    }
}

VASS.Interaction.checkifNoBenefitsSelected = function (executionContext) {
    var formContext = executionContext.getFormContext();
    var removeValue = [];
    var updateValues = formContext.getAttribute("udo_benefitsdiscussed").getValue();
    if (updateValues != null) {
        if ((updateValues.indexOf(752280014) >= 0) && updateValues.length > 1) {
            var indexOfNoBenfitsDiscussed = updateValues.indexOf(752280014);
            removeValue = updateValues.splice(indexOfNoBenfitsDiscussed, 1);
            formContext.getAttribute("udo_benefitsdiscussed").setValue(updateValues);

        }
    }
}

VASS.Interaction.validateBUOfVeterans = function (executionContext) {
    var formContext = executionContext.getFormContext();

    var SSN = formContext.getAttribute("udo_veteranssn").getValue();
    var firstName = formContext.getAttribute("udo_veteranfirstname").getValue() || "";
    var lastName = formContext.getAttribute("udo_veteranlastname").getValue() || "";
    var dob = formContext.getAttribute("udo_veterandob").getValue() || "";
    startTrackEvent("ValidateBUofVeterans>PersonSearch");
    if (SSN != null && firstName != null && lastName != null && dob != null) {
        var query = "?$select=*&$filter=crme_firstname eq '" + firstName + "' and crme_lastname eq '" + lastName + "' and crme_dobstring eq '" + dob + "' and crme_ssn eq '" + SSN + "' and crme_searchtype eq 'CombinedSearchByIdentifier' and crme_isattended eq true";
        Xrm.WebApi.retrieveMultipleRecords("crme_person", query)
            .then(
                function success(result) {
                    stopTrackEvent("ValidateBUofVeterans>PersonSearch");
                    if (result.entities != null) {
                        if (result.entities.length > 0) {
                            console.log("Entering if person exists")
                            if (result.entities[0].crme_returnmessage != null && result.entities[0].crme_returnmessage != "") {
                                console.log("Entering if return message  exists");
                                console.log(result.entities[0].crme_returnmessage);
                                if ((result.entities[0].crme_returnmessage.indexOf("lower Sensitivity Level than CSS") > -1) || (result.entities[0].crme_returnmessage.indexOf("sensitive file - access violation") > -1)) {
                                    hideContactSections(executionContext);
                                    console.log("Entering if return message mataches");
                                }
                                else {
                                    showContactSections(executionContext);
                                    console.log("Entering if return message doesn't match");
                                }
                            }
                            else { // display the contact sections if return message is empty 
                                showContactSections(executionContext);
                                console.log("Entering if no return message ");
                            }
                        }
                        else {
                            // display the contact sections if there is no matching data
                            showContactSections(executionContext);
                            console.log("Entering if no person");
                        }
                    }
                    else {
                        // display the contact sections if there is no matching data
                        showContactSections(executionContext);
                    }
                    stopTrackEvent("ValidateBUofVeterans>PersonSearch");
                }, function (error) {
                    // display the contact sections in case of any exceptions 
                    showContactSections(executionContext);
                    console.log(error);
                    trackException(error);
                    stopTrackEvent("ValidateBUofVeterans>PersonSearch");
                });
    }
    else {
        formContext.ui.tabs.get("detailsTab").sections.get("detailsTab_section_6").setVisible(true);
        formContext.ui.tabs.get("detailsTab").sections.get("detailsTab_section_7").setVisible(true);
        console.log("The values are null, Please check the values")
    }


}

function showContactSections(executionContext) {
    console.log("Entering funtion to show sections");
    var formContext = executionContext.getFormContext();
    formContext.ui.clearFormNotification(guidForSensitivityWarnignBanner);
    formContext.ui.tabs.get("detailsTab").sections.get("detailsTab_section_6").setVisible(true);
    formContext.ui.tabs.get("detailsTab").sections.get("detailsTab_section_7").setVisible(true);//Veteran Benifits
}

function hideContactSections(executionContext) {
    console.log("Entering funtion to hide sections");
    var formContext = executionContext.getFormContext();
    var bannerMessage = "This record is a sensitive level 8 or higher. Please follow current guidance to submit a supervisor callback request";
    formContext.ui.setFormNotification(bannerMessage, "WARNING", guidForSensitivityWarnignBanner);
    formContext.ui.tabs.get("detailsTab").sections.get("detailsTab_section_6").setVisible(false);
    formContext.ui.tabs.get("detailsTab").sections.get("detailsTab_section_7").setVisible(false);//Veteran Benifits
}

VASS.Interaction.executeContactDataAction = function (contactId, actionName, context, resolve, reject) {
    try {
        startTrackEvent("RunAction:" + actionName)
        var formContext = context.getFormContext();
        var target = {};
        target.entityType = "contact";
        target.id = contactId;
        var req = {};
        req.ParentEntityReference = target;
        req.getMetadata = function () {
            return {
                boundParameter: null,
                parameterTypes: {
                    "ParentEntityReference": {
                        typeName: "mscrm.contact",
                        structuralProperty: 5
                    }
                },
                operationType: 0,
                operationName: actionName
            };
        };
        Xrm.WebApi.online.execute(req).then(
            function (data) {
                data.json().then(function (response) {
                    if (response.DataIssue) {
                        //formContext.ui.setFormNotification(response.ResponseMessage, "WARNING", getVeteranUpdateNotificationId);
                        console.log("WARNING " + response.ResponseMessage);
                        stopTrackEvent("RunAction:" + actionName, { "DataIssue": "True" })
                        if (reject != null) reject(response);
                    }
                    else {
                        //formContext.ui.setFormNotification(response.ResponseMessage, "INFO", getVeteranUpdateNotificationId);
                        console.log("INFO " + response.ResponseMessage);
                        if (resolve != null) resolve(response);
                        stopTrackEvent("RunAction:" + actionName, { "DataIssue": "False" })
                        formContext.data.refresh();
                    }
                });
            },
            function (error) {
                //formContext.ui.setFormNotification("Failed to retrieve veteran data.", "ERROR", getVeteranUpdateNotificationId);
                console.log("ERROR " + error.message);
                if (reject != null) reject(error);
                trackException(error);
            }
        );
    }
    catch (ex) {
        //formContext.ui.setFormNotification("Failed to retrieve veteran data.", "ERROR", getVeteranUpdateNotificationId);
        console.log("ERROR " + ex.message);
    }
};

VASS.Interaction.loadContactData = function (context) {
    try {
        var appInsightsProperties = { "method": "VASS.Interaction.loadContactData", "description": "Called on load of veteran" }
        var formContext = context.getFormContext();
        startTrackEvent("LoadVeteranData", appInsightsProperties);
        startTrackEvent("LoadVeteranData>RetrieveSettings", appInsightsProperties);
        Xrm.WebApi.retrieveMultipleRecords("mcs_setting", "?$select=udo_triggergetveteranupdateoninteraction&$top=1").then(
            function success(result) {
                stopTrackEvent("LoadVeteranData>RetrieveSettings", appInsightsProperties);
                if (result.entities.length > 0) {
                    setting = result.entities[0];
                    if (setting.udo_triggergetveteranupdateoninteraction) {

                        var contacts = formContext.getAttribute("udo_contactid").getValue();
                        if (contacts.length > 0) {
                            var contactId = contacts[0].id.replace('{', '').replace('}', '');

                            var contactLoadPromise = new Promise(function (resolve, reject) {
                                VASS.Interaction.executeContactDataAction(contactId, "udo_GetVeteranUpdates", context, resolve, reject);
                            });

                            contactLoadPromise.then(function () {

                                VASS.Interaction.executeContactDataAction(contactId, "udo_GetAddresses", context, null, null);
                            });
                        }
                    }
                    else {
                        //formContext.ui.setFormNotification("Get Veteran Update disabled on interaction", "INFO", getVeteranUpdateNotificationId);
                        console.log("INFO " + "Get Veteran Update disabled on interaction");
                    }
                    stopTrackEvent("LoadVeteranData", appInsightsProperties);
                }
            },
            function (error) {
                console.log("ERROR " + "Failed to retrieve settings.");
                //formContext.ui.setFormNotification("Failed to retrieve settings.", "ERROR", getVeteranUpdateNotificationId);
                // handle error conditions
            });
    }
    catch (ex) {
        //formContext.ui.setFormNotification("Failed to retrieve veteran data.", "ERROR", getVeteranUpdateNotificationId);
        console.log("ERROR " + ex.message);
    }
};

VASS.Interaction.createTimeLog = function (executionContext) {
    formContext = executionContext.getFormContext();
    var interactionId = formContext.data.entity.getId().replace("{", "").replace("}", "");

    var entity = {
        "udo_interactionid@odata.bind": "/udo_interactiontimelogs(" + interactionId + ")"
    };

    Xrm.WebApi.online.createRecord("udo_interactiontimelog", entity).then(
        function success(result) {
            timelogId = result.id;
        },
        function (error) {
            console.log(error.message);
        }
    );
};

VASS.Interaction.ContinuelogEndTime = function (commandProperties, primaryControl) {
    var entity = {};
    entity.udo_endtime = new Date().toUTCString();

    Xrm.WebApi.online.updateRecord("udo_interactiontimelog", timelogId, entity).then(
        function success(result) {
            var updatedEntityId = result.id;
        },
        function (error) {
            Xrm.Utility.alertDialog(error.message);
        }
    );
}

VASS.Interaction.logEndTime = function (commandProperties, primaryControl) {
    /*
    if (commandProperties.SourceControlId !== null) {
        var sourceControlId = commandProperties.SourceControlId;
        if (sourceControlId === "udo_interaction|NoRelationship|Form|udo.udo_interaction.CloseSuccess.Button") {

            VASS.Interaction.DisableSuccessfullContactMade(primaryControl)
                .then(function success(result) {
                    if (result == false) {
                        var alertStrings = {
                            confirmButtonLabel: "OK",
                            text: "Please review and select the right resolution.",
                            title: "Error resolving interaction."
                        };
                        Xrm.Navigation.openAlertDialog(alertStrings);                        
                        return;
                    } else {
                        VASS.Interaction.ContinuelogEndTime(primaryControl);
                    }
                });
        } else if (sourceControlId === "udo_interaction|NoRelationship|Form|udo.udo_interaction.CloseFailure.Button") {
            VASS.Interaction.DisableFailedToMakeContact(primaryControl)
                .then(function success(result) {
                    if (result == false) {
                        var alertStrings = {
                            confirmButtonLabel: "OK",
                            text: "Please review and select the right resolution.",
                            title: "Error resolving interaction."
                        };
                        Xrm.Navigation.openAlertDialog(alertStrings);
                        return;
                    } else {
                        VASS.Interaction.ContinuelogEndTime(primaryControl);
                    }
                });
        } else {
            VASS.Interaction.ContinuelogEndTime(primaryControl);
        }
    } */
};

VASS.Interaction.onChangeReassignmentReason = function (executionContext) {
    var formContext = executionContext.getFormContext();
    var reassignReason = formContext.getAttribute("udo_reassignmentreasonnew").getValue();
    if (reassignReason == 752280000) {
        formContext.ui.setFormNotification("Reassignment reason is set to After Hours", "WARNING", "2");
    }
    else {
        formContext.ui.clearFormNotification("2");
    }

};

//To pass the execution context to the html webresource
//https://docs.microsoft.com/en-us/powerapps/developer/model-driven-apps/clientapi/reference/controls/getcontentwindow

VASS.Interaction.loadEnrollmentEligibility = function (executionContext) {
    var formContext = executionContext.getFormContext();
    var wrControl = formContext.getControl("WebResource_EligibilitySubgrid");
    if (wrControl) {
        wrControl.getContentWindow().then(
            function (contentWindow) {
                contentWindow.setContext(formContext, VASS);
            }
        )
    }
};

VASS.Interaction.formatContactName = function (executionContext) {
    var formContext = executionContext.getFormContext();
    var contact = formContext.getAttribute("udo_contactid").getValue();
    startTrackEvent("FormatContactName");
    if (contact != null) {
        var contactId = contact[0].id.replace('{', '').replace('}', '');

        var formattedFirstName;
        var formattedLastName;
        var formattedMiddleName;
        startTrackEvent("FormatContactName>GetContactDetails");
        Xrm.WebApi.online.retrieveRecord("contact", contactId, "?$select=firstname,lastname,middlename,udo_formattedfirstname,udo_formattedlastname,udo_formattedmiddlename").then(
            function success(result) {
                stopTrackEvent("FormatContactName>GetContactDetails");
                var puncArr = ["-", "'"];
                if (result["firstname"] != null || result["firstname"] != undefined) {
                    formattedFirstName = result["firstname"].toLowerCase();
                    for (i = 0; i < puncArr.length; i++) {
                        fNameArr = formattedFirstName.split(puncArr[i])
                        for (j = 0; j < fNameArr.length; j++) {
                            fNameArr[j] = titleCase(fNameArr[j]);
                        }
                        formattedFirstName = fNameArr.join(puncArr[i]);
                    }
                }
                if (result["lastname"] != null || result["lastname"] != undefined) {
                    formattedLastName = result["lastname"].toLowerCase();
                    for (i = 0; i < puncArr.length; i++) {
                        lNameArr = formattedLastName.split(puncArr[i])
                        for (j = 0; j < lNameArr.length; j++) {
                            lNameArr[j] = titleCase(lNameArr[j]);
                        }
                        formattedLastName = lNameArr.join(puncArr[i]);
                    }
                }
                if (result["middlename"] != null || result["middlename"] != undefined) {
                    formattedMiddleName = result["middlename"].toLowerCase();
                    for (i = 0; i < puncArr.length; i++) {
                        mNameArr = formattedMiddleName.split(puncArr[i])
                        for (j = 0; j < mNameArr.length; j++) {
                            mNameArr[j] = titleCase(mNameArr[j]);
                        }
                        formattedMiddleName = mNameArr.join(puncArr[i]);
                    }
                }

                if (formattedFirstName != result["udo_formattedfirstname"] ||
                    formattedLastName != result["udo_formattedlastname"] ||
                    formattedMiddleName != result["udo_formattedmiddlename"]) {
                    var entity = {
                        "udo_formattedfirstname": formattedFirstName,
                        "udo_formattedlastname": formattedLastName,
                        "udo_formattedmiddlename": formattedMiddleName
                    };

                    startTrackEvent("FormatContactName>UpdateFormatedNames");

                    Xrm.WebApi.online.updateRecord("contact", contactId, entity).then(
                        function success(result) {
                            var updatedEntityId = result.id;
                            stopTrackEvent("FormatContactName>UpdateFormatedNames");
                        },
                        function (error) {
                            console.log(error.message);
                            trackException(error);
                        }
                    );
                }
                stopTrackEvent("FormatContactName");
            },
            function (error) {
                console.log(error.message);
                stopTrackEvent("FormatContactName");
                trackException(error);
            }
        )
    }
};
//CRMUDO-3558
VASS.Interaction.ValidateVASSInteractionOrder = function (executionContext) {
    var formContext = executionContext.getFormContext();
    var status_check = formContext.getAttribute("statecode").getValue();
    if (status_check == "1") {
        return;//inactive , no need to check other condition
    }
    var vass_assignment = formContext.getAttribute("udo_mheointeractionassignmentid").getValue();
    if (vass_assignment != null) {
        var vass_assignment_id = vass_assignment[0].id.replace('{', '').replace('}', '');
        var stDate = formContext.getAttribute("udo_intervalstart").getValue();

        var stMonth = stDate.getMonth() + 1;
        var stDay = stDate.getDate();
        var stYear = stDate.getFullYear();
        var startDate = stMonth + "-" + stDay + "-" + stYear;

        var selected_interactionId = formContext.data.entity.getId().replace('{', '').replace('}', '');
        //Logic = If there are related Interaction to the VASS Interaction Assignment that have a status of 
        //active and have an Interaction that has an Interval End date
        //that is before the Interval Start date on the selected / clicked interaction.Show the warning message
        var fetchData = {
            "udo_mheointeractionassignmentid": vass_assignment_id,
            "udo_intervalstart": startDate,
            "udo_interactionid": selected_interactionId,
            "statecode": "0"
        };
        var fetchXml = [
            "?fetchXml=<fetch>",
            "  <entity name='udo_interaction'>",
            "    <attribute name='udo_intervalstart'/>",
            "    <attribute name='udo_intervalend'/>",
            "    <attribute name='statecode'/>",
            "    <filter>",
            "      <condition attribute='udo_mheointeractionassignmentid' operator='eq' value='", fetchData.udo_mheointeractionassignmentid, "'/>",
            "      <condition attribute='udo_intervalend' operator='lt' value='", fetchData.udo_intervalstart, "'/>",
            "      <condition attribute='udo_interactionid' operator='ne' value='", fetchData.udo_interactionid, "'/>",
            "      <condition attribute='statecode' operator='eq' value='", fetchData.statecode, "'/>",
            "    </filter>",
            "  </entity>",
            "</fetch>"
        ].join("");

        Xrm.WebApi.online.retrieveMultipleRecords("udo_interaction", fetchXml).then(
            function (results) {
                if (results.entities.length > 0) {

                    var alertStrings = {
                        confirmButtonLabel: "OK",
                        text: "Please review and resolve previous Interactions associated with this record.",
                        title: "Error resolving previous interaction(s)."
                    };
                    Xrm.Navigation.openAlertDialog(alertStrings);
                }

            },
            function (error) {
                console.log("ERROR " + error.message);
            }
        );


    }
};
//CRMUDO-3555
VASS.Interaction.DisableFailedToMakeContact = function (primaryControl) {
    return new Promise(function (resolve, reject) {
        var fetchData = {
            "udo_interactionid": primaryControl.data.entity.getId().replace('{', '').replace('}', ''),
            "statecode": "0"
        };
        var fetchXml = [
            "?fetchXml=<fetch>",
            "  <entity name='udo_outboundcallattempt'>",
            "    <attribute name='createdon'/>",
            "    <attribute name='udo_disposition'/>",
            "    <attribute name='statecode'/>",
            "    <attribute name='statuscode'/>",
            "    <filter>",
            "      <condition attribute='udo_interactionid' operator='eq' value='", fetchData.udo_interactionid, "'/>",
            "      <condition attribute='statecode' operator='eq' value='", fetchData.statecode, "'/>",
            "    </filter>",
            "    <order attribute='createdon' descending='true'/>",
            "  </entity>",
            "</fetch>"
        ].join("");

        Xrm.WebApi.online.retrieveMultipleRecords("udo_outboundcallattempt", fetchXml).then(
            function success(results) {
                if (results.entities.length > 0) {
                    var activeCallCounts = results.entities.length;
                    var lastDisposition = results.entities[0]["udo_disposition@OData.Community.Display.V1.FormattedValue"];
                    if (lastDisposition === "Bad Phone Number") {
                        //A Bad phone number disposition is logged as the latest attempt, or
                        resolve(true);//enable the button
                    } else if (activeCallCounts >= 7 && (lastDisposition !== "Successful Contact" && lastDisposition !== "Do Not Contact Request" && lastDisposition !== "Call Back Request")) {
                        //7 or more Call attempts are logged, and
                        //A Successful Contact or Do not Contact Request is not logged as the latest attempt.
                        resolve(true);
                    } else {
                        resolve(false);
                    }
                } else {
                    console.log("No active call records found for this interaction.");
                    resolve(false);
                }

            },
            function (error) {
                reject(error.message);
                console.log(error.message);
            }
        )

    });
};
//CRMUDO-3555
VASS.Interaction.DisableSuccessfullContactMade = function (primaryControl) {

    return new Promise(function (resolve, reject) {
        var fetchData = {
            "udo_interactionid": primaryControl.data.entity.getId().replace('{', '').replace('}', ''),
            "statecode": "0"
        };
        var fetchXml = [
            "?fetchXml=<fetch>",
            "  <entity name='udo_outboundcallattempt'>",
            "    <attribute name='createdon'/>",
            "    <attribute name='udo_disposition'/>",
            "    <attribute name='statecode'/>",
            "    <attribute name='statuscode'/>",
            "    <filter>",
            "      <condition attribute='udo_interactionid' operator='eq' value='", fetchData.udo_interactionid, "'/>",
            "      <condition attribute='statecode' operator='eq' value='", fetchData.statecode, "'/>",
            "    </filter>",
            "    <order attribute='createdon' descending='true'/>",
            "  </entity>",
            "</fetch>"
        ].join("");

        Xrm.WebApi.online.retrieveMultipleRecords("udo_outboundcallattempt", fetchXml).then(
            function success(results) {
                if (results.entities.length > 0) {
                    var lastDisposition = results.entities[0]["udo_disposition@OData.Community.Display.V1.FormattedValue"];
                    if (lastDisposition === "Successful Contact" || lastDisposition === "Do Not Contact Request") {
                        /*For a VASS interaction to be resolved as “Successful Contact Made”, the following must be met:
                        A Successful Contact or Do not Contact Request is logged as the latest attempt.
                        Need to make Successful contact made unselectable if the condition above is false.
                        */
                        resolve(true);
                    } else {
                        resolve(false);
                    }

                } else {
                    console.log("No active call records found for this interaction.");
                    resolve(false);
                }

            },
            function (error) {
                reject(error.message);
                console.log(error.message);
            }
        )

    });
};
//crmudo-3557
VASS.Interaction.EnforceResolution = function (executionContext) {

    var eventArgs = executionContext.getEventArgs();
    if (eventArgs != null || eventArgs != 'undefined' ) {
        //save save&close,save as completed, save and new, autosave
        if (eventArgs.getSaveMode() == 1 || eventArgs.getSaveMode() == 2 || eventArgs.getSaveMode() == 58 || eventArgs.getSaveMode() == 59 || eventArgs.getSaveMode() == 70) {
            eventArgs.preventDefault();
        }
    }// this will prevent save   

    var formContext = executionContext.getFormContext();
    var interactionId = formContext.data.entity.getId().replace('{', '').replace('}', '');
    var interactionDispostion = formContext.getAttribute("udo_mheocalldisposition").getValue();    
    //752,280,000 Successful Contact Made
    //752,280,001 Failed to Make Contact
    //752,280,002 Late Entry - Unable to Contact
    var fetchData = {
        "udo_interactionid": interactionId,
        "statecode": "0"
    };
    var fetchXml = [
        "?fetchXml=<fetch>",
        "  <entity name='udo_outboundcallattempt'>",
        "    <attribute name='udo_disposition'/>",
        "    <attribute name='udo_direction'/>",
        "    <attribute name='statecode'/>",
        "    <attribute name='createdon'/>",       
        "    <filter>",
        "      <condition attribute='udo_interactionid' operator='eq' value='", fetchData.udo_interactionid, "'/>",
        "      <condition attribute='statecode' operator='eq' value='", fetchData.statecode, "'/>",
        "    </filter>",
        "    <order attribute='createdon' descending='true'/>",
        "  </entity>",
        "</fetch>"
    ].join("");

    Xrm.WebApi.online.retrieveMultipleRecords("udo_outboundcallattempt", fetchXml).then(
        function success(results) {
            if (results.entities != null && results.entities.length > 0) {
                var noOfcallAttemptsRelated = results.entities.length;
                var lastDisposition = results.entities[0]["udo_disposition@OData.Community.Display.V1.FormattedValue"];
                var lastDispositionDirection = results.entities[0]["udo_direction@OData.Community.Display.V1.FormattedValue"];
                /*Successful Contact 752,280,000
                 Left Voicemail 752,280,002
                 Do Not Contact Request 752,280,003
                 Unable to Contact/No Voicemail Required 752,280,001
                 Bad Phone Number 752,280,005
                 Call Back Request 752,280,004
                 */
                var alertStrings = {
                    confirmButtonLabel: "OK",
                    text: "",
                    title: "Resolve Interaction"
                };
                if (noOfcallAttemptsRelated >= 7 && lastDisposition != "Call Back Request" && lastDispositionDirection == "Outbound" && interactionDispostion != "Failed to Make Contact") {
                    //Enforce Failed to Make Contact resolution
                    alertStrings.text = "This VASS Interaction must be resolved as Failed to Make Contact.";
                    Xrm.Navigation.openAlertDialog(alertStrings);
                } else if (lastDisposition == "Successful Contact" && lastDispositionDirection == "Outbound" && interactionDispostion != "Successful Contact Made") {
                    //Enforce Successful Contact Made resolution
                    alertStrings.text = "This VASS Interaction must be resolved as Successful Contact Made.";
                    Xrm.Navigation.openAlertDialog(alertStrings);
                } else {
                    //do nothing
                }
            }
        },
        function (error) {           
            console.log(error.message);
        }
    )//api call ends  
};

VASS.Interaction.LoadInteractionResolverMenu = function (commandProperties) {
   
    var commandId = "";

    if (commandProperties.SourceControlId !== null) {
        var source = commandProperties.SourceControlId.split("|");
        if (source.length > 3) {
            commandId = source[0] + "|" + source[1] + "|" + source[2] + "|udo." + source[0] + ".Close.Command";
        }
    }
   

    var menuItems = [
        "<Menu Id='udo.udo_interaction.Close.Button.Menu'>",
        "<MenuSection Id = 'udo.udo_interaction.Close.Section' Sequence = '5' DisplayMode = 'Menu16'>",
        "<Controls Id='udo.udo_interaction.Close.Section.Controls'>",
        "<Button Command='" + commandId + "' Id='udo.udo_interaction.CloseSuccess.Button2' LabelText='Successful Contact Made2' Sequence='10' ModernImage='$webresource:udo_check-32.svg' IsEnabled='False' />",
        "<Button Command='" + commandId + "' Id='udo.udo_interaction.CloseFailure.Button2' LabelText='Failed to Make Contact2' Sequence='15' ModernImage='$webresource:udo_wrong-32.svg' IsDisabled='True' />",
        "<Button Command='" + commandId + "' Id='udo.udo_interaction.CloseLateEntry.Button2' LabelText='Late Entry - Unable to Contact2' Sequence='20' ModernImage='$webresource:udo_message-warning-red-16.svg' />",
        "</Controls></MenuSection></Menu >"
    ].join("");

    commandProperties["PopulationXML"] = menuItems;
}

function titleCase(string) {
    var sentence = string.split(" ");
    for (var i = 0; i < sentence.length; i++) {
        sentence[i] = sentence[i][0].toUpperCase() + sentence[i].slice(1);
    }
    var formattedString = sentence.toString().replace(',', ' ').replace(',', ' ');
    return formattedString;
}

