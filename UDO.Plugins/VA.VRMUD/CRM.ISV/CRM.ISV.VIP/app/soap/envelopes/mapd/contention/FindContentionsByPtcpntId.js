Ext.define('VIP.soap.envelopes.mapd.contention.FindContentionsByPtcpntId', {
    extend: 'VIP.soap.envelopes.mapd.ContentionTemplate',
    alias: 'envelopes.FindContentionsByPtcpntId', 
    config: {
        ptcpntId: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();
        
        me.setBody('', {
            namespace: 'ser',
            ptcpntId: {
                namespace: '',
                value: me.getPtcpntId()
            }
        });
    }
});