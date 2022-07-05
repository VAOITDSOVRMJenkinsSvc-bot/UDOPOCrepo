Ext.define('VIP.soap.envelopes.share.claimant.FindAllRelationships', {
    extend: 'VIP.soap.envelopes.share.ClaimantTemplate',
    alias: 'envelopes.FindAllRelationships',
    config: {
        ptcpntId: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('findAllRelationships', {
            namespace: 'ser',
            ptcpntId: {
                namespace: '',
                value: me.getPtcpntId()
            }
        });
    }

});


