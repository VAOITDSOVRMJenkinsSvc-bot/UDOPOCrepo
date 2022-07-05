var exCon = null;
var formContext = null;

function onLoad(execCon) {
    exCon = execCon;
    formContext = exCon.getFormContext();
    // Run GetAddresses for VeteranProfile
    UDO.CustomActions.OnActionParams("contact", "udo_addressescomplete", "udo_GetAddresses", "Addresses");
}


function onLoadTab(execCon) {
    exCon = execCon;
    formContext = exCon.getFormContext();
    var currentForm = Xrm.Page.ui.formSelector.getCurrentItem();
    if (currentForm.getLabel() == "Veteran Profile") return;

    var forms = Xrm.Page.ui.formSelector.items.get();
    for (i = 0; i < forms.length; i++) {
        if (forms[i].getLabel() == "Veteran Profile") {
            forms[i].navigate();
            return;
        }
    }
}

function interactionHistoryTabOnChange(executionObj) {
    var eventSource = executionObj.getEventSource();
    if (eventSource.getVisible() && eventSource.getDisplayState() == "expanded") {

        var select = "crme_ihfrom,crme_ihto,crme_ihsource";
        var expand = null;
        var currentUserId = Xrm.Page.context.getUserId();

        SDK.REST.retrieveRecord(currentUserId, "SystemUser", select, expand,
            function(obj) { successCallBack(obj) },
             function (err) { errorCallBack(err) });
    }
}

function successCallBack(response) {
    ///GET ATTRIBUTES
    //crme_ihfrom
    //crme_ihto
    //crme_ihsource

    ///SET ATTRIBUTES
    //crme_ihfrom
    //crme_ihto
    //crme_ihlobs

    var ihFromControl = Xrm.Page.getAttribute("crme_ihfrom");
    var ihToControl = Xrm.Page.getAttribute("crme_ihto");
    var ihLOBControl = Xrm.Page.getAttribute("crme_ihlobs");


    if (ihFromControl != null && response["crme_ihfrom"] != null)
        ihFromControl.setValue(response["crme_ihfrom"]);

    if (ihToControl != null && response["crme_ihto"] != null)
        ihToControl.setValue(response["crme_ihto"]);

    if (ihLOBControl != null && response["crme_ihsource"] != null)
        ihLOBControl.setValue(response["crme_ihsource"]);
}

function errorCallBack(error) {

}

function onLoadDoD(execCon) {
    exCon = execCon;
    formContext = exCon.getFormContext();
    var DOD = Xrm.Page.getAttribute("udo_dateofdeath");

    if (DOD != null) {
        var DODValue = DOD.getValue();
        if (DODValue != null) {
            Xrm.Page.ui.setFormNotification("This Veteran was Deceased on " + DODValue, "WARNING", "dodmessage");
        }
    }
}
function AddressDetailTabChange() {
    var tab = Xrm.Page.ui.tabs.get("VeteranDetails");
    if (tab != null) {
        if (tab.getDisplayState != "collapsed") {
            var section = tab.sections.get("address");

            if (section != null) {
                section.setVisible(true);
            }
        }
    }
}

function DependentsDetailTabChange() {
    var tab = Xrm.Page.ui.tabs.get("Dependents");

    if (tab != null) {
        if (tab.getDisplayState != "collapsed") {
            var section = tab.sections.get("dependents");

            if (section != null) {
                section.setVisible(true);
            }
        }
    }
}

function RelationshipsDetailTabChange() {
    var tab = Xrm.Page.ui.tabs.get("Relationships");

    if (tab != null) {
        if (tab.getDisplayState != "collapsed") {
            var section = tab.sections.get("relationships");

            if (section != null) {
                section.setVisible(true);
            }
        }
    }
}

function FlashesDetailTabChange() {
    var tab = Xrm.Page.ui.tabs.get("Flashes");

    if (tab != null) {
        if (tab.getDisplayState != "collapsed") {
            var section = tab.sections.get("flashes");

            if (section != null) {
                section.setVisible(true);
            }
        }
    }
}

function POADetailTabChange() {
    var tab = Xrm.Page.ui.tabs.get("POA");

    if (tab != null) {
        if (tab.getDisplayState != "collapsed") {
            var section = tab.sections.get("PastPOA");

            if (section != null) {
                section.setVisible(true);
            }
        }
    }
}

function FiduciaryDetailTabChange() {
    var tab = Xrm.Page.ui.tabs.get("Fiduciary");

    if (tab != null) {
        if (tab.getDisplayState != "collapsed") {
            var section = tab.sections.get("PastFiduciaries");

            if (section != null) {
                section.setVisible(true);
            }
        }
    }
}

function PhoneCallTabChange() {
    var tab = Xrm.Page.ui.tabs.get("phonecall");

    if (tab != null) {
        if (tab.getDisplayState != "collapsed") {
            var section = tab.sections.get("pc1");

            if (section != null) {
                section.setVisible(true);
            }
            section = tab.sections.get("pc2");

            if (section != null) {
                section.setVisible(true);
            }
        }
    }
}
function SRTabChange() {
    var tab = Xrm.Page.ui.tabs.get("sr");

    if (tab != null) {
        if (tab.getDisplayState != "collapsed") {
            var section = tab.sections.get("sr1");

            if (section != null) {
                section.setVisible(true);
            }
            section = tab.sections.get("sr2");

            if (section != null) {
                section.setVisible(true);
            }
        }
    }
}
function ITFTabChange() {
    var tab = Xrm.Page.ui.tabs.get("itf");

    if (tab != null) {
        if (tab.getDisplayState != "collapsed") {
            var section = tab.sections.get("itf1");

            if (section != null) {
                section.setVisible(true);
            }
            section = tab.sections.get("itf2");

            if (section != null) {
                section.setVisible(true);
            }
        }
    }
}


function CADDTabChange() {
    var tab = Xrm.Page.ui.tabs.get("cadd");

    if (tab != null) {
        if (tab.getDisplayState != "collapsed") {
            var section = tab.sections.get("cadd1");

            if (section != null) {
                section.setVisible(true);
            }
            section = tab.sections.get("cadd2");

            if (section != null) {
                section.setVisible(true);
            }
        }
    }
}
function CHATTabChange() {
    var tab = Xrm.Page.ui.tabs.get("chat");

    if (tab != null) {
        if (tab.getDisplayState != "collapsed") {
            var section = tab.sections.get("chat1");

            if (section != null) {
                section.setVisible(true);
            }
            section = tab.sections.get("chat2");

            if (section != null) {
                section.setVisible(true);
            }
        }
    }
}
function ADTabChange() {
    var tab = Xrm.Page.ui.tabs.get("ad");

    if (tab != null) {
        if (tab.getDisplayState != "collapsed") {
            var section = tab.sections.get("ad1");

            if (section != null) {
                section.setVisible(true);
            }
            section = tab.sections.get("ad2");

            if (section != null) {
                section.setVisible(true);
            }
        }
    }
}
function FNODTabChange() {
    var tab = Xrm.Page.ui.tabs.get("fnod");

    if (tab != null) {
        if (tab.getDisplayState != "collapsed") {
            var section = tab.sections.get("fnod1");

            if (section != null) {
                section.setVisible(true);
            }
            section = tab.sections.get("fnod2");

            if (section != null) {
                section.setVisible(true);
            }
        }
    }
}