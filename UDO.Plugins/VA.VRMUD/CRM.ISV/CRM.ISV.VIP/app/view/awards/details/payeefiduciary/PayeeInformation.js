/**
* @author Jonas Dawson
* @class VIP.view.awards.details.payeefiduciary.PayeeInformation
*
* The field set for the payee information in the awards tab details section.
*/
Ext.define('VIP.view.awards.details.payeefiduciary.PayeeInformation', {
    extend: 'Ext.form.Panel',
    alias: 'widget.awards.details.payeefiduciary.payeeinformation',
    title: 'Payee Information (Select Award to Populate)',
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
        , width: 250
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;

        // TODO: format Date columns
        me.items = [{
            xtype: 'displayfield',
            fieldLabel: 'Payee Name',
            width: 350,
            name: 'payeeName',
            colspan: 2
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Payee SSN',
            name: 'payeeSSN'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Payee Sex',
            name: 'payeeSex'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Payee Type',
            name: 'payeeType'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Payee DOB',
            name: 'payeeBirthDate_F'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Fiduciary',
            name: 'fiduciaryDecisionTypeName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Competency',
            name: 'competency'
        }];

        me.callParent();
    }
});