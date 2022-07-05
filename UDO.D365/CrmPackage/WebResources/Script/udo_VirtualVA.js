"use strict";
///<reference path="udo_CRMCommonJS.js" />
///<reference path="udo_customaction_onload.js"/>
///<reference path="udo_Shared.js"/>

//var Va = Va || {};
//Va.Udo = Va.Udo || {};
//Va.Udo.Crm = Va.Udo.Crm || {};
//Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
//Va.Udo.Crm.Scripts.VirtualVA = Va.Udo.Crm.Scripts.VirtualVA || {};

//window["ENTITY_SET_NAMES"] = window["ENTITY_SET_NAMES"] || JSON.stringify({
//    "udo_virtualva": "udo_virtualvas",
//    "systemuser": "systemusers",
//    "va_systemsettings": "va_systemsettingses"
//});

//window["ENTITY_PRIMARY_KEYS"] = window["ENTITY_PRIMARY_KEYS"] || JSON.stringify({
//    "udo_virtualva": "udo_virtualvaid",
//    //"va_systemsettings": "va_systemsettingsid",
//    "systemuser": "systemuserid"
//});

////window.parent.Va = Va;

//Va.Udo.Crm.Scripts.VirtualVA = {

//    webApi: null,
//    Util: null,
//    Sec: null,
//    exCon: null,
//    formContext: null,
//    lib: null,
//    Xrm: null,
//    docParams: { fileNetDocumentId: null, fileNetDocumentSource: null, documentFormatCode: null, jro: null },
//    envParams: { VVABase: null, VVAUserName: null, VVAPassword: null, isPROD: null, globalDAC: null },
//    userParams: { PcrId: null },

//    OnLoad: function (execCon) {
//        Va.Udo.Crm.Scripts.VirtualVA.WriteLog("Virtual VA - OnLoad - started");
//        Va.Udo.Crm.Scripts.VirtualVA.exCon = execCon;
//        Va.Udo.Crm.Scripts.VirtualVA.InstantiateCommonScripts(Va.Udo.Crm.Scripts.VirtualVA.exCon);
//        Va.Udo.Crm.Scripts.VirtualVA.WriteLog("Virtual VA - OnLoad - completed");
//    },
//    Initialize: function () {
//        if (Va.Udo.Crm.Scripts.VirtualVA.lib === null || typeof Va.Udo.Crm.Scripts.VirtualVA.lib === "undefined") {
//            if (Va.Udo.Crm.Scripts.VirtualVA.exCon === null || typeof Va.Udo.Crm.Scripts.VirtualVA.exCon === "undefined") {
//                Va.Udo.Crm.Scripts.VirtualVA.exCon = Xrm.Utilty.getGlobalContext;
//            }
//            Va.Udo.Crm.Scripts.VirtualVA.InstantiateCommonScripts(Va.Udo.Crm.Scripts.VirtualVA.exCon);
//        }
//    },
//    InstantiateCommonScripts: function (exCon) {

//        Va.Udo.Crm.Scripts.VirtualVA.WriteLog("Virtual VA - InstantiateCommonScripts - started");
//        var version = exCon.getVersion();
//        lib = new CrmCommonJS.CrmCommon(version, exCon);
//        Va.Udo.Crm.Scripts.VirtualVA.webApi = lib.WebApi;
//        Va.Udo.Crm.Scripts.VirtualVA.Util = lib.Util;
//        Va.Udo.Crm.Scripts.VirtualVA.Sec = lib.Security;

//        if (Va.Udo.Crm.Scripts.VirtualVA.Xrm === null || typeof Va.Udo.Crm.Scripts.VirtualVA.Xrm === "undefined") {
//            Va.Udo.Crm.Scripts.VirtualVA.Xrm = window.parent["Xrm"];
//        }

//        Va.Udo.Crm.Scripts.VirtualVA.webApi = new CrmCommonJS.WebApi(version);

//        Va.Udo.Crm.Scripts.VirtualVA.WriteLog("Virtual VA - InstantiateCommonScripts - completed");
//    },
//    openVVADocument: function (docGuid) {
//        Va.Udo.Crm.Scripts.VirtualVA.WriteLog("Virtual VA - openVVADocument - started");
//        Xrm.Utility.showProgressIndicator("Locating Virtual VA Document");
//        Va.Udo.Crm.Scripts.VirtualVA.loadDocParams(docGuid);
//        Va.Udo.Crm.Scripts.VirtualVA.WriteLog("Virtual VA - openVVADocument - completed");
//    },
//    loadDocParams: function (docGuid) {

