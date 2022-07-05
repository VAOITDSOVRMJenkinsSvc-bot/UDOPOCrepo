"use strict";

var POIRequestForm = "691331a3-1e2c-49af-a347-ebf715f8bbb5";
var POIProcessingForm = "c572ca23-db8a-4ec9-bf91-b50b6bdd7902";

function onLoad(exCon) {
    var formContext = exCon.getFormContext();
    UDO.Shared.FormContext = formContext;
    var globalContext = Xrm.Utility.getGlobalContext();

    formContext.getControl("udo_requestedaction").removeOption(751880008);

    if (formContext.ui.formSelector.getCurrentItem().getId() === POIProcessingForm) {
        UDO.Shared.GetCurrentAppProperties().then(
            function (appProperties) {
                var appId = appProperties.appId;
                var targetEntity = "udo_pointofinteraction";
                var url;
                var Id = formContext.data.entity.getId().replace("{", "").replace("}", "");
                if ((Id === "") || (Id === null)) {
                    url = globalContext.getClientUrl() + "/main.aspx?appid=" + appId + "&cmdbar=false&navbar=off&newWindow=true&pagetype=entityrecord&etn=" + targetEntity + "&formid=" + POIRequestForm;
                } else {
                    url = globalContext.getClientUrl() + "/main.aspx?appid=" + appId + "&cmdbar=false&navbar=off&newWindow=true&pagetype=entityrecord&etn=" + targetEntity + "&id=" + Id + "&formid=" + POIRequestForm;
                }

                // USD Agents will always see the Request form
                if ((window.IsUSD) || (parent.window.IsUSD)) {
                    window.open(url);
                } else if ((formContext.data.entity.getId() === "") || (formContext.data.entity.getId() === null)) {
                    window.open(url);
                }
            });
    }
}

function statusReasonUpdate(exCon) {
    var formContext = exCon.getFormContext();
    var statusReason = formContext.getAttribute("statuscode").getValue();
    var entityName = formContext.data.entity.getEntityName();
    if (statusReason === 751880000) {
        var d = new Date();
        formContext.getAttribute("udo_rejectiontimestamp").setValue(d);
    }
    else if (statusReason == 2) {
        if ((window.IsUSD) || (parent.window.IsUSD)) {
            UDO.Shared.GetEntitySetName(entityName).then(
                function (setName) {
                    var state = 1; // Inactive
                    var status = 751880005; // Request Completed in UDO
                    var recordId = formContext.data.entity.getId().replace("{", "").replace("}", "");
                    UDO.Shared.SetRecordStatus(recordId, setName, state, status).then(function () {
                        formContext.data.refresh(false);
                    });
                }
            );
        }
        else {
            UDO.Shared.GetEntitySetName(entityName).then(
                function (setName) {
                    var state = 1; // Inactive
                    var status = 751880006; // Request Completed in Web App
                    var recordId = formContext.data.entity.getId().replace("{", "").replace("}", "");
                    UDO.Shared.SetRecordStatus(recordId, setName, state, status).then(function () {
                        formContext.data.refresh(false);
                    });
                }
            );
        }
    }
}

function USD_Submit(context, caller) {
    if (!CheckRequiredFields(context)) {
        UDO.Shared.openAlertDialog("Please enter all required fields.");
        return;
    }
    var StatusReason = context.getAttribute("statuscode");
    StatusReason.setValue(1); // 1 = Pending

    context.data.save().then(
        function (success) {
            if ((window.IsUSD) || (parent.window.IsUSD)) {
                if (caller === "POI Global") {
                    window.open("http://uii/POI Global/Close");
                }
                
                if (caller === "Point of Interaction") {
                    window.open("http://uii/Point of Interaction/Close");
                }
            }
        },
        function (errorCallback) {
            alert("An error has occurred. Please try again");
        });
}

function CheckRequiredFields(context) {
    var populated = true;
    context.getAttribute(function (attribute, index) {
        if (attribute.getRequiredLevel() == "required") {
            if (attribute.getValue() === null) {
                populated = false;
            }
        }
    });
    return populated;
}

function USD_RequestCompleted(context, caller) {
    if (!CheckRequiredFields(context)) {
        UDO.Shared.openAlertDialog("Please enter all required fields.");
        return;
    }
    var entityName = context.data.entity.getEntityName();
    UDO.Shared.GetEntitySetName(entityName).then(
        function (setName) {
            var state = 1; // Inactive
            var status = 2; // Request Completed 
            context.data.save().then(function (success) {
                var recordId = context.data.entity.getId().replace("{", "").replace("}", "");
                UDO.Shared.SetRecordStatus(recordId, setName, state, status).then(function () {
                    context.data.refresh(false).then(function () {
                        context.ui.refreshRibbon();

                        if ((window.IsUSD) || (parent.window.IsUSD)) {
                            status = 751880005 //Request Completed in UDO
                        }
                        else {
                            status = 751880006 //Request Completed in Web App
                        }
                        UDO.Shared.SetRecordStatus(recordId, setName, state, status).then(function () {
                            if ((window.IsUSD) || (parent.window.IsUSD)) {
                                if (caller === "POI Global") {
                                    window.open("http://uii/POI Global/Close");
                                }
                                
                                if (caller === "Point of Interaction") {
                                    window.open("http://uii/Point of Interaction/Close");
                                }
                            }
                        },
                            function (error) {
                                UDO.Shared.openAlertDialog("An error has occurred:" + error);
                            });
                    });
                },
                    function (error) {
                        UDO.Shared.openAlertDialog("An error has occurred:" + error);
                    });
            },
                function (error) {
                    UDO.Shared.openAlertDialog("An error has occurred:" + error);
                });
        });
}

function USD_Activate(context) {
    UDO.Shared.openConfirmDialog("Are you sure you want to activate this record?").then(
        function (confirm) {
            if (confirm.confirmed) {
                var entityName = context.data.entity.getEntityName();
                UDO.Shared.GetEntitySetName(entityName).then(
                    function (setName) {
                        var state = 0; // 0 = Active, 1 = Inactive
                        var status = 751880003; // 751880003 = Draft
                        var recordId = context.data.entity.getId().replace("{", "").replace("}", "");
                        context.data.save().then(function () {
                            UDO.Shared.SetRecordStatus(recordId, setName, state, status).then(function () {
                                context.data.refresh(false).then(function () {
                                    context.ui.refreshRibbon();
                                });
                            },
                                function (error) {
                                    UDO.Shared.openAlertDialog("An error has occurred:" + error);
                                });
                        },
                            function (error) {
                                UDO.Shared.openAlertDialog("An error has occurred:" + error);
                            });
                    },
                    function (error) {
                        UDO.Shared.openAlertDialog(error);
                    });
            }
        },
        function error() {
            UDO.Shared.openAlertDialog("Open confirm error");
        });
}
