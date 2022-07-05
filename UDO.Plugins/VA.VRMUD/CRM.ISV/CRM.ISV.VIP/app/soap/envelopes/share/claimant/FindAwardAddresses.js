Ext.define('VIP.soap.envelopes.share.claimant.FindAwardAddresses', {
    extend: 'VIP.soap.envelopes.share.ClaimantTemplate',
    alias: 'envelopes.FindAwardAddresses',
    config: {
        fileNumber: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),
    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('findAwardAddresses', {
            namespace: 'ser',
            fileNumber: me.getFileNumber()
        });
    }

});


