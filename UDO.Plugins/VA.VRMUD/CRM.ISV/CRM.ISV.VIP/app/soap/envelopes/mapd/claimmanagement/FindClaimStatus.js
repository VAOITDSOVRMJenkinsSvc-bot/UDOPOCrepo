Ext.define('VIP.soap.envelopes.mapd.claimmanagement.FindClaimStatus', {
    extend: 'VIP.soap.envelopes.mapd.ClaimManagementTemplate',
    alias: 'envelopes.FindClaimStatus',
    config: {
        claimId: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('findClaimStatus', {
            namespace: 'ser',
            claimId: {
                namespace: '',
                value: me.getClaimId()
            }
        });

        me.addNamespace('ser', 'http://services.mapd.benefits.vba.va.gov/');
    }
});