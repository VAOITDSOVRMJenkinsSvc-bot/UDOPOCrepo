//Replaces the 2007 end point call for getting the SOJ with the 2011 REST Call
//needed by va_servicerequestcall.js

function va_crm_udo_getSoj() {
    try {
        var userId = Xrm.Page.getAttribute('va_pcrofrecordid').getValue()[0].id;

        CrmRestKit2011.Retrieve('SystemUser', userId, ['SiteId'], false)
                 .done(function (data) {
                     debugger;
                     siteId = data.d.SiteId.Id;
                 })
                 .fail(function (data) {
                     UTIL.restKitError(err, 'Failed to retrieve regional office data');
                 }
          );

        var cols = ['Name', 'Address1_Country', 'Address1_City', 'Address1_Fax', 'Address1_StateOrProvince', 'Address1_Line1', 'Address1_Line2', 'Address1_Line3', 'Address1_PostalCode'];
        CrmRestKit2011.Retrieve('Site', siteId, cols, false)
                             .done(function (data) {
                                 sojAddress = (data.d.Name && data.d.Name.length > 0 ? data.d.Name + '<br/>' : '') +
                                     (data.d.Address1_Line1 && data.d.Address1_Line1.length > 0 ? data.d.Address1_Line1 + '<br/>' : '') +
                                     (data.d.Address1_Line2 && data.d.Address1_Line2.length > 0 ? data.d.Address1_Line2 + '<br/>' : '') +
                                     (data.d.Address1_Line3 && data.d.Address1_Line3.length > 0 ? data.d.Address1_Line3 + '<br/>' : '') +
                                     (data.d.Address1_City && data.d.Address1_City.length > 0 ? data.d.Address1_City + ', ' : '') +
                                     (data.d.Address1_StateOrProvince && data.d.Address1_StateOrProvince.length > 0 ? data.d.Address1_StateOrProvince + ' ' : '') +
                                     (data.d.Address1_PostalCode && data.d.Address1_PostalCode.length > 0 ? data.d.Address1_PostalCode + '<br/>' : '') +
                                     (data.d.Address1_Country && data.d.Address1_Country.length > 0 ? data.d.Address1_Country + '<br/>' : '') +
                                     (data.d.Address1_Fax && data.d.Address1_Fax.length > 0 ? '<br/>FAX: ' + data.d.Address1_Fax : '');
                             })
                             .fail(function (err) {
                                 UTIL.restKitError(err, 'Failed to retrieve regional office data');
                             });
    } catch (err) {
        return (null);
    }
    return (sojAddress);
}