Ext.define('VIP.soap.envelopes.share.claims.FindBenefitClaimDetailsByBnftClaimId', {
    extend: 'VIP.soap.envelopes.share.BenefitClaimDetailsByBnftClaimIdTemplate',
    alias: 'envelopes.FindBenefitClaimDetailsByBnftClaimId',
    requires: [
        'VIP.model.claims.BenefitClaimDetailsByBnftClaimId'
    ],
    config: {
        bnftClaimId: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('findBenefitClaimDetailsByBnftClaimId', {
            namespace: 'ebs',
            bnftClaimId: {
                namespace: 'ebs',
                value: me.getBnftClaimId()
            }
        });
    }

});
