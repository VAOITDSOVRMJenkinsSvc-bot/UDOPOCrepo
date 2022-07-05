"use strict";

var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
Va.Udo.Crm.Scripts.Interaction = Va.Udo.Crm.Scripts.Interaction || {};

Va.Udo.Crm.Scripts.Interaction.Attributes = Va.Udo.Crm.Scripts.Interaction.Attributes || {};
Va.Udo.Crm.Scripts.Interaction.Attributes.NoPhoneNumberAvailable = "udo_nophonenumberavailable";
Va.Udo.Crm.Scripts.Interaction.Attributes.PhoneNumber = "udo_phonenumber";
var formContext = null;
var globalCon = Xrm.Utility.getGlobalContext();
var version = globalCon.getVersion();
var optOutVet = false;//  opt-out value on vet profile
var securityPinVet = "";
var optOutTry = 0;

Va.Udo.Crm.Scripts.Interaction.getOptOut = function (exCon) {

    formContext = exCon.getFormContext();
    try {
        var pin = formContext.getAttribute("udo_securitypin");
        if (pin != null) {
            var pinValue = pin.getValue();
            securityPinVet = pinValue;
            console.log("pin:" + pinValue);
        }
        else {
            console.log("pin null");
        }
        var opt = formContext.getAttribute("udo_optoutofvbatextsemails");
        if (opt != null) {
            var optValue = opt.getValue();
            optOutVet = optValue;
            console.log("opt:" + optValue);
        }
        else {
            console.log("opt false");
        }
        console.log("no more fetch");
    }
    catch (e) {
        console.log("I am here in error");
        console.log(e);
        //Do not raise event
    }

}
Va.Udo.Crm.Scripts.Interaction.onLoad = function (exCon) {
    formContext = exCon.getFormContext();
    try {
        formContext = exCon.getFormContext();

        if ((parent.window.IsUSD === true) || (window.IsUSD === true)) {
            formContext.getAttribute('udo_firstname').addOnChange(Va.Udo.Crm.Scripts.Interaction.FirstNameChange);
            formContext.getAttribute('udo_lastname').addOnChange(Va.Udo.Crm.Scripts.Interaction.LastNameChange);
            formContext.getAttribute('udo_phonenumber').addOnChange(Va.Udo.Crm.Scripts.Interaction.PhoneNumberChange);

            // Omnichannel
            formContext.getAttribute('udo_channel').addOnChange(Va.Udo.Crm.Scripts.Interaction.Channel);
            formContext.getAttribute('udo_interactiontype').addOnChange(Va.Udo.Crm.Scripts.Interaction.InteractionType);
            formContext.getAttribute('udo_abletoservicechatrequest').addOnChange(Va.Udo.Crm.Scripts.Interaction.AbleToServiceChatRequest);

            Va.Udo.Crm.Scripts.Interaction.FirstNameChange(exCon);
            Va.Udo.Crm.Scripts.Interaction.LastNameChange(exCon);
            Va.Udo.Crm.Scripts.Interaction.PhoneNumberChange(exCon);
        }

        try {
            var createdBy = formContext.getControl("createdby");
            var udCreatedBy = formContext.getControl("udo_udcreatedby");
            var udCreatedByAttr = formContext.getAttribute("udo_udcreatedby");
            var addlDetailsTab = formContext.ui.tabs.get("AdditionalDetailsTab");
            if (udCreatedByAttr.getValue()) {
                createdBy.setVisible(false);
                udCreatedBy.setVisible(true);
            }
            else {
                createdBy.setVisible(true);
                udCreatedBy.setVisible(false);
            }
            addlDetailsTab.setDisplayState("collapsed");

            formContext.getAttribute("udo_firstname").setSubmitMode("always");
            formContext.getAttribute("udo_lastname").setSubmitMode("always");
            formContext.getAttribute("udo_phonenumber").setSubmitMode("always");
            formContext.getAttribute("udo_nophonenumberavailable").setSubmitMode("always");

            var channel = formContext.getAttribute("udo_channel").getValue();
            if (channel === 752280002 || channel === 752280003) {
                formContext.getControl("udo_veteranfirstname").setVisible(true);
                formContext.getControl("udo_veteranmiddlename").setVisible(true);
                formContext.getControl("udo_veteranlastname").setVisible(true);
                formContext.getControl("udo_veterandob").setVisible(true);
                formContext.getControl("udo_veteranssn").setVisible(true);
                formContext.getControl("udo_purposeofthevisit").setVisible(true);
                formContext.getControl("udo_hasspecialsituations").setVisible(true);

                formContext.ui.tabs.get("detailsTab").sections.get("udo_special_situations").setVisible(true);
            }

            // Omnichannel
            Va.Udo.Crm.Scripts.Interaction.SetOmnichannelFields(exCon);

            Va.Udo.Crm.Scripts.Interaction.CheckPhoneForInitiateRequest();
            Va.Udo.Crm.Scripts.Interaction.getOptOut(exCon);

            formContext.data.save(saveOptions).then(
                function () {
                    //formContext.data.refresh();
                    console.log("Success on save")
                },
                function () {
                    console.log("Error on save")
                });
            console.log("just saved form in onload");
        }
        catch (e) {
            //Any error handling
        }
    }
    catch (e) {
        //Any error handling
    }
}

Va.Udo.Crm.Scripts.Interaction.Channel = function(exCon) {
    Va.Udo.Crm.Scripts.Interaction.SetOmnichannelFields(exCon);
}

Va.Udo.Crm.Scripts.Interaction.InteractionType = function(exCon) {
    Va.Udo.Crm.Scripts.Interaction.SetOmnichannelFields(exCon);    
}

Va.Udo.Crm.Scripts.Interaction.AbleToServiceChatRequest = function(exCon) {
    Va.Udo.Crm.Scripts.Interaction.SetOmnichannelFields(exCon);    
}

Va.Udo.Crm.Scripts.Interaction.SetOmnichannelFields = function (exCon) {
    var udo_channel = formContext.getAttribute("udo_channel").getValue();            
    var udo_interactiontype = formContext.getAttribute("udo_interactiontype").getValue();
    if ((udo_channel === 752280001) && (udo_interactiontype === 751880001)) {
        formContext.getControl("udo_abletoservicechatrequest").setVisible(true);
        formContext.getControl("udo_msdyn_ocliveworkitem").setVisible(true);

        var udo_abletoservicechatrequest = formContext.getAttribute("udo_abletoservicechatrequest").getValue();
        if (udo_abletoservicechatrequest === 751880001) {
            formContext.getControl("udo_chatservicereason").setVisible(true);
        } else {
            formContext.getControl("udo_chatservicereason").setVisible(false);
        }

    } else {
        formContext.getControl("udo_abletoservicechatrequest").setVisible(false);
        formContext.getControl("udo_chatservicereason").setVisible(false);
        formContext.getControl("udo_msdyn_ocliveworkitem").setVisible(false);
    }
}

