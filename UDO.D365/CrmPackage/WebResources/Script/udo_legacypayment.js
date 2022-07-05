"use strict"; 

function ConvertDate(date) {
    var convertDate = new Date(date);

    var curr_date = convertDate.getDate();
    var curr_month = convertDate.getMonth() + 1;
    var curr_year = convertDate.getFullYear();

    curr_date = (curr_date < 10) ? '0' + curr_date : curr_date;

    return curr_month + "/" + curr_date + "/" + curr_year;
}

function generateURL(paymentId, folderloc, radDate, isLegacy) {
    try {
        var context = Xrm.Utility.getGlobalContext();
        var part2 = encodeURIComponent("&id=" + paymentId + "&roj=" + folderloc + "&rad=" + ConvertDate(radDate) + "&isLegacyPayment=" + isLegacy);
        var url = context.getClientUrl() + "/WebResources/udo_payments_debtsDMC?data=" + part2;
        return url;

    }
    catch (e) {
        return "error"
    }
}

function setFocusOnSearchTextBox() {
    setTimeout('document.getElementById("crmGrid_udo_udo_idproof_udo_legacypaymenthistory_IdProofId_findCriteria").focus()', 1000)
}
