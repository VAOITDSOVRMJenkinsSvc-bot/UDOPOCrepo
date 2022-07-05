if (typeof CIHS === 'undefined') { CIHS = { __namespace: true }; }

CIHS.LoadView = (function () {

    var field_crme_edipi = '';
    var field_crme_icn = '';
    var field_crme_participantid = '';
    var field_crme_ssn = '';
    var field_crme_ihlobs = '';
    var field_crme_ihfrom = '';
    var field_crme_ihto = '';
    var CDWSPURL = '';
    var TestSSN = '';

    return {
        CIHSLOBView: cihsLOBView,
        OnTabOpen: cihsLOBView
    }

    function cihsLOBView(executionContext) {

        var filter = "$select=crme_name,crme_Value";
        //prompt('Filter', filter);
        SDK.REST.retrieveMultipleRecords("crme_cihssettings", filter, retrieveCIHSSettingsCallBack, retrieveCIHSSettingsErrorCallBack, retrieveCIHSSettingsComplete);
    }

    function retrieveCIHSSettingsCallBack(data, textStatus, XmlHttpRequest) {
        Xrm.Page.ui.clearFormNotification("CIHSERROR");
        if (data != null && data.length > 0) {

            for (var i = 0; i < data.length; i++) {

                var settingName = data[i].crme_name == null ? "" : data[i].crme_name;

                if (settingName === "ih_LOBEDIPIFieldName")
                    field_crme_edipi = data[i].crme_Value == null ? "" : data[i].crme_Value;

                if (settingName === "ih_LOBICNFieldName")
                    field_crme_icn = data[i].crme_Value == null ? "" : data[i].crme_Value;

                if (settingName === "ih_LOBParticipantIdFieldName")
                    field_crme_participantid = data[i].crme_Value == null ? "" : data[i].crme_Value;

                if (settingName === "ih_LOBSSNFieldName")
                    field_crme_ssn = data[i].crme_Value == null ? "" : data[i].crme_Value;

                if (settingName === "ih_LOBFieldNameFilter2")
                    field_crme_ihlobs = data[i].crme_Value == null ? "" : data[i].crme_Value;

                if (settingName === "ih_LOBFieldNameFilter1")
                    field_crme_ihfrom = data[i].crme_Value == null ? "" : data[i].crme_Value;

                if (settingName === "ih_LOBFieldNameFilter3")
                    field_crme_ihto = data[i].crme_Value == null ? "" : data[i].crme_Value;

                if (settingName === "CDWSPURL")
                    CDWSPURL = data[i].crme_Value == null ? "" : data[i].crme_Value;

                if (settingName === "TestSSN")
                    TestSSN = data[i].crme_Value == null ? "" : data[i].crme_Value;

            }
        }
    }

    function retrieveCIHSSettingsErrorCallBack(error) {
        console.log(error.message);
        Xrm.Page.ui.setFormNotification("An error occurred retrieving CIHS Settings. If the issue persist please contact your System Administrator.", "WARNING", "CIHSERROR");
    }

    function retrieveCIHSSettingsComplete() {
        var crme_edipi = Xrm.Page.data.entity.attributes.get(field_crme_edipi).getValue();
        var crme_icn = Xrm.Page.data.entity.attributes.get(field_crme_icn).getValue();
        var crme_participantid = Xrm.Page.data.entity.attributes.get(field_crme_participantid).getValue();
        var crme_ssn = Xrm.Page.data.entity.attributes.get(field_crme_ssn).getValue();
        var crme_ihlobs = Xrm.Page.data.entity.attributes.get(field_crme_ihlobs).getValue();
        var crme_ihfrom = Xrm.Page.getAttribute(field_crme_ihfrom).getValue();
        var crme_ihto = Xrm.Page.getAttribute(field_crme_ihto).getValue();
        var outfromdate = 'null';
        var outtodate = 'null';

        if (TestSSN != null)
            crme_ssn = TestSSN;

        if (crme_ihfrom != null) {
            var fromyear = crme_ihfrom.getFullYear() + "";
            var frommonth = (crme_ihfrom.getMonth() + 1) + "";
            var fromday = crme_ihfrom.getDate() + "";
            outfromdate = fromyear + "-" + frommonth + "-" + fromday;
        }

        if (crme_ihto != null) {
            var toyear = crme_ihto.getFullYear() + "";
            var tomonth = (crme_ihto.getMonth() + 1) + "";
            var today = crme_ihto.getDate() + "";
            outtodate = toyear + "-" + tomonth + "-" + today;
        }

        if (crme_edipi == 'UNK')
            crme_edipi = ''

        var targetURL = CDWSPURL;
        var iframe = Xrm.Page.ui.controls.get("IFRAME_CDW_CIHS");
        iframe.setVisible(true);

        targetURL = targetURL.replace("{edipi}", crme_edipi);
        targetURL = targetURL.replace("{icn}", crme_icn);
        targetURL = targetURL.replace("{participantid}", crme_participantid);
        targetURL = targetURL.replace("{ssn}", crme_ssn);
        targetURL = targetURL.replace("{lob}", crme_ihlobs);
        targetURL = targetURL.replace("{fromdate}", outfromdate);
        targetURL = targetURL.replace("{todate}", outtodate);

        iframe.setSrc(targetURL);
    }

    function buildQueryFilter(field, value, and) {
        if (and) {
            return " and " + field + " eq '" + value + "'";
        } else {
            return field + " eq '" + value + "'";
        }
    }
})();