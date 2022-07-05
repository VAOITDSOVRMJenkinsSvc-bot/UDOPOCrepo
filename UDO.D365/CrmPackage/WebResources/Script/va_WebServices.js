"use strict";

/// <reference path="/Intellisense/XrmPage-vsdoc.js" />

//=====================================================================================================
// START WebService
//=====================================================================================================
var retrieveFormContext = function (executionContext) {
    if (executionContext && exCon.getFormContext) {
        return executionContext.getFormContext();
    }
    else {
        return null;
    }
}
var webservice = function (executionContext, context) {
    this.formContext = retrieveFormContext(executionContext);
    this.context = context;
    this.applicationName = 'CRMUD';
    this.wsMessage = new webServiceMessage();
    this.wsMessage.documentElement = 'webserviceMessage';
    this.wsMessage.stackTrace = '';
    this.executed = false;

    // Overwrite if using mapd
    this.prefix = 'q0';
    this.prefixUrl = 'http://services.share.benefits.vba.va.gov/';

    if (this.context !== undefined && this.context) {
        this.wsMessage.callerId = this.context.user.loginName;
        this.wsMessage.clientMachine = this.context.user.clientMachine;
        this.wsMessage.stationId = this.context.user.stationId;
    }

    this.buildSoapEnvelopeHeader = function () {
        return this.soapEnvelopeAndHeader = '<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" '
            + 'xmlns:' + this.prefix + '="' + this.prefixUrl + '" xmlns:xsd="http://www.w3.org/2001/XMLSchema" '
            + 'xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"><soapenv:Header>[wsseSecurity]</soapenv:Header>[soapBody]</soapenv:Envelope>';
    };
    ///*****START buildWsseSecurity()*****///
    this.buildWsseSecurity = function () {
        this.wsMessage.stackTrace += 'buildWsseSecurity();';
        if (this.context.user.userName === null) {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'User Name contains a null value';
            return null;
        }

        var wsseSecurity = '<wsse:Security '
            + 'xmlns:wsse="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd">'
            + '<wsse:UsernameToken '
            + 'xmlns:wsu="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd">'
            + '<wsse:Username>' + this.context.user.userName + '</wsse:Username>'
            + '<wsse:Password Type="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText"></wsse:Password></wsse:UsernameToken>[vaServiceHeaders]</wsse:Security>';

        return wsseSecurity;
    };
    ///*****END buildWsseSecurity()*****///

    ///*****START buildSoapBody()*****///
    this.buildSoapBody = function () {
        this.wsMessage.stackTrace += 'buildSoapBody();';

        if (_currentEnv.isPROD && this.serviceName === 'personSearch') {
            return this.buildSoapBodyInnerXml();
        }

        if (this.soapBodyInnerXml === null) {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'SOAP body does not contain the proper information';
            return null;
        }

        var soapBody = '<soapenv:Body>' + this.soapBodyInnerXml + '</soapenv:Body>';
        return soapBody;
    };
    ///*****END buildSoapBody()*****///

    ///*****START buildVAServiceHeaders()*****///
    this.buildVAServiceHeaders = function () {
        this.wsMessage.stackTrace += 'buildVAServiceHeaders();';
        if (this.context.user.clientMachine === null) {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'Client Machine contains a null value';
            return null;
        }
        if (this.context.user.stationId === null) {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'Station Id contains a null value';
            return null;
        }
        if (this.applicationName === null) {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'Application Name contains a null value';
            return null;
        }

        var vaServiceHeaders = '<vaws:VaServiceHeaders xmlns:vaws="http://vbawebservices.vba.va.gov/vawss">'
            + '<vaws:CLIENT_MACHINE>' + this.context.user.clientMachine + '</vaws:CLIENT_MACHINE>'
            + '<vaws:STN_ID>' + this.context.user.stationId + '</vaws:STN_ID>'
            + '<vaws:applicationName>' + this.context.user.applicationName + '</vaws:applicationName>' +
            (this.useExternalUid === true ?
                '<vaws:ExternalUid>' + this.context.user.loginName + '</vaws:ExternalUid>' + '<vaws:ExternalKey>' + this.context.user.loginName + '</vaws:ExternalKey>' :
                '') +
            '</vaws:VaServiceHeaders>';

        return vaServiceHeaders;
    };
    ///*****END buildVAServiceHeaders()*****///

    ///*****START buildSoapEnvelope()*****///
    this.buildSoapEnvelope = function () {
        this.wsMessage.stackTrace += 'buildSoapEnvelope();';
        this.soapBodyInnerXml = this.buildSoapBodyInnerXml();
        if (this.soapBodyInnerXml === null) {
            if (!this.wsMessage.errorFlag) {
                this.wsMessage.errorFlag = true;
                this.wsMessage.description = 'SOAP Body Inner XML contains a null value';
            }
            return null;
        }

        var soapEnvelope = this.buildSoapEnvelopeHeader();

        var wsseSecurity = this.buildWsseSecurity();
        var vaServiceHeaders = this.buildVAServiceHeaders();
        var soapBody = this.buildSoapBody();

        if (wsseSecurity === null || vaServiceHeaders === null || soapBody === null) {
            return null;
        }

        soapEnvelope = soapEnvelope.replace('[wsseSecurity]', wsseSecurity);
        soapEnvelope = soapEnvelope.replace('[vaServiceHeaders]', vaServiceHeaders);
        soapEnvelope = soapEnvelope.replace('[soapBody]', soapBody);

        return soapEnvelope;
    };
    ///*****END buildSoapEnvelope()*****///

    ///*****START executeSoapRequest()*****///
    this.executeSoapRequest = function () {

        // Validate that we have a target URL for our request
        this.wsMessage.stackTrace += 'executeSoapRequest();';
        if (this.webserviceRequestUrl === null) {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'Webservice Request URL contains a null value';
            this.wsMessage.pushMessage();
            return null;
        }

        // Set variables 
        var request = null;
        var preRequestTimeStamp;
        var postRequestTimeStamp;
        var execTime;
        var errorFlag = false;
        var description;
        var origmessage;
        var env;

        // Build SOAP Body
        if (_currentEnv.isPROD && this.serviceName === 'personSearch') {
            env = this.buildSoapBody();
        } else {
            env = this.buildSoapEnvelope();
        }

        if (env === null) {
            return null;
        }

        this.wsMessage.xmlRequest = env;

        try {
            request = new XMLHttpRequest();

        } catch (err) {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'Failed to create XML HTTP Object.  Failed to execute web service request';
            this.wsMessage.pushMessage();
            return null;
        }

        if ((request === null) && window.XMLHttpRequest) {
            request = new XMLHttpRequest();
        } else if (request === null) {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'Failed to create XML HTTP Object.  Failed to execute web service request';
            this.wsMessage.pushMessage();
            return null;
        }

        var usingDAC = false;
        var resp = null;
        var serviceUri = this.webserviceRequestUrl;

        // Adjust service URL and SOAP envelope if we are using DAC
        if (_currentEnv.globalDAC && _currentEnv.globalDAC.length > 0 &&
            (this.ignoreGlobalDACUrl === undefined || this.ignoreGlobalDACUrl !== true)) {
            usingDAC = true;
            serviceUri = _currentEnv.globalDAC;
        }
        if (this.serviceDACUrl && this.serviceDACUrl.length > 0) {
            usingDAC = true;
            serviceUri = this.serviceDACUrl;
        }
        if (usingDAC) {
            env = this.createProxyServiceSoapHeader(this.webserviceRequestUrl, env);
        }

        //Retrieve Subscription Keys
        var subscriptionKey;
        var subscriptionKeyE;
        var subscriptionKeyS;

        // Make API request to retrieve APIM keys
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

        // Create request to Redirect Service
        request.open('POST', serviceUri, false);
        request.setRequestHeader('Ocp-Apim-Subscription-Key', subscriptionKey);
        request.setRequestHeader('Ocp-Apim-Subscription-Key-E', subscriptionKeyE);
        request.setRequestHeader('Ocp-Apim-Subscription-Key-S', subscriptionKeyS);
        request.setRequestHeader('Content-Type', 'text/xml; charset=UTF-8');
        request.setRequestHeader("Accept", "application/xml, text/xml, */*");

        preRequestTimeStamp = new Date().getTime();

        try {
            // Parameters
            var parameters = {};
            parameters.SoapRequest = env;
            parameters.RequestURL = serviceUri; // "https://dev.integration.d365.va.gov/veis/Redirect/"; // Edm.String

            // Create request to Redirect Service
            var request = new XMLHttpRequest();
            request.open("POST", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.2/udo_RedirectSvc", false);
            request.setRequestHeader("OData-MaxVersion", "4.0");
            request.setRequestHeader("OData-Version", "4.0");
            request.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            request.setRequestHeader("Accept", "application/json");
            request.onreadystatechange = function () {
                if (this.readyState === 4) {
                    request.onreadystatechange = null;
                    if (this.status === 200 || this.status === 204) {
                        var result = JSON.parse(this.response);

                        // Return Type: mscrm.udo_RedirectSvcResponse
                        // Output Parameters
                        var timeout = result["Timeout"]; // Edm.Boolean
                        var exception = result["Exception"]; // Edm.Boolean
                        var dataissue = result["DataIssue"]; // Edm.Boolean
                        var responsemessage = result["ResponseMessage"]; // Edm.String

                        // If we had an exception occur during custom action execution, throw an error
                        if (exception === true || exception === "true") {
                            this.wsMessage.errorFlag = true;
                            this.wsMessage.description = responsemessage;
                            this.wsMessage.pushMessage();

                            return responsemessage;
                        }

                        // Calculate execution time
                        postRequestTimeStamp = new Date().getTime();
                        execTime = (postRequestTimeStamp - preRequestTimeStamp);
                        var totalExecutionTime = _totalWebServiceExecutionTime;
                        totalExecutionTime = totalExecutionTime + execTime;
                        _totalWebServiceExecutionTime = totalExecutionTime;

                        // Set response variable
                        resp = responsemessage;

                        // Parse XML if we are using DAC
                        if (usingDAC) {
                            var parser = new DOMParser();
                            var rx = parser.parseFromString(responsemessage, "text/xml");
                            var x = rx.getElementsByTagName("ExecuteResult")[0];
                            var y = x.childNodes[0];
                            var resNode = y.nodeValue;

                            if (!resNode) {
                                var msg = "No response from DAC";
                                var title = "Web Service Call Error";
                                UDO.Shared.openAlertDialog(msg, title, null, null);
                            } else {
                                resp = resNode;
                            }
                        }

                        // If response is blank, we need to set the error flag to true
                        if (resp === null || resp === '' || resp === undefined) {
                            errorFlag = true;
                            description = 'Did not receive data from web service request';
                        }

                        origmessage = responsemessage;

                    } else {
                        errorFlag = true;
                        description = 'Failed to send web service request. Web Service may be unavailable at the time of execution.' +
                            (senderr ? '\nError: ' + senderr.message : '') + '\n';

                        resp = responsemessage;
                    }
                }
            };

            // Finally send request to Redirect Service
            request.send(JSON.stringify(parameters));

        } catch (senderr) {
            this.wsMessage.errorFlag = true;
            this.wsMessage.description = 'Failed to send web service request. Web Service may be unavailable at the time of execution.' +
                (senderr ? '\nError: ' + senderr.message : '') + '\n';
            this.wsMessage.pushMessage();
            return responsemessage;
        }

        this.wsMessage.executionTime = execTime;
        this.wsMessage.originalResponse = origmessage;

        if (errorFlag) {
            this.wsMessage.errorFlag = errorFlag;
            this.wsMessage.description = description;
            this.wsMessage.pushMessage();
        }

        return resp;
    };
    ///*****END executeSoapRequest()*****///

    ///*****START executeRequest()*****///
    this.executeRequest = function () {
        this.wsMessage.stackTrace += 'executeRequest();';

        var executeMessage = (this.wsMessage.friendlyServiceName ? ("Retrieving " + this.wsMessage.friendlyServiceName) : ('Executing ' + this.serviceName));

        if (this.suppressProgressDlg !== true) {
            ShowProgress(executeMessage);
        }

        this.responseXml = this.executeSoapRequest();
        this.webServiceRequestSuccess();

        return !this.wsMessage.errorFlag;
    };
    ///*****END executeRequest()*****///

    this.webServiceRequestSuccess = function () {
        var responseXmlObject;
        var returnCode;
        var returnMessage;
        if (this.responseXml !== null && this.responseXml !== '' && this.responseXml !== undefined) {
            // responseXmlObject = parseXmlObject(this.responseXml);
            var parser = new DOMParser();
            var responseXmlObject = parser.parseFromString(this.responseXml, "text/xml");

            var hasTitleWithError = false;
            this.wsMessage.accessViolation = false;
            if (responseXmlObject.getElementsByTagName('title').length > 0 || responseXmlObject.getElementsByTagName('TITLE').length > 0) {
                var txt = '';
                var y;

                var x = responseXmlObject.getElementsByTagName("title")[0];
                if (x > 0) {//title
                    y = x.childNodes[0];
                    txt = y.nodeValue;
                }

                x = responseXmlObject.getElementsByTagName("TITLE")[0];
                //TITLE 
                if (x > 0) { //TITLE
                    y = x.childNodes[0];
                    txt = y.nodeValue;
                }

                //if (responseXmlObject.selectSingleNode('//title'))
                //    txt = responseXmlObject.selectSingleNode('//title').text;
                //else
                //    txt = responseXmlObject.selectSingleNode('//TITLE').text;

                if (txt && (txt.toUpperCase() === '404 NOT FOUND' || (txt.length >= 5 && txt.substring(0, 5).toUpperCase() === 'ERROR'))) {
                    hasTitleWithError = true;
                    returnMessage = txt;
                    returnCode = 'ERROR';
                    if (txt.toUpperCase() === '404 NOT FOUND') {
                        returnCode = '404';
                    }
                }
            }

            var faultNodes = ['S:Fault', 'ns2:Fault', 'env:Fault', 'soap:Fault'];
            var hasFault = false;
            for (var i = 0; i < faultNodes.length; i++) {
                if (responseXmlObject.getElementsByTagName(faultNodes[i]) && responseXmlObject.getElementsByTagName(faultNodes[i]).length > 0) {
                    hasFault = true;
                    break;
                }
            }

            if (hasFault) {
                this.wsMessage.errorFlag = true;
                returnCode = '';
                var x;
                var y;
                x = responseXmlObject.getElementsByTagName("faultCode")[0];
                y = x.childNodes[0];
                returnCode = y.nodeValue;

                //if (responseXmlObject.selectSingleNode('//faultCode'))
                //    returnCode = responseXmlObject.selectSingleNode('//faultCode').text;
                //else {
                //    if (responseXmlObject.selectSingleNode('//faultcode'))
                //        returnCode = responseXmlObject.selectSingleNode('//faultcode').text;
                //}

                returnMessage = '';
                x = responseXmlObject.getElementsByTagName("ns2:exception/message")[0];
                y = x.childNodes[0];
                var msg2 = y.nodeValue;

                //var msg2 = (responseXmlObject.selectSingleNode('//ns2:exception/message') &&
                //    responseXmlObject.selectSingleNode('//ns2:exception/message').text ?
                //    ' (' + responseXmlObject.selectSingleNode('//ns2:exception/message').text + ')' : '');

                x = responseXmlObject.getElementsByTagName("ns2:ShareException/message")[0];
                if (x > 0) {
                    y = x.childNodes[0];
                    returnMessage = y.nodeValue;
                }
                else {
                    x = responseXmlObject.getElementsByTagName("faultstring")[0];
                    if (x > 0) {
                        y = x.childNodes[0];
                        returnMessage = y.nodeValue;
                    }
                }

                //if (responseXmlObject.selectSingleNode('//ns2:ShareException/message')) {
                //    returnMessage = responseXmlObject.selectSingleNode('//ns2:ShareException/message').text;
                //} else {
                //    if (responseXmlObject.selectSingleNode('//faultstring'))
                //        returnMessage = responseXmlObject.selectSingleNode('//faultstring').text;
                //}
                // check if the service wants to ignore this message
                if (this.ignoredExceptionMessage && this.ignoredExceptionMessage === returnMessage) this.wsMessage.errorFlag = false;

                returnMessage += msg2;
            } else if (hasTitleWithError) {
                this.wsMessage.errorFlag = true;

            } else if (responseXmlObject.getElementsByTagName("RETURN_CODE")[0] && responseXmlObject.getElementsByTagName("RETURN_CODE")[0].childNodes[0] &&
                responseXmlObject.getElementsByTagName("RETURN_CODE")[0].childNodes[0].indexOf('SENS100') !== -1) {
                this.wsMessage.errorFlag = true;
                this.wsMessage.accessViolation = true;
                var x;
                var y;
                x = responseXmlObject.getElementsByTagName("RETURN_CODE")[0];
                y = x.childNodes[0];
                returnCode = y.nodeValue;

                //returnCode = responseXmlObject.selectSingleNode('//RETURN_CODE').text;
                x = responseXmlObject.getElementsByTagName("RETURN_MESSAGE")[0];
                if (x > 0) {
                    y = x.childNodes[0];
                    returnMessage = y.nodeValue;
                }
                else {
                    returnMessage = ""
                }
                // returnMessage = responseXmlObject.selectSingleNode('//RETURN_MESSAGE') !== null ? responseXmlObject.selectSingleNode('//RETURN_MESSAGE').text : '';

                //} else if (SingleNodeExists(responseXmlObject, '//displayMessage') && SingleNodeExists(responseXmlObject, '//errorCode')) {
                //    this.wsMessage.errorFlag = true;
                //    returnCode = responseXmlObject.selectSingleNode('//errorCode').text;
                //    returnMessage = responseXmlObject.selectSingleNode('//displayMessage').text;
            } else if ((responseXmlObject.getElementsByTagName("displayMessage")[0]) && (responseXmlObject.getElementsByTagName("errorCode")[0]) > 0) {
                this.wsMessage.errorFlag = true;
                var x;
                var y;
                x = responseXmlObject.getElementsByTagName("errorCode")[0];
                y = x.childNodes[0];
                returnCode = y.nodeValue;

                //returnCode = responseXmlObject.selectSingleNode('//errorCode').text;
                x = responseXmlObject.getElementsByTagName("displayMessage")[0];
                y = x.childNodes[0];
                returnMessage = y.nodeValue;
                //returnMessage = responseXmlObject.selectSingleNode('//displayMessage').text;
            } else {
                this.wsMessage.errorFlag = false;
                var x;
                var y;
                x = responseXmlObject.getElementsByTagName("returnCode")[0];
                if (x !== null && x !== undefined) {
                    y = x.childNodes[0];
                    returnCode = y.nodeValue;
                }
                else {
                    returnCode = "";
                }
                //returnCode = responseXmlObject.selectSingleNode('//returnCode') !== null ?
                //    responseXmlObject.selectSingleNode('//returnCode').text : '';
                x = responseXmlObject.getElementsByTagName("returnMessage")[0];
                if (x !== null && x !== undefined) {
                    y = x.childNodes[0];
                    returnMessage = y.nodeValue;
                }
                else {
                    returnMessage = "";
                }

                //returnMessage = responseXmlObject.selectSingleNode('//returnMessage') !== null ?
                //    responseXmlObject.selectSingleNode('//returnMessage').text : '';
            }

            this.wsMessage.description = 'Return Code:  ' + returnCode + '; Return Message:  ' + returnMessage;

            if (this.responseFieldSchema &&
                (this.wsMessage.errorFlag !== true || this.wsMessage.accessViolation === true)
                && this.formContext.getAttribute(this.responseFieldSchema)) {
                this.formContext.getAttribute(this.responseFieldSchema).setValue(this.responseXml);
            }

            if (this.responseTimestamp && this.formContext.getAttribute(this.responseTimestamp)) {
                this.formContext.getAttribute(this.responseTimestamp).setValue(new Date());
            }

            // result is not XML 
            //if (!responseXmlObject.xml || responseXmlObject.xml.length === 0) {
            //    returnMessage = this.responseXml;
            //    returnCode = 'ERROR-Unexpected Response';
            //    this.wsMessage.errorFlag = true;
            //    this.wsMessage.description = this.responseXml;
            //}
        }

        //**********************
        // Exceptions:
        // 1. old payment service response
        if (this.wsMessage.description && this.wsMessage.description.indexOf('BDN Transaction Failed') >= 0) {
            this.wsMessage.errorFlag = false;
        }
        //**********************

        this.wsMessage.xmlResponse = this.wsMessage.originalResponse;
        this.wsMessage.pushMessage();
    };

    this.initializeSearchParameters = function () {
        //Initialize search parameters
        if (this.context && this.context.parameters && this.requiredSearchParameters) {
            for (p in this.context.parameters) {
                if (this.context.parameters[p] && this.context.parameters[p] !== undefined && this.context.parameters[p] !== '') {
                    this.requiredSearchParameters[p] = this.context.parameters[p];
                }
            }
        }
    };

    this.requiredParametersMissing = function () {
        var missingParameters = null;
        for (p in this.requiredSearchParameters) {
            if (this.requiredSearchParameters[p] === null) {
                if (missingParameters === null) {
                    missingParameters = new Array();
                }

                missingParameters[p] = p;

            }
        }

        return missingParameters;
    };

    this.webServiceRequestError = function () {
    };

    this.createProxyServiceSoapHeader = function (address, payload) {
        return '<?xml version="1.0" encoding="utf-8"?><soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">' +
            '<soap12:Body><Execute xmlns="http://tempuri.org/">' +
            '<address>' + address + '</address>' +
            '<value><payload><![CDATA[' + payload + ']]></payload></value></Execute></soap12:Body></soap12:Envelope>';
    };

    this.testHeader = function () {
        return '<?xml version="1.0" encoding="utf-8"?><soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">' +
            //'<soapenv:Header><wsse:Security xmlns:wsse="http://schemas.xmlsoap.org/ws/2002/12/secext"><wsse:UsernameToken><wsse:Username>Bob</wsse:Username>' +
            //'<wsse:Password>YourStr0ngPassWord</wsse:Password></wsse:UsernameToken></wsse:Security></soapenv:Header>' +
            '<soap12:Body><Test xmlns="http://tempuri.org/"></Test></soap12:Body></soap12:Envelope>';
    };
};

webservice.prototype.request;
webservice.prototype.webserviceRequestUrl;
webservice.prototype.soapBodyInnerXml;
webservice.prototype.responseXml;
webservice.prototype.responseFieldSchema;
webservice.prototype.responseTimestamp;
webservice.prototype.serviceName;
webservice.prototype.friendlyServiceName;
webservice.prototype.prefix;
webservice.prototype.prefixUrl;
webservice.prototype.soapEnvelopeAndHeader;
webservice.prototype.preRequestTimeStamp;
webservice.prototype.postRequestTimeStamp;
webservice.prototype.requiredSearchParameters;
webservice.prototype.executed;
webservice.prototype.dataSourceForKey;
webservice.prototype.ignoredExceptionMessage;
webservice.prototype.ignoreRequiredParMissingWarning;
//webservice.prototype.serviceDACUrl; //needs to be defined on each ws

webservice.prototype.useExternalUid;
webservice.prototype.buildSoapBodyInnerXml = function () {
};
webservice.prototype.executeRequest = function () {
};
// END WebService
//=====================================================================================================
