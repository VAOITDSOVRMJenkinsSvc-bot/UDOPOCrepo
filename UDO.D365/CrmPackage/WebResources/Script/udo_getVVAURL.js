///<reference path="udo_CRMCommonJS.js" />
///<reference path="udo_customaction_onload.js"/>

var returnError = "";
var returnUrl = "";
var globalContext = Xrm.Utility.getGlobalContext();
var version = globalContext.getVersion();
var docParams = { fileNetDocumentId: null, fileNetDocumentSource: null, documentFormatCode: null, jro: null };
var envParams = { VVABase: null, VVAUserName: null, VVAPassword: null, isPROD: null, globalDAC: null };
var userParams = { PcrId: null };
var webApi;
var lib;

window.parent.openVVADocument = openVVADocument;


function openVVADocument(docGuid) {
    //get values of docParams, envParams, userParams
    var exCon = FetchContext();
    _instantiateCommonScripts(exCon);
    loadDocParams(docGuid);
    //loadEnvParams();
    //loadUserParams();

    //if (returnError != null) {
    //    return { Error: returnError, Url: returnUrl };
    //}
}

function instantiateCommonScripts(exCon) {
    exContext = exCon;
    lib = new CrmCommonJS.CrmCommon(version, exCon);
    webApi = lib.WebApi;
    Util = lib.Utility;
    formContext = exCon.getFormContext();
}


function fetchContext() {
    return exCon;
}

