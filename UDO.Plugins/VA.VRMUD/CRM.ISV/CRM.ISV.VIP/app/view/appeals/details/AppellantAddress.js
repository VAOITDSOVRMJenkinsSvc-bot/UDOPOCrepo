/**
* @author Ivan Yurisevic
* @class VIP.view.appeals.details.AppellantAddress
*
* The view for the current POA fieldset
*/
Ext.define('VIP.view.appeals.details.AppellantAddress', {
    extend: 'Ext.form.Panel',
    alias: 'widget.appeals.details.appellantaddress',
    title: 'Appellant Address',
    layout: {
        type: 'table',
        columns: 4,
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
        labelWidth: 120,
        width: 250
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;

        me.items = [
            {
                xtype: 'displayfield',
                fieldLabel: 'Address 1',
                name: 'appellantAddressLine1',
                width: 400
            },
            {
                xtype: 'displayfield',
                fieldLabel: 'Address 2',
                name: 'appellantAddressLine2',
                colspan: 3,
                width: 400
            }, 
            {
                
                // row 2
                xtype: 'displayfield',
                fieldLabel: 'City',
                name: 'appellantAddressCityName'
            }, {
                xtype: 'displayfield',
                fieldLabel: 'State',
                name: 'appellantAddressStateCode'
            }, {
                xtype: 'displayfield',
                fieldLabel: 'Zip Code',
                name: 'appellantAddressZipCode'
            }, {
                xtype: 'displayfield',
                fieldLabel: 'Country',
                name: 'appellantAddressCountryName'
            }, {
                
                // row 3
                xtype: 'displayfield',
                fieldLabel: 'Address Mod By',
                name: 'appellantAddressLastModifiedByROName'
            }, {
                xtype: 'displayfield',
                fieldLabel: 'Address Mod Date',
                name: 'appellantAddressLastModifiedDate_f'
            }, {
                xtype: 'displayfield',
                fieldLabel: 'Address Notes',
                name: 'appellantAddressNotes'
            }, {
                xtype: 'displayfield',
                fieldLabel: 'Work Phone',
                name: 'appellantWorkPhone'
            }, {
                
                // row 4
                xtype: 'displayfield',
                fieldLabel: 'Home Phone',
                name: 'appellantHomePhone'
            }
        ];

        me.callParent();
    }
});