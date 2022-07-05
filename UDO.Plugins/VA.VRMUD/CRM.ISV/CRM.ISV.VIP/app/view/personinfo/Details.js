/**
* @class VIP.view.personinfo.Details
* Details on the queried person.
*/
Ext.define('VIP.view.personinfo.Details', {
    extend: 'Ext.tab.Panel',
    alias: 'widget.personinfo.details',
    tabPosition: 'top',
    plain: true,
    requires: [
        'VIP.view.personinfo.details.GeneralDetails',
        'VIP.view.personinfo.details.Flashes',
        'VIP.view.personinfo.details.Dependents',
        'VIP.view.personinfo.details.AllRelationships',
        'VIP.view.personinfo.details.Addresses'
    ],

    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'personinfo.details.generaldetails' },
            { xtype: 'personinfo.details.addresses' },
            { xtype: 'personinfo.details.dependents' },
            { xtype: 'personinfo.details.allrelationships' },
            { xtype: 'personinfo.details.flashes' }
        ];

        me.callParent();
    }
});