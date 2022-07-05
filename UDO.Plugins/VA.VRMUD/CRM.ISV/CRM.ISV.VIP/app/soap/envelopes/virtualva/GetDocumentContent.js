Ext.define('VIP.soap.envelopes.virtualva.GetDocumentContent', {
    extend: 'VIP.soap.envelopes.virtualva.VirtualVaTemplate',
    alias: 'envelopes.GetDocumentContent',
    config: {
        fnDcmntId: '',
        fnDcmntSource: '',
        dcmntFormatCd: '',
        jro: '',
        userId: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);
        
        me.callParent();

        me.setBody('DocumentContent', {
            namespace: 'ser',
            fnDcmntId: {
                namespace: 'ser',
                value: me.getFnDcmntId()
            },
            fnDcmntSource: {
                namespace: 'ser',
                value: me.getFnDcmntSource()
            },
            dcmntFormatCd: {
                namespace: 'ser',
                value: me.getDcmntFormatCd()
            },
            jro: {
                namespace: 'ser',
                value: me.getJro()
            },
            userId: {
                namespace: 'ser',
                value: me.getUserId()
            }
        });

        me.addNamespace('ser', 'http://service.bfi.va.gov/');
    }

});
