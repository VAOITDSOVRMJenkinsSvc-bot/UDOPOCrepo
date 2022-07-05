function OptionSave(ExecutionObj) {
    //Saving Portion - Validation
    var formContext = ExecutionObj.getFormContext();
    var sName = formContext.getAttribute("uii_name");

    // Validate the field information.
    if (sName.getValue() != null) {
        if (validateSpecialCharacters(sName.getValue()) == false) {
            formContext.ui.controls.get("uii_name").setFocus();
            alert(jsErr_InvalidName);
            ExecutionObj.getEventArgs().preventDefault();
            //event.returnValue = false;
            return false;
        }

    }
      //event.returnValue = true;
       return true;

};