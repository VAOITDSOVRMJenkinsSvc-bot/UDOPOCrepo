"use strict";

var _formContext = null;
var globalContext = Xrm.Utility.getGlobalContext();
var version = globalContext.getVersion();
var lib;
var webApi;
var formHelper;
var CRM_FORM_TYPE_CREATE = 1;
var CRM_FORM_TYPE_UPDATE = 2;
var Stage1 = "c64097cb-570d-4635-8d0d-23bafb5d66aa"; // General and Dependents
var Stage2 = "a01b2c1f-10af-4164-9361-18ff7a44ed0f"; // Summary
var Stage3 = "f5795ce8-aa58-480b-bd5d-840fd47d0b08"; // Associated Documents
var PreviousTab = "";
var CurrentTab = "";
var _refreshingSummaryText = false;
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

function instantiateCommonScripts(executionContext) {
    lib = new CrmCommonJS.CrmCommon(version, executionContext);
    webApi = lib.WebApi;
    formHelper = new CrmCommonJS.FormHelper(executionContext);
    _formContext = executionContext.getFormContext();
}

function OnLoad(executionContext) {
    instantiateCommonScripts(executionContext);
    showHidePensionFields(executionContext);

    var propertiesAppInsights = {
        "method": "Va.Udo.Crm.Scripts.DependentMaintenance.onLoad", "description": "Called on load of UDO Dependent Maintenance form"
    };
    startTrackEvent("UDO Dependent Maintenance onLoad", propertiesAppInsights);

    // Check for Plugin hidden error on form
    var formError = formHelper.getValue("crme_hiddenerrormessage");
    if (formError !== null) {
        console.log("Exception occurred loading Dependent Maintenance information: " + formError);

        var msg = "";
        var title = "Error Initializing Dependent Maintenance";
        if (formError.includes("500")) {
            msg = "A problem occurred retrieving data from the backend VA system.  Please cancel this Dependent Maintenance window and try creating a new one in a few minutes.  If problem persists, please contact your local IT.";
        }
        else {
            msg = formError;
        }
        UDO.Shared.openAlertDialog(msg, title, 300, 450).then(
            function success(result) {
            },
            function (error) {
                console.log("Error displaying alert dialog: " + error.message);
            }
        );
    }

    //  set the claim date if not specified
    if (formHelper.getValue("crme_claimdate") === null) {
        var currentdate = new Date();
        formHelper.setValue("crme_claimdate", currentdate);
    }

    var addr1Att = _formContext.getAttribute("crme_address1");
    if (addr1Att !== null) {
        if (addr1Att.getValue() !== null) {

            var addr1 = addr1Att.getValue();
            if (addr1.length > 20) {
                var msg = "The Veteran’s address line 1 is longer than 20 characters and cannot be submitted to RBPS. Complete CADD before proceeding to add/edit/remove dependents. ";
                var title = "Address Too Long";
                UDO.Shared.openAlertDialog(msg, title, 300, 450).then(
                    function success(result) {
                    },
                    function (error) {
                        console.log(error.message);
                    }
                );
            }
        }
    }

    var SummaryTab = _formContext.ui.tabs.get("tab_Summary");
    SummaryTab.addTabStateChange(SummaryTabClicked);

    var DependentsTab = _formContext.ui.tabs.get("tab_General_Dependents");
    DependentsTab.addTabStateChange(DependentTabClicked);

    var txnStatus = formHelper.getValue("crme_txnstatus");
    var initialTxnStatus = formHelper.getAttribute("crme_txnstatus").getInitialValue();
    if (txnStatus !== initialTxnStatus && initialTxnStatus === 935950000) {
        txnStatus = initialTxnStatus;
        formHelper.setValue("crme_txnstatus", initialTxnStatus);
    }

    // If the status is NOT DRAFT - everything is read only after that
    if (txnStatus !== 935950000) {
        formHelper.setDisabled("crme_claimdate", true);
        formHelper.setDisabled("crme_maritalstatus", true);
        formHelper.setDisabled("crme_timesmarried", true);
    }

    var activeStage = _formContext.data.process.getActiveStage();
    if (activeStage !== undefined && (activeStage.getId() === Stage1 || activeStage.getId() === Stage2)) {
        formHelper.setValue("crme_txnstatus", 935950000); // Keep in Draft state

        if (formHelper.getValue("crme_maritalstatus") !== null) {
            preSave(_formContext);  // In order for dependents grid to populate
        }
    }

    _formContext.data.process.addOnStageChange(StageChanged);

    // Set initial Tab focus
    StageChanged(executionContext);

    buildFormButtons(executionContext); // Dynamically generate form buttons each time in UCI

    stopTrackEvent("UDO Dependent Maintenance onLoad", propertiesAppInsights);
}