Va.Udo.Crm.Scripts.Interaction.onSave = function (exCon) {
    try {
        console.log("starting onsave");
        formContext = exCon.getFormContext();
        Va.Udo.Crm.Scripts.Interaction.CheckPhoneForInitiateRequest();
        //if (window.isUSD === true) {
        var firstName = formContext.getAttribute("udo_firstname").getValue();
        firstName = firstName === null ? "" : firstName;
        var lastName = formContext.getAttribute("udo_lastname").getValue();
        lastName = lastName === null ? "" : lastName;
        var phoneNumber = formContext.getAttribute("udo_phonenumber").getValue();
        phoneNumber = phoneNumber === null ? "" : phoneNumber;
        var noPhoneNumberAvailable = formContext.getAttribute("udo_nophonenumberavailable").getValue();
        noPhoneNumberAvailable = noPhoneNumberAvailable === null ? "" : noPhoneNumberAvailable;
        var isCallerAuthorized = formContext.getAttribute("udo_iscallerauthorized").getValue();
        isCallerAuthorized = isCallerAuthorized === null ? "" : isCallerAuthorized;
        var eventString = Va.Udo.Crm.Scripts.Interaction.AppendIfNotEmpty("&firstName=", firstName) +
            Va.Udo.Crm.Scripts.Interaction.AppendIfNotEmpty("&lastName=", lastName) +
            Va.Udo.Crm.Scripts.Interaction.AppendIfNotEmpty("&phoneNumber=", phoneNumber) +
            Va.Udo.Crm.Scripts.Interaction.AppendIfNotEmpty("&noPhoneNumberAvailable=", noPhoneNumberAvailable) +
            Va.Udo.Crm.Scripts.Interaction.AppendIfNotEmpty("&isCallerAuthorized=", isCallerAuthorized);

        if (parent.window.IsUSD === true) {
            //window.open("http://event/?eventName=InteractionOnSave" + eventString);
        }

        //  save changes to opt-out
        var optOut = formContext.getAttribute("udo_optoutofvbatextsemails").getValue();
        var pin = formContext.getAttribute("udo_securitypin").getValue();
        var updateContact = false;
        if (optOut !== optOutVet) {
            console.log("update opt:" + optOut);
            updateContact = true;
        }
        if (pin !== null && pin !== securityPinVet) {
            updateContact = true;
            console.log("update pin:" + pin);
        }
        if (updateContact) {
            //var vet = formContext.getAttribute("udo_contactid").getValue();
            var vet;
            var intID = formContext.data.entity.getId().replace('{', '').replace('}', '');

            var fetchXML = "<fetch>" +
                "<entity name='contact' >" +
                "<attribute name='udo_optoutvbatextemail' />" +
                "<attribute name='udo_optoutlastupdated' />" +
                "<attribute name='lastname' />" +
                "<link-entity name='udo_idproof' from='udo_veteran' to='contactid' link-type='inner' >" +
                "<filter type='and' >" +
                "<condition attribute='udo_interaction' operator='eq' value='" + intID + "' />" +
                "</filter>" +
                "</link-entity>" +
                "</entity>" +
                "</fetch>";
            console.log("fetchXML:" + fetchXML);
            fetchXML = "?fetchXml=" + encodeURIComponent(fetchXML);

            Xrm.WebApi.retrieveMultipleRecords("contact", fetchXML).then(
                function success(result) {
                    // console.log("I am here");
                    if (result.entities[0].contactid !== null) {
                        vet = result.entities[0].contactid;
                        var vetId = vet;
                        var vetUpdate = {
                            udo_optoutvbatextemail: optOut,
                            udo_optoutlastupdated: new Date().toISOString(),
                            udo_securitypin: pin,
                            udo_securitypinlastupdated: new Date().toISOString()
                        };
                        optOutVet = optOut;
                        securityPinVet = pin;
                        console.log("updated opt and pin:" + optOut + ":" + securityPinVet);
                        Xrm.WebApi.updateRecord("contact", vetId, vetUpdate).then(
                            function (response) {
                                //  reset opt out var
                                optOutVet = optOut;
                            }
                        );
                    }
                },
                function (error) {
                    console.log("I am here in error");
                }
            );

        }
        console.log("onsave COMPLETED");
        Va.Udo.Crm.Scripts.Interaction.getOptOut(exCon);
    }
    catch (e) {
        //Do not raise event
    }
}

Va.Udo.Crm.Scripts.Interaction.PhoneNumberChange = function (executionContext) {
    var formContext = executionContext.getFormContext();
    if ((parent.window.IsUSD === true) || (window.IsUSD === true)) {
        var va_phonenumber = formContext.getAttribute("udo_phonenumber");
        if (va_phonenumber !== null) {
            var phonenumber = va_phonenumber.getValue();
            if (phonenumber === null) {
                phonenumber = "";
            }
            window.open("http://uii/Global Manager/CopyToContext?phonenumber=" + phonenumber);
            //window.open("http://event/?EventName=EmergencySetPhoneNumberToContext&phonenumber=" + phonenumber);
            window.open("http://event/?EventName=EmergencySetPhoneNumberToContext");
        }
    }
}

Va.Udo.Crm.Scripts.Interaction.FirstNameChange = function (executionContext) {
    var formContext = executionContext.getFormContext();
    if ((parent.window.IsUSD === true) || (window.IsUSD === true)) {
        var va_firstname = formContext.getAttribute("udo_firstname");
        if (va_firstname !== null) {
            var firstname = va_firstname.getValue();
            if (firstname === null) {
                firstname = "";
            }
            window.open("http://uii/Global Manager/CopyToContext?firstname=" + firstname);
            //window.open("http://event/?EventName=EmergencySetFirstNameToContext&firstname=" + firstname);
            window.open("http://event/?EventName=EmergencySetFirstNameToContext");
        }
    }
}

Va.Udo.Crm.Scripts.Interaction.LastNameChange = function (executionContext) {
    var formContext = executionContext.getFormContext();
    if ((parent.window.IsUSD === true) || (window.IsUSD === true)) {
        var va_lastname = formContext.getAttribute("udo_lastname");
        if (va_lastname !== null) {
            var lastname = va_lastname.getValue();
            if (lastname === null) {
                lastname = "";
            }
            window.open("http://uii/Global Manager/CopyToContext?lastname=" + lastname);
            //window.open("http://event/?EventName=EmergencySetLastNameToContext&lastname=" + lastname);
            window.open("http://event/?EventName=EmergencySetLastNameToContext");
        }
    }
}

Va.Udo.Crm.Scripts.Interaction.AppendIfNotEmpty = function (param, arg) {
    var concatString = "";
    if (arg !== null && arg !== "")
        concatString = param + arg;
    return concatString;
}

Va.Udo.Crm.Scripts.Interaction.CheckPhoneForInitiateRequest = function () {
    var noPhoneNumberAvailableAttribute = formContext.getAttribute(Va.Udo.Crm.Scripts.Interaction.Attributes.NoPhoneNumberAvailable);
    var phoneNumberAttribute = formContext.getAttribute(Va.Udo.Crm.Scripts.Interaction.Attributes.PhoneNumber);

    var firstName = formContext.getAttribute("udo_firstname").getValue();
    var lastName = formContext.getAttribute("udo_lastname").getValue();

    if (firstName === null && lastName === null)
        return;

    if (noPhoneNumberAvailableAttribute !== null && phoneNumberAttribute !== null) {

        if (noPhoneNumberAvailableAttribute.getValue() === 0 && phoneNumberAttribute.getValue() === null)
            formContext.ui.setFormNotification("Interaction has no phone #; cannot initiate a new request. Please enter a phone # or click No Phone # Available and save Interaction before clicking Initiate a New Request.", "WARNING", "nophonemessage");
        else
            formContext.ui.clearFormNotification("nophonemessage")
    }
}

