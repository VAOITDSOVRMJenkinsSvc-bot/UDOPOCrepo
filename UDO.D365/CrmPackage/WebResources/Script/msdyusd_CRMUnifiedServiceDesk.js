// JavaScript source code
/* Start Script Execution */
var exCon = null;
var formContext = null;
function OnCrmWindowManagementPageLoad(execCon) {
    exCon = execCon;
    formContext = exCon.getFormContext();
    var XRM_FORM_TYPE_CREATE = 1;
    var XRM_FORM_TYPE_UPDATE = 2;
    var XRM_FORM_TYPE_READONLY = 3;
    var XRM_FORM_TYPE_DISABLED = 4;

    switch (Xrm.Page.ui.getFormType()) {
        case XRM_FORM_TYPE_CREATE:
            CRMWindowHostType(true);
            break;
        case XRM_FORM_TYPE_UPDATE:
        case XRM_FORM_TYPE_READONLY:
        case XRM_FORM_TYPE_DISABLED:
            CRMWindowHostType(false);
            break;
    }
}


function NonUSD() {
    // initialize form
    var generalTab = Xrm.Page.ui.tabs.get("{c0330026-8257-4219-8bc2-998017d1b6f7}");
    generalTab.setVisible(true);  // always visible!
    // skip {04ca7cc4-d509-470c-a65b-76067e429b6e} - General Section
    generalTab.sections.get("{c0330026-8257-4219-8bc2-998017d1b6f7}_section_7").setVisible(true);  // Unified Service Desk Section
    Xrm.Page.ui.controls.get("msdyusd_savedurl").setVisible(false);
    Xrm.Page.ui.controls.get("msdyusd_allowmultiplepages").setVisible(false);
    Xrm.Page.ui.controls.get("msdyusd_maximumbrowsers").setVisible(false);
    Xrm.Page.ui.controls.get("msdyusd_dashboardname").setVisible(false);
    generalTab.sections.get("{4b8adb5c-c3d8-de11-a899-00155d289c61}").setVisible(true);  // Hosted App Type
    Xrm.Page.ui.controls.get("uii_hostedapplicationtype").setVisible(true);
    generalTab.sections.get("usdpanel").setVisible(false);  // USD Panel
    Xrm.Page.ui.controls.get("msdyusd_paneltype").setVisible(false);
    Xrm.Page.ui.controls.get("msdyusd_xaml").setVisible(false);
    generalTab.sections.get("{badd49e5-c3d8-de11-a899-00155d289c61}").setVisible(true);  // Common Properties
    Xrm.Page.ui.controls.get("uii_isglobalapplication").setVisible(true);
    Xrm.Page.ui.controls.get("uii_isdependentonworkflow").setVisible(true);
    Xrm.Page.ui.controls.get("uii_displaygroup").setVisible(true);
    Xrm.Page.ui.controls.get("uii_minimumsizex").setVisible(true);
    Xrm.Page.ui.controls.get("uii_minimumsizey").setVisible(true);
    Xrm.Page.ui.controls.get("uii_optimalsizex").setVisible(true);
    Xrm.Page.ui.controls.get("uii_optimalsizey").setVisible(true);
    generalTab.sections.get("{c6d143eb-c3d8-de11-a899-00155d289c61}").setVisible(true);  // Adapter Configuration
    Xrm.Page.ui.controls.get("uii_adaptermode").setVisible(true);
    Xrm.Page.ui.controls.get("uii_adapteruri").setVisible(true);
    Xrm.Page.ui.controls.get("uii_adaptertype").setVisible(true);
    generalTab.sections.get("{c7d143eb-c3d8-de11-a899-00155d289c61}").setVisible(true);  // dynamic
    Xrm.Page.ui.controls.get("uii_isappdynamic").setVisible(true);
    Xrm.Page.ui.controls.get("uii_usercanclose").setVisible(true);
    Xrm.Page.ui.controls.get("uii_isshowintoolbardropdown").setVisible(true);
    Xrm.Page.ui.controls.get("msdyusd_hostingtype").setVisible(false);
	Xrm.Page.ui.controls.get("msdyusd_prefetchdata").setVisible(false);

    var hostingTab = Xrm.Page.ui.tabs.get("Hosting");
    hostingTab.setVisible(true);
    hostingTab.sections.get("ExternalApplicationSettings").setVisible(false);
    Xrm.Page.ui.controls.get("uii_externalappuri").setVisible(true);
    Xrm.Page.ui.controls.get("uii_externalapparguments").setVisible(true);
    Xrm.Page.ui.controls.get("uii_externalappworkingdirectory").setVisible(true);
    Xrm.Page.ui.controls.get("uii_managehosting").setVisible(true);
    hostingTab.sections.get("ApplicationHosting").setVisible(false);
    Xrm.Page.ui.controls.get("uii_applicationhostingmode").setVisible(true);
    Xrm.Page.ui.controls.get("uii_isattachinputthread").setVisible(true);
    Xrm.Page.ui.controls.get("uii_isshowmenu").setVisible(true);
    Xrm.Page.ui.controls.get("uii_isnomessagepump").setVisible(true);
    Xrm.Page.ui.controls.get("uii_mainwindowacquisitiontimeout").setVisible(true);
    hostingTab.sections.get("AlternateTopLevelWindow").setVisible(false);
    Xrm.Page.ui.controls.get("uii_toplevelwindowmode").setVisible(true);
    Xrm.Page.ui.controls.get("uii_toplevelwindowcaption").setVisible(true);
    Xrm.Page.ui.controls.get("uii_findwindowclass").setVisible(true);
    Xrm.Page.ui.controls.get("uii_islimittoprocess").setVisible(true);
    hostingTab.sections.get("WebApplicationHomePage").setVisible(false);
    Xrm.Page.ui.controls.get("uii_webappurl").setVisible(true);
    Xrm.Page.ui.controls.get("uii_iswebappusetoolbar").setVisible(true);
    Xrm.Page.ui.controls.get("uii_isusenewbrowserprocess").setVisible(true);
    Xrm.Page.ui.controls.get("uii_managepopups").setVisible(true);
    hostingTab.sections.get("AssemblyInfo").setVisible(false);
    Xrm.Page.ui.controls.get("uii_assemblyuri").setVisible(true);
    Xrm.Page.ui.controls.get("uii_assemblytype").setVisible(true);
    hostingTab.sections.get("DynamicPositioningAttributes").setVisible(false);
    Xrm.Page.ui.controls.get("uii_isretainframeandcaption").setVisible(true);
    Xrm.Page.ui.controls.get("uii_isretainontaskbar").setVisible(true);
    Xrm.Page.ui.controls.get("uii_isretainsystemmenu").setVisible(true);
    Xrm.Page.ui.controls.get("uii_isrestoreifminimized").setVisible(true);
    Xrm.Page.ui.controls.get("uii_removesizingcontrols").setVisible(true);
    hostingTab.sections.get("CitrixApplicationSettings").setVisible(false);
    Xrm.Page.ui.controls.get("uii_icafilename").setVisible(true);
    Xrm.Page.ui.controls.get("uii_remote_processacquisitionattempts").setVisible(true);
    Xrm.Page.ui.controls.get("uii_remote_processacquisitiondelay").setVisible(true);
    Xrm.Page.ui.controls.get("uii_processacquisitionfilename").setVisible(true);

    var translationTab = Xrm.Page.ui.tabs.get("TranslationServices");
    translationTab.setVisible(false);
    generalTab.sections.get("usdpanel").setVisible(false);  // USD Panel
    Xrm.Page.ui.controls.get("msdyusd_paneltype").setVisible(false);

    var automationTab = Xrm.Page.ui.tabs.get("{9c1adb13-c3d8-de11-a899-00155d289c61}");
    automationTab.setVisible(true);  // automation
    automationTab.sections.get("{08b2d4ae-c5d8-de11-a899-00155d289c61}").setVisible(true); // Automation Bindings
    Xrm.Page.ui.controls.get("uii_automationxml").setVisible(true);

    var extensionsTab = Xrm.Page.ui.tabs.get("{b49ba11c-c3d8-de11-a899-00155d289c61}");
    extensionsTab.setVisible(true);  // extensions
    extensionsTab.sections.get("{4ef881cf-c5d8-de11-a899-00155d289c61}").setVisible(true); // Extensions
    Xrm.Page.ui.controls.get("uii_extensionsxml").setVisible(true);

    HostedApplicationTypeOnChange();
}



