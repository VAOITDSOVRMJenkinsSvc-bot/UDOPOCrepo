Ext.define('VIP.soap.envelopes.share.PaymentInfoTemplate', {
    extend: 'VIP.soap.envelopes.VARequestBuilder',
    config: {
        serviceUrl: 'vrm-ws/PaymentInformationService',
        useExternalUid: true,
        isDacRequest: true
    },
    constructor: function (config) {
        var me = this,
            corpBase = me.getEnvironment() ? me.getEnvironment().get('CORP') : 'http://vbmscert.vba.va.gov/';

        me.initConfig(config);

        //Base Url config set up in VA Request Builder
        me.setBaseUrl(corpBase);

        me.callParent();
    }
});