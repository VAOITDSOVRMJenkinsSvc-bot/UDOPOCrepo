/**
* @author
*
* @class VIP.mixin.StationOfJurisdiction
*
* Mixin for the service request
*/
Ext.define('VIP.mixin.StationOfJurisdiction', {
    
    getStationOfJurisdiction: function (stationCode) {
        var sojCode = null,
            resultSet = null,
            va_RegionalOfficeId = null;

        // data comes as '317 - St. Petersburg'; try to use code
        if (stationCode && stationCode.length >= 3 /*&& soj.charAt(4) == '-'*/) {
            sojCode = stationCode.substring(0, 3);

	    var columns = ['va_name', 'va_regionalofficeId'];
	    var filter = "va_Code eq '" + sojCode + "' ";
	    var orderby = "&$orderby=" + encodeURIComponent("va_name desc");
	    var query = parent.Xrm.Page.context.getServerUrl() + '/XRMServices/2011/OrganizationData.svc' + "/" + "va_regionalofficeSet"
                    + "?$select=" + columns.join(',') + "&$filter=" + encodeURIComponent(filter) + orderby;

	    var priorServiceRequests = parent.CrmRestKit2011.ByQueryUrl(query, false);
            priorServiceRequests.fail(function (error) {
		UTIL.restKitError(error, 'Failed to get regional office info');
            })
	    priorServiceRequests.done(function (data) {
                if (data && data.d.results && data.d.results.length > 0) {
                    va_RegionalOfficeId = {
                        Id: data.d.results[0].va_regionalofficeId,
                        LogicalName: 'va_regionaloffice',
                        Name: data.d.results[0].va_name
                    };
                }
            });
        } 
        return va_RegionalOfficeId;
    },
    
    getStationOfJurisdictionAsync: function (sojCode, callback) {

        var columns = ['va_name', 'va_regionalofficeId', 'va_IsVAIPilot'];
        var filter = "va_Code eq '" + sojCode + "' ";
        var orderby = "&$orderby=" + encodeURIComponent("va_name desc");
        var query = parent.Xrm.Page.context.getServerUrl() + '/XRMServices/2011/OrganizationData.svc' + "/" + "va_regionalofficeSet"
                    + "?$select=" + columns.join(',') + "&$filter=" + encodeURIComponent(filter) + orderby;
        parent.CrmRestKit2011.ByQueryUrl(query)
            .fail(function (error) {
                UTIL.restKitError(error, 'Failed to get regional office info');
            })
	        .done(function (data) {
	            var soj;
	            if (data && data.d.results && data.d.results.length > 0) {
	                soj = {
	                    entityReference: {
	                        Id: data.d.results[0].va_regionalofficeId,
	                        LogicalName: 'va_regionaloffice',
	                        Name: data.d.results[0].va_name
	                    },
	                    isPilot: Boolean(data.d.results[0].va_IsVAIPilot)
	                };
	                callback(soj);
	            } else callback(null);

	        });
    },
    
    sojIsNotPilotMessage: function() {
        return 'This SOJ is not part of the VAI Pilot.\r\nPlease exit CRM and use Right Now Web tool to submit VAI.';
    }
});