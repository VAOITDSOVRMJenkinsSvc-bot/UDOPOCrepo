"use strict";

var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};

var confirmStrings = {};
confirmStrings.confirmButtonLabel = "Yes";
confirmStrings.cancelButtonLabel = "No";
var confirmOptions = { height: 300, width: 600 };
var webApi = null;
window.parent.USDEncode = USDEncode;
var globalCon = Xrm.Utility.getGlobalContext();

function ConvertDate(date) {
    var convertDate = new Date(date);

    var curr_date = convertDate.getDate();
    var curr_month = convertDate.getMonth() + 1;
    var curr_year = convertDate.getFullYear();

    curr_date = (curr_date < 10) ? '0' + curr_date : curr_date;

    return curr_month + "/" + curr_date + "/" + curr_year;
}

function USDEncode(context, paymentId, radDate) {
    try {
        var context = Xrm.Utility.getGlobalContext();
        var part2 = encodeURIComponent("&id=" + paymentId + "&rad=" + ConvertDate(radDate));
        var url = context.getClientUrl() + "/WebResources/udo_payments_debtsDMC?data=" + part2;
        return url;
    }
    catch (e) {
        return "error"
    }
}

function paymentAdjustmentsTabChange(exCon) {
    instantiateCommonScripts(exCon);
    var tab = formHelper.getTab("paymentAdjustmentsTab");
    if (tab !== null) {
        if (tab.getDisplayState !== "collapsed") {
            var section = tab.sections.get("paymentAdjustmentsSection");

            if (section !== null) {
                section.setVisible(true);
            }

        }
    }
}

function awardAdjustmentsTabChange(exCon) {
    instantiateCommonScripts(exCon);
    var tab = formHelper.getTab("awardAdjustmentsTab");
    if (tab !== null) {
        if (tab.getDisplayState !== "collapsed") {
            var section = tab.sections.get("awardAdjustmentsSection");

            if (section !== null) {
                section.setVisible(true);
            }
        }
    }
}
