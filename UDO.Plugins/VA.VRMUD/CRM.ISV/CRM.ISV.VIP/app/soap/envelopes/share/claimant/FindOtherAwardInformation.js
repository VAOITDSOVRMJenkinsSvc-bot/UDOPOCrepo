Ext.define('VIP.soap.envelopes.share.claimant.FindOtherAwardInformation', {
    extend: 'VIP.soap.envelopes.share.ClaimantTemplate',
    requires: [
        'VIP.util.Xml'
    ],
    alias: 'envelopes.FindOtherAwardInformation',
    config: {
        ptcpntVetId: '',
        ptcpntBeneId: '',
        ptcpntRecipId: '',
        awardTypeCd: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('findOtherAwardInformation', {
            namespace: 'ser',
            ptcpntVetId: {
                namespace: '',
                value: me.getPtcpntVetId()
            },
            ptcpntBeneId: {
                namespace: '',
                value: me.getPtcpntBeneId()
            },
            ptcpntRecipId: {
                namespace: '',
                value: me.getPtcpntRecipId()
            },
            awardTypeCd: {
                namespace: '',
                value: me.getAwardTypeCd()
            }
        });
    }

});
