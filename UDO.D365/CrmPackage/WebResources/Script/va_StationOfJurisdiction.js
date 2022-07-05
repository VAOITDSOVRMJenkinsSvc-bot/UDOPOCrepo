var FORM_TYPE_CREATE = 1;
var FORM_TYPE_UPDATE = 2;
var FORM_TYPE_READ_ONLY = 3;
var FORM_TYPE_DISABLED = 4;
var FORM_TYPE_QUICK_CREATE = 5;
var FORM_TYPE_BULK_EDIT = 6;
var FORM_TYPE_READ_OPTIMIZED = 11;

function getStationOfJurisdiction(sojCode) {
    var columns = ['va_name', 'va_regionalofficeId', 'va_IsVAIPilot', 'va_IsAddDependent'];
    var filter = "va_Code eq '" + sojCode + "' ";
    var orderby = "&$orderby=" + encodeURIComponent("va_name desc");
    //TODO: convert to form context
    var query = Xrm.Page.context.getClientUrl() + '/XRMServices/2011/OrganizationData.svc' + "/" + "va_regionalofficeSet"
		+ "?$select=" + columns.join(',') + "&$filter=" + encodeURIComponent(filter) + orderby;
    var soj;
    //TODO: convert to WebAPI
    WebApi.prototype.RetrieveMultiple("va_regionaloffice", columns, "?$select=" + columns.join(',') + "&$filter=" + encodeURIComponent(filter) + orderby)

    //CrmRestKit2011.ByQueryUrl(query, false)
	.fail(function (error) {
	    //TODO: check for WebAPI error handler
	    UTIL.restKitError(error, 'Failed to get regional office info');
	})
	.done(function (data) {
	    if (data && data.d.results && data.d.results.length > 0) {
	        soj = {
	            entityReference: {
	                Id: data.d.results[0].va_regionalofficeId,
	                LogicalName: 'va_regionaloffice',
	                Name: data.d.results[0].va_name
	            },
	            isPilot: Boolean(data.d.results[0].va_IsVAIPilot),
	            isAddDependent: Boolean(data.d.results[0].va_IsAddDependent)
	        };
	    }
	});

    return soj;
}

function getStationNumber() {
    //TODO: convert to form context
    var userId = Xrm.Page.context.getUserId();
    var columns = ['va_StationNumber'];
    var filter = "SystemUserId eq (guid'" + userId + "') ";
    var orderby = "&$orderby=" + encodeURIComponent("va_StationNumber desc");
    //TODO: convert to form context
    var query = Xrm.Page.context.getClientUrl() + '/XRMServices/2011/OrganizationData.svc' + "/" + "SystemUserSet"
		+ "?$select=" + columns.join(',') + "&$filter=" + encodeURIComponent(filter) + orderby;
    var stationNumber;
    //TODO: convert to WebAPI
    WebApi.prototype.RetrieveMultiple("SystemUsers", columns, "?$select=" + columns.join(',') + "&$filter=" + encodeURIComponent(filter) + orderby)
    //CrmRestKit2011.ByQueryUrl(query, false)
	.fail(function (error) {
	    //TODO: check for WebAPI error handler
	    UTIL.restKitError(error, 'Failed to get user info');
	})
	.done(function (data) {
	    if (data && data.d.results && data.d.results.length > 0) {
	        stationNumber = data.d.results[0].va_StationNumber;
	    }
	});

    return stationNumber;
}

function isAddDependentEnabled() {
    //TODO: convert to form context
    var formType = Xrm.Page.ui.getFormType();

    if (formType == FORM_TYPE_UPDATE) {
        //var noadisagree = Xrm.Page.getAttribute('va_noadisagree').getValue();
        //TODO: convert to form context
        var va_disposition = Xrm.Page.getAttribute('va_disposition').getValue();
        //TODO: convert to form context
        var va_noastatement = Xrm.Page.getAttribute('va_noastatement').getValue();

        var soj = getStationOfJurisdiction(getStationNumber());

        if (va_disposition == 953850032 && va_noastatement && soj) {
            return soj.isAddDependent;
        }
    }

    return false;
}

function isAddDependentEnabled_LeftNav() {
    //TODO: convert to form context
    var formType = Xrm.Page.ui.getFormType();

    if (formType != FORM_TYPE_CREATE) {
        //var noadisagree = Xrm.Page.getAttribute('va_noadisagree').getValue();
        //TODO: convert to form context
        var va_disposition = Xrm.Page.getAttribute('va_disposition').getValue();
        //TODO: convert to form context
        var va_noastatement = Xrm.Page.getAttribute('va_noastatement').getValue();
        var soj = getStationOfJurisdiction(getStationNumber());

        if (va_disposition == 953850032 && va_noastatement && soj) {
            return soj.isAddDependent;
        }
    }

    return false;
}
//TODO: This looks like the same group of links from the ShowMessage file, can we put these in the same library and reference them?
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
