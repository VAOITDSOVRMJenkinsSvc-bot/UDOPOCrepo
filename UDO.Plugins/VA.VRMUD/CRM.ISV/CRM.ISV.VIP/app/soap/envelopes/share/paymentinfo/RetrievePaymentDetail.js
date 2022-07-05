Ext.define('VIP.soap.envelopes.share.paymentinfo.RetrievePaymentDetail', {
    extend: 'VIP.soap.envelopes.share.PaymentInfoTemplate',
    alias: 'envelopes.RetrievePaymentDetail',
    config: {
        PaymentId: '',
        FbtId: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),
    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('retrievePaymentDetail', {
            namespace: 'ws',
            PaymentId: {
                namespace: '',
                value: me.getPaymentId()
            },
            FbtId: {
                namespace: '',
                value: me.getFbtId()
            }
        });
    }

});
