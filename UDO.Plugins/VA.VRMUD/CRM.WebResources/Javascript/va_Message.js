﻿﻿/// <reference path="Intellisense/XrmPage-vsdoc.js" />
_VRMMESSAGE = [];

var vrmMessage = function() {
    this.toJSONString = function() {
        var jsonString =
            '{"methodName": "' + this.methodName + '",'
                + '"errorFlag": "' + this.errorFlag + '",'
                + '"warningFlag": "' + this.warningFlag + '",'
                + '"nodataFlag": "' + this.nodataFlag + '",'
                + '"description": "' + this.description + '",'
                + '"stackTrace": "' + this.stackTrace + '",'
                + '"recordCount": "' + this.recordCount + '",'
                + '"executionTime": "' + this.executionTime + '",'
                + '"callerId": "' + this.callerId + '",'
                + '"stationId": "' + this.stationId + '",'
                + '"serviceName": "' + this.serviceName + '",'
                + '"name": "' + this.name + '",'
                + '"xmlResponse": "' + this.xmlResponse + '",'
                + '"clientMachine": "' + this.clientMachine + '"}';

        return jsonString;
    };
    this.toXMLString = function() {
        var xmlDoc;
        var xmlEl;
        var xmlText;

        xmlDoc = parseXmlObject('<' + this.documentElement + '></' + this.documentElement + '>');

        for (propertyName in this) {
            if (!(this[propertyName] instanceof Function)) {
                if (propertyName != 'documentElement') {
                    xmlEl = xmlDoc.createElement(propertyName);
                    xmlText = xmlDoc.createTextNode(this[propertyName]);
                    xmlEl.appendChild(xmlText);

                    xmlDoc = xmlDoc.getElementsByTagName(this.documentElement)[0];
                    xmlDoc.appendChild(xmlEl);
                    xmlDoc = parseXmlObject(xmlDoc.xml);
                }
            }
        }
        return xmlDoc.xml;
    }
    this.updateProgressMessage = function(tabName, sectionName) {
        var progress = _progressInterval;
        var progressTotal = _totalProgress;
        var i = (progress / (progressTotal - 1))
        var executeMessage = 'Retrieving ' +
            (this.friendlyServiceName ? this.friendlyServiceName : this.serviceName) +
            ' web service...\n'
            + 'Total Progress:  ' + Math.round(100 * i) + '% completed';

        Xrm.Page.ui.tabs.get(tabName).sections.get(sectionName).setLabel(executeMessage);

        progress++;
        _progressInterval = progress;
    };
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
    _VRMMESSAGE.push(this);
};

var actionsMessage = function() {
};
actionsMessage.prototype = new vrmMessage;
actionsMessage.prototype.constructor = actionsMessage;
actionsMessage.prototype.pushMessage = function() {
    _VRMMESSAGE.push(this);
};

var crmMessage = function () { };
crmMessage.prototype = new vrmMessage;
crmMessage.prototype.constructor = crmMessage;
crmMessage.prototype.pushMessage = function() {
    _VRMMESSAGE.push(this);
};

// Helper method
function GetErrorMessages(separator) {
    var msg = '';
    if (_VRMMESSAGE && _VRMMESSAGE.length > 0) {
        for (var i = 0; i < _VRMMESSAGE.length; i++) {
            if (_VRMMESSAGE[i].errorFlag) msg += separator + _VRMMESSAGE[i].description;
        }
    }
    return msg;
}