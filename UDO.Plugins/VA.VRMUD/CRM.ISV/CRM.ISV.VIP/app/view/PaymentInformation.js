/**
* @author Josh Oliver
* @class VIP.view.PaymentInformation
*
* The main panel for paymentinformation payments tab
*/
Ext.define('VIP.view.PaymentInformation', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.paymentinformation',
    title: 'Payment Information',
    bodyStyle: {
        background: '#DEEBF7',
        padding: '2px'
    },
    requires: ['VIP.view.paymentinformation.Payments', 'VIP.view.paymentinformation.Details'],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'paymentinformation.payments', flex: 1 }, 
            { xtype: 'paymentinformation.details', flex: 1}
        ];

        me.callParent();
    }
});