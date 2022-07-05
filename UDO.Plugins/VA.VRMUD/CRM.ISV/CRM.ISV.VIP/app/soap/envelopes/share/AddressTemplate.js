Ext.define('VIP.soap.envelopes.share.AddressTemplate', {
    extend: 'VIP.soap.envelopes.VARequestBuilder',
    config: {
        serviceUrl: 'AddressWebServiceBean/AddressWebService',
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