function StageChanged(executionContext) {
    if (_formContext === undefined || _formContext === null) {
        _formContext = executionContext.getFormContext();
    }

    var activeStage = _formContext.data.process.getActiveStage();
    if (activeStage !== undefined && activeStage.getId() === Stage1) {
        _formContext.ui.tabs.get("tab_General_Dependents").setFocus();
    } else {
        _formContext.ui.tabs.get("tab_Summary").setFocus();

        refreshSummaryText(executionContext);
    }

    console.log("Stage changed");
}

function TabChanged(executionContext, tabname) {
    buildFormButtons(executionContext); // Dynamically generate form buttons each time in UCI

    if (tabname === "SummaryTab") {
        refreshSummaryText(executionContext);
    }

    console.log("Tab changed to: " + tabname);
}

function SummaryTabClicked(executionContext) {
    TabChanged(executionContext, "SummaryTab");

    console.log("Summary tab clicked");
}

function DependentTabClicked(executionContext) {
    TabChanged(executionContext, "DependentTab");
    console.log("Dependent tab clicked");
}

function preSave(formContext) {
    var MaritalStatus = formContext.getAttribute("crme_maritalstatus");
    MaritalStatus.setRequiredLevel("none");

    formContext.data.save().then(
        function Success() {
            console.log("Success on Save");
            MaritalStatus.setRequiredLevel("required");
        },
        function Error(err) {
            console.log("Error on Save: " + err);
            MaritalStatus.setRequiredLevel("required");
        });
}

function setSubject() {
    if (_formContext.getAttribute('regardingobjectid').getValue() !== null && _formContext.getAttribute('subject').getValue() === null) {
        var vetFirstName = _formContext.getAttribute("crme_firstname").getValue();
        var vetLastName = _formContext.getAttribute("crme_lastname").getValue();
        var vetName = vetLastName + ", " + vetFirstName + " - Depdendent Maintenance";

        _formContext.getAttribute('subject').setValue(vetName);
    }
}

function refreshDependentSubGrid() {
    var grid = _formContext.getControl("list_Dependents");

    grid.refresh();

    console.log("Dependent subgrid refreshed.");
}

function buildFormButtons(executionContext) {
    var formContext = executionContext.getFormContext();
    var buttonControlGeneralTab = formContext.getControl("WebResource_ButtonGenerator_GeneralTab");
    var buttonControlSummaryTab = formContext.getControl("WebResource_ButtonGenerator_SummaryTab");

    var buttonArrayGeneralTab = [
        {
            id: "btnMoveNextGeneralTab",
            text: "Next »",
            action: function () {
                // logic to execute when the button is pressed
                // note: you can use the formContext object in this function
                formContext.data.save().then(function () {
                    formContext.data.process.moveNext();

                    formContext.ui.tabs.get("tab_Summary").setFocus();
                },
                    function (err) {
                        console.log("Error saving form: " + err);
                    })
            }
        }
    ]

    var buttonArraySummaryTab = [
        {
            id: "btnMovePreviousSummaryTab",
            text: "« Previous",
            action: function () {
                // logic to execute when the button is pressed
                // note: you can use the formContext object in this function
                formContext.data.save().then(function () {
                    formContext.data.process.movePrevious();

                    formContext.ui.tabs.get("tab_General_Dependents").setFocus();
                },
                    function (err) {
                        console.log("Error saving form: " + err);
                    });
            }
        },
        {
            id: "btnSubmitDependentsSummaryTab",
            text: "Submit Dependent(s)",
            action: function () {
                // logic to execute when the button is pressed
                // note: you can use the formContext object in this function

                // Verify BPF is active before proceeding
                if (bpfStatus() === "active") {
                    if (window.IsUSD === true) {
                        setTimeout(function () {
                            window.open("http://event/?eventName=SubmitDependentMaintenance");
                        }, 500);
                    } else {
                        submitTransaction(true); // Found in crme_dependentmaintenanceRibbon.js
                    }
                }
            }
        }
    ]

    if (buttonControlGeneralTab) {
        buttonControlGeneralTab.getContentWindow().then(
            function (contentWindow) {
                contentWindow.ButtonGenerator.CreateNewButtons(buttonArrayGeneralTab);
            }
        )
    }

    if (buttonControlSummaryTab) {
        buttonControlSummaryTab.getContentWindow().then(
            function (contentWindow) {
                contentWindow.ButtonGenerator.CreateNewButtons(buttonArraySummaryTab);
            }
        )
    }
}

