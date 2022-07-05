Ext.define('VIP.soap.envelopes.mvi.MviTemplate', {
    extend: 'VIP.soap.envelopes.VARequestBuilder',
    config: {
        isDacRequest: true
    },
    constructor: function (config) {
        var me = this,
            mviBase = me.getEnvironment() ? me.getEnvironment().get('MVIBase') : 'http://ps-esr.commserv.healthevet.va.gov:9193/';

        me.setServiceUrl(me.getEnvironment() ? me.getEnvironment().get('MVI') : 'psim_webservice/stage1a/IdMWebService');

        me.initConfig(config);

        //Base Url config set up in VA Request Builder
        me.setBaseUrl(mviBase);

        me.callParent();
    }
});
