/**
* View for Phones associated with the person
*/
Ext.define('VIP.view.personVadir.details.Phones', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.personvadir.details.phones',
    store: 'personVadir.Phone',
    title: 'Phones',

    initComponent: function () {
        var me = this;

        me.store = 'personVadir.Phone';

        me.columns = [
            { xtype: 'rownumberer' },
			{ header: 'Phone Type', dataIndex: 'phoneType', width: 80 },
			{ header: 'Phone Number', dataIndex: 'phoneNumberFormatted', width: 200 }
        ];

        me.callParent();
    }
}); 