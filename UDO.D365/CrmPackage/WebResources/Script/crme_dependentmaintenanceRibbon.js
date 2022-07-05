"use strict";

var _globalContext = Xrm.Utility.getGlobalContext();
//var _version = _globalContext.getVersion();
//var _lib = new CrmCommonJS.CrmCommon(_version, null);
//var _webApi = _lib.WebApi;
var _formContext = null;

function waitForSaveForSubmit(context) {
    console.log("Submitting dependent(s)...");

    submitTransaction(true);
}

function waitForSaveForCancel() {
    console.log("Cancelled dependent maintenance transaction.");

    cancelTransaction(true);
}

function openSelectedDependent(executionContext) {
    _formContext = executionContext.getFormContext();

    var grid = document.getElementById("crme_relateddependents");
    var ids = grid.control.get_selectedIds();
    var selectedDependentId = ids[0];

    window.open("http://uii/Global Manager/CopyToContext?crmeDependentId=" + selectedDependentId);
    //window.open("http://event/?eventName=SelectedCrmeDependent&crmeDependentId=" + selectedDependentId);
    window.open("http://event/?eventName=SelectedCrmeDependent");
}

function bpfStatus() {
    var bpfStatus = _formContext.data.process.getStatus();

    if (bpfStatus !== "active") {
        var msg = "Dependent(s) have already been submitted once.  Business Process Flow is set to '" + bpfStatus + "'. Skipping submit.";
        var title = "Skipping Submit";
        UDO.Shared.openAlertDialog(msg, title, 300, 450);
        console.log(msg);
    }

    return bpfStatus;
}

function submitTransaction_USD(formContext) {
    _formContext = formContext;

    submitTransaction();
}

function ShowDependentValidationAlert(msg, title) {
    UDO.Shared.openAlertDialog(msg, title, 200, 400).then(
        function success(result) {
            console.log("Alert dialog closed");
        },
        function (error) {
            console.log(error.message);
        }
    );
}
function ValidateDependentSpouseMarriageMethod() {
    console.log("Inside ValidateDependentMarriageMethod Method");
    var id = _formContext.data.entity.getId().replace("{", "").replace("}", "");
    var query = "?$select=udo_marriagemethod&$filter=_crme_dependentmaintenance_value eq " + id + " and (udo_marriagemethod ne 752280000) and (crme_maintenancetype ne 935950002) and (crme_dependentrelationship eq 935950001)";// relationship type marriage
    return new Promise(function (resolve, reject) {
        Xrm.WebApi.retrieveMultipleRecords("crme_dependent", query).then(
            function (result) {
                var response = !(result.entities.length > 0);
                //if results.entities.length > 0, invalid dependents
                resolve(response);
            },
            function (error) {
                console.log(error.message);
                ShowDependentValidationAlert("Error on reading marriage method of dependent(s).", "Submit Error.");
                reject(error);
            }
        );

    });

}

function ValidateDependentChildRelationship() {
    console.log("Inside ValidateDependentChildRelationship Method");
    var id = _formContext.data.entity.getId().replace("{", "").replace("}", "");
    var query = "?$select=crme_dependentrelationship,crme_childrelationship,crme_childage1823inschool&$filter=_crme_dependentmaintenance_value eq " + id +
        " and (crme_maintenancetype ne 935950002)" +
        " and (crme_dependentrelationship eq 935950000)"; // Dependent Child
    return new Promise(function (resolve, reject) {
        Xrm.WebApi.retrieveMultipleRecords("crme_dependent", query).then(
            function (data) {
                if (data.entities.length > 0) {
                    console.log("Found " + data.entities.length + " dependent(s) to Add.");
                    var childRelationshipInvalid = false;
                    data.entities.forEach(function (dep) {
                        var childRelationship = dep.crme_childrelationship;
                        var childage1823inschool = dep.crme_childage1823inschool;
                        // childRelationship = Adoption & child age 18-23 and in school = false
                        if (childRelationship == 935950002 && childage1823inschool == false) {
                            childRelationshipInvalid = true;
                        }
                    });

                    resolve(!childRelationshipInvalid);
                }
                else {
                    console.log("No entities found in ValidateDependentChildRelationship function.");
                    resolve(true);
                }
            },
            function (error) {
                console.log(error.message);
                ShowDependentValidationAlert("Error on reading child relationship of child dependent(s).", title);
                reject(error);
            }
        );
    });
}

function DependedentMaintenanceSubmittedToday() {
    console.log("Inside DependedentMaintenanceSubmittedToday");
    var id = _formContext.data.entity.getId().replace("{", "").replace("}", "");
    var veteran = _formContext.getAttribute("regardingobjectid").getValue()[0].id.replace("{", "").replace("}", "");
    var query = "?$select=createdon&$filter=_regardingobjectid_value eq " + veteran + " and ((crme_txnstatus eq 935950002) or (crme_txnstatus eq 935950001) or (crme_txnstatus eq 935950003)) and Microsoft.Dynamics.CRM.Today(PropertyName=@p1)&@p1='createdon'";
    return new Promise(function (resolve, reject) {
        Xrm.WebApi.retrieveMultipleRecords("crme_dependentmaintenance", query).then(
            function (result) {
                if (result.entities.length > 0) {
                    var msg = "Dependent Maintenance record for this veteran has already been submitted today.";
                    var title = "Dependent Maintenance Submitted";
                    UDO.Shared.openAlertDialog(msg, title, 300, 450);
                }
                var response = (result.entities.length > 0);
                resolve(response);
            },
            function (error) {
                console.log(error.message);
                reject(error);
            }
        );

    });
}

