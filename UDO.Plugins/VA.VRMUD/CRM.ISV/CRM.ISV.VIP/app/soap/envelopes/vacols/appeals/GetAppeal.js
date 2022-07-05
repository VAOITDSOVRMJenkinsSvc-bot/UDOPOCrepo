    Ext.define('VIP.soap.envelopes.vacols.appeals.GetAppeal', {
    extend: 'VIP.soap.envelopes.vacols.VacolsTemplate',
    alias: 'envelopes.GetAppeal',
    config: {
        AppealKey: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('getAppeal', {
            namespace: 'app',
            namespaces: {
                'app': 'http://www.va.gov/schema/AppealService'
            },
            getAppealCriteria: {
                namespace: 'app',
                AppealKey: {
                    namespace: '',
                    value: me.getAppealKey()
                }
            }
        });
    }

});
