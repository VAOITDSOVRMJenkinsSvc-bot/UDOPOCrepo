"use strict";

var globalContext = Xrm.Utility.getGlobalContext();
var formContext = null;
var version = globalContext.getVersion();
var lib;
var webApi;
var formHelper;

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
    try {
        lib = new CrmCommonJS.CrmCommon(version, executionContext);
        webApi = lib.WebApi;
        formHelper = new CrmCommonJS.FormHelper(executionContext);

        if (executionContext.getFormContext !== undefined) {
            formContext = executionContext.getFormContext();
        } else if (executionContext.getAttribute !== undefined) {
            formContext = executionContext;
        }
    } catch (ex) {
        console.log("Exception: " + ex.message);
    }
}

function onLoad(executionContext) {
    try {
        instantiateCommonScripts(executionContext);

        var propertiesAppInsights = {
            "method": "Va.Udo.Crm.Scripts.ClaimEstablishment.onLoad", "description": "Called on load of UDO Claim Establishment form"
        };
        startTrackEvent("UDO Military Service onLoad", propertiesAppInsights);

        formHelper.clearFormNotification("121");

        var udoClaimReceivedDate = formHelper.getAttribute("udo_claimreceiveddate").getValue();
        if (udoClaimReceivedDate) {
            formHelper.getTab("tab_InsertSection").setVisible(false);
        } else {
            formHelper.getTab("tab_InsertSection").setVisible(false);
        }

        var userRoles = Xrm.Utility.getGlobalContext().userSettings.roles;
        CheckUserRole("System Administrator", userRoles).then(function reply(hasRoles) {
            if (hasRoles) {
                formHelper.getTab("tab_System_Admin").setVisible(false);

                formHelper.getTab("tab_InsertSection").setVisible(false);
            } else {
                formHelper.getTab("tab_System_Admin").setVisible(false);
            }

            var udoExceptionOccurrred = formHelper.getAttribute("udo_exceptionoccurred").getValue();
            if (udoExceptionOccurrred) {

                var udoExceptionMessage = formHelper.getAttribute("udo_exceptionmessage").getValue();
                formHelper.setFormNotification("Last Exception Message - " + udoExceptionMessage, "ERROR", "121");
            }
        });
    } catch (ex) {
        console.log("Exception: " + ex.message);
    }
}
    stopTrackEvent("UDO Claim Establishment onLoad", propertiesAppInsights);
}

function CheckUserRole(roleName, userRoles) {
    var admin = false;

    try {
        var filter = "$select=roleid,name&$filter=name eq '" + roleName + "'";

        return webApi.RetrieveMultiple("role", null, filter).then(function (adminRoles) {
            for (var adminRole in adminRoles) {
                for (var userRole in userRoles) {
                    if (adminRole === userRole) {
                        admin = true;
                    }
                }
            }
        });

        return admin;
    } catch (ex) {
        console.log("Exception: " + ex.message);
    }
}

function createClaimEstablishmentNote(operationName) {
    try {
        console.log("inCreateNote");

        return new Promise(function (resolve, reject) {
            var entity = {};
            var endProductDesc = "";
            if (formContext.getAttribute("udo_endproduct") !== null) {
                var erEndProduct = formContext.getAttribute("udo_endproduct").getValue();
                endProductDesc = erEndProduct[0].name;
            }

            entity.udo_participantid = formContext.getAttribute("udo_participantid") === null
                ? " "
                : formContext.getAttribute("udo_participantid").getValue();

            if (formContext.getAttribute("udo_veteranid") !== null) {
                var selectedItem = formContext.getAttribute("udo_veteranid").getValue();
                entity["udo_veteranid@odata.bind"] = "/contacts(" + selectedItem[0].id.replace(/{/g, "").replace(/}/g, "") + ")";
            }

            if (formContext.getAttribute("udo_personid") !== null) {
                var erPerson = formContext.getAttribute("udo_personid").getValue();
                entity["udo_personid@odata.bind"] = "/udo_persons(" + erPerson[0].id.replace(/{/g, "").replace(/}/g, "") + ")";
            }

            if (formContext.getAttribute("udo_idproofid") !== null) {
                var erIdProof = formContext.getAttribute("udo_idproofid").getValue();
                entity["udo_idproofid@odata.bind"] = "/udo_idproofs(" + erIdProof[0].id.replace(/{/g, "").replace(/}/g, "") + ")";
            }
            entity.udo_name = "Notes from End Product";
            entity.udo_notetext = "End Product - " + endProductDesc + " was inserted";
            entity.udo_editable = true;
            entity.udo_fromudo = true;

            Xrm.WebApi.online.createRecord("udo_note", entity).then(
                function success(result) {
                    var newEntityId = result.id;
                    resolve(newEntityId);
                },
                function (error) {
                    console.log("error:");
                    reject(error.message);
                }
            );
        });
    } catch (ex) {
        console.log("Exception: " + ex.message);
    }
}

