function PCR() {
    //TODO: convert to Xrm.Navigation
    window.open("http://vbaw.vba.va.gov/bl/27/quality_training/training/pcrinfo.htm");
}
function VAGOVINTER() {
    //TODO: convert to Xrm.Navigation
    window.open("http://www.va.gov/");
}
function VAGOVINTRA() {
    //TODO: convert to Xrm.Navigation
    window.open("http://vaww.va.gov/default.asp");
}
function VIRTUALVA() {
    //TODO: convert to Xrm.Navigation
    window.open("http://virtualva.vba.va.gov/");
}
function RateChart() {
    //TODO: convert to Xrm.Navigation
    window.open("http://vbaw.vba.va.gov/bl/21/publicat/Manuals/Rates/rates_home.htm");
}
function PhoneScript() {
    //TODO: convert to Xrm.Navigation
    window.open("http://vbaw.vba.va.gov/bl/27/quality_training/training/scripts.htm");
}
function FactSheet() {
    //TODO: convert to Xrm.Navigation
    window.open("http://vaww.nca.va.gov/comm_outreach/fact_sheets.asp");
}
function VBA() {
    //TODO: convert to Xrm.Navigation
    window.open("http://vbaw.vba.va.gov/ ");
}
function FormSite() {
    //TODO: convert to Xrm.Navigation
    window.open("http://vaww4.va.gov/vaforms/");
}
function DirectServices() {
    //TODO: convert to Xrm.Navigation
    window.open("http://vbaw.vba.va.gov/bl/27/quality_training/training/index.htm ");
}
function CFR() {
    //TODO: convert to Xrm.Navigation
    window.open("http://vbaw.vba.va.gov/bl/21/publicat/Regs/Part4/index.htm");
}
function DisabilityCalc() {
    //TODO: convert to Xrm.Navigation
    window.open("http://vbaedwweb1.vba.va.gov:7778/oowa-bin/oowaro/ExpSrv634/dbxwdevkit/xwd_init?door3.db/web3/3003");
}
function AverageDaysOfClaim() {
    //TODO: convert to Xrm.Navigation
    window.open("http://vbaedwweb1.vba.va.gov:7778/oowa-bin/oowaro/ExpSrv634/dbxwdevkit/xwd_init?door3.db/web3/3003");
}
function PensionCalc() {
    //TODO: convert to form context
    var orgname = Xrm.Page.context.getOrgUniqueName();
    //TODO: convert to form context
    var scriptRoot = Xrm.Page.context.getClientUrl().replace(orgname, '');
    //TODO: convert to Xrm.Navigation
    window.open(scriptRoot + "/ISV/Documents/SMPportionworksheet.xlsx");
}
function DocRepository() {
    //TODO: convert to form context
    var org = Xrm.Page.context.getOrgUniqueName();
    //TODO: convert to form context
    var Doc_Root = Xrm.Page.context.getClientUrl().replace(org, '');
    //TODO: convert to Xrm.Navigation
    window.open(Doc_Root + "/ISV/Documents/index.html");
}
