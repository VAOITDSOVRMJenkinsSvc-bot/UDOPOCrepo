/**
* @author Ivan Yurisevic
* @class VIP.view.birls.birlsdetails.insurancepolicy.PolicyInfo
*
* The view for the BIRLS fieldset at the top section
*/
Ext.define('VIP.view.birls.birlsdetails.insurancepolicy.PolicyInfo', {
    extend: 'Ext.form.Panel',
    alias: 'widget.birls.birlsdetails.insurancepolicy.policyinfo',
    store: 'Birls',
    title: 'Policy Info',
    flex: 2,
    layout: {
        type: 'table',
        columns: 2,
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

        me.items = [{
            xtype: 'displayfield',
            fieldLabel: 'Insurance Prefix',
            name: 'insurancePrefix'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Insurance Number',
            name: 'insuranceFileNumber'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Insurance Jurisdiction',
            name: 'insuranceJuris'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Insurance Lapse /Purge Date',
            name: 'dateOfInsLapsedPurge'
        }];

        me.callParent();
    }
});