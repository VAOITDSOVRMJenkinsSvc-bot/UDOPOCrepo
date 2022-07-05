var locale = "1033";
var body = document.getElementsByTagName("body")[0];

function addScript(path) {
    var script = document.createElement("script");
    script.type = "text/javascript";
    script.onerror = handleLocError;
    script.src = path;
    body.appendChild(script);
}

function handleLocError() {
    var script = document.createElement("script");
    script.type = "text/javascript";
    var url = "WebResources/msdyusd_/scripts/SettingsPageStrings.js";
    var prefix = CrmEncodeDecode.CrmUrlEncode(window.parent.WEB_RESOURCE_ORG_VERSION_NUMBER);
    url = Xrm.Page.context.getClientUrl() + "/" + prefix + "/" + url;
    script.src = url;
    body.appendChild(script);
}

function LoadSettings(context, isexeccontext) {
    var ctx;
    if (isexeccontext === false) {
        ctx = context;
    }
    else {
        ctx = context.getContext();
    }
    locale = ctx.getUserLcid();
    var url = "WebResources/msdyusd_/scripts/SettingsPageStrings" + CrmEncodeDecode.CrmUrlEncode(locale == 1033 ? "" : "_" + locale) + ".js";
    var prefix = CrmEncodeDecode.CrmUrlEncode(window.parent.WEB_RESOURCE_ORG_VERSION_NUMBER);
    url = Xrm.Page.context.getClientUrl() + "/" + prefix + "/" + url;
    addScript(url);
}

function LoadResources() {
    if (window.USD == undefined) {
        Type.registerNamespace("USD");
    }
    var ctx = Xrm.Page.context;
    locale = ctx.getUserLcid();
    var url = "WebResources/msdyusd_/scripts/SettingsPageStrings" + CrmEncodeDecode.CrmUrlEncode(locale == 1033 ? "" : "_" + locale) + ".js";
    var prefix = CrmEncodeDecode.CrmUrlEncode(window.parent.WEB_RESOURCE_ORG_VERSION_NUMBER);
    url = Xrm.Page.context.getClientUrl() + "/" + prefix + "/" + url;
    var safeUrl = Xrm.Page.context.getClientUrl() + "/" + prefix + "/" + "WebResources/msdyusd_/scripts/SettingsPageStrings.js";
    var xhr = new XMLHttpRequest();
    xhr.open('GET', url, false)
    if (xhr.addEventListener) {
        xhr.addEventListener('load', function () {
            if (xhr.status == 200) {
                eval(xhr.responseText);
            }
            else {
                LoadResourcesOnError(url, safeUrl);
            }
        });
    } else {
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4) {
                if (xhr.status == 200) {
                    eval(xhr.responseText);
                }
                else {
                    LoadResourcesOnError(url, safeUrl);
                }
            }
        }
    }
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.send(null);
}

function LoadResourcesOnError(url, safeUrl) {
    try {
        if (url != safeUrl) {
            url = safeUrl;
            xhr.open('GET', url, false)
            xhr.setRequestHeader("Content-Type", "application/json");
            xhr.send(null);
        }
    } catch (e) {
        showAlertDialog('', e.message, '', '');
    }
}

function IsDefault_OnChange(executionContext) {
    var attribute = executionContext.getEventSource();
    var fieldName = attribute.getName();
    var initial = Xrm.Page.getAttribute(fieldName).getInitialValue();
    var selected = Xrm.Page.getAttribute(fieldName).getSelectedOption();
    var selValue = selected.value;
    if (initial == 0 && selValue == 1) {
        showConfirmDialog(USD.Resources.ConfigurationPage.title, USD.Resources.ConfigurationPage.IsDefaultNotification, USD.Resources.ConfigurationPage.OK, USD.Resources.ConfigurationPage.Cancel, AssignConfigDefaultPostConfirm);
    }
}

function SetConfigAsDefault() {
    showConfirmDialog(USD.Resources.ConfigurationPage.title, USD.Resources.ConfigurationPage.IsDefaultNotification, USD.Resources.ConfigurationPage.OK, USD.Resources.ConfigurationPage.Cancel, AssignConfigDefaultPostConfirm);
}

function UnSetIsDefault_OnDeactivate(context) {
    if (context != null && context.getEventArgs() != null) {
        var saveMode = context.getEventArgs().getSaveMode();
        if (saveMode == 5) {
            var isdefault = Xrm.Page.getAttribute("msdyusd_isdefault").getSelectedOption();
            if (isdefault.value == 1) {
                Xrm.Page.getAttribute('msdyusd_isdefault').setValue(0);
            }
        }
    }
}

