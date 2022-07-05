//Meta Data for the Application Type Options
var No_Automation = 1;
var Wf_Assembly = 2;
var wf_XAML = 3;
var entity_HostedApplication = "uii_hostedapplication";
var Web_Hosted_Application = "Web Hosted Application";
var formContext = null;
function HideAllAutomationControls() {

    Xrm.Page.ui.controls.get("uii_workflowassemblytype").setVisible(false);
    Xrm.Page.ui.controls.get("uii_workflowxaml").setVisible(false);
}

function SetHostFieldReqLevel() {

    Xrm.Page.data.entity.attributes.get("uii_workflowassemblytype").setRequiredLevel("none");
    //Xrm.Page.data.entity.attributes.get("uii_assemblyuri").setRequiredLevel("none");
    Xrm.Page.data.entity.attributes.get("uii_workflowxaml").setRequiredLevel("none");
}


function DisplaywfXAMLSection() {
    Xrm.Page.ui.controls.get("uii_workflowxaml").setVisible(true);
    Xrm.Page.data.entity.attributes.get("uii_workflowassemblytype").setValue("");
    Xrm.Page.data.entity.attributes.get("uii_workflowxaml").setRequiredLevel("required");
}

function DisplaywfAssemblySection() {

    Xrm.Page.ui.controls.get("uii_workflowassemblytype").setVisible(true);
    Xrm.Page.data.entity.attributes.get("uii_workflowxaml").setValue("");
    Xrm.Page.data.entity.attributes.get("uii_workflowassemblytype").setRequiredLevel("required");
}

function AutomationModeOnChange() {

    // Get the field that fired the event.
    var sField = Xrm.Page.data.entity.attributes.get("uii_automationmode");

    //var sField = crmForm.all.uii_automationmode;
    //var option = crmForm.all.uii_automationmode;
    if (Xrm.Page.data.entity.attributes.get("uii_isdefault").getValue() == true) {
        Xrm.Page.ui.controls.get("uii_isdefault").setDisabled(true);
    }
    else {
        Xrm.Page.ui.controls.get("uii_isdefault").setDisabled(false);
    }

    //Mark all controls except automation mode  as invisible and later based on the selection mark appropriate sections as visible
    HideAllAutomationControls();
    var oAsynchronous = Xrm.Page.ui.controls.get("uii_isrunmodeasynchronous");
    oAsynchronous.setDisabled(false);

    // Validate the field information.
    if (typeof (sField) != "undefined" && sField != null) {
        var sAutomationType = sField.getSelectedOption().value;

        //Setting Hosting Fields as Non Mandatory
        SetHostFieldReqLevel();

        // Based on the AutomationType selection select the visibility of nodes. 
        switch (sAutomationType) {

            case Wf_Assembly:
                DisplaywfAssemblySection();

                break;
            case wf_XAML:
                DisplaywfXAMLSection();
                break;
            default:
                oAsynchronous.DataValue = false;
                oAsynchronous.disabled = true;
                break;
        }
    }
    //alert('uiiworkflowxaml'+Xrm.Page.ui.controls.get("uii_workflowxaml").getVisible());
    //alert('uiiworkflowassemblytype'+Xrm.Page.ui.controls.get("uii_workflowassemblytype").getVisible());

}

function ActionSave(ExecutionObj) {

    //Saving Portion - Validation
    //Validating XML Content
    formContext = ExecutionObj.getFormContext();
    var res = '';
    var sWorkflowXAML = Xrm.Page.data.entity.attributes.get("uii_workflowxaml");
    var sExtensionXml = Xrm.Page.data.entity.attributes.get("uii_extensionsxml");

    // Get the field that fired the event.
    var sField = Xrm.Page.data.entity.attributes.get("uii_automationmode");
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
    if (typeof (sExtensionXml) != "undefined" && sExtensionXml.getValue() != null) {
        res = validateXML(sExtensionXml.getValue());
        if (res != '') {
            alert(jsErr_InvalidExtnXml + "\n" + res);
            Xrm.Page.ui.controls.get("uii_extensionsxml").setFocus();
            ExecutionObj.getEventArgs().preventDefault();
            //event.returnValue = false;
            return false;

        }
    }
    // Validate the field information.
    if (typeof (sField) != "undefined" && sField != null) {
        var sAutomationType = sField.getSelectedOption().value;

        // Based on the AutomationType selection select the visibility of nodes. 
        switch (sAutomationType) {
            case wf_XAML:
                res = validateXML(sWorkflowXAML.getValue());
                if (res != '') {
                    alert(jsErr_InvalidWrkflowXml + "\n" + res);
                    Xrm.Page.ui.controls.get("uii_workflowxaml").setFocus();
                    ExecutionObj.getEventArgs().preventDefault();
                    //event.returnValue = false;
                    return false;
                }

        }
    }
    //event.returnValue = true;
    return true;
}