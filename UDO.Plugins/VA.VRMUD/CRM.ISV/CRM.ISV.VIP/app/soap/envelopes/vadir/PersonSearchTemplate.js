Ext.define('VIP.soap.envelopes.vadir.PersonSearchTemplate', {
    extend: 'VIP.soap.envelopes.VARequestBuilder',
    config: {
        serviceUrl: 'crm/Person',
        useExternalUid: true,
        isDacRequest: true
    },
    constructor: function (config) {
        var me = this,
            vadir = me.getEnvironment() ? me.getEnvironment().get('VADIR') : 'http://vavdrapp80.aac.va.gov';

        me.initConfig(config);

        //Base Url config set up in VA Request Builder
        me.setBaseUrl(vadir);

        me.callParent();
    }
});