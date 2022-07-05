///<reference path="udo_CRMCommonJS.js" />
///<reference path="udo_customaction_onload.js"/>

var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.ControlPanel = Va.Udo.Crm.ControlPanel || {};
Va.Udo.Crm.ControlPanel.VBMSeFolder = Va.Udo.Crm.ControlPanel.VBMSeFolder || {};
Va.Udo.Crm.ControlPanel.VirtualVA = Va.Udo.Crm.ControlPanel.VirtualVA || {};

window["ENTITY_SET_NAMES"] = window["ENTITY_SET_NAMES"] || JSON.stringify({
    "annotation": "annotations",
    "udo_virtualva": "udo_virtualvas",
    "systemuser": "systemusers",
    "va_systemsettings": "va_systemsettingses"
});

window["ENTITY_PRIMARY_KEYS"] = window["ENTITY_PRIMARY_KEYS"] || JSON.stringify({
    "annotation": "annotationid",
    "udo_virtualva": "udo_virtualvaid",
    //"va_systemsettings": "va_systemsettingsid",
    "systemuser": "systemuserid"
});

window.parent.Va = Va;

Va.Udo.Crm.ControlPanel = {

    isEmpty: function (str) {
        if (str && str !== '')
            return false;
        else
            return true;
    },
    WriteLog: function (msg) {
        var table = document.getElementById("LogTable");
        var row = table.insertRow(table.rows.length);
        var cell1 = row.insertCell(0);
        var cell2 = row.insertCell(1);

        var dt = new Date();
        var utcDate = dt.toUTCString();

        cell1.innerHTML = utcDate;
        cell2.innerHTML = msg;
    },
    encodeString: function (str) {

        return String(str).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');

    }
};

Va.Udo.Crm.ControlPanel.VBMSeFolder = {

    globalContext: null,
    version: null,
    lib: null,
    webApi: null,
    sec: null,
    Util: null,
    exCon: null,
    formContext: null,

    onLoad: function (execCon) {

        Va.Udo.Crm.ControlPanel.VBMSeFolder.exCon = execCon;
        Va.Udo.Crm.ControlPanel.VBMSeFolder.lib = new CrmCommonJS.CrmCommon(version, exCon);
        Va.Udo.Crm.ControlPanel.VBMSeFolder.version = exCon.getVersion();
        Va.Udo.Crm.ControlPanel.VBMSeFolder.webApi = lib.WebApi;
        Va.Udo.Crm.ControlPanel.VBMSeFolder.Util = lib.Util;
        Va.Udo.Crm.ControlPanel.VBMSeFolder.Sec = lib.Security;
        Va.Udo.Crm.ControlPanel.VBMSeFolder.globalContext = Xrm.Utility.getGlobalContext();

    },
    retrieveVBMSeFolderDocument: function (eFolderDocId) {

        try {
            Va.Udo.Crm.ControlPanel.VBMSeFolder.executeAction(eFolderDocId, "udo_vbmsefolder", "udo_GetVBMSeFolderDocuments");
        }
        catch (ex) {

            var message = { confirmButtonLabel: "OK", text: ex.message };
            var alertOptions = { height: 200, width: 500 };
            Xrm.Navigation.openAlertDialog(message, alertOptions).then(
                function success(result) {
                    console.log("Alert dialog closed");
                },
                function (error) {
                    console.log(error.message);
                }
            );
        }
    },
    executeAction: function (entityId, entityName, requestName) {

        var parentEntity = {};
        parentEntity.id = entityId;
        parentEntity.entityType = entityName;

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

        webApi.ExecuteRequest(udo_request)
            .then(
                function (response) {
                    Va.Udo.Crm.ControlPanel.VBMSeFolder.retrieveAnnotation(JSON.parse(response.responseText).result.annotationid)
                        .then(function (data) {
                            Va.Udo.Crm.ControlPanel.VBMSeFolder.loadVBMSAnnotationSuccess(data);
                        });
                })
            .catch(function (err) {

                var message = { confirmButtonLabel: "OK", text: err.message };
                var alertOptions = { height: 200, width: 500 };
                Xrm.Navigation.openAlertDialog(message, alertOptions).then(
                    function success(result) {
                        console.log("Alert dialog closed");
                    },
                    function (error) {
                        console.log(error.message);
                    }
                );

            });
    },
    retrieveAnnotation: function (annotationid) {

        return webApi.RetrieveRecord(annotationid, "annotation", null, null);

    },
    loadVBMSAnnotationSuccess: function (data) {

        Va.Udo.Crm.ControlPanel.VBMSeFolder.VBMSdownload(data.filename, data.mimetype, data.documentbody);
    },
    VBMSdownload: function (fileName, mimeType, fileContents) {

        try {
            var file = {
                fileContent: fileContents, //Contents of the file.  
                fileName: fileName, //Name of the file. 
                fileSize: fileContents.length, //Size of the file in KB. 
                mimeType: mimeType //MIME type of the file.
            };
            Xrm.Navigation.openFile(file, 1);

        } catch (err) {

            var message = { confirmButtonLabel: "OK", text: err.message };
            var alertOptions = { height: 200, width: 500 };
            Xrm.Navigation.openAlertDialog(message, alertOptions).then(
                function success(result) {
                    console.log("Alert dialog closed");
                },
                function (error) {
                    console.log(error.message);
                }
            );
        }

    }

};

