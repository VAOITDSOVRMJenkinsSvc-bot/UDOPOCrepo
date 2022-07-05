function getAttributeValue(attributeName) {
    var attribute = Xrm.Page.getAttribute(attributeName);
    return attribute.getValue();
}
function checkForCallerDetails() {
    try {
        var attributeAuthorized = Xrm.Page.getAttribute("udo_iscallerauthorized");
        attributeAuthorized.setValue(1);

        var errorMessage = "Please address the following issues before you proceed:";
        var error = false;
        var phoneNumber = getAttributeValue("udo_phonenumber");
        if (getAttributeValue("udo_lastname") == null) {
            errorMessage += "\nEnter Last Name.";
            error = true;
        }
        if (phoneNumber == null) {
            var attribute = getAttributeValue("udo_nophonenumberavailable");
            if (attribute === true) {
                //Xrm.Page.data.save();
            }
            else {
                error = true;
                errorMessage += "\nEnter a phone number or set the No Phone # Avail checkbox to Yes.";
            }
        }
        if (error) {
            window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
            Va.Udo.Crm.Scripts.Popup
                .MsgBox(errorMessage,
                    Va.Udo.Crm.Scripts.Popup.PopupStyles.Exclamation,
                    "Interaction Update",
                    { height: 200, width: 350 });
            //alert(errorMessage);
        }
    }
    catch (e) {
        //alert(e.message);
        window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
        Va.Udo.Crm.Scripts.Popup
    .MsgBox(e.message,
        Va.Udo.Crm.Scripts.Popup.PopupStyles.Critical,
        "Interaction Update Error:",
        { height: 200, width: 350 });
    }
    //window.open('http://uii/Interaction/ScanForDataParameters');
}

//Xrm.Page.data.save().then(checkForCallerDetails);
Xrm.Page.data.entity.save();
Xrm.Page.data.entity.save();
checkForCallerDetails();
