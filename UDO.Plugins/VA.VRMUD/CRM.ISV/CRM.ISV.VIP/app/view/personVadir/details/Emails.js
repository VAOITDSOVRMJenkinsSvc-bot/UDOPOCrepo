/**
* View for Emails associated with the person
*/
Ext.define('VIP.view.personVadir.details.Emails', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.personvadir.details.emails',
    store: 'personVadir.Email',
    title: 'Emails',

    initComponent: function () {
        var me = this;

        me.store = 'personVadir.Email';

        me.columns = [
            { header: '', xtype: 'rownumberer' },
			{ header: 'Email Address', dataIndex: 'emailaddress', width: 400 },
			{ header: 'Email Source', dataIndex: 'emailsource', width: 150 },
			{ header: 'Email Address Priority', dataIndex: 'emailaddresspriority', width: 150 }
        ];

        me.callParent();
    }
}); 