function ValidatePensionSubmission() {
    return new Promise(function (resolve, reject) {
        console.log("Checking Valid Pension Submission..........");
        var dep_main_id = _formContext.data.entity.getId().replace("{", "").replace("}", "");
        var awardType = _formContext.getAttribute("udo_awardtype").getValue();
        Xrm.WebApi.online.retrieveRecord("crme_dependentmaintenance", dep_main_id, "?$select=activityid,_regardingobjectid_value").then(
            function success(depmaintenance) {
                if (depmaintenance != null) {
                    var vetid = depmaintenance["_regardingobjectid_value"];
                    var vetsnapShotQuery = "?$select=createdon,modifiedon,udo_effectivedate,udo_sccombinedrating,_udo_veteranid_value&$filter=_udo_veteranid_value eq " + vetid + "&$orderby=createdon desc&$top=1";
                    Xrm.WebApi.online.retrieveMultipleRecords("udo_veteransnapshot", vetsnapShotQuery).then(
                        function success(results) {
                            if (results != null && results.entities.length > 0 && results.entities[0] != null) {
                                var lastSnapshot = results.entities[0];
                                console.log("Last Snapshot is found: ");
                                console.log(lastSnapshot);

                                var SC_rating = Number(lastSnapshot["udo_sccombinedrating"]);
                                if (SC_rating >= 30) {//allow submission regardless of award type
                                    resolve(true);
                                }
                                else if (SC_rating < 30 && awardType === 751880001) {//compensation
                                    resolve(false);
                                } else if (SC_rating < 30 && awardType === 751880000) {//pension
                                    resolve(true);
                                }

                            } else {
                                resolve(false);
                            }
                        },
                        function (error) {
                            console.log(error.message);
                            resolve(false);
                        });

                } else {
                    resolve(false);
                }
            },
            function (error) {
                console.log("Error on reading dependent maintenance type of related veteran" + error.message);
                console.log(error.message);
                reject(error);
            }
        );
    });
}

function ValidateForeignMarriageBirth() {
    console.log("Checking Invalid Dependedent record...Begin");
    var id = _formContext.data.entity.getId().replace("{", "").replace("}", "");
    var childquery = "?$filter=_crme_dependentmaintenance_value eq " + id;
    childquery += " and ((crme_childplaceofbirthcountryid/crme_country ne null) and (crme_childplaceofbirthcountryid/crme_country ne 'USA'))";//country other than USA
    childquery += " and (crme_maintenancetype eq 935950000) and (crme_dependentrelationship eq 935950000)";//ADD  and child

    var marriageQuery = "?$filter=_crme_dependentmaintenance_value eq " + id;
    marriageQuery += " and((crme_marriagecountryid/crme_country ne null) and (crme_marriagecountryid/crme_country ne 'USA')) ";//country other than USA
    marriageQuery += " and (crme_maintenancetype eq 935950000) and (crme_dependentrelationship eq 935950001)";//ADD  and spouse

    return new Promise(function (resolve, reject) {
        Xrm.WebApi.retrieveMultipleRecords("crme_dependent", childquery).then(
            function (childResult) {
                if (childResult.entities.length > 0) {
                    console.log("Invalid child birth country records present, returning false");
                    resolve(false);//invalid child birth country,no need to further check
                } else {
                    Xrm.WebApi.retrieveMultipleRecords("crme_dependent", marriageQuery).then(
                        function (spouseresult) {
                            if (spouseresult.entities.length > 0) {
                                console.log("Invalid marriage country records present, returning false");
                                resolve(false);////invalid spouse marriage country
                            } else {
                                console.log("Valid records present, returning true");
                                ///check spouse marriage history

                                var fetchData = {
                                    "crme_dependentrelationship": "935950001",//spouse
                                    "crme_maintenancetype": "935950000",//add
                                    "activityid": id
                                };
                                var fetchXml = [
                                    "?fetchXml=<fetch>",
                                    "  <entity name='crme_spousemaritalhistory'>",
                                    "    <attribute name='crme_marriagestartcountryid'/>",
                                    "    <attribute name='crme_marriageendcountryid'/>",
                                    "    <attribute name='udo_dependent'/>",
                                    "    <link-entity name='crme_dependent' from='crme_dependentid' to='udo_dependent' link-type='inner'>",
                                    "      <filter>",
                                    "        <condition attribute='crme_dependentrelationship' operator='eq' value='", fetchData.crme_dependentrelationship, "'/>",
                                    "        <condition attribute='crme_maintenancetype' operator='eq' value='", fetchData.crme_maintenancetype, "'/>",
                                    "      </filter>",
                                    "      <link-entity name='crme_dependentmaintenance' from='activityid' to='crme_dependentmaintenance' link-type='inner'>",
                                    "        <filter>",
                                    "          <condition attribute='activityid' operator='eq' value='", fetchData.activityid, "'/>",
                                    "        </filter>",
                                    "      </link-entity>",
                                    "    </link-entity>",
                                    "  </entity>",
                                    "</fetch>"
                                ].join("");

                                Xrm.WebApi.retrieveMultipleRecords("crme_spousemaritalhistory", fetchXml)
                                    .then(function (results) {
                                        if (results === null || results.entities === null || results.entities.length === 0) {
                                            resolve(true);//no spouse history avaialble no further check is required
                                        } else {
                                            //check if the marriage start and end country is USA
                                            var validSpouseHistory = true;
                                            results.entities.forEach(function (sp) {
                                                console.log(sp);
                                                var marriageStartCountry = sp["_crme_marriagestartcountryid_value@OData.Community.Display.V1.FormattedValue"];
                                                var marriageEndCountry = sp["_crme_marriageendcountryid_value@OData.Community.Display.V1.FormattedValue"];
                                                if (marriageStartCountry !== "USA" || marriageEndCountry !== "USA") {
                                                    validSpouseHistory = false;
                                                }
                                            });

                                            if (validSpouseHistory == false) {
                                                resolve(false);
                                            } else {
                                                resolve(true);
                                            }

                                        }
                                    }, function (error) {
                                        console.log("Error on reading spouse marriage history" + error.message);
                                        ShowDependentValidationAlert("Error on reading spouse marriage history of dependent(s).", "Submit Error.");
                                        reject(error);
                                    });

                            }//else ends
                        }, function (spouserror) {
                            console.log("Error on reading marriage country of spouse dependents" + spouserror.message);
                            ShowDependentValidationAlert("Error on reading marriage country of dependent(s).", "Submit Error.");
                            reject(spouserror);
                        }
                    );
                }
            },
            function (childerror) {
                console.log("Error on reading birth country of child dependents" + childerror.message);
                console.log(childerror.message);
                ShowDependentValidationAlert("Error on reading birth country of child dependent(s).", "Submit Error.");
                reject(childerror);
            }
        );

    });

}

