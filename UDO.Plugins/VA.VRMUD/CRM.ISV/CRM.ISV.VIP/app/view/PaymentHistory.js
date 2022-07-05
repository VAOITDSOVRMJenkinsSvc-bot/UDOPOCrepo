/**
* @author Ivan Yurisevic
* @class VIP.view.PaymentHistory
*
* The model for old payment history service
*/
Ext.define('VIP.view.PaymentHistory', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.paymenthistory',
    title: 'Legacy Payment History',
    bodyStyle: {
        background: '#DEEBF7',
        padding: '2px'
    },
    requires: ['VIP.view.paymenthistory.Details', 'VIP.view.paymenthistory.Payments'],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'toolbar',
            items: [{
                xtype: 'paymentdisposition',
                id: 'id_PaymentHistory_01'
            }, {
                xtype: 'tbseparator',
                id: 'id_PaymentHistory_02'
            }, {
                xtype: 'button',
                text: 'Education Cutoff Dates',
                tooltip: 'Opens a new window with the education cutoff dates website',
                iconCls: 'icon-request',
                action: 'showeducationcutoff',
                id: 'id_PaymentHistory_03'
            }, {
                xtype: 'tbseparator',
                id: 'id_PaymentHistory_04'
            }, {
                xtype: 'button',
                text: 'Payment Script',
                tooltip: 'Opens up the Payments script.',
                iconCls: 'icon-script',
                action: 'showpaymentscript',
                id: 'id_PaymentHistory_05'
            }, {
                xtype: 'tbseparator',
                id: 'id_PaymentHistory_06'
            }, {
                xtype: 'button',
                text: 'Script for C&P',
                width: 100,
                tooltip: 'Opens up the Script for C&P.',
                iconCls: 'icon-script',
                action: 'showcandpscript',
                id: 'id_PaymentHistory_07'
            }
            ]
        },
        { xtype: 'paymenthistory.details', flex: 1 },
        { xtype: 'paymenthistory.payments', flex: 4 }
        ];

        me.callParent();
    }
});
