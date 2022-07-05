function NonHostedApplicationSave(ExecutionObj) {
    var formContext = ExecutionObj.getFormContext();
    //Saving Portion - Validation
    //Validating XML Content
    var res = '';
    var sName = Xrm.Page.data.entity.attributes.get("uii_name");

    // Validate the field information.
    if (sName.getValue() != null) {
        if (validateSpecialCharacters(sName.getValue()) == false) {
            Xrm.Page.ui.controls.get("uii_name").setFocus();

            alert(jsErr_InvalidName);
            ExecutionObj.getEventArgs().preventDefault();
            //event.returnValue = false;
            return false;
        }

    }
    //event.returnValue = true;
    return true;

};