function executeVVA() {
    if (returnError) {
        alert(returnError);
        return;
    }

    //Retrieve Subscription Keys
    var subscriptionKey;
    var subscriptionKeyE;
    var subscriptionKeyS;

    var req = new XMLHttpRequest();
    req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/mcs_settings?$select=udo_ocpapimsubscriptionkey,udo_ocpapimsubscriptionkeyeast,udo_ocpapimsubscriptionkeysouth&$filter=statecode eq 0", false);
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
    req.onreadystatechange = function () {
        if (this.readyState === 4) {
            req.onreadystatechange = null;
            if (this.status === 200) {
                var results = JSON.parse(this.response);
                subscriptionKey = results.value[0]["udo_ocpapimsubscriptionkey"];
                subscriptionKeyE = results.value[0]["udo_ocpapimsubscriptionkeyeast"];
                subscriptionKeyS = results.value[0]["udo_ocpapimsubscriptionkeysouth"];
            } else {
                Xrm.Utility.alertDialog(this.statusText);
            }
        }
    };
    req.send();

    var env = '<?xml version="1.0" encoding="utf-8"?>'
        + '<soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">'
        + '<soap:Body><DownloadVVADocument xmlns="http://tempuri.org/">'
        + '<vvaServiceUri>' + envParams.VVABase
        + 'VABFI/services/vva' + '</vvaServiceUri>'
        + '<userName>' + envParams.VVAUserName + '</userName>'
        + '<password>' + envParams.VVAPass + '</password>'
        + '<fnDocId>' + docParams.fileNetDocumentId + '</fnDocId>'
        + '<fnDocSource>' + docParams.fileNetDocumentSource + '</fnDocSource>'
        + '<docFormatCode>' + docParams.documentFormatCode + '</docFormatCode>'
        + '<jro>' + docParams.jro + '</jro>'
        + '<userId>' + userParams.PcrId + '</userId>'
        + '</DownloadVVADocument></soap:Body></soap:Envelope>';

    var request = new ActiveXObject('Microsoft.XMLHTTP');

    request.open('POST', envParams.globalDAC, true);
    request.setRequestHeader('SOAPAction', '');
    request.setRequestHeader('Ocp-Apim-Subscription-Key', subscriptionKey);
    request.setRequestHeader('Ocp-Apim-Subscription-Key-E', subscriptionKeyE);
    request.setRequestHeader('Ocp-Apim-Subscription-Key-S', subscriptionKeyS);
    request.setRequestHeader('Content-Type', 'text/xml; charset=utf-8');
    request.setRequestHeader('Content-Length', env.length);
    request.onreadystatechange = function () {
        if (request.readyState == 4 && request.status == 200) { /* complete */
            if (request.responseXML) {
                // check for error VVAFAULT
                var startPos = request.responseXML.text.indexOf("://") + 3; //remove the https://
                var endPos = request.responseXML.text.substring(startPos).indexOf("/"); //the first / after https:// is the end of the domain
                var dns = request.responseXML.text.substring(startPos, endPos + startPos);

                if (request.responseXML.text && request.responseXML.text.length > 7 && request.responseXML.text.substring(0, 8) === 'VVAFAULT') {
                    alert(request.responseXML.text.substring(8));
                }
                //verify that it is redirecting to a va.gov site and the path includes /dac/vva (case insensitive)
                else if ((request.responseXML.text.toLowerCase().indexOf('/dac/vva') > 0) && (dns.substring(dns.length - 7).toLowerCase() === '.va.gov')) {
                    returnUrl = request.responseXML.text;

                    if (validInput(request.responseXML.text)) {
                        window.open(encodeURI(request.responseXML.text));
                    }
                    else {
                        console.log("Invalid character(s) found in url: " + request.responseXML.text);
                    } 
                }
                else {
                    alert("Invalid VVA document location: " + request.responseXML.text);
                }
            }
        }
    };
    request.send(env);
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

function loadDocParams(docGuid) {
    
    Xrm.WebApi.online.retrieveRecord("udo_virtualva", docGuid, "?$select=udo_documentformat,udo_documentid,udo_documentsource,udo_regionaloffice").then(
        function success(result) {
            loadDocParamsSuccess(result);
        },
        function (error) {
            //Xrm.Utility.alertDialog(error.message);
            writeError(error);
        });

}

function loadDocParamsSuccess(result) {
    docParams.fileNetDocumentId = result["udo_documentid"];
    docParams.fileNetDocumentSource = result["udo_documentsource"];
    docParams.documentFormatCode = result["udo_documentformat"];
    docParams.jro = result["udo_regionaloffice"];
    loadEnvParams();
}

function writeError(error) {
    processError(error.message);
}

function processError(msg) {
    if (returnError === "") {
        returnError = msg;
    }
    else {
        returnError = returnError + "/n " + msg;
    }
}

function loadEnvParams() {

    Xrm.WebApi.online.retrieveMultipleRecords("va_systemsettings", "?$select=va_description,va_name&$filter=va_name eq 'VVAUser' and  va_name eq 'VVAPassword' and  va_name eq 'VVABase' and  va_name eq 'isProd' and  va_name eq 'globalDAC'").then(
        function success(results) {
            loadEnvParamsSuccess(results);
        },
        function (error) {
            writeError(error);
        });

}

function loadEnvParamsSuccess(retrievedSystemSettings) {
    for (var i = 0; i < retrievedSystemSettings.length; i++) {

        var settingName = retrievedSystemSettings[i].va_name;
        switch (settingName) {
            case 'VVAUser':
                envParams.VVAUserName = retrievedSystemSettings[i].va_description;
                break;
            case 'VVAPassword':
                envParams.VVAPass = retrievedSystemSettings[i].va_description;
                break;
            case 'VVABase':
                envParams.VVABase = retrievedSystemSettings[i].va_description;
                break;
            case 'isProd':
                envParams.isPROD = retrievedSystemSettings[i].va_description;
                break;
            case 'globalDAC':
                envParams.globalDAC = retrievedSystemSettings[i].va_description;
                break;
            default:
        }
    }
    if (isEmpty(envParams.VVAUserName) || isEmpty(envParams.VVAPass) || isEmpty(envParams.VVABase) || isEmpty(envParams.globalDAC))
        processError("Cannot complete action.  Issue with environment settings.");

    loadUserParams();
}

function commonOnCompleteHandler() { }

function loadUserParams() {
    var curUser = globalContext.userSettings.userId;
    var curUserId = curUser.charAt(0) === "{" ? curUser.substring(1, 37) : curUser;

    Xrm.WebApi.online.retrieveRecord("systemuser", curUserId, "?$select=va_wsloginname,domainname").then(
        function success(result) {
            loadUserParamSuccess(result);
        },
        function (error) {
            //Xrm.Utility.alertDialog(error.message);
            writeError(error);
        });
}

function loadUserParamsSuccess(result) {
    if (result["va_wsloginname"] && result["va_wsloginname"].length > 0)
        userParams.PcrId = result["va_wsloginname"];
    else {
        slashPos = result["domainname"].indexOf('\\');
        userParams.PcrId = result["domainname"].substr(slashPos + 1);
    }
    executeVVA();
}

function isEmpty(str) {
    if (str && str != '')
        return false;
    else
        return true;
}