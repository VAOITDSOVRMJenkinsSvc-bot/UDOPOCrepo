Ext.define('VIP.soap.envelopes.share.benefitclaim.UpdateBenefitClaimAddress', {
    extend: 'VIP.soap.envelopes.share.BenefitClaimTemplate',
    alias: 'envelopes.UpdateBenefitClaimAddress',
    config: {
        // TODO: THIS LIST IS NOT COMPLETE
        fileNumber: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('updateBenefitClaimAddress', {
            namespace: 'ser',
            fileNumber: {
                namespace: '',
                value: me.getFileNumber()
            }
        });
    }

});