function UpdateConfigAsDefault(configId) {
    try {
        showConfirmDialog(USD.Resources.ConfigurationPage.title, USD.Resources.ConfigurationPage.IsDefaultNotification, USD.Resources.ConfigurationPage.OK, USD.Resources.ConfigurationPage.Cancel,
			function (returnValue) {
			    if (returnValue) {
			        var msdyusd_configuration = new Object();
			        msdyusd_configuration.msdyusd_isdefault = true;
			        var jsonEntity = window.JSON.stringify(msdyusd_configuration);
			        var serverUrl = Xrm.Page.context.getClientUrl();
			        var ODataPath = serverUrl + "/XRMServices/2011/OrganizationData.svc";
			        var select = "/msdyusd_configurationSet(guid'" + CrmEncodeDecode.CrmUrlEncode(configId) + "')";
			        $.ajax({
			            type: "POST",
			            contentType: "application/json; charset=utf-8",
			            datatype: "json",
			            url: ODataPath + select,
			            data: jsonEntity,
			            beforeSend: function (XMLHttpRequest) {
			                XMLHttpRequest.setRequestHeader("Accept", "application/json");
			                XMLHttpRequest.setRequestHeader("X-HTTP-Method", "MERGE");
			            },
			            success: function () {
			                var objectTypeCode = Xrm.Internal.getEntityCode("msdyusd_configuration");
			                Xrm.Internal.refreshParentGrid(objectTypeCode, "", "");
			            },
			            error: function (xmlHttpRequest, textStatus, errorThrown) {
			                showAlertDialog(textStatus, errorThrown, '', '');
			            }
			        });
			    }
			});
    }
    catch (e) { showAlertDialog('', e.message, '', ''); }
}

function retrieveConfigStatus(Id) {
    var serverUrl = Xrm.Page.context.getClientUrl();
    var ODataPath = serverUrl + "/XRMServices/2011/OrganizationData.svc";
    var select = "/msdyusd_configurationSet(guid'" + CrmEncodeDecode.CrmUrlEncode(Id) + "')?$select=statecode,msdyusd_isdefault";
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: ODataPath + select,
        data: null,
        beforeSend: function (XMLHttpRequest) {
            XMLHttpRequest.setRequestHeader("Accept", "application/json");
        },
        error: function (xmlHttpRequest, textStatus, errorThrown) {
            showAlertDialog(textStatus, errorThrown, '', '');
        },
        success: function (data, textstatus, XMLHttpRequest) {
            var records = window.JSON.parse(XMLHttpRequest.responseText).d;
            var stateCode = records.statecode.Value;
            var isDefault = records.msdyusd_isdefault;
            if ((stateCode != 0) || (stateCode === null) || (stateCode === undefined) || (stateCode === "")) {
                showAlertDialog(USD.Resources.ConfigurationPage.ErrorTitle, USD.Resources.ConfigurationPage.ErrorMessage, '', USD.Resources.ConfigurationPage.Cancel);
                return;
            }
            else {
                if (isDefault) {
                    showAlertDialog(USD.Resources.ConfigurationPage.ErrorTitle, USD.Resources.ConfigurationPage.SetDefaultErrorMessage, USD.Resources.ConfigurationPage.okButton, '');
                }
                else {
                    UpdateConfigAsDefault(Id);
                }
                return;
            }
        }
    });
}

function SetSelectedConfigAsDefault(SelectedControlSelectedItemIds) {
    if (SelectedControlSelectedItemIds != null && SelectedControlSelectedItemIds != "") {
        var strIds = SelectedControlSelectedItemIds.toString();
        var arrIds = strIds.split(",");
        for (var i = 0; i < arrIds.length; i++) {
            retrieveConfigStatus(arrIds[i]);
        }
    }
}

function showConfirmDialog(title, message, okText, cancelText, callback) {
    var options = new Xrm.DialogOptions;
    options.width = 550;
    options.height = 205;
    var url = Mscrm.CrmUri.create("$Webresource:msdyusd_/ConfirmDialog.html?Data=");
    Xrm.Internal.openDialog(url + encodeURIComponent("title=" + title + "&message=" + message + "&ok=" + okText + "&cancel=" + cancelText), options, window, null, callback);
}