function getRelatedVeteranAddress() {
    var dep_main_id = _formContext.data.entity.getId().replace("{", "").replace("}", "");
    return new Promise(function (resolve, reject) {
        Xrm.WebApi.online.retrieveRecord("crme_dependentmaintenance", dep_main_id, "?$select=activityid,_regardingobjectid_value").then(
            function success(depmaintenance) {
                if (depmaintenance != null) {
                    var vetid = depmaintenance["_regardingobjectid_value"];
                    var vetsnapShotQuery = "?$select=createdon,modifiedon,udo_effectivedate,udo_mailingcountry,_udo_veteranid_value&$filter=_udo_veteranid_value eq " + vetid + "&$orderby=createdon desc&$top=1";
                    Xrm.WebApi.online.retrieveMultipleRecords("udo_veteransnapshot", vetsnapShotQuery).then(
                        function success(results) {
                            if (results != null && results.entities.length > 0 && results.entities[0] != null) {
                                var lastSnapshot = results.entities[0];
                                console.log("Last Snapshot is found: ");
                                console.log(lastSnapshot);
                                if (lastSnapshot["udo_mailingcountry"] == "USA") {
                                    resolve(true);
                                } else {
                                    resolve(false);
                                }
                            } else {
                                resolve(false);
                            }
                        },
                        function (error) {
                            ShowDependentValidationAlert("Error on reading mailing address type and country of related veteran.", "Submit Error.");
                            resolve(false);
                        });

                } else {
                    resolve(false);
                }
            },
            function (error) {
                console.log("Error on reading dependent maintenance type of related veteran" + error.message);
                console.log(error.message);
                ShowDependentValidationAlert("Error on reading mailing address type and country of related veteran.", "Submit Error.");
                reject(error);
            }
        );

    });


}

function submitTransaction() {
    DependedentMaintenanceSubmittedToday()
        .then(function success(result) {
            if (result == false) {
                if (bpfStatus() === "active") {
                    _formContext.ui.clearFormNotification("SUBMITTRANSACTIONRESULT");

                    ValidatePensionSubmission().then(
                        function (result) {
                            if (result == false) {
                                console.log("INVALID SC Rating, exit submitting dependents...");
                                var scTitle = "Veteran's Rating is Below 30%";
                                var scMessage = "\n\r" + "This Veteran's compensation rating percentage is below 30%, and the Dependent Maintenance cannot be submitted.";
                                ShowDependentValidationAlert(scMessage, scTitle);
                            } else {
                                ValidateDependentSpouseMarriageMethod()
                                    .then(function success(result) {
                                        if (result == false) {
                                            console.log("INVALID DEPENDENTS, exit submitting dependents...");
                                            var marriageMethodTitle = "Invalid Marriage Method";
                                            var marriageMethodMsg = "\n\r" + "Only marriage type religious/civil ceremony are eligible for Dependent Maintenance submission. Veteran must submit the 21-686C according to Dependency Issues - Procedural Changes and FAQs.";
                                            ShowDependentValidationAlert(marriageMethodMsg, marriageMethodTitle);
                                            return;
                                        } else {
                                            //check foreign birth or marriage country
                                            ValidateForeignMarriageBirth()
                                                .then(function success(result) {
                                                    if (result == false) {
                                                        console.log("INVALID DEPENDENTS, exit submitting dependents...");
                                                        var foreignBirthTitle = "Invalid Marriage or Birth Country";
                                                        var foreignBirthMsg = "\n\r" + "The dependent's marriage or birth countries must be USA to be eligible for Dependent Maintenance submission. You must export the correct forms and route according to Dependency Issues - Procedural Changes and FAQs.";
                                                        ShowDependentValidationAlert(foreignBirthMsg, foreignBirthTitle);
                                                        return;
                                                    } else {
                                                        getRelatedVeteranAddress()
                                                            .then(function success(result) {
                                                                if (result == false) {
                                                                    console.log("INVALID DEPENDENTS, exit submitting dependents...");
                                                                    var caddTitle = "Invalid Veteran Address";
                                                                    var caddMsg = "\n\r" + "The Veteran's mailing address must have a country of USA to be eligible for Dependent Maintenance submission. You must export the correct forms and route according to Dependency Issues - Procedural Changes and FAQs.";
                                                                    ShowDependentValidationAlert(caddMsg, caddTitle);
                                                                    return;
                                                                } else {
                                                                    ValidateDependentChildRelationship()
                                                                        .then(function success(result) {
                                                                            if (result == false) {
                                                                                console.log("INVALID CHILD DEPENDENT, exit submitting dependents...");
                                                                                var childTitle = "Invalid Dependent Child Relationship";
                                                                                var childMsg = "Adopted children must be submitted by the Veteran/Beneficiary with adoption decree according to Dependency Issues - Procedural Changes and FAQs.";
                                                                                ShowDependentValidationAlert(childMsg, childTitle);
                                                                                return;
                                                                            }
                                                                            else {
                                                                                var id = _formContext.data.entity.getId().replace("{", "").replace("}", "");

                                                                                var filter = "?$select=crme_dependentid,crme_name,crme_dob&$filter=_crme_dependentmaintenance_value eq " + id +
                                                                                    " and ((crme_maintenancetype eq 935950000) or (crme_maintenancetype eq 935950001) or (crme_maintenancetype eq 752280000))";

                                                                                Xrm.WebApi.retrieveMultipleRecords("crme_dependent", filter)
                                                                                    .then(function (data) {
                                                                                        if (data.entities.length > 0) {
                                                                                            console.log("Found " + data.entities.length + " dependent(s) to Add.");

                                                                                            var missingDOB = false;
                                                                                            data.entities.forEach(function (dep) {
                                                                                                if (dep.crme_dob === null) {
                                                                                                    var msg = "Data of Birth is missing for Dependent '" + dep.crme_name + "'. Submit cancelled.";
                                                                                                    var title = "Submit Error";
                                                                                                    UDO.Shared.openAlertDialog(msg, title, 250, 500).then(
                                                                                                        function success(result) {
                                                                                                        },
                                                                                                        function (error) {
                                                                                                            console.log(error.message);
                                                                                                        }
                                                                                                    );

                                                                                                    missingDOB = true;
                                                                                                }
                                                                                            });

                                                                                            if (!missingDOB) {
                                                                                                continueSubmitting();
                                                                                            }
                                                                                        } else {
                                                                                            console.log("No Dependent record found to Add.");

                                                                                            var msg = "Add at least one dependent before submitting.";
                                                                                            var title = "No Dependent(s) Added";
                                                                                            UDO.Shared.openAlertDialog(msg, title, 300, 450).then(
                                                                                                function success(result) {
                                                                                                },
                                                                                                function (error) {
                                                                                                    console.log(error.message);
                                                                                                }
                                                                                            );

                                                                                            // CSDev Reset Marital Status to Required
                                                                                            maritalstatus.setRequiredLevel('required');

                                                                                            return;
                                                                                        }
                                                                                    })
                                                                                    .catch(function (err) {
                                                                                        console.log("Error retrieving Dependent records to Add: " + err);

                                                                                        var msg = "Error submitting dependent(s).";
                                                                                        var title = "Submit Error";
                                                                                        UDO.Shared.openAlertDialog(msg, title, 300, 450).then(
                                                                                            function success(result) {
                                                                                            },
                                                                                            function (error) {
                                                                                                console.log(error.message);
                                                                                            }
                                                                                        );
                                                                                    });

                                                                            }
                                                                        }
                                                                        ); // ValidateDependentChildRelationship
                                                                }
                                                            });//validate CADD address type
                                                    }
                                                });//validate foreign birth or marriage
                                        }
                                    });//validate marriage method       
                            }
                        });//validate pension                                     
                }//bpf status
            }//submittedToday
        });
}

