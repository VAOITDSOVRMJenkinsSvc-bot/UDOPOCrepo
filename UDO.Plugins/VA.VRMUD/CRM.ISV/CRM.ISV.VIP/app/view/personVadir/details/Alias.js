/**
* View for Alias associated with the person
*/
Ext.define('VIP.view.personVadir.details.Alias', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.personvadir.details.alias',
    store: 'personVadir.Alias',
    title: 'Alias',
    

    initComponent: function () {
        var me = this;

        me.store = 'personVadir.Alias';

        me.columns = [
            { header: '', xtype: 'rownumberer' },
			{ header: 'First Name', flex: 1, dataIndex: 'firstName' },
			{ header: 'Last Name', flex: 1, dataIndex: 'lastName' },
			{ header: 'Effective Date', flex: 1, dataIndex: 'effectiveDate', xtype: 'datecolumn', format: 'm/d/Y' }
        ];

        me.callParent();
    }
}); 