window["ENTITY_SET_NAMES"] = window["ENTITY_SET_NAMES"] || JSON.stringify({
	"annotation": "annotations",
	"vbms_document": "vbms_documents"
});

window["ENTITY_PRIMARY_KEYS"] = window["ENTITY_PRIMARY_KEYS"] || JSON.stringify({
	"annotation": "annotationid",
	"vbms_document": "vbms_documentid"
});

//var globalContext = GetGlobalContext();
//var version = globalContext.getVersion();
var params = {};
var parentExecutionContext = null;
var parentFormContext = null;
var dialogCtrl = null;
var dialogSection = null;
var dialogTab = null;
var successCallback = null;
var errorCallback = null;
var cancelCallback = null;

var _guid = function () {
	var randomValuesArray = new Uint16Array(8);
	var s4 = function (i) {
		return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
	};

	var crypto = window.crypto;
	if (!crypto) crypto = window.msCrypto;
	if (crypto && crypto.getRandomValues) {
		crypto.getRandomValues(randomValuesArray);
		s4 = function (i) {
			var v = randomValuesArray[i].toString(16);
			while (v.length < 4) { v = "0" + v; }
			return v;
		};
	}

	// uses secure crypto is available
	return s4(0) + s4(1) + "-" + s4(2) + "-" + s4(3) + "-" + s4(4) + "-" + s4(5) + s4(6) + s4(7);
};

var getUrlParams = function () {
	var par = location.search.substr(1).split("data=");
	var data = decodeURIComponent(par[1]).split("&");
	for (var i in data) {
		var paramPair = data[i].split("=");
		params[paramPair[0]] = paramPair[1]
	}
};

