"use strict";
/// <reference path="va_corpsearch.js" />
var exCon = null;
var formContext = null;
var globCon = null;
//var webApi = null;
var globalContext = Xrm.Utility.getGlobalContext();
var version = globalContext.getVersion();
var lib;
var webApi;
var formHelper;
//window.parent.ExecuteFNOD = ExecuteFNOD;
parent.ExecuteFNOD = ExecuteFNOD;
//window.parent.IssuePMC = IssuePMC;
parent.IssuePMC = IssuePMC;
//window.parent.CopyPMC = CopyPMC;
//window.parent.ExecuteMOD = ExecuteMOD;
//window.parent.ValidateMOD = ValidateMOD;
//window.parent.updateTreasuryFields = updateTreasuryFields;
//window.parent.ValidateMODAddress = ValidateMODAddress;
//window.parent.CopyLastKnownAddressToSpouseFields = CopyLastKnownAddressToSpouseFields;
//window.parent.RetrieveVetInfo = RetrieveVetInfo;
parent.CopyPMC = CopyPMC;
parent.ExecuteMOD = ExecuteMOD;
parent.ValidateMOD = ValidateMOD;
parent.updateTreasuryFields = updateTreasuryFields;
parent.ValidateMODAddress = ValidateMODAddress;
parent.CopyLastKnownAddressToSpouseFields = CopyLastKnownAddressToSpouseFields;
parent.RetrieveVetInfo = RetrieveVetInfo;

function createAndOpenServiceRequest(action) {
    var openServiceRequest = function (servicerequestId) {
        //window.open("../../main.aspx?etn=udo_servicerequest&pagetype=entityrecord&id=" + servicerequestId);
        var clientURL = "http://uii/UdoServiceRequest/Navigate?url=" + globalContext.getClientUrl() + "/main.aspx?cmdbar=false&navbar=off&newWindow=true&pagetype=entityrecord&etn=udo_servicerequest&id=" + servicerequestId;
        window.open(clientURL);
        $('div#tmpDialog').hide();
    };

    var failure = function (section, error) {
        var message = "There was a problem with the " + section + " record.\r\n" +
            error.message;
        Va.Udo.Crm.Scripts.Popup.MsgBox(message, Va.Udo.Crm.Scripts.Popup.PopupStyles.Critical, "Error", { width: 400, height: 165 });
    };

    var serviceRequest = {};

    var SetValue = function (k, v) {
        if (v !== null) serviceRequest[k] = v;
    };
    var NewEntityReference = function (id, logicalname) {
        if (id === null || logicalname === null) return null;
        return { Id: id, LogicalName: logicalname };
    };
    var GetEntityReference = function (lookupName) {
        var lookup = formHelper.getAttribute(lookupName.toLowerCase()).getValue();
        if (lookup === null || lookup.length < 1) return null;
        return NewEntityReference(formHelper.getSelectedLookupIdFormatted(lookupName), lookup[0].entityType);
    };
    var GetFormValue = function (attributeName) {
        var attr = formHelper.getAttribute(attributeName.toLowerCase());
        if (typeof attr === 'undefined' || attr === null) return null;
        return attr.getValue();
    }

    // Set idproof Values
    var idp = GetEntityReference('udo_idproof');
    // console.log(idp);
    SetValue('udo_servicerequestsid', idp);
    //CrmRestKit2011.Retrieve("udo_idproof", idp.Id, ["udo_Interaction"], false)
    webApi.RetrieveRecord(idp.Id, "udo_idproofs", ["_udo_interaction_value"])
        .then(function (idproof) {
            if (idproof["_udo_interaction_value"]) {
                //CrmRestKit2011.Retrieve("udo_interaction", idproof.d.udo_Interaction.Id, ["udo_FirstName", "udo_LastName", "udo_Relationship"], false)
                webApi.RetrieveRecord(idproof["_udo_interaction_value"], "udo_interactions", ["udo_firstname", "udo_lastname", "udo_relationship"])
                    .then(function (interaction) {
                        // console.log(interaction);
                        CreateAndOpenServiceRequestCallback(idproof, interaction);
                    });
            } else {
                CreateAndOpenServiceRequestCallback(idproof, null);
            }
        });

    function CreateAndOpenServiceRequestCallback(idproof, interaction) {
        if (idproof["_udo_interaction_value"]) {
            SetValue('udo_originatinginteractionid', NewEntityReference(idproof["_udo_interaction_value"], 'udo_interaction'));
        } else {
            //throw error?
            return;
        }
        if (interaction !== null) {
            // Set Interaction Values
            SetValue('udo_firstname', interaction.udo_firstname);
            SetValue('udo_lastname', interaction.udo_lastname);
            SetValue('udo_relationtoveteran', interaction.udo_relationship);
            var reporter = interaction.udo_firstname + ' ' + interaction.udo_lastname;
            SetValue('udo_nameofreportingindividual', reporter);
        }

        // Set FNOD Form Value for SR
        SetValue('udo_relatedveteranid', GetEntityReference("va_veterancontactid"));
        SetValue('udo_personid', GetEntityReference("udo_deceasedperson"));
        SetValue('udo_dateofdeath', GetFormValue('va_dateofdeath'));
        SetValue('udo_filenumber', GetFormValue('va_filenumber'));
        SetValue('udo_isveteran', true);
        SetValue('udo_vetfirstname', GetFormValue('va_firstname'));
        SetValue('udo_vetlastname', GetFormValue('va_lastname'));
        SetValue('udo_branchofservice', GetFormValue('va_birlbos'));
        //SetValue('udo_RegionalOfficeId', GetFormValue('va_soj'));

        // Set the Action - Action can be a number (optionsetvalue) or one of the options below:
        switch (action.toString().toLowerCase()) {
            case "820": action = 1; break;
            case "820a": action = 953850001; break;
            case "820d": action = 953850002; break;
            case "820f": action = 953850003; break;
            case "email forms": action = 953850004; break;
            case "letter": action = 953850005; break;
            case "non emergency email": action = 953850006; break;
        }
        var actionVal = parseInt(action.toString());
        if (!isNaN(actionVal)) {
            SetValue('udo_action', { Value: actionVal });
        }
        //Set Veteran Tab Values (if obtainable)
        if (typeof RetrieveVeteranDetailsfromVeteranTab === 'function') {
            var vet = RetrieveVeteranDetailsfromVeteranTab();

            if (vet !== null) {
                SetValue('udo_regionalofficeid', vet.udo_regionalofficeid);
                SetValue('udo_emailofveteran', vet.udo_emailofveteran);
                SetValue('udo_relatedrveteranid', vet.udo_relatedveteranid);
                SetValue('udo_ssn', vet.udo_ssn);
                SetValue('udo_participantid', vet.udo_participantid);
                SetValue('udo_branchofservice', vet.udo_branchofservice);
                SetValue('udo_vetfirstname', vet.udo_vetfirstname);
                SetValue('udo_vetlastname', vet.udo_vetlastname);
                SetValue('udo_vetlastname', vet.udo_vetlastname);
            }
        }
        if (!serviceRequest.udo_participantid) {
            var personlookup = GetEntityReference("udo_deceasedperson");
            //console.log("personLookup");
            //console.log(personlookup);
            //CrmRestKit2011.Retrieve("udo_person", personlookup.Id, ["udo_ptcpntid"], false)
            webApi.RetrieveRecord(personlookup.Id, "udo_persons")
                .then(function (person) {

                    SetValue('udo_participantid', person.udo_ptcpntid);
                    //console.log("serviceRequest");
                    //console.log(serviceRequest);
                    USDCreateLetterOrServiceRequest(
                        serviceRequest['udo_servicerequestsid'].Id,
                        serviceRequest['udo_originatinginteractionid'].Id,
                        serviceRequest['udo_relatedveteranid'].Id,
                        "udo_servicerequest", "SERVICEREQUEST", serviceRequest);
                });
        } else {
            USDCreateLetterOrServiceRequest(
                serviceRequest['udo_servicerequestsid'].id,
                serviceRequest['udo_originatinginteractionid'].id,
                serviceRequest['udo_RelatedVeteranId'].id,
                "udo_servicerequest", "SERVICEREQUEST", serviceRequest);
        }
    }
}

function USDCreateLetterOrServiceRequest(idpid, interactionid, vetid, targetEntity, searchType, data) {
    var updateinfo = {};
    //var dump = "In:\r\n";
    for (var k in data) {
        if (typeof data[k] !== "undefined" && data[k] !== null && data[k] !== "") {
            //console.log(data[k]);
            updateinfo[k] = data[k];


        }
    }


    function error_callback(error) {
        var message = "Error Opening Record: " + error.statuscode + ":" + error.message;
        //Va.Udo.Crm.Scripts.Popup.MsgBox(message, Va.Udo.Crm.Scripts.Popup.PopupStyles.Critical, "Error", { width: 400, height: 165 });
        Xrm.Navigation.openAlertDialog({ text: message }).then(function () {
            console.log("An error occurred in error_callback:");
            console.log(error);
            return;
        });
        //alert("Error Opening Record: " + status +":"+xhr.statusText);
    }

    function assembleUpdateObject(data) {
        var entity = {};
        entity.udo_action = data["udo_action"].Value;
        entity.udo_dateofdeath = new Date(data["udo_dateofdeath"]).toISOString();
        entity.udo_filenumber = data["udo_filenumber"];
        entity.udo_firstname = data["udo_firstname"];
        entity.udo_isveteran = data["udo_isveteran"];
        entity.udo_lastname = data["udo_lastname"];
        entity.udo_nameofreportingindividual = data["udo_nameofreportingindividual"];
        entity["udo_originatinginteractionid@odata.bind"] = "/udo_interactions(" + data["udo_originatinginteractionid"].Id + ")";
        entity.udo_participantid = data["udo_participantid"];
        entity["udo_personid@odata.bind"] = "/udo_persons(" + data["udo_personid"].Id + ")";
        entity["udo_relatedveteranid@odata.bind"] = "/contacts(" + data["udo_relatedveteranid"].Id + ")";
        entity.udo_relationtoveteran = data["udo_relationtoveteran"];
        entity["udo_servicerequestsid@odata.bind"] = "/udo_idproofs(" + data["udo_servicerequestsid"].Id + ")";
        entity.udo_vetfirstname = data["udo_vetfirstname"];
        entity.udo_vetlastname = data["udo_vetlastname"];
        return entity;
    }

    function create_callback(data) {
        //alert("create_callback");
        if (data !== null) {
            var targetId = data[0].crme_personid;
            // console.log("at update");
            var updateEntity = assembleUpdateObject(updateinfo);

            webApi.UpdateRecord(targetId, targetEntity, updateEntity)
                .then(function () {
                    var formOptions = {
                        entityId: targetId,
                        entityName: targetEntity,
                        openInNewWindow: true
                    };
                    Xrm.Navigation.openForm(formOptions);
                }).catch(function (error) { error_callback(error); });
        }
        else {
            //Va.Udo.Crm.Scripts.Popup.MsgBox("No Data Returned", Va.Udo.Crm.Scripts.Popup.PopupStyles.Critical, "Error");
            Xrm.Navigation.openAlertDialog({ text: "No Data Returned" }).then(function () {
                return;
            });
            //alert("No Data Returned.");
        }
    }

    function create() {
        var buildQueryFilter = function (field, value, filter) {
            if (filter.length > 0) {
                return " and " + field + " eq '" + value + "'";
            } else {
                return "$filter=" + field + " eq " + value + "";
            }
        };

        $('div#tmpDialog').show();

        var filter = "";
        var pid = "";
        if (data.udo_participantid) {
            pid = data.udo_participantid;
        } else {
            var pidattr = formHelper.getAttribute("udo_participantid");
            if (pidattr) {
                pid = pidattr.getValue();

            }
        }

        if (typeof pid !== "undefined" && pid !== null) {
            filter += buildQueryFilter("crme_participantid", "'" + pid + "'", filter);
        }

        // Not all search types use these, but some do, so it is easier to include them
        var personattr = formHelper.getAttribute("udo_deceasedperson");
        if (personattr) {
            var personval = personattr.getValue();
            if (personval) {
                var personId = personval[0].id;
                personId = personId.replace("{", "").replace("}", "");
                filter += buildQueryFilter("crme_udopersonguid", personId, filter);
            }
        }

        filter += buildQueryFilter("crme_udoidproofid", idpid, filter);
        filter += buildQueryFilter("crme_udointeractionid", interactionid, filter);
        filter += buildQueryFilter("crme_udoveteranid", vetid, filter);
        filter += buildQueryFilter("crme_searchtype", searchType, filter);
        //CrmRestKit2011.ByQuery("crme_person", ["crme_personId", "crme_SearchType", "crme_url"], filter).done(create_callback).fail(error_callback);
        //console.log("at create retrievemultiple");
        //console.log(filter);
        webApi.RetrieveMultiple("crme_person", ["crme_personid", "crme_searchtype", "crme_url"], filter)
            .then(function (data) {
                //console.log("from retrieveMultiple:");
                //console.log(data);
                create_callback(data);
            })
            .catch(function (error) { error_callback(error); });
    }
    create();
}



var webResourceUrl = null, parent_page = null, page_opener = null,
    spouse_action_options = null, //to save dropdown options in case things get removed and need to be refreshed
    _country_list = [],
    _country_list_USA = 0,
    _doHaveCorpRecord = false,
    _ranFnodThisTime = false,
    _modPrompt = false,
    _redrawing_spouse_action = false;

function initiateCommonScripts(exCon) {
    return new Promise(function (resolve, reject) {
        try {
            //console.log("I am in the right place");
            lib = new CrmCommonJS.CrmCommon(version, exCon);
            webApi = lib.WebApi;
            formHelper = new CrmCommonJS.FormHelper(exCon);
            resolve();
        } catch (e) {
            console.log("An error occurred in initiateCommonScripts: " + e);
            rebuildAllButtonsAsync(execContext);
            reject(e);
        }
    });

}

function getExCon() {
    return exCon;
}

function addbeforeunloadEvent(func) {
    var oldonbeforeunload = window.onbeforeunload;
    if (typeof window.onbeforeunload !== 'function') {
        window.onbeforeunload = func;
    } else {
        window.onbeforeunload = function () {
            oldonbeforeunload();
            func();
        }
    }
}

function ValidateFnodCompletion(executionContext) {
    var formContext = executionContext.getFormContext();

    //console.log("ValidateFnodCompletion");
    var maxRetries = 15,
        vetSearchCtx = new vrmContext(exCon);

    ShowProgress('Validating FNOD Completion (attempt 1 of ' + maxRetries.toString() + ')');

    // IMPORTANT: findGeneralInformationByFileNumber often does not report correct DOD after FNOD, but findGeneralInformationByPtcpntIds does
    // therefore, run findGeneralInformationByPtcpntIds

    if (_UserSettings === null || _UserSettings === undefined) { _UserSettings = GetUserSettingsForWebservice(exCon); }
    vetSearchCtx.user = _UserSettings;
    usefindGeneralInformationByPtcpntIds = false;
    vetSearchCtx.parameters['fileNumber'] = formHelper.getAttribute('va_filenumber').getValue();
    //}
    //}

    var dodResponseNode = usefindGeneralInformationByPtcpntIds ? '//return/vetDeathDate' : '//return/DATE_OF_DEATH',
        serviceName = usefindGeneralInformationByPtcpntIds ? 'findGeneralInformationByPtcpntIds' : 'findGeneralInformationByFileNumber',
        vetInfo = usefindGeneralInformationByPtcpntIds ? new findGeneralInformationByPtcpntIds(vetSearchCtx) : new findGeneralInformationByFileNumber(vetSearchCtx),
        notificationSection = formContext.ui.tabs.get('general_tab').sections.get('execution_results');

    for (var i = 0; i < maxRetries; i++) {
        vetInfo.executeRequest();

        if (vetInfo.wsMessage.errorFlag) {
            // runtime error
            CloseProgress();
            var message = 'Error during the execution of ' + serviceName + ' service for Veteran.\nCannot determine the completion status of FNOD call.\n\n' + vetInfo.wsMessage.description;
            Xrm.Navigation.openAlertDialog({ text: message }, { height: 300, width: 500 }).then(function () {
                notificationSection.setLabel('Cannot determine the completion status of FNOD call; ' + serviceName + ' call failed');
                return false;
            });
            //Va.Udo.Crm.Scripts.Popup.MsgBox(message, Va.Udo.Crm.Scripts.Popup.PopupStyles.Critical, "Error", { width: 400, height: 165 });
            //alert(message);

        }
        var xmlObject = _XML_UTIL.parseXmlObject(vetInfo.responseXml);
        var dod = SingleNodeExists(xmlObject, dodResponseNode) ? xmlObject.selectSingleNode(dodResponseNode).text : '';
        if (dod !== null && dod.length > 0) {
            // FNOD completed, DOD is on recrod
            notificationSection.setLabel('FNOD/Memorial Certificate Execution Results: FNOD Date of Death found – MOD check executed');
            CloseProgress();
            return true;
        }
        // if we are here FNOD process didn't update DOD yet (or failed, we have no way to knowing which is true)
        // try to search CORP again
        ShowProgress('Validating FNOD Completion (attempt ' + (i + 2).toString() + ' of ' + maxRetries.toString() + ')');
    }
    // if we are here, we couldn't get DOD from corp after maxRetries
    // notify user
    notificationSection.setLabel('FNOD - Date of Death NOT FOUND. Wait a few minutes and refresh the screen or contact tech support.');

    CloseProgress();
    return false;
}

function getMandatoryFields(executionContext, type) {
    var formContext = executionContext.getFormContext();

    var mandatoryFields = {
        "FNOD": ["va_dateofdeath", "va_causeofdeath", "va_fnodfirstname", "va_fnodlastname"],
        "PMC": [],
        "MOD": ["va_survivingspouseisvalidformod", "va_typeofnotice", "va_spouseaddress1", "va_treasuryaddress1", "va_treasuryaddress2", "va_treasuryaddress3"],
        "0820F": [],
        "0820A": ["udo_pmc", "udo_nokletter", "udo_21530", "udo_21534", "udo_401330", "udo_otherpleasespecify", "udo_0820aquestionone", "udo_0820aquestiontwo", "udo_0820aquestionthree", "udo_0820aquestionfour"],
        "DBL": ["udo_letteraddressing", "udo_regionalofficeid"]
    }

    if (formContext.getAttribute("va_veteranhassurvivingspouse").getValue() === true || type === "reset") {
        mandatoryFields["MOD"].push("va_spousessn", "va_spousedob");
    }

    return mandatoryFields;
}

function SetMandatoryFields(executionContext, type) {
    var sets = Array();
    sets['FNOD'] = getMandatoryFields(executionContext, type)["FNOD"];
    sets['PMC'] = getMandatoryFields(executionContext, type)["PMC"];
    sets['MOD'] = getMandatoryFields(executionContext, type)["MOD"];
    sets["DBL"] = getMandatoryFields(executionContext, type)["DBL"];
    sets["0820F"] = getMandatoryFields(executionContext, type)["0820F"];
    sets["0820A"] = getMandatoryFields(executionContext, type)["0820A"];

    if (type === 'reset') {
        for (var op in sets) {
            var arr = sets[op];
            for (var c in arr) {
                formHelper.getAttribute(arr[c]).setSubmitMode('dirty');
                formHelper.getAttribute(arr[c]).setRequiredLevel('none');
            }
        }
        return;
    }
    var arr = sets[type];
    if (!arr) return;

    for (c in arr) {
        formHelper.getAttribute(arr[c]).setSubmitMode('always');
        formHelper.getAttribute(arr[c]).setRequiredLevel('required');
    };

}