function showAlertDialog(title, message, okText, cancelText) {
    var options = new Xrm.DialogOptions;
    options.width = 550;
    options.height = 205;
    var url = Mscrm.CrmUri.create("$Webresource:msdyusd_/ConfirmDialog.html?Data=");
    Xrm.Internal.openDialog(url + encodeURIComponent("title=" + title + "&message=" + message + "&ok=" + okText + "&cancel=" + cancelText), options, window, null, null);
}

function AssignConfigDefaultPostConfirm(returnValue) {
    if (returnValue) {
        var configId = Xrm.Page.data.entity.getId();
        SetDefaultConfiguration(configId);
    }
    else {
        var initial = Xrm.Page.getAttribute('msdyusd_isdefault').getInitialValue();
        var selected = Xrm.Page.getAttribute('msdyusd_isdefault').getSelectedOption();
        var selValue = selected.value;
        if (initial == 0 && selValue == 1) {
            Xrm.Page.getAttribute('msdyusd_isdefault').setValue(0);
        }
        Xrm.Page.getControl('msdyusd_isdefault').setFocus();
    }
}

function SetDefaultConfiguration(configId) {
    try {
        var configEntity = {};
        configEntity.msdyusd_isdefault = true;

        SDK.REST.updateRecord(
            configId,
            configEntity,
            "msdyusd_configuration",
            function (entity) {
                Xrm.Page.getAttribute('msdyusd_isdefault').setValue(1);
            },
            function (error) {
                showAlertDialog('', error.message, '', '');
            }
        );
    } catch (e) {
        showAlertDialog('', e.message, '', '');
    }
}


function HideSetDefaultCommandButton() {
    return Xrm.Page.getAttribute('msdyusd_isdefault').getValue();
}

var parentRecordId = '';
var cloneConfigName = '';
function CloneRecord(recordId, selectedcontrol) {

    if (recordId == undefined || !(recordId.toString())) {
        if (Xrm.Page.data != null && Xrm.Page.data.entity != null) {
            recordId = Xrm.Page.data.entity.getId();
            cloneConfigName = Xrm.Page.getAttribute("msdyusd_name").getValue();
        }
    }
    else {
        cloneConfigName = selectedcontrol.getCellValue("msdyusd_name", recordId.toString());
    }
    parentRecordId = recordId;
    showConfirmDialog(USD.Resources.ConfigurationPage.cloneTitle, USD.Resources.ConfigurationPage.cloneMessage, USD.Resources.ConfigurationPage.cloneButton, USD.Resources.ConfigurationPage.cloneCancel, onCloneConfirm);
}

function onCloneConfirm(doClone) {
    try {
        if (!doClone) {
            parentRecordId = '';
            cloneConfigName = '';
            return;
        }

        var lookupData = new Array();
        var lookupItem = new Object();
        lookupItem.Id = parentRecordId.toString();
        lookupItem.LogicalName = "msdyusd_configuration";

        var configEntity = {};
        configEntity.msdyusd_parentconfigurationid = lookupItem;
        configEntity.msdyusd_name = cloneConfigName + USD.Resources.ConfigurationPage.copy;

        SDK.REST.createRecord(
			configEntity,
			"msdyusd_configuration",
			function (entity) {
			    if (Xrm.Page.ui == null) {
			        var objectTypeCode = Xrm.Internal.getEntityCode("msdyusd_configuration")
			        Xrm.Internal.refreshParentGrid(objectTypeCode, "", "");
			    }
			    else {
			        if (entity != null && entity.msdyusd_configurationId != null) {
			            Xrm.Utility.openEntityForm("msdyusd_configuration", entity.msdyusd_configurationId);
			            Xrm.Page.ui.controls.get("msdyusd_name").setFocus(true);
			        }
			    }
			},
			function (error) {
			    if (error.message.indexOf('inactive record') > 0)
			        showAlertDialog(USD.Resources.ConfigurationPage.inactiveCloneTitle, USD.Resources.ConfigurationPage.inactiveCloneAlert, USD.Resources.ConfigurationPage.okButton, '');
			    else
			        showAlertDialog(USD.Resources.ConfigurationPage.inactiveCloneTitle, USD.Resources.ConfigurationPage.cloningError + "\r\n" + error.message, USD.Resources.ConfigurationPage.okButton, '');
			}
		);
        parentRecordId = '';
        cloneConfigName = '';
    } catch (e) {
        //    alert(e);
        parentRecordId = '';
        cloneConfigName = '';
    }
}