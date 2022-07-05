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
	var query = Xrm.Page.context.getServerUrl() + '/XRMServices/2011/OrganizationData.svc' + "/" + "va_regionalofficeSet"
		+ "?$select=" + columns.join(',') + "&$filter=" + encodeURIComponent(filter) + orderby;
	var soj;
	CrmRestKit2011.ByQueryUrl(query, false)
	.fail(function (error) {
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
	var userId = Xrm.Page.context.getUserId();
	var columns = ['va_StationNumber'];
	var filter = "SystemUserId eq (guid'" + userId + "') ";
	var orderby = "&$orderby=" + encodeURIComponent("va_StationNumber desc");
	var query = Xrm.Page.context.getServerUrl() + '/XRMServices/2011/OrganizationData.svc' + "/" + "SystemUserSet"
		+ "?$select=" + columns.join(',') + "&$filter=" + encodeURIComponent(filter) + orderby;
	var stationNumber;
	CrmRestKit2011.ByQueryUrl(query, false)
	.fail(function (error) {
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
	var formType = Xrm.Page.ui.getFormType();

	if (formType == FORM_TYPE_UPDATE) {
		var soj = getStationOfJurisdiction(getStationNumber());

		if (soj) {
			return soj.isAddDependent;
		}
	}

	return false;
}

function isAddDependentEnabled_LeftNav() {
	var formType = Xrm.Page.ui.getFormType();

	if (formType != FORM_TYPE_CREATE) {
		var soj = getStationOfJurisdiction(getStationNumber());

		if (soj) {
			return soj.isAddDependent;
		}
	}

	return false;
}