Va.Udo.Crm.Scripts.Interaction.NoPhoneNumberAvailableonChange = function () {
    var noPhoneNumberAvailableAttribute = formContext.getAttribute(Va.Udo.Crm.Scripts.Interaction.Attributes.NoPhoneNumberAvailable);
    var phoneNumberAttribute = formContext.getAttribute(Va.Udo.Crm.Scripts.Interaction.Attributes.PhoneNumber);
    if (noPhoneNumberAvailableAttribute !== null && phoneNumberAttribute !== null) {
        if (noPhoneNumberAvailableAttribute.getValue() === 1) {
            phoneNumberAttribute.setRequiredLevel("none");
        }
        else {
            phoneNumberAttribute.setRequiredLevel("required");
        }
    }
}

Va.Udo.Crm.Scripts.Interaction.PhoneNumberonChange = function () {
    try {
        var phoneAttribute = formContext.getAttribute(Va.Udo.Crm.Scripts.Interaction.Attributes.PhoneNumber);
        if (phoneAttribute !== null) {
            var phone = phoneAttribute.getValue();
            var result = Va.Udo.Crm.Scripts.Utility.formatTelephone(phone);
            phoneAttribute.setValue(result);
        }
    }
    catch (e) {
        //Do not raise event
    }
}

Va.Udo.Crm.Scripts.PopulateOptOut = function (vetId) {
    console.log("PopulateOptOut was called - COMMENTED OUT");

    //Xrm.WebApi.retrieveRecord("contact", vetId, "?$select=udo_optoutvbatextemail,udo_securitypin").then(
    //    function success(result) {
    //        if (result.udo_optoutvbatextemail !== null) {
    //            optOutVet = result.udo_optoutvbatextemail;
    //            formContext.getAttribute("udo_optoutofvbatextsemails").setValue(optOutVet);
    //            console.log("oPT NOW UPDATED:" + optOutVet);

    //            //formContext.data.save();
    //        }
    //        if (result.udo_securitypin === null || result.udo_securitypin === undefined) {

    //        }
    //        else {
    //            securityPinVet = result.udo_securitypin;
    //            formContext.getAttribute("udo_securitypin").setValue(securityPinVet)
    //            console.log("pin NOW UPDATED:" + securityPinVet);
    //        }
    //        formContext.data.save();
    //    },
    //    function (error) {
    //    }
    //);
}

Va.Udo.Crm.Scripts.Interaction.UpdateCallerDetails = function (context, CallerPhoneNumber, CallerFirstName, CallerLastName, VeteranId) {
    try {
        Xrm.WebApi.retrieveRecord("contact", VeteranId, "?$select=udo_optoutvbatextemail,udo_securitypin").then(
            function success(result) {
                if (result.udo_optoutvbatextemail !== null) {
                    optOutVet = result.udo_optoutvbatextemail;
                    formContext.getAttribute("udo_optoutofvbatextsemails").setValue(optOutVet);
                    console.log("oPT NOW UPDATED:" + optOutVet);
                }
                if (result.udo_securitypin !== null || result.udo_securitypin !== undefined) {
                    securityPinVet = result.udo_securitypin;
                    formContext.getAttribute("udo_securitypin").setValue(securityPinVet)
                    console.log("pin NOW UPDATED:" + securityPinVet);
                }

                context.getAttribute("udo_relationship").setValue(752280000);
                context.getAttribute("udo_firstname").setValue(CallerFirstName);
                context.getAttribute("udo_lastname").setValue(CallerLastName);

                console.log("Caller details updated on Interaction.");

                if (CallerPhoneNumber !== "") {
                    context.getAttribute("udo_phonenumber").setValue(CallerPhoneNumber);
                    context.getAttribute("udo_nophonenumberavailable").setValue(false);
                }

                context.ui.clearFormNotification("PhoneNumberError");

                var phoneNumber = context.getAttribute("udo_phonenumber").getValue();
                if (phoneNumber === null) {
                    var attribute = context.getAttribute("udo_nophonenumberavailable").getValue();
                    if (attribute === false) {
                        context.ui.setFormNotification("Interaction has no phone #; cannot initiate a new request. Please enter a phone # or click No Phone # Available and save Interaction before clicking Initiate a New Request.", "WARNING", "nophonemessage");
                    }
                }

                //console.log("VeteranId:" +VeteranId);
                if (VeteranId !== null) {
                    Va.Udo.Crm.Scripts.PopulateOptOut(VeteranId);
                }
            },
            function (error) {
                alert("An error has occurred retrieving contact on updating interaction: " + error);
            });
    }
    catch (e) {
        if (parent.window.IsUSD === true) {
            window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
        }
        UDO.Shared.openAlertDialog("Could not update caller details on the Interaction.\nTry editing the Interaction fields and saving directly.", "", 120, 260);
    }
}

Va.Udo.Crm.Scripts.Interaction.ShowTabFromConfigUCI = function (context, selectedRequestTypeName, selectedRequestSubTypeName, fetchUdoRequestTypeUdoConfiguration) {
    if (selectedRequestTypeName === "FNOD" && selectedRequestSubTypeName === "Death of a Veteran") {
        if (parent.window.IsUSD === true) {
            window.open("http://uii/Global Manager/ShowTab?FNODAutoOpenHostedControl");
        }
    } else if (selectedRequestTypeName === "Dependent Maintenance" && selectedRequestSubTypeName === "Dependent Verification") {
        if (parent.window.IsUSD === true) {
            window.open("http://uii/Global Manager/ShowTab?DEPENDENT VERIFICATION People List");
        }
    } else if (selectedRequestTypeName === "Dependent Maintenance" && selectedRequestSubTypeName !== "Dependent Verification") {

        var jsonString = "{    pages: [],    showTab: 'DepMaintenance'}";
        if (jsonString !== "") {
            var configObj = eval("(" + jsonString + ")");
            var relName = configObj.showTab;
            if (parent.window.IsUSD === true) {
                window.open("http://uii/Global Manager/ShowTab?" + relName);
            }
        }

    } else {
        var jsonString = fetchUdoRequestTypeUdoConfiguration;
        if (jsonString !== "") {
            var configObj = eval("(" + jsonString + ")");
            var relName = configObj.showTab;
            if (parent.window.IsUSD === true) {
                window.open("http://uii/Global Manager/ShowTab?" + relName);
            }
        }
    }
}