function checkClaimEstablishmentRequiredFields() {
    try {
        return new Promise(function (resolve, reject) {
            console.log("in checkFields");
            var foundError = false;

            if (formContext.getAttribute("udo_filenumber").getValue() === null) {
                foundError = true;
            }

            if (formContext.getAttribute("udo_firstname").getValue() === null) {
                foundError = true;
            }

            if (formContext.getAttribute("udo_lastname").getValue() === null) {
                foundError = true;
            }

            if (formContext.getAttribute("udo_endproduct").getValue() === null) {
                foundError = true;
            }

            if (formContext.getAttribute("udo_ssn").getValue() === null) {
                foundError = true;
            }

            if (formContext.getAttribute("udo_dateofclaim").getValue() === null) {
                foundError = true;
            }

            if (formContext.getAttribute("udo_sectionunitno").getValue() === null) {
                foundError = true;
            }

            if (formContext.getAttribute("udo_benefitclaimtype").getValue() === null) {
                foundError = true;
            }

            if (formContext.getAttribute("udo_payeecodeid").getValue() === null) {
                foundError = true;
            }

            resolve(foundError);
        });
    } catch (ex) {
        console.log("Exception: " + ex.message);
    }
}

function insertClaimEstablishment(executionContext) {
    try {
        instantiateCommonScripts(executionContext);

        return new Promise(function (resolve, reject) {
            formContext.ui.clearFormNotification("121");

            var guid = formContext.data.entity.getId();
            var cleanGuid = guid.replace(/{/g, "").replace(/}/g, "");

            var parameters = {};
            var parententityreference = {};
            parententityreference.udo_claimestablishmentid = cleanGuid;
            parententityreference["@odata.type"] = "Microsoft.Dynamics.CRM.udo_claimestablishment";
            parameters.ParentEntityReference = parententityreference;

            var udo_InsertClaimEstablishmentRequest = {
                ParentEntityReference: parameters.ParentEntityReference,

                getMetadata: function () {
                    return {
                        boundParameter: null,
                        parameterTypes: {
                            "ParentEntityReference": {
                                "typeName": "mscrm.crmbaseentity",
                                "structuralProperty": 5
                            }
                        },
                        operationType: 0,
                        operationName: "udo_InsertClaimEstablishment"
                    };
                }
            };

            return checkClaimEstablishmentRequiredFields().then(function (foundError) {
                if (foundError) {
                    formContext.ui.setFormNotification("One or more required fields are missing.", "ERROR", "121");

                    console.log(foundError);
                } else {
                    return formContext.data.save().then(function () {
                        return Xrm.WebApi.online.execute(udo_InsertClaimEstablishmentRequest).then(
                            function success(result) {
                                result.json().then(
                                    function (responseText) {
                                        console.log(responseText);
                                        if (responseText.DataIssue !== false || responseText.Timeout !== false || responseText.Exception !== false) {
                                            formContext.ui.setFormNotification("An error occurred - " + responseText.ResponseMessage, "ERROR", "121");

                                            reject();
                                        }
                                        else {
                                            formContext.ui.setFormNotification("Insert Claim Establishment Completed", "INFORMATION");
                                            createClaimEstablishmentNote("udo_InsertClaimEstablishment").then(function (response) {
                                                resolve();

                                                if (window.IsUSD !== null && window.IsUSD === true) {
                                                    window.open("http://event/?EventName=RefreshClaimEstablishment");
                                                    window.open("http://event/?eventName=CloseClaimEstablishmentForm");
                                                }
                                            });
                                        }
                                    }
                                );
                            },
                            function (error) {
                                formContext.ui.setFormNotification("Error initiating " +
                                    "udo_InsertClaimEstablishment" +
                                    " record; please try refreshing the page: " +
                                    error.message, "ERROR", "121");

                                reject();
                            }
                        );
                    });
                }
            });
        });
    } catch (ex) {
        console.log("Exception: " + ex.message);
    }
}

