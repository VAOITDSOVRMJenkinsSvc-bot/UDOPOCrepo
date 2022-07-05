var exCon = null;
var formContext = null;
function PageOnLoad(execCon)
{
    exCon = execCon;
    formContext = exCon.getFormContext();
    var XRM_FORM_TYPE_CREATE = 1;
    var XRM_FORM_TYPE_UPDATE = 2;
    var XRM_FORM_TYPE_READONLY = 3;
    var XRM_FORM_TYPE_DISABLED = 4;

    switch (Xrm.Page.ui.getFormType()) {
        case XRM_FORM_TYPE_CREATE:
        case XRM_FORM_TYPE_UPDATE:
            break;
        case XRM_FORM_TYPE_READONLY:
        case XRM_FORM_TYPE_DISABLED:
            break;
    }
    //OnEnableConditionChange();
}

function OnEnableConditionChange()
{
    var oCondition = Xrm.Page.data.entity.attributes.get("msdyusd_enablecondition");
    if (oCondition.getSelectedOption() != null 
         && oCondition.getSelectedOption().value == 803750002)
    {  // Script Condition
        Xrm.Page.ui.controls.get("msdyusd_scriptcondition").setVisible(true);
    }
    else  
    {
        Xrm.Page.ui.controls.get("msdyusd_scriptcondition").setVisible(false);
    }
}

// EXAMPLES:
// Xrm.Page.ui.tabs.get("GeneralTab").sections.get("CTI").setVisible(true);
// Xrm.Page.data.entity.attributes.get("msdyusd_crmwindowhosttype").fireOnChange();
// var oFromSearchUI = Xrm.Page.ui.controls.get("msdyusd_fromsearch");