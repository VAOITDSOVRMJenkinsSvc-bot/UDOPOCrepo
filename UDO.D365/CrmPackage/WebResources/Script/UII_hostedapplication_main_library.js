/* Start Script Execution */

function OnCrmPageLoad(exCon) 
{
    //hostedapponload();
    var formContext = exCon.getFormContext();

    var XRM_FORM_TYPE_CREATE = 1;
    var XRM_FORM_TYPE_UPDATE = 2;
    var XRM_FORM_TYPE_READONLY = 3;
    var XRM_FORM_TYPE_DISABLED = 4;

    switch (Xrm.Page.ui.getFormType()) {
        case XRM_FORM_TYPE_CREATE:
        case XRM_FORM_TYPE_UPDATE:
            Xrm.Page.data.entity.attributes.get("uii_adaptermode").fireOnChange();
            Xrm.Page.data.entity.attributes.get("uii_isappdynamic").fireOnChange();
            Xrm.Page.data.entity.attributes.get("uii_hostedapplicationtype").fireOnChange();
            Xrm.Page.data.entity.attributes.get("uii_applicationhostingmode").fireOnChange();
            Xrm.Page.data.entity.attributes.get("uii_toplevelwindowmode").fireOnChange();
            break;
        case XRM_FORM_TYPE_READONLY:
        case XRM_FORM_TYPE_DISABLED:
            AdapterOnChange();
            HostedApplicationTypeOnChange();
            TopLevelWindowOnChange();
            DynamicAppOnChange();
            ApplicationHostingModeOnChange();
            break;
    }
}