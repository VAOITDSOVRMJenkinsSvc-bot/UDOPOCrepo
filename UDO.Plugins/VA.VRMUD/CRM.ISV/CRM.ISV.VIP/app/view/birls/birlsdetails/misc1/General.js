/**
* @author Ivan Yurisevic
* @class VIP.view.birls.birlsdetails.misc1.General
*
* The view for the BIRLS fieldset at the top section
*/

Ext.define('VIP.view.birls.birlsdetails.misc1.General', {
    extend: 'Ext.form.Panel',
    alias: 'widget.birls.birlsdetails.misc1.general',
    store: 'Birls',
    title: 'General',
    layout: {
        type: 'table',
        columns: 4,
        tableAttrs: {
            style: {
                width: '100%'
            }
        }
    },
    defaults: {
        labelStyle: 'font-weight:bold;font-size:11px;',
        fieldStyle: 'font-size:11px;',
        labelWidth: 180
        ,width: 200
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;

        me.items = [
            {
                xtype: 'displayfield',
                fieldLabel: 'Disability Severance Pay',
                name: 'disabilitySeverancePay'
            },
            {
                xtype: 'displayfield',
                fieldLabel: 'Lump Sum Readjustment Pay',
                name: 'lumpSumReadjustmentPay'
            },
            {
                xtype: 'displayfield',
                fieldLabel: 'Separation Pay',
                name: 'separationPay'
            },
            {
                xtype: 'displayfield',
                fieldLabel: 'POW Days',
                name: 'powNumberOfDays'
            }
        ];

        me.callParent();
    }
});
