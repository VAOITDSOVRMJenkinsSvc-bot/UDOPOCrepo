/**
* @author Ivan Yurisevic
* @class VIP.view.denials.DenialDetail
*
* The view for the current DenialDetail fieldset
*/
Ext.define('VIP.view.denials.DenialDetail', {
    extend: 'Ext.form.Panel',
    alias: 'widget.denials.denialdetail',
    store: 'Denial',
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
        labelWidth: 120,
        width: 200
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'displayfield',
            fieldLabel: 'Admin Date',
            name: 'adminDate'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Claim Payee Code',
            name: 'claimPayeeCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Claim Type Code',
            name: 'claimTypeCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Decision Type',
            name: 'decisionType'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Program Type Code',
            name: 'programTypeCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'RBA Id',
            name: 'rbaId'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Reason Preview',
            name: 'decisionName',
            colspan: 2,
            width: 400
        },
        {
            xtype: 'container',
            colspan: 4,
            width: 1000,
            items: [{
                xtype: 'button',
                text: 'Load Full Reason',
                action: 'loadfullreason'
            }, {
                xtype: 'displayfield',
                fieldLabel: '<b>Full Reason</b>',
                fieldStyle: 'font-size:11px;',
                id: 'fullreason',
                padding: '0 0 0 5',
                width: 900
            }]
        }

        ];

        me.callParent();
    }
});