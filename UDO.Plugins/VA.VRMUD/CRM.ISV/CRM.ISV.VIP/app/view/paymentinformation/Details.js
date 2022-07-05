/**
* @author Jonas Dawson
* @class VIP.view.paymentinformation.Details
*
* Tab panel for payment details
*/
Ext.define('VIP.view.paymentinformation.Details', {
    extend: 'Ext.tab.Panel',
    alias: 'widget.paymentinformation.details',
    tabPosition: 'top',
    requires: [
        'VIP.view.paymentinformation.details.PaymentDetails',
        'VIP.view.paymentinformation.details.PaymentAdjustments',
        'VIP.view.paymentinformation.details.AwardAdjustments',
        'VIP.view.paymentinformation.details.AwardReasons'
    ],

    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'paymentinformation.details.paymentdetails' },
            { xtype: 'paymentinformation.details.paymentadjustments' },
            { xtype: 'paymentinformation.details.awardadjustments' },
            { xtype: 'paymentinformation.details.awardreasons' }
        ];

        me.callParent();
    }
});