Va.Udo.Crm.ControlPanel.VirtualVA = {

    globalContext: null,
    version: null,
    webApi: null,
    Util: null,
    Sec: null,
    exCon: null,
    formContext: null,
    lib: null,
    Xrm: null,
    docParams: { fileNetDocumentId: null, fileNetDocumentSource: null, documentFormatCode: null, jro: null },
    envParams: { VVABase: null, VVAUserName: null, VVAPassword: null, isPROD: null, globalDAC: null },
    userParams: { PcrId: null },

    OnLoad: function (execCon) {
        Va.Udo.Crm.ControlPanel.WriteLog("Virtual VA - OnLoad - started");

        Va.Udo.Crm.ControlPanel.VirtualVA.exCon = execCon;
        Va.Udo.Crm.ControlPanel.VirtualVA.lib = new CrmCommonJS.CrmCommon(version, exCon);
        Va.Udo.Crm.ControlPanel.VirtualVA.version = exCon.getVersion();
        Va.Udo.Crm.ControlPanel.VirtualVA.webApi = lib.WebApi;
        Va.Udo.Crm.ControlPanel.VirtualVA.Util = lib.Util;
        Va.Udo.Crm.ControlPanel.VirtualVA.Sec = lib.Security;
        Va.Udo.Crm.ControlPanel.VirtualVA.globalContext = Xrm.Utility.getGlobalContext();

        Va.Udo.Crm.ControlPanel.WriteLog("Virtual VA - OnLoad - completed");
    },
    openVVADocument: function (docGuid) {

        Va.Udo.Crm.ControlPanel.WriteLog("Virtual VA - openVVADocument - started");

        if (window.IsUSD === true) {
            window.open("http://event/?eventname=VVAShowProgressIndicator");
        } else {
            Xrm.Utility.showProgressIndicator("Virtual VA Download Started");
        }

        Va.Udo.Crm.ControlPanel.VirtualVA.loadDocParams(docGuid);
        Va.Udo.Crm.ControlPanel.WriteLog("Virtual VA - openVVADocument - completed");

    },
    loadDocParams: function (docGuid) {

        Va.Udo.Crm.ControlPanel.WriteLog("Virtual VA - loadDocParams - started");

        Xrm.WebApi.online.retrieveRecord("udo_virtualva", docGuid, "?$select=udo_documentformat,udo_documentid,udo_documentsource,udo_regionaloffice").then(
            function success(result) {
                Va.Udo.Crm.ControlPanel.VirtualVA.loadDocParamsSuccess(result);
            },
            function (error) {
                Va.Udo.Crm.ControlPanel.WriteLog("loadDocParams Error - " + error.message);

                if (window.IsUSD == true) {
                    window.open("http://event/?eventname=VVAAlertDialog&error=" + Va.Udo.Crm.ControlPanel.encodeString(error.message));
                } else {
                    Xrm.Utility.closeProgressIndicator();
                }
            });

        Va.Udo.Crm.ControlPanel.WriteLog("Virtual VA - loadDocParams - completed");
    },
    loadDocParamsSuccess: function (result) {

        Va.Udo.Crm.ControlPanel.VirtualVA.docParams.fileNetDocumentId = result["udo_documentid"];
        Va.Udo.Crm.ControlPanel.WriteLog("Virtual VA - loadDocParamsSuccess - udo_documentid " + Va.Udo.Crm.ControlPanel.VirtualVA.docParams.fileNetDocumentId);

        Va.Udo.Crm.ControlPanel.VirtualVA.docParams.fileNetDocumentSource = result["udo_documentsource"];
        Va.Udo.Crm.ControlPanel.WriteLog("Virtual VA - loadDocParamsSuccess - udo_documentsource " + Va.Udo.Crm.ControlPanel.VirtualVA.docParams.fileNetDocumentSource);

        Va.Udo.Crm.ControlPanel.VirtualVA.docParams.documentFormatCode = result["udo_documentformat"];
        Va.Udo.Crm.ControlPanel.WriteLog("Virtual VA - loadDocParamsSuccess - udo_documentformat " + Va.Udo.Crm.ControlPanel.VirtualVA.docParams.documentFormatCode);

        Va.Udo.Crm.ControlPanel.VirtualVA.docParams.jro = result["udo_regionaloffice"];
        Va.Udo.Crm.ControlPanel.WriteLog("Virtual VA - loadDocParamsSuccess - udo_regionaloffice " + Va.Udo.Crm.ControlPanel.VirtualVA.docParams.jro);

        Va.Udo.Crm.ControlPanel.VirtualVA.loadEnvParams();
    },
    loadEnvParams: function () {

        Va.Udo.Crm.ControlPanel.WriteLog("Virtual VA - loadEnvParams - started");

        Xrm.WebApi.online.retrieveMultipleRecords("va_systemsettings", "?$select=va_description,va_name&$filter=va_name eq 'VVAUser' or  va_name eq 'VVAPassword' or  va_name eq 'VVABase' or  va_name eq 'isProd' or  va_name eq 'globalDAC'").then(
            function success(results) {
                Va.Udo.Crm.ControlPanel.VirtualVA.loadEnvParamsSuccess(results);
            },
            function (error) {
                Va.Udo.Crm.ControlPanel.WriteLog("loadEnvParams Error - " + error.message);

                if (window.IsUSD == true) {
                    window.open("http://event/?eventname=VVAAlertDialog&error=" + Va.Udo.Crm.ControlPanel.encodeString(error.message));
                } else {
                    Xrm.Utility.closeProgressIndicator();
                }
            });

        Va.Udo.Crm.ControlPanel.WriteLog("Virtual VA - loadEnvParams - Completed");
    },
    loadEnvParamsSuccess: function (retrievedSystemSettings) {
        for (var i = 0; i < retrievedSystemSettings.entities.length; i++) {

            var settingName = retrievedSystemSettings.entities[i].va_name;
            switch (settingName) {
                case 'VVAUser':
                    Va.Udo.Crm.ControlPanel.VirtualVA.envParams.VVAUserName = retrievedSystemSettings.entities[i].va_description;
                    break;
                case 'VVAPassword':
                    Va.Udo.Crm.ControlPanel.VirtualVA.envParams.VVAPass = retrievedSystemSettings.entities[i].va_description;
                    break;
                case 'VVABase':
                    Va.Udo.Crm.ControlPanel.VirtualVA.envParams.VVABase = retrievedSystemSettings.entities[i].va_description;
                    break;
                case 'isProd':
                    Va.Udo.Crm.ControlPanel.VirtualVA.envParams.isPROD = retrievedSystemSettings.entities[i].va_description;
                    break;
                case 'globalDAC':
                    Va.Udo.Crm.ControlPanel.VirtualVA.envParams.globalDAC = retrievedSystemSettings.entities[i].va_description;
                    break;
                default:
            }
            Va.Udo.Crm.ControlPanel.WriteLog("Virtual VA - loadEnvParamsSuccess - " + retrievedSystemSettings.entities[i].va_name + " - " + retrievedSystemSettings.entities[i].va_description);
        }
        if (Va.Udo.Crm.ControlPanel.isEmpty(Va.Udo.Crm.ControlPanel.VirtualVA.envParams.VVAUserName) || Va.Udo.Crm.ControlPanel.isEmpty(Va.Udo.Crm.ControlPanel.VirtualVA.envParams.VVAPass) || Va.Udo.Crm.ControlPanel.isEmpty(Va.Udo.Crm.ControlPanel.VirtualVA.envParams.VVABase) || Va.Udo.Crm.ControlPanel.isEmpty(Va.Udo.Crm.ControlPanel.VirtualVA.envParams.globalDAC))
            Va.Udo.Crm.ControlPanel.WriteLog("Cannot complete action.  Issue with environment settings.");

        Va.Udo.Crm.ControlPanel.VirtualVA.loadUserParams();
    },
    loadUserParams: function () {

        Va.Udo.Crm.ControlPanel.WriteLog("Virtual VA - loadUserParams - started");

        Va.Udo.Crm.ControlPanel.VirtualVA.globalContext = Xrm.Utility.getGlobalContext();
        var curUser = Va.Udo.Crm.ControlPanel.VirtualVA.globalContext.userSettings.userId;
        var curUserId = curUser.charAt(0) === "{" ? curUser.substring(1, 37) : curUser;

        Xrm.WebApi.online.retrieveRecord("systemuser", curUserId, "?$select=va_wsloginname,domainname").then(
            function success(result) {
                Va.Udo.Crm.ControlPanel.VirtualVA.loadUserParamsSuccess(result);
            },
            function (error) {
                Va.Udo.Crm.ControlPanel.WriteLog("loadUserParams Error - " + error.message);

                if (window.IsUSD == true) {
                    window.open("http://event/?eventname=VVAAlertDialog&error=" + Va.Udo.Crm.ControlPanel.encodeString(error.message));
                } else {
                    Xrm.Utility.closeProgressIndicator();
                }
            });

        Va.Udo.Crm.ControlPanel.WriteLog("Virtual VA - loadUserParams - Completed");
    },
    loadUserParamsSuccess: function (result) {
        if (result["va_wsloginname"] && result["va_wsloginname"].length > 0) {
            Va.Udo.Crm.ControlPanel.VirtualVA.userParams.PcrId = result["va_wsloginname"];
            Va.Udo.Crm.ControlPanel.WriteLog("Virtual VA - loadUserParamsSuccess - va_wsloginname = " + Va.Udo.Crm.ControlPanel.VirtualVA.userParams.PcrId);
        } else {
            slashPos = result["domainname"].indexOf('\\');
            Va.Udo.Crm.ControlPanel.VirtualVA.userParams.PcrId = result["domainname"].substr(slashPos + 1);
            Va.Udo.Crm.ControlPanel.WriteLog("Virtual VA - loadUserParamsSuccess - domainname = " + Va.Udo.Crm.ControlPanel.VirtualVA.userParams.PcrId);
        }
        Va.Udo.Crm.ControlPanel.VirtualVA.executeVVA();
    },
    executeVVA: function executeVVA() {

        Va.Udo.Crm.ControlPanel.WriteLog("Virtual VA - executeVVA - started");

        var env = '<?xml version="1.0" encoding="utf-8"?>'
            + '<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">'
            + '<soap:Body><DownloadVVADocument xmlns="http://tempuri.org/">'
            + '<vvaServiceUri>' + Va.Udo.Crm.ControlPanel.VirtualVA.envParams.VVABase
            + 'VABFI/services/vva' + '</vvaServiceUri>'
            + '<userName>' + Va.Udo.Crm.ControlPanel.VirtualVA.envParams.VVAUserName + '</userName>'
            + '<password>' + Va.Udo.Crm.ControlPanel.VirtualVA.envParams.VVAPass + '</password>'
            + '<fnDocId>' + Va.Udo.Crm.ControlPanel.VirtualVA.docParams.fileNetDocumentId + '</fnDocId>'
            + '<fnDocSource>' + Va.Udo.Crm.ControlPanel.VirtualVA.docParams.fileNetDocumentSource + '</fnDocSource>'
            + '<docFormatCode>' + Va.Udo.Crm.ControlPanel.VirtualVA.docParams.documentFormatCode + '</docFormatCode>'
            + '<jro>' + Va.Udo.Crm.ControlPanel.VirtualVA.docParams.jro + '</jro>'
            + '<userId>' + Va.Udo.Crm.ControlPanel.VirtualVA.userParams.PcrId + '</userId>'
            + '</DownloadVVADocument></soap:Body></soap:Envelope>';

        Va.Udo.Crm.ControlPanel.WriteLog(env);

        var request = new ActiveXObject('Microsoft.XMLHTTP');

        request.open('POST', Va.Udo.Crm.ControlPanel.VirtualVA.envParams.globalDAC, true);
        request.setRequestHeader('SOAPAction', '');
        request.setRequestHeader('Content-Type', 'text/xml; charset=utf-8');
        request.setRequestHeader('Content-Length', env.length);
        request.onreadystatechange = function () {
            if (request.readyState === 4 && request.status === 200) { /* complete */
                if (request.responseXML) {
                    // check for error VVAFAULT
                    //var response = request.responseXML.text;
                    var startPos = request.responseXML.text.indexOf("://") + 3; //remove the https://
                    var endPos = request.responseXML.text.substring(startPos).indexOf("/"); //the first / after https:// is the end of the domain
                    var dns = request.responseXML.text.substring(startPos, endPos + startPos);

                    if (request.responseXML.text && request.responseXML.text.length > 7 && request.responseXML.text.substring(0, 8) === 'VVAFAULT') {

                        if (window.IsUSD == true) {
                            window.open("http://event/?eventname=VVAAlertDialog&error=" + Va.Udo.Crm.ControlPanel.encodeString(request.responseXML.text.substring(8)));
                        } else {
                            Xrm.Utility.closeProgressIndicator();
                            message = { confirmButtonLabel: "OK", text: "Virtual VA Error \n\n" + request.responseXML.text.substring(8) };
                            alertOptions = { height: 200, width: 500 };
                            Xrm.Navigation.openAlertDialog(message, alertOptions).then(
                                function success(result) {
                                    console.log("Alert dialog closed");
                                },
                                function (error) {
                                    console.log(error.message);
                                }
                            );
                        }
                    }
                    //verify that it is redirecting to a va.gov site and the path includes /dac/vva (case insensitive)
                    else if ((request.responseXML.text.toLowerCase().indexOf('/dac/vva') > 0) && (dns.substring(dns.length - 7).toLowerCase() === '.va.gov')) {

                        if (window.IsUSD === true) {
                            window.open("http://event/?eventname=VVACloseProgressIndicator");
                        } else {
                            Xrm.Utility.closeProgressIndicator();
                        }

                        if (validInput(request.responseXML.text)) {
                            window.open(encodeURI(request.responseXML.text));
                        }
                        else {
                            console.log("Invalid character(s) found in url: " + request.responseXML.text);
                        }
                    }
                    else {

                        if (window.IsUSD === true) {
                            window.open("http://event/?eventname=VVAAlertDialog&error=" + "Invalid Virtual VA document location: " + Va.Udo.Crm.ControlPanel.encodeString(request.responseXML.text));
                        } else {
                            Xrm.Utility.closeProgressIndicator();

                            var message = { confirmButtonLabel: "OK", text: "Invalid Virtual VA document location: " + request.responseXML.text };
                            var alertOptions = { height: 200, width: 500 };
                            Xrm.Navigation.openAlertDialog(message, alertOptions).then(
                                function success(result) {
                                    console.log("Alert dialog closed");
                                },
                                function (error) {
                                    console.log(error.message);
                                }
                            );
                        }
                    }
                }
                Va.Udo.Crm.ControlPanel.WriteLog("Virtual VA - executeVVA - Completed");
            } else {
                Va.Udo.Crm.ControlPanel.WriteLog("Virtual VA - executeVVA - Ready State " + request.readyState + " Status " + request.status + " Response " + request.responseText);

                if (request.readyState === 4) {

                    if (window.IsUSD === true) {
                        window.open("http://event/?eventname=VVAAlertDialog&error=" + Va.Udo.Crm.ControlPanel.encodeString(request.responseXML.text));
                    } else {
                        Xrm.Utility.closeProgressIndicator();

                        message = { confirmButtonLabel: "OK", text: "Virtual VA Error \n\n" + request.responseXML.text };
                        alertOptions = { height: 200, width: 500 };
                        Xrm.Navigation.openAlertDialog(message, alertOptions).then(
                            function success(result) {
                                console.log("Alert dialog closed");
                            },
                            function (error) {
                                console.log(error.message);
                            }
                        );
                    }

                }
            }
        };
        request.send(env);
    }

};

function getIdFromUrl(url) {
    var str = url;
    var docGuid = str.substring(str.lastIndexOf("id=") + 3);
    Va.Udo.Crm.ControlPanel.VirtualVA.openVVADocument(docGuid);
}

function validInput(text) {
    var reg = /^[a-z0-9!?@#\$%\^\&*\)\(+=._-]+$/i;
    if (reg.test(text)) {
        return true;
    }
    else {
        return false;
    }
}