var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
Va.Udo.Crm.Scripts.CommonFunctions = Va.Udo.Crm.Scripts.CommonFunctions || {};
var exCon = null;
var formContext = null;
//WARNING! This is unsupported and should probably be removed
/*
Va.Udo.Crm.Scripts.CommonFunctions.TabOrderLefttoRight = function () {
    for (var i = 0; i < formContext.ui.controls.getLength() ; i++) {
        var control = formContext.ui.controls.get(i);
        var element = document.getElementById(control.getName());

        if (element.tabIndex && element.tabIndex != "0") {
            if (element.className == 'ms-crm-Hidden-NoBehavior')
                continue;
            if (element.tagName == 'A') {
                if (element.className != 'ms-crm-InlineTabHeaderText')
                    continue;
            }
            element.tabIndex = 1000 + (i * 10);
        }
    }
}*/
//WARNING! This is unsupported and should probably be removed
/*
Va.Udo.Crm.Scripts.CommonFunctions.TabOrderLefttoRightOnSection = function (tabName, sectionName) {
    exCon = execCon;
    formContext = exCon.getFormContext();
    if (tabName == '' || sectionName == ''){alert('Tab name or section name is unavailable'); return false;}
    var Section = formContext.ui.tabs.get(tabName).sections.get(sectionName);

    for (var i = 0; i < Section.controls.getLength() ; i++) {
        var control = formContext.ui.controls.get(i);
        var element = document.getElementById(control.getName());

        if (element.tabIndex && element.tabIndex != "0") {
            if (element.className == 'ms-crm-Hidden-NoBehavior')
                continue;
            if (element.tagName == 'A') {
                if (element.className != 'ms-crm-InlineTabHeaderText')
                    continue;
            }
            element.tabIndex = 1000 + (i * 10);
        }
    }
}*/

Va.Udo.Crm.Scripts.CommonFunctions.ToggleCreatedByDisplayOnload = function (execCon) {
    exCon = execCon;
    formContext = exCon.getFormContext();
 try {
        var createdBy = formContext.getControl("createdby");
        var udCreatedBy = formContext.getControl("udo_udcreatedby");
        var udCreatedByAttr = formContext.getAttribute("udo_udcreatedby");
        if (udCreatedByAttr.getValue()) {
            createdBy.setVisible(false);
            udCreatedBy.setVisible(true);
        }
        else {
            createdBy.setVisible(true);
            udCreatedBy.setVisible(false);
        }

    }
    catch (e) {
        alert("Encountered an error: " + e);
    }
}