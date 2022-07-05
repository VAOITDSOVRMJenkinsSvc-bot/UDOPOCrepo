/**
* @author Dmitri Riz
* @class VIP.services.CrmEnvironment
*
* Getter for Crm Environment data
*/
Ext.define('VIP.services.CrmEnvironment', {

    GetCurrentCrmUser: function () {
        var app = _extApp; if (Ext.isEmpty(app)) { app = this.application; }
        if (!Ext.isEmpty(app) && !Ext.isEmpty(app.pcrContext)) { return app.pcrContext; } // 'VIP.model.Pcr'

        var context = null;
        if (parent && parent._vipSearchContext) { context = parent._vipSearchContext; }
        if (!context) {
            context = {};
            context.user = parent._GetUserSettingsForWebservice();
        }
        return context.user;
    }
});