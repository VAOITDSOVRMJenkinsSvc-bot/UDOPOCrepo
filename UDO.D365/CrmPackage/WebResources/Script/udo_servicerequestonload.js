"use strict";

//#region App Insights related functions
function startTrackEvent(name, properties) {
    try {
        if (Va.Udo.AppInsights.IsInitialized && Va.Udo.AppInsights.startTrackEvent) {
            Va.Udo.AppInsights.startTrackEvent(name, properties);
        }
    }
    catch (ex) {
        console.log("Error occured while logging startTrackEvent to App Insights: " + ex.message);
    }
}

function stopTrackEvent(name, properties) {
    try {
        if (Va.Udo.AppInsights.IsInitialized && Va.Udo.AppInsights.stopTrackEvent) {
            Va.Udo.AppInsights.stopTrackEvent(name, properties);
        }
    }
    catch (ex) {
        console.log("Error occured while logging stopTrackEvent to App Insights: " + ex.message);
    }
}

function trackException(ex) {
    try {
        if (Va.Udo.AppInsights.IsInitialized && Va.Udo.AppInsights.trackException) {
            Va.Udo.AppInsights.trackException(ex);
        }
    }
    catch (ex) {
        console.log("Error occured while logging trackException to App Insights: " + ex.message);
    }
}

function trackPageView(name, properties) {
    try {
        if (Va.Udo.AppInsights.IsInitialized && Va.Udo.AppInsights.trackPageView) {
            Va.Udo.AppInsights.trackPageView(name, properties);
        }
    }
    catch (ex) {
        console.log("Error occured while logging trackPageView to App Insights: " + ex.message);
    }
}
//#endregion

/*******************BEGIN correspondence OnLoad******************/

// These methods are called OnLoad from USD Action for correspondence interactions
var CorrSubType = null;
var exCon = null;
var formContext = null;
var webApi = null;
var secLib = null;
var ssrsReportName = '';
var reportId = '';
var globalContext = null;
var personType = null;
var confirmStrings = {};
confirmStrings.confirmButtonLabel = "Yes";
confirmStrings.cancelButtonLabel = "No";
var confirmOptions = { height: 120, width: 260 };
var alertStrings = {};
var alertOptions = { height: 120, width: 260 };
//define function calls at global level
window.parent.PopulateEnclosures = PopulateEnclosuresSR;
window.parent.SendEmailToRO = SendEmailToRO;
window.parent.copyMailingAddress = copyMailingAddress;

var BuildFilter = function () {
    //Only Correspondence Category
    var docTypeFilter = "<filter type='and'><condition attribute='udo_vbmscategory' operator='eq' value='752280000' />";
    //Only matching controlled correspondence type
    docTypeFilter += "<condition attribute='udo_controlledcorrespondencetype' operator='eq' value='" + CorrSubType + "' /></filter>";
    formContext.getControl("udo_vbmsdoctype").addCustomFilter(docTypeFilter, "udo_vbmsdoctype");
};

var FilterDocTypes = function () {
    formContext.getControl("udo_vbmsdoctype").addPreSearch(BuildFilter);
};

var SRFilterOnLoad = function (type, subtype) {

    CorrSubType = subtype;
    if (type === "752280000" && subtype !== null) {
        FilterDocTypes();
    }
};

/*******************END correspondence OnLoad******************/

/**************BEGIN main OnLoad event (formscript)************/
var Globals = {};
Globals.dynamics365AppId = "b2acc5aa-c267-ea11-a99d-001dd8009f4b";
Globals.selectedRequestStatus = null;
Globals.selectedRequestStatusValue = null;
Globals.loading = true;
Globals.sendMailButtonClicked = false;
Globals.sendEmailsThroughCode = null;
Globals.runningEmailGenAfterAutoSave = null;
Globals.originalRO = null;

Globals.srAction = {};
Globals.srAction.Action0820Value = 1;
Globals.srAction.Action0820aValue = 953850001;
Globals.srAction.Action0820dValue = 953850002;
Globals.srAction.Action0820fValue = 953850003;
Globals.srAction.ActionEmailFormsValue = 953850004;
Globals.srAction.ActionLetterValue = 953850006;
Globals.srAction.ActionNonEmergencyEmailValue = 953850007;
Globals.srAction.Action0820Text = '0820';
Globals.srAction.Action0820aText = '0820a';
Globals.srAction.Action0820dText = '0820d';
Globals.srAction.Action0820fText = '0820f';
Globals.srAction.ActionEmailFormsText = 'Email Forms';
Globals.srAction.ActionLetterText = 'Letter';
Globals.srAction.ActionNonEmergencyEmailText = 'Non Emergency Email';

Globals.srRequestStatus = {};
Globals.srRequestStatus.SentValue = 953850002;
Globals.srRequestStatus.SentText = 'Sent';
Globals.srRequestStatus.QueuedtoSendValue = 953850001;
Globals.srRequestStatus.QueuedtoSendText = 'Queued to Send';
Globals.srRequestStatus.ResolvedValue = 953850006;
Globals.srRequestStatus.ResolvedText = 'Resolved';
Globals.srRequestStatus.CancelledValue = 953850007;
Globals.srRequestStatus.CancelledText = 'Cancelled';

// This method will be call from CRM form
function OnLoad(execCon) {
    //Perform any initialization as required.
    var appInsightsProps = {method: "OnLoad", description: "Called on load of Service Request form."};
    startTrackEvent("UDO Service Request OnLoad", appInsightsProps);
    try {
        exCon = execCon;
        formContext = exCon.getFormContext();
        globalContext = Xrm.Utility.getGlobalContext();
        var version = globalContext.getVersion();
        var lib = new CrmCommonJS.CrmCommon(version, exCon);
        webApi = lib.WebApi;
        secLib = lib.Security;
        onFormLoad();
    }
    catch(e) {
        trackException(e);
    }
    stopTrackEvent("UDO Service Request OnLoad", appInsightsProps);
}

function GetExecutionContext() {
    return exCon;
}



// Begin Form onload event - retrieve data from VIP if necessary
function onFormLoad() {
    Globals.runningEmailGenAfterAutoSave = false;
    Globals.sendEmailsThroughCode = true;
    showHistoricalData();
    serviceRequest();
    getVBMSRole();
    ServiceTypeChange();
    reports.getReports();
    defaultQuickWrite();
}

function fetchContext() {
    return exCon;
}

// Show some of the  Notes section fields which contains the historical information for the migrated records. These fields will not show up for newly created UDO Service Request Records
function showHistoricalData() {
    try {
        var modifiedBy = formContext.getControl("modifiedby");
        var udCreatedBy = formContext.getControl("udo_udcreatedby");

        var udCreatedByAttr = formContext.getAttribute("udo_udcreatedby");
        if (udCreatedByAttr.getValue()) {
            //notesTab.setVisible(true);
            modifiedBy.setVisible(false);
            udCreatedBy.setVisible(true);
        }
        //else {
        //    //tab not visible by default
        //    notesTab.setVisible(false);
        //}

    }
    catch (e) {
        UDO.Shared.openAlertDialog("Encountered an error: " + e);
    }
}

