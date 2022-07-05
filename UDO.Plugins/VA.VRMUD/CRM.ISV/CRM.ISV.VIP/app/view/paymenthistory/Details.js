/**
* @author Dmitri Riz
* @class VIP.view.paymenthistory.Details
*
* Tab panel for payment details for old payment history
*/
Ext.define('VIP.view.paymenthistory.Details', {
    extend: 'Ext.form.Panel',
    alias: 'widget.paymenthistory.details',
    store: 'PaymentHistory',
    title: 'Payment Details',
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
        labelWidth: 120,
        width: 300
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'displayfield',
            fieldLabel: 'File Number',
            name: 'completeFileNumber'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Payee',
            name: 'payeeCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Name',
            name: 'fullName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Last Activity Date',
            name: 'lastActivityDate'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Last Fiche Date',
            name: 'lastFicheDate'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Prior Fiche Date',
            name: 'priorFicheDate'
        }];

        me.callParent();
    }
});