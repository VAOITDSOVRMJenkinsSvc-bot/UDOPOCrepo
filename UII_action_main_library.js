function ActionOnLoad(exCon) {
    var formContext = exCon.getFormContext();

        //HideAssociatedViewButton('uii_uii_hostedapplication_uii_action', 'Add Existing Action');
        
        var XRM_FORM_TYPE_CREATE = 1;
        var XRM_FORM_TYPE_UPDATE = 2;
        var XRM_FORM_TYPE_READONLY = 3;
        var XRM_FORM_TYPE_DISABLED = 4;

        switch (formContext.ui.getFormType()) {
            case XRM_FORM_TYPE_CREATE:
            case XRM_FORM_TYPE_UPDATE:
                AutomationModeOnChange();
                //Xrm.Page.data.entity.attributes.get("uii_automationmode").FireOnChange();                
                break;
            case XRM_FORM_TYPE_READONLY:
            case XRM_FORM_TYPE_DISABLED:
                AutomationModeOnChange();

        }        
    }