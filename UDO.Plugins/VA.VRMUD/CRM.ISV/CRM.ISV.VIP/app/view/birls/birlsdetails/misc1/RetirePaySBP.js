/**
* @author Ivan Yurisevic
* @class VIP.view.birls.birlsdetails.misc1.RetirePaySBP
*
* The view for the BIRLS fieldset at the top section
*/

Ext.define('VIP.view.birls.birlsdetails.misc1.RetirePaySBP', {
    extend: 'Ext.form.Panel',
    alias: 'widget.birls.birlsdetails.misc1.retirepaysbp',
    store: 'Birls',
    title: 'Retire Pay SBP',
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
        labelWidth: 120
        //,width: 200
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;

        me.items = [
            {
                xtype: 'displayfield',
                fieldLabel: 'Current Pay Amt',
                name: 'currentRetirePaySBP'
            },
            {
                xtype: 'displayfield',
                fieldLabel: 'Current Pay Date',
                name: 'currentRetirePayDateSBP'
            },
            {
                xtype: 'displayfield',
                fieldLabel: 'Prior Pay Amt',
                name: 'priorRetirePaySBP'
            },
            {
                xtype: 'displayfield',
                fieldLabel: 'Prior Pay Date',
                name: 'priorPayDateSBP'
            }
        ];

        me.callParent();
    }
});