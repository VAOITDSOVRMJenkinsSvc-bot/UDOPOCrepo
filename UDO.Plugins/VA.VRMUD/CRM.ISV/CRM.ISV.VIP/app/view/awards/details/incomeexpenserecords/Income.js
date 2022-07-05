/**
* @author Jonas Dawson
* @class VIP.view.awards.details.incomeexpenserecords.Income
*
* Income records for the selected award
*/
Ext.define('VIP.view.awards.details.incomeexpenserecords.Income', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.awards.details.incomeexpenserecords.income',
    store: 'awards.Income',
    title: 'Income (Filtered on selected Income Summary record)',
    flex: 2,

    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
                { header: 'Annual Amount', dataIndex: 'annualAmount', xtype: 'numbercolumn', format: '$0,000.00' },
                { header: 'Exclusion Amount', dataIndex: 'exclusionAmount', xtype: 'numbercolumn', format: '$0,000.00' },
                { header: 'Exclusion Type', dataIndex: 'exclusionTypeName' },
                { header: 'Income Type', dataIndex: 'incomeTypeName' },
                { header: 'First Name', dataIndex: 'firstName' },
                { header: 'Last Name', dataIndex: 'lastName' },
                { header: 'Middle Name', dataIndex: 'middleName' }
            ]
        };

        me.callParent();
    }
});
