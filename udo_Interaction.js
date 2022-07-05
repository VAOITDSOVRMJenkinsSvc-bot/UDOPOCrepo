var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
Va.Udo.Crm.Scripts.Interaction = Va.Udo.Crm.Scripts.Interaction || {};

Va.Udo.Crm.Scripts.Interaction.Attributes = Va.Udo.Crm.Scripts.Interaction.Attributes || {};
Va.Udo.Crm.Scripts.Interaction.Attributes.NoPhoneNumberAvailable = "udo_nophonenumberavailable";
Va.Udo.Crm.Scripts.Interaction.Attributes.PhoneNumber = "udo_phonenumber";
var exCon = null;
var formContext = null;
Va.Udo.Crm.Scripts.Interaction.onLoad = function (execCon) {
    exCon = execCon;
    formContext = exCon.getFormContext();
    try {
        var createdBy = Xrm.Page.getControl("createdby");
        var udCreatedBy = Xrm.Page.getControl("udo_udcreatedby");
        var udCreatedByAttr = Xrm.Page.getAttribute("udo_udcreatedby");
        var addlDetailsTab = Xrm.Page.ui.tabs.get("AdditionalDetailsTab");
        if (udCreatedByAttr.getValue()) {
            createdBy.setVisible(false);
            udCreatedBy.setVisible(true);
        }
        else {
            createdBy.setVisible(true);
            udCreatedBy.setVisible(false);
        }
        addlDetailsTab.setDisplayState("collapsed");

        Xrm.Page.getAttribute("udo_firstname").setSubmitMode("always");
        Xrm.Page.getAttribute("udo_lastname").setSubmitMode("always");
        Xrm.Page.getAttribute("udo_phonenumber").setSubmitMode("always");
        Xrm.Page.getAttribute("udo_nophonenumberavailable").setSubmitMode("always");

        var channel = Xrm.Page.getAttribute("udo_channel").getValue();
        if (channel == 752280002 || channel == 752280003) {
            Xrm.Page.getControl("udo_veteranfirstname").setVisible(true);
            Xrm.Page.getControl("udo_veteranmiddlename").setVisible(true);
            Xrm.Page.getControl("udo_veteranlastname").setVisible(true);
            Xrm.Page.getControl("udo_veterandob").setVisible(true);
            Xrm.Page.getControl("udo_veteranssn").setVisible(true);
            Xrm.Page.getControl("udo_purposeofthevisit").setVisible(true);
            Xrm.Page.getControl("udo_hasspecialsituations").setVisible(true);

            Xrm.Page.ui.tabs.get("detailsTab").sections.get("udo_special_situations").setVisible(true);
        }

        Va.Udo.Crm.Scripts.Interaction.CheckPhoneForInitiateRequest();

    }
    catch (e) {
        //Do not raise event
    }
}

