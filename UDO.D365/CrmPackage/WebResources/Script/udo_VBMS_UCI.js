"use strict";

var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Uci = Va.Udo.Crm.Uci || {};
Va.Udo.Crm.Uci.Scripts = Va.Udo.Crm.Uci.Scripts || {};

/// <summary>VBMS Uploader constructor function to upload files to VBMS</summary>
/// <param name="executionContext" type="Object">executionContext passed from the form</param>
/// <param name="paramObj" type="Object">parameter object containing values such as: hideTabWhenComplete, tabName, sectionName, webresourceName, fileNumber, vbmsUploadRole, vbmsDocType, parentEntityLogicalName, parentEntityId, successCallback, errorCallback</param>
/// <returns type="Object">Returns the VbmsUploader object; example: var vbms = new VbmsUploader(exCon, parObj);</returns>
Va.Udo.Crm.Uci.Scripts.VbmsUploader = function (executionContext, paramObj) {
	// Confirm required params have been provided
	if (arguments.length !== 2) {
		throw ("An error occurred in VbmsUci constructor: Required number of parameters: 2");
	}

	// Private variables
	var _private = {
		_HasVBMSPermission: false,
		_EntityName: "",
		hideTabWhenComplete: paramObj.hideTabWhenComplete,
		tabName: paramObj.tabName,
		sectionName: paramObj.sectionName,
		webresourceName: paramObj.webresourceName,
		fileNumber: paramObj.fileNumber,
		vbmsUploadRole: paramObj.vbmsUploadRole,
		vbmsDocType: paramObj.vbmsDocType,
		parentEntityLogicalName: paramObj.parentEntityLogicalName,
		parentEntityId: paramObj.parentEntityId,
		successCallback: paramObj.successCallback,
		errorCallback: paramObj.errorCallback,
		cancelCallback: paramObj.cancelCallback,
		allowedEntities: [
			"udo_servicerequest",
			"udo_lettergeneration",
			"udo_vbmsdocument",
			"va_fnod"
		],

		// context variables
		globalContext: Xrm.Utility.getGlobalContext(),
		//version: globalContext.getVersion(),
		exCon: executionContext,
		formContext: executionContext.getFormContext(),
		dialogTab: {},
		dialogSection: {},
		dialogCtrl: {}
	}

	// Public variables
	var self = this;

	// Public methods
	self.Document = {
		TypeCode: null,
		VBMSDocId: null,
		Create: function (filenumber, claimnumber, firstname, middlename, lastname, role, entityId, entityName, doctype) {
			if (_private.allowedEntities.indexOf(entityName) === -1) {
				return;
			}

			_private._EntityName = entityName;

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

				self.Document.VBMSDocId = entityId;
				self.Uploader.Open("ctrludovbmsuploader");


			} else {
				var vbmsDoc = { udo_name: "VBMS Document" };

				
				if (!isNullOrEmpty(entityId) && entityName === "udo_servicerequest") {
					vbmsDoc["udo_servicerequestid@odata.bind"] = "/udo_servicerequests(" + entityId.replace("{", "").replace("}", "") + ")";
				}
				if (!isNullOrEmpty(entityId) && entityName === "udo_lettergeneration") {
					vbmsDoc["udo_lettergenerationid@odata.bind"] = "/udo_lettergenerations(" + entityId.replace("{", "").replace("}", "") + ")";
				}
				if (!isNullOrEmpty(entityId) && entityName === "va_fnod") {
					// TODO: Finish adding va_fnod relationship
					vbmsDoc["udo_fnodid@odata.bind"] = "/va_fnods(" + entityId.replace("{", "").replace("}", "") + ")";
				}
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

				Xrm.WebApi.createRecord("udo_vbmsdocument", vbmsDoc).then(
					function (data) {
						self.Document.CreateDocumentComplete(data);
					},
					function (error) {
						errorAlert("VBMS Document failed to create: " + error.message);
					}
				);
			}

		},
		CreateDocumentComplete: function (req) {

			if (req === null && req.id === null) return;

			self.Document.VBMSDocId = req.id;
			self.Uploader.Open("ctrludovbmsuploader");
		},
		Delete: function (docid) {
			Xrm.WebApi.deleteRecord("udo_vbmsdocument", docid).then(
				function (data) {
					console.log("Message from Document.Delete(): successfully deleted " + data.entityType);
				},
				function (error) {
					errorAlert("VBMS Document failed to delete - " + error.message);
				}
			);
		}
	}

	self.Note = {
		VBMSNoteId: null,
		Create: function (vbmsDocumentId) {

			if (vbmsDocumentId !== null) {

				var note = {};
				note.notetext = "VBMS Document";
				note.objectid = { Id: vbmsDocumentId.toString(), LogicalName: "udo_vbmsdocument" };
				note.objecttypecode = "udo_vbmsdocument";

				Xrm.WebApi.createRecord("annotation", note).then(
					function (data) {
						CreateNoteComplete(data);
					},
					function (err) {
						errorAlert("Annotation failed to create - " + err.message);
					}
				);
			}
		},
		CreateNoteComplete: function (data) {
			function PopupWindow(url, w, h) {
				var left = (screen.width / 2) - (w / 2);
				var top = (screen.height / 2) - (h / 2);
				return window.open(url, "", "width=" + w + ", height=" + h + ", top=" + top + ", left=" + left + ", toolbar=no");
			}

			if (data !== null && data.id !== null) {
				self.Note.VBMSNoteId = data.id;
			}
		},
		FindNote: function (docid) {
			return new Promise(function (resolve, reject) {
				docid = docid.replace("{", "").replace("}", "");
				var columns = ["name", "annotationid"];
				filter = "?$select=name,annotationid&$filter=objectid eq " + docid + "";

				Xrm.WebApi.retrieveMultipleRecords("annotation", filter).then(
					function (data) {
						loadNoteComplete(data.value);
						return resolve(true);
					},
					function (error) {
						errorAlert("An error occurred within Note.FindNote(): " + error.message);
						return reject(error);
					}
				);
			});
		},
		loadNoteComplete: function (data) {
			var result;
			for (var i = 0; i < data.length; i++) {
				var note = data[i];
				self.Note.VBMSNoteId = note.annotationid;
				result = note.annotationid;
			}

			if (data.length === 0) {
				errorAlert("Unexpected error - Could not find uploaded Note.");
			}
		}
	}

	/// <summary>VBMS Uploader function to open the VBMS upload dialog</summary>
	/// <param name="filenumber" type="string">filenumber for the VBMS upload</param>
	/// <param name="claimnumber" type="string">claimnumber for the VBMS upload</param>
	/// <param name="firstname" type="string">firstname for the VBMS upload</param>
	/// <param name="middlename" type="string">middlename for the VBMS upload</param>
	/// <param name="lastname" type="string">lastname for the VBMS upload</param>
	/// <param name="role" type="string">role for the VBMS upload</param>
	/// <param name="doctype" type="Object">doctype lookup object for the VBMS upload</param>
	/// <returns type="undefined">Does not return data; opens the VBMS uploader dialog on the form</returns>
	self.OpenVBMSDialog = function (filenumber, claimnumber, firstname, middlename, lastname, role, doctype) {
		// Check if uploader is already open
		_private.dialogTab = _private.formContext.ui.tabs.get(_private.tabName);
		_private.dialogSection = _private.dialogTab.sections.get(_private.sectionName);
		_private.dialogCtrl = _private.formContext.getControl(_private.webresourceName);

		if (_private.dialogTab.getVisible() && _private.dialogSection.getVisible() && _private.dialogCtrl.getVisible()) {
			return;
		}

		var requiredAttributesWithNullValues = [];
		_private.formContext.data.entity.attributes.forEach(function (attribute, index) {

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
			_private.formContext.data.save(true).then(successSave, function () { errorAlert("An error occurred when saving."); });
		}

		function successSave() {
			self.Initialize();

			if (!_private._HasVBMSPermission) {
				errorAlert("You must be granted VBMS Permission to continue to upload a document");
				return;
			}

            var ssn = "";
			if (!filenumber)
				ssn = _private.fileNumber;
			else
				ssn = param.data.filenumber;

			if (!role) {
				if (_private.vbmsUploadRole) {
					role = _private.vbmsUploadRole;
				}
			}

			if (!doctype) {
				if (_private.vbmsDocType) {
					doctype = _private.vbmsDocType;
				}
			}
			if (ssn !== null) {
				var entityId;
				var message = "Did you intend to upload a document \n for xxx - xx -" + ssn.substr(ssn.length - 4) + "?";

				var confirmStrings = { title: "Upload Document?", text: message, subtitle: "", confirmButtonLabel: "Yes", cancelButtonLabel: "No" };
				var confirmOptions = { height: 200, width: 400 };
				Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions)
					.then(function (success) {
						var entityId = _private.parentEntityId;
						if (_private.parentEntityId === null || _private.parentEntityId === undefined) {
							entityId = _private.formContext.data.entity.getId();
						}
						if (success.confirmed) {
							var entityName = _private.parentEntityLogicalName;
							if (_private.parentEntityLogicalName === null || _private.parentEntityLogicalName === undefined) {
								entityName = _private.formContext.data.entity.getEntityName().toLowerCase();
							}

							self.Document.Create(filenumber, claimnumber, firstname, middlename,
								lastname, role, entityId, entityName, doctype);
						}
						else {
							return;
						}
					}, function (error) {
						errorAlert("Error uploading document " + error.message);
					}
					);
			} else {
				errorAlert("Please enter File/Claim number then save.");
			}
		}
	}

	self.Initialize = function () {
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

		if (checkUserHasRole(["VBMS User", "VBMS Administrator", "System Administrator"]).found) {
			_private._HasVBMSPermission = true;
		}

		self.Document.TypeCode = getEntityTypeCode("udo_vbmsdocument");
	}

	self.Uploader = {
		Open: function (id) {
			var params = {
				docid: self.Document.VBMSDocId,
				dialogCtrl: _private.webresourceName,
				dialogSection: _private.sectionName,
				dialogTab: _private.tabName,
				hideTabWhenComplete: _private.hideTabWhenComplete,
				successCallback: _private.successCallback,
				errorCallback: _private.errorCallback,
				cancelCallback: _private.cancelCallback
			}

			if (_private.dialogTab.getVisible() === false) {
				_private.dialogTab.setVisible(true);
			}
			_private.dialogSection.setVisible(true);
			_private.dialogCtrl.setVisible(true);

			// Initialize the VBMS Upload control
			_private.dialogCtrl.getContentWindow().then(function (innerContentWin) {
				innerContentWin.Initialize(_private.exCon, params);
			});
		}
	}

	// Private Methods
	function errorAlert(message) {
		var alertStrings = { text: message };
		var alertOptions = { height: 120, width: 260 };
		Xrm.Navigation.openAlertDialog(alertStrings, alertOptions);
	}

	function checkUserHasRole(roleNames) {
		var responseObj = {
			found: false,
			message: null
		}

		if (roleNames === null || roleNames === undefined || roleNames.length < 1) {
			responseObj.message = new Error("Error in checkUserHasRole: roleNames is null, undefined, or empty");
			return responseObj;
		}

		var userRoles = _private.globalContext.userSettings.roles;

		if (userRoles === null || userRoles === undefined || userRoles.length < 1) {
			responseObj.message = new Error("Error in checkUserHasRole: userRoles is null, undefined, or empty");
			return responseObj;
		}

		var userRoleNames = [];
		var len = roleNames.length;

		// Populate userRoleNames
		userRoles.forEach(function (roleObj) {
			var roleName = roleObj.name;
			userRoleNames.push(roleName);
		});

		// Check if userRoleNames contains one of the roles provided
		for (var i = 0; i < len; i++) {
			if (userRoleNames.indexOf(roleNames[i]) > -1) {
				responseObj.found = true;
				break;
			}
		}

		return responseObj;
	}
}