function continueSubmitting() {
    // Turn off the following fields requirement check
    var maritalstatus = _formContext.getAttribute('crme_maritalstatus');
    if (maritalstatus !== null) {
        maritalstatus.setRequiredLevel('none');
    }

    // Check if marital status not set
    if (maritalstatus !== null &&
        maritalstatus.getValue() === null) {
        var msg = "Marital status is not specified.";
        var title = "Missing Marital Status";
        UDO.Shared.openAlertDialog(msg, title, 300, 450).then(
            function success(result) {
            },
            function (error) {
                console.log(error.message);
            }
        );

        maritalstatus.setRequiredLevel('required');
        return;
    }

    //  check if form is NOT in DRAFT mode
    var txnStatus = _formContext.getAttribute('crme_txnstatus').getValue();
    if (txnStatus !== 935950000) {
        _formContext.getAttribute("udo_actiontaken").setValue(true);
        _formContext.ui.clearFormNotification("NODMACTIONTAKENYET");
        closeDependentMaintenance();
        return;
    }

    // This will check for Address Line 1 character length greater than 20
    var addr1Att = _formContext.getAttribute("crme_address1");
    if (addr1Att !== null) {
        if (addr1Att.getValue() !== null) {
            var addr1 = addr1Att.getValue();

            if (addr1.length > 20) {
                var msg = "\n\r" + "The Veteran Address 1 length exceeds 20 characters in length. To Submit, complete steps below :" + "\n\r" + "1. Close Dep. Maintenance Tab" + "\n\r" + "2. Complete CADD to move address characters to address line 2" + "\n\r" + "3. Navigate back to Dep. Maintenance";
                var title = "Unable to Submit";
                UDO.Shared.openAlertDialog(msg, title, 300, 600).then(
                    function success(result) {
                        console.log("Alert dialog closed");
                    },
                    function (error) {
                        console.log(error.message);
                    }
                );

                return false;
            }
        }
    }

    //Validate SSN for all dependents is stamped.

    ActionvalidateSSN()
        .then(function (result) {
            if (result.ok) {
                result.json().then(function (response) {
                    var validationResponse = response.validationResponse;
                    if (validationResponse === true) // This means Validation passed.
                    {
                        Xrm.Utility.showProgressIndicator("Submitting Dependents... Please wait");
                        _formContext.getAttribute("udo_actiontaken").setValue(true);
                        _formContext.getAttribute('crme_txnstatus').setValue(935950001); // Submitted

                        _formContext.data.save().then(function () {
                            // Progress to the last BPF step
                            _formContext.data.process.moveNext(function (returnValue) {
                                console.log("Business Process Flow moveNext() call to move to last step returned: " + returnValue);

                                // Finalize BPF
                                _formContext.data.process.setStatus("finished", function (returnValue) {
                                    console.log("Business Process Flow status returned: " + returnValue);

                                    CreateSummaryNote();
                                });
                            });
                        },
                            function (err) {
                                console.log("Error saving form: " + err);
                                Xrm.Utility.closeProgressIndicator();

                                rebuildDMButtons();
                            });
                    }
                    else {
                        var msg = "\n\r" + "One or more new dependent additions has a blank SSN. Please update this information by selecting previous or export form and submit manually.";
                        var title = "Unable to Submit";
                        UDO.Shared.openAlertDialog(msg, title, 300, 600).then(
                            function success(result) {
                                console.log("Alert dialog closed");
                            },
                            function (error) {
                                console.log(error.message);
                            }
                        );

                        return;
                    }
                });
            }
        }, function (error) {
            var msg = "\n\r" + "Unable to Validate Dependent SSN. Please try again after sometime.";
            var title = "Unable to Validate Dependent SSN";
            UDO.Shared.openAlertDialog(msg, title, 300, 600).then(
                function success(result) {
                    console.log("Alert dialog closed");
                },
                function (error) {
                    console.log(error.message);
                }
            );

            return;
        });
}

