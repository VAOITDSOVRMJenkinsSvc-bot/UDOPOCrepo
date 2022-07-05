Ext.define('VIP.soap.envelopes.share.paymentinfo.FindComericaRoutngTrnsitNbr', {
    extend: 'VIP.soap.envelopes.share.PaymentInfoTemplate',
    config: {
        isDacRequest: true,
        serviceUrl: 'DdeftWebServiceBean/DdeftWebService'
    },

    //analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),
    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer', {
        customReadRecords: function (response, currentResultSet, reader) {
            if (!currentResultSet.count && currentResultSet.success) {
            }
            else {
                return reader.read(response);
            }
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