Va.Udo.Crm.Scripts.Interaction.onSave = function () {
    try {
        Va.Udo.Crm.Scripts.Interaction.CheckPhoneForInitiateRequest();
        //if (window.isUSD == true) {
        var firstName = Xrm.Page.getAttribute("udo_firstname").getValue();
        firstName = firstName == null ? "" : firstName;
        var lastName = Xrm.Page.getAttribute("udo_lastname").getValue();
        lastName = lastName == null ? "" : lastName;
        var phoneNumber = Xrm.Page.getAttribute("udo_phonenumber").getValue();
        phoneNumber = phoneNumber == null ? "" : phoneNumber;
        var noPhoneNumberAvailable = Xrm.Page.getAttribute("udo_nophonenumberavailable").getValue();
        noPhoneNumberAvailable = noPhoneNumberAvailable == null ? "" : noPhoneNumberAvailable;
        var isCallerAuthorized = Xrm.Page.getAttribute("udo_iscallerauthorized").getValue();
        isCallerAuthorized = isCallerAuthorized == null ? "" : isCallerAuthorized;
        var eventString = Va.Udo.Crm.Scripts.Interaction.AppendIfNotEmpty("&firstName=", firstName) +
            Va.Udo.Crm.Scripts.Interaction.AppendIfNotEmpty("&lastName=", lastName) +
            Va.Udo.Crm.Scripts.Interaction.AppendIfNotEmpty("&phoneNumber=", phoneNumber) +
            Va.Udo.Crm.Scripts.Interaction.AppendIfNotEmpty("&noPhoneNumberAvailable=", noPhoneNumberAvailable) +
            Va.Udo.Crm.Scripts.Interaction.AppendIfNotEmpty("&isCallerAuthorized=", isCallerAuthorized);

        if (window.IsUSD == true) {
            window.open("http://event/?eventName=InteractionOnSave" + eventString);
        }


        //if (lastName &&
        //    lastName !== "" &&
        //    (noPhoneNumberAvailable === true || (noPhoneNumberAvailable === false) && phoneNumber !== "")) {
        //    window.open("http://uii/Global Manager/CopyToContext?InteractionCanProceed=true");
        //}
        //}
    }
    catch (e) {
        //Do not raise event
    }
}

Va.Udo.Crm.Scripts.Interaction.AppendIfNotEmpty = function (param, arg) {
    var concatString = "";
    if (arg != null && arg !== "")
        concatString = param + arg;
    return concatString;
}

Va.Udo.Crm.Scripts.Interaction.CheckPhoneForInitiateRequest = function () {
    var noPhoneNumberAvailableAttribute = Xrm.Page.getAttribute(Va.Udo.Crm.Scripts.Interaction.Attributes.NoPhoneNumberAvailable);
    var phoneNumberAttribute = Xrm.Page.getAttribute(Va.Udo.Crm.Scripts.Interaction.Attributes.PhoneNumber);

    var firstName = Xrm.Page.getAttribute("udo_firstname").getValue();
    var lastName = Xrm.Page.getAttribute("udo_lastname").getValue();

    if (firstName == null && lastName == null)
        return;

    if (noPhoneNumberAvailableAttribute != null && phoneNumberAttribute != null) {

        if (noPhoneNumberAvailableAttribute.getValue() == 0 && phoneNumberAttribute.getValue() == null)
            Xrm.Page.ui.setFormNotification("Interaction has no phone #; cannot initiate a new request. Please enter a phone # or click No Phone # Available and save Interaction before clicking Initiate a New Request.", "WARNING", "nophonemessage");
        else
            Xrm.Page.ui.clearFormNotification("nophonemessage")
    }
}

Va.Udo.Crm.Scripts.Interaction.NoPhoneNumberAvailableonChange = function () {
    var noPhoneNumberAvailableAttribute = Xrm.Page.getAttribute(Va.Udo.Crm.Scripts.Interaction.Attributes.NoPhoneNumberAvailable);
    var phoneNumberAttribute = Xrm.Page.getAttribute(Va.Udo.Crm.Scripts.Interaction.Attributes.PhoneNumber);
    if (noPhoneNumberAvailableAttribute != null && phoneNumberAttribute != null) {
        if (noPhoneNumberAvailableAttribute.getValue() == 1) {
            phoneNumberAttribute.setRequiredLevel("none");
        }
        else {
            phoneNumberAttribute.setRequiredLevel("required");
        }
    }
}

Va.Udo.Crm.Scripts.Interaction.PhoneNumberonChange = function () {
    try {
        var phoneAttribute = Xrm.Page.getAttribute(Va.Udo.Crm.Scripts.Interaction.Attributes.PhoneNumber);
        if (phoneAttribute != null) {
            var phone = phoneAttribute.getValue();
            var result = Va.Udo.Crm.Scripts.Utility.formatTelephone(phone);
            phoneAttribute.setValue(result);
        }
    }
    catch (e) {
        //Do not raise event
    }
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
