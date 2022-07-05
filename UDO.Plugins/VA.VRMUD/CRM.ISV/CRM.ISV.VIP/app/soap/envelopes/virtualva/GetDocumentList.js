Ext.define('VIP.soap.envelopes.virtualva.GetDocumentList', {
    extend: 'VIP.soap.envelopes.virtualva.VirtualVaTemplate',
    alias: 'envelopes.GetDocumentList',
    config: {
        claimNbr: ''        
    },

    analyzeResponse: Ext.create('VIP.soap.analyzers.virtualva.GetDocumentList'),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('DocumentList', {
            namespace: 'ser',
            claimNbr: {
                namespace: 'ser',
                value: me.getClaimNbr()
            }
        });

        me.addNamespace('ser', 'http://service.bfi.va.gov/');
    }
});
