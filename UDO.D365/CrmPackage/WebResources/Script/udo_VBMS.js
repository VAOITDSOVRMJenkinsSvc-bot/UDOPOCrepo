"use strict";

var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
Va.Udo.Crm.Scripts.VBMS = Va.Udo.Crm.Scripts.VBMS || {};
var globalContext = Xrm.Utility.getGlobalContext();
var version = globalContext.getVersion();
//var webApi;
//var Util;
var Sec;
var exCon;
var formContext;
var lib;
var UDO = UDO || {};
var _EntityName;

Va.Udo.Crm.Scripts.VBMS = {
    _HasVBMSPermission: false,
    _EntityName: "",

    OnLoad: function (execCon) {
        var propertiesAppInsights = {
            "method": "Va.Udo.Crm.Scripts.VBMS.OnLoad", "description": "Called on load of UDO VBMS Document (VBMS Upload) form"
        };
        exCon = execCon;
        Va.Udo.Crm.Scripts.VBMS.InstantiateCommonScripts(exCon);
    },
    InstantiateCommonScripts: function (exCon) {
        lib = new CrmCommonJS.CrmCommon(version, exCon);
        //webApi = lib.WebApi;
        //Util = lib.Util;
        Sec = lib.Security;
        formContext = exCon.getFormContext();

    },
    Initialize: function () {
        if (lib === null || typeof lib === "undefined") {
            if (exCon === null || typeof exCon === "undefined") {
                exCon = fetchContext();
            }
            Va.Udo.Crm.Scripts.VBMS.InstantiateCommonScripts(exCon);
        }
        function getEntityTypeCode(entityName) {
            try {
                var command = new RemoteCommand("LookupService", "RetrieveTypeCode");
                command.SetParameter("entityName", entityName);
                var result = command.Execute();
                if (result.Success && typeof result.ReturnValue === "number") {
                    return result.ReturnValue;
                }
                else {
                    return null;
                }
            }
            catch (ex) {
                return null;
            }
        }

        if (Sec.UserHasRole("VBMS User")) {
            Va.Udo.Crm.Scripts.VBMS._HasVBMSPermission = true;
        }
        else if (Sec.UserHasRole("VBMS Administrator")) {
            Va.Udo.Crm.Scripts.VBMS._HasVBMSPermission = true;
        }
        else if (Sec.UserHasRole("System Administrator")) {
            Va.Udo.Crm.Scripts.VBMS._HasVBMSPermission = true;
        }

        Va.Udo.Crm.Scripts.VBMS.Document.TypeCode = getEntityTypeCode("udo_vbmsdocument");
    },
    Document: {
        TypeCode: null,
        VBMSDocId: null,
        Create: function (filenumber, claimnumber, firstname, middlename, lastname, role, entityId, entityName, doctype) {

            if (entityName !== "udo_servicerequest" && entityName !== "udo_lettergeneration" && entityName !== "udo_vbmsdocument") return;

            _EntityName = entityName;

            function isNullOrEmpty(str) {
                if (str instanceof Array) {
                    for (var i = 0; i < str.length; i++) {
                        if (!isNullOrEmpty(str[i])) return false;
                    }
                    return true;
                }
                if (typeof str === 'undefined' || str === null) return true;
                return str.length < 1;
            }

            var errors = "";
            if (entityId === null) {
                if (isNullOrEmpty([filenumber, claimnumber, firstname, middlename, lastname])) {
                    errors += (errors.length > 0 ? "\r\n" : "") +
                        "Either a service request or the detailed information (filenumber,claimnumber,firstname,middlename,lastname) must be provided.";
                }
                if (isNullOrEmpty(filenumber))
                    errors += (errors.length > 0 ? "\r\n" : "") +
                        "File Number missing. Please enter a File Number.";
                if (isNullOrEmpty(firstname))
                    errors += (errors.length > 0 ? "\r\n" : "") +
                        "First Name missing. Please enter the First Name.";
                if (isNullOrEmpty(lastname))
                    errors += (errors.length > 0 ? "\r\n" : "") +
                        "Last Name missing. Please enter the Last Name.";
            }

            if (errors.length > 0) throw errors;


            if (entityName === "udo_vbmsdocument") {

                Va.Udo.Crm.Scripts.VBMS.Document.VBMSDocId = entityId;
                Va.Udo.Crm.Scripts.VBMS.Uploader.Open("ctrludovbmsuploader");
            }
            else {
                var vbmsDoc = { udo_name: "VBMS Document" };

                if (!isNullOrEmpty(entityId) && entityName === "udo_servicerequest")
                    vbmsDoc["udo_servicerequestid@odata.bind"] = "/udo_servicerequests(" + entityId.replace("{", "").replace("}", "") + ")";
                if (!isNullOrEmpty(entityId) && entityName === "udo_lettergeneration")
                    vbmsDoc["udo_lettergenerationid@odata.bind"] = "/udo_lettergenerations(" + entityId.replace("{", "").replace("}", "") + ")";
                if (!isNullOrEmpty(filenumber))
                    vbmsDoc.udo_filenumber = filenumber;
                if (!isNullOrEmpty(claimnumber))
                    vbmsDoc.udo_claimnumber = claimnumber;
                if (!isNullOrEmpty(firstname))
                    vbmsDoc.udo_firstname = firstname;
                if (!isNullOrEmpty(middlename))
                    vbmsDoc.udo_middlename = middlename;
                if (!isNullOrEmpty(lastname))
                    vbmsDoc.udo_lastname = lastname;
                if (!isNullOrEmpty(role))
                    vbmsDoc.udo_vbmsuploadrole = role;
                if (!isNullOrEmpty(doctype)) {
                    if (doctype[0] !== null) {
                        vbmsDoc["udo_vbmsdocumenttype@odata.bind"] = "/" + doctype[0].entityType + "s(" + doctype[0].id.replace("{", "").replace("}", "") + ")";
                    }
                }

                Xrm.WebApi.createRecord("udo_vbmsdocument", vbmsDoc)
                    .then(function (data) {
                        Va.Udo.Crm.Scripts.VBMS.Document.CreateDocumentComplete(data);
                    })
                    .catch(function (err) {
                        Va.Udo.Crm.Scripts.VBMS.Uploader.Error("VBMS Document failed to create.");
                    });
            }

        },
        CreateDocumentComplete: function (req) {

            if (req === null && req.id === null) return;

            Va.Udo.Crm.Scripts.VBMS.Document.VBMSDocId = req.id;
            Va.Udo.Crm.Scripts.VBMS.Uploader.Open("ctrludovbmsuploader");
        },
        Delete: function (docid) {

            Xrm.WebApi.deleteRecord("udo_vbmsdocument", docid)
                .then(function (data) {

                })
                .catch(function errorHandler(error) {
                    Va.Udo.Crm.Scripts.VBMS.Uploader.Error("VBMS Document failed to delete - " + err.description);
                });

        }
    },
    Note: {
        VBMSNoteId: null,
        Create: function (vbmsDocumentId) {

            if (vbmsDocumentId !== null) {

                var note = {};
                note.notetext = "VBMS Document";
                note.objectid = { Id: vbmsDocumentId.toString(), LogicalName: "udo_vbmsdocument" };
                note.objecttypecode = "udo_vbmsdocument";
                Xrm.WebApi.createRecord("annotation", note)
                    .then(function (data) {
                        CreateNoteComplete(data);
                    })
                    .catch(function (err) {
                        Va.Udo.Crm.Scripts.VBMS.Uploader.Error("Annotation failed to create - " + err.description);
                    });
            }
        },
        CreateNoteComplete: function (data) {

            function PopupWindow(url, w, h) {
                var left = (screen.width / 2) - (w / 2);
                var top = (screen.height / 2) - (h / 2);
                return window.open(url, "", "width=" + w + ", height=" + h + ", top=" + top + ", left=" + left + ", toolbar=no");
            }

            if (data !== null && data.id !== null) {
                Va.Udo.Crm.Scripts.VBMS.Note.VBMSNoteId = data.id;
            }
        },
        FindNote: function (docid) {
            return new Promise(function (resolve, reject) {
                docid = docid.replace("{", "").replace("}", "");
                var columns = ["name", "annotationid"]; 
                filter = "?$select=name,annotationid&$filter=objectid eq " + docid + "";
                Xrm.WebApi.retrieveMultipleRecords("annotation", filter)
                    .then(function () {
                        loadNoteComplete(data.value);
                        return resolve(true);
                    },
                        function (error) {
                            Va.Udo.Crm.Scripts.VBMS.Uploader.Error(error.message);
                            return reject(error);
                        });
            });
        },
        loadNoteComplete: function (data) {

            var result;
            for (var i = 0; i < data.length; i++) {
                var note = data[i];
                Va.Udo.Crm.Scripts.VBMS.Note.VBMSNoteId = note.annotationid;
                result = note.annotationid;
            }

            if (data.length === 0) {
                Va.Udo.Crm.Scripts.VBMS.Uploader.Error("Unexpected error - Could not find uploaded Note.");
            }
        }
    },

    OpenVBMSDialog: function (context, filenumber, claimnumber, firstname, middlename, lastname, role, doctype) {

        var requiredAttributesWithNullValues = [];
        formContext.data.entity.attributes.forEach(function (attribute, index) {

            if (attribute.getValue() === null && attribute.getRequiredLevel() === "required") {

                requiredAttributesWithNullValues.push(attribute);

            }

        });

        if (requiredAttributesWithNullValues.length > 0) {

            var message = "The following fields are required:\n";

            for (var i = 0; i < requiredAttributesWithNullValues.length; i++) {

                message += requiredAttributesWithNullValues[i].controls.get(0).getLabel() + "\n";

            }

            Xrm.Navigation.openAlertDialog({ text: message });

        } else {
            formContext.data.save(true).then(function () { successSave(filenumber); }, errorSave);
        }

        function successSave(filenumber) {

            Va.Udo.Crm.Scripts.VBMS.Initialize();

            if (!Va.Udo.Crm.Scripts.VBMS._HasVBMSPermission) {
                Va.Udo.Crm.Scripts.VBMS.Uploader.Error("You must be granted VBMS Permission to continue to upload a document");
                return;
            }

            var ssn = "";
            if (!filenumber)
                ssn = formContext.getAttribute("udo_filenumber").getValue();
            else
                ssn = param.data.filenumber;

            if (!role) {
                if (formContext.getAttribute("udo_vbmsuploadrole")) {
                    role = formContext.getAttribute("udo_vbmsuploadrole").getValue();
                }
            }

            if (!doctype) {
                if (formContext.getAttribute("udo_vbmsdoctype")) {
                    doctype = formContext.getAttribute("udo_vbmsdoctype").getValue();
                }
            }
            if (ssn !== null) {     
                var entityId;
                var message = "Did you intend to upload a document \n for xxx - xx -" + ssn.substr(ssn.length - 4) + "?";
                UDO.Shared.openConfirmDialog(message, "Upload Document?", 200, 400, "Yes", "No")
                    .then(function (success) {
                        entityId = formContext.data.entity.getId();
                        if (success.confirmed) {
                            var entityName = formContext.data.entity.getEntityName().toLowerCase();

                            Va.Udo.Crm.Scripts.VBMS.Document.Create(filenumber, claimnumber, firstname, middlename, lastname, role, entityId, entityName, doctype);
                        }
                        else {
                            return;
                        }
                    },
                        function (error) {
                            var msg = { message: "Error uploading document " + error.message };
                            formContext.ui.setFormNotification(msg, "ERROR", "VBMSUPLOADSTATUS");

                            Xrm.Navigation.openErrorDialog(msg);
                        }
                    );
            } else {
                Va.Udo.Crm.Scripts.VBMS.Uploader.Error("Please enter File/Claim number then save.");
            }
        }

        function errorSave(errorCode, errorMessage) {
            var message = "errorSave";
            var alertStrings = { text: message };
            var alertOptions = { height: 120, width: 260 };
            Xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
        }
    },
    Uploader: {
        Open: function (id) {

            var params = encodeURIComponent("docid=" + Va.Udo.Crm.Scripts.VBMS.Document.VBMSDocId);
            var url = globalContext.getClientUrl() + "/WebResources/udo_uploadVBMS?data=" + params;


            var wrControl = formContext.getControl("WebResource_uploadVBMS");
            if (wrControl) {
                wrControl.getContentWindow().then(function (contentWindow) {
                    contentWindow.setContext(formContext);
                })
            }

            var dialogTab = formContext.ui.tabs.get("{040d30d2-de4c-438a-9c84-7576f6b74327}");
            var dialogCtrl = formContext.getControl("WebResource_uploadVBMS");
            dialogCtrl.setSrc(url);
            dialogTab.setVisible(true);
            dialogCtrl.setVisible(true);
            dialogTab.setFocus();
        },

        Error: function (message) {

            var alertStrings = { text: message };
            var alertOptions = { height: 120, width: 260 };
            Xrm.Navigation.openAlertDialog(alertStrings, alertOptions);

            formContext.ui.setFormNotification(message, "ERROR", "VBMSUPLOADSTATUS");
        }
    }
};

