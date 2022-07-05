/**
* @author Ivan Yurisevic
* @class VIP.view.personinfo.details.addresses.TreasuryAddress
*
* The view for Addresses associated with the person
*/
Ext.define('VIP.view.personinfo.details.addresses.TreasuryAddress', {
    extend: 'Ext.form.Panel',
    alias: 'widget.personinfo.details.addresses.treasuryaddress',
    layout: {
        type: 'table',
        columns: 1,
        tableAttrs: {
            style: {
                width: '100%'
            }
        },
        tdAttrs: {
            width: '25%'
        }
    },
    defaults: {
        labelStyle: 'font-weight:bold;font-size:11px;',
        fieldStyle: 'font-size:11px;',
        labelWidth: 130,
        width: 600
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'displayfield',
            fieldLabel: 'Treasury Address',
            name: 'fullTreasuryAddress'
        }];

        me.callParent();
    }
});
