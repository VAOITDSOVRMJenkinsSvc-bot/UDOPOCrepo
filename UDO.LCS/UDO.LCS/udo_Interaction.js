var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
Va.Udo.Crm.Scripts.Interaction = Va.Udo.Crm.Scripts.Interaction || {};

Va.Udo.Crm.Scripts.Interaction.Attributes = Va.Udo.Crm.Scripts.Interaction.Attributes || {};
Va.Udo.Crm.Scripts.Interaction.Attributes.NoPhoneNumberAvailable = "udo_nophonenumberavailable";
Va.Udo.Crm.Scripts.Interaction.Attributes.PhoneNumber = "udo_phonenumber";

Va.Udo.Crm.Scripts.Interaction.onLoad = function () {
// CSDev Left Intentionally Blank 
}

Va.Udo.Crm.Scripts.Interaction.onSave = function () {
// CSDev Left Intentionally Blank 
}

Va.Udo.Crm.Scripts.Interaction.AppendIfNotEmpty = function (param, arg) {
// CSDev Left Intentionally Blank 
}

Va.Udo.Crm.Scripts.Interaction.CheckPhoneForInitiateRequest = function () {
// CSDev Left Intentionally Blank 
}

Va.Udo.Crm.Scripts.Interaction.NoPhoneNumberAvailableonChange = function () {
// CSDev Left Intentionally Blank 
}

Va.Udo.Crm.Scripts.Interaction.PhoneNumberonChange = function () {
// CSDev Left Intentionally Blank 
}

//Deprecated: using USD RunXrmCommand with udo_popup.js
//Va.Udo.Crm.Scripts.Interaction.InitiateSessionCloseOut = function (requestId, isCaddFormDirty) {
//    if (isCaddFormDirty) {
//        if (!Va.Udo.Crm.Scripts.Interaction.validateCADD()) {
//            //Todo: Show CADD form and cancel session close out
//            window.open("http://uii/Global Manager/ShowTab?data=CADD");
//            return;
//        }
//    }
//    var interactionId = Xrm.Page.data.entity.getId();
//    //Else intitate session close out procedure
//    //Update Request end time
//    var endTime = new Date();
//    var endTimeString = endTime.toString();
//    if (requestId != "") {

//        CrmRestKit2011.Update("udo_request", requestId, { udo_EndTime: endTime }, false).fail(function (xhr, statusCode, code) { alert(xhr.responseText); });
//    }
//    //Update Interaction end time
//    CrmRestKit2011.Update("udo_interaction", interactionId, { udo_EndTime: endTime, udo_Status: false }, false).fail(function (xhr, statusCode, code) { alert(xhr.responseText); });
//    window.open("http://uii/Session Tabs/CloseSession");

//}

//Va.Udo.Crm.Scripts.Interaction.validateCADD = function () {
//    var caddConfirm = confirm("There are unsaved changes on the form.  Press OK to continue with close and discard changes on CADD.   Press Cancel to go to CADD.");
//    return caddConfirm;
//}
