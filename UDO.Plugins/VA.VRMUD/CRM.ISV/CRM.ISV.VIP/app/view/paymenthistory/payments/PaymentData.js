/**
* @author Ivan Yurisevic
* @class VIP.view.paymenthistory.payments.PaymentData
*
* The view for the returned payments grid for old payment history
*/
Ext.define('VIP.view.paymenthistory.payments.PaymentData', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.paymenthistory.payments.paymentdata',
    title: 'Payment Data',

    requires: [
        'VIP.view.paymenthistory.payments.paymentdata.Payments',
        'VIP.view.paymenthistory.payments.paymentdata.PaymentAddress'
    ],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [
            {xtype: 'paymenthistory.payments.paymentdata.payments', flex: 1},
            {xtype: 'paymenthistory.payments.paymentdata.paymentaddress', flex: 1}
        ];

        me.callParent();
    }
});