function CRMWindowHostTypeOnChange() {
    CRMWindowHostType(true);
}

function CRMWindowHostType(forceUpdate) {

    var oCRMWindowHostType = Xrm.Page.data.entity.attributes.get("msdyusd_crmwindowhosttype");

    // Get the Application type
    var oHAType = Xrm.Page.data.entity.attributes.get("uii_hostedapplicationtype");

    // Validate the field information.
    if (typeof (oCRMWindowHostType) != "undefined" && oCRMWindowHostType != null) {
        var sCRMWindowHostType;
        if (oCRMWindowHostType.getSelectedOption() != null)
            sCRMWindowHostType = oCRMWindowHostType.getSelectedOption().value;
        else
            sCRMWindowHostType = 803750003;

        // Based on the App Type selection select the visibility of nodes. 
        switch (sCRMWindowHostType) {
            case 803750002: // Non-CRM Window
                NonUSD();
                break;

            case 803750000: // CRM Dialog
                USDBasic(forceUpdate, "MainPanel", "Microsoft.Crm.UnifiedServiceDesk.Dynamics", "Microsoft.Crm.UnifiedServiceDesk.Dynamics.BrowserWindowEx", true, null, false, true, false, sCRMWindowHostType, false, true);
                break;

            case 803750003: // CRM page
                USDBasic(forceUpdate, "MainPanel", "Microsoft.Crm.UnifiedServiceDesk.Dynamics", "Microsoft.Crm.UnifiedServiceDesk.Dynamics.BrowserWindowEx", true, null, null, true, false, sCRMWindowHostType, true, true, true);
                break;

            case 803750001: // CRM Global Manager
                USDBasic(forceUpdate, "hiddenpanel", "Microsoft.Crm.UnifiedServiceDesk.Dynamics", "Microsoft.Crm.UnifiedServiceDesk.Dynamics.CRMGlobalManager", false, true, false, false, true, sCRMWindowHostType, false);
                break;

            case 803750004: // USD Enabled Web Application
                USDBasic(forceUpdate, null, "Microsoft.Crm.UnifiedServiceDesk.Dynamics", "Microsoft.Crm.UnifiedServiceDesk.Dynamics.GenericWebBrowser", true, null, null, true, false, sCRMWindowHostType, true, true, true);
                break;

            case 803750005: // USD Hosted Control
                USDBasic(forceUpdate, null, null, null, null, null, false, true, false, sCRMWindowHostType, true);
                break;

            case 803750006: // USD Agent Scripting
                USDBasic(forceUpdate, "WorkflowPanel", "Microsoft.Crm.UnifiedServiceDesk.Dynamics", "Microsoft.Crm.UnifiedServiceDesk.Dynamics.ScriptControl", false, false, false, true, false, sCRMWindowHostType, false);
                break;

            case 803750007: // USD Ribbon Hosted Control
                USDBasic(forceUpdate, "HiddenPanel", null, null, false, null, false, false, false, sCRMWindowHostType, false);
                break;

            case 803750008: // USD Toolbar Container
                USDBasic(forceUpdate, "ToolbarPanel", "Microsoft.Crm.UnifiedServiceDesk.Dynamics", "Microsoft.Crm.UnifiedServiceDesk.Dynamics.Toolbar", false, true, false, true, false, sCRMWindowHostType, false);
                break;

            case 803750009: // USD Session Tabs
                USDBasic(forceUpdate, "SessionTabsPanel", "Microsoft.Crm.UnifiedServiceDesk.Dynamics", "Microsoft.Crm.UnifiedServiceDesk.Dynamics.SessionTabsControl", false, true, false, false, false, sCRMWindowHostType, false);
                break;

            case 803750010: // USD User Notes
                USDBasic(forceUpdate, null, "Microsoft.Crm.UnifiedServiceDesk.Dynamics", "Microsoft.Crm.UnifiedServiceDesk.Dynamics.UserNotes", true, false, false, true, false, sCRMWindowHostType, false);
                break;

            case 803750011: // CRM Connection (CRM Util Control)
                USDBasic(forceUpdate, "HiddenPanel", "Microsoft.Crm.UnifiedServiceDesk.InteractionControl", "Microsoft.Crm.UnifiedServiceDesk.InteractionControl.CrmUtilControl", false, true, false, false, false, sCRMWindowHostType, false);
                break;

            case 803750012: // USD Session Lines
                USDBasic(forceUpdate, "SessionExplorerPanel", "Microsoft.Crm.UnifiedServiceDesk.Dynamics", "Microsoft.Crm.UnifiedServiceDesk.Dynamics.SessionOverview", false, false, false, true, false, sCRMWindowHostType, false);
                break;
            case 803750013: // USD Tree Bar
                USDBasic(forceUpdate, "SessionExplorerPanel", "Microsoft.Crm.UnifiedServiceDesk.Dynamics", "Microsoft.Crm.UnifiedServiceDesk.Dynamics.TreeBar", false, null, false, true, false, sCRMWindowHostType, false);
                break;
            case 803750014: // CTI Desktop Manager
                USDBasic(forceUpdate, "HiddenPanel", null, null, false, true, false, true, false, sCRMWindowHostType, false);
                break;
            case 803750015: // USD Panel
                USDPanel(forceUpdate);
                break;
            case 803750016: // USD Debugger
                USDBasic(forceUpdate, "MainPanel", "Microsoft.Crm.UnifiedServiceDesk.Dynamics", "Microsoft.Crm.UnifiedServiceDesk.Dynamics.Debugger", true, true, false, true, false, sCRMWindowHostType, true);
                break;
            case 803750017: // USD Todo List
                USDBasic(forceUpdate, "LeftPanel1", "Microsoft.Crm.UnifiedServiceDesk.Dynamics", "Microsoft.Crm.UnifiedServiceDesk.Dynamics.TodoList", false, null, false, true, false, sCRMWindowHostType, false);
                break;
            case 803750018: // SSO
                USDBasic(forceUpdate, "HiddenPanel", "Microsoft.Crm.UnifiedServiceDesk.Sso", "Microsoft.Crm.UnifiedServiceDesk.Sso.Sso", false, true, false, false, false, sCRMWindowHostType, false);
                break;
            case 803750019: // KM Control
                USDBasic(forceUpdate, "MainPanel", "Microsoft.Crm.UnifiedServiceDesk.Dynamics", "Microsoft.Crm.UnifiedServiceDesk.Dynamics.Controls.KnowledgeManagement.KMControl", true, null, null, true, false, sCRMWindowHostType, true, true);
                break;
            case 803750020: // Listener Control
                USDBasic(forceUpdate, "HiddenPanel", null, null, null, false, false, false, false, sCRMWindowHostType, false, false);
                break;
            case 803750021: // ISH Control
                USDBasic(forceUpdate, "MainPanel", "Microsoft.Crm.UnifiedServiceDesk.Dynamics", "Microsoft.Crm.UnifiedServiceDesk.Dynamics.SPAControl", true, null, null, true, false, sCRMWindowHostType, true, true);
                break;
            case 803750022: // PopupNotification Control
                USDBasic(forceUpdate, "HiddenPanel", "Microsoft.Crm.UnifiedServiceDesk.Dynamics", "Microsoft.Crm.UnifiedServiceDesk.Dynamics.PopupNotification", true, null, false, false, false, sCRMWindowHostType, true);
                break;
        }
    }
}

