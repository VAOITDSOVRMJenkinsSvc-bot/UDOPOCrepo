/**
* @author Ivan Yurisevic
* @class VIP.view.militaryservice.militarytabs.ReadjustmentPay
*
* The central tab panel to display all tabs for military info
*/
Ext.define('VIP.view.militaryservice.militarytabs.ReadjustmentPay', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.militaryservice.militarytabs.readjustmentpay',
    title: 'Readjustment Pay',

    requires: [
        'VIP.view.militaryservice.militarytabs.readjustmentpay.ReadjustmentBalances',
        'VIP.view.militaryservice.militarytabs.readjustmentpay.ReadjustmentPayments'
    ],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'militaryservice.militarytabs.readjustmentpay.readjustmentpayments', flex: 1 },
            { xtype: 'militaryservice.militarytabs.readjustmentpay.readjustmentbalances', flex: 1 }
        ];

        me.callParent();
    }
});
