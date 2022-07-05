Ext.define('VIP.soap.envelopes.share.paymentinfo.RetrievePaymentSummaryWithBDN', {
    extend: 'VIP.soap.envelopes.share.PaymentInfoTemplate',
    config: {
        ParticipantId: '',
        FileNumber: '',
        PayeeCode: '00'
    },

    analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),
    
    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        me.callParent();

        me.setBody('retrievePaymentSummaryWithBDN', {
            namespace: 'ws',
            ParticipantId: {
                namespace: '',
                value: me.getParticipantId()
            },
            FileNumber: {
                namespace: '',
                value: me.getFileNumber()
            },
            PayeeCode: {
                namespace: '',
                value: me.getPayeeCode()
            }
        });
    }

});
