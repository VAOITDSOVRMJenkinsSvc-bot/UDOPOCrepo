Ext.define('VIP.soap.envelopes.share.sharestandarddata.FindRegionalOffices', {
    extend: 'VIP.soap.envelopes.share.ShareTemplate',
    alias: 'envelopes.FindRegionalOffices',
    config: {
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),
    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('findRegionalOffices', {
            namespace: 'ser'
        });
    }

});
