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
    FromAndSearchMutuallyExclusive();
    EntityAndUrlMutuallyExclusive();
    FieldOnlyValueForOnLoad();
    PageOptionsOnlyValidForTab();
    FrameForDashboardOnly();
    DestinationFrameForDashboardOnly();
    InitiatingCTIDesktopManagerOnly();
    OnSingleMatchDecision();
}

function OnDestinationChange()
{
    var oDestination = Xrm.Page.data.entity.attributes.get("msdyusd_destination");
    if (Xrm.Page.ui.controls.get("msdyusd_destination").getVisible() == false)
    {
        Xrm.Page.ui.tabs.get("ResultTab").sections.get("Tab").setVisible(false);
        Xrm.Page.ui.tabs.get("ResultTab").sections.get("EntitySearch").setVisible(false);
    }
    else if (oDestination.getSelectedOption() == null 
         || oDestination.getSelectedOption().value == 803750000)
    {  // Tab
	if (oDestination.getSelectedOption() == null)
	    oDestination.setValue(803750000);
        Xrm.Page.ui.tabs.get("ResultTab").sections.get("Tab").setVisible(true);
        Xrm.Page.ui.tabs.get("ResultTab").sections.get("EntitySearch").setVisible(false);
    }
    else  // 803750001
    {  // Entity Search
        Xrm.Page.ui.tabs.get("ResultTab").sections.get("Tab").setVisible(false);
        Xrm.Page.ui.tabs.get("ResultTab").sections.get("EntitySearch").setVisible(true);
    }
    PageOptionsOnlyValidForTab();
}

function FromAndSearchMutuallyExclusive()
{
    var oFromData =Xrm.Page.data.entity.attributes.get("msdyusd_from");
    var oFromUI =Xrm.Page.ui.controls.get("msdyusd_from");
    var oFromSearchData = Xrm.Page.data.entity.attributes.get("msdyusd_fromsearch");
    var oFromSearchUI = Xrm.Page.ui.controls.get("msdyusd_fromsearch");
    if (oFromData.getValue() != null && oFromData.getValue() != "")
    {
        oFromUI.setVisible(true);
        oFromSearchUI.setVisible(false);
    }
    else if (oFromSearchData.getValue() != null && oFromSearchData.getValue() != "")
    {
        oFromUI.setVisible(false);
        oFromSearchUI.setVisible(true);
    }
    else
    {
        oFromUI.setVisible(true);
        oFromSearchUI.setVisible(true);
    }
}

function EntityAndUrlMutuallyExclusive()
{
    var oEntityData =Xrm.Page.data.entity.attributes.get("msdyusd_entity");
    var oEntityUI =Xrm.Page.ui.controls.get("msdyusd_entity");
    var oUrlSearchData = Xrm.Page.data.entity.attributes.get("msdyusd_url");
    var oUrlSearchUI = Xrm.Page.ui.controls.get("msdyusd_url");
    if (oEntityData.getValue() != null && oEntityData.getValue() != "")
    {
        oEntityUI .setVisible(true);
        oUrlSearchUI .setVisible(false);
    }
    else if (oUrlSearchData .getValue() != null && oUrlSearchData .getValue() != "")
    {
        oEntityUI .setVisible(false);
        oUrlSearchUI .setVisible(true);
    }
    else
    {
        oEntityUI .setVisible(true);
        oUrlSearchUI .setVisible(true);
    }
}

function FieldOnlyValueForOnLoad()
{
    var oRouteTypeData =Xrm.Page.data.entity.attributes.get("msdyusd_routetype");
    var oFieldData = Xrm.Page.data.entity.attributes.get("msdyusd_field");
    var oFieldUI = Xrm.Page.ui.controls.get("msdyusd_field");
    if (oRouteTypeData .getValue() != null && oRouteTypeData .getValue() == 803750001)
    {
        oFieldUI .setVisible(true);
    }
    else
    {
        oFieldUI .setVisible(false);
    }
}

function PageOptionsOnlyValidForTab()
{
    var oDestinationData =Xrm.Page.data.entity.attributes.get("msdyusd_destination");
    var oOptionsSection = Xrm.Page.ui.tabs.get("ResultTab").sections.get("OptionsSection");
    if (Xrm.Page.ui.controls.get("msdyusd_destination").getVisible() == false)
    {
        oOptionsSection.setVisible(false);
    }
    else if (oDestinationData.getValue() != null && oDestinationData.getValue() == 803750000)
    {
        oOptionsSection.setVisible(true);
    }
    else
    {
        oOptionsSection.setVisible(false);
    }
}

function FrameForDashboardOnly()
{
   Xrm.Page.ui.controls.get("msdyusd_sourceframe").setVisible(false);
    var oFromData =Xrm.Page.data.entity.attributes.get("msdyusd_from");
    if (oFromData.getValue() != null && oFromData.getValue() != "")
    {
         // is this entity selected, a Dashboard layout app?
        RetrieveHostedControl(oFromData.getValue()[0].id, SourceFrameHandler);
    }
}

