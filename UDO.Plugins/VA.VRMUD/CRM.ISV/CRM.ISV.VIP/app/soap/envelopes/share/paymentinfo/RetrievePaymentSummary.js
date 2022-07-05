Ext.define('VIP.soap.envelopes.share.paymentinfo.RetrievePaymentSummary', {
    extend: 'VIP.soap.envelopes.share.PaymentInfoTemplate',
    config: {
        ParticipantId: ''
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),
    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('retrievePaymentSummary', {
            namespace: 'ws',
            ParticipantId: {
                namespace: '',
                value: me.getParticipantId()
            }
        });
    }

});
