Ext.define('VIP.soap.envelopes.share.claimant.FindMonthOfDeath', {
    extend: 'VIP.soap.envelopes.share.ClaimantTemplate',
    alias: 'envelopes.FindMonthOfDeath',
    config: {
        fileNumber: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),
    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('findMonthOfDeath', {
            namespace: 'ser',
            fileNumber: me.getFileNumber()
        });
    }

});
