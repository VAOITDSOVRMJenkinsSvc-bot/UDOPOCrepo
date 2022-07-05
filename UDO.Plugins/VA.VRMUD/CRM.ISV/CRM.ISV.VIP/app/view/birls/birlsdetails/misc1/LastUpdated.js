/**
* @author Ivan Yurisevic
* @class VIP.view.birls.birlsdetails.misc1.LastUpdated
*
* The view for the BIRLS fieldset at the top section
*/

Ext.define('VIP.view.birls.birlsdetails.misc1.LastUpdated', {
    extend: 'Ext.form.Panel',
    alias: 'widget.birls.birlsdetails.misc1.lastupdated',
    store: 'Birls',
    title: 'Last Updated',
    layout: {
        type: 'table',
        columns: 3,
        tableAttrs: {
            style: {
                width: '100%'
            }
        }
    },
    defaults: {
        labelStyle: 'font-weight:bold;font-size:11px;',
        fieldStyle: 'font-size:11px;',
        labelWidth: 160
        ,width: 200
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;

        me.items = [
            {//ROW 1
                xtype: 'displayfield',
                fieldLabel: 'Employee Num',
                name: 'employeeNumber'
            },
            {
                xtype: 'displayfield',
                fieldLabel: 'Employee Station Num',
                name: 'employeeStationNumber'
            },
            {
                xtype: 'displayfield',
                fieldLabel: 'Date Last Updated',
                name: 'dateOfUpdate'
            }
        ];

        me.callParent();
    }
});