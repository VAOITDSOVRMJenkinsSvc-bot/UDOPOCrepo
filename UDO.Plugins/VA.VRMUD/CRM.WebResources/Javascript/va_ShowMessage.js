function PCR() {
    window.open("http://vbaw.vba.va.gov/bl/27/quality_training/training/pcrinfo.htm");
}
function VAGOVINTER() {
    window.open("http://www.va.gov/");
}
function VAGOVINTRA() {
    window.open("http://vaww.va.gov/default.asp");
}
function VIRTUALVA() {
    window.open("http://virtualva.vba.va.gov/");
}
function RateChart() {
    window.open("http://vbaw.vba.va.gov/bl/21/publicat/Manuals/Rates/rates_home.htm");
}
function PhoneScript() {
    window.open("http://vbaw.vba.va.gov/bl/27/quality_training/training/scripts.htm");
}
function FactSheet() {
    window.open("http://vaww.nca.va.gov/comm_outreach/fact_sheets.asp");
}
function VBA() {
    window.open("http://vbaw.vba.va.gov/ ");
}
function FormSite() {
    window.open("http://vaww4.va.gov/vaforms/");
}
function DirectServices() {
    window.open("http://vbaw.vba.va.gov/bl/27/quality_training/training/index.htm ");
}
function CFR() {
    window.open("http://vbaw.vba.va.gov/bl/21/publicat/Regs/Part4/index.htm");
}
function DisabilityCalc() {
    window.open("http://vbaedwweb1.vba.va.gov:7778/oowa-bin/oowaro/ExpSrv634/dbxwdevkit/xwd_init?door3.db/web3/3003");
}
function AverageDaysOfClaim() {
    window.open("http://vbaedwweb1.vba.va.gov:7778/oowa-bin/oowaro/ExpSrv634/dbxwdevkit/xwd_init?door3.db/web3/3003");
}
function PensionCalc() {
    var orgname = Xrm.Page.context.getOrgUniqueName();
    var scriptRoot = Xrm.Page.context.getServerUrl().replace(orgname, '');
    window.open(scriptRoot + "ISV/Documents/SMPportionworksheet.xlsx");
}
function DocRepository() {
    var org = Xrm.Page.context.getOrgUniqueName();
    var Doc_Root = Xrm.Page.context.getServerUrl().replace(org, '');
    window.open(Doc_Root + "ISV/Documents/index.html");
}