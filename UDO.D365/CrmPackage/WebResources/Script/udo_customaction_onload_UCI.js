///<reference path="udo_CRMCommonJS.js" />
if (typeof UDO === 'undefined') { UDO = { __namespace: true }; }
var globalContext = Xrm.Utility.getGlobalContext();
var version = globalContext.getVersion();
//var lib = new CrmCommonJS.CrmCommon(version, null);
//var webApi = lib.WebApi;
//var formHelper = lib.FormHelper;
var lib;
var webApi;
var formHelper;
var execCon;

UDO.CustomActions = (function () {

    var executingActions = [];
    var allComplete;
    var pageRefreshPending = false;
    var retryCount = 0;

    function instantiateCommonScripts(exCon) {
        execCon = exCon;
        lib = new CrmCommonJS.CrmCommon(version, exCon);
        webApi = lib.WebApi;
        formHelper = new CrmCommonJS.FormHelper(exCon);
    }



    function tabOpen(exCon, completeEntity, completeField, requestName, refreshGrid, additionalParams, handleTimeout) {
        var eventSource = exCon.getEventSource();
        if (eventSource.getVisible() && eventSource.getDisplayState() === "expanded") {
            actionParams(exCon, completeEntity, completeField, requestName, refreshGrid, additionalParams, handleTimeout);
        }
    }

    function actionOnLoad(exCon, completeEntity, completeField, requestName, refreshGrid, additionalParams, handleTimeout) {
        instantiateCommonScripts(exCon);
        actionParams(exCon, completeEntity, completeField, requestName, refreshGrid, additionalParams, handleTimeout);
    }

    function actionParams(exCon, completeEntity, completeField, requestName, refreshGrid, additionalParams, handleTimeout) {
        //On VeteranHistory, for ITF loading process only.
        //Should replace with web service call
        if (formHelper.getAttribute("udo_itfloading") !== null) {
            formHelper.setValue("udo_itfloading", 752280002);
            formHelper.setSubmitMode("udo_itfloading", true);
            formHelper.saveRecord();
        }

        if (executingActions.indexOf(requestName) === -1)
            executingActions.push(requestName);
        else
            return;
        var entityName = formHelper.getEntityLogicalName();
        var entityId;

        if (entityName !== completeEntity) {
            if (!formHelper.getValue("udo_idproofid")) return;
            entityId = formHelper.getSelectedLookupId("udo_idproofid");
        }
        else {
            entityId = formHelper.getCurrentRecordIdFormatted();
        }
       
        if (completeField === "") {
            executeAction(exCon, entityId, completeEntity, requestName, refreshGrid, additionalParams, handleTimeout, false);
            return;
        } else {
            //if this entity is the complete entity and has the status field on it, 
            //then use the local status field to determine if it should be run.
            if (entityName === completeEntity) {
                var completeAttr = formHelper.getAttribute(completeField);
                if (completeAttr !== null) {
                    var completeFieldType = completeAttr.getAttributeType();
                    var completeValue = completeAttr.getValue();

                    if (completeFieldType === "optionset") {
                        //complete flag present and true
                        if (completeValue === 752280002) {
                            actionsComplete(exCon, requestName);
                            if (allComplete && pageRefreshPending) {
                                pageRefreshPending = false;
                                formHelper.refreshRecord();
                            }
                            return;
                        }
                    }
                    else if (completeFieldType === "boolean") {
                        //complete flag present and true
                        if (completeValue) {
                            actionsComplete(exCon, requestName);
                            if (allComplete && pageRefreshPending) {
                                pageRefreshPending = false;
                                formHelper.refreshRecord();
                            }
                            return;
                        }
                    }
                    else {
                        //complete flag present and true
                        actionsComplete(exCon, requestName);
                        formHelper.setFormNotification("Invalid complete flag supplied for " + requestName + ". If the issue persist please contact your System Administrator.", "WARNING", requestName);
                        if (allComplete && pageRefreshPending) {
                            pageRefreshPending = false;
                            formHelper.refreshRecord();
                        }
                        return;
                    }
                }
            }
        }
        webApi.RetrieveRecord(entityId, completeEntity, null).done(
            function (data) {
                retrieveSuccessCallBack(exCon, data, entityId, completeEntity, completeField, requestName, refreshGrid);
            }
        );
    }

    function retrieveSuccessCallBack(exCon, obj, entityId, completeEntity, completeField, requestName, refreshGrid, additionalParams, handleTimeout) {
        var Complete = false;
        Complete = obj[completeField];
        if (!Complete || (Complete && (Complete !== true && Complete !== 752280002)))
            executeAction(exCon, entityId, completeEntity, requestName, refreshGrid, additionalParams, handleTimeout);
        else {
            actionsComplete(exCon, requestName, refreshGrid);
            if (allComplete && pageRefreshPending) {
                pageRefreshPending = false;
                formHelper.refreshRecord();
            }
        }
    }

    function retrieveErrorCallBack(exCon, err, entityId, completeEntity, completeField, requestName, refreshGrid, additionalParams, handleTimeout) {
        executeAction(exCon, entityId, completeEntity, requestName, refreshGrid, additionalParams, handleTimeout);
    }

    function assembleRequest(referenceId, referenceEntity, actionName, additionalParams) {
        var parameters = {};
        var parententityreference = {};
        parententityreference[referenceEntity + "id"] = referenceId; //Delete if creating new record 
        parententityreference["@odata.type"] = "Microsoft.Dynamics.CRM." + referenceEntity;
        parameters.ParentEntityReference = parententityreference;


        parameterTypes = {
            "ParentEntityReference": {
                "typeName": "mscrm.crmbaseentity",
                "structuralProperty": 5
            }
        };
        if (additionalParams) {
            for (var i = 0; i < additionalParams.length; i++) {
                parameterTypes[additionalParams[i]["Key"]] = {
                    "typeName": additionalParams[i]["Type"],
                    "structuralProperty": 1
                };
            }
        }


        var actionRequest = {
            ParentEntityReference: parameters.ParentEntityReference,

            getMetadata: function () {


                return {
                    boundParameter: null,
                    parameterTypes: parameterTypes,
                    operationType: 0,
                    operationName: actionName
                };
            }
        };
        if (additionalParams) {
            for (var j = 0; j < additionalParams.length; j++) {
                actionRequest[additionalParams[j]["Key"]] = additionalParams[j]["Value"];
            }
        }

        return actionRequest;
    }

    function executeAction(exCon, parentEntityId, parentEntityName, requestName, refreshGrid, additionalParams, handleTimeout) {
        //var formContext = (exCon);

        formHelper.setFormNotification("Retrieving data from source systems for " + requestName + ". Please wait.", "INFO", requestName);
        var actionRequest = assembleRequest(parentEntityId, parentEntityName, requestName, additionalParams);

        Xrm.WebApi.online.execute(actionRequest).then(
            function success(result) {
                result.json().then(
                    function (response) {
                        if (response.DataIssue) {
                            Xrm.Navigation.openAlertDialog({ title: "WARNING", text: response.ResponseMessage });
                        }
                        actionsComplete(exCon, requestName, refreshGrid);
                    }
                );
            }
            ,
            function (error) {
                Xrm.Utility.alertDialog(error.message);
            }
        );
        
        // webApi.executeAction(actionRequest)
        //     .done(function (response) {
        //         var parsedResponse = JSON.parse(response.responseText);
        //         if (parsedResponse.DataIssue) {
        //             Xrm.Navigation.openAlertDialog({ title: "WARNING", text: parsedResponse.ResponseMessage });
        //         }
        //         actionsComplete(exCon, requestName, refreshGrid);
        //     });
    }

    function onResponse(exCon, responseObject, parentEntityId, parentEntityName, requestName, refreshGrid, additionalParams, handleTimeout) {

        if (responseObject.Timeout !== false && handleTimeout === "true") {

            //using a "while" here even though it isn't really a while loop (breaks every time)
            if (retryCount < 50) {
                retryCount++;
                setTimeout(executeAction(exCon, parentEntityId, parentEntityName, requestName, refreshGrid, additionalParams, handleTimeout), 5000);
                return;
            }
            //On VeteranHistory, for ITF loading process only.
            //formContext.ui.setFormNotification("Retrieval of these ITFs is taking longer than expected. Right-click in the ITF grid and select 'Refresh List' to see ITF records as they are being retrieved.", "WARNING", requestName);
            formHelper.setFormNotification("Retrieval of these ITFs is taking longer than expected. Right-click in the ITF grid and select 'Refresh List' to see ITF records as they are being retrieved.", "WARNING", requestName);
            return;
        }

        actionsComplete(exCon, requestName, refreshGrid);
        if (responseObject.DataIssue !== false || responseObject.Timeout !== false || responseObject.Exception !== false) {
            formHelper.clearFormNotification(requestName);
            if (responseObject.ResponseMessage)
                formHelper.setFormNotification(responseObject.ResponseMessage, "WARNING", requestName);
            else {
                formHelper.clearFormNotification("An unexpected exception has occurred. Please try refreshing the page. If the issues persists please contact your System Administrator.", "WARNING", requestName);
            }
            if (refreshGrid) {
                var grids = refreshGrid.split(";");
                for (var i = 0; i < grids.length; i++) {
                    var subgrid = formHelper.getControl(grids[i]);

                    if (subgrid === null)
                        return;
                    subgrid.refresh();
                }
            }
        } else {
            formHelper.clearFormNotification("loading");
            if (refreshGrid) {
                var grids = refreshGrid.split(";");
                for (var i = 0; i < grids.length; i++) {
                    //var subgrid = formContext.getControl(grids[i]);
                    var subgrid = formHelper.getControl(grids[i]);
                    if (subgrid === null)
                        return;
                    subgrid.refresh();
                }
                formHelper.refreshRecord();
            }
            else {

                if ("undefined" !== typeof responseObject.EntityUpdate && !responseObject.EntityUpdate) {
                    return;
                }

                if (!allComplete)
                    pageRefreshPending = true;
                else {
                    pageRefreshPending = false;
                    formHelper.refreshRecord();
                }
            }
        }
    }

    function actionsComplete(exCon, requestName, refreshGrid) {
        formHelper.clearFormNotification(requestName);
        formHelper.refreshRecord();
        if (refreshGrid) {
            var grids = refreshGrid.split(";");
            for (var i = 0; i < grids.length; i++) {
                var subgrid = formHelper.getControl(grids[i]);

                if (subgrid === null)
                    return;
                subgrid.refresh();
            }
        }
        var i = executingActions.indexOf(requestName);
        formHelper.refreshRecord();

        if (i !== -1)
            executingActions.splice(i, 1);

        if (executingActions.length > 0)
            return false;
        else {
            allComplete = true;
            return true;
        }
    }

    return {
        OnLoad: actionOnLoad,
        OnTabOpen: tabOpen,
        OnActionParams: actionParams
        
    };
})();