function USDPanel(forceUpdate) {
    var panelType = Xrm.Page.data.entity.attributes.get("msdyusd_paneltype");
    if (panelType.getSelectedOption() != null)
        panelType = panelType.getSelectedOption().value;
    else
        panelType = 803750000;
    switch (panelType) {
        case 803750000:
            USDAdvanced(forceUpdate, "MainWorkArea", "Microsoft.Crm.UnifiedServiceDesk.Dynamics", "Microsoft.Crm.UnifiedServiceDesk.Dynamics.PanelLayouts.Standard", false, true, false, false, false, 803750015, panelType, true);
            break;
        case 803750001:
            USDAdvanced(forceUpdate, "MainWorkArea", null, null, null, null, false, true, false, 803750015, panelType, true);
            break;
        case 803750002:
            USDAdvanced(forceUpdate, "MainWorkArea", "Microsoft.Crm.UnifiedServiceDesk.Dynamics", "Microsoft.Crm.UnifiedServiceDesk.Dynamics.PanelLayouts.CRMConfigured", null, null, false, true, false, 803750015, panelType, true);
            break;
        case 803750003:
            USDAdvanced(forceUpdate, "MainWorkArea", "Microsoft.Crm.UnifiedServiceDesk.Dynamics", "Microsoft.Crm.UnifiedServiceDesk.Dynamics.PanelLayouts.RibbonLayout", false, true, false, false, false, 803750015, panelType, true);
            break;
        case 803750004:
            USDAdvanced(forceUpdate, "MainPanel", "Microsoft.Crm.UnifiedServiceDesk.Dynamics", "Microsoft.Crm.UnifiedServiceDesk.Dynamics.PanelLayouts.VerticalSplit", true, null, false, true, false, 803750015, panelType, true);
            break;
        case 803750005:
            USDAdvanced(forceUpdate, "MainPanel", "Microsoft.Crm.UnifiedServiceDesk.Dynamics", "Microsoft.Crm.UnifiedServiceDesk.Dynamics.PanelLayouts.HorizontalSplit", true, null, false, true, false, 803750015, panelType, true);
            break;
    }
}

