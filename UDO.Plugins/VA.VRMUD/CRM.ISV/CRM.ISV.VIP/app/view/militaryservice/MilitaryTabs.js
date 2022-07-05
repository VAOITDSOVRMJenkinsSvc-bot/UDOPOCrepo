/**
* @author Ivan Yurisevic
* @class VIP.view.militaryservice.MilitaryTabs
*
* The central tab panel to display all tabs for military info
*/
Ext.define('VIP.view.militaryservice.MilitaryTabs', {
    extend: 'Ext.tab.Panel',
    alias: 'widget.militaryservice.militarytabs',

    requires: [
        'VIP.view.militaryservice.militarytabs.TourHistory',
        'VIP.view.militaryservice.militarytabs.Decorations',
        'VIP.view.militaryservice.militarytabs.TheatersAndPow',
        'VIP.view.militaryservice.militarytabs.RetirementPay',
        'VIP.view.militaryservice.militarytabs.SeverancePay',
        'VIP.view.militaryservice.militarytabs.ReadjustmentPay',
        'VIP.view.militaryservice.militarytabs.SeparationPay',
        'VIP.view.militaryservice.militarytabs.MilitaryPersons'
    ],
    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'militaryservice.militarytabs.tourhistory' },
            { xtype: 'militaryservice.militarytabs.decorations' },
            { xtype: 'militaryservice.militarytabs.theatersandpow' },
            { xtype: 'militaryservice.militarytabs.retirementpay' },
            { xtype: 'militaryservice.militarytabs.severancepay' },
            { xtype: 'militaryservice.militarytabs.readjustmentpay' },
            { xtype: 'militaryservice.militarytabs.separationpay' },
            { xtype: 'militaryservice.militarytabs.militarypersons' }
        ];

        me.callParent();
    }
});