UDO.CustomActions.USD = (function () {

    //Debug var is only here to allow us to quickly debug messages found in the Grid loading resource (timing issues?)
    var Debug = false;

    var destUrl = "";
    var nextEventName;
    var entityId = null;
    var entityName = null;
    var requestName = null;
    var additionalParams;
    var executingActions = [];
    var allComplete;
    var handleTimeout = null;

    var Initialize = function () {
        console.log("in load function");
        instantiateCommonScripts();
        getDataParam();
        if (executingActions.indexOf(requestName) == -1)
            executingActions.push(requestName);
        else
            return;

        $('#notFoundDiv').hide();
        $('#loadingGifDiv').focus();


        destUrl = "http://event/?eventName=" + nextEventName;

        var requestParams =
            [{
                Key: "ParentEntityReference",
                Type: Va.Udo.Crm.Scripts.Process.DataType.EntityReference,
                Value: { id: entityId, entityType: entityName }
            }]

        if (additionalParams) {
            requestParams = $.merge(additionalParams, requestParams);
        }
        //TODO: convert to webapi
        var actionRequest = assembleRequest(entityId, entityName, requestName, additionalParams);
        webApi.executeAction(actionRequest)
            .done(function (response) {
                console.log("usd action response");
                console.log(response);
                onResponse(response, requestName);
            });
    };

    function instantiateCommonScripts(exCon) {
        lib = new CrmCommonJS.CrmCommon(version, exCon);
        webApi = lib.WebApi;
        formHelper = new CrmCommonJS.FormHelper(exCon);
    }


    function onResponse(responseObject, requestName) {
        console.log(responseObject);
        if (responseObject.Timeout === true && handleTimeout === "true") {
            if (retryCount < 50) {
                retryCount++;
                setTimeout(executeAction(parentEntityId, parentEntityName, requestName,  additionalParams, handleTimeout), 5000);
                return;
            }
        }

        $('#loadingGifDiv').hide();
        actionsComplete(requestName);
        if (responseObject.status !== 200) {
            $('#notFoundDiv').text(responseObject.ResponseMessage);
            $('#notFoundDiv').show();
        } else {
            window.open(destUrl, "_self");
        }
    }

    function assembleRequest(referenceId, referenceEntity, actionName, additionalParams) {
        var parameters = {};
        var parententityreference = {};
        parententityreference[referenceEntity + "id"] = referenceId; //Delete if creating new record 
        parententityreference["@odata.type"] = "Microsoft.Dynamics.CRM." + referenceEntity;
        parameters.ParentEntityReference = parententityreference;


        parameterTypes = {
            "ParentEntityReference": {
                "typeName": "mscrm.crmbaseentity",
                "structuralProperty": 5
            }
        };
        if (additionalParams) {
            for (var i = 0; i < additionalParams.length; i++) {
                parameterTypes[additionalParams[i]["Key"]] = {
                    "typeName": additionalParams[i]["Type"],
                    "structuralProperty": 1
                };
            }
        }


        var actionRequest = {
            ParentEntityReference: parameters.ParentEntityReference,

            getMetadata: function () {


                return {
                    boundParameter: null,
                    parameterTypes: parameterTypes,
                    operationType: 0,
                    operationName: actionName
                };
            }
        };
        if (additionalParams) {
            for (var j = 0; j < additionalParams.length; j++) {
                actionRequest[additionalParams[j]["Key"]] = additionalParams[j]["Value"];
            }
        }

        return actionRequest;
    }

    function actionsComplete(requestName) {
        console.log("actionsComplete");
        var i = executingActions.indexOf(requestName);

        if (i !== -1)
            executingActions.splice(i, 1);

        if (executingActions.length > 0)
            return false;
        else {
            allComplete = true;
            return true;
        }
    }

    function getDataParam() {
        //Get the any query string parameters and load them
        //into the vals array

        var vals = new Array();
        if (location.search !== "") {
            vals = location.search.substr(1).split("&");
            for (var i in vals) {
                vals[i] = vals[i].replace(/\+/g, " ").split("=");
            }
            //look for the parameter named 'data'
            var found = false;
            for (var i in vals) {
                if (vals[i][0].toLowerCase() === "data") {
                    parseDataValue(vals[i][1]);
                    found = true;
                    break;
                }
            }
            if (!found) {
                $('#loadingGifDiv').hide();
                $('#notFoundDiv').text("No Parameters passed to dialog");
                $('#notFoundDiv').show();

                return;
            }
        }
        else {
            $('#loadingGifDiv').hide();
            $('#notFoundDiv').text("No Parameters passed to dialog");
            $('#notFoundDiv').show();

            return;
        }
    }

    function parseDataValue(datavalue) {

        if (datavalue !== "") {
            var vals = new Array();

            vals = decodeURIComponent(datavalue).split("&");
            console.log(vals);
            for (var i in vals) {
                vals[i] = vals[i].replace(/\+/g, " ").split("=");

                switch (vals[i][0].toLowerCase()) {
                    case 'entityid': entityId = vals[i][1]; break;
                    case 'entityname': entityName = vals[i][1]; break;
                    case 'requestname': requestName = vals[i][1]; break;
                    case 'nexteventname': nextEventName = vals[i][1]; break;
                    case 'additionalparams': additionalParams = vals[i][1]; break;
                    case 'handletimetout': handleTimeout = vals[i][1]; break;
                    default: break;
                }
            }
        }
        else {
            $('#loadingGifDiv').hide();
            $('#notFoundDiv').text("No Parameters passed to dialog");
            $('#notFoundDiv').show();
            return;
        }
    }

    return {
        onLoad: Initialize
    }
})();