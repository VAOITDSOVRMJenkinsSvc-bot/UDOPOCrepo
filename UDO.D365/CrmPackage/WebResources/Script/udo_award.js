"use strict";

var globalCon = Xrm.Utility.getGlobalContext();
// window.parent.USDCreateServiceRequest = USDCreateServiceRequest;
// window.parent.USDCreateLetter = USDCreateLetter;

function USDCreateLetter(context, idProofId, interactionId, contactId, data) {
    var title = "Create Letter?";
    var message = "Please confirm you would like to use the information on this award to create a letter.";
    UDO.Shared.openConfirmDialog(message, title, 300, 600, "Yes", "No")
        .then(function (confirm) {
            if (confirm.confirmed) {
                var message = "Creating Letter";
                USDCreateLetterOrServiceRequest(context, idProofId, "udo_lettergeneration", "udo_InitiateLetter", message, data);
            }
            else {
                return;
            }
        });


}

function USDCreateServiceRequest(context, idProofId, interactionId, vetId, data) {
    var title = "Create Service Request?";
    var message = "Please confirm you would like to use the information on this award to create a service request.";
    UDO.Shared.openConfirmDialog(message, title, 300, 600, "Yes", "No")
        .then(function (confirm) {
            if (confirm.confirmed) {
                var message = "Creating Service Request";
                USDCreateLetterOrServiceRequest(context, idProofId, "udo_servicerequest", "udo_InitiateServiceRequest", message, data);
            }
            else {
                return;
            }
        });
}

var lib;
var webApi;

function USDCreateLetterOrServiceRequest(context, idProofId, targetEntity, messageName, progressMsg, data)
{
    UDO.Shared.ShowProgressIndicator(progressMsg);
    var updateinfo = {};
    var awardId = UDO.Shared.GetCurrentRecordIdFormatted();

    if (messageName === "udo_InitiateLetter") {
        updateinfo["udo_awardid@odata.bind"] = "/udo_awards(" + awardId.replace("{", "").replace("}", "") + ")";
    }
    else if (messageName === "udo_InitiateServiceRequest") {
        if (data !== null && typeof data !== "undefined" && data !== "") {
            Object.keys(data).forEach(function (key) {
                if (data[key].hasOwnProperty("Id") && data[key].hasOwnProperty("LogicalName")) {
                    updateinfo[key.toLowerCase() + "@odata.bind"] = "/" + data[key].LogicalName + "s(" + data[key].Id.replace("{", "").replace("}", "") + ")";
                }
                else {
                    updateinfo[key.toLowerCase()] = data[key];
                }
            });
        }
    }
	
	function create_callback(data) {
        if (data.DataIssue !== false || data.Timeout !== false || data.Exception !== false) {
            UDO.Shared.CloseProgressIndicator();
            UDO.Shared.openAlertDialog("an issue occurred while initializing the request: " + data.ResponseMessage, "", 120, 260);
	    }
	    else {
	        if (data !== null) {
                var targetId = targetEntity === "udo_lettergeneration" ? data.result.udo_lettergenerationid : data.result.udo_servicerequestid;
                webApi.UpdateRecord(targetId, targetEntity, updateinfo)
                    .then(function () {
                        // var url = globalCon.getClientUrl() + "/main.aspx?etn=" + targetEntity + "&pagetype=entityrecord&id=" +
                        //     targetId;

                        UDO.Shared.GetCurrentAppProperties().then(
                            function (appProperties) {
                                var appId = appProperties.appId;
                                var targetTab = targetEntity === "udo_lettergeneration" ? "LetterGeneration" : "UdoServiceRequest";

                                var url = "http://uii/" + targetTab + "/Navigate?url=" + globalCon.getClientUrl() + "/main.aspx?appid=" + appId + "&cmdbar=false&navbar=off&newWindow=true&pagetype=entityrecord&etn=" + targetEntity + "&id=" + targetId;
                                //var url = globalCon.getClientUrl() + "/main.aspx?appid=" + appId + "&newWindow=true&pagetype=entityrecord&etn=" + targetEntity + "&id=" + targetId;

                                if (parent.window.IsUSD === true) {
                                    window.open(url);

                                    if (targetTab === "UdoServiceRequest") {
                                        window.open("http://uii/Global Manager/ShowTab?UdoServiceRequest");
                                    }

                                    if (targetTab === "LetterGeneration") {
                                        window.open("http://uii/Global Manager/ShowTab?LetterGeneration");
                                    }
                                }
                                UDO.Shared.CloseProgressIndicator();       
                            },
                            function (error) {
                                console.log(error);
                            });
                    })
                    .catch(function (err) {
                        UDO.Shared.CloseProgressIndicator();
                        UDO.Shared.openAlertDialog(err.message, "", 120, 260);
                    });
	        }
	    }
	}

    function create() {
        var cols = ["udo_personid"];
        var filter = "$filter=udo_ptcpntid eq '" + data.udo_ParticipantID + "' and _udo_idproofid_value eq " + idProofId;
        //var filter = "$filter=_udo_idproofid_value eq " + idProofId + " and _udo_awardid_value eq " + awardId;
        //$filter=_udo_awardid_value eq awardGuid and  _udo_idproofid_value eq idProofGuid
        webApi.RetrieveMultiple("udo_person", cols, filter)
            .then(function (data) {
                if (data.length !== 0) {
                    var personid = data[0].udo_personid;
                    RunOnlineExecute(personid, messageName);    
                } else {
                    UDO.Shared.CloseProgressIndicator();
                }
            }).catch(function (err) {
                UDO.Shared.CloseProgressIndicator();
                UDO.Shared.openAlertDialog(err.message, "", 120, 260);
            });
    }
    
    function BuildRequest(personId, messageName) {
        var parameters = {};
        var parententityreference = {};
        var udo_Request;
        parententityreference.id = personId;
        parententityreference.entityType = "udo_person";
        parameters.ParentEntityReference = parententityreference;

        if (messageName === "udo_InitiateServiceRequest") {
            parameters.NoPayeeDetails = false;

            var udo_InitiateServiceRequest = {
                ParentEntityReference: parameters.ParentEntityReference,
                NoPayeeDetails: parameters.NoPayeeDetails,

                getMetadata: function () {
                    return {
                        boundParameter: null,
                        parameterTypes: {
                            "ParentEntityReference": {
                                "typeName": "EntityReference",
                                "structuralProperty": 5
                            },
                            "NoPayeeDetails": {
                                "typeName": "Edm.Boolean",
                                "structuralProperty": 1
                            }
                        },
                        operationType: 0,
                        operationName: messageName
                    };
                }
            };
            udo_Request = udo_InitiateServiceRequest;
        }
        else {
            var udo_InitiateLetterRequest = {
                ParentEntityReference: parameters.ParentEntityReference,

                getMetadata: function () {
                    return {
                        boundParameter: null,
                        parameterTypes: {
                            "ParentEntityReference": {
                                "typeName": "EntityReference",
                                "structuralProperty": 5
                            }
                        },
                        operationType: 0,
                        operationName: messageName
                    };
                }
            };
            udo_Request = udo_InitiateLetterRequest;
        }
        return udo_Request;
    }
    
    function RunOnlineExecute(personId, messageName) {
        var req = BuildRequest(personId, messageName);
        UDO.Shared.ExecuteAction(req)
            .then(function (data) {
                create_callback(data);
            })
            .catch(function (err) {
                UDO.Shared.CloseProgressIndicator();
                UDO.Shared.openAlertDialog(err.message, "", 120, 260);
            });
    }
	create();
}

