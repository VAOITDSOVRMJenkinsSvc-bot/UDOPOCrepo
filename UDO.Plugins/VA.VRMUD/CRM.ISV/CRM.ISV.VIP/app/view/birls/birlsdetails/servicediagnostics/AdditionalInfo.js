/**
* @author Ivan Yurisevic
* @class VIP.view.birls.birlsdetails.misc1.LastUpdated
*
* The view for the BIRLS fieldset at the top section
*/
Ext.define('VIP.view.birls.birlsdetails.servicediagnostics.AdditionalInfo', {
    extend: 'Ext.form.Panel',
    alias: 'widget.birls.birlsdetails.servicediagnostics.additionalinfo',
    store: 'Birls',
    title: 'Additional Info',
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
        labelWidth: 200
        //,width: 200
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'displayfield',
            fieldLabel: 'Num of Svc Connected Diags',
            name: 'numberOfServiceConnectedDiagnostics'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Additional Diagnostics',
            name: 'additionalDiagnosticsInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Diagnostics Verified',
            name: 'diagsVerifiedInd'
        }];

        me.callParent();
    }
});