Ext.define('VIP.soap.envelopes.share.claimant.FindDenialsByPtcpntId', {
    extend: 'VIP.soap.envelopes.share.ClaimantTemplate',
    alias: 'envelopes.FindDenialsByPtcpntId',
    config: {
        ptcpntId: ''
    },
    /*analyzeResponse: function (response, reader) {
        return reader.read(response);
    },*/

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),
    
    constructor: function (config) {
        var me = this;
        me.initConfig(config);

        me.callParent();
        
        me.setBody('findDenialsByPtcpntId', {
            namespace: 'ser',
            ptcpntId: {
                namespace: '',
                value: me.getPtcpntId()
            }
        });
    }

});
