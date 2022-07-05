Ext.define('VIP.soap.envelopes.mapd.contention.FindContentions', {
    extend: 'VIP.soap.envelopes.mapd.ContentionTemplate',
    alias: 'envelopes.FindContentions',
    config: {
        claimId: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer', {
        customReadRecords: function (response, currentResultSet, reader) {
            var newResultSet;
            
            VIP.util.Xml.wrap("specialIssue", "specialIssues", "contentions", response);

            newResultSet = reader.read(response);

            return newResultSet;
        }
    }),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('findContentions', {
            namespace: 'ser',
            claimId: {
                namespace: '',
                value: me.getClaimId()
            }
        });

        me.addNamespace('ser', 'http://services.mapd.benefits.vba.va.gov/');
    }
});