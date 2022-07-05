var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
Va.Udo.Crm.Scripts.VBMS = Va.Udo.Crm.Scripts.VBMS || {};
var globalContext = Xrm.Utility.getGlobalContext();
var version = globalContext.getVersion();
var lib = new CrmCommonJS(version);
var webApi = lib.WebApi;
var Util = lib.Util;
var Sec = lib.Security;

Va.Udo.Crm.Scripts.VBMS = {
    _HasVBMSPermission: false,
    Initialize: function () {

        /*function UserHasRole(roleName) {
            //TODO: convert to form context
            var serverUrl = Xrm.Page.context.getClientUrl();
            var oDataEndpointUrl = serverUrl + "/XRMServices/2011/OrganizationData.svc/";
            oDataEndpointUrl += "RoleSet?$filter=Name eq '" + roleName + "'";
            var service = GetRequestObject();

            if (service != null) {

                service.open("GET", oDataEndpointUrl, false);
                service.setRequestHeader("X-Requested-Width", "XMLHttpRequest");
                service.setRequestHeader("Accept", "application/json,text/javascript, *");
                //TODO: convert to WebAPI
                service.send(null);

                var requestResults = eval('(' + service.responseText + ')').d;
				
                if (requestResults != null) {

                    for (var x = 0; x < requestResults.results.length; x++) {

                        var role = requestResults.results[x];
                        var id = role.RoleId;

                        //Get Current User Roles
                        var currentUserRoles = Xrm.Page.context.getUserRoles();

                        //Check whether current user roles has the role passed as argument
                        for (var i = 0; i < currentUserRoles.length; i++) {
                            var userRole = currentUserRoles[i];
                            if (GuidsAreEqual(userRole, id)) {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        function GetRequestObject() {

            if (window.XMLHttpRequest) {
                return new window.XMLHttpRequest;
            }

            else {
                try {
                    return new ActiveXObject("MSXML2.XMLHTTP.3.0");
                }

                catch (ex) {
                    return null;
                }
            }
        }

        function GuidsAreEqual(guid1, guid2) {

            var isEqual = false;
            if (guid1 == null || guid2 == null) {
                isEqual = false;
            }
            else {
                isEqual = (guid1.replace(/[{}]/g, "").toLowerCase() == guid2.replace(/[{}]/g, "").toLowerCase());
            }

            return isEqual;
        }*/

        function getEntityTypeCode(entityName) {
            try {
                var command = new RemoteCommand("LookupService", "RetrieveTypeCode");
                command.SetParameter("entityName", entityName);
                var result = command.Execute();
                if (result.Success && typeof result.ReturnValue == "number") {
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
        Create: function (filenumber, firstname, middlename, lastname, lettergenerationid) {

            function isNullOrEmpty(str) {
                if (str instanceof Array) {
                    for (var i = 0; i < str.length; i++) {
                        if (!isNullOrEmpty(str[i])) return false;
                    }
                    return true;
                }
                if (typeof str === 'undefined' || str == null) return true;
                return str.length < 1;
            }

            var errors = "";
            if (servicerequestid == null && lettergenerationid == null) {
                if (isNullOrEmpty([filenumber, firstname, lastname])) {

                    errors += (errors.length > 0 ? "\r\n" : "") +
                        "Either a letter generation or the detailed information (filenumber,firstname,lastname) must be provided.";
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

            var vbmsDoc = { udo_name: "VBMS Document" };

            if (!isNullOrEmpty(lettergenerationid))
                vbmsDoc.udo_lettergenerationid = { Id: lettergenerationid, LogicalName: "udo_lettergeneration" };
            if (!isNullOrEmpty(filenumber))
                vbmsDoc.udo_filenumber = filenumber;
            if (!isNullOrEmpty(firstname))
                vbmsDoc.udo_firstname = firstname;
            if (!isNullOrEmpty(middlename))
                vbmsDoc.udo_middlename = middlename;
            if (!isNullOrEmpty(lastname))
                vbmsDoc.udo_lastname = lastname;

            //SDK.JQuery.createRecord(vbmsDoc, "udo_vbmsdocument", Va.Udo.Crm.Scripts.VBMS.Document.CreateDocumentComplete,
            webApi.CreateRecord(vbmsDoc, "udo_vbmsdocument")
            .then(function (data) {
                    CreateDocumentComplete(data)
                })
            .catch(function(error){
                alert("VBMS Document failed to create");
            });

        },
        CreateDocumentComplete: function (req) {

            if (req == null && req.id == null) return;

            Va.Udo.Crm.Scripts.VBMS.Document.VBMSDocId = req.id;

            function PopupWindow(url, w, h) {
                var left = (screen.width / 2) - (w / 2);
                var top = (screen.height / 2) - (h / 2);
                return window.open(url, "", "width=" + w + ", height=" + h + ", top=" + top + ", left=" + left + ", toolbar=no");
            }
            //TODO: convert to form context
            var urlName = Xrm.Page.context.getClientUrl();
            var url = urlName + '/Notes/edit.aspx?hideDesc=1&pId=' + encodeURIComponent("{" + req.udo_vbmsdocumentId + "}") + '&pType=' + Va.Udo.Crm.Scripts.VBMS.Document.TypeCode;
            var popup = PopupWindow(url, 500, 200);

            function looper() {
                if (!popup.closed) {
                    window.setTimeout(looper, 300);
                } else {
                    // Check if note was created
                    Va.Udo.Crm.Scripts.VBMS.Note.FindNote(docid)
                    .then(function () {
                        // If no note with document was created, then trigger a delete of the vbms document record
                        if (Va.Udo.Crm.Scripts.VBMS.Note.VBMSNoteId == null) {
                            Va.Udo.Crm.Scripts.VBMS.Document.Delete();
                        }
                    })
                    .catch(function(error){
                        
                    });
                }
            }

            window.setTimeout(looper, 300);

        },
        Delete: function () {
            //TODO: convert to WebAPI
            //CrmRestKit2011.deleteRecord(docid, "udo_vbmsdocument", function () { },
            webApi.DeleteRecord(docid, "udo_vbmsdocument")
            .then(function(){

            })
            .catch(function errorHandler(error) {
                    alert("VBMS Document failed to delete - " + err.message);
                }
            );

        }
    },
    Note: {
        VBMSNoteId: null,
        Create: function (vbmsDocumentId) {

            if (vbmsDocumentId != null) {

                var note = {};
                note.NoteText = "VBMS Document";
                note.ObjectId = { Id: vbmsDocumentId.toString(), LogicalName: "udo_vbmsdocument" };
                note.ObjectTypeCode = "udo_vbmsdocument";
                //TODO: convert to WebAPI
                //SDK.JQuery.createRecord(note, "Annotation", Va.Udo.Crm.Scripts.VBMS.Note.CreateNoteComplete,
                webApi.CreateRecord(note, "Annotation")
                .then(function(data){
                    CreateNoteComplete(data);
                })
                .catch(function (err) {
                    alert("Annotation failed to create - " + err.message);
                });
            }
        },
        CreateNoteComplete: function (data) {

            function PopupWindow(url, w, h) {
                var left = (screen.width / 2) - (w / 2);
                var top = (screen.height / 2) - (h / 2);
                return window.open(url, "", "width=" + w + ", height=" + h + ", top=" + top + ", left=" + left + ", toolbar=no");
            }
            //TODO: convert to form context
            var urlName = Xrm.Page.context.getClientUrl();

            if (data != null && data.AnnotationId != null) {
                Va.Udo.Crm.Scripts.VBMS.Note.VBMSNoteId = data.AnnotationId;

            }
        },
        FindNote: function (docid, callback) {
            //var options = "$select=Name,AnnotationId&$filter=ObjectId/Id eq (guid'" + docid + "')";
            return new Promise(function(resolve, reject){
                var columns = ["Name", "AnnotationId"];
                var filter = "$ObjectId eq guid'" + docid + "'" ;
                //TODO: convert to WebAPI
                //CrmRestKit2011.retrieveMultipleRecords("annotation", options, Va.Udo.Crm.Scripts.VBMS.Note.loadNoteComplete, function (error) { alert(error.message); }, callback);
                webApi.RetrieveMultiple("annotation", columns, filter)
                .then(function(data){
                    loadNoteComplete(data);
                    resolve(true);
                })
                .catch(function(error){
                    alert(error.message);
                    reject(error);    
                });
            });
        },
        loadNoteComplete: function (data) {

            for (var i = 0; i < data.length; i++) {
                alert(Object.keys(data[i]));
                var note = data[i];
                Va.Udo.Crm.Scripts.VBMS.Note.VBMSNoteId = note.id;
            }

            if (data.length == 0) {
                alert("Unexpected error - Could not find uploaded Note.");
            }
        }
    },
    OpenVBMSDialog: function (filenumber, firstname, middlename, lastname, ssid) {

        Va.Udo.Crm.Scripts.VBMS.Initialize();

        if (!Va.Udo.Crm.Scripts.VBMS._HasVBMSPermission) {
            alert("You must be granted VBMS Permission to continue to upload a document");
            return;
        }
        var encodedId = Va.Udo.Crm.Scripts.VBMS.ObfuscateString(ssid);
        if (encodedId != null) {
            var lettergenerationid;
            if (confirm("Did you intend to upload a document for xxx-xx-" + atob(encodedId))) {
                //TODO: convert to form context
                if (Xrm.Page.data.entity.getEntityName().toLowerCase() == "udo_lettergeneration") {
                    //TODO: convert to form context
                    lettergenerationid = Xrm.Page.data.entity.getId();
                }

                Va.Udo.Crm.Scripts.VBMS.Document.Create(filenumber, firstname, middlename, lastname, lettergenerationid);
            } 
        } else {
            alert("Please enter SSN then save.");
        }
    },
    ObfuscateString: function (ssid) {
        if (ssid != null){
            var last4 = ssid.substr(ssid.length - 4);
            var encodedString = btoa(last4);
		
            return encodedString;
        }
        return null;
		
    }
}