function RetrieveHostedControl(id, hostedApplicationReturnFunction)
{
   SDK.REST.retrieveRecord(
       id,
       "UII_hostedapplication",
       null,null,
        hostedApplicationReturnFunction,
        errorHandler
   );  
}

function SourceFrameHandler(hostedApplication) 
{
    if ( hostedApplication.msdyusd_CRMWindowHostType.Value == 803750013 ) // USD Dashboard Layout
    {
         Xrm.Page.ui.controls.get("msdyusd_sourceframe").setVisible(true);
    }
    else
    {
        Xrm.Page.data.entity.attributes.get("msdyusd_sourceframe").setValue("");
    }
}

function DestinationFrameForDashboardOnly()
{
    Xrm.Page.ui.controls.get("msdyusd_dashboardframe").setVisible(false);
    var oFromData =Xrm.Page.data.entity.attributes.get("msdyusd_application");
    if (oFromData.getValue() != null && oFromData.getValue() != "")
    {
         // is this entity selected, a Dashboard layout app?
        RetrieveHostedControl(oFromData.getValue()[0].id, DestinatonFrameHandler);
    }
}

function DestinatonFrameHandler(hostedApplication) 
{
    if ( hostedApplication.msdyusd_CRMWindowHostType.Value == 803750013 ) // USD Dashboard Layout
    {
         Xrm.Page.ui.controls.get("msdyusd_dashboardframe").setVisible(true);
    }
    else
    {
        Xrm.Page.data.entity.attributes.get("msdyusd_dashboardframe").setValue("");
    }
}

function InitiatingCTIDesktopManagerOnly()
{
    var oFromData =Xrm.Page.data.entity.attributes.get("msdyusd_from");
    if (oFromData.getValue() != null && oFromData.getValue() != "")
    {
         // is this entity selected, a Dashboard layout app?
        RetrieveHostedControl(oFromData.getValue()[0].id, InitiatingActivityHandler);
    }
    else
    {
        InitiatingActivityHandler(null);
    }
}

function InitiatingActivityHandler(hostedApplication) 
{
    if ( hostedApplication != null && hostedApplication.msdyusd_CRMWindowHostType.Value == 803750014 ) // CTI Desktop Manager
    {
        Xrm.Page.ui.tabs.get("CTITab").setVisible(true);
        Xrm.Page.ui.controls.get("msdyusd_routetype").setVisible(false);
        Xrm.Page.data.entity.attributes.get("msdyusd_routetype").setRequiredLevel("none");
        Xrm.Page.ui.controls.get("msdyusd_action").setVisible(false);
        Xrm.Page.data.entity.attributes.get("msdyusd_action").setRequiredLevel("none");
        Xrm.Page.ui.controls.get("msdyusd_entity").setVisible(false);
        Xrm.Page.ui.controls.get("msdyusd_url").setVisible(false);
    }
    else
    {
        Xrm.Page.ui.tabs.get("CTITab").setVisible(false);
        Xrm.Page.ui.controls.get("msdyusd_routetype").setVisible(true);
        Xrm.Page.data.entity.attributes.get("msdyusd_routetype").setRequiredLevel("required");
        Xrm.Page.ui.controls.get("msdyusd_action").setVisible(true);
        Xrm.Page.data.entity.attributes.get("msdyusd_action").setRequiredLevel("required");
        Xrm.Page.ui.controls.get("msdyusd_entity").setVisible(true);
	// the above automatically sets the msdyusd_url appropriately
    }
    OnSingleMatchDecision();
    Xrm.Page.data.entity.attributes.get("msdyusd_nomatchdecision").fireOnChange();
    Xrm.Page.data.entity.attributes.get("msdyusd_singlematchdecision").fireOnChange();
    Xrm.Page.data.entity.attributes.get("msdyusd_multiplematchesdecision").fireOnChange();
}

function OnSingleMatchDecision()
{
    if (Xrm.Page.ui.tabs.get("CTITab").getVisible() == false)
    {
        OnDestinationChange();  /// have to do this anyway
        return;
    }
    if (Xrm.Page.data.entity.attributes.get("msdyusd_singlematchdecision").getValue() == 803750003)
    {
        // Create Session, Load Match, Call Action
        Xrm.Page.ui.controls.get("msdyusd_destination").setVisible(true);
    }
    else
    {
        Xrm.Page.ui.controls.get("msdyusd_destination").setVisible(false);
    }
    OnDestinationChange();
}

function ActionDisplay(decisionControl, actionControl)
{
    if (Xrm.Page.data.entity.attributes.get(decisionControl).getValue() == 803750001)
    {
        // Next Rule
        Xrm.Page.ui.controls.get(actionControl).setVisible(false);
    }
    else
    {
        Xrm.Page.ui.controls.get(actionControl).setVisible(true);
    }
}

function errorHandler(error) {
 alert(error.message);
}

// EXAMPLES:
// Xrm.Page.ui.tabs.get("GeneralTab").sections.get("CTI").setVisible(true);
// Xrm.Page.data.entity.attributes.get("msdyusd_crmwindowhosttype").fireOnChange();