"use strict";

var lib;
var webApi;
var formHelper;

function onLoadUDO(exCon) {
    UDO.Shared.GetFormContext(exCon);
    // Run GetAddresses for VeteranProfile
    UDO.CustomActions.OnLoad(exCon, "contact", "udo_addressescomplete", "udo_GetAddresses", "Addresses");
}

function onLoad(exCon) {
    // Run GetAddresses for VeteranProfile
    UDO.CustomActions.OnLoad(exCon, "contact", "udo_addressescomplete", "udo_GetAddresses", "Addresses");
}

function instantiateCommonScripts(exCon) {
    lib = new CrmCommonJS.CrmCommon(version, exCon);
    webApi = lib.WebApi;
    formHelper = new CrmCommonJS.FormHelper(exCon);
}

function onLoadDoD(exCon) {
    instantiateCommonScripts(exCon);
    var DOD = formHelper.getAttribute("udo_dateofdeath");

    if (DOD != null) {
        var DODValue = DOD.getValue();
        if (DODValue != null) {
            formHelper.setFormNotification("This Veteran was Deceased on " + DODValue, "WARNING", "dodmessage");
        }
    }
}

function onLoadDoDUDO(executionContext) {
    instantiateCommonScripts(executionContext);

    UDO.Shared.GetFormContext(executionContext);
    var DOD = UDO.Shared.GetFieldValue("udo_dateofdeath");
    if (DOD !== null) {
        UDO.Shared.FormContext.ui.setFormNotification("This Veteran was Deceased on " + DOD, "WARNING", "dodmessage");
    }
}

function AddressDetailTabChange(exCon) {
    instantiateCommonScripts(exCon);
    var tab = formHelper.getTab("VeteranDetails");
    if (tab !== null) {
        if (tab.getDisplayState !== "collapsed") {
            var section = tab.sections.get("address");

            if (section !== null) {
                section.setVisible(true);
            }
        }
    }
}

function DependentsDetailTabChange(exCon) {
    instantiateCommonScripts(exCon);
    var tab = formHelper.getTab("Dependents");

    if (tab !== null) {
        if (tab.getDisplayState !== "collapsed") {
            var section = tab.sections.get("dependents");

            if (section !== null) {
                section.setVisible(true);
            }
        }
    }
}

function RelationshipsDetailTabChange(exCon) {
    instantiateCommonScripts(exCon);
    var tab = formHelper.getTab("Relationships");

    if (tab !== null) {
        if (tab.getDisplayState !== "collapsed") {
            var section = tab.sections.get("relationships");

            if (section !== null) {
                section.setVisible(true);
            }
        }
    }
}

function FlashesDetailTabChange(exCon) {
    instantiateCommonScripts(exCon);
    var tab = formHelper.getTab("Flashes");

    if (tab !== null) {
        if (tab.getDisplayState !== "collapsed") {
            var section = tab.sections.get("flashes");

            if (section !== null) {
                section.setVisible(true);
            }
        }
    }
}

function POADetailTabChange(exCon) {
    instantiateCommonScripts(exCon);
    var tab = formHelper.getTab("POA");

    if (tab !== null) {
        if (tab.getDisplayState !== "collapsed") {
            var section = tab.sections.get("PastPOA");

            if (section !== null) {
                section.setVisible(true);
            }
        }
    }
}

function FiduciaryDetailTabChange(exCon) {
    instantiateCommonScripts(exCon);
    var tab = formHelper.getTab("Fiduciary");

    if (tab !== null) {
        if (tab.getDisplayState !== "collapsed") {
            var section = tab.sections.get("PastFiduciaries");

            if (section !== null) {
                section.setVisible(true);
            }
        }
    }
}

function PhoneCallTabChange(exCon) {
    instantiateCommonScripts(exCon);
    var tab = formHelper.getTab("phonecall");

    if (tab !== null) {
        if (tab.getDisplayState !== "collapsed") {
            var section = tab.sections.get("pc1");

            if (section !== null) {
                section.setVisible(true);
            }
            section = tab.sections.get("pc2");

            if (section !== null) {
                section.setVisible(true);
            }
        }
    }
}
function SRTabChange(exCon) {
    instantiateCommonScripts(exCon);
    var tab = formHelper.getTab("sr");

    if (tab !== null) {
        if (tab.getDisplayState !== "collapsed") {
            var section = tab.sections.get("sr1");

            if (section !== null) {
                section.setVisible(true);
            }
            section = tab.sections.get("sr2");

            if (section !== null) {
                section.setVisible(true);
            }
        }
    }
}
function ITFTabChange(exCon) {
    instantiateCommonScripts(exCon);
    var tab = formHelper.getTab("itf");

    if (tab !== null) {
        if (tab.getDisplayState !== "collapsed") {
            var section = tab.sections.get("itf1");

            if (section !== null) {
                section.setVisible(true);
            }
            section = tab.sections.get("itf2");

            if (section !== null) {
                section.setVisible(true);
            }
        }
    }
}


function CADDTabChange(exCon) {
    instantiateCommonScripts(exCon);
    var tab = formHelper.getTab("cadd");

    if (tab !== null) {
        if (tab.getDisplayState !== "collapsed") {
            var section = tab.sections.get("cadd1");

            if (section !== null) {
                section.setVisible(true);
            }
            section = tab.sections.get("cadd2");

            if (section !== null) {
                section.setVisible(true);
            }
        }
    }
}
function CHATTabChange(exCon) {
    instantiateCommonScripts(exCon);
    var tab = formHelper.getTab("chat");

    if (tab !== null) {
        if (tab.getDisplayState !== "collapsed") {
            var section = tab.sections.get("chat1");

            if (section !== null) {
                section.setVisible(true);
            }
            section = tab.sections.get("chat2");

            if (section !== null) {
                section.setVisible(true);
            }
        }
    }
}
function ADTabChange(exCon) {
    instantiateCommonScripts(exCon);
    var tab = formHelper.getTab("ad");

    if (tab !== null) {
        if (tab.getDisplayState !== "collapsed") {
            var section = tab.sections.get("ad1");

            if (section !== null) {
                section.setVisible(true);
            }
            section = tab.sections.get("ad2");

            if (section !== null) {
                section.setVisible(true);
            }
        }
    }
}
function FNODTabChange(exCon) {
    instantiateCommonScripts(exCon);
    var tab = formHelper.getTab("fnod");

    if (tab !== null) {
        if (tab.getDisplayState !== "collapsed") {
            var section = tab.sections.get("fnod1");

            if (section !== null) {
                section.setVisible(true);
            }
            section = tab.sections.get("fnod2");

            if (section !== null) {
                section.setVisible(true);
            }
        }
    }
}

function SaveVeteran(context) {
    context.save().then(InitiateScan(context));
}

function InitiateScan(context)
{
    var createdOn= context.getAttribute('createdon');

    if (createdOn !== null) {
             window.open("http://uii/Veteran/ScanForDataParameters");
    }
    else {
        context.data.refresh(false);
            window.setTimeout('InitiateScan()', 2000);

    }
}