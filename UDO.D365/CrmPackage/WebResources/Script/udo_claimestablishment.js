"use strict";
/// <reference path="udo_process.js" />
/// <reference path="va_CrmRestKit_2011" />
//File is only called from servicerequest.js and the call is commented out. Not used
var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.CustomAction = Va.Udo.Crm.CustomAction || {};
Va.Udo.Crm.CustomAction.ClaimEstablishment = Va.Udo.Crm.CustomAction.ClaimEstablishment || {};

Va.Udo.Crm.CustomAction.ClaimEstablishment = function () {

    var globalCon = Xrm.Utility.getGlobalContext();
    var version = globalCon.getVersion();
    var webApi = new CrmCommonJS.WebApi(version);
    var customActionCallBack = null;
    var customActionCallComplete = null;
    var errorException = null;
    var defaultTypeCodeName = "FOIA/Privacy Act Request";

    var retrieveClaimEstablishmentType = function (typecodename) {

        return new Promise(function (resolve, reject) {

            var columns = ['udo_claimestablishmenttypecodeid', 'udo_name', 'udo_typecode'];
            var filter = "$filter=udo_name eq '" + encodeURIComponent(typecodename) + "'";

            webApi.RetrieveMultiple('udo_claimestablishmenttypecode', columns, filter)
                .then(function (data) {
                    if (data && data.length > 0) {

                        //var guid = "{" + data[0].udo_claimestablishmenttypecodeid + "}";
                        //var name = data[0].udo_name;
                        var claimType = {
                            guid: "{" + data[0].udo_claimestablishmenttypecodeid + "}",
                            name: data[0].udo_name,
                            entityType: "udo_claimestablishmenttypecode"
                        };

                        return resolve(claimType);
                    }
                })
                .catch(function (err) {
                    return reject(err, "Record not found");
                });

        });
    };

    var updateClaimEstablishmentRecord = function (resultid) {

        //var dbl = $.Deferred();
        return new Promise(function (resolve, reject) {
            retrieveClaimEstablishmentType(defaultTypeCodeName)
                .then(function (claimType) {
                    var entity = {};
                    entity["udo_endproduct@odata.bind"] = "/" + claimType.entityType + "s(" + claimType.guid.replace("{", "").replace("}", "") + ")";
                    entity.udo_dateofclaim = new Date().toISOString();
                    webApi.UpdateRecord(resultid, "udo_claimestablishment", entity)
                        .then(function (updatedata) {
                            return resolve(updatedata);
                        })
                        .catch(function (err) {
                            return reject(err);
                        });
                })
                .catch(function (err){
                    return reject(err);
            });
        });
        /*var endProductEntityReference = retrieveClaimEstablishmentType(defaultTypeCodeName).fail(
            function (err) {
                dbl.reject(err);
            }).done(
                function (endproductId, endproductName, endproductEntityType) {
                    CrmRestKit2011.Update('udo_claimestablishment', resultid,
                        {
                            udo_EndProduct: {
                                "__metadata": { "type": "Microsoft.Crm.Sdk.Data.Services.EntityReference" },
                                "Id": endproductId,
                                "LogicalName": endproductEntityType
                            },
                            udo_DateofClaim: new Date()

                        }, false)
                        .fail(function (xhr, statusCode, code) {
                            dbl.reject(xhr.responseText);
                        })
                        .done(function (data, status, xhr) {
                            dbl.resolve(resultid);
                        });
                });

        return dbl.promise();*/
    };

    var initiate = function (personGuid, executeCallBack, executeCallComplete, errorCallBack) {

        // Check if Person ID i smissing
        if (!personGuid) {
            throw "Person GUID is missing - please  contact your System Administrator";
        }
        personGuid = personGuid.replace(/{/g, "").replace(/}/g, "");

        // Check for Call Back Function
        if (typeof executeCallBack !== "undefined") {
            if (typeof executeCallBack === "function") {
                customActionCallBack = executeCallBack;
            } else {
                throw executeCallBack + " function could not be found";
            }
        } else {
            customActionCallBack = null;
        }

        // Check for Call Complete Function
        if (typeof executeCallComplete !== "undefined") {
            if (typeof executeCallComplete === "function") {
                customActionCallComplete = executeCallComplete;
            } else {
                throw executeCallComplete + " function could not be found";
            }
        } else {
            customActionCallComplete = null;
        }

        // Check for Call Complete Function
        if (typeof errorCallBack !== "undefined") {
            if (typeof errorCallBack === "function") {
                errorException = errorCallBack;
            } else {
                throw errorCallBack + " function could not be found";
            }
        } else {
            errorException = null;
        }
        var parameters = {};
        var parententityreference = { id: personGuid, entityType: "udo_person" };
        parameters.ParentEntityReference = parententityreference;

        var udo_InitiateClaimEstablishmentRequest = {
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
                    operationName: "udo_InitiateClaimEstablishment"
                };
            }
        };
        /*var requestParams = null;
        requestParams =
            [
                {
                    Key: "ParentEntityReference",
                    Type: Va.Udo.Crm.Scripts.Process.DataType.EntityReference,
                    Value: {
                        id: personGuid,
                        entityType: "udo_person"
                    }
                }
            ];

        Va.Udo.Crm.Scripts.Process.ExecuteAction("udo_InitiateClaimEstablishment", requestParams)*/
        webApi.ExecuteRequest(udo_InitiateClaimEstablishmentRequest, null)
            .then(function (data) {
                data = JSON.parse(data.responseText);
                callBack(data);

                if (data.DataIssue === false && data.Timeout === false && data.Exception === false) {
                    callComplete(data.result.udo_claimestablishmentid);
                }
            })
            .catch(function (err) {

                exceptionCallBack(err);
            });

    };

    var insert = function (claimEstablishmentGuid, executeCallBack, executeCallComplete, errorCallBack) {

        // Check if Claim Establishment ID i smissing
        if (!claimEstablishmentGuid) {
            throw "Claim Establishment GUID is missing - please  contact your System Administrator";
        }
        claimEstablishmentGuid = claimEstablishmentGuid.id.replace(/{/g, "").replace(/}/g, "");

        // Check for Call Back Function
        if (typeof executeCallBack !== "undefined") {
            if (typeof executeCallBack === "function") {
                customActionCallBack = executeCallBack;
            } else {
                throw executeCallBack + " function could not be found";
            }
        } else {
            customActionCallBack = null;
        }

        // Check for Call Complete Function
        if (typeof executeCallComplete !== "undefined") {
            if (typeof executeCallComplete === "function") {
                customActionCallComplete = executeCallComplete;
            } else {
                throw executeCallComplete + " function could not be found";
            }
        } else {
            customActionCallComplete = null;
        }

        // Check for Call Complete Function
        if (typeof errorCallBack !== "undefined") {
            if (typeof errorCallBack === "function") {
                errorException = errorCallBack;
            } else {
                throw errorCallBack + " function could not be found";
            }
        } else {
            errorException = null;
        }
        var parameters = {};
        var parententityreference = { id: claimEstablishmentGuid, entityType: "udo_claimestablishment" };
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
        /*var requestParams = null;
        requestParams =
            [
                {
                    Key: "ParentEntityReference",
                    Type: Va.Udo.Crm.Scripts.Process.DataType.EntityReference,
                    Value: {
                        id: claimEstablishmentGuid,
                        entityType: "udo_claimestablishment"
                    }
                }
            ];

        Va.Udo.Crm.Scripts.Process.ExecuteAction("udo_InsertClaimEstablishment", requestParams)*/
        webApi.ExecuteRequest(udo_InsertClaimEstablishmentRequest, null)
            .then(function (data) {
                data = JSON.parse(data.responseText);
                callBack(data);

                if (data.DataIssue === false && data.Timeout === false && data.Exception === false) {
                    callComplete(data.result.udo_claimestablishmentid);
                }
            })
            .catch(function (err) {
                exceptionCallBack(err);
            });
    };

    var clear = function (claimEstablishmentGuid, executeCallBack, executeCallComplete, errorCallBack) {

        // Check if Claim Establishment ID i smissing
        if (!claimEstablishmentGuid) {
            throw "Claim Establishment GUID is missing - please  contact your System Administrator";
        }

        claimEstablishmentGuid = claimEstablishmentGuid.replace(/{/g, "").replace(/}/g, "");

        // Check for Call Back Function
        if (typeof executeCallBack !== "undefined") {
            if (typeof executeCallBack === "function") {
                Va.Udo.Crm.CustomAction.ClaimEstablishment.CustomctionCallBack = executeCallBack;
            } else {
                throw executeCallBack + " function could not be found";
            }
        } else {
            Va.Udo.Crm.CustomAction.ClaimEstablishment.CustomctionCallBack = null;
        }

        // Check for Call Complete Function
        if (typeof executeCallComplete !== "undefined") {
            if (typeof executeCallComplete === "function") {
                Va.Udo.Crm.CustomAction.ClaimEstablishment.CustomctionCallComplete = executeCallComplete;
            } else {
                throw executeCallComplete + " function could not be found";
            }
        } else {
            Va.Udo.Crm.CustomAction.ClaimEstablishment.CustomctionCallComplete = null;
        }

        // Check for Call Complete Function
        if (typeof errorCallBack !== "undefined") {
            if (typeof errorCallBack === "function") {
                Va.Udo.Crm.CustomAction.ClaimEstablishment.ErrorException = errorCallBack;
            } else {
                throw errorCallBack + " function could not be found";
            }
        } else {
            Va.Udo.Crm.CustomAction.ClaimEstablishment.ErrorException = null;
        }
        var parameters = {};
        var parententityreference = { id: claimEstablishmentGuid, entityType: "udo_claimestablishment" };
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
        /*var requestParams = null;
        requestParams =
            [
                {
                    Key: "ParentEntityReference",
                    Type: Va.Udo.Crm.Scripts.Process.DataType.EntityReference,
                    Value: {
                        id: claimEstablishmentGuid,
                        entityType: "udo_claimestablishment"
                    }
                }
            ];

        Va.Udo.Crm.Scripts.Process.ExecuteAction("udo_ClearClaimEstablishment", requestParams)*/
        webApi.ExecuteRequest(udo_ClearClaimEstablishmentRequest, null)
            .then(function (data) {
                data = JSON.parse(data.responseText);
                executeCallBack(data);

                if (data.DataIssue === false && data.Timeout === false && data.Exception === false) {
                    executeCallComplete(data.result.udo_claimestablishmentid);
                }

            })
            .catch(function (err) {

                exceptionCallBack(err);
            });
    };

    var callBack = function (data) {

        if (customActionCallBack !== null) {
            customActionCallBack(data);
        }
    };

    var callComplete = function (resultid) {

        if (customActionCallComplete !== null) {
            customActionCallComplete(resultid);
        }
    };

    var exceptionCallBack = function (err) {

        if (errorException !== null) {
            errorException(err);
        }
    };

    var createNote = function (formContext) {

        var cols = {};
        cols.udo_name = "Notes from End Product";
        //cols.udo_ClaimId = formContext.getAttribute("udo_ClaimId") === null
        //    ? null
        //    : formContext.getAttribute("udo_ClaimId").getValue();
        cols.udo_ParticipantID = formContext.getAttribute("udo_participantid") === null
            ? null
            : formContext.getAttribute("udo_participantid").getValue();

        if (formContext.getAttribute("udo_relatedveteranid") !== null) {
            var selectedItem = formContext.getAttribute("udo_relatedveteranid").getValue();
            cols.udo_VeteranId = {
                Id: selectedItem[0].id,
                LogicalName: selectedItem[0].typename,
                Name: selectedItem[0].name
            };
        }

        if (formContext.getAttribute("udo_personid") !== null) {
            var erPerson = formContext.getAttribute("udo_personid").getValue();
            cols.udo_personId = { Id: erPerson[0].id, LogicalName: erPerson[0].typename, Name: erPerson[0].name };
        }

        if (formContext.getAttribute("udo_idproofid") !== null) {
            var erIdProof = formContext.getAttribute("udo_idproofid").getValue();
            cols.udo_idProofId = {
                Id: erIdProof[0].id,
                LogicalName: erIdProof[0].typename,
                Name: erIdProof[0].name
            };
        }

        cols.udo_NoteText = "End Product - " + defaultTypeCodeName + " was inserted and cleared";
        cols.udo_editable = true;
        cols.udo_fromUDO = true;
        //cols.udo_createdt = Va.Udo.Crm.MapdNote.Shared_GetNewDateText();

        var note = CrmRestKit2011.Create("udo_note", cols).fail(function (xhr, status, errorThrown) {

            Va.Udo.Crm.MapdNote.CreateNoteLog("CreateNote - Error: " + xhr.responseJSON.error.message.value);
            //cn.reject(xhr.responseJSON.error.message.value);

        }).done(function (data, status, xhr) {

        });
    };

    return {
        RetrieveClaimEstablishmentType: retrieveClaimEstablishmentType,
        UpdateClaimEstablishmentRecord: updateClaimEstablishmentRecord,
        Initiate: initiate,
        Insert: insert,
        Clear: clear,
        CreateNote: createNote
    };

}();