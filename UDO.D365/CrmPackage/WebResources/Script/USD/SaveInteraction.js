//Deprecated: See SetCallerAndCreateRequest instead
//function getAttributeValue(attributeName) {
//    var attribute = Xrm.Page.getAttribute(attributeName);
//    return attribute.getValue();
//}
//function checkForCallerDetails() {
//    try {
//        var errorMessage = "Please address the following issues before you proceed";
//        var error = false;
//        var phoneNumber = getAttributeValue("udo_phonenumber");
//        if (getAttributeValue("udo_lastname") == null) {
//            errorMessage += "\nEnter last name.";
//            error = true;
//        }
//        if (phoneNumber == null) {
//            var attribute = Xrm.Page.getAttribute("udo_nophonenumberavailable");
//            if (attribute.getValue() == true) {
//                Xrm.Page.data.save();
//            }
//            else {
//                error = true;
//                errorMessage += "\nEnter a phone number or set the No Phone # Available flag to Yes.";

//            }
//        }
//        if (error) {
//            alert(errorMessage);
//        }
//    }
//    catch (e) {
//        alert(e.message);
//    }
//    window.open('http://uii/Interaction/ScanForDataParameters');
//}

////Xrm.Page.data.save().then(checkForCallerDetails);
//Xrm.Page.data.entity.save();
//checkForCallerDetails();