function refreshSummaryText(executionContext) {
    if (_refreshingSummaryText) {
        return;
    }
    else {
        _refreshingSummaryText = true;
    }
    console.log("Refreshing WebResource_SummaryText...");

    if (_formContext === undefined || _formContext === null) {
        if (executionContext && executionContext.getFormContext) {
            _formContext = executionContext.getFormContext();
        }
    }

    var webResource = _formContext.getControl("WebResource_SummaryText");
    var src = webResource.getSrc();

    var aboutBlank = "about:blank";
    webResource.setSrc(aboutBlank);

    setTimeout(function () {
        webResource.setSrc(src);
        console.log("WebResource_SummaryText refreshed");
        _refreshingSummaryText = false;
    }, 2000);
}

function CheckStatus() {
    var sts = _formContext.getAttribute("crme_txnstatus").getText();
    var res = confirm("You have not submitted your Dependent Maintenance record.  Press OK to continue with the close and discard changes.  Press Cancel to remain on this record.");
    if (res == true) {
        return "True";
    } else {
        return "False";
    }
}

function showHidePensionFields(executionContext) {
    var formContext = executionContext.getFormContext();
    var awardType = formContext.getAttribute("udo_awardtype").getValue();
    if (awardType === 751880000) {//pension
        //show fields
        formContext.getControl("udo_ishouseholdgreaterthanthreshhold").setVisible(true);
        formContext.getControl("udo_dependentshaveincome").setVisible(true);
        formContext.getControl("udo_householdnetworththreshold").setVisible(true);
        //set required       
        formContext.getAttribute("udo_dependentshaveincome").setRequiredLevel("required");
        formContext.getAttribute("udo_ishouseholdgreaterthanthreshhold").setRequiredLevel("required");

        var stDate = new Date();
        var stMonth = stDate.getMonth() + 1;
        var stDay = stDate.getDate();
        var stYear = stDate.getFullYear();
        var formattedDate = stMonth + "-" + stDay + "-" + stYear;

        console.log("Today's formatted date is :" + formattedDate);

        var fetchData = {
            "va_name": "Dependent Maintenance - household net worth threshold for pension dependents",
            "today": formattedDate,
            "statecode": "0"
        };
        var fetchXml = [
            "?fetchXml=<fetch top='1'>",
            "  <entity name='va_systemsettings'>",
            "    <attribute name='udo_enddate'/>",
            "    <attribute name='va_description'/>",
            "    <attribute name='udo_startdate'/>",
            "    <attribute name='va_name'/>",
            "    <attribute name='va_type'/>",
            "    <filter>",
            "      <condition attribute='va_name' operator='eq' value='", fetchData.va_name, "'/>",
            "      <condition attribute='udo_startdate' operator='on-or-before' value='", fetchData.today, "'/>",
            "      <condition attribute='udo_enddate' operator='on-or-after' value='", fetchData.today, "'/>",
            "      <condition attribute='statecode' operator='eq' value='", fetchData.statecode, "'/>",
            "    </filter>",
            "  </entity>",
            "</fetch>"
        ].join("");

        Xrm.WebApi.retrieveMultipleRecords("va_systemsettings", fetchXml)
            .then(function (results) {
                if (results != null && results.entities.length > 0 && results.entities[0] != null) {
                    var threshHold = results.entities[0];
                    var threshHoldValue = threshHold["va_description"].trim();
                    //set threshold and make readonly
                    formContext.getAttribute("udo_householdnetworththreshold").setValue(Number(threshHoldValue));
                    formContext.getControl("udo_householdnetworththreshold").setDisabled(true);
                    console.log("Threshhold is found and set to: " + threshHoldValue);
                } else {                    
                    formContext.getControl("udo_householdnetworththreshold").setDisabled(true);                    
                    var alertStrings = {
                        confirmButtonLabel: "OK",
                        text: "There is no active income threshold amount entered in the system. Please contact your System Administrator to update the income threshold amount.",
                        title: "Active Threshold Not Found"
                    };
                    Xrm.Navigation.openAlertDialog(alertStrings);
                }
            }, function (error) {
                formContext.getControl("udo_householdnetworththreshold").setDisabled(true);               
                var alertStrings = {
                    confirmButtonLabel: "OK",
                    text: "There is no active income threshold amount entered in the system. Please contact your System Administrator to update the income threshold amount.",
                    title: "Active Threshold Not Found"
                };
                Xrm.Navigation.openAlertDialog(alertStrings);//3617
                console.log("Error on reading Dependent Maintenance - household net worth threshold for pension dependents." + error.message);
                }
        );

    } else {
        //hide fields
        formContext.getControl("udo_ishouseholdgreaterthanthreshhold").setVisible(false);
        formContext.getControl("udo_dependentshaveincome").setVisible(false);
        formContext.getControl("udo_householdnetworththreshold").setVisible(false);
        //set optional
        formContext.getAttribute("udo_ishouseholdgreaterthanthreshhold").setRequiredLevel("none");
        formContext.getAttribute("udo_dependentshaveincome").setRequiredLevel("none");
        formContext.getAttribute("udo_householdnetworththreshold").setRequiredLevel("none");
    }

}