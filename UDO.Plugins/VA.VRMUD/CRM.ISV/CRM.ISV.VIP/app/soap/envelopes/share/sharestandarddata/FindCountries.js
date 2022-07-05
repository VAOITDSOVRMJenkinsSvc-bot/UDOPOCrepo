Ext.define('VIP.soap.envelopes.share.sharestandarddata.FindCountries', {
    extend: 'VIP.soap.envelopes.share.ShareTemplate',
    alias: 'envelopes.FindCountries',
    config: {       
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),
    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('findCountries', {
            namespace: 'ser'
        });
    }

});
