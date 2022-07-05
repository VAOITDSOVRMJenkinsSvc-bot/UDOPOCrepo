function insertModDate() {
    lastmod = document.lastModified;
    lastmoddate = Date.parse(lastmod);
    today = new Date();
    difference = today - lastmoddate;
    days = Math.round(difference / (1000 * 60 * 60 * 24));
    if (days <= 14) {
        document.writeln("<p align=center style=\"color:red; font-size:20px;\"><b><u>NOTE</u> - THIS WAS JUST UPDATED ON " + lastmod + "</b></p>");
    }
}

function LoadClaimScriptData() {
    var UrlParams = getUrlParams();
    if (UrlParams != null && UrlParams.id && UrlParams != "") idStr = UrlParams.id
    else return;

    var claim = fetchClaim(idStr);
    var contentionRec = fetchContentions(idStr);
    var contentions = "";
    for (index = 0; index < contentionRec.lenght; index++) {
        contentions += contentionRec[index].clmntTxt + "; ";
    }

    if (contentions.length == 0) contentions = '(name contentions)';
    var clName = document.getElementById('claimName');
    var dateOpen = document.getElementById('dateOpen');
    var cont = document.getElementById('contentions');

    clName.innerHTML = '<U>' + claim.claimTypeName + '</U>';
    dateOpen.innerHTML = '<U>' + claim.claimReceiveDate + '</U>';
    cont.innerHTML = '<U>' + contentions + '</U>';
}

function fetchClaim(idStr) {
    var filter = "$filter=_udo_claimid_value eq " + idStr.replace("{", "").replace("}", "");
    var columns = ['udo_claimdescription', 'udo_DateOfClaim'];
    var claim = {};

    var me = this;
    Xrm.WebApi.retrieveMultipleRecords("udo_claim", "?$select=" + columns.join(',') + "&" + filter)
    //.fail(
    //    function (err) { })
    .then(
    function (data) {
        if (data.value && data.value.length > 0) {
            claim.claimTypeName = data.value[0].udo_claimdescription;
            claim.claimReceiveDate = me.convertODataDate(data.value[0].udo_DateOfClaim);
        }
    });

    return claim;
}
function fetchContentions(idStr) {
    var filter = "$filter=_udo_claimid_value eq " + idStr.replace("{", "").replace("}", "");
    var columns = ['udo_Description'];
    var contentionsRecords = [];
    //TODO: needs to be convereted to webapi
    //CrmRestKit2011.ByQuery('udo_contention', columns, filter, false)
    Xrm.WebApi.retrieveMultipleRecords("udo_contention", "?$select=udo_Description&" + filter)
    //.fail(
    //    function (err) { })
    .then(
    function (data) {
        if (data.value && data.value.length > 0) {
            for (index = 0; index < data.value.length; index++) {
                me.results.claim.contentionsRecords[index] = {};
                me.results.claim.contentionsRecords[index].clmntTxt = data.value[index].udo_Description;
            }
        }
    });
    return contentionsRecords;
}
function getUrlParams() {
    try {
        var sPageUrl = location.search.substring(1);
        var regex = new RegExp("[\\?&]?data=([^&#]*)");
        sPageUrl = decodeURIComponent(regex.exec(sPageUrl)[1]);
        var params = sPageUrl.split('&');
        var UrlParams = {};

        for (var index = 0; index < params.length; index++) {
            param = params[index].split('=');
            UrlParams[param[0]] = decodeURIComponent(param[1].split("#")[0]);
        }

        return UrlParams;
    }
    catch (err) {
        return null;
    }
}
