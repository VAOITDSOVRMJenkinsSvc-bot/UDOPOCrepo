"use strict";

var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
Va.Udo.Crm.Scripts.VBMSeFolderDocumentDownload = Va.Udo.Crm.Scripts.VBMSeFolderDocumentDownload || {};

//var globalContext = Xrm.Utility.getGlobalContext();
//var version = globalContext.getVersion();
//var lib;
//var webApi;
//var Util;
//var UDO = UDO || {};
var _executionContext = null;
//var _formContext = null;

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

//function instantiateCommonScripts(exCon) {
//    if (exCon) {
//        _executionContext = exCon;
//        lib = new CrmCommonJS.CrmCommon(version, exCon);
//        webApi = lib.WebApi;
//        Util = lib.Utility;
//        _formContext = exCon.getFormContext();
//    }
//}

function onLoad(executionContext) {
    _executionContext = executionContext;
    //instantiateCommonScripts(executionContext);
    var propertiesAppInsights = {
        "method": "Va.Udo.Crm.Scripts.VBMSeFolder.onLoad", "description": "Called on load of UDO VBMS eFolder form"
    };
    startTrackEvent("UDO VBMS eFolder onLoad", propertiesAppInsights);

    stopTrackEvent("UDO VBMS eFolder onLoad", propertiesAppInsights);
}

function fetchContext() {
    return _executionContext;
}
function retrieveVBMSeFolderDocument(eFolderDocId) {
    // Retrieve annotationId for vbms doc id passed in then call download.
    try {
        $('#loadingGifDiv').show();
        var requestName = "udo_GetVBMSeFolderDocuments";

        executeAction(eFolderDocId, "udo_vbmsefolder", requestName);
    }
    catch (ex) {
        $('#notFoundDiv').text("An error occurred while attempting to retrieve data. Please refresh the page and try again. If this error persists, please contact the application support team.");
        $('#notFoundDiv').show();
        $('#loadingGifDiv').hide();
    }
}

function executeAction(entityId, entityName, requestName) {
    $('#loadingGifDiv').show();

    $('#notFoundDiv').hide();
    $('#loadingGifDiv').focus();

    var parentEntity = {
        udo_vbmsefolderid: entityId,
        "@odata.type": "Microsoft.Dynamics.CRM." + entityName
    };
    //parentEntity.udo_vbmsefolderid = entityId;
    //parentEntity.entityType = entityName;

    var udo_request = {
        ParentEntityReference: parentEntity,

        getMetadata: function () {
            return {
                boundParameter: null,
                parameterTypes: {
                    "ParentEntityReference": {
                        "typeName": "mscrm.baseentity",
                        "structuralProperty": 5
                    }
                },
                operationName: requestName,
                operationType: 0
            };
        }
    };

    Xrm.WebApi.online.execute(udo_request)
        .then(
            function (response) {
                response.json().then(body => retrieveAnnotation(body.result.annotationid).then(data => loadVBMSAnnotationSuccess(data)));
            })
        .catch(function (err) {
            $('#loadingGifDiv').hide();
            $('#notFoundDiv').text("An error occurred while attempting to retrieve data. Please refresh the page and try again. If this error persists, please contact the application support team.");
            $('#notFoundDiv').show();
        });
}

function onResponse(responseObject, requestName) {
    if (responseObject.DataIssue !== false || responseObject.Timeout !== false || responseObject.Exception !== false) {
        $('#loadingGifDiv').hide();
        $('#notFoundDiv').text(responseObject.ResponseMessage);
        $('#notFoundDiv').show();
    } else {
        retrieveAnnotation(responseObject.result.id);
    }
}

function retrieveAnnotation(annotationid) {
    return Xrm.WebApi.retrieveRecord("annotation", annotationid.replace("{", "").replace("}", ""));
}

function loadVBMSAnnotationSuccess(data) {
    VBMSdownload(data.filename, data.mimetype, data.documentbody);
}

function VBMSdownload(fileName, mimeType, fileContents) {

    function dataURItoBlob(dataURI) {
        // convert base64 to raw binary data held in a string
        var byteString = atob(dataURI);
        // write the bytes of the string to an ArrayBuffer
        var ab = new ArrayBuffer(byteString.length);
        var ia = new Uint8Array(ab);
        for (var i = 0; i < byteString.length; i++) {
            ia[i] = byteString.charCodeAt(i);
        }

        // write the ArrayBuffer to a blob, and you're done
        var bb = new Blob([ab]);
        //saveAs(bb, filename); //pass blob and file name
        return bb;
    }

    try {
        var file = {
            fileContent: fileContents, //Contents of the file.  
            fileName: fileName, //Name of the file. 
            fileSize: fileContents.length, //Size of the file in KB. 
            mimeType: mimeType //MIME type of the file.
        };

        Xrm.Navigation.openFile(file, 1);

        return true;
    } catch (err) {
        $('#loadingGifDiv').hide();
        $('#notFoundDiv').text("An error occurred while attempting to retrieve data. Please refresh the page and try again. If this error persists, please contact the application support team.");
        $('#notFoundDiv').show();
    }
    return false;
}

function VBMSwriteError(error) {
    $('#loadingGifDiv').hide();
    $('#notFoundDiv').text("An error occurred while attempting to retrieve data. Please refresh the page and try again. If this error persists, please contact the application support team.");
    $('#notFoundDiv').show();
}
