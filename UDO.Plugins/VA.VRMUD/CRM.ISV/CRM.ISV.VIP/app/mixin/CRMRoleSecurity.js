/**
* @author
*
* @class VIP.mixin.CRMRoleSecurity
*
* Mixin for handling CRM security roles
*/
Ext.define('VIP.mixin.CRMRoleSecurity', {

    UserHasRole: function (roleName) {
        var me = this;
        var rv = false;
        var query = parent.Xrm.Page.context.getServerUrl() + '/XRMServices/2011/OrganizationData.svc/' + "RoleSet?$filter=Name eq '" + roleName + "'&$select=RoleId";

        var requestResults = parent.CrmRestKit2011.ByQueryUrl(query, false);

        requestResults.fail(function (error) {
            UTIL.restKitError(error, 'Failed to get role information');
        });
        
        requestResults.done(function (data) {
            if (data && data.d.results && data.d.results.length > 0) {
                for (var i = 0; i < data.d.results.length; i++) {
                    var role = data.d.results[i];
                    var id = role.RoleId;
                    var currentUserRoles = parent.Xrm.Page.context.getUserRoles();
                    for (var j = 0; j < currentUserRoles.length; j++) {
                        var userRole = currentUserRoles[j];
                        if (me.GuidsAreEqual(userRole, id)) {
                            rv = true;
                            break;
                        }
                    }
                    if (rv) break;
                }
            }
        });
        return rv;
    }, 
        
    GuidsAreEqual: function (guid1, guid2) {
        var isEqual = false;

        if (guid1 == null || guid2 == null) {
            isEqual = false;
        }
        else {
            isEqual = guid1.replace(/[{}]/g, "").toLowerCase() == guid2.replace(/[{}]/g, "").toLowerCase();
        }

        return isEqual;
    }
});