var setUpDialog = function () {
	var _buttonTypes = {
		Ok: 1,
		Cancel: 2,
		Abort: 3,
		Retry: 4,
		Ignore: 5,
		Yes: 6,
		No: 7
	};

	var _getKey = function (arr, val) {
		for (var key in arr) {
			if (val === arr[key]) return key;
		}
		return "<null>";
	};

	var pid = "popup-" + _guid();
	var popupWidth = 300;
	var popupHeight = 200;


	// Set Default Title
	var title = "VBMS Upload";
	var content = "<div style='margin:20px;'><input type='file' name='ctrludovbmsuploader_file' id='ctrludovbmsuploader_file' style='width:100%'/></div>";

	var popupStyle = "z-index:500;position:fixed;top:50%;left:50%;" +
		"margin-left:-" + Math.floor(popupWidth / 2).toString() + "px;" +
		"margin-top:-" + Math.floor(popupHeight / 2).toString() + "px;" +
		"width:" + popupWidth.toString() + "px;height:" + popupHeight.toString() + "px;" +
		"font-family:Segoe UI, Tahoma, Arial;font-size:11px;color:#000;background-color:#fff;" +
		"border:3px solid #000;";


	//var popupDiv = $("<div tabindex='0' class='popup' style='" + popupStyle + "' id='" + pid + "' role='alertdialog' aria-labelledby='" + pid + "_popupTitle' aria-describedby='" + pid + "_popupMessage' />");
	var popupDiv = $("<div tabindex='0' class='popup' id='" + pid + "' role='alertdialog' aria-labelledby='" + pid + "_popupTitle' aria-describedby='" + pid + "_popupMessage' />");
	var titleClass = "popupTitle popup Question";
	var titleDiv = $("<div class='" + titleClass + "' id='" + pid + "_popupTitle'/>");
	titleDiv.text(title);
	// draggable title area...

	var mousemoveEvent = function (e) {
		var popup = $("#" + pid);
		if (popup.data("dragging")) {
			var o = popup.data("dragoffset");
			popup.offset({
				top: e.pageY - o.top,
				left: e.pageX - o.left
			});
		}
	};

	var mouseupEvent = function (e) {
		var popup = $("#" + pid);
		popup.data("dragging", false);
		popup.data("dragoffset", null);
		$(this).removeAttr('unselectable');
		$(document.body).unbind('mousemove', mousemoveEvent);
		popup.unbind('mouseup', mouseupEvent);
	};

	titleDiv.css('cursor', 'move')
		.mousedown(function (e) {
			var popup = $("#" + pid);
			popup.data("dragging", true);
			var offset = {
				top: (e.pageY - $(this).offset().top),// + $(window).scrollTop(),
				left: (e.pageX - $(this).offset().left)// + $(window).scrollLeft()
			};
			$(this).attr('unselectable', 'on');
			popup.data("dragoffset", offset);
			// movement is tracked on body
			$(document.body).mousemove(mousemoveEvent);
			popup.mouseup(mouseupEvent);
		});

	//  popupDiv.append(titleDiv);

	var contentDiv = $("<div class='popupContent'/>");
	contentDiv.html(content);
	contentDiv.height((popupHeight - 70).toString() + "px");

	var buttonfocus = function () { };

	popupDiv.append(contentDiv);

	var Button = function (popupId, buttonType, setfocus) {
		var name = _getKey(_buttonTypes, buttonType);
		var btn = $("<button type='button' id='popupBtn" + name + "' class='popupButton' />");
		//var btn = $("<button type='button' id='popupBtn" + name + "' class='popupButton' aria-label='" + ariaLabel + "' />");
		btn.text(name);
		var data = {
			PopupId: popupId,
			ClickedButton: {
				Name: name,
				ButtonType: buttonType
			}
		};
		btn.click(data, function (e) {
			var clicked = e.data.ClickedButton;
			var clickType = e.data.ClickedButton.ButtonType;

			var values = {};
			var popup = $("#" + e.data.PopupId);
			$("#" + e.data.PopupId + " input").each(function () {
				if (this.files) {
					values[this.name] = { value: this.value, files: this.files };
				} else {
					values[this.name] = this.value;
				}
			});
			$("#" + e.data.PopupId + " select").each(function () { values[this.name] = this.value; });
			$("#" + e.data.PopupId + " textarea").each(function () { values[this.name] = this.value; });

			var result = {
				Clicked: clicked,
				Values: values
			};

			popup.remove();  //get and remove the popup
			if (clickType === _buttonTypes.Cancel) {
				dialogCtrl.setVisible(false);
				dialogSection.setVisible(false);
				if (params.hideTabWhenComplete !== false) {
					dialogTab.setVisible(false);
				}

				parentFormContext.data.refresh(false).then(function () {
					if (cancelCallback !== null && cancelCallback !== undefined && typeof cancelCallback === "function") {
						cancelCallback();
					}
				});
			}
			else if (clickType === _buttonTypes.Ok) {
				readFile(result);
			}

		});

		if (setfocus) buttonfocus = function () { btn.focus(); }

		return btn;
	};

	var buttonsDiv = $("<div class='popupButtons'/>");

	buttonsDiv.append(Button(pid, _buttonTypes.Cancel, false));
	buttonsDiv.append(Button(pid, _buttonTypes.Ok, true));
	popupDiv.append(buttonsDiv);
	$(document.body).append(popupDiv);
	popupDiv.find("input, select, textarea").first().focus();
};

