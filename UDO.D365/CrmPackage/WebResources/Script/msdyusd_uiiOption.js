var exCon = null;
var formContext = null;

function GlobalOptionChanged() {
    var globaloption = Xrm.Page.getAttribute("msdyusd_globaloption");
    //Others->1000000040
    //HelpToImprove->100000002
    if (globaloption != null && globaloption.getSelectedOption().value < 1000000040) {
        if (globaloption.getSelectedOption().value == 100000002) {
            Xrm.Page.getControl("IFRAME_PrivacyHelp").setVisible(true);
            SetFrameSrc();
        }
        else {
            Xrm.Page.getControl("IFRAME_PrivacyHelp").setVisible(false);
        }
        Xrm.Page.getControl("uii_name").setDisabled(true);
        Xrm.Page.getAttribute("uii_value").setValue("");
        Xrm.Page.getAttribute("uii_name").setValue(globaloption.getSelectedOption().text);
    }
    else {
        Xrm.Page.ui.controls.get("IFRAME_PrivacyHelp").setVisible(false);
        Xrm.Page.getAttribute("uii_name").setValue("");
        Xrm.Page.getAttribute("uii_value").setValue("");
        Xrm.Page.getControl("uii_name").setDisabled(false);
    }
};


function OnFormLoad(execCon) {
    exCon = execCon;
    formContext = exCon.getFormContext();
    var globaloption = Xrm.Page.getAttribute("msdyusd_globaloption");
    if (globaloption != null) {

        if (globaloption.getSelectedOption().value == 100000002) {
            Xrm.Page.ui.controls.get("IFRAME_PrivacyHelp").setVisible(true);
            SetFrameSrc();
        }
        else {
            Xrm.Page.ui.controls.get("IFRAME_PrivacyHelp").setVisible(false);
        }

        if(window.parent.IS_ONPREMISE && globaloption.getOption(100000002) != null )
        {
            Xrm.Page.getControl("msdyusd_globaloption").removeOption(100000002)
        }
    }
};

function SetFrameSrc() {
    var IFrame = Xrm.Page.ui.controls.get("IFRAME_PrivacyHelp");
    var serverUrl = Xrm.Page.context.getClientUrl();
    var orgName = Xrm.Page.context.getOrgUniqueName();
    var url = serverUrl + "/WebResources/msdyusd_/PrivacyLink.html";
    IFrame.setSrc(url);
}