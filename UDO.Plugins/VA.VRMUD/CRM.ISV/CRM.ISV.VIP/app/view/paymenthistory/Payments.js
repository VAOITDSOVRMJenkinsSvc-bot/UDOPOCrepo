/**
* @author Ivan Yurisevic
* @class VIP.view.paymenthistory.Payments
*
* The central tab panel to display all tabs for 
*/
Ext.define('VIP.view.paymenthistory.Payments', {
    extend: 'Ext.tab.Panel',
    alias: 'widget.paymenthistory.payments',

    requires: [
        'VIP.view.paymenthistory.payments.PaymentData',
        'VIP.view.paymenthistory.payments.ReturnedPayments'
    ],

    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'paymenthistory.payments.paymentdata' },
            { xtype: 'paymenthistory.payments.returnedpayments' }
        ];

        me.callParent();
    }
});