var readFile = function (data) {
	var file = null;
	try {
		file = data.Values["ctrludovbmsuploader_file"].files[0];
	} catch (e) {
		file = null;
	}
	if (file === null) {
		alert("No file provided for upload.");
		return;
	} else if (file.name.substring(file.name.length - 4).toLowerCase() !== ".pdf") {
		alert("PDFs are currently the only document type supported by VBMS Upload.");
		return;
	}

	var ShowProgress = function (docid) {
		if (typeof docid === "undefined" || docid === null) return;  //should not be called without docid
		var deferred = $.Deferred();

		var popupId = 'popup-' + _guid();

		Va.Udo.Crm.Scripts.Popup.Popup("Uploading to VBMS...",
			"<div style='padding:2px;text-align:center;vert-align:middle'>" +
			"<img src='/WebResources/udo_/images/search/loading.gif' alt='Uploading...' aria-label='Uploading...'/>" +
			"</div>",
			{
				id: popupId,
				buttons: Va.Udo.Crm.Scripts.Popup.PopupStyles.NoButtons + Va.Udo.Crm.Scripts.Popup.PopupStyles.Information,
				showImage: false
			}
		);

		//return $("#" + popupId);

		var statusCheck = function (docid, attempts) {
			if (!attempts) attempts = 0;

			Xrm.WebApi.retrieveRecord(docid.replace("{", "").replace("}", ""), )

			var req = new XMLHttpRequest();
			req.open("GET", parent.Xrm.Page.context.getClientUrl() + "/api/data/v9.1/udo_vbmsdocuments(" + docid.replace("{", "").replace("}", "") + ")?$select=statuscode", false);
			req.setRequestHeader("OData-MaxVersion", "4.0");
			req.setRequestHeader("OData-Version", "4.0");
			req.setRequestHeader("Accept", "application/json");
			req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
			req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
			req.onreadystatechange = function () {
				if (this.readyState === 4) {
					req.onreadystatechange = null;

					var _EntityName = parent.Xrm.Page.data.entity.getEntityName();

					if (this.status === 200) {
						var result = JSON.parse(this.response);


						if (attempts > 30) { //2.5 minutes

							var result = { message: "The upload timed out.  Please verify the file was uploaded to VBMS.", timeout: true };
							deferred.reject(result);
							return;
						}

						var statuscode = result["statuscode"];
						if (statuscode == 1) {
							setTimeout(function () { statusCheck(docid, 1 + attempts); }, 5000);
							return;
						}


						if (statuscode == 752280000) { //success
							var result = { message: "File Uploaded Successfully" };

							if (_EntityName == "udo_vbmsdocument" && window.IsUSD == true) {
								window.open("http://event/?EventName=Refresh");
							}

							deferred.resolve(result);
							return;
						} else { // failure
							var result = { message: "The upload of the document failed.", timeout: false };

							if (_EntityName == "udo_vbmsdocument" && window.IsUSD == true) {
								window.open("http://event/?EventName=Refresh");
							}

							deferred.reject(result);
							return;
						}

					} else {
						var result = { message: "The upload of the document failed.", timeout: false };

						if (_EntityName == "udo_vbmsdocument" && window.IsUSD == true) {
							window.open("http://event/?EventName=Refresh");
						}

						deferred.reject(result);
						return;
					}
				}
			};
			req.send();

		};

		statusCheck(docid, 0);
		return deferred.promise();

	};

	var ConvertToBase64 = function (data) {
		// Written according to the information for the algorithm on https://en.wikipedia.org/wiki/Base64
		var code = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/';

		var encodeChunk = function (chunk) {
			var offset = 63; // 2^6 - 1 (six bits, all ones, 111111=63=0x3F)
			// combine 3 8-bit values into a single value
			var combined = (chunk[0] << 16) | (chunk[1] << 8) | chunk[2];
			// split into 6-bit segments
			// combined, movied to the right 18 bits, then & with offset, 12 (6 less), 6, 0...
			var segments = [combined >> 18 & offset, combined >> 12 & offset, combined >> 6 & offset, combined & offset];
			// codify and return as string
			return code[segments[0]] + code[segments[1]] + code[segments[2]] + code[segments[3]];
		};

		var encodeExtra = function (chunk) {
			// pad zeros, encode, and replace what should be completely null sections with =
			if (chunk.length === 1) {
				return encodeChunk([chunk[0], 0, 0]).substring(0, 2) + "==";
			}
			return encodeChunk([chunk[0], chunk[1], 0]).substring(0, 3) + "=";
		};

		var b64 = [];

		var arr = new Uint8Array(data);

		var extra = arr.length % 3;
		var length = arr.length - extra;
		for (var i = 0; i < length; i += 3) {
			b64.push(encodeChunk([arr[i], arr[i + 1], arr[i + 2]]));
		}
		if (extra === 1) {
			b64.push(encodeExtra([arr[length]]));
		} else if (extra === 2) {
			b64.push(encodeExtra([arr[length], arr[length + 1]]));
		}

		return b64.join('');
	};

	var loaded = function () {
		var failMessage = function (err) {
			alert("The file was not able to be uploaded.");
		};
		// Obtain the read file data
		var arrayBuffer = this.result;
		var b64 = ConvertToBase64(arrayBuffer);

		// Attach to Note
		var docid = params.docid;
		var _filename = file.name.substring(Math.max(0, Math.max(file.name.lastIndexOf('\\'), file.name.lastIndexOf('/'))));
		var _subject = "VBMS Upload: " + _filename;
		var _objecttypecode = "udo_vbmsdocument";
		var _mimetype = "application/pdf";
		var _documentbody = b64;
		//var _regardingobjectid = { id: docid, LogicalName: "udo_vbmsdocument" };

		var entityObj = {};
		entityObj["objectid_udo_vbmsdocument@odata.bind"] = "/udo_vbmsdocuments(" + docid.replace("{", "").replace("}", "") + ")";
		entityObj.subject = _subject;
		entityObj.objecttypecode = _objecttypecode;
		entityObj.filename = _filename;
		entityObj.mimetype = _mimetype;
		entityObj.documentbody = _documentbody;

		//var popup = ShowProgress(docid);
		//parent.Xrm.Utility.showProgressIndicator("Uploading to VBMS");

		var req = new XMLHttpRequest();
		req.open("POST", parent.Xrm.Page.context.getClientUrl() + "/api/data/v9.1/annotations", false);
		req.setRequestHeader("OData-MaxVersion", "4.0");
		req.setRequestHeader("OData-Version", "4.0");
		req.setRequestHeader("Accept", "application/json");
		req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
		req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
		req.onreadystatechange = function () {
			if (this.readyState === 4) {
				req.onreadystatechange = null;
				if (this.status === 204) {

					var uri = this.getResponseHeader("OData-EntityId");
					var regExp = /\(([^)]+)\)/;
					var matches = regExp.exec(uri);
					var newEntityId = matches[1];

					ShowProgress(docid)
						.done(function (data) {
							Va.Udo.Crm.Scripts.Popup.MsgBox(data.message,
								Va.Udo.Crm.Scripts.Popup.PopupStyles.Information,
								"VBMS File Uploaded");
							// parent.Xrm.Utility.closeProgressIndicator();
							dialogCtrl.setVisible(false);
							dialogSection.setVisible(false);
							if (params.hideTabWhenComplete !== false) {
								dialogTab.setVisible(false);
							}

							if (successCallback !== null && successCallback !== undefined && typeof successCallback === "function") {
								successCallback();
							}
						})
						.fail(function (data) {
							if (data.timeout) {
								Va.Udo.Crm.Scripts.Popup.MsgBox(data.message,
									Va.Udo.Crm.Scripts.Popup.PopupStyles.Exclamation,
									"VBMS Upload Timeout");
							} 

							// parent.Xrm.Utility.closeProgressIndicator();
							dialogCtrl.setVisible(false);
							dialogSection.setVisible(false);
							if (params.hideTabWhenComplete !== false) {
								dialogTab.setVisible(false);
							}

							if (errorCallback !== null && errorCallback !== undefined && typeof errorCallback === "function") {
								errorCallback();
							}
						});

					//formContext.data.refresh(false);
					//alert("VBMS File Uploaded");

				} else {

					// parent.Xrm.Utility.closeProgressIndicator();
					Xrm.Navigation.openAlertDialog(this.statusText);
				}
			}
		};
		req.send(JSON.stringify(entityObj));

	};

	var errorHandler = function (evt) {
		if (this.error.name === "NotReadableError") {
			alert("The file provided could not be read.");
		}
	};

	var reader = new FileReader();
	// Handle progress, success, and errors
	reader.onload = loaded;
	reader.onerror = errorHandler;
	reader.readAsArrayBuffer(file);

};

