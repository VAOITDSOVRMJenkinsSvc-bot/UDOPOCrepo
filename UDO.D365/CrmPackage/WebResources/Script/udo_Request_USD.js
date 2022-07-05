"use strict";

var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
Va.Udo.Crm.Scripts.Request = Va.Udo.Crm.Scripts.Request || {};

Va.Udo.Crm.Scripts.Request.VerifyCallTypeDetails = function (context) {
    try {
        context.data.save().then(function () {
            if (context.getAttribute("udo_calltype") === null || context.getAttribute("udo_callsubtype") === null) {
                UDO.Shared.openAlertDialog("Please enter the call type and subtype and click on Get Request Details.");
            }
        }, null);
    }
    catch (e) { 
        UDO.Shared.openAlertDialog(e.message); 
    }
}