Va.Udo.Crm.Scripts.Interaction.checkForCallerDetails = function (context) {
    try {

        var attributeAuthorized = context.getAttribute("udo_iscallerauthorized");
        attributeAuthorized.setValue(true);

        var errorMessage = "Please address the following issues before you proceed:";
        var error = false;

        var LastName = context.getAttribute("udo_lastname");
        if (LastName !== null) {
            if (LastName.getValue() === null) {
                errorMessage += "\nEnter Last Name.";
                error = true;
            }
        } else {
            errorMessage += "\nEnter Last Name.";
            error = true;
        }

        var phoneNumber = context.getAttribute("udo_phonenumber");
        if (phoneNumber !== null) {
            if (phoneNumber.getValue() === null) {
                var NoPhoneNumberAvailable = context.getAttribute("udo_nophonenumberavailable");
                if (NoPhoneNumberAvailable !== null) {
                    if (NoPhoneNumberAvailable.getValue() === false) {
                        errorMessage += "\nEnter a phone number or set the No Phone # Avail checkbox to Yes.";
                        error = true;
                    }
                } else {
                    errorMessage += "\nEnter a phone number or set the No Phone # Avail checkbox to Yes.";
                    error = true;
                }
            }
        } else {
            var NoPhoneNumberAvailable = context.getAttribute("udo_nophonenumberavailable");
            if (NoPhoneNumberAvailable !== null) {
                if (NoPhoneNumberAvailable.getValue() === false) {
                    errorMessage += "\nEnter a phone number or set the No Phone # Avail checkbox to Yes.";
                    error = true;
                }
            } else {
                errorMessage += "\nEnter a phone number or set the No Phone # Avail checkbox to Yes.";
                error = true;
            }
        }

        if (error) {
            if (parent.window.IsUSD === true) {
                window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
            }

            UDO.Shared.openAlertDialog(errorMessage, "Interaction Update:", 200, 350);
        }
        else {
            if (parent.window.IsUSD === true) {

                window.open("http://uii/Global Manager/CopyToContext?firstName=" + context.getAttribute("udo_firstname").getValue());
                window.open("http://uii/Global Manager/CopyToContext?lastName=" + context.getAttribute("udo_lastname").getValue());
                window.open("http://uii/Global Manager/CopyToContext?phoneNumber=" + context.getAttribute("udo_phonenumber").getValue());
                window.open("http://uii/Global Manager/CopyToContext?noPhoneNumberAvailable=" + context.getAttribute("udo_nophonenumberavailable").getValue());
                window.open("http://uii/Global Manager/CopyToContext?isCallerAuthorized=true");

                window.open("http://uii/Global Manager/CopyToContext?callerfirstname=" + context.getAttribute("udo_firstname").getValue());
                window.open("http://uii/Global Manager/CopyToContext?callerlastname=" + context.getAttribute("udo_lastname").getValue());
                window.open("http://uii/Global Manager/CopyToContext?callerphonenumber=" + context.getAttribute("udo_phonenumber").getValue());
                window.open("http://uii/Global Manager/CopyToContext?callernophonenumberavailable=" + context.getAttribute("udo_nophonenumberavailable").getValue());
        
                window.open("http://event/?EventName=SetCallerAndCreateRequest");

                //window.open("http://event/?EventName=SetCallerAndCreateRequest&firstName=" +
                //    context.getAttribute("udo_firstname") + "&lastName=" +
                //    context.getAttribute("udo_lastname") + "&phoneNumber=" +
                //    context.getAttribute("udo_phonenumber") + "&noPhoneNumberAvailable=" +
                //    context.getAttribute("udo_nophonenumberavailable") + "&isCallerAuthorized=true");

            }
        }
    }
    catch (e) {
        if (parent.window.IsUSD === true) {
            window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
        }

        UDO.Shared.openAlertDialog(e.message, "Interaction Update Error:", 200, 350);
    }
    return error.toString();
}

Va.Udo.Crm.Scripts.Interaction.USD_Save = function(context) {
    context.data.save().then(function () {
        window.open("http://uii/Global Manager/CopyToContext?callerfirstname=" + context.getAttribute("udo_firstname").getValue());
        window.open("http://uii/Global Manager/CopyToContext?callerlastname=" + context.getAttribute("udo_lastname").getValue());
        window.open("http://uii/Global Manager/CopyToContext?callerphonenumber=" + context.getAttribute("udo_phonenumber").getValue());
        window.open("http://uii/Global Manager/CopyToContext?callernophonenumberavailable=" + context.getAttribute("udo_nophonenumberavailable").getValue());
    }, function () {
        UDO.Shared.openAlertDialog("Save failed. Please try again. ");
    });    
}

Va.Udo.Crm.Scripts.Interaction.callCheckForCallerDetails = function (context) {
    if (!context.data.entity.getIsDirty()) {
        Va.Udo.Crm.Scripts.Interaction.checkForCallerDetails(context);
    }
    else {
        context.data.save().then(
            function () {
                Va.Udo.Crm.Scripts.Interaction.checkForCallerDetails(context);
            },
            function (error) {
                UDO.Shared.openAlertDialog("Save failed. " + error.message, "", 120, 260);
            }
        );
    }
}

Va.Udo.Crm.Scripts.Interaction.CheckForSessionClose = function (context) {
    try {

        if (confirm("Are you sure you want to close the current call session?")) {
            return 'true';
        }
        else {
            return 'false';
        }
    }
    catch (e) {
        UDO.Shared.openAlertDialog("An unexpected error occurred - " + e.message, "", 120, 260);
        return 'false';
    }
}

Va.Udo.Crm.Scripts.Interaction.InitiateSessionCloseOut = function (context, RequestId, CADDFormIsDirty) {
    if (frames.customScriptsFrame) {
        frames.customScriptsFrame.eval("Va.Udo.Crm.Scripts.Interaction.InitiateSessionCloseOut(" + RequestId + "," + CADDFormIsDirty + ")");
    }
}

Va.Udo.Crm.Scripts.Interaction.KeyboardShorcutSetFocusOnTitle = function (context, SetFocusScript) {
    SetFocusScript;
    setTimeout('SetFocus("udo_title")', 1000);
}

Va.Udo.Crm.Scripts.Interaction.OpenCallScriptPayment = function (context, PaymentId, FolderLocation, ReleasedActivityDutyDate) {
    if (PaymentId === "") {
        if (parent.window.IsUSD === true) {
            window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
        }
        UDO.Shared.openAlertDialog("Please select an payment first.", "Interaction Update", 200, 350);
    } else {
        var dataParams = "id=" + PaymentId;
        dataParams += "&roj=" + FolderLocation;
        dataParams += "&rad=" + ReleasedActivityDutyDate;
        var customParams = "?data=" + encodeURIComponent(dataParams);
        Xrm.Navigation.openWebResource("udo_payments_debtsDMC.html" + customParams);
    }
}

Va.Udo.Crm.Scripts.Interaction.OpenSmartScriptAppealScript = function (context, AppealId) {
    if (AppealId === "") {
        window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");

        UDO.Shared.openAlertDialog("Please select an appeal first.", "Interaction Update Error", 200, 350);

    } else {
        var dataParams = "id=" + AppealId;
        //dataParams += "&roj=" +"[[contact.udo_folderlocation]+]";
        //dataParams += "&rad=" + "[[contact.udo_releasedactivedutydate]+]";
        var customParams = "?data=" + encodeURIComponent(dataParams)
        Xrm.Navigation.openWebResource("udo_AppealSmart.html" + customParams);
    }
}

