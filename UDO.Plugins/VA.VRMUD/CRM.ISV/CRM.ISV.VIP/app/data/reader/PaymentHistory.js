Ext.define('VIP.data.reader.PaymentHistory', {
    extend: 'Ext.data.reader.Xml',
    alias: 'reader.paymenthistory',
    requires: [
        'VIP.util.Xml'
    ],
    read: function (response) {
        var me = this;

        VIP.util.Xml.wrap("payment", "payments", "paymentRecord", response);
        VIP.util.Xml.wrap("paymentAddresses", "paymentAddress", "paymentRecord", response);
        VIP.util.Xml.wrap("returnPayment", "returnPayments", "paymentRecord", response);

        return me.callParent([response]);
    }
});