function clearClaimEstablishment(executionContext) {
    try {
        instantiateCommonScripts(executionContext);

        return new Promise(function (resolve, reject) {
            console.log("in clear");
            formContext.ui.clearFormNotification("121");
            var guid = formContext.data.entity.getId();
            var cleanGuid = guid.replace(/{/g, "").replace(/}/g, "");

            var parameters = {};
            var parententityreference = {};
            parententityreference.udo_claimestablishmentid = cleanGuid;
            parententityreference["@odata.type"] = "Microsoft.Dynamics.CRM.udo_claimestablishment";
            parameters.ParentEntityReference = parententityreference;

            var udo_ClearClaimEstablishmentRequest = {
                ParentEntityReference: parameters.ParentEntityReference,

                getMetadata: function () {
                    return {
                        boundParameter: null,
                        parameterTypes: {
                            "ParentEntityReference": {
                                "typeName": "mscrm.crmbaseentity",
                                "structuralProperty": 5
                            }
                        },
                        operationType: 0,
                        operationName: "udo_ClearClaimEstablishment"
                    };
                }
            };

            return checkClaimEstablishmentRequiredFields().then(function (foundError) {
                if (foundError) {
                    formContext.ui.setFormNotification("One or more required fields are missing.", "ERROR", "121");

                    console.log(foundError);
                } else {
                    return formContext.data.save().then(function () {
                        Xrm.WebApi.online.execute(udo_ClearClaimEstablishmentRequest).then(
                            function success(result) {
                                result.json().then(
                                    function (responseText) {
                                        console.log(responseText);
                                        if (responseText.DataIssue !== false || responseText.Timeout !== false || responseText.Exception !== false) {
                                            formContext.ui.setFormNotification("An error occurred - " + responseText.ResponseMessage, "ERROR", "121");

                                            reject();
                                        }
                                        else {
                                            formContext.ui.setFormNotification("Clear Claim Establishment Completed", "INFORMATION");
                                        }
                                        createClaimEstablishmentNote("udo_ClearClaimEstablishment").then(function (response) {
                                            resolve();

                                            if (window.IsUSD !== null && window.IsUSD === true) {
                                                window.open("http://event/?EventName=RefreshClaimEstablishment");
                                                window.open("http://event/?eventName=CloseClaimEstablishmentForm");
                                            }
                                        });
                                    }
                                );
                            },
                            function (error) {
                                formContext.ui.setFormNotification("Error initiating " +
                                    "udo_ClearClaimEstablishment" +
                                    " record; please try refreshing the page: " +
                                    err.responseText, "ERROR", "121");

                                reject();
                            }
                        );
                    });
                }
            });
        });
    } catch (ex) {
        console.log("Exception: " + ex.message);
    }
}

function closeClaimEstablishmentPage(executionContext) {
    try {
        instantiateCommonScripts(executionContext);

        var requiredFields = false;
        if (formContext.getAttribute("udo_filenumber").getValue() === null) {
            requiredFields = true;
        }

        if (formContext.getAttribute("udo_benefitclaimtype").getValue() === null) {
            requiredFields = true;
        }

        if (formContext.getAttribute("udo_payeecodeid").getValue() === null) {
            requiredFields = true;
        }

        if (formContext.getAttribute("udo_endproduct").getValue() === null) {
            requiredFields = true;
        }

        if (formContext.getAttribute("udo_ssn").getValue() === null) {
            requiredFields = true;
        }

        if (formContext.getAttribute("udo_firstname").getValue() === null) {
            requiredFields = true;
        }

        if (formContext.getAttribute("udo_dateofclaim").getValue() === null) {
            requiredFields = true;
        }

        if (formContext.getAttribute("udo_sectionunitno").getValue() === null) {
            requiredFields = true;
        }

        if (formContext.data.entity.getIsDirty() || requiredFields === true) {
            if (UDOCheckMandatoryFields()) {
                formContext.data.save();

                window.open("http://event/?eventName=CloseClaimEstablishmentForm");
            } else {
                var msg = "Are you sure you want to close this form?";
                var title = "Close Tab";
                UDO.Shared.openConfirmDialog(msg, title, "Yes", "No")
                    .then(
                        function (response) {
                            if (response.confirmed) {
                                UDOSetRequiredLevelOnAllRequiredFields();
                                formContext.data.save();

                                window.open("http://event/?eventName=CloseClaimEstablishmentForm");
                            }
                        });
            }
        }
        else {
            window.open("http://event/?eventName=CloseClaimEstablishmentForm");
        }
    } catch (ex) {
        console.log("Exception: " + ex.message);
    }
}

function UDOCheckMandatoryFields() {
    var populated = true;

    try {
        formContext.getAttribute(function (attribute, index) {
            if (attribute.getRequiredLevel() === "required") {
                if (attribute.getValue() === null) {
                    populated = false;
                }
            }
        });
    } catch (ex) {
        console.log("Exception: " + ex.message);
    }

    return populated;
}

function UDOSetRequiredLevelOnAllRequiredFields() {
    try {
        formContext.getAttribute(function (attribute, index) {
            if (attribute.getRequiredLevel() === "required") {
                if (attribute.getValue() === null) {
                    var myName = attribute.getName();
                    formContext.getAttribute(myName).setRequiredLevel("none");
                }
            }
        });
    } catch (ex) {
        console.log("Exception: " + ex.message);
    }
}

function refreshGrid() {
    document.getElementById("refreshButtonLink").click();
}