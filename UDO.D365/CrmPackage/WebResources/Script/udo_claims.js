"use strict";

var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
Va.Udo.Crm.Scripts.ClaimsDocumentDownload = Va.Udo.Crm.Scripts.ClaimsDocumentDownload || {};

var webApi = null;
var globalCon = Xrm.Utility.getGlobalContext();

function USDCreateLetter(context, idProofId, interactionId, contactId, data) {
    var message = "Please confirm you would like to use the information on this claim to create a letter.";
    UDO.Shared.openConfirmDialog(message, "", 300, 600, "Yes", "No")
    .then(function (success) {
        if (success.confirmed) {
            UDO.Shared.ShowProgressIndicator("Generating Letter");
            USDCreateLetterOrServiceRequest(context, idProofId, "udo_lettergeneration", "udo_InitiateLetter", null, data);
        }
        else {
            return;
        }
    });   
}

function USDCreateServiceRequest(context, idProofId, interactionId, vetId, data) {
    var title = "Create Service Request?";
    var message = "Please confirm you would like to use the information on this claim to create a letter.";
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

function USDCreateLetterOrServiceRequest(context, idProofId, targetEntity, messageName, progressMsg, data) {

    var updateinfo = {};
    var claimId = UDO.Shared.GetCurrentRecordIdFormatted();

    var updateinfo = {};
    if (messageName === "udo_InitiateLetter") {
        updateinfo["udo_claimid@odata.bind"] = "/udo_claims(" + claimId.replace("{", "").replace("}", "") + ")";
    }

    updateinfo.udo_claimnumber = context.getAttribute("udo_claimidstring").getValue();

    function error_callback(xhr, status, ethrow) {
        UDO.Shared.openAlertDialog("Error Opening Record: " + status + ":" + xhr.statusText);    }

    function create_callback(data) {
        if (data.DataIssue !== false || data.Timeout !== false || data.Exception !== false) {
            UDO.Shared.CloseProgressIndicator();
            UDO.Shared.openAlertDialog("An issue occurred while initializing the request: " + data.ResponseMessage);
        }
        else {
            if (data !== null) {
                var targetId = targetEntity === "udo_lettergeneration" ? data.result.udo_lettergenerationid : data.result.udo_servicerequestid;

                Xrm.WebApi.updateRecord(targetEntity, targetId, updateinfo)
                    .then(function () {

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
                        UDO.Shared.openAlertDialog(err.message);
                    });
            }
        }
    }

    function BuildRequest(personId, messageName) {
        var udo_Request;
        var parameters = {};
        var parententityreference = {};
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
                UDO.Shared.openAlertDialog(err.message);
            });    
    }    
    
    function create() {                
        var Entity = "udo_person";
        var Select = "?$select=udo_personid";
        var Filter = "&$filter=udo_ptcpntid eq '" + data.udo_ParticipantID + "' and _udo_idproofid_value eq " + idProofId;

        Xrm.WebApi.retrieveMultipleRecords(Entity, Select + Filter).then(
            function success(result) {
                if (result.entities.length !== 0) {
                    var personid = result.entities[0].udo_personid;
                    RunOnlineExecute(personid, messageName);
                } else {
                    UDO.Shared.CloseProgressIndicator();
                }
            },
            function (error) {
                console.log(error.message);
            }
        )
        .catch(function (err) {
            UDO.Shared.CloseProgressIndicator();
            UDO.Shared.openAlertDialog(err.message);
        });
    }
    create();
}

function RunScriptAutomation(context, ParticipantId, IdProofId, InteractionId, ContactId) {
    UDO.Shared.FormContext = context;

    var letter = {};
    letter.udo_ParticipantID = ParticipantId;  
    USDCreateLetter(context, IdProofId,InteractionId,ContactId, letter);
}

function AutomateSR(context, VeteranId, RegionalOfficeId, EmailAddress1, SSN, FileNumber, DateOfDeath, ParticipantId, BranchOfService, FirstName, LastName, ClaimIdString, IdProofId, InteractionId) {
    UDO.Shared.FormContext = context;

    var serviceRequest = {};
    if (VeteranId !== "") {

        if (RegionalOfficeId !== "")
            serviceRequest.udo_RegionalOfficeId = { Id: RegionalOfficeId, LogicalName: "va_regionaloffice" };

        if (EmailAddress1 !== "")
            serviceRequest.udo_EmailofVeteran = EmailAddress1;

        serviceRequest.udo_RelatedVeteranId = { Id: VeteranId, LogicalName: "contact" };
        serviceRequest.udo_SSN = SSN;
        serviceRequest.udo_FileNumber = FileNumber;
        serviceRequest.udo_DateofDeath = DateOfDeath;
        serviceRequest.udo_ParticipantID = ParticipantId;
        serviceRequest.udo_BranchofService = BranchOfService;
        serviceRequest.udo_VetFirstName = FirstName;
        serviceRequest.udo_VetLastName = LastName;
        serviceRequest.udo_Claim = ClaimIdString;
        serviceRequest.udo_ClaimNumber = ClaimIdString;
    }

    USDCreateServiceRequest(context, IdProofId, InteractionId, VeteranId, serviceRequest);
}

