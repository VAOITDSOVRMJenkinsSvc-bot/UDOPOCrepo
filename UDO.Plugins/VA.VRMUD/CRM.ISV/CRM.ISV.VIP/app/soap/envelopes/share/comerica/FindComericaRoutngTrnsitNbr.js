Ext.define('VIP.soap.envelopes.share.Comerica.FindComericaRoutngTrnsitNbr', {
    extend: 'VIP.soap.envelopes.share.ComericaTemplate',
    requires: [
        'VIP.model.Comerica'
    ],
    config: {
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer', {
        customReadRecords: function (response, currentResultSet, reader) {
            return reader.read(response);
        },
        scrubFilter: function (record) {
            var recordHasData = false;

            for (var i in record.data) {
                if (!Ext.isEmpty(record.data[i]) && record.data[i] != 0 && i != 'numberOfCPClaimRecords') {
                    recordHasData = true;
                    break;
                }
            }

            return recordHasData;
        }
    }),

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('findComericaRoutngTrnsitNbr', {
            namespace: 'com'
        });
    }

});