function ActionvalidateSSN() {
    var parameters = {};
    var entity = {};

    entity.id = _formContext.data.entity.getId().replace("{", "").replace("}", "");
    entity.entityType = "crme_dependentmaintenance";
    parameters.entity = entity;


    var udo_validateSSNRequest = {
        ParentEntityReference: parameters.entity,

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
                operationName: "udo_ValidateSSN"
            };
        }
    };
    return Xrm.WebApi.online.execute(udo_validateSSNRequest);

}

function CreateSummaryNote() {
    var parameters = {};
    var entity = {};
    entity.id = _formContext.data.entity.getId().replace("{", "").replace("}", "");
    entity.entityType = "crme_dependentmaintenance";
    parameters.entity = entity;

    var udo_CreateNoteRequest = {

        ParentEntityReference: parameters.entity,

        getMetadata: function () {
            return {

                parameterTypes: {
                    "ParentEntityReference": {
                        "typeName": "mscrm.crmbaseentity",
                        "structuralProperty": 5
                    }
                },
                operationType: 0,
                operationName: "udo_CreateNote"
            };
        }
    };

    _formContext.getAttribute("udo_actiontaken").setValue(true);
    _formContext.ui.clearFormNotification("NODMACTIONTAKENYET");

    Xrm.WebApi.online.execute(udo_CreateNoteRequest).then(function (response) {
        if (response.ok) {
            response.json().then(
                function (data) {
                    var noteID = data.NoteID || "";
                    var noteContent = data.NoteContent || "";
                    var respMsg = data.ResponseMessage || "";

                    Xrm.Utility.closeProgressIndicator();

                    if (noteID === "") {
                        console.log("Error creating maintenance note: " + respMsg);

                        var msg = "\nYour Dependent Maintenance transaction was submitted.\n\nDependent System has failed to create Maintenance Note.";
                        var title = "Unable to create Note";
                        UDO.Shared.openAlertDialog(msg, title, 200, 500).then(
                            function success(result) {
                            },
                            function (error) {
                            }
                        );
                    }
                    else {
                        var msg = "\nYour Dependent Maintenance transaction was submitted.\n\nDependent Maintenance note has been created with the \nfollowing content: \n\n" + noteContent;
                        var title = "Note Created.";
                        UDO.Shared.openAlertDialog(msg, title, 300, 500).then(
                            function success(result) {
                            },
                            function (error) {
                            }
                        );
                    }

                    rebuildDMButtons();
                });
        }
        else {
            Xrm.Utility.closeProgressIndicator();

            var msg = "\nYour Dependent Maintenance transaction was submitted.\n\nDependent System did not respond to create Maintenance Note.";
            var title = "Create Note response bad";
            UDO.Shared.openAlertDialog(msg, title, 200, 500).then(
                function success(result) {
                },
                function (error) {
                }
            );

            rebuildDMButtons();
        }
    });
}

function cancelTransaction_USD(formContext) {
    _formContext = formContext;

    cancelTransaction();
}

function cancelTransaction() {
    _formContext.getAttribute("udo_actiontaken").setValue(true);
    _formContext.ui.clearFormNotification("NODMACTIONTAKENYET");

    // Check if form is NOT in DRAFT mode
    var txnStatus = _formContext.getAttribute('crme_txnstatus').getValue();
    if (txnStatus !== 935950000) {
        closeDependentMaintenance();
        return;
    }

    // Turn off the following fields requirement check
    var maritalstatus = _formContext.getAttribute('crme_maritalstatus');
    if (maritalstatus !== null) {
        maritalstatus.setRequiredLevel('none');
    }

    var id = _formContext.data.entity.getId().replace("{", "").replace("}", "")

    var filter = "?$select=crme_dependentid&$filter=_crme_dependentmaintenance_value eq " + id + " and crme_maintenancetype eq 935950000"; // Add

    Xrm.WebApi.retrieveMultipleRecords("crme_dependent", filter)
        .then(function (data) {
            if (data.entities.length > 0) {
                console.log("Found " + data.entities.length + " dependent(s) that have been Added.");

                var response = confirm('Data was changed. Click on Cancel to stay on this form or click on OK to delete this transaction.');
                if (response === true) {
                    _formContext.getAttribute('crme_txnstatus').setValue(935950004);
                    _formContext.data.save().then(function () {
                        closeDependentMaintenance();
                    },
                        function (err) {
                            console.log("Error saving form: " + err);
                        });
                }
            } else {
                console.log("No Dependent records added.  Continue with cancel.");

                closeDependentMaintenance();
            }
        })
        .catch(function (err) {
            console.log("Error retrieving Dependent records that have been Added: " + err);

            var msg = "Error submitting dependent(s).";
            var title = "Submission Error";
            UDO.Shared.openAlertDialog(msg, title, 300, 500).then(
                function success(result) {
                },
                function (error) {
                }
            );
        });

    // Set these fields as required again
    if (maritalstatus !== null) {
        maritalstatus.setRequiredLevel('required');
    }
}

function closeDependentMaintenance() {
    var dmActionTaken = _formContext.getAttribute("udo_actiontaken").getValue();

    if (!dmActionTaken) {
        console.log("Cannot close Dependent Maintenance");
        _formContext.ui.setFormNotification("Cannot close, no action [Download 686c, Download 674, or Submit] has been taken yet on Dependent Maintenance transaction.", "WARNING", "NODMACTIONTAKENYET");
    }
    else {
        console.log("Calling USD event to close Dependent Maintenance");

        _formContext.ui.clearFormNotification("NODMACTIONTAKENYET");

        window.open("http://event/?eventname=CloseDependentMaintenanceEvent");
    }
}

function isDirty() {
    ret = false;

    var id = _formContext.data.entity.getId();
    var columns = ['crme_dependentId'];
    var filter = "crme_DependentMaintenance/Id eq guid'" + id + "'  and crme_MaintenanceType/Value eq 935950000";
    var collection = CrmRestKit.ByQueryAll('crme_dependent', columns, filter, false)
        .done(function (data, status, xhr) {
            if (data.length > 0) {
                ret = true;
            }

        });

    return ret;
}

