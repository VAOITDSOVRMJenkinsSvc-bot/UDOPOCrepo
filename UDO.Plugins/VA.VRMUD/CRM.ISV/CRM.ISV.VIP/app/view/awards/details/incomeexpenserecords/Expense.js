/**
* @author Jonas Dawson
* @class VIP.view.awards.details.incomeexpenserecords.Expense
*
* Expense Records for the selected award
*/
Ext.define('VIP.view.awards.details.incomeexpenserecords.Expense', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.awards.details.incomeexpenserecords.expense',
    store: 'awards.Expense',
    title: 'Expense (Filtered on selected Income Summary record)',
    flex: 1,

    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
                { header: 'Annual Amount', dataIndex: 'annualAmount', xtype: 'numbercolumn', format: '$0,000.00' },
                { header: 'Type', dataIndex: 'typeName' }
            ]
        };

        me.callParent();
    }
});
