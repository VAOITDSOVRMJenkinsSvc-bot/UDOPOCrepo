Ext.define('VIP.soap.envelopes.mapd.developmentnotes.FindDevelopmentNotes', {
    extend: 'VIP.soap.envelopes.mapd.DevelopmentNotesTemplate',
    alias: 'envelopes.FindDevelopmentNotes',
    config: {
        ptcpntId: '',
        claimId: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('findDevelopmentNotes', {
            namespace: 'ser',
            ptcpntId: {
                namespace: '',
                value: me.getPtcpntId()
            },
            claimId: {
                namespace: '',
                value: me.getClaimId()
            }
        });
        me.addNamespace('ser', 'http://services.mapd.benefits.vba.va.gov/');
    }
});