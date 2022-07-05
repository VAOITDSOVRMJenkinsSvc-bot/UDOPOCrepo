/**
* @author Ivan Yurisevic
* @class VIP.view.birls.birlsdetails.folderlocinfo.AdditionalInfo
*
* The view for the BIRLS fieldset at the top section
*/

Ext.define('VIP.view.birls.birlsdetails.folderlocinfo.AdditionalInfo', {
    extend: 'Ext.form.Panel',
    alias: 'widget.birls.birlsdetails.folderlocinfo.additionalinfo',
    store: 'Birls',
    title: 'Additional Info',
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
        ,width: 200
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;

        me.items = [
            {
                xtype: 'displayfield',
                fieldLabel: 'CH 31',
                name: 'ch31Ind'
            },
            {
                xtype: 'displayfield',
                fieldLabel: 'CH 32/903',
                name: 'ch32_903_Ind'
            },
            {
                xtype: 'displayfield',
                fieldLabel: 'CH34',
                name: 'ch34Ind'
            },
            {
                xtype: 'displayfield',
                fieldLabel: '901',
                name: 'ind901'
            },
            {
                xtype: 'displayfield',
                fieldLabel: 'Comp & Pen',
                name: 'compensationPensionVeteranInd'
            },
            {
                xtype: 'displayfield',
                fieldLabel: 'Svc Med Record',
                name: 'serviceMedicalRecordInd'
            },
            {
                xtype: 'displayfield',
                fieldLabel: 'CH 32 Bene',
                name: 'ch32BeneficiaryInd'
            },
            {
                xtype: 'displayfield',
                fieldLabel: 'CH 32 Bank',
                name: 'ch32BankInd'
            }
        ];

        me.callParent();
    }
});
