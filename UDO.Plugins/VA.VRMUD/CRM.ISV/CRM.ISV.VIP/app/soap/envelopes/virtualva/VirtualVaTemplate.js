Ext.define('VIP.soap.envelopes.virtualva.VirtualVaTemplate', {
    extend: 'VIP.soap.envelopes.VARequestBuilder',
    requires: [
        'VIP.services.CrmEnvironment'
    ],
    
    config: {
        serviceUrl: 'VABFI/services/vva',
        vvaUserName: '',
        vvaPassword: ''
    },

    constructor: function (config) {
        var me = this,
            virtualVaBase = me.getEnvironment() ? me.getEnvironment().get('VVABase') : 'https://vbaphi5dopp.vba.va.gov:7002/';

        me.initConfig(config);

        //Base Url config set up in VA Request Builder
        me.setBaseUrl(virtualVaBase);

        me.callParent();
    },

    postCreate: function () {
        var me = this,
            userName = me.getVvaUserName(),
            password = me.getVvaPassword(),
            request = me.getRequest();

        if (!Ext.isEmpty(request)) {
            if (!Ext.isEmpty(userName)) {
                request.Envelope.Header.Security.UsernameToken.Username = userName;
            }
            if (!Ext.isEmpty(password)) {
                request.Envelope.Header.Security.UsernameToken.Password.value = password;
            }
        }
    }
});
