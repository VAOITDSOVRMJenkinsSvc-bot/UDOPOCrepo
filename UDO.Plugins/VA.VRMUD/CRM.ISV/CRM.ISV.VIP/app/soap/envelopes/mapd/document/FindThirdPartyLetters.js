Ext.define('VIP.soap.envelopes.mapd.document.FindThirdPartyLetters', {
    extend: 'VIP.soap.envelopes.mapd.DocumentTemplate',
    config: {
        documentId: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('findThirdPartyLetters', {
            namespace: 'ser',
            documentId: {
                namespace: '',
                value: me.getDocumentId()
            }
        });

        me.addNamespace('ser', 'http://services.mapd.benefits.vba.va.gov/');
    }
});