Va.Udo.Crm.Scripts.Interaction.ResizeScreen = function (context) {
    try {
        if (context.data.entity.getIsDirty() === false) {
            context.data.refresh();
        }
    }
    catch (e) {
        if (parent.window.IsUSD === true) {
            window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
        }
        UDO.Shared.openAlertDialog("Unable to refresh page data" + e.message, "", 200, 350);
    }
}

Va.Udo.Crm.Scripts.Interaction.SetAbusiveCallerOpen = function (context) {
    try {
        var isAbusiveCaller = context.getAttribute("udo_isabusivecaller");
        var abusiveCallerMessage = "";
        if (isAbusiveCaller.getValue() === false) {
            isAbusiveCaller.setValue(true);
            abusiveCallerMessage = "Abusive Caller flag is set to YES";
        }
        else {
            isAbusiveCaller.setValue(false);
            abusiveCallerMessage = "Abusive Caller flag is set to NO";
        }
        context.data.save().then(function () {
            if (parent.window.IsUSD === true) {
                window.open("http://event/?eventName=UnhideAndFocusInteraction");
            }
            UDO.Shared.openAlertDialog(abusiveCallerMessage);
        },
            function () {
                if (parent.window.IsUSD === true) {
                    window.open("http://event/?eventName=UnhideAndFocusInteraction");
                }
                UDO.Shared.openAlertDialog("Automation for abusive caller failed.");
            });
    }
    catch (e) {
        if (parent.window.IsUSD === true) {
            window.open("http://event/?eventName=UnhideAndFocusInteraction");
        }
        UDO.Shared.openAlertDialog("Automation for Abusive Caller failed.\n" + e.message);
    }
}

Va.Udo.Crm.Scripts.Interaction.SetCallerAsUnauthorized = function (context) {
    try {

        var attribute = context.getAttribute("udo_iscallerauthorized");
        attribute.setValue(false);
        context.data.save();
    }
    catch (e) {
        if (parent.window.IsUSD === true) {
            window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
        }
        UDO.Shared.openAlertDialog("Could not set caller as unauthorized.\n" + e.message, "Interaction Update", 200, 350);
    }
}

Va.Udo.Crm.Scripts.Interaction.SetCallerDisconnected = function (context) {
    try {
        var isCallerDisconnected = context.getAttribute("udo_callerdisconnected");
        var callerDisconnectedMessage = "";
        if (isCallerDisconnected.getValue() === false) {
            isCallerDisconnected.setValue(true);
            callerDisconnectedMessage = "Caller Disconnected flag is set to YES";
        }
        else {
            isCallerDisconnected.setValue(false);
            callerDisconnectedMessage = "Caller Disconnected flag is set to NO";
        }
        context.data.save().then(function () {
            if (parent.window.IsUSD === true) {
                window.open("http://event/?eventName=UnhideAndFocusInteraction");
            }
            UDO.Shared.openAlertDialog(callerDisconnectedMessage);
        }, function () {
            if (parent.window.IsUSD === true) {
                window.open("http://event/?eventName=UnhideAndFocusInteraction");
            }
            UDO.Shared.openAlertDialog("Automation to change the abusive caller failed. ");
        });
    }
    catch (e) {
        if (parent.window.IsUSD === true) {
            window.open("http://event/?eventName=UnhideAndFocusInteraction");
        }
        UDO.Shared.openAlertDialog("Automation to change the abusive caller failed.  \n" + e.message);
    }
}

Va.Udo.Crm.Scripts.Interaction.updateAttributePrivateAgent = function (context, attributeName, attributeValue) {
    try {
        var attribute = context.getAttribute(attributeName);
        attribute.setValue(attributeValue);
    }
    catch (e) {
        if (parent.window.IsUSD === true) {
            window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
        }

        UDO.Shared.openAlertDialog("Could not set the relationship to Private Agent.Try updating the relationship Interaction field and saving directly.", "Interaction Update Error", 200, 350);
    }
}

Va.Udo.Crm.Scripts.Interaction.SetCallerRelationshipAsPrivateAgent = function (context) {
    try {
        var attribute = context.getAttribute("udo_relationship");
        attribute.setValue(752280010);
        context.data.save();
    }
    catch (e) {
        if (parent.window.IsUSD === true) {
            window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
        }

        UDO.Shared.openAlertDialog("Could not set the relationship to Private Agent.Try updating the relationship Interaction field and saving directly.", "Interaction Update Error", 200, 350);
    }
}

Va.Udo.Crm.Scripts.Interaction.SetCallerRelationshipAsPrivateAttorney = function (context) {
    try {
        var attribute = context.getAttribute("udo_relationship");
        attribute.setValue(752280009);
        context.data.save();
    }
    catch (e) {
        if (parent.window.IsUSD === true) {
            window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
        }
        UDO.Shared.openAlertDialog("Could not set the relationship to Private Attorney.Try updating the relationship Interaction field and saving directly.", "Interaction Update Error", 200, 350);
    }
}

Va.Udo.Crm.Scripts.Interaction.SetPCRDisconnectsCall = function (context) {
    try {
        var isPcrDisconnected = context.getAttribute("udo_pcrdisconnectedcall");
        var pcrDisconnectedMessage = "";
        if (isPcrDisconnected.getValue() === false) {
            isPcrDisconnected.setValue(true);
            pcrDisconnectedMessage = "PCR Disconnected flag is set to YES";
        }
        else {
            isPcrDisconnected.setValue(false);
            pcrDisconnectedMessage = "PCR Disconnected flag is set to NO";
        }
        context.data.save().then(function () {
            if (parent.window.IsUSD === true) {
                window.open("http://event/?eventName=UnhideAndFocusInteraction");
            }
            UDO.Shared.openAlertDialog(pcrDisconnectedMessage);
        }, function () {
            if (parent.window.IsUSD === true) {
                window.open("http://event/?eventName=UnhideAndFocusInteraction");
            }
            UDO.Shared.openAlertDialog("Automation to change the abusive caller failed. ");
        });
    }
    catch (e) {
        if (parent.window.IsUSD === true) {
            window.open("http://event/?eventName=UnhideAndFocusInteraction");
        }
        UDO.Shared.openAlertDialog("Automation failed: Caller Disconnected Call. \n" + e.message);
    }
}

Va.Udo.Crm.Scripts.Interaction.ShowIDCallScriptWindow = function (context) {
    frames.customScriptsFrame.eval("Va.Udo.Crm.Scripts.ScriptControl.ShowScript('Scripts_CallerIDScript',true)");
}

