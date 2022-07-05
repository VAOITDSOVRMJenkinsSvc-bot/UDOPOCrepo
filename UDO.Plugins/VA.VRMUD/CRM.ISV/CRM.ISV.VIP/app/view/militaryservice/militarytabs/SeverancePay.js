/**
* @author Ivan Yurisevic
* @class VIP.view.militaryservice.militarytabs.SeverancePay
*
* The central tab panel to display all tabs for military info
*/
Ext.define('VIP.view.militaryservice.militarytabs.SeverancePay', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.militaryservice.militarytabs.severancepay',
    title: 'Severance Pay',

    requires: [
        'VIP.view.militaryservice.militarytabs.severancepay.SeveranceBalances',
        'VIP.view.militaryservice.militarytabs.severancepay.SeverancePayments'
    ],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'militaryservice.militarytabs.severancepay.severancepayments', flex: 1 },
            { xtype: 'militaryservice.militarytabs.severancepay.severancebalances', flex: 1 }
        ];

        me.callParent();
    }
});