//        Va.Udo.Crm.Scripts.VirtualVA.WriteLog("Virtual VA - loadDocParams - started");

//        Xrm.WebApi.online.retrieveRecord("udo_virtualva", docGuid, "?$select=udo_documentformat,udo_documentid,udo_documentsource,udo_regionaloffice").then(
//            function success(result) {
//                Va.Udo.Crm.Scripts.VirtualVA.loadDocParamsSuccess(result);
//            },
//            function (error) {
//                Va.Udo.Crm.Scripts.VirtualVA.WriteLog("loadDocParams Error - " + error.message);
//            });

//        Va.Udo.Crm.Scripts.VirtualVA.WriteLog("Virtual VA - loadDocParams - completed");
//    },
//    loadDocParamsSuccess: function (result) {

//        Va.Udo.Crm.Scripts.VirtualVA.docParams.fileNetDocumentId = result["udo_documentid"];
//        Va.Udo.Crm.Scripts.VirtualVA.WriteLog("Virtual VA - loadDocParamsSuccess - udo_documentid " + Va.Udo.Crm.Scripts.VirtualVA.docParams.fileNetDocumentId);

//        Va.Udo.Crm.Scripts.VirtualVA.docParams.fileNetDocumentSource = result["udo_documentsource"];
//        Va.Udo.Crm.Scripts.VirtualVA.WriteLog("Virtual VA - loadDocParamsSuccess - udo_documentsource " + Va.Udo.Crm.Scripts.VirtualVA.docParams.fileNetDocumentSource);

//        Va.Udo.Crm.Scripts.VirtualVA.docParams.documentFormatCode = result["udo_documentformat"];
//        Va.Udo.Crm.Scripts.VirtualVA.WriteLog("Virtual VA - loadDocParamsSuccess - udo_documentformat " + Va.Udo.Crm.Scripts.VirtualVA.docParams.documentFormatCode);

//        Va.Udo.Crm.Scripts.VirtualVA.docParams.jro = result["udo_regionaloffice"];
//        Va.Udo.Crm.Scripts.VirtualVA.WriteLog("Virtual VA - loadDocParamsSuccess - udo_regionaloffice " + Va.Udo.Crm.Scripts.VirtualVA.docParams.jro);

//        Va.Udo.Crm.Scripts.VirtualVA.loadEnvParams();
//    },
//    loadEnvParams: function () {

//        Va.Udo.Crm.Scripts.VirtualVA.WriteLog("Virtual VA - loadEnvParams - started");

//        Xrm.WebApi.online.retrieveMultipleRecords("va_systemsettings", "?$select=va_description,va_name&$filter=va_name eq 'VVAUser' or  va_name eq 'VVAPassword' or  va_name eq 'VVABase' or  va_name eq 'isProd' or  va_name eq 'globalDAC'").then(
//            function success(results) {
//                Va.Udo.Crm.Scripts.VirtualVA.loadEnvParamsSuccess(results);
//            },
//            function (error) {
//                Va.Udo.Crm.Scripts.VirtualVA.WriteLog("loadEnvParams Error - " + error.message);
//            });

//        Va.Udo.Crm.Scripts.VirtualVA.WriteLog("Virtual VA - loadEnvParams - Completed");
//    },
//    loadEnvParamsSuccess: function (retrievedSystemSettings) {
//        for (var i = 0; i < retrievedSystemSettings.entities.length; i++) {

