Ext.define('VIP.soap.envelopes.claims.BenefitClaimDetailsByBnftClaimId', {
    extend: 'VIP.soap.envelopes.claims.BenefitClaimDetailsByBnftClaimIdTemplate',
    requires: [
        'VIP.util.Xml'
    ],
    alias: 'claims.BenefitClaimDetailsByBnftClaimId',
    config: {
        bnftClaimId: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('BenefitClaimDetailsByBnftClaimId', {
            namespaces: {
                'ret': 'http://www.ebenefits.va.gov/wsdl/RetrieveAccountActivityService/'
            },
            namespace: 'ret',
            claimId: {
                namespace: '',
                value: me.getBnftClaimId()
            }
        });
    }

});