function SetGlobalVarsFromParams(exCon, paramsObj) {
	params = paramsObj;
	parentExecutionContext = exCon;
	parentFormContext = parentExecutionContext.getFormContext();

	if (paramsObj.dialogCtrl === null || paramsObj.dialogCtrl === undefined) {
		dialogCtrl = parentFormContext.getControl("WebResource_uploadVBMS");
	} else {
		dialogCtrl = parentFormContext.getControl(paramsObj.dialogCtrl);
	}

	if (paramsObj.dialogTab === null || paramsObj.dialogTab === undefined) {
		dialogTab = parentFormContext.ui.tabs.get("uploadVBMS");
	} else {
		dialogTab = parentFormContext.ui.tabs.get(paramsObj.dialogTab);
	}

	if (paramsObj.dialogSection === null || paramsObj.dialogSection === undefined) {
		dialogSection = dialogTab.sections.get("uploadVBMSSec");
	} else {
		dialogSection = dialogTab.sections.get(paramsObj.dialogSection);
	}

	if (paramsObj.successCallback !== null && paramsObj.successCallback !== undefined) {
		successCallback = paramsObj.successCallback;
	}

	if (paramsObj.errorCallback !== null && paramsObj.errorCallback !== undefined) {
		errorCallback = paramsObj.errorCallback;
	}
}

function Initialize(exCon, paramsObj) {
	// Set global variables from the executionContext and paramsObj
	SetGlobalVarsFromParams(exCon, paramsObj);

	// Run main setupDialog function
	setUpDialog();
}