function rebuildDMButtons() {
    setTimeout(function () {
        window.open("http://event/?eventName=BuildDependentMaintenanceButtons");
    }, 500);
}

function download686cPDF_USD(formContext) {
    var param = {
        "data": {
            "action": "download",
            "formatType": "PDF",
            "docType": "686c"
        }
    };

    DownloadDocGen(formContext, param);
}

function download686cWord_USD(formContext) {
    var param = {
        "data": {
            "action": "download",
            "formatType": "Word",
            "docType": "686c"
        }
    };

    DownloadDocGen(formContext, param);
}

function Download686c(executionContext, param) {
    DownloadDocGen(executionContext.getFormContext(), param);
}

function download674PDF_USD(formContext) {
    var param = {
        "data": {
            "action": "download",
            "formatType": "PDF",
            "docType": "674"
        }
    };

    DownloadDocGen(formContext, param);
}

function download674Word_USD(formContext) {
    var param = {
        "data": {
            "action": "download",
            "formatType": "Word",
            "docType": "674"
        }
    };

    DownloadDocGen(formContext, param);
}

function Download674(executionContext, param) {
    DownloadDocGen(executionContext.getFormContext(), param);
}

function DownloadDocGen(formContext, param) {
    _formContext = formContext;

    // Set defaults, formatType PDF and action to open
    var formatType = (param.data.formatType !== null && typeof param.data.formatType !== "undefined") ? param.data.formatType : "PDF";
    var action = (param.data.action !== null && typeof param.data.action !== "undefined") ? param.data.action : "open";
    var docType = (param.data.docType !== null && typeof param.data.docType !== "undefined") ? param.data.docType : "";
    var docTemplate = "";

    action = action.toLowerCase();

    // Check to make sure form is saved before continuing.
    if (_formContext.ui.getFormType() === CRM_FORM_TYPE_CREATE) {
        var msg = "You have to save the form before proceeding.";
        var title = "Save First";
        UDO.Shared.openAlertDialog(msg, title, 300, 500).then(
            function success(result) {
                return;
            },
            function (error) {
                return;
            }
        );
    }
    else if (_formContext.ui.getFormType() === CRM_FORM_TYPE_UPDATE) {

        if (_formContext.getAttribute('crme_maritalstatus').getValue() === null) {
            var msg = "Enter the required details on the form and save before proceeding.";
            var title = "Marital Status Not Set";
            UDO.Shared.openAlertDialog(msg, title, 300, 500).then(
                function success(result) {
                    return;
                },
                function (error) {
                    return;
                }
            );
        }
    }

    _formContext.ui.clearFormNotification("NODMACTIONTAKENYET");
    _formContext.getAttribute("udo_actiontaken").setValue(true);

    _formContext.data.save().then(
        function Success() {
            // Generate Document
            Xrm.Utility.showProgressIndicator("Generating Form(s) " + docType + " ... Please wait");
            var dmId = _formContext.data.entity.getId().replace("{", "").replace("}", "");

            if (docType === "686c") {
                retrieveActiveSettings("udo_cutoffdate686c",
                    function (data) {
                        var cutoffDate = data;
                        var dmDate = _formContext.getAttribute("createdon").getValue();
                        var cutoffDate = (cutoffDate.udo_cutoffdate686c) ? (new Date(cutoffDate.udo_cutoffdate686c)) : null;

                        if (dmDate >= cutoffDate) {
                            docTemplate = "686 Add Dependent Form";
                        }
                        else {
                            docTemplate = "686 Add Dependent Form - Pre 620";
                        }

                        var entity = {};
                        entity.id = dmId;
                        entity.entityType = "crme_dependentmaintenance";
                        var vetFirstName = _formContext.getAttribute("crme_firstname").getValue();
                        var vetLastName = _formContext.getAttribute("crme_lastname").getValue();

                        // Execute the CRM Action that invokes the call to the DocGen EC
                        ExecuteGenerateDocGenAction(true, formatType, docTemplate, "udo_GenerateForm686c", entity, entity, vetFirstName + " " + vetLastName);
                    },
                    function (err) {
                        Xrm.Utility.closeProgressIndicator();
                        _formContext.ui.setFormNotification("An unexpected error occurred during document download.", "ERROR", "DOCUMENTGENERATIONERROR");
                        console.log(err);
                    });
            }
            else if (docType === "674") {
                docTemplate = "674 School Aged Child";

                // School Age Child and maintenance type either add or edit
                var fetchXml = "?fetchXml=<fetch>" +
                    "<entity name='crme_dependent'>" +
                    "<attribute name='crme_name'/>" +
                    "<order attribute='crme_dependentrelationship' descending='true'/>" +
                    "<filter type='or'>" +
                    "<filter type='and'>" +
                    "<condition attribute='crme_dependentrelationship' operator='eq' value='935950000'/>" +
                    "<condition attribute='crme_maintenancetype' operator='ne' value='935950002'/>" +
                    "<condition attribute='crme_childage1823inschool' operator='eq' value='1'/>" +
                    "</filter>" +
                    "<filter type='and'>" +
                    "<condition attribute='crme_dependentrelationship' operator='eq' value='935950000'/>" +
                    "<condition attribute='crme_maintenancetype' operator='ne' value='752280000'/>" +
                    "<condition attribute='crme_childage1823inschool' operator='eq' value='1'/>" +
                    "</filter>" +
                    "</filter>" +
                    "<attribute name='crme_dependentrelationship'/>" +
                    "<attribute name='crme_dependentid'/>" +
                    "<link-entity name='crme_dependentmaintenance' from='activityid' to='crme_dependentmaintenance' alias='dm'>" +
                    "<filter type='and'>" +
                    "<condition attribute='activityid' operator='eq' uitype='crme_dependentmaintenance' value='" + dmId + "'/>" +
                    "</filter>" +
                    "</link-entity>" +
                    "</entity>" +
                    "</fetch > ";

                Xrm.WebApi.retrieveMultipleRecords("crme_dependent", fetchXml)
                    .then(function (data) {
                        if (data === null || data.entities === null || data.entities.length === 0) {
                            Xrm.Utility.closeProgressIndicator();
                            var msg = "There are no school age child dependents to generate 674 form.";
                            var title = "Missing Child Dependent(s)";
                            UDO.Shared.openAlertDialog(msg, title, 300, 500).then(
                                function success(result) {
                                    return;
                                },
                                function (error) {
                                    return;
                                }
                            );
                        } else {
                            var parentEntity = {};
                            parentEntity.id = dmId;
                            parentEntity.entityType = "crme_dependentmaintenance";

                            var dependentCnt = data.entities.length;
                            for (var i = 0; i < dependentCnt; i++) {
                                var entity = {};
                                entity.id = data.entities[i]["crme_dependentid"];
                                entity.entityType = "crme_dependent";
                                var filenameSuffix = data.entities[i]["crme_name"];

                                // Execute the CRM Action for each dependent 
                                //   to invoke the call calling DocGen EC
                                ExecuteGenerateDocGenAction(false, formatType, docTemplate, "udo_GenerateForm674", entity, parentEntity, filenameSuffix);
                            }
                        }
                    });
            }
            else {
                Xrm.Utility.closeProgressIndicator();

                var msg = "Invalid document type.";
                var title = "Document Type Invalid";
                UDO.Shared.openAlertDialog(msg, title, 300, 500).then(
                    function success(result) {
                    },
                    function (error) {
                    }
                );
            }
        },
        function Error() {
            Xrm.Utility.closeProgressIndicator();

            var msg = "An unexpected error occurred during document download.";
            var title = "Document Download Error";
            UDO.Shared.openAlertDialog(msg, title, 300, 500).then(
                function success(result) {
                },
                function (error) {
                }
            );
        });
}