Va.Udo.Crm.Scripts.Interaction.ShowCallScript = function (context, SelectedRequestTypeName, ClaimId, AppealId, PaymentId, FolderLocation, ReleasedActiveDutyDate, SelectedRequestTypeId) {
    var idStr = "";
    var SwitchName = SelectedRequestTypeName;
    var haveScript = false;
    var extraData = null;

    if (SwitchName === "") {
        UDO.Shared.openAlertDialog("Please select a request and request subtype first.", "", 120, 260);
    } else {
        switch (SwitchName.toLowerCase()) {
            case "claim":
                if (ClaimId === "") {
                    UDO.Shared.openAlertDialog("Please select a claim first.", "", 120, 260);
                    haveScript = false;
                } else {
                    idStr = ClaimId;
                    haveScript = true;
                }
                break;
            case "appeals":
                if (AppealId === "") {
                    UDO.Shared.openAlertDialog("Please select an appeal first.", "", 120, 260);
                    haveScript = false;
                } else {
                    idStr = AppealId;
                    haveScript = true;
                }
                break;
            case "payments / debts":
            case "payments/debts":
                if (PaymentId === "") {
                    UDO.Shared.openAlertDialog("Please select an payment first.", "", 120, 260);
                    haveScript = false;
                } else {
                    idStr = PaymentId;
                    extraData = 'roj=' + FolderLocation;
                    extraData += "&rad=" + ReleasedActiveDutyDate;
                    haveScript = true;
                }
                break;

            case "dependent maintenance":
            case "correspondence and forms":
            case "corespondence and forms": // there is a typo in the name
            case "ebenefits":  //CSS bug in this one to track down  
            case "email forms":
            case "fnod":
            case "foia/privacy act":
            case "general benefits information for vba":
            case "general benefits information for vha":
            case "general benefit information for nca":
            case "general request for va phone number/address/fax":
            case "ghost call/disconnected call":
            case "media inquiries":
            case "medical":
            case "non va calls":
            case "other benefits - comp or pension":
            case "other va business lines":
            case "sensitive file":
            case "sep/vso":
            case "special issues":
            case "suicide call":
            case "threat call":
            case "update information":
            case "va media campaign 2013 - online":
            case "va media campaign 2013 - radio":
            case "va media campaign 2013 - tv":
            case "va media campaign 2013 - social media":
            case "fiduciary":
                idStr = "";
                haveScript = true;
                break;
            default:
                idStr = null;
                haveScript = false;
                UDO.Shared.openAlertDialog("There are no scripts defined for this request type.", "", 120, 260);
        }
    }

    if (haveScript) Va.Udo.Crm.Scripts.ScriptControl.OpenCallScript(SelectedRequestTypeId, idStr, extraData);
}

Va.Udo.Crm.Scripts.Interaction.UpdateRelationshipAsDependent = function (context) {
    try {

        var attribute = context.getAttribute("udo_relationship");
        attribute.setValue(752280001);
        context.data.save();
    }
    catch (e) {
        if (parent.window.IsUSD === true) {
            window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
        }
        UDO.Shared.openAlertDialog("Could not set the relationship to Dependent.\nTry updating the relationship Interaction field and saving directly.", "Interaction Update Error", 200, 350);
    }
}

Va.Udo.Crm.Scripts.Interaction.UpdateRelationShipAsFiduciary = function (context) {
    try {

        var attribute = context.getAttribute("udo_relationship");
        attribute.setValue(752280004);
        context.data.save();
    }
    catch (e) {
        if (parent.window.IsUSD === true) {
            window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
        }
        UDO.Shared.openAlertDialog("Could not set the relationship to Fiduciary.\nTry updating the relationship Interaction field and saving directly.", "Interaction Update Error", 200, 350);
    }
}

Va.Udo.Crm.Scripts.Interaction.updateAttributeOther = function (context, attributeName, attributeValue) {
    try {
        var attribute = context.getAttribute(attributeName);
        attribute.setValue(attributeValue);
    }
    catch (e) {
        if (parent.window.IsUSD === true) {
            window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
        }
        UDO.Shared.openAlertDialog("Could not set the relationship to Other.\nTry updating the relationship Interaction field and saving directly.", "Interaction Update Error", 200, 350);
    }
}

Va.Udo.Crm.Scripts.Interaction.UpdateRelationshipAsOther = function (context) {
    try {
        var attribute = context.getAttribute("udo_relationship");
        attribute.setValue(752280008);
        context.data.save();
    }
    catch (e) {
        if (parent.window.IsUSD === true) {
            window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
        }
        UDO.Shared.openAlertDialog("Could not set the relationship to Other.\nTry updating the relationship Interaction field and saving directly.", "Interaction Update Error", 200, 350);
    }
}

Va.Udo.Crm.Scripts.Interaction.UpdateRelationshipAsPOA = function (context) {
    try {
        var attribute = context.getAttribute("udo_relationship");
        attribute.setValue(752280002);
        context.data.save();
    }
    catch (e) {
        if (parent.window.IsUSD === true) {
            window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
        }
        UDO.Shared.openAlertDialog("Could not set the relationship to POA.\nTry updating the relationship Interaction field and saving directly.", "Interaction Update Error", 200, 350);
    }
}

Va.Udo.Crm.Scripts.Interaction.updateAttributeRep = function (attributeName, attributeValue) {
    try {
        var attribute = context.getAttribute(attributeName);
        attribute.setValue(attributeValue);
    }
    catch (e) {
        UDO.Shared.openAlertDialog("Could not automate fields while updating caller details" + e.message, "", 120, 260);
    }
}

Va.Udo.Crm.Scripts.Interaction.UpdateRelationshipasRep = function (context) {
    try {
        var attribute = context.getAttribute("udo_relationship");
        attribute.setValue(752280007);
        context.data.save();
    }
    catch (e) {
        UDO.Shared.openAlertDialog("Could not automate fields while updating caller details" + e.message, "", 120, 260);
    }
}

Va.Udo.Crm.Scripts.Interaction.updateAttributeRelSpouse = function (attributeName, attributeValue, context) {
    try {
        var attribute = context.getAttribute(attributeName);
        attribute.setValue(attributeValue);
    }
    catch (e) {
        if (parent.window.IsUSD === true) {
            window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
        }
        UDO.Shared.openAlertDialog("Could not set the relationship to Spouse.Try updating the relationship Interaction field and saving directly.", "Interaction Update Error", 200, 350);
    }
}

Va.Udo.Crm.Scripts.Interaction.UpdateRelationshipAsSpouse = function (context) {
    try {
        var attribute = context.getAttribute("udo_relationship");
        attribute.setValue(752280005);
        context.data.save();
    }
    catch (e) {
        if (parent.window.IsUSD === true) {
            window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
        }
        UDO.Shared.openAlertDialog("Could not set the relationship to Spouse.Try updating the relationship Interaction field and saving directly.", "Interaction Update Error", 200, 350);
    }
}

Va.Udo.Crm.Scripts.Interaction.updateAttributeThirdParty = function (attributeName, attributeValue) {
    try {
        var attribute = context.getAttribute(attributeName);
        attribute.setValue(attributeValue);
    }
    catch (e) {
        UDO.Shared.openAlertDialog("Could not automate fields while updating caller details" + e.message, "", 120, 260);
    }
}

Va.Udo.Crm.Scripts.Interaction.UpdateRelationshipToThirdParty = function (context) {
    try {
        var attribute = context.getAttribute("udo_relationship");
        attribute.setValue(752280003);
        context.data.save();
    }
    catch (e) {
        UDO.Shared.openAlertDialog("Could not automate fields while updating caller details" + e.message, "", 120, 260);
    }
}

