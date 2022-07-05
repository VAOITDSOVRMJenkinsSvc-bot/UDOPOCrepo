function loadReport() {
    //TODO: convert to form context
    var ssn = Xrm.Page.getAttribute("va_ssn").getValue();
    //TODO: convert to form context 
    var serverUrl = Xrm.Page.context.getServerUrl();

    var url = serverUrl + "crmreports/viewer/viewer.aspx?action=run&helpID=Contact%20History.rdl&id=%7bCBC5C264-2BE6-E411-A4A6-02BF0A191462%7d&p:SSN=" + ssn;

    SetReportUrl(url, "IFRAME_ContactHistory", ssn);
}

function SetReportUrl(reportUrl, iFrame, ssn) {
    if (ssn == null) {
        //TODO: convert to form context
        Xrm.Page.getControl(iFrame).setVisible(false);
    }
    else {
        //TODO: convert to form context
        Xrm.Page.getControl(iFrame).setVisible(true);
        //TODO: convert to form context
        Xrm.Page.getControl(iFrame).setSrc(reportUrl);
    }
}