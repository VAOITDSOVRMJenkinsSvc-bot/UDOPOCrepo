Ext.define('VIP.soap.envelopes.share.address.FindAllPtcpntAddrsByPtcpntId', {
    extend: 'VIP.soap.envelopes.share.AddressTemplate',
    alias: 'envelopes.FindAllPtcpntAddrsByPtcpntId',
    config: {
        ptcpntId: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this;
        
        me.initConfig(config);

        me.callParent();

        me.setBody('findAllPtcpntAddrsByPtcpntId', {
            namespace: 'add',
            namespaces: {
                'add': 'http://address.services.vetsnet.vba.va.gov/'
            },
            ptcpntId: {
                namespace: '',
                value: me.getPtcpntId()
            }
        });
    }

});
