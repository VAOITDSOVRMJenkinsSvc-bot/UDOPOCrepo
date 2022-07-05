/**
* @class VIP.view.personinfo.Details
*
* Details on the queried person.
*/
Ext.define('VIP.view.personVadir.Details', {
    extend: 'Ext.tab.Panel',
    alias: 'widget.personvadir.details',
    tabPosition: 'top',
    plain: true,
    requires: [
        'VIP.view.personVadir.details.Addresses',
        'VIP.view.personVadir.details.Alias',
        'VIP.view.personVadir.details.Phones',
        'VIP.view.personVadir.details.Emails'
    ],

    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'personvadir.details.alias' },
            { xtype: 'personvadir.details.addresses' },
            { xtype: 'personvadir.details.phones' },
            { xtype: 'personvadir.details.emails' }
        ];

        me.callParent();
    }
});