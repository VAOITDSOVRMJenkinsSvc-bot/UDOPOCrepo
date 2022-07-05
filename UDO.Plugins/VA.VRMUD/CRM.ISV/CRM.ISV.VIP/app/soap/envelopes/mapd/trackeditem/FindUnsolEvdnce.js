Ext.define('VIP.soap.envelopes.mapd.trackeditem.FindUnsolEvdnce', {
    extend: 'VIP.soap.envelopes.mapd.TrackedItemTemplate',
    config: {
        Claiment_ptpcpnt_id: '' // Misspelling and casing in the web service
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('findUnsolEvdnce', {
            namespace: 'ser',
            Claiment_ptpcpnt_id: {
                namespace: '',
                value: me.getClaiment_ptpcpnt_id()
            }
        });

        me.addNamespace('ser', 'http://services.mapd.benefits.vba.va.gov/');

    }
});