var formContext = null;
function WorkflowSave(ExecutionObj) {
    formContext = ExecutionObj.getFormContext();
    //Saving Portion - Validation
    //Validating XML Content
    var res = '';
    var sName = Xrm.Page.data.entity.attributes.get("uii_name");

    // Validate the field information.
    if (typeof (sName) != "undefined" && sName.getValue() != null) {
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

function WorkflowMaponSave(ExecutionObj) {
    formContext = ExecutionObj.getFormContext();
    //Saving Portion - Validation
    //Validating XML Content
    var res = '';
    var sName = Xrm.Page.data.entity.attributes.get("uii_sequence");

    // Validate the field information.
    if (typeof (sName) != "undefined" && sName.getValue() != null) {
        if (IsNumeric(sName.getValue()) == false) {
            Xrm.Page.ui.controls.get("uii_sequence").setFocus();
            alert(jsErr_InvalidSeqNumber);
            ExecutionObj.getEventArgs().preventDefault();
            //event.returnValue = false;
            return false;
        }
        //If number check for range
        if (Number(sName.getValue()) < 1 || Number(sName.getValue()) > 2147483647) {
            Xrm.Page.ui.controls.get("uii_sequence").setFocus();
            alert(jsErr_InvalidSeqNumber);
            ExecutionObj.getEventArgs().preventDefault();
            //event.returnValue = false;
            return false;
        }
    }
    //event.returnValue = true;
    return true;

};


function WorkflowStepSave(ExecutionObj) {
    formContext = ExecutionObj.getFormContext();

    //Saving Portion - Validation
    //Validating XML Content
    var res = '';
    var sName = Xrm.Page.data.entity.attributes.get("uii_name");

    // Validate the field information.
    if (typeof (sName) != "undefined" && sName.getValue() != null) {
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