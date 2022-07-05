/**
* @author Jonas Dawson
* @class VIP.view.awards.details.Deductions
*
* Deductions for the selected award line
*/
Ext.define('VIP.view.awards.details.Deductions', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.awards.details.deductions',
    store: 'awards.Deductions',
    title: 'Deductions',

    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
                { header: 'Type', dataIndex: 'code' },
                { header: 'Description', dataIndex: 'name' },
                { header: 'Deduction Amount', dataIndex: 'amount', xtype: 'numbercolumn', format: '$0,000.00' },
                { header: 'Balance Amount', dataIndex: 'balance', xtype: 'numbercolumn', format: '$0,000.00' }
            ]
        };

        me.callParent();
    }
});