Va.Udo.Crm.Scripts.Interaction.updateAttributeUnknown = function (context, attributeName, attributeValue) {
    try {
        var attribute = context.getAttribute(attributeName);
        attribute.setValue(attributeValue);
    }
    catch (e) {
        UDO.Shared.openAlertDialog("Could not automate fields while updating caller details" + e.message, "", 120, 260);
    }
}

Va.Udo.Crm.Scripts.Interaction.UpdateRelationshiptoUnknown = function (context) {
    try {
        var attribute = context.getAttribute("udo_relationship");
        attribute.setValue(752280006);
        context.data.save();
    }
    catch (e) {
        UDO.Shared.openAlertDialog("Could not automate fields while updating caller details" + e.message, "", 120, 260);
    }
}

Va.Udo.Crm.Scripts.Interaction.SetCallAsVirtualCall = function (context) {
    try {
        var isVirtualCall = context.getAttribute("udo_virtualcall");
        var virtualCallMessage = "";
        if (isVirtualCall.getValue() === false) {
            isVirtualCall.setValue(true);
            virtualCallMessage = "The call is flagged as a Virtual Call.";
        }
        else {
            isVirtualCall.setValue(false);
            virtualCallMessage = "The call is NOT flagged as a Virtual Call.";
        }
        context.data.save().then(function () {
            if (parent.window.IsUSD === true) {
                window.open("http://event/?eventName=UnhideAndFocusInteraction");
            }
            UDO.Shared.openAlertDialog(virtualCallMessage);
        }, function () {
            if (parent.window.IsUSD === true) {
                window.open("http://event/?eventName=UnhideAndFocusInteraction");
            }
            UDO.Shared.openAlertDialog("Automation to change the virtual call failed. ");
        });
    }
    catch (e) {
        if (parent.window.IsUSD === true) {
            window.open("http://event/?eventName=UnhideAndFocusInteraction");
        }
        UDO.Shared.openAlertDialog("Automation to change the virtual call failed. \n " + e.message);
    }
}

Va.Udo.Crm.Scripts.Interaction.OpenCallScriptClaims = function (context, ClaimId) {
    if (ClaimId === "") {
        UDO.Shared.openAlertDialog("Please select a claim first.", "", 120, 260);
    } else {
        var customParams = "?data=id%3D" + ClaimId;
        Xrm.Navigation.openWebResource("udo_claimStatus.html" + customParams);
    }
}

Va.Udo.Crm.Scripts.Interaction.CloseSessionFromInteractionScreen = function (context, InteractionId, RequestId) {
    Va.Udo.Crm.Scripts.Interaction.InitiateSessionCloseOut(InteractionId, RequestId);
}

Va.Udo.Crm.Scripts.Interaction.InitiateSessionCloseOut = function (interactionId, requestId) {

    var UpdateFailed = false;

    var endTime = new Date();
    if (requestId !== "") {

        var entity = {};
        entity.udo_endtime = endTime;

        Xrm.WebApi.online.updateRecord("udo_request", requestId, entity).then(
            function success(result) {
                if (parent.window.IsUSD === true) {
                    window.open("http://uii/Session Controller/IncrementCloseSessionValidations");
                }
            },
            function (error) {
                UpdateFailed = true;
                if (parent.window.IsUSD === true) {
                    window.open("http://uii/Session Controller/IncrementCloseSessionFailures");
                    window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
                }
                UDO.Shared.openAlertDialog("There was an error while updating the Request before closing the Session.\n\nError: " + err.message);
            }
        );
    } else {
        // PASS
        if (parent.window.IsUSD === true) {
            window.open("http://uii/Session Controller/IncrementCloseSessionValidations");
        }
    }
    //Update Interaction end time
    if (interactionId !== "") {

        entity = {};
        entity.udo_endtime = endTime;
        entity.udo_status = false;

        Xrm.WebApi.online.updateRecord("udo_interaction", interactionId, entity).then(
            function success(result) {
                if (parent.window.IsUSD === true) {
                    window.open("http://uii/Session Controller/IncrementCloseSessionValidations");
                }
            },
            function (error) {

                UpdateFailed = true;
                if (parent.window.IsUSD === true) {
                    window.open("http://uii/Session Controller/IncrementCloseSessionFailures");
                    window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
                }
                UDO.Shared.openAlertDialog("There was an error while updating the Interaction before closing the Session.\n\nError:  " + err.message);
            }
        );
    } else {
        // PASS
        if (parent.window.IsUSD === true) {
            window.open("http://uii/Session Controller/IncrementCloseSessionValidations");
        }
    }

    if (UpdateFailed) {
        if (parent.window.IsUSD === true) {
            window.open("http://event/?eventname=SetSessionCloseToInvalid");
        }
    } else {
        if (parent.window.IsUSD === true) {
            window.open("http://event/?eventname=SetSessionCloseToValid");
        }
    }
}

Va.Udo.Crm.Scripts.Interaction.SessionCloseRequested = function (context, InteractionId, RequestId) {
    Va.Udo.Crm.Scripts.Interaction.InitiateSessionCloseOutCloseRequested(InteractionId, RequestId);
}

Va.Udo.Crm.Scripts.Interaction.InitiateSessionCloseOutCloseRequested = function (interactionId, requestId) {
    var endTime = new Date();
    if (requestId !== "") {

        var entity = {};
        entity.udo_endtime = endTime;

        Xrm.WebApi.online.updateRecord("udo_request", requestId, entity).then(
            function success(result) {
                if (parent.window.IsUSD === true) {
                    window.open("http://uii/Session Controller/IncrementCloseSessionValidations");
                }
            },
            function (error) {
                if (parent.window.IsUSD === true) {
                    window.open("http://uii/Session Controller/IncrementCloseSessionFailures");
                    window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
                }
                UDO.Shared.openAlertDialog("There was an error while updating the Request before closing the Session.\n\nError: " + xhr.responseText, "Request Update Error", 200, 350);
            }
        );
    } else {
        // PASS
        if (parent.window.IsUSD === true) {
            window.open("http://uii/Session Controller/IncrementCloseSessionValidations");
        }
    }
    //Update Interaction end time
    if (interactionId !== "") {

        var entity = {};
        entity.udo_endtime = endTime;
        entity.udo_status = false;

        Xrm.WebApi.online.updateRecord("udo_interaction", interactionId, entity).then(
            function success(result) {
                if (parent.window.IsUSD === true) {
                    window.open("http://uii/Session Controller/IncrementCloseSessionValidations");
                }
            },
            function (error) {
                if (parent.window.IsUSD === true) {
                    window.open("http://uii/Session Controller/IncrementCloseSessionFailures");
                    window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
                }
                // FAIL
                UDO.Shared.openAlertDialog("There was an error while updating the Interaction before closing the Session.\n\nError: " + statusCode + "\n\n" + xhr.responseText, "Interaction Update Error", 200, 350);
            }
        );
    } else {
        // PASS
        if (parent.window.IsUSD === true) {
            window.open("http://uii/Session Controller/IncrementCloseSessionValidations");
        }
    }
}

