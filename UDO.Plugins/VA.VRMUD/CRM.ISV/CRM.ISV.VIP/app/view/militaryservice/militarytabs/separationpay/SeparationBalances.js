/**
* @author Ivan Yurisevic
* @class VIP.view.militaryservice.militarytabs.separationpay.SeparationBalances
*
* The view for Flashes associated with the person
*/
Ext.define('VIP.view.militaryservice.militarytabs.separationpay.SeparationBalances', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.militaryservice.militarytabs.separationpay.separationbalances',
    store: 'militaryservice.SeparationBalance', 
    title: 'Separation Balances',

    initComponent: function () {
        var me = this;
        me.columns = {
            defaults: { flex: 1 },
            items: [
                { xtype: 'rownumberer' },
                { header: 'Current Balance', dataIndex: 'currentBalance' },
                { header: 'Original Balance', dataIndex: 'originalBalance' },
                { header: 'Date of Zero Balance', dataIndex: 'dateOfZeroBalance' }
            ]
        };

        me.callParent();
    }
});