function serviceRequest() {
    //Adding Functions to OnChange:
    formContext.getAttribute('udo_quickwriteid').addOnChange(QuickWriteChange);
    formContext.getAttribute('udo_action').addOnChange(ServiceTypeChange);
    //formContext.getAttribute('udo_action').addOnChange(EmailtoVet);
    formContext.getAttribute('udo_requeststatus').addOnChange(StatusChange);
    formContext.getAttribute('udo_processedfnodinshare').addOnChange(ProcessedInShareChange);
    formContext.getAttribute('udo_other').addOnChange(FnodOtherChange);

    if (formContext.getAttribute('udo_action').getValue() !== Globals.srAction.ActionLetterValue) {
        var control = formContext.ui.controls.get("udo_action");
        control.removeOption(Globals.srAction.ActionLetterValue);

    }

    if (formContext.getAttribute('udo_rpo').getValue() !== (Globals.srAction.ActionEmailFormsValue || null)) {
        formContext.getAttribute('udo_rpotext').setValue(formContext.getAttribute('udo_rpo').getText());
    }

    /** End event setting **/

    Globals.selectedRequestStatus = formContext.getAttribute("udo_requeststatus").getText();
    Globals.selectedRequestStatusValue = formContext.getAttribute("udo_requeststatus").getValue();

    if (Globals.selectedRequestStatusValue === Globals.srRequestStatus.SentValue) { formContext.getControl("udo_requeststatus").setDisabled(true); }


    //Events to happen when Service Request is New or Existing
    if (formContext.ui.getFormType() === CRM_FORM_TYPE_CREATE || formContext.ui.getFormType() === CRM_FORM_TYPE_UPDATE) {



        if (formContext.getAttribute('udo_srrelation').getValue() === null && formContext.getAttribute('udo_relationtoveteran').getValue() !== null) {
            formContext.getAttribute('udo_srrelation').setValue(formContext.getAttribute('udo_relationtoveteran').getText());
        }

        //If SOJ field is blank, prompt the user
        if (formContext.getAttribute('udo_action').getValue() !== null) {

            var action = formContext.getAttribute('udo_action').getSelectedOption().text;
            if (action !== Globals.srAction.ActionEmailFormsText) {
                if (!formContext.getAttribute('udo_regionalofficeid').getValue()) {
                    if (formContext.getAttribute('udo_eccssn').getValue() !== null) {
                        alertStrings.text = "Warning: RPO is empty.\n\nTo select RPO manually, click on the Lookup icon next to Send To box on the top of the screen.";
                        UDO.Shared.openAlertDialog(alertStrings.text).then(
                            function success(result) {
                                console.log("Alert dialog closed");
                            },
                            function (error) {
                                console.log(error.message);
                            }
                        );
                    }
                    else {
                        alertStrings.text = "Warning: SOJ is empty.\n\nTo select SOJ manually, click on the Lookup icon next to Send To box on the top of the screen.";
                        UDO.Shared.openAlertDialog(alertStrings.text).then(
                            function success(result) {
                                console.log("Alert dialog closed");
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
    else {
        formContext.getControl("udo_robutton").setVisible(false);
    }

    formContext.getControl('udo_deathrelatedinformationchecklists').setLabel('Answered questions concerning possible benefit entitlements referring to "Death Related Information Checklist" work aid');
    formContext.getControl('udo_benefitsstopfirstofmonth').setLabel('Advised the caller the benefits will be stopped the first of the month of death and that any payment issued following that date must be returned (if applicable)');
    formContext.getControl('udo_willroutereportofdeath').setLabel('Will route this report of death to Regional Office of Jurisdiction or PMC via encrypted email for stop payment processing');

    StatusChange();
    GetSystemSettings();

    Globals.originalRO = GetLookupId('udo_regionalofficeid');
    if (formContext.getAttribute('udo_sendnotestomapd').getValue() === true) {
        // Temporarily removing disable
        //disableFormFields(true);
    }

    Globals.loading = false;

    //Toggle visibility of Enclosures based on Action Type
    //cchannon 386442
    // if (formContext.getAttribute('udo_action').getValue() === Globals.srAction.ActionEmailFormsValue) {
    //     formContext.ui.tabs.get("tab_Enclosures").setVisible(true);
    // }

}

// UDO DEV TN: IGNORE - Not Used, this was original attempt to keep iFrame, but not supported so udo_embeddedbutton.html is used. 
function setUpButtons(iFrameCon) {
    var webResourceUrl = globalContext.getClientUrl() + '/WebResources/va_';
    var source = iFrameCon.getEventSource();

    if (formContext.ui.getFormType() === CRM_FORM_TYPE_CREATE || formContext.ui.getFormType() === CRM_FORM_TYPE_UPDATE) {
        switch (source.getLabel()) {
            case "GetRO":
                //Send E-mail button
                Va.Udo.Crm.Scripts.Buttons.AddButton(formContext, source.getName(), (Globals.sendEmailsThroughCode ? "Send E-mail" : "Set Service Request as Sent"),
                    "Click to send email", SendEmailToRO, webResourceUrl + 'email_go.png');
                break;
            case "PopulateEnclosures":
                //Enclosures button
                Va.Udo.Crm.Scripts.Buttons.AddButton(formContext, source.getName(), "Populate Enclosures", "Click to Populate Enclosures", PopulateEnclosures, webResourceUrl + 'arrow_refresh.png');
                break;
            case "CopyAddress":
                //Copy veteran's address        
                Va.Udo.Crm.Scripts.Buttons.AddButton(formContext, source.getName(), "Copy Address", "Click to copy address", copyMailingAddress, "");
        }
    }
}
/*******************************END ONLOAD EVENT*****************************************/

//Retrieve the system settings information related to Service Request
var serviceUri = "";
var emailMessage = "";
var email0820Message = "";
var createSRConfirmMessage = "";
function GetSystemSettings() {
    var columns = ['va_description', 'va_name'];
    var filter = "$filter=startswith(va_name,'UDO_SR_')";
    webApi.RetrieveMultiple("va_systemsettings", columns, filter)
        .then(function (data) {
            if (data && data.length > 0) {
                for (var i = 0; i < data.length; i++) {

                    var name = data[i].va_name;//va_Description;
                    switch (name) {
                        case 'UDO_SR_RepWS':
                            serviceUri = data[i].va_description;
                            break;
                        case 'UDO_SR_EmailMessage':
                            emailMessage = data[i].va_description;
                            break;
                        case 'UDO_SR_Email0820Message':
                            email0820Message = data[i].va_description;
                            break;
                        case 'UDO_SR_CreateSRConfirmMessage':
                            createSRConfirmMessage = data[i].va_description;
                            break;
                    }
                }
            }
        })
        .catch(function (err) {
            HandleRestError(err, 'Failed to retrieve station data');
        });
}

////Set the send email to veteran field when the action is set to Email Forms
//function EmailtoVet() {
//    if (formContext.getAttribute('udo_action').getValue() !== null) {
//        if (formContext.getAttribute('udo_action').getValue() === Globals.srAction.ActionEmailFormsValue) {
//            formContext.getAttribute('udo_sendemailtoveteran').setValue(true);
//        }
//        else {
//            formContext.getAttribute('udo_sendemailtoveteran').setValue(false);
//        }
//    }
//}

function StatusChange() {
    // if sent or cancelled, cannot change record
    var status = formContext.getAttribute("udo_requeststatus").getText();
    var alertStrings = {};
    var alertOptions = { height: 120, width: 260 };
    //Globals.selectedRequestStatus = status;
    var selectedRequestStatusValue = formContext.getAttribute("udo_requeststatus").getValue();

    var isStatusSentOrresolved = (selectedRequestStatusValue === Globals.srRequestStatus.SentValue || selectedRequestStatusValue === Globals.srRequestStatus.QueuedtoSendValue || selectedRequestStatusValue === Globals.srRequestStatus.ResolvedValue || selectedRequestStatusValue === Globals.srRequestStatus.CancelledValue);

    if (isStatusSentOrresolved) {
        disableFormFields(true);
    }

    // only admins can change status
    // If not 0820, PCRs can change it to Resolved, if current status allows
    var action = (formContext.getAttribute("udo_action").getSelectedOption() === null ? '' : formContext.getAttribute("udo_action").getText());
    var vai0820 = (action === Globals.srAction.Action0820Text || action === Globals.srAction.Action0820aText || action === Globals.srAction.Action0820fText || action === Globals.srAction.Action0820dText);
    if (!Globals.loading) {

        var canChangeStatus = (secLib.UserHasRole("System Administrator") || secLib.UserHasRole("Supervisor") || (!vai0820 && Globals.selectedRequestStatusValue !== Globals.srRequestStatus.SentValue && Globals.selectedRequestStatusValue !== Globals.srRequestStatus.QueuedtoSendValue));
        if (!canChangeStatus) {
            alertStrings.text = "Combination of your security role and current service request status does not allow you to modify status; use 'Send Email' button to send Service Request and attached reports via email.";
            UDO.Shared.openAlertDialog(alertStrings.text).then(
                function success(result) {
                    formContext.getAttribute("udo_requeststatus").setValue(Globals.selectedRequestStatusValue);
                },
                function (error) {
                    console.log(error.message);
                }
            );
            return;
        }
    }

    if (!Globals.loading && (selectedRequestStatusValue === Globals.srRequestStatus.QueuedtoSendValue || selectedRequestStatusValue === Globals.srRequestStatus.SentValue)) {
        var alertMsg = status + " cannot be set manually; use 'Send Email' button instead and/or workflow processing rules.";
        UDO.Shared.openAlertDialog(alertMsg).then(
            function success(result) {
                formContext.getAttribute("udo_requeststatus").setValue(Globals.selectedRequestStatusValue);
            },
            function (error) {
                console.log(error.message);
            }
        );
        //ro = (Globals.selectedRequestStatusValue === Globals.srRequestStatus.SentValue || Globals.selectedRequestStatusValue === Globals.srRequestStatus.ResolvedValue || Globals.selectedRequestStatusValue === Globals.srRequestStatus.CancelledValue);
        disableFormFields(true);
    }
}

function doesControlHaveAttribute(control) {
    var controlType = control.getControlType();
    return controlType !== "iframe" && controlType !== "webresource" && controlType !== "subgrid" && control.setDisabled !== undefined;
}

function disableFormFields(onOff) {
    formContext.ui.controls.forEach(function (control, index) {
        if (doesControlHaveAttribute(control)) {
            control.setDisabled(onOff);
        }
    });
}

//Populate the enclosure text area which is referred in the reports/email
function PopulateEnclosuresSR() {
    var enclosures = '';
    var columns = ['udo_servicerequestid', '&$expand=udo_udo_servicerequest_va_externaldocument($select=va_externaldocumentid, va_name, va_documentlocation)'];


    webApi.RetrieveRecord(formContext.data.entity.getId(), "udo_servicerequest", columns)
        .then(function (data) {
            if (data && data.udo_udo_servicerequest_va_externaldocument.length > 0) {
                var externalDoc = data.udo_udo_servicerequest_va_externaldocument;
                for (var i = 0; i < externalDoc.length; i++) {
                    //var r = data[i].va_externaldocumentid;
                    enclosures += (enclosures.length > 0 ? '\n' : '') + externalDoc[i].va_name;
                    if (formContext.getAttribute('udo_enclosures').getValue() !== enclosures) {
                        formContext.getAttribute('udo_enclosures').setValue(enclosures);
                        // update data to make sure enclosures get on report
                        if (formContext.ui.getFormType() === CRM_FORM_TYPE_UPDATE) {
                            webApi.UpdateRecord(formContext.data.entity.getId(), "udo_servicerequest", { udo_enclosures: enclosures })
                                .then(function (data) {
                                })
                                .catch(
                                    function (err) {
                                        HandleRestError(err, 'Failed to update servicerequest enclosures');
                                    });
                        }
                    }

                }
            }
        }).catch(
            function (err) {
                HandleRestError(err, 'Failed to retrieve service request external document data');
            });


}



// TODO: change function to match 193 in udo_letter_onload.js
// Review needed - Matt/Rahul determine where filedownloaded comes from. 
function SendEmailToRO(filedownloaded) {

    //Check to make sure form is saved before continuing.
    if (formContext.ui.getFormType() === CRM_FORM_TYPE_CREATE) {
        UDO.Shared.openAlertDialog("You have to save the form before proceeding.","Alert", 120, 260).then(
            function success(result) {
                console.log("Alert dialog closed");
            },
            function (error) {
                console.log(error.message);
            }
        );
        return;
    }

    if (formContext.getAttribute("udo_requeststatus").getSelectedOption().value === Globals.srRequestStatus.SentValue) { //Sent
        var msg = 'Current Request Status is "Sent". Please confirm that you would like to send an email again.';
        var title = "Confirm Status";
        confirmStrings.title = title;
        confirmStrings.text = msg;
        UDO.Shared.openConfirmDialog(msg, title, 300, 550, "OK", "Cancel")
        //UDO.Shared.openConfirmDialog(confirmStrings, confirmOptions)
            .then(
                function (response) {
                    if (response.confirmed) {
                        confirmSend(filedownloaded);
                    }
                    else {
                        return;
                    }
                },
                function (error) {
                    UDO.Shared.openAlertDialog("Error resending email");

                });


    }
    else {
        confirmSend(filedownloaded);
    }
    function confirmSend(filedownloaded) {
        var roId = GetLookupId('udo_regionalofficeid');
        var action = formContext.getAttribute("udo_action").getSelectedOption().text;
        var is0820 = (action === Globals.srAction.Action0820Text || action === Globals.srAction.Action0820aText || action === Globals.srAction.Action0820fText || action === Globals.srAction.Action0820dText || action === Globals.srAction.ActionEmailFormsText);

        formContext.getControl("udo_update").setDisabled(false);
        formContext.getAttribute("udo_update").setSubmitMode('always');
        if (is0820) {
            var script0 = emailMessage;
            if (action === Globals.srAction.Action0820dText) script0 += email0820Message;

            // send mail could be running after automatic save. In this case, script was already prompted
            if (!Globals.runningEmailGenAfterAutoSave) {
                UDO.Shared.openConfirmDialog(script0, "", 400, 500).then(
                    function (success) {
                        SendEmailToROContinue(filedownloaded);
                    },
                    function (error) {
                        return;
                    });
        
                    // if (!confirm(script0)) {
                    //     return;
                    // } else {
                    //     SendEmailToROContinue(filedownloaded);
                    // }
            }

            var rs = formContext.getAttribute('udo_readscript').getValue();
            if (rs === null || rs === false) formContext.getAttribute('udo_readscript').setValue(true);

        } else {

            SendEmailToROContinue(filedownloaded);
        }
    }
}

function SendEmailToROContinue(filedownloaded) {


    if (Globals.sendEmailsThroughCode) {
        CreateOutlookEmail2(filedownloaded);
    }
}

//not used.  Remove
function ConfirmSendEmail() {
    // REM: TN: Replaced popup with openConfirmDialog
    return new Promise(function (resolve, reject) {
        var msg = "Would you like to mark this record as Sent?";
        var title = "Mark as Sent";
        var confirmStrings = { title: title, text: msg, confirmButtonLabel: "Yes", cancelButtonLabel: "No" };
        UDO.Shared.openConfirmDialog(msg, title, 300, 550, "Yes", "No")
        //UDO.Shared.openConfirmDialog(confirmStrings)
            .then(
                function (response) {
                    _letterGenerationThroughCode = true;
                    formContext.getAttribute("udo_requeststatus").setValue(Globals.srRequestStatus.SentValue); //Sent
                    formContext.data.save();
                    return resolve();
                },
                function (error) {
                    return reject();

                });
    });

    // TODO: replace with Xrm.Navigation (above)
    /*
    var dfd = $.Deferred();
    var msg = "Would you like to mark this record as Sent?";
    var title = "Mark as Sent";
    var buttons = Va.Udo.Crm.Scripts.Popup.PopupStyles.YesNo;
    var style = Va.Udo.Crm.Scripts.Popup.PopupStyles.Question;
    Va.Udo.Crm.Scripts.Popup.MsgBox(msg, buttons + style, title)
    .done(function (data) {
        Globals.sendMailButtonClicked = true;
        formContext.getAttribute("udo_requeststatus").setValue(Globals.srRequestStatus.SentValue); //Sent
        formContext.data.save("save");
        setTimeout(function () {
            dfd.resolve();
        }, 2000); // resolve 2 seconds after the popup is gone.
    })
    .fail(function (data) {
        setTimeout(function () {
            dfd.reject();
        }, 2000); // reject 2 seconds after the popup is gone.
    });
    return dfd.promise();
    */
}

function ProcessClaimEstablishment() {
    var dfd = $.Deferred();

    //var action = formContext.getAttribute("udo_action").getSelectedOption().text;
    //var requesttype = formContext.getAttribute("udo_requesttype").getValue();
    //var requestsubtype = formContext.getAttribute("udo_requestsubtype").getValue();
    var personid = formContext.getAttribute("udo_personid").getValue();

    //if (action === Globals.srAction.Action0820Text && requesttype === "FOIA/Privacy Act" && requestsubtype === "Release of Records") {
    formContext.ui.clearFormNotification("898");
    Xrm.Utility.showProgressIndicator("Processing");

    Va.Udo.Crm.CustomAction.ClaimEstablishment.Initiate(personid[0].id,
        function (data) {

            if (data.DataIssue === true || data.Timeout === true || data.Exception === true) {
                formContext.ui.setFormNotification("Claim Establishment Initiate Error - " + data.ResponseMessage,
                    "ERROR",
                    "898");
                Xrm.Utility.closeProgressIndicator();
                dfd.reject("Claim Establishment Initiate Error - " + data.ResponseMessage);
            }
        },
        function (resultid) {

            Va.Udo.Crm.CustomAction.ClaimEstablishment.UpdateClaimEstablishmentRecord(resultid)
                .then(
                    function (resultid) {

                        Va.Udo.Crm.CustomAction.ClaimEstablishment.Insert(resultid,
                            function (data) {

                                if (data.DataIssue === true || data.Timeout === true || data.Exception === true) {
                                    Xrm.Utility.closeProgressIndicator();
                                    formContext.ui.setFormNotification("Claim Establishment Insert Error - " + data.ResponseMessage, "ERROR", "898");
                                    dfd.reject("Claim Establishment Insert Error - " + data.ResponseMessage);
                                }

                            },
                            function (resultid) {

                                Va.Udo.Crm.CustomAction.ClaimEstablishment.Clear(resultid,
                                    function (data) {

                                        if (data.DataIssue === true || data.Timeout === true || data.Exception === true) {
                                            Xrm.Utility.closeProgressIndicator();
                                            formContext.ui.setFormNotification("Claim Establishment Clear Error - " + data.ResponseMessage, "ERROR", "898");
                                            dfd.reject("Claim Establishment Clear Error - " + data.ResponseMessage);
                                        }

                                    },
                                    function (resultid) {

                                        Va.Udo.Crm.CustomAction.ClaimEstablishment.CreateNote(formContext, "FOIA/Privacy Act Request");
                                        Xrm.Utility.closeProgressIndicator();
                                        formContext.ui.setFormNotification("Claim Establishment EP 510 was Inserted and Cleared Completed", "INFORMATION", "898");
                                        dfd.resolve();
                                    },
                                    function (err) {
                                        Xrm.Utility.closeProgressIndicator();
                                        dfd.reject();
                                    });
                            });

                    })
                .catch(
                    function (err) {
                        Xrm.Utility.closeProgressIndicator();
                        formContext.ui.setFormNotification("Claim Establishment Update Error - " + err, "ERROR", "898");
                    });

        },
        function (err) {
            Xrm.Utility.closeProgressIndicator();
            dfd.reject();
        });

    //} else {
    //    dfd.reject();
    //}

    return dfd.promise();
}

// REM: Made to Match line 148 - 165 in udo_letter_onload.js
function ConfirmMarkAsSent() {
    // REM: TN: Replaced popup with openConfirmDialog
    return new Promise(function (resolve, reject) {
        var msg = "Would you like to mark this record as Sent?";
        var title = "Mark as Sent";
        var confirmStrings = { title: title, text: msg, confirmButtonLabel: "Yes", cancelButtonLabel: "No" };
        var confirmOptions = { height: 120, width: 260 };
        UDO.Shared.openConfirmDialog(msg, title, 120, 260, "OK", "Cancel")
            .then(
                function (response) {
                    Globals.sendMailButtonClicked = true;
                    formContext.getAttribute("udo_requeststatus").setValue(Globals.srRequestStatus.SentValue); //Sent
                    formContext.data.save();
                    return resolve();
                },
                function (error) {
                    return reject();

                });
    });
    // return dfd.promise();
    // TODO: Replace Popup with Xrm.Navigation (above)
    /*
    var dfd = $.Deferred();
    var msg = "Would you like to mark this record as Sent?";
    var title = "Mark as Sent";
    var buttons = Va.Udo.Crm.Scripts.Popup.PopupStyles.YesNo;
    var style = Va.Udo.Crm.Scripts.Popup.PopupStyles.Question;
    
    Va.Udo.Crm.Scripts.Popup.MsgBox(msg, buttons + style, title)
    .done(function (data) {
        Globals.sendMailButtonClicked = true;
        formContext.getAttribute("udo_requeststatus").setValue(Globals.srRequestStatus.SentValue); //Sent
        formContext.data.save("save");
        setTimeout(function () {
            dfd.resolve();
        }, 2000); // resolve 2 seconds after the popup is gone.
    })
    .fail(function (data) {
        setTimeout(function () {
            dfd.reject();
        }, 2000); // reject 2 seconds after the popup is gone.
    });
    return dfd.promise();
    */
}

//Create the email and populate the necessary details
function CreateOutlookEmail2(filedownloaded) {

    var reportName = '';
    var action = null;
    var dispSubType = formContext.getAttribute("udo_disposition").getText();
    if (dispSubType === undefined) dispSubType = '';
    var arrFiles = null;
    var alertMsg;
    var alertStrings = {};
    var alertOptions = { height: 120, width: 260 };
    action = formContext.getAttribute("udo_action").getSelectedOption().text;
    switch (action) {
        case "0820":
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
        default:
            break;
    }

    var doCreatePDF = true;

    if (!reportName || reportName.length === 0) {
        doCreatePDF = false;
    }
    else {

        reportName = reportName + " - UDO";
        ssrsReportName = reportName;
        reportId = getReportID(ssrsReportName);
    }


    var isDirty = formContext.data.entity.getIsDirty();

    // get forms attached to email
    var attachmentList = '',
        attachmentListx = '',
        enclosures = '';
    var attachUrlList = new Array();
    // get attachments
    if (action !== Globals.srAction.ActionNonEmergencyEmailText) {
        var columns = ["udo_servicerequestid", "&$expand=udo_udo_servicerequest_va_externaldocument($select=va_externaldocumentid, va_name, va_documentlocation)"];
        //var filter = "filter=udo_servicerequestid eq " + formContext.data.entity.getId().replace("{", "").replace("}", "") + ")";
        webApi.RetrieveRecord(formContext.data.entity.getId(), "udo_servicerequest", columns)
            .then(function (data) {
                if (data && data.udo_udo_servicerequest_va_externaldocument.length > 0) {
                    var externalDoc = data.udo_udo_servicerequest_va_externaldocument;
                    for (var i = 0; i < externalDoc.length; i++) {
                        var r = externalDoc[i].va_externaldocumentid;
                        if (externalDoc[i].va_name !== 'Home Loans' && externalDoc[i].va_name !== 'Education'
                            && externalDoc[i].va_name !== 'VR&E' && externalDoc[i].va_name !== 'Life Insurance' && externalDoc[i].va_name !== 'Pension') {

                            attachmentList += ("<a href='" + externalDoc[i].va_documentlocation + "'>" + externalDoc[i].va_name + "</a><br/>");
                            attachUrlList.push(externalDoc[i].va_documentlocation);

                        }
                        else {
                            attachmentListx += ("<a href='" + externalDoc[i].va_documentlocation + "'>" + externalDoc[i].va_name + "</a><br/>");
                        }
                        enclosures += (enclosures.length > 0 ? '\n' : '') + externalDoc[i].va_name;

                        /*columns = ['va_name', 'va_documentlocation'];
                        filter = "$filter=va_externaldocumentid eq " + r.replace("}", "").replace("}", "") + ")";
                        webApi.RetrieveMultiple("va_externaldocument", columns, filter)
                            .then(function (data) {
                                if (data && data.length > 0) {
                                    if (data[0].va_name !== 'Home Loans' && data[0].va_name !== 'Education'
                                        && data[0].va_name !== 'VR&E' && data[0].va_name !== 'Life Insurance' && data[0].va_name !== 'Pension') {
                                        attachmentList += ("<a href='" + data[0].va_documentlocation + "'>" + data[0].va_name + "</a><br/>");
                                        attachUrlList.push(data[0].va_documentlocation);
                                    }
                                    else {
                                        attachmentListx += ("<a href='" + data[0].va_documentlocation + "'>" + data[0].va_name + "</a><br/>");
                                    }
                                    enclosures += (enclosures.length > 0 ? '\n' : '') + data[0].va_name;
                                }
                            })
                            .catch(function (error) { HandleRestError(error, 'Failed to retrieve external document data'); });*/
                    }
                }
                attachmentCallback();
            })
            .catch(function (error) { HandleRestError(error, "Failed to retrieve service request external document data"); });
    }
    else {
        attachmentCallback();
    }

    function attachmentCallback() {
        if (formContext.getAttribute('udo_enclosures').getValue() !== enclosures) {
            formContext.getAttribute('udo_enclosures').setValue(enclosures);
            // update data to make sure enclosures get on report

            if (formContext.ui.getFormType() === CRM_FORM_TYPE_UPDATE) {

                webApi.UpdateRecord(formContext.data.entity.getId(), "udo_servicerequest", { udo_enclosures: "enclosures" })
                    .then(
                        function (data) {
                        })
                    .catch(function (err) {
                        HandleRestError(err, 'Failed to update servicerequest enclosures');
                    });

            }
        }

        if (doCreatePDF) {
            if (isDirty) {
                //if (confirm('Some of the data on the screen has changed and was not saved. Would you like to save the record before sending e-mail?\n\nIf you select OK, the screen will reload after save, and sending e-mail will resume.\n\nSelect Cancel to skip saving and go ahead with email generation.')) {
                formContext.data.save("save");
                //return false;
                //} 
            }

            // call ws to get the attachment file names from shared folder
            var names = '';
            var request = null;
            try {
                //request = new ActiveXObject('Microsoft.XMLHTTP');
            } catch (err) { UDO.Shared.openAlertDialog(err.message); }

            if ((request === null) && window.XMLHttpRequest) {
                request = new XMLHttpRequest();
            } else if (request === null) {
                alertMsg = "Exception: Failed to create XML HTTP Object. Failed to execute web service request";
                UDO.Shared.openAlertDialog(alertMsg).then(
                    function success(result) {
                        console.log("Alert dialog closed");
                    },
                    function (error) {
                        console.log(error.message);
                    }
                );
                return false;
            }

            //var env = '<?xml version="1.0" encoding="utf-8"?><soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" //xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">' + '<soap12:Body><DownloadReport //xmlns="http://tempuri.org/">' + '<serviceRequestId>' + formContext.data.entity.getId() + '</serviceRequestId>' + '<action>' + action.replace('&', '&amp;') //+ '-UDO</action>' + '<dispSubType>' + dispSubType.replace('&', '&amp;') + '</dispSubType>' + '</DownloadReport></soap12:Body></soap12:Envelope>';

            try {
                ShowProgress('Generating Report Output');

                //request.open('POST', serviceUri, false);
                //request.setRequestHeader('SOAPAction', '');
                //request.setRequestHeader('Content-Type', 'text/xml; charset=utf-8');
                //request.setRequestHeader('Content-Length', env.length);

                //request.send(env);
            } catch (rex) {
                request = null;
                alertMsg = "Call to the Web Service to generate the report had failed: " + rex.description;
                UDO.Shared.openAlertDialog(alertMsg).then(
                    function success(result) {
                        console.log("Alert dialog closed");
                    },
                    function (error) {
                        console.log(error.message);
                    }
                );
                return false;
            } finally {
                CloseProgress();
            }

            //names = request.responseText;
            //if (names) {
            //var rx = parseXmlObject(names);
            //var resNode = rx.selectSingleNode('//DownloadReportResult');
            //if (!resNode) {
            //names = 'Exception: No response from Report Generation WS.';
            //} else {
            //names = resNode.text;
            //}
            //}
            //request = null;

            //if (!names || names.length === 0 || names.indexOf('Exception') !== -1) {
            //UDO.Shared.openAlertDialog('Call to Report Generation Web Service had failed or no reports had been //generated.\n\n' + names);
            //return false;
            //}
            //arrFiles = names.split(',');
        }

        var to = '';

        // get RO email
        var sendToRO = true,
            sendToVet = (formContext.getAttribute('udo_sendemailtoveteran').getValue()),
            sojAddress = '';
        var roId = GetLookupId('udo_regionalofficeid');
        if (!roId) {
            roId = Globals.originalRO;
            sendToRO = false;
        }

        if (roId) {

            var cols = ['emailaddress', '_va_intakecenterid_value', '_va_pensioncenterid_value'];

            webApi.RetrieveRecord(roId, "va_regionaloffice", cols)
                .then(function (data) {
                    if (data) {
                        var r = data;
                        to = r.emailaddress;
                        if (formContext.getAttribute('udo_letteraddressing').getValue() === 953850000 && r["_va_intakecenterid_value"])//Compensationi
                        {
                            //todo: update to webapi
                            var columns = ['udo_returnmailingaddress'];

                            webApi.RetrieveRecord(r["_va_intakecenterid_value"], "va_intakecenter", columns)
                                .then(function (data) {
                                    if (data && data.udo_returnmailingaddress) {
                                        sojAddress = data.udo_returnmailingaddress.replaceAll('\n', '<br/>');
                                    }
                                    roIDCallback();
                                }).catch(function (err) { HandleRestError(err, 'Failed to retrieve intake center data'); });
                        }
                        else if (r["_va_pensioncenterid_value"]) { //Pension Center
                            //todo: update to webapi
                            var cols2 = ['udo_returnmailingaddress'];

                            webApi.RetrieveRecord(r["_va_pensioncenterid_value"], "va_pensioncenter", cols2)
                                .then(function (data) {
                                    if (data && data.udo_returnmailingaddress) {
                                        sojAddress = data.udo_returnmailingaddress.replaceAll('\n', '<br/>');
                                    }
                                    roIDCallback();
                                })
                                .catch(function (err) { HandleRestError(err, 'Failed to retrieve pension center data'); });
                        }
                        else {
                            roIDCallback();
                        }
                    }
                    else {
                        roIDCallback();
                    }
                }).catch(function (err) { HandleRestError(err, 'Failed to retrieve regional office data'); });
        }
        else {
            roIDCallback();
        }

        function roIDCallback() {

            var s = '<BR/>';
            var newReq = (formContext.ui.getFormType() === CRM_FORM_TYPE_CREATE ? "New " : "");
            var veteran = (formContext.getAttribute('udo_relatedveteranid').getValue() ? formContext.getAttribute('udo_relatedveteranid').getValue()[0].name : 'Unknown');

            var issue = formContext.getAttribute('udo_issue').getText();
            var subject = newReq + "Request for" /*+ "  " + veteran*/
                + ": " + (!dispSubType || dispSubType.length === 0 ? issue : dispSubType);

            if (sendToVet) {
                if (action === Globals.srAction.ActionEmailFormsText) {
                    subject = 'Requested VA Forms';
                }
                else {
                    subject = 'Requested Information from VA';
                }
            }

            var body = '';
            var nonEmergency = (action === Globals.srAction.ActionNonEmergencyEmailText);

            var PCRLookupValue = formContext.getAttribute('udo_pcrofrecordid').getValue();
            var PCRName = PCRLookupValue === null ? '' : PCRLookupValue[0].name;
            var PCRId = PCRLookupValue === null ? '' : PCRLookupValue[0].id;

            var StationNumber = null;

            if (PCRId !== '') {
                //todo: update to webapi
                //CrmRestKit2011.Retrieve('SystemUser', PCRId, ['va_StationNumber'], false)
                webApi.RetrieveRecord(PCRId, "systemuser", ["va_stationnumber"])
                    .then(function (data) {
                        StationNumber = data.va_stationnumber;
                        stationCallback();
                    })
                    .catch(function (err) { HandleRestError(err, 'Failed to retrieve station data'); });
            }
            else {
                stationCallback();
            }

            function stationCallback() {
                var PCRNameArray = PCRName.split(',');
                var PCRNameFullName = '';
                if (PCRNameArray.length > 0) {
                    PCRNameFullName = (PCRNameArray[1] === undefined ? '' : PCRNameArray[1]) + ' ' + (PCRNameArray[0] === undefined ? '' : PCRNameArray[0]);
                }
                var Description = formContext.getAttribute('udo_description').getValue();
                if (Description === null) {  //ensure no 'null' in email body
                    Description = '';
                }

                // For non-emergency email to Vet, using special body
                if ((!nonEmergency || (nonEmergency && sendToVet === false)) && action !== Globals.srAction.ActionEmailFormsText) {
                    if (action === Globals.srAction.Action0820Text || action === Globals.srAction.Action0820aText || action === Globals.srAction.Action0820fText) {
                        body = PCRNameFullName.trim() + s + 'Station Number ' + StationNumber;
                    }
                    else {
                        if (nonEmergency && sendToVet === false) {
                            var cfirstname = formContext.getAttribute("udo_firstname").getValue();
                            cfirstname = cfirstname === null ? "" : cfirstname;
                            var clastname = formContext.getAttribute("udo_lastname").getValue();
                            clastname = clastname === null ? "" : clastname;
                            var mfirstname = formContext.getAttribute("udo_srfirstname").getValue();
                            mfirstname = mfirstname === null ? "" : mfirstname;
                            var mlastname = formContext.getAttribute("udo_srlastname").getValue();
                            mlastname = mlastname === null ? "" : mlastname;
                            var filenumber = formContext.getAttribute("udo_filenumber").getValue();
                            filenumber = filenumber === null ? "" : filenumber;
                            var ssn = formContext.getAttribute("udo_ssn").getValue();
                            ssn = ssn === null ? "" : ssn;
                            var phone = formContext.getAttribute("udo_dayphone").getValue();
                            phone = phone === null ? "N/A" : phone;

                            body = "A " + newReq + "service request has been submitted for the Veteran " + veteran + s + s + (to && to.length > 0 ? "RO Email Address: " + to + s : "") + "Service Request Number: " + formContext.getAttribute("udo_reqnumber").getValue() + s + "Service Request Action: " + action + s + "Service Request Type: " + issue + s;
                            body += "+-: " + s;
                            body += "Callers Name: " + cfirstname + " " + clastname + s;
                            body += "Veteran/Beneficiary's Name (if different): " + mfirstname + " " + mlastname + s;
                            body += "File or Social Security Number: " + (filenumber === "" ? ssn : filenumber) + s;
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
                    body = attachmentList + "Please do not respond to this message.  Unfortunately, replies to this message will be routed to a mailbox which is unable to receive replies.<br/><br/>" + "Please be aware that you can change your direct deposit, check the status of your claim, obtain a copy of your DD 214, and access additional VA benefit information via va.gov at " + "<a href='www.va.gov'>www.va.gov</a>" + ". Users are able to log into VA.gov via their existing MyHealtheVet, DS Logon, or ID.me credentials. To register for an account, follow the online prompts on VA.gov.<br/><br/>" + "We are happy to help you with any questions or concerns you may have.  If you have questions or need additional assistance, please see our contact information listed below.<br/><br/>" + "How to Contact VA:<br/><br/>" + "Online: <br/>" + "<a href='http://www.va.gov'>www.va.gov</a><br/><br/>" + "By Phone:<br/>" + "1-800-827-1000<br/>711 (Federal Relay Service for Hearing Impaired)<br/><br/>";

                    if (attachmentListx.length > 0) {
                        body += attachmentListx;
                    }
                    else if (sojAddress !== '') {
                        body += "By Mail: <br/>";
                        body += sojAddress;
                    }
                }

                var toLine = '', ccLine = '';

                if (sendToVet && action === Globals.srAction.ActionEmailFormsText) {
                    toLine = formContext.getAttribute('udo_emailofveteran').getValue();
                } else if (sendToVet && sendToRO) {
                    toLine = to;
                    ccLine = formContext.getAttribute('udo_emailofveteran').getValue();
                } else if (sendToVet && !sendToRO) {
                    toLine = formContext.getAttribute('udo_emailofveteran').getValue();
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
                Va.Udo.Crm.MapdNote.Initialize(exCon);
                ConfirmMarkAsSent()
                    .then(function (data) {
                        Va.Udo.Crm.MapdNote.PromptQuestion()

                            .then(function (createNote) {
                                if (createNote) {
                                    Va.Udo.Crm.MapdNote.CreateNote().then(
                                        function () {
                                            Va.Udo.Crm.MapdNote.SetFormNotification("", "A note has been created for this " + Va.Udo.Crm.MapdNote.TypeOfNote, "INFORMATION");
                                            openOutlookEmail(emailOptions, filedownloaded);
                                        });
                                } else {
                                    Va.Udo.Crm.MapdNote.SetFormNotification("", "User decided not to create a note for this " + Va.Udo.Crm.MapdNote.TypeOfNote, "WARNING");
                                    openOutlookEmail(emailOptions, filedownloaded);
                                }
                            });

                    });

            }
        }
    }
}

//cchano - updated to reflect item #646985
//Yes&No fields associated to 0820a should be nulled out and required when 0820a is selected
function RequireYesNo(fnod) {

    var submitMode = '';
    var requiredLevel = '';
    if (fnod === true) {
        submitMode = 'dirty';
        requiredLevel = 'required';
    }
    else {
        submitMode = 'never';
        requiredLevel = 'none';
    }
    //if (formContext.getAttribute("udo_benefitsstopped").getRequiredLevel() !== requiredLevel) { //only toggle fields if not already set

    //formContext.getAttribute("udo_fnodreportingfor").setValue(null);
    //formContext.getAttribute("udo_fnodreportingfor").setSubmitMode(submitMode);
    //formContext.getAttribute("udo_fnodreportingfor").setRequiredLevel(requiredLevel);
    // Death of Veteran - FNOD Action
    
    //formContext.getAttribute("udo_benefitsstopped").setValue(null);
    //formContext.getAttribute("udo_benefitsstopped").setRequiredLevel(requiredLevel);
    formContext.getAttribute("udo_benefitsstopped").setSubmitMode(submitMode);

    //formContext.getAttribute("udo_deathrelatedinformationchecklists").setValue(null);
    formContext.getAttribute("udo_deathrelatedinformationchecklists").setSubmitMode(submitMode);
    //formContext.getAttribute("udo_deathrelatedinformationchecklists").setRequiredLevel(requiredLevel);

    //formContext.getAttribute("udo_lookedupvetrecord").setValue(null);
    formContext.getAttribute("udo_lookedupvetrecord").setSubmitMode(submitMode);
    //formContext.getAttribute("udo_lookedupvetrecord").setRequiredLevel(requiredLevel);

    //formContext.getAttribute("udo_processedfnodinshare").setValue(null);
    formContext.getAttribute("udo_processedfnodinshare").setSubmitMode(submitMode);
    //formContext.getAttribute("udo_processedfnodinshare").setRequiredLevel(requiredLevel);

    // 0820a - FNOD ACTIONS - I CERTIFY I SENT THE FOLLOWING
    //formContext.getAttribute("udo_pmc").setValue(null);
    formContext.getAttribute("udo_pmc").setSubmitMode(submitMode);
    formContext.getAttribute("udo_pmc").setRequiredLevel(requiredLevel);

    //formContext.getAttribute("udo_nokletter").setValue(null);
    formContext.getAttribute("udo_nokletter").setSubmitMode(submitMode);
    formContext.getAttribute("udo_nokletter").setRequiredLevel(requiredLevel);

    //formContext.getAttribute("udo_21530").setValue(null);
    formContext.getAttribute("udo_21530").setSubmitMode(submitMode);
    formContext.getAttribute("udo_21530").setRequiredLevel(requiredLevel);

    //formContext.getAttribute("udo_21534").setValue(null);
    formContext.getAttribute("udo_21534").setSubmitMode(submitMode);
    formContext.getAttribute("udo_21534").setRequiredLevel(requiredLevel);

    //formContext.getAttribute("udo_401330").setValue(null);
    formContext.getAttribute("udo_401330").setSubmitMode(submitMode);
    formContext.getAttribute("udo_401330").setRequiredLevel(requiredLevel);

    //formContext.getAttribute("udo_other").setValue(null);
    formContext.getAttribute("udo_other").setSubmitMode(submitMode);
    formContext.getAttribute("udo_other").setRequiredLevel(requiredLevel);

    // 0820a - DEATH OF NON-VET BENEFICIARY FOR STOP PAYMENT ACTION. I CERTIFY THAT I...
    //formContext.getAttribute("udo_benefitsstopfirstofmonth").setValue(null);
    formContext.getAttribute("udo_benefitsstopfirstofmonth").setSubmitMode(submitMode);
    //formContext.getAttribute("udo_benefitsstopfirstofmonth").setRequiredLevel(requiredLevel);

    //formContext.getAttribute("udo_willroutereportofdeath").setValue(null);
    formContext.getAttribute("udo_willroutereportofdeath").setSubmitMode(submitMode);
    //formContext.getAttribute("udo_willroutereportofdeath").setRequiredLevel(requiredLevel);

    //formContext.getAttribute("udo_possibleburialinnationalcemetery").setValue(null);
    formContext.getAttribute("udo_possibleburialinnationalcemetery").setSubmitMode(submitMode);
    //formContext.getAttribute("udo_possibleburialinnationalcemetery").setRequiredLevel(requiredLevel);

    //formContext.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a").setVisible(fnod);
    //formContext.ui.tabs.get("Tab_SRinfo").sections.get("Section_0820a2").setVisible(fnod);


    // }
}

function refresh0820dSubGrid(context) {
    var grid = context.getControl("form0820payments");
    grid.refresh();
}

function open0820dQuickCreate(context, ServiceRequestId) {
    var grid = context.getControl("form0820payments");
    grid.setFocus();
    var entityFormOptions = {};
    entityFormOptions["entityName"] = "udo_form0820payments";
    entityFormOptions["useQuickCreateForm"] = true;
    
    // Set default values for the Contact form
    var formParameters = {};
    
    // Set lookup field
    formParameters["udo_servicerequestid"] = ServiceRequestId;
    // End of set lookup field
    
    // Open the form.
    Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
        function (success) {
            grid.setFocus();
            console.log(success);
        },
        function (error) {
            grid.setFocus();
            console.log(error);
        });
}

// change visibility of sections on the form based on type
function ServiceTypeChange() {
    if (formContext.getAttribute('udo_action').getValue() !== null) {
        //var ga = formContext.getAttribute;
        var selectedAction = formContext.getAttribute('udo_action').getSelectedOption().text;
        var reportName = null;
        if ((window.IsUSD === true) || (parent.window.IsUSD === true)) {
            window.open("http://uii/Global Manager/CopyToContext?ServiceRequestAction=" + selectedAction);
            //window.open("http://event/?EventName=ServiceRequestActionChange&action=" + selectedAction);
        }
        var selectedActionValue = formContext.getAttribute('udo_action').getValue();
        if (!selectedAction) return;

        //Set default values for tabs/attributes
        var tabs =
        {
            "tabGen": { tab: "{040d30d2-de4c-438a-9c84-7576f6b74327}", section: null, isVisible: true },
            "secVet": { tab: "{040d30d2-de4c-438a-9c84-7576f6b74327}", section: "Tab_General_Vet", isVisible: false },
            "sec4": { tab: "{040d30d2-de4c-438a-9c84-7576f6b74327}", section: "Tab_General_section_4", isVisible: false },
            //"tabContact": { tab: "tab_contact", section: null, isVisible: false },
            "tabContact": { tab: "{040d30d2-de4c-438a-9c84-7576f6b74327}", section: "tab_6_section_1", isVisible: false },
            "tabContact2": { tab: "{040d30d2-de4c-438a-9c84-7576f6b74327}", section: "ecc_information", isVisible: false },
            "tabContact3": { tab: "{040d30d2-de4c-438a-9c84-7576f6b74327}", section: "mailing_address_tab_section_4", isVisible: false },
            "tabContact4": { tab: "{040d30d2-de4c-438a-9c84-7576f6b74327}", section: "tab_5_section_1", isVisible: false },
            //"tabSr": { tab: "Tab_SRinfo", section: null, isVisible: false },
            "secSr820": { tab: "{040d30d2-de4c-438a-9c84-7576f6b74327}", section: "Section_0820", isVisible: false },
            
            //"tab820a": { tab: "tab_section_0820a", section: null, isVisible: false },
            "sec820a": { tab: "{040d30d2-de4c-438a-9c84-7576f6b74327}", section: "Section_0820a", isVisible: false },
            "sec820a2": { tab: "{040d30d2-de4c-438a-9c84-7576f6b74327}", section: "Section_0820a2", isVisible: false },
            "sec820aPmnt": { tab: "{040d30d2-de4c-438a-9c84-7576f6b74327}", section: "0820a_StopPayment", isVisible: false },
            "sec820dPmnt": { tab: "{040d30d2-de4c-438a-9c84-7576f6b74327}", section: "Section_0820dPayment", isVisible: false },
            "sec820dVai": { tab: "{040d30d2-de4c-438a-9c84-7576f6b74327}", section: "Section_VAI", isVisible: false },
            
            //"tabDependent": { tab: "tab_dependent_information", section: null, isVisible: false },
            "secDependent": { tab: "{040d30d2-de4c-438a-9c84-7576f6b74327}", section: "section_dependent", isVisible: false },
            "secDependenta": { tab: "{040d30d2-de4c-438a-9c84-7576f6b74327}", section: "section_dependent0820a", isVisible: false },
            
            "tabEnclosures": { tab: "{040d30d2-de4c-438a-9c84-7576f6b74327}", section: "tab_ai_section_8", isVisible: false },
            "tabEnclosures2": { tab: "{040d30d2-de4c-438a-9c84-7576f6b74327}", section: "tab_ai_section_docsandforms", isVisible: false }
            
        };
        toggleTabs(tabs);

        var attributes =
        {
            "deathDate": { name: "udo_dateofdeath", required: "none", submitMode: null, isVisible: false },
            "contactType": { name: "udo_typeofcontact", required: "none", submitMode: null, isVisible: true },
            "dob": { name: "udo_dateofbirthofdeceased", required: "none", submitMode: null, isVisible: false },
            "benefitsStpd": { name: "udo_benefitsstopped", required: "none", submitMode: null, isVisible: false },
            "deathInfo": { name: "udo_deathrelatedinformationchecklists", required: "none", submitMode: null, isVisible: false },
            "vetRec": { name: "udo_lookedupvetrecord", required: "none", submitMode: null, isVisible: false },
            "fnodShare": { name: "udo_processedfnodinshare", required: "none", submitMode: null, isVisible: false },
            "fnodExplanation": { name: "udo_processedfnodinshareexplanation", required: "none", submitMode: null, isVisible: false },
            "pmc": { name: "udo_pmc", required: "none", submitMode: null, isVisible: false },
            "_401330": { name: "udo_401330", required: "none", submitMode: null, isVisible: false },
            "nok": { name: "udo_nokletter", required: "none", submitMode: null, isVisible: false },
            "other": { name: "udo_other", required: "none", submitMode: null, isVisible: false },
            "_21530": { name: "udo_21530", required: "none", submitMode: null, isVisible: false },
            "_21534": { name: "udo_21534", required: "none", submitMode: null, isVisible: false },
            "benefitsStop": { name: "udo_benefitsstopfirstofmonth", required: "none", submitMode: null, isVisible: false },
            "routeDeath": { name: "udo_willroutereportofdeath", required: "none", submitMode: null, isVisible: false },
            "natlCemetary": { name: "udo_possibleburialinnationalcemetery", required: "none", submitMode: null, isVisible: false },
            "reportingName": { name: "udo_nameofreportingindividual", required: "none", submitMode: null, isVisible: false },
            "deathLocation": { name: "udo_locationofdeath", required: "none", submitMode: null, isVisible: false },
            "script": { name: "udo_readscript", required: "none", submitMode: null, isVisible: false },
            "enroute": { name: "udo_enroutetova", required: "none", submitMode: null, isVisible: false },
            "nonUSDeath": { name: "udo_nonuslocationofdeath", required: "none", submitMode: null, isVisible: false }
        };
        //Show/hide fields and tabs based on action type.
        var setFields = function () {

            switch (selectedActionValue) {

                case Globals.srAction.ActionEmailFormsValue:
                    tabs.secVet.isVisible = true;
                    tabs.tabEnclosures.isVisible = true;
                    tabs.tabEnclosures2.isVisible = true;

                    toggleTabs(tabs);
                    toggleAttributes(attributes);
                    formContext.getAttribute("udo_sendemailtoveteran").setValue(true);
                    RequireYesNo(false);
                    break;

                case Globals.srAction.Action0820Value:
                    //tabs.tabSr.isVisible = true;
                    tabs.secSr820.isVisible = true;
                    tabs.secVet.isVisible = true;
                    tabs.sec4.isVisible = true;
                    tabs.tabContact.isVisible = true;
                    //tabs.tabContact2.isVisible = true;
                    tabs.tabContact3.isVisible = true;
                    tabs.tabContact4.isVisible = true;

                    attributes.reportingName.isVisible = true;
                    attributes.script.isVisible = true;

                    formContext.getAttribute('udo_sendemailtoveteran').setValue(false);

                    toggleTabs(tabs);
                    toggleAttributes(attributes);
                    reportName = "27-0820 - Report of General Information";
                    RequireYesNo(false);
                    break;

                case Globals.srAction.Action0820fValue:
                    //tabs.tabSr.isVisible = true;
                    tabs.secSr820.isVisible = true;
                    tabs.secVet.isVisible = true;
                    tabs.sec4.isVisible = true;
                    tabs.tabContact.isVisible = true;
                    //tabs.tabContact2.isVisible = true;
                    tabs.tabContact3.isVisible = true;
                    tabs.tabContact4.isVisible = true;
                    //tabs.tabDependent.isVisible = true;
                    tabs.secDependent.isVisible = true;

                    attributes.reportingName.isVisible = true;
                    attributes.script.isVisible = true;

                    formContext.getAttribute("udo_srssn").addOnChange(function () { popSingleDepField("udo_srssn", "udo_depssn"); });
                    formContext.getAttribute("udo_srgender").addOnChange(function () { popSingleDepField("udo_srgender", "udo_depgender"); });
                    formContext.getAttribute("udo_address1").addOnChange(function () { popSingleDepField("udo_address1", "udo_depaddress"); });
                    formContext.getAttribute("udo_zipcode").addOnChange(function () { popSingleDepField("udo_zipcode", "udo_depzipcode"); });
                    formContext.getAttribute("udo_srfirstname").addOnChange(function () { popSingleDepField("udo_srfirstname", "udo_depfirstname"); });
                    formContext.getAttribute("udo_city").addOnChange(function () { popSingleDepField("udo_city", "udo_depcity"); });
                    formContext.getAttribute("udo_srdobtext").addOnChange(function () { popSingleDepField("udo_srdobtext", "udo_depdob"); });
                    formContext.getAttribute("udo_srlastname").addOnChange(function () { popSingleDepField("udo_srlastname", "udo_deplastname"); });
                    formContext.getAttribute("udo_email").addOnChange(function () { popSingleDepField("udo_email", "udo_depemail"); });
                    formContext.getAttribute("udo_state").addOnChange(function () { popSingleDepField("udo_state", "udo_depstate"); });

                    formContext.getAttribute('udo_sendemailtoveteran').setValue(false);
                    reportName = "27-0820f - Report of Month of Death";

                    toggleTabs(tabs);
                    toggleAttributes(attributes);
                    RequireYesNo(false);
                    break;

                case Globals.srAction.Action0820dValue:
                    // Confirm this is required
                    //tabs.tabSr.isVisible = true;
                    tabs.secSr820.isVisible = true;

                    tabs.sec820dPmnt.isVisible = true;
                    tabs.secVet.isVisible = true;
                    tabs.sec4.isVisible = true;
                    tabs.tabContact.isVisible = true;
                    //tabs.tabContact2.isVisible = true;
                    tabs.tabContact3.isVisible = true;
                    tabs.tabContact4.isVisible = true;

                    //attributes.reportingName.isVisible = true;
                    attributes.script.isVisible = true;

                    formContext.getAttribute('udo_sendemailtoveteran').setValue(false);
                    reportName = "27-0820d - Report of Non-Receipt of Payment";

                    toggleTabs(tabs);
                    toggleAttributes(attributes);
                    RequireYesNo(false);
                    break;

                case Globals.srAction.Action0820aValue:
                    formContext.getAttribute('udo_sendemailtoveteran').setValue(false);

                    //tabs.tabSr.isVisible = true;
                    tabs.secSr820.isVisible = true;
                    //tabs.tabDependent.isVisible = true;
                    tabs.secDependenta.isVisible = true;
                    tabs.secVet.isVisible = true;
                    tabs.sec4.isVisible = true;
                    tabs.tabContact.isVisible = true;
                    //tabs.tabContact2.isVisible = true;
                    tabs.tabContact3.isVisible = true;
                    tabs.tabContact4.isVisible = true;
                    //tabs.tab820a.isVisible = true;


                    attributes.deathDate.required = "required";
                    attributes.dob.isVisible = true;
                    attributes.dob.required = "required";
                    attributes.fnodExplanation.isVisible = true;
                    attributes.pmc.required = "required";
                    attributes.pmc.isVisible = true;
                    attributes._401330.required = "required";
                    attributes._401330.isVisible = true;
                    attributes.nok.required = "required";
                    attributes.nok.isVisible = true;
                    attributes.other.required = "required";
                    attributes.other.isVisible = true;
                    attributes._21530.required = "required";
                    attributes._21530.isVisible = true;
                    attributes._21534.required = "required";
                    attributes._21534.isVisible = true;


                    attributes.contactType.required = "required";
                    attributes.contactType.submitMode = "always";
                    attributes.reportingName.isVisible = true;
                    attributes.script.isVisible = true;
                    attributes.enroute.isVisible = true;
                    attributes.deathDate.isVisible = true;
                    attributes.nonUSDeath.isVisible = true;
                    attributes.deathLocation.isVisible = true;


                    if (type !== null) {
                        //tab_section_0820a
                        if (type === 752280000) {  //Value Self, is a Veteran
                            tabs.sec820a.isVisible = true;
                            tabs.sec820a2.isVisible = true;

                            attributes.benefitsStpd.required = "required";
                            attributes.benefitsStpd.isVisible = true;
                            attributes.deathInfo.required = "required";
                            attributes.deathInfo.isVisible = true;
                            attributes.vetRec.required = "required";
                            attributes.vetRec.isVisible = true;
                            attributes.fnodShare.required = "required";
                            attributes.fnodShare.isVisible = true;

                        }
                        else { //is not a Veteran
                            tabs.sec820a2.isVisible = true;
                            tabs.sec820aPmnt.isVisible = true;

                            attributes.benefitsStop.required = "required";
                            attributes.benefitsStop.isVisible = true;
                            attributes.routeDeath.required = "required";
                            attributes.routeDeath.isVisible = true;
                            attributes.natlCemetary.required = "required";
                            attributes.natlCemetary.isVisible = true;


                        }
                    }
                    reportName = "27-0820a - Report of First Notice of Death";

                    toggleTabs(tabs);
                    toggleAttributes(attributes);
                    ProcessedInShareChange();
                    FnodOtherChange();
                    RequireYesNo(true);
                    break;


                case Globals.srAction.ActionNonEmergencyEmailValue:
                    //tabs.tabSr.isVisible = true;
                    tabs.tabGen.isVisible = true;
                    tabs.secVet.isVisible = true;
                    tabs.tabContact.isVisible = true;
                    //tabs.tabContact2.isVisible = true;
                    tabs.tabContact3.isVisible = true;
                    tabs.tabContact4.isVisible = true;

                    formContext.getAttribute('udo_sendemailtoveteran').setValue(false);

                    toggleTabs(tabs);
                    toggleAttributes(attributes);
                    RequireYesNo(false);
                    break;

                default:
                    tabs.secVet.isVisible = true;
                    tabs.sec4.isVisible = true;
                    tabs.tabContact.isVisible = true;
                    //tabs.tabContact2.isVisible = true;
                    tabs.tabContact3.isVisible = true;
                    tabs.tabContact4.isVisible = true;

                    toggleTabs(tabs);
                    toggleAttributes(attributes);
                    RequireYesNo(false);
                    break;
            }

            if (reportName !== null) {
                reportName += " - UDO";
                var cols = ["udo_letterid"];
                var filter = "$filter=udo_name eq '" + encodeURIComponent(reportName) + "'";
                webApi.RetrieveMultiple("udo_letter", cols, filter)
                    .then(function (data) {
                        var letterId = "{" + data[0].udo_letterid + "}";
                        formContext.getAttribute("udo_letter").setValue([{ entityType: "udo_letter", id: letterId, name: reportName }]);
                    })
                    .catch(function (err) {
                        UDO.Shared.openAlertDialog("Error setting letter type");
                    });
            }

            formContext.getControl("udo_dateoffirstnotice").setVisible(false);

            //var afControls = [formContext.getControl("udo_locationofdeath"),
            //formContext.getControl("udo_enroutetova"),
            //formContext.getControl("udo_dateofdeath"),
            //formContext.getControl("udo_nonuslocationofdeath")];
            //var visibility = (selectedActionValue === Globals.srAction.Action0820aValue || selectedAction === Globals.srAction.Action0820fValue);

            var requestType = formContext.getAttribute('udo_requesttype').getValue();
            var requestSubType = formContext.getAttribute('udo_requestsubtype').getValue();

            if (selectedActionValue === Globals.srAction.Action0820aValue && requestType === "FNOD" && requestSubType === "Death of a Non-Veteran Beneficiary") {

                //visibility = false;
                //formContext.getControl("udo_placeofdeath").setValue("");
                //formContext.getControl("udo_enroutetova").setValue("");
                formContext.getControl("udo_dateofdeath").setValue("");
            }

            //for (var c in afControls) {
            //    afControls[c].setVisible(visibility);
            // }
        };
        var type = null;
        if (formContext.getAttribute('udo_personid').getValue() !== null && selectedActionValue === Globals.srAction.Action0820aValue) {
            var prsn = formContext.getAttribute('udo_personid').getValue();
            var prsnId = prsn[0].id;
            var columns = ['udo_type'];
            webApi.RetrieveRecord(prsnId, "udo_person", columns)
                .then(function success(result) {
                    if (result) {
                        type = result.udo_type;
                    }
                    setFields();
                })
                .catch(function (error) {

                });

        }
        else {
            setFields();
        }


    }
}

function toggleTabs(tabs) {
    $.each(tabs, function (tabName, tab) {
        if (tab.section !== null) {
            formContext.ui.tabs.get(tab.tab).sections.get(tab.section).setVisible(tab.isVisible);
        }
        else {
            formContext.ui.tabs.get(tab.tab).setVisible(tab.isVisible);
        }
    });
}

function toggleAttributes(attributes) {
    $.each(attributes, function (attrName, attr) {
        if (attr.isVisible !== null) {
            formContext.getControl(attr.name).setVisible(attr.isVisible);
        }
        if (attr.submitMode !== null) {
            formContext.getAttribute(attr.name).setSubmitMode(attr.submitMode);
        }
        formContext.getAttribute(attr.name).setRequiredLevel(attr.required);
    });
}

function QuickWriteChange() {
    var id = GetLookupId('udo_quickwriteid');
    if (!id) return;
    var descAttribute = formContext.getAttribute('udo_description');
    //todo: convert to web api
    var columns = ['va_quickwritetext'];

    webApi.RetrieveRecord(id, "va_quickwrite", columns)
        .then(function (data) {
            if (data) {
                var qw = data.va_quickwritetext;
                var val = descAttribute.getValue();

                // process substition tokens
                // each one looks like <!udo_ssn!> or <!va_ssn!>
                qw = ReplaceFieldTokens(qw);
                if (!qw) qw = '';

                descAttribute.setValue((val ? val + ' \n' + qw : qw));
                formContext.getAttribute('udo_quickwriteid').setValue(null);
                formContext.getControl('udo_description').setFocus();

                getManager();
            }
        })
        .catch(function (err) { HandleRestError(err, 'Failed to retrieve text.'); });
}

function ProcessedInShareChange() {
    var showExplanation = formContext.getAttribute('udo_processedfnodinshare').getValue() === false;
    formContext.getControl('udo_processedfnodinshareexplanation').setVisible(showExplanation);
    if (!showExplanation) {
        formContext.getAttribute('udo_processedfnodinshareexplanation').setValue(null);
    }
}

function FnodOtherChange() {
    formContext.getControl('udo_otherspecification').setVisible((formContext.getAttribute("udo_other").getValue() === true));
}
//TODO: will need to convert away from using window.open.  Probably will not work.
function ShowScript() {
    var scriptSource = '../../WebResources/udo_/scripts/jurisdictions_routing.html';
    var win;
    if (!_scriptWindowHandle || _scriptWindowHandle.closed) {
        win = _scriptWindowHandle = window.open(scriptSource, "CallScript", "width=600,height=500,scrollbars=1,resizable=1");
    } else {
        win = _scriptWindowHandle.open(scriptSource, "CallScript", "width=600,height=500,scrollbars=1,resizable=1");
    }
    win.focus();
}

function getVBMSRole() {

    var currentUserId = globalContext.userSettings.userId;
    var vbmsUploadrole;

    webApi.RetrieveRecord(currentUserId, "systemuser", ["udo_vbmsuploadrole"])
        .then(function (data) {
            vbmsUploadrole = data.udo_vbmsuploadrole;

            if (vbmsUploadrole !== null)
                formContext.getAttribute("udo_vbmsuploadrole").setValue(vbmsUploadrole.Value);

        })
        .catch(function (err) { HandleRestError(err, 'Failed to retrieve VBMS Upload Role data'); });

}

function getManager() {
    var pcrOfRecordId = formContext.getAttribute('udo_pcrofrecordid').getValue();
    //todo: convert to web api
    var columns = ["_parentsystemuserid_value"];
    var filter = "$filter=systemuserid eq " + pcrOfRecordId[0].id.replace("{", "").replace("}", "") + "";

    webApi.RetrieveRecord(pcrOfRecordId[0].id, "systemuser", columns)
        .then(function (data) {
            if (data) {
                var parentSystemUserId = data["_parentsystemuserid_value"];

                if (parentSystemUserId === null) {
                    UDO.Shared.openAlertDialog('It has been detected that you do not have a manager assigned. It is recommended a manager is assigned to you.');

                    var description = formContext.getAttribute('udo_description');
                    description.setValue(description.getValue().replace('(Signature of NCCM)', ''));

                    return;
                }

                columns = ['firstname', 'lastname'];
                filter = "$filter=systemuserid eq " + parentSystemUserId.replace("{", "").replace("}", "") + "";

                webApi.RetrieveMultiple("systemuser", columns, filter)
                    .then(function (data) {
                        if (data && data.length === 1) {
                            var manager = data[0].firstname + ' ' + data[0].lastname;

                            var description = formContext.getAttribute('udo_description');
                            description.setValue(description.getValue().replace('(Signature of NCCM)', 'Sincerely,<br/>' + manager));
                        }
                    })
                    .catch(function (err) { HandleRestError(err, 'Failed to retrieve manager data'); });
            }
        })
        .catch(function (err) { HandleRestError(err, 'Failed to retrieve manager id'); });
}

function copyMailingAddress() {
    //var ga = ga = formContext.getAttribute;

    formContext.getAttribute("udo_address1").setValue(formContext.getAttribute("udo_mailing_address1").getValue());
    formContext.getAttribute("udo_address2").setValue(formContext.getAttribute("udo_mailing_address2").getValue());
    formContext.getAttribute("udo_address3").setValue(formContext.getAttribute("udo_mailing_address3").getValue());
    formContext.getAttribute("udo_city").setValue(formContext.getAttribute("udo_mailing_city").getValue());
    formContext.getAttribute("udo_state").setValue(formContext.getAttribute("udo_mailing_state").getValue());
    formContext.getAttribute("udo_zipcode").setValue(formContext.getAttribute("udo_mailing_zip").getValue());
    formContext.getAttribute("udo_country").setValue(formContext.getAttribute("udo_mailingcountry").getValue());
    formContext.getAttribute("udo_email").setValue(formContext.getAttribute("udo_sremail").getValue());
    formContext.getAttribute("udo_phone").setValue(formContext.getAttribute("udo_dayphone").getValue());

    popAllDepFields();
}

function popAllDepFields() {
    if (formContext.getAttribute("udo_relationtoveteran").getText() !== "Veteran(Self)") {
        formContext.getAttribute("udo_depssn").setValue(formContext.getAttribute("udo_srssn").getValue());
        formContext.getAttribute("udo_depgender").setValue(formContext.getAttribute("udo_srgender").getValue());
        formContext.getAttribute("udo_depaddress").setValue(formContext.getAttribute("udo_address1").getValue());
        formContext.getAttribute("udo_depzipcode").setValue(formContext.getAttribute("udo_zipcode").getValue());
        formContext.getAttribute("udo_depfirstname").setValue(formContext.getAttribute("udo_srfirstname").getValue());
        formContext.getAttribute("udo_deprelation").setValue(formContext.getAttribute("udo_relationtoveteran").getText());
        formContext.getAttribute("udo_depcity").setValue(formContext.getAttribute("udo_city").getValue());
        formContext.getAttribute("udo_depdob").setValue(formContext.getAttribute("udo_srdobtext").getValue());
        formContext.getAttribute("udo_deplastname").setValue(formContext.getAttribute("udo_srlastname").getValue());
        formContext.getAttribute("udo_depemail").setValue(formContext.getAttribute("udo_email").getValue());
        formContext.getAttribute("udo_depstate").setValue(formContext.getAttribute("udo_state").getValue());
    }
}

function popSingleDepField(fieldChanged, depField) {
    var ga = ga = formContext.getAttribute;
    if (formContext.getAttribute("udo_relationtoveteran").getText() !== "Veteran(Self)") {
        if (fieldChanged === "udo_relationtoveteran") {
            formContext.getAttribute(depField).setValue(formContext.getAttribute(fieldChanged).getText());
        }
        else {
            formContext.getAttribute(depField).setValue(formContext.getAttribute(fieldChanged).getValue());
        }
    }
}

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.split(search).join(replacement);
}

function GetLookupId(lookupAttributeName) {
    var id = null;
    if (formContext.getAttribute(lookupAttributeName).getValue() &&
        formContext.getAttribute(lookupAttributeName).getValue().length > 0 &&
        formContext.getAttribute(lookupAttributeName).getValue()[0]) {
        id = formContext.getAttribute(lookupAttributeName).getValue()[0].id;
    }

    return id;
}
//TODO: convert to return promise
function HandleRestError(err, msg) {
    if (err.message) {
        Xrm.Navigation.openErrorDialog({ message: err.message });
    }
    else {
        Xrm.Navigation.openErrorDialog({ message: msg });
    }
}

// process substition tokens
// each one looks like <!va_ssn!> or <!udo_ssn!>
// will take input abe<!va_ssn!>asdf and replace with abe555667777asdf
function ReplaceFieldTokens(qw) {
    if (!qw || qw.length === 0) return qw;

    var op = '<!', po = '!>', pos = qw.indexOf(op);
    while (pos !== -1) {
        var pos2 = qw.indexOf(po);
        if (pos2 === -1) break; // cannot find closing tag
        var token = qw.substring(pos + po.length, pos2); // token without tags
        var attr = formContext.getAttribute(token.replace("va_", "udo_"));
        var attrVal = '';
        if (attr && attr.getValue()) {
            switch (attr.getAttributeType()) {
                case 'datetime':
                    attrVal = attr.getValue().format("MM/dd/yyyy");
                    break;
                case 'lookup':
                    attrVal = attr.getValue()[0].name;
                    break;
                case 'optionset':
                    attrVal = attr.getText();
                    break;
                default:
                    attrVal = attr.getValue().toString();
            }
        }
        qw = qw.replace(new RegExp(op + token + po, 'g'), attrVal);

        pos = qw.indexOf(op);
    }
    return qw;
}

function getReportID(reportName) {
    var len = reports.list.length;

    for (var i = 0; i < len; i++) {
        if (reports.list[i].name === reportName) {
            return reports.list[i].reportId;
        }
    }

    return '';
}

var reports = {
    list: [],
    getReports: function () {
        var len, me = this;

        var cols = ["reportid", "name"];
        webApi.RetrieveMultiple("report", cols, null)
            .then(function (data) {
                if (data && data.length > 0) {
                    len = data.length;

                    for (var i = 0; i < len; i++) {
                        me.list.push({ name: data[i].name, reportId: data[i].reportid });
                    }
                }
            }).catch(function (error) {
                throw new Error(error.message);
                // console.log("fail")
            });
    }
};

function b64blob(fileName, contentType, b64Data) {
    // Convert to Char Array
    var byteChars = atob(b64Data);

    // Convert to Number Array
    var byteNums = new Array(byteChars.length);
    for (var i = 0; i < byteChars.length; i++) {
        byteNums[i] = byteChars.charCodeAt(i);
    }

    // Convert to byteArray
    var byteArray = new Uint8Array(byteNums);
    var blob = new Blob([byteArray], { type: contentType, name: fileName || "download" });
    return blob;
}


function openOutlookEmail(opts, filedownloaded) {
    var outlookApp, mailItem, options,
        defaults = {
            subject: 'VAI',
            to: null,
            cc: null,
            isHtml: true,
            body: '',
            files: [],
            attachements: []
        };

    options = $.extend(defaults, opts);

    try {        

        if (options.to === "" || options.to === null) {
            throw new Error('The email requires a recipent!');
        }

        var url = "";
        if ((filedownloaded !== "") && (filedownloaded !== null)) {
            url = "http://event/?EventName=OpenOutlookEmail&EmailTo=" + options.to + "&EmailSubject=" + options.subject + "&EmailBody=" + options.body + "&EmailAttachmentPath=" +  filedownloaded;
        } else {
            url = "http://event/?EventName=OpenOutlookEmail&EmailTo=" + options.to + "&EmailSubject=" + options.subject + "&EmailBody=" + options.body + "&EmailAttachmentPath=" + options.attachements;
        }
        window.open(url);

        // outlookApp = new ActiveXObject("Outlook.Application");
        // mailItem = outlookApp.CreateItem(0);
        // mailItem.Subject = options.subject;
        // if (options.to) {
        //     mailItem.To = options.to;
        // } else {
        //     throw new Error('The email requires a recipent!');
        // }

        // if (options.cc) {
        //     mailItem.CC = options.cc;
        // }

        // if (options.isHtml) {
        //     mailItem.HTMLBody = options.body;
        // } else {
        //     mailItem.Body = options.body;
        // }

        // mailItem.display(0);
        // mailItem.GetInspector.WindowState = 2;

        // var hasinvalidAttachmentLink = false;
        // // filesreports
        // if (options.files) {
        //     for (var j = 0; j < options.files.length; j++) {
        //         try {

        //             mailItem.Attachments.Add(options.files[j]);
        //         }
        //         catch (ex) {
        //             hasinvalidAttachmentLink = true;
        //         }
        //     }
        // }

        // if (filedownloaded) {
        //     try {
        //         mailItem.Attachments.Add(filedownloaded);
        //     }
        //     catch (ex) {
        //         hasinvalidAttachmentLink = true;
        //     }
        // }

        // // add form attachments
        // if (options.attachements) {
        //     for (var i = 0; i < options.attachements.length; i++) {
        //         try {
        //             mailItem.Attachments.Add(options.attachements[i]);
        //         }
        //         catch (ex) {
        //             hasinvalidAttachmentLink = true;
        //         }
        //     }
        // }

        // if (hasinvalidAttachmentLink) {
        //     UDO.Shared.openAlertDialog('One or more files could not be attached as the files are not available or accessible');
        // }
    }
    catch (ex) {
        UDO.Shared.openAlertDialog('Failed to create Outlook message: ' + ex.message);
    } finally {
        outlookApp = null;
    }
}

function CreateAndOpenServiceRequest() {
    var isDirty = formContext.data.entity.getIsDirty();
    if (isDirty) {
        var msg = "Some of the data on the screen has changed and was not saved. Would you like to continue performing this operation without saving the record?\n\nIf you select OK, the unsaved values would not be saved.\n\nSelect Cancel to abort the Service Request creation process and to save the updated information.";
        var title = "Unsaved Changes";
        UDO.Shared.openConfirmDialog(msg, title, 300, 550, "OK", "Cancel")
            .then(function (confirm) {
                if (confirm.confirmed) {
                    ExecuteCloneServiceRequest()
                        .then(function (data) {
                            data.json().then(function(response) {
                                var id = response.udo_servicerequestid;
                                openServiceRequest(id);
                            });
                        }).catch(function (data) {
                            UDO.Shared.openAlertDialog("Failed to create and open service request.","Action Failed", alertOptions.height,alertOptions.width);
                        });
                }
                else {
                    formContext.data.save("save");
                    return false;
                }
            });
    }
    else {
        ExecuteCloneServiceRequest()
            .then(function (data) {
                data.json().then(function(response) {
                    var id = response.udo_servicerequestid;
                    openServiceRequest(id);
                });
            }).catch(function (err) {
                UDO.Shared.openAlertDialog(err.message);
            });
    }
}

//TODO: convert to web api
function ExecuteCloneServiceRequest() {
    var parameters = {};
    var entity = {};
    entity.id = formContext.data.entity.getId().replace("{", "").replace("}", "");
    entity.entityType = "udo_servicerequest";
    parameters.entity = entity;

    var cloneServiceRequest = {
        entity: parameters.entity,

        getMetadata: function () {
            return {
                boundParameter: "entity",
                parameterTypes: {
                    "entity": {
                        "typeName": "mscrm.udo_servicerequest",
                        "structuralProperty": 5
                    }
                },
                operationType: 0,
                operationName: "udo_CloneServiceRequest"
            }
        }
    };
    return webApi.ExecuteRequest(cloneServiceRequest);
}

function openServiceRequest(servicerequestId) {
    UDO.Shared.GetCurrentAppProperties().then(
        function (appProperties) {
            var appId = appProperties.appId;
            var targetEntity = "udo_servicerequest";
            //var url = Xrm.Utility.getGlobalContext().getClientUrl() + "/main.aspx?appid=" + appId + "&newWindow=true&pagetype=entityrecord&etn=" + targetEntity + "&id=" + servicerequestId;

            if (parent.window.IsUSD === true) {
                var clientURL = "http://uii/UdoServiceRequest/Navigate?url=" + Xrm.Utility.getGlobalContext().getClientUrl() + "/main.aspx?appid=" + appId + "&cmdbar=false&navbar=off&newWindow=true&pagetype=entityrecord&etn=udo_servicerequest&id=" + servicerequestId;
                window.open(clientURL);            
            }
        },
        function (error) {
            console.log(error);
        });
}

function validInput(text) {
    var reg = /^[a-z0-9!?@#\$%\^\&*\)\(+=._-]+$/i;
    if (reg.test(text)) {
        return true;
    }
    else {
        return false;
    }
}

function defaultQuickWrite() {
    var requestType = formContext.getAttribute("udo_requesttype").getValue();
    var requestSubtype = formContext.getAttribute("udo_requestsubtype").getValue();
    var payeeCode = formContext.getAttribute("udo_payeecode");
    var quickWriteDescription = formContext.getAttribute("udo_description");

    if (requestType === "Dependent Maintenance" && requestSubtype === "Dependent Verification") {
        console.log("Dependent Verification request subtype processing...");

        var person = formContext.getAttribute('udo_personid').getValue();
        var personId = person[0].id;
        var columns = ['udo_payeecode'];
        webApi.RetrieveRecord(personId, "udo_person", columns)
            .then(function success(result) {
                if (result) {
                    var udo_payeeCode = result.udo_payeecode;
                    payeeCode.setValue(udo_payeeCode);

                    var filterValue = "";
                    if (udo_payeeCode === "00") { // Vet
                        filterValue = "21P-0538-Dependents Questionairre";
                        console.log("Payee Code: " + udo_payeeCode + " will have Quick Write defaulted.");
                    }
                    else if (udo_payeeCode === "10") { // Widow
                        filterValue = "21P-0537-Marital Questionnaire";
                        console.log("Payee Code: " + udo_payeeCode + " will have Quick Write defaulted.");
                    }
                    else {
                        console.log("Payee Code: " + udo_payeeCode + " is not used to default Quick Write.");

                        return;
                    }

                    var cols = ["va_quickwritetext"];
                    var filter = "$filter=va_name eq '" + filterValue + "'";
                    webApi.RetrieveMultiple("va_quickwrite", cols, filter)
                        .then(function (data) {
                            console.log("Quick Write text retrieved: " + data[0].va_quickwritetext);

                            quickWriteDescription.setValue(data[0].va_quickwritetext);
                        })
                        .catch(function (err) {
                            console.log("Error retrieving Quick Write Text for Dependent Verification: " + err);
                        });
                }
                else {
                    console.log("Unable to read udo_person record: " + personId);
                }
            })
            .catch(function (error) {
                console.log("Unable to retrieve person record.");
            });
    }
    else {
        console.log("Skipping Quick Write default for request: " + requestType + " and request subtype: " + requestSubtype);
    }
}

function CheckMandatoryFields(context) {
    var populated = true;
    context.getAttribute(function (attribute, index) {
        if (attribute.getRequiredLevel() === "required") {
            if (attribute.getValue() === null) {
                populated = false;
            }
        }
    });
    return populated;
}

function processPreview0820(context) {
    context.ui.clearFormNotification("80");
    var isDirty = context.data.entity.getIsDirty();
    if (!isDirty) {
        window.open("http://event/?eventname=Preview0820");
    } else {
        context.data.save().then(function () {
            if (!CheckMandatoryFields(context)) {
                context.ui.setFormNotification("You have required fields that must be entered before clicking on Preview 0820", "WARNING", "80");
            } else {
                window.open("http://event/?eventname=Preview0820");
            }
        });
    }
}

function UDOCloseSR(context) {
    if (context.data.entity.getIsDirty()) {
        if (CheckMandatoryFields(context)) {
            context.data.save().then(function () {
                window.open("http://close/");
            });
            
        } else {
            Xrm.Navigation.openConfirmDialog({ text: "Are you sure you want to close this form?"},null,
             function () {
                UDOSetRequiredLevelOnAllRequiredFields(context);
                context.data.save().then(function () {
                    window.open("http://close/");
                });
             },
             function () {
                 // do nothing
             }
            );
        }
    }
    else {                      
        context.data.save().then(function () {
                window.open("http://close/");
            }, function () {
                Xrm.Navigation.openConfirmDialog({ text: "Are you sure you want to close this form?"},null,
                function () {
                    UDOSetRequiredLevelOnAllRequiredFields(context);
                    context.data.save().then(function () {
                        window.open("http://close/");
                    });
             },
             function () {
                 // do nothing
             });
        });
    }
}

function UDOSetRequiredLevelOnAllRequiredFields(context) {
    context.getAttribute(function (attribute, index) {
        if (attribute.getRequiredLevel() === "required") {
            if (attribute.getValue() === null) {
                var myName = attribute.getName();
                context.getAttribute(myName).setRequiredLevel("none");
            }
        }
    });
}

function serviceRequestSave(context) {  
    context.data.save(); 
} 
function serviceRequestCancel(context) {  
    if (confirm("Do you want to cancel the service Request?")) {  
        var action = context.getAttribute("udo_requeststatus").setValue(953850007);  
        serviceRequestSave(context);  
    } 
}

function processSendEmail(context) {	
	context.ui.clearFormNotification("80");
	var isDirty = context.data.entity.getIsDirty();
	if (!isDirty) {
		 window.open("http://event/?eventname=SendEmail");
	} else {
		context.data.save().then(function() {
			if (!CheckMandatoryFields(context)) {
				 context.ui.setFormNotification("You have required fields that must be entered before clicking on Send Email", "WARNING", "80");
			} else {
				 window.open("http://event/?eventname=SendEmail");
			}			
		});
	}
}

function process510EndProduct(context) {
    if (confirm("Please confirm that you would like to try to submit a 510 End Product")) {
        ProcessClaimEstablishment();
    }
}

function RunSendEmail(context, filedownload) {    
    SendEmailToRO(filedownload);
}
//RunSendEmail("[[ServiceRequest.DownloadedFileJS]+]");