function ExecuteGenerateDocGenAction(uploadFile, formatType, docTemplate, opName, entity, parentEntity, filenameSuffix) {
    var parameters = {};

    parameters.entity = entity;
    parameters.DocumentTemplate = docTemplate;
    parameters.UploadAttachment = false;
    if (formatType !== "PDF") {
        parameters.ConvertToPdf = false;
    }
    else {
        parameters.ConvertToPdf = true;
    }
    parameters.UploadAttachment = uploadFile;

    var udo_GenerateFormRequest = {
        entity: parameters.entity,
        ParentEntityReference: parentEntity,
        AttachmentEntityReference: parentEntity,
        DocumentTemplate: parameters.DocumentTemplate,
        ConvertToPdf: parameters.ConvertToPdf,
        UploadAttachment: parameters.UploadAttachment,
        FilenameSuffix: filenameSuffix,

        getMetadata: function () {
            return {
                boundParameter: "entity",
                parameterTypes: {
                    "entity": {
                        "typeName": "mscrm." + entity.entityType,
                        "structuralProperty": 5
                    },
                    "ParentEntityReference": {
                        "typeName": "mscrm." + parentEntity.entityType,
                        "structuralProperty": 5
                    },
                    "DocumentTemplate": {
                        "typeName": "Edm.String",
                        "structuralProperty": 1
                    },
                    "AttachmentEntityReference": {
                        "typeName": "mscrm." + parentEntity.entityType,
                        "structuralProperty": 5
                    },
                    "ConvertToPdf": {
                        "typeName": "Edm.Boolean",
                        "structuralProperty": 1
                    },
                    "UploadAttachment": {
                        "typeName": "Edm.Boolean",
                        "structuralProperty": 1
                    },
                    "FilenameSuffix": {
                        "typeName": "Edm.String",
                        "structuralProperty": 1
                    }
                },
                operationType: 0,
                operationName: opName
            };
        }
    };

    console.log("Calling custom action " + opName + "for " + filenameSuffix + " with request object: " + JSON.stringify(udo_GenerateFormRequest));

    Xrm.WebApi.online.execute(udo_GenerateFormRequest).then(
        function (response) {
            Xrm.Utility.closeProgressIndicator();

            if (response.ok) {
                response.json().then(
                    function (data) {
                        var mimeType = data.MimeType || "";
                        var fileContents = data.Base64FileContents || "";
                        var fileName = data.FileName || "";
                        var respMsg = data.ResponseMessage || "Error retrieving document contents for " + filenameSuffix;
                        var attachmentId = data.AttachmentId || "";

                        if (fileContents === null || fileContents.length === 0) {
                            _formContext.ui.setFormNotification("An unexpected error occurred during document download.  Check Associated Documents tab or please wait and try again.", "ERROR", "DOCUMENTGENERATIONERROR");
                            console.error(respMsg);
                            return;
                        }

                        //  Add annotation file here instead of within DocGen custom action above
                        if (!uploadFile) {
                            var annotation = {};
                            annotation["objectid_crme_dependentmaintenance@odata.bind"] = "/crme_dependentmaintenances(" + parentEntity.id.toString() + ")";
                            annotation.objecttypecode = "crme_dependentmaintenance";
                            annotation.subject = docTemplate;
                            annotation.notetext = filenameSuffix;
                            annotation.filename = fileName;
                            annotation.documentbody = fileContents;
                            annotation.mimetype = mimeType;

                            Xrm.WebApi.createRecord("annotation", annotation)
                                .then(function (data) {
                                    console.log("Annotation 674 record " + filenameSuffix + " created for Dependent Maintenance.");
                                },
                                    function (err) {
                                        console.log("Unable to create Annotation 674 record " + filenameSuffix + " for Dependent Maintenance: " + err.message);
                                    });
                        }

                        // Success - download file
                        Va.Udo.Crm.Scripts.Download(fileName, mimeType, fileContents)
                            .done(function () {
                                console.log("Document download complete");
                            });

                        console.log("Generate form request complete for " + filenameSuffix);
                    });
            } else {
                _formContext.ui.setFormNotification("An unexpected error occurred during document download.  Check Associated Documents tab or please wait and try again.", "ERROR", "DOCUMENTGENERATIONERROR");
                console.log("Generate form request complete, but document download failure.");
                return;
            }
        },
        function (error) {
            Xrm.Utility.closeProgressIndicator();

            // Show error notification
            _formContext.ui.setFormNotification("An unexpected error occurred during document generation. Please wait and try again. Exception Details: " + error.message, "ERROR", "DOCUMENTGENERATIONEXCEPTION");

            console.log("Generate form request failed: " + error);
        }
    );
}

