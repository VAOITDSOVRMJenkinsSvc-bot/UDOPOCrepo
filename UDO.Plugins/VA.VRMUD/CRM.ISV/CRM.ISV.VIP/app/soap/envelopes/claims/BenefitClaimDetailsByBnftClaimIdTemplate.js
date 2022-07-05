Ext.define('VIP.soap.envelopes.claims.BenefitClaimDetailsByBnftClaimIdTemplate', {
    extend: 'VIP.soap.envelopes.VARequestBuilder',
    config: {
        serviceUrl: 'iw-wsprovider/services/RetrieveAccountActivityService',
        isDacRequest: true
    },
    constructor: function (config) {
        var me = this,
            eBenefitsBase = me.getEnvironment() ? me.getEnvironment().get('EbenefitsBase') : 'http://vaebnweb2.aac.va.gov/';

        me.initConfig(config);

        me.setBaseUrl(eBenefitsBase);

        me.callParent();
    }
});
