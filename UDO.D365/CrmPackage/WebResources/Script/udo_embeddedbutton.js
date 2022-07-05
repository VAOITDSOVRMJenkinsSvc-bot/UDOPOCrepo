"use strict";
var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Buttons = Va.Udo.Crm.Buttons || {};

Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
Va.Udo.Crm.Scripts.Buttons = Va.Udo.Crm.Scripts.Buttons || {};
var clickEvent = null;
var btnConfig = null;
var formContext;
Va.Udo.Crm.Scripts.Buttons.AddButton = function (formCon, id, name, label, method, image, enabled) {
    console.log("adding button");
    //initiateCommonScripts(null);
    console.log(formHelper);
    formContext = formCon;
	var config = {};
	config.name = name;
	config.label = label;
	config.onClick = method;
	config.image = image;
	config.id = id;
	if (typeof enabled === "undefined" || enabled===null) enabled=true;
    config.enabled = enabled;
    Va.Udo.Crm.Scripts.setupButton(config);
	Va.Udo.Crm.Buttons[id] = config;
};
Va.Udo.Crm.Scripts.Buttons.EnableButton=function(id) {
	try {
		var btn = Va.Udo.Crm.Buttons[id];
		btn.enable();
	} catch (err) {
		UDO.Shared.openAlertDialog("The button ["+id+"] was not found or initialized.");
	}
};
Va.Udo.Crm.Scripts.Buttons.DisableButton=function(id) {
	try {
		var btn = Va.Udo.Crm.Buttons[id];
		btn.disable();
	} catch (err) {
		UDO.Shared.openAlertDialog("The button ["+id+"] was not found or initialized.");
	}
};

Va.Udo.Crm.Scripts.setupButton = function (config) {
    console.log("setting up button with:");
    console.log(config);
        //  + "&onClick=" + config.onClick
    var params = encodeURIComponent("name=" + config.name + "&label=" + config.label +
                                    "&image=" + config.image + "&id=" + config.id + "&enabled=" + config.enabled);
    var control = formHelper.getControl(config.id);   
    console.log(control);
    var url = Xrm.Utility.getGlobalContext().getClientUrl() + "/WebResources/udo_embeddedbutton.html?data=" + params;
   
    control.setSrc(url);
    if (config.enabled) {
        var button = control.getObject().contentDocument.getElementById("embeddedbutton");
        $(button).on("click", config.onClick);
    }
};
