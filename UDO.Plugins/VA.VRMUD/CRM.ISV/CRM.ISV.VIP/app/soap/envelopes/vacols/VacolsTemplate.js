Ext.define('VIP.soap.envelopes.vacols.VacolsTemplate', {
    extend: 'VIP.soap.envelopes.VARequestBuilder',
    config: {        
        serviceUrl: 'VIERSService/v1/AppealService/Appeal'
    },
    constructor: function (config) {
        var me = this,
            vacolsBase = me.getEnvironment() ? me.getEnvironment().get('VacolsBase') : 'http://vaausvrsapp81.aac.va.gov/';

        me.initConfig(config);

        //Base Url config set up in VA Request Builder
        me.setBaseUrl(vacolsBase);

        me.callParent();
    }
});
