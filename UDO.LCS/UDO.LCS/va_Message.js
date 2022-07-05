﻿﻿/// <reference path="Intellisense/XrmPage-vsdoc.js" />
_VRMMESSAGE = [];

var vrmMessage = function() {
// CSDev Left Intentionally Blank 
};
vrmMessage.prototype.methodName;
vrmMessage.prototype.stackTrace;
vrmMessage.prototype.errorFlag;
vrmMessage.prototype.warningFlag;
vrmMessage.prototype.nodataFlag;
vrmMessage.prototype.description;
vrmMessage.prototype.recordCount;
vrmMessage.prototype.executionTime;
vrmMessage.prototype.callerId;
vrmMessage.prototype.stationId;
vrmMessage.prototype.clientMachine;
vrmMessage.prototype.documentElement;
vrmMessage.prototype.serviceName;
vrmMessage.prototype.friendlyServiceName;
vrmMessage.prototype.name;
vrmMessage.prototype.xmlResponse;
vrmMessage.prototype.originalResponse;
vrmMessage.prototype.xmlRequest;
vrmMessage.prototype.accessViolation;
vrmMessage.prototype.pushMessage = function() {
};
vrmMessage.prototype.logMessage = function() {
};

var webServiceMessage = function() {
};
webServiceMessage.prototype = new vrmMessage;
webServiceMessage.prototype.constructor = webServiceMessage;
webServiceMessage.prototype.pushMessage = function() {
    // CSDev Left Intentionally Blank _VRMMESSAGE.push(this);
};

var actionsMessage = function() {
};
actionsMessage.prototype = new vrmMessage;
actionsMessage.prototype.constructor = actionsMessage;
actionsMessage.prototype.pushMessage = function() {
   // CSDev Left Intentionally Blank  _VRMMESSAGE.push(this);
};

var crmMessage = function () { };
crmMessage.prototype = new vrmMessage;
crmMessage.prototype.constructor = crmMessage;
crmMessage.prototype.pushMessage = function() {
    // CSDev Left Intentionally Blank _VRMMESSAGE.push(this);
};

// Helper method
function GetErrorMessages(separator) {
// CSDev Left Intentionally Blank 
}