function UDOclosePage() {
    var requiredFields = false;
    if (formContext.getAttribute("udo_vbmsdocumenttype").getValue() === null) {
        requiredFields = true;
    }

    if (formContext.data.entity.getIsDirty() || requiredFields === true) {
        if (UDOCheckMandatoryFields()) {
            formContext.data.save();
            window.open("http://close/");
        } else {
            return new Promise(function (resolve, reject) {
                UDO.Shared.openConfirmDialog("Are you sure you want to close this form ?", "", 200, 450, "OK", "Cancel").then(
                    function (response) {
                        if (response.confirmed) {
                            UDOSetRequiredLevelOnAllRequiredFields();
                            formContext.data.save();
                            window.open("http://close/");
                        }
                        return resolve();
                    },
                    function (error) {
                        return reject();
                    });
            });
        }
    }
    else {
        window.open("http://close/");
    }
}
function UDOCheckMandatoryFields() {
    var populated = true;
    formContext.getAttribute(function (attribute, index) {
        if (attribute.getRequiredLevel() === "required") {
            if (attribute.getValue() === null) {
                populated = false;
            }
        }
    });
    return populated;
}
function UDOSetRequiredLevelOnAllRequiredFields() {
    formContext.getAttribute(function (attribute, index) {
        if (attribute.getRequiredLevel() === "required") {
            if (attribute.getValue() === null) {
                var myName = attribute.getName();
                formContext.getAttribute(myName).setRequiredLevel("none");
            }
        }
    });
}

function uploadVBMSDoc() {
    Va.Udo.Crm.Scripts.VBMS.OpenVBMSDialog(formContext, '', '', '', '', '');
}