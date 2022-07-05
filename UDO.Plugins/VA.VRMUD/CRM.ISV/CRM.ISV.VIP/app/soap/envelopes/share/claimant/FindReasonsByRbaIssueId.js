Ext.define('VIP.soap.envelopes.share.claimant.FindReasonsByRbaIssueId', {
    extend: 'VIP.soap.envelopes.share.ClaimantTemplate',
    alias: 'envelopes.FindReasonsByRbaIssueId',
    config: {
        rbaIssueId: ''
    },

    /*
    * @customReadRecords
    * Need to remove namespaces so that the model will recognize the XML and read it. Uses RegEx to remove namespaces.
    */
    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer', {
        customReadRecords: function (response, currentResultSet, reader) {
            var me = this,
                xml = response.xml,
                mod = xml.replace(/<[A-Z]*[a-z]*[0-9]*:/gm, "<"),
                mod = mod.replace(/<\/[A-Z]*[a-z]*[0-9]*:/gm, "</"),
                newResultSet;

            response.loadXML(mod);
            newResultSet = reader.read(response);

            return newResultSet;
        }
    }),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();
        me.setBody('findReasonsByRbaIssueId', {
            namespace: 'ser',
            rbaIssueId: {
                namespace: '',
                value: me.getRbaIssueId()
            }
        });
    }

});