function RunScriptAutomation(context, participantId, idProofId, interactionId, contactId) {
    UDO.Shared.FormContext = context;
    var letter = {};
    letter.udo_ParticipantID = participantId;
    USDCreateLetter(context, idProofId, interactionId, contactId, letter);
}

function USDAutomateServiceRequest(context, veteranId, regionalOfficeId, emailAddress, SSN, fileNumber, dateOfDeath, participantId, branchOfService, firstName, lastName, idProof, interactionId) {

    UDO.Shared.FormContext = context;

    var serviceRequest = {};

    if (veteranId !== "") {
        if (regionalOfficeId !== "") {
            serviceRequest.udo_RegionalOfficeId = { Id: regionalOfficeId, LogicalName: "va_regionaloffice" };
        }
        if (emailAddress !== "") {
            serviceRequest.udo_EmailofVeteran = emailAddress;
        }
        serviceRequest.udo_RelatedVeteranId = { Id: veteranId, LogicalName: "contact" };
        serviceRequest.udo_SSN = SSN;
        serviceRequest.udo_FileNumber = fileNumber;
        if (dateOfDeath !== "") {
            serviceRequest.udo_DateofDeath = dateOfDeath;
        }
        serviceRequest.udo_ParticipantID = participantId;
        serviceRequest.udo_BranchofService = branchOfService;
        serviceRequest.udo_VetFirstName = firstName;
        serviceRequest.udo_VetLastName = lastName;
    }

    USDCreateServiceRequest(context, idProof, interactionId, veteranId, serviceRequest);
}