function USDBasic(forceUpdate, panelName, assembly, className, dynamic, isGlobalApplication, allowMultiplePages, editPanelName, isGlobalManager, sCRMWindowHostType, dynamicVisible, showWebHostingType, showAdapterConfiguration) {
	USDAdvanced(forceUpdate, panelName, assembly, className, dynamic, isGlobalApplication, allowMultiplePages, editPanelName, isGlobalManager, sCRMWindowHostType, 0, dynamicVisible, showWebHostingType, showAdapterConfiguration);
}

function USDAdvanced(forceUpdate, panelName, assembly, className, dynamic, isGlobalApplication, allowMultiplePages, editPanelName, isGlobalManager, sCRMWindowHostType, panelType, dynamicVisible, showWebHostingType, showAdapterConfiguration) {
    if(showAdapterConfiguration == null || typeof(showAdapterConfiguration) == 'undefined')
		showAdapterConfiguration = false;

	// initialize form
    var generalTab = Xrm.Page.ui.tabs.get("{c0330026-8257-4219-8bc2-998017d1b6f7}");
    generalTab.setVisible(true);  // always visible!
    // skip {04ca7cc4-d509-470c-a65b-76067e429b6e} - General Section
    generalTab.sections.get("{c0330026-8257-4219-8bc2-998017d1b6f7}_section_7").setVisible(true);  // Unified Service Desk Section
    Xrm.Page.ui.controls.get("msdyusd_savedurl").setVisible(false);
    Xrm.Page.ui.controls.get("msdyusd_allowmultiplepages").setVisible(false);
    Xrm.Page.ui.controls.get("msdyusd_maximumbrowsers").setVisible(false);
    Xrm.Page.ui.controls.get("msdyusd_dashboardname").setVisible(false);
    generalTab.sections.get("{4b8adb5c-c3d8-de11-a899-00155d289c61}").setVisible(false);  // Hosted App Type
    Xrm.Page.ui.controls.get("uii_hostedapplicationtype").setVisible(false);
    generalTab.sections.get("usdpanel").setVisible(false);  // USD Panel
    Xrm.Page.ui.controls.get("msdyusd_paneltype").setVisible(false);
    Xrm.Page.ui.controls.get("msdyusd_xaml").setVisible(false);
    generalTab.sections.get("{badd49e5-c3d8-de11-a899-00155d289c61}").setVisible(false);  // Common Properties
    Xrm.Page.ui.controls.get("uii_isglobalapplication").setVisible(false);
    Xrm.Page.ui.controls.get("uii_isdependentonworkflow").setVisible(false);
    Xrm.Page.ui.controls.get("uii_displaygroup").setVisible(false);
    Xrm.Page.ui.controls.get("uii_minimumsizex").setVisible(false);
    Xrm.Page.ui.controls.get("uii_minimumsizey").setVisible(false);
    Xrm.Page.ui.controls.get("uii_optimalsizex").setVisible(false);
    Xrm.Page.ui.controls.get("uii_optimalsizey").setVisible(false);
    generalTab.sections.get("{c6d143eb-c3d8-de11-a899-00155d289c61}").setVisible(showAdapterConfiguration);  // Adapter Configuration
    Xrm.Page.ui.controls.get("uii_adaptermode").setVisible(showAdapterConfiguration);
    Xrm.Page.ui.controls.get("uii_adapteruri").setVisible(showAdapterConfiguration);
    Xrm.Page.ui.controls.get("uii_adaptertype").setVisible(showAdapterConfiguration);
    generalTab.sections.get("{c7d143eb-c3d8-de11-a899-00155d289c61}").setVisible(false);  // dynamic
    Xrm.Page.ui.controls.get("uii_isappdynamic").setVisible(false);
    Xrm.Page.ui.controls.get("uii_usercanclose").setVisible(false);
    Xrm.Page.ui.controls.get("uii_isshowintoolbardropdown").setVisible(false);
    Xrm.Page.ui.controls.get("msdyusd_hostingtype").setVisible(false);
	Xrm.Page.ui.controls.get("msdyusd_prefetchdata").setVisible(false);

    var hostingTab = Xrm.Page.ui.tabs.get("Hosting");
    hostingTab.setVisible(false);
    hostingTab.sections.get("ExternalApplicationSettings").setVisible(false);
    Xrm.Page.ui.controls.get("uii_externalappuri").setVisible(false);
    Xrm.Page.ui.controls.get("uii_externalapparguments").setVisible(false);
    Xrm.Page.ui.controls.get("uii_externalappworkingdirectory").setVisible(false);
    Xrm.Page.ui.controls.get("uii_managehosting").setVisible(false);
    hostingTab.sections.get("ApplicationHosting").setVisible(false);
    Xrm.Page.ui.controls.get("uii_applicationhostingmode").setVisible(false);
    Xrm.Page.ui.controls.get("uii_isattachinputthread").setVisible(false);
    Xrm.Page.ui.controls.get("uii_isshowmenu").setVisible(false);
    Xrm.Page.ui.controls.get("uii_isnomessagepump").setVisible(false);
    Xrm.Page.ui.controls.get("uii_mainwindowacquisitiontimeout").setVisible(false);
    hostingTab.sections.get("AlternateTopLevelWindow").setVisible(false);
    Xrm.Page.ui.controls.get("uii_toplevelwindowmode").setVisible(false);
    Xrm.Page.ui.controls.get("uii_toplevelwindowcaption").setVisible(false);
    Xrm.Page.ui.controls.get("uii_findwindowclass").setVisible(false);
    Xrm.Page.ui.controls.get("uii_islimittoprocess").setVisible(false);
    hostingTab.sections.get("WebApplicationHomePage").setVisible(false);
    Xrm.Page.ui.controls.get("uii_webappurl").setVisible(false);
    Xrm.Page.data.entity.attributes.get("uii_webappurl").setRequiredLevel("none");
    Xrm.Page.ui.controls.get("uii_iswebappusetoolbar").setVisible(false);
    Xrm.Page.ui.controls.get("uii_isusenewbrowserprocess").setVisible(false);
    Xrm.Page.ui.controls.get("uii_managepopups").setVisible(false);
    hostingTab.sections.get("AssemblyInfo").setVisible(false);
    Xrm.Page.ui.controls.get("uii_assemblyuri").setVisible(false);
    Xrm.Page.ui.controls.get("uii_assemblytype").setVisible(false);
    hostingTab.sections.get("DynamicPositioningAttributes").setVisible(false);
    Xrm.Page.ui.controls.get("uii_isretainframeandcaption").setVisible(false);
    Xrm.Page.ui.controls.get("uii_isretainontaskbar").setVisible(false);
    Xrm.Page.ui.controls.get("uii_isretainsystemmenu").setVisible(false);
    Xrm.Page.ui.controls.get("uii_isrestoreifminimized").setVisible(false);
    Xrm.Page.ui.controls.get("uii_removesizingcontrols").setVisible(false);
    hostingTab.sections.get("CitrixApplicationSettings").setVisible(false);
    Xrm.Page.ui.controls.get("uii_icafilename").setVisible(false);
    Xrm.Page.ui.controls.get("uii_remote_processacquisitionattempts").setVisible(false);
    Xrm.Page.ui.controls.get("uii_remote_processacquisitiondelay").setVisible(false);
    Xrm.Page.ui.controls.get("uii_processacquisitionfilename").setVisible(false);
	Xrm.Page.ui.controls.get("uii_isappdynamic").setDisabled(false);

    var translationTab = Xrm.Page.ui.tabs.get("TranslationServices");
    translationTab.setVisible(false);

    var automationTab = Xrm.Page.ui.tabs.get("{9c1adb13-c3d8-de11-a899-00155d289c61}");
    automationTab.setVisible(false);  // automation
    automationTab.sections.get("{08b2d4ae-c5d8-de11-a899-00155d289c61}").setVisible(false); // Automation Bindings
    Xrm.Page.ui.controls.get("uii_automationxml").setVisible(false);

    if (typeof (sCRMWindowHostType) != "undefined" && sCRMWindowHostType != null)
    {
        if(sCRMWindowHostType == 803750005 || sCRMWindowHostType == 803750022 || sCRMWindowHostType == 803750009 ) // For Hosted Control and PopupNotification Control, always display Extensions tab
        {
            DisplayExtensions(true);
        }
        else 
        {
            DisplayExtensions(false);
        }
    }


    ////////////////////////////////
    // Get the Application type
    var oHAType = Xrm.Page.data.entity.attributes.get("uii_hostedapplicationtype");
    oHAType.setValue(1);	// Hosted Control

    var displayGroupValue = Xrm.Page.data.entity.attributes.get("uii_displaygroup").getValue();
    if (displayGroupValue == "" || displayGroupValue == null || forceUpdate)
        Xrm.Page.data.entity.attributes.get("uii_displaygroup").setValue(panelName);
    if (editPanelName == null || editPanelName == true) {
        generalTab.setVisible(true);
        generalTab.sections.get("{badd49e5-c3d8-de11-a899-00155d289c61}").setVisible(true);
        Xrm.Page.ui.controls.get("uii_displaygroup").setVisible(true);
    }
    if (forceUpdate && !showAdapterConfiguration)
        Xrm.Page.data.entity.attributes.get("uii_adaptermode").setValue(1);	// Use No Adapter
    if (isGlobalApplication == null) {
        generalTab.setVisible(true);
        generalTab.sections.get("{badd49e5-c3d8-de11-a899-00155d289c61}").setVisible(true);
        Xrm.Page.ui.controls.get("uii_isglobalapplication").setVisible(true);
    }
    else if (forceUpdate) {
        Xrm.Page.data.entity.attributes.get("uii_isglobalapplication").setValue(isGlobalApplication);
    }
    if (forceUpdate) {
        Xrm.Page.data.entity.attributes.get("uii_usercanclose").setValue(false);
    }
	if (forceUpdate && !showAdapterConfiguration) {
        Xrm.Page.data.entity.attributes.get("uii_adapteruri").setValue("");
        Xrm.Page.data.entity.attributes.get("uii_adaptertype").setValue("");
    }
    if (dynamicVisible == true) {
        generalTab.setVisible(true);
        generalTab.sections.get("{c7d143eb-c3d8-de11-a899-00155d289c61}").setVisible(true);
        Xrm.Page.ui.controls.get("uii_isappdynamic").setVisible(true);
        Xrm.Page.ui.controls.get("uii_usercanclose").setVisible(true);
    }
    if (dynamic != null && forceUpdate) {
        Xrm.Page.data.entity.attributes.get("uii_isappdynamic").setValue(dynamic);
		Xrm.Page.data.entity.attributes.get("uii_usercanclose").setValue(dynamic);
    }
    if (forceUpdate)
        Xrm.Page.data.entity.attributes.get("uii_isshowintoolbardropdown").setValue(false);
    if (assembly == null || className == null) {
        hostingTab.setVisible(true);
        hostingTab.sections.get("AssemblyInfo").setVisible(true);
        Xrm.Page.ui.controls.get("uii_assemblyuri").setVisible(true);
        Xrm.Page.ui.controls.get("uii_assemblytype").setVisible(true);
		if(sCRMWindowHostType == 803750020) // Listener Control
		{
			Xrm.Page.data.entity.attributes.get("uii_assemblytype").setRequiredLevel("required");
			Xrm.Page.data.entity.attributes.get("uii_assemblyuri").setRequiredLevel("required");			
		}
    }
    if (assembly != null && forceUpdate) {
        Xrm.Page.data.entity.attributes.get("uii_assemblyuri").setValue(assembly);
    }
    if (className != null && forceUpdate) {
        Xrm.Page.data.entity.attributes.get("uii_assemblytype").setValue(className);
    }
    if (allowMultiplePages == null) {
        Xrm.Page.ui.controls.get("msdyusd_allowmultiplepages").setVisible(true);
       AllowMultiplePagesChanged();
    }
    else if (forceUpdate) {
        Xrm.Page.data.entity.attributes.get("msdyusd_allowmultiplepages").setValue(allowMultiplePages);
    }

    if (isGlobalManager == true) {
        translationTab.setVisible(true);
    }

   if (showWebHostingType != null && showWebHostingType == true)
    {
      Xrm.Page.ui.controls.get("msdyusd_hostingtype").setVisible(true);
	}

				// special case scenarios
    if (sCRMWindowHostType != null) {
        if (sCRMWindowHostType == 803750013) { 

        }
    else if (sCRMWindowHostType == 803750015) {
		generalTab.sections.get("usdpanel").setVisible(true);  // USD Panel
		Xrm.Page.ui.controls.get("msdyusd_paneltype").setVisible(true);
		if (panelType == 803750000
			 || panelType == 803750003) {   // USD Panel Layout
			 	// Nothing to do here.
			 }
            else if (panelType == 803750001) {
			hostingTab.setVisible(true);
			hostingTab.sections.get("AssemblyInfo").setVisible(true);
			Xrm.Page.ui.controls.get("uii_assemblyuri").setVisible(true);
                Xrm.Page.ui.controls.get("uii_assemblytype").setVisible(true);
                }
                else if (panelType == 803750002) {
                Xrm.Page.ui.controls.get("msdyusd_xaml").setVisible(true);
            }
        }
        //If it is ISH Control, then disable hosting type and default its value to IE Process
        var ohostingType = Xrm.Page.getControl("msdyusd_hostingtype");
        if (sCRMWindowHostType == 803750021) {
            Xrm.Page.data.entity.attributes.get("msdyusd_hostingtype") !=null
                ? Xrm.Page.data.entity.attributes.get("msdyusd_hostingtype").setValue(803750001) : "" ;
            ohostingType !=null ? ohostingType.setDisabled(true) : "";
        }
        else
        {
            ohostingType !=null ? ohostingType.setDisabled(false) : "";
        }

        if (sCRMWindowHostType == 803750022) {
            Xrm.Page.ui.controls.get("uii_usercanclose").setVisible(false);
            Xrm.Page.data.entity.attributes.get("uii_usercanclose").setValue(false);
			Xrm.Page.data.entity.attributes.get("uii_isappdynamic").setValue(true);
			Xrm.Page.ui.controls.get("uii_isappdynamic").setDisabled(true);
        }

		if (sCRMWindowHostType == 803750003)  
		{
			Xrm.Page.ui.controls.get("msdyusd_prefetchdata").setVisible(true);
		}
    }
}

