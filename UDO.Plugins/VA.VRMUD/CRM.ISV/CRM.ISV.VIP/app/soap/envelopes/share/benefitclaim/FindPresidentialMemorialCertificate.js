Ext.define('VIP.soap.envelopes.share.benefitclaim.FindPresidentialMemorialCertificate', {
    extend: 'VIP.soap.envelopes.share.BenefitClaimTemplate',
    alias: 'envelopes.FindPresidentialMemorialCertificate',
    config: {
        fileNumber: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('findPresidentialMemorialCertificate', {
            namespace: 'ser',
            fileNumber: {
                namespace: '',
                value: me.getFileNumber()
            }
        });
    }

});
