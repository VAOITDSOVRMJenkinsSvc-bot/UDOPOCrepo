/*
    // Call an Action
    Process.callAction("udo_Retrieve",
    [{
        key: "Target",
        type: Process.Type.EntityReference,
        value: new Process.EntityReference("account", Xrm.Page.data.entity.getId())
    },
    {
        key: "ColumnSet",
        type: Process.Type.String,
        value: "name, statuscode"
    }],
    function (params) {
        // Success
        alert("Name = " + params["Entity"].get("name") + "\n" +
                "Status = " + params["Entity"].formattedValues["statuscode"]);
    },
    function (e, t) {
        // Error
        alert(e);

        // Write the trace log to the dev console
        if (window.console && console.error) {
            console.error(e + "\n" + t);
        }
    });

    // Call a Workflow
    Process.callWorkflow("4AB26754-3F2F-4B1D-9EC7-F8932331567A", Xrm.Page.data.entity.getId(),
        function () {
            alert("Workflow executed successfully");
        },
        function () {
            alert("Error executing workflow");
        });

    // Call a Dialog
    Process.callDialog("C50B3473-F346-429F-8AC7-17CCB1CA45BC", "contact", Xrm.Page.data.entity.getId(), 
        function () { 
            Xrm.Page.data.refresh(); 
        });
*/

var Process = Process || {};

// Supported Action input parameter types
Process.Type = {
    Bool: "c:boolean",
    Float: "c:double", // Not a typo
    Decimal: "c:decimal",
    Int: "c:int",
    String: "c:string",
    DateTime: "c:dateTime",
    Guid: "c:guid",
    EntityReference: "a:EntityReference",
    OptionSet: "a:OptionSetValue",
    Money: "a:Money",
    Entity: "a:Entity",
    EntityCollection: "a:EntityCollection"
}

// inputParams: Array of parameters to pass to the Action. Each param object should contain key, value, and type.
// successCallback: Function accepting 1 argument, which is an array of output params. Access values like: params["key"]
// errorCallback: Function accepting 1 argument, which is the string error message. Can be null.
// Unless the Action is global, you must specify a 'Target' input parameter as EntityReference
// actionName is required
Process.callAction = function (actionName, inputParams, successCallback, errorCallback, url) {
 // CSDev Left Intentionally Blank 
   
}

// Runs the specified workflow for a particular record
// successCallback and errorCallback accept no arguments
// workflowId, and recordId are required
Process.callWorkflow = function (workflowId, recordId, successCallback, errorCallback, url) {
 // CSDev Left Intentionally Blank 
}

// Pops open the specified dialog process for a particular record
// dialogId, entityName, and recordId are required
// callback fires even if the dialog is closed/cancelled
Process.callDialog = function (dialogId, entityName, recordId, callback, url) {
  // CSDev Left Intentionally Blank 
}

Process._emptyGuid = "00000000-0000-0000-0000-000000000000";

// This can be used to execute custom requests if needed - useful for me testing the SOAP :)
Process._callActionBase = function (requestXml, successCallback, errorCallback, url) {
// CSDev Left Intentionally Blank 
}

// Get only the immediate child nodes for a specific tag, otherwise entitycollections etc mess it up
Process._getChildNodes = function (node, childNodesName) {
// CSDev Left Intentionally Blank 
}

// Get a single child node for a specific tag
Process._getChildNode = function (node, childNodeName) {
// CSDev Left Intentionally Blank 
}

// Gets the first not null value from a collection of nodes
Process._getNodeTextValueNotNull = function (nodes) {
// CSDev Left Intentionally Blank 
}

// Gets the string value of the XML node
Process._getNodeTextValue = function (node) {
// CSDev Left Intentionally Blank 
}

// Gets the value of a parameter based on its type, can be recursive for entities
Process._getValue = function (node) {
 // CSDev Left Intentionally Blank 
}

Process._getEntityData = function (entityNode) {
// CSDev Left Intentionally Blank 
}

Process._getXmlValue = function (key, dataType, value) {
 // CSDev Left Intentionally Blank 
}

Process._getXmlEntityData = function (entity) {
// CSDev Left Intentionally Blank 
}

Process._htmlEncode = function (s) {
// CSDev Left Intentionally Blank 
}

Process.Entity = function (logicalName, id, attributes) {
// CSDev Left Intentionally Blank 
}

// Gets the value of the attribute without having to check null
Process.Entity.prototype.get = function (key) {
// CSDev Left Intentionally Blank 
}

Process.EntityReference = function (entityType, id, name) {
// CSDev Left Intentionally Blank 
}

Process.Attribute = function (value, type) {
// CSDev Left Intentionally Blank 
}
