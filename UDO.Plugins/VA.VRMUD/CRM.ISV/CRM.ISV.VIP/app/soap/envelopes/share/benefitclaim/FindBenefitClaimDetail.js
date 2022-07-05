Ext.define('VIP.soap.envelopes.share.benefitclaim.FindBenefitClaimDetail', {
    extend: 'VIP.soap.envelopes.share.BenefitClaimTemplate',
    alias: 'envelopes.FindBenefitClaimDetail',
    config: {
        benefitClaimId: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this;
        
        me.initConfig(config);

        me.callParent();

        me.setBody('findBenefitClaimDetail', {
            namespace: 'ser',
            benefitClaimId: {
                namespace: '',
                value: me.getBenefitClaimId()
            }
        });
    }

});