Va.Udo.Crm.Scripts.Interaction.IDProofCompleteUpdateCallerDetails = function (context, CallerPhoneNumber, CallerFirstName, CallerLastName) {
    try {
        context.getAttribute("udo_relationship").setValue(752280000);
        context.getAttribute("udo_firstname").setValue(CallerFirstName);
        context.getAttribute("udo_lastname").setValue(CallerLastName);
        if (CallerPhoneNumber !== "") {
            context.getAttribute("udo_phonenumber").setValue(CallerPhoneNumber);
            context.getAttribute("udo_nophonenumberavailable").setValue(false);
        }

        context.ui.clearFormNotification("PhoneNumberError");

        var phoneNumber = context.getAttribute("udo_phonenumber").getValue();
        if (phoneNumber === null) {
            var attribute = context.getAttribute("udo_nophonenumberavailable").getValue();
            if (attribute === false) {
                context.ui.setFormNotification("Interaction has no phone #; cannot initiate a new request. Please enter a phone # or click No Phone # Available and save Interaction before clicking Initiate a New Request.", "WARNING", "nophonemessage");
            }
        }
    }
    catch (e) {
        if (parent.window.IsUSD === true) {
            window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
        }
        UDO.Shared.openAlertDialog("Could not update caller details on the Interaction.\nTry editing the Interaction fields and saving directly.", "", 120, 260);
    }
}

Va.Udo.Crm.Scripts.Interaction.UpdateCallerDetails2 = function (context, CallerPhoneNumber, CallerFirstName, CallerLastName) {
    Va.Udo.Crm.Scripts.Interaction.updateAttributeCallerDetails(context, "udo_relationship", 752280000);
    Va.Udo.Crm.Scripts.Interaction.updateAttributeCallerDetails(context, "udo_firstname", CallerFirstName);
    Va.Udo.Crm.Scripts.Interaction.updateAttributeCallerDetails(context, "udo_lastname", CallerLastName);
    if (CallerPhoneNumber !== "") {
        Va.Udo.Crm.Scripts.Interaction.updateAttributeCallerDetails(context, "udo_phonenumber", CallerPhoneNumber);
        Va.Udo.Crm.Scripts.Interaction.updateAttributeCallerDetails(context, "udo_nophonenumberavailable", false);
    }
    Va.Udo.Crm.Scripts.Interaction.updateAttributeCallerDetails(context, "udo_iscallerauthorized", true);
    context.data.save().then(
        function () {
            // do nothing
        },
        function (error) {
            console.log(error);
        });
}

Va.Udo.Crm.Scripts.Interaction.updateAttributeCallerDetails = function (context, attributeName, attributeValue) {
    try {

        var attribute = context.getAttribute(attributeName);
        attribute.setValue(attributeValue);
    }
    catch (e) {
        if (parent.window.IsUSD === true) {
            window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
        }

        UDO.Shared.openAlertDialog("Could not update caller details on the Interaction.\nTry editing the Interaction fields and saving directly.", "", 120, 260);
    }
}

Va.Udo.Crm.Scripts.Interaction.CreateAndShowEmergency = function (context, veteranId) {
    var valueAttrib = context.getAttribute("udo_phonenumber");

    var phoneStr = valueAttrib.getValue();
    if (phoneStr === null) phoneStr = "No Number Provided";

    valueAttrib = context.getAttribute("udo_firstname");
    var FNameStr = valueAttrib.getValue();
    if (FNameStr === null) FNameStr = "Emergency";

    valueAttrib = context.getAttribute("udo_lastname");
    var LNameStr = valueAttrib.getValue();

    var valueStr = "";
    if (LNameStr === null) valueStr = "Emergency";

    var emergencyCall = {};
    var vetId = veteranId;
    emergencyCall["va_veteranid@odata.bind"] = "/contacts(" + vetId + ")";
    emergencyCall.va_emergencytype = 953850003;
    emergencyCall.va_name = "Emergency"
    emergencyCall.va_firstname = FNameStr;
    emergencyCall.va_lastname = LNameStr;
    emergencyCall.va_phonenumber = phoneStr;

    var urlStr = globalCon.getClientUrl();

    Xrm.WebApi.createRecord("va_emergencycall", emergencyCall)
        .then(function (data) {
            Va.Udo.Crm.Scripts.Interaction.createEmergencyReqCallBack(data);
        },
            function (err) {
                UDO.Shared.openAlertDialog("An error has occurred:" + err);
            });
}

Va.Udo.Crm.Scripts.Interaction.createEmergencyReqCallBack = function (createEmergencyReq) {
    if (createEmergencyReq.id !== null) {
        var entityId = createEmergencyReq.id;

        UDO.Shared.GetCurrentAppProperties().then(
            function (appProperties) {
                var appId = appProperties.appId;

                var url = globalCon.getClientUrl() + "/main.aspx?appid=" + appId + "&newWindow=true&pagetype=entityrecord&cmdbar=false&navbar=off&etn=va_emergencycall&id=" + entityId;

                if (parent.window.IsUSD === true) {
                    window.open(url);
                }
            },
            function (error) {
                console.log(error);
            });
    } else {
        UDO.Shared.openAlertDialog("Could not create emergency record.");
    }
}

Va.Udo.Crm.Scripts.Interaction.USD_HideSelector = function (context) {
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


function validatePin(execContext) {
    try {
        var formContext = execContext.getFormContext();
        var pinControl = formContext.getControl("udo_securitypin");
        var pinNotificationId = "pinValidationNotification";

        function validateFourDigitNumber(pin) {
            // Regex to check for 5 digit number
            var re = /^[0-9]{4}$/;
            return re.test(pin);
        }

        function validatePinDigits(pin) {
            switch (pin) {
                case "0000":
                    return false;
                case "1111":
                    return false;
                case "2222":
                    return false;
                case "3333":
                    return false;
                case "4444":
                    return false;
                case "5555":
                    return false;
                case "6666":
                    return false;
                case "7777":
                    return false;
                case "8888":
                    return false;
                case "9999":
                    return false;
                default:
                    return true;
            }
        }

        pinControl.clearNotification(pinNotificationId);

        var currentPin = formContext.getAttribute("udo_securitypin").getValue();
        if ((currentPin === null) && securityPinVet !== "") {
            pinControl.addNotification({
                messages: ["Pin cannot be cleared or erased"],
                notificationLevel: "ERROR",
                uniqueId: pinNotificationId
            });
            return;
        }



        if (!validatePinDigits(formContext.getAttribute("udo_securitypin").getValue())) {
            pinControl.addNotification({
                messages: ["Pin cannot have the same four (4) consecutive digits"],
                notificationLevel: "ERROR",
                uniqueId: pinNotificationId
            });
        }

        if (currentPin !== null) {
            if (!validateFourDigitNumber(formContext.getAttribute("udo_securitypin").getValue())) {
                pinControl.addNotification({
                    messages: ["Pin must be four (4) digits"],
                    notificationLevel: "ERROR",
                    uniqueId: pinNotificationId
                });
            }
        }




        return true;
    } catch (e) {
        // handleError(e);
        return false;
    }
}