//            var settingName = retrievedSystemSettings.entities[i].va_name;
//            switch (settingName) {
//                case 'VVAUser':
//                    Va.Udo.Crm.Scripts.VirtualVA.envParams.VVAUserName = retrievedSystemSettings.entities[i].va_description;
//                    break;
//                case 'VVAPassword':
//                    Va.Udo.Crm.Scripts.VirtualVA.envParams.VVAPass = retrievedSystemSettings.entities[i].va_description;
//                    break;
//                case 'VVABase':
//                    Va.Udo.Crm.Scripts.VirtualVA.envParams.VVABase = retrievedSystemSettings.entities[i].va_description;
//                    break;
//                case 'isProd':
//                    Va.Udo.Crm.Scripts.VirtualVA.envParams.isPROD = retrievedSystemSettings.entities[i].va_description;
//                    break;
//                case 'globalDAC':
//                    Va.Udo.Crm.Scripts.VirtualVA.envParams.globalDAC = retrievedSystemSettings.entities[i].va_description;
//                    break;
//                default:
//            }
//            Va.Udo.Crm.Scripts.VirtualVA.WriteLog("Virtual VA - loadEnvParamsSuccess - " + retrievedSystemSettings.entities[i].va_name + " - " + retrievedSystemSettings.entities[i].va_description);
//        }
//        if (Va.Udo.Crm.Scripts.VirtualVA.isEmpty(Va.Udo.Crm.Scripts.VirtualVA.envParams.VVAUserName) || Va.Udo.Crm.Scripts.VirtualVA.isEmpty(Va.Udo.Crm.Scripts.VirtualVA.envParams.VVAPass) || Va.Udo.Crm.Scripts.VirtualVA.isEmpty(Va.Udo.Crm.Scripts.VirtualVA.envParams.VVABase) || Va.Udo.Crm.Scripts.VirtualVA.isEmpty(Va.Udo.Crm.Scripts.VirtualVA.envParams.globalDAC))
//            Va.Udo.Crm.Scripts.VirtualVA.WriteLog("Cannot complete action.  Issue with environment settings.");

//        Va.Udo.Crm.Scripts.VirtualVA.loadUserParams();
//    },
//    loadUserParams: function () {

//        Va.Udo.Crm.Scripts.VirtualVA.WriteLog("Virtual VA - loadUserParams - started");

//        var curUser = Va.Udo.Crm.Scripts.VirtualVA.exCon.userSettings.userId;
//        var curUserId = curUser.charAt(0) === "{" ? curUser.substring(1, 37) : curUser;

//        Xrm.WebApi.online.retrieveRecord("systemuser", curUserId, "?$select=va_wsloginname,domainname").then(
//            function success(result) {
//                Va.Udo.Crm.Scripts.VirtualVA.loadUserParamsSuccess(result);
//            },
//            function (error) {
//                //Xrm.Utility.alertDialog(error.message);
//                Va.Udo.Crm.Scripts.VirtualVA.WriteLog("loadUserParams Error - " + error.message);
//            });

//        Va.Udo.Crm.Scripts.VirtualVA.WriteLog("Virtual VA - loadUserParams - Completed");
//    },
//    loadUserParamsSuccess: function (result) {
//        if (result["va_wsloginname"] && result["va_wsloginname"].length > 0) {
//            Va.Udo.Crm.Scripts.VirtualVA.userParams.PcrId = result["va_wsloginname"];
//            Va.Udo.Crm.Scripts.VirtualVA.WriteLog("Virtual VA - loadUserParamsSuccess - va_wsloginname = " + Va.Udo.Crm.Scripts.VirtualVA.userParams.PcrId);
//        } else {
//            slashPos = result["domainname"].indexOf('\\');
//            Va.Udo.Crm.Scripts.VirtualVA.userParams.PcrId = result["domainname"].substr(slashPos + 1);
//            Va.Udo.Crm.Scripts.VirtualVA.WriteLog("Virtual VA - loadUserParamsSuccess - domainname = " + Va.Udo.Crm.Scripts.VirtualVA.userParams.PcrId);
//        }
//        Va.Udo.Crm.Scripts.VirtualVA.executeVVA();
//    },
//    executeVVA: function executeVVA() {

//        Va.Udo.Crm.Scripts.VirtualVA.WriteLog("Virtual VA - executeVVA - started");
//        Xrm.Utility.closeProgressIndicator();
//        //if (returnError) {
//        //    alert(returnError);
//        //    return;
//        //}
//        var env = '<?xml version="1.0" encoding="utf-8"?>'
//            + '<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">'
//            + '<soap:Body><DownloadVVADocument xmlns="http://tempuri.org/">'
//            + '<vvaServiceUri>' + Va.Udo.Crm.Scripts.VirtualVA.envParams.VVABase
//            + 'VABFI/services/vva' + '</vvaServiceUri>'
//            + '<userName>' + Va.Udo.Crm.Scripts.VirtualVA.envParams.VVAUserName + '</userName>'
//            + '<password>' + Va.Udo.Crm.Scripts.VirtualVA.envParams.VVAPass + '</password>'
//            + '<fnDocId>' + Va.Udo.Crm.Scripts.VirtualVA.docParams.fileNetDocumentId + '</fnDocId>'
//            + '<fnDocSource>' + Va.Udo.Crm.Scripts.VirtualVA.docParams.fileNetDocumentSource + '</fnDocSource>'
//            + '<docFormatCode>' + Va.Udo.Crm.Scripts.VirtualVA.docParams.documentFormatCode + '</docFormatCode>'
//            + '<jro>' + Va.Udo.Crm.Scripts.VirtualVA.docParams.jro + '</jro>'
//            + '<userId>' + Va.Udo.Crm.Scripts.VirtualVA.userParams.PcrId + '</userId>'
//            + '</DownloadVVADocument></soap:Body></soap:Envelope>';

