/// <reference path="/Intellisense/XrmPage-vsdoc.js" />
//=====================================================================================================
// START Action
//=====================================================================================================
var action = function (context) {
    //this.context = context;
    //this.actionMessage = new actionsMessage();
    //this.actionMessage.documentElement = 'actionsMessage';
    //this.actionMessage.stackTrace = '';
    //this.context.updateProgress = new Boolean(true);
    this.webservices;
    this.analyzers;
    this.hasErrors;
}
//action.prototype.performAction = function () { }

action.prototype.analyzeResults = function () {
    
}
action.prototype.getMessages = function () {}
// END Action
//=====================================================================================================




var shareGetRegionalOffices = function (context) {
    this.context = context;
}
shareGetRegionalOffices.prototype = new action;
shareGetRegionalOffices.prototype.constructor = shareGetRegionalOffices;
shareGetRegionalOffices.prototype.performAction = function () {
    var getRegionalOffices = new findRegionalOffices(this.context);
    getRegionalOffices.responseFieldSchema = 'va_roxml';
    getRegionalOffices.responseTimestamp = 'va_wstimestamp';
    getRegionalOffices.executeRequest();
}