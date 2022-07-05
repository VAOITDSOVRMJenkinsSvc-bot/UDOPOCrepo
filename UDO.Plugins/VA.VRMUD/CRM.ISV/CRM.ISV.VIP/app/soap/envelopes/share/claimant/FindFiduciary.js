Ext.define('VIP.soap.envelopes.share.claimant.FindFiduciary', {
    extend: 'VIP.soap.envelopes.share.ClaimantTemplate',
    alias: 'envelopes.FindFiduciary',
    config: {
        fileNumber: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('findFiduciary', {
            namespace: 'ser',
            fileNumber: {
                namespace: '',
                value: me.getFileNumber()
            }
        });
    }

});