function retrieveActiveSettings(setting, successCallback, errorCallback) {
    var id = "Active Settings";

    var filter = "?$select=udo_cutoffdate686c&$filter=mcs_name eq '" + id + "'";

    Xrm.WebApi.retrieveMultipleRecords("mcs_setting", filter)
        .then(function (data) {
            if (data.entities.length > 0) {
                if (successCallback) {
                    successCallback(data.entities[0]);
                }
            }
            else {
                if (errorCallback) {
                    errorCallback("No data retrieved from Active Setting for " + setting);
                }
            }
        })
        .catch(function (err) {
            if (errorCallback) {
                errorCallback("Error retrieving Active Setting record: " + err);
            }
        });
}

function launchDependentVerification(formContext) {
    _formContext = formContext;

    _formContext.ui.clearFormNotification("SERVICEREQUESTEXCEPTION");
    Xrm.Utility.showProgressIndicator("Initializing Service Request... Please wait");

    var parameters = {};
    parameters.NoPayeeDetails = false;

    var parententityreference = {};
    parententityreference.contactid = _formContext.getAttribute("regardingobjectid").getValue()[0].id.replace("{", "").replace("}", "");
    parententityreference["@odata.type"] = "Microsoft.Dynamics.CRM.contact";
    parameters.ParentEntityReference = parententityreference;

    var udo_InitiateServiceRequestRequest = {
        NoPayeeDetails: parameters.NoPayeeDetails,
        ParentEntityReference: parameters.ParentEntityReference,
        RequestType: "Dependent Maintenance",
        RequestSubType: "Dependent Verification",

        getMetadata: function () {
            return {
                boundParameter: null,
                parameterTypes: {
                    "NoPayeeDetails": {
                        "typeName": "Edm.Boolean",
                        "structuralProperty": 1
                    },
                    "ParentEntityReference": {
                        "typeName": "mscrm.crmbaseentity",
                        "structuralProperty": 5
                    },
                    "RequestType": {
                        "typeName": "Edm.String",
                        "structuralProperty": 1
                    },
                    "RequestSubType": {
                        "typeName": "Edm.String",
                        "structuralProperty": 1
                    }
                },
                operationType: 0,
                operationName: "udo_InitiateServiceRequest"
            };
        }
    };

    Xrm.WebApi.online.execute(udo_InitiateServiceRequestRequest)
        .then(function (response) {
            if (response.ok) {
                response.json().then(
                    function (data) {
                        if (!data.Exception) {
                            UDO.Shared.GetCurrentAppProperties().then(
                                function (appProperties) {
                                    var appId = appProperties.appId;
                                    //var url = _globalContext.getClientUrl() + "/main.aspx?appid=" + appId + "&etn=udo_servicerequest&pagetype=entityrecord&id=" + data.result["udo_servicerequestid"];
                                    var url = "http://uii/UdoServiceRequest/Navigate?url=" + _globalContext.getClientUrl() + "/main.aspx?appid=" + appId + "&cmdbar=false&navbar=off&newWindow=true&pagetype=entityrecord&etn=udo_servicerequest&id=" + data.result["udo_servicerequestid"];
                                    window.open(url);
                                    window.open("http://uii/Global Manager/ShowTab?UdoServiceRequest");
                                },
                                function (error) {
                                    console.log(error);
                                });
                        } else {
                            _formContext.ui.setFormNotification("An unexpected error occurred while creating the Service Request. Please wait and try again. Response Error: " + data.ResponseMessage, "ERROR", "SERVICEREQUESTEXCEPTION");
                        }
                    }
                );

                Xrm.Utility.closeProgressIndicator();
            } else {
                _formContext.ui.setFormNotification("An unexpected error occurred while initializing the Service Request. Please wait and try again. Response Error: " + response.ResponseMessage, "ERROR", "SERVICEREQUESTEXCEPTION");

                Xrm.Utility.closeProgressIndicator();
            }
        })
        .catch(function (err) {
            _formContext.ui.setFormNotification("An unexpected error occurred while initializing the Service Request. Please wait and try again. Exception: " + err.message, "ERROR", "SERVICEREQUESTEXCEPTION");

            Xrm.Utility.closeProgressIndicator();
        });
}

function showDependentSubgridDeleteButton() {
    var showButton = true;
    var selectedRows = _formContext.getControl("list_Dependents").getGrid().getSelectedRows();

    selectedRows.forEach(function (row) {
        var attributes = row.getData().getEntity().attributes;

        attributes.forEach(function (attribute) {
            var attributeName = attribute.getName();

            if (attributeName === "crme_maintenancetype") {
                var maintType = attribute.getValue();

                // Do not allow user to use Dependent subgrid delete button if any selection(s) are flagged as "System"
                if (maintType === "935950002") {
                    showButton = false;
                }
            }
        });
    });

    return showButton;
}

function resubmitFailedDMSubmission() {
    _formContext.ui.clearFormNotification("SUBMITTRANSACTIONRESULT");
    _formContext.ui.setFormNotification("Attempting to resubmit DM record...", "INFORMATION", "NODMACTIONTAKENYET");

    var txnStatus = _formContext.getAttribute('crme_txnstatus').getValue();

    // Only allow a Failed transmission status to be resubmitted
    if (txnStatus === 935950002) {
        // Reset BPF to active
        _formContext.data.process.setStatus("active", function (returnValue) {
            console.log("Business Process Flow status set to " + returnValue);

            _formContext.getAttribute('crme_txnstatus').setValue(935950000); // Set to Draft transmission status

            submitTransaction();
        });
    } else {
        _formContext.ui.setFormNotification("Only a Failed DM record can be resubmitted.", "ERROR", "RESUBMITERROR");
    }
}