function SpecifyUrlOnChange() {
    CRMWindowHostTypeOnChange();
}

function ToolbarOnLoad(execCon) {
    var XRM_FORM_TYPE_CREATE = 1;
    var XRM_FORM_TYPE_UPDATE = 2;
    var XRM_FORM_TYPE_READONLY = 3;
    var XRM_FORM_TYPE_DISABLED = 4;

    exCon = execCon;
    formContext = exCon.getFormContext();

    switch (Xrm.Page.ui.getFormType()) {
        case XRM_FORM_TYPE_CREATE:
        case XRM_FORM_TYPE_UPDATE:
            Xrm.Page.data.entity.attributes.get("msdyusd_autoload").fireOnChange();
            break;
        case XRM_FORM_TYPE_READONLY:
        case XRM_FORM_TYPE_DISABLED:
            OnAutoLoadChange();
            break;
    }
}

function AllowMultiplePagesChanged() {
   if (Xrm.Page.data.entity.attributes.get("msdyusd_allowmultiplepages").getValue() == true)
       Xrm.Page.ui.controls.get("msdyusd_maximumbrowsers").setVisible(true);
   else
       Xrm.Page.ui.controls.get("msdyusd_maximumbrowsers").setVisible(false);
}

function OnAutoLoadChange() {
    var oAutoLoad = Xrm.Page.data.entity.attributes.get("msdyusd_autoload");
    if (oAutoLoad.getSelectedOption() != null
         && oAutoLoad.getSelectedOption().value == 803750003) {  // Browser Window with Toolbar
        SetToolbarVisible(true);
    }
    else {
        SetToolbarVisible(false);
    }
}

function SetToolbarVisible(vis) {
    var items = Xrm.Page.ui.navigation.items.get();
    for (var i in items) {
        var item = items[i];
        var itemId = item.getId();
        if (itemId == "navmsdyusd_msdyusd_toolbarstrip_uii_hostedapplication")
            item.setVisible(vis);
        //alert(itemLabel + ':' + itemId );
    }
}

function DisplayExtensions(show)
{
    var extensionsTab = Xrm.Page.ui.tabs.get("{b49ba11c-c3d8-de11-a899-00155d289c61}");
    extensionsTab.setVisible(show);  // extensions
    extensionsTab.sections.get("{4ef881cf-c5d8-de11-a899-00155d289c61}").setVisible(show); // Extensions
    Xrm.Page.ui.controls.get("uii_extensionsxml").setVisible(show);
}