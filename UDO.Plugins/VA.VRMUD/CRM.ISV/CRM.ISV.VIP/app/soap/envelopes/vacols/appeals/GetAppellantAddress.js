Ext.define('VIP.soap.envelopes.vacols.appeals.GetAppellantAddress', {
    extend: 'VIP.soap.envelopes.vacols.VacolsTemplate',
    alias: 'envelopes.GetAppellantAddress',
    config: {
        ssn: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),
    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('getAppellantAddress', {
            namespace: 'app',
            getAddressCriteria: {
                ssn: me.getSsn()
            }
        });
    }

});