//        var request = new ActiveXObject('Microsoft.XMLHTTP');

//        request.open('POST', Va.Udo.Crm.Scripts.VirtualVA.envParams.globalDAC, true);
//        request.setRequestHeader('SOAPAction', '');
//        request.setRequestHeader('Content-Type', 'text/xml; charset=utf-8');
//        request.setRequestHeader('Content-Length', env.length);
//        request.onreadystatechange = function () {
//            if (request.readyState === 4 && request.status === 200) { /* complete */
//                if (request.responseXML) {
//                    // check for error VVAFAULT
//                    //var response = request.responseXML.text;
//                    var startPos = request.responseXML.text.indexOf("://") + 3; //remove the https://
//                    var endPos = request.responseXML.text.substring(startPos).indexOf("/"); //the first / after https:// is the end of the domain
//                    var dns = request.responseXML.text.substring(startPos, endPos + startPos);

//                    if (request.responseXML.text && request.responseXML.text.length > 7 && request.responseXML.text.substring(0, 8) === 'VVAFAULT') {
//                        //returnError = request.responseXML.text.substring(8);
//                        UDO.Shared.openAlertDialog(request.responseXML.text.substring(8));
//                    }
//                    //verify that it is redirecting to a va.gov site and the path includes /dac/vva (case insensitive)
//                    else if ((request.responseXML.text.toLowerCase().indexOf('/dac/vva') > 0) && (dns.substring(dns.length - 7).toLowerCase() === '.va.gov')) {
//                        returnUrl = request.responseXML.text;
//                        window.open(request.responseXML.text);
//                    }
//                    else {
//                        //returnError = "Invalid VVA document location: " + request.responseXML.text;
//                        UDO.Shared.openAlertDialog("Invalid VVA document location: " + request.responseXML.text);
//                    }
//                }
//                Va.Udo.Crm.Scripts.VirtualVA.WriteLog("Virtual VA - executeVVA - Completed");
//            } else {
//                Va.Udo.Crm.Scripts.VirtualVA.WriteLog("Virtual VA - executeVVA - Ready State " + request.readyState + " Status " + request.status + " Response " + request.responseText);

//                if (request.readyState === 4) {

//                    var message = { confirmButtonLabel: "OK", text: request.responseXML.text };
//                    var alertOptions = { height: 300, width: 600 };
//                    Xrm.Navigation.openAlertDialog(message, alertOptions).then(
//                        function success(result) {
//                            console.log("Alert dialog closed");
//                        },
//                        function (error) {
//                            console.log(error.message);
//                        }
//                    );
//                }
//            }
//        };
//        request.send(env);
//    },
//    isEmpty: function (str) {
//        if (str && str !== '')
//            return false;
//        else
//            return true;
//    },
//    WriteLog: function (msg) {
//        var table = document.getElementById("LogTable");
//        var row = table.insertRow(table.rows.length);
//        var cell1 = row.insertCell(0);
//        var cell2 = row.insertCell(1);

//        var dt = new Date();
//        var utcDate = dt.toUTCString();

//        cell1.innerHTML = utcDate;
//        cell2.innerHTML = msg;
//    }
//};

function VVAShowProcessIndicator() {
    Xrm.Utility.showProgressIndicator("Virtual VA Download Started");
}


function OpenVVADocumentUSD(context, virtualVAId) {
    if (virtualVAId == "") {
        UDO.Shared.openAlertDialog("Please select (double-click the record) a virtual VA document first.");
    } else {
        idStr = virtualVAId;
        UDO.Shared.openAlertDialog(idStr);
    }
}

function VVAAlertDialog(context, error) {
    UDO.Shared.openAlertDialog("Virtual VA Download Error: \n\n " + error);
}

function VVACloseProgressIndicator() {
    Xrm.Utility.closeProgressIndicator();
}