//Meta Data for the Application Type Options
//var Hosted_Control = "Hosted Control";
//var Web_Hosted_Application = "Web Hosted Application";
//var External_Hosted_Application = "External Hosted Application";
//var Remote_Hosted_Application = "Remote Hosted Application";
var CitrixAssemblyType = "Microsoft.Uii.Csr.CitrixIntegration";
var CitrixAssemblyURI = "Microsoft.Uii.Csr.CitrixIntegration.CitrixApplicationHostedControl";
var Hosted_Control = 1;
var Web_Hosted_Application = 2;
var External_Hosted_Application = 3;
var Remote_Hosted_Application = 4;
var formContext = null;

function hostedapponload() {
    //HideAssociatedViewButton('uii_uii_hostedapplication_uii_action','Add existing UII Action to this record');
}

function HostedApplicationSave(ExecutionObj) {
    formContext = ExecutionObj.getFormContext();
    //Saving Portion - Validation
    //Validating XML Content
    var res = '';
    var sAutomationXml = Xrm.Page.data.entity.attributes.get("uii_automationxml");
    var sExtensionXml = Xrm.Page.data.entity.attributes.get("uii_extensionsxml");
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
    if (typeof (sAutomationXml) != "undefined" && sAutomationXml.getValue() != null) {
        res = validateXML(sAutomationXml.getValue());
        if (res != '') {
            alert(jsErr_InvalidAutoXml + "\n" + res);
            Xrm.Page.ui.controls.get("uii_automationxml").setFocus();
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
        //event.returnValue = true;
        return true;
    }
}


function HideAllSectionsInHostingTab() {
    var hostingTab = Xrm.Page.ui.tabs.get("Hosting");

    // Hide all the sections.
    var allSections = hostingTab.sections.get();
    for (var i in allSections) {
        var eachSection = allSections[i];
        eachSection.setVisible(false);
    }
}

function DisplayHostedControlSection() {
    //Display only the Assembly Info Section
    var hostingTab = Xrm.Page.ui.tabs.get("Hosting");

    var sectionAssemblyInfo = hostingTab.sections.get("AssemblyInfo");
    sectionAssemblyInfo.setVisible(true);

    Xrm.Page.data.entity.attributes.get("uii_assemblyuri").setRequiredLevel("required");
    Xrm.Page.data.entity.attributes.get("uii_assemblytype").setRequiredLevel("required");
}

function DisplayExternalApplicationSection() {
    //Show the following sections:
    //  1.ExternalApplicationSettings
    //  2.ApplicationHosting
    //  3.AlternateTopLevelWindow    

    var hostingTab = Xrm.Page.ui.tabs.get("Hosting");

    var sectionExternalAppSettings = hostingTab.sections.get("ExternalApplicationSettings");
    sectionExternalAppSettings.setVisible(true);

    var sectionAppHosting = hostingTab.sections.get("ApplicationHosting");
    sectionAppHosting.setVisible(true);
    ShowApplicationHostingControls();

    var sectionAltTopLevelWindow = hostingTab.sections.get("AlternateTopLevelWindow");
    sectionAltTopLevelWindow.setVisible(true);

    // set the mandate fields.
    Xrm.Page.data.entity.attributes.get("uii_externalappuri").setRequiredLevel("required");
}

function DisplayRemotedApplicationSection() {
    var hostingTab = Xrm.Page.ui.tabs.get("Hosting");

    //Display Application Hosting Section
    var sectionAppHosting = hostingTab.sections.get("ApplicationHosting");
    sectionAppHosting.setVisible(true);
    ShowApplicationHostingControls();

    //Display Alternate Top Level Window
    var sectionAltTopLevelWindow = hostingTab.sections.get("AlternateTopLevelWindow");
    sectionAltTopLevelWindow.setVisible(true);

    // Display Assembly Info Section
    var sectionAssemblyInfo = hostingTab.sections.get("AssemblyInfo");
    sectionAssemblyInfo.setVisible(true);

    // Display Remoted Application Section
    var sectionCitrixSettings = hostingTab.sections.get("CitrixApplicationSettings");
    sectionCitrixSettings.setVisible(true);

    //Set the mandatory Fields...
    Xrm.Page.data.entity.attributes.get("uii_assemblyuri").setRequiredLevel("required");
    Xrm.Page.data.entity.attributes.get("uii_assemblytype").setRequiredLevel("required");
    Xrm.Page.data.entity.attributes.get("uii_icafilename").setRequiredLevel("required");
}

function DisplayWebApplicationSection() {
    // Unhide the web application settings section.
    var hostingTab = Xrm.Page.ui.tabs.get("Hosting");
    var sectionwebApp = hostingTab.sections.get("WebApplicationHomePage");
    sectionwebApp.setVisible(true);

    // Set the mandate requirements.
    Xrm.Page.data.entity.attributes.get("uii_webappurl").setRequiredLevel("required");

    // Show the Application Hosting section.
    ShowApplicationHostingControls();
}

function SetHostFieldReqLevelAsNone() {
    Xrm.Page.data.entity.attributes.get("uii_externalappuri").setRequiredLevel("none");
    Xrm.Page.data.entity.attributes.get("uii_webappurl").setRequiredLevel("none");
    Xrm.Page.data.entity.attributes.get("uii_assemblyuri").setRequiredLevel("none");
    Xrm.Page.data.entity.attributes.get("uii_assemblytype").setRequiredLevel("none");
    Xrm.Page.data.entity.attributes.get("uii_icafilename").setRequiredLevel("none");
}

function ShowApplicationHostingControls() {
    // Make the section visible
    var hostingTab = Xrm.Page.ui.tabs.get("Hosting");
    var sectionAppHosting = hostingTab.sections.get("ApplicationHosting");
    sectionAppHosting.setVisible(true);

    // Hide all controls
    var allControls = sectionAppHosting.controls.get();
    for (var i in allControls) {
        var eachControl = allControls[i];
        eachControl.setVisible(false);
    }

    // Show the controls based on the application type.

    var appType = Xrm.Page.data.entity.attributes.get("uii_hostedapplicationtype").getSelectedOption().value;

    switch (appType) {
        case Web_Hosted_Application:
            {
                // Hide all controls
                var allControls = sectionAppHosting.controls.get();
                for (var i in allControls) {
                    var eachControl = allControls[i];
                    eachControl.setVisible(false);
                }
                // Show only the Application Hosting Mode.    
                Xrm.Page.ui.controls.get("uii_applicationhostingmode").setVisible(true);
            }
            break;
        case Remote_Hosted_Application:
            {
                // Hide all controls
                var allControls = sectionAppHosting.controls.get();
                for (var i in allControls) {
                    var eachControl = allControls[i];
                    eachControl.setVisible(false);
                }
                // Only Main Window Acquisition Timeout needs to be displayed
                Xrm.Page.ui.controls.get("uii_mainwindowacquisitiontimeout").setVisible(true);
            }
            break;
        case External_Hosted_Application:
            {
                // Show all controls
                var allControls = sectionAppHosting.controls.get();
                for (var i in allControls) {
                    var eachControl = allControls[i];
                    eachControl.setVisible(true);
                }
            }
            break;
    }

}

function HostedApplicationTypeOnChange() {
    // Get the Application type
    var oField = Xrm.Page.data.entity.attributes.get("uii_hostedapplicationtype");

    // Mark all section as invisible and later based on the selection mark appropriate sections as visible
    HideAllSectionsInHostingTab();

    // Validate the field information.
    if (typeof (oField) != "undefined" && oField != null) {
        var sAppType = oField.getSelectedOption().value;

        // Setting Hosting Fields as Non Mandatory
        SetHostFieldReqLevelAsNone();

        // Based on the App Type selection select the visibility of nodes. 
        switch (sAppType) {
            case Hosted_Control: //Display only Assembly Info
                DisplayHostedControlSection();
                break;

            case Web_Hosted_Application:
                DisplayWebApplicationSection();
                break;

            case External_Hosted_Application:
                DisplayExternalApplicationSection();
                break;

            case Remote_Hosted_Application:
                DisplayRemotedApplicationSection();
                break;
        }
        ApplicationHostingModeOnChange(); // This needs to be called when there is a app type change .
    }


}

function ApplicationHostingModeOnChange() {

    var Use_HostOutside = 1;
    var Use_DynamicPositioning = 3;
    var Use_SetParent = 2;

    // Get the field that fired the event.
    var oField = Xrm.Page.data.entity.attributes.get("uii_applicationhostingmode");
    var oAppType = Xrm.Page.data.entity.attributes.get("uii_hostedapplicationtype");
    var oShowMenu = Xrm.Page.data.entity.attributes.get("uii_isshowmenu");
    var oAttachedInputThread = Xrm.Page.data.entity.attributes.get("uii_isattachinputthread");


    var uiShowMenu = Xrm.Page.ui.controls.get("uii_isshowmenu");
    var uiAttachedInputThread = Xrm.Page.ui.controls.get("uii_isattachinputthread");
    var uiUseToolbar = Xrm.Page.ui.controls.get("uii_iswebappusetoolbar");
    var oiUseToolbar = Xrm.Page.data.entity.attributes.get("uii_iswebappusetoolbar");
    uiUseToolbar.setDisabled(false);
    

    var hostingTab = Xrm.Page.ui.tabs.get("Hosting");
    var sectionDynamicPositioning = hostingTab.sections.get("DynamicPositioningAttributes");
    sectionDynamicPositioning.setVisible(false);

    var sAppType = 0;
    if (typeof (oAppType) != "undefined" && oAppType != null) {
        sAppType = oField.getSelectedOption().value;
    }

    uiShowMenu.setDisabled(true);
    uiAttachedInputThread.setDisabled(true);

    // Validate the field information.
    if (typeof (oField) != "undefined" && oField != null) {
        var sHostingMode = oField.getSelectedOption().value;

        // Change visibility of ShowMenu, AttachInputThreads based on Hosting Mode Selection.
        switch (sHostingMode) {

            case Use_HostOutside:
                uiUseToolbar.setDisabled(true);
                oiUseToolbar.setValue(false);
                break;

            case Use_DynamicPositioning:
                uiShowMenu.setDisabled(true);
                oShowMenu.setValue(false);
                uiAttachedInputThread.setDisabled(false);

                if (sAppType != Web_Hosted_Application && sAppType != Hosted_Control) {
                    sectionDynamicPositioning.setVisible(true);
                }
                else {
                    sectionDynamicPositioning.setVisible(false);
                }
                break;

            case Use_SetParent:
                uiShowMenu.setDisabled(false);
                uiAttachedInputThread.setDisabled(true);
                oAttachedInputThread.setValue(false);
                break;
        }
    }
}

function AdapterOnChange() {
    var Use_No_Adapter = 1;
    var Use_Automation_Adapter = 2;
    var Use_Adapter = 3;

    // Get the field that fired the event.
    var oField = Xrm.Page.data.entity.attributes.get("uii_adaptermode");
    var oAdapterURI = Xrm.Page.data.entity.attributes.get("uii_adapteruri");
    var oAdapterType = Xrm.Page.data.entity.attributes.get("uii_adaptertype");

    var uiField = Xrm.Page.ui.controls.get("uii_adaptermode");
    var uiAdapterURI = Xrm.Page.ui.controls.get("uii_adapteruri");
    var uiAdapterType = Xrm.Page.ui.controls.get("uii_adaptertype");

    // Validate the field information.
    if (typeof (oField) != "undefined" && oField != null) {
        var sAdapterMode = oField.getSelectedOption().value;

        // Change visibility of Type, URI based on Adapter Mode Selection.
        switch (sAdapterMode) {

            case Use_No_Adapter:
                uiAdapterURI.setDisabled(true);
                oAdapterURI.setValue("");
                uiAdapterType.setDisabled(true);
                oAdapterType.setValue("");
                break;

            case Use_Automation_Adapter:
                uiAdapterURI.setDisabled(true);
                oAdapterURI.setValue("Microsoft.Uii.HostedApplicationToolkit.AutomationHosting");
                uiAdapterType.setDisabled(true);
                oAdapterType.setValue("Microsoft.Uii.HostedApplicationToolkit.AutomationHosting.AutomationAdapter");
                break;

            case Use_Adapter:
                uiAdapterURI.setDisabled(false);
                uiAdapterType.setDisabled(false);
                break;
        }
    }
}

function DynamicAppOnChange() {

    // Get the field that fired the event.
    var oField = Xrm.Page.data.entity.attributes.get("uii_isappdynamic");
    var oShowinToolBar = Xrm.Page.data.entity.attributes.get("uii_isshowintoolbardropdown");
    var oUserCanClose = Xrm.Page.data.entity.attributes.get("uii_usercanclose");

    var uiField = Xrm.Page.ui.controls.get("uii_isappdynamic");
    var uiShowinToolBar = Xrm.Page.ui.controls.get("uii_isshowintoolbardropdown");
    var uiUserCanClose = Xrm.Page.ui.controls.get("uii_usercanclose");

    // Validate the field information.
    if (typeof (oField) != "undefined" && oField != null) {
        var isDynamicApp = oField.getValue();

        // Enable UserCanClose, ShowMenu if it  is DynamicApp
        if (isDynamicApp == false) {
            uiShowinToolBar.setDisabled(true);
            uiUserCanClose.setDisabled(true);
            oShowinToolBar.setValue(false);
            oUserCanClose.setValue(false);

            Xrm.Page.ui.controls.get("uii_isdependentonworkflow").setDisabled(false);
        }

        else {
            uiShowinToolBar.setDisabled(false);
            uiUserCanClose.setDisabled(false);
            Xrm.Page.ui.controls.get("uii_isdependentonworkflow").setDisabled(true);
            Xrm.Page.data.entity.attributes.get("uii_isdependentonworkflow").setValue(false);
        }

    }
}

function TopLevelWindowOnChange() {

    var Use_FindWindow = 3;

    // Get the field that fired the event.
    var oField = Xrm.Page.data.entity.attributes.get("uii_toplevelwindowmode");
    var oCaption = Xrm.Page.data.entity.attributes.get("uii_toplevelwindowcaption");
    var oFindWindow = Xrm.Page.data.entity.attributes.get("uii_findwindowclass");
    var oLimitToProcess = Xrm.Page.data.entity.attributes.get("uii_islimittoprocess");

    var uiField = Xrm.Page.ui.controls.get("uii_toplevelwindowmode");
    var uiCaption = Xrm.Page.ui.controls.get("uii_toplevelwindowcaption");
    var uiFindWindow = Xrm.Page.ui.controls.get("uii_findwindowclass");
    var uiLimitToProcess = Xrm.Page.ui.controls.get("uii_islimittoprocess");

    uiCaption.setDisabled(true);
    uiFindWindow.setDisabled(true);
    uiLimitToProcess.setDisabled(true);

    // Validate the field information.
    if (typeof (oField) != "undefined" && oField != null) {
        var sTopLevelMode = oField.getSelectedOption().value;

        // Change visibility ofcontrols based on Top Level Mode Selection.
        switch (sTopLevelMode) {

            case Use_FindWindow:
                uiCaption.setDisabled(false);
                uiFindWindow.setDisabled(false);
                uiLimitToProcess.setDisabled(false);
                break;
            default:
                oCaption.setValue("");
                oFindWindow.setValue("");
                oLimitToProcess.setValue(false);
                break;

        }
    }
}