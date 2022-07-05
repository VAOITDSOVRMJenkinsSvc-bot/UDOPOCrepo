/**
* @author Jonas Dawson
* @class VIP.view.awards.details.IncomeExpenseRecords
*
* Income Expense Records for the selected award line
*/
Ext.define('VIP.view.awards.details.IncomeExpenseRecords', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.awards.details.incomeexpenserecords',
    title: 'Income/Expense Records',
    layout: {
        type: 'hbox',
        align: 'stretch'
    },
    requires: [
        'VIP.view.awards.details.incomeexpenserecords.Income',
        'VIP.view.awards.details.incomeexpenserecords.Expense'
    ],
    initComponent: function () {
        var me = this;

        me.items = [
            {
                xtype: 'awards.details.incomeexpenserecords.income'
            },
            {
                xtype: 'awards.details.incomeexpenserecords.expense'
            }
        ];

        me.callParent();
    }
});