function ExecuteFNOD(executionContext) {
    try {
        SetMandatoryFields(executionContext, 'reset');
        SetMandatoryFields(executionContext, 'FNOD');
        _ranFnodThisTime = false;
        var promiseArray = [];
        var ranProcessesPromise = new Promise(function (resolve, reject) {
            try {
                if (formHelper.getAttribute('va_ranfnod').getValue() || formHelper.getAttribute('va_ranpmc').getValue()) {
                    Xrm.Navigation.openAlertDialog({ text: "You cannot run the FNOD process twice and/or after PMC has been issued. \n\nIf you need to rerun the FNOD, please create a new FNOD request." }, { height: 300, width: 500 }).then(function () {
                        resolve(false);
                    });
                } else {
                    resolve(true);
                }
            } catch (e) {
                console.log("An error occurred in ranProcessesPromise: " + e);
                reject(e);
            }
        });
        promiseArray.push(ranProcessesPromise);

        var retrievedFirstName = formHelper.getValue('va_birlsfirstname'); //va_firstname
        var retrievedLastName = formHelper.getValue('va_birlslastname');   // va_lastname
        var enteredFirstName = formHelper.getValue('va_fnodfirstname');
        var enteredLastName = formHelper.getValue('va_fnodlastname');

        var namePromise = new Promise(function (resolve, reject) {
            try {
                if (!enteredFirstName || !enteredLastName) {
                    var message = 'First/last name of the veteran must be provided.';
                    Xrm.Navigation.openAlertDialog({ text: message }, { height: 300, width: 500 }).then(function () {
                        resolve(false);
                    });
                } else {
                    resolve(true);
                }
            } catch (e) {
                console.log("An error occurred in namePromise: " + e);
                reject(e);
            }
        });
        promiseArray.push(namePromise);


        var mismatchedNamePromise = new Promise(function (resolve, reject) {
            try {
                if (retrievedFirstName && retrievedLastName && enteredFirstName && enteredLastName) {
                    retrievedFirstName = retrievedFirstName.toUpperCase();
                    retrievedLastName = retrievedLastName.toUpperCase();
                    enteredFirstName = enteredFirstName.toString().toUpperCase();
                    enteredLastName = enteredLastName.toString().toUpperCase();

                    if (enteredFirstName !== retrievedFirstName || enteredLastName !== retrievedLastName) {
                        var message = 'The name you have entered does not match the name reported by BIRLS: \n\n' +
                            'Entered:  ' + enteredFirstName + ' ' + enteredLastName + '\n' +
                            'BIRLS:  ' + retrievedFirstName + ' ' + retrievedLastName + '\n\n' +
                            'First Notice of Death cannot be processed because names do not match.';
                        Xrm.Navigation.openAlertDialog({ text: message }, { height: 300, width: 500 }).then(function () {
                            resolve(false);
                        });
                    } else {
                        resolve(true);
                    }
                }
            } catch (e) {
                console.log("An error occurred in mismatchedNamePromise: " + e);
                reject(e);
            }
        });
        promiseArray.push(mismatchedNamePromise);

        var deathPromise = new Promise(function (resolve, reject) {
            try {
                if (!formHelper.getAttribute('va_dateofdeath').getValue() || !formHelper.getAttribute('va_causeofdeath').getValue()) {
                    var message = 'Date of death/cause of death must be provided.';
                    Xrm.Navigation.openAlertDialog({ text: message }, { height: 300, width: 500 }).then(function () {
                        resolve(false);
                    });
                } else {
                    resolve(true);
                }
            } catch (e) {
                console.log("An error occurred in deathPromise: " + e);
                reject(e);
            }
        });
        promiseArray.push(deathPromise);

        var dod = new Date(Date.parse(formHelper.getAttribute('va_dateofdeath').getValue()));
        var today = new Date();
        var futureDeathPromise = new Promise(function (resolve, reject) {
            try {
                if (dod > today) {
                    var message = 'Date of death cannot be a future date.';
                    Xrm.Navigation.openAlertDialog({ text: message }, { height: 300, width: 500 }).then(function () {
                        resolve(false);
                    });
                } else {
                    resolve(true);
                }
            } catch (e) {
                console.log("An error occurred in futureDeathPromise: " + e);
                reject(e);
            }
        });
        promiseArray.push(futureDeathPromise);


        Promise.all(promiseArray).then(function (result) {
            var fnodValid = true;
            for (var value = 0; value < result.length; value++) {
                if (!result[value]) {
                    fnodValid = false;
                    break;
                }
            }
            if (fnodValid) {
                var confirmStrings = {
                    confirmButtonLabel: "YES",
                    cancelButtonLabel: "NO",
                    title: "WARNING",
                    text: "You are about to run the FNOD process for the selected veteran. This operation cannot be undone.\n\nDo you want to proceed?"
                };
                Xrm.Navigation.openConfirmDialog(confirmStrings, { height: 300, width: 500 }).then(function (confirmObject) {
                    if (!confirmObject.confirmed) {
                        return;
                    }
                    var vetSearchCtx = new vrmContext(exCon);
                    vetSearchCtx.user = _UserSettings;

                    vetSearchCtx.parameters['fileNumber'] = formHelper.getAttribute('va_filenumber').getValue();

                    // First step: run Sync process if user requested it or CORP doesn't exist
                    if (!_doHaveCorpRecord || formHelper.getAttribute('va_synccorpandbirls').getValue() === true) {
                        if (formHelper.getAttribute('va_synccorpandbirls').getValue() !== true) { formHelper.getAttribute('va_synccorpandbirls').setValue(true); }
                        if (!ExecuteCorpBIRLSSync(vetSearchCtx, true)) {
                            var message = 'FNOD unsuccessful. Please reattempt.';
                            Xrm.Navigation.openAlertDialog({ text: message }, { height: 300, width: 500 }).then(function () {
                                return;
                            });

                        }
                    }

                    vetSearchCtx.parameters['dateOfDeath'] = dod.format('MM/dd/yyyy');

                    var cause = '';

                    switch (formHelper.getAttribute('va_causeofdeath').getValue()) {
                        case 953850000:
                            cause = 'Combat';
                            break;
                        case 953850001:
                            cause = 'Natural';
                            break;
                        case 953850002:
                            cause = 'Other';
                            break;
                        case 953850003:
                            cause = 'Unknown';
                            break;
                        default:
                            cause = 'Unknown';
                            break;
                    }

                    vetSearchCtx.parameters['causeOfDeath'] = cause;
                    var runFNOD = new updateFirstNoticeOfDeath(vetSearchCtx);
                    runFNOD.suppressProgressDlg = true;
                    runFNOD.executeRequest();
                    //console.log("runFNOD");
                    //console.log(runFNOD);
                    formHelper.getAttribute('va_fnodrequest').setValue(NN(runFNOD.wsMessage.xmlRequest));
                    formHelper.getAttribute('va_fnodresponse').setValue(NN(runFNOD.responseXml));

                    formHelper.getAttribute('va_vetdateofdeath').setValue(formHelper.getAttribute('va_dateofdeath').getValue());
                    formHelper.getAttribute('va_fnodrequeststatus').setValue("A First Notice of Death request has been processed successfully.");
                    formHelper.getAttribute('va_ranfnod').setValue(true);
                    formHelper.getAttribute('va_fnoddate').setValue(new Date());

                    // Show alert that FNOD processed
                    showMessage("FNOD Processed Successfully", "FNOD has been processed successfully.");

                    // Validate MOD Eligibility
                    ValidateMOD(executionContext);
                });
            }
        });
    } catch (e) {
        console.log("An error occurred within ExecuteFNOD: " + e);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function IssuePMC(executionContext) {
    var formContext = executionContext.getFormContext();

    if (!formContext.getAttribute('va_ranpmc').getValue()) {
        var requiredFields = ["va_newpmcvetfirstname", "va_newpmcvetlastname", "va_newpmcrecipaddress1", "va_newpmcrecipcity", "va_newpmcrecipstate", "va_newpmcrecipzip", "va_newpmcreciprelationshiptovet", "va_newpmcrecipsalutation"];
        var emptyFields = [];

        requiredFields.forEach(function (fieldName) {
            var val = formContext.getAttribute(fieldName).getValue();
            if (val === null) {
                emptyFields.push(fieldName);
            }
        });

        if (emptyFields.length > 0) {
            var fieldString = "";

            for (var i = 0; i < emptyFields.length; i++) {
                var label = formContext.getControl(emptyFields[i]).getLabel();
                fieldString += label;

                if (i + 1 !== emptyFields.length) {
                    fieldString += ", ";
                }
            }

            showMessage("Unable to Issue PMC", "The following required fields are missing values: " + fieldString);
            return;
        }

        var confirmStrings = {
            text: "Have you verified that the Veteran had an honorable or general discharge?",
            title: "Confirmation",
            confirmButtonLabel: "Confirm"
        };

        Xrm.Navigation.openConfirmDialog(confirmStrings, { height: 300, width: 500 }).then(function (confirmObject) {
            if (!confirmObject.confirmed) {
                return;
            }
            formHelper.getAttribute('va_confirmedvetsdischarge').setValue(true);

            var vetSearchCtx = new vrmContext(exCon);
            //_UserSettings = GetUserSettingsForWebservice(exCon);
            vetSearchCtx.user = _UserSettings;

            vetSearchCtx.parameters['fileNumber'] = formHelper.getAttribute('va_filenumber').getValue();
            vetSearchCtx.parameters['veteranFirstName'] = formHelper.getAttribute('va_newpmcvetfirstname').getValue();
            vetSearchCtx.parameters['veteranMiddleInitial'] = formHelper.getAttribute('va_newpmcvetmiddleinitial').getValue();
            vetSearchCtx.parameters['veteranLastName'] = formHelper.getAttribute('va_newpmcvetlastname').getValue();
            vetSearchCtx.parameters['veteranSuffixName'] = formHelper.getAttribute('va_newpmcvetsuffix').getValue();
            vetSearchCtx.parameters['station'] = formHelper.getAttribute('va_newpmcvetstation').getValue();
            vetSearchCtx.parameters['salutation'] = formHelper.getAttribute('va_newpmcrecipsalutation').getValue();
            vetSearchCtx.parameters['title'] = formHelper.getAttribute('va_newpmcrecipname').getValue();
            vetSearchCtx.parameters['addressLine1'] = formHelper.getAttribute('va_newpmcrecipaddress1').getValue();
            vetSearchCtx.parameters['addressLine2'] = formHelper.getAttribute('va_newpmcrecipaddress2').getValue();
            vetSearchCtx.parameters['city'] = formHelper.getAttribute('va_newpmcrecipcity').getValue();
            vetSearchCtx.parameters['state'] = formHelper.getAttribute('va_newpmcrecipstate').getValue();
            vetSearchCtx.parameters['zipCode'] = formHelper.getAttribute('va_newpmcrecipzip').getValue();
            vetSearchCtx.parameters['realtionshipToVeteran'] = formHelper.getAttribute('va_newpmcreciprelationshiptovet').getValue();
            var insertPMC = new insertPresidentialMemorialCertificate(vetSearchCtx);
            insertPMC.executeRequest();
            //console.log(insertPMC);
            formHelper.getAttribute('va_insertpmcrequest').setValue(NN(insertPMC.wsMessage.xmlRequest));

            CloseProgress();
            if (insertPMC.wsMessage.errorFlag) {
                formHelper.getAttribute('va_pmcrequeststatus').setValue(insertPMC.wsMessage.description);
                var message = "Error processing PMC.\n\nInternal Error: " + insertPMC.wsMessage.description;
                Xrm.Navigation.openAlertDialog({ text: message }, { height: 300, width: 500 }).then(function () {
                    return;
                });
            }
            else {
                formHelper.getAttribute('va_pmcrequeststatus').setValue("A Presidential Memorial Certificate has been processed successfully.");
                CreatePMCNote(executionContext);
                // Commenting, as this tab and section is no longer shown on the form
                // formHelper.setSectionVisible("execution_results", "general_tab", true);
                disableFnodModPmcControls(executionContext);
                formHelper.getAttribute('va_ranpmc').setValue(true);
                formHelper.saveRecord();
                Xrm.Navigation.openAlertDialog({ text: "Presidential Memorial Certificate has been processed successfully." }, { height: 300, width: 500 }).then(function () {
                    return;
                });
            }
        });
    }
    else {
        var message = 'This form cannot be modified after a new PMC has already been issued. Please create a new FNOD/PMC request.';
        Xrm.Navigation.openAlertDialog({ text: message }, { height: 300, width: 500 }).then(function () {
            return;
        });
    }
}

function CopyPMC() {
    // Commenting, as this tab and section is no longer shown on the form
	/*
    // FNOD Ran, Show or Hide ExecutionResults
    if (formHelper.getAttribute('va_ranpmc').getValue() || formHelper.getAttribute('va_ranfnod').getValue() || formHelper.getAttribute('va_ranmod').getValue()) {
        formHelper.setSectionVisible("execution_results", "general_tab", true);

    }
    else {
        formHelper.setSectionVisible("execution_results", "general_tab", false);
    }
	*/
    if (!formHelper.getAttribute('va_ranpmc').getValue()) {
        formHelper.getAttribute('va_newpmcvetfirstname').setValue(formHelper.getAttribute('va_existingpmcvetfirstname').getValue());
        formHelper.getAttribute('va_newpmcvetmiddleinitial').setValue(formHelper.getAttribute('va_existingpmcvetmiddleinitial').getValue());
        formHelper.getAttribute('va_newpmcvetlastname').setValue(formHelper.getAttribute('va_existingpmcvetlastname').getValue());
        formHelper.getAttribute('va_newpmcvetsuffix').setValue(formHelper.getAttribute('va_existingpmcvetsuffix').getValue());
        formHelper.getAttribute('va_newpmcvetstation').setValue(formHelper.getAttribute('va_existingpmcvetstation').getValue());
        formHelper.getAttribute('va_newpmcrecipsalutation').setValue(formHelper.getAttribute('va_existingpmcrecipsalutation').getValue());
        formHelper.getAttribute('va_newpmcrecipname').setValue(formHelper.getAttribute('va_existingpmcrecipname').getValue());
        formHelper.getAttribute('va_newpmcrecipaddress1').setValue(formHelper.getAttribute('va_existingpmcrecipaddress1').getValue());
        formHelper.getAttribute('va_newpmcrecipaddress2').setValue(formHelper.getAttribute('va_existingpmcrecipaddress2').getValue());
        formHelper.getAttribute('va_newpmcrecipcity').setValue(formHelper.getAttribute('va_existingpmcrecipcity').getValue());
        formHelper.getAttribute('va_newpmcrecipstate').setValue(formHelper.getAttribute('va_existingpmcrecipstate').getValue());
        formHelper.getAttribute('va_newpmcrecipzip').setValue(formHelper.getAttribute('va_existingpmcrecipzip').getValue());
        formHelper.getAttribute('va_newpmcreciprelationshiptovet').setValue(formHelper.getAttribute('va_existingpmcreciprelationshiptovet').getValue());
    }
    else {
        var message = 'This form cannot be modified after a new PMC has been issued. Please create a new FNOD/PMC request.';
        Xrm.Navigation.openAlertDialog({ text: message }, { height: 300, width: 500 });
    }
}
//not used?
function RetrieveAddressesFromParent(parent_page) {

    var addrXml = parent_page.getAttribute('va_findaddressresponse').getValue(),
        address_xmlObject = addrXml && addrXml.length > 0 ? _XML_UTIL.parseXmlObject(addrXml) : null,
        found_mailing = false,
        mailing_address = '',
        addressNodes = address_xmlObject ? address_xmlObject.selectNodes('//return') : null;

    if (addressNodes) {
        for (var i = 0; i < addressNodes.length; i++) {         //looping through addresses and
            if (addressNodes[i].selectSingleNode('ptcpntAddrsTypeNm').text === 'Mailing' && !found_mailing) {

                mailing_address += SingleNodeExists(addressNodes[i], 'addrsOneTxt') ? addressNodes[i].selectSingleNode('addrsOneTxt').text : '';
                mailing_address += SingleNodeExists(addressNodes[i], 'addrsTwoTxt') ? '\n' + addressNodes[i].selectSingleNode('addrsTwoTxt').text : '';
                mailing_address += SingleNodeExists(addressNodes[i], 'addrsThreeTxt') ? '\n' + addressNodes[i].selectSingleNode('addrsThreeTxt').text : '';
                mailing_address += SingleNodeExists(addressNodes[i], 'cityNm') ? '\n' + addressNodes[i].selectSingleNode('cityNm').text : '';
                mailing_address += SingleNodeExists(addressNodes[i], 'zipPrefixNbr') ? ', ' + addressNodes[i].selectSingleNode('zipPrefixNbr').text : '';
                var country = SingleNodeExists(addressNodes[i], 'cntryNm') ? addressNodes[i].selectSingleNode('cntryNm').text : '';

                mailing_address += country ? '\n' + country : '';

                mailing_address += SingleNodeExists(addressNodes[i], 'mltyPostOfficeTypeCd') ? '\n' + addressNodes[i].selectSingleNode('mltyPostOfficeTypeCd').text : ''; //APO
                mailing_address += SingleNodeExists(addressNodes[i], 'mltyPostalTypeCd') ? ' ' + addressNodes[i].selectSingleNode('mltyPostalTypeCd').text : ''; //AE

                found_mailing = true;

                formHelper.getAttribute('va_lastknownaddress').setValue(mailing_address);
            }
        }
    }
}

function CopyLastKnownAddressToSpouseFields(executionContext) {
    var copyValue = function (src, dst) {
        var val = formHelper.getAttribute(src).getValue();
        formHelper.getAttribute(dst).setValue(val);
    };
    copyValue('udo_lastaddress1', 'va_spouseaddress1');
    copyValue('udo_lastaddress2', 'va_spouseaddress2');
    copyValue('udo_lastaddress3', 'va_spouseaddress3');
    copyValue('udo_lastcity', 'va_spousecity');
    copyValue('udo_lastzipcode', 'va_spousezipcode');
    copyValue('udo_lastcountry', 'va_spousecountry');
    copyValue('udo_lastforeignpostalcode', 'va_spouseforeignpostalcode');
    copyValue('udo_lastaddresstype', 'va_spouseaddresstype');
    copyValue('udo_lastoverseasmilitarypostalcode', 'va_spouseoverseasmilitarypostalcode');
    copyValue('udo_lastoverseasmilitarypostofficetypeco', 'va_spouseoverseasmilitarypostofficetypecode');

    var stateOptions = formHelper.getAttribute('va_spousestatelist').getOptions();
    var state = formHelper.getAttribute('udo_laststate').getValue();
    if (state !== "") {
        var i;
        for (i = 0; i < stateOptions.length; i++) {
            if (stateOptions[i].text === state) {
                stateValue = stateOptions[i].value;
                break;
            }
        }
    }

    formHelper.getAttribute('va_spousestatelist').setValue(stateValue);

    updateTreasuryFields();
    usaaddress(executionContext);
}

function RetrieveVetInfo() {

    //debugger;
    // mark that CORP doesn't exist
    _doHaveCorpRecord = false;

    veteranInformation_xml = formHelper.getAttribute('va_findcorprecordresponse').getValue();

    //VTRIGILI 2014-12-30 - define varible to cover case whne there is no corp response
    //                      with our definition, it crashes later in the code
    var veteranInformation_xmlObject = null;

    if (veteranInformation_xml !== null) { veteranInformation_xmlObject = _XML_UTIL.parseXmlObject(veteranInformation_xml); }

    generalInfoResponseByPid_xml = formHelper.getAttribute("va_generalinformationresponsebypid").getValue();
    generalInfoResponseByPid_xmlObject = null;
    if (generalInfoResponseByPid_xml !== null) { generalInfoResponseByPid_xmlObject = _XML_UTIL.parseXmlObject(generalInfoResponseByPid_xml); }

    birls_xml = formHelper.getAttribute("va_findbirlsresponse").getValue();
    var birls_xmlObject = null;
    if (birls_xml !== null) { birls_xmlObject = _XML_UTIL.parseXmlObject(birls_xml); }

    dependents_xml = formHelper.getAttribute("va_finddependentsresponse").getValue();
    dependents_xmlObject = null;
    if (dependents_xml !== null) { dependents_xmlObject = _XML_UTIL.parseXmlObject(dependents_xml); }

    var vetFileNumber = SingleNodeExists(veteranInformation_xmlObject, '//return/fileNumber')
        ? veteranInformation_xmlObject.selectSingleNode('//return/fileNumber').text : null;

    // mark that CORP does exist
    if (vetFileNumber && vetFileNumber.length > 0) { _doHaveCorpRecord = true; }

    var vetFirstName = SingleNodeExists(veteranInformation_xmlObject, '//return/firstName')
        ? veteranInformation_xmlObject.selectSingleNode('//return/firstName').text : null;

    var vetLastName = SingleNodeExists(veteranInformation_xmlObject, '//return/lastName')
        ? veteranInformation_xmlObject.selectSingleNode('//return/lastName').text : null;

    var vetDOB = SingleNodeExists(veteranInformation_xmlObject, '//return/dateOfBirth')
        ? veteranInformation_xmlObject.selectSingleNode('//return/dateOfBirth').text : null;

    var vetDOD = SingleNodeExists(birls_xmlObject, '//return/DATE_OF_DEATH')
        ? birls_xmlObject.selectSingleNode('//return/DATE_OF_DEATH').text : null;

    if (!vetDOD) {
        vetDOD = SingleNodeExists(generalInfoResponseByPid_xmlObject, '//return/vetDeathDate')
            ? generalInfoResponseByPid_xmlObject.selectSingleNode('//return/vetDeathDate').text : null;
        if (vetDOD) {
            formHelper.getAttribute('va_vetdateofdeath').setValue(new Date(FormatExtjsDate(vetDOD)));
        }
    } else {
        formHelper.getAttribute('va_vetdateofdeath').setValue(new Date(vetDOD));
    }

    var vetSex = SingleNodeExists(generalInfoResponseByPid_xmlObject, '//return/vetSex')
        ? generalInfoResponseByPid_xmlObject.selectSingleNode('//return/vetSex').text : null;

    var vetSOJ = SingleNodeExists(generalInfoResponseByPid_xmlObject, '//return/stationOfJurisdiction')
        ? generalInfoResponseByPid_xmlObject.selectSingleNode('//return/stationOfJurisdiction').text : null;

    var vetPOA = SingleNodeExists(generalInfoResponseByPid_xmlObject, '//return/powerOfAttorney')
        ? generalInfoResponseByPid_xmlObject.selectSingleNode('//return/powerOfAttorney').text : null;

    // if no corp, get data from BIRLS
    var bilrsFirstName = '', birlsLastName = '', birlsFileNo = '';
    if (birls_xmlObject) {
        if (SingleNodeExists(birls_xmlObject, '//return/CLAIM_NUMBER')) {
            formHelper.getAttribute('va_birlsfileno').setValue(birls_xmlObject.selectSingleNode('//return/CLAIM_NUMBER').text);
            if (!vetFileNumber || vetFileNumber.length === 0) {
                vetFileNumber = birls_xmlObject.selectSingleNode('//return/CLAIM_NUMBER').text;
            }
        }

        if ((!vetFileNumber || vetFileNumber.length === 0) && SingleNodeExists(birls_xmlObject, '//return/SOC_SEC_NUMBER')) {
            vetFileNumber = birls_xmlObject.selectSingleNode('//return/SOC_SEC_NUMBER').text;
        }

        if (SingleNodeExists(birls_xmlObject, '//return/FIRST_NAME')) {
            formHelper.getAttribute('va_birlsfirstname').setValue(birls_xmlObject.selectSingleNode('//return/FIRST_NAME').text);
            if (!vetFirstName || vetFirstName.length === 0) {
                vetFirstName = birls_xmlObject.selectSingleNode('//return/FIRST_NAME').text;
            }
        }
        if (SingleNodeExists(birls_xmlObject, '//return/LAST_NAME')) {
            formHelper.getAttribute('va_birlslastname').setValue(birls_xmlObject.selectSingleNode('//return/LAST_NAME').text);
            if (!vetLastName || vetLastName.length === 0) {
                vetLastName = birls_xmlObject.selectSingleNode('//return/LAST_NAME').text;
            }
        }
        if ((!vetDOB || vetDOB.length === 0) && SingleNodeExists(birls_xmlObject, '//return/DATE_OF_BIRTH')) {
            vetDOB = birls_xmlObject.selectSingleNode('//return/DATE_OF_BIRTH').text;
        }
        if ((!vetSex || vetSex.length === 0) && SingleNodeExists(birls_xmlObject, '//return/SEX_CODE')) {
            vetSex = birls_xmlObject.selectSingleNode('//return/SEX_CODE').text;
        }
        if ((!vetSOJ || vetSOJ.length === 0) && SingleNodeExists(birls_xmlObject, '//return/FOLDER/FOLDER_CURRENT_LOCATION')) {
            vetSOJ = birls_xmlObject.selectSingleNode('//return/SEX_CODE').text;
        }
    }

    formHelper.getAttribute('va_filenumber').setValue(vetFileNumber);
    formHelper.getAttribute('va_firstname').setValue(vetFirstName);
    formHelper.getAttribute('va_lastname').setValue(vetLastName);

    //Addition of new DOB Text Field:  RTC 108417 - Handle Dates before 1900.
    formHelper.getAttribute('va_dateofbirthtext').setValue(vetDOB);
    formHelper.getAttribute('va_dateofbirthtext').setSubmitMode('always');

    if (vetSex === 'M') {
        formHelper.getAttribute('va_sex').setValue(953850000);
    } else if (vetSex === 'F') {
        formHelper.getAttribute('va_sex').setValue(953850001);
    } else {
        formHelper.getAttribute('va_sex').setValue(953850002);
    }

    formHelper.getAttribute('va_soj').setValue(vetSOJ);
    formHelper.getAttribute('va_poa').setValue(vetPOA);


    var dependents = '';
    returnNode = null;
    if (dependents_xmlObject) {
        returnNode = dependents_xmlObject.selectNodes('//return');
        var dependentNodes = returnNode && returnNode[0] && returnNode[0].childNodes ? returnNode[0].childNodes : null;

        if (returnNode && dependentNodes) {
            for (var i = 0; i < dependentNodes.length; i++) {         //looping through dependents
                if (dependentNodes[i].nodeName === 'persons') {
                    dependents += 'Name: ' + dependentNodes[i].selectSingleNode('firstName').text + ' ' + dependentNodes[i].selectSingleNode('lastName').text;
                    dependents += '     Relationship: ' + dependentNodes[i].selectSingleNode('relationship').text;
                    dependents += '     DOB: ' + dependentNodes[i].selectSingleNode('dateOfBirth').text;
                    if (dependentNodes[i].selectSingleNode('dateOfDeath').text !== '') { dependents += '     DOD: ' + dependentNodes[i].selectSingleNode('dateOfDeath').text; }
                    if (dependentNodes[i].selectSingleNode('ssn').text !== '') { dependents += '     SSN: ' + dependentNodes[i].selectSingleNode('ssn').text; }

                    //If spouse, alive and has a SSN, display his/her Awards
                    if ((dependentNodes[i].selectSingleNode('ssn').text !== '') && (dependentNodes[i].selectSingleNode('dateOfDeath').text === '') && (dependentNodes[i].selectSingleNode('relationship').text === 'Spouse')) {

                        var spouseSearchCtx = new vrmContext(exCon);
                        _UserSettings = GetUserSettingsForWebservice(exCon);
                        spouseSearchCtx.user = _UserSettings;

                        spouseSearchCtx.parameters['fileNumber'] = dependentNodes[i].selectSingleNode('ssn').text; //'225789999';//                        
                        var spouseInfo = new findGeneralInformationByFileNumber(spouseSearchCtx);
                        spouseInfo.executeRequest();

                        if (!spouseInfo.wsMessage.errorFlag) {
                            var spouseInfo_xmlObject = _XML_UTIL.parseXmlObject(spouseInfo.responseXml);
                            var awardsExist = SingleNodeExists(spouseInfo_xmlObject, '//return/numberOfAwardBenes') ? spouseInfo_xmlObject.selectSingleNode('//return/numberOfAwardBenes').text : '';
                            if (awardsExist !== '' & awardsExist > 0) {
                                dependents += '     Has Awards: Yes';
                            } else {
                                dependents += '     Has Awards?: No';
                            }
                        }
                    }

                    dependents += '\n';
                    //                    if (i !== dependentNodes.length && dependentNodes.length > 1) { dependents += '***************************************************************************************************\n'; }
                }
            }
        }
    }

    formHelper.getAttribute('va_listofdependents').setValue(dependents);

    var folderlocation = '';
    returnNode = birls_xmlObject.selectNodes('//return/folders');
    birlsNodes = returnNode[0].childNodes;

    //VTRIGILI 2014-12-30 Added if to handle the case where there is no BIRLS data
    var returnNode = null;
    var birlsNodes = null;
    if (birls_xmlObject !== null) {
        returnNode = birls_xmlObject.selectNodes('//return/folders');
    }

    if (returnNode) {
        birlsNodes = returnNode[0].childNodes;
        for (var i = 0; i < birlsNodes.length; i++) {         //looping through folders
            if (birlsNodes[i].nodeName === 'FOLDER') {
                if (birlsNodes[i].selectSingleNode('FOLDER_TYPE').text === 'CLAIM') {
                    folderlocation = birlsNodes[i].selectSingleNode('FOLDER_CURRENT_LOCATION').text;
                    break;
                }
            }
        }
    }

    formHelper.getAttribute('va_folderlocation').setValue(folderlocation);

    formHelper.getAttribute('va_lastknownaddress').setValue(_fnod_.parseAddress(_fnod_.getAddress(formHelper.getAttribute('va_findaddressresponse').getValue(), 'Mailing')));

    var vetSearchCtx = new vrmContext(exCon);
    _UserSettings = GetUserSettingsForWebservice(exCon);
    vetSearchCtx.user = _UserSettings;
    vetSearchCtx.parameters['fileNumber'] = formHelper.getAttribute('va_filenumber').getValue();
    var findPMC = new findPresidentialMemorialCertificate(vetSearchCtx);
    findPMC.executeRequest();

    formHelper.getAttribute('va_findpmcrequest').setValue(NN(findPMC.wsMessage.xmlRequest));

    CloseProgress();

    if (findPMC.wsMessage.errorFlag) {
        var message = 'Find PMC web service failed to refresh information based on the file Number provided.\n\nMid-tier components reported this error: \n\n' + findPMC.wsMessage.description;
        Xrm.Navigation.openAlertDialog({ text: message }, { height: 300, width: 500 }).then(function () {
            return;
        });
    }
    else {
        var pmc_xmlObject = _XML_UTIL.parseXmlObject(findPMC.responseXml);

        var pmc_vetFirstName = SingleNodeExists(pmc_xmlObject, '//return/veteranFirstName')
            ? pmc_xmlObject.selectSingleNode('//return/veteranFirstName').text : null;

        var pmc_vetMiddleInitial = SingleNodeExists(pmc_xmlObject, '//return/veteranMiddleInitial')
            ? pmc_xmlObject.selectSingleNode('//return/veteranMiddleInitial').text : null;

        var pmc_vetLastName = SingleNodeExists(pmc_xmlObject, '//return/veteranLastName')
            ? pmc_xmlObject.selectSingleNode('//return/veteranLastName').text : null;

        var pmc_vetSuffixName = SingleNodeExists(pmc_xmlObject, '//return/veteranSuffixName')
            ? pmc_xmlObject.selectSingleNode('//return/veteranSuffixName').text : null;

        var pmc_station = SingleNodeExists(pmc_xmlObject, '//return/station')
            ? pmc_xmlObject.selectSingleNode('//return/station').text : null;

        var pmc_recipSalutation = SingleNodeExists(pmc_xmlObject, '//return/salutation')
            ? pmc_xmlObject.selectSingleNode('//return/salutation').text : null;

        var pmc_recipTitle = SingleNodeExists(pmc_xmlObject, '//return/title')
            ? pmc_xmlObject.selectSingleNode('//return/title').text : null;

        var pmc_recipAddressLine1 = SingleNodeExists(pmc_xmlObject, '//return/addressLine1')
            ? pmc_xmlObject.selectSingleNode('//return/addressLine1').text : null;

        var pmc_recipAddressLine2 = SingleNodeExists(pmc_xmlObject, '//return/addressLine2')
            ? pmc_xmlObject.selectSingleNode('//return/addressLine2').text : null;

        var pmc_recipCity = SingleNodeExists(pmc_xmlObject, '//return/city')
            ? pmc_xmlObject.selectSingleNode('//return/city').text : null;

        var pmc_recipState = SingleNodeExists(pmc_xmlObject, '//return/state')
            ? pmc_xmlObject.selectSingleNode('//return/state').text : null;

        var pmc_recipZip = SingleNodeExists(pmc_xmlObject, '//return/zipCode')
            ? pmc_xmlObject.selectSingleNode('//return/zipCode').text : null;

        var pmc_recipRelationshipToVet = SingleNodeExists(pmc_xmlObject, '//return/realtionshipToVeteran')
            ? pmc_xmlObject.selectSingleNode('//return/realtionshipToVeteran').text : null;

        formHelper.getAttribute('va_existingpmcvetfirstname').setValue(pmc_vetFirstName);
        formHelper.getAttribute('va_existingpmcvetmiddleinitial').setValue(pmc_vetMiddleInitial);
        formHelper.getAttribute('va_existingpmcvetlastname').setValue(pmc_vetLastName);
        formHelper.getAttribute('va_existingpmcvetsuffix').setValue(pmc_vetSuffixName);
        formHelper.getAttribute('va_existingpmcrecipsalutation').setValue(pmc_recipSalutation);
        formHelper.getAttribute('va_existingpmcrecipname').setValue(pmc_recipTitle);
        formHelper.getAttribute('va_existingpmcrecipaddress1').setValue(pmc_recipAddressLine1);
        formHelper.getAttribute('va_existingpmcrecipaddress2').setValue(pmc_recipAddressLine2);
        formHelper.getAttribute('va_existingpmcrecipcity').setValue(pmc_recipCity);
        formHelper.getAttribute('va_existingpmcrecipstate').setValue(pmc_recipState);
        formHelper.getAttribute('va_existingpmcrecipzip').setValue(pmc_recipZip);
        formHelper.getAttribute('va_existingpmcreciprelationshiptovet').setValue(pmc_recipRelationshipToVet);

        if ((pmc_station) && (pmc_station !== '0')) {
            formHelper.getAttribute('va_existingpmcvetstation').setValue(pmc_station);
            formHelper.getAttribute('va_newpmcvetstation').setValue(pmc_station);
        }
        else if ((formHelper.getAttribute('va_folderlocation').getValue() !== null) && ((pmc_station === null) || (pmc_station === '0'))) {
            formHelper.getAttribute('va_existingpmcvetstation').setValue(folderlocation);
            formHelper.getAttribute('va_newpmcvetstation').setValue(folderlocation);
        }
        else if ((formHelper.getAttribute('va_folderlocation').getValue() === null) && ((pmc_station === null) || (pmc_station === '0'))) {
            formHelper.getAttribute('va_existingpmcvetstation').setValue('10');
            formHelper.getAttribute('va_newpmcvetstation').setValue('10');
        }

    }

}

function DisableAll() {
    formHelper.ui().controls.forEach(function (control, index) {
        if (control.getControlType() !== "webresource" && control.getControlType() !== "iframe") {
            control.setDisabled(true);
        }
    });
}

function disableFnodModPmcControls(executionContext) {
    try {
        var formContext = executionContext.getFormContext();
        var fnodTab = formContext.ui.tabs.get("fnod_tab");
        var modTab = formContext.ui.tabs.get("mod_tab");
        var pmcTab = formContext.ui.tabs.get("pmc_tab");

        var tabArray = [fnodTab, modTab, pmcTab];

        // Disable all controls in each tab
        tabArray.forEach(function (tabObj) {
            var tabSections = tabObj.sections;

            tabSections.forEach(function (sectionObj) {
                var controls = sectionObj.controls;

                controls.forEach(function (control) {
                    control.setDisabled(true);
                });
            });
        });

        return true;
    } catch (e) {
        console.log("An error occurred in disableFnodModPmcControls: " + e.message);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function DisableMODFields() {
    formHelper.setSectionVisible("mod_tab_section_3", "mod_tab", true);
    formHelper.setSectionVisible("spouse_section0", "mod_tab", true);
    formHelper.setSectionVisible("spouse_section1", "mod_tab", true);
    formHelper.setSectionVisible("spouse_section2", "mod_tab", true);
    formHelper.setSectionVisible("spouse_section3", "mod_tab", true);
    formHelper.setSectionVisible("spouse_section4", "mod_tab", true);
    formHelper.setSectionVisible("spouse_section5", "mod_tab", true);
    formHelper.setSectionVisible("spouse_section5b", "mod_tab", true);

    // Hide - this section is no longer needed on the form
    // formHelper.setTabVisible("treasuryadd", true);
    formHelper.setSectionVisible("spouse_section6", "mod_tab", true);
    formHelper.setSectionVisible("spouse_section7", "mod_tab", true);

    //formHelper.setSectionVisible("spouse_section5a", "mod_tab", false);
    //formHelper.setSectionVisible("mod_button_section", "mod_tab", false);

    formHelper.setSectionVisible("spouse_section5a", "mod_tab", true);
    formHelper.setSectionVisible("mod_button_section", "mod_tab", true);

    formHelper.setDisabled('va_spousefirstname', true);
    formHelper.setDisabled('va_spousemiddlename', true);
    formHelper.setDisabled('va_spouselastname', true);
    formHelper.setDisabled('va_spousesuffix', true);
    formHelper.setDisabled('va_spousessn', true);
    formHelper.setDisabled('va_spousedob', true);
    formHelper.setDisabled('va_spouseaddress1', true);
    formHelper.setDisabled('va_spouseaddress2', true);
    formHelper.setDisabled('va_spouseaddress3', true);
    formHelper.setDisabled('va_spousecity', true);
    formHelper.setDisabled('va_spousestatelist', true);
    formHelper.setDisabled('va_spousezipcode', true);
    formHelper.setDisabled('va_spousecountry', true);
    formHelper.setDisabled('va_spousecountrylist', true);
    formHelper.setDisabled('va_spouseaddresstype', true);
    formHelper.setDisabled('va_spouseoverseasmilitarypostalcode', true);
    formHelper.setDisabled('va_spouseoverseasmilitarypostofficetypecode', true);
    formHelper.setDisabled('va_spouseforeignpostalcode', true);
    formHelper.setDisabled('va_provincename', true);
    formHelper.setDisabled('va_territoryname', true);

}

function ExecuteMOD(executionContext) {
    try {
        Xrm.Utility.showProgressIndicator("Validating");

        var formContext = executionContext.getFormContext();

        //debugger;
        if (ValidateZipcode(executionContext) === false) {
            return false;
        }

        var vetSearchCtx = new vrmContext(exCon);
        var ga = formHelper.getAttribute;
        //var _UserSettings = GetUserSettingsForWebservice(exCon);
        if (_UserSettings === null || _UserSettings === undefined) { _UserSettings = GetUserSettingsForWebservice(exCon); }

        if (formHelper.getValue("va_isnationalorstatecemetery")) {
            vetSearchCtx.parameters['isNationalOrStateCemetery'] = "Y";
        } else {
            vetSearchCtx.parameters['isNationalOrStateCemetery'] = "N";
        }

        vetSearchCtx.user = _UserSettings;

        vetSearchCtx.parameters['cntrl_Mod_Tran_Id'] = formHelper.getValue("va_modtranid"); //2041

        var noticetypeSelection = formHelper.getAttribute("va_typeofnotice").getSelectedOption().text,
            spouserecordactionSelection = NN(ga('va_spouserecordaction').getText());
		/*
		Per Jinmay Patel :Here are four possible values for spouse change indicator. 
 
		A - Add a Spouse
		M - Modify Spouse Info along with address (Name, DOB, SSN and address)
		N - Next of Kin processing
		blank or do not populate - No change to spouse Info but changes allowed to address only (In other words, no changes allowed for spouse name, DOB, SSN)
		*/

        switch (spouserecordactionSelection) {
            case 'Add New Spouse':
                vetSearchCtx.parameters['spouseChangeInd'] = 'A';
                break;
            case 'Modify Existing Spouse':
                vetSearchCtx.parameters['spouseChangeInd'] = 'M';
                break;
            case 'Send Next of Kin Letter':
                vetSearchCtx.parameters['spouseChangeInd'] = 'N';
                break;
            default:
                vetSearchCtx.parameters['spouseChangeInd'] = '';
                break;
        }

        //no longer used
        //var VetLetArray = ["Telephone FNOD", "Personal Interview FNOD", "Insurance PCR FNOD", "Death Match"]; //valid choices for the Vet letter to be sent  

        if (spouserecordactionSelection === 'Send Next of Kin Letter') {
            vetSearchCtx.parameters['mod_Letter_Type_Cd'] = 'NEXTKINPLUS';
        } else {
            vetSearchCtx.parameters['mod_Letter_Type_Cd'] = 'VET';
        }

        var modProcessType = '';
        var spouseValid = ga('va_survivingspouseisvalidformod').getValue();

        if (vetSearchCtx.parameters['mod_Letter_Type_Cd'] === 'NEXTKINPLUS') {
            modProcessType = 'LMOD';
        } else {
            if (spouseValid) {
                modProcessType = 'GMOD';
            } else {
                modProcessType = 'GMOD'; //modProcessType = 'LMOD';
            }
        }
        var promiseArray = [];

        var ssaInquiryPromise = ExecuteSsaInquiry();
        promiseArray.push(ssaInquiryPromise);

        var modProcessTypePromise = new Promise(function (resolve, reject) {
            try {
                if (modProcessType === undefined || modProcessType === null || modProcessType === "") {
                    var message = 'Error: Unable to determine MOD Process Type (GMOD or LMOD). Please contact the support team to research this issue.';
                    Xrm.Navigation.openAlertDialog({ text: message }, { height: 300, width: 500 }).then(function () {
                        resolve(false);
                    });
                } else {
                    resolve(true);
                }
            } catch (e) {
                Xrm.Utility.closeProgressIndicator();
                console.log("An error occurred in modProcessTypePromise: " + e);
                reject(e);
            }
        });
        promiseArray.push(modProcessTypePromise);

        var spouseInfoPromise = new Promise(function (resolve, reject) {
            try {
                if (modProcessType === 'GMOD') {
                    // Defect 87024 For GMOD the mod_Letter_Type_Cd should always be PAYPLUS
                    vetSearchCtx.parameters['mod_Letter_Type_Cd'] = 'PAYPLUS';

                    // Defect 102179 The following fields are required for GMOD : First Name, Last Name, SSN and DOB
                    var fields = [
                        { name: 'First Name', value: 'va_spousefirstname' },
                        { name: 'Last Name', value: 'va_spouselastname' },
                        { name: 'SSN', value: 'va_spousessn' },
                        { name: 'Date of Birth', value: 'va_spousedob' }
                    ];

                    for (var field in fields) {
                        var fieldName = fields[field].value, val = ga(fieldName).getValue();
                        if (val === null || val.length === 0) {
                            var messageSub = 'Field ' + fields[field].name + ' is required when Process Type is No change to Spouse, Add a spouse, or Modify a spouse.';
                            Xrm.Utility.closeProgressIndicator();
                            Xrm.Navigation.openAlertDialog({ text: messageSub }, { height: 300, width: 500 }).then(function () {
                                formHelper.setDisabled(fieldName, false);
                                formHelper.getControl(fieldName).setFocus();
                                resolve(false);
                            });
                        }
                    }
                    var ssn = (ga('va_spousessn').getValue());
                    ssn = ssn ? ssn.trim() : '';
                    ssn.replace(new RegExp('-', 'g'), '').replace(new RegExp(' ', 'g'), '');
                    if (!ssn || ssn.length !== 9) {
                        var messageSub1 = 'SSN is required when Process Type is No change to Spouse, Add a spouse, or Modify a spouse. Please make sure it contains 9 digits.';
                        Xrm.Utility.closeProgressIndicator();
                        Xrm.Navigation.openAlertDialog({ text: messageSub1 }, { height: 300, width: 500 }).then(function () {
                            formHelper.getControl('va_spousessn').setDisabled(false);
                            formHelper.getControl('va_spousessn').setFocus();
                            resolve(false);
                        });

                    }
                } else {
                    resolve(true);
                }
                resolve(true);
            } catch (e) {
                Xrm.Utility.closeProgressIndicator();
                console.log("An error occurred in spouseInfoPromise: " + e);
                reject(e);
            }
        });
        promiseArray.push(spouseInfoPromise);

        ga('va_modprocesstype').setValue(modProcessType);
        vetSearchCtx.parameters['mod_Procs_Type_Cd'] = modProcessType;  //e.g., GMOD

        var survivingSpouseModPromise = new Promise(function (resolve, reject) {
            try {
                if (ga('va_survivingspouseisvalidformod').getValue() === false && ga('va_modprocesstype').getValue() !== 'LMOD') {
                    Xrm.Utility.closeProgressIndicator();
                    Xrm.Navigation.openAlertDialog({ text: 'Please confirm that the surviving spouse is eligible for the MOD transaction.' }, { height: 300, width: 500 }).then(function () {
                        resolve(false);
                    });
                } else {
                    resolve(true);
                }
            } catch (e) {
                Xrm.Utility.closeProgressIndicator();
                console.log("An error occurred in survivingSpouseModPromise: " + e);
                reject(e);
            }
        });
        promiseArray.push(survivingSpouseModPromise);
        //If not just letter, spouse's info must be verified


        vetSearchCtx.parameters['fileNumber'] = ga('va_filenumber').getValue();
        vetSearchCtx.parameters['spouseSSNNumber'] = ga('va_spousessn').getValue();
        var msj = ga('va_modsoj').getValue();
        if (msj && msj.length >= 3) { msj = msj.substring(0, 3); }
        vetSearchCtx.parameters['stationNumber'] = msj;
        vetSearchCtx.parameters['veteranParticipantID'] = ga('va_modvetptcpntid').getValue();

        if (modProcessType === 'LMOD') { // This means Send Next of Kin Letter
            vetSearchCtx.parameters['beneParticipantID'] = '';
        }
        else {
            vetSearchCtx.parameters['beneParticipantID'] = ga('va_modspouseptcpntid').getValue();
        }

        if (modProcessType === 'GMOD' && vetSearchCtx.parameters['mod_Letter_Type_Cd'] === '') {
            vetSearchCtx.parameters['letterRecipientID'] = ''; //payment only, no letters
        } else {
            vetSearchCtx.parameters['letterRecipientID'] = ga('va_modvetptcpntid').getValue();
        }

        vetSearchCtx.parameters['spouseFirstName'] = ga('va_spousefirstname').getValue();
        vetSearchCtx.parameters['spouseMiddleName'] = ga('va_spousemiddlename').getValue();
        vetSearchCtx.parameters['spouseLastName'] = ga('va_spouselastname').getValue();
        vetSearchCtx.parameters['normalizedAddressLine1'] = ga('va_spouseaddress1').getValue();
        vetSearchCtx.parameters['normalizedAddressLine2'] = NN(ga('va_spouseaddress2').getValue());
        vetSearchCtx.parameters['normalizedAddressLine3'] = NN(ga('va_spouseaddress3').getValue());


        var temp1 = '';
        switch (ga('va_spouseoverseasmilitarypostofficetypecode').getValue()) {
            case 953850000:
                temp1 = "APO";
                break;
            case 953850001:
                temp1 = "DPO";
                break;
            case 953850002:
                temp1 = "FPO";
                break;
            default:
                break;
        }
        vetSearchCtx.parameters['militaryPostalOfficeTypeCode'] = temp1;

        temp1 = '';
        switch (ga('va_spouseoverseasmilitarypostalcode').getValue()) {
            case 953850000:
                temp1 = "AA";
                break;
            case 953850001:
                temp1 = "AE";
                break;
            case 953850002:
                temp1 = "AP";
                break;
            default:
                break;
        }
        vetSearchCtx.parameters['militaryPostalTypeCode'] = temp1;

        vetSearchCtx.parameters['provinceName'] = NN(ga('va_provincename').getValue());
        vetSearchCtx.parameters['territoryName'] = NN(ga('va_territoryname').getValue());

        vetSearchCtx.parameters['stateCode'] = (formHelper.getAttribute("va_spousestatelist").getSelectedOption() === null ? '' : formHelper.getAttribute("va_spousestatelist").getSelectedOption().text);
        vetSearchCtx.parameters['zipCode'] = NN(ga('va_spousezipcode').getValue());

        var spouseDOB = ga('va_spousedob').getValue();
        if (spouseDOB && spouseDOB.toString().length > 0) { spouseDOB = new Date(Date.parse(spouseDOB)).format('MM/dd/yyyy'); }
        if (spouseDOB === null || spouseDOB.toString().length <= 1) { spouseDOB = "        "; }
        vetSearchCtx.parameters['spouseBirthDate'] = spouseDOB;
        vetSearchCtx.parameters['cityName'] = NN(ga('va_spousecity').getValue());

        // make country code 3 digits
        var country = ga('va_spousecountry').getValue();
        if (country === null) {
            country = '';
        }
        var cU = country.toUpperCase(),
            isUs = (cU === 'US' || cU === 'USA' || cU === 'U.S.A.' || cU === 'UNITED STATES' || cU === 'UNITED STATES OF AMERICA'),
            isDomestic = (isUs === true || country === '');

        if (country && cU === 'US') { country = 'USA'; }
        var countryNamePromise = new Promise(function (resolve, reject) {
            try {
                if (country && country.length > 0 && country.length < 3) {
                    var messageSub2 = 'Please make sure that Country Name is at least 3 characters long.';
                    Xrm.Utility.closeProgressIndicator();
                    Xrm.Navigation.openAlertDialog({ text: messageSub2 }, { height: 300, width: 500 }).then(function () {
                        resolve(false);
                    });
                } else {
                    resolve(true);
                }
            } catch (e) {
                Xrm.Utility.closeProgressIndicator();
                console.log("An error occurred in countryNamePromise: " + e);
                reject(e);
            }
        });
        promiseArray.push(countryNamePromise);

        if (isUs) { country = country.toUpperCase(); }
        else if (country) {
            // Defect 96137 For Country code, other than USA, use the full name but make it Upper and lower e.g. France or Sri Lanka
            country = NormalizeCountry(country);
        }

        // Issue 87240, do not allow NONE for country code
        var noCountryCodePromise = new Promise(function (resolve, reject) {
            try {
                if (country.toUpperCase() === 'NONE') {
                    var messageSub3 = 'Invalid country code.  Please use blank field instead of NONE.';
                    Xrm.Utility.closeProgressIndicator();
                    Xrm.Navigation.openAlertDialog({ text: messageSub3 }, { height: 300, width: 500 }).then(function () {
                        resolve(false);
                    });
                } else {
                    resolve(true);
                }
            } catch (e) {
                Xrm.Utility.closeProgressIndicator();
                console.log("An error occurred in noCountryCodePromise: " + e);
                reject(e);
            }
        });
        promiseArray.push(noCountryCodePromise);

        var charCodePromise = new Promise(function (resolve, reject) {
            try {
                if (country && country.length > 0 && country.length < 3) {
                    var messageSub4 = 'Please make sure that Country Name is at least 3 characters long.';
                    Xrm.Utility.closeProgressIndicator();
                    Xrm.Navigation.openAlertDialog({ text: messageSub4 }, { height: 300, width: 500 }).then(function () {
                        resolve(false);
                    });
                } else {
                    resolve(true);
                }
            } catch (e) {
                Xrm.Utility.closeProgressIndicator();
                console.log("An error occurred in charCodePromise: " + e);
                reject(e);
            }
        });
        promiseArray.push(charCodePromise);

        vetSearchCtx.parameters['spouseForeignPostalCode'] = '';

        var requireds, domOrIntlAddress = (isUs || ga('va_spouseaddresstype').getText() === 'Domestic' || ga('va_spouseaddresstype').getText() === 'International');

        if (ga('va_spouseaddresstype').getText() === 'Domestic') {
            requireds = ['va_spouseaddress1', 'va_spousecity', 'va_spousecountry', 'va_spousestatelist'];
        } else if (ga('va_spouseaddresstype').getText() === 'International') {
            requireds = ['va_spouseaddress1', 'va_spousecity', 'va_spousecountry'];
        } else if (ga('va_spouseaddresstype').getText() === 'Overseas Military') {
            requireds = ['va_spouseaddress1', 'va_spouseoverseasmilitarypostalcode', 'va_spouseoverseasmilitarypostofficetypecode'];
        } else {
            requireds = ['va_spouseaddress1'];
        }


        //   if (domOrIntlAddress) {

        var domIntPromise = new Promise(function (resolve, reject) {
            try {
                var error = CheckDomIntRequireds(requireds);
                if (error) {
                    var messageSub5 = error;
                    Xrm.Utility.closeProgressIndicator();
                    Xrm.Navigation.openAlertDialog({ text: messageSub5 }, { height: 300, width: 500 }).then(function () {
                        resolve(false);
                    });
                } else {
                    resolve(true);
                }
            } catch (e) {
                Xrm.Utility.closeProgressIndicator();
                console.log("An error occurred in domIntPromise: " + e);
                reject(e);
            }
        });
        promiseArray.push(domIntPromise);

        updateTreasuryFields();
        //        break;
        //    }
        //}

        //let's check the fields again
        var treasuryAdressPromise = new Promise(function (resolve, reject) {
            try {
                requireds = [ga('va_treasuryaddress1').getValue(), ga('va_treasuryaddress2').getValue(), ga('va_treasuryaddress3').getValue()];
                var i;
                for (i = 0; i < requireds.length; i++) {
                    if (!requireds[i] || requireds[i].length === 0) {
                        formHelper.setDisabled('va_treasuryaddress1', false);
                        formHelper.setDisabled('va_treasuryaddress2', false);
                        formHelper.setDisabled('va_treasuryaddress3', false);
                        var messageSub6 = 'Unable to generate values the following required fields:\n\Treasury Address 1; Treasury Address 2; Treasury Address 3 \n\n\Please populate the fields manually.';
                        Xrm.Utility.closeProgressIndicator();
                        Xrm.Navigation.openAlertDialog({ text: messageSub6 }, { height: 300, width: 500 }).then(function () {
                            resolve(false);
                        });
                    }
                }
                resolve(true);
            } catch (e) {
                Xrm.Utility.closeProgressIndicator();
                console.log("An error occurred in treasuryAdressPromise: " + e);
                reject(e);
            }
        });
        promiseArray.push(treasuryAdressPromise);

        // prompt changed on 10/26/12
        //var modPrompt = "WARNING: You are about to run the MOD process for the selected veteran. Please read the following IDs back to the caller and confirm: \n\n Veteran's File No: " + NN(ga('va_filenumber').getValue()) + "\n Spouse's SSN: " + NN(ga('va_spousessn').getValue()) + "\n\nThe address you provided will be used for letters and MOD awards. Click OK to process or Cancel to make any changes and then Click on submit MOD/NOK/Letters button.";

        //console.log("atpromiseall");

        Promise.all(promiseArray).then(function (result) {

            //console.log("in promise all");
            //console.log(result);
            var modValid = true;
            for (var value = 0; value < result.length; value++) {
                if (!result[value]) {
                    modValid = false;
                    break;
                }
            }
            if (modValid) {
                var modPrompt = "The address entered will be used for the MOD payment (widow’s address) or NOK Letter (Veteran’s address of record). Click 'OK' to process or 'Cancel' to make any changes. After making any changes, Click the 'Update Treasury Address' and 'Submit MOD/NOK/Letters' buttons";
                var confirmStrings = {
                    title: "WARNING",
                    text: modPrompt
                };
                Xrm.Utility.closeProgressIndicator();
                Xrm.Navigation.openConfirmDialog(confirmStrings, { height: 300, width: 500 }).then(function confirm(confirmObject) {

                    //console.log(confirmObject);
                    if (!confirmObject.confirmed) {
                        return;
                    }

                    // Show progress indicator
                    Xrm.Utility.showProgressIndicator("Submitting MOD/NOK/LETTERS");

                    // Save first
                    formContext.data.save().then(function (success) {
                        vetSearchCtx.parameters['countryName'] = country;

                        vetSearchCtx.parameters['treasuryMailingAddressLine1'] = NN(ga('va_treasuryaddress1').getValue());
                        vetSearchCtx.parameters['treasuryMailingAddressLine2'] = NN(ga('va_treasuryaddress2').getValue());
                        vetSearchCtx.parameters['treasuryMailingAddressLine3'] = NN(ga('va_treasuryaddress3').getValue());
                        vetSearchCtx.parameters['treasuryMailingAddressLine4'] = NN(ga('va_treasuryaddress4').getValue());
                        vetSearchCtx.parameters['treasuryMailingAddressLine5'] = NN(ga('va_treasuryaddress5').getValue());
                        vetSearchCtx.parameters['treasuryMailingAddressLine6'] = NN(ga('va_treasuryaddress6').getValue());
                        var updateMod = new updateMonthOfDeath(vetSearchCtx);
                        updateMod.executeRequest();

                        // Show progress indicator
                        Xrm.Utility.showProgressIndicator("Submitting MOD/NOK/LETTERS");

                        // Commenting, as the progress indicator is now closed later in the execution pipeline
                        // CloseProgress();

                        ga('va_updatemonthofdeathrequest').setValue(NN(updateMod.wsMessage.xmlRequest));
                        ga('va_updatemonthofdeathresponse').setValue(NN(updateMod.wsMessage.xmlResponse));

                        var dt = new Date();

                        var modError = updateMod.wsMessage.errorFlag, desc = '', resp = updateMod.wsMessage.xmlResponse;
                        if (modError) {
                            desc = 'Mid-tier components reported this error: ' + NN(updateMod.wsMessage.description);
                        } else if (resp && resp.length > 0) {
                            if (resp.indexOf('Month of Death not processed') >= 0 || resp.indexOf('ns2:ShareException') >= 0) {
                                modError = true;
                                desc = 'Please review MOD Response box in Web Service Response section for the detailed error description.';
                            }
                        }
                        var modErrorPromise = new Promise(function (resolve, reject) {
                            try {
                                if (modError) {
                                    CreateMODFailedNote(executionContext);
                                    ga('va_modrequeststatus').setValue(NN(updateMod.wsMessage.description).substring(0, 2000));
                                    //ga('va_modresults').setValue('Failure: (' + dt.toDateString() + ') ' + updateMOD.wsMessage.description);
                                    var messageSub7 = 'Update MOD web service failed to execute.\n\n' + desc;
                                    Xrm.Utility.closeProgressIndicator();
                                    Xrm.Navigation.openAlertDialog({ text: messageSub7 }, { height: 300, width: 500 }).then(function () {
                                        resolve(false);
                                    });
                                }
                                else {
                                    resolve(true);
                                }
                            } catch (e) {
                                Xrm.Utility.closeProgressIndicator();
                                console.log("An error occurred in initiateCommonScripts: " + e);
                                reject(e);
                            }
                        });

                        modErrorPromise.then(function (result) {
                            if (result) {
                                // Auto-generate 0820f
                                var autoGenerate0820fPromise = autoGenerate0820fAsync(executionContext);

                                // Set 'Submit MOD Failed' field to false
                                formContext.getAttribute("udo_submitmodfailed").setValue(false);

                                ga('va_modrequeststatus').setValue("A Month of Death record has been processed successfully.");
                                //ga('va_modresults').setValue('Success (' + dt.toDateString() + ')');

                                CreateMODSuccessNote(executionContext);

                                // Commenting, as this tab and section is no longer shown on the form
                                // formHelper.setSectionVisible("general_tab", "execution_results", true);
                                DisableMODFields(); //DisableAll();
                                ga('va_ranmod').setValue(true);
                                // Commenting, as this tab and section is no longer shown on the form
								/*
								// FNOD Ran, Show or Hide ExecutionResults
								if (formHelper.getAttribute('va_ranpmc').getValue() || formHelper.getAttribute('va_ranfnod').getValue() || formHelper.getAttribute('va_ranmod').getValue()) {
									formHelper.setSectionVisible("general_tab", "execution_results", true);
								}
								else {
									formHelper.setSectionVisible("general_tab", "execution_results", false);
								}
								*/

                                autoGenerate0820fPromise.finally(function () {
                                    Xrm.Utility.closeProgressIndicator();
                                    var messageSub8 = "A Month of Death record has been processed successfully. Please click OK to save the record.";
                                    Xrm.Navigation.openAlertDialog({ text: messageSub8 }, { height: 300, width: 500 }).then(function () {
                                        formContext.data.save();
                                    });
                                });
                            } else {
                                Xrm.Utility.closeProgressIndicator();

                                // Set 'Submit MOD Failed' field to true
                                formContext.getAttribute("udo_submitmodfailed").setValue(true);
                            }
                        });
                    });
                });
            }
        });
    } catch (e) {
        Xrm.Utility.closeProgressIndicator();
        console.log("An error occurred in ExecuteMOD: " + e);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function autoGenerate0820fAsync(executionContext) {
    return new Promise(function (resolve, reject) {
        try {
            var formContext = executionContext.getFormContext();

            if (formContext.getAttribute("va_veteranhassurvivingspouse").getValue() === true) {
                generateLetterAsync(executionContext, "0820F-FNOD", null, true, false, true, false).then(function (result) {
                    if (result) {
                        formContext.getAttribute("udo_ran0820f").setValue(true);
                        return resolve(true);
                    } else {
                        console.log("An error occurred in autoGenerate0820fAsync (inner): 0820F was not generated/uploaded successfully.");
                        return reject(false);
                    }
                }).catch(function (error) {
                    console.log("An error occurred in autoGenerate0820fAsync (outer): 0820F was not generated/uploaded successfully.");
                    return reject(e);
                });
            } else {
                return resolve(false);
            }
        } catch (e) {
            console.log("An error occurred in autoGenerate0820fAsync: " + e);
            return reject(e);
        }
    });
}

function CheckDomIntRequireds(requireds) {
    var ga = formHelper.getAttribute;
    var gc = formHelper.getControl;
    var missingFields = [];
    var error = null;

    for (var i = 0; i < requireds.length; i++) {
        if (!ga(requireds[i]).getValue() || ga(requireds[i]).getValue().length === 0) {
            missingFields.push(requireds[i]);
            gc(requireds[i]).setDisabled(false);
        }
    }
    if (missingFields.length > 0) {
        error = 'Following fields are required for MOD:\n\n';
        for (var i = 0; i < missingFields.length; i++) {
            if (i !== 0) {
                error += '; ';
            }

            if (missingFields[i] === 'va_spouseaddress1') {
                error += 'Address 1';
            }
            else if (missingFields[i] === 'va_spousecity') {
                error += 'City';
            }
            else if (missingFields[i] === 'va_spousecountry') {
                error += 'Country';
            }
            else if (missingFields[i] === 'va_spouseoverseasmilitarypostalcode') {
                error += 'Overseas Military Postal Code';
            }
            else if (missingFields[i] === 'va_spouseoverseasmilitarypostofficetypecode') {
                error += 'Overseas Military Post Office Type Code';
            }
            else if (missingFields[i] === 'va_spousestatelist') {
                error += 'State';
            }
        }
        console.log("An error occurred in CheckDomIntRequireds: " + error);
        return error;
    }
}
function ExecuteCorpBIRLSSync(vetSearchCtx, alertOK) {
    //debugger;
    var sync = new syncCorporateBirls(vetSearchCtx);
    sync.suppressProgressDlg = true;
    sync.executeRequest();
    formHelper.getAttribute('va_synccorpandbirlsresponse').setValue(sync.responseXml);


    if (sync.wsMessage.errorFlag) {
        var message = 'Sync Corp and BIRLS web service had failed to execute correctly.\n\nMid-tier components reported this error: ' + sync.wsMessage.description;
        Xrm.Navigation.openAlertDialog({ text: message }, { height: 300, width: 500 }).then(function () {
            // formHelper.data.entity.save("save");
            formHelper.data.save("save");
            return false;
        });
    }

    formHelper.getAttribute('va_ransync').setValue(true);
    //if (alertOK) {
    //    var messageRan = sync.wsMessage.description;
    //    var x = _XML_UTIL.parseXmlObject(sync.responseXml);
    //    if (SingleNodeExists(x, '//return')) {
    //        messageRan = x.selectSingleNode('//return').text;
    //    }
    //}
    return true;
}

function ValidateMOD(executionContext) {
    try {
        var formContext = executionContext.getFormContext();

        // Check if ValidateMOD has already run
        var ranFindModAttr = formContext.getAttribute("va_ranfindmod");
        var ranFnodAttr = formContext.getAttribute("va_ranfnod");
        if (ranFindModAttr.getValue() === true || ranFnodAttr.getValue() === false || formContext.ui.getFormType() !== 2) {
            return;
        }

        formContext.getAttribute('va_modeligible').setValue(false);

        // Get user settings using context - this will provide the file number
        var vetSearchCtx = new vrmContext(exCon);

        if (_UserSettings === null || _UserSettings === undefined) { 
            _UserSettings = GetUserSettingsForWebservice(exCon); 
        }

        vetSearchCtx.user = _UserSettings;
        vetSearchCtx.parameters['fileNumber'] = formHelper.getValue('va_filenumber');

        // Execute findMOD
        var findMOD = new findMonthOfDeath(vetSearchCtx);
        var findModRequestSucceeded =  findMOD.executeRequest();
        CloseProgress();
        formHelper.setValue('va_findmonthofdeathrequest', NN(findMOD.wsMessage.xmlRequest));

        formHelper.setFieldVisible("va_modeligible", true);
        ranFindModAttr.setValue(true);

        if (!findModRequestSucceeded) {
            throw { message: 'Web Service Request failed. Please consult trace logs for further details.' };
        }

        // Parse results from service request
        var parser = new DOMParser();
        var parsedDoc = parser.parseFromString(findMOD.wsMessage.xmlResponse, "text/xml");
        var newDoc = parser.parseFromString(parsedDoc.getElementsByTagName("ExecuteResult")[0].childNodes[0].nodeValue, "text/xml");

        if (newDoc.getElementsByTagName("message")[0]) {
            //if response doesnt contain "Not eligible" and parsed xml has a node called eligibleIndicator set to Y, then continue as eligible
            var isEligible = (findMOD.responseXml.indexOf("Not Eligible") === -1 &&
                (SingleNodeExists(mod_xmlObject, '//return/eligibleIndicator') ? mod_xmlObject.selectSingleNode('//return/eligibleIndicator').text : null) === 'Y');

            if (!isEligible) {
                return 'Not Eligible';
            }
        } else {
            // Set indicator fields based on returned response
            var returedXml = (parsedDoc.getElementsByTagName("ExecuteResult")[0].childNodes[0]);
            formHelper.setValue("va_modeligible", true);

            var spouseExistsIndicator = parser.parseFromString(returedXml.data, "text/xml").getElementsByTagName("spouseExistsIndicator")[0].childNodes[0]
                ? parser.parseFromString(returedXml.data, "text/xml").getElementsByTagName("spouseExistsIndicator")[0].childNodes[0].nodeValue
                : null;

            if (spouseExistsIndicator === 'Y') {
                setValueFireOnChange(executionContext, "va_veteranhassurvivingspouse", true);
                formHelper.setValue("va_survivingspouseisvalidformod", true);
                RedrawSpouseFields('No selection', executionContext);
            } else {
                setValueFireOnChange(executionContext, "va_veteranhassurvivingspouse", false);
                formHelper.setValue("va_survivingspouseisvalidformod", false);
                RedrawSpouseFields('NOK', executionContext);
                formHelper.getControl('va_spouserecordaction').removeOption(953850001); // if no spouse exists, remove Modify option
                formHelper.setValue("va_spouserecordaction", 953850002); //default to NOK
            }

            RetrieveSpouseInfo(parser.parseFromString(returedXml.data, "text/xml"), executionContext);

            //retrieving key information from findMOD to be sent back when running UpdateMOD
            var monthOfDeathTranId = parser.parseFromString(returedXml.data, "text/xml").getElementsByTagName("monthOfDeathTranId")[0].childNodes[0]
                ? parser.parseFromString(returedXml.data, "text/xml").getElementsByTagName("monthOfDeathTranId")[0].childNodes[0].nodeValue
                : null;

            var soj = parser.parseFromString(returedXml.data, "text/xml").getElementsByTagName("soj")[0].childNodes[0]
                ? parser.parseFromString(returedXml.data, "text/xml").getElementsByTagName("soj")[0].childNodes[0].nodeValue
                : null;

            var vetPtcpntId = parser.parseFromString(returedXml.data, "text/xml").getElementsByTagName("vetPtcpntId")[0].childNodes[0]
                ? parser.parseFromString(returedXml.data, "text/xml").getElementsByTagName("vetPtcpntId")[0].childNodes[0].nodeValue
                : null;

            var spousePtcpntId = parser.parseFromString(returedXml.data, "text/xml").getElementsByTagName("spousePtcpntId")[0].childNodes[0]
                ? parser.parseFromString(returedXml.data, "text/xml").getElementsByTagName("spousePtcpntId")[0].childNodes[0].nodeValue
                : null;

            if (monthOfDeathTranId) { 
                formHelper.setValue('va_modtranid', monthOfDeathTranId); 
            }
            if (soj) { 
                formHelper.setValue('va_modsoj', soj); 
            }
            if (vetPtcpntId) { 
                formHelper.setValue('va_modvetptcpntid', vetPtcpntId); 
            }
            if (spousePtcpntId) { 
                formHelper.getAttribute('va_modspouseptcpntid').setValue(spousePtcpntId);
            }

            showHideSpouseModSections(executionContext);

            return 'Eligible';
        }
    } catch (e) {
        showMessage("An Error Occurred", "Validate MOD Failed. Error: " + e.message);
    }
}

function GetCountryList(vetSearchCtx, executionContext) {
    try {
        var formContext = executionContext.getFormContext();
        var CountryListNodes;
        _country_list = [];
        _country_list_USA = 0;
        formHelper.getControl('va_spousecountrylist').clearOptions();
        var getCountryList = new findCountries(vetSearchCtx);
        getCountryList.suppressProgressDlg = true;
        var succesfulRequest = getCountryList.executeRequest();

        // Check for request failure
        if (!succesfulRequest) {
            throw { "message": "There was an issue with the Web Service request for country codes."};
        }

        var parser = new DOMParser();
        var CountryList_xmlObject = parser.parseFromString(getCountryList.responseXml, "text/xml");
        var returnNode = CountryList_xmlObject.getElementsByTagName("return")[0];
        CountryListNodes = returnNode.childNodes;      

        // Loop through countries
        if (CountryListNodes) {
            for (var i = 0; i < CountryListNodes.length; i++) {
                // Make sure we only have relevent nodes
                if (CountryListNodes[i].nodeName === 'types') {
                    var oOption = {};
                    var code = CountryListNodes[i].getElementsByTagName("code")[0].textContent;

                    if (isNaN(code)) {
                        oOption.value = i + 70000;
                    } else {
                        oOption.value = parseInt(code);
                    }

                    oOption.text = CountryListNodes[i].getElementsByTagName("name")[0].textContent;

                    if (oOption.text === "USA") {
                        _country_list_USA = oOption.value;
                    }

                    _country_list[_country_list.length] = oOption;
                    formContext.getControl('va_spousecountrylist').addOption(oOption);
                }
            }
        }
    }
    catch (e) {
        showMessage("An Error Occurred", "Get Country Codes failed. Error: " + e.message);
    }

}

function RetrieveSpouseInfo(xml, executionContext) {

    var spouseFirstName = xml.getElementsByTagName("spouseFirstName")[0].childNodes[0]
        ? xml.getElementsByTagName("spouseFirstName")[0].childNodes[0].nodeValue
        : null;
    //SingleNodeExists(mod_xmlObject, '//return/spouseFirstName')
    //? mod_xmlObject.selectSingleNode('//return/spouseFirstName').text : null;

    var spouseMiddleName = xml.getElementsByTagName("spouseMiddleName")[0].childNodes[0]
        ? xml.getElementsByTagName("spouseMiddleName")[0].childNodes[0].nodeValue
        : null;
    //SingleNodeExists(mod_xmlObject, '//return/spouseMiddleName')
    //? mod_xmlObject.selectSingleNode('//return/spouseMiddleName').text : null;

    var spouseLastName = xml.getElementsByTagName("spouseLastName")[0].childNodes[0]
        ? xml.getElementsByTagName("spouseLastName")[0].childNodes[0].nodeValue
        : null;
    //SingleNodeExists(mod_xmlObject, '//return/spouseLastName')
    //? mod_xmlObject.selectSingleNode('//return/spouseLastName').text : null;

    var spouseSuffix = xml.getElementsByTagName("spouseSuffix")[0].childNodes[0]
        ? xml.getElementsByTagName("spouseSuffix")[0].childNodes[0].nodeValue
        : null;
    //SingleNodeExists(mod_xmlObject, '//return/spouseSuffix')
    //    ? mod_xmlObject.selectSingleNode('//return/spouseSuffix').text : null;

    var spouseSsn = xml.getElementsByTagName("spouseSsn")[0].childNodes[0]
        ? xml.getElementsByTagName("spouseSsn")[0].childNodes[0].nodeValue
        : null;
    //SingleNodeExists(mod_xmlObject, '//return/spouseSsn')
    //? mod_xmlObject.selectSingleNode('//return/spouseSsn').text : null;

    var spouseDateOfBirth = xml.getElementsByTagName("spouseDateOfBirth")[0].childNodes[0]
        ? xml.getElementsByTagName("spouseDateOfBirth")[0].childNodes[0].nodeValue
        : null;
    //SingleNodeExists(mod_xmlObject, '//return/spouseDateOfBirth')
    //    ? mod_xmlObject.selectSingleNode('//return/spouseDateOfBirth').text : null;

    var spouseAddressLine1 = xml.getElementsByTagName("spouseAddressLine1")[0].childNodes[0]
        ? xml.getElementsByTagName("spouseAddressLine1")[0].childNodes[0].nodeValue
        : null;
    //SingleNodeExists(mod_xmlObject, '//return/spouseAddressLine1')
    //    ? mod_xmlObject.selectSingleNode('//return/spouseAddressLine1').text : null;

    var spouseAddressLine2 = xml.getElementsByTagName("spouseAddressLine2")[0].childNodes[0]
        ? xml.getElementsByTagName("spouseAddressLine2")[0].childNodes[0].nodeValue
        : null;
    //SingleNodeExists(mod_xmlObject, '//return/spouseAddressLine2')
    //    ? mod_xmlObject.selectSingleNode('//return/spouseAddressLine2').text : null;

    var spouseAddressLine3 = xml.getElementsByTagName("spouseAddressLine3")[0].childNodes[0]
        ? xml.getElementsByTagName("spouseAddressLine3")[0].childNodes[0].nodeValue
        : null;
    //SingleNodeExists(mod_xmlObject, '//return/spouseAddressLine3')
    //? mod_xmlObject.selectSingleNode('//return/spouseAddressLine3').text : null;

    var spouseCity = xml.getElementsByTagName("spouseCity")[0].childNodes[0]
        ? xml.getElementsByTagName("spouseCity")[0].childNodes[0].nodeValue
        : null;
    //SingleNodeExists(mod_xmlObject, '//return/spouseCity')
    //    ? mod_xmlObject.selectSingleNode('//return/spouseCity').text : null;

    var spouseState = xml.getElementsByTagName("spouseState")[0].childNodes[0]
        ? xml.getElementsByTagName("spouseState")[0].childNodes[0].nodeValue
        : null;
    //SingleNodeExists(mod_xmlObject, '//return/spouseState')
    //    ? mod_xmlObject.selectSingleNode('//return/spouseState').text : null;

    var spouseZipCode = xml.getElementsByTagName("spouseZipCode")[0].childNodes[0]
        ? xml.getElementsByTagName("spouseZipCode")[0].childNodes[0].nodeValue
        : null;
    //SingleNodeExists(mod_xmlObject, '//return/spouseZipCode')
    //    ? mod_xmlObject.selectSingleNode('//return/spouseZipCode').text : null;

    var spouseCountryTypeName = xml.getElementsByTagName("spouseCountryTypeName")[0].childNodes[0]
        ? xml.getElementsByTagName("spouseCountryTypeName")[0].childNodes[0].nodeValue
        : null;
    //SingleNodeExists(mod_xmlObject, '//return/spouseCountryTypeName')
    //    ? mod_xmlObject.selectSingleNode('//return/spouseCountryTypeName').text : null;

    var spouseForeignPostalCode = xml.getElementsByTagName("spouseForeignPostalCode")[0].childNodes[0]
        ? xml.getElementsByTagName("spouseForeignPostalCode")[0].childNodes[0].nodeValue
        : null;
    //SingleNodeExists(mod_xmlObject, '//return/spouseForeignPostalCode')
    //    ? mod_xmlObject.selectSingleNode('//return/spouseForeignPostalCode').text : null;

    var spouseMilitaryPostOfficeCode = xml.getElementsByTagName("spouseMilitaryPostOfficeCode")[0].childNodes[0]
        ? xml.getElementsByTagName("spouseMilitaryPostOfficeCode")[0].childNodes[0].nodeValue
        : null;
    //SingleNodeExists(mod_xmlObject, '//return/spouseMilitaryPostOfficeCode')
    //    ? mod_xmlObject.selectSingleNode('//return/spouseMilitaryPostOfficeCode').text : null;  //APO, DPO, FPO

    var spouseMilitaryPostalTypeCode = xml.getElementsByTagName("spouseMilitaryPostalTypeCode")[0].childNodes[0]
        ? xml.getElementsByTagName("spouseMilitaryPostalTypeCode")[0].childNodes[0].nodeValue
        : null;
    //SingleNodeExists(mod_xmlObject, '//return/spouseMilitaryPostalTypeCode')
    //    ? mod_xmlObject.selectSingleNode('//return/spouseMilitaryPostalTypeCode').text : null; //AA, AE, AP

    var spouseProvinceName = xml.getElementsByTagName("spouseProvinceName")[0].childNodes[0]
        ? xml.getElementsByTagName("spouseProvinceName")[0].childNodes[0].nodeValue
        : null;
    //SingleNodeExists(mod_xmlObject, '//return/spouseProvinceName')
    //    ? mod_xmlObject.selectSingleNode('//return/spouseProvinceName').text : null;

    var spouseTeritoryName = xml.getElementsByTagName("spouseTeritoryName")[0].childNodes[0]
        ? xml.getElementsByTagName("spouseTeritoryName")[0].childNodes[0].nodeValue
        : null;
    //SingleNodeExists(mod_xmlObject, '//return/spouseTeritoryName')
    //    ? mod_xmlObject.selectSingleNode('//return/spouseTeritoryName').text : null;


    formHelper.getAttribute('va_spousefirstname').setValue(NN(spouseFirstName));
    formHelper.getAttribute('va_spousemiddlename').setValue(NN(spouseMiddleName));
    formHelper.getAttribute('va_spouselastname').setValue(NN(spouseLastName));
    formHelper.getAttribute('va_spousesuffix').setValue(NN(spouseSuffix));
    formHelper.getAttribute('va_spousessn').setValue(NN(spouseSsn));
    var newDOB = null;
    if (spouseDateOfBirth && spouseDateOfBirth.length > 0) { newDOB = new Date(spouseDateOfBirth); }
    formHelper.getAttribute('va_spousedob').setValue(newDOB);
    formHelper.getAttribute('va_spouseaddress1').setValue(NN(spouseAddressLine1));
    formHelper.getAttribute('va_spouseaddress2').setValue(NN(spouseAddressLine2));
    formHelper.getAttribute('va_spouseaddress3').setValue(NN(spouseAddressLine3));
    formHelper.getAttribute('va_spousecity').setValue(NN(spouseCity));


    if (spouseState) {
        setOptionSet(executionContext, "va_spousestatelist", spouseState);
    }

    formHelper.getAttribute('va_spousezipcode').setValue(NN(spouseZipCode));


    var cName = NN(spouseCountryTypeName);
    if (cName && cName.length > 0) { cName = cName.toString().toUpperCase(); }
    formHelper.getAttribute('va_spousecountry').setValue(cName);

    formHelper.getAttribute('va_spouseforeignpostalcode').setValue(NN(spouseForeignPostalCode));
    formHelper.getAttribute('va_provincename').setValue(NN(spouseProvinceName));
    formHelper.getAttribute('va_territoryname').setValue(NN(spouseTeritoryName));

    //Updating treasury fields

    updateTreasuryFields();

    //end of treasury field update

    var temp = null;

    switch (spouseMilitaryPostalTypeCode) {
        case 'AA':
            temp = 953850000;
            break;
        case 'AE':
            temp = 953850001;
            break;
        case 'AP':
            temp = 953850002;
            break;
        default:
            break;
    }

    if (temp) {
        formHelper.getAttribute('va_spouseoverseasmilitarypostalcode').setValue(temp);  //AA, AE, AP
    }

    temp = null;

    switch (spouseMilitaryPostOfficeCode) {
        case 'APO':
            temp = 953850000;
            break;
        case 'DPO':
            temp = 953850001;
            break;
        case 'FPO':
            temp = 953850002;
            break;
        default:
            break;
    }

    if (temp) {
        formHelper.getAttribute('va_spouseoverseasmilitarypostofficetypecode').setValue(temp);  //APO, DPO, FPO 
    }


    if (spouseMilitaryPostOfficeCode || spouseMilitaryPostalTypeCode) {
        formHelper.getAttribute('va_spouseaddresstype').setValue(953850002); //overseas
    } else {

        if (spouseCountryTypeName === null || spouseCountryTypeName === '' || spouseCountryTypeName === 'USA') {
            formHelper.getAttribute('va_spouseaddresstype').setValue(953850000); //domestic
        } else {
            formHelper.getAttribute('va_spouseaddresstype').setValue(953850001); //international
        }
    }
}

//This function updates GUI/enforces business rules based on the dropdown "If spouse info is incorrect, please select from the following"
function RedrawSpouseFields(mode, executionContext) {
    var formContext = executionContext.getFormContext();
    _redrawing_spouse_action = true;
    //Redraw dropdown, items might have been removed
    formHelper.getControl('va_spouserecordaction').clearOptions();

    for (var i = 1; i <= spouse_action_options.length; i++) {
        if (spouse_action_options[spouse_action_options.length - i].value !== "null") {
            formHelper.getControl('va_spouserecordaction').addOption(spouse_action_options[spouse_action_options.length - i], i - 1);
        }
    }

    switch (mode) {
        case 'No selection':
            // Disable fields
            disableEnableSpouseFields(executionContext, true)
            break;
        case 'Add':
            // Enable fields
            disableEnableSpouseFields(executionContext, false)

            // Set SSN and DOB to null
            formContext.getAttribute("va_spousessn").setValue(null);
            formContext.getAttribute("va_spousedob").setValue(null);

            // Set the value of the dropdown after the redraw
            formHelper.setValue('va_spouserecordaction', 953850000);

            // Set "has spouse" to true
            if (formContext.getAttribute("va_veteranhassurvivingspouse").getValue() !== true) {
                setValueFireOnChange(executionContext, "va_veteranhassurvivingspouse", true);
            }
            break;
        case 'Modify':
            // Enable fields
            disableEnableSpouseFields(executionContext, false)

            // Set SSN and DOB to null
            formContext.getAttribute("va_spousessn").setValue(null);
            formContext.getAttribute("va_spousedob").setValue(null);

            // Set the value of the dropdown after the redraw
            formHelper.setValue('va_spouserecordaction', 953850001);
            break;
        case 'NOK':
            // Set fields to null
            formHelper.setValue('va_spousefirstname', null);
            formHelper.setValue('va_spousemiddlename', null);
            formHelper.setValue('va_spouselastname', null);
            formHelper.setValue('va_spousesuffix', null);
            formHelper.setValue('va_spousessn', null);
            formHelper.setValue('va_spousedob', null);

            // Disable fields
            disableEnableSpouseFields(executionContext, true)

            // Set the value of the dropdown after the redraw
            formHelper.setValue('va_spouserecordaction', 953850002);
            break;
        default:
            break;
    }
    _redrawing_spouse_action = false;
}

function disableEnableSpouseFields(executionContext, disabled) {
    try {
        // Get formContext
        var formContext = executionContext.getFormContext();

        // Disable/enable all occurrences of below fields
        disableEnableAllFieldControls(executionContext, 'va_spousefirstname', disabled);
        disableEnableAllFieldControls(executionContext, 'va_spousemiddlename', disabled);
        disableEnableAllFieldControls(executionContext, 'va_spouselastname', disabled);
        disableEnableAllFieldControls(executionContext, 'va_spousesuffix', disabled);

        // Only disable/enable first occurrence of the below
        formContext.getControl("va_spousessn").setDisabled(disabled);
        formContext.getControl("va_spousedob").setDisabled(disabled);
    } catch (e) {
        console.log("An error occurred within disableEnableSpouseFields(): " + e.message);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function updateTreasuryFields() {
    // Hide - this section is no longer needed on the form
    // formHelper.setTabVisible("treasuryadd", true);
    formHelper.setSectionVisible("spouse_section6", "mod_tab", true);
    formHelper.setSectionVisible("spouse_section7", "mod_tab", true);

    var spouseFirstName = formHelper.getValue('va_spousefirstname');
    var spouseMiddleName = formHelper.getValue('va_spousemiddlename');
    var spouseLastName = formHelper.getValue('va_spouselastname');
    var spouseSuffix = formHelper.getValue('va_spousesuffix');
    var spouseSsn = formHelper.getValue('va_spousessn');
    var spouseDateOfBirth = new Date(formHelper.getValue('va_spousedob'));
    var spouseAddressLine1 = formHelper.getValue('va_spouseaddress1');
    var spouseAddressLine2 = formHelper.getValue('va_spouseaddress2');
    var spouseAddressLine3 = formHelper.getValue('va_spouseaddress3');
    var spouseCity = formHelper.getValue('va_spousecity');
    var spouseState = (formHelper.getAttribute("va_spousestatelist").getSelectedOption() === null ? '' : formHelper.getAttribute("va_spousestatelist").getSelectedOption().text);
    var spouseZipCode = formHelper.getValue('va_spousezipcode');
    var spouseCountryTypeName = formHelper.getValue('va_spousecountry');
    if (spouseCountryTypeName && spouseCountryTypeName.length > 0) {
        spouseCountryTypeName = spouseCountryTypeName.toString().toUpperCase();
    }
    var spouseAddressType = (formHelper.getAttribute("va_spouseaddresstype").getSelectedOption() === null ? '' : formHelper.getAttribute("va_spouseaddresstype").getSelectedOption().text);
    var spouseMilitaryOfficeTypeCode = (formHelper.getAttribute("va_spouseoverseasmilitarypostofficetypecode").getSelectedOption() === null ? '' : formHelper.getAttribute("va_spouseoverseasmilitarypostofficetypecode").getSelectedOption().text); //APO
    var spouseMilitaryPostalCode = (formHelper.getAttribute("va_spouseoverseasmilitarypostalcode").getSelectedOption() === null ? '' : formHelper.getAttribute("va_spouseoverseasmilitarypostalcode").getSelectedOption().text); //AA

    var treasuryAddressArray = new Array(6);
    var addr2Present = false;
    var addr3Present = false;

    spouseFirstName = NN(spouseFirstName);
    spouseMiddleName = NN(spouseMiddleName);
    spouseLastName = NN(spouseLastName);

    //calculate full name
    var spouseFullName = "";
    spouseFullName = spouseFirstName;
    if (spouseMiddleName && spouseMiddleName.length > 0) spouseFullName += " " + spouseMiddleName;
    spouseFullName += " " + spouseLastName;

    spouseAddressLine1 = NN(spouseAddressLine1);
    spouseAddressLine2 = NN(spouseAddressLine2);
    spouseAddressLine3 = NN(spouseAddressLine3);
    spouseCity = NN(spouseCity);
    spouseState = NN(spouseState);
    spouseZipCode = NN(spouseZipCode);
    spouseCountryTypeName = NN(spouseCountryTypeName);

    //debugger;

    // per Wayne Dahlsing 6/14/12, zip code must NOT be in treasury fields
    var useZipCode = false;

    var j = 0;

    if ((!spouseFirstName && spouseFirstName.length === 0) || (!spouseLastName && spouseLastName.length === 0)) {
        spouseFullName = 'No Spouse';
    }

    treasuryAddressArray[j] = spouseFullName.substring(0, 20);

    if (spouseFullName && spouseFullName.length > 20) {
        j++;
        treasuryAddressArray[j] = spouseFullName.substring(20, 40);
    }

    j++;
    treasuryAddressArray[j] = spouseAddressLine1.substring(0, 20);

    if (spouseAddressLine1 && spouseAddressLine1.length > 20) {
        j++;
        treasuryAddressArray[j] = spouseAddressLine1.substring(20, 40);
    }

    if (spouseAddressLine2 && spouseAddressLine2.length > 0) addr2Present = true;
    if (spouseAddressLine3 && spouseAddressLine3.length > 0) addr3Present = true;

    // if either ADDR2 or ADDR3 are present, deal with formatting.
    if (addr2Present || addr3Present) {

        if (addr2Present && !addr3Present) {
            treasuryAddressArray[j + 1] = spouseAddressLine2.substring(0, 20);

            if (spouseAddressType === 'Overseas Military') {
                treasuryAddressArray[j + 2] = (spouseMilitaryOfficeTypeCode + " " + spouseMilitaryPostalCode).substring(0, 20);
                j = j + 2;
            } else {
                treasuryAddressArray[j + 2] = (spouseCity + " " + spouseState).substring(0, 20);
                //ticket 104329.. appending country name for int addresses
                if (spouseCountryTypeName && spouseCountryTypeName.length > 0 && spouseCountryTypeName !== 'USA') {
                    treasuryAddressArray[j + 3] = spouseCountryTypeName;
                    j = j + 3;
                } else {
                    j = j + 2;
                }
            }
        }

        if (!addr2Present && addr3Present) {
            treasuryAddressArray[j + 1] = spouseAddressLine3.substring(0, 20);

            if (spouseAddressType === 'Overseas Military') {
                treasuryAddressArray[j + 2] = (spouseMilitaryOfficeTypeCode + " " + spouseMilitaryPostalCode).substring(0, 20);
                j = j + 2;
            } else {
                treasuryAddressArray[j + 2] = (spouseCity + " " + spouseState).substring(0, 20);
                //ticket 104329.. appending country name for int addresses
                if (spouseCountryTypeName && spouseCountryTypeName.length > 0 && spouseCountryTypeName !== 'USA') {
                    treasuryAddressArray[j + 3] = spouseCountryTypeName;
                    j = j + 3;
                } else {
                    j = j + 2;
                }
            }
        }

        if (addr2Present && addr3Present) {
            treasuryAddressArray[j + 1] = spouseAddressLine2.substring(0, 20);
            treasuryAddressArray[j + 2] = spouseAddressLine3.substring(0, 20);

            if (spouseAddressType === 'Overseas Military') {
                treasuryAddressArray[j + 3] = (spouseMilitaryOfficeTypeCode + " " + spouseMilitaryPostalCode).substring(0, 20);
                j = j + 3;
            } else {
                treasuryAddressArray[j + 3] = (spouseCity + " " + spouseState).substring(0, 20);
                //ticket 104329.. appending country name for int addresses
                if (spouseCountryTypeName && spouseCountryTypeName.length > 0 && spouseCountryTypeName !== 'USA') {
                    treasuryAddressArray[j + 4] = spouseCountryTypeName;
                    j = j + 4;
                } else {
                    j = j + 3;
                }
            }
        }

    } else {

        if (spouseAddressType === 'Overseas Military') {
            treasuryAddressArray[j + 1] = (spouseMilitaryOfficeTypeCode + " " + spouseMilitaryPostalCode).substring(0, 20);
            j = j + 1;
        } else {
            treasuryAddressArray[j + 1] = (spouseCity + " " + spouseState).substring(0, 20);
            //ticket 104329.. appending country name for int addresses
            if (spouseCountryTypeName && spouseCountryTypeName.length > 0 && spouseCountryTypeName !== 'USA') {
                treasuryAddressArray[j + 2] = spouseCountryTypeName;
                j = j + 2;
            } else {
                j = j + 1;
            }
        }

    }

    formHelper.setValue("va_treasuryaddress1", treasuryAddressArray[0]);
    formHelper.setValue("va_treasuryaddress2", treasuryAddressArray[1]);
    formHelper.setValue("va_treasuryaddress3", treasuryAddressArray[2]);
    formHelper.setValue("va_treasuryaddress4", treasuryAddressArray[3]);
    formHelper.setValue("va_treasuryaddress5", treasuryAddressArray[4]);
    formHelper.setValue("va_treasuryaddress6", treasuryAddressArray[5]);

    if (j >= 6) {
        var message = "Warning: Unable to convert treasury address to the 6x20 format. Please update it manually.";
        Xrm.Navigation.openAlertDialog({ text: message }, { height: 300, width: 500 });
    }
}

function ValidateMODAddress(executionContext) {
    if (ValidateZipcode(executionContext) === false) {
        return false;
    }
    var success = ValidateMODFields();
    if (success) {
        Xrm.Navigation.openAlertDialog({ text: "Validation Succeeded" }, { height: 300, width: 500 });
    }
}

function ValidateZipcode(executionContext) {
    var formContext = executionContext.getFormContext();
    var va_spousezipcode = formContext.getAttribute('va_spousezipcode').getValue();
    if (va_spousezipcode !== null && va_spousezipcode.match(/[a-zA-Z]/)) {
        var message = 'Spouse/Last Known Address Zip Code field contains invalid alphabetical characters';
        Xrm.Navigation.openAlertDialog({ text: message }, { height: 300, width: 500 }).then(function () {
            return false;
        });
    }
    return true;
}

function TestForAllowedChars(text, nums) {
    var chars = {
        1: "[~]+",
        2: "[`]+",
        3: "[!]+",
        4: "[@]+",
        5: "[#]+",
        6: "[$]+",
        7: "[%]+",
        8: "[\\^]+",
        9: "[&]+",
        10: "[*]+",
        11: "[(]+",
        12: "[)]+",
        13: "[_]+",
        14: "[-]+",
        15: "[+]+",
        16: "[=]+",
        17: "[|]+",
        18: "[\\\\]+",
        19: "[}]+",
        20: "[]]+",
        21: "[{]+",
        22: "[[]+",
        23: "[']+",
        24: "[\"]+",
        25: "[:]+",
        26: "[;]+",
        27: "[\\/]+",
        28: "[?]+",
        29: "[.]+",
        30: "[>]+",
        31: "[,]+",
        32: "[<]+",
        33: "[  ]{2,}"
    };


    for (var remove in nums) {
        chars[nums[remove]] = null;
    }

    var success;
    for (var invalid in chars) {
        if (chars[invalid] !== null) {
            var myregex = new RegExp(chars[invalid]);
            success = !myregex.test(text);
        }
        if (success === false) break;
    }
    return success;
}

function ValidateMODFields() {
    var Errors = {};

    var fields = {
        va_spouseaddress1: 35,
        va_spouseaddress2: 35,
        va_spouseaddress3: 35,
        va_spousecity: 30,
        va_spousezipcode: 5
    };

    //Applies the max length restriction to fields.
    for (var field in fields) {
        var input = formHelper.getAttribute(field).getValue();
        if (input && input.length > fields[field]) {
            Errors[formHelper.getControl(field).getLabel()] = "Payment field length surpassed max value of " + fields[field] + ";";
        }
        if (formHelper.getControl(field).getLabel() === "Zip Code") {
            var zipSize = formHelper.getAttribute(field).getValue();
            if (zipSize && zipSize.length !== 5) {
                if (Errors[formHelper.getControl(field).getLabel()] !== undefined) {
                    Errors[formHelper.getControl(field).getLabel()] += "Field must be exactly 5 characters;";
                }
                else {
                    Errors[formHelper.getControl(field).getLabel()] = "Field must be exactly 5 characters;";
                }
            }
        }
        if (formHelper.getControl(field).getLabel() === "Address 1") {
            var success = TestForAllowedChars(formHelper.getAttribute(field).getValue(), [14, 27]);
            if (success === false) {
                if (Errors[formHelper.getControl(field).getLabel()] !== undefined) {
                    Errors[formHelper.getControl(field).getLabel()] += "Field can only contain alphanumeric characters, dashes, slashes, and single spaces;";
                }
                else {
                    Errors[formHelper.getControl(field).getLabel()] = "Field can only contain alphanumeric characters, dashes, slashes, and single spaces;";
                }
            }
        }

        var success = TestForAllowedChars(formHelper.getAttribute(field).getValue(), [33, 24, 5, 7, 9, 23, 11, 12, 15, 31, 14, 29, 27, 25, 4]);
        if (success === false) {
            if (Errors[formHelper.getControl(field).getLabel()] !== undefined) {
                Errors[formHelper.getControl(field).getLabel()] += "Field contains a non-allowable character;";
            }
            else {
                Errors[formHelper.getControl(field).getLabel()] = "Field contains a non-allowable character;";
            }
        }
    }

    if (parseInt(formHelper.getAttribute("va_spousezipcode").getValue()) === NaN) {
        if (Errors[formHelper.getControl("va_spousezipcode").getLabel()] !== undefined) {
            Errors[formHelper.getControl("va_spousezipcode").getLabel()] += 'Must be a number;';
        }
        else {
            Errors[formHelper.getControl("va_spousezipcode").getLabel()] = 'Must be a number;';
        }
    }

    var text = "";
    for (var a in Errors) {
        text += a + ": " + Errors[a] + "\n";
    }
    if (text !== "") {
        var message = "Validation Failed: \n\n" + text;
        Xrm.Navigation.openAlertDialog({ text: message }, { height: 300, width: 500 }).then(function () {
            return false;
        });
        //Va.Udo.Crm.Scripts.Popup.MsgBox(message, Va.Udo.Crm.Scripts.Popup.PopupStyles.Exclamation, "Warning");
        return false;
    }
    else {
        Xrm.Navigation.openAlertDialog({ text: "MOD fields validated" }, { height: 300, width: 500 }).then(function () {
            return true;
        });

    }
}

function NN(s) { return (s === null ? '' : s); }

function NormalizeCountry(country) {
    var words = country.split(' '), parsedCountry = '';
    for (var i = 0, l = words.length; i < l; i++) {
        // skip parent in a word
        var s = words[i].toLowerCase(), index = (s.substr(0, 1) === '(' ? 1 : 0), s1 = s.substr(index, 1).toUpperCase();
        words[i] = (index > 0 ? '(' : '') + s1 + s.substr(index + 1, words[i].length - 1 - index);

        parsedCountry += words[i] + (i < l - 1 ? ' ' : '');
    }
    return parsedCountry;
}

function usaaddress(executionContext) {
    try {
        var formContext = executionContext.getFormContext();
        var va_spouseaddresstype = formContext.getAttribute("va_spouseaddresstype").getValue();
        var nullFields = [];

        if (va_spouseaddresstype === 953850000) { //Domestic
            //if (_country_list_USA === 0 || _country_list_USA === undefined) {
            //    _country_list_USA = null;
            //}
            _country_list_USA = null;
            setValueFireOnChange(executionContext, "va_spousecountrylist", _country_list_USA);
            setValueFireOnChange(executionContext, "va_spousecountry", "USA");

            nullFields = ['va_spouseoverseasmilitarypostalcode', 'va_spouseoverseasmilitarypostofficetypecode'];
        }
        else if (va_spouseaddresstype === 953850001) { //International
            nullFields = ['va_spousestatelist', 'va_spousezipcode', 'va_spouseoverseasmilitarypostalcode', 'va_spouseoverseasmilitarypostofficetypecode'];
        }
        else if (va_spouseaddresstype === 953850002) { //Overseas Military
            nullFields = ['va_spousecity', 'va_spousestatelist', 'va_spousecountry', 'va_spousecountrylist'];
        }

        nullFields.forEach(function (fieldName) {
            setValueFireOnChange(executionContext, fieldName, null);
        });

        showHideSpouseAddressFields(executionContext);
        showHideSpouseModSections(executionContext);
        showHideCallerSpouseInfo(executionContext);

        return true;
    } catch (e) {
        console.log("An error occurred in usaaddress: " + e);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function showHideSpouseAddressFields(executionContext) {
    try {
        var formContext = executionContext.getFormContext();
        var spouseaddresstype = formContext.getAttribute("va_spouseaddresstype").getValue();
        var hiddenFields = [];
        var visibleFields = [];

        switch (spouseaddresstype) {
            case 953850000: //Domestic
                hiddenFields = ["va_spousecountry", "va_spouseoverseasmilitarypostalcode", "va_spouseoverseasmilitarypostofficetypecode"];
                visibleFields = ["va_spousecity", "va_spousestatelist", "va_spousezipcode"];
                break;
            case 953850001: //International: 
                hiddenFields = ["va_spousestatelist", "va_spousezipcode", "va_spouseoverseasmilitarypostalcode", "va_spouseoverseasmilitarypostofficetypecode"];
                visibleFields = ["va_spousecity", "va_spousecountry"];
                break;
            case 953850002: //Overseas Military
                hiddenFields = ["va_spousecity", "va_spousestatelist", "va_spousecountry"];
                visibleFields = ["va_spousezipcode", "va_spouseoverseasmilitarypostalcode", "va_spouseoverseasmilitarypostofficetypecode"];
                break;
        }

        hiddenFields.forEach(function (fieldName) {
            showHideAllFieldControls(executionContext, fieldName, false);
        });

        visibleFields.forEach(function (fieldName) {
            showHideAllFieldControls(executionContext, fieldName, true);
        });

        return true;
    } catch (e) {
        throw e;
    }
}

function callerAddressTypeOnChange(executionContext) {
    try {
        setCallerAddressFieldValues(executionContext);
        showHideCallerAddressFields(executionContext);
        showHideCallerSpouseInfo(executionContext);
    } catch (e) {
        console.log("An unexpected error occurred in callerAddressTypeOnChange: " + e.message);
        throw e;
    }
}

function showHideCallerAddressFields(executionContext) {
    try {
        var formContext = executionContext.getFormContext();
        var addressType = formContext.getAttribute("udo_calleraddresstype").getText();
        var hiddenFields = [];
        var visibleFields = [];

        switch (addressType) {
            case "Overseas Military":
                hiddenFields = ["udo_callercity", "udo_callerstatelist", "udo_callerzipcode", "udo_callercountry"];
                visibleFields = ["udo_calleroverseasmilitarypostalcode", "udo_calleroverseasmilitarypostofficetypecode", "udo_callerforeignpostalcode"];
                break;
            case "Domestic":
                hiddenFields = ["udo_callercountry", "udo_calleroverseasmilitarypostalcode", "udo_calleroverseasmilitarypostofficetypecode", "udo_callerforeignpostalcode"];
                visibleFields = ["udo_callercity", "udo_callerstatelist", "udo_callerzipcode"];
                break;
            case "International":
                hiddenFields = ["udo_callerstatelist", "udo_callerzipcode", "udo_calleroverseasmilitarypostalcode", "udo_calleroverseasmilitarypostofficetypecode", "udo_callerforeignpostalcode"];
                visibleFields = ["udo_callercity", "udo_callercountry"];
                break;
            default:
                break;
        }

        hiddenFields.forEach(function (fieldName) {
            showHideAllFieldControls(executionContext, fieldName, false);
        });

        visibleFields.forEach(function (fieldName) {
            showHideAllFieldControls(executionContext, fieldName, true);
        });

        return true;
    } catch (e) {
        console.log("An error occurred during showHideCallerAddressFields: " + e.message);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function setCallerAddressFieldValues(executionContext) {
    try {
        var formContext = executionContext.getFormContext();
        var addressType = formContext.getAttribute("udo_calleraddresstype").getText();
        var nullFields = [];

        switch (addressType) {
            case "Overseas Military":
                nullFields = ["udo_callercity", "udo_callerstatelist", "udo_callerzipcode", "udo_callercountry"];
                break;
            case "Domestic":
                nullFields = ["udo_calleroverseasmilitarypostalcode", "udo_calleroverseasmilitarypostofficetypecode", "udo_callerforeignpostalcode"];
                setValueFireOnChange(executionContext, "udo_callercountry", "USA");
                break;
            case "International":
                nullFields = ["udo_callerstatelist", "udo_callerzipcode", "udo_calleroverseasmilitarypostalcode", "udo_calleroverseasmilitarypostofficetypecode", "udo_callerforeignpostalcode"];
                break;
            default:
                break;
        }

        nullFields.forEach(function (fieldName) {
            setValueFireOnChange(executionContext, fieldName, null);
        });

        return true;
    } catch (e) {
        console.log("An error occurred during setCallerAddressFieldValues: " + e.message);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function setupForm(executionObj) {
    try {
        //initializeCommonVariables(executionObj);
        initiateCommonScripts(executionObj).then(function () {
            //var createdBy = formContext.getControl("createdby");
            //var udCreatedBy = formContext.getControl("udo_udcreatedby");
            //var udCreatedByAttr = formContext.getAttribute("udo_udcreatedby");
            var createdBy = formHelper.getControl("createdby");
            var udCreatedBy = formHelper.getControl("udo_udcreatedby");
            var udCreatedByAttr = formHelper.getAttribute("udo_udcreatedby");
            if (udCreatedByAttr.getValue()) {
                createdBy.setVisible(false);
                udCreatedBy.setVisible(true);
            }
            else {
                createdBy.setVisible(true);
                udCreatedBy.setVisible(false);
            }

            environmentConfigurations.initalize().then(function (data) {
                commonFunctions.initalize(executionObj);
                ws.vetRecord.initalize(executionObj);
                ws.claimant.initalize(executionObj);
                ws.benefitClaim.initalize(executionObj);
                ws.shareStandardData.initalize(executionObj);

                webResourceUrl = globCon.getClientUrl() + '/WebResources/va_';
                var getA = formHelper.getAttribute;

                getA('va_filenumber').setSubmitMode('always');
                getA('va_ranfnod').setSubmitMode('always');
                getA('va_ransync').setSubmitMode('always');
                getA('va_ranpmc').setSubmitMode('always');
                getA('va_ranmod').setSubmitMode('always');
                getA('va_ranfindmod').setSubmitMode('always');
                getA('va_modeligible').setSubmitMode('always');
                getA('va_createdsr').setSubmitMode('always');
                getA('va_fnodrequeststatus').setSubmitMode('always');
                getA('va_pmcrequeststatus').setSubmitMode('always');
                getA('va_modrequeststatus').setSubmitMode('always');
                getA('va_veteranhassurvivingspouse').setSubmitMode('always');
                getA('va_survivingspouseisvalidformod').setSubmitMode('always');
                getA('va_typeofnotice').setSubmitMode('always');
                getA('va_spouserecordaction').setSubmitMode('always');
                getA('va_webserviceresponse').setSubmitMode('always');
                getA('va_findcorprecordresponse').setSubmitMode('always');
                getA('va_generalinformationresponsebypid').setSubmitMode('always');
                getA('va_fnodrequest').setSubmitMode('always');
                getA('va_fnodresponse').setSubmitMode('always');
                getA('va_findmonthofdeathrequest').setSubmitMode('always');
                getA('va_findmonthofdeathresponse').setSubmitMode('always');
                getA('va_updatemonthofdeathrequest').setSubmitMode('always');
                getA('va_synccorpandbirlsresponse').setSubmitMode('always');
                getA('va_updatemonthofdeathresponse').setSubmitMode('always');
                getA('va_findpmcrequest').setSubmitMode('always');
                getA('va_findpmcresponse').setSubmitMode('always');
                getA('va_insertpmcrequest').setSubmitMode('always');
                getA('va_insertpmcresponse').setSubmitMode('always');
                //getA('va_fnodresults').setSubmitMode('always');
                //getA('va_modresults').setSubmitMode('always');

                //"Hidden" fields
                getA('va_modprocesstype').setSubmitMode('always');
                getA('va_modsoj').setSubmitMode('always');
                getA('va_modtranid').setSubmitMode('always');
                getA('va_modvetptcpntid').setSubmitMode('always');
                getA('va_modspouseptcpntid').setSubmitMode('always');

                //Spouse Theory
                getA('va_spousefirstname').setSubmitMode('always');
                getA('va_spousemiddlename').setSubmitMode('always');
                getA('va_spouselastname').setSubmitMode('always');
                getA('va_spousesuffix').setSubmitMode('always');
                getA('va_spousessn').setSubmitMode('always');
                getA('va_spousedob').setSubmitMode('always');
                getA('va_spouseaddress2').setSubmitMode('always');
                getA('va_spouseaddress3').setSubmitMode('always');
                getA('va_spousecity').setSubmitMode('always');
                getA('va_spousestatelist').setSubmitMode('always');
                getA('va_spousezipcode').setSubmitMode('always');
                getA('va_spousecountry').setSubmitMode('always');
                getA('va_spousecountrylist').setSubmitMode('always');
                getA('va_spouseaddresstype').setSubmitMode('always');
                getA('va_spouseoverseasmilitarypostalcode').setSubmitMode('always');
                getA('va_spouseoverseasmilitarypostofficetypecode').setSubmitMode('always');
                getA('va_spouseforeignpostalcode').setSubmitMode('always');
                getA('va_provincename').setSubmitMode('always');
                getA('va_territoryname').setSubmitMode('always');

                //Treasury fields
                getA('va_treasuryaddress1').setSubmitMode('always');
                getA('va_treasuryaddress2').setSubmitMode('always');
                getA('va_treasuryaddress3').setSubmitMode('always');
                getA('va_treasuryaddress4').setSubmitMode('always');
                getA('va_treasuryaddress5').setSubmitMode('always');
                getA('va_treasuryaddress6').setSubmitMode('always');

                setUpButtons("WebResource_UpdateVetData", executionObj);

                var vetSearchCtx = new vrmContext(exCon);

                //getA("va_ranfnod").setValue(true);
                getA("va_ranpmc").setValue(false);
                //getA("va_ranmod").setValue(true);

                GetUserSettingsForWebService()
                    .then(function (userData) {
                        _UserSettings = userData;
                        vetSearchCtx.user = userData;
                        GetCountryList(vetSearchCtx, executionObj);

                        // Validate MOD Eligibility
                        ValidateMOD(executionObj);

                        if (getA('va_ranfnod').getValue() === true) {
                            formHelper.setDisabled("va_dateofdeath", true);
                            formHelper.setDisabled("va_causeofdeath", true);
                        }
                    })
                    .catch(function (error) {
                        console.log("An error occurred in GetUserSettingsForWebService: " + error);
                    });

                var onSpouseCountryListChange = function () {
                    //var varMyValue = $("#va_spousecountrylist option:selected");
                    var varMyValue = formHelper.getAttribute("va_spousecountrylist");
                    if (varMyValue !== null) {
                        var cName = varMyValue.getText();
                        // Defect 96137 For Country code, other than USA, use the full name but make it Upper and lower e.g. France or Sri Lanka
                        if (cName && cName.length > 0 && cName !== 'US' && cName !== 'USA') {
                            cName = NormalizeCountry(cName);
                        } // cName.toString().toUpperCase(); }
                        formHelper.setValue("va_spousecountry", cName);
                    }
                }

                //commented out call, test.
                function onChange(el, func) {
                    // We use click instead of onchange due to ie11 crash on drop down onchange event when the size is updated.
                    // The size is updated in the addOption CRM function
                    //debugger;
                    if (el.attachEvent) {
                        el.attachEvent('onclick', func);
                    }
                    else if (el.addEventListener) {
                        el.addEventListener('click', func);
                    }
                }

                //onChange($("#va_spousecountrylist select")[0], onSpouseCountryListChange);
                //onChange($("#va_spouserecordaction select")[0], onSpouseRecordActionChange);

                formHelper.getAttribute("va_spousecountrylist").addOnChange(onSpouseCountryListChange);


                //save dropdown choices into a global variable
                spouse_action_options = getA("va_spouserecordaction").getOptions();


                if (getA('va_ranpmc').getValue() && getA('va_ranfnod').getValue() && getA('va_ranmod').getValue()) {
                    disableFnodModPmcControls(executionObj);
                }

                var createdon = getA('createdon').getValue();
                var today = new Date().setHours(0, 0, 0, 0);

                if (createdon) {
                    createdon = createdon.setHours(0, 0, 0, 0);

                    if (new Date(createdon) < new Date(today)) {
                        //DisableAll();
                    }
                }

                // Commenting, as this tab and section is no longer shown on the form
				/*
                if (getA('va_ranpmc').getValue() || getA('va_ranfnod').getValue() || getA('va_ranmod').getValue()) {
                    formHelper.setSectionVisible("execution_results", "general_tab", true);
                }
                else {
                    formHelper.setSectionVisible("execution_results", "general_tab", false);
                }
				*/

                //if MOD has already been run, hide buttons inlcuding Validate MOD, and show all MOD fields.
                if (getA('va_ranmod').getValue()) {
                    DisableMODFields();
                }

                // on closing if ran fnod, but not MOD and either didn't check mod elig, or is elig, prompt to run MOD

                addbeforeunloadEvent(function () {
                    if (!_ranFnodThisTime && !_modPrompt) {    // _ranFnodThisTime us true if we just executed FNOD, which auto triggers save
                        var getA = formHelper.getAttribute;
                        var isClosing = true;

                        if (isClosing && getA('va_ranfnod').getValue() === true && getA('va_ranmod').getValue() !== true &&
                            (getA('va_ranfindmod').getValue() !== true || getA('va_modeligible').getValue() === true)) {
                            _modPrompt = true;
                            if (!confirm('You have executed FNOD but did not execute MOD. Please confirm that you would like to close this screen without executing MOD.\n\nTo close the screen, click OK. To return back to the screen and execute MOD, click CANCEL.')) {
                                // RU12 Remove event.returnValue = false;
                                var evArgs = executionObj.getEventArgs();
                                if (evArgs !== null) { evArgs.preventDefault(); }
                                return false;
                            }
                        }
                    }
                });

                // Commenting, as this tab and section is no longer shown on the form
				/*
                // FNOD Ran, Show or Hide ExecutionResults
                if (formHelper.getValue('va_ranpmc') || formHelper.getValue('va_ranfnod') || formHelper.getValue('va_ranmod')) {
                    formHelper.setSectionVisible("execution_results", "general_tab", true);
                } else {
                    formHelper.setSectionVisible("execution_results", "general_tab", false);
				}
				*/
            }).catch(function (error) {
                console.log("An error occurred in environmentConfigurations.initalize.then: " + error);
                showMessage("An Error Occurred", error.message);
            });
        });

        /////
        // Setup FNODProcess BPF Navigation
        /////
        runBpfEvents(executionObj);

        /////
        // Initialize Form
        /////
        getVBMSRoleAsync(executionObj);
        showHideSpouseAddressFields(executionObj);
        showHideSpouseModSections(executionObj);
        showHideCallerAddressFields(executionObj);
        addTabStateEventHandlers(executionObj);
        populateCallerInfo(executionObj);
        callerAddressTypeOnChange(executionObj);
        usaaddress(executionObj);
        validateDateOfDeathNotFuture(executionObj);
        //var wrControl = formHelper.getControl("WebResource_IssuePMC");
        //if (wrControl !== 'undefined' && wrControl !== null) {
        //    wrControl.getContentWindow().then(
        //        function (contentWindow) {
        //            contentWindow.setContext123(exCon);
        //        }
        //    )
        //}

    }
    catch (e) {
        showMessage("An Error Occurred", "Encountered an error: " + e);
    }
    //CloseProgress();   
}

function setUpButtons(iFrameName, executionContext) {
    try {
        var formContext = executionContext.getFormContext();
        var source = formHelper.ui().controls.get(iFrameName);
        //var webResourceUrl = globeCon.getClientUrl() + '/WebResources/va_';

        function createButtonClick(clickHandler) {
            return function () {
                clickHandler();
                rebuildAllButtonsAsync(executionContext);
            }
        }

        switch (source.getLabel()) {
            case "SubmitFNOD":
                var organizationSettings = Xrm.Utility.getGlobalContext().organizationSettings;
                var orgName = organizationSettings.uniqueName;
                //var isprod = orgName.includes("prod");
                var isprod = orgName.indexOf("-prod");
                if (isprod > 0) {
                    Va.Udo.Crm.Uci.Scripts.Buttons.AddButton(formContext, source.getName(), "Submit FNOD (irreversible)", "Click to irreversably submit FNOD",
                        createButtonClick(function () { ExecuteFNOD(executionContext); }), null);
                }
                else {
                    function buildSubmitFnodButton(enableButton) {
                        Va.Udo.Crm.Uci.Scripts.Buttons.AddButton(formContext, source.getName(), "Submit FNOD (irreversible)", "Click to irreversably submit FNOD", createButtonClick(function () { ExecuteFNOD(executionContext); }), null, enableButton);
                    }

                    try {
                        // Check config value to confirm if button should be disabled
                        webApi.RetrieveMultiple("mcs_setting", ["udo_disablefnodsubmission"], "$filter=mcs_name eq 'Active Settings'").then(
                            function (result) {
                                if (result !== undefined && result !== null && result.length === 1 && result[0]["udo_disablefnodsubmission"] === true) {
                                    return Promise.resolve(true);
                                } else {
                                    return Promise.resolve(false);
                                }
                            },
                            function (error) {
                                console.log("A web service error occurred within setUpButtons.SubmitFNOD: " + error);
                                return Promise.resolve(false);
                            }
                        ).then(
                            function (result) {
                                buildSubmitFnodButton(!result);
                            }
                        );
                    } catch (e) {
                        console.log("An error occurred within setUpButtons.SubmitFNOD: " + e);
                        buildSubmitFnodButton(true);
                    }
                }
                break;
            case "IssuePMC":
                Va.Udo.Crm.Uci.Scripts.Buttons.AddButton(formContext, source.getName(), "Issue PMC", "Click to Issue PMC",
                    createButtonClick(function () { IssuePMC(executionContext); }), null);
                break;
            case "CopyPMC":
                Va.Udo.Crm.Uci.Scripts.Buttons.AddButton(formContext, source.getName(), "Copy Existing to new (optional)", "Click to copy the existing PMC to new",
                    createButtonClick(function () { CopyPMC(); }), null);
                break;
            case "SubmitMOD":
                Va.Udo.Crm.Uci.Scripts.Buttons.AddButton(formContext, source.getName(), "Submit MOD/NOK/LETTERS", "Click to submit MOD",
                    createButtonClick(function () { ExecuteMOD(executionContext); }), null);
                break;
			/*
			case "ValidateMOD":
				Va.Udo.Crm.Scripts.Buttons.AddButton(formContext, source.getName(), "Validate MOD Eligibility", "Click to validate MOD eligibility",
					function () {
						ValidateMOD();
					}, null);
				break;
			*/
            case "GenerateTreasuryAddress":
                Va.Udo.Crm.Uci.Scripts.Buttons.AddButton(formContext, source.getName(), "Generate/Update Treasury Address", "Click to generate or update treasury address",
                    createButtonClick(function () { updateTreasuryFields(); }), null);
                break;
            case "ValidateMODFields":
                Va.Udo.Crm.Uci.Scripts.Buttons.AddButton(formContext, source.getName(), "Validate MOD Address", "Click to validate MOD address",
                    createButtonClick(function () { ValidateMODAddress(executionContext); }), null);
                break;
            case "CopyLastKnowAddress":
                Va.Udo.Crm.Uci.Scripts.Buttons.AddButton(formContext, source.getName(), "Copy from Last Known Address", "Click to copy from last known address",
                    createButtonClick(function () { CopyLastKnownAddressToSpouseFields(executionContext); }), null);
                break;
            case "UpdateVetData":
                Va.Udo.Crm.Uci.Scripts.Buttons.AddButton(formContext, source.getName(), "Refresh Veteran's Data", "Click to refresh the veteran's data",
                    createButtonClick(function () { RetrieveVetInfo(); }), null);
                break;
            case "CopyCallerInfo":
                Va.Udo.Crm.Uci.Scripts.Buttons.AddButton(
                    formContext,
                    source.getName(),
                    "Copy Caller Info to Surviving Spouse",
                    "Click to copy the caller information to the surviving spouse",
                    createButtonClick(function () {
                        copyCallerToSpouse(executionContext);
                    }),
                    null
                );
                break;
            case "Preview0820F":
                Va.Udo.Crm.Uci.Scripts.Buttons.AddButton(
                    formContext,
                    source.getName(),
                    "Preview 0820F",
                    "Preview 0820F",
                    createButtonClick(function () {
                        if (formContext.getAttribute("va_spousessn").getValue() !== null && formContext.getAttribute("va_spousedob").getValue()) {
                            generateLetterAsync(executionContext, "0820F-FNOD", source.getName()).then(function (result) {
                                if (result) {
                                    formContext.getAttribute("udo_ran0820f").setValue(true);
                                    CreatePreview0820fNote(executionContext);
                                }
                            });
                        } else {
                            showMessage("Alert", "Surviving Spouse's SSN and DOB are required if processing the 0820F.");
                        }
                    }),
                    null
                );
                break;
            case "Preview0820A":
                Va.Udo.Crm.Uci.Scripts.Buttons.AddButton(
                    formContext,
                    source.getName(),
                    "Preview 0820A",
                    "Preview 0820A",
                    createButtonClick(function () {
                        // Check that mandatory toggle fields are 'Yes'


                        generateLetterAsync(executionContext, "0820A-FNOD", source.getName()).then(function (result) {
                            if (result) {
                                formContext.getAttribute("udo_ran0820a").setValue(true);
                            }
                        });
                    }),
                    null
                );
                break;
            case "VBMSUpload":
                Va.Udo.Crm.Uci.Scripts.Buttons.AddButton(
                    formContext,
                    source.getName(),
                    "VBMS Upload",
                    "VBMS Upload",
                    createButtonClick(function () {
                        var formContext = executionContext.getFormContext();
                        var uploadRole = formContext.getAttribute("udo_0820avbmsuploadrole").getValue();
                        var docType = formContext.getAttribute("udo_0820avbmsdoctype").getValue();

                        if (uploadRole === null || docType === null) {
                            showMessage("Alert", "'VBMS Upload Role' and 'VBMS Doc Type' are required in order to initiate a VBMS Upload.");
                        } else {
                            initiateVbmsUpload(executionContext, "0820A");
                        }
                    }),
                    null
                );
                break;
            case "ViewLetter":
                Va.Udo.Crm.Uci.Scripts.Buttons.AddButton(
                    formContext,
                    source.getName(),
                    "View Letter",
                    "View Letter",
                    createButtonClick(function () {
                        CreateDBLNote(executionContext);
                        openDblCrmReport(executionContext, "Death Benefits Letter - FNOD", formContext.data.entity.getId());
                        formContext.getAttribute("udo_randbl").setValue(true);
                    }),
                    null
                );
                break;
            case "DownloadPDF":
                Va.Udo.Crm.Uci.Scripts.Buttons.AddButton(
                    formContext,
                    source.getName(),
                    "Download PDF",
                    "Download PDF",
                    createButtonClick(function () {
                        CreateDBLNote(executionContext);
                        generateLetterAsync(executionContext, "Death Benefit Letter - FNOD", source.getName(), null, null, null, null, "PDF").then(function (result) {
                            if (result) {
                                formContext.getAttribute("udo_randbl").setValue(true);
                            }
                        });
                    }),
                    null
                );
                break;
            case "DownloadWord":
                Va.Udo.Crm.Uci.Scripts.Buttons.AddButton(
                    formContext,
                    source.getName(),
                    "Download Word",
                    "Download Word",
                    createButtonClick(function () {
                        CreateDBLNote(executionContext);
                        generateLetterAsync(executionContext, "Death Benefit Letter - FNOD", source.getName(), null, null, null, null, "word").then(function (result) {
                            if (result) {
                                formContext.getAttribute("udo_randbl").setValue(true);
                            }
                        });
                    }),
                    null
                );
                break;
            case "LetterVBMSUpload":
                Va.Udo.Crm.Uci.Scripts.Buttons.AddButton(
                    formContext,
                    source.getName(),
                    "VBMS Upload",
                    "VBMS Upload",
                    createButtonClick(function () {
                        var formContext = executionContext.getFormContext();
                        var uploadRole = formContext.getAttribute("udo_dblvbmsuploadrole").getValue();
                        var docType = formContext.getAttribute("udo_dblvbmsdoctype").getValue();

                        if (uploadRole === null || docType === null) {
                            showMessage("Alert", "'VBMS Upload Role' and 'VBMS Doc Type' are required in order to initiate a VBMS Upload.");
                        } else {
                            initiateVbmsUpload(executionContext, "DBL");
                        }
                    }),
                    null
                );
                break;
            case "Enclosures":
                Va.Udo.Crm.Uci.Scripts.Buttons.AddButton(
                    formContext,
                    source.getName(),
                    "Populate Enclosures",
                    "Populate Enclosures",
                    createButtonClick(function () {
                        populateEnclosures(executionContext);
                    }),
                    null
                );
                break;
        }
    } catch (e) {
        console.log("An error occurred in setUpButtons: " + e.message);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}
//Looks like unused.
function spouseRecordActionChange(executionContext) {
    var formContext = executionContext.getFormContext();

    usaaddress(executionContext);
    if (_redrawing_spouse_action === true) return; // Do nothing if it is during a redraw
    _redrawing_spouse_action = true;
    var actionAttr = formContext.getAttribute("va_spouserecordaction");
    var action = '';
    if (actionAttr !== null && actionAttr.getText() !== null) action = actionAttr.getText();

    var mode = '';
    switch (action) {
        case 'Add New Spouse':
            mode = 'Add';
            break;
        case 'Modify Existing Spouse':
            mode = 'Modify';
            break;
        case 'Send Next of Kin Letter':
            mode = 'NOK';
            break;
        default:
            mode = 'No selection';
            break;
    }

    RedrawSpouseFields(mode, executionContext);
    _redrawing_spouse_action = false;

}

function showMessage(title, message) {
    var alertStrings = {
        confirmButtonLabel: "OK",
        text: message,
        title: title
    };
    return Xrm.Navigation.openAlertDialog(alertStrings);
}

function ExecuteSsaInquiry() {
    function getSsaInquirySettings() {
        return new Promise(function (resolve, reject) {
            try {
                webApi.RetrieveMultiple("mcs_setting", ["udo_ssainquirysetting"], "$filter=mcs_name eq 'Active Settings'")
                    .then(function (data) {
                        if (data !== undefined && data !== null && data.length === 1 && data[0]["udo_ssainquirysetting"] === true) {
                            resolve(true);
                        } else {
                            resolve(false);
                        }
                    });
            } catch (e) {
                console.log("An error occurred in getSsaInquirySettings: " + e);
                reject(e);
            }
        });
    }

    function getDob() {
        function addZero(dateString) {
            if (dateString.length === 1) {
                dateString = "0" + dateString;
            }

            return dateString
        }

        var dob = formHelper.getValue("va_spousedob");
        var month = addZero((dob.getMonth() + 1).toString());
        var day = addZero(dob.getDate().toString());
        var year = dob.getFullYear().toString();

        var dateString = month + day + year;

        return dateString;
    }

    function validateSpouseInfo() {
        var continueToken = true;
        var fieldsToValidate = ["va_spousessn", "va_spousefirstname", "va_spouselastname"];

        return new Promise(function (resolve, reject) {
            try {
                if (formHelper.getValue("va_veteranhassurvivingspouse") !== true) {
                    continueToken = false;
                } else {
                    fieldsToValidate.forEach(function (field) {
                        if (formHelper.getValue(field) === undefined || formHelper.getValue(field) === null) {
                            continueToken = false;
                        }
                    });
                }

                resolve(continueToken);
            } catch (e) {
                console.log("An error occurred in validateSpouseInfo: " + e);
                reject(e);
            }
        });
    }

    return new Promise(function (resolve, reject) {
        try {
            var continueToken = true;

            // Validate surviving spouse info
            validateSpouseInfo()
                .then(function (cont) {
                    if (continueToken === true && cont === true) {
                        // Get SSA inquiry setting
                        return getSsaInquirySettings();
                    } else {
                        continueToken = false;
                        return false;
                    }
                }).then(function (runSsaInquiry) {
                    if (continueToken === true && runSsaInquiry === true) {
                        // Execute custom action
                        return webApi.executeAction(
                            {
                                ParentEntityReference: {
                                    entityType: "va_fnod",
                                    id: formHelper.getCurrentRecordIdFormatted()
                                },
                                FileNumber: formHelper.getValue("va_spousessn"),
                                DOB: getDob(),
                                FirstName: formHelper.getValue("va_spousefirstname"),
                                LastName: formHelper.getValue("va_spouselastname"),
                                VetFileNumber: formHelper.getValue("va_filenumber"),
                                getMetadata: function () {
                                    return {
                                        boundParameter: null,
                                        parameterTypes: {
                                            ParentEntityReference: {
                                                typeName: "mscrm.va_fnod",
                                                structuralProperty: 5
                                            },
                                            FileNumber: {
                                                typeName: "Edm.String",
                                                structuralProperty: 1
                                            },
                                            DOB: {
                                                typeName: "Edm.String",
                                                structuralProperty: 1
                                            },
                                            FirstName: {
                                                typeName: "Edm.String",
                                                structuralProperty: 1
                                            },
                                            LastName: {
                                                typeName: "Edm.String",
                                                structuralProperty: 1
                                            },
                                            VetFileNumber: {
                                                typeName: "Edm.String",
                                                structuralProperty: 1
                                            }
                                        },
                                        operationType: 0,
                                        operationName: "udo_SsaDeathMatchInquiry",
                                    };
                                }
                            });
                    } else {
                        continueToken = false;
                        return false;
                    }
                }).then(function (result) {
                    if (continueToken === true && result.ok) {
                        return result.json();
                    } else {
                        continueToken = false;
                        return false;
                    }
                }, function (error) {
                    console.log("An error occurred during SSA Inquiry: ", error.message);
                    continueToken = false;
                }).then(function (response) {
                    if (continueToken === true && response.DateOfDeath) {
                        Xrm.Utility.closeProgressIndicator();
                        return showMessage("Unable to Submit MOD", "SSA Inquiry reports the spouse passed away, 0820F should not be processed.");
                    } else if (continueToken === true && (response.Timeout || response.Exception || response.DataIssue)) {
                        console.log("An error occurred during SSA Inquiry: ", response.ResponseMessage);
                        continueToken = false;
                        return false;
                    } else {
                        continueToken = false;
                        return false;
                    }
                }).then(function success(messageResult) {
                    resolve(!continueToken);
                }).catch(function (failedError) {
                    console.log("An error occurred during SSA Inquiry (inner): " + failedError);
                    resolve(true);
                });
        } catch (e) {
            console.log("An error occurred during SSA Inquiry: " + e.message);
            rebuildAllButtonsAsync(execContext);
            throw (e);
            resolve(true);
        }
    });
}

function hideCallerSpouseInfo(executionContext, toggleVal) {
    try {
        var formContext = executionContext.getFormContext();

        if (toggleVal === null || toggleVal === undefined) {
            toggleVal = false;
        }

        setValueFireOnChange(executionContext, "udo_showcallerinformationandsurvivingspouse", toggleVal);
        return true;
    } catch (e) {
        console.log("An error occurred when attempting to hide caller and spouse info: " + e.message);
        rebuildAllButtonsAsync(execContext);
        throw (e);
        return false;
    }
}

function getFnodBpfTabSets() {
    try {
        var FNODProcessArray = [
            {
                title: "First Notice Of Death",
                tab: "fnod_tab",
                section: "fnod_tab_nextprevious_section",
                webresource: "WebResource_FNODNavigationButtons",
                formButtons: ["WebResource_CopyCallerInfo", "WebResource_SubmitFNOD"]
            },
            {
                title: "Month Of Death",
                tab: "mod_tab",
                section: "mod_tab_nextprevious_section",
                webresource: "WebResource_MODNavigationButtons",
                formButtons: ["WebResource_ValidateMODFields", "WebResource_SubmitMOD", "WebResource_GenerateTreasuryAddress", "WebResource_CopyLastKnowAddress"]
            },
            {
                title: "Service Request - 0820F",
                tab: "0820f_tab",
                section: "0820f_tab_nextprevious_section",
                webresource: "WebResource_0820FNavigationButtons",
                formButtons: ["WebResource_Preview0820F"]
            },
            {
                title: "Presidential Memorial Certificate",
                tab: "pmc_tab",
                section: "pmc_tab_nextprevious_section",
                webresource: "WebResource_PMCNavigationButtons",
                formButtons: ["WebResource_CopyPMC", "WebResource_IssuePMC"]
            },
            {
                title: "Service Request - 0820A",
                tab: "0820a_tab",
                section: "0820a_tab_nextprevious_section",
                webresource: "WebResource_0820ANavigationButtons",
                formButtons: ["WebResource_Preview0820A", "WebResource_VBMSUpload"]
            },
            {
                title: "Death Benefit Letter",
                tab: "deathbenefitletter_tab",
                section: "deathbenefitletter_tab_nextprevious_section",
                webresource: "WebResource_DBLNavigationButtons",
                formButtons: ["WebResource_ViewLetter", "WebResource_DownloadPDF", "WebResource_DownloadWord", "WebResource_LetterVBMSUpload", "WebResource_Enclosures"]
            }
        ];

        return FNODProcessArray;
    } catch (e) {
        console.log("An error occurred in getFnodBpfTabSets(): " + e.message);
    }
}

function runBpfEvents(executionContext) {
    try {
        var formContext = executionContext.getFormContext();
        var activeProcess = formContext.data.process.getActiveProcess();
        if (activeProcess.getName() === "FNOD Process") {
            addBpfEventHandlersAsync(executionContext);
            showActiveTab(executionContext);
            disableBpfControls(executionContext);
        } else {
            console.log("There is no active BPF stage on the form, or the form type is not 'update'");
        }
    } catch (e) {
        console.log("An error occurred in runBpfEvents: " + e.message);
    }
}

function showActiveTab(executionContext) {
    var formContext = executionContext.getFormContext();
    var FNODProcessArray = getFnodBpfTabSets();
    var activeStageName = formContext.data.process.getActiveStage().getName().toLowerCase();

    FNODProcessArray.forEach(function (tabSet) {
        var activeTab = formContext.ui.tabs.get(tabSet.tab);

        if (tabSet.title.toLowerCase() === activeStageName) {
            activeTab.setVisible(true);
            activeTab.sections.get(tabSet.section).setVisible(true);
            activeTab.setDisplayState("expanded");
            activeTab.setFocus();
        } else {
            activeTab.setVisible(false);
        }
    });
}

function buildNavigationButtonsAsync(executionContext) {
    try {
        var formContext = executionContext.getFormContext();

        function getNavButtonArray(wrName) {
            return new Promise(function (resolve, reject) {
                try {
                    function saveAndExecute(func) {
                        formContext.data.save().then(
                            function (success) {
                                func();
                                rebuildAllButtonsAsync(executionContext);
                            },
                            function (error) {
                                rebuildAllButtonsAsync(executionContext);
                            }
                        );
                    }

                    function addButtonAction(func) {
                        var actionFunction = function () {
                            saveAndExecute(func);
                        }

                        return actionFunction;
                    }

                    function moveUntilStageName(timesToMove, stageName) {
                        var moveLimit = timesToMove;
                        var moveCount = 0;

                        function moveAgain() {
                            var activeStage = formContext.data.process.getActiveStage().getName();
                            if (activeStage !== stageName && moveCount < moveLimit) {
                                moveCount++;
                                formContext.data.process.moveNext(moveAgain);
                            }
                        }

                        moveCount++;
                        formContext.data.process.moveNext(moveAgain);
                    }

                    var buttonArray = [];
                    var prevButton = {
                        id: "PreviousButton",
                        text: "« Previous",
                        name: "Previous Stage",
                        title: "Proceed to the previous stage",
                        action: function () {
                            // Remove requirement for mandatory fields. This will allow the
                            // user to navigate backwards without filling out required fields
                            SetMandatoryFields(executionContext, "reset");

                            // Remove any validation functions. This will allow the
                            // user to navigate backwards without filling out required fields
                            RemoveAllValidation(executionContext);

                            saveAndExecute(function () {
                                formContext.data.process.movePrevious();
                            });
                        }
                    };
                    var nextButton = {
                        id: "NextButton",
                        text: "Next »",
                        name: "Next Stage",
                        title: "Proceed to the next stage",
                        action: addButtonAction(function () {
                            formContext.data.process.moveNext();
                        })
                    };
                    var closeReviewButton = {
                        id: "CloseReviewSummary",
                        text: "Close & Review Summary",
                        name: "Close and Review Summary",
                        title: "Close the current record and review a summary",
                        action: addButtonAction(function () {
                            closeAndReviewSummaryAsync(executionContext);
                        })
                    };

                    switch (wrName) {
                        case "WebResource_FNODNavigationButtons":
                            buttonArray = [
                                nextButton,
                                {
                                    id: "SkipModButton",
                                    text: "Skip MOD »",
                                    name: "Skip MOD Stages",
                                    title: "Skip the Month of Death stages",
                                    action: addButtonAction(function () {
                                        moveUntilStageName(3, "Presidential Memorial Certificate");
                                    })
                                }
                            ];
                            break;
                        case "WebResource_DBLNavigationButtons":
                            buttonArray = [
                                prevButton,
                                closeReviewButton
                            ];
                            break;
                        case "WebResource_PMCNavigationButtons":
                            buttonArray = [
                                prevButton,
                                nextButton,
                                {
                                    id: "Skip0820aButton",
                                    text: "Skip 0820A »",
                                    name: "Skip 0820A Stage",
                                    title: "Skip the 0820A stage",
                                    action: addButtonAction(function () {
                                        moveUntilStageName(2, "Death Benefit Letter");
                                    })
                                },
                                closeReviewButton
                            ];
                            break;
                        case "WebResource_0820ANavigationButtons":
                            buttonArray = [
                                prevButton,
                                nextButton,
                                closeReviewButton
                            ]
                            break;
                        default:
                            buttonArray = [
                                prevButton,
                                nextButton
                            ]
                            break;
                    }

                    resolve(buttonArray);
                } catch (e) {
                    console.log("An error occurred in getNavButtonArray: " + e);
                    reject(e);
                }
            });
        }

        function buildNavigationButton(wrName, buttonArrayPromise) {
            return new Promise(function (resolve, reject) {
                try {
                    var buttonControl = formContext.getControl(wrName);

                    if (buttonControl) {
                        buttonControl.getContentWindow().then(
                            function (contentWindow) {
                                buttonArrayPromise.then(
                                    function (buttonArray) {
                                        contentWindow.ButtonGenerator.CreateNewButtons(buttonArray);
                                        resolve(true);
                                    }
                                );
                            }
                        );
                    } else {
                        resolve(false);
                    }
                } catch (e) {
                    console.log("An error occurred in buildNavigationButton: " + e);
                    reject(e);
                }
            });
        }

        return new Promise(function (resolve, reject) {
            try {
                var activeStageName = formContext.data.process.getActiveStage().getName().toLowerCase();
                var FNODProcessArray = getFnodBpfTabSets();
                var wrName;
                var i;
                for (i = 0; i < FNODProcessArray.length; i++) {
                    if (activeStageName === FNODProcessArray[i].title.toLowerCase()) {
                        wrName = FNODProcessArray[i].webresource;
                        break;
                    }
                }

                var buttonArrayPromise = getNavButtonArray(wrName);
                buildNavigationButton(wrName, buttonArrayPromise).then(function (result) {
                    if (result) {
                        resolve(true);
                    } else {
                        resolve(false);
                    }
                });
            } catch (e) {
                console.log("An error occurred in buildNavigationButtonsAsync: " + e.message);
                reject(e);
            }
        });
    } catch (e) {
        console.log("An error occurred in buildNavigationButtonsAsync (outer): " + e.message);
        throw e;
    }
}

// Scenario 1
function CreateMODSuccessNote(executionContext) {
    //debugger;
    var NoteText = "MOD Submitted";
    CreateNote(executionContext, NoteText);
}

// Scenario 2a
function CreateMODFailedNote(executionContext) {
    //debugger;
    var NoteText = "MOD Failed";
    CreateNote(executionContext, NoteText);
}

// Scenario 2b
function CreatePreview0820fNote(executionContext) {
    //debugger;
    var NoteText = "0820f Generated";
    CreateNote(executionContext, NoteText);
}

// Scenario 3. Scenario 4 ignored
function CreatePMCNote(executionContext) {
    //debugger;
    var NoteText = "PMC Submitted";
    CreateNote(executionContext, NoteText);
}

// Scenario 5
function Create0820aUploadedToVBMSNote(executionContext) {
    //debugger;
    var NoteText;

    var CallerFirstName = formContext.getAttribute("udo_callerfirstname");
    var CallerLastName = formContext.getAttribute("udo_callerlastname");
    var VeteranFirstName = formContext.getAttribute("va_firstname");
    var VeteranLastName = formContext.getAttribute("va_lastname");

    var udo_pmc = formContext.getAttribute("udo_pmc");
    var udo_nokletter = formContext.getAttribute("udo_nokletter");
    var udo_21530 = formContext.getAttribute("udo_21530");
    var udo_21534 = formContext.getAttribute("udo_21534");
    var udo_401330 = formContext.getAttribute("udo_401330");
    var udo_otherpleasespecify = formContext.getAttribute("udo_otherpleasespecify");
    var udo_0820aexplanationtwo = formContext.getAttribute("udo_0820aexplanationtwo");

    var CallerFullName;
    var VeteranFullName;
    var DateOfDeath = formContext.getAttribute("va_dateofdeath");

    if ((CallerFirstName !== null) && (CallerLastName !== null)) {
        CallerFullName = CallerFirstName.getValue() + " " + CallerLastName.getValue();
    }

    if ((VeteranFirstName !== null) && (VeteranLastName !== null)) {
        VeteranFullName = VeteranFirstName.getValue() + " " + VeteranLastName.getValue();
    }

    if (DateOfDeath !== null) {
        DateOfDeath = DateOfDeath.getValue();
        var DateOfDeathMMDDYYYY = getFormattedDate(DateOfDeath);
    }

    NoteText = "\n";
    NoteText = "0820a Submitted and uploaded to VBMS \n";
    NoteText += "Deceased Full Name: " + VeteranFullName + "\n";
    NoteText += "Date of Death: " + DateOfDeathMMDDYYYY + "\n";
    NoteText += "Reported By: " + CallerFullName + "\n";
    NoteText += "\n";

    NoteText += "0820a - FNOD ACTION - I CERTIFY I SENT THE FOLLOWING:" + "\n";
    NoteText += "PMC: " + (udo_pmc.getValue() ? 'Yes' : 'No') + "\n";
    NoteText += "NOK LETTER: " + (udo_nokletter.getValue() ? 'Yes' : 'No') + "\n";
    NoteText += "21-530: " + (udo_21530.getValue() ? 'Yes' : 'No') + "\n";
    NoteText += "21-534: " + (udo_21534.getValue() ? 'Yes' : 'No') + "\n";
    NoteText += "40:1330: " + (udo_401330.getValue() ? 'Yes' : 'No') + "\n";
    NoteText += "Other (Please specify): " + (udo_otherpleasespecify.getValue() ? 'Yes' : 'No') + "\n";

    if (udo_otherpleasespecify.getValue() === true) {
        NoteText += "Explanation: " + udo_0820aexplanationtwo.getValue() + "\n";
    } else {
        NoteText += "Explanation: N/A \n";
    }


    CreateNote(executionContext, NoteText);

    //showMessage("Close & Review Summary Button", "This button is not yet functioning.");
}

// Scenario 6
function CreateDBLNote(executionContext) {
    //debugger;
    var NoteText = "Death Benefit Letter Generated";
    CreateNote(executionContext, NoteText);
}

function getFormattedDate(date) {
    //debugger;
    var year = date.getFullYear();

    var month = (1 + date.getMonth()).toString();
    month = month.length > 1 ? month : '0' + month;

    var day = date.getDate().toString();
    day = day.length > 1 ? day : '0' + day;

    return month + '/' + day + '/' + year;
}

function UploadNoteAsync(Id) {
    return new Promise(function (resolve, reject) {
        try {
            var parameters = {};
            var entity = {};
            entity.id = Id;
            entity.entityType = "va_fnod";
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

            Xrm.WebApi.online.execute(udo_CreateNoteRequest, null).then(function (response) {
                //debugger;
                if (response.ok) {
                    response.json().then(
                        function (data) {
                            var noteID = data.NoteID || "";
                            var noteContent = data.NoteContent || "";
                            var respMsg = data.ResponseMessage || "";

                            Xrm.Utility.closeProgressIndicator();

                            if (noteID === "") {
                                console.log("Error creating note: " + respMsg);

                                var msg = "\nYour FNOD transaction was submitted. Please press OK to close the record.\n\nSystem has failed to create Maintenance Note.";
                                var title = "Unable to Create Note";
                                var alertOptions = { height: 200, width: 600 };
                                var alertStrings = { title: title, text: msg, confirmButtonLabel: "OK" };
                                Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                                    function success(result) {
                                        //console.log("Success");
                                        return resolve(true);
                                    },
                                    function (error) {
                                        //console.log("Error");
                                        return reject(false);
                                    }
                                );
                            }
                            else {
                                var msg = "\nYour FNOD transaction was submitted. Please press OK to close the record.\n\nNote has been created with the following content: \n\n" + noteContent;
                                var title = "Note Created";
                                var alertOptions = { height: 300, width: 600 };
                                var alertStrings = { title: title, text: msg, confirmButtonLabel: "OK" };
                                Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                                    function success(result) {
                                        //console.log("Success");
                                        return resolve(true);
                                    },
                                    function (error) {
                                        //console.log("Error");
                                        return reject(false);
                                    }
                                );
                            }
                        });
                }
                else {
                    var msg = "\nYour FNOD transaction was submitted. Please press OK to close the record.\n\nSystem did not respond to create Maintenance Note.";
                    var title = "Create Note Response Bad";
                    var alertOptions = { height: 200, width: 600 };
                    var alertStrings = { title: title, text: msg, confirmButtonLabel: "OK" };
                    Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                        function success(result) {
                            //console.log("Success");
                            return resolve(true);
                        },
                        function (error) {
                            //console.log("Error");
                            return reject(false);
                        }
                    );
                }
            });
        } catch (e) {
            console.log("An error occurred in UploadNoteAsync: " + e);
            reject(e);
        }
    });
}

function CreateNote(executionContext, newNoteText) {
    // Set formContext
    var formContext = executionContext.getFormContext();

    // Retrieve note then update
    var Entity = "va_fnod";
    var Id = formContext.data.entity.getId().replace("{", "").replace("}", "");
    var Select = "?$select=udo_summarynote";

    //debugger;
    Xrm.WebApi.retrieveRecord(Entity, Id, Select).then(
        function success(result) {
            if (result !== null) {
                //console.log(result.udo_summarynote);
                var noteText;
                if (result.udo_summarynote) {
                    noteText = result.udo_summarynote + "\n";
                } else {
                    noteText = "";
                }
                noteText = noteText + newNoteText;
                //console.log(noteText);

                UpdateNote(noteText, Id);
            }
        },
        function (error) {
            console.log("An error occurred in CreateNote: " + error.message);
        }
    );
}

function UpdateNote(noteText, Id) {
    var data =
    {
        "udo_summarynote": noteText
    }

    //debugger;
    Xrm.WebApi.updateRecord("va_fnod", Id, data).then(
        function success(result) {
            //console.log("Note successfully updated");
            //UploadNoteAsync(Id);
        },
        function (error) {
            console.log("An error occurred in UpdateNote: " + error.message);
        }
    );
}

function defaultTabOnLoad(executionContext, tabObj) {
    try {
        // Rebuild all buttons
        rebuildAllButtonsAsync(executionContext);

        // Set mandatory fields
        identifyMandatoryFields(executionContext);
        return true;
    } catch (e) {
        console.log("An error occurred in defaultTabOnLoad: " + e.message);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function defaultTabOnClose(executionContext, tabObj) {
    try {
        // Do nothing
        return;
    } catch (e) {
        console.log("An error occurred within defaultTabOnClose: " + e);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function executeTabStateChange(executionContext, tabObj) {
    try {
        if (tabObj.getDisplayState() === "expanded") {
            // Run tab onload event
            executeTabOnLoad(executionContext, tabObj);
        } else {
            // Run tab onclose event
            executeTabOnClose(executionContext, tabObj);
        }
    } catch (e) {
        console.log("An error occurred within executeTabStateChange: " + e);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function executeTabOnLoad(executionContext, tabObj) {
    try {
        var tabName = tabObj.getName();
        var tabOnLoadFunc = null;

        // Run tab onload event
        switch (tabName) {
            case "mod_tab":
                tabOnLoadFunc = modTabOnLoad;
                break;
            case "0820f_tab":
                tabOnLoadFunc = zero820fOnLoad;
                break;
            case "0820a_tab":
                tabOnLoadFunc = zero820aOnLoad;
                break;
            default:
                tabOnLoadFunc = function () { return; }
                break;
        }

        defaultTabOnLoad(executionContext, tabObj);
        tabOnLoadFunc(executionContext, tabObj);
    } catch (e) {
        console.log("An error occurred within executeTabOnLoad: " + e);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function executeTabOnClose(executionContext, tabObj) {
    try {
        var tabName = tabObj.getName();
        var tabOnCloseFunc = null;

        // Run tab onclose event
        switch (tabName) {
            case "mod_tab":
                tabOnCloseFunc = modTabOnClose;
                break;
            case "0820f_tab":
                tabOnCloseFunc = zero820fOnClose;
                break;
            default:
                tabOnCloseFunc = function () { return; }
                break;
        }

        defaultTabOnClose(executionContext, tabObj);
        tabOnCloseFunc(executionContext, tabObj);
    } catch (e) {
        console.log("An error occurred within executeTabOnClose: " + e);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function modTabOnLoad(executionContext, tabObj) {
    try {
        var formContext = executionContext.getFormContext();

        // Show MOD banner
        displayModBanner(executionContext);

        // Setup Spouse SSN Validation
        if (formContext.getAttribute("va_modeligible").getValue() === true && formContext.getAttribute("va_veteranhassurvivingspouse").getValue() === true) {
            // Validate Spouse SSN
            validateSpouseSsn(executionContext);

            // Set SSN validation
            addSpouseSsnValidation(executionContext);
        }

        // Setup va_veteranhassurvivingspouse onChange
        addVeteranHasSurvivingSpouseOnChange(executionContext);
    } catch (e) {
        console.log("An error occurred within modTabOnLoad: " + e);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function zero820aOnLoad(executionContext, tabObj) {
    try {
        // Set toggle field to false
        hideCallerSpouseInfo(executionContext);
    } catch (e) {
        console.log("An error occurred within zero820aOnLoad: " + e);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function zero820fOnLoad(executionContext, tabObj) {
    try {
        // Set toggle field to true
        hideCallerSpouseInfo(executionContext, true);

        // Validate Spouse SSN
        validateSpouseSsn(executionContext);

        // Set SSN validation
        addSpouseSsnValidation(executionContext);
    } catch (e) {
        console.log("An error occurred within zero820fOnLoad: " + e);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function modTabOnClose(executionContext, tabObj) {
    try {
        // Hide MOD banner
        hideModBanner(executionContext);

        // Remove validation
        removeSpouseSsnValidation(executionContext);
        removeVeteranHasSurvivingSpouseOnChange(executionContext);
    } catch (e) {
        console.log("An error occurred within modTabOnClose: " + e);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function zero820fOnClose(executionContext, tabObj) {
    try {
        // Set toggle field to false
        hideCallerSpouseInfo(executionContext);

        // Remove validation
        removeSpouseSsnValidation(executionContext);
    } catch (e) {
        console.log("An error occurred within zero820fOnClose: " + e);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function addTabStateEventHandlers(executionContext) {
    try {
        var formContext = executionContext.getFormContext();
        var tabs = ["fnod_tab", "mod_tab", "0820f_tab", "pmc_tab", "0820a_tab", "deathbenefitletter_tab"];
        var foundExpandedTab = false;

        tabs.forEach(function (tab) {
            var tabObj = formContext.ui.tabs.get(tab);

            tabObj.addTabStateChange(function (executionContext) {
                executeTabStateChange(executionContext, tabObj);
            });

            if (!foundExpandedTab) {
                if (tabObj.getDisplayState() === "expanded") {
                    executeTabOnLoad(executionContext, tabObj);
                    foundExpandedTab = true;
                }
            }
        });

        return true;
    } catch (e) {
        console.log("An error occurred in addTabStateEventHandlers: " + e.message);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function validateSpouseSsn(executionContext) {
    try {
        ValidateSsn(executionContext, "va_spousessn", true);
    } catch (e) {
        console.log("An error occurred within validateSpouseSsn: " + e);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function addSpouseSsnValidation(executionContext) {
    try {
        var formContext = executionContext.getFormContext();

        formContext.getAttribute("va_spousessn").addOnChange(validateSpouseSsn);
    } catch (e) {
        console.log("An error occurred within addSpouseSsnValidation: " + e);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function removeSpouseSsnValidation(executionContext) {
    try {
        var formContext = executionContext.getFormContext();
        var ssnControl = formContext.getControl("va_spousessn");
        var ssnNotificationId = "SsnValidationNotification";

        ssnControl.clearNotification(ssnNotificationId);

        formContext.getAttribute("va_spousessn").removeOnChange(validateSpouseSsn);
    } catch (e) {
        console.log("An error occurred within removeSpouseSsnValidation: " + e);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function veteranHasSurvivingSpouseOnChange(executionContext) {
    try {
        var formContext = executionContext.getFormContext();
        var hasSpouse = formContext.getAttribute("va_veteranhassurvivingspouse").getValue();

        if (hasSpouse) {
            validateSpouseSsn(executionContext);
            addSpouseSsnValidation(executionContext);
        } else {
            removeSpouseSsnValidation(executionContext);
        }
    } catch (e) {
        console.log("An error occurred within veteranHasSurvivingSpouseOnChange: " + e);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function addVeteranHasSurvivingSpouseOnChange(executionContext) {
    try {
        var formContext = executionContext.getFormContext();

        formContext.getAttribute("va_veteranhassurvivingspouse").addOnChange(veteranHasSurvivingSpouseOnChange);
    } catch (e) {
        console.log("An error occurred within addVeteranHasSurvivingSpouseOnChange: " + e);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function removeVeteranHasSurvivingSpouseOnChange(executionContext) {
    try {
        var formContext = executionContext.getFormContext();

        formContext.getAttribute("va_veteranhassurvivingspouse").removeOnChange(veteranHasSurvivingSpouseOnChange);
    } catch (e) {
        console.log("An error occurred within removeVeteranHasSurvivingSpouseOnChange: " + e);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function buildFormTabButtonsAsync(executionContext, tabCollection) {
    return new Promise(function (resolve, reject) {
        try {
            var FNODProcessArray = getFnodBpfTabSets();
            var buttonArray;

            tabCollection.forEach(function (tab) {
                var tabName = tab.getName();
                var i;
                for (i = 0; i < FNODProcessArray.length; i++) {
                    if (tabName.toLowerCase() === FNODProcessArray[i].tab.toLowerCase()) {
                        buttonArray = FNODProcessArray[i].formButtons;
                        buttonArray.forEach(function (formButton) {
                            setUpButtons(formButton, executionContext);
                        });
                        break;
                    }
                }
            });

            resolve(true);
        } catch (e) {
            console.log("An error occurred in buildFormTabButtonsAsync: " + e.message);
            reject(e);
        }
    });
}

function addBpfEventHandlersAsync(executionContext) {
    return new Promise(function (resolve, reject) {
        try {
            var formContext = executionContext.getFormContext();

            // Add stage change event handler
            var proc = formContext.data.process;
            proc.addOnStageChange(showActiveTab);
            proc.addOnStageChange(disableBpfControls);

            resolve(true);
        } catch (e) {
            console.log("An error occurred when adding BPF event handlers: " + e.message);
            rebuildAllButtonsAsync(execContext);
            throw (e);
            reject(false);
        }
    });
}

function showHideCallerSpouseInfo(executionContext) {
    try {
        var formContext = executionContext.getFormContext();

        // Get toggle field
        var toggle = formContext.getAttribute("udo_showcallerinformationandsurvivingspouse")

        // Set toggle field to never save
        toggle.setSubmitMode("never");

        // Show/hide caller and spouse info
        var tab0820f = formContext.ui.tabs.get("0820f_tab");
        var tab0820a = formContext.ui.tabs.get("0820a_tab");
        if (toggle.getValue() === true) {
            tab0820f.sections.get("0820f_tab_callerinformation_section").setVisible(true);
            tab0820f.sections.get("0820f_tab_spouseinformation_section").setVisible(true);
            tab0820a.sections.get("0820a_tab_callerinformation_section").setVisible(true);
            tab0820a.sections.get("0820a_tab_spouseinformation_section").setVisible(true);
        } else {
            tab0820f.sections.get("0820f_tab_callerinformation_section").setVisible(false);
            tab0820f.sections.get("0820f_tab_spouseinformation_section").setVisible(false);
            tab0820a.sections.get("0820a_tab_callerinformation_section").setVisible(false);
            tab0820a.sections.get("0820a_tab_spouseinformation_section").setVisible(false);
        }

        return true;
    } catch (e) {
        console.log("An error occurred when attempting to show/hide caller and spouse info: " + e.message);
        rebuildAllButtonsAsync(execContext);
        throw (e);
        return false;
    }
}

function populateCallerInfo(executionContext) {
    try {
        var relationshipOptionSetMappings = {
            "752280000": "Veteran(Self)",
            "752280005": "Spouse",
            "752280001": "Dependent",
            "752280002": "VSO/POA/AA",
            "752280003": "Authorized Third Party",
            "752280004": "Fiduciary",
            "752280009": "Private Attorney",
            "752280010": "Private Agent",
            "752280007": "Representative",
            "752280006": "Unknown",
            "752280008": "Other"
        }

        var formContext = executionContext.getFormContext();

        // Check if caller info has already been retrieved
        var ranRetrieveCallerInfoAttr = formContext.getAttribute("udo_ranretrievecallerinfo");
        var ranFnodAttr = formContext.getAttribute("va_ranfnod");
        if (ranRetrieveCallerInfoAttr.getValue() === true || formContext.ui.getFormType() !== 2 || ranFnodAttr.getValue() === true) {
            //console.log("Returning from populateCallerInfo()");
            return;
        }

        var idProofId = formContext.getAttribute("udo_idproof").getValue()[0].id.replace("{", "").replace("}", "");
        var options = "?$select=udo_firstname,udo_lastname,udo_phonenumber,udo_addressline1,udo_addressline2,udo_city,udo_state,udo_country,udo_zipcode,udo_relationship&$filter=udo_udo_interaction_udo_idproof_Interaction/any(o:o/udo_idproofid eq '" + idProofId + "')";

        Xrm.WebApi.retrieveMultipleRecords("udo_interaction", options).then(
            function (result) {
                if (result.entities.length !== 1) {
                    showMessage("An Error Occurred", "An issue occurred when retrieving caller information. Number of callers found: " + result.entities.length);
                    return;
                }

                var interaction = result.entities[0];

                formContext.getAttribute("udo_callerfirstname").setValue(interaction["udo_firstname"]);
                formContext.getAttribute("udo_callerlastname").setValue(interaction["udo_lastname"]);
                formContext.getAttribute("udo_callerdayphone").setValue(interaction["udo_phonenumber"]);
                formContext.getAttribute("udo_callerrelationtoveteran").setValue(relationshipOptionSetMappings[interaction["udo_relationship"].toString()]);

                populateNameReportingIndividual(executionContext, interaction["udo_firstname"], interaction["udo_lastname"]);

                ranRetrieveCallerInfoAttr.setValue(true);
            }
        );
    } catch (e) {
        console.log("An error occurred in populateCallerInfo: " + e);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function populateNameReportingIndividual(executionContext, firstname, lastname) {
    try {
        var formContext = executionContext.getFormContext();
        var fullname = "";

        if ((firstname === null || firstname === undefined) && (lastname === null || lastname === undefined)) {
            // Set'Name of Reporting Individual' field
            formContext.getAttribute("udo_nameofreportingindividual").setValue(null);
            return false;
        }

        // Confirm firstname is not empty
        if (firstname !== null && firstname !== undefined) {
            fullname += firstname;
            fullname += " ";
        }

        // Confirm lastname is not empty
        if (lastname !== null && lastname !== undefined) {
            fullname += lastname;
        }

        // Set'Name of Reporting Individual' field
        formContext.getAttribute("udo_nameofreportingindividual").setValue(fullname.trim());

        return true;
    } catch (e) {
        console.log("An error occurred within populateNameReportingIndividual: " + e);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function populatePmc(executionContext) {
    try {
        var formContext = executionContext.getFormContext();
        var recipient = formContext.getAttribute("udo_pmcrecipient").getText();
        var firstName = null;
        var lastName = null;
        var address1 = null;
        var address2 = null;
        var city = null;
        var state = null;
        var zip = null;

        switch (recipient) {
            case "Caller":
                firstName = formContext.getAttribute("udo_callerfirstname").getValue();
                lastName = formContext.getAttribute("udo_callerlastname").getValue();
                address1 = formContext.getAttribute("udo_calleraddress1").getValue();
                address2 = formContext.getAttribute("udo_calleraddress2").getValue();
                city = getOneOrOther(formContext.getAttribute("udo_callercity").getValue(), [formContext.getAttribute("udo_calleroverseasmilitarypostofficetypecode").getValue()]);
                state = getOneOrOther(formContext.getAttribute("udo_callerstatelist").getText(), [formContext.getAttribute("udo_calleroverseasmilitarypostalcode").getText()]);
                zip = formContext.getAttribute("udo_callerzipcode").getValue();
                break;
            case "Surviving Spouse":
                firstName = formContext.getAttribute("va_spousefirstname").getValue();
                lastName = formContext.getAttribute("va_spouselastname").getValue();
                address1 = formContext.getAttribute("va_spouseaddress1").getValue();
                address2 = formContext.getAttribute("va_spouseaddress2").getValue();
                city = getOneOrOther(formContext.getAttribute("va_spousecity").getValue(), [formContext.getAttribute("va_spouseoverseasmilitarypostofficetypecode").getValue()]);
                state = getOneOrOther(formContext.getAttribute("va_spousestatelist").getText(), [formContext.getAttribute("va_spouseoverseasmilitarypostalcode").getText()]);
                zip = formContext.getAttribute("va_spousezipcode").getValue();
                break;
            default:
                break;
        }

        formContext.getAttribute("va_newpmcrecipname").setValue(combineFirstLast(firstName, lastName));
        formContext.getAttribute("va_newpmcrecipaddress1").setValue(address1);
        formContext.getAttribute("va_newpmcrecipaddress2").setValue(address2);
        formContext.getAttribute("va_newpmcrecipcity").setValue(city);
        formContext.getAttribute("va_newpmcrecipstate").setValue(state);
        formContext.getAttribute("va_newpmcrecipzip").setValue(zip);

        function combineFirstLast(first, last) {
            var firstLast = null;

            if (first !== null && last !== null) {
                firstLast = first + " " + last;
            } else if (first !== null) {
                firstLast = first;
            } else if (last !== null) {
                firstLast = last;
            }

            return firstLast;
        }
    } catch (e) {
        console.log("An error occurred when populating PMC fields: " + e.message);
        rebuildAllButtonsAsync(execContext);
        throw (e);
        return false;
    }
}

function populateDbl(executionContext) {
    try {
        var formContext = executionContext.getFormContext();
        var recipient = formContext.getAttribute("udo_dblrecipient").getText();
        var firstName = null;
        var lastName = null;
        var phone = null;
        var addressType = null;
        var address1 = null;
        var address2 = null;
        var address3 = null;
        var city = null;
        var state = null;
        var zip = null;
        var country = null;

        switch (recipient) {
            case "Caller":
                firstName = formContext.getAttribute("udo_callerfirstname").getValue();
                lastName = formContext.getAttribute("udo_callerlastname").getValue();
                phone = formContext.getAttribute("udo_callerdayphone").getValue();
                addressType = formContext.getAttribute("udo_calleraddresstype").getValue();
                address1 = formContext.getAttribute("udo_calleraddress1").getValue();
                address2 = formContext.getAttribute("udo_calleraddress2").getValue();
                address3 = formContext.getAttribute("udo_calleraddress3").getValue();
                city = getOneOrOther(formContext.getAttribute("udo_callercity").getValue(), [formContext.getAttribute("udo_calleroverseasmilitarypostofficetypecode").getValue()]);
                state = getOneOrOther(formContext.getAttribute("udo_callerstatelist").getText(), [formContext.getAttribute("udo_calleroverseasmilitarypostalcode").getText()]);
                zip = formContext.getAttribute("udo_callerzipcode").getValue();
                country = formContext.getAttribute("udo_callercountry").getValue();
                break;
            case "Surviving Spouse":
                firstName = formContext.getAttribute("va_spousefirstname").getValue();
                lastName = formContext.getAttribute("va_spouselastname").getValue();
                phone = formContext.getAttribute("udo_spousedayphone").getValue();
                addressType = formContext.getAttribute("va_spouseaddresstype").getValue();
                address1 = formContext.getAttribute("va_spouseaddress1").getValue();
                address2 = formContext.getAttribute("va_spouseaddress2").getValue();
                address3 = formContext.getAttribute("va_spouseaddress3").getValue();
                city = getOneOrOther(formContext.getAttribute("va_spousecity").getValue(), [formContext.getAttribute("va_spouseoverseasmilitarypostofficetypecode").getValue()]);
                state = getOneOrOther(formContext.getAttribute("va_spousestatelist").getText(), [formContext.getAttribute("va_spouseoverseasmilitarypostalcode").getText()]);
                zip = formContext.getAttribute("va_spousezipcode").getValue();
                country = formContext.getAttribute("va_spousecountry").getValue();
                break;
            default:
                break;
        }

        formContext.getAttribute("udo_dblfirstname").setValue(firstName);
        formContext.getAttribute("udo_dbllastname").setValue(lastName);
        formContext.getAttribute("udo_dbldayphone").setValue(phone);
        formContext.getAttribute("udo_dbladdresstype").setValue(addressType);
        formContext.getAttribute("udo_dbladdress1").setValue(address1);
        formContext.getAttribute("udo_dbladdress2").setValue(address2);
        formContext.getAttribute("udo_dbladdress3").setValue(address3);
        formContext.getAttribute("udo_dblcity").setValue(city);
        formContext.getAttribute("udo_dblstate").setValue(state);
        formContext.getAttribute("udo_dblzipcode").setValue(zip);
        formContext.getAttribute("udo_dblcountry").setValue(country);
    } catch (e) {
        console.log("An error occurred when populating DBL fields: " + e.message);
        rebuildAllButtonsAsync(execContext);
        throw (e);
        return false;
    }
}

// RunXrmCommand Function for USD UCI Hosted Control
function RetrieveVeteranDetailsfromVeteranTab(formContext, veteranid, va_stationofjurisdictionid, fetchsojid, udo_emailaddress1, udo_ssn, udo_filenumber, udo_dateofdeath, udo_participantid, udo_branchofservice, udo_firstname, udo_lastname) {
    var serviceRequest = {};
    if (veteranid !== "") {
        if (va_stationofjurisdictionid !== "")
            serviceRequest.udo_RegionalOfficeId = { Id: fetchsojid, LogicalName: "va_regionaloffice" };
        if (udo_emailaddress1 !== "")
            serviceRequest.udo_EmailofVeteran = udo_emailaddress1;
        serviceRequest.udo_RelatedVeteranId = { Id: veteranid, LogicalName: "contact" };
        serviceRequest.udo_SSN = udo_ssn;
        serviceRequest.udo_FileNumber = udo_filenumber;
        serviceRequest.udo_DateofDeath = udo_dateofdeath;
        serviceRequest.udo_ParticipantID = udo_participantid;
        serviceRequest.udo_BranchofService = udo_branchofservice;
        serviceRequest.udo_VetFirstName = udo_firstname;
        serviceRequest.udo_VetLastName = udo_lastname;
    }
    return serviceRequest;
}

function disableEnableAllFieldControls(executionContext, fieldName, disabled) {
    try {
        var formContext = executionContext.getFormContext();
        var controls = formContext.getAttribute(fieldName).controls;

        controls.forEach(function (ctrl) {
            var ctrlName = ctrl.getName();
            formContext.getControl(ctrlName).setDisabled(disabled);
        });

        return true;
    } catch (e) {
        console.log("An error occurred when trying to disable/enable the " + fieldName + "field. Error message: " + e.message);
        rebuildAllButtonsAsync(execContext);
        throw (e);
        return false;
    }
}

function showHideAllFieldControls(executionContext, fieldName, visible) {
    try {
        var formContext = executionContext.getFormContext();
        var controls = formContext.getAttribute(fieldName).controls;

        controls.forEach(function (ctrl) {
            var ctrlName = ctrl.getName();
            formContext.getControl(ctrlName).setVisible(visible);
        });

        return true;
    } catch (e) {
        console.log("An error occurred during showHideAllFieldControls. Error message: " + e.message);
        rebuildAllButtonsAsync(execContext);
        throw (e);
        return false;
    }
}

function identifyMandatoryFields(executionContext) {
    try {
        var formContext = executionContext.getFormContext();
        var tabCollection = formContext.ui.tabs;

        var tabMappings = {
            "fnod_tab": "FNOD",
            "mod_tab": "MOD",
            "0820f_tab": "0820F",
            "0820a_tab": "0820A",
            "pmc_tab": "PMC",
            "deathbenefitletter_tab": "DBL"
        }

        SetMandatoryFields(executionContext, "reset");

        tabCollection.forEach(function (tab) {
            if (tab.getDisplayState() === "expanded") {
                var tabName = tab.getName()
                switch (tabName) {
                    case "mod_tab":
                        if (formContext.getAttribute("va_modeligible").getValue() === true) {
                            SetMandatoryFields(executionContext, tabMappings[tabName]);
                        }
                        break;
                    default:
                        SetMandatoryFields(executionContext, tabMappings[tabName]);
                }
            }
        });

        return true;
    } catch (e) {
        console.log("An error occurred within identifyMandatoryFields: " + e.message);
        rebuildAllButtonsAsync(execContext);
        throw (e);
        return false;
    }
}

function showHideSpouseModSections(executionContext) {
    try {
        var formContext = executionContext.getFormContext();
        var isEligible = formContext.getAttribute("va_modeligible").getValue();

        var spouseSections = [
            "spouse_section0",
            "spouse_section1",
            "spouse_section2",
            "spouse_section3",
            "spouse_section4",
            "spouse_section5",
            "spouse_section5a",
            "spouse_section6",
            "spouse_section7",
            "mod_tab_modbuttons_section"
        ];

        // Get mod tab object
        var modTab = formContext.ui.tabs.get("mod_tab");

        // Set sections visible/hidden
        spouseSections.forEach(function (section) {
            modTab.sections.get(section).setVisible(isEligible);
        });

        return true;
    } catch (e) {
        console.log("An error occurred in showHideSpouseModSections: " + e.message);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function initiateVbmsUpload(executionContext, type) {
    try {
        if (Va.Udo.Crm.Uci.Scripts.VbmsUploader === undefined || Va.Udo.Crm.Uci.Scripts.VbmsUploader === null) {
            showMessage("Unable to Perform VBMS Upload", "An error occurred when attempting to open VBMS upload.");
        } else {
            var formContext = executionContext.getFormContext();
            var tab;
            var section;
            var webresource;
            var filenumber = formContext.getAttribute("va_filenumber").getValue();
            var uploadrole;
            var doctype;
            var parententityname = formContext.data.entity.getEntityName();
            var parententityid = formContext.data.entity.getId().replace("{", "").replace("}", "");
            var successCallback;
            var errorCancelCallback = function () {
                rebuildAllButtonsAsync(executionContext);
            };

            switch (type) {
                case "0820A":
                    tab = "0820a_tab";
                    section = "0820a_tab_vbmsupload_section";
                    webresource = "WebResource_0820AUploadVBMS";
                    uploadrole = formContext.getAttribute("udo_0820avbmsuploadrole").getValue();
                    doctype = formContext.getAttribute("udo_0820avbmsdoctype").getValue();
                    successCallback = function () {
                        Create0820aUploadedToVBMSNote(executionContext);
                        rebuildAllButtonsAsync(executionContext);
                    };
                    break;
                case "DBL":
                    tab = "deathbenefitletter_tab";
                    section = "deathbenefitletter_tab_vbmsupload_section";
                    webresource = "WebResource_DBLUploadVBMS";
                    uploadrole = formContext.getAttribute("udo_dblvbmsuploadrole").getValue();
                    doctype = formContext.getAttribute("udo_dblvbmsdoctype").getValue();
                    successCallback = function () {
                        rebuildAllButtonsAsync(executionContext);
                    };
                    break;
            }

            var paramsObj = {
                tabName: tab,
                sectionName: section,
                webresourceName: webresource,
                parentEntityLogicalName: parententityname,
                parentEntityId: parententityid,
                vbmsUploadRole: uploadrole,
                vbmsDocType: doctype,
                fileNumber: filenumber,
                hideTabWhenComplete: false,
                successCallback: successCallback,
                errorCallback: errorCancelCallback,
                cancelCallback: errorCancelCallback
            }
            var vbmsObj = new Va.Udo.Crm.Uci.Scripts.VbmsUploader(executionContext, paramsObj);

            vbmsObj.OpenVBMSDialog("", "", "", "", "", "", "");
        }
    } catch (e) {
        console.log("An error occurred within initiateVbmsUpload(): " + e.message);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function copyCallerToSpouse(executionContext) {
    try {
        var confirmStrings = {
            confirmButtonLabel: "YES",
            cancelButtonLabel: "NO",
            title: "WARNING",
            text: "You are about to copy the current Caller Information to the Surviving Spouse Information. This will overwrite any current Surviving Spouse Information.\n\nDo you want to proceed?"
        };

        Xrm.Navigation.openConfirmDialog(confirmStrings, { height: 300, width: 500 }).then(
            function (confirmObj) {
                if (!confirmObj.confirmed) {
                    return;
                }

                var formContext = executionContext.getFormContext();
                var spouseRecordActionVal;

                var fieldMappings = {
                    "udo_callerfirstname": "va_spousefirstname",
                    "udo_callerlastname": "va_spouselastname",
                    "udo_calleraddresstype": "va_spouseaddresstype",
                    "udo_calleraddress1": "va_spouseaddress1",
                    "udo_calleraddress2": "va_spouseaddress2",
                    "udo_calleraddress3": "va_spouseaddress3",
                    "udo_callercity": "va_spousecity",
                    "udo_callerstatelist": "va_spousestatelist",
                    "udo_callerzipcode": "va_spousezipcode",
                    "udo_callercountry": "va_spousecountry",
                    "udo_calleroverseasmilitarypostalcode": "va_spouseoverseasmilitarypostalcode",
                    "udo_calleroverseasmilitarypostofficetypecode": "va_spouseoverseasmilitarypostofficetypecode",
                    "udo_callerforeignpostalcode": "va_spouseforeignpostalcode",
                    "udo_callerdayphone": "udo_spousedayphone"
                }

                if (formContext.getAttribute("va_veteranhassurvivingspouse").getValue() !== true) {
                    spouseRecordActionVal = 953850000;
                } else {
                    spouseRecordActionVal = 953850001;
                }

                setValueFireOnChange(executionContext, "va_veteranhassurvivingspouse", true);
                setValueFireOnChange(executionContext, "va_spouserecordaction", spouseRecordActionVal);

                for (var x in fieldMappings) {
                    var callerVal = formContext.getAttribute(x).getValue();

                    // Set spouse field value and fire onchange event
                    setValueFireOnChange(executionContext, fieldMappings[x], callerVal);
                }

                // console.log("The current Caller Information has been successfully copied to the Surviving Spouse Information");
            }
        );
    } catch (e) {
        console.log("An error occurred within copyCallerToSpouse: " + e.message);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function getVBMSRoleAsync(executionContext) {
    var formContext = executionContext.getFormContext();
    var globalContext = Xrm.Utility.getGlobalContext();
    var currentUserId = globalContext.userSettings.userId.replace("{", "").replace("}", "");
    var vbmsUploadrole;

    Xrm.WebApi.retrieveRecord("systemuser", currentUserId, "?$select=udo_vbmsuploadrole").then(
        function (data) {
            vbmsUploadrole = data.udo_vbmsuploadrole;

            if (vbmsUploadrole !== null) {
                formContext.getAttribute("udo_0820avbmsuploadrole").setValue(vbmsUploadrole);
                formContext.getAttribute("udo_dblvbmsuploadrole").setValue(vbmsUploadrole);
            }

        })
        .catch(function (e) {
            console.log("Failed to retrieve VBMS Upload Role data: " + e.message);
        }
        );
}

function generateLetterAsync(executionContext, reportName, webResourceName, runInBackground, downloadDoc, uploadToVbms, saveFirst, formatType) {
    // Note: If downloadDoc is equal to true or null, then webResourceName is required

    // Declare variables
    var webresource = webResourceName
    var report = reportName;
    if (formatType === null || formatType === undefined) {
        formatType = "PDF";
    }
    var background = false; // Default: do not run in background
    var download = true; // Default: download
    var uploadVbms = false; // Default: do not upload
    var save = true; // Default: save first
    var fnodRef;
    var personRef;
    var generateFnodRequest;
    var claimNumber;
    var generateFnodResponse;

    if (runInBackground !== null && runInBackground !== undefined) {
        background = runInBackground;
    }

    if (downloadDoc !== null && downloadDoc !== undefined) {
        download = downloadDoc;
    }

    if (uploadToVbms !== null && uploadToVbms !== undefined) {
        uploadVbms = uploadToVbms;
    }

    if (saveFirst !== null && saveFirst !== undefined) {
        save = saveFirst;
    }

    // Set formContext
    var formContext = executionContext.getFormContext();

    // Check to make sure form is saved before continuing
    if (formContext.ui.getFormType() === 1) {
        showMessage("An Unexpected Error Occurred", "Please save the form first before proceeding.").then(
            function () {
                // console.log("Please save the form first before proceeding.");
                return Promise.reject("Please save the form first before proceeding.");
            }
        );
    }

    // Execute main logic
    return new Promise(function (resolve, reject) {
        try {
            // Save the form
            saveIfNeededAsync(save).then(function () {
                // Show progress indicator
                if (background !== true) {
                    Xrm.Utility.showProgressIndicator("Generating Letter");
                }

                // Get current fnod record data
                return getFnodFormDataAsync();
            }).then(function (result) {
                return doActionAsync();
            }).then(function (result) {
                // Close progress indicator
                if (background !== true) {
                    Xrm.Utility.closeProgressIndicator();
                }
                //console.log("generateLetterAsync completed successfully");
                return resolve(true);
            }).catch(function (error) {
                // Close progress indicator
                if (background !== true) {
                    Xrm.Utility.closeProgressIndicator();
                }

                console.log("An error occurred in generateLetterAsync (inner): " + error.message);
                return reject(error);
            });
        } catch (e) {
            console.log("An error occurred in generateLetterAsync: " + e);
            reject(e);
        }
    });

    function saveIfNeededAsync(saveOrNot) {
        return new Promise(function (resolve, reject) {
            try {
                if (saveOrNot) {
                    formContext.data.save().then(
                        function (success) {
                            return resolve(true);
                        },
                        function (error) {
                            console.log("An error occurredin saveIfNeededAsync: " + error);
                            return reject(error);
                        }
                    );
                } else {
                    return resolve(true);
                }
            } catch (e) {
                console.log("An error occurred in saveIfNeededAsync (outer): " + e);
                reject(e);
            }
        });
    }

    // Function to get form data
    function getFnodFormDataAsync() {
        return new Promise(function (resolve, reject) {
            try {
                // Get udo_person reference
                if (formContext.getAttribute("udo_deceasedperson").getValue() !== null && formContext.getAttribute("udo_deceasedperson").getValue() !== undefined) {
                    personRef = formContext.getAttribute("udo_deceasedperson").getValue()[0];
                }

                if (personRef === null || personRef === undefined) {
                    personRef = { id: "", entityType: "udo_person" };
                }

                // Get current va_fnod reference
                fnodRef = {
                    id: formContext.data.entity.getId().replace("{", "").replace("}", ""),
                    entityType: "va_fnod"
                }

                // Get udo_claimnumber reference
                claimNumber = formContext.getAttribute("udo_claimnumber").getValue();

                if (claimNumber === null || personclaimNumberRef === undefined) {
                    claimNumber = "";
                }

                return resolve(true);
            } catch (e) {
                return reject("An error occurred when retrieving the FNOD form data: " + e.message);
            }
        });
    }

    // Function to control flow based on action
    function doActionAsync() {
        return new Promise(function (resolve, reject) {
            try {
                // Build custom action object
                buildCustomActionAsync().then(function () {
                    // Execute custom action
                    return executeGenerateFnodAsync();
                }).then(function () {
                    // Close progress indicator
                    Xrm.Utility.closeProgressIndicator();

                    // If docAction is download, then download
                    if (download === true) {
                        return initiateDocDownloadAsync();
                    }

                    return Promise.resolve(true);
                }).then(function () {
                    return resolve(true);
                }).catch(function () {
                    // TODO: Handle failure
                    reject("An error occurred when downloading the document");
                });
            } catch (e) {
                console.log("An error occurred in doActionAsync: " + e);
                reject(e);
            }
        });
    }

    // Function to create custom action object
    function buildCustomActionAsync() {
        return new Promise(function (resolve, reject) {
            try {
                generateFnodRequest = {
                    entity: fnodRef,
                    Report: null,
                    ReportName: report,
                    FormatType: formatType,
                    Person: personRef,
                    ClaimNumber: claimNumber,
                    SourceUrl: report,
                    UploadToVBMS: uploadVbms,
                    getMetadata: function () {
                        return {
                            boundParameter: "entity",
                            parameterTypes: {
                                entity: {
                                    typeName: "mscrm.va_fnod",
                                    structuralProperty: 5
                                },
                                Report: {
                                    typeName: "mscrm.crmbaseentity",
                                    structuralProperty: 5
                                },
                                ReportName: {
                                    typeName: "Edm.String",
                                    structuralProperty: 1
                                },
                                FormatType: {
                                    typeName: "Edm.String",
                                    structuralProperty: 1
                                },
                                Person: {
                                    typeName: "mscrm.udo_person",
                                    structuralProperty: 5
                                },
                                ClaimNumber: {
                                    typeName: "Edm.String",
                                    structuralProperty: 1
                                },
                                SourceUrl: {
                                    typeName: "Edm.String",
                                    structuralProperty: 1
                                },
                                UploadToVBMS: {
                                    typeName: "Edm.Boolean",
                                    structuralProperty: 1
                                },
                            },
                            operationType: 0,
                            operationName: "udo_GenerateFNOD"
                        };
                    }
                }

                return resolve(true);
            } catch (e) {
                console.log("An error occurred in buildCustomActionAsync: " + e);
                reject(e);
            }
        });
    }

    // Function to execute custom action
    function executeGenerateFnodAsync() {
        return new Promise(function (resolve, reject) {
            try {
                Xrm.WebApi.online.execute(generateFnodRequest).then(function (response) {
                    if (!response.ok) {
                        return reject("Execute udo_GenerateFNOD failed: " + status);
                    }

                    response.json().then(function (json) {
                        generateFnodResponse = json;
                        return resolve(true);
                    });
                }, function (error) {
                    return reject("Execute udo_GenerateFNOD failed: " + status);
                });
            } catch (e) {
                console.log("An error occurred in executeGenerateFnodAsync: " + e);
                reject(e);
            }
        });
    }

    // Function to handle custom action response
    function initiateDocDownloadAsync() {
        return new Promise(function (resolve, reject) {
            try {
                if (generateFnodResponse.Base64FileContents === null || generateFnodResponse.Base64FileContents.length === 0) {
                    showMessage("Failed to Create Document", (generateFnodResponse.UploadMessage || "Letter was not generated successfully."))
                    return reject("Failed to create document: " + generateFnodResponse.UploadMessage);
                }

                if (Va.Udo.Crm.Uci.Scripts.Downloader !== null && Va.Udo.Crm.Uci.Scripts.Downloader !== undefined) {
                    if (generateFnodResponse.FileName.indexOf(".docx") > -1) {
                        generateFnodResponse.MimeType = "application/msword";
                        generateFnodResponse.FileName = generateFnodResponse.FileName.replace(".docx", ".doc");
                    }

                    var params = {
                        webResourceName: webresource,
                        fileName: generateFnodResponse.FileName,
                        contentType: generateFnodResponse.MimeType,
                        b64Data: generateFnodResponse.Base64FileContents
                    }
                    var downloader = new Va.Udo.Crm.Uci.Scripts.Downloader(executionContext, params);
                    downloader.DownloadAsync();
                    return resolve(true);
                }
            } catch (e) {
                console.log("An error occurred in initiateDocDownloadAsync: " + e);
                reject(e);
            }
        });
    }
}

function populateEnclosures(executionContext) {
    var formContext = executionContext.getFormContext();
    var fnodId = formContext.data.entity.getId().replace("{", "").replace("}", "");
    var enclosures = "";

    var options = "?$select=va_fnodid&$expand=udo_va_fnod_va_externaldocument($select=va_externaldocumentid, va_name, va_documentlocation)";

    Xrm.WebApi.retrieveRecord("va_fnod", fnodId, options).then(function (data) {
        if (data && data.udo_va_fnod_va_externaldocument.length > 0) {
            var externalDocCollection = data.udo_va_fnod_va_externaldocument;
            for (var i = 0; i < externalDocCollection.length; i++) {
                var r = externalDocCollection[i].va_externaldocumentid;
                enclosures += (enclosures.length > 0 ? '\n' : '') + externalDocCollection[i].va_name;
            }

            if (formContext.getAttribute('udo_enclosures').getValue() !== enclosures) {
                formContext.getAttribute('udo_enclosures').setValue(enclosures);
                // update data to make sure enclosures get on report
                if (formContext.ui.getFormType() === 2) {
                    var patchRequestObj = { udo_enclosures: enclosures };
                    Xrm.WebApi.updateRecord("va_fnod", fnodId, patchRequestObj).then(function (data) {

                    }, function (error) {
                        showMessage("An Unexpected Error Occurred", "Failed to update servicerequest enclosures");
                        console.log("An error occurred in populateEnclosures: Failed to update servicerequest enclosures: " + error.message);
                    });
                }
            }
        }
    }, function (error) {
        showMessage("An Unexpected Error Occurred", "Failed to retrieve service request external document data");
        console.log("An error occurred in populateEnclosures: Failed to retrieve service request external document data: " + error.message);
    });
}

function validateDateOfDeathNotFuture(executionContext) {
    var formContext = executionContext.getFormContext();
    var dod = formContext.getAttribute("va_dateofdeath").getValue();

    if (dod === null) {
        formContext.getControl("va_dateofdeath").clearNotification();
        return;
    }

    var dodYear = dod.getFullYear();
    var dodMonth = dod.getMonth();
    var dodDay = dod.getDate();
    var today = new Date();
    var todayYear = today.getFullYear();
    var todayMonth = today.getMonth();
    var todayDay = today.getDate();
    var isError = false;

    if ((dodYear > todayYear) || ((dodYear === todayYear) && (dodMonth > todayMonth)) || ((dodYear === todayYear) && (dodMonth === todayMonth) && (dodDay > todayDay))) {
        isError = true;
    }

    if (isError === true) {
        formContext.getControl("va_dateofdeath").setNotification("Date of Death cannot be in the future", "dodNotification");
    } else {
        formContext.getControl("va_dateofdeath").clearNotification();
    }
}

function disableBpfControls(executionContext) {
    var formContext = executionContext.getFormContext();
    var bpfFields = [
        "va_ranfnod",
        "va_modeligible",
        "va_ranmod",
        "udo_submitmodfailed",
        "udo_ran0820f",
        "va_ranpmc",
        "va_ranpmc_1",
        "udo_ran0820a",
        "udo_randbl"
    ]

    var currentControl = null;
    bpfFields.forEach(function (field) {
        currentControl = formContext.getControl("header_process_" + field);

        if (currentControl !== null && currentControl !== undefined) {
            currentControl.setDisabled(true);
        }
    });
}

function setOptionSet(executionContext, optionSetField, textValue) {
    var formContext = executionContext.getFormContext();
    var options = formContext.getAttribute(optionSetField).getOptions();
    var length = options.length;
    var i;
    for (i = 0; i < length; i++) {
        if (options[i].text === textValue) {
            formContext.getAttribute(optionSetField).setValue(options[i].value);
            break;
        }
    }

    // If no match, set to null
    formContext.getAttribute(optionSetField).setValue(null);
}

function displayModBanner(executionContext) {
    var formContext = executionContext.getFormContext();
    var modTabObj = formContext.ui.tabs.get("mod_tab");
    var notifyId = "modEligibleNotify";

    if (modTabObj.getDisplayState() === "expanded" && formContext.getAttribute("va_modeligible").getValue() === false) {
        var message = "Please submit 0820F. Click Next to navigate to the 0820F form.";
        formContext.ui.setFormNotification(message, "INFO", notifyId);
    }
}

function hideModBanner(executionContext) {
    var formContext = executionContext.getFormContext();
    var modTabObj = formContext.ui.tabs.get("mod_tab");
    var notifyId = "modEligibleNotify";
    formContext.ui.clearFormNotification(notifyId);
}

function openDblCrmReport(executionContext, reportName, fnodid) {
    var formContext = executionContext.getFormContext();
    var globalContext = Xrm.Utility.getGlobalContext();
    var reportId = null;
    var helpId = null;
    var fnodId = fnodid;

    // Set API options
    var options = "?$select=reportid,name,filename&$filter=name eq '" + reportName + "'";

    // Save record
    formContext.data.save().then(function () {
        return Xrm.WebApi.retrieveMultipleRecords("report", options);
    }).then(function (result) {
        // Confirm one report returned
        if (result.entities.length !== 1) {
            throw "Number of matching reports does not equal one";
        }

        // Set reportId and fileName
        reportId = result.entities[0].reportid;
        helpId = result.entities[0].filename;

        // Open Report
        if (checkIsUSD()) {
            window.open("http://event/?eventname=OpenDblReport-FNOD&helpID=" + helpId + "&id=" + reportId + "&FNODID=" + fnodId);
        } else {
            var crmBaseUrl = globalContext.getClientUrl();
            var reportBaseUrl = "/crmreports/viewer/viewer.aspx?action=run&helpID={0}&id={1}&p:FNODID={2}";

            var reportUrl = crmBaseUrl + reportBaseUrl.replace("{0}", helpId).replace("{1}", reportId).replace("{2}", fnodId);

            Xrm.Navigation.openUrl(reportUrl);
        }
    });
}

function getOneOrOther(primaryFieldVal, backupFieldValsArray) {
    if (primaryFieldVal !== null && primaryFieldVal !== undefined) {
        return primaryFieldVal;
    }
    var i;
    for (i = 0; i < backupFieldValsArray.length; i++) {
        if (backupFieldValsArray[i] !== null && backupFieldValsArray[i] !== undefined) {
            return backupFieldValsArray[i];
        }
    }

    return null;
}

function closeAndReviewSummaryAsync(executionContext) {
    return new Promise(function (resolve, reject) {
        try {
            var formContext = executionContext.getFormContext();

            formContext.data.save().then(function (success) {
                var Id = formContext.data.entity.getId().replace("{", "").replace("}", "");
                return UploadNoteAsync(Id).then(function (result) {
                    if (result) {
                        if (checkIsUSD()) {
                            window.open("http://uii/" + encodeURIComponent("FNOD Form") + "/Close");
                        } else {
                            formContext.ui.close();
                        }
                    } else {
                        return Promise.reject("An issue occurred when uploading the note.");
                    }
                }).catch(function (error) {
                    console.log("An error occurred in closeAndReviewSummaryAsync catch: " + error);
                    return resolve(false);
                });
            });
        } catch (e) {
            console.log("An error occurred in closeAndReviewSummaryAsync: " + e);
            reject(e);
        }
    });
}

function checkIsUSD() {
    if ((window.IsUSD !== undefined && window.IsUSD !== null && window.IsUSD === true) || (parent.window.IsUSD !== undefined && parent.window.IsUSD !== null && parent.window.IsUSD === true)) {
        return true;
    } else {
        return false;
    }
}

function rebuildAllButtonsAsync(executionContext) {
    //console.log("Rebuilding all buttons");

    var formContext = executionContext.getFormContext();
    var activeProcess = formContext.data.process.getActiveProcess();

    if (activeProcess.getName() === "FNOD Process") {
        buildNavigationButtonsAsync(executionContext);
    }

    var allTabs = formContext.ui.tabs;
    var expandedTabs = [];

    allTabs.forEach(function (tab) {
        if (tab.getDisplayState() === "expanded") {
            expandedTabs.push(tab);
        }
    });

    buildFormTabButtonsAsync(executionContext, expandedTabs);
}

function callerStateOnChange(executionContext) {
    var formContext = executionContext.getFormContext();
    var textVal = formContext.getAttribute("udo_callerstatelist").getText();
    formContext.getAttribute("udo_callerstate").setValue(textVal);
}

function setValueFireOnChange(executionContext, fieldName, value) {
    var formContext = executionContext.getFormContext();
    var attr = formContext.getAttribute(fieldName);
    attr.setValue(value);
    attr.fireOnChange();
    return true;
}

/// <summary>Validates the "SSN" field passed into the function</summary>
/// <param name="execContext" type="Object">executionContext passed from the form event handler</param>
/// <param name="fieldName" type="string">name of the field to validate the SSN for</param>
/// <returns type="Boolean">Returns true if the function executed successfully</returns>
function ValidateSsn(execContext, fieldName, allowNull) {
    try {
        var formContext = execContext.getFormContext();
        var ssnControl = formContext.getControl(fieldName);
        var ssnNotificationId = "SsnValidationNotification";
        var attr = formContext.getAttribute(fieldName);

        function validateNineDigitNumber(ssnString) {
            // Regex to check for 9 digit number
            var re = /^[0-9]{9}$/;
            return re.test(ssnString);
        }

        ssnControl.clearNotification(ssnNotificationId);

        if (attr.getValue() === null && allowNull) {
            return;
        }

        if (!validateNineDigitNumber(attr.getValue())) {
            ssnControl.addNotification({
                messages: ["SSN must be nine (9) digits"],
                notificationLevel: "ERROR",
                uniqueId: ssnNotificationId
            });
        }

        return true;
    } catch (e) {
        console.log("An error occurred within ValidateSsn: " + e);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}

function RemoveAllValidation(executionContext) {
    try {
        removeSpouseSsnValidation(executionContext);
        removeVeteranHasSurvivingSpouseOnChange(executionContext);
    } catch (e) {
        console.log("An error occurred within RemoveAllValidation: " + e);
        rebuildAllButtonsAsync(execContext);
        throw (e);
    }
}
