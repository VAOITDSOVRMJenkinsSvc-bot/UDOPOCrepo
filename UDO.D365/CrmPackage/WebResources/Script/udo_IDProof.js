"use strict";

var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
//Va.Udo.Crm.Scripts.ClaimsDocumentDownload = Va.Udo.Crm.Scripts.ClaimsDocumentDownload || {};
var confirmStrings = {};
confirmStrings.confirmButtonLabel = "Yes";
confirmStrings.cancelButtonLabel = "No";
var confirmOptions = { height: 300, width: 600 };
var webApi = null;
window.parent.BrowserWindowReadyEvent = BrowserWindowReadyEvent;
var globalCon = Xrm.Utility.getGlobalContext();

function BrowserWindowReadyEvent(context) {
    context.ui.formSelector.items.forEach(
        function (control, index) {
            console.log(control.setVisible(false));
        }
    );
    context.ui.headerSection.setBodyVisible(false);
    context.ui.headerSection.setCommandBarVisible(false);
    context.ui.headerSection.setTabNavigatorVisible(false);
    context.ui.footerSection.setVisible(false);
}

function CheckAllIdProofFields(context) {
    var fields = ["udo_verifiedfirstname", "udo_verifiedlastname", "udo_verifieddob", "udo_verifiedssn", "udo_verifiedbranchofservice"];
    for (var i = 0; i < fields.length; i++) {
        try {
            var boolAttribute = context.getAttribute(fields[i]);
            boolAttribute.setValue(true);
        }
        catch (e) {
            UDO.Shared.openAlertDialog("Could not automate Verified fields" + e.message);
        }
    }
    context.data.save();
}
