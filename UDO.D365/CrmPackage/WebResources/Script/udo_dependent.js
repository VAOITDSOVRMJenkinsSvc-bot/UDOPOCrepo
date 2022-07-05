"use strict";

function DependentsDetailTabChange(context) {
    var tab = context.ui.tabs.get("Dependents");

    if (tab !== null) {
        if (tab.getDisplayState !== "collapsed") {
            var section = tab.sections.get("dependents");

            if (section !== null) {
                section.setVisible(true);
            }
        }
    }
}

function RelationshipsDetailTabChange(context) {
    var tab = context.ui.tabs.get("Relationships");

    if (tab !== null) {
        if (tab.getDisplayState !== "collapsed") {
            var section = tab.sections.get("relationships");

            if (section !== null) {
                section.setVisible(true);
            }
        }
    }
}

function FlashesDetailTabChange(context) {
    var tab = context.ui.tabs.get("Flashes");

    if (tab !== null) {
        if (tab.getDisplayState !== "collapsed") {
            var section = tab.sections.get("flashes");

            if (section !== null) {
                section.setVisible(true);
            }
        }
    }
}

function POADetailTabChange(context) {
    var tab = context.ui.tabs.get("POA");

    if (tab !== null) {
        if (tab.getDisplayState !== "collapsed") {
            var section = tab.sections.get("PastPOA");

            if (section !== null) {
                section.setVisible(true);
            }
        }
    }
}

function FiduciaryDetailTabChange(context) {
    var tab = context.ui.tabs.get("Fiduciary");

    if (tab !== null) {
        if (tab.getDisplayState !== "collapsed") {
            var section = tab.sections.get("PastFiduciaries");

            if (section !== null) {
                section.setVisible(true);
            }
        }
    }
}
function AwardsDetailTabChange(context) {
    var tab = context.ui.tabs.get("Awards");

    if (tab !== null) {
        if (tab.getDisplayState !== "collapsed") {
            var section = tab.sections.get("awards");

            if (section !== null) {
                section.setVisible(true);
            }
        }
    }
}
function ClaimsDetailTabChange(context) {
    var tab = context.ui.tabs.get("Claims");

    if (tab !== null) {
        if (tab.getDisplayState !== "collapsed") {
            var section = tab.sections.get("claims");

            if (section !== null) {
                section.setVisible(true);
            }
        }
    }
}