/**
* @author Ivan Yurisevic
* @class VIP.view.militaryservice.militarytabs.SeparationPay
*
* The central tab panel to display all tabs for military info
*/
Ext.define('VIP.view.militaryservice.militarytabs.SeparationPay', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.militaryservice.militarytabs.separationpay',
    title: 'Separation Pay',

    requires: [
        'VIP.view.militaryservice.militarytabs.separationpay.SeparationBalances',
        'VIP.view.militaryservice.militarytabs.separationpay.SeparationPayments'
    ],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'militaryservice.militarytabs.separationpay.separationpayments', flex: 1 },
            { xtype: 'militaryservice.militarytabs.separationpay.separationbalances', flex: 1 }
        ];

        me.callParent();
    }
});
