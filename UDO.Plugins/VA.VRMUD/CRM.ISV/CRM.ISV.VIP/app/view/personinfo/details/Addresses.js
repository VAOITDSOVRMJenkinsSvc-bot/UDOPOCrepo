/**
* @class VIP.view.personinfo.details.AddressPanel
* Details on the queried person.
*/
Ext.define('VIP.view.personinfo.details.Addresses', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.personinfo.details.addresses',
    title: 'Addresses',

    requires: [
        'VIP.view.personinfo.details.addresses.AddressList',
        'VIP.view.personinfo.details.addresses.TreasuryAddress'
    ],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'personinfo.details.addresses.addresslist', flex: 2 },
            { xtype: 'personinfo.details.addresses.treasuryaddress', flex: 1 }
        ];

        me.callParent();
    }
});