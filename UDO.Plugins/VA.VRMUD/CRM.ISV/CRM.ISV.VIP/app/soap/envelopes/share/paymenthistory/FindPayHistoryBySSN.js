Ext.define('VIP.soap.envelopes.share.paymenthistory.FindPayHistoryBySSN', {
    extend: 'VIP.soap.envelopes.share.PaymentHistoryTemplate',
    alias: 'envelopes.FindPayHistoryBySSN',
    config: {
        ssn: '',
        useExternalUid: true
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),
    constructor: function (config) {
        var me = this;
        
        me.initConfig(config);

        me.callParent();

        me.setBody('findPayHistoryBySSN', {
            namespace: 'pay',
            ssn: {
                namespace: '',
                value: me.getSsn()
            }
        });

    }
});
