Ext.define('VIP.soap.envelopes.mapd.trackeditem.FindTrackedItems', {
    extend: 'VIP.soap.envelopes.mapd.TrackedItemTemplate',
    alias: 'envelopes.FindTrackedItems',
    config: {
        claimId: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this;
        
        me.initConfig(config);

        me.callParent();

        me.setBody('findTrackedItems', {
            namespace: 'ser',            
            claimId: {
                namespace: '',
                value: me.getClaimId()
            }
        });

        me.addNamespace('ser', 'http://services.mapd.benefits.vba.va.gov/');
    }
});