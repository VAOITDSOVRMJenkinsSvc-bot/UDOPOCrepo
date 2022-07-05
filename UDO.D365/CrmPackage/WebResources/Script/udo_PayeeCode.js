var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
//Va.Udo.Crm.Scripts.ClaimsDocumentDownload = Va.Udo.Crm.Scripts.ClaimsDocumentDownload || {};
var confirmStrings = {};
confirmStrings.confirmButtonLabel = "Yes";
confirmStrings.cancelButtonLabel = "No";
var confirmOptions = { height: 300, width: 600 };
var webApi = null;
window.parent.BrowserWindowReadyEvent = BrowserWindowReadyEvent;
window.parent.BrowserWindowReadyEvent = USDEncode;
var globalCon = Xrm.Utility.getGlobalContext();

function BrowserWindowReadyEvent(context) {
    context.ui.formSelector.items.forEach(
        function (control, index) {
            console.log(control.setVisible(false));
        }
    );
    context.ui.headerSection.setBodyVisible(false);
    context.ui.headerSection.setCommandBarVisible(false);
    context.ui.headerSection.setTabNavigatorVisible(false);
    context.ui.footerSection.setVisible(false);
}

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
